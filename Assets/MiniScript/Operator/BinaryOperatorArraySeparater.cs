using System;
using System.Collections.Generic;

namespace MiniScript
{
	public class BinaryOperatorArraySeparater<T>
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override BinaryOperatorType BinaryOperatorType => BinaryOperatorType.ArraySeparator;
		public override string OperatorCode => ",";

		private List<MiniValue<T>> _values;

		public override MiniValue<T> Evalute(IContext<T> context)
		{
			MiniValue<T> left = Left.Evalute(context);
			MiniValue<T> right = Right.Evalute(context);

			if (left.ValueType != EValueType.Array)
			{
				if (_values is null)
				{
					_values = new();
				}
				_values.Clear();
				_values.Add(left);
			}
			else
			{
				_values = left.GetArray();
			}
			_values.Add(right);

			return new MiniValue<T>(_values);
		}
	}


}

