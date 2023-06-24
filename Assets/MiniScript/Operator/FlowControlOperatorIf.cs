using System;
using System.Collections.Generic;


namespace MiniScript
{
	public class FlowControlOperatorIf<T>
		: FlowControlOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.FlowControl;
		public override string OperatorCode => "if";


		public override MiniValue<T> Evalute(IContext<T> context)
		{
			MiniValue<T> result = Judge.Evalute(context);

			if (result.ToBool())
			{
				Statement.Evalute(context);
			}
			return result;
		}
	}
}
