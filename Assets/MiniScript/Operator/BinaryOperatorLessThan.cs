using System;

namespace MiniScript
{
	public class BinaryOperatorLessThan<T>
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override BinaryOperatorType BinaryOperatorType => BinaryOperatorType.LessThan;
		public override string OperatorCode => "<";

		public override MiniValue<T> Evalute(Context<T> context)
		{
			MiniValue<T> left = Left.Evalute(context);
			MiniValue<T> right = Right.Evalute(context);

			return new MiniValue<T>(MiniValue<T>.Calculator.LessThan(left.Value, right.Value));
		}
	}


}

