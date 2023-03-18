using System;

namespace MiniScript
{
	public class BinaryOperatorGreaterThan<T>
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override BinaryOperatorType BinaryOperatorType => BinaryOperatorType.GreaterThan;
		public override string OperatorCode => ">";

		public override MiniValue<T> Evalute(IContext<T> context)
		{
			MiniValue<T> left = Left.Evalute(context);
			MiniValue<T> right = Right.Evalute(context);

			return new MiniValue<T>(MiniValue<T>.Calculator.GreaterThan(left.Value, right.Value));
		}
	}


}

