using System;
using System.IO;
using System.Security;

namespace log4net.Util.PatternStringConverters
{
	/// <summary>
	/// Write an <see cref="T:System.Environment.SpecialFolder" /> folder path to the output
	/// </summary>
	/// <remarks>
	/// <para>
	/// Write an special path environment folder path to the output writer.
	/// The value of the <see cref="P:log4net.Util.PatternConverter.Option" /> determines 
	/// the name of the variable to output. <see cref="P:log4net.Util.PatternConverter.Option" />
	/// should be a value in the <see cref="T:System.Environment.SpecialFolder" /> enumeration.
	/// </para>
	/// </remarks>
	/// <author>Ron Grabowski</author>
	internal sealed class EnvironmentFolderPathPatternConverter : PatternConverter
	{
		/// <summary>
		/// The fully qualified type of the EnvironmentFolderPathPatternConverter class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(EnvironmentFolderPathPatternConverter);

		/// <summary>
		/// Write an special path environment folder path to the output
		/// </summary>
		/// <param name="writer">the writer to write to</param>
		/// <param name="state">null, state is not set</param>
		/// <remarks>
		/// <para>
		/// Writes the special path environment folder path to the output <paramref name="writer" />.
		/// The name of the special path environment folder path to output must be set
		/// using the <see cref="P:log4net.Util.PatternConverter.Option" />
		/// property.
		/// </para>
		/// </remarks>
		protected override void Convert(TextWriter writer, object state)
		{
			try
			{
				if (Option != null && Option.Length > 0)
				{
					string folderPath = Environment.GetFolderPath((Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), Option, true));
					if (folderPath != null && folderPath.Length > 0)
					{
						writer.Write(folderPath);
					}
				}
			}
			catch (SecurityException exception)
			{
				LogLog.Debug(declaringType, "Security exception while trying to expand environment variables. Error Ignored. No Expansion.", exception);
			}
			catch (Exception exception2)
			{
				LogLog.Error(declaringType, "Error occurred while converting environment variable.", exception2);
			}
		}
	}
}
