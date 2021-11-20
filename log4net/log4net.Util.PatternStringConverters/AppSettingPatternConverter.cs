using System.Collections;
using System.Configuration;
using System.IO;

namespace log4net.Util.PatternStringConverters
{
	/// <summary>
	/// AppSetting pattern converter
	/// </summary>
	/// <remarks>
	/// <para>
	/// This pattern converter reads appSettings from the application configuration file.
	/// </para>
	/// <para>
	/// If the <see cref="P:log4net.Util.PatternConverter.Option" /> is specified then that will be used to
	/// lookup a single appSettings value. If no <see cref="P:log4net.Util.PatternConverter.Option" /> is specified
	/// then all appSettings will be dumped as a list of key value pairs.
	/// </para>
	/// <para>
	/// A typical use is to specify a base directory for log files, e.g.
	/// <example>
	/// <![CDATA[
	/// <log4net>
	///    <appender name="MyAppender" type="log4net.Appender.RollingFileAppender">
	///      <file type="log4net.Util.PatternString" value="appsetting{LogDirectory}MyApp.log"/>
	///       ...
	///   </appender>
	/// </log4net>
	/// ]]>
	/// </example>
	/// </para>
	/// </remarks>
	internal sealed class AppSettingPatternConverter : PatternConverter
	{
		private static Hashtable _appSettingsHashTable;

		private static IDictionary AppSettingsDictionary
		{
			get
			{
				if (_appSettingsHashTable == null)
				{
					Hashtable hashtable = new Hashtable();
					foreach (string appSetting in ConfigurationManager.AppSettings)
					{
						hashtable.Add(appSetting, ConfigurationManager.AppSettings[appSetting]);
					}
					_appSettingsHashTable = hashtable;
				}
				return _appSettingsHashTable;
			}
		}

		/// <summary>
		/// Write the property value to the output
		/// </summary>
		/// <param name="writer"><see cref="T:System.IO.TextWriter" /> that will receive the formatted result.</param>
		/// <param name="state">null, state is not set</param>
		/// <remarks>
		/// <para>
		/// Writes out the value of a named property. The property name
		/// should be set in the <see cref="P:log4net.Util.PatternConverter.Option" />
		/// property.
		/// </para>
		/// <para>
		/// If the <see cref="P:log4net.Util.PatternConverter.Option" /> is set to <c>null</c>
		/// then all the properties are written as key value pairs.
		/// </para>
		/// </remarks>
		protected override void Convert(TextWriter writer, object state)
		{
			if (Option != null)
			{
				PatternConverter.WriteObject(writer, null, ConfigurationManager.AppSettings[Option]);
			}
			else
			{
				PatternConverter.WriteDictionary(writer, null, AppSettingsDictionary);
			}
		}
	}
}
