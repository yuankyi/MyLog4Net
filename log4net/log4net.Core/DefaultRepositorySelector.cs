using log4net.Config;
using log4net.Plugin;
using log4net.Repository;
using log4net.Util;
using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace log4net.Core
{
	/// <summary>
	/// The default implementation of the <see cref="T:log4net.Core.IRepositorySelector" /> interface.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Uses attributes defined on the calling assembly to determine how to
	/// configure the hierarchy for the repository.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	public class DefaultRepositorySelector : IRepositorySelector
	{
		/// <summary>
		/// The fully qualified type of the DefaultRepositorySelector class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(DefaultRepositorySelector);

		private const string DefaultRepositoryName = "log4net-default-repository";

		private readonly Hashtable m_name2repositoryMap = new Hashtable();

		private readonly Hashtable m_assembly2repositoryMap = new Hashtable();

		private readonly Hashtable m_alias2repositoryMap = new Hashtable();

		private readonly Type m_defaultRepositoryType;

		/// <summary>
		/// Event to notify that a logger repository has been created.
		/// </summary>
		/// <value>
		/// Event to notify that a logger repository has been created.
		/// </value>
		/// <remarks>
		/// <para>
		/// Event raised when a new repository is created.
		/// The event source will be this selector. The event args will
		/// be a <see cref="T:log4net.Core.LoggerRepositoryCreationEventArgs" /> which
		/// holds the newly created <see cref="T:log4net.Repository.ILoggerRepository" />.
		/// </para>
		/// </remarks>
		public event LoggerRepositoryCreationEventHandler LoggerRepositoryCreatedEvent
		{
			add
			{
				m_loggerRepositoryCreatedEvent += value;
			}
			remove
			{
				m_loggerRepositoryCreatedEvent -= value;
			}
		}

		private event LoggerRepositoryCreationEventHandler m_loggerRepositoryCreatedEvent;

		/// <summary>
		/// Creates a new repository selector.
		/// </summary>
		/// <param name="defaultRepositoryType">The type of the repositories to create, must implement <see cref="T:log4net.Repository.ILoggerRepository" /></param>
		/// <remarks>
		/// <para>
		/// Create an new repository selector.
		/// The default type for repositories must be specified,
		/// an appropriate value would be <see cref="T:log4net.Repository.Hierarchy.Hierarchy" />.
		/// </para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="defaultRepositoryType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="defaultRepositoryType" /> does not implement <see cref="T:log4net.Repository.ILoggerRepository" />.</exception>
		public DefaultRepositorySelector(Type defaultRepositoryType)
		{
			if (defaultRepositoryType == null)
			{
				throw new ArgumentNullException("defaultRepositoryType");
			}
			if (!typeof(ILoggerRepository).IsAssignableFrom(defaultRepositoryType))
			{
				throw SystemInfo.CreateArgumentOutOfRangeException("defaultRepositoryType", defaultRepositoryType, "Parameter: defaultRepositoryType, Value: [" + (((object)defaultRepositoryType != null) ? defaultRepositoryType.ToString() : null) + "] out of range. Argument must implement the ILoggerRepository interface");
			}
			m_defaultRepositoryType = defaultRepositoryType;
			Type source = declaringType;
			Type defaultRepositoryType2 = m_defaultRepositoryType;
			LogLog.Debug(source, "defaultRepositoryType [" + (((object)defaultRepositoryType2 != null) ? defaultRepositoryType2.ToString() : null) + "]");
		}

		/// <summary>
		/// Gets the <see cref="T:log4net.Repository.ILoggerRepository" /> for the specified assembly.
		/// </summary>
		/// <param name="repositoryAssembly">The assembly use to lookup the <see cref="T:log4net.Repository.ILoggerRepository" />.</param>
		/// <remarks>
		/// <para>
		/// The type of the <see cref="T:log4net.Repository.ILoggerRepository" /> created and the repository 
		/// to create can be overridden by specifying the <see cref="T:log4net.Config.RepositoryAttribute" /> 
		/// attribute on the <paramref name="repositoryAssembly" />.
		/// </para>
		/// <para>
		/// The default values are to use the <see cref="T:log4net.Repository.Hierarchy.Hierarchy" /> 
		/// implementation of the <see cref="T:log4net.Repository.ILoggerRepository" /> interface and to use the
		/// <see cref="P:System.Reflection.AssemblyName.Name" /> as the name of the repository.
		/// </para>
		/// <para>
		/// The <see cref="T:log4net.Repository.ILoggerRepository" /> created will be automatically configured using 
		/// any <see cref="T:log4net.Config.ConfiguratorAttribute" /> attributes defined on
		/// the <paramref name="repositoryAssembly" />.
		/// </para>
		/// </remarks>
		/// <returns>The <see cref="T:log4net.Repository.ILoggerRepository" /> for the assembly</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="repositoryAssembly" /> is <see langword="null" />.</exception>
		public ILoggerRepository GetRepository(Assembly repositoryAssembly)
		{
			if (repositoryAssembly == null)
			{
				throw new ArgumentNullException("repositoryAssembly");
			}
			return CreateRepository(repositoryAssembly, m_defaultRepositoryType);
		}

		/// <summary>
		/// Gets the <see cref="T:log4net.Repository.ILoggerRepository" /> for the specified repository.
		/// </summary>
		/// <param name="repositoryName">The repository to use to lookup the <see cref="T:log4net.Repository.ILoggerRepository" />.</param>
		/// <returns>The <see cref="T:log4net.Repository.ILoggerRepository" /> for the specified repository.</returns>
		/// <remarks>
		/// <para>
		/// Returns the named repository. If <paramref name="repositoryName" /> is <c>null</c>
		/// a <see cref="T:System.ArgumentNullException" /> is thrown. If the repository 
		/// does not exist a <see cref="T:log4net.Core.LogException" /> is thrown.
		/// </para>
		/// <para>
		/// Use <see cref="M:CreateRepository(string, Type)" /> to create a repository.
		/// </para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="repositoryName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:log4net.Core.LogException"><paramref name="repositoryName" /> does not exist.</exception>
		public ILoggerRepository GetRepository(string repositoryName)
		{
			if (repositoryName == null)
			{
				throw new ArgumentNullException("repositoryName");
			}
			lock (this)
			{
				ILoggerRepository obj = m_name2repositoryMap[repositoryName] as ILoggerRepository;
				if (obj == null)
				{
					throw new LogException("Repository [" + repositoryName + "] is NOT defined.");
				}
				return obj;
			}
		}

		/// <summary>
		/// Create a new repository for the assembly specified 
		/// </summary>
		/// <param name="repositoryAssembly">the assembly to use to create the repository to associate with the <see cref="T:log4net.Repository.ILoggerRepository" />.</param>
		/// <param name="repositoryType">The type of repository to create, must implement <see cref="T:log4net.Repository.ILoggerRepository" />.</param>
		/// <returns>The repository created.</returns>
		/// <remarks>
		/// <para>
		/// The <see cref="T:log4net.Repository.ILoggerRepository" /> created will be associated with the repository
		/// specified such that a call to <see cref="M:GetRepository(Assembly)" /> with the
		/// same assembly specified will return the same repository instance.
		/// </para>
		/// <para>
		/// The type of the <see cref="T:log4net.Repository.ILoggerRepository" /> created and
		/// the repository to create can be overridden by specifying the
		/// <see cref="T:log4net.Config.RepositoryAttribute" /> attribute on the 
		/// <paramref name="repositoryAssembly" />.  The default values are to use the 
		/// <paramref name="repositoryType" /> implementation of the 
		/// <see cref="T:log4net.Repository.ILoggerRepository" /> interface and to use the
		/// <see cref="P:System.Reflection.AssemblyName.Name" /> as the name of the repository.
		/// </para>
		/// <para>
		/// The <see cref="T:log4net.Repository.ILoggerRepository" /> created will be automatically
		/// configured using any <see cref="T:log4net.Config.ConfiguratorAttribute" /> 
		/// attributes defined on the <paramref name="repositoryAssembly" />.
		/// </para>
		/// <para>
		/// If a repository for the <paramref name="repositoryAssembly" /> already exists
		/// that repository will be returned. An error will not be raised and that 
		/// repository may be of a different type to that specified in <paramref name="repositoryType" />.
		/// Also the <see cref="T:log4net.Config.RepositoryAttribute" /> attribute on the
		/// assembly may be used to override the repository type specified in 
		/// <paramref name="repositoryType" />.
		/// </para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="repositoryAssembly" /> is <see langword="null" />.</exception>
		public ILoggerRepository CreateRepository(Assembly repositoryAssembly, Type repositoryType)
		{
			return CreateRepository(repositoryAssembly, repositoryType, "log4net-default-repository", true);
		}

		/// <summary>
		/// Creates a new repository for the assembly specified.
		/// </summary>
		/// <param name="repositoryAssembly">the assembly to use to create the repository to associate with the <see cref="T:log4net.Repository.ILoggerRepository" />.</param>
		/// <param name="repositoryType">The type of repository to create, must implement <see cref="T:log4net.Repository.ILoggerRepository" />.</param>
		/// <param name="repositoryName">The name to assign to the created repository</param>
		/// <param name="readAssemblyAttributes">Set to <c>true</c> to read and apply the assembly attributes</param>
		/// <returns>The repository created.</returns>
		/// <remarks>
		/// <para>
		/// The <see cref="T:log4net.Repository.ILoggerRepository" /> created will be associated with the repository
		/// specified such that a call to <see cref="M:GetRepository(Assembly)" /> with the
		/// same assembly specified will return the same repository instance.
		/// </para>
		/// <para>
		/// The type of the <see cref="T:log4net.Repository.ILoggerRepository" /> created and
		/// the repository to create can be overridden by specifying the
		/// <see cref="T:log4net.Config.RepositoryAttribute" /> attribute on the 
		/// <paramref name="repositoryAssembly" />.  The default values are to use the 
		/// <paramref name="repositoryType" /> implementation of the 
		/// <see cref="T:log4net.Repository.ILoggerRepository" /> interface and to use the
		/// <see cref="P:System.Reflection.AssemblyName.Name" /> as the name of the repository.
		/// </para>
		/// <para>
		/// The <see cref="T:log4net.Repository.ILoggerRepository" /> created will be automatically
		/// configured using any <see cref="T:log4net.Config.ConfiguratorAttribute" /> 
		/// attributes defined on the <paramref name="repositoryAssembly" />.
		/// </para>
		/// <para>
		/// If a repository for the <paramref name="repositoryAssembly" /> already exists
		/// that repository will be returned. An error will not be raised and that 
		/// repository may be of a different type to that specified in <paramref name="repositoryType" />.
		/// Also the <see cref="T:log4net.Config.RepositoryAttribute" /> attribute on the
		/// assembly may be used to override the repository type specified in 
		/// <paramref name="repositoryType" />.
		/// </para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="repositoryAssembly" /> is <see langword="null" />.</exception>
		public ILoggerRepository CreateRepository(Assembly repositoryAssembly, Type repositoryType, string repositoryName, bool readAssemblyAttributes)
		{
			if (repositoryAssembly == null)
			{
				throw new ArgumentNullException("repositoryAssembly");
			}
			if (repositoryType == null)
			{
				repositoryType = m_defaultRepositoryType;
			}
			lock (this)
			{
				ILoggerRepository loggerRepository = m_assembly2repositoryMap[repositoryAssembly] as ILoggerRepository;
				if (loggerRepository == null)
				{
					LogLog.Debug(declaringType, "Creating repository for assembly [" + (((object)repositoryAssembly != null) ? repositoryAssembly.ToString() : null) + "]");
					string repositoryName2 = repositoryName;
					Type repositoryType2 = repositoryType;
					if (readAssemblyAttributes)
					{
						GetInfoForAssembly(repositoryAssembly, ref repositoryName2, ref repositoryType2);
					}
					Type source = declaringType;
					string[] obj = new string[7]
					{
						"Assembly [",
						((object)repositoryAssembly != null) ? repositoryAssembly.ToString() : null,
						"] using repository [",
						repositoryName2,
						"] and repository type [",
						null,
						null
					};
					Type type = repositoryType2;
					obj[5] = (((object)type != null) ? type.ToString() : null);
					obj[6] = "]";
					LogLog.Debug(source, string.Concat(obj));
					loggerRepository = (m_name2repositoryMap[repositoryName2] as ILoggerRepository);
					if (loggerRepository == null)
					{
						loggerRepository = CreateRepository(repositoryName2, repositoryType2);
						if (readAssemblyAttributes)
						{
							try
							{
								LoadAliases(repositoryAssembly, loggerRepository);
								LoadPlugins(repositoryAssembly, loggerRepository);
								ConfigureRepository(repositoryAssembly, loggerRepository);
							}
							catch (Exception exception)
							{
								LogLog.Error(declaringType, "Failed to configure repository [" + repositoryName2 + "] from assembly attributes.", exception);
							}
						}
					}
					else
					{
						LogLog.Debug(declaringType, "repository [" + repositoryName2 + "] already exists, using repository type [" + loggerRepository.GetType().FullName + "]");
						if (readAssemblyAttributes)
						{
							try
							{
								LoadPlugins(repositoryAssembly, loggerRepository);
							}
							catch (Exception exception2)
							{
								LogLog.Error(declaringType, "Failed to configure repository [" + repositoryName2 + "] from assembly attributes.", exception2);
							}
						}
					}
					m_assembly2repositoryMap[repositoryAssembly] = loggerRepository;
				}
				return loggerRepository;
			}
		}

		/// <summary>
		/// Creates a new repository for the specified repository.
		/// </summary>
		/// <param name="repositoryName">The repository to associate with the <see cref="T:log4net.Repository.ILoggerRepository" />.</param>
		/// <param name="repositoryType">The type of repository to create, must implement <see cref="T:log4net.Repository.ILoggerRepository" />.
		/// If this param is <see langword="null" /> then the default repository type is used.</param>
		/// <returns>The new repository.</returns>
		/// <remarks>
		/// <para>
		/// The <see cref="T:log4net.Repository.ILoggerRepository" /> created will be associated with the repository
		/// specified such that a call to <see cref="M:GetRepository(string)" /> with the
		/// same repository specified will return the same repository instance.
		/// </para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="repositoryName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:log4net.Core.LogException"><paramref name="repositoryName" /> already exists.</exception>
		public ILoggerRepository CreateRepository(string repositoryName, Type repositoryType)
		{
			if (repositoryName == null)
			{
				throw new ArgumentNullException("repositoryName");
			}
			if (repositoryType == null)
			{
				repositoryType = m_defaultRepositoryType;
			}
			lock (this)
			{
				ILoggerRepository loggerRepository = null;
				loggerRepository = (m_name2repositoryMap[repositoryName] as ILoggerRepository);
				if (loggerRepository != null)
				{
					throw new LogException("Repository [" + repositoryName + "] is already defined. Repositories cannot be redefined.");
				}
				ILoggerRepository loggerRepository2 = m_alias2repositoryMap[repositoryName] as ILoggerRepository;
				if (loggerRepository2 != null)
				{
					if (loggerRepository2.GetType() == repositoryType)
					{
						LogLog.Debug(declaringType, "Aliasing repository [" + repositoryName + "] to existing repository [" + loggerRepository2.Name + "]");
						loggerRepository = loggerRepository2;
						m_name2repositoryMap[repositoryName] = loggerRepository;
					}
					else
					{
						LogLog.Error(declaringType, "Failed to alias repository [" + repositoryName + "] to existing repository [" + loggerRepository2.Name + "]. Requested repository type [" + repositoryType.FullName + "] is not compatible with existing type [" + loggerRepository2.GetType().FullName + "]");
					}
				}
				if (loggerRepository == null)
				{
					Type source = declaringType;
					string[] obj = new string[5]
					{
						"Creating repository [",
						repositoryName,
						"] using type [",
						null,
						null
					};
					Type type = repositoryType;
					obj[3] = (((object)type != null) ? type.ToString() : null);
					obj[4] = "]";
					LogLog.Debug(source, string.Concat(obj));
					loggerRepository = (ILoggerRepository)Activator.CreateInstance(repositoryType);
					loggerRepository.Name = repositoryName;
					m_name2repositoryMap[repositoryName] = loggerRepository;
					OnLoggerRepositoryCreatedEvent(loggerRepository);
				}
				return loggerRepository;
			}
		}

		/// <summary>
		/// Test if a named repository exists
		/// </summary>
		/// <param name="repositoryName">the named repository to check</param>
		/// <returns><c>true</c> if the repository exists</returns>
		/// <remarks>
		/// <para>
		/// Test if a named repository exists. Use <see cref="M:CreateRepository(string, Type)" />
		/// to create a new repository and <see cref="M:GetRepository(string)" /> to retrieve 
		/// a repository.
		/// </para>
		/// </remarks>
		public bool ExistsRepository(string repositoryName)
		{
			lock (this)
			{
				return m_name2repositoryMap.ContainsKey(repositoryName);
			}
		}

		/// <summary>
		/// Gets a list of <see cref="T:log4net.Repository.ILoggerRepository" /> objects
		/// </summary>
		/// <returns>an array of all known <see cref="T:log4net.Repository.ILoggerRepository" /> objects</returns>
		/// <remarks>
		/// <para>
		/// Gets an array of all of the repositories created by this selector.
		/// </para>
		/// </remarks>
		public ILoggerRepository[] GetAllRepositories()
		{
			lock (this)
			{
				ICollection values = m_name2repositoryMap.Values;
				ILoggerRepository[] array = new ILoggerRepository[values.Count];
				values.CopyTo(array, 0);
				return array;
			}
		}

		/// <summary>
		/// Aliases a repository to an existing repository.
		/// </summary>
		/// <param name="repositoryAlias">The repository to alias.</param>
		/// <param name="repositoryTarget">The repository that the repository is aliased to.</param>
		/// <remarks>
		/// <para>
		/// The repository specified will be aliased to the repository when created. 
		/// The repository must not already exist.
		/// </para>
		/// <para>
		/// When the repository is created it must utilize the same repository type as 
		/// the repository it is aliased to, otherwise the aliasing will fail.
		/// </para>
		/// </remarks>
		/// <exception cref="T:System.ArgumentNullException">
		/// <para><paramref name="repositoryAlias" /> is <see langword="null" />.</para>
		/// <para>-or-</para>
		/// <para><paramref name="repositoryTarget" /> is <see langword="null" />.</para>
		/// </exception>
		public void AliasRepository(string repositoryAlias, ILoggerRepository repositoryTarget)
		{
			if (repositoryAlias == null)
			{
				throw new ArgumentNullException("repositoryAlias");
			}
			if (repositoryTarget == null)
			{
				throw new ArgumentNullException("repositoryTarget");
			}
			lock (this)
			{
				if (m_alias2repositoryMap.Contains(repositoryAlias))
				{
					if (repositoryTarget != (ILoggerRepository)m_alias2repositoryMap[repositoryAlias])
					{
						throw new InvalidOperationException("Repository [" + repositoryAlias + "] is already aliased to repository [" + ((ILoggerRepository)m_alias2repositoryMap[repositoryAlias]).Name + "]. Aliases cannot be redefined.");
					}
				}
				else if (m_name2repositoryMap.Contains(repositoryAlias))
				{
					if (repositoryTarget != (ILoggerRepository)m_name2repositoryMap[repositoryAlias])
					{
						throw new InvalidOperationException("Repository [" + repositoryAlias + "] already exists and cannot be aliased to repository [" + repositoryTarget.Name + "].");
					}
				}
				else
				{
					m_alias2repositoryMap[repositoryAlias] = repositoryTarget;
				}
			}
		}

		/// <summary>
		/// Notifies the registered listeners that the repository has been created.
		/// </summary>
		/// <param name="repository">The repository that has been created.</param>
		/// <remarks>
		/// <para>
		/// Raises the <see cref="E:log4net.Core.DefaultRepositorySelector.LoggerRepositoryCreatedEvent" /> event.
		/// </para>
		/// </remarks>
		protected virtual void OnLoggerRepositoryCreatedEvent(ILoggerRepository repository)
		{
			LoggerRepositoryCreationEventHandler loggerRepositoryCreatedEvent = this.m_loggerRepositoryCreatedEvent;
			if (loggerRepositoryCreatedEvent != null)
			{
				loggerRepositoryCreatedEvent(this, new LoggerRepositoryCreationEventArgs(repository));
			}
		}

		/// <summary>
		/// Gets the repository name and repository type for the specified assembly.
		/// </summary>
		/// <param name="assembly">The assembly that has a <see cref="T:log4net.Config.RepositoryAttribute" />.</param>
		/// <param name="repositoryName">in/out param to hold the repository name to use for the assembly, caller should set this to the default value before calling.</param>
		/// <param name="repositoryType">in/out param to hold the type of the repository to create for the assembly, caller should set this to the default value before calling.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="assembly" /> is <see langword="null" />.</exception>
		private void GetInfoForAssembly(Assembly assembly, ref string repositoryName, ref Type repositoryType)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			try
			{
				LogLog.Debug(declaringType, "Assembly [" + assembly.FullName + "] Loaded From [" + SystemInfo.AssemblyLocationInfo(assembly) + "]");
			}
			catch
			{
			}
			try
			{
				object[] customAttributes = Attribute.GetCustomAttributes(assembly, typeof(RepositoryAttribute), false);
				object[] array = customAttributes;
				if (array == null || array.Length == 0)
				{
					LogLog.Debug(declaringType, "Assembly [" + (((object)assembly != null) ? assembly.ToString() : null) + "] does not have a RepositoryAttribute specified.");
				}
				else
				{
					if (array.Length > 1)
					{
						LogLog.Error(declaringType, "Assembly [" + (((object)assembly != null) ? assembly.ToString() : null) + "] has multiple log4net.Config.RepositoryAttribute assembly attributes. Only using first occurrence.");
					}
					RepositoryAttribute repositoryAttribute = array[0] as RepositoryAttribute;
					if (repositoryAttribute == null)
					{
						LogLog.Error(declaringType, "Assembly [" + (((object)assembly != null) ? assembly.ToString() : null) + "] has a RepositoryAttribute but it does not!.");
					}
					else
					{
						if (repositoryAttribute.Name != null)
						{
							repositoryName = repositoryAttribute.Name;
						}
						if (repositoryAttribute.RepositoryType != null)
						{
							if (typeof(ILoggerRepository).IsAssignableFrom(repositoryAttribute.RepositoryType))
							{
								repositoryType = repositoryAttribute.RepositoryType;
							}
							else
							{
								Type source = declaringType;
								Type repositoryType2 = repositoryAttribute.RepositoryType;
								LogLog.Error(source, "DefaultRepositorySelector: Repository Type [" + (((object)repositoryType2 != null) ? repositoryType2.ToString() : null) + "] must implement the ILoggerRepository interface.");
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "Unhandled exception in GetInfoForAssembly", exception);
			}
		}

		/// <summary>
		/// Configures the repository using information from the assembly.
		/// </summary>
		/// <param name="assembly">The assembly containing <see cref="T:log4net.Config.ConfiguratorAttribute" />
		/// attributes which define the configuration for the repository.</param>
		/// <param name="repository">The repository to configure.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <para><paramref name="assembly" /> is <see langword="null" />.</para>
		/// <para>-or-</para>
		/// <para><paramref name="repository" /> is <see langword="null" />.</para>
		/// </exception>
		private void ConfigureRepository(Assembly assembly, ILoggerRepository repository)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (repository == null)
			{
				throw new ArgumentNullException("repository");
			}
			object[] customAttributes = Attribute.GetCustomAttributes(assembly, typeof(ConfiguratorAttribute), false);
			object[] array = customAttributes;
			if (array != null && array.Length != 0)
			{
				Array.Sort(array);
				customAttributes = array;
				for (int i = 0; i < customAttributes.Length; i++)
				{
					ConfiguratorAttribute configuratorAttribute = (ConfiguratorAttribute)customAttributes[i];
					if (configuratorAttribute != null)
					{
						try
						{
							configuratorAttribute.Configure(assembly, repository);
						}
						catch (Exception exception)
						{
							LogLog.Error(declaringType, "Exception calling [" + configuratorAttribute.GetType().FullName + "] .Configure method.", exception);
						}
					}
				}
			}
			if (repository.Name == "log4net-default-repository")
			{
				string appSetting = SystemInfo.GetAppSetting("log4net.Config");
				if (appSetting != null && appSetting.Length > 0)
				{
					string text = null;
					try
					{
						text = SystemInfo.ApplicationBaseDirectory;
					}
					catch (Exception exception2)
					{
						LogLog.Warn(declaringType, "Exception getting ApplicationBaseDirectory. appSettings log4net.Config path [" + appSetting + "] will be treated as an absolute URI", exception2);
					}
					string text2 = appSetting;
					if (text != null)
					{
						text2 = Path.Combine(text, appSetting);
					}
					bool result = false;
					bool.TryParse(SystemInfo.GetAppSetting("log4net.Config.Watch"), out result);
					if (result)
					{
						FileInfo configFile = null;
						try
						{
							configFile = new FileInfo(text2);
						}
						catch (Exception exception3)
						{
							LogLog.Error(declaringType, "DefaultRepositorySelector: Exception while parsing log4net.Config file physical path [" + text2 + "]", exception3);
						}
						try
						{
							LogLog.Debug(declaringType, "Loading and watching configuration for default repository from AppSettings specified Config path [" + text2 + "]");
							XmlConfigurator.ConfigureAndWatch(repository, configFile);
						}
						catch (Exception exception4)
						{
							LogLog.Error(declaringType, "DefaultRepositorySelector: Exception calling XmlConfigurator.ConfigureAndWatch method with ConfigFilePath [" + text2 + "]", exception4);
						}
					}
					else
					{
						Uri uri = null;
						try
						{
							uri = new Uri(text2);
						}
						catch (Exception exception5)
						{
							LogLog.Error(declaringType, "Exception while parsing log4net.Config file path [" + appSetting + "]", exception5);
						}
						if (uri != null)
						{
							LogLog.Debug(declaringType, "Loading configuration for default repository from AppSettings specified Config URI [" + uri.ToString() + "]");
							try
							{
								XmlConfigurator.Configure(repository, uri);
							}
							catch (Exception exception6)
							{
								Type source = declaringType;
								Uri uri2 = uri;
								LogLog.Error(source, "Exception calling XmlConfigurator.Configure method with ConfigUri [" + (((object)uri2 != null) ? uri2.ToString() : null) + "]", exception6);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Loads the attribute defined plugins on the assembly.
		/// </summary>
		/// <param name="assembly">The assembly that contains the attributes.</param>
		/// <param name="repository">The repository to add the plugins to.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <para><paramref name="assembly" /> is <see langword="null" />.</para>
		/// <para>-or-</para>
		/// <para><paramref name="repository" /> is <see langword="null" />.</para>
		/// </exception>
		private void LoadPlugins(Assembly assembly, ILoggerRepository repository)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (repository == null)
			{
				throw new ArgumentNullException("repository");
			}
			object[] customAttributes = Attribute.GetCustomAttributes(assembly, typeof(PluginAttribute), false);
			object[] array = customAttributes;
			if (array != null && array.Length != 0)
			{
				customAttributes = array;
				for (int i = 0; i < customAttributes.Length; i++)
				{
					IPluginFactory pluginFactory = (IPluginFactory)customAttributes[i];
					try
					{
						repository.PluginMap.Add(pluginFactory.CreatePlugin());
					}
					catch (Exception exception)
					{
						LogLog.Error(declaringType, "Failed to create plugin. Attribute [" + pluginFactory.ToString() + "]", exception);
					}
				}
			}
		}

		/// <summary>
		/// Loads the attribute defined aliases on the assembly.
		/// </summary>
		/// <param name="assembly">The assembly that contains the attributes.</param>
		/// <param name="repository">The repository to alias to.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <para><paramref name="assembly" /> is <see langword="null" />.</para>
		/// <para>-or-</para>
		/// <para><paramref name="repository" /> is <see langword="null" />.</para>
		/// </exception>
		private void LoadAliases(Assembly assembly, ILoggerRepository repository)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (repository == null)
			{
				throw new ArgumentNullException("repository");
			}
			object[] customAttributes = Attribute.GetCustomAttributes(assembly, typeof(AliasRepositoryAttribute), false);
			object[] array = customAttributes;
			if (array != null && array.Length != 0)
			{
				customAttributes = array;
				for (int i = 0; i < customAttributes.Length; i++)
				{
					AliasRepositoryAttribute aliasRepositoryAttribute = (AliasRepositoryAttribute)customAttributes[i];
					try
					{
						AliasRepository(aliasRepositoryAttribute.Name, repository);
					}
					catch (Exception exception)
					{
						LogLog.Error(declaringType, "Failed to alias repository [" + aliasRepositoryAttribute.Name + "]", exception);
					}
				}
			}
		}
	}
}
