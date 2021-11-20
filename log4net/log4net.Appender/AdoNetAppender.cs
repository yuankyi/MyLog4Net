using log4net.Core;
using log4net.Util;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;

namespace log4net.Appender
{
	/// <summary>
	/// Appender that logs to a database.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="T:log4net.Appender.AdoNetAppender" /> appends logging events to a table within a
	/// database. The appender can be configured to specify the connection 
	/// string by setting the <see cref="P:log4net.Appender.AdoNetAppender.ConnectionString" /> property. 
	/// The connection type (provider) can be specified by setting the <see cref="P:log4net.Appender.AdoNetAppender.ConnectionType" />
	/// property. For more information on database connection strings for
	/// your specific database see <a href="http://www.connectionstrings.com/">http://www.connectionstrings.com/</a>.
	/// </para>
	/// <para>
	/// Records are written into the database either using a prepared
	/// statement or a stored procedure. The <see cref="P:log4net.Appender.AdoNetAppender.CommandType" /> property
	/// is set to <see cref="F:System.Data.CommandType.Text" /> (<c>System.Data.CommandType.Text</c>) to specify a prepared statement
	/// or to <see cref="F:System.Data.CommandType.StoredProcedure" /> (<c>System.Data.CommandType.StoredProcedure</c>) to specify a stored
	/// procedure.
	/// </para>
	/// <para>
	/// The prepared statement text or the name of the stored procedure
	/// must be set in the <see cref="P:log4net.Appender.AdoNetAppender.CommandText" /> property.
	/// </para>
	/// <para>
	/// The prepared statement or stored procedure can take a number
	/// of parameters. Parameters are added using the <see cref="M:log4net.Appender.AdoNetAppender.AddParameter(log4net.Appender.AdoNetAppenderParameter)" />
	/// method. This adds a single <see cref="T:log4net.Appender.AdoNetAppenderParameter" /> to the
	/// ordered list of parameters. The <see cref="T:log4net.Appender.AdoNetAppenderParameter" />
	/// type may be subclassed if required to provide database specific
	/// functionality. The <see cref="T:log4net.Appender.AdoNetAppenderParameter" /> specifies
	/// the parameter name, database type, size, and how the value should
	/// be generated using a <see cref="T:log4net.Layout.ILayout" />.
	/// </para>
	/// </remarks>
	/// <example>
	/// An example of a SQL Server table that could be logged to:
	/// <code lang="SQL">
	/// CREATE TABLE [dbo].[Log] ( 
	///   [ID] [int] IDENTITY (1, 1) NOT NULL ,
	///   [Date] [datetime] NOT NULL ,
	///   [Thread] [varchar] (255) NOT NULL ,
	///   [Level] [varchar] (20) NOT NULL ,
	///   [Logger] [varchar] (255) NOT NULL ,
	///   [Message] [varchar] (4000) NOT NULL 
	/// ) ON [PRIMARY]
	/// </code>
	/// </example>
	/// <example>
	/// An example configuration to log to the above table:
	/// <code lang="XML" escaped="true">
	/// <appender name="AdoNetAppender_SqlServer" type="log4net.Appender.AdoNetAppender">
	///   <connectionType value="System.Data.SqlClient.SqlConnection, System.Data, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
	///   <connectionString value="data source=SQLSVR;initial catalog=test_log4net;integrated security=false;persist security info=True;User ID=sa;Password=sa" />
	///   <commandText value="INSERT INTO Log ([Date],[Thread],[Level],[Logger],[Message]) VALUES (@log_date, @thread, @log_level, @logger, @message)" />
	///   <parameter>
	///     <parameterName value="@log_date" />
	///     <dbType value="DateTime" />
	///     <layout type="log4net.Layout.PatternLayout" value="%date{yyyy'-'MM'-'dd HH':'mm':'ss'.'fff}" />
	///   </parameter>
	///   <parameter>
	///     <parameterName value="@thread" />
	///     <dbType value="String" />
	///     <size value="255" />
	///     <layout type="log4net.Layout.PatternLayout" value="%thread" />
	///   </parameter>
	///   <parameter>
	///     <parameterName value="@log_level" />
	///     <dbType value="String" />
	///     <size value="50" />
	///     <layout type="log4net.Layout.PatternLayout" value="%level" />
	///   </parameter>
	///   <parameter>
	///     <parameterName value="@logger" />
	///     <dbType value="String" />
	///     <size value="255" />
	///     <layout type="log4net.Layout.PatternLayout" value="%logger" />
	///   </parameter>
	///   <parameter>
	///     <parameterName value="@message" />
	///     <dbType value="String" />
	///     <size value="4000" />
	///     <layout type="log4net.Layout.PatternLayout" value="%message" />
	///   </parameter>
	/// </appender>
	/// </code>
	/// </example>
	/// <author>Julian Biddle</author>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	/// <author>Lance Nehring</author>
	public class AdoNetAppender : BufferingAppenderSkeleton
	{
		/// <summary>
		/// The list of <see cref="T:log4net.Appender.AdoNetAppenderParameter" /> objects.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The list of <see cref="T:log4net.Appender.AdoNetAppenderParameter" /> objects.
		/// </para>
		/// </remarks>
		protected ArrayList m_parameters;

