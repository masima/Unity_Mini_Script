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
			ILoopControl loopControl = null;
			while (Judge.EvaluteInner(context).ToBool())
			{
				MiniValue<T> result = Statement.EvaluteInner(context);
				if (result.ValueType.IsLoopControl())
				{
					if (result.TryGetOperator(out loopControl)
						&& loopControl.FlowControlFlag.IsBreak()
						)
					{
						break;
					}
				}
			}

			if (loopControl is not null
				&& loopControl.FlowControlFlag.IsReturn())
			{
				return new MiniValue<T>(loopControl);
			}
			return default;
		}
	}
}
