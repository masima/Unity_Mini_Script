using System;

namespace MiniScript
{
	public class BinaryOperatorEqual<T>
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.Equal;
		public override string OperatorCode => "==";

		public override MiniValue<T> Evalute(IContext<T> context)
		{
			MiniValue<T> left = Left.EvaluteInner(context);
			MiniValue<T> right = Right.EvaluteInner(context);

			return new MiniValue<T>(left.Value.Equals(right.Value));
		}
	}


}

