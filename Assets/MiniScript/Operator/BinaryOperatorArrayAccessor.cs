using System;
using System.Collections.Generic;


namespace MiniScript
{
	/// <summary>
	/// 配列参照
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BinaryOperatorArrayAccessor<T>
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.ArrayAccessor;
		public override string OperatorCode => "[";


		public override MiniValue<T> Evalute(IContext<T> context)
		{
			MiniValue<T> left = Left.EvaluteInner(context);

			switch(left.GetObject())
			{
				case List<MiniValue<T>> array:
				{
					MiniValue<T> right = Right.EvaluteInner(context);
					int index = right.IntegerValue;
					return array[index];
				}
				default:
					throw new Exception($"not support type:{left.GetObject().GetType().ToString()}");
			}
		}
	}
}
