using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniScript
{
	public interface IOperator
	{
		//BinaryOperatorType BinaryOperatorType { get; }
		//string OperatorCode { get; }
		int Priority { get; }
		bool IsFinalized { get; }
	}

	public interface IOperator<T>
		: IOperator
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		MiniValue<T> Evalute(IContext<T> context);
	}


}
