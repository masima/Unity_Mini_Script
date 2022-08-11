using System;


namespace MiniScript
{
	public struct CalculatorInteger : ICalculator<int>
	{
		public float ToSingle(int value)
		{
			return value;
		}

		public int Convert(bool value)
		{
			return value ? 1 : 0;
		}
		public int Convert(float value)
		{
			return (int)value;
		}

		public int Add(int left, int right)
		{
			return left + right;
		}
		public int Substruct(int left, int right)
		{
			return left - right;
		}
		public int Multiple(int left, int right)
		{
			return left * right;
		}
		public int Divide(int left, int right)
		{
			return left / right;
		}
	}
}
