using System;
using System.Collections.Generic;

namespace MiniScript
{
	public class BinaryOperatorDictionaryAccessor<T>
		: BinaryOperator<T>
		, IOperatorOnFinalized
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public const string ConstOperationCode = ".";
		public override OperatorType OperatorType => OperatorType.DictionaryAccessor;
		public override string OperatorCode => ConstOperationCode;

		private List<string> _values;
		private string[] _splitedPath;


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

		public bool IsOnFinishedRequired => _values is not null;
		public void OnFinalized()
		{
			_splitedPath = _values.ToArray();
			_values = null;
		}

		public override MiniValue<T> Evalute(IContext<T> context)
		{
			return GetByPath(context, _splitedPath);
		}
		public static MiniValue<T> GetByPath(IContext<T> context, string[] hierarchy)
		{
			MiniValue<T> miniValue = default;
			int lastIndex = hierarchy.Length - 1;
			for	(int i = 0; i < lastIndex; i++)
			{
				if (context.TryGetValue(hierarchy[i], out MiniValue<T> childValue))
				{
					miniValue = childValue;
					context = miniValue.GetDictionary();
				}
				else
				{
					throw new InvalidOperationException($"undefined {string.Join(ConstOperationCode, hierarchy)}");
				}
			}
			if (context.TryGetValue(hierarchy[lastIndex], out miniValue))
			{
				return miniValue;
			}
			throw new InvalidOperationException($"undefined {string.Join(ConstOperationCode, hierarchy)}");
		}


		public void AssignmentTo(IContext<T> context, MiniValue<T> value)
		{
			IContext<T> dictionary = null;
			string contextKey = _splitedPath[0];
			if (context.TryGetValue(contextKey, out MiniValue<T> miniValue))
			{
				dictionary = miniValue.GetDictionary();
			}
			if (dictionary is null)
			{
				dictionary = context.Instantiate();
				miniValue = new MiniValue<T>(dictionary);
				context[contextKey] = miniValue;
			}

			int lastIndex = _splitedPath.Length - 1;
			for (int i = 1; i < lastIndex; i++)
			{
				string key = _splitedPath[i];
				if (dictionary.TryGetValue(key, out MiniValue<T> childValue))
				{
					miniValue = childValue;
					dictionary = miniValue.GetDictionary();
				}
				else
				{
					var childDictionary = context.Instantiate();
					dictionary[key] = new MiniValue<T>(childDictionary);
					dictionary = childDictionary;
				}
			}
			dictionary[_splitedPath[lastIndex]] = value;
		}
	}


}