		/// <summary>
		/// The security context to use for privileged calls
		/// </summary>
		private SecurityContext m_securityContext;

		/// <summary>
		/// The <see cref="T:System.Data.IDbConnection" /> that will be used
		/// to insert logging events into a database.
		/// </summary>
		private IDbConnection m_dbConnection;

		/// <summary>
		/// Database connection string.
		/// </summary>
		private string m_connectionString;

		/// <summary>
		/// The appSettings key from App.Config that contains the connection string.
		/// </summary>
		private string m_appSettingsKey;

		/// <summary>
		/// The connectionStrings key from App.Config that contains the connection string.
		/// </summary>
		private string m_connectionStringName;

		/// <summary>
		/// String type name of the <see cref="T:System.Data.IDbConnection" /> type name.
		/// </summary>
		private string m_connectionType;

		/// <summary>
		/// The text of the command.
		/// </summary>
		private string m_commandText;

		/// <summary>
		/// The command type.
		/// </summary>
		private CommandType m_commandType;

		/// <summary>
		/// Indicates whether to use transactions when writing to the database.
		/// </summary>
		private bool m_useTransactions;

		/// <summary>
		/// Indicates whether to reconnect when a connection is lost.
		/// </summary>
		private bool m_reconnectOnError;

		/// <summary>
		/// The fully qualified type of the AdoNetAppender class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(AdoNetAppender);

		/// <summary>
		/// Gets or sets the database connection string that is used to connect to 
		/// the database.
		/// </summary>
		/// <value>
		/// The database connection string used to connect to the database.
		/// </value>
		/// <remarks>
		/// <para>
		/// The connections string is specific to the connection type.
		/// See <see cref="P:log4net.Appender.AdoNetAppender.ConnectionType" /> for more information.
		/// </para>
		/// </remarks>
		/// <example>Connection string for MS Access via ODBC:
		/// <code>"DSN=MS Access Database;UID=admin;PWD=;SystemDB=C:\data\System.mdw;SafeTransactions = 0;FIL=MS Access;DriverID = 25;DBQ=C:\data\train33.mdb"</code>
		/// </example>
		/// <example>Another connection string for MS Access via ODBC:
		/// <code>"Driver={Microsoft Access Driver (*.mdb)};DBQ=C:\Work\cvs_root\log4net-1.2\access.mdb;UID=;PWD=;"</code>
		/// </example>
		/// <example>Connection string for MS Access via OLE DB:
		/// <code>"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Work\cvs_root\log4net-1.2\access.mdb;User Id=;Password=;"</code>
		/// </example>
		public string ConnectionString
		{
			get
			{
				return m_connectionString;
			}
			set
			{
				m_connectionString = value;
			}
		}

		/// <summary>
		/// The appSettings key from App.Config that contains the connection string.
		/// </summary>
		public string AppSettingsKey
		{
			get
			{
				return m_appSettingsKey;
			}
			set
			{
				m_appSettingsKey = value;
			}
		}

		/// <summary>
		/// The connectionStrings key from App.Config that contains the connection string.
		/// </summary>
		/// <remarks>
		/// This property requires at least .NET 2.0.
		/// </remarks>
		public string ConnectionStringName
		{
			get
			{
				return m_connectionStringName;
			}
			set
			{
				m_connectionStringName = value;
			}
		}

