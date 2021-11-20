using log4net.Core;
using log4net.Util;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Threading;

namespace log4net.Appender
{
	/// <summary>
	/// Appender that rolls log files based on size or date or both.
	/// </summary>
	/// <remarks>
	/// <para>
	/// RollingFileAppender can roll log files based on size or date or both
	/// depending on the setting of the <see cref="P:log4net.Appender.RollingFileAppender.RollingStyle" /> property.
	/// When set to <see cref="F:log4net.Appender.RollingFileAppender.RollingMode.Size" /> the log file will be rolled
	/// once its size exceeds the <see cref="P:log4net.Appender.RollingFileAppender.MaximumFileSize" />.
	/// When set to <see cref="F:log4net.Appender.RollingFileAppender.RollingMode.Date" /> the log file will be rolled
	/// once the date boundary specified in the <see cref="P:log4net.Appender.RollingFileAppender.DatePattern" /> property
	/// is crossed.
	/// When set to <see cref="F:log4net.Appender.RollingFileAppender.RollingMode.Composite" /> the log file will be
	/// rolled once the date boundary specified in the <see cref="P:log4net.Appender.RollingFileAppender.DatePattern" /> property
	/// is crossed, but within a date boundary the file will also be rolled
	/// once its size exceeds the <see cref="P:log4net.Appender.RollingFileAppender.MaximumFileSize" />.
	/// When set to <see cref="F:log4net.Appender.RollingFileAppender.RollingMode.Once" /> the log file will be rolled when
	/// the appender is configured. This effectively means that the log file can be
	/// rolled once per program execution.
	/// </para>
	/// <para>
	/// A of few additional optional features have been added:
	/// <list type="bullet">
	/// <item>Attach date pattern for current log file <see cref="P:log4net.Appender.RollingFileAppender.StaticLogFileName" /></item>
	/// <item>Backup number increments for newer files <see cref="P:log4net.Appender.RollingFileAppender.CountDirection" /></item>
	/// <item>Infinite number of backups by file size <see cref="P:log4net.Appender.RollingFileAppender.MaxSizeRollBackups" /></item>
	/// </list>
	/// </para>
	///
	/// <note>
	/// <para>
	/// For large or infinite numbers of backup files a <see cref="P:log4net.Appender.RollingFileAppender.CountDirection" /> 
	/// greater than zero is highly recommended, otherwise all the backup files need
	/// to be renamed each time a new backup is created.
	/// </para>
	/// <para>
	/// When Date/Time based rolling is used setting <see cref="P:log4net.Appender.RollingFileAppender.StaticLogFileName" /> 
	/// to <see langword="true" /> will reduce the number of file renamings to few or none.
	/// </para>
	/// </note>
	///
	/// <note type="caution">
	/// <para>
	/// Changing <see cref="P:log4net.Appender.RollingFileAppender.StaticLogFileName" /> or <see cref="P:log4net.Appender.RollingFileAppender.CountDirection" /> without clearing
	/// the log file directory of backup files will cause unexpected and unwanted side effects.  
	/// </para>
	/// </note>
	///
	/// <para>
	/// If Date/Time based rolling is enabled this appender will attempt to roll existing files
	/// in the directory without a Date/Time tag based on the last write date of the base log file.
	/// The appender only rolls the log file when a message is logged. If Date/Time based rolling 
	/// is enabled then the appender will not roll the log file at the Date/Time boundary but
	/// at the point when the next message is logged after the boundary has been crossed.
	/// </para>
	///
	/// <para>
	/// The <see cref="T:log4net.Appender.RollingFileAppender" /> extends the <see cref="T:log4net.Appender.FileAppender" /> and
	/// has the same behavior when opening the log file.
	/// The appender will first try to open the file for writing when <see cref="M:log4net.Appender.RollingFileAppender.ActivateOptions" />
	/// is called. This will typically be during configuration.
	/// If the file cannot be opened for writing the appender will attempt
	/// to open the file again each time a message is logged to the appender.
	/// If the file cannot be opened for writing when a message is logged then
	/// the message will be discarded by this appender.
	/// </para>
	/// <para>
	/// When rolling a backup file necessitates deleting an older backup file the
	/// file to be deleted is moved to a temporary name before being deleted.
	/// </para>
	///
	/// <note type="caution">
	/// <para>
	/// A maximum number of backup files when rolling on date/time boundaries is not supported.
	/// </para>
	/// </note>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	/// <author>Aspi Havewala</author>
	/// <author>Douglas de la Torre</author>
	/// <author>Edward Smit</author>
	public class RollingFileAppender : FileAppender
	{
		/// <summary>
		/// Style of rolling to use
		/// </summary>
		/// <remarks>
		/// <para>
		/// Style of rolling to use
		/// </para>
		/// </remarks>
		public enum RollingMode
		{
			/// <summary>
			/// Roll files once per program execution
			/// </summary>
			/// <remarks>
			/// <para>
			/// Roll files once per program execution.
			/// Well really once each time this appender is
			/// configured.
			/// </para>
			/// <para>
			/// Setting this option also sets <c>AppendToFile</c> to
			/// <c>false</c> on the <c>RollingFileAppender</c>, otherwise
			/// this appender would just be a normal file appender.
			/// </para>
			/// </remarks>
			Once,
			/// <summary>
			/// Roll files based only on the size of the file
			/// </summary>
			Size,
			/// <summary>
			/// Roll files based only on the date
			/// </summary>
			Date,
			/// <summary>
			/// Roll files based on both the size and date of the file
			/// </summary>
			Composite
		}

		/// <summary>
		/// The code assumes that the following 'time' constants are in a increasing sequence.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The code assumes that the following 'time' constants are in a increasing sequence.
		/// </para>
		/// </remarks>
		protected enum RollPoint
		{
			/// <summary>
			/// Roll the log not based on the date
			/// </summary>
			InvalidRollPoint = -1,
			/// <summary>
			/// Roll the log for each minute
			/// </summary>
			TopOfMinute,
			/// <summary>
			/// Roll the log for each hour
			/// </summary>
			TopOfHour,
			/// <summary>
			/// Roll the log twice a day (midday and midnight)
			/// </summary>
			HalfDay,
			/// <summary>
			/// Roll the log each day (midnight)
			/// </summary>
			TopOfDay,
			/// <summary>
			/// Roll the log each week
			/// </summary>
			TopOfWeek,
			/// <summary>
			/// Roll the log each month
			/// </summary>
			TopOfMonth
		}

