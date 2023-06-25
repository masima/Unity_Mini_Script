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
				("1<2", Convert(1<2)),
				("1<1", Convert(1<1)),
				("2<1", Convert(2<1)),

				("1>2", Convert(1>2)),
				("1>1", Convert(1>1)),
				("2>1", Convert(2>1)),

				("1<=2", Convert(1<=2)),
				("1<=1", Convert(1<=1)),
				("2<=1", Convert(2<=1)),

				("1>=2", Convert(1>=2)),
				("1>=1", Convert(1>=1)),
				("2>=1", Convert(2>=1)),

				("1==2", Convert(1==2)),
				("1==1", Convert(1==1)),
				("2==1", Convert(2==1)),

				("1!=2", Convert(1!=2)),
				("1!=1", Convert(1!=1)),
				("2!=1", Convert(2!=1)),

				("1+2<3", Convert(1+2<3)),
				("1+2>3", Convert(1+2>3)),
				("1+2==3", Convert(1+2==3)),

				("1<2+3", Convert(1<2+3)),
				("1>2+3", Convert(1>2+3)),
				("1==2+3", Convert(1==2+3)),
				("5==2+3", Convert(5==2+3)),
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

	}
}
