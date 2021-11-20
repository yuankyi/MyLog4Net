using System;

namespace log4net.Util
{
	/// <summary>
	///
	/// </summary>
	public class LogReceivedEventArgs : EventArgs
	{
		private readonly LogLog loglog;

		/// <summary>
		///
		/// </summary>
		public LogLog LogLog
		{
			get
			{
				return loglog;
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="loglog"></param>
		public LogReceivedEventArgs(LogLog loglog)
		{
			this.loglog = loglog;
		}
	}
}
