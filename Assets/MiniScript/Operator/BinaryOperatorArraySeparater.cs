using System;
using System.Collections.Generic;

namespace MiniScript
{
	public class BinaryOperatorArraySeparater<T>
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.ArraySeparator;
		public override string OperatorCode => ",";

		private List<MiniValue<T>> _values;
		private MiniList<T> _results;


		public static MiniValue<T> InstantiateSingleParameter(MiniValue<T> value)
		{
			var arraySeparater = new BinaryOperatorArraySeparater<T>();
			arraySeparater._results = new MiniList<T>();
			arraySeparater._values = new List<MiniValue<T>>();
			arraySeparater._values.Clear();
			if (value.ValueType.IsValid())
			{
				arraySeparater._values.Add(value);
			}
			arraySeparater.Left = MiniValue<T>.Default;
			arraySeparater.Right = MiniValue<T>.Default;

			return new MiniValue<T>(arraySeparater);
		}

		public override MiniValue<T> Finailze(Stack<MiniValue<T>> rpnStack)
		{
			base.Finailze(rpnStack);
			if (!Left.TryGetOperator(out BinaryOperatorArraySeparater<T> separater))
			{
				_results = new MiniList<T>();
				_values = new List<MiniValue<T>>();
				_values.Add(Left);
				_values.Add(Right);
				separater = this;
			}
			else
			{
				separater._values.Add(Right);
			}
			separater.FinalizeInner();
			return new MiniValue<T>(separater);
		}
		private void FinalizeInner()
		{
			Left = MiniValue<T>.Default;
			Right = MiniValue<T>.Default;
			if (_results.Capacity < _values.Count)
			{
				if (0 == _results.Capacity)
				{
					_results.Capacity = 4;
				}
				else
				{
					_results.Capacity *= 2;
				}
			}
		}


		public override MiniValue<T> Evalute(IContext<T> context)
		{
			// _results.Capacity = _values.Count;
			_results.Clear();
			foreach	(MiniValue<T> value in _values)
			{
				_results.Add(value.Evalute(context));
			}
			return new MiniValue<T>(_results);
		}
	}


}

