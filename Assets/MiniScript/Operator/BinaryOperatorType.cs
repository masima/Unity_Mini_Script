namespace MiniScript
{
	public enum BinaryOperatorType
	{
		Mul =  PriorityUnit * 0,
		Div,
		Mod,

		Plus =  PriorityUnit * 1,
		Minus,

		GreaterEqual = PriorityUnit * 2,
		Lower,

		Equal =  PriorityUnit * 3,
		NotEqual,
		
		
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
