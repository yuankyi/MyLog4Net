using System;

namespace log4net.Util
{
	/// <summary>
	/// Implementation of Stacks collection for the <see cref="T:log4net.LogicalThreadContext" />
	/// </summary>
	/// <remarks>
	/// <para>
	/// Implementation of Stacks collection for the <see cref="T:log4net.LogicalThreadContext" />
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	public sealed class LogicalThreadContextStacks
	{
		private readonly LogicalThreadContextProperties m_properties;

		/// <summary>
		/// The fully qualified type of the ThreadContextStacks class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(LogicalThreadContextStacks);

		/// <summary>
		/// Gets the named thread context stack
		/// </summary>
		/// <value>
		/// The named stack
		/// </value>
		/// <remarks>
		/// <para>
		/// Gets the named thread context stack
		/// </para>
		/// </remarks>
		public LogicalThreadContextStack this[string key]
		{
			get
			{
				LogicalThreadContextStack logicalThreadContextStack = null;
				object obj = m_properties[key];
				if (obj == null)
				{
					logicalThreadContextStack = new LogicalThreadContextStack(key, registerNew);
					m_properties[key] = logicalThreadContextStack;
				}
				else
				{
					logicalThreadContextStack = (obj as LogicalThreadContextStack);
					if (logicalThreadContextStack == null)
					{
						string text = SystemInfo.NullText;
						try
						{
							text = obj.ToString();
						}
						catch
						{
						}
						LogLog.Error(declaringType, "ThreadContextStacks: Request for stack named [" + key + "] failed because a property with the same name exists which is a [" + obj.GetType().Name + "] with value [" + text + "]");
						logicalThreadContextStack = new LogicalThreadContextStack(key, registerNew);
					}
				}
				return logicalThreadContextStack;
			}
		}

		/// <summary>
		/// Internal constructor
		/// </summary>
		/// <remarks>
		/// <para>
		/// Initializes a new instance of the <see cref="T:log4net.Util.ThreadContextStacks" /> class.
		/// </para>
		/// </remarks>
		internal LogicalThreadContextStacks(LogicalThreadContextProperties properties)
		{
			m_properties = properties;
		}

		private void registerNew(string stackName, LogicalThreadContextStack stack)
		{
			m_properties[stackName] = stack;
		}
	}
}
