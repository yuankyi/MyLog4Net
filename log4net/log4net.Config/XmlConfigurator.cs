using log4net.Repository;
using log4net.Util;
using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Xml;

namespace log4net.Config
{
	/// <summary>
	/// Use this class to initialize the log4net environment using an Xml tree.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Configures a <see cref="T:log4net.Repository.ILoggerRepository" /> using an Xml tree.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	public sealed class XmlConfigurator
	{
		/// <summary>
		/// Class used to watch config files.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Uses the <see cref="T:System.IO.FileSystemWatcher" /> to monitor
		/// changes to a specified file. Because multiple change notifications
		/// may be raised when the file is modified, a timer is used to
		/// compress the notifications into a single event. The timer
		/// waits for <see cref="F:log4net.Config.XmlConfigurator.ConfigureAndWatchHandler.TimeoutMillis" /> time before delivering
		/// the event notification. If any further <see cref="T:System.IO.FileSystemWatcher" />
		/// change notifications arrive while the timer is waiting it
		/// is reset and waits again for <see cref="F:log4net.Config.XmlConfigurator.ConfigureAndWatchHandler.TimeoutMillis" /> to
		/// elapse.
		/// </para>
		/// </remarks>
		private sealed class ConfigureAndWatchHandler : IDisposable
		{
			/// <summary>
			/// Holds the FileInfo used to configure the XmlConfigurator
			/// </summary>
			private FileInfo m_configFile;

			/// <summary>
			/// Holds the repository being configured.
			/// </summary>
			private ILoggerRepository m_repository;

			/// <summary>
			/// The timer used to compress the notification events.
			/// </summary>
			private Timer m_timer;

			/// <summary>
			/// The default amount of time to wait after receiving notification
			/// before reloading the config file.
			/// </summary>
			private const int TimeoutMillis = 500;

			/// <summary>
			/// Watches file for changes. This object should be disposed when no longer
			/// needed to free system handles on the watched resources.
			/// </summary>
			private FileSystemWatcher m_watcher;

			/// <summary>
			/// Initializes a new instance of the <see cref="T:log4net.Config.XmlConfigurator.ConfigureAndWatchHandler" /> class to
			/// watch a specified config file used to configure a repository.
			/// </summary>
			/// <param name="repository">The repository to configure.</param>
			/// <param name="configFile">The configuration file to watch.</param>
			/// <remarks>
			/// <para>
			/// Initializes a new instance of the <see cref="T:log4net.Config.XmlConfigurator.ConfigureAndWatchHandler" /> class.
			/// </para>
			/// </remarks>
			[SecuritySafeCritical]
			public ConfigureAndWatchHandler(ILoggerRepository repository, FileInfo configFile)
			{
				m_repository = repository;
				m_configFile = configFile;
				m_watcher = new FileSystemWatcher();
				m_watcher.Path = m_configFile.DirectoryName;
				m_watcher.Filter = m_configFile.Name;
				m_watcher.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime);
				m_watcher.Changed += ConfigureAndWatchHandler_OnChanged;
				m_watcher.Created += ConfigureAndWatchHandler_OnChanged;
				m_watcher.Deleted += ConfigureAndWatchHandler_OnChanged;
				m_watcher.Renamed += ConfigureAndWatchHandler_OnRenamed;
				m_watcher.EnableRaisingEvents = true;
				m_timer = new Timer(OnWatchedFileChange, null, -1, -1);
			}

			/// <summary>
			/// Event handler used by <see cref="T:log4net.Config.XmlConfigurator.ConfigureAndWatchHandler" />.
			/// </summary>
			/// <param name="source">The <see cref="T:System.IO.FileSystemWatcher" /> firing the event.</param>
			/// <param name="e">The argument indicates the file that caused the event to be fired.</param>
			/// <remarks>
			/// <para>
			/// This handler reloads the configuration from the file when the event is fired.
			/// </para>
			/// </remarks>
			private void ConfigureAndWatchHandler_OnChanged(object source, FileSystemEventArgs e)
			{
				LogLog.Debug(declaringType, "ConfigureAndWatchHandler: " + e.ChangeType.ToString() + " [" + m_configFile.FullName + "]");
				m_timer.Change(500, -1);
			}

			/// <summary>
			/// Event handler used by <see cref="T:log4net.Config.XmlConfigurator.ConfigureAndWatchHandler" />.
			/// </summary>
			/// <param name="source">The <see cref="T:System.IO.FileSystemWatcher" /> firing the event.</param>
			/// <param name="e">The argument indicates the file that caused the event to be fired.</param>
			/// <remarks>
			/// <para>
			/// This handler reloads the configuration from the file when the event is fired.
			/// </para>
			/// </remarks>
			private void ConfigureAndWatchHandler_OnRenamed(object source, RenamedEventArgs e)
			{
				LogLog.Debug(declaringType, "ConfigureAndWatchHandler: " + e.ChangeType.ToString() + " [" + m_configFile.FullName + "]");
				m_timer.Change(500, -1);
			}

			/// <summary>
			/// Called by the timer when the configuration has been updated.
			/// </summary>
			/// <param name="state">null</param>
			private void OnWatchedFileChange(object state)
			{
				InternalConfigure(m_repository, m_configFile);
			}

			/// <summary>
			/// Release the handles held by the watcher and timer.
			/// </summary>
			[SecuritySafeCritical]
			public void Dispose()
			{
				m_watcher.EnableRaisingEvents = false;
				m_watcher.Dispose();
				m_timer.Dispose();
			}
		}

