using System;

namespace MiniScript
{
	public interface IBinaryOperator
	{
		//BinaryOperatorType BinaryOperatorType { get; }
		//string OperatorCode { get; }
	}

	public abstract class BinaryOperator<T>
		: IBinaryOperator
		//where T : struct, IMValue
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public abstract BinaryOperatorType BinaryOperatorType { get; }
		public int Priority => BinaryOperatorType.GetPriority();
		public abstract string OperatorCode { get; }
		public MiniValue<T> Left;
		public MiniValue<T> Right;


		public abstract MiniValue<T> Evalute(Context<T> context);
	}

}
