using log4net.Core;
using System;

namespace log4net.Util
{
	/// <summary>
	/// Implements log4net's default error handling policy which consists 
	/// of emitting a message for the first error in an appender and 
	/// ignoring all subsequent errors.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The error message is processed using the LogLog sub-system by default.
	/// </para>
	/// <para>
	/// This policy aims at protecting an otherwise working application
	/// from being flooded with error messages when logging fails.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	/// <author>Ron Grabowski</author>
	public class OnlyOnceErrorHandler : IErrorHandler
	{
		/// <summary>
		/// The UTC date the error was recorded.
		/// </summary>
		private DateTime m_enabledDateUtc;

		/// <summary>
		/// Flag to indicate if it is the first error
		/// </summary>
		private bool m_firstTime = true;

		/// <summary>
		/// The message recorded during the first error.
		/// </summary>
		private string m_message;

		/// <summary>
		/// The exception recorded during the first error.
		/// </summary>
		private Exception m_exception;

		/// <summary>
		/// The error code recorded during the first error.
		/// </summary>
		private ErrorCode m_errorCode;

		/// <summary>
		/// String to prefix each message with
		/// </summary>
		private readonly string m_prefix;

		/// <summary>
		/// The fully qualified type of the OnlyOnceErrorHandler class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(OnlyOnceErrorHandler);

		/// <summary>
		/// Is error logging enabled
		/// </summary>
		/// <remarks>
		/// <para>
		/// Is error logging enabled. Logging is only enabled for the
		/// first error delivered to the <see cref="T:log4net.Util.OnlyOnceErrorHandler" />.
		/// </para>
		/// </remarks>
		public bool IsEnabled
		{
			get
			{
				return m_firstTime;
			}
		}

		/// <summary>
		/// The date the first error that trigged this error handler occurred, or <see cref="F:System.DateTime.MinValue" /> if it has not been triggered.
		/// </summary>
		public DateTime EnabledDate
		{
			get
			{
				if (m_enabledDateUtc == DateTime.MinValue)
				{
					return DateTime.MinValue;
				}
				return m_enabledDateUtc.ToLocalTime();
			}
		}

		/// <summary>
		/// The UTC date the first error that trigged this error handler occured, or <see cref="F:System.DateTime.MinValue" /> if it has not been triggered.
		/// </summary>
		public DateTime EnabledDateUtc
		{
			get
			{
				return m_enabledDateUtc;
			}
		}

		/// <summary>
		/// The message from the first error that trigged this error handler.
		/// </summary>
		public string ErrorMessage
		{
			get
			{
				return m_message;
			}
		}

		/// <summary>
		/// The exception from the first error that trigged this error handler.
		/// </summary>
		/// <remarks>
		/// May be <see langword="null" />.
		/// </remarks>
		public Exception Exception
		{
			get
			{
				return m_exception;
			}
		}

		/// <summary>
		/// The error code from the first error that trigged this error handler.
		/// </summary>
		/// <remarks>
		/// Defaults to <see cref="F:log4net.Core.ErrorCode.GenericFailure" />
		/// </remarks>
		public ErrorCode ErrorCode
		{
			get
			{
				return m_errorCode;
			}
		}

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <remarks>
		/// <para>
		/// Initializes a new instance of the <see cref="T:log4net.Util.OnlyOnceErrorHandler" /> class.
		/// </para>
		/// </remarks>
		public OnlyOnceErrorHandler()
		{
			m_prefix = "";
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="prefix">The prefix to use for each message.</param>
		/// <remarks>
		/// <para>
		/// Initializes a new instance of the <see cref="T:log4net.Util.OnlyOnceErrorHandler" /> class
		/// with the specified prefix.
		/// </para>
		/// </remarks>
		public OnlyOnceErrorHandler(string prefix)
		{
			m_prefix = prefix;
		}

		/// <summary>
		/// Reset the error handler back to its initial disabled state.
		/// </summary>
		public void Reset()
		{
			m_enabledDateUtc = DateTime.MinValue;
			m_errorCode = ErrorCode.GenericFailure;
			m_exception = null;
			m_message = null;
			m_firstTime = true;
		}

		/// <summary>
		/// Log an Error
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="e">The exception.</param>
		/// <param name="errorCode">The internal error code.</param>
		/// <remarks>
		/// <para>
		/// Invokes <see cref="M:log4net.Util.OnlyOnceErrorHandler.FirstError(System.String,System.Exception,log4net.Core.ErrorCode)" /> if and only if this is the first error or the first error after <see cref="M:log4net.Util.OnlyOnceErrorHandler.Reset" /> has been called.
		/// </para>
		/// </remarks>
		public void Error(string message, Exception e, ErrorCode errorCode)
		{
			if (m_firstTime)
			{
				FirstError(message, e, errorCode);
			}
		}

		/// <summary>
		/// Log the very first error
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="e">The exception.</param>
		/// <param name="errorCode">The internal error code.</param>
		/// <remarks>
		/// <para>
		/// Sends the error information to <see cref="T:log4net.Util.LogLog" />'s Error method.
		/// </para>
		/// </remarks>
		public virtual void FirstError(string message, Exception e, ErrorCode errorCode)
		{
			m_enabledDateUtc = DateTime.UtcNow;
			m_errorCode = errorCode;
			m_exception = e;
			m_message = message;
			m_firstTime = false;
			if (LogLog.InternalDebugging && !LogLog.QuietMode)
			{
				LogLog.Error(declaringType, "[" + m_prefix + "] ErrorCode: " + errorCode.ToString() + ". " + message, e);
			}
		}

		/// <summary>
		/// Log an Error
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <param name="e">The exception.</param>
		/// <remarks>
		/// <para>
		/// Invokes <see cref="M:log4net.Util.OnlyOnceErrorHandler.FirstError(System.String,System.Exception,log4net.Core.ErrorCode)" /> if and only if this is the first error or the first error after <see cref="M:log4net.Util.OnlyOnceErrorHandler.Reset" /> has been called.
		/// </para>
		/// </remarks>
		public void Error(string message, Exception e)
		{
			Error(message, e, ErrorCode.GenericFailure);
		}

		/// <summary>
		/// Log an error
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <remarks>
		/// <para>
		/// Invokes <see cref="M:log4net.Util.OnlyOnceErrorHandler.FirstError(System.String,System.Exception,log4net.Core.ErrorCode)" /> if and only if this is the first error or the first error after <see cref="M:log4net.Util.OnlyOnceErrorHandler.Reset" /> has been called.
		/// </para>
		/// </remarks>
		public void Error(string message)
		{
			Error(message, null, ErrorCode.GenericFailure);
		}
	}
}
