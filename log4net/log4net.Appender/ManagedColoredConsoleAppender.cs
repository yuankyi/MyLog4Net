using log4net.Core;
using log4net.Util;
using System;
using System.IO;

namespace log4net.Appender
{
	/// <summary>
	/// Appends colorful logging events to the console, using the .NET 2
	/// built-in capabilities.
	/// </summary>
	/// <remarks>
	/// <para>
	/// ManagedColoredConsoleAppender appends log events to the standard output stream
	/// or the error output stream using a layout specified by the 
	/// user. It also allows the color of a specific type of message to be set.
	/// </para>
	/// <para>
	/// By default, all output is written to the console's standard output stream.
	/// The <see cref="P:log4net.Appender.ManagedColoredConsoleAppender.Target" /> property can be set to direct the output to the
	/// error stream.
	/// </para>
	/// <para>
	/// When configuring the colored console appender, mappings should be
	/// specified to map logging levels to colors. For example:
	/// </para>
	/// <code lang="XML" escaped="true">
	/// <mapping>
	/// 	<level value="ERROR" />
	/// 	<foreColor value="DarkRed" />
	/// 	<backColor value="White" />
	/// </mapping>
	/// <mapping>
	/// 	<level value="WARN" />
	/// 	<foreColor value="Yellow" />
	/// </mapping>
	/// <mapping>
	/// 	<level value="INFO" />
	/// 	<foreColor value="White" />
	/// </mapping>
	/// <mapping>
	/// 	<level value="DEBUG" />
	/// 	<foreColor value="Blue" />
	/// </mapping>
	/// </code>
	/// <para>
	/// The Level is the standard log4net logging level while
	/// ForeColor and BackColor are the values of <see cref="T:System.ConsoleColor" />
	/// enumeration.
	/// </para>
	/// <para>
	/// Based on the ColoredConsoleAppender
	/// </para>
	/// </remarks>
	/// <author>Rick Hobbs</author>
	/// <author>Nicko Cadell</author>
	/// <author>Pavlos Touboulidis</author>
	public class ManagedColoredConsoleAppender : AppenderSkeleton
	{
		/// <summary>
		/// A class to act as a mapping between the level that a logging call is made at and
		/// the color it should be displayed as.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Defines the mapping between a level and the color it should be displayed in.
		/// </para>
		/// </remarks>
		public class LevelColors : LevelMappingEntry
		{
			private ConsoleColor foreColor;

			private bool hasForeColor;

			private ConsoleColor backColor;

			private bool hasBackColor;

			/// <summary>
			/// The mapped foreground color for the specified level
			/// </summary>
			/// <remarks>
			/// <para>
			/// Required property.
			/// The mapped foreground color for the specified level.
			/// </para>
			/// </remarks>
			public ConsoleColor ForeColor
			{
				get
				{
					return foreColor;
				}
				set
				{
					foreColor = value;
					hasForeColor = true;
				}
			}

			internal bool HasForeColor
			{
				get
				{
					return hasForeColor;
				}
			}

			/// <summary>
			/// The mapped background color for the specified level
			/// </summary>
			/// <remarks>
			/// <para>
			/// Required property.
			/// The mapped background color for the specified level.
			/// </para>
			/// </remarks>
			public ConsoleColor BackColor
			{
				get
				{
					return backColor;
				}
				set
				{
					backColor = value;
					hasBackColor = true;
				}
			}

			internal bool HasBackColor
			{
				get
				{
					return hasBackColor;
				}
			}
		}

		/// <summary>
		/// The <see cref="P:log4net.Appender.ManagedColoredConsoleAppender.Target" /> to use when writing to the Console 
		/// standard output stream.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <see cref="P:log4net.Appender.ManagedColoredConsoleAppender.Target" /> to use when writing to the Console 
		/// standard output stream.
		/// </para>
		/// </remarks>
		public const string ConsoleOut = "Console.Out";

		/// <summary>
		/// The <see cref="P:log4net.Appender.ManagedColoredConsoleAppender.Target" /> to use when writing to the Console 
		/// standard error output stream.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <see cref="P:log4net.Appender.ManagedColoredConsoleAppender.Target" /> to use when writing to the Console 
		/// standard error output stream.
		/// </para>
		/// </remarks>
		public const string ConsoleError = "Console.Error";

		/// <summary>
		/// Flag to write output to the error stream rather than the standard output stream
		/// </summary>
		private bool m_writeToErrorStream;

		/// <summary>
		/// Mapping from level object to color value
		/// </summary>
		private LevelMapping m_levelMapping = new LevelMapping();

		/// <summary>
		/// Target is the value of the console output stream.
		/// This is either <c>"Console.Out"</c> or <c>"Console.Error"</c>.
		/// </summary>
		/// <value>
		/// Target is the value of the console output stream.
		/// This is either <c>"Console.Out"</c> or <c>"Console.Error"</c>.
		/// </value>
		/// <remarks>
		/// <para>
		/// Target is the value of the console output stream.
		/// This is either <c>"Console.Out"</c> or <c>"Console.Error"</c>.
		/// </para>
		/// </remarks>
		public virtual string Target
		{
			get
			{
				if (!m_writeToErrorStream)
				{
					return "Console.Out";
				}
				return "Console.Error";
			}
			set
			{
				string b = value.Trim();
				if (SystemInfo.EqualsIgnoringCase("Console.Error", b))
				{
					m_writeToErrorStream = true;
				}
				else
				{
					m_writeToErrorStream = false;
				}
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
		/// Add a mapping of level to color - done by the config file
		/// </summary>
		/// <param name="mapping">The mapping to add</param>
		/// <remarks>
		/// <para>
		/// Add a <see cref="T:log4net.Appender.ManagedColoredConsoleAppender.LevelColors" /> mapping to this appender.
		/// Each mapping defines the foreground and background colors
		/// for a level.
		/// </para>
		/// </remarks>
		public void AddMapping(LevelColors mapping)
		{
			m_levelMapping.Add(mapping);
		}

		/// <summary>
		/// This method is called by the <see cref="M:AppenderSkeleton.DoAppend(log4net.Core.LoggingEvent)" /> method.
		/// </summary>
		/// <param name="loggingEvent">The event to log.</param>
		/// <remarks>
		/// <para>
		/// Writes the event to the console.
		/// </para>
		/// <para>
		/// The format of the output will depend on the appender's layout.
		/// </para>
		/// </remarks>
		protected override void Append(LoggingEvent loggingEvent)
		{
			TextWriter textWriter = (!m_writeToErrorStream) ? Console.Out : Console.Error;
			Console.ResetColor();
			LevelColors levelColors = m_levelMapping.Lookup(loggingEvent.Level) as LevelColors;
			if (levelColors != null)
			{
				if (levelColors.HasBackColor)
				{
					Console.BackgroundColor = levelColors.BackColor;
				}
				if (levelColors.HasForeColor)
				{
					Console.ForegroundColor = levelColors.ForeColor;
				}
			}
			string value = RenderLoggingEvent(loggingEvent);
			textWriter.Write(value);
			Console.ResetColor();
		}

		/// <summary>
		/// Initialize the options for this appender
		/// </summary>
		/// <remarks>
		/// <para>
		/// Initialize the level to color mappings set on this appender.
		/// </para>
		/// </remarks>
		public override void ActivateOptions()
		{
			base.ActivateOptions();
			m_levelMapping.ActivateOptions();
		}
	}
}
