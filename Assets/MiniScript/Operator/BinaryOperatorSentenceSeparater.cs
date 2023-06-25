using System;
using System.Collections.Generic;

namespace MiniScript
{
	public class BinaryOperatorSentenceSeparater<T>
		: BinaryOperator<T>
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


		public override MiniValue<T> Evalute(IContext<T> context)
		{
			MiniValue<T> left = Left.Evalute(context);
			MiniValue<T> right = Right.Evalute(context);

			return right;
		}
	}


}

