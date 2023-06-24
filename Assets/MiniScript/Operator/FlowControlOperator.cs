using System;
using System.Collections.Generic;

namespace MiniScript
{
	public abstract class FlowControlOperator<T>
		: IOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public abstract OperatorType OperatorType { get; }
		public int Priority => OperatorType.GetPriority();
		public abstract string OperatorCode { get; }

		public MiniValue<T> Judge;
		public MiniValue<T> Statement;

		public bool IsFinalized
		{
			get
			{
				return Judge.ValueType.IsValid() && Statement.ValueType.IsValid();
			}
		}

		public virtual MiniValue<T> SplitSentence(
			MiniDecoder<T> decoder
			, string sentence
			, ref int startat
			)
		{
			if (!decoder.TryGetStatement(sentence, ref startat, "()", out Judge))
			{
				throw new InvalidOperationException();
			}
			if (!decoder.TryGetStatement(sentence, ref startat, "{}", out Statement))
			{
				throw new InvalidOperationException();
			}

			MiniValue<T> flowControlValue = new(this);
			if (!decoder.TryGetFlowControlOperator(sentence, startat, out OperatorInfo operatorInfo))
			{
				return flowControlValue;
			}
			if (operatorInfo.Type.BaseType.GetGenericTypeDefinition() != typeof(FlowControlOperator<>))
			{
				return flowControlValue;
			}
			// else
			startat += operatorInfo.OperatorCode.Length;

			var subOperator = Activator.CreateInstance(operatorInfo.Type) as FlowControlOperator<T>;
			subOperator.Judge = flowControlValue;
			return subOperator.SplitSentence(decoder, sentence, ref startat);
		}

		public virtual MiniValue<T> Finailze(Stack<MiniValue<T>> rpnStack)
		{
			Judge = rpnStack.Pop();
			Statement = rpnStack.Pop();
			return new MiniValue<T>(this);
		}


		public abstract MiniValue<T> Evalute(IContext<T> context);
	}

}
