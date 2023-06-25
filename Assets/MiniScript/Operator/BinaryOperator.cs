using System;
using System.Collections.Generic;

namespace MiniScript
{
	public abstract class BinaryOperator<T>
		: IOperator<T>
		//where T : struct, IMValue
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public abstract OperatorType OperatorType { get; }
		public int Priority => OperatorType.GetPriority();
		public abstract string OperatorCode { get; }

		public MiniValue<T> Left;
		public MiniValue<T> Right;

		public bool IsFinalized
		{
			get
			{
				return Left.ValueType.IsValid() && Right.ValueType.IsValid();
			}
		}

		public virtual List<MiniValue<T>>.Enumerator ConvertToRpn(
			List<MiniValue<T>>.Enumerator enumerator
			, List<MiniValue<T>> rpn
			, out int insertIndex
			)
		{
			// Get right value
			if (!enumerator.MoveNext())
			{
				throw new FormatException("right value nothing.");
			}
			insertIndex = MiniDecoder<T>.GetInsertPosition(rpn, Priority);
			rpn.Insert(insertIndex++, enumerator.Current);
			rpn.Insert(insertIndex, new MiniValue<T>(this));

			return enumerator;
		}

		public virtual MiniValue<T> Finailze(Stack<MiniValue<T>> rpnStack)
		{
			Right = rpnStack.Pop();
			Left = rpnStack.Pop();
			return new MiniValue<T>(this);
		}


		public abstract MiniValue<T> Evalute(IContext<T> context);
	}

}
