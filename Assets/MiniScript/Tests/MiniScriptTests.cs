using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.TestTools;
using MiniScript;


namespace MiniScript.Tests
{
	using MiniValue = MiniValue<float>;
	public class Decoder : MiniDecoder<float> {}
	public class Context : Context<float> {}

	public class MiniScriptTests
	{
		Decoder _decoder;

		/// <summary>
		/// Test開始時の初期化
		/// </summary>
		[SetUp]
		public void Setup()
		{
			// Operatorを登録
			Decoder.Setup();
			// 使い回し用Decoder
			_decoder = new Decoder();
		}

		[Test]
		public void TestSimple()
		{
			// Decoder.Setup()実行後
			var decoder = new Decoder();
			float result = decoder.Decode("1+2").Evalute(null).FloatValue;
			Assert.AreEqual(result, 3f);
		}

		/// <summary>
		/// 変数を使用するサンプル
		/// </summary>
		[Test]
		public void TestSimple_Variable()
		{
			// Decoder.Setup()実行後
			var decoder = new Decoder();
			// 式の解析
			MiniValue miniValue = decoder.Decode("a+b");

			var context = new Context();
			float result;
			// 変数設定
			context
				.Set("a", 1f)
				.Set("b", 2f);
			result = 1f + 2f;
			Assert.AreEqual(miniValue.Evalute(context).FloatValue, result);

			// 別の値設定
			context
				.Set("a", 2f)
				.Set("b", 3f);
			result = 2f + 3f;
			Assert.AreEqual(miniValue.Evalute(context).FloatValue, result);
		}

		[Test]
		public void TestConst()
		{
			var patterns = new(string sentence, float result)[]
			{
				("1+2-3", 1+2-3),
				("1+(2-3)", 1+(2-3)),

				("1+2*3", 1+2*3),
				("(1+2)*3", (1+2)*3),
				("1*2+3", 1*2+3),

				("1+2/3", 1+2f/3),
				("1/2+3", 1f/2+3),

				("1+2<3", MiniValue.Convert(1+2<3)),
				("1+2>3", MiniValue.Convert(1+2>3)),
				("1+2==3", MiniValue.Convert(1+2==3)),
			};
			TestPatterns(patterns);
		}

		[Test]
		public void TestVariable(
			[Values(1,2,3,5,7,11)] float a
			, [Values(1,2,3,5,7,11)] float b
			, [Values(1,2,3,5,7,11)] float c
			)
		{
			// 変数保存場所生成
			var context = new Context();
			// 変数値設定
			context[nameof(a)] = new MiniValue(a);
			context[nameof(b)] = new MiniValue(b);
			context[nameof(c)] = new MiniValue(c);
			{
				var patterns = new (string sentence, float result)[]
				{
					("a+b+c", a+b+c),
					("a-b+c", a-b+c),
					("a+b-c", a+b-c),

					("a+b*c", a+b*c),
					("a*b+c", a*b+c),

					("a+b/c", a+b/c),
					("a/b+c", a/b+c),
				};
				TestPatterns(patterns, context);
			}
		}


		public void TestPatterns(
			(string sentence, float result)[] patterns
			, Context context = null
			)
		{
			foreach (var pattern in patterns)
			{
				var miniValue = _decoder.Decode(pattern.sentence);
				float result = miniValue.Evalute(context).FloatValue;
				Assert.AreEqual(result, pattern.result
					, $"{pattern.sentence} result:{pattern.result}");
			}
		}
	}

}

