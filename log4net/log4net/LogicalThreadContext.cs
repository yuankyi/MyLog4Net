using log4net.Util;

namespace log4net
{
	/// <summary>
	/// The log4net Logical Thread Context.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <c>LogicalThreadContext</c> provides a location for <see cref="T:System.Runtime.Remoting.Messaging.CallContext" /> specific debugging 
	/// information to be stored.
	/// The <c>LogicalThreadContext</c> properties override any <see cref="T:log4net.ThreadContext" /> or <see cref="T:log4net.GlobalContext" />
	/// properties with the same name.
	/// </para>
	/// <para>
	/// For .NET Standard 1.3 this class uses
	/// System.Threading.AsyncLocal rather than <see cref="T:System.Runtime.Remoting.Messaging.CallContext" />.
	/// </para>
	/// <para>
	/// The Logical Thread Context has a properties map and a stack.
	/// The properties and stack can 
	/// be included in the output of log messages. The <see cref="T:log4net.Layout.PatternLayout" />
	/// supports selecting and outputting these properties.
	/// </para>
	/// <para>
	/// The Logical Thread Context provides a diagnostic context for the current call context. 
	/// This is an instrument for distinguishing interleaved log
	/// output from different sources. Log output is typically interleaved
	/// when a server handles multiple clients near-simultaneously.
	/// </para>
	/// <para>
	/// The Logical Thread Context is managed on a per <see cref="T:System.Runtime.Remoting.Messaging.CallContext" /> basis.
	/// </para>
	/// <para>
	/// The <see cref="T:System.Runtime.Remoting.Messaging.CallContext" /> requires a link time 
	/// <see cref="T:System.Security.Permissions.SecurityPermission" /> for the
	/// <see cref="F:System.Security.Permissions.SecurityPermissionFlag.Infrastructure" />.
	/// If the calling code does not have this permission then this context will be disabled.
	/// It will not store any property values set on it.
	/// </para>
	/// </remarks>
	/// <example>Example of using the thread context properties to store a username.
	/// <code lang="C#">
	/// LogicalThreadContext.Properties["user"] = userName;
	/// log.Info("This log message has a LogicalThreadContext Property called 'user'");
	/// </code>
	/// </example>
	/// <example>Example of how to push a message into the context stack
	/// <code lang="C#">
	/// using(LogicalThreadContext.Stacks["LDC"].Push("my context message"))
	/// {
	/// 	log.Info("This log message has a LogicalThreadContext Stack message that includes 'my context message'");
	///
	/// } // at the end of the using block the message is automatically popped 
	/// </code>
	/// </example>
	/// <threadsafety static="true" instance="true" />
	/// <author>Nicko Cadell</author>
	public sealed class LogicalThreadContext
	{
		/// <summary>
		/// The thread context properties instance
		/// </summary>
		private static readonly LogicalThreadContextProperties s_properties = new LogicalThreadContextProperties();

		/// <summary>
		/// The thread context stacks instance
		/// </summary>
		private static readonly LogicalThreadContextStacks s_stacks = new LogicalThreadContextStacks(s_properties);

		/// <summary>
		/// The thread properties map
		/// </summary>
		/// <value>
		/// The thread properties map
		/// </value>
		/// <remarks>
		/// <para>
		/// The <c>LogicalThreadContext</c> properties override any <see cref="T:log4net.ThreadContext" /> 
		/// or <see cref="T:log4net.GlobalContext" /> properties with the same name.
		/// </para>
		/// </remarks>
		public static LogicalThreadContextProperties Properties
		{
			get
			{
				return s_properties;
			}
		}

		/// <summary>
		/// The thread stacks
		/// </summary>
		/// <value>
		/// stack map
		/// </value>
		/// <remarks>
		/// <para>
		/// The logical thread stacks.
		/// </para>
		/// </remarks>
		public static LogicalThreadContextStacks Stacks
		{
			get
			{
				return s_stacks;
			}
		}

		/// <summary>
		/// Private Constructor. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// Uses a private access modifier to prevent instantiation of this class.
		/// </para>
		/// </remarks>
		private LogicalThreadContext()
		{
		}
	}
}
