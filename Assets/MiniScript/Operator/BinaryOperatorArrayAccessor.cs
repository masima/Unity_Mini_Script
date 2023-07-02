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
		, IAssignmentTo<T>
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
				case IContext<T> targetContext:
				{
					MiniValue<T> right = Right.EvaluteInner(context);
					switch (right.ValueType)
					{
						case EValueType.Const:
						case EValueType.String:
							return targetContext[right.StringValue];
						default:
							throw new InvalidOperationException($"invalid value type : {right.ValueType.ToString()}");
					}
				}
			default:
					throw new Exception($"not support type:{left.GetObject().GetType().ToString()}");
			}
		}

		public void AssignmentTo(IContext<T> context, MiniValue<T> value)
		{
			MiniValue<T> left = Left.EvaluteInner(context);

			switch (left.GetObject())
			{
			case List<MiniValue<T>> array:
				{
					MiniValue<T> right = Right.EvaluteInner(context);
					int index = right.IntegerValue;
					array[index] = value;
					return;
				}
			case IContext<T> targetContext:
				{
					MiniValue<T> right = Right.EvaluteInner(context);
					switch (right.ValueType)
					{
					case EValueType.Const:
					case EValueType.String:
						targetContext[right.StringValue] = value;
						return;
					default:
						throw new InvalidOperationException($"invalid value type : {right.ValueType.ToString()}");
					}
				}
			default:
				throw new Exception($"not support type:{left.GetObject().GetType().ToString()}");
			}
		}
	}
}
