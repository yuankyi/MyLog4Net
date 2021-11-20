using log4net.Appender;
using log4net.Layout;
using log4net.Repository;
using log4net.Util;
using System;
using System.Collections;
using System.Reflection;

namespace log4net.Config
{
	/// <summary>
	/// Use this class to quickly configure a <see cref="T:log4net.Repository.Hierarchy.Hierarchy" />.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Allows very simple programmatic configuration of log4net.
	/// </para>
	/// <para>
	/// Only one appender can be configured using this configurator.
	/// The appender is set at the root of the hierarchy and all logging
	/// events will be delivered to that appender.
	/// </para>
	/// <para>
	/// Appenders can also implement the <see cref="T:log4net.Core.IOptionHandler" /> interface. Therefore
	/// they would require that the <see cref="M:log4net.Core.IOptionHandler.ActivateOptions()" /> method
	/// be called after the appenders properties have been configured.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	public sealed class BasicConfigurator
	{
		/// <summary>
		/// The fully qualified type of the BasicConfigurator class.
		/// </summary>
		/// <remarks>
		/// Used by the internal logger to record the Type of the
		/// log message.
		/// </remarks>
		private static readonly Type declaringType = typeof(BasicConfigurator);

		/// <summary>
		/// Initializes a new instance of the <see cref="T:log4net.Config.BasicConfigurator" /> class. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// Uses a private access modifier to prevent instantiation of this class.
		/// </para>
		/// </remarks>
		private BasicConfigurator()
		{
		}

		/// <summary>
		/// Initializes the log4net system with a default configuration.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Initializes the log4net logging system using a <see cref="T:log4net.Appender.ConsoleAppender" />
		/// that will write to <c>Console.Out</c>. The log messages are
		/// formatted using the <see cref="T:log4net.Layout.PatternLayout" /> layout object
		/// with the <see cref="F:log4net.Layout.PatternLayout.DetailConversionPattern" />
		/// layout style.
		/// </para>
		/// </remarks>
		public static ICollection Configure()
		{
			return Configure(LogManager.GetRepository(Assembly.GetCallingAssembly()));
		}

		/// <summary>
		/// Initializes the log4net system using the specified appenders.
		/// </summary>
		/// <param name="appenders">The appenders to use to log all logging events.</param>
		/// <remarks>
		/// <para>
		/// Initializes the log4net system using the specified appenders.
		/// </para>
		/// </remarks>
		public static ICollection Configure(params IAppender[] appenders)
		{
			ArrayList arrayList = new ArrayList();
			ILoggerRepository repository = LogManager.GetRepository(Assembly.GetCallingAssembly());
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				InternalConfigure(repository, appenders);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		/// <summary>
		/// Initializes the log4net system using the specified appender.
		/// </summary>
		/// <param name="appender">The appender to use to log all logging events.</param>
		/// <remarks>
		/// <para>
		/// Initializes the log4net system using the specified appender.
		/// </para>
		/// </remarks>
		public static ICollection Configure(IAppender appender)
		{
			return Configure(new IAppender[1]
			{
				appender
			});
		}

		/// <summary>
		/// Initializes the <see cref="T:log4net.Repository.ILoggerRepository" /> with a default configuration.
		/// </summary>
		/// <param name="repository">The repository to configure.</param>
		/// <remarks>
		/// <para>
		/// Initializes the specified repository using a <see cref="T:log4net.Appender.ConsoleAppender" />
		/// that will write to <c>Console.Out</c>. The log messages are
		/// formatted using the <see cref="T:log4net.Layout.PatternLayout" /> layout object
		/// with the <see cref="F:log4net.Layout.PatternLayout.DetailConversionPattern" />
		/// layout style.
		/// </para>
		/// </remarks>
		public static ICollection Configure(ILoggerRepository repository)
		{
			ArrayList arrayList = new ArrayList();
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				PatternLayout patternLayout = new PatternLayout();
				patternLayout.ConversionPattern = "%timestamp [%thread] %level %logger %ndc - %message%newline";
				patternLayout.ActivateOptions();
				ConsoleAppender consoleAppender = new ConsoleAppender();
				consoleAppender.Layout = patternLayout;
				consoleAppender.ActivateOptions();
				InternalConfigure(repository, consoleAppender);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		/// <summary>
		/// Initializes the <see cref="T:log4net.Repository.ILoggerRepository" /> using the specified appender.
		/// </summary>
		/// <param name="repository">The repository to configure.</param>
		/// <param name="appender">The appender to use to log all logging events.</param>
		/// <remarks>
		/// <para>
		/// Initializes the <see cref="T:log4net.Repository.ILoggerRepository" /> using the specified appender.
		/// </para>
		/// </remarks>
		public static ICollection Configure(ILoggerRepository repository, IAppender appender)
		{
			return Configure(repository, new IAppender[1]
			{
				appender
			});
		}

		/// <summary>
		/// Initializes the <see cref="T:log4net.Repository.ILoggerRepository" /> using the specified appenders.
		/// </summary>
		/// <param name="repository">The repository to configure.</param>
		/// <param name="appenders">The appenders to use to log all logging events.</param>
		/// <remarks>
		/// <para>
		/// Initializes the <see cref="T:log4net.Repository.ILoggerRepository" /> using the specified appender.
		/// </para>
		/// </remarks>
		public static ICollection Configure(ILoggerRepository repository, params IAppender[] appenders)
		{
			ArrayList arrayList = new ArrayList();
			using (new LogLog.LogReceivedAdapter(arrayList))
			{
				InternalConfigure(repository, appenders);
			}
			repository.ConfigurationMessages = arrayList;
			return arrayList;
		}

		private static void InternalConfigure(ILoggerRepository repository, params IAppender[] appenders)
		{
			IBasicRepositoryConfigurator basicRepositoryConfigurator = repository as IBasicRepositoryConfigurator;
			if (basicRepositoryConfigurator != null)
			{
				basicRepositoryConfigurator.Configure(appenders);
			}
			else
			{
				LogLog.Warn(declaringType, "BasicConfigurator: Repository [" + ((repository != null) ? repository.ToString() : null) + "] does not support the BasicConfigurator");
			}
		}
	}
}
