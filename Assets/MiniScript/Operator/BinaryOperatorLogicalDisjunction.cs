﻿using System;

namespace MiniScript
{
	public class BinaryOperatorLogicalDisjunction<T> 
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override BinaryOperatorType BinaryOperatorType => BinaryOperatorType.LogicalDisjunction;
		public override string OperatorCode => "||";

		public override MiniValue<T> Evalute(Context<T> context)
		{
			bool left = Left.Evalute(context).ToBool();
			bool right = Right.Evalute(context).ToBool();

			return new MiniValue<T>(left || right);
		}
	}


}
