using System;
using System.IO;
using System.Security;

namespace log4net.Util.PatternStringConverters
{
	/// <summary>
	/// Write an environment variable to the output
	/// </summary>
	/// <remarks>
	/// <para>
	/// Write an environment variable to the output writer.
	/// The value of the <see cref="P:log4net.Util.PatternConverter.Option" /> determines 
	/// the name of the variable to output.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	internal sealed class EnvironmentPatternConverter : PatternConverter
	{
		/// <summary>
		/// The fully qualified type of the EnvironmentPatternConverter class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(EnvironmentPatternConverter);

		/// <summary>
		/// Write an environment variable to the output
		/// </summary>
		/// <param name="writer">the writer to write to</param>
		/// <param name="state">null, state is not set</param>
		/// <remarks>
		/// <para>
		/// Writes the environment variable to the output <paramref name="writer" />.
		/// The name of the environment variable to output must be set
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
					string environmentVariable = Environment.GetEnvironmentVariable(Option);
					if (environmentVariable == null)
					{
						environmentVariable = Environment.GetEnvironmentVariable(Option, EnvironmentVariableTarget.User);
					}
					if (environmentVariable == null)
					{
						environmentVariable = Environment.GetEnvironmentVariable(Option, EnvironmentVariableTarget.Machine);
					}
					if (environmentVariable != null && environmentVariable.Length > 0)
					{
						writer.Write(environmentVariable);
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
