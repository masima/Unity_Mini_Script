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
		/// delegateテスト
		/// </summary>
		/// <param name="index"></param>
		[Test]
		public void TestFunction_Delegate(
			[Values(0, 1, 2)] float a
			,[Values(0, 1, 2)] float b
			)
		{
			// 変数保存場所生成
			var context = new Context();
			context.Set(nameof(a), a);
			context.Set(nameof(b), b);
			var patterns = new (string sentence, float result)[]
			{
				// delegateの定義と実行
				("addValue=(x,y)=>{x+y};addValue(a,b)", a+b),
				// ローカル変数テスト
				("x=1;y=2;addValue=(x,y)=>{x+y};addValue(a,b);x+y", 1+2),
			};
			TestPatterns(patterns, context);
		}
	}
}
