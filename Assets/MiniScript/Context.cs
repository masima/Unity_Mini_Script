using System;
using System.Collections.Generic;


namespace MiniScript
{
	public interface IContext<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public MiniValue<T> this[string key]
		{
			get;
			set;
		}

		public bool TryGetValue(string key, out MiniValue<T> value);
	}
	public class Context<T>
		: Dictionary<string, MiniValue<T>>
		, IContext<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{

	}
}
