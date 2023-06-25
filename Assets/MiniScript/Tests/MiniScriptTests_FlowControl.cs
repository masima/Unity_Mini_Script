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
		public void TestFlowControl_if(
			[Values(0, 1, 2)] float a
			)
		{
			// 変数保存場所生成
			var context = new Context();
			context.Set(nameof(a), a);
			var patterns = new (string sentence, float result)[]
			{
				("b=1;if(a==1){b=2};b", (a==1) ? 2:1),
				("b=1;if(a==1){b=2}b", (a==1) ? 2:1),
				("b=1;if(a!=1){b=2};b", (a!=1) ? 2:1),
				("b=1;if(a==1){b=(b=2)*2};b", (a==1) ? 4:1),
				("b=1;if(a==1){b=2}else{b=3};b", (a==1) ? 2:3),
				("b=1;if(a!=1){b=2}else{b=3};b", (a!=1) ? 2:3),

				// 複数行確認(if,else)
				(@"
					b = 1;
					if (a != 1)
					{
						b = 2
					}
					else
					{
						b = 3
					};
					b", (a!=1) ? 2:3),

				// 明示的な文分割の省略確認(if,else)
				(@"
					b = 1;
					if (a != 1)
					{
						b = 2
					}
					else
					{
						b = 3
					}
					b", (a!=1) ? 2:3),

				// 複数行確認(if,else if)
				(@"
					b = 1;
					if (a == 1)
					{
						b = 2
					}
					else if (a == 2)
					{
						b = 3
					}
					b", (a==1) ? 2:(a==2) ? 3:1),

				// 複数行確認(if,else if,else)
				(@"
					b = 1;
					if (a == 1)
					{
						b = 2
					}
					else if (a == 2)
					{
						b = 3
					}
					else
					{
						b = 4
					}
					b", (a==1) ? 2:(a==2) ? 3:4),

				// 空文対応
				(@"
					b = 1;
					if (a == 1)
					{
						b = 2;
					}
					else if (a == 2)
					{
						b = 3;
					}
					b", (a==1) ? 2:(a==2) ? 3:1),

			};
			TestPatterns(patterns, context);
		}
	}
}
