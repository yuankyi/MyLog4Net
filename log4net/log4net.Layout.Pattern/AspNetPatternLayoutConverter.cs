using log4net.Core;
using log4net.Util;
using System.IO;
using System.Web;

namespace log4net.Layout.Pattern
{
	/// <summary>
	/// Abstract class that provides access to the current HttpContext (<see cref="P:System.Web.HttpContext.Current" />) that 
	/// derived classes need.
	/// </summary>
	/// <remarks>
	/// This class handles the case when HttpContext.Current is null by writing
	/// <see cref="P:log4net.Util.SystemInfo.NotAvailableText" /> to the writer.
	/// </remarks>
	/// <author>Ron Grabowski</author>
	internal abstract class AspNetPatternLayoutConverter : PatternLayoutConverter
	{
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
		{
			if (HttpContext.Current == null)
			{
				writer.Write(SystemInfo.NotAvailableText);
			}
			else
			{
				Convert(writer, loggingEvent, HttpContext.Current);
			}
		}

		/// <summary>
		/// Derived pattern converters must override this method in order to
		/// convert conversion specifiers in the correct way.
		/// </summary>
		/// <param name="writer"><see cref="T:System.IO.TextWriter" /> that will receive the formatted result.</param>
		/// <param name="loggingEvent">The <see cref="T:log4net.Core.LoggingEvent" /> on which the pattern converter should be executed.</param>
		/// <param name="httpContext">The <see cref="T:System.Web.HttpContext" /> under which the ASP.Net request is running.</param>
		protected abstract void Convert(TextWriter writer, LoggingEvent loggingEvent, HttpContext httpContext);
	}
}
