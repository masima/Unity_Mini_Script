using System;

namespace MiniScript
{
	public class BinaryOperatorAssignment<T>
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.Assignment;
		public override string OperatorCode => "=";

		public override MiniValue<T> Evalute(IContext<T> context)
		{
			MiniValue<T> right = Right.EvaluteInner(context);
			Left.AssignmentTo(context, right);

			return right;
		}
	}


}

