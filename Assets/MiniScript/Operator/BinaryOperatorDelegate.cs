using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniScript
{
	public class BinaryOperatorDelegate<T> 
		: BinaryOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
 		, IComparable<T>
	{
		public override OperatorType OperatorType => OperatorType.Delegate;
		public override string OperatorCode => "=>";


		public override MiniValue<T> Finailze(Stack<MiniValue<T>> rpnStack)
		{
			DelegateInfo delegateInfo = new();
			delegateInfo.Statement = rpnStack.Pop();
			MiniValue<T> paramValue = rpnStack.Pop();
			if (paramValue.TryGetOperator(out BinaryOperatorArraySeparater<T> arraySeparater))
			{
				delegateInfo.Parameters = arraySeparater.Values
					.Select(_ => new Parameter { Name = _.StringValue, })
					.ToArray();
			}
			else if (!paramValue.ValueType.IsValid())
			{
				delegateInfo.Parameters = new Parameter[0];
			}
			else
			{
				delegateInfo.Parameters = new Parameter[]
				{
					new Parameter { Name = paramValue.StringValue },
				};
			}
			return new MiniValue<T>(delegateInfo.Func);
		}

		private struct Parameter
		{
			public string Name;
			public MiniValue<T> Value;
		}
		private class DelegateInfo
		{
			public Parameter[] Parameters;

			public MiniValue<T> Statement;

			public MiniValue<T> Func(IContext<T> context, List<MiniValue<T>> parameters)
			{
				SetupValues(context, parameters);
				MiniValue<T> result = Statement.EvaluteInner(context);
				RestoreValues(context);
				return result;
			}

			private void SetupValues(IContext<T> context, List<MiniValue<T>> parameters)
			{
				for	(int i = 0; i < Parameters.Length; i++)
				{
					string name = Parameters[i].Name;
					if (context.TryGetValue(name, out MiniValue<T> value))
					{
						Parameters[i].Value = value;
					}
					else
					{
						Parameters[i].Value = default;
					}
					if (i < parameters.Count)
					{
						context[name] = parameters[i];
					}
				}
			}
			private void RestoreValues(IContext<T> context)
			{
				for	(int i = 0; i < Parameters.Length; i++)
				{
					string name = Parameters[i].Name;
					if (Parameters[i].Value.ValueType.IsValid())
					{
						context[name] = Parameters[i].Value;
					}
					else
					{
						context[name] = new MiniValue<T>();
					}
				}
			}
		}


		public override MiniValue<T> Evalute(IContext<T> context)
		{
			throw new InvalidOperationException();
		}

	}


}

