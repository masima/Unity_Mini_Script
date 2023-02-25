using System;

namespace MiniScript
{
    public class BinaryOperatorMinus<T> 
        : BinaryOperator<T>
        where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
        , IComparable<T>
    {
        public override BinaryOperatorType BinaryOperatorType => BinaryOperatorType.Minus;
		public override string OperatorCode => "-";

        public override MiniValue<T> Evalute(Context<T> context)
        {
            MiniValue<T> left = Left.Evalute(context);
            MiniValue<T> right = Right.Evalute(context);

            return new MiniValue<T>(MiniValue<T>.Calculator.Substruct(left.Value, right.Value));
        }
    }


}

