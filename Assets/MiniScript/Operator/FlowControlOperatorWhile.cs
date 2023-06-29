using System;
using System.Collections.Generic;


namespace MiniScript
{
	public class FlowControlOperatorWhile<T>
		: FlowControlOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.FlowControl;
		public override string OperatorCode => "while";


		public override MiniValue<T> Evalute(IContext<T> context)
		{
			while (Judge.Evalute(context).ToBool())
			{
				MiniValue<T> result = Statement.Evalute(context);
				if (result.ValueType.IsLoopControl())
				{
					if (result.TryGetOperator(out FlowControlOperatorBreak<T> _))
					{
						break;
					}
				}
			}
			return default;
		}
	}
}
