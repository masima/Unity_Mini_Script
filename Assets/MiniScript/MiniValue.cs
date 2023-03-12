using System;


namespace MiniScript
{
	public struct MiniValue<T>
		//: IMiniValue
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		private byte _valueType;
		public EValueType ValueType
		{
			get => (EValueType)_valueType;
		}

		private T _value;
		public T Value => _value;

		public float FloatValue
		{
			get => _value.ToSingle(null);
		}
		public int IntegerValue
		{
			get => _value.ToInt32(null);
		}

		/// <summary>
		/// binary operator等。
		/// </summary>
		private object _object;
		public TOperator GetBinaryOperator<TOperator>()
			where TOperator : BinaryOperator<T>
		{
			return _object as TOperator;
		}
		public IBinaryOperator GetBinaryOperator()
		{
			return _object as IBinaryOperator;
		}
		public int GetBinaryOperatorPriority()
		{
			return (_object as BinaryOperator<T>).Priority;
		}



		static ICalculator s_calculator;
		public static ICalculator<T> Calculator
		{
			get => s_calculator as ICalculator<T>;
			set => s_calculator = value;
		}
		static MiniValue()
		{
			if (typeof(T) == typeof(float))
			{
				s_calculator = new CalculatorFloat();
			}
			else if (typeof(T) == typeof(int))
			{
				s_calculator = new CalculatorInteger();
			}
		}

		public static EValueType GetValueType(Type type)
		{
			if (type == typeof(int))
			{
				return EValueType.Integer;
			}
			else if (type == typeof(float))
			{
				return EValueType.Float;
			}
			return EValueType.Unknown;
		}

		public static MiniValue<T> GetConstValue(string value)
		{
			return new MiniValue<T>(Calculator.Convert(value));
		}

		public static MiniValue<T> GetConstValue(ReadOnlySpan<char> value)
		{
			return new MiniValue<T>(Calculator.Convert(value));
		}




		public MiniValue(T value)
		{
			switch (value)
			{
			case int:
				_valueType = (byte)EValueType.Integer;
				break;
			case float:
				_valueType = (byte)EValueType.Float;
				break;
			default:
				throw new InvalidOperationException();
			}
			_value = value;
			_object = null;
		}
		public MiniValue(bool value)
		{
			_valueType = (byte)GetValueType(typeof(T));
			_value = Calculator.Convert(value);
			_object = null;
		}
		public MiniValue(float value)
		{
			_valueType = (byte)GetValueType(typeof(T));
			_value = Calculator.Convert(value);
			_object = null;
		}
		public MiniValue(BinaryOperator<T> binaryOperator)
		{
			_valueType = (byte)EValueType.BinaryOperator;
			_value = default;
			_object = binaryOperator;
		}

		public MiniValue(string value)
		{
			_valueType = (byte)EValueType.String;
			_value = default;
			_object = value;
		}


		public MiniValue<T> Evalute(Context<T> context)
		{
			switch ((EValueType)_valueType)
			{
			case EValueType.Integer:
			case EValueType.Float:
				return this;
			case EValueType.Variable:
			{
				var key = _object as string;
				if (context.TryGetValue(key, out MiniValue<T> value))
				{
					return value;
				}
				throw new System.Exception($"Undefined variable : {key}");
			}
			case EValueType.BinaryOperator:
				return (_object as BinaryOperator<T>).Evalute(context);
			default:
				throw new System.Exception($"not support type : {_valueType.ToString()}");
			}
		}

		public void ConvertToVariable()
		{
			_valueType = (byte)EValueType.Variable;
		}
	}

}
