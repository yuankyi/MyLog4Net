using System;

namespace log4net.Util
{
	/// <summary>
	/// Implementation of Properties collection for the <see cref="T:log4net.ThreadContext" />
	/// </summary>
	/// <remarks>
	/// <para>
	/// Class implements a collection of properties that is specific to each thread.
	/// The class is not synchronized as each thread has its own <see cref="T:log4net.Util.PropertiesDictionary" />.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	public sealed class ThreadContextProperties : ContextPropertiesBase
	{
		/// <summary>
		/// Each thread will automatically have its instance.
		/// </summary>
		[ThreadStatic]
		private static PropertiesDictionary _dictionary;

		/// <summary>
		/// Gets or sets the value of a property
		/// </summary>
		/// <value>
		/// The value for the property with the specified key
		/// </value>
		/// <remarks>
		/// <para>
		/// Gets or sets the value of a property
		/// </para>
		/// </remarks>
		public override object this[string key]
		{
			get
			{
				if (_dictionary != null)
				{
					return _dictionary[key];
				}
				return null;
			}
			set
			{
				GetProperties(true)[key] = value;
			}
		}

		/// <summary>
		/// Internal constructor
		/// </summary>
		/// <remarks>
		/// <para>
		/// Initializes a new instance of the <see cref="T:log4net.Util.ThreadContextProperties" /> class.
		/// </para>
		/// </remarks>
		internal ThreadContextProperties()
		{
		}

		/// <summary>
		/// Remove a property
		/// </summary>
		/// <param name="key">the key for the entry to remove</param>
		/// <remarks>
		/// <para>
		/// Remove a property
		/// </para>
		/// </remarks>
		public void Remove(string key)
		{
			if (_dictionary != null)
			{
				_dictionary.Remove(key);
			}
		}

		/// <summary>
		/// Get the keys stored in the properties.
		/// </summary>
		/// <para>
		/// Gets the keys stored in the properties.
		/// </para>
		/// <returns>a set of the defined keys</returns>
		public string[] GetKeys()
		{
			if (_dictionary != null)
			{
				return _dictionary.GetKeys();
			}
			return null;
		}

		/// <summary>
		/// Clear all properties
		/// </summary>
		/// <remarks>
		/// <para>
		/// Clear all properties
		/// </para>
		/// </remarks>
		public void Clear()
		{
			if (_dictionary != null)
			{
				_dictionary.Clear();
			}
		}

		/// <summary>
		/// Get the <c>PropertiesDictionary</c> for this thread.
		/// </summary>
		/// <param name="create">create the dictionary if it does not exist, otherwise return null if does not exist</param>
		/// <returns>the properties for this thread</returns>
		/// <remarks>
		/// <para>
		/// The collection returned is only to be used on the calling thread. If the
		/// caller needs to share the collection between different threads then the 
		/// caller must clone the collection before doing so.
		/// </para>
		/// </remarks>
		internal PropertiesDictionary GetProperties(bool create)
		{
			if (_dictionary == null && create)
			{
				_dictionary = new PropertiesDictionary();
			}
			return _dictionary;
		}
	}
}
