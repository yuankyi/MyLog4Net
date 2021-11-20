using log4net.Core;
using log4net.Util;
using System;
using System.IO;

namespace log4net.Layout.Pattern
{
	/// <summary>
	/// Write the caller stack frames to the output
	/// </summary>
	/// <remarks>
	/// <para>
	/// Writes the <see cref="P:log4net.Core.LocationInfo.StackFrames" /> to the output writer, using format:
	/// type3.MethodCall3 &gt; type2.MethodCall2 &gt; type1.MethodCall1
	/// </para>
	/// </remarks>
	/// <author>Michael Cromwell</author>
	internal class StackTracePatternConverter : PatternLayoutConverter, IOptionHandler
	{
		private int m_stackFrameLevel = 1;

		/// <summary>
		/// The fully qualified type of the StackTracePatternConverter class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(StackTracePatternConverter);

		/// <summary>
		/// Initialize the converter
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is part of the <see cref="T:log4net.Core.IOptionHandler" /> delayed object
		/// activation scheme. The <see cref="M:log4net.Layout.Pattern.StackTracePatternConverter.ActivateOptions" /> method must 
		/// be called on this object after the configuration properties have
		/// been set. Until <see cref="M:log4net.Layout.Pattern.StackTracePatternConverter.ActivateOptions" /> is called this
		/// object is in an undefined state and must not be used. 
		/// </para>
		/// <para>
		/// If any of the configuration properties are modified then 
		/// <see cref="M:log4net.Layout.Pattern.StackTracePatternConverter.ActivateOptions" /> must be called again.
		/// </para>
		/// </remarks>
		public void ActivateOptions()
		{
			if (Option != null)
			{
				string text = Option.Trim();
				if (text.Length != 0)
				{
					int val;
					if (SystemInfo.TryParse(text, out val))
					{
						if (val <= 0)
						{
							LogLog.Error(declaringType, "StackTracePatternConverter: StackeFrameLevel option (" + text + ") isn't a positive integer.");
						}
						else
						{
							m_stackFrameLevel = val;
						}
					}
					else
					{
						LogLog.Error(declaringType, "StackTracePatternConverter: StackFrameLevel option \"" + text + "\" not a decimal integer.");
					}
				}
			}
		}

		/// <summary>
		/// Write the strack frames to the output
		/// </summary>
		/// <param name="writer"><see cref="T:System.IO.TextWriter" /> that will receive the formatted result.</param>
		/// <param name="loggingEvent">the event being logged</param>
		/// <remarks>
		/// <para>
		/// Writes the <see cref="P:log4net.Core.LocationInfo.StackFrames" /> to the output writer.
		/// </para>
		/// </remarks>
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
		{
			StackFrameItem[] stackFrames = loggingEvent.LocationInformation.StackFrames;
			if (stackFrames == null || stackFrames.Length == 0)
			{
				LogLog.Error(declaringType, "loggingEvent.LocationInformation.StackFrames was null or empty.");
			}
			else
			{
				int num = m_stackFrameLevel - 1;
				while (num >= 0)
				{
					if (num >= stackFrames.Length)
					{
						num--;
					}
					else
					{
						StackFrameItem stackFrameItem = stackFrames[num];
						writer.Write("{0}.{1}", stackFrameItem.ClassName, GetMethodInformation(stackFrameItem.Method));
						if (num > 0)
						{
							writer.Write(" > ");
						}
						num--;
					}
				}
			}
		}

		/// <summary>
		/// Returns the Name of the method
		/// </summary>
		/// <param name="method"></param>
		/// <remarks>This method was created, so this class could be used as a base class for StackTraceDetailPatternConverter</remarks>
		/// <returns>string</returns>
		internal virtual string GetMethodInformation(MethodItem method)
		{
			return method.Name;
		}
	}
}