		/// <summary>
		/// This interface is used to supply Date/Time information to the <see cref="T:log4net.Appender.RollingFileAppender" />.
		/// </summary>
		/// <remarks>
		/// This interface is used to supply Date/Time information to the <see cref="T:log4net.Appender.RollingFileAppender" />.
		/// Used primarily to allow test classes to plug themselves in so they can
		/// supply test date/times.
		/// </remarks>
		public interface IDateTime
		{
			/// <summary>
			/// Gets the <i>current</i> time.
			/// </summary>
			/// <value>The <i>current</i> time.</value>
			/// <remarks>
			/// <para>
			/// Gets the <i>current</i> time.
			/// </para>
			/// </remarks>
			DateTime Now
			{
				get;
			}
		}

		/// <summary>
		/// Default implementation of <see cref="T:log4net.Appender.RollingFileAppender.IDateTime" /> that returns the current time.
		/// </summary>
		private class LocalDateTime : IDateTime
		{
			/// <summary>
			/// Gets the <b>current</b> time.
			/// </summary>
			/// <value>The <b>current</b> time.</value>
			/// <remarks>
			/// <para>
			/// Gets the <b>current</b> time.
			/// </para>
			/// </remarks>
			public DateTime Now
			{
				get
				{
					return DateTime.Now;
				}
			}
		}

		/// <summary>
		/// Implementation of <see cref="T:log4net.Appender.RollingFileAppender.IDateTime" /> that returns the current time as the coordinated universal time (UTC).
		/// </summary>
		private class UniversalDateTime : IDateTime
		{
			/// <summary>
			/// Gets the <b>current</b> time.
			/// </summary>
			/// <value>The <b>current</b> time.</value>
			/// <remarks>
			/// <para>
			/// Gets the <b>current</b> time.
			/// </para>
			/// </remarks>
			public DateTime Now
			{
				get
				{
					return DateTime.UtcNow;
				}
			}
		}

		/// <summary>
		/// The fully qualified type of the RollingFileAppender class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(RollingFileAppender);

		/// <summary>
		/// This object supplies the current date/time.  Allows test code to plug in
		/// a method to control this class when testing date/time based rolling. The default
		/// implementation uses the underlying value of DateTime.Now.
		/// </summary>
		private IDateTime m_dateTime;

		/// <summary>
		/// The date pattern. By default, the pattern is set to <c>".yyyy-MM-dd"</c> 
		/// meaning daily rollover.
		/// </summary>
		private string m_datePattern = ".yyyy-MM-dd";

		/// <summary>
		/// The actual formatted filename that is currently being written to
		/// or will be the file transferred to on roll over
		/// (based on staticLogFileName).
		/// </summary>
		private string m_scheduledFilename;

		/// <summary>
		/// The timestamp when we shall next recompute the filename.
		/// </summary>
		private DateTime m_nextCheck = DateTime.MaxValue;

		/// <summary>
		/// Holds date of last roll over
		/// </summary>
		private DateTime m_now;

		/// <summary>
		/// The type of rolling done
		/// </summary>
		private RollPoint m_rollPoint;

		/// <summary>
		/// The default maximum file size is 10MB
		/// </summary>
		private long m_maxFileSize = 10485760L;

		/// <summary>
		/// There is zero backup files by default
		/// </summary>
		private int m_maxSizeRollBackups;

		/// <summary>
		/// How many sized based backups have been made so far
		/// </summary>
		private int m_curSizeRollBackups;

		/// <summary>
		/// The rolling file count direction. 
		/// </summary>
		private int m_countDirection = -1;

		/// <summary>
		/// The rolling mode used in this appender.
		/// </summary>
		private RollingMode m_rollingStyle = RollingMode.Composite;

		/// <summary>
		/// Cache flag set if we are rolling by date.
		/// </summary>
		private bool m_rollDate = true;

		/// <summary>
		/// Cache flag set if we are rolling by size.
		/// </summary>
		private bool m_rollSize = true;

		/// <summary>
		/// Value indicating whether to always log to the same file.
		/// </summary>
		private bool m_staticLogFileName = true;

		/// <summary>
		/// Value indicating whether to preserve the file name extension when rolling.
		/// </summary>
		private bool m_preserveLogFileNameExtension;

		/// <summary>
		/// FileName provided in configuration.  Used for rolling properly
		/// </summary>
		private string m_baseFileName;

		/// <summary>
		/// A mutex that is used to lock rolling of files.
		/// </summary>
		private Mutex m_mutexForRolling;

		/// <summary>
		/// The 1st of January 1970 in UTC
		/// </summary>
		private static readonly DateTime s_date1970 = new DateTime(1970, 1, 1);

		/// <summary>
		/// Gets or sets the strategy for determining the current date and time. The default
		/// implementation is to use LocalDateTime which internally calls through to DateTime.Now. 
		/// DateTime.UtcNow may be used on frameworks newer than .NET 1.0 by specifying
		/// <see cref="T:log4net.Appender.RollingFileAppender.UniversalDateTime" />.
		/// </summary>
		/// <value>
		/// An implementation of the <see cref="T:log4net.Appender.RollingFileAppender.IDateTime" /> interface which returns the current date and time.
		/// </value>
		/// <remarks>
		/// <para>
		/// Gets or sets the <see cref="T:log4net.Appender.RollingFileAppender.IDateTime" /> used to return the current date and time.
		/// </para>
		/// <para>
		/// There are two built strategies for determining the current date and time, 
		/// <see cref="T:log4net.Appender.RollingFileAppender.LocalDateTime" />
		/// and <see cref="T:log4net.Appender.RollingFileAppender.UniversalDateTime" />.
		/// </para>
		/// <para>
		/// The default strategy is <see cref="T:log4net.Appender.RollingFileAppender.LocalDateTime" />.
		/// </para>
		/// </remarks>
		public IDateTime DateTimeStrategy
		{
			get
			{
				return m_dateTime;
			}
			set
			{
				m_dateTime = value;
			}
		}

