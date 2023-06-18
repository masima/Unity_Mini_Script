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

		public IContext<T> Instantiate();
		public int Count { get; }
		public bool TryGetValue(string key, out MiniValue<T> value);
	}
	public class Context<T>
		: Dictionary<string, MiniValue<T>>
		, IContext<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public IContext<T> Instantiate()
		{
			return new Context<T>();
		}

		public Context<T> Set(string key, T value)
		{
			this[key] = new MiniValue<T>(value);
			return this;
		}

		public MiniValue<T> GetByPath(string path)
		{
			return BinaryOperatorDictionaryAccessor<T>.GetByPath(
				this
				, path.Split(BinaryOperatorDictionaryAccessor<T>.ConstOperationCode));
		}
	}
}
