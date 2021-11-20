using log4net.Core;
using log4net.Util;
using System;
using System.Text;

namespace log4net.Layout.Pattern
{
	/// <summary>
	/// Write the caller stack frames to the output
	/// </summary>
	/// <remarks>
	/// <para>
	/// Writes the <see cref="P:log4net.Core.LocationInfo.StackFrames" /> to the output writer, using format:
	/// type3.MethodCall3(type param,...) &gt; type2.MethodCall2(type param,...) &gt; type1.MethodCall1(type param,...)
	/// </para>
	/// </remarks>
	/// <author>Adam Davies</author>
	internal class StackTraceDetailPatternConverter : StackTracePatternConverter
	{
		/// <summary>
		/// The fully qualified type of the StackTraceDetailPatternConverter class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(StackTracePatternConverter);

		internal override string GetMethodInformation(MethodItem method)
		{
			string result = "";
			try
			{
				string str = "";
				string[] parameters = method.Parameters;
				StringBuilder stringBuilder = new StringBuilder();
				if (parameters != null && parameters.GetUpperBound(0) > 0)
				{
					for (int i = 0; i <= parameters.GetUpperBound(0); i++)
					{
						stringBuilder.AppendFormat("{0}, ", parameters[i]);
					}
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Remove(stringBuilder.Length - 2, 2);
					str = stringBuilder.ToString();
				}
				result = base.GetMethodInformation(method) + "(" + str + ")";
				return result;
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "An exception ocurred while retreiving method information.", exception);
				return result;
			}
		}
	}
}
