

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestInterpolateAttributes
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
		public static void test1()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(0, 1.0 / 3.0);
			poly.lineTo(0, 2.0 / 3.0);
			poly.lineTo(0, 4.0 / 3.0);
			poly.lineTo(0, System.Math.sqrt(6.0));
			poly.lineTo(0, System.Math.sqrt(7.0));
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 0, 3);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, 5);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 2, 0, 7);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 5, 0, 11);
			poly.interpolateAttributes(com.esri.core.geometry.VertexDescription.Semantics.M, 
				0, 1, 0, 1);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0) == 3);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0) == 5);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0) == 7);
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.NumberUtils.isNaN(poly.getAttributeAsDbl
				(com.esri.core.geometry.VertexDescription.Semantics.M, 3, 0)));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.NumberUtils.isNaN(poly.getAttributeAsDbl
				(com.esri.core.geometry.VertexDescription.Semantics.M, 4, 0)));
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 5, 0) == 11);
			poly.interpolateAttributes(com.esri.core.geometry.VertexDescription.Semantics.M, 
				0, 1, 0, 2);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0) == 3);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0) == 5);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0) == 7);
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.NumberUtils.isNaN(poly.getAttributeAsDbl
				(com.esri.core.geometry.VertexDescription.Semantics.M, 3, 0)));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.NumberUtils.isNaN(poly.getAttributeAsDbl
				(com.esri.core.geometry.VertexDescription.Semantics.M, 4, 0)));
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 5, 0) == 11);
			poly.interpolateAttributes(com.esri.core.geometry.VertexDescription.Semantics.M, 
				0, 2, 0, 5);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0) == 3);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0) == 5);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0) == 7);
			double a3 = poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 3, 0);
			NUnit.Framework.Assert.IsTrue(a3 > 7 && a3 < 11);
			double a4 = poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 4, 0);
			NUnit.Framework.Assert.IsTrue(a4 > a3 && a4 < 11);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 5, 0) == 11);
			poly.startPath(0, System.Math.sqrt(8.0));
			poly.lineTo(0, System.Math.sqrt(10.0));
			poly.lineTo(0, System.Math.sqrt(11.0));
		}

		[NUnit.Framework.Test]
		public static void test2()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(0, 1.0 / 3.0);
			poly.startPath(0, System.Math.sqrt(8.0));
			poly.lineTo(0, System.Math.sqrt(10.0));
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, System.Math
				.sqrt(3.0));
			poly.interpolateAttributes(com.esri.core.geometry.VertexDescription.Semantics.M, 
				0, 1, 1, 0);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0) == System.Math.sqrt(3.0));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.NumberUtils.isNaN(poly.getAttributeAsDbl
				(com.esri.core.geometry.VertexDescription.Semantics.M, 2, 0)));
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 3, 0, System.Math
				.sqrt(5.0));
			poly.interpolateAttributes(com.esri.core.geometry.VertexDescription.Semantics.M, 
				0, 1, 1, 1);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0) == System.Math.sqrt(3.0));
			double a2 = poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0);
			NUnit.Framework.Assert.IsTrue(a2 == System.Math.sqrt(3.0));
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 3, 0) == System.Math.sqrt(5.0));
		}

		[NUnit.Framework.Test]
		public static void test3()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, System.Math.sqrt(0.0));
			poly.lineTo(0, System.Math.sqrt(5.0));
			poly.startPath(0, System.Math.sqrt(8.0));
			poly.lineTo(0, System.Math.sqrt(10.0));
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 0, System.Math
				.sqrt(3.0));
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 2, 0, System.Math
				.sqrt(5.0));
			poly.interpolateAttributes(com.esri.core.geometry.VertexDescription.Semantics.M, 
				0, 0, 1, 0);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0) == System.Math.sqrt(3.0));
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0) == System.Math.sqrt(5.0));
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0) == System.Math.sqrt(5.0));
		}

		[NUnit.Framework.Test]
		public static void test4()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, System.Math.sqrt(0.0));
			poly.lineTo(0, System.Math.sqrt(1.0));
			poly.startPath(0, System.Math.sqrt(1.0));
			poly.lineTo(0, System.Math.sqrt(2.0));
			poly.startPath(0, System.Math.sqrt(2.0));
			poly.lineTo(0, System.Math.sqrt(3.0));
			poly.startPath(0, System.Math.sqrt(3.0));
			poly.lineTo(0, System.Math.sqrt(4.0));
			poly.startPath(0, System.Math.sqrt(4.0));
			poly.lineTo(0, System.Math.sqrt(5.0));
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, System.Math
				.sqrt(1.0));
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 8, 0, System.Math
				.sqrt(4.0));
			poly.interpolateAttributes(com.esri.core.geometry.VertexDescription.Semantics.M, 
				0, 1, 4, 0);
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.NumberUtils.isNaN(poly.getAttributeAsDbl
				(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 0)));
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0) == System.Math.sqrt(1.0));
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0) == System.Math.sqrt(1.0));
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 3, 0) == System.Math.sqrt(2.0));
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 4, 0) == System.Math.sqrt(2.0));
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 5, 0) == System.Math.sqrt(3.0));
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 6, 0) == System.Math.sqrt(3.0));
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 7, 0) == System.Math.sqrt(4.0));
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 8, 0) == System.Math.sqrt(4.0));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.NumberUtils.isNaN(poly.getAttributeAsDbl
				(com.esri.core.geometry.VertexDescription.Semantics.M, 9, 0)));
		}

		[NUnit.Framework.Test]
		public static void test5()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 1);
			poly.lineTo(1, 1);
			poly.lineTo(1, 0);
			poly.startPath(2, 0);
			poly.lineTo(2, 1);
			poly.lineTo(3, 1);
			poly.lineTo(3, 0);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, 1);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 3, 0, 3);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 6, 0, 1);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 5, 0, 4);
			poly.interpolateAttributes(com.esri.core.geometry.VertexDescription.Semantics.M, 
				0, 3, 1);
			poly.interpolateAttributes(com.esri.core.geometry.VertexDescription.Semantics.M, 
				0, 1, 3);
			poly.interpolateAttributes(com.esri.core.geometry.VertexDescription.Semantics.M, 
				1, 2, 1);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0) == 2);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0) == 1);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0) == 2);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 3, 0) == 3);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 4, 0) == 3);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 5, 0) == 4);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 6, 0) == 1);
			NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 7, 0) == 2);
		}
	}
}
