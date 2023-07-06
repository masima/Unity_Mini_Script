using System.Diagnostics;
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

	public partial class MiniScriptTests
	{
		/// <summary>
		/// 辞書型変数参照
		/// </summary>
		/// <param name="index"></param>
		[Test]
		public void TestDictionary()
		{
			// 変数保存場所生成
			var context = new Context();
			{
				var patterns = new (string sentence, float result)[]
				{
					("a.a=1;a.b=2;a.a+a.b", 3),
					("a.a=1;a.b=2;a.a<a.b", Convert(1<2)),

					("a.a=1;a.b=2;a.a<a.a+a.b", Convert(1<1+2)),
					("a.a=1;a.b=2;a.a>a.a+a.b", Convert(1>1+2)),
					("a.a=1;a.b=2;a.a+a.b<a.a", Convert(1+2<1)),
					("a.a=1;a.b=2;a.a+a.b>a.a", Convert(1+2>1)),

					("a.a=1;a.b=2;a.a<a.a*a.b", Convert(1<1*2)),
					("a.a=1;a.b=2;a.a>a.a*a.b", Convert(1>1*2)),
					("a.a=1;a.b=2;a.a*a.b<a.a", Convert(1*2<1)),
					("a.a=1;a.b=2;a.a*a.b>a.a", Convert(1*2>1)),
				};
				TestPatterns(patterns, context);
			}
		}

		/// <summary>
		/// 辞書型変数参照
		/// </summary>
		/// <param name="index"></param>
		[Test]
		public void TestDictionary_Initialize(
			[Values(0, 1, 2)] float a
			, [Values(0, 1, 2)] float b
			)
		{
			// 変数保存場所生成
			var context = new Context();
			context
				.Set(nameof(a), a)
				.Set(nameof(b), b)
				;
			{
				var patterns = new (string sentence, float result)[]
				{
					// 空辞書を代入することで実行時のGC Allocを軽減する
					("d=[];d.a=a;d.b=b;d.a+d.b", a+b),
					// 初期値を指定する
					("d=[a:a,b:b]; d.a+d.b", a+b),
					("d=[a:a+b,b:a*b]; d.a+d.b", a+b+a*b),
				};
				TestPatterns(patterns, context);
			}
		}

		[Test]
		public void TestDictionary_GetByPath()
		{
			var context = new Context();
			MiniValue miniValue = _decoder.Decode("a.b.c=123");
			miniValue.Evalute(context);
			Assert.AreEqual(context.GetByPath("a.b.c").Value, 123f);
		}

	}
}