		/// <summary>
		/// Gets or sets the type name of the <see cref="T:System.Data.IDbConnection" /> connection
		/// that should be created.
		/// </summary>
		/// <value>
		/// The type name of the <see cref="T:System.Data.IDbConnection" /> connection.
		/// </value>
		/// <remarks>
		/// <para>
		/// The type name of the ADO.NET provider to use.
		/// </para>
		/// <para>
		/// The default is to use the OLE DB provider.
		/// </para>
		/// </remarks>
		/// <example>Use the OLE DB Provider. This is the default value.
		/// <code>System.Data.OleDb.OleDbConnection, System.Data, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</code>
		/// </example>
		/// <example>Use the MS SQL Server Provider. 
		/// <code>System.Data.SqlClient.SqlConnection, System.Data, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</code>
		/// </example>
		/// <example>Use the ODBC Provider. 
		/// <code>Microsoft.Data.Odbc.OdbcConnection,Microsoft.Data.Odbc,version=1.0.3300.0,publicKeyToken=b77a5c561934e089,culture=neutral</code>
		/// This is an optional package that you can download from 
		/// <a href="http://msdn.microsoft.com/downloads">http://msdn.microsoft.com/downloads</a> 
		/// search for <b>ODBC .NET Data Provider</b>.
		/// </example>
		/// <example>Use the Oracle Provider. 
		/// <code>System.Data.OracleClient.OracleConnection, System.Data.OracleClient, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</code>
		/// This is an optional package that you can download from 
		/// <a href="http://msdn.microsoft.com/downloads">http://msdn.microsoft.com/downloads</a> 
		/// search for <b>.NET Managed Provider for Oracle</b>.
		/// </example>
		public string ConnectionType
		{
			get
			{
				return m_connectionType;
			}
			set
			{
				m_connectionType = value;
			}
		}

		/// <summary>
		/// Gets or sets the command text that is used to insert logging events
		/// into the database.
		/// </summary>
		/// <value>
		/// The command text used to insert logging events into the database.
		/// </value>
		/// <remarks>
		/// <para>
		/// Either the text of the prepared statement or the
		/// name of the stored procedure to execute to write into
		/// the database.
		/// </para>
		/// <para>
		/// The <see cref="P:log4net.Appender.AdoNetAppender.CommandType" /> property determines if
		/// this text is a prepared statement or a stored procedure.
		/// </para>
		/// <para>
		/// If this property is not set, the command text is retrieved by invoking
		/// <see cref="M:log4net.Appender.AdoNetAppender.GetLogStatement(log4net.Core.LoggingEvent)" />.
		/// </para>
		/// </remarks>
		public string CommandText
		{
			get
			{
				return m_commandText;
			}
			set
			{
				m_commandText = value;
			}
		}

		/// <summary>
		/// Gets or sets the command type to execute.
		/// </summary>
		/// <value>
		/// The command type to execute.
		/// </value>
		/// <remarks>
		/// <para>
		/// This value may be either <see cref="F:System.Data.CommandType.Text" /> (<c>System.Data.CommandType.Text</c>) to specify
		/// that the <see cref="P:log4net.Appender.AdoNetAppender.CommandText" /> is a prepared statement to execute, 
		/// or <see cref="F:System.Data.CommandType.StoredProcedure" /> (<c>System.Data.CommandType.StoredProcedure</c>) to specify that the
		/// <see cref="P:log4net.Appender.AdoNetAppender.CommandText" /> property is the name of a stored procedure
		/// to execute.
		/// </para>
		/// <para>
		/// The default value is <see cref="F:System.Data.CommandType.Text" /> (<c>System.Data.CommandType.Text</c>).
		/// </para>
		/// </remarks>
		public CommandType CommandType
		{
			get
			{
				return m_commandType;
			}
			set
			{
				m_commandType = value;
			}
		}

