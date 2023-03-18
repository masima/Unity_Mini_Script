using System;

namespace MiniScript
{
	public class BinaryOperatorSentenceSeparater<T>
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override BinaryOperatorType BinaryOperatorType => BinaryOperatorType.SentenceSeparator;
		public override string OperatorCode => ";";

		public override MiniValue<T> Evalute(IContext<T> context)
		{
			MiniValue<T> left = Left.Evalute(context);
			MiniValue<T> right = Right.Evalute(context);

			return right;
		}
	}


}

