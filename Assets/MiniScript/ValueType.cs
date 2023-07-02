using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MiniScript
{
	[Flags]
	public enum EValueType
	{
		Unknown = 0,
		Const,
		
		String,

		/// <summary>
		/// 変数
		/// </summary>
		Variable,
		Array,
		Dictionary,
		Function,
		LoopControl,



		Operator = 0x80,
		PrimaryOpeartor = Operator,
		UnrayOpeartor,
		BinaryOperator,
	}

	public static class EValueTypeExtensions
	{
		public static bool IsValid(this EValueType valueType)
		{
			return valueType != EValueType.Unknown;
		}
		public static bool IsOperator(this EValueType valueType)
		{
			return (valueType & EValueType.Operator) != 0;
		}
		public static bool IsString(this EValueType valueType)
		{
			return valueType == EValueType.String;
		}
		public static bool IsArray(this EValueType valueType)
		{
			return valueType == EValueType.Array;
		}
		public static bool IsLoopControl(this EValueType valueType)
		{
			return valueType == EValueType.LoopControl;
		}
	}
}
