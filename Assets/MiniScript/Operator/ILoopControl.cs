using System.Collections;
using System.Collections.Generic;


namespace MiniScript
{
	public interface ILoopControl : IOperator
	{
		public FlowControlFlag FlowControlFlag { get; }
	}

}