		/// <summary>
		/// Maps repository names to ConfigAndWatchHandler instances to allow a particular
		/// ConfigAndWatchHandler to dispose of its FileSystemWatcher when a repository is 
		/// reconfigured.
		/// </summary>
		private static readonly Hashtable m_repositoryName2ConfigAndWatchHandler = new Hashtable();

		/// <summary>
		/// The fully qualified type of the XmlConfigurator class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(XmlConfigurator);

		/// <summary>
		/// Private constructor
		/// </summary>
		private XmlConfigurator()
		{
		}

		/// <summary>
		/// Automatically configures the <see cref="T:log4net.Repository.ILoggerRepository" /> using settings
		/// stored in the application's configuration file.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Each application has a configuration file. This has the
		/// same name as the application with '.config' appended.
		/// This file is XML and calling this function prompts the
		/// configurator to look in that file for a section called
		/// <c>log4net</c> that contains the configuration data.
		/// </para>
		/// <para>
		/// To use this method to configure log4net you must specify 
		/// the <see cref="T:log4net.Config.Log4NetConfigurationSectionHandler" /> section
		/// handler for the <c>log4net</c> configuration section. See the
		/// <see cref="T:log4net.Config.Log4NetConfigurationSectionHandler" /> for an example.
		/// </para>
		/// </remarks>
		/// <param name="repository">The repository to configure.</param>
		public static ICollection Configure(ILoggerRepository repository)
		{
			ArrayList arrayList = new ArrayList();
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				InternalConfigure(repository);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		private static void InternalConfigure(ILoggerRepository repository)
		{
			LogLog.Debug(declaringType, "configuring repository [" + repository.Name + "] using .config file section");
			try
			{
				LogLog.Debug(declaringType, "Application config file is [" + SystemInfo.ConfigurationFileLocation + "]");
			}
			catch
			{
				LogLog.Debug(declaringType, "Application config file location unknown");
			}
			try
			{
				XmlElement xmlElement = ConfigurationManager.GetSection("log4net") as XmlElement;
				if (xmlElement == null)
				{
					LogLog.Error(declaringType, "Failed to find configuration section 'log4net' in the application's .config file. Check your .config file for the <log4net> and <configSections> elements. The configuration section should look like: <section name=\"log4net\" type=\"log4net.Config.Log4NetConfigurationSectionHandler,log4net\" />");
				}
				else
				{
					InternalConfigureFromXml(repository, xmlElement);
				}
			}
			catch (ConfigurationException ex)
			{
				if (ex.BareMessage.IndexOf("Unrecognized element") >= 0)
				{
					LogLog.Error(declaringType, "Failed to parse config file. Check your .config file is well formed XML.", ex);
				}
				else
				{
					string str = "<section name=\"log4net\" type=\"log4net.Config.Log4NetConfigurationSectionHandler," + Assembly.GetExecutingAssembly().FullName + "\" />";
					LogLog.Error(declaringType, "Failed to parse config file. Is the <configSections> specified as: " + str, ex);
				}
			}
		}

		/// <summary>
		/// Automatically configures the log4net system based on the 
		/// application's configuration settings.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Each application has a configuration file. This has the
		/// same name as the application with '.config' appended.
		/// This file is XML and calling this function prompts the
		/// configurator to look in that file for a section called
		/// <c>log4net</c> that contains the configuration data.
		/// </para>
		/// <para>
		/// To use this method to configure log4net you must specify 
		/// the <see cref="T:log4net.Config.Log4NetConfigurationSectionHandler" /> section
		/// handler for the <c>log4net</c> configuration section. See the
		/// <see cref="T:log4net.Config.Log4NetConfigurationSectionHandler" /> for an example.
		/// </para>
		/// </remarks>
		/// <seealso cref="T:log4net.Config.Log4NetConfigurationSectionHandler" />
		public static ICollection Configure()
		{
			return Configure(LogManager.GetRepository(Assembly.GetCallingAssembly()));
		}

		/// <summary>
		/// Configures log4net using a <c>log4net</c> element
		/// </summary>
		/// <remarks>
		/// <para>
		/// Loads the log4net configuration from the XML element
		/// supplied as <paramref name="element" />.
		/// </para>
		/// </remarks>
		/// <param name="element">The element to parse.</param>
		public static ICollection Configure(XmlElement element)
		{
			ArrayList arrayList = new ArrayList();
			ILoggerRepository repository = LogManager.GetRepository(Assembly.GetCallingAssembly());
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				InternalConfigureFromXml(repository, element);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		/// <summary>
		/// Configures log4net using the specified configuration file.
		/// </summary>
		/// <param name="configFile">The XML file to load the configuration from.</param>
		/// <remarks>
		/// <para>
		/// The configuration file must be valid XML. It must contain
		/// at least one element called <c>log4net</c> that holds
		/// the log4net configuration data.
		/// </para>
		/// <para>
		/// The log4net configuration file can possible be specified in the application's
		/// configuration file (either <c>MyAppName.exe.config</c> for a
		/// normal application on <c>Web.config</c> for an ASP.NET application).
		/// </para>
		/// <para>
		/// The first element matching <c>&lt;configuration&gt;</c> will be read as the 
		/// configuration. If this file is also a .NET .config file then you must specify 
		/// a configuration section for the <c>log4net</c> element otherwise .NET will 
		/// complain. Set the type for the section handler to <see cref="T:System.Configuration.IgnoreSectionHandler" />, for example:
		/// <code lang="XML" escaped="true">
		/// <configSections>
		/// 	<section name="log4net" type="System.Configuration.IgnoreSectionHandler" />
		/// </configSections>
		/// </code>
		/// </para>
		/// <example>
		/// The following example configures log4net using a configuration file, of which the 
		/// location is stored in the application's configuration file :
		/// </example>
		/// <code lang="C#">
		/// using log4net.Config;
		/// using System.IO;
		/// using System.Configuration;
		///
		/// ...
		///
		/// XmlConfigurator.Configure(new FileInfo(ConfigurationSettings.AppSettings["log4net-config-file"]));
		/// </code>
		/// <para>
		/// In the <c>.config</c> file, the path to the log4net can be specified like this :
		/// </para>
		/// <code lang="XML" escaped="true">
		/// <configuration>
		/// 	<appSettings>
		/// 		<add key="log4net-config-file" value="log.config" />
		/// 	</appSettings>
		/// </configuration>
		/// </code>
		/// </remarks>
		public static ICollection Configure(FileInfo configFile)
		{
			ArrayList arrayList = new ArrayList();
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				InternalConfigure(LogManager.GetRepository(Assembly.GetCallingAssembly()), configFile);
				return arrayList;
			}
		}

		/// <summary>
		/// Configures log4net using the specified configuration URI.
		/// </summary>
		/// <param name="configUri">A URI to load the XML configuration from.</param>
		/// <remarks>
		/// <para>
		/// The configuration data must be valid XML. It must contain
		/// at least one element called <c>log4net</c> that holds
		/// the log4net configuration data.
		/// </para>
		/// <para>
		/// The <see cref="T:System.Net.WebRequest" /> must support the URI scheme specified.
		/// </para>
		/// </remarks>
		public static ICollection Configure(Uri configUri)
		{
			ArrayList arrayList = new ArrayList();
			ILoggerRepository repository = LogManager.GetRepository(Assembly.GetCallingAssembly());
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				InternalConfigure(repository, configUri);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		/// <summary>
		/// Configures log4net using the specified configuration data stream.
		/// </summary>
		/// <param name="configStream">A stream to load the XML configuration from.</param>
		/// <remarks>
		/// <para>
		/// The configuration data must be valid XML. It must contain
		/// at least one element called <c>log4net</c> that holds
		/// the log4net configuration data.
		/// </para>
		/// <para>
		/// Note that this method will NOT close the stream parameter.
		/// </para>
		/// </remarks>
		public static ICollection Configure(Stream configStream)
		{
			ArrayList arrayList = new ArrayList();
			ILoggerRepository repository = LogManager.GetRepository(Assembly.GetCallingAssembly());
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				InternalConfigure(repository, configStream);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		/// <summary>
		/// Configures the <see cref="T:log4net.Repository.ILoggerRepository" /> using the specified XML 
		/// element.
		/// </summary>
		/// <remarks>
		/// Loads the log4net configuration from the XML element
		/// supplied as <paramref name="element" />.
		/// </remarks>
		/// <param name="repository">The repository to configure.</param>
		/// <param name="element">The element to parse.</param>
		public static ICollection Configure(ILoggerRepository repository, XmlElement element)
		{
			ArrayList arrayList = new ArrayList();
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				LogLog.Debug(declaringType, "configuring repository [" + repository.Name + "] using XML element");
				InternalConfigureFromXml(repository, element);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		/// <summary>
		/// Configures the <see cref="T:log4net.Repository.ILoggerRepository" /> using the specified configuration 
		/// file.
		/// </summary>
		/// <param name="repository">The repository to configure.</param>
		/// <param name="configFile">The XML file to load the configuration from.</param>
		/// <remarks>
		/// <para>
		/// The configuration file must be valid XML. It must contain
		/// at least one element called <c>log4net</c> that holds
		/// the configuration data.
		/// </para>
		/// <para>
		/// The log4net configuration file can possible be specified in the application's
		/// configuration file (either <c>MyAppName.exe.config</c> for a
		/// normal application on <c>Web.config</c> for an ASP.NET application).
		/// </para>
		/// <para>
		/// The first element matching <c>&lt;configuration&gt;</c> will be read as the 
		/// configuration. If this file is also a .NET .config file then you must specify 
		/// a configuration section for the <c>log4net</c> element otherwise .NET will 
		/// complain. Set the type for the section handler to <see cref="T:System.Configuration.IgnoreSectionHandler" />, for example:
		/// <code lang="XML" escaped="true">
		/// <configSections>
		/// 	<section name="log4net" type="System.Configuration.IgnoreSectionHandler" />
		/// </configSections>
		/// </code>
		/// </para>
		/// <example>
		/// The following example configures log4net using a configuration file, of which the 
		/// location is stored in the application's configuration file :
		/// </example>
		/// <code lang="C#">
		/// using log4net.Config;
		/// using System.IO;
		/// using System.Configuration;
		///
		/// ...
		///
		/// XmlConfigurator.Configure(new FileInfo(ConfigurationSettings.AppSettings["log4net-config-file"]));
		/// </code>
		/// <para>
		/// In the <c>.config</c> file, the path to the log4net can be specified like this :
		/// </para>
		/// <code lang="XML" escaped="true">
		/// <configuration>
		/// 	<appSettings>
		/// 		<add key="log4net-config-file" value="log.config" />
		/// 	</appSettings>
		/// </configuration>
		/// </code>
		/// </remarks>
		public static ICollection Configure(ILoggerRepository repository, FileInfo configFile)
		{
			ArrayList arrayList = new ArrayList();
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				InternalConfigure(repository, configFile);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		private static void InternalConfigure(ILoggerRepository repository, FileInfo configFile)
		{
			LogLog.Debug(declaringType, "configuring repository [" + repository.Name + "] using file [" + ((configFile != null) ? configFile.ToString() : null) + "]");
			if (configFile == null)
			{
				LogLog.Error(declaringType, "Configure called with null 'configFile' parameter");
			}
			else if (File.Exists(configFile.FullName))
			{
				FileStream fileStream = null;
				int num = 5;
				while (true)
				{
					if (--num >= 0)
					{
						try
						{
							fileStream = configFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
						}
						catch (IOException exception)
						{
							if (num == 0)
							{
								LogLog.Error(declaringType, "Failed to open XML config file [" + configFile.Name + "]", exception);
								fileStream = null;
							}
							Thread.Sleep(250);
							continue;
						}
					}
					break;
				}
				if (fileStream != null)
				{
					try
					{
						InternalConfigure(repository, fileStream);
					}
					finally
					{
						fileStream.Dispose();
					}
				}
			}
			else
			{
				LogLog.Debug(declaringType, "config file [" + configFile.FullName + "] not found. Configuration unchanged.");
			}
		}

		/// <summary>
		/// Configures the <see cref="T:log4net.Repository.ILoggerRepository" /> using the specified configuration 
		/// URI.
		/// </summary>
		/// <param name="repository">The repository to configure.</param>
		/// <param name="configUri">A URI to load the XML configuration from.</param>
		/// <remarks>
		/// <para>
		/// The configuration data must be valid XML. It must contain
		/// at least one element called <c>log4net</c> that holds
		/// the configuration data.
		/// </para>
		/// <para>
		/// The <see cref="T:System.Net.WebRequest" /> must support the URI scheme specified.
		/// </para>
		/// </remarks>
		public static ICollection Configure(ILoggerRepository repository, Uri configUri)
		{
			ArrayList arrayList = new ArrayList();
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				InternalConfigure(repository, configUri);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		private static void InternalConfigure(ILoggerRepository repository, Uri configUri)
		{
			LogLog.Debug(declaringType, "configuring repository [" + repository.Name + "] using URI [" + (((object)configUri != null) ? configUri.ToString() : null) + "]");
			if (configUri == null)
			{
				LogLog.Error(declaringType, "Configure called with null 'configUri' parameter");
			}
			else if (configUri.IsFile)
			{
				InternalConfigure(repository, new FileInfo(configUri.LocalPath));
			}
			else
			{
				WebRequest webRequest = null;
				try
				{
					webRequest = WebRequest.Create(configUri);
				}
				catch (Exception exception)
				{
					LogLog.Error(declaringType, "Failed to create WebRequest for URI [" + (((object)configUri != null) ? configUri.ToString() : null) + "]", exception);
				}
				if (webRequest != null)
				{
					try
					{
						webRequest.Credentials = CredentialCache.DefaultCredentials;
					}
					catch
					{
					}
					try
					{
						using (WebResponse webResponse = webRequest.GetResponse())
						{
							if (webResponse != null)
							{
								using (Stream configStream = webResponse.GetResponseStream())
								{
									InternalConfigure(repository, configStream);
								}
							}
						}
					}
					catch (Exception exception2)
					{
						LogLog.Error(declaringType, "Failed to request config from URI [" + (((object)configUri != null) ? configUri.ToString() : null) + "]", exception2);
					}
				}
			}
		}

		/// <summary>
		/// Configures the <see cref="T:log4net.Repository.ILoggerRepository" /> using the specified configuration 
		/// file.
		/// </summary>
		/// <param name="repository">The repository to configure.</param>
		/// <param name="configStream">The stream to load the XML configuration from.</param>
		/// <remarks>
		/// <para>
		/// The configuration data must be valid XML. It must contain
		/// at least one element called <c>log4net</c> that holds
		/// the configuration data.
		/// </para>
		/// <para>
		/// Note that this method will NOT close the stream parameter.
		/// </para>
		/// </remarks>
		public static ICollection Configure(ILoggerRepository repository, Stream configStream)
		{
			ArrayList arrayList = new ArrayList();
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				InternalConfigure(repository, configStream);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		private static void InternalConfigure(ILoggerRepository repository, Stream configStream)
		{
			LogLog.Debug(declaringType, "configuring repository [" + repository.Name + "] using stream");
			if (configStream == null)
			{
				LogLog.Error(declaringType, "Configure called with null 'configStream' parameter");
			}
			else
			{
				XmlDocument xmlDocument = new XmlDocument
				{
					XmlResolver = null
				};
				try
				{
					XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
					xmlReaderSettings.DtdProcessing = DtdProcessing.Ignore;
					using (XmlReader reader = XmlReader.Create(configStream, xmlReaderSettings))
					{
						xmlDocument.Load(reader);
					}
				}
				catch (Exception exception)
				{
					LogLog.Error(declaringType, "Error while loading XML configuration", exception);
					xmlDocument = null;
				}
				if (xmlDocument != null)
				{
					LogLog.Debug(declaringType, "loading XML configuration");
					XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("log4net");
					if (elementsByTagName.Count == 0)
					{
						LogLog.Debug(declaringType, "XML configuration does not contain a <log4net> element. Configuration Aborted.");
					}
					else if (elementsByTagName.Count > 1)
					{
						LogLog.Error(declaringType, "XML configuration contains [" + elementsByTagName.Count.ToString() + "] <log4net> elements. Only one is allowed. Configuration Aborted.");
					}
					else
					{
						InternalConfigureFromXml(repository, elementsByTagName[0] as XmlElement);
					}
				}
			}
		}

		/// <summary>
		/// Configures log4net using the file specified, monitors the file for changes 
		/// and reloads the configuration if a change is detected.
		/// </summary>
		/// <param name="configFile">The XML file to load the configuration from.</param>
		/// <remarks>
		/// <para>
		/// The configuration file must be valid XML. It must contain
		/// at least one element called <c>log4net</c> that holds
		/// the configuration data.
		/// </para>
		/// <para>
		/// The configuration file will be monitored using a <see cref="T:System.IO.FileSystemWatcher" />
		/// and depends on the behavior of that class.
		/// </para>
		/// <para>
		/// For more information on how to configure log4net using
		/// a separate configuration file, see <see cref="M:Configure(FileInfo)" />.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Configure(FileInfo)" />
		public static ICollection ConfigureAndWatch(FileInfo configFile)
		{
			ArrayList arrayList = new ArrayList();
			ILoggerRepository repository = LogManager.GetRepository(Assembly.GetCallingAssembly());
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				InternalConfigureAndWatch(repository, configFile);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		/// <summary>
		/// Configures the <see cref="T:log4net.Repository.ILoggerRepository" /> using the file specified, 
		/// monitors the file for changes and reloads the configuration if a change 
		/// is detected.
		/// </summary>
		/// <param name="repository">The repository to configure.</param>
		/// <param name="configFile">The XML file to load the configuration from.</param>
		/// <remarks>
		/// <para>
		/// The configuration file must be valid XML. It must contain
		/// at least one element called <c>log4net</c> that holds
		/// the configuration data.
		/// </para>
		/// <para>
		/// The configuration file will be monitored using a <see cref="T:System.IO.FileSystemWatcher" />
		/// and depends on the behavior of that class.
		/// </para>
		/// <para>
		/// For more information on how to configure log4net using
		/// a separate configuration file, see <see cref="M:Configure(FileInfo)" />.
		/// </para>
		/// </remarks>
		/// <seealso cref="M:Configure(FileInfo)" />
		public static ICollection ConfigureAndWatch(ILoggerRepository repository, FileInfo configFile)
		{
			ArrayList arrayList = new ArrayList();
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				InternalConfigureAndWatch(repository, configFile);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		private static void InternalConfigureAndWatch(ILoggerRepository repository, FileInfo configFile)
		{
			LogLog.Debug(declaringType, "configuring repository [" + repository.Name + "] using file [" + ((configFile != null) ? configFile.ToString() : null) + "] watching for file updates");
			if (configFile == null)
			{
				LogLog.Error(declaringType, "ConfigureAndWatch called with null 'configFile' parameter");
			}
			else
			{
				InternalConfigure(repository, configFile);
				try
				{
					lock (m_repositoryName2ConfigAndWatchHandler)
					{
						ConfigureAndWatchHandler configureAndWatchHandler = (ConfigureAndWatchHandler)m_repositoryName2ConfigAndWatchHandler[configFile.FullName];
						if (configureAndWatchHandler != null)
						{
							m_repositoryName2ConfigAndWatchHandler.Remove(configFile.FullName);
							configureAndWatchHandler.Dispose();
						}
						configureAndWatchHandler = new ConfigureAndWatchHandler(repository, configFile);
						m_repositoryName2ConfigAndWatchHandler[configFile.FullName] = configureAndWatchHandler;
					}
				}
				catch (Exception exception)
				{
					LogLog.Error(declaringType, "Failed to initialize configuration file watcher for file [" + configFile.FullName + "]", exception);
				}
			}
		}

		/// <summary>
		/// Configures the specified repository using a <c>log4net</c> element.
		/// </summary>
		/// <param name="repository">The hierarchy to configure.</param>
		/// <param name="element">The element to parse.</param>
		/// <remarks>
		/// <para>
		/// Loads the log4net configuration from the XML element
		/// supplied as <paramref name="element" />.
		/// </para>
		/// <para>
		/// This method is ultimately called by one of the Configure methods 
		/// to load the configuration from an <see cref="T:System.Xml.XmlElement" />.
		/// </para>
		/// </remarks>
		private static void InternalConfigureFromXml(ILoggerRepository repository, XmlElement element)
		{
			if (element == null)
			{
				LogLog.Error(declaringType, "ConfigureFromXml called with null 'element' parameter");
			}
			else if (repository == null)
			{
				LogLog.Error(declaringType, "ConfigureFromXml called with null 'repository' parameter");
			}
			else
			{
				LogLog.Debug(declaringType, "Configuring Repository [" + repository.Name + "]");
				IXmlRepositoryConfigurator xmlRepositoryConfigurator = repository as IXmlRepositoryConfigurator;
				if (xmlRepositoryConfigurator == null)
				{
					LogLog.Warn(declaringType, "Repository [" + ((repository != null) ? repository.ToString() : null) + "] does not support the XmlConfigurator");
				}
				else
				{
					XmlDocument obj = new XmlDocument
					{
						XmlResolver = null
					};
					XmlElement element2 = (XmlElement)obj.AppendChild(obj.ImportNode(element, true));
					xmlRepositoryConfigurator.Configure(element2);
				}
			}
		}
	}
}
