using System;
using System.Diagnostics;
using System.IO;
using System.Security;

namespace log4net.Util.PatternStringConverters
{
	/// <summary>
	/// Write the current process ID to the output
	/// </summary>
	/// <remarks>
	/// <para>
	/// Write the current process ID to the output writer
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	internal sealed class ProcessIdPatternConverter : PatternConverter
	{
		/// <summary>
		/// The fully qualified type of the ProcessIdPatternConverter class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(ProcessIdPatternConverter);

		/// <summary>
		/// Write the current process ID to the output
		/// </summary>
		/// <param name="writer">the writer to write to</param>
		/// <param name="state">null, state is not set</param>
		/// <remarks>
		/// <para>
		/// Write the current process ID to the output <paramref name="writer" />.
		/// </para>
		/// </remarks>
		[SecuritySafeCritical]
		protected override void Convert(TextWriter writer, object state)
		{
			try
			{
				writer.Write(Process.GetCurrentProcess().Id);
			}
			catch (SecurityException)
			{
				LogLog.Debug(declaringType, "Security exception while trying to get current process id. Error Ignored.");
				writer.Write(SystemInfo.NotAvailableText);
			}
		}
	}
}
