#define DEBUG
using log4net.Core;
using log4net.Layout;
using System;
using System.Diagnostics;

namespace log4net.Appender
{
	/// <summary>
	/// Appends log events to the <see cref="T:System.Diagnostics.Debug" /> system.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The application configuration file can be used to control what listeners 
	/// are actually used. See the MSDN documentation for the 
	/// <see cref="T:System.Diagnostics.Debug" /> class for details on configuring the
	/// debug system.
	/// </para>
	/// <para>
	/// Events are written using the <see cref="M:System.Diagnostics.Debug.Write(string,string)" />
	/// method. The event's logger name is passed as the value for the category name to the Write method.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	public class DebugAppender : AppenderSkeleton
	{
		/// <summary>
		/// Immediate flush means that the underlying writer or output stream
		/// will be flushed at the end of each append operation.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Immediate flush is slower but ensures that each append request is 
		/// actually written. If <see cref="P:log4net.Appender.DebugAppender.ImmediateFlush" /> is set to
		/// <c>false</c>, then there is a good chance that the last few
		/// logs events are not actually written to persistent media if and
		/// when the application crashes.
		/// </para>
		/// <para>
		/// The default value is <c>true</c>.</para>
		/// </remarks>
		private bool m_immediateFlush = true;

		/// <summary>
		/// Defaults to a <see cref="T:log4net.Layout.PatternLayout" /> with %logger as the pattern.
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
		/// Formats the category parameter sent to the Debug method.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Defaults to a <see cref="T:log4net.Layout.PatternLayout" /> with %logger as the pattern which will use the logger name of the current 
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
		/// Initializes a new instance of the <see cref="T:log4net.Appender.DebugAppender" />.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Default constructor.
		/// </para>
		/// </remarks>
		public DebugAppender()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:log4net.Appender.DebugAppender" /> 
		/// with a specified layout.
		/// </summary>
		/// <param name="layout">The layout to use with this appender.</param>
		/// <remarks>
		/// <para>
		/// Obsolete constructor.
		/// </para>
		/// </remarks>
		[Obsolete("Instead use the default constructor and set the Layout property")]
		public DebugAppender(ILayout layout)
		{
			Layout = layout;
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
			Debug.Flush();
			return true;
		}

		/// <summary>
		/// Writes the logging event to the <see cref="T:System.Diagnostics.Debug" /> system.
		/// </summary>
		/// <param name="loggingEvent">The event to log.</param>
		/// <remarks>
		/// <para>
		/// Writes the logging event to the <see cref="T:System.Diagnostics.Debug" /> system.
		/// If <see cref="P:log4net.Appender.DebugAppender.ImmediateFlush" /> is <c>true</c> then the <see cref="M:System.Diagnostics.Debug.Flush" />
		/// is called.
		/// </para>
		/// </remarks> 
		protected override void Append(LoggingEvent loggingEvent)
		{
			if (m_category == null)
			{
				Debug.Write(RenderLoggingEvent(loggingEvent));
			}
			else
			{
				string text = m_category.Format(loggingEvent);
				if (string.IsNullOrEmpty(text))
				{
					Debug.Write(RenderLoggingEvent(loggingEvent));
				}
				else
				{
					Debug.Write(RenderLoggingEvent(loggingEvent), text);
				}
			}
			if (m_immediateFlush)
			{
				Debug.Flush();
			}
		}
	}
}