		/// <summary>
		/// Should transactions be used to insert logging events in the database.
		/// </summary>
		/// <value>
		/// <c>true</c> if transactions should be used to insert logging events in
		/// the database, otherwise <c>false</c>. The default value is <c>true</c>.
		/// </value>
		/// <remarks>
		/// <para>
		/// Gets or sets a value that indicates whether transactions should be used
		/// to insert logging events in the database.
		/// </para>
		/// <para>
		/// When set a single transaction will be used to insert the buffered events
		/// into the database. Otherwise each event will be inserted without using
		/// an explicit transaction.
		/// </para>
		/// </remarks>
		public bool UseTransactions
		{
			get
			{
				return m_useTransactions;
			}
			set
			{
				m_useTransactions = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="P:log4net.Appender.AdoNetAppender.SecurityContext" /> used to call the NetSend method.
		/// </summary>
		/// <value>
		/// The <see cref="P:log4net.Appender.AdoNetAppender.SecurityContext" /> used to call the NetSend method.
		/// </value>
		/// <remarks>
		/// <para>
		/// Unless a <see cref="P:log4net.Appender.AdoNetAppender.SecurityContext" /> specified here for this appender
		/// the <see cref="P:log4net.Core.SecurityContextProvider.DefaultProvider" /> is queried for the
		/// security context to use. The default behavior is to use the security context
		/// of the current thread.
		/// </para>
		/// </remarks>
		public SecurityContext SecurityContext
		{
			get
			{
				return m_securityContext;
			}
			set
			{
				m_securityContext = value;
			}
		}

		/// <summary>
		/// Should this appender try to reconnect to the database on error.
		/// </summary>
		/// <value>
		/// <c>true</c> if the appender should try to reconnect to the database after an
		/// error has occurred, otherwise <c>false</c>. The default value is <c>false</c>, 
		/// i.e. not to try to reconnect.
		/// </value>
		/// <remarks>
		/// <para>
		/// The default behaviour is for the appender not to try to reconnect to the
		/// database if an error occurs. Subsequent logging events are discarded.
		/// </para>
		/// <para>
		/// To force the appender to attempt to reconnect to the database set this
		/// property to <c>true</c>.
		/// </para>
		/// <note>
		/// When the appender attempts to connect to the database there may be a
		/// delay of up to the connection timeout specified in the connection string.
		/// This delay will block the calling application's thread. 
		/// Until the connection can be reestablished this potential delay may occur multiple times.
		/// </note>
		/// </remarks>
		public bool ReconnectOnError
		{
			get
			{
				return m_reconnectOnError;
			}
			set
			{
				m_reconnectOnError = value;
			}
		}

		/// <summary>
		/// Gets or sets the underlying <see cref="T:System.Data.IDbConnection" />.
		/// </summary>
		/// <value>
		/// The underlying <see cref="T:System.Data.IDbConnection" />.
		/// </value>
		/// <remarks>
		/// <see cref="T:log4net.Appender.AdoNetAppender" /> creates a <see cref="T:System.Data.IDbConnection" /> to insert 
		/// logging events into a database.  Classes deriving from <see cref="T:log4net.Appender.AdoNetAppender" /> 
		/// can use this property to get or set this <see cref="T:System.Data.IDbConnection" />.  Use the 
		/// underlying <see cref="T:System.Data.IDbConnection" /> returned from <see cref="P:log4net.Appender.AdoNetAppender.Connection" /> if 
		/// you require access beyond that which <see cref="T:log4net.Appender.AdoNetAppender" /> provides.
		/// </remarks>
		protected IDbConnection Connection
		{
			get
			{
				return m_dbConnection;
			}
			set
			{
				m_dbConnection = value;
			}
		}

		/// <summary> 
		/// Initializes a new instance of the <see cref="T:log4net.Appender.AdoNetAppender" /> class.
		/// </summary>
		/// <remarks>
		/// Public default constructor to initialize a new instance of this class.
		/// </remarks>
		public AdoNetAppender()
		{
			ConnectionType = "System.Data.OleDb.OleDbConnection, System.Data, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
			UseTransactions = true;
			CommandType = CommandType.Text;
			m_parameters = new ArrayList();
			ReconnectOnError = false;
		}

		/// <summary>
		/// Initialize the appender based on the options set
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is part of the <see cref="T:log4net.Core.IOptionHandler" /> delayed object
		/// activation scheme. The <see cref="M:log4net.Appender.AdoNetAppender.ActivateOptions" /> method must 
		/// be called on this object after the configuration properties have
		/// been set. Until <see cref="M:log4net.Appender.AdoNetAppender.ActivateOptions" /> is called this
		/// object is in an undefined state and must not be used. 
		/// </para>
		/// <para>
		/// If any of the configuration properties are modified then 
		/// <see cref="M:log4net.Appender.AdoNetAppender.ActivateOptions" /> must be called again.
		/// </para>
		/// </remarks>
		public override void ActivateOptions()
		{
			base.ActivateOptions();
			if (SecurityContext == null)
			{
				SecurityContext = SecurityContextProvider.DefaultProvider.CreateSecurityContext(this);
			}
			InitializeDatabaseConnection();
		}

		/// <summary>
		/// Override the parent method to close the database
		/// </summary>
		/// <remarks>
		/// <para>
		/// Closes the database command and database connection.
		/// </para>
		/// </remarks>
		protected override void OnClose()
		{
			base.OnClose();
			DiposeConnection();
		}

		/// <summary>
		/// Inserts the events into the database.
		/// </summary>
		/// <param name="events">The events to insert into the database.</param>
		/// <remarks>
		/// <para>
		/// Insert all the events specified in the <paramref name="events" />
		/// array into the database.
		/// </para>
		/// </remarks>
		protected override void SendBuffer(LoggingEvent[] events)
		{
			if (ReconnectOnError && (Connection == null || Connection.State != ConnectionState.Open))
			{
				LogLog.Debug(declaringType, "Attempting to reconnect to database. Current Connection State: " + ((Connection == null) ? SystemInfo.NullText : Connection.State.ToString()));
				InitializeDatabaseConnection();
			}
			if (Connection != null && Connection.State == ConnectionState.Open)
			{
				if (UseTransactions)
				{
					using (IDbTransaction dbTransaction = Connection.BeginTransaction())
					{
						try
						{
							SendBuffer(dbTransaction, events);
							dbTransaction.Commit();
						}
						catch (Exception e)
						{
							try
							{
								dbTransaction.Rollback();
							}
							catch (Exception)
							{
							}
							ErrorHandler.Error("Exception while writing to database", e);
						}
					}
				}
				else
				{
					SendBuffer(null, events);
				}
			}
		}

		/// <summary>
		/// Adds a parameter to the command.
		/// </summary>
		/// <param name="parameter">The parameter to add to the command.</param>
		/// <remarks>
		/// <para>
		/// Adds a parameter to the ordered list of command parameters.
		/// </para>
		/// </remarks>
		public void AddParameter(AdoNetAppenderParameter parameter)
		{
			m_parameters.Add(parameter);
		}

		/// <summary>
		/// Writes the events to the database using the transaction specified.
		/// </summary>
		/// <param name="dbTran">The transaction that the events will be executed under.</param>
		/// <param name="events">The array of events to insert into the database.</param>
		/// <remarks>
		/// <para>
		/// The transaction argument can be <c>null</c> if the appender has been
		/// configured not to use transactions. See <see cref="P:log4net.Appender.AdoNetAppender.UseTransactions" />
		/// property for more information.
		/// </para>
		/// </remarks>
		protected virtual void SendBuffer(IDbTransaction dbTran, LoggingEvent[] events)
		{
			if (CommandText != null && CommandText.Trim() != "")
			{
				using (IDbCommand dbCommand = Connection.CreateCommand())
				{
					dbCommand.CommandText = CommandText;
					dbCommand.CommandType = CommandType;
					if (dbTran != null)
					{
						dbCommand.Transaction = dbTran;
					}
					dbCommand.Prepare();
					foreach (LoggingEvent loggingEvent in events)
					{
						dbCommand.Parameters.Clear();
						foreach (AdoNetAppenderParameter parameter in m_parameters)
						{
							parameter.Prepare(dbCommand);
							parameter.FormatValue(dbCommand, loggingEvent);
						}
						dbCommand.ExecuteNonQuery();
					}
				}
			}
			else
			{
				using (IDbCommand dbCommand2 = Connection.CreateCommand())
				{
					if (dbTran != null)
					{
						dbCommand2.Transaction = dbTran;
					}
					foreach (LoggingEvent logEvent in events)
					{
						string logStatement = GetLogStatement(logEvent);
						LogLog.Debug(declaringType, "LogStatement [" + logStatement + "]");
						dbCommand2.CommandText = logStatement;
						dbCommand2.ExecuteNonQuery();
					}
				}
			}
		}

		/// <summary>
		/// Formats the log message into database statement text.
		/// </summary>
		/// <param name="logEvent">The event being logged.</param>
		/// <remarks>
		/// This method can be overridden by subclasses to provide 
		/// more control over the format of the database statement.
		/// </remarks>
		/// <returns>
		/// Text that can be passed to a <see cref="T:System.Data.IDbCommand" />.
		/// </returns>
		protected virtual string GetLogStatement(LoggingEvent logEvent)
		{
			if (Layout == null)
			{
				ErrorHandler.Error("AdoNetAppender: No Layout specified.");
				return "";
			}
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				Layout.Format(stringWriter, logEvent);
				return stringWriter.ToString();
			}
		}

		/// <summary>
		/// Creates an <see cref="T:System.Data.IDbConnection" /> instance used to connect to the database.
		/// </summary>
		/// <remarks>
		/// This method is called whenever a new IDbConnection is needed (i.e. when a reconnect is necessary).
		/// </remarks>
		/// <param name="connectionType">The <see cref="T:System.Type" /> of the <see cref="T:System.Data.IDbConnection" /> object.</param>
		/// <param name="connectionString">The connectionString output from the ResolveConnectionString method.</param>
		/// <returns>An <see cref="T:System.Data.IDbConnection" /> instance with a valid connection string.</returns>
		protected virtual IDbConnection CreateConnection(Type connectionType, string connectionString)
		{
			IDbConnection obj = (IDbConnection)Activator.CreateInstance(connectionType);
			obj.ConnectionString = connectionString;
			return obj;
		}

		/// <summary>
		/// Resolves the connection string from the ConnectionString, ConnectionStringName, or AppSettingsKey
		/// property.
		/// </summary>
		/// <remarks>
		/// ConnectiongStringName is only supported on .NET 2.0 and higher.
		/// </remarks>
		/// <param name="connectionStringContext">Additional information describing the connection string.</param>
		/// <returns>A connection string used to connect to the database.</returns>
		protected virtual string ResolveConnectionString(out string connectionStringContext)
		{
			if (ConnectionString != null && ConnectionString.Length > 0)
			{
				connectionStringContext = "ConnectionString";
				return ConnectionString;
			}
			if (!string.IsNullOrEmpty(ConnectionStringName))
			{
				ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[ConnectionStringName];
				if (connectionStringSettings != null)
				{
					connectionStringContext = "ConnectionStringName";
					return connectionStringSettings.ConnectionString;
				}
				throw new LogException("Unable to find [" + ConnectionStringName + "] ConfigurationManager.ConnectionStrings item");
			}
			if (AppSettingsKey != null && AppSettingsKey.Length > 0)
			{
				connectionStringContext = "AppSettingsKey";
				string appSetting = SystemInfo.GetAppSetting(AppSettingsKey);
				if (appSetting == null || appSetting.Length == 0)
				{
					throw new LogException("Unable to find [" + AppSettingsKey + "] AppSettings key.");
				}
				return appSetting;
			}
			connectionStringContext = "Unable to resolve connection string from ConnectionString, ConnectionStrings, or AppSettings.";
			return string.Empty;
		}

		/// <summary>
		/// Retrieves the class type of the ADO.NET provider.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Gets the Type of the ADO.NET provider to use to connect to the
		/// database. This method resolves the type specified in the 
		/// <see cref="P:log4net.Appender.AdoNetAppender.ConnectionType" /> property.
		/// </para>
		/// <para>
		/// Subclasses can override this method to return a different type
		/// if necessary.
		/// </para>
		/// </remarks>
		/// <returns>The <see cref="T:System.Type" /> of the ADO.NET provider</returns>
		protected virtual Type ResolveConnectionType()
		{
			try
			{
				return SystemInfo.GetTypeFromString(ConnectionType, true, false);
			}
			catch (Exception e)
			{
				ErrorHandler.Error("Failed to load connection type [" + ConnectionType + "]", e);
				throw;
			}
		}

		/// <summary>
		/// Connects to the database.
		/// </summary>		
		private void InitializeDatabaseConnection()
		{
			string connectionStringContext = "Unable to determine connection string context.";
			string text = string.Empty;
			try
			{
				DiposeConnection();
				text = ResolveConnectionString(out connectionStringContext);
				Connection = CreateConnection(ResolveConnectionType(), text);
				using (SecurityContext.Impersonate(this))
				{
					Connection.Open();
				}
			}
			catch (Exception e)
			{
				ErrorHandler.Error("Could not open database connection [" + text + "]. Connection string context [" + connectionStringContext + "].", e);
				Connection = null;
			}
		}

		/// <summary>
		/// Cleanup the existing connection.
		/// </summary>
		/// <remarks>
		/// Calls the IDbConnection's <see cref="M:System.Data.IDbConnection.Close" /> method.
		/// </remarks>
		private void DiposeConnection()
		{
			if (Connection != null)
			{
				try
				{
					Connection.Close();
				}
				catch (Exception exception)
				{
					LogLog.Warn(declaringType, "Exception while disposing cached connection object", exception);
				}
				Connection = null;
			}
		}
	}
}
