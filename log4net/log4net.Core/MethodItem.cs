using log4net.Util;
using System;
using System.Collections;
using System.Reflection;

namespace log4net.Core
{
	/// <summary>
	/// provides method information without actually referencing a System.Reflection.MethodBase
	/// as that would require that the containing assembly is loaded.
	/// </summary>
	[Serializable]
	public class MethodItem
	{
		private readonly string m_name;

		private readonly string[] m_parameters;

		/// <summary>
		/// The fully qualified type of the StackFrameItem class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(MethodItem);

		/// <summary>
		/// When location information is not available the constant
		/// <c>NA</c> is returned. Current value of this string
		/// constant is <b>?</b>.
		/// </summary>
		private const string NA = "?";

		/// <summary>
		/// Gets the method name of the caller making the logging 
		/// request.
		/// </summary>
		/// <value>
		/// The method name of the caller making the logging 
		/// request.
		/// </value>
		/// <remarks>
		/// <para>
		/// Gets the method name of the caller making the logging 
		/// request.
		/// </para>
		/// </remarks>
		public string Name
		{
			get
			{
				return m_name;
			}
		}

		/// <summary>
		/// Gets the method parameters of the caller making
		/// the logging request.
		/// </summary>
		/// <value>
		/// The method parameters of the caller making
		/// the logging request
		/// </value>
		/// <remarks>
		/// <para>
		/// Gets the method parameters of the caller making
		/// the logging request.
		/// </para>
		/// </remarks>
		public string[] Parameters
		{
			get
			{
				return m_parameters;
			}
		}

		/// <summary>
		/// constructs a method item for an unknown method.
		/// </summary>
		public MethodItem()
		{
			m_name = "?";
			m_parameters = new string[0];
		}

		/// <summary>
		/// constructs a method item from the name of the method.
		/// </summary>
		/// <param name="name"></param>
		public MethodItem(string name)
			: this()
		{
			m_name = name;
		}

		/// <summary>
		/// constructs a method item from the name of the method and its parameters.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parameters"></param>
		public MethodItem(string name, string[] parameters)
			: this(name)
		{
			m_parameters = parameters;
		}

		/// <summary>
		/// constructs a method item from a method base by determining the method name and its parameters.
		/// </summary>
		/// <param name="methodBase"></param>
		public MethodItem(MethodBase methodBase)
			: this(methodBase.Name, GetMethodParameterNames(methodBase))
		{
		}

		private static string[] GetMethodParameterNames(MethodBase methodBase)
		{
			ArrayList arrayList = new ArrayList();
			try
			{
				ParameterInfo[] parameters = methodBase.GetParameters();
				int upperBound = parameters.GetUpperBound(0);
				for (int i = 0; i <= upperBound; i++)
				{
					ArrayList arrayList2 = arrayList;
					Type parameterType = parameters[i].ParameterType;
					arrayList2.Add((((object)parameterType != null) ? parameterType.ToString() : null) + " " + parameters[i].Name);
				}
			}
			catch (Exception exception)
			{
				LogLog.Error(declaringType, "An exception ocurred while retreiving method parameters.", exception);
			}
			return (string[])arrayList.ToArray(typeof(string));
		}
	}
}
