using log4net.Util;
using System;

namespace log4net.Core
{
	/// <summary>
	/// Portable data structure used by <see cref="T:log4net.Core.LoggingEvent" />
	/// </summary>
	/// <remarks>
	/// <para>
	/// Portable data structure used by <see cref="T:log4net.Core.LoggingEvent" />
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	public struct LoggingEventData
	{
		/// <summary>
		/// The logger name.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The logger name.
		/// </para>
		/// </remarks>
		public string LoggerName;

		/// <summary>
		/// Level of logging event.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Level of logging event. Level cannot be Serializable
		/// because it is a flyweight.  Due to its special serialization it
		/// cannot be declared final either.
		/// </para>
		/// </remarks>
		public Level Level;

		/// <summary>
		/// The application supplied message.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The application supplied message of logging event.
		/// </para>
		/// </remarks>
		public string Message;

		/// <summary>
		/// The name of thread
		/// </summary>
		/// <remarks>
		/// <para>
		/// The name of thread in which this logging event was generated
		/// </para>
		/// </remarks>
		public string ThreadName;

		/// <summary>
		/// Gets or sets the local time the event was logged
		/// </summary>
		/// <remarks>
		/// <para>
		/// Prefer using the <see cref="P:log4net.Core.LoggingEventData.TimeStampUtc" /> setter, since local time can be ambiguous.
		/// </para>
		/// </remarks>
		[Obsolete("Prefer using TimeStampUtc, since local time can be ambiguous in time zones with daylight savings time.")]
		public DateTime TimeStamp;

		private DateTime _timeStampUtc;

		/// <summary>
		/// Location information for the caller.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Location information for the caller.
		/// </para>
		/// </remarks>
		public LocationInfo LocationInfo;

		/// <summary>
		/// String representation of the user
		/// </summary>
		/// <remarks>
		/// <para>
		/// String representation of the user's windows name,
		/// like DOMAIN\username
		/// </para>
		/// </remarks>
		public string UserName;

		/// <summary>
		/// String representation of the identity.
		/// </summary>
		/// <remarks>
		/// <para>
		/// String representation of the current thread's principal identity.
		/// </para>
		/// </remarks>
		public string Identity;

		/// <summary>
		/// The string representation of the exception
		/// </summary>
		/// <remarks>
		/// <para>
		/// The string representation of the exception
		/// </para>
		/// </remarks>
		public string ExceptionString;

		/// <summary>
		/// String representation of the AppDomain.
		/// </summary>
		/// <remarks>
		/// <para>
		/// String representation of the AppDomain.
		/// </para>
		/// </remarks>
		public string Domain;

		/// <summary>
		/// Additional event specific properties
		/// </summary>
		/// <remarks>
		/// <para>
		/// A logger or an appender may attach additional
		/// properties to specific events. These properties
		/// have a string key and an object value.
		/// </para>
		/// </remarks>
		public PropertiesDictionary Properties;

		/// <summary>
		/// Gets or sets the UTC time the event was logged
		/// </summary>
		/// <remarks>
		/// <para>
		/// The TimeStamp is stored in the UTC time zone.
		/// </para>
		/// </remarks>
		public DateTime TimeStampUtc
		{
			get
			{
				if (TimeStamp != default(DateTime) && _timeStampUtc == default(DateTime))
				{
					return TimeStamp.ToUniversalTime();
				}
				return _timeStampUtc;
			}
			set
			{
				_timeStampUtc = value;
				TimeStamp = _timeStampUtc.ToLocalTime();
			}
		}
	}
}
