using System;
using System.Collections.Generic;

namespace MiniScript
{
	public class DefinitionOperatorDictionary<T>
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.ArraySeparator;
		public override string OperatorCode => String.Empty;

		private List<BinaryOperatorKeyValuePair<T>> _values;
		public List<BinaryOperatorKeyValuePair<T>> Values => _values;
		private IContext<T> _results;


		public DefinitionOperatorDictionary()
		{

		}

		public DefinitionOperatorDictionary(IContext<T> context, MiniValue<T> value)
		{
			Left = MiniValue<T>.Default;
			Right = MiniValue<T>.Default;
			_values = new List<BinaryOperatorKeyValuePair<T>>();
			_results = context;
			context.Clear();
			if (value.TryGetOperator(out BinaryOperatorArraySeparater<T> separater))
			{
			foreach (MiniValue<T> v in separater.Values)
				{
					if (v.TryGetOperator(out BinaryOperatorKeyValuePair<T> pair))
					{
						_values.Add(pair);
					}
					else
					{
						throw new FormatException();
					}
				}
			}
			else if (value.TryGetOperator(out BinaryOperatorKeyValuePair<T> pair))
			{
				_values.Add(pair);
			}
		}

		public override MiniValue<T> Evalute(IContext<T> context)
		{
			_results.Clear();
			foreach (BinaryOperatorKeyValuePair<T> value in _values)
			{
				string key = value.Left.StringValue;
				MiniValue<T> result = value.Right.EvaluteInner(context);
				_results[key] = result;
			}
			return new MiniValue<T>(_results);
		}
	}


}

