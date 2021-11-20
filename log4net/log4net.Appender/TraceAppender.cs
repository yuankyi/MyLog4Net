#define TRACE
using log4net.Core;
using log4net.Layout;
using System;
using System.Diagnostics;

namespace log4net.Appender
{
	/// <summary>
	/// Appends log events to the <see cref="T:System.Diagnostics.Trace" /> system.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The application configuration file can be used to control what listeners 
	/// are actually used. See the MSDN documentation for the 
	/// <see cref="T:System.Diagnostics.Trace" /> class for details on configuring the
	/// trace system.
	/// </para>
	/// <para>
	/// Events are written using the <c>System.Diagnostics.Trace.Write(string,string)</c>
	/// method. The event's logger name is the default value for the category parameter 
	/// of the Write method. 
	/// </para>
	/// <para>
	/// <b>Compact Framework</b><br />
	/// The Compact Framework does not support the <see cref="T:System.Diagnostics.Trace" />
	/// class for any operation except <c>Assert</c>. When using the Compact Framework this
	/// appender will write to the <see cref="T:System.Diagnostics.Debug" /> system rather than
	/// the Trace system. This appender will therefore behave like the <see cref="T:log4net.Appender.DebugAppender" />.
	/// </para>
	/// </remarks>
	/// <author>Douglas de la Torre</author>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	/// <author>Ron Grabowski</author>
	public class TraceAppender : AppenderSkeleton
	{
		/// <summary>
		/// Immediate flush means that the underlying writer or output stream
		/// will be flushed at the end of each append operation.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Immediate flush is slower but ensures that each append request is 
		/// actually written. If <see cref="P:log4net.Appender.TraceAppender.ImmediateFlush" /> is set to
		/// <c>false</c>, then there is a good chance that the last few
		/// logs events are not actually written to persistent media if and
		/// when the application crashes.
		/// </para>
		/// <para>
		/// The default value is <c>true</c>.</para>
		/// </remarks>
		private bool m_immediateFlush = true;

		/// <summary>
		/// Defaults to %logger
		/// </summary>
		private PatternLayout m_category = new PatternLayout("%logger");

		/// <summary>
		/// Gets or sets a value that indicates whether the appender will 
		/// flush at the end of each write.
		/// </summary>
		/// <remarks>
		/// <para>The default behavior is to flush at the end of each 
		/// write. If the option is set to<c>false</c>, then the underlying 
		/// stream can defer writing to physical medium to a later time. 
		/// </para>
		/// <para>
		/// Avoiding the flush operation at the end of each append results 
		/// in a performance gain of 10 to 20 percent. However, there is safety
		/// trade-off involved in skipping flushing. Indeed, when flushing is
		/// skipped, then it is likely that the last few log events will not
		/// be recorded on disk when the application exits. This is a high
		/// price to pay even for a 20% performance gain.
		/// </para>
		/// </remarks>
		public bool ImmediateFlush
		{
			get
			{
				return m_immediateFlush;
			}
			set
			{
				m_immediateFlush = value;
			}
		}

		/// <summary>
		/// The category parameter sent to the Trace method.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Defaults to %logger which will use the logger name of the current 
		/// <see cref="T:log4net.Core.LoggingEvent" /> as the category parameter.
		/// </para>
		/// <para>
		/// </para> 
		/// </remarks>
		public PatternLayout Category
		{
			get
			{
				return m_category;
			}
			set
			{
				m_category = value;
			}
		}

		/// <summary>
		/// This appender requires a <see cref="N:log4net.Layout" /> to be set.
		/// </summary>
		/// <value><c>true</c></value>
		/// <remarks>
		/// <para>
		/// This appender requires a <see cref="N:log4net.Layout" /> to be set.
		/// </para>
		/// </remarks>
		protected override bool RequiresLayout
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:log4net.Appender.TraceAppender" />.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Default constructor.
		/// </para>
		/// </remarks>
		public TraceAppender()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:log4net.Appender.TraceAppender" /> 
		/// with a specified layout.
		/// </summary>
		/// <param name="layout">The layout to use with this appender.</param>
		/// <remarks>
		/// <para>
		/// Obsolete constructor.
		/// </para>
		/// </remarks>
		[Obsolete("Instead use the default constructor and set the Layout property")]
		public TraceAppender(ILayout layout)
		{
			Layout = layout;
		}

		/// <summary>
		/// Writes the logging event to the <see cref="T:System.Diagnostics.Trace" /> system.
		/// </summary>
		/// <param name="loggingEvent">The event to log.</param>
		/// <remarks>
		/// <para>
		/// Writes the logging event to the <see cref="T:System.Diagnostics.Trace" /> system.
		/// </para>
		/// </remarks>
		protected override void Append(LoggingEvent loggingEvent)
		{
			Trace.Write(RenderLoggingEvent(loggingEvent), m_category.Format(loggingEvent));
			if (m_immediateFlush)
			{
				Trace.Flush();
			}
		}

		/// <summary>
		/// Flushes any buffered log data.
		/// </summary>
		/// <param name="millisecondsTimeout">The maximum time to wait for logging events to be flushed.</param>
		/// <returns><c>True</c> if all logging events were flushed successfully, else <c>false</c>.</returns>
		public override bool Flush(int millisecondsTimeout)
		{
			if (m_immediateFlush)
			{
				return true;
			}
			Trace.Flush();
			return true;
		}
	}
}
