using System;
using System.Collections;
using System.Collections.Generic;

namespace MiniScript
{
	public class BinaryOperatorFunction<T> 
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.Function;
		public override string OperatorCode => "(";


		public override MiniValue<T> Evalute(IContext<T> context)
		{
			MiniValue<T> left = Left.EvaluteInner(context);
			MiniValue<T> right = Right.EvaluteInner(context);

			var func = left.GetObject<MiniValue<T>.Function>();
			var parameters = right.GetObject<List<MiniValue<T>>>();

			MiniValue<T> result = func.Invoke(context, parameters);
			if (result.TryGetOperator(out FlowControlOperatorReturn<T> operatorReturn))
			{
				return operatorReturn.ReturnValue;
			}
			return result;
		}
	}


}

