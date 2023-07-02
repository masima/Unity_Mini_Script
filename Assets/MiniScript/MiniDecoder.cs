using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace MiniScript
{
	using System.Text.RegularExpressions;

	public struct OperatorInfo
	{
		internal Type Type;
		internal string OperatorCode;
	}

	public class MiniDecoder<T>
		where T : struct, IComparable, IFormattable, IConvertible, IEquatable<T>
		, IComparable<T>
	{
		/// <summary>
		/// 演算子リスト
		/// </summary>
		// private static readonly List<OperatorInfo> s_unrayOperators = new();
		private static readonly List<OperatorInfo> s_flowControlOperators = new();
		private static readonly List<OperatorInfo> s_binaryOperators = new();
		public static void Setup(Assembly assembly = null)
		{
			s_flowControlOperators.Clear();
			s_binaryOperators.Clear();

			Assembly miniDecoderAssembly = typeof(MiniDecoder<T>).Assembly;
			RegisterOperators(miniDecoderAssembly);
			if (assembly is not null && miniDecoderAssembly != assembly)
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
					// && _.BaseType.GetGenericTypeDefinition() == typeof(IOperator<>)
					)
				.ToArray();

			foreach (Type type in binaryOperatorTypes)
			{
				List<OperatorInfo> infos;
				var genericTypeDefinition = type.BaseType.GetGenericTypeDefinition();
				if (genericTypeDefinition == typeof(BinaryOperator<>))
				{
					infos = s_binaryOperators;
				}
				else if (genericTypeDefinition == typeof(FlowControlOperator<>))
				{
					infos = s_flowControlOperators;
				}
				else
				{
					continue;
				}
				var genericType = type.MakeGenericType(typeof(T));
				var propertyInfo = genericType.GetProperty(nameof(IOperator<T>.OperatorCode));
				if (null == propertyInfo)
				{
					continue;
				}
				var defaultValue = Activator.CreateInstance(genericType);
				string code = (string)propertyInfo.GetValue(defaultValue);
				infos.Add(new OperatorInfo
				{
					Type = genericType,
					OperatorCode = code,
				});
			}
			// 文字数の多い順に並べる
			s_flowControlOperators.Sort((left, right) => right.OperatorCode.Length - left.OperatorCode.Length);
			s_binaryOperators.Sort((left, right) => right.OperatorCode.Length - left.OperatorCode.Length);
		}


		public MiniValue<T> Decode(string sentence, int startat = 0)
		{
			return DecodeInner(sentence, ref startat, InvalidEndCode);
		}
		internal MiniValue<T> DecodeInner(string sentence, ref int startat, char endCode)
		{
			List<MiniValue<T>> elements = SplitSentence(sentence, ref startat, endCode);
			ConvertToRpn(elements);
			return FinalizeRpn();
		}

		
		static readonly Regex s_regexConstValue = new Regex(@"-?\d+(\.\d*)?");
		static readonly Regex s_regexWord = new Regex(@"\w+");
		const string CommentCode = "//";
		

		readonly List<MiniValue<T>> _elements = new();
		readonly List<MiniValue<T>> _rpn = new();
		MiniDecoder<T> _childDecoder = null;

		const char InvalidEndCode = (char)0;

		internal List<MiniValue<T>> SplitSentence(
			string sentence
			, ref int startat
			, char endCode = InvalidEndCode)
		{
			var list = _elements;
			list.Clear();

			bool isLastIsValue = false;
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
					return list;
				}
				if (TrimComment(sentence, ref startat))
				{
					continue;
				}

				switch (c)
				{
					case '(':
					{
						++startat;
						var childValue = DecodeChild(sentence, ref startat, ')');
						if (isLastIsValue)
						{
							// 関数呼び出し
							list.Add(new MiniValue<T>(
								new BinaryOperatorFunction<T>()
								));
							if (childValue.TryGetOperator<BinaryOperatorArraySeparater<T>>(out var _))
							{
								list.Add(childValue);
							}
							else
							{
								// 引数が１つでも配列として渡す
								var arraySeparaterValue = BinaryOperatorArraySeparater<T>.InstantiateSingleParameter(childValue);
								list.Add(arraySeparaterValue);
							}
						}
						else
						{
							if (childValue.ValueType.IsValid())
							{
								list.Add(childValue);
							}
							else
							{
								// 空配列
								var arraySeparaterValue = BinaryOperatorArraySeparater<T>.InstantiateSingleParameter(childValue);
								list.Add(arraySeparaterValue);
							}
						}
						isLastIsValue = true;
						break;
					}
					case '{':
					{
						++startat;
						var childValue = DecodeChild(sentence, ref startat, '}');
						list.Add(childValue);
						isLastIsValue = true;
						break;
					}
					default:
						if (isLastIsValue)
						{
							// 値の後
							if (c == '[')
							{
								// 配列参照
								++startat;
								var childValue = DecodeChild(sentence, ref startat, ']');
								list.Add(new MiniValue<T>(
									new BinaryOperatorArrayAccessor<T>()
									));
								list.Add(childValue);
							}
							else if (TryGetBinaryOperator(sentence, startat, out OperatorInfo operatorInfo))
							{
								list.Add(new MiniValue<T>(
									Activator.CreateInstance(operatorInfo.Type)
									as BinaryOperator<T>));
								startat += operatorInfo.OperatorCode.Length;
								isLastIsValue = false;
							}
							else
							{
								if (c == '}')
								{
									return list;
								}
								else
								{
									throw new Exception($"inavlid format:{GetErrorPositionMessage(sentence, startat)}");
								}
							}
						}
						else
						{
							if (TryGetFlowControlOperator(sentence, ref startat, out OperatorInfo flowControlOperatorInfo))
							{
								// 制御
								var flowControlOperator = Activator.CreateInstance(
									flowControlOperatorInfo.Type)
									as FlowControlOperator<T>;
								startat += flowControlOperatorInfo.OperatorCode.Length;
								list.Add(flowControlOperator.SplitSentence(
									this
									, sentence
									, ref startat
									));
								// 文を分割しておく
								list.Add(new MiniValue<T>(new BinaryOperatorSentenceSeparater<T>()));
							}
							else 
							if (TryGetConstValue(sentence, startat, out string value))
							{
								// 値
								list.Add(MiniValue<T>.GetConstValue(value));
								startat += value.Length;
								isLastIsValue = true;
							}
							else if (TryGetBinaryOperator(sentence, startat, out OperatorInfo operatorInfo))
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
								list.Add(new MiniValue<T>(word).ConvertToVariable());
								startat += word.Length;
								isLastIsValue = true;
							}
							else
							{
								throw new Exception($"inavlid format:{GetErrorPositionMessage(sentence, startat)}");
							}
						}
						break;
				}
			}

			if (endCode != InvalidEndCode
				&& endCode != BinaryOperatorSentenceSeparater<T>.OperatorCodeConst
				)
			{
				throw new Exception($"not found '{endCode.ToString()}' {GetErrorPositionMessage(sentence, startat)}");
			}

			return list;
		}

		public delegate MiniValue<T> DecodeChildCb(string sentence, ref int startat, char endCode);
		internal MiniValue<T> DecodeChild(string sentence, ref int startat, char endCode)
		{
			if (_childDecoder is null)
			{
				_childDecoder = new MiniDecoder<T>();
			}
			var childValue = _childDecoder.DecodeInner(sentence, ref startat , endCode);
			return childValue;
		}
		public bool TryGetStatement(
			string sentence
			, ref int startat
			, string beginEnd
			, out MiniValue<T> value)
		{
			while (startat < sentence.Length)
			{
				char c = sentence[startat];
				if (IsSpace(c))
				{
					++startat;
					continue;
				}
				if (c == beginEnd[0])
				{
					++startat;
					value = DecodeChild(sentence, ref startat, beginEnd[1]);
					return true;
				}
				break;
			}
			value = default;
			return false;
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
				+ $" : {sentence.Substring(startClipPosition, startClipLength)}"
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
			case '\n':
			case '\r':
				return true;
			default:
				return false;
			}
		}
		static bool IsReturnCode(char c)
		{
			switch (c)
			{
				case '\r':
				case '\n':
					return true;
				default:
					return false;
			}
		}
		static bool TrimStart(string sentence, ref int startat)
		{
			while (startat < sentence.Length)
			{
				if (!IsSpace(sentence[startat]))
				{
					return true;
				}
				++startat;
			}
			return false;
		}
		static bool TrimComment(string sentence, ref int startat)
		{
			if (sentence.Length < startat + CommentCode.Length)
			{
				return false;
			}
			if (sentence[startat..(startat + CommentCode.Length)] == CommentCode)
			{
				startat += CommentCode.Length;
				// コメント部分スキップ
				do 
				{
					if (sentence.Length <= startat)
					{
						return true;
					}
				} while (!IsReturnCode(sentence[startat++]));
				// 改行削除
				while (IsReturnCode(sentence[startat++]));
				return true;
			}
			return false;
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


		public bool TryGetFlowControlOperator(string sentence, ref int startat, out OperatorInfo value)
		{
			if (!TrimStart(sentence, ref startat))
			{
				value = default;
				return false;
			}
			foreach (OperatorInfo op in s_flowControlOperators)
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

		public bool TryGetBinaryOperator(string sentence, int startat, out OperatorInfo value)
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

 		void ConvertToRpn(List<MiniValue<T>> elements)
		{
			_rpn.Clear();
			var enumerator = elements.GetEnumerator();
			while(enumerator.MoveNext())
			{
				var value = enumerator.Current;
				if (value.ValueType.IsOperator()
					&& !value.GetOperator().IsFinalized)
				{
					if (value.TryGetOperator(out BinaryOperator<T> binaryOperator))
					{
						enumerator = binaryOperator.ConvertToRpn(enumerator, _rpn, out int insertIndex);
					}
					else
					{
						throw new InvalidOperationException();
					}
				}
				else
				{
					_rpn.Add(value);
				}
			}
		}
		public static int GetInsertPosition(
			IReadOnlyList<MiniValue<T>> rpn
			, int operatorPriority
			)
		{
			for (int i = rpn.Count - 1; 0 <= i; --i)
			{
				var value = rpn[i];
				if (!value.ValueType.IsOperator()
					|| value.GetOperator().IsFinalized)
				{
					return i + 1;
				}
				IOperator binaryOperator = value.GetOperator();
				if (binaryOperator.Priority <= operatorPriority)
				{
					return i + 1;
				}
			}
			throw new FormatException("left value nothing");
		}

		readonly Stack<MiniValue<T>> _rpnStack = new();
		readonly List<IOperatorOnFinalized> _onFinalizedList = new();
		/// <summary>
		/// 逆ポーランド状態から最終結果作成
		/// </summary>
		MiniValue<T> FinalizeRpn()
		{
			if (0 == _rpn.Count)
			{
				// 値なし
				return new MiniValue<T>();
			}
			_rpnStack.Clear();
			_onFinalizedList.Clear();
			foreach (MiniValue<T> value in _rpn)
			{
				MiniValue<T> pushValue;
				if (value.ValueType.IsOperator()
					&& !value.GetOperator().IsFinalized)
				{
					if (value.TryGetOperator(out BinaryOperator<T> op))
					{
						pushValue = op.Finailze(_rpnStack);
						if (op is IOperatorOnFinalized opOnFinalized)
						{
							if (opOnFinalized.IsOnFinishedRequired)
							{
								_onFinalizedList.Add(op as IOperatorOnFinalized);
							}
						}
					}
					else
					{
						throw new InvalidOperationException();
					}
				}
				else
				{
					pushValue = value;
				}
				_rpnStack.Push(pushValue);
			}

			// 最終処理
			foreach	(var op in _onFinalizedList)
			{
				op.OnFinalized();
			}
			_onFinalizedList.Clear();
			return _rpnStack.Pop();
		}	
	}

}
