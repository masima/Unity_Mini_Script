using System;
using System.Collections.Generic;

namespace MiniScript
{
	public class MiniList<T>
		: List<MiniValue<T>>
		, IContext<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public MiniValue<T> this[string key]
		{
			get
			{
				throw new InvalidOperationException();
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public IContext<T> Instantiate()
		{
			return new MiniList<T>();
		}
		// public int Count { get; }
		public bool TryGetValue(string key, out MiniValue<T> value)
		{
			switch (key)
			{
				case "add":
					value = new MiniValue<T>(Add);
					return true;
				case "clear":
					value = new MiniValue<T>(ClearList);
					return true;
				case "count":
					value = new MiniValue<T>(Count);
					return true;
				case "insert":
					value = new MiniValue<T>(Insert);
					return true;
				case "pop":
					value = new MiniValue<T>(Pop);
					return true;
				case "removeat":
					value = new MiniValue<T>(RemoveAt);
					return true;
				default:
					throw new InvalidOperationException();
			}
		}
		private MiniValue<T> Add(IContext<T> context, List<MiniValue<T>> values)
		{
			foreach (MiniValue<T> value in values)
			{
				Add(value.EvaluteInner(context));
			}
			return new MiniValue<T>(this);
		}
		private MiniValue<T> ClearList(IContext<T> context, List<MiniValue<T>> values)
		{
			Clear();
			return new MiniValue<T>(this);
		}
		private MiniValue<T> Insert(IContext<T> context, List<MiniValue<T>> values)
		{
			int index = values[0].EvaluteInner(context).IntegerValue;
			for (int i = 1; i < values.Count; i++)
			{
				Insert(index + i - 1, values[i].EvaluteInner(context));
			}
			return new MiniValue<T>(this);
		}
		private MiniValue<T> RemoveAt(IContext<T> context, List<MiniValue<T>> values)
		{
			int index = values[0].EvaluteInner(context).IntegerValue;
			RemoveAt(index);
			return new MiniValue<T>(this);
		}
		private MiniValue<T> Pop(IContext<T> context, List<MiniValue<T>> values)
		{
			int lastIndex = Count - 1;
			MiniValue<T> lastValue = this[lastIndex].EvaluteInner(context);
			RemoveAt(lastIndex);
			return lastValue;
		}
	}
}
