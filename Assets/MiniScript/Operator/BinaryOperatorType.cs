namespace MiniScript
{
	public enum BinaryOperatorType
	{
		Mul =  PriorityUnit * 0,
		Div,
		Mod,

		Plus =  PriorityUnit * 1,
		Minus,

		GreaterThan =  PriorityUnit * 2,
		GreaterThanOrEqualTo,
		LessThan,
		LessThanOrEqualTo,

		Equal =  PriorityUnit * 3,
		NotEqual,
		
		LogicalConjunction = PriorityUnit * 4,
		LogicalDisjunction = PriorityUnit * 5,


		Assignment = PriorityUnit * 10,

		Separator = PriorityUnit * 11,


		PriorityUnit = 0x100,
		PriorityShift = 8,
	}
	public static class BinaryOperatorTypeExtensions
	{
		public static int GetPriority(this BinaryOperatorType type)
		{
			return (int)type >> (int)BinaryOperatorType.PriorityShift;
		}
	}
}
