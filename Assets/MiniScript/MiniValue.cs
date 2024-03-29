﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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


		/// <summary>
		/// 使用するCalculatorをセットする
		/// </summary>
		/// <param name="calculator"></param>
		public static void Setup(ICalculator<T> calculator)
		{
			s_calculator = calculator;
		}
		/// <summary>
		/// Assembly内の適応するCalculatorをセットする
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static bool Setup(Assembly assembly)
		{
			foreach (Type type in assembly.GetTypes())
			{
				foreach (Type interfaseType in type.GetInterfaces())
				{
					if (!interfaseType.IsGenericType)
					{
						continue;
					}
					if (interfaseType.GetGenericTypeDefinition() == typeof(ICalculator<>))
					{
						Type genericArgumentType = interfaseType.GetGenericArguments()[0];
						if (genericArgumentType == typeof(T))
						{
							s_calculator = Activator.CreateInstance(type) as ICalculator<T>;
							return true;
						}

					}
				}
			}

			return false;
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
			_valueType = (byte)EValueType.Const;
			_value = value;
			_object = null;
		}
		public MiniValue(bool value)
		{
			_valueType = (byte)EValueType.Const;
			_value = Calculator.Convert(value);
			_object = null;
		}
		public MiniValue(int value)
		{
			_valueType = (byte)EValueType.Const;
			_value = Calculator.Convert(value);
			_object = null;
		}
		public MiniValue(float value)
		{
			_valueType = (byte)EValueType.Const;
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
			case EValueType.Const:
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

		internal void AssignmentTo(IContext<T> context, MiniValue<T> value)
		{
			switch (ValueType)
			{
				case EValueType.Variable:
					context[AssignmentKey] = value;
					break;
				default:
					if (TryGetOperator(out IAssignmentTo<T> accessor))
					{
						accessor.AssignmentTo(context, value);
						return;
					}
					throw new InvalidOperationException();
			}
		}

		internal MiniValue<T> ConvertToVariable()
		{
			_valueType = (byte)EValueType.Variable;
			return this;
		}
	}

}
