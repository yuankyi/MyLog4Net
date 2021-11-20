using System;

namespace log4net.Config
{
	/// <summary>
	/// Assembly level attribute that specifies the logging repository for the assembly.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Assemblies are mapped to logging repository. This attribute specified 
	/// on the assembly controls
	/// the configuration of the repository. The <see cref="P:log4net.Config.RepositoryAttribute.Name" /> property specifies the name
	/// of the repository that this assembly is a part of. The <see cref="P:log4net.Config.RepositoryAttribute.RepositoryType" />
	/// specifies the type of the <see cref="T:log4net.Repository.ILoggerRepository" /> object 
	/// to create for the assembly. If this attribute is not specified or a <see cref="P:log4net.Config.RepositoryAttribute.Name" /> 
	/// is not specified then the assembly will be part of the default shared logging repository.
	/// </para>
	/// <para>
	/// This attribute can only be specified on the assembly and may only be used
	/// once per assembly.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	[Serializable]
	[AttributeUsage(AttributeTargets.Assembly)]
	public class RepositoryAttribute : Attribute
	{
		private string m_name;

		private Type m_repositoryType;

		/// <summary>
		/// Gets or sets the name of the logging repository.
		/// </summary>
		/// <value>
		/// The string name to use as the name of the repository associated with this
		/// assembly.
		/// </value>
		/// <remarks>
		/// <para>
		/// This value does not have to be unique. Several assemblies can share the
		/// same repository. They will share the logging configuration of the repository.
		/// </para>
		/// </remarks>
		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of repository to create for this assembly.
		/// </summary>
		/// <value>
		/// The type of repository to create for this assembly.
		/// </value>
		/// <remarks>
		/// <para>
		/// The type of the repository to create for the assembly.
		/// The type must implement the <see cref="T:log4net.Repository.ILoggerRepository" />
		/// interface.
		/// </para>
		/// <para>
		/// This will be the type of repository created when 
		/// the repository is created. If multiple assemblies reference the
		/// same repository then the repository is only created once using the
		/// <see cref="P:log4net.Config.RepositoryAttribute.RepositoryType" /> of the first assembly to call into the 
		/// repository.
		/// </para>
		/// </remarks>
		public Type RepositoryType
		{
			get
			{
				return m_repositoryType;
			}
			set
			{
				m_repositoryType = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:log4net.Config.RepositoryAttribute" /> class.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Default constructor.
		/// </para>
		/// </remarks>
		public RepositoryAttribute()
		{
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="T:log4net.Config.RepositoryAttribute" /> class 
		/// with the name of the repository.
		/// </summary>
		/// <param name="name">The name of the repository.</param>
		/// <remarks>
		/// <para>
		/// Initialize the attribute with the name for the assembly's repository.
		/// </para>
		/// </remarks>
		public RepositoryAttribute(string name)
		{
			m_name = name;
		}
	}
}
