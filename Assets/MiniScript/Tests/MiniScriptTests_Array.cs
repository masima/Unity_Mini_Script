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
		/// 配列参照
		/// </summary>
		/// <param name="index"></param>
		[Test]
		public void TestArray(
			[Values(0, 1, 2)] float index
			)
		{
			// 変数保存場所生成
			var context = new Context();
			context.Set("index", index);
			{
				var patterns = new (string sentence, float result)[]
				{
					("a=(1,2,3);a[index]", (new float[] {1, 2, 3})[(int)index]),
					("a=(index,index*2,index*3);a[index]", (new float[] {index, index * 2, index * 3})[(int)index]),
				};
				TestPatterns(patterns, context);
			}
		}

	}
}
