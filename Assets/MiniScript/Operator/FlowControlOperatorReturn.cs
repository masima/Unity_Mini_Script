using System.Diagnostics;
using System;
using System.Collections.Generic;


namespace MiniScript
{
	public class FlowControlOperatorReturn<T>
		: FlowControlOperator<T>, ILoopControl
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.FlowControl;
		public override string OperatorCode => "return";
		public FlowControlFlag FlowControlFlag => FlowControlFlag.Break | FlowControlFlag.Return;

		public MiniValue<T> ReturnValue { get; private set; }

		public override MiniValue<T> SplitSentence(
			MiniDecoder<T> decoder
			, string sentence
			, ref int startat
			)
		{
			MiniValue<T> flowControlValue = new(this);
			// Judge = MiniValue<T>.Default;
			var list = decoder.SplitSentence(
				sentence
				, ref startat
				, endCode: BinaryOperatorSentenceSeparater<T>.OperatorCodeConst
				);
			Statement = list[0];
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

			ReturnValue = Statement.Evalute(context);
			return new MiniValue<T>(this);
		}
	}
}
