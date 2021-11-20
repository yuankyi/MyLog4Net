using log4net.Util;
using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security;

namespace log4net.Core
{
	/// <summary>
	/// The internal representation of caller location information.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class uses the <c>System.Diagnostics.StackTrace</c> class to generate
	/// a call stack. The caller's information is then extracted from this stack.
	/// </para>
	/// <para>
	/// The <c>System.Diagnostics.StackTrace</c> class is not supported on the 
	/// .NET Compact Framework 1.0 therefore caller location information is not
	/// available on that framework.
	/// </para>
	/// <para>
	/// The <c>System.Diagnostics.StackTrace</c> class has this to say about Release builds:
	/// </para>
	/// <para>
	/// "StackTrace information will be most informative with Debug build configurations. 
	/// By default, Debug builds include debug symbols, while Release builds do not. The 
	/// debug symbols contain most of the file, method name, line number, and column 
	/// information used in constructing StackFrame and StackTrace objects. StackTrace 
	/// might not report as many method calls as expected, due to code transformations 
	/// that occur during optimization."
	/// </para>
	/// <para>
	/// This means that in a Release build the caller information may be incomplete or may 
	/// not exist at all! Therefore caller location information cannot be relied upon in a Release build.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	[Serializable]
	public class LocationInfo
	{
		private readonly string m_className;

		private readonly string m_fileName;

		private readonly string m_lineNumber;

		private readonly string m_methodName;

		private readonly string m_fullInfo;

		private readonly StackFrameItem[] m_stackFrames;

		/// <summary>
		/// The fully qualified type of the LocationInfo class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(LocationInfo);

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
		public string MethodName
		{
			get
			{
				return m_methodName;
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
		/// Gets the stack frames from the stack trace of the caller making the log request
		/// </summary>
		public StackFrameItem[] StackFrames
		{
			get
			{
				return m_stackFrames;
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="callerStackBoundaryDeclaringType">The declaring type of the method that is
		/// the stack boundary into the logging system for this call.</param>
		/// <remarks>
		/// <para>
		/// Initializes a new instance of the <see cref="T:log4net.Core.LocationInfo" />
		/// class based on the current thread.
		/// </para>
		/// </remarks>
		public LocationInfo(Type callerStackBoundaryDeclaringType)
		{
			m_className = "?";
			m_fileName = "?";
			m_lineNumber = "?";
			m_methodName = "?";
			m_fullInfo = "?";
			if (callerStackBoundaryDeclaringType != null)
			{
				try
				{
					StackTrace stackTrace = new StackTrace(true);
					int i;
					for (i = 0; i < stackTrace.FrameCount; i++)
					{
						StackFrame frame = stackTrace.GetFrame(i);
						if (frame != null && frame.GetMethod().DeclaringType == callerStackBoundaryDeclaringType)
						{
							break;
						}
					}
					for (; i < stackTrace.FrameCount; i++)
					{
						StackFrame frame2 = stackTrace.GetFrame(i);
						if (frame2 != null && frame2.GetMethod().DeclaringType != callerStackBoundaryDeclaringType)
						{
							break;
						}
					}
					if (i < stackTrace.FrameCount)
					{
						int num = stackTrace.FrameCount - i;
						ArrayList arrayList = new ArrayList(num);
						m_stackFrames = new StackFrameItem[num];
						for (int j = i; j < stackTrace.FrameCount; j++)
						{
							arrayList.Add(new StackFrameItem(stackTrace.GetFrame(j)));
						}
						arrayList.CopyTo(m_stackFrames, 0);
						StackFrame frame3 = stackTrace.GetFrame(i);
						if (frame3 != null)
						{
							MethodBase method = frame3.GetMethod();
							if (method != null)
							{
								m_methodName = method.Name;
								if (method.DeclaringType != null)
								{
									m_className = method.DeclaringType.FullName;
								}
							}
							m_fileName = frame3.GetFileName();
							m_lineNumber = frame3.GetFileLineNumber().ToString(NumberFormatInfo.InvariantInfo);
							m_fullInfo = m_className + "." + m_methodName + "(" + m_fileName + ":" + m_lineNumber + ")";
						}
					}
				}
				catch (SecurityException)
				{
					LogLog.Debug(declaringType, "Security exception while trying to get caller stack frame. Error Ignored. Location Information Not Available.");
				}
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="className">The fully qualified class name.</param>
		/// <param name="methodName">The method name.</param>
		/// <param name="fileName">The file name.</param>
		/// <param name="lineNumber">The line number of the method within the file.</param>
		/// <remarks>
		/// <para>
		/// Initializes a new instance of the <see cref="T:log4net.Core.LocationInfo" />
		/// class with the specified data.
		/// </para>
		/// </remarks>
		public LocationInfo(string className, string methodName, string fileName, string lineNumber)
		{
			m_className = className;
			m_fileName = fileName;
			m_lineNumber = lineNumber;
			m_methodName = methodName;
			m_fullInfo = m_className + "." + m_methodName + "(" + m_fileName + ":" + m_lineNumber + ")";
		}
	}
}
