using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace MiniScript
{
	using System.Text.RegularExpressions;
	public class MiniDecoder<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		private struct OperatorInfo
		{
			internal Type Type;
			internal string OperatorCode;
		}

		/// <summary>
		/// 演算子リスト
		/// </summary>
		private static readonly List<OperatorInfo> s_unrayOperators = new();
		private static readonly List<OperatorInfo> s_binaryOperators = new();
		public static void Setup(Assembly assembly)
		{
			s_binaryOperators.Clear();

			Assembly miniDecoderAssembly = typeof(MiniDecoder<T>).Assembly;
			RegisterOperators(miniDecoderAssembly);
			if (miniDecoderAssembly != assembly)
			{
				RegisterOperators(assembly);
			}

		}
		private static void RegisterOperators(Assembly assembly)
		{
			Type[] binaryOperatorTypes = assembly.GetTypes()
				.Where(_ =>
					_.BaseType != null
					&& !_.IsAbstract 
					&& _.BaseType.IsGenericType 
					&& _.BaseType.GetGenericTypeDefinition() == typeof(BinaryOperator<>)
					)
				.ToArray();

			foreach (Type type in binaryOperatorTypes)
			{
				var genericType = type.MakeGenericType(typeof(T));
				var propertyInfo = genericType.GetProperty(nameof(BinaryOperator<T>.OperatorCode));
				if (null == propertyInfo)
				{
					continue;
				}
				var defaultValue = Activator.CreateInstance(genericType);
				string code = (string)propertyInfo.GetValue(defaultValue);
				s_binaryOperators.Add(new OperatorInfo
				{
					Type = genericType,
					OperatorCode = code,
				});
			}
			// 文字数の多い順に並べる
			s_binaryOperators.Sort((left, right) => String.CompareOrdinal(right.OperatorCode, left.OperatorCode));
		}


		public MiniValue<T> Decode(string sentence, int startat = 0)
		{
			return DecodeInner(sentence, ref startat, InvalidEndCode);
		}
		private MiniValue<T> DecodeInner(string sentence, ref int startat, char endCode)
		{
			List<MiniValue<T>> elements = SplitSentence(sentence, ref startat, endCode);
			ConvertToRpn(elements);
			return FinalizeRpn();
		}

		
		static readonly Regex s_regexConstValue = new Regex(@"-?\d+(\.\d*)?");
		static readonly Regex s_regexWord = new Regex(@"\w+");
		

		readonly List<MiniValue<T>> _elements = new();
		readonly List<MiniValue<T>> _rpn = new();

		const char InvalidEndCode = (char)0;

		private List<MiniValue<T>> SplitSentence(
			string sentence
			, ref int startat
			, char endCode = InvalidEndCode)
		{
			var list = _elements;
			list.Clear();

			bool isLastConst = false;
			while (startat < sentence.Length)
			{
				char c = sentence[startat];
				if (IsSpace(c))
				{
					++startat;
					continue;
				}

				if (c == endCode){
					++startat;
					break;
				}
				switch (c)
				{
					case '(':
					{
						++startat;
						var childDecorder = new MiniDecoder<T>();
						var childValue = childDecorder.DecodeInner(sentence, ref startat , ')');
						list.Add(childValue);
						break;
					}
					default:
						if (isLastConst)
						{
							// 値の後
							if (TryGetOperator(sentence, startat, out OperatorInfo operatorInfo))
							{
								list.Add(new MiniValue<T>(
									Activator.CreateInstance(operatorInfo.Type)
									as BinaryOperator<T>));
								startat += operatorInfo.OperatorCode.Length;
								isLastConst = false;
							}
							else
							{
								throw new Exception($"inavlid format:{GetErrorPositionMessage(sentence, startat)}");
							}
						}
						else
						{
							// 値または単項演算子
							if (TryGetConstValue(sentence, startat, out string value))
							{
								list.Add(MiniValue<T>.GetConstValue(value));
								startat += value.Length;
								isLastConst = true;
							}
							else if (TryGetOperator(sentence, startat, out OperatorInfo operatorInfo))
							{
								// ２項演算子
								list.Add(new MiniValue<T>(
									Activator.CreateInstance(operatorInfo.Type)
									as BinaryOperator<T>));
								startat += operatorInfo.OperatorCode.Length;
							}
							else if (TryGetWord(sentence, startat, out string word))
							{
								// 変数
								list.Add(new MiniValue<T>(word));
								startat += word.Length;
							}
							else
							{
							}
						}
						break;
				}
			}

			return list;
		}
		static string GetErrorPositionMessage(string sentence, int startat)
		{
			int clipLength = 8;
			int startClipPosition = startat - clipLength;
			int startClipLength = clipLength;
			if (startClipPosition < 0)
			{
				startClipLength += startClipPosition;
				startClipPosition = 0;
			}
			int endClipLength = clipLength;
			if (sentence.Length < startat + clipLength)
			{
				endClipLength = sentence.Length - startat;
			}

			return $"({startat.ToString()})"
				+ $" {sentence.Substring(startClipPosition, startClipLength)}"
				+ "^^^"
				+ $" {sentence.Substring(startat, endClipLength)}";
		}
		static bool IsSpace(char c)
		{
			switch (c)
			{
			case ' ':
			case '\t':
			case '　':
				return true;
			default:
				return false;
			}
		}
		static bool TryGetConstValue(string sentence, int startat, out string value)
		{
				Match match = s_regexConstValue.Match(sentence, startat);
				if (match.Success)
				{
					Group group = match.Groups[0];
					if (startat == group.Index)
					{
						value = sentence.Substring(startat, group.Length);
						return true;
					}
				}
				value = default;
				return false;
		}
		static bool TryGetWord(string sentence, int startat, out string value)
		{
				Match match = s_regexWord.Match(sentence, startat);
				if (match.Success)
				{
					Group group = match.Groups[0];
					if (startat == group.Index)
					{
						value = sentence.Substring(startat, group.Length);
						return true;
					}
				}
				value = default;
				return false;
		}


		bool TryGetOperator(string sentence, int startat, out OperatorInfo value)
		{
			foreach (OperatorInfo op in s_binaryOperators)
			{
				if (0 == string.CompareOrdinal(
						sentence, startat
						, op.OperatorCode, 0, op.OperatorCode.Length))
				{
					value = op;
					return true;
				}
			}

			value = default;
			return false;
		}

		private BinaryOperator<T> CreateBinaryOperator(string code)
		{
			foreach (var info in s_binaryOperators)
			{
				if (info.OperatorCode == code)
				{
					return (BinaryOperator<T>)Activator.CreateInstance(info.Type);
				}
			}

			throw new Exception($"no supprot operator : {code}");
		}
 
 
 		MiniValue<T> ConvertToRpn(List<MiniValue<T>> elements)
		{
			_rpn.Clear();
			var enumerator = elements.GetEnumerator();
			while(enumerator.MoveNext())
			{
				var value = enumerator.Current;
				if (value.ValueType.IsOperator()
					&& !value.GetBinaryOperator().IsFinalized)
				{
					IBinaryOperator binaryOperator = value.GetBinaryOperator();
					// Get right value
					if (!enumerator.MoveNext())
					{
						throw new FormatException("right value nothing.");
					}
					int insertIndex = GetInsertPosition(binaryOperator.Priority);
					_rpn.Insert(insertIndex, enumerator.Current);
					_rpn.Insert(insertIndex + 1, value);
				}
				else
				{
					_rpn.Add(value);
				}
			}
			return _rpn.Last();
		}
		int GetInsertPosition(int operatorPriority)
		{
			for (int i = _rpn.Count - 1; 0 <= i; --i)
			{
				var value = _rpn[i];
				if (!value.ValueType.IsOperator()
					|| value.GetBinaryOperator().IsFinalized)
				{
					return i + 1;
				}
				IBinaryOperator binaryOperator = value.GetBinaryOperator();
				if (operatorPriority <= binaryOperator.Priority)
				{
					return i + 1;
				}
			}
			throw new FormatException("left value nothing");
		}

		readonly Stack<MiniValue<T>> _rpnStack = new();
		/// <summary>
		/// 逆ポーランド状態から最終結果作成
		/// </summary>
		MiniValue<T> FinalizeRpn()
		{
			_rpnStack.Clear();
			foreach (var value in _rpn)
			{
				if (value.ValueType.IsOperator()
					&& !value.GetBinaryOperator().IsFinalized)
				{
					var op = value.GetBinaryOperator<BinaryOperator<T>>();
					op.Right = _rpnStack.Pop();
					op.Left = _rpnStack.Pop();
				}
				else if (value.ValueType == EValueType.String)
				{
					value.ConvertToVariable();
				}
				_rpnStack.Push(value);
			}
			return _rpnStack.Pop();
		}	
	}

}
