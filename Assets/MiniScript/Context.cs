using System;
using System.Collections.Generic;


namespace MiniScript
{
	public class Context<T> : Dictionary<string, MiniValue<T>>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public void Set(string key, T value)
		{
			this[key] = new MiniValue<T>(value);
		}

	}
}
