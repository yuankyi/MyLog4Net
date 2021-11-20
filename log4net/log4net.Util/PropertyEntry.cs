namespace log4net.Util
{
	/// <summary>
	/// A class to hold the key and data for a property set in the config file
	/// </summary>
	/// <remarks>
	/// <para>
	/// A class to hold the key and data for a property set in the config file
	/// </para>
	/// </remarks>
	public class PropertyEntry
	{
		private string m_key;

		private object m_value;

		/// <summary>
		/// Property Key
		/// </summary>
		/// <value>
		/// Property Key
		/// </value>
		/// <remarks>
		/// <para>
		/// Property Key.
		/// </para>
		/// </remarks>
		public string Key
		{
			get
			{
				return m_key;
			}
			set
			{
				m_key = value;
			}
		}

		/// <summary>
		/// Property Value
		/// </summary>
		/// <value>
		/// Property Value
		/// </value>
		/// <remarks>
		/// <para>
		/// Property Value.
		/// </para>
		/// </remarks>
		public object Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		/// <summary>
		/// Override <c>Object.ToString</c> to return sensible debug info
		/// </summary>
		/// <returns>string info about this object</returns>
		public override string ToString()
		{
			string[] obj = new string[5]
			{
				"PropertyEntry(Key=",
				m_key,
				", Value=",
				null,
				null
			};
			object value = m_value;
			obj[3] = ((value != null) ? value.ToString() : null);
			obj[4] = ")";
			return string.Concat(obj);
		}
	}
}
