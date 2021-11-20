using log4net.Core;
using log4net.Util;
using System.IO;
using System.Web;

namespace log4net.Layout.Pattern
{
	/// <summary>
	/// Converter for items in the <see cref="T:System.Web.HttpContext" />.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Outputs an item from the <see cref="T:System.Web.HttpContext" />.
	/// </para>
	/// </remarks>
	/// <author>Ron Grabowski</author>
	internal sealed class AspNetContextPatternConverter : AspNetPatternLayoutConverter
	{
		/// <summary>
		/// Write the ASP.Net HttpContext item to the output
		/// </summary>
		/// <param name="writer"><see cref="T:System.IO.TextWriter" /> that will receive the formatted result.</param>
		/// <param name="loggingEvent">The <see cref="T:log4net.Core.LoggingEvent" /> on which the pattern converter should be executed.</param>
		/// <param name="httpContext">The <see cref="T:System.Web.HttpContext" /> under which the ASP.Net request is running.</param>
		/// <remarks>
		/// <para>
		/// Writes out the value of a named property. The property name
		/// should be set in the <see cref="P:log4net.Util.PatternConverter.Option" />
		/// property.
		/// </para>
		/// </remarks>
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent, HttpContext httpContext)
		{
			if (Option != null)
			{
				PatternConverter.WriteObject(writer, loggingEvent.Repository, httpContext.Items[Option]);
			}
			else
			{
				PatternConverter.WriteObject(writer, loggingEvent.Repository, httpContext.Items);
			}
		}
	}
}
