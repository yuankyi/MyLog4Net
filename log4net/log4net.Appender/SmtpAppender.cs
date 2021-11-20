using log4net.Core;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace log4net.Appender
{
	/// <summary>
	/// Send an e-mail when a specific logging event occurs, typically on errors 
	/// or fatal errors.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The number of logging events delivered in this e-mail depend on
	/// the value of <see cref="P:log4net.Appender.BufferingAppenderSkeleton.BufferSize" /> option. The
	/// <see cref="T:log4net.Appender.SmtpAppender" /> keeps only the last
	/// <see cref="P:log4net.Appender.BufferingAppenderSkeleton.BufferSize" /> logging events in its 
	/// cyclic buffer. This keeps memory requirements at a reasonable level while 
	/// still delivering useful application context.
	/// </para>
	/// <note type="caution">
	/// Authentication and setting the server Port are only available on the MS .NET 1.1 runtime.
	/// For these features to be enabled you need to ensure that you are using a version of
	/// the log4net assembly that is built against the MS .NET 1.1 framework and that you are
	/// running the your application on the MS .NET 1.1 runtime. On all other platforms only sending
	/// unauthenticated messages to a server listening on port 25 (the default) is supported.
	/// </note>
	/// <para>
	/// Authentication is supported by setting the <see cref="P:log4net.Appender.SmtpAppender.Authentication" /> property to
	/// either <see cref="F:log4net.Appender.SmtpAppender.SmtpAuthentication.Basic" /> or <see cref="F:log4net.Appender.SmtpAppender.SmtpAuthentication.Ntlm" />.
	/// If using <see cref="F:log4net.Appender.SmtpAppender.SmtpAuthentication.Basic" /> authentication then the <see cref="P:log4net.Appender.SmtpAppender.Username" />
	/// and <see cref="P:log4net.Appender.SmtpAppender.Password" /> properties must also be set.
	/// </para>
	/// <para>
	/// To set the SMTP server port use the <see cref="P:log4net.Appender.SmtpAppender.Port" /> property. The default port is 25.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	public class SmtpAppender : BufferingAppenderSkeleton
	{
		/// <summary>
		/// Values for the <see cref="P:log4net.Appender.SmtpAppender.Authentication" /> property.
		/// </summary>
		/// <remarks>
		/// <para>
		/// SMTP authentication modes.
		/// </para>
		/// </remarks>
		public enum SmtpAuthentication
		{
			/// <summary>
			/// No authentication
			/// </summary>
			None,
			/// <summary>
			/// Basic authentication.
			/// </summary>
			/// <remarks>
			/// Requires a username and password to be supplied
			/// </remarks>
			Basic,
			/// <summary>
			/// Integrated authentication
			/// </summary>
			/// <remarks>
			/// Uses the Windows credentials from the current thread or process to authenticate.
			/// </remarks>
			Ntlm
		}

		private string m_to;

		private string m_cc;

		private string m_bcc;

		private string m_from;

		private string m_subject;

		private string m_smtpHost;

		private Encoding m_subjectEncoding = Encoding.UTF8;

		private Encoding m_bodyEncoding = Encoding.UTF8;

		private SmtpAuthentication m_authentication;

		private string m_username;

		private string m_password;

		private int m_port = 25;

		private MailPriority m_mailPriority;

		private bool m_enableSsl;

		private string m_replyTo;

		private static readonly char[] ADDRESS_DELIMITERS = new char[2]
		{
			',',
			';'
		};

		/// <summary>
		/// Gets or sets a comma- or semicolon-delimited list of recipient e-mail addresses (use semicolon on .NET 1.1 and comma for later versions).
		/// </summary>
		/// <value>
		/// <para>
		/// For .NET 1.1 (System.Web.Mail): A semicolon-delimited list of e-mail addresses.
		/// </para>
		/// <para>
		/// For .NET 2.0 (System.Net.Mail): A comma-delimited list of e-mail addresses.
		/// </para>
		/// </value>
		/// <remarks>
		/// <para>
		/// For .NET 1.1 (System.Web.Mail): A semicolon-delimited list of e-mail addresses.
		/// </para>
		/// <para>
		/// For .NET 2.0 (System.Net.Mail): A comma-delimited list of e-mail addresses.
		/// </para>
		/// </remarks>
		public string To
		{
			get
			{
				return m_to;
			}
			set
			{
				m_to = MaybeTrimSeparators(value);
			}
		}

		/// <summary>
		/// Gets or sets a comma- or semicolon-delimited list of recipient e-mail addresses 
		/// that will be carbon copied (use semicolon on .NET 1.1 and comma for later versions).
		/// </summary>
		/// <value>
		/// <para>
		/// For .NET 1.1 (System.Web.Mail): A semicolon-delimited list of e-mail addresses.
		/// </para>
		/// <para>
		/// For .NET 2.0 (System.Net.Mail): A comma-delimited list of e-mail addresses.
		/// </para>
		/// </value>
		/// <remarks>
		/// <para>
		/// For .NET 1.1 (System.Web.Mail): A semicolon-delimited list of e-mail addresses.
		/// </para>
		/// <para>
		/// For .NET 2.0 (System.Net.Mail): A comma-delimited list of e-mail addresses.
		/// </para>
		/// </remarks>
		public string Cc
		{
			get
			{
				return m_cc;
			}
			set
			{
				m_cc = MaybeTrimSeparators(value);
			}
		}

		/// <summary>
		/// Gets or sets a semicolon-delimited list of recipient e-mail addresses
		/// that will be blind carbon copied.
		/// </summary>
		/// <value>
		/// A semicolon-delimited list of e-mail addresses.
		/// </value>
		/// <remarks>
		/// <para>
		/// A semicolon-delimited list of recipient e-mail addresses.
		/// </para>
		/// </remarks>
		public string Bcc
		{
			get
			{
				return m_bcc;
			}
			set
			{
				m_bcc = MaybeTrimSeparators(value);
			}
		}

		/// <summary>
		/// Gets or sets the e-mail address of the sender.
		/// </summary>
		/// <value>
		/// The e-mail address of the sender.
		/// </value>
		/// <remarks>
		/// <para>
		/// The e-mail address of the sender.
		/// </para>
		/// </remarks>
		public string From
		{
			get
			{
				return m_from;
			}
			set
			{
				m_from = value;
			}
		}

		/// <summary>
		/// Gets or sets the subject line of the e-mail message.
		/// </summary>
		/// <value>
		/// The subject line of the e-mail message.
		/// </value>
		/// <remarks>
		/// <para>
		/// The subject line of the e-mail message.
		/// </para>
		/// </remarks>
		public string Subject
		{
			get
			{
				return m_subject;
			}
			set
			{
				m_subject = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the SMTP relay mail server to use to send 
		/// the e-mail messages.
		/// </summary>
		/// <value>
		/// The name of the e-mail relay server. If SmtpServer is not set, the 
		/// name of the local SMTP server is used.
		/// </value>
		/// <remarks>
		/// <para>
		/// The name of the e-mail relay server. If SmtpServer is not set, the 
		/// name of the local SMTP server is used.
		/// </para>
		/// </remarks>
		public string SmtpHost
		{
			get
			{
				return m_smtpHost;
			}
			set
			{
				m_smtpHost = value;
			}
		}

		/// <summary>
		/// Obsolete
		/// </summary>
		/// <remarks>
		/// Use the BufferingAppenderSkeleton Fix methods instead 
		/// </remarks>
		/// <remarks>
		/// <para>
		/// Obsolete property.
		/// </para>
		/// </remarks>
		[Obsolete("Use the BufferingAppenderSkeleton Fix methods")]
		public bool LocationInfo
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		/// <summary>
		/// The mode to use to authentication with the SMTP server
		/// </summary>
		/// <remarks>
		/// <note type="caution">Authentication is only available on the MS .NET 1.1 runtime.</note>
		/// <para>
		/// Valid Authentication mode values are: <see cref="F:log4net.Appender.SmtpAppender.SmtpAuthentication.None" />, 
		/// <see cref="F:log4net.Appender.SmtpAppender.SmtpAuthentication.Basic" />, and <see cref="F:log4net.Appender.SmtpAppender.SmtpAuthentication.Ntlm" />. 
		/// The default value is <see cref="F:log4net.Appender.SmtpAppender.SmtpAuthentication.None" />. When using 
		/// <see cref="F:log4net.Appender.SmtpAppender.SmtpAuthentication.Basic" /> you must specify the <see cref="P:log4net.Appender.SmtpAppender.Username" /> 
		/// and <see cref="P:log4net.Appender.SmtpAppender.Password" /> to use to authenticate.
		/// When using <see cref="F:log4net.Appender.SmtpAppender.SmtpAuthentication.Ntlm" /> the Windows credentials for the current
		/// thread, if impersonating, or the process will be used to authenticate. 
		/// </para>
		/// </remarks>
		public SmtpAuthentication Authentication
		{
			get
			{
				return m_authentication;
			}
			set
			{
				m_authentication = value;
			}
		}

		/// <summary>
		/// The username to use to authenticate with the SMTP server
		/// </summary>
		/// <remarks>
		/// <note type="caution">Authentication is only available on the MS .NET 1.1 runtime.</note>
		/// <para>
		/// A <see cref="P:log4net.Appender.SmtpAppender.Username" /> and <see cref="P:log4net.Appender.SmtpAppender.Password" /> must be specified when 
		/// <see cref="P:log4net.Appender.SmtpAppender.Authentication" /> is set to <see cref="F:log4net.Appender.SmtpAppender.SmtpAuthentication.Basic" />, 
		/// otherwise the username will be ignored. 
		/// </para>
		/// </remarks>
		public string Username
		{
			get
			{
				return m_username;
			}
			set
			{
				m_username = value;
			}
		}

		/// <summary>
		/// The password to use to authenticate with the SMTP server
		/// </summary>
		/// <remarks>
		/// <note type="caution">Authentication is only available on the MS .NET 1.1 runtime.</note>
		/// <para>
		/// A <see cref="P:log4net.Appender.SmtpAppender.Username" /> and <see cref="P:log4net.Appender.SmtpAppender.Password" /> must be specified when 
		/// <see cref="P:log4net.Appender.SmtpAppender.Authentication" /> is set to <see cref="F:log4net.Appender.SmtpAppender.SmtpAuthentication.Basic" />, 
		/// otherwise the password will be ignored. 
		/// </para>
		/// </remarks>
		public string Password
		{
			get
			{
				return m_password;
			}
			set
			{
				m_password = value;
			}
		}

		/// <summary>
		/// The port on which the SMTP server is listening
		/// </summary>
		/// <remarks>
		/// <note type="caution">Server Port is only available on the MS .NET 1.1 runtime.</note>
		/// <para>
		/// The port on which the SMTP server is listening. The default
		/// port is <c>25</c>. The Port can only be changed when running on
		/// the MS .NET 1.1 runtime.
		/// </para>
		/// </remarks>
		public int Port
		{
			get
			{
				return m_port;
			}
			set
			{
				m_port = value;
			}
		}

		/// <summary>
		/// Gets or sets the priority of the e-mail message
		/// </summary>
		/// <value>
		/// One of the <see cref="T:System.Net.Mail.MailPriority" /> values.
		/// </value>
		/// <remarks>
		/// <para>
		/// Sets the priority of the e-mails generated by this
		/// appender. The default priority is <see cref="F:System.Net.Mail.MailPriority.Normal" />.
		/// </para>
		/// <para>
		/// If you are using this appender to report errors then
		/// you may want to set the priority to <see cref="F:System.Net.Mail.MailPriority.High" />.
		/// </para>
		/// </remarks>
		public MailPriority Priority
		{
			get
			{
				return m_mailPriority;
			}
			set
			{
				m_mailPriority = value;
			}
		}

		/// <summary>
		/// Enable or disable use of SSL when sending e-mail message
		/// </summary>
		/// <remarks>
		/// This is available on MS .NET 2.0 runtime and higher
		/// </remarks>
		public bool EnableSsl
		{
			get
			{
				return m_enableSsl;
			}
			set
			{
				m_enableSsl = value;
			}
		}

		/// <summary>
		/// Gets or sets the reply-to e-mail address.
		/// </summary>
		/// <remarks>
		/// This is available on MS .NET 2.0 runtime and higher
		/// </remarks>
		public string ReplyTo
		{
			get
			{
				return m_replyTo;
			}
			set
			{
				m_replyTo = value;
			}
		}

		/// <summary>
		/// Gets or sets the subject encoding to be used.
		/// </summary>
		/// <remarks>
		/// The default encoding is the operating system's current ANSI codepage.
		/// </remarks>
		public Encoding SubjectEncoding
		{
			get
			{
				return m_subjectEncoding;
			}
			set
			{
				m_subjectEncoding = value;
			}
		}

		/// <summary>
		/// Gets or sets the body encoding to be used.
		/// </summary>
		/// <remarks>
		/// The default encoding is the operating system's current ANSI codepage.
		/// </remarks>
		public Encoding BodyEncoding
		{
			get
			{
				return m_bodyEncoding;
			}
			set
			{
				m_bodyEncoding = value;
			}
		}

		/// <summary>
		/// This appender requires a <see cref="N:log4net.Layout" /> to be set.
		/// </summary>
		/// <value><c>true</c></value>
		/// <remarks>
		/// <para>
		/// This appender requires a <see cref="N:log4net.Layout" /> to be set.
		/// </para>
		/// </remarks>
		protected override bool RequiresLayout
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Sends the contents of the cyclic buffer as an e-mail message.
		/// </summary>
		/// <param name="events">The logging events to send.</param>
		protected override void SendBuffer(LoggingEvent[] events)
		{
			try
			{
				using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
				{
					string header = Layout.Header;
					if (header != null)
					{
						stringWriter.Write(header);
					}
					for (int i = 0; i < events.Length; i++)
					{
						RenderLoggingEvent(stringWriter, events[i]);
					}
					header = Layout.Footer;
					if (header != null)
					{
						stringWriter.Write(header);
					}
					SendEmail(stringWriter.ToString());
				}
			}
			catch (Exception e)
			{
				ErrorHandler.Error("Error occurred while sending e-mail notification.", e);
			}
		}

		/// <summary>
		/// Send the email message
		/// </summary>
		/// <param name="messageBody">the body text to include in the mail</param>
		protected virtual void SendEmail(string messageBody)
		{
			using (SmtpClient smtpClient = new SmtpClient())
			{
				if (!string.IsNullOrEmpty(m_smtpHost))
				{
					smtpClient.Host = m_smtpHost;
				}
				smtpClient.Port = m_port;
				smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
				smtpClient.EnableSsl = m_enableSsl;
				if (m_authentication == SmtpAuthentication.Basic)
				{
					smtpClient.Credentials = new NetworkCredential(m_username, m_password);
				}
				else if (m_authentication == SmtpAuthentication.Ntlm)
				{
					smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
				}
				using (MailMessage mailMessage = new MailMessage())
				{
					mailMessage.Body = messageBody;
					mailMessage.BodyEncoding = m_bodyEncoding;
					mailMessage.From = new MailAddress(m_from);
					mailMessage.To.Add(m_to);
					if (!string.IsNullOrEmpty(m_cc))
					{
						mailMessage.CC.Add(m_cc);
					}
					if (!string.IsNullOrEmpty(m_bcc))
					{
						mailMessage.Bcc.Add(m_bcc);
					}
					if (!string.IsNullOrEmpty(m_replyTo))
					{
						mailMessage.ReplyToList.Add(new MailAddress(m_replyTo));
					}
					mailMessage.Subject = m_subject;
					mailMessage.SubjectEncoding = m_subjectEncoding;
					mailMessage.Priority = m_mailPriority;
					smtpClient.Send(mailMessage);
				}
			}
		}

		/// <summary>
		///   trims leading and trailing commas or semicolons
		/// </summary>
		private static string MaybeTrimSeparators(string s)
		{
			if (!string.IsNullOrEmpty(s))
			{
				return s.Trim(ADDRESS_DELIMITERS);
			}
			return s;
		}
	}
}
