using System;

namespace MiniScript
{
	public class BinaryOperatorLogicalDisjunction<T> 
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.LogicalDisjunction;
		public override string OperatorCode => "||";

		public override MiniValue<T> Evalute(IContext<T> context)
		{
			bool left = Left.Evalute(context).ToBool();
			if (left)
			{
				return new MiniValue<T>(true);
			}
			bool right = Right.Evalute(context).ToBool();

			return new MiniValue<T>(left);
		}
	}


}

