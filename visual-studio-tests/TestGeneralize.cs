using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestGeneralize : NUnit.Framework.TestFixtureAttribute
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
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorGeneralize op = (com.epl.geometry.OperatorGeneralize)engine.GetOperator(com.epl.geometry.Operator.Type.Generalize);
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(1, 1);
			poly.LineTo(2, 0);
			poly.LineTo(3, 2);
			poly.LineTo(4, 1);
			poly.LineTo(5, 0);
			poly.LineTo(5, 10);
			poly.LineTo(0, 10);
			com.epl.geometry.Geometry geom = op.Execute(poly, 2, true, null);
			com.epl.geometry.Polygon p = (com.epl.geometry.Polygon)geom;
			com.epl.geometry.Point2D[] points = p.GetCoordinates2D();
			NUnit.Framework.Assert.IsTrue(points.Length == 4);
			NUnit.Framework.Assert.IsTrue(points[0].x == 0 && points[0].y == 0);
			NUnit.Framework.Assert.IsTrue(points[1].x == 5 && points[1].y == 0);
			NUnit.Framework.Assert.IsTrue(points[2].x == 5 && points[2].y == 10);
			NUnit.Framework.Assert.IsTrue(points[3].x == 0 && points[3].y == 10);
			com.epl.geometry.Geometry geom1 = op.Execute(geom, 5, false, null);
			p = (com.epl.geometry.Polygon)geom1;
			points = p.GetCoordinates2D();
			NUnit.Framework.Assert.IsTrue(points.Length == 3);
			NUnit.Framework.Assert.IsTrue(points[0].x == 0 && points[0].y == 0);
			NUnit.Framework.Assert.IsTrue(points[1].x == 5 && points[1].y == 10);
			NUnit.Framework.Assert.IsTrue(points[2].x == 5 && points[2].y == 10);
			geom1 = op.Execute(geom, 5, true, null);
			p = (com.epl.geometry.Polygon)geom1;
			points = p.GetCoordinates2D();
			NUnit.Framework.Assert.IsTrue(points.Length == 0);
		}

		[NUnit.Framework.Test]
		public static void Test2()
		{
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorGeneralize op = (com.epl.geometry.OperatorGeneralize)engine.GetOperator(com.epl.geometry.Operator.Type.Generalize);
			com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
			polyline.StartPath(0, 0);
			polyline.LineTo(1, 1);
			polyline.LineTo(2, 0);
			polyline.LineTo(3, 2);
			polyline.LineTo(4, 1);
			polyline.LineTo(5, 0);
			polyline.LineTo(5, 10);
			polyline.LineTo(0, 10);
			com.epl.geometry.Geometry geom = op.Execute(polyline, 2, true, null);
			com.epl.geometry.Polyline p = (com.epl.geometry.Polyline)geom;
			com.epl.geometry.Point2D[] points = p.GetCoordinates2D();
			NUnit.Framework.Assert.IsTrue(points.Length == 4);
			NUnit.Framework.Assert.IsTrue(points[0].x == 0 && points[0].y == 0);
			NUnit.Framework.Assert.IsTrue(points[1].x == 5 && points[1].y == 0);
			NUnit.Framework.Assert.IsTrue(points[2].x == 5 && points[2].y == 10);
			NUnit.Framework.Assert.IsTrue(points[3].x == 0 && points[3].y == 10);
			com.epl.geometry.Geometry geom1 = op.Execute(geom, 5, false, null);
			p = (com.epl.geometry.Polyline)geom1;
			points = p.GetCoordinates2D();
			NUnit.Framework.Assert.IsTrue(points.Length == 2);
			NUnit.Framework.Assert.IsTrue(points[0].x == 0 && points[0].y == 0);
			NUnit.Framework.Assert.IsTrue(points[1].x == 0 && points[1].y == 10);
			geom1 = op.Execute(geom, 5, true, null);
			p = (com.epl.geometry.Polyline)geom1;
			points = p.GetCoordinates2D();
			NUnit.Framework.Assert.IsTrue(points.Length == 2);
			NUnit.Framework.Assert.IsTrue(points[0].x == 0 && points[0].y == 0);
			NUnit.Framework.Assert.IsTrue(points[1].x == 0 && points[1].y == 10);
		}
	}
}
