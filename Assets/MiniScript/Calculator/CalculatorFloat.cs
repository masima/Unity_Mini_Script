using System;


namespace MiniScript
{
	public struct CalculatorFloat : ICalculator<float>
	{
		public float ToSingle(float value)
		{
			return value;
		}

		public float Convert(bool value)
		{
			return value ? 1.0f : 0.0f;
		}
		public float Convert(float value)
		{
			return value;
		}
		public float Convert(string value)
		{
			return float.Parse(value);
		}

		public float Convert(ReadOnlySpan<char> value)
		{
			return float.Parse(value);
		}



		public float Add(float left, float right)
		{
			return left + right;
		}
		public float Substruct(float left, float right)
		{
			return left - right;
		}
		public float Multiple(float left, float right)
		{
			return left * right;
		}
		public float Divide(float left, float right)
		{
			return left / right;
		}
		public float Mod(float left, float right)
		{
			return left % right;
		}
	}
}
