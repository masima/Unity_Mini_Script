using System;

namespace MiniScript
{
	public class BinaryOperatorLogicalConjunction<T> 
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.LogicalConjunction;
		public override string OperatorCode => "&&";

		public override MiniValue<T> Evalute(IContext<T> context)
		{
			bool left = Left.Evalute(context).ToBool();
			bool right = Right.Evalute(context).ToBool();

			return new MiniValue<T>(left && right);
		}
	}


}

