

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestMathUtils
	{
		/// <exception cref="System.Exception"/>
		protected override void setUp()
		{
			base.setUp();
		}

		/// <exception cref="System.Exception"/>
		protected override void tearDown()
		{
			base.tearDown();
		}

		[NUnit.Framework.Test]
		public static void testKahanSummation()
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
			NUnit.Framework.Assert.IsTrue(System.Math.abs(s - trueAnswer) > 1e-9);
			// precision loss
			com.esri.core.geometry.MathUtils.KahanSummator sum = new com.esri.core.geometry.MathUtils.KahanSummator
				(0);
			for (int i_1 = 0; i_1 < 10000; i_1++)
			{
				if (i_1 == 0)
				{
					sum.add(1e6);
				}
				else
				{
					sum.add(1e-7);
				}
			}
			double kahanResult = sum.getResult();
			// 1000000.0009999000 //C++
			// 1000000.0009999 //Java
			NUnit.Framework.Assert.IsTrue(kahanResult == trueAnswer);
		}
		// nice answer!
	}
}
