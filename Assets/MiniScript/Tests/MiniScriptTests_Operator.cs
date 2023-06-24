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
				// ("1+2-3", 1+2-3),
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

	}
}
