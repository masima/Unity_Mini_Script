﻿using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
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
		/// Test開始前１度だけ実行
		/// </summary>
		[OneTimeSetUp]
		public void OneTimeSetUp()
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
		public void TestOperator()
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

				("3%2", 3f%2f),
			};
			TestPatterns(patterns);
		}

		/// <summary>
		/// 比較演算を使用する
		/// </summary>
		[Test]
		public void TestOperator_Compare()
		{
			var patterns = new(string sentence, float result)[]
			{
				("1<2", MiniValue.Convert(1<2)),
				("1<1", MiniValue.Convert(1<1)),
				("2<1", MiniValue.Convert(2<1)),

				("1>2", MiniValue.Convert(1>2)),
				("1>1", MiniValue.Convert(1>1)),
				("2>1", MiniValue.Convert(2>1)),

				("1<=2", MiniValue.Convert(1<=2)),
				("1<=1", MiniValue.Convert(1<=1)),
				("2<=1", MiniValue.Convert(2<=1)),

				("1>=2", MiniValue.Convert(1>=2)),
				("1>=1", MiniValue.Convert(1>=1)),
				("2>=1", MiniValue.Convert(2>=1)),

				("1==2", MiniValue.Convert(1==2)),
				("1==1", MiniValue.Convert(1==1)),
				("2==1", MiniValue.Convert(2==1)),

				("1!=2", MiniValue.Convert(1!=2)),
				("1!=1", MiniValue.Convert(1!=1)),
				("2!=1", MiniValue.Convert(2!=1)),

				("1+2<3", MiniValue.Convert(1+2<3)),
				("1+2>3", MiniValue.Convert(1+2>3)),
				("1+2==3", MiniValue.Convert(1+2==3)),

				("1<2+3", MiniValue.Convert(1<2+3)),
				("1>2+3", MiniValue.Convert(1>2+3)),
				("1==2+3", MiniValue.Convert(1==2+3)),
				("5==2+3", MiniValue.Convert(5==2+3)),
			};
			TestPatterns(patterns);
		}

		/// <summary>
		/// 変数で演算する
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
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
			context
				.Set(nameof(a), a)
				.Set(nameof(b), b)
				.Set(nameof(c), c)
				;
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

		/// <summary>
		/// 論理式で判定する
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		[Test]
		public void TestVariable_Logical(
			[Values(1,2,3,5,7,11)] float a
			, [Values(1,2,3,5,7,11)] float b
			, [Values(1,2,3,5,7,11)] float c
			)
		{
			// 変数保存場所生成
			var context = new Context();
			// 変数値設定
			context
				.Set(nameof(a), a)
				.Set(nameof(b), b)
				.Set(nameof(c), c)
				;
			{
				var patterns = new (string sentence, float result)[]
				{
					("a+b==c || a==b+c", Convert(a+b==c || a==b+c)),
					("a*b==c || a==b*c", Convert(a*b==c || a==b*c)),

					("a==b || b==c", Convert(a==b || b==c)),
					("a==b || b==c && a==c", Convert(a==b || b==c && a==c)),
					("a==b && b==c || a==c", Convert(a==b && b==c || a==c)),
				};
				TestPatterns(patterns, context);
			}
		}

		/// <summary>
		/// 変数に値を代入する
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		[Test]
		public void TestVariable_Assignment(
			[Values(1,2,3,5,7,11)] float a
			, [Values(1,2,3,5,7,11)] float b
			, [Values(1,2,3,5,7,11)] float c
			)
		{
			// 変数保存場所生成
			var context = new Context();
			// 変数値設定
			float d;
			context
				.Set(nameof(a), a)
				.Set(nameof(b), b)
				.Set(nameof(c), c)
				;
			{
				var patterns = new (string sentence, float result)[]
				{
					("(d=a+b*c)+d", (d=a+b*c)+d),
				};
				TestPatterns(patterns, context);
			}

		}


		[Test]
		public void TestCustomFunction()
		{
			var context = new Context();

			// 関数登録
			context["sum"] = new MiniValue<float>(Sum);
			{
				var patterns = new (string sentence, float result)[]
				{
					("sum(1,2,3)", new float[] {1,2,3}.Sum()),
				};
				TestPatterns(patterns, context);
			}
		}
		/// <summary>
		/// 合計値を求める
		/// </summary>
		/// <param name="context"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		private static MiniValue Sum(IContext<float> context, List<MiniValue> parameters)
		{
			float sum = 0;
			foreach	(var value in parameters)
			{
				sum += value.FloatValue;
			}
			return new MiniValue<float>(sum);
		}


		public float Convert(bool value)
		{
			return MiniValue.Convert(value);
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
