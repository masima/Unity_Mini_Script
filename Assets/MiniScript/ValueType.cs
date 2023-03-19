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
		Float,
		Integer,
		
		String,

		/// <summary>
		/// 変数
		/// </summary>
		Variable,
		Array,



		Operator = 0x80,
		UnrayOpeartor = Operator,
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
	}
}
