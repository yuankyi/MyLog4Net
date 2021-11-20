namespace log4net.Util
{
	/// <summary>
	/// Delegate type used for LogicalThreadContextStack's callbacks.
	/// </summary>
	public delegate void TwoArgAction<T1, T2>(T1 t1, T2 t2);
}
