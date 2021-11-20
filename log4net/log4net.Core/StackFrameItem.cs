using log4net.Util;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace log4net.Core
{
	/// <summary>
	/// provides stack frame information without actually referencing a System.Diagnostics.StackFrame
	/// as that would require that the containing assembly is loaded.
	/// </summary>
	[Serializable]
	public class StackFrameItem
	{
		private readonly string m_lineNumber;

		private readonly string m_fileName;

		private readonly string m_className;

		private readonly string m_fullInfo;

		private readonly MethodItem m_method;

		/// <summary>
		/// The fully qualified type of the StackFrameItem class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(StackFrameItem);

		/// <summary>
		/// When location information is not available the constant
		/// <c>NA</c> is returned. Current value of this string
		/// constant is <b>?</b>.
		/// </summary>
		private const string NA = "?";

		/// <summary>
		/// Gets the fully qualified class name of the caller making the logging 
		/// request.
		/// </summary>
		/// <value>
		/// The fully qualified class name of the caller making the logging 
		/// request.
		/// </value>
		/// <remarks>
		/// <para>
		/// Gets the fully qualified class name of the caller making the logging 
		/// request.
		/// </para>
		/// </remarks>
		public string ClassName
		{
			get
			{
				return m_className;
			}
		}

		/// <summary>
		/// Gets the file name of the caller.
		/// </summary>
		/// <value>
		/// The file name of the caller.
		/// </value>
		/// <remarks>
		/// <para>
		/// Gets the file name of the caller.
		/// </para>
		/// </remarks>
		public string FileName
		{
			get
			{
				return m_fileName;
			}
		}

		/// <summary>
		/// Gets the line number of the caller.
		/// </summary>
		/// <value>
		/// The line number of the caller.
		/// </value>
		/// <remarks>
		/// <para>
		/// Gets the line number of the caller.
		/// </para>
		/// </remarks>
		public string LineNumber
		{
			get
			{
				return m_lineNumber;
			}
		}

		/// <summary>
		/// Gets the method name of the caller.
		/// </summary>
		/// <value>
		/// The method name of the caller.
		/// </value>
		/// <remarks>
		/// <para>
		/// Gets the method name of the caller.
		/// </para>
		/// </remarks>
		public MethodItem Method
		{
			get
			{
				return m_method;
			}
		}

		/// <summary>
		/// Gets all available caller information
		/// </summary>
		/// <value>
		/// All available caller information, in the format
		/// <c>fully.qualified.classname.of.caller.methodName(Filename:line)</c>
		/// </value>
		/// <remarks>
		/// <para>
		/// Gets all available caller information, in the format
		/// <c>fully.qualified.classname.of.caller.methodName(Filename:line)</c>
		/// </para>
		/// </remarks>
		public string FullInfo
		{
			get
			{
				return m_fullInfo;
			}
		}

		/// <summary>
		/// returns a stack frame item from a stack frame. This 
		/// </summary>
		/// <param name="frame"></param>
		/// <returns></returns>
		public StackFrameItem(StackFrame frame)
		{
			m_lineNumber = "?";
			m_fileName = "?";
			m_method = new MethodItem();
			m_className = "?";
			try
			{
				m_lineNumber = frame.GetFileLineNumber().ToString(NumberFormatInfo.InvariantInfo);
				m_fileName = frame.GetFileName();
				MethodBase method = frame.GetMethod();
				if (method != null)
				{
					if (method.DeclaringType != null)
					{
						m_className = method.DeclaringType.FullName;
					}
					m_method = new MethodItem(method);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "An exception ocurred while retreiving stack frame information.", exception);
			}
			m_fullInfo = m_className + "." + m_method.Name + "(" + m_fileName + ":" + m_lineNumber + ")";
		}
	}
}
