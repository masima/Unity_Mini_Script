using System;
using System.Collections.Generic;

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
		public string StringValue
		{
			get => _object?.ToString() ?? _value.ToString();
		}

		public bool ToBool()
		{
			return Calculator.ToBool(_value);
		}

		public static MiniValue<T> Default => new MiniValue<T>((T)default);


		/// <summary>
		/// binary operator等。
		/// </summary>
		private object _object;
		public bool TryGetOperator<TOperator>(out TOperator operatorInstance)
			where TOperator : IOperator
		{
			if (_object is TOperator op)
			{
				operatorInstance = op;
				return true;
			}
			operatorInstance = default;
			return false;
		}
		public TOperator GetOperator<TOperator>()
			where TOperator : class, IOperator
		{
			return _object as TOperator;
		}
		public IOperator GetOperator()
		{
			return _object as IOperator;
		}
		public TObject GetObject<TObject>()
			where TObject : class
		{
			return _object as TObject;
		}
		public object GetObject()
		{
			return _object;
		}
		public IContext<T> GetDictionary()
		{
			return _object as IContext<T>;
		}

		public MiniList<T> GetArray()
		{
			return _object as MiniList<T>;
		}


		static ICalculator s_calculator;
		public static ICalculator<T> Calculator
		{
			get => s_calculator as ICalculator<T>;
			set => s_calculator = value;
		}
		public static T Convert(bool value)
		{
			return Calculator.Convert(value);
		}

		public delegate MiniValue<T> Function(IContext<T> context, List<MiniValue<T>> values);


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
		public MiniValue(int value)
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
		public MiniValue(IOperator operatorObject)
		{
			switch (operatorObject)
			{
				case FlowControlOperatorReturn<T> operatorReturn:
					_valueType = (byte)EValueType.LoopControl;
					_value = operatorReturn.ReturnValue.Value;
					_object = operatorReturn;
					break;
				case ILoopControl:
					_valueType = (byte)EValueType.LoopControl;
					_value = default;
					break;
				default:
					_valueType = (byte)EValueType.Operator;
					_value = default;
					break;					
			}
			_object = operatorObject;
		}
		public MiniValue(MiniList<T> values)
		{
			_valueType = (byte)EValueType.Array;
			_value = Calculator.Convert(values.Count);
			_object = values;
		}
		public MiniValue(IContext<T> values)
		{
			_valueType = (byte)EValueType.Dictionary;
			_value = Calculator.Convert(values.Count);
			_object = values;
		}
		public MiniValue(Function function)
		{
			_valueType = (byte)EValueType.Function;
			_value = default;
			_object = function;
		}

		public MiniValue(string value)
		{
			_valueType = (byte)EValueType.String;
			_value = default;
			_object = value;
		}

		private string AssignmentKey => _object as string;

		public MiniValue<T> Evalute(IContext<T> context)
		{
			MiniValue<T> result = EvaluteInner(context);
			if (result.TryGetOperator(out FlowControlOperatorReturn<T> returnOperator))
			{
				return returnOperator.ReturnValue;
			}
			return result;
		}
		internal MiniValue<T> EvaluteInner(IContext<T> context)
		{
			switch ((EValueType)_valueType)
			{
			case EValueType.Unknown:
			case EValueType.Integer:
			case EValueType.Float:
			case EValueType.Function:
				return this;
			case EValueType.Variable:
			{
				var key = AssignmentKey;
				if (context.TryGetValue(key, out MiniValue<T> value))
				{
					return value;
				}
				throw new System.Exception($"Undefined variable : {key}");
			}
			default:
				switch (_object)
				{
					case IOperator<T> op:
						return op.Evalute(context);
					default:
						throw new System.Exception($"not support type : {((EValueType)_valueType).ToString()}");
				}
			}
		}

		public void AssignmentTo(IContext<T> context, MiniValue<T> value)
		{
			switch (ValueType)
			{
				case EValueType.Variable:
					context[AssignmentKey] = value;
					break;
				default:
					if (TryGetOperator(out BinaryOperatorDictionaryAccessor<T> accessor))
					{
						accessor.AssignmentTo(context, value);
						return;
					}
					throw new InvalidOperationException();
			}
		}

		public MiniValue<T> ConvertToVariable()
		{
			_valueType = (byte)EValueType.Variable;
			return this;
		}
	}

}
