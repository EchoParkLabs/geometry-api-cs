/*
Copyright 2017-2021 David Raleigh

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

For additional information, contact:

email: davidraleigh@gmail.com
*/
using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestMathUtils : NUnit.Framework.TestFixtureAttribute
	{
		/// <exception cref="System.Exception"/>
		[SetUp]
        protected void SetUp()
		{
			
		}

		/// <exception cref="System.Exception"/>
		protected void TearDown()
		{
			
		}

		[NUnit.Framework.Test]
		public static void TestKahanSummation()
		{
			double s = 0.0;
			for (int i = 0; i < 10000; i++)
			{
				if (i == 0)
				{
					s += 1e6;
				}
				else
				{
					s += 1e-7;
				}
			}
			double trueAnswer = 1e6 + 9999 * 1e-7;
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(s - trueAnswer) > 1e-9);
			// precision loss
			com.epl.geometry.MathUtils.KahanSummator sum = new com.epl.geometry.MathUtils.KahanSummator(0);
			for (int i_1 = 0; i_1 < 10000; i_1++)
			{
				if (i_1 == 0)
				{
					sum.Add(1e6);
				}
				else
				{
					sum.Add(1e-7);
				}
			}
			double kahanResult = sum.GetResult();
			// 1000000.0009999000 //C++
			// 1000000.0009999 //Java
			NUnit.Framework.Assert.IsTrue(kahanResult == trueAnswer);
		}
		// nice answer!
	}
}
