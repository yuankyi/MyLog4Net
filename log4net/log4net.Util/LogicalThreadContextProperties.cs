using System;
using System.Runtime.Remoting.Messaging;
using System.Security;

namespace log4net.Util
{
	/// <summary>
	/// Implementation of Properties collection for the <see cref="T:log4net.LogicalThreadContext" />
	/// </summary>
	/// <remarks>
	/// <para>
	/// Class implements a collection of properties that is specific to each thread.
	/// The class is not synchronized as each thread has its own <see cref="T:log4net.Util.PropertiesDictionary" />.
	/// </para>
	/// <para>
	/// This class stores its properties in a slot on the <see cref="T:System.Runtime.Remoting.Messaging.CallContext" /> named
	/// <c>log4net.Util.LogicalThreadContextProperties</c>.
	/// </para>
	/// <para>
	/// For .NET Standard 1.3 this class uses
	/// System.Threading.AsyncLocal rather than <see cref="T:System.Runtime.Remoting.Messaging.CallContext" />.
	/// </para>
	/// <para>
	/// The <see cref="T:System.Runtime.Remoting.Messaging.CallContext" /> requires a link time 
	/// <see cref="T:System.Security.Permissions.SecurityPermission" /> for the
	/// <see cref="F:System.Security.Permissions.SecurityPermissionFlag.Infrastructure" />.
	/// If the calling code does not have this permission then this context will be disabled.
	/// It will not store any property values set on it.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	public sealed class LogicalThreadContextProperties : ContextPropertiesBase
	{
		private const string c_SlotName = "log4net.Util.LogicalThreadContextProperties";

		/// <summary>
		/// Flag used to disable this context if we don't have permission to access the CallContext.
		/// </summary>
		private bool m_disabled;

		/// <summary>
		/// The fully qualified type of the LogicalThreadContextProperties class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(LogicalThreadContextProperties);

		/// <summary>
		/// Gets or sets the value of a property
		/// </summary>
		/// <value>
		/// The value for the property with the specified key
		/// </value>
		/// <remarks>
		/// <para>
		/// Get or set the property value for the <paramref name="key" /> specified.
		/// </para>
		/// </remarks>
		public override object this[string key]
		{
			get
			{
				PropertiesDictionary properties = GetProperties(false);
				if (properties != null)
				{
					return properties[key];
				}
				return null;
			}
			set
			{
				PropertiesDictionary propertiesDictionary = new PropertiesDictionary(GetProperties(true));
				propertiesDictionary[key] = value;
				SetLogicalProperties(propertiesDictionary);
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>
		/// <para>
		/// Initializes a new instance of the <see cref="T:log4net.Util.LogicalThreadContextProperties" /> class.
		/// </para>
		/// </remarks>
		internal LogicalThreadContextProperties()
		{
		}

		/// <summary>
		/// Remove a property
		/// </summary>
		/// <param name="key">the key for the entry to remove</param>
		/// <remarks>
		/// <para>
		/// Remove the value for the specified <paramref name="key" /> from the context.
		/// </para>
		/// </remarks>
		public void Remove(string key)
		{
			PropertiesDictionary properties = GetProperties(false);
			if (properties != null)
			{
				PropertiesDictionary propertiesDictionary = new PropertiesDictionary(properties);
				propertiesDictionary.Remove(key);
				SetLogicalProperties(propertiesDictionary);
			}
		}

		/// <summary>
		/// Clear all the context properties
		/// </summary>
		/// <remarks>
		/// <para>
		/// Clear all the context properties
		/// </para>
		/// </remarks>
		public void Clear()
		{
			if (GetProperties(false) != null)
			{
				SetLogicalProperties(new PropertiesDictionary());
			}
		}

		/// <summary>
		/// Get the PropertiesDictionary stored in the LocalDataStoreSlot for this thread.
		/// </summary>
		/// <param name="create">create the dictionary if it does not exist, otherwise return null if is does not exist</param>
		/// <returns>the properties for this thread</returns>
		/// <remarks>
		/// <para>
		/// The collection returned is only to be used on the calling thread. If the
		/// caller needs to share the collection between different threads then the 
		/// caller must clone the collection before doings so.
		/// </para>
		/// </remarks>
		internal PropertiesDictionary GetProperties(bool create)
		{
			if (!m_disabled)
			{
				try
				{
					PropertiesDictionary propertiesDictionary = GetLogicalProperties();
					if (propertiesDictionary == null && create)
					{
						propertiesDictionary = new PropertiesDictionary();
						SetLogicalProperties(propertiesDictionary);
					}
					return propertiesDictionary;
				}
				catch (SecurityException exception)
				{
					m_disabled = true;
					LogLog.Warn(declaringType, "SecurityException while accessing CallContext. Disabling LogicalThreadContextProperties", exception);
				}
			}
			if (create)
			{
				return new PropertiesDictionary();
			}
			return null;
		}

		/// <summary>
		/// Gets the call context get data.
		/// </summary>
		/// <returns>The peroperties dictionary stored in the call context</returns>
		/// <remarks>
		/// The <see cref="T:System.Runtime.Remoting.Messaging.CallContext" /> method <see cref="M:System.Runtime.Remoting.Messaging.CallContext.GetData(System.String)" /> has a
		/// security link demand, therfore we must put the method call in a seperate method
		/// that we can wrap in an exception handler.
		/// </remarks>
		[SecuritySafeCritical]
		private static PropertiesDictionary GetLogicalProperties()
		{
			return CallContext.LogicalGetData("log4net.Util.LogicalThreadContextProperties") as PropertiesDictionary;
		}

		/// <summary>
		/// Sets the call context data.
		/// </summary>
		/// <param name="properties">The properties.</param>
		/// <remarks>
		/// The <see cref="T:System.Runtime.Remoting.Messaging.CallContext" /> method <see cref="M:System.Runtime.Remoting.Messaging.CallContext.SetData(System.String,System.Object)" /> has a
		/// security link demand, therfore we must put the method call in a seperate method
		/// that we can wrap in an exception handler.
		/// </remarks>
		[SecuritySafeCritical]
		private static void SetLogicalProperties(PropertiesDictionary properties)
		{
			CallContext.LogicalSetData("log4net.Util.LogicalThreadContextProperties", properties);
		}
	}
}
