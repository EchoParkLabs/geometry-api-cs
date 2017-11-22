using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestInterpolateAttributes : NUnit.Framework.TestFixtureAttribute
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
		public static void Test1()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 0);
			poly.LineTo(0, 1.0 / 3.0);
			poly.LineTo(0, 2.0 / 3.0);
			poly.LineTo(0, 4.0 / 3.0);
			poly.LineTo(0, System.Math.Sqrt(6.0));
			poly.LineTo(0, System.Math.Sqrt(7.0));
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 0, 3);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, 5);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 2, 0, 7);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 5, 0, 11);
			poly.InterpolateAttributes(com.epl.geometry.VertexDescription.Semantics.M, 0, 1, 0, 1);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0) == 3);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == 5);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0) == 7);
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.NumberUtils.IsNaN(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 3, 0)));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.NumberUtils.IsNaN(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 4, 0)));
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 5, 0) == 11);
			poly.InterpolateAttributes(com.epl.geometry.VertexDescription.Semantics.M, 0, 1, 0, 2);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0) == 3);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == 5);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0) == 7);
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.NumberUtils.IsNaN(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 3, 0)));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.NumberUtils.IsNaN(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 4, 0)));
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 5, 0) == 11);
			poly.InterpolateAttributes(com.epl.geometry.VertexDescription.Semantics.M, 0, 2, 0, 5);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0) == 3);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == 5);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0) == 7);
			double a3 = poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 3, 0);
			NUnit.Framework.Assert.IsTrue(a3 > 7 && a3 < 11);
			double a4 = poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 4, 0);
			NUnit.Framework.Assert.IsTrue(a4 > a3 && a4 < 11);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 5, 0) == 11);
			poly.StartPath(0, System.Math.Sqrt(8.0));
			poly.LineTo(0, System.Math.Sqrt(10.0));
			poly.LineTo(0, System.Math.Sqrt(11.0));
		}

		[NUnit.Framework.Test]
		public static void Test2()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 0);
			poly.LineTo(0, 1.0 / 3.0);
			poly.StartPath(0, System.Math.Sqrt(8.0));
			poly.LineTo(0, System.Math.Sqrt(10.0));
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, System.Math.Sqrt(3.0));
			poly.InterpolateAttributes(com.epl.geometry.VertexDescription.Semantics.M, 0, 1, 1, 0);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == System.Math.Sqrt(3.0));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.NumberUtils.IsNaN(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0)));
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 3, 0, System.Math.Sqrt(5.0));
			poly.InterpolateAttributes(com.epl.geometry.VertexDescription.Semantics.M, 0, 1, 1, 1);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == System.Math.Sqrt(3.0));
			double a2 = poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0);
			NUnit.Framework.Assert.IsTrue(a2 == System.Math.Sqrt(3.0));
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 3, 0) == System.Math.Sqrt(5.0));
		}

		[NUnit.Framework.Test]
		public static void Test3()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, System.Math.Sqrt(0.0));
			poly.LineTo(0, System.Math.Sqrt(5.0));
			poly.StartPath(0, System.Math.Sqrt(8.0));
			poly.LineTo(0, System.Math.Sqrt(10.0));
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 0, System.Math.Sqrt(3.0));
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 2, 0, System.Math.Sqrt(5.0));
			poly.InterpolateAttributes(com.epl.geometry.VertexDescription.Semantics.M, 0, 0, 1, 0);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0) == System.Math.Sqrt(3.0));
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == System.Math.Sqrt(5.0));
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0) == System.Math.Sqrt(5.0));
		}

		[NUnit.Framework.Test]
		public static void Test4()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, System.Math.Sqrt(0.0));
			poly.LineTo(0, System.Math.Sqrt(1.0));
			poly.StartPath(0, System.Math.Sqrt(1.0));
			poly.LineTo(0, System.Math.Sqrt(2.0));
			poly.StartPath(0, System.Math.Sqrt(2.0));
			poly.LineTo(0, System.Math.Sqrt(3.0));
			poly.StartPath(0, System.Math.Sqrt(3.0));
			poly.LineTo(0, System.Math.Sqrt(4.0));
			poly.StartPath(0, System.Math.Sqrt(4.0));
			poly.LineTo(0, System.Math.Sqrt(5.0));
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, System.Math.Sqrt(1.0));
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 8, 0, System.Math.Sqrt(4.0));
			poly.InterpolateAttributes(com.epl.geometry.VertexDescription.Semantics.M, 0, 1, 4, 0);
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.NumberUtils.IsNaN(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0)));
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == System.Math.Sqrt(1.0));
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0) == System.Math.Sqrt(1.0));
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 3, 0) == System.Math.Sqrt(2.0));
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 4, 0) == System.Math.Sqrt(2.0));
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 5, 0) == System.Math.Sqrt(3.0));
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 6, 0) == System.Math.Sqrt(3.0));
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 7, 0) == System.Math.Sqrt(4.0));
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 8, 0) == System.Math.Sqrt(4.0));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.NumberUtils.IsNaN(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 9, 0)));
		}

		[NUnit.Framework.Test]
		public static void Test5()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 1);
			poly.LineTo(1, 1);
			poly.LineTo(1, 0);
			poly.StartPath(2, 0);
			poly.LineTo(2, 1);
			poly.LineTo(3, 1);
			poly.LineTo(3, 0);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, 1);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 3, 0, 3);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 6, 0, 1);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 5, 0, 4);
			poly.InterpolateAttributes(com.epl.geometry.VertexDescription.Semantics.M, 0, 3, 1);
			poly.InterpolateAttributes(com.epl.geometry.VertexDescription.Semantics.M, 0, 1, 3);
			poly.InterpolateAttributes(com.epl.geometry.VertexDescription.Semantics.M, 1, 2, 1);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0) == 2);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == 1);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0) == 2);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 3, 0) == 3);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 4, 0) == 3);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 5, 0) == 4);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 6, 0) == 1);
			NUnit.Framework.Assert.IsTrue(poly.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 7, 0) == 2);
		}
	}
}
