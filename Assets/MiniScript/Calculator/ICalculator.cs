using System;


namespace MiniScript
{
	public interface ICalculator
	{

	}
	public interface ICalculator<T> : ICalculator
		where T : struct, IFormattable, IEquatable<T>
	{
		float ToSingle(T value);

		T Convert(bool value);
		T Convert(float value);
		T Convert(string value);
		T Convert(ReadOnlySpan<char> value);

		T Add(T left, T right);
		T Substruct(T left, T right);
		T Multiple(T left, T right);
		T Divide(T left, T right);
		T Mod(T left, T right);
	}


}