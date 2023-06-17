using System;
using System.Collections.Generic;

namespace MiniScript
{
	public class BinaryOperatorDictionaryAccessor<T>
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.DictionaryAccessor;
		public override string OperatorCode => ".";

		private List<string> _values;


		public override MiniValue<T> Finailze(Stack<MiniValue<T>> rpnStack)
		{
			base.Finailze(rpnStack);
			if (!Left.TryGetOperator(out BinaryOperatorDictionaryAccessor<T> accessor))
			{
				_values = new List<string>();
				_values.Add(Left.StringValue);
				_values.Add(Right.StringValue);
				accessor = this;
			}
			else
			{
				accessor._values.Add(Right.StringValue);
			}
			Left = MiniValue<T>.Default;
			Right = MiniValue<T>.Default;
			return new MiniValue<T>(accessor);
		}

		public override MiniValue<T> Evalute(IContext<T> context)
		{
			Dictionary<string, MiniValue<T>> dictionary = null;
			string contextKey = _values[0];
			if (context.TryGetValue(contextKey, out MiniValue<T> miniValue))
			{
				dictionary = miniValue.GetDictionary();
			}
			if (dictionary is null)
			{
				throw new InvalidOperationException($"undefined {string.Join(OperatorCode, _values)}");
			}

			for (int i = 1; i < _values.Count; i++)
			{
				if (dictionary.TryGetValue(_values[i], out MiniValue<T> childValue))
				{
					miniValue = childValue;
					dictionary = miniValue.GetDictionary();
				}
				else
				{
					throw new InvalidOperationException($"undefined {string.Join(OperatorCode, _values)}");
				}
			}

			return miniValue;
		}

		public void AssignmentTo(IContext<T> context, MiniValue<T> value)
		{
			Dictionary<string, MiniValue<T>> dictionary = null;
			string contextKey = _values[0];
			if (context.TryGetValue(contextKey, out MiniValue<T> miniValue))
			{
				dictionary = miniValue.GetDictionary();
			}
			if (dictionary is null)
			{
				dictionary = new Dictionary<string, MiniValue<T>>();
				miniValue = new MiniValue<T>(dictionary);
				context[contextKey] = miniValue;
			}

			int lastIndex = _values.Count - 1;
			for (int i = 1; i < lastIndex; i++)
			{
				string key = _values[i];
				if (dictionary.TryGetValue(key, out MiniValue<T> childValue))
				{
					miniValue = childValue;
					dictionary = miniValue.GetDictionary();
				}
				else
				{
					var childDictionary = new Dictionary<string, MiniValue<T>>();
					dictionary[key] = new MiniValue<T>(childDictionary);
					dictionary = childDictionary;
				}
			}
			dictionary[_values[lastIndex]] = value;
		}
	}


}

