using System;

namespace log4net.Util
{
	/// <summary>
	/// The static class ILogExtensions contains a set of widely used
	/// methods that ease the interaction with the ILog interface implementations.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class contains methods for logging at different levels and checks the
	/// properties for determining if those logging levels are enabled in the current
	/// configuration.
	/// </para>
	/// </remarks>
	/// <example>Simple example of logging messages
	/// <code lang="C#">
	/// using log4net.Util;
	///
	/// ILog log = LogManager.GetLogger("application-log");
	///
	/// log.InfoExt("Application Start");
	/// log.DebugExt("This is a debug message");
	/// </code>
	/// </example>
	public static class ILogExtensions
	{
		/// <summary>
		/// The fully qualified type of the Logger class.
		/// </summary>
		private static readonly Type declaringType = typeof(ILogExtensions);

		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Debug" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="callback">The lambda expression that gets the object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>INFO</c>
		/// enabled by reading the value <seealso cref="P:log4net.ILog.IsDebugEnabled" /> property.
		/// This check happens always and does not depend on the <seealso cref="T:log4net.ILog" />
		/// implementation.  If this logger is <c>INFO</c> enabled, then it converts 
		/// the message object (retrieved by invocation of the provided callback) to a 
		/// string by invoking the appropriate <see cref="T:log4net.ObjectRenderer.IObjectRenderer" />.
		/// It then proceeds to call all the registered appenders in this logger 
		/// and also higher in the hierarchy depending on the value of 
		/// the additivity flag.
		/// </para>
		/// <para><b>WARNING</b> Note that passing an <see cref="T:System.Exception" /> 
		/// to this method will print the name of the <see cref="T:System.Exception" /> 
		/// but no stack trace. To print a stack trace use the 
		/// <see cref="M:log4net.Util.ILogExtensions.DebugExt(log4net.ILog,System.Func{System.Object},System.Exception)" /> form instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Debug(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsDebugEnabled" />
		public static void DebugExt(this ILog logger, Func<object> callback)
		{
			try
			{
				if (logger.IsDebugEnabled)
				{
					logger.Debug(callback());
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Debug" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="callback">The lambda expression that gets the object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// See the <see cref="M:log4net.Util.ILogExtensions.DebugExt(log4net.ILog,System.Object)" /> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Debug(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsDebugEnabled" />
		public static void DebugExt(this ILog logger, Func<object> callback, Exception exception)
		{
			try
			{
				if (logger.IsDebugEnabled)
				{
					logger.Debug(callback(), exception);
				}
			}
			catch (Exception exception2)
			{
				LogLog.Error(declaringType, "Exception while logging", exception2);
			}
		}

		/// <overloads>Log a message object with the <see cref="F:log4net.Core.Level.Debug" /> level.</overloads> //TODO
		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Debug" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>INFO</c>
		/// enabled by reading the value <seealso cref="P:log4net.ILog.IsDebugEnabled" /> property.
		/// This check happens always and does not depend on the <seealso cref="T:log4net.ILog" />
		/// implementation. If this logger is <c>INFO</c> enabled, then it converts 
		/// the message object (passed as parameter) to a string by invoking the appropriate
		/// <see cref="T:log4net.ObjectRenderer.IObjectRenderer" />. It then 
		/// proceeds to call all the registered appenders in this logger 
		/// and also higher in the hierarchy depending on the value of 
		/// the additivity flag.
		/// </para>
		/// <para><b>WARNING</b> Note that passing an <see cref="T:System.Exception" /> 
		/// to this method will print the name of the <see cref="T:System.Exception" /> 
		/// but no stack trace. To print a stack trace use the 
		/// <see cref="M:log4net.Util.ILogExtensions.DebugExt(log4net.ILog,System.Object,System.Exception)" /> form instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Debug(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsDebugEnabled" />
		public static void DebugExt(this ILog logger, object message)
		{
			try
			{
				if (logger.IsDebugEnabled)
				{
					logger.Debug(message);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Debug" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// See the <see cref="M:log4net.Util.ILogExtensions.DebugExt(log4net.ILog,System.Object)" /> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Debug(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsDebugEnabled" />
		public static void DebugExt(this ILog logger, object message, Exception exception)
		{
			try
			{
				if (logger.IsDebugEnabled)
				{
					logger.Debug(message, exception);
				}
			}
			catch (Exception exception2)
			{
				LogLog.Error(declaringType, "Exception while logging", exception2);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Debug" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.DebugExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Debug(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsDebugEnabled" />
		public static void DebugFormatExt(this ILog logger, string format, object arg0)
		{
			try
			{
				if (logger.IsDebugEnabled)
				{
					logger.DebugFormat(format, arg0);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Debug" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.DebugExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Debug(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsDebugEnabled" />
		public static void DebugFormatExt(this ILog logger, string format, params object[] args)
		{
			try
			{
				if (logger.IsDebugEnabled)
				{
					logger.DebugFormat(format, args);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Debug" /> level.
		/// </summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> that supplies culture-specific formatting information</param>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.DebugExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Debug(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsDebugEnabled" />
		public static void DebugFormatExt(this ILog logger, IFormatProvider provider, string format, params object[] args)
		{
			try
			{
				if (logger.IsDebugEnabled)
				{
					logger.DebugFormat(provider, format, args);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Debug" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.DebugExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Debug(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsDebugEnabled" />
		public static void DebugFormatExt(this ILog logger, string format, object arg0, object arg1)
		{
			try
			{
				if (logger.IsDebugEnabled)
				{
					logger.DebugFormat(format, arg0, arg1);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Debug" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <param name="arg2">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.DebugExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Debug(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsDebugEnabled" />
		public static void DebugFormatExt(this ILog logger, string format, object arg0, object arg1, object arg2)
		{
			try
			{
				if (logger.IsDebugEnabled)
				{
					logger.DebugFormat(format, arg0, arg1, arg2);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Info" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="callback">The lambda expression that gets the object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>INFO</c>
		/// enabled by reading the value <seealso cref="P:log4net.ILog.IsInfoEnabled" /> property.
		/// This check happens always and does not depend on the <seealso cref="T:log4net.ILog" />
		/// implementation.  If this logger is <c>INFO</c> enabled, then it converts 
		/// the message object (retrieved by invocation of the provided callback) to a 
		/// string by invoking the appropriate <see cref="T:log4net.ObjectRenderer.IObjectRenderer" />.
		/// It then proceeds to call all the registered appenders in this logger 
		/// and also higher in the hierarchy depending on the value of 
		/// the additivity flag.
		/// </para>
		/// <para><b>WARNING</b> Note that passing an <see cref="T:System.Exception" /> 
		/// to this method will print the name of the <see cref="T:System.Exception" /> 
		/// but no stack trace. To print a stack trace use the 
		/// <see cref="M:log4net.Util.ILogExtensions.InfoExt(log4net.ILog,System.Func{System.Object},System.Exception)" /> form instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Info(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsInfoEnabled" />
		public static void InfoExt(this ILog logger, Func<object> callback)
		{
			try
			{
				if (logger.IsInfoEnabled)
				{
					logger.Info(callback());
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Info" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="callback">The lambda expression that gets the object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// See the <see cref="M:log4net.Util.ILogExtensions.InfoExt(log4net.ILog,System.Object)" /> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Info(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsInfoEnabled" />
		public static void InfoExt(this ILog logger, Func<object> callback, Exception exception)
		{
			try
			{
				if (logger.IsInfoEnabled)
				{
					logger.Info(callback(), exception);
				}
			}
			catch (Exception exception2)
			{
				LogLog.Error(declaringType, "Exception while logging", exception2);
			}
		}

		/// <overloads>Log a message object with the <see cref="F:log4net.Core.Level.Info" /> level.</overloads> //TODO
		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Info" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>INFO</c>
		/// enabled by reading the value <seealso cref="P:log4net.ILog.IsInfoEnabled" /> property.
		/// This check happens always and does not depend on the <seealso cref="T:log4net.ILog" />
		/// implementation. If this logger is <c>INFO</c> enabled, then it converts 
		/// the message object (passed as parameter) to a string by invoking the appropriate
		/// <see cref="T:log4net.ObjectRenderer.IObjectRenderer" />. It then 
		/// proceeds to call all the registered appenders in this logger 
		/// and also higher in the hierarchy depending on the value of 
		/// the additivity flag.
		/// </para>
		/// <para><b>WARNING</b> Note that passing an <see cref="T:System.Exception" /> 
		/// to this method will print the name of the <see cref="T:System.Exception" /> 
		/// but no stack trace. To print a stack trace use the 
		/// <see cref="M:log4net.Util.ILogExtensions.InfoExt(log4net.ILog,System.Object,System.Exception)" /> form instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Info(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsInfoEnabled" />
		public static void InfoExt(this ILog logger, object message)
		{
			try
			{
				if (logger.IsInfoEnabled)
				{
					logger.Info(message);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Info" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// See the <see cref="M:log4net.Util.ILogExtensions.InfoExt(log4net.ILog,System.Object)" /> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Info(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsInfoEnabled" />
		public static void InfoExt(this ILog logger, object message, Exception exception)
		{
			try
			{
				if (logger.IsInfoEnabled)
				{
					logger.Info(message, exception);
				}
			}
			catch (Exception exception2)
			{
				LogLog.Error(declaringType, "Exception while logging", exception2);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Info" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.InfoExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Info(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsInfoEnabled" />
		public static void InfoFormatExt(this ILog logger, string format, object arg0)
		{
			try
			{
				if (logger.IsInfoEnabled)
				{
					logger.InfoFormat(format, arg0);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Info" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.InfoExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Info(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsInfoEnabled" />
		public static void InfoFormatExt(this ILog logger, string format, params object[] args)
		{
			try
			{
				if (logger.IsInfoEnabled)
				{
					logger.InfoFormat(format, args);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Info" /> level.
		/// </summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> that supplies culture-specific formatting information</param>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.InfoExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Info(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsInfoEnabled" />
		public static void InfoFormatExt(this ILog logger, IFormatProvider provider, string format, params object[] args)
		{
			try
			{
				if (logger.IsInfoEnabled)
				{
					logger.InfoFormat(provider, format, args);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Info" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.InfoExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Info(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsInfoEnabled" />
		public static void InfoFormatExt(this ILog logger, string format, object arg0, object arg1)
		{
			try
			{
				if (logger.IsInfoEnabled)
				{
					logger.InfoFormat(format, arg0, arg1);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Info" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <param name="arg2">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.InfoExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Info(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsInfoEnabled" />
		public static void InfoFormatExt(this ILog logger, string format, object arg0, object arg1, object arg2)
		{
			try
			{
				if (logger.IsInfoEnabled)
				{
					logger.InfoFormat(format, arg0, arg1, arg2);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Warn" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="callback">The lambda expression that gets the object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>WARN</c>
		/// enabled by reading the value <seealso cref="P:log4net.ILog.IsWarnEnabled" /> property.
		/// This check happens always and does not depend on the <seealso cref="T:log4net.ILog" />
		/// implementation.  If this logger is <c>WARN</c> enabled, then it converts 
		/// the message object (retrieved by invocation of the provided callback) to a 
		/// string by invoking the appropriate <see cref="T:log4net.ObjectRenderer.IObjectRenderer" />.
		/// It then proceeds to call all the registered appenders in this logger 
		/// and also higher in the hierarchy depending on the value of 
		/// the additivity flag.
		/// </para>
		/// <para><b>WARNING</b> Note that passing an <see cref="T:System.Exception" /> 
		/// to this method will print the name of the <see cref="T:System.Exception" /> 
		/// but no stack trace. To print a stack trace use the 
		/// <see cref="M:log4net.Util.ILogExtensions.WarnExt(log4net.ILog,System.Func{System.Object},System.Exception)" /> form instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Warn(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsWarnEnabled" />
		public static void WarnExt(this ILog logger, Func<object> callback)
		{
			try
			{
				if (logger.IsWarnEnabled)
				{
					logger.Warn(callback());
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Warn" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="callback">The lambda expression that gets the object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// See the <see cref="M:log4net.Util.ILogExtensions.WarnExt(log4net.ILog,System.Object)" /> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Warn(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsWarnEnabled" />
		public static void WarnExt(this ILog logger, Func<object> callback, Exception exception)
		{
			try
			{
				if (logger.IsWarnEnabled)
				{
					logger.Warn(callback(), exception);
				}
			}
			catch (Exception exception2)
			{
				LogLog.Error(declaringType, "Exception while logging", exception2);
			}
		}

		/// <overloads>Log a message object with the <see cref="F:log4net.Core.Level.Warn" /> level.</overloads> //TODO
		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Warn" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>WARN</c>
		/// enabled by reading the value <seealso cref="P:log4net.ILog.IsWarnEnabled" /> property.
		/// This check happens always and does not depend on the <seealso cref="T:log4net.ILog" />
		/// implementation. If this logger is <c>WARN</c> enabled, then it converts 
		/// the message object (passed as parameter) to a string by invoking the appropriate
		/// <see cref="T:log4net.ObjectRenderer.IObjectRenderer" />. It then 
		/// proceeds to call all the registered appenders in this logger 
		/// and also higher in the hierarchy depending on the value of 
		/// the additivity flag.
		/// </para>
		/// <para><b>WARNING</b> Note that passing an <see cref="T:System.Exception" /> 
		/// to this method will print the name of the <see cref="T:System.Exception" /> 
		/// but no stack trace. To print a stack trace use the 
		/// <see cref="M:log4net.Util.ILogExtensions.WarnExt(log4net.ILog,System.Object,System.Exception)" /> form instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Warn(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsWarnEnabled" />
		public static void WarnExt(this ILog logger, object message)
		{
			try
			{
				if (logger.IsWarnEnabled)
				{
					logger.Warn(message);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Warn" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// See the <see cref="M:log4net.Util.ILogExtensions.WarnExt(log4net.ILog,System.Object)" /> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Warn(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsWarnEnabled" />
		public static void WarnExt(this ILog logger, object message, Exception exception)
		{
			try
			{
				if (logger.IsWarnEnabled)
				{
					logger.Warn(message, exception);
				}
			}
			catch (Exception exception2)
			{
				LogLog.Error(declaringType, "Exception while logging", exception2);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Warn" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.WarnExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Warn(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsWarnEnabled" />
		public static void WarnFormatExt(this ILog logger, string format, object arg0)
		{
			try
			{
				if (logger.IsWarnEnabled)
				{
					logger.WarnFormat(format, arg0);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Warn" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.WarnExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Warn(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsWarnEnabled" />
		public static void WarnFormatExt(this ILog logger, string format, params object[] args)
		{
			try
			{
				if (logger.IsWarnEnabled)
				{
					logger.WarnFormat(format, args);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Warn" /> level.
		/// </summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> that supplies culture-specific formatting information</param>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.WarnExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Warn(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsWarnEnabled" />
		public static void WarnFormatExt(this ILog logger, IFormatProvider provider, string format, params object[] args)
		{
			try
			{
				if (logger.IsWarnEnabled)
				{
					logger.WarnFormat(provider, format, args);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Warn" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.WarnExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Warn(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsWarnEnabled" />
		public static void WarnFormatExt(this ILog logger, string format, object arg0, object arg1)
		{
			try
			{
				if (logger.IsWarnEnabled)
				{
					logger.WarnFormat(format, arg0, arg1);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Warn" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <param name="arg2">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.WarnExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Warn(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsWarnEnabled" />
		public static void WarnFormatExt(this ILog logger, string format, object arg0, object arg1, object arg2)
		{
			try
			{
				if (logger.IsWarnEnabled)
				{
					logger.WarnFormat(format, arg0, arg1, arg2);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Error" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="callback">The lambda expression that gets the object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>ERROR</c>
		/// enabled by reading the value <seealso cref="P:log4net.ILog.IsErrorEnabled" /> property.
		/// This check happens always and does not depend on the <seealso cref="T:log4net.ILog" />
		/// implementation.  If this logger is <c>ERROR</c> enabled, then it converts 
		/// the message object (retrieved by invocation of the provided callback) to a 
		/// string by invoking the appropriate <see cref="T:log4net.ObjectRenderer.IObjectRenderer" />.
		/// It then proceeds to call all the registered appenders in this logger 
		/// and also higher in the hierarchy depending on the value of 
		/// the additivity flag.
		/// </para>
		/// <para><b>WARNING</b> Note that passing an <see cref="T:System.Exception" /> 
		/// to this method will print the name of the <see cref="T:System.Exception" /> 
		/// but no stack trace. To print a stack trace use the 
		/// <see cref="M:log4net.Util.ILogExtensions.ErrorExt(log4net.ILog,System.Func{System.Object},System.Exception)" /> form instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Error(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsErrorEnabled" />
		public static void ErrorExt(this ILog logger, Func<object> callback)
		{
			try
			{
				if (logger.IsErrorEnabled)
				{
					logger.Error(callback());
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Error" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="callback">The lambda expression that gets the object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// See the <see cref="M:log4net.Util.ILogExtensions.ErrorExt(log4net.ILog,System.Object)" /> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Error(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsErrorEnabled" />
		public static void ErrorExt(this ILog logger, Func<object> callback, Exception exception)
		{
			try
			{
				if (logger.IsErrorEnabled)
				{
					logger.Error(callback(), exception);
				}
			}
			catch (Exception exception2)
			{
				LogLog.Error(declaringType, "Exception while logging", exception2);
			}
		}

		/// <overloads>Log a message object with the <see cref="F:log4net.Core.Level.Error" /> level.</overloads> //TODO
		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Error" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>ERROR</c>
		/// enabled by reading the value <seealso cref="P:log4net.ILog.IsErrorEnabled" /> property.
		/// This check happens always and does not depend on the <seealso cref="T:log4net.ILog" />
		/// implementation. If this logger is <c>ERROR</c> enabled, then it converts 
		/// the message object (passed as parameter) to a string by invoking the appropriate
		/// <see cref="T:log4net.ObjectRenderer.IObjectRenderer" />. It then 
		/// proceeds to call all the registered appenders in this logger 
		/// and also higher in the hierarchy depending on the value of 
		/// the additivity flag.
		/// </para>
		/// <para><b>WARNING</b> Note that passing an <see cref="T:System.Exception" /> 
		/// to this method will print the name of the <see cref="T:System.Exception" /> 
		/// but no stack trace. To print a stack trace use the 
		/// <see cref="M:log4net.Util.ILogExtensions.ErrorExt(log4net.ILog,System.Object,System.Exception)" /> form instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Error(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsErrorEnabled" />
		public static void ErrorExt(this ILog logger, object message)
		{
			try
			{
				if (logger.IsErrorEnabled)
				{
					logger.Error(message);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Error" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// See the <see cref="M:log4net.Util.ILogExtensions.ErrorExt(log4net.ILog,System.Object)" /> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Error(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsErrorEnabled" />
		public static void ErrorExt(this ILog logger, object message, Exception exception)
		{
			try
			{
				if (logger.IsErrorEnabled)
				{
					logger.Error(message, exception);
				}
			}
			catch (Exception exception2)
			{
				LogLog.Error(declaringType, "Exception while logging", exception2);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Error" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.ErrorExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Error(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsErrorEnabled" />
		public static void ErrorFormatExt(this ILog logger, string format, object arg0)
		{
			try
			{
				if (logger.IsErrorEnabled)
				{
					logger.ErrorFormat(format, arg0);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Error" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.ErrorExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Error(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsErrorEnabled" />
		public static void ErrorFormatExt(this ILog logger, string format, params object[] args)
		{
			try
			{
				if (logger.IsErrorEnabled)
				{
					logger.ErrorFormat(format, args);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Error" /> level.
		/// </summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> that supplies culture-specific formatting information</param>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.ErrorExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Error(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsErrorEnabled" />
		public static void ErrorFormatExt(this ILog logger, IFormatProvider provider, string format, params object[] args)
		{
			try
			{
				if (logger.IsErrorEnabled)
				{
					logger.ErrorFormat(provider, format, args);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Error" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.ErrorExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Error(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsErrorEnabled" />
		public static void ErrorFormatExt(this ILog logger, string format, object arg0, object arg1)
		{
			try
			{
				if (logger.IsErrorEnabled)
				{
					logger.ErrorFormat(format, arg0, arg1);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Error" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <param name="arg2">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.ErrorExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Error(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsErrorEnabled" />
		public static void ErrorFormatExt(this ILog logger, string format, object arg0, object arg1, object arg2)
		{
			try
			{
				if (logger.IsErrorEnabled)
				{
					logger.ErrorFormat(format, arg0, arg1, arg2);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Fatal" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="callback">The lambda expression that gets the object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>FATAL</c>
		/// enabled by reading the value <seealso cref="P:log4net.ILog.IsFatalEnabled" /> property.
		/// This check happens always and does not depend on the <seealso cref="T:log4net.ILog" />
		/// implementation.  If this logger is <c>FATAL</c> enabled, then it converts 
		/// the message object (retrieved by invocation of the provided callback) to a 
		/// string by invoking the appropriate <see cref="T:log4net.ObjectRenderer.IObjectRenderer" />.
		/// It then proceeds to call all the registered appenders in this logger 
		/// and also higher in the hierarchy depending on the value of 
		/// the additivity flag.
		/// </para>
		/// <para><b>WARNING</b> Note that passing an <see cref="T:System.Exception" /> 
		/// to this method will print the name of the <see cref="T:System.Exception" /> 
		/// but no stack trace. To print a stack trace use the 
		/// <see cref="M:log4net.Util.ILogExtensions.FatalExt(log4net.ILog,System.Func{System.Object},System.Exception)" /> form instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Fatal(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsFatalEnabled" />
		public static void FatalExt(this ILog logger, Func<object> callback)
		{
			try
			{
				if (logger.IsFatalEnabled)
				{
					logger.Fatal(callback());
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Fatal" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="callback">The lambda expression that gets the object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// See the <see cref="M:log4net.Util.ILogExtensions.FatalExt(log4net.ILog,System.Object)" /> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Fatal(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsFatalEnabled" />
		public static void FatalExt(this ILog logger, Func<object> callback, Exception exception)
		{
			try
			{
				if (logger.IsFatalEnabled)
				{
					logger.Fatal(callback(), exception);
				}
			}
			catch (Exception exception2)
			{
				LogLog.Error(declaringType, "Exception while logging", exception2);
			}
		}

		/// <overloads>Log a message object with the <see cref="F:log4net.Core.Level.Fatal" /> level.</overloads> //TODO
		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Fatal" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="message">The message object to log.</param>
		/// <remarks>
		/// <para>
		/// This method first checks if this logger is <c>FATAL</c>
		/// enabled by reading the value <seealso cref="P:log4net.ILog.IsFatalEnabled" /> property.
		/// This check happens always and does not depend on the <seealso cref="T:log4net.ILog" />
		/// implementation. If this logger is <c>FATAL</c> enabled, then it converts 
		/// the message object (passed as parameter) to a string by invoking the appropriate
		/// <see cref="T:log4net.ObjectRenderer.IObjectRenderer" />. It then 
		/// proceeds to call all the registered appenders in this logger 
		/// and also higher in the hierarchy depending on the value of 
		/// the additivity flag.
		/// </para>
		/// <para><b>WARNING</b> Note that passing an <see cref="T:System.Exception" /> 
		/// to this method will print the name of the <see cref="T:System.Exception" /> 
		/// but no stack trace. To print a stack trace use the 
		/// <see cref="M:log4net.Util.ILogExtensions.FatalExt(log4net.ILog,System.Object,System.Exception)" /> form instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Fatal(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsFatalEnabled" />
		public static void FatalExt(this ILog logger, object message)
		{
			try
			{
				if (logger.IsFatalEnabled)
				{
					logger.Fatal(message);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Log a message object with the <see cref="F:log4net.Core.Level.Fatal" /> level including
		/// the stack trace of the <see cref="T:System.Exception" /> passed
		/// as a parameter.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception to log, including its stack trace.</param>
		/// <remarks>
		/// <para>
		/// See the <see cref="M:log4net.Util.ILogExtensions.FatalExt(log4net.ILog,System.Object)" /> form for more detailed information.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Fatal(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsFatalEnabled" />
		public static void FatalExt(this ILog logger, object message, Exception exception)
		{
			try
			{
				if (logger.IsFatalEnabled)
				{
					logger.Fatal(message, exception);
				}
			}
			catch (Exception exception2)
			{
				LogLog.Error(declaringType, "Exception while logging", exception2);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Fatal" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.FatalExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Fatal(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsFatalEnabled" />
		public static void FatalFormatExt(this ILog logger, string format, object arg0)
		{
			try
			{
				if (logger.IsFatalEnabled)
				{
					logger.FatalFormat(format, arg0);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Fatal" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.FatalExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Fatal(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsFatalEnabled" />
		public static void FatalFormatExt(this ILog logger, string format, params object[] args)
		{
			try
			{
				if (logger.IsFatalEnabled)
				{
					logger.FatalFormat(format, args);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Fatal" /> level.
		/// </summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> that supplies culture-specific formatting information</param>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.FatalExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Fatal(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsFatalEnabled" />
		public static void FatalFormatExt(this ILog logger, IFormatProvider provider, string format, params object[] args)
		{
			try
			{
				if (logger.IsFatalEnabled)
				{
					logger.FatalFormat(provider, format, args);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Fatal" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.FatalExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Fatal(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsFatalEnabled" />
		public static void FatalFormatExt(this ILog logger, string format, object arg0, object arg1)
		{
			try
			{
				if (logger.IsFatalEnabled)
				{
					logger.FatalFormat(format, arg0, arg1);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <see cref="F:log4net.Core.Level.Fatal" /> level.
		/// </summary>
		/// <param name="logger">The logger on which the message is logged.</param>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="arg0">An Object to format</param>
		/// <param name="arg1">An Object to format</param>
		/// <param name="arg2">An Object to format</param>
		/// <remarks>
		/// <para>
		/// The message is formatted using the <c>String.Format</c> method. See
		/// <see cref="M:System.String.Format(System.String,System.Object[])" /> for details of the syntax of the format string and the behavior
		/// of the formatting.
		/// </para>
		/// <para>
		/// This method does not take an <see cref="T:System.Exception" /> object to include in the
		/// log event. To pass an <see cref="T:System.Exception" /> use one of the <see cref="M:log4net.Util.ILogExtensions.FatalExt(log4net.ILog,System.Object,System.Exception)" />
		/// methods instead.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:log4net.ILog.Fatal(System.Object)" />
		/// <seealso cref="P:log4net.ILog.IsFatalEnabled" />
		public static void FatalFormatExt(this ILog logger, string format, object arg0, object arg1, object arg2)
		{
			try
			{
				if (logger.IsFatalEnabled)
				{
					logger.FatalFormat(format, arg0, arg1, arg2);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Exception while logging", exception);
			}
		}
	}
}
