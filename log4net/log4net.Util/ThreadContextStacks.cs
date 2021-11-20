using System;

namespace log4net.Util
{
	/// <summary>
	/// Implementation of Stacks collection for the <see cref="T:log4net.ThreadContext" />
	/// </summary>
	/// <remarks>
	/// <para>
	/// Implementation of Stacks collection for the <see cref="T:log4net.ThreadContext" />
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	public sealed class ThreadContextStacks
	{
		private readonly ContextPropertiesBase m_properties;

		/// <summary>
		/// The fully qualified type of the ThreadContextStacks class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(ThreadContextStacks);

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
		public ThreadContextStack this[string key]
		{
			get
			{
				ThreadContextStack threadContextStack = null;
				object obj = m_properties[key];
				if (obj == null)
				{
					threadContextStack = new ThreadContextStack();
					m_properties[key] = threadContextStack;
				}
				else
				{
					threadContextStack = (obj as ThreadContextStack);
					if (threadContextStack == null)
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
						threadContextStack = new ThreadContextStack();
					}
				}
				return threadContextStack;
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
		internal ThreadContextStacks(ContextPropertiesBase properties)
		{
			m_properties = properties;
		}
	}
}
