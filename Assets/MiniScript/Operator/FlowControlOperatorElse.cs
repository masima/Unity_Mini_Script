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
			if (!decoder.TryGetStatement(sentence, ref startat, "{}", out Statement))
			{
				throw new InvalidOperationException();
			}
			return new MiniValue<T>(this);
		}

		public override MiniValue<T> Evalute(IContext<T> context)
		{
			MiniValue<T> result = Judge.Evalute(context);

			if (!result.ToBool())
			{
				Statement.Evalute(context);
			}
			return result;
		}
	}
}
