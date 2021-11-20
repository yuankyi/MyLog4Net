using System;

namespace log4net.Core
{
	/// <summary>
	/// An evaluator that triggers on an Exception type
	/// </summary>
	/// <remarks>
	/// <para>
	/// This evaluator will trigger if the type of the Exception
	/// passed to <see cref="M:IsTriggeringEvent(LoggingEvent)" />
	/// is equal to a Type in <see cref="P:log4net.Core.ExceptionEvaluator.ExceptionType" />.    /// 
	/// </para>
	/// </remarks>
	/// <author>Drew Schaeffer</author>
	public class ExceptionEvaluator : ITriggeringEventEvaluator
	{
		/// <summary>
		/// The type that causes the trigger to fire.
		/// </summary>
		private Type m_type;

		/// <summary>
		/// Causes subclasses of <see cref="P:log4net.Core.ExceptionEvaluator.ExceptionType" /> to cause the trigger to fire.
		/// </summary>
		private bool m_triggerOnSubclass;

		/// <summary>
		/// The type that triggers this evaluator.
		/// </summary>
		public Type ExceptionType
		{
			get
			{
				return m_type;
			}
			set
			{
				m_type = value;
			}
		}

		/// <summary>
		/// If true, this evaluator will trigger on subclasses of <see cref="P:log4net.Core.ExceptionEvaluator.ExceptionType" />.
		/// </summary>
		public bool TriggerOnSubclass
		{
			get
			{
				return m_triggerOnSubclass;
			}
			set
			{
				m_triggerOnSubclass = value;
			}
		}

		/// <summary>
		/// Default ctor to allow dynamic creation through a configurator.
		/// </summary>
		public ExceptionEvaluator()
		{
		}

		/// <summary>
		/// Constructs an evaluator and initializes to trigger on <paramref name="exType" />
		/// </summary>
		/// <param name="exType">the type that triggers this evaluator.</param>
		/// <param name="triggerOnSubClass">If true, this evaluator will trigger on subclasses of <see cref="P:log4net.Core.ExceptionEvaluator.ExceptionType" />.</param>
		public ExceptionEvaluator(Type exType, bool triggerOnSubClass)
		{
			if (exType == null)
			{
				throw new ArgumentNullException("exType");
			}
			m_type = exType;
			m_triggerOnSubclass = triggerOnSubClass;
		}

		/// <summary>
		/// Is this <paramref name="loggingEvent" /> the triggering event?
		/// </summary>
		/// <param name="loggingEvent">The event to check</param>
		/// <returns>This method returns <c>true</c>, if the logging event Exception 
		/// Type is <see cref="P:log4net.Core.ExceptionEvaluator.ExceptionType" />. 
		/// Otherwise it returns <c>false</c></returns>
		/// <remarks>
		/// <para>
		/// This evaluator will trigger if the Exception Type of the event
		/// passed to <see cref="M:IsTriggeringEvent(LoggingEvent)" />
		/// is <see cref="P:log4net.Core.ExceptionEvaluator.ExceptionType" />.
		/// </para>
		/// </remarks>
		public bool IsTriggeringEvent(LoggingEvent loggingEvent)
		{
			if (loggingEvent == null)
			{
				throw new ArgumentNullException("loggingEvent");
			}
			if (m_triggerOnSubclass && loggingEvent.ExceptionObject != null)
			{
				Type type = loggingEvent.ExceptionObject.GetType();
				if (!(type == m_type))
				{
					return m_type.IsAssignableFrom(type);
				}
				return true;
			}
			if (!m_triggerOnSubclass && loggingEvent.ExceptionObject != null)
			{
				return loggingEvent.ExceptionObject.GetType() == m_type;
			}
			return false;
		}
	}
}
