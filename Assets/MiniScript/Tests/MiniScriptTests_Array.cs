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

		[Test]
		public void TestArray_Edit()
		{
			// 変数保存場所生成
			var context = new Context();
			{
				var patterns = new (string sentence, float[] result)[]
				{
					("a=();a.add(1);a.add(2,3)", new float[] {1,2,3}),

					("a=(1,2,3);a.add(4)", new float[] {1,2,3,4}),
					("a=(1,2,3);a.clear()", new float[] {}),

					("a=(1,2,3);a.insert(0,4)", new float[] {4,1,2,3}),
					("a=(1,2,3);a.insert(1,4)", new float[] {1,4,2,3}),
					("a=(1,1+1,3);a.insert(1,2+2)", new float[] {1,2+2,1+1,3}),
					("a=1;b=2;c=3;array=(a,a+a,c);array.insert(a,b+b)", new float[] {1,2+2,1+1,3}),

					("a=(1,2,3);a.removeat(1)", new float[] {1,3}),
					("a=(1,2,3);(a.pop(),a.pop(),a.pop())", new float[] {3,2,1}),
					("a=(1,2,3);(a.pop()+1,a.pop()+2,a.pop()*3)", new float[] {3+1,2+2,1*3}),
				};

				TestPatterns(patterns, context);
			}
		}
	}
}
