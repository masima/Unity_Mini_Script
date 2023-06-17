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

	}
}
