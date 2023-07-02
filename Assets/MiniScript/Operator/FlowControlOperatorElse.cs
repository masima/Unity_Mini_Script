using System;
using System.Collections.Generic;


namespace MiniScript
{
	public class FlowControlOperatorElse<T>
		: FlowControlOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.FlowControl;
		public override string OperatorCode => "else";


		public override MiniValue<T> SplitSentence(
			MiniDecoder<T> decoder
			, string sentence
			, ref int startat
			)
		{
			if (!Judge.ValueType.IsValid())
			{
				throw new InvalidOperationException();
			}

			MiniValue<T> flowControlValue = new(this);
			if (decoder.TryGetStatement(sentence, ref startat, "{}", out Statement))
			{
				return flowControlValue;
			}

			if (!decoder.TryGetFlowControlOperator(sentence, ref startat, out OperatorInfo operatorInfo))
			{
				throw new InvalidOperationException();
			}

			// elseの後のif
			startat += operatorInfo.OperatorCode.Length;

			var subOperator = Activator.CreateInstance(operatorInfo.Type) as FlowControlOperator<T>;
			Statement = subOperator.SplitSentence(decoder, sentence, ref startat);
			return flowControlValue;
		}

		public override MiniValue<T> Evalute(IContext<T> context)
		{
			MiniValue<T> result = Judge.EvaluteInner(context);
			if (result.ValueType.IsLoopControl())
			{
				return result;
			}

			if (!result.ToBool())
			{
				Statement.EvaluteInner(context);
			}
			return result;
		}
	}
}
