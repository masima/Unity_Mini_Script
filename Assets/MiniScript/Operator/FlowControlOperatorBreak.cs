using System.Diagnostics;
using System;
using System.Collections.Generic;


namespace MiniScript
{
	public interface ILoopControl {}

	public class FlowControlOperatorBreak<T>
		: FlowControlOperator<T>, ILoopControl
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.FlowControl;
		public override string OperatorCode => "break";


		public override MiniValue<T> SplitSentence(
			MiniDecoder<T> decoder
			, string sentence
			, ref int startat
			)
		{
			MiniValue<T> flowControlValue = new(this);
			// Judge = MiniValue<T>.Default;
			Statement = MiniValue<T>.Default;
			return flowControlValue;
		}

		public override MiniValue<T> Evalute(IContext<T> context)
		{
			if (Judge.ValueType.IsValid())
			{
				MiniValue<T> result = Judge.Evalute(context);
				if (result.ValueType.IsLoopControl())
				{
					return result;
				}
			}
			return new MiniValue<T>(this);
		}
	}
}