		/// <summary>
		/// Gets or sets the date pattern to be used for generating file names
		/// when rolling over on date.
		/// </summary>
		/// <value>
		/// The date pattern to be used for generating file names when rolling 
		/// over on date.
		/// </value>
		/// <remarks>
		/// <para>
		/// Takes a string in the same format as expected by 
		/// <see cref="T:log4net.DateFormatter.SimpleDateFormatter" />.
		/// </para>
		/// <para>
		/// This property determines the rollover schedule when rolling over
		/// on date.
		/// </para>
		/// </remarks>
		public string DatePattern
		{
			get
			{
				return m_datePattern;
			}
			set
			{
				m_datePattern = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum number of backup files that are kept before
		/// the oldest is erased.
		/// </summary>
		/// <value>
		/// The maximum number of backup files that are kept before the oldest is
		/// erased.
		/// </value>
		/// <remarks>
		/// <para>
		/// If set to zero, then there will be no backup files and the log file 
		/// will be truncated when it reaches <see cref="P:log4net.Appender.RollingFileAppender.MaxFileSize" />.  
		/// </para>
		/// <para>
		/// If a negative number is supplied then no deletions will be made.  Note 
		/// that this could result in very slow performance as a large number of 
		/// files are rolled over unless <see cref="P:log4net.Appender.RollingFileAppender.CountDirection" /> is used.
		/// </para>
		/// <para>
		/// The maximum applies to <b>each</b> time based group of files and 
		/// <b>not</b> the total.
		/// </para>
		/// </remarks>
		public int MaxSizeRollBackups
		{
			get
			{
				return m_maxSizeRollBackups;
			}
			set
			{
				m_maxSizeRollBackups = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum size that the output file is allowed to reach
		/// before being rolled over to backup files.
		/// </summary>
		/// <value>
		/// The maximum size in bytes that the output file is allowed to reach before being 
		/// rolled over to backup files.
		/// </value>
		/// <remarks>
		/// <para>
		/// This property is equivalent to <see cref="P:log4net.Appender.RollingFileAppender.MaximumFileSize" /> except
		/// that it is required for differentiating the setter taking a
		/// <see cref="T:System.Int64" /> argument from the setter taking a <see cref="T:System.String" /> 
		/// argument.
		/// </para>
		/// <para>
		/// The default maximum file size is 10MB (10*1024*1024).
		/// </para>
		/// </remarks>
		public long MaxFileSize
		{
			get
			{
				return m_maxFileSize;
			}
			set
			{
				m_maxFileSize = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum size that the output file is allowed to reach
		/// before being rolled over to backup files.
		/// </summary>
		/// <value>
		/// The maximum size that the output file is allowed to reach before being 
		/// rolled over to backup files.
		/// </value>
		/// <remarks>
		/// <para>
		/// This property allows you to specify the maximum size with the
		/// suffixes "KB", "MB" or "GB" so that the size is interpreted being 
		/// expressed respectively in kilobytes, megabytes or gigabytes. 
		/// </para>
		/// <para>
		/// For example, the value "10KB" will be interpreted as 10240 bytes.
		/// </para>
		/// <para>
		/// The default maximum file size is 10MB.
		/// </para>
		/// <para>
		/// If you have the option to set the maximum file size programmatically
		/// consider using the <see cref="P:log4net.Appender.RollingFileAppender.MaxFileSize" /> property instead as this
		/// allows you to set the size in bytes as a <see cref="T:System.Int64" />.
		/// </para>
		/// </remarks>
		public string MaximumFileSize
		{
			get
			{
				return m_maxFileSize.ToString(NumberFormatInfo.InvariantInfo);
			}
			set
			{
				m_maxFileSize = OptionConverter.ToFileSize(value, m_maxFileSize + 1);
			}
		}

		/// <summary>
		/// Gets or sets the rolling file count direction. 
		/// </summary>
		/// <value>
		/// The rolling file count direction.
		/// </value>
		/// <remarks>
		/// <para>
		/// Indicates if the current file is the lowest numbered file or the
		/// highest numbered file.
		/// </para>
		/// <para>
		/// By default newer files have lower numbers (<see cref="P:log4net.Appender.RollingFileAppender.CountDirection" /> &lt; 0),
		/// i.e. log.1 is most recent, log.5 is the 5th backup, etc...
		/// </para>
		/// <para>
		/// <see cref="P:log4net.Appender.RollingFileAppender.CountDirection" /> &gt;= 0 does the opposite i.e.
		/// log.1 is the first backup made, log.5 is the 5th backup made, etc.
		/// For infinite backups use <see cref="P:log4net.Appender.RollingFileAppender.CountDirection" /> &gt;= 0 to reduce 
		/// rollover costs.
		/// </para>
		/// <para>The default file count direction is -1.</para>
		/// </remarks>
		public int CountDirection
		{
			get
			{
				return m_countDirection;
			}
			set
			{
				m_countDirection = value;
			}
		}

		/// <summary>
		/// Gets or sets the rolling style.
		/// </summary>
		/// <value>The rolling style.</value>
		/// <remarks>
		/// <para>
		/// The default rolling style is <see cref="F:log4net.Appender.RollingFileAppender.RollingMode.Composite" />.
		/// </para>
		/// <para>
		/// When set to <see cref="F:log4net.Appender.RollingFileAppender.RollingMode.Once" /> this appender's
		/// <see cref="P:log4net.Appender.FileAppender.AppendToFile" /> property is set to <c>false</c>, otherwise
		/// the appender would append to a single file rather than rolling
		/// the file each time it is opened.
		/// </para>
		/// </remarks>
		public RollingMode RollingStyle
		{
			get
			{
				return m_rollingStyle;
			}
			set
			{
				m_rollingStyle = value;
				switch (m_rollingStyle)
				{
				case RollingMode.Once:
					m_rollDate = false;
					m_rollSize = false;
					base.AppendToFile = false;
					break;
				case RollingMode.Size:
					m_rollDate = false;
					m_rollSize = true;
					break;
				case RollingMode.Date:
					m_rollDate = true;
					m_rollSize = false;
					break;
				case RollingMode.Composite:
					m_rollDate = true;
					m_rollSize = true;
					break;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to preserve the file name extension when rolling.
		/// </summary>
		/// <value>
		/// <c>true</c> if the file name extension should be preserved.
		/// </value>
		/// <remarks>
		/// <para>
		/// By default file.log is rolled to file.log.yyyy-MM-dd or file.log.curSizeRollBackup.
		/// However, under Windows the new file name will loose any program associations as the
		/// extension is changed. Optionally file.log can be renamed to file.yyyy-MM-dd.log or
		/// file.curSizeRollBackup.log to maintain any program associations.
		/// </para>
		/// </remarks>
		public bool PreserveLogFileNameExtension
		{
			get
			{
				return m_preserveLogFileNameExtension;
			}
			set
			{
				m_preserveLogFileNameExtension = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to always log to
		/// the same file.
		/// </summary>
		/// <value>
		/// <c>true</c> if always should be logged to the same file, otherwise <c>false</c>.
		/// </value>
		/// <remarks>
		/// <para>
		/// By default file.log is always the current file.  Optionally
		/// file.log.yyyy-mm-dd for current formatted datePattern can by the currently
		/// logging file (or file.log.curSizeRollBackup or even
		/// file.log.yyyy-mm-dd.curSizeRollBackup).
		/// </para>
		/// <para>
		/// This will make time based rollovers with a large number of backups 
		/// much faster as the appender it won't have to rename all the backups!
		/// </para>
		/// </remarks>
		public bool StaticLogFileName
		{
			get
			{
				return m_staticLogFileName;
			}
			set
			{
				m_staticLogFileName = value;
			}
		}

		/// <summary>
		/// Cleans up all resources used by this appender.
		/// </summary>
		~RollingFileAppender()
		{
			if (m_mutexForRolling != null)
			{
				m_mutexForRolling.Dispose();
				m_mutexForRolling = null;
			}
		}

		/// <summary>
		/// Sets the quiet writer being used.
		/// </summary>
		/// <remarks>
		/// This method can be overridden by sub classes.
		/// </remarks>
		/// <param name="writer">the writer to set</param>
		protected override void SetQWForFiles(TextWriter writer)
		{
			base.QuietWriter = new CountingQuietTextWriter(writer, ErrorHandler);
		}

		/// <summary>
		/// Write out a logging event.
		/// </summary>
		/// <param name="loggingEvent">the event to write to file.</param>
		/// <remarks>
		/// <para>
		/// Handles append time behavior for RollingFileAppender.  This checks
		/// if a roll over either by date (checked first) or time (checked second)
		/// is need and then appends to the file last.
		/// </para>
		/// </remarks>
		protected override void Append(LoggingEvent loggingEvent)
		{
			AdjustFileBeforeAppend();
			base.Append(loggingEvent);
		}

		/// <summary>
		/// Write out an array of logging events.
		/// </summary>
		/// <param name="loggingEvents">the events to write to file.</param>
		/// <remarks>
		/// <para>
		/// Handles append time behavior for RollingFileAppender.  This checks
		/// if a roll over either by date (checked first) or time (checked second)
		/// is need and then appends to the file last.
		/// </para>
		/// </remarks>
		protected override void Append(LoggingEvent[] loggingEvents)
		{
			AdjustFileBeforeAppend();
			base.Append(loggingEvents);
		}

		/// <summary>
		/// Performs any required rolling before outputting the next event
		/// </summary>
		/// <remarks>
		/// <para>
		/// Handles append time behavior for RollingFileAppender.  This checks
		/// if a roll over either by date (checked first) or time (checked second)
		/// is need and then appends to the file last.
		/// </para>
		/// </remarks>
		protected virtual void AdjustFileBeforeAppend()
		{
			try
			{
				if (m_mutexForRolling != null)
				{
					m_mutexForRolling.WaitOne();
				}
				if (m_rollDate)
				{
					DateTime now = m_dateTime.Now;
					if (now >= m_nextCheck)
					{
						m_now = now;
						m_nextCheck = NextCheckDate(m_now, m_rollPoint);
						RollOverTime(true);
					}
				}
				if (m_rollSize && File != null && ((CountingQuietTextWriter)base.QuietWriter).Count >= m_maxFileSize)
				{
					RollOverSize();
				}
			}
			finally
			{
				if (m_mutexForRolling != null)
				{
					m_mutexForRolling.ReleaseMutex();
				}
			}
		}

		/// <summary>
		/// Creates and opens the file for logging.  If <see cref="P:log4net.Appender.RollingFileAppender.StaticLogFileName" />
		/// is false then the fully qualified name is determined and used.
		/// </summary>
		/// <param name="fileName">the name of the file to open</param>
		/// <param name="append">true to append to existing file</param>
		/// <remarks>
		/// <para>This method will ensure that the directory structure
		/// for the <paramref name="fileName" /> specified exists.</para>
		/// </remarks>
		protected override void OpenFile(string fileName, bool append)
		{
			lock (this)
			{
				fileName = GetNextOutputFileName(fileName);
				long count = 0L;
				if (append)
				{
					using (base.SecurityContext.Impersonate(this))
					{
						if (System.IO.File.Exists(fileName))
						{
							count = new FileInfo(fileName).Length;
						}
					}
				}
				else if (LogLog.IsErrorEnabled && m_maxSizeRollBackups != 0 && FileExists(fileName))
				{
					LogLog.Error(declaringType, "RollingFileAppender: INTERNAL ERROR. Append is False but OutputFile [" + fileName + "] already exists.");
				}
				if (!m_staticLogFileName)
				{
					m_scheduledFilename = fileName;
				}
				base.OpenFile(fileName, append);
				((CountingQuietTextWriter)base.QuietWriter).Count = count;
			}
		}

		/// <summary>
		/// Get the current output file name
		/// </summary>
		/// <param name="fileName">the base file name</param>
		/// <returns>the output file name</returns>
		/// <remarks>
		/// The output file name is based on the base fileName specified.
		/// If <see cref="P:log4net.Appender.RollingFileAppender.StaticLogFileName" /> is set then the output 
		/// file name is the same as the base file passed in. Otherwise
		/// the output file depends on the date pattern, on the count
		/// direction or both.
		/// </remarks>
		protected string GetNextOutputFileName(string fileName)
		{
			if (!m_staticLogFileName)
			{
				fileName = fileName.Trim();
				if (m_rollDate)
				{
					fileName = CombinePath(fileName, m_now.ToString(m_datePattern, DateTimeFormatInfo.InvariantInfo));
				}
				if (m_countDirection >= 0)
				{
					fileName = CombinePath(fileName, "." + m_curSizeRollBackups.ToString());
				}
			}
			return fileName;
		}

		/// <summary>
		/// Determines curSizeRollBackups (only within the current roll point)
		/// </summary>
		private void DetermineCurSizeRollBackups()
		{
			m_curSizeRollBackups = 0;
			string text = null;
			string baseFile = null;
			using (base.SecurityContext.Impersonate(this))
			{
				text = Path.GetFullPath(m_baseFileName);
				baseFile = Path.GetFileName(text);
			}
			ArrayList existingFiles = GetExistingFiles(text);
			InitializeRollBackups(baseFile, existingFiles);
			LogLog.Debug(declaringType, "curSizeRollBackups starts at [" + m_curSizeRollBackups.ToString() + "]");
		}

		/// <summary>
		/// Generates a wildcard pattern that can be used to find all files
		/// that are similar to the base file name.
		/// </summary>
		/// <param name="baseFileName"></param>
		/// <returns></returns>
		private string GetWildcardPatternForFile(string baseFileName)
		{
			if (m_preserveLogFileNameExtension)
			{
				return Path.GetFileNameWithoutExtension(baseFileName) + "*" + Path.GetExtension(baseFileName);
			}
			return baseFileName + "*";
		}

		/// <summary>
		/// Builds a list of filenames for all files matching the base filename plus a file
		/// pattern.
		/// </summary>
		/// <param name="baseFilePath"></param>
		/// <returns></returns>
		private ArrayList GetExistingFiles(string baseFilePath)
		{
			ArrayList arrayList = new ArrayList();
			string text = null;
			using (base.SecurityContext.Impersonate(this))
			{
				string fullPath = Path.GetFullPath(baseFilePath);
				text = Path.GetDirectoryName(fullPath);
				if (Directory.Exists(text))
				{
					string fileName = Path.GetFileName(fullPath);
					string[] files = Directory.GetFiles(text, GetWildcardPatternForFile(fileName));
					if (files != null)
					{
						for (int i = 0; i < files.Length; i++)
						{
							string fileName2 = Path.GetFileName(files[i]);
							if (fileName2.StartsWith(Path.GetFileNameWithoutExtension(fileName)))
							{
								arrayList.Add(fileName2);
							}
						}
					}
				}
			}
			LogLog.Debug(declaringType, "Searched for existing files in [" + text + "]");
			return arrayList;
		}

		/// <summary>
		/// Initiates a roll over if needed for crossing a date boundary since the last run.
		/// </summary>
		private void RollOverIfDateBoundaryCrossing()
		{
			if (m_staticLogFileName && m_rollDate && FileExists(m_baseFileName))
			{
				DateTime dateTime;
				using (base.SecurityContext.Impersonate(this))
				{
					dateTime = ((!(DateTimeStrategy is UniversalDateTime)) ? System.IO.File.GetLastWriteTime(m_baseFileName) : System.IO.File.GetLastWriteTimeUtc(m_baseFileName));
				}
				LogLog.Debug(declaringType, "[" + dateTime.ToString(m_datePattern, DateTimeFormatInfo.InvariantInfo) + "] vs. [" + m_now.ToString(m_datePattern, DateTimeFormatInfo.InvariantInfo) + "]");
				if (!dateTime.ToString(m_datePattern, DateTimeFormatInfo.InvariantInfo).Equals(m_now.ToString(m_datePattern, DateTimeFormatInfo.InvariantInfo)))
				{
					m_scheduledFilename = CombinePath(m_baseFileName, dateTime.ToString(m_datePattern, DateTimeFormatInfo.InvariantInfo));
					LogLog.Debug(declaringType, "Initial roll over to [" + m_scheduledFilename + "]");
					RollOverTime(false);
					LogLog.Debug(declaringType, "curSizeRollBackups after rollOver at [" + m_curSizeRollBackups.ToString() + "]");
				}
			}
		}

		/// <summary>
		/// Initializes based on existing conditions at time of <see cref="M:log4net.Appender.RollingFileAppender.ActivateOptions" />.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Initializes based on existing conditions at time of <see cref="M:log4net.Appender.RollingFileAppender.ActivateOptions" />.
		/// The following is done
		/// <list type="bullet">
		/// <item>determine curSizeRollBackups (only within the current roll point)</item>
		/// <item>initiates a roll over if needed for crossing a date boundary since the last run.</item>
		/// </list>
		/// </para>
		/// </remarks>
		protected void ExistingInit()
		{
			DetermineCurSizeRollBackups();
			RollOverIfDateBoundaryCrossing();
			if (!base.AppendToFile)
			{
				bool flag = false;
				string nextOutputFileName = GetNextOutputFileName(m_baseFileName);
				using (base.SecurityContext.Impersonate(this))
				{
					flag = System.IO.File.Exists(nextOutputFileName);
				}
				if (flag)
				{
					if (m_maxSizeRollBackups == 0)
					{
						LogLog.Debug(declaringType, "Output file [" + nextOutputFileName + "] already exists. MaxSizeRollBackups is 0; cannot roll. Overwriting existing file.");
					}
					else
					{
						LogLog.Debug(declaringType, "Output file [" + nextOutputFileName + "] already exists. Not appending to file. Rolling existing file out of the way.");
						RollOverRenameFiles(nextOutputFileName);
					}
				}
			}
		}

		/// <summary>
		/// Does the work of bumping the 'current' file counter higher
		/// to the highest count when an incremental file name is seen.
		/// The highest count is either the first file (when count direction
		/// is greater than 0) or the last file (when count direction less than 0).
		/// In either case, we want to know the highest count that is present.
		/// </summary>
		/// <param name="baseFile"></param>
		/// <param name="curFileName"></param>
		private void InitializeFromOneFile(string baseFile, string curFileName)
		{
			if (curFileName.StartsWith(Path.GetFileNameWithoutExtension(baseFile)) && !curFileName.Equals(baseFile))
			{
				if (m_rollDate && !m_staticLogFileName)
				{
					string str = m_dateTime.Now.ToString(m_datePattern, DateTimeFormatInfo.InvariantInfo);
					string value = m_preserveLogFileNameExtension ? (Path.GetFileNameWithoutExtension(baseFile) + str) : (baseFile + str);
					string value2 = m_preserveLogFileNameExtension ? Path.GetExtension(baseFile) : "";
					if (!curFileName.StartsWith(value) || !curFileName.EndsWith(value2))
					{
						LogLog.Debug(declaringType, "Ignoring file [" + curFileName + "] because it is from a different date period");
						return;
					}
				}
				try
				{
					int backUpIndex = GetBackUpIndex(curFileName);
					if (backUpIndex > m_curSizeRollBackups)
					{
						if (m_maxSizeRollBackups != 0)
						{
							if (-1 == m_maxSizeRollBackups)
							{
								m_curSizeRollBackups = backUpIndex;
							}
							else if (m_countDirection >= 0)
							{
								m_curSizeRollBackups = backUpIndex;
							}
							else if (backUpIndex <= m_maxSizeRollBackups)
							{
								m_curSizeRollBackups = backUpIndex;
							}
						}
						LogLog.Debug(declaringType, "File name [" + curFileName + "] moves current count to [" + m_curSizeRollBackups.ToString() + "]");
					}
				}
				catch (FormatException)
				{
					LogLog.Debug(declaringType, "Encountered a backup file not ending in .x [" + curFileName + "]");
				}
			}
		}

		/// <summary>
		/// Attempts to extract a number from the end of the file name that indicates
		/// the number of the times the file has been rolled over.
		/// </summary>
		/// <remarks>
		/// Certain date pattern extensions like yyyyMMdd will be parsed as valid backup indexes.
		/// </remarks>
		/// <param name="curFileName"></param>
		/// <returns></returns>
		private int GetBackUpIndex(string curFileName)
		{
			int val = -1;
			string text = curFileName;
			if (m_preserveLogFileNameExtension)
			{
				text = Path.GetFileNameWithoutExtension(text);
			}
			int num = text.LastIndexOf(".");
			if (num > 0)
			{
				SystemInfo.TryParse(text.Substring(num + 1), out val);
			}
			return val;
		}

		/// <summary>
		/// Takes a list of files and a base file name, and looks for 
		/// 'incremented' versions of the base file.  Bumps the max
		/// count up to the highest count seen.
		/// </summary>
		/// <param name="baseFile"></param>
		/// <param name="arrayFiles"></param>
		private void InitializeRollBackups(string baseFile, ArrayList arrayFiles)
		{
			if (arrayFiles != null)
			{
				string baseFile2 = baseFile.ToLowerInvariant();
				foreach (string arrayFile in arrayFiles)
				{
					InitializeFromOneFile(baseFile2, arrayFile.ToLowerInvariant());
				}
			}
		}

		/// <summary>
		/// Calculates the RollPoint for the datePattern supplied.
		/// </summary>
		/// <param name="datePattern">the date pattern to calculate the check period for</param>
		/// <returns>The RollPoint that is most accurate for the date pattern supplied</returns>
		/// <remarks>
		/// Essentially the date pattern is examined to determine what the
		/// most suitable roll point is. The roll point chosen is the roll point
		/// with the smallest period that can be detected using the date pattern
		/// supplied. i.e. if the date pattern only outputs the year, month, day 
		/// and hour then the smallest roll point that can be detected would be
		/// and hourly roll point as minutes could not be detected.
		/// </remarks>
		private RollPoint ComputeCheckPeriod(string datePattern)
		{
			string text = s_date1970.ToString(datePattern, DateTimeFormatInfo.InvariantInfo);
			for (int i = 0; i <= 5; i++)
			{
				string text2 = NextCheckDate(s_date1970, (RollPoint)i).ToString(datePattern, DateTimeFormatInfo.InvariantInfo);
				LogLog.Debug(declaringType, "Type = [" + i.ToString() + "], r0 = [" + text + "], r1 = [" + text2 + "]");
				if (text != null && text2 != null && !text.Equals(text2))
				{
					return (RollPoint)i;
				}
			}
			return RollPoint.InvalidRollPoint;
		}

		/// <summary>
		/// Initialize the appender based on the options set
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is part of the <see cref="T:log4net.Core.IOptionHandler" /> delayed object
		/// activation scheme. The <see cref="M:log4net.Appender.RollingFileAppender.ActivateOptions" /> method must 
		/// be called on this object after the configuration properties have
		/// been set. Until <see cref="M:log4net.Appender.RollingFileAppender.ActivateOptions" /> is called this
		/// object is in an undefined state and must not be used. 
		/// </para>
		/// <para>
		/// If any of the configuration properties are modified then 
		/// <see cref="M:log4net.Appender.RollingFileAppender.ActivateOptions" /> must be called again.
		/// </para>
		/// <para>
		/// Sets initial conditions including date/time roll over information, first check,
		/// scheduledFilename, and calls <see cref="M:log4net.Appender.RollingFileAppender.ExistingInit" /> to initialize
		/// the current number of backups.
		/// </para>
		/// </remarks>
		public override void ActivateOptions()
		{
			if (m_dateTime == null)
			{
				m_dateTime = new LocalDateTime();
			}
			if (m_rollDate && m_datePattern != null)
			{
				m_now = m_dateTime.Now;
				m_rollPoint = ComputeCheckPeriod(m_datePattern);
				if (m_rollPoint == RollPoint.InvalidRollPoint)
				{
					throw new ArgumentException("Invalid RollPoint, unable to parse [" + m_datePattern + "]");
				}
				m_nextCheck = NextCheckDate(m_now, m_rollPoint);
			}
			else if (m_rollDate)
			{
				ErrorHandler.Error("Either DatePattern or rollingStyle options are not set for [" + base.Name + "].");
			}
			if (base.SecurityContext == null)
			{
				base.SecurityContext = SecurityContextProvider.DefaultProvider.CreateSecurityContext(this);
			}
			using (base.SecurityContext.Impersonate(this))
			{
				base.File = FileAppender.ConvertToFullPath(base.File.Trim());
				m_baseFileName = base.File;
			}
			m_mutexForRolling = new Mutex(false, m_baseFileName.Replace("\\", "_").Replace(":", "_").Replace("/", "_"));
			if (m_rollDate && File != null && m_scheduledFilename == null)
			{
				m_scheduledFilename = CombinePath(File, m_now.ToString(m_datePattern, DateTimeFormatInfo.InvariantInfo));
			}
			ExistingInit();
			base.ActivateOptions();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="path1"></param>
		/// <param name="path2">.1, .2, .3, etc.</param>
		/// <returns></returns>
		private string CombinePath(string path1, string path2)
		{
			string extension = Path.GetExtension(path1);
			if (m_preserveLogFileNameExtension && extension.Length > 0)
			{
				return Path.Combine(Path.GetDirectoryName(path1), Path.GetFileNameWithoutExtension(path1) + path2 + extension);
			}
			return path1 + path2;
		}

		/// <summary>
		/// Rollover the file(s) to date/time tagged file(s).
		/// </summary>
		/// <param name="fileIsOpen">set to true if the file to be rolled is currently open</param>
		/// <remarks>
		/// <para>
		/// Rollover the file(s) to date/time tagged file(s).
		/// Resets curSizeRollBackups. 
		/// If fileIsOpen is set then the new file is opened (through SafeOpenFile).
		/// </para>
		/// </remarks>
		protected void RollOverTime(bool fileIsOpen)
		{
			if (m_staticLogFileName)
			{
				if (m_datePattern == null)
				{
					ErrorHandler.Error("Missing DatePattern option in rollOver().");
					return;
				}
				string path = m_now.ToString(m_datePattern, DateTimeFormatInfo.InvariantInfo);
				if (m_scheduledFilename.Equals(CombinePath(File, path)))
				{
					ErrorHandler.Error("Compare " + m_scheduledFilename + " : " + CombinePath(File, path));
					return;
				}
				if (fileIsOpen)
				{
					CloseFile();
				}
				for (int i = 1; i <= m_curSizeRollBackups; i++)
				{
					string fromFile = CombinePath(File, "." + i.ToString());
					string toFile = CombinePath(m_scheduledFilename, "." + i.ToString());
					RollFile(fromFile, toFile);
				}
				RollFile(File, m_scheduledFilename);
			}
			m_curSizeRollBackups = 0;
			m_scheduledFilename = CombinePath(File, m_now.ToString(m_datePattern, DateTimeFormatInfo.InvariantInfo));
			if (fileIsOpen)
			{
				SafeOpenFile(m_baseFileName, false);
			}
		}

		/// <summary>
		/// Renames file <paramref name="fromFile" /> to file <paramref name="toFile" />.
		/// </summary>
		/// <param name="fromFile">Name of existing file to roll.</param>
		/// <param name="toFile">New name for file.</param>
		/// <remarks>
		/// <para>
		/// Renames file <paramref name="fromFile" /> to file <paramref name="toFile" />. It
		/// also checks for existence of target file and deletes if it does.
		/// </para>
		/// </remarks>
		protected void RollFile(string fromFile, string toFile)
		{
			if (FileExists(fromFile))
			{
				DeleteFile(toFile);
				try
				{
					LogLog.Debug(declaringType, "Moving [" + fromFile + "] -> [" + toFile + "]");
					using (base.SecurityContext.Impersonate(this))
					{
						System.IO.File.Move(fromFile, toFile);
					}
				}
				catch (Exception e)
				{
					ErrorHandler.Error("Exception while rolling file [" + fromFile + "] -> [" + toFile + "]", e, ErrorCode.GenericFailure);
				}
			}
			else
			{
				LogLog.Warn(declaringType, "Cannot RollFile [" + fromFile + "] -> [" + toFile + "]. Source does not exist");
			}
		}

		/// <summary>
		/// Test if a file exists at a specified path
		/// </summary>
		/// <param name="path">the path to the file</param>
		/// <returns>true if the file exists</returns>
		/// <remarks>
		/// <para>
		/// Test if a file exists at a specified path
		/// </para>
		/// </remarks>
		protected bool FileExists(string path)
		{
			using (base.SecurityContext.Impersonate(this))
			{
				return System.IO.File.Exists(path);
			}
		}

		/// <summary>
		/// Deletes the specified file if it exists.
		/// </summary>
		/// <param name="fileName">The file to delete.</param>
		/// <remarks>
		/// <para>
		/// Delete a file if is exists.
		/// The file is first moved to a new filename then deleted.
		/// This allows the file to be removed even when it cannot
		/// be deleted, but it still can be moved.
		/// </para>
		/// </remarks>
		protected void DeleteFile(string fileName)
		{
			if (FileExists(fileName))
			{
				string text = fileName;
				string text2 = fileName + "." + Environment.TickCount.ToString() + ".DeletePending";
				try
				{
					using (base.SecurityContext.Impersonate(this))
					{
						System.IO.File.Move(fileName, text2);
					}
					text = text2;
				}
				catch (Exception exception)
				{
					LogLog.Debug(declaringType, "Exception while moving file to be deleted [" + fileName + "] -> [" + text2 + "]", exception);
				}
				try
				{
					using (base.SecurityContext.Impersonate(this))
					{
						System.IO.File.Delete(text);
					}
					LogLog.Debug(declaringType, "Deleted file [" + fileName + "]");
				}
				catch (Exception ex)
				{
					if (text == fileName)
					{
						ErrorHandler.Error("Exception while deleting file [" + text + "]", ex, ErrorCode.GenericFailure);
					}
					else
					{
						LogLog.Debug(declaringType, "Exception while deleting temp file [" + text + "]", ex);
					}
				}
			}
		}

		/// <summary>
		/// Implements file roll base on file size.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If the maximum number of size based backups is reached
		/// (<c>curSizeRollBackups == maxSizeRollBackups</c>) then the oldest
		/// file is deleted -- its index determined by the sign of countDirection.
		/// If <c>countDirection</c> &lt; 0, then files
		/// {<c>File.1</c>, ..., <c>File.curSizeRollBackups -1</c>}
		/// are renamed to {<c>File.2</c>, ...,
		/// <c>File.curSizeRollBackups</c>}. Moreover, <c>File</c> is
		/// renamed <c>File.1</c> and closed.
		/// </para>
		/// <para>
		/// A new file is created to receive further log output.
		/// </para>
		/// <para>
		/// If <c>maxSizeRollBackups</c> is equal to zero, then the
		/// <c>File</c> is truncated with no backup files created.
		/// </para>
		/// <para>
		/// If <c>maxSizeRollBackups</c> &lt; 0, then <c>File</c> is
		/// renamed if needed and no files are deleted.
		/// </para>
		/// </remarks>
		protected void RollOverSize()
		{
			CloseFile();
			LogLog.Debug(declaringType, "rolling over count [" + ((CountingQuietTextWriter)base.QuietWriter).Count.ToString() + "]");
			LogLog.Debug(declaringType, "maxSizeRollBackups [" + m_maxSizeRollBackups.ToString() + "]");
			LogLog.Debug(declaringType, "curSizeRollBackups [" + m_curSizeRollBackups.ToString() + "]");
			LogLog.Debug(declaringType, "countDirection [" + m_countDirection.ToString() + "]");
			RollOverRenameFiles(File);
			if (!m_staticLogFileName && m_countDirection >= 0)
			{
				m_curSizeRollBackups++;
			}
			SafeOpenFile(m_baseFileName, false);
		}

		/// <summary>
		/// Implements file roll.
		/// </summary>
		/// <param name="baseFileName">the base name to rename</param>
		/// <remarks>
		/// <para>
		/// If the maximum number of size based backups is reached
		/// (<c>curSizeRollBackups == maxSizeRollBackups</c>) then the oldest
		/// file is deleted -- its index determined by the sign of countDirection.
		/// If <c>countDirection</c> &lt; 0, then files
		/// {<c>File.1</c>, ..., <c>File.curSizeRollBackups -1</c>}
		/// are renamed to {<c>File.2</c>, ...,
		/// <c>File.curSizeRollBackups</c>}. 
		/// </para>
		/// <para>
		/// If <c>maxSizeRollBackups</c> is equal to zero, then the
		/// <c>File</c> is truncated with no backup files created.
		/// </para>
		/// <para>
		/// If <c>maxSizeRollBackups</c> &lt; 0, then <c>File</c> is
		/// renamed if needed and no files are deleted.
		/// </para>
		/// <para>
		/// This is called by <see cref="M:log4net.Appender.RollingFileAppender.RollOverSize" /> to rename the files.
		/// </para>
		/// </remarks>
		protected void RollOverRenameFiles(string baseFileName)
		{
			if (m_maxSizeRollBackups != 0)
			{
				if (m_countDirection < 0)
				{
					if (m_curSizeRollBackups == m_maxSizeRollBackups)
					{
						DeleteFile(CombinePath(baseFileName, "." + m_maxSizeRollBackups.ToString()));
						m_curSizeRollBackups--;
					}
					for (int num = m_curSizeRollBackups; num >= 1; num--)
					{
						RollFile(CombinePath(baseFileName, "." + num.ToString()), CombinePath(baseFileName, "." + (num + 1).ToString()));
					}
					m_curSizeRollBackups++;
					RollFile(baseFileName, CombinePath(baseFileName, ".1"));
				}
				else
				{
					if (m_curSizeRollBackups >= m_maxSizeRollBackups && m_maxSizeRollBackups > 0)
					{
						int num2 = m_curSizeRollBackups - m_maxSizeRollBackups;
						if (m_staticLogFileName)
						{
							num2++;
						}
						string text = baseFileName;
						if (!m_staticLogFileName)
						{
							if (m_preserveLogFileNameExtension)
							{
								string extension = Path.GetExtension(text);
								string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
								int num3 = fileNameWithoutExtension.LastIndexOf(".");
								if (num3 >= 0)
								{
									text = fileNameWithoutExtension.Substring(0, num3) + extension;
								}
							}
							else
							{
								int num4 = text.LastIndexOf(".");
								if (num4 >= 0)
								{
									text = text.Substring(0, num4);
								}
							}
						}
						DeleteFile(CombinePath(text, "." + num2.ToString()));
					}
					if (m_staticLogFileName)
					{
						m_curSizeRollBackups++;
						RollFile(baseFileName, CombinePath(baseFileName, "." + m_curSizeRollBackups.ToString()));
					}
				}
			}
		}

		/// <summary>
		/// Get the start time of the next window for the current rollpoint
		/// </summary>
		/// <param name="currentDateTime">the current date</param>
		/// <param name="rollPoint">the type of roll point we are working with</param>
		/// <returns>the start time for the next roll point an interval after the currentDateTime date</returns>
		/// <remarks>
		/// <para>
		/// Returns the date of the next roll point after the currentDateTime date passed to the method.
		/// </para>
		/// <para>
		/// The basic strategy is to subtract the time parts that are less significant
		/// than the rollpoint from the current time. This should roll the time back to
		/// the start of the time window for the current rollpoint. Then we add 1 window
		/// worth of time and get the start time of the next window for the rollpoint.
		/// </para>
		/// </remarks>
		protected DateTime NextCheckDate(DateTime currentDateTime, RollPoint rollPoint)
		{
			DateTime result = currentDateTime;
			switch (rollPoint)
			{
			case RollPoint.TopOfMinute:
				result = result.AddMilliseconds((double)(-result.Millisecond));
				result = result.AddSeconds((double)(-result.Second)).AddMinutes(1.0);
				break;
			case RollPoint.TopOfHour:
				result = result.AddMilliseconds((double)(-result.Millisecond));
				result = result.AddSeconds((double)(-result.Second));
				result = result.AddMinutes((double)(-result.Minute)).AddHours(1.0);
				break;
			case RollPoint.HalfDay:
				result = result.AddMilliseconds((double)(-result.Millisecond));
				result = result.AddSeconds((double)(-result.Second));
				result = result.AddMinutes((double)(-result.Minute));
				result = ((result.Hour >= 12) ? result.AddHours((double)(-result.Hour)).AddDays(1.0) : result.AddHours((double)(12 - result.Hour)));
				break;
			case RollPoint.TopOfDay:
				result = result.AddMilliseconds((double)(-result.Millisecond));
				result = result.AddSeconds((double)(-result.Second));
				result = result.AddMinutes((double)(-result.Minute));
				result = result.AddHours((double)(-result.Hour)).AddDays(1.0);
				break;
			case RollPoint.TopOfWeek:
				result = result.AddMilliseconds((double)(-result.Millisecond));
				result = result.AddSeconds((double)(-result.Second));
				result = result.AddMinutes((double)(-result.Minute));
				result = result.AddHours((double)(-result.Hour));
				result = result.AddDays((double)(7 - result.DayOfWeek));
				break;
			case RollPoint.TopOfMonth:
				result = result.AddMilliseconds((double)(-result.Millisecond));
				result = result.AddSeconds((double)(-result.Second));
				result = result.AddMinutes((double)(-result.Minute));
				result = result.AddHours((double)(-result.Hour));
				result = result.AddDays((double)(1 - result.Day)).AddMonths(1);
				break;
			}
			return result;
		}
	}
}
