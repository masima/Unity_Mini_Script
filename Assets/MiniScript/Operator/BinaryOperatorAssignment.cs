using System;

namespace MiniScript
{
	public class BinaryOperatorAssignment<T>
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override BinaryOperatorType BinaryOperatorType => BinaryOperatorType.Assignment;
		public override string OperatorCode => "=";

		public override MiniValue<T> Evalute(Context<T> context)
		{
			MiniValue<T> right = Right.Evalute(context);

			if (Left.ValueType == EValueType.Variable)
			{
				Left.AssignmentTo(context, right);
			}

			return right;
		}
	}


}

