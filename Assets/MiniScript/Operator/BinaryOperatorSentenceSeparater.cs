using System;
using System.Collections.Generic;

namespace MiniScript
{
	public class BinaryOperatorSentenceSeparater<T>
		: BinaryOperator<T>
		, IOperatorOnFinalized
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.SentenceSeparator;
		public override string OperatorCode => ";";


		public override List<MiniValue<T>>.Enumerator ConvertToRpn(
			List<MiniValue<T>>.Enumerator enumerator
			, List<MiniValue<T>> rpn
			, out int insertIndex
			)
		{
			insertIndex = MiniDecoder<T>.GetInsertPosition(rpn, Priority);
			// Get right value
			if (enumerator.MoveNext())
			{
				MiniValue<T> rightValue = enumerator.Current;
				while (rightValue.TryGetOperator(out BinaryOperatorSentenceSeparater<T> _))
				{
					// 空文
					if (!enumerator.MoveNext())
					{
						return enumerator;
					}
					rightValue = enumerator.Current;
				}
				rpn.Insert(insertIndex++, rightValue);
				rpn.Insert(insertIndex, new MiniValue<T>(this));
			}
			else
			{
				// 右辺値無し
			}

			return enumerator;
		}


		private List<MiniValue<T>> _values;
		private MiniValue<T>[] _statements;


		public override MiniValue<T> Finailze(Stack<MiniValue<T>> rpnStack)
		{
			base.Finailze(rpnStack);
			if (!Left.TryGetOperator(out BinaryOperatorSentenceSeparater<T> accessor))
			{
				_values = new List<MiniValue<T>>();
				_values.Add(Left);
				_values.Add(Right);
				accessor = this;
			}
			else
			{
				accessor._values.Add(Right);
			}
			Left = MiniValue<T>.Default;
			Right = MiniValue<T>.Default;
			return new MiniValue<T>(accessor);
		}

		public bool IsOnFinishedRequired => _values is not null;
		public void OnFinalized()
		{
			_statements = _values.ToArray();
			_values = null;
		}


		public override MiniValue<T> Evalute(IContext<T> context)
		{
			MiniValue<T> lastValue = default;
			foreach	(var value in _statements)
			{
				lastValue = value.Evalute(context);
				if (lastValue.ValueType.IsLoopControl())
				{
					return lastValue;
				}
			}
			return lastValue;
		}
	}


}

