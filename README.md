# MyLog4Net

# 步骤  ----------------------------------------

## 1. 引用

使用 NuGet 添加对 log4net.dll 的引用

## 2. 配置


#### 2.1 自定义配置文件

可以为任意文件名(也可以直接配置在app.config/web.config)

log4net框架会在相对于AppDomain.CurrentDomain.BaseDirectory 属性定义的目录路径下查找配置文件。

框架在配置文件里要查找的唯一标识是<log4net>标签

    <log4net>
        <root>
        	<Logger>
        	<appender>
        		<layout>
        		<filter>

#### 2.2 关联配置文件

在项目的AssemblyInfo.cs文件里添加以下的语句, 关联2.1创建的配置文件
```csharp
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "mylog.config", Watch = true)]
```

## 3. 使用

```csharp
Log4net.ILog log = Log4net.LogManager.GetLogger("MyLogger");
if (log.IsInfoEnabled) log.Info("message);
```




# 组件  ----------------------------------------

## Logger（记录器）

Logger是应用程序需要交互的主要组件，它用来产生日志消息

同名的logger属于同一个实例, 地址一样

每个实例化的Logger对象对被log4net作为命名实体（Named Entity）来维护。
log4net使用继承体系，也就是说假如存在两个Logger，名字分别为a.b.c和a.b。那么a.b就是a.b.c的祖先。
每个Logger都继承了它祖先的属性。所有的Logger都从Root继承,Root本身也是一个Logger。

#### 日志的等级:

它们由高到底分别为：

OFF > FATAL > ERROR > WARN > INFO > DEBUG  > ALL 

在具体写日志时，一般可以这样理解日志等级：

- FATAL（致命错误）：记录系统中出现的能使用系统完全失去功能，服务停止，系统崩溃等使系统无法继续运行下去的错误。例如，数据库无法连接，系统出现死循环。

- ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。

- WARN（警告）：记录系统中不影响系统继续运行，但不符合系统运行正常条件，有可能引起系统错误的信息。例如，记录内容为空，数据内容不正确等。

- INFO（一般信息）：记录系统运行中应该让用户知道的基本信息。例如，服务开始运行，功能已经开户等。

- DEBUG （调试信息）：记录系统用于调试的一切信息，内容或者是一些关键数据内容的输出。



## Repository（库）

epository主要用于负责日志对象组织结构的维护。


## Appender（附着器）

Appenders用来定义日志的输出方式，即日志要写到那种介质上去。
可以继承log4net.Appender.AppenderSkeleton类自定义。

#### 已实现的输出方式：

- AdoNetAppender 将日志记录到数据库中。可以采用SQL和存储过程两种方式。

	`buffersize 指缓存数据条数, 达到指定值后一并将数据写入数据库`

- AnsiColorTerminalAppender 将日志高亮输出到ANSI终端。

- AspNetTraceAppender  能用asp.net中Trace的方式查看记录的日志。

- BufferingForwardingAppender 在输出到子Appenders之前先缓存日志事件。

- ConsoleAppender 将日志输出到应用程序控制台。

- EventLogAppender 将日志写到Windows Event Log。

- FileAppender 将日志输出到文件。

- ForwardingAppender 发送日志事件到子Appenders。

- LocalSyslogAppender 将日志写到local syslog service (仅用于UNIX环境下)。

- MemoryAppender 将日志存到内存缓冲区。

- NetSendAppender 将日志输出到Windows Messenger service.这些日志信息将在用户终端的对话框中显示。

- OutputDebugStringAppender 将日志输出到Debuger，如果程序没有Debuger，就输出到系统Debuger。如果系统Debuger也不可用，将忽略消息。

- RemoteSyslogAppender 通过UDP网络协议将日志写到Remote syslog service。

- RemotingAppender 通过.NET Remoting将日志写到远程接收端。

- RollingFileAppender 将日志以回滚文件的形式写到文件中。

- SmtpAppender 将日志写到邮件中。

- SmtpPickupDirAppender 将消息以文件的方式放入一个目录中，像IIS SMTP agent这样的SMTP代理就可以阅读或发送它们。

- TelnetAppender 客户端通过Telnet来接受日志事件。

- TraceAppender 将日志写到.NET trace 系统。

- UdpAppender 将日志以无连接UDP数据报的形式送到远程宿主或用UdpClient的形式广播。

## Appender Filters

使用过滤器可以过滤掉Appender输出的内容。

#### 过滤器通常有以下几种：

- DenyAllFilter 阻止所有的日志事件被记录

- LevelMatchFilter 只有指定等级的日志事件才被记录

- LevelRangeFilter 日志等级在指定范围内的事件才被记录

- LoggerMatchFilter 与Logger名称匹配，才记录

- PropertyFilter 消息匹配指定的属性值时才被记录

- StringMathFilter 消息匹配指定的字符串才被记录


## Layout（布局）

Layout用于控制Appender的输出格式，可以是线性的也可以是XML。

一个Appender只能有一个Layout。

最常用的Layout应该是经典格式的PatternLayout

Layout可以自己实现，需要从log4net.Layout.LayoutSkeleton类继承

#### 具体的格式有以下内置可选项：

- %m(message):输出的日志消息，如ILog.Debug(…)输出的一条消息 

- %n(new line):换行 

- %d(datetime):输出当前语句运行的时刻 

- %r(run time):输出程序从运行到执行到当前语句时消耗的毫秒数 

- %t(thread id):当前语句所在的线程ID 

- %p(priority): 日志的当前优先级别，即DEBUG、INFO、WARN…等 

- %c(class):当前日志对象的名称，例如： 

- %f(file):输出语句所在的文件名。 

- %l(line)：输出语句所在的行号。 

- %数字：表示该项的最小长度，如果不够，则用空格填充，如“%-5level”表示level的最小宽度是5个字符，如果实际长度不够5个字符则以空格填充。
