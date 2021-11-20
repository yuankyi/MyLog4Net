using log4net.Core;
using System;
using System.Collections;
using System.Globalization;

namespace log4net.Util
{
	/// <summary>
	/// Most of the work of the <see cref="T:log4net.Layout.PatternLayout" /> class
	/// is delegated to the PatternParser class.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <c>PatternParser</c> processes a pattern string and
	/// returns a chain of <see cref="T:log4net.Util.PatternConverter" /> objects.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	public sealed class PatternParser
	{
		/// <summary>
		/// Sort strings by length
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="T:System.Collections.IComparer" /> that orders strings by string length.
		/// The longest strings are placed first
		/// </para>
		/// </remarks>
		private sealed class StringLengthComparer : IComparer
		{
			public static readonly StringLengthComparer Instance = new StringLengthComparer();

			private StringLengthComparer()
			{
			}

			public int Compare(object x, object y)
			{
				string text = x as string;
				string text2 = y as string;
				if (text == null && text2 == null)
				{
					return 0;
				}
				if (text == null)
				{
					return 1;
				}
				if (text2 == null)
				{
					return -1;
				}
				return text2.Length.CompareTo(text.Length);
			}
		}

		private const char ESCAPE_CHAR = '%';

		/// <summary>
		/// The first pattern converter in the chain
		/// </summary>
		private PatternConverter m_head;

		/// <summary>
		///  the last pattern converter in the chain
		/// </summary>
		private PatternConverter m_tail;

		/// <summary>
		/// The pattern
		/// </summary>
		private string m_pattern;

		/// <summary>
		/// Internal map of converter identifiers to converter types
		/// </summary>
		/// <remarks>
		/// <para>
		/// This map overrides the static s_globalRulesRegistry map.
		/// </para>
		/// </remarks>
		private Hashtable m_patternConverters = new Hashtable();

		/// <summary>
		/// The fully qualified type of the PatternParser class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(PatternParser);

		/// <summary>
		/// Get the converter registry used by this parser
		/// </summary>
		/// <value>
		/// The converter registry used by this parser
		/// </value>
		/// <remarks>
		/// <para>
		/// Get the converter registry used by this parser
		/// </para>
		/// </remarks>
		public Hashtable PatternConverters
		{
			get
			{
				return m_patternConverters;
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="pattern">The pattern to parse.</param>
		/// <remarks>
		/// <para>
		/// Initializes a new instance of the <see cref="T:log4net.Util.PatternParser" /> class 
		/// with the specified pattern string.
		/// </para>
		/// </remarks>
		public PatternParser(string pattern)
		{
			m_pattern = pattern;
		}

		/// <summary>
		/// Parses the pattern into a chain of pattern converters.
		/// </summary>
		/// <returns>The head of a chain of pattern converters.</returns>
		/// <remarks>
		/// <para>
		/// Parses the pattern into a chain of pattern converters.
		/// </para>
		/// </remarks>
		public PatternConverter Parse()
		{
			string[] matches = BuildCache();
			ParseInternal(m_pattern, matches);
			return m_head;
		}

		/// <summary>
		/// Build the unified cache of converters from the static and instance maps
		/// </summary>
		/// <returns>the list of all the converter names</returns>
		/// <remarks>
		/// <para>
		/// Build the unified cache of converters from the static and instance maps
		/// </para>
		/// </remarks>
		private string[] BuildCache()
		{
			string[] array = new string[m_patternConverters.Keys.Count];
			m_patternConverters.Keys.CopyTo(array, 0);
			Array.Sort(array, 0, array.Length, StringLengthComparer.Instance);
			return array;
		}

		/// <summary>
		/// Internal method to parse the specified pattern to find specified matches
		/// </summary>
		/// <param name="pattern">the pattern to parse</param>
		/// <param name="matches">the converter names to match in the pattern</param>
		/// <remarks>
		/// <para>
		/// The matches param must be sorted such that longer strings come before shorter ones.
		/// </para>
		/// </remarks>
		private void ParseInternal(string pattern, string[] matches)
		{
			int i = 0;
			while (i < pattern.Length)
			{
				int num = pattern.IndexOf('%', i);
				if (num < 0 || num == pattern.Length - 1)
				{
					ProcessLiteral(pattern.Substring(i));
					i = pattern.Length;
				}
				else if (pattern[num + 1] == '%')
				{
					ProcessLiteral(pattern.Substring(i, num - i + 1));
					i = num + 2;
				}
				else
				{
					ProcessLiteral(pattern.Substring(i, num - i));
					i = num + 1;
					FormattingInfo formattingInfo = new FormattingInfo();
					if (i < pattern.Length && pattern[i] == '-')
					{
						formattingInfo.LeftAlign = true;
						i++;
					}
					for (; i < pattern.Length && char.IsDigit(pattern[i]); i++)
					{
						if (formattingInfo.Min < 0)
						{
							formattingInfo.Min = 0;
						}
						formattingInfo.Min = formattingInfo.Min * 10 + int.Parse(pattern[i].ToString(), NumberFormatInfo.InvariantInfo);
					}
					if (i < pattern.Length && pattern[i] == '.')
					{
						i++;
					}
					for (; i < pattern.Length && char.IsDigit(pattern[i]); i++)
					{
						if (formattingInfo.Max == 2147483647)
						{
							formattingInfo.Max = 0;
						}
						formattingInfo.Max = formattingInfo.Max * 10 + int.Parse(pattern[i].ToString(), NumberFormatInfo.InvariantInfo);
					}
					int num2 = pattern.Length - i;
					for (int j = 0; j < matches.Length; j++)
					{
						string text = matches[j];
						if (text.Length <= num2 && string.Compare(pattern, i, text, 0, text.Length) == 0)
						{
							i += matches[j].Length;
							string option = null;
							if (i < pattern.Length && pattern[i] == '{')
							{
								i++;
								int num3 = pattern.IndexOf('}', i);
								if (num3 >= 0)
								{
									option = pattern.Substring(i, num3 - i);
									i = num3 + 1;
								}
							}
							ProcessConverter(matches[j], option, formattingInfo);
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Process a parsed literal
		/// </summary>
		/// <param name="text">the literal text</param>
		private void ProcessLiteral(string text)
		{
			if (text.Length > 0)
			{
				ProcessConverter("literal", text, new FormattingInfo());
			}
		}

		/// <summary>
		/// Process a parsed converter pattern
		/// </summary>
		/// <param name="converterName">the name of the converter</param>
		/// <param name="option">the optional option for the converter</param>
		/// <param name="formattingInfo">the formatting info for the converter</param>
		private void ProcessConverter(string converterName, string option, FormattingInfo formattingInfo)
		{
			LogLog.Debug(declaringType, "Converter [" + converterName + "] Option [" + option + "] Format [min=" + formattingInfo.Min.ToString() + ",max=" + formattingInfo.Max.ToString() + ",leftAlign=" + formattingInfo.LeftAlign.ToString() + "]");
			ConverterInfo converterInfo = (ConverterInfo)m_patternConverters[converterName];
			if (converterInfo == null)
			{
				LogLog.Error(declaringType, "Unknown converter name [" + converterName + "] in conversion pattern.");
			}
			else
			{
				PatternConverter patternConverter = null;
				try
				{
					patternConverter = (PatternConverter)Activator.CreateInstance(converterInfo.Type);
				}
				catch (Exception ex)
				{
					LogLog.Error(declaringType, "Failed to create instance of Type [" + converterInfo.Type.FullName + "] using default constructor. Exception: " + ex.ToString());
				}
				patternConverter.FormattingInfo = formattingInfo;
				patternConverter.Option = option;
				patternConverter.Properties = converterInfo.Properties;
				IOptionHandler optionHandler = patternConverter as IOptionHandler;
				if (optionHandler != null)
				{
					optionHandler.ActivateOptions();
				}
				AddConverter(patternConverter);
			}
		}

		/// <summary>
		/// Resets the internal state of the parser and adds the specified pattern converter 
		/// to the chain.
		/// </summary>
		/// <param name="pc">The pattern converter to add.</param>
		private void AddConverter(PatternConverter pc)
		{
			if (m_head == null)
			{
				m_head = (m_tail = pc);
			}
			else
			{
				m_tail = m_tail.SetNext(pc);
			}
		}
	}
}
