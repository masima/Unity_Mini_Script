namespace MiniScript
{
    public enum BinaryOperatorType
    {
        Plus,
        Minus,

        Mul =  PriorityUnit * 1,
        Div,
		Mod,

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
