using log4net.Core;

namespace log4net.Filter
{
	/// <summary>
	/// Implement this interface to provide customized logging event filtering
	/// </summary>
	/// <remarks>
	/// <para>
	/// Users should implement this interface to implement customized logging
	/// event filtering. Note that <see cref="T:log4net.Repository.Hierarchy.Logger" /> and 
	/// <see cref="T:log4net.Appender.AppenderSkeleton" />, the parent class of all standard
	/// appenders, have built-in filtering rules. It is suggested that you
	/// first use and understand the built-in rules before rushing to write
	/// your own custom filters.
	/// </para>
	/// <para>
	/// This abstract class assumes and also imposes that filters be
	/// organized in a linear chain. The <see cref="M:log4net.Filter.IFilter.Decide(log4net.Core.LoggingEvent)" />
	/// method of each filter is called sequentially, in the order of their 
	/// addition to the chain.
	/// </para>
	/// <para>
	/// The <see cref="M:log4net.Filter.IFilter.Decide(log4net.Core.LoggingEvent)" /> method must return one
	/// of the integer constants <see cref="F:log4net.Filter.FilterDecision.Deny" />, 
	/// <see cref="F:log4net.Filter.FilterDecision.Neutral" /> or <see cref="F:log4net.Filter.FilterDecision.Accept" />.
	/// </para>
	/// <para>
	/// If the value <see cref="F:log4net.Filter.FilterDecision.Deny" /> is returned, then the log event is dropped 
	/// immediately without consulting with the remaining filters.
	/// </para>
	/// <para>
	/// If the value <see cref="F:log4net.Filter.FilterDecision.Neutral" /> is returned, then the next filter
	/// in the chain is consulted. If there are no more filters in the
	/// chain, then the log event is logged. Thus, in the presence of no
	/// filters, the default behavior is to log all logging events.
	/// </para>
	/// <para>
	/// If the value <see cref="F:log4net.Filter.FilterDecision.Accept" /> is returned, then the log
	/// event is logged without consulting the remaining filters.
	/// </para>
	/// <para>
	/// The philosophy of log4net filters is largely inspired from the
	/// Linux ipchains.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	public interface IFilter : IOptionHandler
	{
		/// <summary>
		/// Property to get and set the next filter
		/// </summary>
		/// <value>
		/// The next filter in the chain
		/// </value>
		/// <remarks>
		/// <para>
		/// Filters are typically composed into chains. This property allows the next filter in 
		/// the chain to be accessed.
		/// </para>
		/// </remarks>
		IFilter Next
		{
			get;
			set;
		}

		/// <summary>
		/// Decide if the logging event should be logged through an appender.
		/// </summary>
		/// <param name="loggingEvent">The LoggingEvent to decide upon</param>
		/// <returns>The decision of the filter</returns>
		/// <remarks>
		/// <para>
		/// If the decision is <see cref="F:log4net.Filter.FilterDecision.Deny" />, then the event will be
		/// dropped. If the decision is <see cref="F:log4net.Filter.FilterDecision.Neutral" />, then the next
		/// filter, if any, will be invoked. If the decision is <see cref="F:log4net.Filter.FilterDecision.Accept" /> then
		/// the event will be logged without consulting with other filters in
		/// the chain.
		/// </para>
		/// </remarks>
		FilterDecision Decide(LoggingEvent loggingEvent);
	}
}
