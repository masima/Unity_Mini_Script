using System;
using System.Collections;
using System.Collections.Generic;
using MiniScript;


namespace MiniScript
{
	public interface IAssignmentTo<T> : IOperator<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		public void AssignmentTo(IContext<T> context, MiniValue<T> value);
	}
}
