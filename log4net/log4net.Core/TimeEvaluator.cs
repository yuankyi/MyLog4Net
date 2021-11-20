using System;

namespace log4net.Core
{
	/// <summary>
	/// An evaluator that triggers after specified number of seconds.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This evaluator will trigger if the specified time period 
	/// <see cref="P:log4net.Core.TimeEvaluator.Interval" /> has passed since last check.
	/// </para>
	/// </remarks>
	/// <author>Robert Sevcik</author>
	public class TimeEvaluator : ITriggeringEventEvaluator
	{
		/// <summary>
		/// The time threshold for triggering in seconds. Zero means it won't trigger at all.
		/// </summary>
		private int m_interval;

		/// <summary>
		/// The UTC time of last check. This gets updated when the object is created and when the evaluator triggers.
		/// </summary>
		private DateTime m_lastTimeUtc;

		/// <summary>
		/// The default time threshold for triggering in seconds. Zero means it won't trigger at all.
		/// </summary>
		private const int DEFAULT_INTERVAL = 0;

		/// <summary>
		/// The time threshold in seconds to trigger after
		/// </summary>
		/// <value>
		/// The time threshold in seconds to trigger after.
		/// Zero means it won't trigger at all.
		/// </value>
		/// <remarks>
		/// <para>
		/// This evaluator will trigger if the specified time period 
		/// <see cref="P:log4net.Core.TimeEvaluator.Interval" /> has passed since last check.
		/// </para>
		/// </remarks>
		public int Interval
		{
			get
			{
				return m_interval;
			}
			set
			{
				m_interval = value;
			}
		}

		/// <summary>
		/// Create a new evaluator using the <see cref="F:log4net.Core.TimeEvaluator.DEFAULT_INTERVAL" /> time threshold in seconds.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Create a new evaluator using the <see cref="F:log4net.Core.TimeEvaluator.DEFAULT_INTERVAL" /> time threshold in seconds.
		/// </para>
		/// <para>
		/// This evaluator will trigger if the specified time period 
		/// <see cref="P:log4net.Core.TimeEvaluator.Interval" /> has passed since last check.
		/// </para>
		/// </remarks>
		public TimeEvaluator()
			: this(0)
		{
		}

		/// <summary>
		/// Create a new evaluator using the specified time threshold in seconds.
		/// </summary>
		/// <param name="interval">
		/// The time threshold in seconds to trigger after.
		/// Zero means it won't trigger at all.
		/// </param>
		/// <remarks>
		/// <para>
		/// Create a new evaluator using the specified time threshold in seconds.
		/// </para>
		/// <para>
		/// This evaluator will trigger if the specified time period 
		/// <see cref="P:log4net.Core.TimeEvaluator.Interval" /> has passed since last check.
		/// </para>
		/// </remarks>
		public TimeEvaluator(int interval)
		{
			m_interval = interval;
			m_lastTimeUtc = DateTime.UtcNow;
		}

		/// <summary>
		/// Is this <paramref name="loggingEvent" /> the triggering event?
		/// </summary>
		/// <param name="loggingEvent">The event to check</param>
		/// <returns>This method returns <c>true</c>, if the specified time period 
		/// <see cref="P:log4net.Core.TimeEvaluator.Interval" /> has passed since last check.. 
		/// Otherwise it returns <c>false</c></returns>
		/// <remarks>
		/// <para>
		/// This evaluator will trigger if the specified time period 
		/// <see cref="P:log4net.Core.TimeEvaluator.Interval" /> has passed since last check.
		/// </para>
		/// </remarks>
		public bool IsTriggeringEvent(LoggingEvent loggingEvent)
		{
			if (loggingEvent == null)
			{
				throw new ArgumentNullException("loggingEvent");
			}
			if (m_interval == 0)
			{
				return false;
			}
			lock (this)
			{
				if (DateTime.UtcNow.Subtract(m_lastTimeUtc).TotalSeconds > (double)m_interval)
				{
					m_lastTimeUtc = DateTime.UtcNow;
					return true;
				}
				return false;
			}
		}
	}
}
