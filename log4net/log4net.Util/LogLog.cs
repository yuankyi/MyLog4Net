#define TRACE
using System;
using System.Collections;
using System.Diagnostics;

namespace log4net.Util
{
	/// <summary>
	/// Outputs log statements from within the log4net assembly.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Log4net components cannot make log4net logging calls. However, it is
	/// sometimes useful for the user to learn about what log4net is
	/// doing.
	/// </para>
	/// <para>
	/// All log4net internal debug calls go to the standard output stream
	/// whereas internal error messages are sent to the standard error output 
	/// stream.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	public sealed class LogLog
	{
		/// <summary>
		/// Subscribes to the LogLog.LogReceived event and stores messages
		/// to the supplied IList instance.
		/// </summary>
		public class LogReceivedAdapter : IDisposable
		{
			private readonly IList items;

			private readonly LogReceivedEventHandler handler;

			/// <summary>
			///
			/// </summary>
			public IList Items
			{
				get
				{
					return items;
				}
			}

			/// <summary>
			///
			/// </summary>
			/// <param name="items"></param>
			public LogReceivedAdapter(IList items)
			{
				this.items = items;
				handler = LogLog_LogReceived;
				LogReceived += handler;
			}

			private void LogLog_LogReceived(object source, LogReceivedEventArgs e)
			{
				items.Add(e.LogLog);
			}

			/// <summary>
			///
			/// </summary>
			public void Dispose()
			{
				LogReceived -= handler;
			}
		}

		private readonly Type source;

		private readonly DateTime timeStampUtc;

		private readonly string prefix;

		private readonly string message;

		private readonly Exception exception;

		/// <summary>
		///  Default debug level
		/// </summary>
		private static bool s_debugEnabled;

		/// <summary>
		/// In quietMode not even errors generate any output.
		/// </summary>
		private static bool s_quietMode;

		private static bool s_emitInternalMessages;

		private const string PREFIX = "log4net: ";

		private const string ERR_PREFIX = "log4net:ERROR ";

		private const string WARN_PREFIX = "log4net:WARN ";

		/// <summary>
		/// The Type that generated the internal message.
		/// </summary>
		public Type Source
		{
			get
			{
				return source;
			}
		}

		/// <summary>
		/// The DateTime stamp of when the internal message was received.
		/// </summary>
		public DateTime TimeStamp
		{
			get
			{
				return timeStampUtc.ToLocalTime();
			}
		}

		/// <summary>
		/// The UTC DateTime stamp of when the internal message was received.
		/// </summary>
		public DateTime TimeStampUtc
		{
			get
			{
				return timeStampUtc;
			}
		}

		/// <summary>
		/// A string indicating the severity of the internal message.
		/// </summary>
		/// <remarks>
		/// "log4net: ", 
		/// "log4net:ERROR ", 
		/// "log4net:WARN "
		/// </remarks>
		public string Prefix
		{
			get
			{
				return prefix;
			}
		}

		/// <summary>
		/// The internal log message.
		/// </summary>
		public string Message
		{
			get
			{
				return message;
			}
		}

		/// <summary>
		/// The Exception related to the message.
		/// </summary>
		/// <remarks>
		/// Optional. Will be null if no Exception was passed.
		/// </remarks>
		public Exception Exception
		{
			get
			{
				return exception;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether log4net internal logging
		/// is enabled or disabled.
		/// </summary>
		/// <value>
		/// <c>true</c> if log4net internal logging is enabled, otherwise 
		/// <c>false</c>.
		/// </value>
		/// <remarks>
		/// <para>
		/// When set to <c>true</c>, internal debug level logging will be 
		/// displayed.
		/// </para>
		/// <para>
		/// This value can be set by setting the application setting 
		/// <c>log4net.Internal.Debug</c> in the application configuration
		/// file.
		/// </para>
		/// <para>
		/// The default value is <c>false</c>, i.e. debugging is
		/// disabled.
		/// </para>
		/// </remarks>
		/// <example>
		/// <para>
		/// The following example enables internal debugging using the 
		/// application configuration file :
		/// </para>
		/// <code lang="XML" escaped="true">
		/// <configuration>
		/// 	<appSettings>
		/// 		<add key="log4net.Internal.Debug" value="true" />
		/// 	</appSettings>
		/// </configuration>
		/// </code>
		/// </example>
		public static bool InternalDebugging
		{
			get
			{
				return s_debugEnabled;
			}
			set
			{
				s_debugEnabled = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether log4net should generate no output
		/// from internal logging, not even for errors. 
		/// </summary>
		/// <value>
		/// <c>true</c> if log4net should generate no output at all from internal 
		/// logging, otherwise <c>false</c>.
		/// </value>
		/// <remarks>
		/// <para>
		/// When set to <c>true</c> will cause internal logging at all levels to be 
		/// suppressed. This means that no warning or error reports will be logged. 
		/// This option overrides the <see cref="P:log4net.Util.LogLog.InternalDebugging" /> setting and 
		/// disables all debug also.
		/// </para>
		/// <para>This value can be set by setting the application setting
		/// <c>log4net.Internal.Quiet</c> in the application configuration file.
		/// </para>
		/// <para>
		/// The default value is <c>false</c>, i.e. internal logging is not
		/// disabled.
		/// </para>
		/// </remarks>
		/// <example>
		/// The following example disables internal logging using the 
		/// application configuration file :
		/// <code lang="XML" escaped="true">
		/// <configuration>
		/// 	<appSettings>
		/// 		<add key="log4net.Internal.Quiet" value="true" />
		/// 	</appSettings>
		/// </configuration>
		/// </code>
		/// </example>
		public static bool QuietMode
		{
			get
			{
				return s_quietMode;
			}
			set
			{
				s_quietMode = value;
			}
		}

		/// <summary>
		///
		/// </summary>
		public static bool EmitInternalMessages
		{
			get
			{
				return s_emitInternalMessages;
			}
			set
			{
				s_emitInternalMessages = value;
			}
		}

		/// <summary>
		/// Test if LogLog.Debug is enabled for output.
		/// </summary>
		/// <value>
		/// <c>true</c> if Debug is enabled
		/// </value>
		/// <remarks>
		/// <para>
		/// Test if LogLog.Debug is enabled for output.
		/// </para>
		/// </remarks>
		public static bool IsDebugEnabled
		{
			get
			{
				if (s_debugEnabled)
				{
					return !s_quietMode;
				}
				return false;
			}
		}

		/// <summary>
		/// Test if LogLog.Warn is enabled for output.
		/// </summary>
		/// <value>
		/// <c>true</c> if Warn is enabled
		/// </value>
		/// <remarks>
		/// <para>
		/// Test if LogLog.Warn is enabled for output.
		/// </para>
		/// </remarks>
		public static bool IsWarnEnabled
		{
			get
			{
				return !s_quietMode;
			}
		}

		/// <summary>
		/// Test if LogLog.Error is enabled for output.
		/// </summary>
		/// <value>
		/// <c>true</c> if Error is enabled
		/// </value>
		/// <remarks>
		/// <para>
		/// Test if LogLog.Error is enabled for output.
		/// </para>
		/// </remarks>
		public static bool IsErrorEnabled
		{
			get
			{
				return !s_quietMode;
			}
		}

		/// <summary>
		/// The event raised when an internal message has been received.
		/// </summary>
		public static event LogReceivedEventHandler LogReceived;

		/// <summary>
		/// Formats Prefix, Source, and Message in the same format as the value
		/// sent to Console.Out and Trace.Write.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Prefix + Source.Name + ": " + Message;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:log4net.Util.LogLog" /> class. 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="prefix"></param>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		public LogLog(Type source, string prefix, string message, Exception exception)
		{
			timeStampUtc = DateTime.UtcNow;
			this.source = source;
			this.prefix = prefix;
			this.message = message;
			this.exception = exception;
		}

		/// <summary>
		/// Static constructor that initializes logging by reading 
		/// settings from the application configuration file.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <c>log4net.Internal.Debug</c> application setting
		/// controls internal debugging. This setting should be set
		/// to <c>true</c> to enable debugging.
		/// </para>
		/// <para>
		/// The <c>log4net.Internal.Quiet</c> application setting
		/// suppresses all internal logging including error messages. 
		/// This setting should be set to <c>true</c> to enable message
		/// suppression.
		/// </para>
		/// </remarks>
		static LogLog()
		{
			s_debugEnabled = false;
			s_quietMode = false;
			s_emitInternalMessages = true;
			try
			{
				InternalDebugging = OptionConverter.ToBoolean(SystemInfo.GetAppSetting("log4net.Internal.Debug"), false);
				QuietMode = OptionConverter.ToBoolean(SystemInfo.GetAppSetting("log4net.Internal.Quiet"), false);
				EmitInternalMessages = OptionConverter.ToBoolean(SystemInfo.GetAppSetting("log4net.Internal.Emit"), true);
			}
			catch (Exception ex)
			{
				Error(typeof(LogLog), "Exception while reading ConfigurationSettings. Check your .config file is well formed XML.", ex);
			}
		}

		/// <summary>
		/// Raises the LogReceived event when an internal messages is received.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="prefix"></param>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		public static void OnLogReceived(Type source, string prefix, string message, Exception exception)
		{
			if (LogLog.LogReceived != null)
			{
				LogLog.LogReceived(null, new LogReceivedEventArgs(new LogLog(source, prefix, message, exception)));
			}
		}

		/// <summary>
		/// Writes log4net internal debug messages to the 
		/// standard output stream.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="message">The message to log.</param>
		/// <remarks>
		/// <para>
		/// All internal debug messages are prepended with 
		/// the string "log4net: ".
		/// </para>
		/// </remarks>
		public static void Debug(Type source, string message)
		{
			if (IsDebugEnabled)
			{
				if (EmitInternalMessages)
				{
					EmitOutLine("log4net: " + message);
				}
				OnLogReceived(source, "log4net: ", message, null);
			}
		}

		/// <summary>
		/// Writes log4net internal debug messages to the 
		/// standard output stream.
		/// </summary>
		/// <param name="source">The Type that generated this message.</param>
		/// <param name="message">The message to log.</param>
		/// <param name="exception">An exception to log.</param>
		/// <remarks>
		/// <para>
		/// All internal debug messages are prepended with 
		/// the string "log4net: ".
		/// </para>
		/// </remarks>
		public static void Debug(Type source, string message, Exception exception)
		{
			if (IsDebugEnabled)
			{
				if (EmitInternalMessages)
				{
					EmitOutLine("log4net: " + message);
					if (exception != null)
					{
						EmitOutLine(exception.ToString());
					}
				}
				OnLogReceived(source, "log4net: ", message, exception);
			}
		}

		/// <summary>
		/// Writes log4net internal warning messages to the 
		/// standard error stream.
		/// </summary>
		/// <param name="source">The Type that generated this message.</param>
		/// <param name="message">The message to log.</param>
		/// <remarks>
		/// <para>
		/// All internal warning messages are prepended with 
		/// the string "log4net:WARN ".
		/// </para>
		/// </remarks>
		public static void Warn(Type source, string message)
		{
			if (IsWarnEnabled)
			{
				if (EmitInternalMessages)
				{
					EmitErrorLine("log4net:WARN " + message);
				}
				OnLogReceived(source, "log4net:WARN ", message, null);
			}
		}

		/// <summary>
		/// Writes log4net internal warning messages to the 
		/// standard error stream.
		/// </summary>
		/// <param name="source">The Type that generated this message.</param>
		/// <param name="message">The message to log.</param>
		/// <param name="exception">An exception to log.</param>
		/// <remarks>
		/// <para>
		/// All internal warning messages are prepended with 
		/// the string "log4net:WARN ".
		/// </para>
		/// </remarks>
		public static void Warn(Type source, string message, Exception exception)
		{
			if (IsWarnEnabled)
			{
				if (EmitInternalMessages)
				{
					EmitErrorLine("log4net:WARN " + message);
					if (exception != null)
					{
						EmitErrorLine(exception.ToString());
					}
				}
				OnLogReceived(source, "log4net:WARN ", message, exception);
			}
		}

		/// <summary>
		/// Writes log4net internal error messages to the 
		/// standard error stream.
		/// </summary>
		/// <param name="source">The Type that generated this message.</param>
		/// <param name="message">The message to log.</param>
		/// <remarks>
		/// <para>
		/// All internal error messages are prepended with 
		/// the string "log4net:ERROR ".
		/// </para>
		/// </remarks>
		public static void Error(Type source, string message)
		{
			if (IsErrorEnabled)
			{
				if (EmitInternalMessages)
				{
					EmitErrorLine("log4net:ERROR " + message);
				}
				OnLogReceived(source, "log4net:ERROR ", message, null);
			}
		}

		/// <summary>
		/// Writes log4net internal error messages to the 
		/// standard error stream.
		/// </summary>
		/// <param name="source">The Type that generated this message.</param>
		/// <param name="message">The message to log.</param>
		/// <param name="exception">An exception to log.</param>
		/// <remarks>
		/// <para>
		/// All internal debug messages are prepended with 
		/// the string "log4net:ERROR ".
		/// </para>
		/// </remarks>
		public static void Error(Type source, string message, Exception exception)
		{
			if (IsErrorEnabled)
			{
				if (EmitInternalMessages)
				{
					EmitErrorLine("log4net:ERROR " + message);
					if (exception != null)
					{
						EmitErrorLine(exception.ToString());
					}
				}
				OnLogReceived(source, "log4net:ERROR ", message, exception);
			}
		}

		/// <summary>
		/// Writes output to the standard output stream.  
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <remarks>
		/// <para>
		/// Writes to both Console.Out and System.Diagnostics.Trace.
		/// Note that the System.Diagnostics.Trace is not supported
		/// on the Compact Framework.
		/// </para>
		/// <para>
		/// If the AppDomain is not configured with a config file then
		/// the call to System.Diagnostics.Trace may fail. This is only
		/// an issue if you are programmatically creating your own AppDomains.
		/// </para>
		/// </remarks>
		private static void EmitOutLine(string message)
		{
			try
			{
				Console.Out.WriteLine(message);
				Trace.WriteLine(message);
			}
			catch
			{
			}
		}

		/// <summary>
		/// Writes output to the standard error stream.  
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <remarks>
		/// <para>
		/// Writes to both Console.Error and System.Diagnostics.Trace.
		/// Note that the System.Diagnostics.Trace is not supported
		/// on the Compact Framework.
		/// </para>
		/// <para>
		/// If the AppDomain is not configured with a config file then
		/// the call to System.Diagnostics.Trace may fail. This is only
		/// an issue if you are programmatically creating your own AppDomains.
		/// </para>
		/// </remarks>
		private static void EmitErrorLine(string message)
		{
			try
			{
				Console.Error.WriteLine(message);
				Trace.WriteLine(message);
			}
			catch
			{
			}
		}
	}
}
