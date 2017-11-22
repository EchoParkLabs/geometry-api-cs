

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestGeneralize
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
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorGeneralize op = (com.esri.core.geometry.OperatorGeneralize
				)engine.getOperator(com.esri.core.geometry.Operator.Type.Generalize);
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(1, 1);
			poly.lineTo(2, 0);
			poly.lineTo(3, 2);
			poly.lineTo(4, 1);
			poly.lineTo(5, 0);
			poly.lineTo(5, 10);
			poly.lineTo(0, 10);
			com.esri.core.geometry.Geometry geom = op.execute(poly, 2, true, null);
			com.esri.core.geometry.Polygon p = (com.esri.core.geometry.Polygon)geom;
			com.esri.core.geometry.Point2D[] points = p.getCoordinates2D();
			NUnit.Framework.Assert.IsTrue(points.Length == 4);
			NUnit.Framework.Assert.IsTrue(points[0].x == 0 && points[0].y == 0);
			NUnit.Framework.Assert.IsTrue(points[1].x == 5 && points[1].y == 0);
			NUnit.Framework.Assert.IsTrue(points[2].x == 5 && points[2].y == 10);
			NUnit.Framework.Assert.IsTrue(points[3].x == 0 && points[3].y == 10);
			com.esri.core.geometry.Geometry geom1 = op.execute(geom, 5, false, null);
			p = (com.esri.core.geometry.Polygon)geom1;
			points = p.getCoordinates2D();
			NUnit.Framework.Assert.IsTrue(points.Length == 3);
			NUnit.Framework.Assert.IsTrue(points[0].x == 0 && points[0].y == 0);
			NUnit.Framework.Assert.IsTrue(points[1].x == 5 && points[1].y == 10);
			NUnit.Framework.Assert.IsTrue(points[2].x == 5 && points[2].y == 10);
			geom1 = op.execute(geom, 5, true, null);
			p = (com.esri.core.geometry.Polygon)geom1;
			points = p.getCoordinates2D();
			NUnit.Framework.Assert.IsTrue(points.Length == 0);
		}

		[NUnit.Framework.Test]
		public static void test2()
		{
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorGeneralize op = (com.esri.core.geometry.OperatorGeneralize
				)engine.getOperator(com.esri.core.geometry.Operator.Type.Generalize);
			com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
			polyline.startPath(0, 0);
			polyline.lineTo(1, 1);
			polyline.lineTo(2, 0);
			polyline.lineTo(3, 2);
			polyline.lineTo(4, 1);
			polyline.lineTo(5, 0);
			polyline.lineTo(5, 10);
			polyline.lineTo(0, 10);
			com.esri.core.geometry.Geometry geom = op.execute(polyline, 2, true, null);
			com.esri.core.geometry.Polyline p = (com.esri.core.geometry.Polyline)geom;
			com.esri.core.geometry.Point2D[] points = p.getCoordinates2D();
			NUnit.Framework.Assert.IsTrue(points.Length == 4);
			NUnit.Framework.Assert.IsTrue(points[0].x == 0 && points[0].y == 0);
			NUnit.Framework.Assert.IsTrue(points[1].x == 5 && points[1].y == 0);
			NUnit.Framework.Assert.IsTrue(points[2].x == 5 && points[2].y == 10);
			NUnit.Framework.Assert.IsTrue(points[3].x == 0 && points[3].y == 10);
			com.esri.core.geometry.Geometry geom1 = op.execute(geom, 5, false, null);
			p = (com.esri.core.geometry.Polyline)geom1;
			points = p.getCoordinates2D();
			NUnit.Framework.Assert.IsTrue(points.Length == 2);
			NUnit.Framework.Assert.IsTrue(points[0].x == 0 && points[0].y == 0);
			NUnit.Framework.Assert.IsTrue(points[1].x == 0 && points[1].y == 10);
			geom1 = op.execute(geom, 5, true, null);
			p = (com.esri.core.geometry.Polyline)geom1;
			points = p.getCoordinates2D();
			NUnit.Framework.Assert.IsTrue(points.Length == 2);
			NUnit.Framework.Assert.IsTrue(points[0].x == 0 && points[0].y == 0);
			NUnit.Framework.Assert.IsTrue(points[1].x == 0 && points[1].y == 10);
		}
	}
}
