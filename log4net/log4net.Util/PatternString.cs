using log4net.Core;
using log4net.Util.PatternStringConverters;
using System;
using System.Collections;
using System.Globalization;
using System.IO;

namespace log4net.Util
{
	/// <summary>
	/// This class implements a patterned string.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This string has embedded patterns that are resolved and expanded
	/// when the string is formatted.
	/// </para>
	/// <para>
	/// This class functions similarly to the <see cref="T:log4net.Layout.PatternLayout" />
	/// in that it accepts a pattern and renders it to a string. Unlike the 
	/// <see cref="T:log4net.Layout.PatternLayout" /> however the <c>PatternString</c>
	/// does not render the properties of a specific <see cref="T:log4net.Core.LoggingEvent" /> but
	/// of the process in general.
	/// </para>
	/// <para>
	/// The recognized conversion pattern names are:
	/// </para>
	/// <list type="table">
	///     <listheader>
	///         <term>Conversion Pattern Name</term>
	///         <description>Effect</description>
	///     </listheader>
	///     <item>
	///         <term>appdomain</term>
	///         <description>
	///             <para>
	///             Used to output the friendly name of the current AppDomain.
	///             </para>
	///         </description>
	///     </item>
	///     <item>
	///         <term>appsetting</term>
	///         <description>
	///             <para>
	///             Used to output the value of a specific appSetting key in the application
	///             configuration file.
	///             </para>
	///         </description>
	///     </item>
	///     <item>
	///         <term>date</term>
	///         <description>
	/// 			<para>
	/// 			Used to output the current date and time in the local time zone. 
	/// 			To output the date in universal time use the <c>%utcdate</c> pattern.
	/// 			The date conversion 
	/// 			specifier may be followed by a <i>date format specifier</i> enclosed 
	/// 			between braces. For example, <b>%date{HH:mm:ss,fff}</b> or
	/// 			<b>%date{dd MMM yyyy HH:mm:ss,fff}</b>.  If no date format specifier is 
	/// 			given then ISO8601 format is
	/// 			assumed (<see cref="T:log4net.DateFormatter.Iso8601DateFormatter" />).
	/// 			</para>
	/// 			<para>
	/// 			The date format specifier admits the same syntax as the
	/// 			time pattern string of the <see cref="M:DateTime.ToString(string)" />.
	/// 			</para>
	/// 			<para>
	/// 			For better results it is recommended to use the log4net date
	/// 			formatters. These can be specified using one of the strings
	/// 			"ABSOLUTE", "DATE" and "ISO8601" for specifying 
	/// 			<see cref="T:log4net.DateFormatter.AbsoluteTimeDateFormatter" />, 
	/// 			<see cref="T:log4net.DateFormatter.DateTimeDateFormatter" /> and respectively 
	/// 			<see cref="T:log4net.DateFormatter.Iso8601DateFormatter" />. For example, 
	/// 			<b>%date{ISO8601}</b> or <b>%date{ABSOLUTE}</b>.
	/// 			</para>
	/// 			<para>
	/// 			These dedicated date formatters perform significantly
	/// 			better than <see cref="M:DateTime.ToString(string)" />.
	/// 			</para>
	///         </description>
	///     </item>
	///     <item>
	///         <term>env</term>
	///         <description>
	///             <para>
	/// 			Used to output the a specific environment variable. The key to 
	/// 			lookup must be specified within braces and directly following the
	/// 			pattern specifier, e.g. <b>%env{COMPUTERNAME}</b> would include the value
	/// 			of the <c>COMPUTERNAME</c> environment variable.
	///             </para>
	///             <para>
	///             The <c>env</c> pattern is not supported on the .NET Compact Framework.
	///             </para>
	///         </description>
	///     </item>
	///     <item>
	///         <term>identity</term>
	///         <description>
	/// 			<para>
	/// 			Used to output the user name for the currently active user
	/// 			(Principal.Identity.Name).
	/// 			</para>
	///         </description>
	///     </item>
	///     <item>
	///         <term>newline</term>
	///         <description>
	/// 			<para>
	/// 			Outputs the platform dependent line separator character or
	/// 			characters.
	/// 			</para>
	/// 			<para>
	/// 			This conversion pattern name offers the same performance as using 
	/// 			non-portable line separator strings such as	"\n", or "\r\n". 
	/// 			Thus, it is the preferred way of specifying a line separator.
	/// 			</para> 
	///         </description>
	///     </item>
	///     <item>
	///         <term>processid</term>
	///         <description>
	///             <para>
	/// 			Used to output the system process ID for the current process.
	///             </para>
	///         </description>
	///     </item>
	///     <item>
	///         <term>property</term>
	///         <description>
	/// 			<para>
	/// 			Used to output a specific context property. The key to 
	/// 			lookup must be specified within braces and directly following the
	/// 			pattern specifier, e.g. <b>%property{user}</b> would include the value
	/// 			from the property that is keyed by the string 'user'. Each property value
	/// 			that is to be included in the log must be specified separately.
	/// 			Properties are stored in logging contexts. By default 
	/// 			the <c>log4net:HostName</c> property is set to the name of machine on 
	/// 			which the event was originally logged.
	/// 			</para>
	/// 			<para>
	/// 			If no key is specified, e.g. <b>%property</b> then all the keys and their
	/// 			values are printed in a comma separated list.
	/// 			</para>
	/// 			<para>
	/// 			The properties of an event are combined from a number of different
	/// 			contexts. These are listed below in the order in which they are searched.
	/// 			</para>
	/// 			<list type="definition">
	/// 				<item>
	/// 					<term>the thread properties</term>
	/// 					<description>
	/// 					The <see cref="P:log4net.ThreadContext.Properties" /> that are set on the current
	/// 					thread. These properties are shared by all events logged on this thread.
	/// 					</description>
	/// 				</item>
	/// 				<item>
	/// 					<term>the global properties</term>
	/// 					<description>
	/// 					The <see cref="P:log4net.GlobalContext.Properties" /> that are set globally. These 
	/// 					properties are shared by all the threads in the AppDomain.
	/// 					</description>
	/// 				</item>
	/// 			</list>
	///         </description>
	///     </item>
	///     <item>
	///         <term>random</term>
	///         <description>
	///             <para>
	///             Used to output a random string of characters. The string is made up of
	///             uppercase letters and numbers. By default the string is 4 characters long.
	///             The length of the string can be specified within braces directly following the
	/// 			pattern specifier, e.g. <b>%random{8}</b> would output an 8 character string.
	///             </para>
	///         </description>
	///     </item>
	///     <item>
	///         <term>username</term>
	///         <description>
	/// 			<para>
	/// 			Used to output the WindowsIdentity for the currently
	/// 			active user.
	/// 			</para>
	///         </description>
	///     </item>
	///     <item>
	///         <term>utcdate</term>
	///         <description>
	/// 			<para>
	/// 			Used to output the date of the logging event in universal time. 
	/// 			The date conversion 
	/// 			specifier may be followed by a <i>date format specifier</i> enclosed 
	/// 			between braces. For example, <b>%utcdate{HH:mm:ss,fff}</b> or
	/// 			<b>%utcdate{dd MMM yyyy HH:mm:ss,fff}</b>.  If no date format specifier is 
	/// 			given then ISO8601 format is
	/// 			assumed (<see cref="T:log4net.DateFormatter.Iso8601DateFormatter" />).
	/// 			</para>
	/// 			<para>
	/// 			The date format specifier admits the same syntax as the
	/// 			time pattern string of the <see cref="M:DateTime.ToString(string)" />.
	/// 			</para>
	/// 			<para>
	/// 			For better results it is recommended to use the log4net date
	/// 			formatters. These can be specified using one of the strings
	/// 			"ABSOLUTE", "DATE" and "ISO8601" for specifying 
	/// 			<see cref="T:log4net.DateFormatter.AbsoluteTimeDateFormatter" />, 
	/// 			<see cref="T:log4net.DateFormatter.DateTimeDateFormatter" /> and respectively 
	/// 			<see cref="T:log4net.DateFormatter.Iso8601DateFormatter" />. For example, 
	/// 			<b>%utcdate{ISO8601}</b> or <b>%utcdate{ABSOLUTE}</b>.
	/// 			</para>
	/// 			<para>
	/// 			These dedicated date formatters perform significantly
	/// 			better than <see cref="M:DateTime.ToString(string)" />.
	/// 			</para>
	///         </description>
	///     </item>
	/// 	<item>
	/// 		<term>%</term>
	/// 		<description>
	/// 			<para>
	/// 			The sequence %% outputs a single percent sign.
	/// 			</para>
	/// 		</description>
	/// 	</item>
	/// </list>
	/// <para>
	/// Additional pattern converters may be registered with a specific <see cref="T:log4net.Util.PatternString" />
	/// instance using <see cref="M:AddConverter(ConverterInfo)" /> or
	/// <see cref="M:AddConverter(string, Type)" />.
	/// </para>
	/// <para>
	/// See the <see cref="T:log4net.Layout.PatternLayout" /> for details on the 
	/// <i>format modifiers</i> supported by the patterns.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	public class PatternString : IOptionHandler
	{
		/// <summary>
		/// Internal map of converter identifiers to converter types.
		/// </summary>
		private static Hashtable s_globalRulesRegistry;

		/// <summary>
		/// the pattern
		/// </summary>
		private string m_pattern;

		/// <summary>
		/// the head of the pattern converter chain
		/// </summary>
		private PatternConverter m_head;

		/// <summary>
		/// patterns defined on this PatternString only
		/// </summary>
		private Hashtable m_instanceRulesRegistry = new Hashtable();

		/// <summary>
		/// Gets or sets the pattern formatting string
		/// </summary>
		/// <value>
		/// The pattern formatting string
		/// </value>
		/// <remarks>
		/// <para>
		/// The <b>ConversionPattern</b> option. This is the string which
		/// controls formatting and consists of a mix of literal content and
		/// conversion specifiers.
		/// </para>
		/// </remarks>
		public string ConversionPattern
		{
			get
			{
				return m_pattern;
			}
			set
			{
				m_pattern = value;
			}
		}

		/// <summary>
		/// Initialize the global registry
		/// </summary>
		static PatternString()
		{
			s_globalRulesRegistry = new Hashtable(18);
			s_globalRulesRegistry.Add("appdomain", typeof(AppDomainPatternConverter));
			s_globalRulesRegistry.Add("date", typeof(DatePatternConverter));
			s_globalRulesRegistry.Add("env", typeof(EnvironmentPatternConverter));
			s_globalRulesRegistry.Add("envFolderPath", typeof(EnvironmentFolderPathPatternConverter));
			s_globalRulesRegistry.Add("identity", typeof(IdentityPatternConverter));
			s_globalRulesRegistry.Add("literal", typeof(LiteralPatternConverter));
			s_globalRulesRegistry.Add("newline", typeof(NewLinePatternConverter));
			s_globalRulesRegistry.Add("processid", typeof(ProcessIdPatternConverter));
			s_globalRulesRegistry.Add("property", typeof(PropertyPatternConverter));
			s_globalRulesRegistry.Add("random", typeof(RandomStringPatternConverter));
			s_globalRulesRegistry.Add("username", typeof(UserNamePatternConverter));
			s_globalRulesRegistry.Add("utcdate", typeof(UtcDatePatternConverter));
			s_globalRulesRegistry.Add("utcDate", typeof(UtcDatePatternConverter));
			s_globalRulesRegistry.Add("UtcDate", typeof(UtcDatePatternConverter));
			s_globalRulesRegistry.Add("appsetting", typeof(AppSettingPatternConverter));
			s_globalRulesRegistry.Add("appSetting", typeof(AppSettingPatternConverter));
			s_globalRulesRegistry.Add("AppSetting", typeof(AppSettingPatternConverter));
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <remarks>
		/// <para>
		/// Initialize a new instance of <see cref="T:log4net.Util.PatternString" />
		/// </para>
		/// </remarks>
		public PatternString()
		{
		}

		/// <summary>
		/// Constructs a PatternString
		/// </summary>
		/// <param name="pattern">The pattern to use with this PatternString</param>
		/// <remarks>
		/// <para>
		/// Initialize a new instance of <see cref="T:log4net.Util.PatternString" /> with the pattern specified.
		/// </para>
		/// </remarks>
		public PatternString(string pattern)
		{
			m_pattern = pattern;
			ActivateOptions();
		}

		/// <summary>
		/// Initialize object options
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is part of the <see cref="T:log4net.Core.IOptionHandler" /> delayed object
		/// activation scheme. The <see cref="M:log4net.Util.PatternString.ActivateOptions" /> method must 
		/// be called on this object after the configuration properties have
		/// been set. Until <see cref="M:log4net.Util.PatternString.ActivateOptions" /> is called this
		/// object is in an undefined state and must not be used. 
		/// </para>
		/// <para>
		/// If any of the configuration properties are modified then 
		/// <see cref="M:log4net.Util.PatternString.ActivateOptions" /> must be called again.
		/// </para>
		/// </remarks>
		public virtual void ActivateOptions()
		{
			m_head = CreatePatternParser(m_pattern).Parse();
		}

		/// <summary>
		/// Create the <see cref="T:log4net.Util.PatternParser" /> used to parse the pattern
		/// </summary>
		/// <param name="pattern">the pattern to parse</param>
		/// <returns>The <see cref="T:log4net.Util.PatternParser" /></returns>
		/// <remarks>
		/// <para>
		/// Returns PatternParser used to parse the conversion string. Subclasses
		/// may override this to return a subclass of PatternParser which recognize
		/// custom conversion pattern name.
		/// </para>
		/// </remarks>
		private PatternParser CreatePatternParser(string pattern)
		{
			PatternParser patternParser = new PatternParser(pattern);
			IDictionaryEnumerator enumerator = s_globalRulesRegistry.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
					ConverterInfo converterInfo = new ConverterInfo();
					converterInfo.Name = (string)dictionaryEntry.Key;
					converterInfo.Type = (Type)dictionaryEntry.Value;
					patternParser.PatternConverters.Add(dictionaryEntry.Key, converterInfo);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			enumerator = m_instanceRulesRegistry.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry dictionaryEntry2 = (DictionaryEntry)enumerator.Current;
					patternParser.PatternConverters[dictionaryEntry2.Key] = dictionaryEntry2.Value;
				}
				return patternParser;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		/// <summary>
		/// Produces a formatted string as specified by the conversion pattern.
		/// </summary>
		/// <param name="writer">The TextWriter to write the formatted event to</param>
		/// <remarks>
		/// <para>
		/// Format the pattern to the <paramref name="writer" />.
		/// </para>
		/// </remarks>
		public void Format(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			for (PatternConverter patternConverter = m_head; patternConverter != null; patternConverter = patternConverter.Next)
			{
				patternConverter.Format(writer, null);
			}
		}

		/// <summary>
		/// Format the pattern as a string
		/// </summary>
		/// <returns>the pattern formatted as a string</returns>
		/// <remarks>
		/// <para>
		/// Format the pattern to a string.
		/// </para>
		/// </remarks>
		public string Format()
		{
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				Format(stringWriter);
				return stringWriter.ToString();
			}
		}

		/// <summary>
		/// Add a converter to this PatternString
		/// </summary>
		/// <param name="converterInfo">the converter info</param>
		/// <remarks>
		/// <para>
		/// This version of the method is used by the configurator.
		/// Programmatic users should use the alternative <see cref="M:AddConverter(string,Type)" /> method.
		/// </para>
		/// </remarks>
		public void AddConverter(ConverterInfo converterInfo)
		{
			if (converterInfo == null)
			{
				throw new ArgumentNullException("converterInfo");
			}
			if (!typeof(PatternConverter).IsAssignableFrom(converterInfo.Type))
			{
				Type type = converterInfo.Type;
				throw new ArgumentException("The converter type specified [" + (((object)type != null) ? type.ToString() : null) + "] must be a subclass of log4net.Util.PatternConverter", "converterInfo");
			}
			m_instanceRulesRegistry[converterInfo.Name] = converterInfo;
		}

		/// <summary>
		/// Add a converter to this PatternString
		/// </summary>
		/// <param name="name">the name of the conversion pattern for this converter</param>
		/// <param name="type">the type of the converter</param>
		/// <remarks>
		/// <para>
		/// Add a converter to this PatternString
		/// </para>
		/// </remarks>
		public void AddConverter(string name, Type type)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			ConverterInfo converterInfo = new ConverterInfo();
			converterInfo.Name = name;
			converterInfo.Type = type;
			AddConverter(converterInfo);
		}
	}
}
