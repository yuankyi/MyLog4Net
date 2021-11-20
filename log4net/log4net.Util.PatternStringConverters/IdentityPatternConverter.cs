using System;
using System.IO;
using System.Security;
using System.Threading;

namespace log4net.Util.PatternStringConverters
{
	/// <summary>
	/// Write the current thread identity to the output
	/// </summary>
	/// <remarks>
	/// <para>
	/// Write the current thread identity to the output writer
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	internal sealed class IdentityPatternConverter : PatternConverter
	{
		/// <summary>
		/// The fully qualified type of the IdentityPatternConverter class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(IdentityPatternConverter);

		/// <summary>
		/// Write the current thread identity to the output
		/// </summary>
		/// <param name="writer">the writer to write to</param>
		/// <param name="state">null, state is not set</param>
		/// <remarks>
		/// <para>
		/// Writes the current thread identity to the output <paramref name="writer" />.
		/// </para>
		/// </remarks>
		protected override void Convert(TextWriter writer, object state)
		{
			try
			{
				if (Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity != null && Thread.CurrentPrincipal.Identity.Name != null)
				{
					writer.Write(Thread.CurrentPrincipal.Identity.Name);
				}
			}
			catch (SecurityException)
			{
				LogLog.Debug(declaringType, "Security exception while trying to get current thread principal. Error Ignored.");
				writer.Write(SystemInfo.NotAvailableText);
			}
		}
	}
}
