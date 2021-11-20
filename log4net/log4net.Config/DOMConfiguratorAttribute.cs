using System;

namespace log4net.Config
{
	/// <summary>
	/// Assembly level attribute to configure the <see cref="T:log4net.Config.XmlConfigurator" />.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>AliasDomainAttribute is obsolete. Use AliasRepositoryAttribute instead of AliasDomainAttribute.</b>
	/// </para>
	/// <para>
	/// This attribute may only be used at the assembly scope and can only
	/// be used once per assembly.
	/// </para>
	/// <para>
	/// Use this attribute to configure the <see cref="T:log4net.Config.XmlConfigurator" />
	/// without calling one of the <see cref="M:XmlConfigurator.Configure()" />
	/// methods.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	[Serializable]
	[AttributeUsage(AttributeTargets.Assembly)]
	[Obsolete("Use XmlConfiguratorAttribute instead of DOMConfiguratorAttribute")]
	public sealed class DOMConfiguratorAttribute : XmlConfiguratorAttribute
	{
	}
}
