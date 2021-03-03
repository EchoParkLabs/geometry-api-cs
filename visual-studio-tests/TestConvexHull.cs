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
	public class TestConvexHull : NUnit.Framework.TestFixtureAttribute
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
		public static void TestFewPoints()
		{
			{
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				polygon.AddPath((com.epl.geometry.Point2D[])null, 0, true);
				polygon.InsertPoint(0, -1, com.epl.geometry.Point2D.Construct(5, 5));
				com.epl.geometry.Point convex_hull = (com.epl.geometry.Point)com.epl.geometry.OperatorConvexHull.Local().Execute(polygon, null);
				NUnit.Framework.Assert.IsTrue(convex_hull.GetXY().Equals(com.epl.geometry.Point2D.Construct(5, 5)));
			}
			{
				com.epl.geometry.Point2D[] pts = new com.epl.geometry.Point2D[3];
				pts[0] = com.epl.geometry.Point2D.Construct(0, 0);
				pts[1] = com.epl.geometry.Point2D.Construct(0, 0);
				pts[2] = com.epl.geometry.Point2D.Construct(0, 0);
				int[] out_pts = new int[3];
				int res = com.epl.geometry.ConvexHull.Construct(pts, 3, out_pts);
				NUnit.Framework.Assert.IsTrue(res == 1);
				NUnit.Framework.Assert.IsTrue(out_pts[0] == 0);
			}
			{
				com.epl.geometry.Point2D[] pts = new com.epl.geometry.Point2D[1];
				pts[0] = com.epl.geometry.Point2D.Construct(0, 0);
				int[] out_pts = new int[1];
				int res = com.epl.geometry.ConvexHull.Construct(pts, 1, out_pts);
				NUnit.Framework.Assert.IsTrue(res == 1);
				NUnit.Framework.Assert.IsTrue(out_pts[0] == 0);
			}
		}

		[NUnit.Framework.Test]
		public static void TestDegenerate()
		{
			com.epl.geometry.OperatorConvexHull bounding = (com.epl.geometry.OperatorConvexHull)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ConvexHull);
			com.epl.geometry.OperatorDensifyByLength densify = (com.epl.geometry.OperatorDensifyByLength)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.DensifyByLength);
			{
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				polygon.StartPath(0, 0);
				polygon.LineTo(1, 0);
				polygon.LineTo(0, 0);
				polygon.LineTo(2, 0);
				polygon.LineTo(1, 0);
				polygon.LineTo(3, 0);
				polygon.StartPath(0, 0);
				polygon.LineTo(1, 0);
				polygon.LineTo(0, 0);
				polygon.LineTo(2, 0);
				polygon.LineTo(1, 0);
				polygon.LineTo(3, 0);
				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, .5, null));
				com.epl.geometry.Polyline convex_hull = (com.epl.geometry.Polyline)(bounding.Execute(densified, null));
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				NUnit.Framework.Assert.IsTrue(convex_hull.CalculateArea2D() == 0.0);
				com.epl.geometry.Point2D p1 = convex_hull.GetXY(0);
				com.epl.geometry.Point2D p2 = convex_hull.GetXY(1);
				NUnit.Framework.Assert.IsTrue(p1.x == 0.0 && p1.y == 0.0);
				NUnit.Framework.Assert.IsTrue(p2.x == 3.0 && p2.y == 0.0);
			}
			{
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				polygon.StartPath(0, 0);
				polygon.LineTo(0, 0);
				polygon.LineTo(0, 0);
				polygon.LineTo(com.epl.geometry.NumberUtils.DoubleEps(), 0);
				polygon.LineTo(0, com.epl.geometry.NumberUtils.DoubleEps());
				polygon.LineTo(10, 0);
				polygon.LineTo(10, 0);
				polygon.LineTo(10, 5);
				polygon.LineTo(10, 10);
				polygon.LineTo(5, 10);
				polygon.LineTo(10, 0);
				polygon.LineTo(10, 10);
				polygon.LineTo(5, 10);
				polygon.LineTo(0, 10);
				polygon.LineTo(0, 0);
				polygon.StartPath(0, 0);
				polygon.LineTo(0, 0);
				polygon.LineTo(0, 0);
				polygon.LineTo(10, 0);
				polygon.LineTo(10, 0);
				polygon.LineTo(10, 5);
				polygon.LineTo(10, 10);
				polygon.LineTo(5, 10);
				polygon.LineTo(10, 0);
				polygon.LineTo(10, 10);
				polygon.LineTo(5, 10);
				polygon.LineTo(0, 10);
				polygon.LineTo(0, 0);
				polygon.StartPath(0, 0);
				polygon.LineTo(0, 0);
				polygon.LineTo(0, 0);
				polygon.LineTo(10, 0);
				polygon.LineTo(5, 0);
				polygon.LineTo(10, 0);
				polygon.LineTo(10, 5);
				polygon.LineTo(10, 0);
				polygon.LineTo(10, 10);
				polygon.LineTo(5, 10);
				polygon.LineTo(10, 10);
				polygon.LineTo(5, 10);
				polygon.LineTo(0, 10);
				polygon.LineTo(5, 10);
				polygon.LineTo(0, 0);
				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1, null));
				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(densified, null));
				double area = convex_hull.CalculateArea2D();
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				NUnit.Framework.Assert.IsTrue(area == 100.0);
				com.epl.geometry.Point2D p1 = convex_hull.GetXY(0);
				com.epl.geometry.Point2D p2 = convex_hull.GetXY(1);
				com.epl.geometry.Point2D p3 = convex_hull.GetXY(2);
				com.epl.geometry.Point2D p4 = convex_hull.GetXY(3);
				NUnit.Framework.Assert.IsTrue(p1.x == 0.0 && p1.y == 0.0);
				NUnit.Framework.Assert.IsTrue(p2.x == 0.0 && p2.y == 10.0);
				NUnit.Framework.Assert.IsTrue(p3.x == 10.0 && p3.y == 10.0);
				NUnit.Framework.Assert.IsTrue(p4.x == 10.0 && p4.y == 0.0);
			}
			{
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				polygon.StartPath(0, 0);
				polygon.LineTo(0, 10);
				polygon.LineTo(5, 10);
				polygon.LineTo(5, 5);
				polygon.LineTo(5, 8);
				polygon.LineTo(10, 10);
				polygon.LineTo(10, 0);
				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1, null));
				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(densified, null));
				double area = convex_hull.CalculateArea2D();
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				NUnit.Framework.Assert.IsTrue(area == 100.0);
				com.epl.geometry.Point2D p1 = convex_hull.GetXY(0);
				com.epl.geometry.Point2D p2 = convex_hull.GetXY(1);
				com.epl.geometry.Point2D p3 = convex_hull.GetXY(2);
				com.epl.geometry.Point2D p4 = convex_hull.GetXY(3);
				NUnit.Framework.Assert.IsTrue(p1.x == 0.0 && p1.y == 0.0);
				NUnit.Framework.Assert.IsTrue(p2.x == 0.0 && p2.y == 10.0);
				NUnit.Framework.Assert.IsTrue(p3.x == 10.0 && p3.y == 10.0);
				NUnit.Framework.Assert.IsTrue(p4.x == 10.0 && p4.y == 0.0);
			}
			{
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				polygon.StartPath(0, 0);
				polygon.LineTo(0, 0);
				polygon.LineTo(0, 0);
				polygon.LineTo(10, 0);
				polygon.LineTo(5, 0);
				polygon.LineTo(10, 0);
				polygon.LineTo(10, 5);
				polygon.LineTo(10, 0);
				polygon.LineTo(10, 10);
				polygon.LineTo(5, 10);
				polygon.LineTo(10, 10);
				polygon.LineTo(5, 10);
				polygon.LineTo(0, 10);
				polygon.LineTo(5, 10);
				polygon.LineTo(0, 0);
				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1, null));
				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(densified, null));
				double area = convex_hull.CalculateArea2D();
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				NUnit.Framework.Assert.IsTrue(area == 100.0);
				com.epl.geometry.Point2D p1 = convex_hull.GetXY(0);
				com.epl.geometry.Point2D p2 = convex_hull.GetXY(1);
				com.epl.geometry.Point2D p3 = convex_hull.GetXY(2);
				com.epl.geometry.Point2D p4 = convex_hull.GetXY(3);
				NUnit.Framework.Assert.IsTrue(p1.x == 0.0 && p1.y == 0.0);
				NUnit.Framework.Assert.IsTrue(p2.x == 0.0 && p2.y == 10.0);
				NUnit.Framework.Assert.IsTrue(p3.x == 10.0 && p3.y == 10.0);
				NUnit.Framework.Assert.IsTrue(p4.x == 10.0 && p4.y == 0.0);
			}
			{
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				polygon.StartPath(0, 0);
				polygon.LineTo(0, 0);
				polygon.LineTo(0, 10);
				polygon.LineTo(0, 0);
				polygon.LineTo(10, 10);
				polygon.LineTo(0, 0);
				polygon.LineTo(10, 0);
				polygon.LineTo(0, 0);
				polygon.StartPath(0, 10);
				polygon.LineTo(0, 10);
				polygon.LineTo(10, 10);
				polygon.LineTo(0, 10);
				polygon.LineTo(10, 0);
				polygon.LineTo(0, 10);
				polygon.LineTo(0, 0);
				polygon.LineTo(0, 10);
				polygon.StartPath(10, 10);
				polygon.LineTo(10, 10);
				polygon.LineTo(10, 0);
				polygon.LineTo(10, 10);
				polygon.LineTo(0, 0);
				polygon.LineTo(10, 10);
				polygon.LineTo(0, 10);
				polygon.LineTo(10, 10);
				polygon.StartPath(10, 0);
				polygon.LineTo(10, 0);
				polygon.LineTo(0, 0);
				polygon.LineTo(10, 0);
				polygon.LineTo(0, 10);
				polygon.LineTo(10, 0);
				polygon.LineTo(10, 10);
				polygon.LineTo(10, 0);
				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1, null));
				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(densified, null));
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				com.epl.geometry.Point2D p1 = convex_hull.GetXY(0);
				com.epl.geometry.Point2D p2 = convex_hull.GetXY(1);
				com.epl.geometry.Point2D p3 = convex_hull.GetXY(2);
				com.epl.geometry.Point2D p4 = convex_hull.GetXY(3);
				NUnit.Framework.Assert.IsTrue(p1.x == 0.0 && p1.y == 0.0);
				NUnit.Framework.Assert.IsTrue(p2.x == 0.0 && p2.y == 10.0);
				NUnit.Framework.Assert.IsTrue(p3.x == 10.0 && p3.y == 10.0);
				NUnit.Framework.Assert.IsTrue(p4.x == 10.0 && p4.y == 0.0);
			}
			{
				com.epl.geometry.MultiPoint mpoint = new com.epl.geometry.MultiPoint();
				mpoint.Add(4, 4);
				mpoint.Add(4, 4);
				mpoint.Add(4, 4);
				mpoint.Add(4, 4);
				com.epl.geometry.Point convex_hull = (com.epl.geometry.Point)bounding.Execute(mpoint, null);
				NUnit.Framework.Assert.IsTrue(convex_hull.CalculateArea2D() == 0.0);
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				NUnit.Framework.Assert.IsTrue(convex_hull.GetXY().Equals(com.epl.geometry.Point2D.Construct(4, 4)));
			}
			{
				com.epl.geometry.MultiPoint mpoint = new com.epl.geometry.MultiPoint();
				mpoint.Add(4, 4);
				com.epl.geometry.Point convex_hull = (com.epl.geometry.Point)bounding.Execute(mpoint, null);
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				NUnit.Framework.Assert.IsTrue(convex_hull.GetXY().Equals(com.epl.geometry.Point2D.Construct(4, 4)));
			}
			{
				com.epl.geometry.MultiPoint mpoint = new com.epl.geometry.MultiPoint();
				mpoint.Add(4, 4);
				mpoint.Add(4, 5);
				com.epl.geometry.Polyline convex_hull = (com.epl.geometry.Polyline)bounding.Execute(mpoint, null);
				NUnit.Framework.Assert.IsTrue(convex_hull.GetPointCount() == 2);
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				NUnit.Framework.Assert.IsTrue(convex_hull.CalculateLength2D() == 1.0);
			}
			{
				com.epl.geometry.Line line = new com.epl.geometry.Line();
				line.SetStartXY(0, 0);
				line.SetEndXY(0, 1);
				com.epl.geometry.Polyline convex_hull = (com.epl.geometry.Polyline)bounding.Execute(line, null);
				NUnit.Framework.Assert.IsTrue(convex_hull.GetPointCount() == 2);
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				NUnit.Framework.Assert.IsTrue(convex_hull.CalculateLength2D() == 1.0);
			}
			{
				com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
				polyline.StartPath(0, 0);
				polyline.LineTo(0, 1);
				com.epl.geometry.Polyline convex_hull = (com.epl.geometry.Polyline)bounding.Execute(polyline, null);
				NUnit.Framework.Assert.IsTrue(convex_hull.GetPointCount() == 2);
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				NUnit.Framework.Assert.IsTrue(polyline == convex_hull);
				NUnit.Framework.Assert.IsTrue(convex_hull.CalculateLength2D() == 1.0);
			}
			{
				com.epl.geometry.Envelope env = new com.epl.geometry.Envelope(0, 0, 10, 10);
				NUnit.Framework.Assert.IsTrue(com.epl.geometry.OperatorConvexHull.Local().IsConvex(env, null));
				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)bounding.Execute(env, null);
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				NUnit.Framework.Assert.IsTrue(convex_hull.GetPointCount() == 4);
				NUnit.Framework.Assert.IsTrue(convex_hull.GetXY(0).Equals(com.epl.geometry.Point2D.Construct(0, 0)));
				NUnit.Framework.Assert.IsTrue(convex_hull.GetXY(1).Equals(com.epl.geometry.Point2D.Construct(0, 10)));
				NUnit.Framework.Assert.IsTrue(convex_hull.GetXY(2).Equals(com.epl.geometry.Point2D.Construct(10, 10)));
				NUnit.Framework.Assert.IsTrue(convex_hull.GetXY(3).Equals(com.epl.geometry.Point2D.Construct(10, 0)));
			}
			{
				com.epl.geometry.Envelope env = new com.epl.geometry.Envelope(0, 0, 0, 10);
				NUnit.Framework.Assert.IsTrue(!com.epl.geometry.OperatorConvexHull.Local().IsConvex(env, null));
				com.epl.geometry.Polyline convex_hull = (com.epl.geometry.Polyline)bounding.Execute(env, null);
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				NUnit.Framework.Assert.IsTrue(convex_hull.GetPointCount() == 2);
				NUnit.Framework.Assert.IsTrue(convex_hull.GetXY(0).Equals(com.epl.geometry.Point2D.Construct(0, 0)));
				NUnit.Framework.Assert.IsTrue(convex_hull.GetXY(1).Equals(com.epl.geometry.Point2D.Construct(0, 10)));
			}
			{
				com.epl.geometry.Envelope env = new com.epl.geometry.Envelope(0, 0, 0, 10);
				NUnit.Framework.Assert.IsTrue(!com.epl.geometry.OperatorConvexHull.Local().IsConvex(env, null));
				com.epl.geometry.Polyline convex_hull = (com.epl.geometry.Polyline)bounding.Execute(env, null);
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				NUnit.Framework.Assert.IsTrue(convex_hull.GetPointCount() == 2);
				NUnit.Framework.Assert.IsTrue(convex_hull.GetXY(0).Equals(com.epl.geometry.Point2D.Construct(0, 0)));
				NUnit.Framework.Assert.IsTrue(convex_hull.GetXY(1).Equals(com.epl.geometry.Point2D.Construct(0, 10)));
			}
			{
				com.epl.geometry.Envelope env = new com.epl.geometry.Envelope(5, 5, 5, 5);
				NUnit.Framework.Assert.IsTrue(!com.epl.geometry.OperatorConvexHull.Local().IsConvex(env, null));
				com.epl.geometry.Point convex_hull = (com.epl.geometry.Point)bounding.Execute(env, null);
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				NUnit.Framework.Assert.IsTrue(convex_hull.GetXY().Equals(com.epl.geometry.Point2D.Construct(5, 5)));
			}
		}

		[NUnit.Framework.Test]
		public static void TestSegment()
		{
			{
				com.epl.geometry.Line line = new com.epl.geometry.Line();
				line.SetStartXY(5, 5);
				line.SetEndXY(5, 5);
				NUnit.Framework.Assert.IsTrue(!com.epl.geometry.OperatorConvexHull.Local().IsConvex(line, null));
				com.epl.geometry.Point point = (com.epl.geometry.Point)com.epl.geometry.OperatorConvexHull.Local().Execute(line, null);
				NUnit.Framework.Assert.IsTrue(point.GetXY().Equals(com.epl.geometry.Point2D.Construct(5, 5)));
			}
			{
				com.epl.geometry.Line line = new com.epl.geometry.Line();
				line.SetStartXY(5, 5);
				line.SetEndXY(5, 6);
				NUnit.Framework.Assert.IsTrue(com.epl.geometry.OperatorConvexHull.Local().IsConvex(line, null));
				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)com.epl.geometry.OperatorConvexHull.Local().Execute(line, null);
				NUnit.Framework.Assert.IsTrue(polyline.GetPointCount() == 2);
				NUnit.Framework.Assert.IsTrue(polyline.GetXY(0).Equals(com.epl.geometry.Point2D.Construct(5, 5)));
				NUnit.Framework.Assert.IsTrue(polyline.GetXY(1).Equals(com.epl.geometry.Point2D.Construct(5, 6)));
			}
		}

		[NUnit.Framework.Test]
		public static void TestSquare()
		{
			com.epl.geometry.OperatorConvexHull bounding = (com.epl.geometry.OperatorConvexHull)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ConvexHull);
			com.epl.geometry.OperatorDensifyByLength densify = (com.epl.geometry.OperatorDensifyByLength)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.DensifyByLength);
			com.epl.geometry.OperatorDifference difference = (com.epl.geometry.OperatorDifference)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Difference);
			com.epl.geometry.OperatorContains contains = (com.epl.geometry.OperatorContains)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains);
			com.epl.geometry.Polygon square = new com.epl.geometry.Polygon();
			square.StartPath(0, 0);
			square.LineTo(2, 3);
			square.LineTo(1, 4);
			square.LineTo(0, 5);
			square.LineTo(0, 7);
			square.LineTo(2, 7);
			square.LineTo(0, 10);
			square.LineTo(4, 7);
			square.LineTo(6, 7);
			square.LineTo(7, 10);
			square.LineTo(8, 10);
			square.LineTo(10, 10);
			square.LineTo(8, 7);
			square.LineTo(10, 5);
			square.LineTo(8, 3);
			square.LineTo(10, 1);
			square.LineTo(10, 0);
			square.LineTo(5, 5);
			square.LineTo(8, 0);
			square.LineTo(4, 3);
			square.LineTo(5, 0);
			square.LineTo(3, 1);
			square.LineTo(3, 0);
			square.LineTo(2, 1);
			com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(square, 1.0, null));
			densified.AddAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
			for (int i = 0; i < densified.GetPointCount(); i++)
			{
				densified.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, i, 0, i);
			}
			com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(densified, null));
			com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(densified, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
			NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
			NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
		}

//		[NUnit.Framework.Test]
//		public static void TestPolygons()
//		{
//			com.epl.geometry.OperatorConvexHull bounding = (com.epl.geometry.OperatorConvexHull)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ConvexHull);
//			com.epl.geometry.OperatorDensifyByLength densify = (com.epl.geometry.OperatorDensifyByLength)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.DensifyByLength);
//			com.epl.geometry.OperatorDifference difference = (com.epl.geometry.OperatorDifference)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Difference);
//			{
//				com.epl.geometry.Polygon shape = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[1.3734426370715553,-90],[-24.377532184629967,-63.428606327053856],[-25.684686546621553,90],[-24.260574484321914,80.526315789473699],[-25.414389575040037,90],[-23.851448513708718,90],[-23.100135788742072,87.435887853000679],[5.6085096351011448,-48.713222410606306],[1.3734426370715553,-90]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(shape, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(shape, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[-179.64843749999693,-43.3535476539993],[-179.99999999999696,-43.430006211999284],[-179.99999999999696,39.329644416999436],[-179.64843749999693,38.98862638799943],[-89.99999999999838,29.008084980999506],[-112.8515624999981,-16.20113770599962],[-115.66406249999804,-18.882554574999688],[-124.80468749999788,-23.7925511709996],[-138.86718749999767,-30.6635901109995],[-157.49999999999736,-38.468358112999354],[-162.42187499999724,-39.56498442199932],[-179.64843749999693,-43.3535476539993]],[[179.99999999999696,-43.430006211999284],[179.64843749999693,-43.50646476999926],[162.0703124999973,-42.36267115399919],[160.3124999999973,-42.24790485699929],[143.78906249999756,-41.1680427339993],[138.16406249999767,-39.64744846799925],[98.43749999999845,-28.523889212999524],[78.39843749999878,-5.1644422999998705],[75.9374999999988,19.738611663999766],[88.2421874999986,33.51651305599954],[108.63281249999815,44.160795160999356],[138.16406249999767,51.02062617799914],[140.9765624999976,51.68129673399923],[160.3124999999973,52.8064856429991],[162.0703124999973,52.908902047999206],[163.12499999999727,52.97036560499911],[165.93749999999716,52.97036560499911],[179.99999999999696,39.329644416999436],[179.99999999999696,-43.430006211999284]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 10.0, null));
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(densified, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(densified, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
//				polygon.StartPath(1, 0);
//				polygon.LineTo(-1, 0);
//				polygon.LineTo(0, -1);
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[-38.554566833914528,21.902000764339238],[-30.516168471666138,90],[-38.554566833914528,21.902000764339238]],[[-43.013227444613932,28.423410187206883],[-43.632473335895916,90],[-42.342268693420237,62.208637129146894],[-37.218731802058755,63.685357222187029],[-32.522681335230686,47.080307818055296],[-40.537308829621097,-21.881392019745604],[-47.59510451722663,18.812521648505964],[-53.25344489340366,30.362745244224911],[-46.629060462410138,90],[-50.069277433245119,18.254168921734287],[-42.171214434397982,72.623347387008081],[-43.000844452530551,90],[-46.162281544954659,90],[-39.462049205071331,90],[-47.434856316742902,38.662565208814371],[-52.13115779642537,-19.952586632199857],[-56.025328966335081,90],[-60.056846215416158,-44.023645282268355],[-60.12338894192289,50.374596189881942],[-35.787508034048379,-7.8839007676038513],[-60.880218074135605,-46.447995750907815],[-67.782542852117956,-85.106300958016107],[-65.053131764313761,-0.96651520578494665],[-72.375821140304154,90],[-78.561502106749245,90],[-83.809168672565946,33.234498214085811],[-60.880218054506344,-46.447995733653201],[-75.637095425108981,59.886574792622838],[-71.364085965028096,31.976373491332097],[-67.89968380886117,90],[-67.544349171474749,8.8435794458927504],[-70.780047377934707,80.683454463576624],[-64.996733940204948,34.349882797035313],[-56.631753638680905,39.815838152456926],[-60.392350183516896,52.75446132093407],[-58.51633728692137,90],[-64.646972065627097,41.444197803942579],[-73.355591244695518,-0.15370205145035776],[-43.013227444613932,28.423410187206883]],[[-69.646471076946,-85.716191379686904],[-62.854465128320491,-45.739046580967972],[-71.377481570643141,-90],[-66.613495837251435,-90],[-66.9765142407159,-90],[-66.870099169607329,-90],[-67.23180828626819,-61.248439074609649],[-58.889775875438851,-90],[-53.391995883729322,-69.476385967096491],[-69.646471076946,-85.716191379686904]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 10.0, null));
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(densified, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(densified, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//			}
//			{
//				// assertTrue(bounding.isConvex(*convex_hull, null));
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[-38.554566833914528,21.902000764339238],[-30.516168471666138,90],[-38.554566833914528,21.902000764339238]],[[-43.013227444613932,28.423410187206883],[-43.632473335895916,90],[-42.342268693420237,62.208637129146894],[-37.218731802058755,63.685357222187029],[-32.522681335230686,47.080307818055296],[-40.537308829621097,-21.881392019745604],[-47.59510451722663,18.812521648505964],[-53.25344489340366,30.362745244224911],[-46.629060462410138,90],[-50.069277433245119,18.254168921734287],[-42.171214434397982,72.623347387008081],[-43.000844452530551,90],[-46.162281544954659,90],[-39.462049205071331,90],[-47.434856316742902,38.662565208814371],[-52.13115779642537,-19.952586632199857],[-56.025328966335081,90],[-60.056846215416158,-44.023645282268355],[-60.12338894192289,50.374596189881942],[-35.787508034048379,-7.8839007676038513],[-60.880218074135605,-46.447995750907815],[-67.782542852117956,-85.106300958016107],[-65.053131764313761,-0.96651520578494665],[-72.375821140304154,90],[-78.561502106749245,90],[-83.809168672565946,33.234498214085811],[-60.880218054506344,-46.447995733653201],[-75.637095425108981,59.886574792622838],[-71.364085965028096,31.976373491332097],[-67.89968380886117,90],[-67.544349171474749,8.8435794458927504],[-70.780047377934707,80.683454463576624],[-64.996733940204948,34.349882797035313],[-56.631753638680905,39.815838152456926],[-60.392350183516896,52.75446132093407],[-58.51633728692137,90],[-64.646972065627097,41.444197803942579],[-73.355591244695518,-0.15370205145035776],[-43.013227444613932,28.423410187206883]],[[-69.646471076946,-85.716191379686904],[-62.854465128320491,-45.739046580967972],[-71.377481570643141,-90],[-66.613495837251435,-90],[-66.9765142407159,-90],[-66.870099169607329,-90],[-67.23180828626819,-61.248439074609649],[-58.889775875438851,-90],[-53.391995883729322,-69.476385967096491],[-69.646471076946,-85.716191379686904]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[-36.269498017702901,-26.37490682626369],[-49.146436641060951,-49.881862499696126],[-37.560006446488146,-45.592052597656789],[-39.13770692863632,-69.085816352131204],[-65.415587331361877,-90],[-51.462290812033373,-16.760787566546721],[-28.454456182408332,90],[-36.269498017702901,-26.37490682626369]],[[-40.542178258552283,-90],[-39.13770692863632,-69.085816352131204],[-16.295804332590937,-50.906277575066262],[-40.542178258552283,-90]],[[-16.295804332590937,-50.906277575066262],[-5.6790432913971927,-33.788307256548933],[14.686101893282586,-26.248228042967728],[-16.295804332590937,-50.906277575066262]],[[-37.560006446488146,-45.592052597656789],[-36.269498017702901,-26.37490682626369],[27.479825940672225,90],[71.095881152477034,90],[-5.6790432913971927,-33.788307256548933],[-37.560006446488146,-45.592052597656789]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[-77.020281185856106,-80.085419699581706],[-77.328568930885723,-83.404479889897416],[-80.495259564600545,-90],[-77.020281185856106,-80.085419699581706]],[[-77.941187535385211,-90],[-77.328568930885723,-83.404479889897416],[-39.252034383972621,-4.0994329574862469],[-39.29471328421063,-6.5269494453154593],[-77.941187535385211,-90]],[[-77.020281185856106,-80.085419699581706],[-62.688864277996522,74.208210509833052],[-38.108861278327581,80.371071656873013],[-37.597643844595929,90],[-38.663943358642484,29.350366647752089],[-77.020281185856106,-80.085419699581706]],[[-40.265125886194951,-61.722668598742551],[-39.29471328421063,-6.5269494453154593],[-15.554402498931253,44.750073899273843],[-8.4447006412989474,13.127318978368956],[-5.310206313296316,-4.5170390491918795],[-40.265125886194951,-61.722668598742551]],[[-39.252034383972621,-4.0994329574862469],[-38.663943358642484,29.350366647752089],[-22.476078360563164,75.536520897660651],[-15.632105532320049,45.095683888365997],[-39.252034383972621,-4.0994329574862469]],[[-15.554402498931253,44.750073899273843],[-15.632105532320049,45.095683888365997],[-8.9755856576261941,58.959750756602595],[-15.554402498931253,44.750073899273843]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[-68.840007952128175,37.060080998089632],[-68.145986924561413,31.114096694815196],[-69.187773850176768,30.693518246958952],[-68.840007952128175,37.060080998089632]],[[-75.780513355389928,-90],[-69.21567111077384,30.182802098042274],[-50.875629803516389,37.146119571446704],[-75.780513355389928,-90]],[[4.2911006174797457,-1.144569312564311],[-66.484019915251849,80.191238371060038],[-65.948228008382316,90],[4.2911006174797457,-1.144569312564311]],[[-90,22.291441435181515],[-69.187773850176768,30.693518246958952],[-69.21567111077384,30.182802098042274],[-90,22.291441435181515]],[[-68.840007952128175,37.060080998089632],[-75.019206401201359,90],[-66.484019915251849,80.191238371060038],[-68.840007952128175,37.060080998089632]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[27.570109889215438,22.850616190228489],[75.703105729477357,-90],[2.1548000876241362,-15.817792950796967],[27.570109889215438,22.850616190228489]],[[-0.069915984436478951,-90],[-46.602410662754053,-89.999999998014729],[-14.977190481820156,-41.883452819243004],[-0.069915984436478951,-90]],[[-14.977190481820156,-41.883452819243004],[-34.509989609682322,21.163004866431177],[2.1548000876241362,-15.817792950796967],[-14.977190481820156,-41.883452819243004]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[28.865673900286581,33.379551302126075],[39.505669183485338,-34.957993133630559],[7.152466542048213,-90],[28.865673900286581,33.379551302126075]],[[-64.597291313620858,2.4515644574812248],[20.050002923927103,90],[24.375150856531356,62.220853377417541],[-64.597291313620858,2.4515644574812248]],[[28.865673900286581,33.379551302126075],[24.375150856531356,62.220853377417541],[35.223952527956932,69.508785974507163],[28.865673900286581,33.379551302126075]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[-66.582505384413167,-51.305212522944061],[-60.169897093348865,-90],[-90,-90],[-66.582505384413167,-51.305212522944061]],[[20.858462934004656,-90],[-35.056287147954386,0.78833269359179781],[18.933251883215579,90],[20.858462934004656,-90]],[[-66.582505384413167,-51.305212522944061],[-90,90],[-35.056287147954386,0.78833269359179781],[-66.582505384413167,-51.305212522944061]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[36.692916710279974,-90],[-31.182443792600079,6.434474852744998],[-90,90],[52.245260790065387,57.329280208760991],[36.692916710279974,-90]]]}").GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[-17.959089916602533,-4.3577640799248218],[-29.181784251472308,-90],[-65.493717350127127,15.053615507086979],[-17.959089916602533,-4.3577640799248218]],[[-21.884657435973146,-34.517617672142393],[-17.94005076020704,-4.3655389655558539],[9.3768748358343359,-15.520758655380195],[-21.884657435973146,-34.517617672142393]],[[-17.94005076020704,-4.3655389655558539],[-17.959089916602533,-4.3577640799248218],[-5.8963967801936494,87.694641571893939],[-17.94005076020704,-4.3655389655558539]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[17.198360589495572,-77.168667157542373],[-24.835678541343281,-83.717338556412017],[-30.259244993378722,61.914816728303791],[17.198360589495572,-77.168667157542373]],[[-8.3544985146710644,-90],[17.979891823366039,-79.459092168186686],[21.576625471325329,-90],[-8.3544985146710644,-90]],[[17.979891823366039,-79.459092168186686],[17.198360589495572,-77.168667157542373],[27.846596597209441,-75.509730732825361],[17.979891823366039,-79.459092168186686]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[-1.2588613620456419,13.607321860624268],[61.845346679259052,48.415944386573557],[15.226225965240992,-5.3702891526017318],[0.92681706095183469,1.6819284384951441],[3.8469417404317623,-14.250715301799051],[7.2615297628459139,-14.559458820527061],[4.4896578086498238,-17.757471781424698],[14.589138845678622,-72.861774161244625],[-10.508572009494033,-35.06149380752737],[-58.12642296329372,-90],[-15.260062192400673,90],[-1.2588613620456419,13.607321860624268]],[[0.92681706095183469,1.6819284384951441],[-1.2588613620456419,13.607321860624268],[-11.641308877525201,7.8803076458946304],[0.92681706095183469,1.6819284384951441]],[[-10.508572009494033,-35.06149380752737],[4.4896578086498238,-17.757471781424698],[3.8469417404317623,-14.250715301799051],[-26.125369947914503,-11.54064986657559],[-10.508572009494033,-35.06149380752737]],[[39.829571435268129,-17.504227477249202],[7.2615297628459139,-14.559458820527061],[15.226225965240992,-5.3702891526017318],[39.829571435268129,-17.504227477249202]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[-19.681975166855118,-34.10344217707847],[-90,89.999999998418275],[53.036316534501381,90],[-19.681975166855118,-34.10344217707847]],[[-52.434509065706855,-90],[-29.2339442498794,-50.405148598356135],[-2.8515119199232331,-90],[-52.434509065706855,-90]],[[18.310881874573923,-90],[-25.473718245381271,-43.987822508814972],[-19.681975166855118,-34.10344217707847],[-15.406194071963924,-41.649717163101563],[18.310881874573923,-90]],[[-29.2339442498794,-50.405148598356135],[-52.27954259799813,-15.81822990020261],[-25.473718245381271,-43.987822508814972],[-29.2339442498794,-50.405148598356135]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[49.939516827702498,-90],[-20.470128740962011,-68.102019032647391],[-20.124197553433845,-67.213968219799824],[34.438329237618149,-61.893901496061034],[49.939516827702498,-90]],[[-82.380918375714089,-73.284249936115529],[-4.7432060543229699,9.1484031048644194],[-11.790524932251525,21.926303986370414],[-3.4862200343039369,10.483021157965428],[19.753975453441285,35.158541777575607],[5.5896897290794696,-1.2030408273476854],[73.839023528563189,-58.052174675157325],[34.438329237618149,-61.893901496061034],[3.6757233436274213,-6.1164440290327313],[-20.124197553433845,-67.213968219799824],[-82.380918375714089,-73.284249936115529]],[[5.5896897290794696,-1.2030408273476854],[4.0842948437219349,0.050896618883412792],[-3.4862200343039369,10.483021157965428],[-4.7432060543229699,9.1484031048644194],[3.6757233436274213,-6.1164440290327313],[5.5896897290794696,-1.2030408273476854]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[4.7618727814345867,-14.245890151885444],[-7.1675180359486266,-90],[-83.232840716292529,40.620187389409224],[-29.219286930421923,6.9418934044755334],[-29.378277853968513,6.9629531745072839],[-28.933835455648254,6.7639099538036529],[4.7618727814345867,-14.245890151885444]],[[51.056303527367277,-43.111190419066219],[4.7618727814345867,-14.245890151885444],[5.632592229367642,-8.716640778187827],[-28.933835455648254,6.7639099538036529],[-29.219286930421923,6.9418934044755334],[2.700964609629902,2.7137705544807242],[12.385960896403816,0.48342578457646468],[51.056303527367277,-43.111190419066219]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[-21.426619830983732,-89.379667629404466],[-72.784461583687971,-88.999754827814016],[-81.94289434769162,25.456737039611831],[-38.382426191605546,-57.204127144336077],[-41.663734179022256,-78.439084394036513],[-29.749353943865881,-73.586348060869426],[-21.426619830983732,-89.379667629404466]],[[-21.09971823441461,-90],[-21.426619830983732,-89.379667629404466],[-21.080965849893449,-89.382224558742934],[-21.09971823441461,-90]],[[62.431917153693021,-90],[-21.080965849893449,-89.382224558742934],[-20.486971473666468,-69.813772479288062],[19.166418765782844,-53.662915804391695],[63.671046682728601,-90],[62.431917153693021,-90]],[[-29.749353943865881,-73.586348060869426],[-38.382426191605546,-57.204127144336077],[-31.449272112025476,-12.336278393150847],[-41.028899505665962,-4.5147159296945967],[-30.750049689226596,-7.8112663207986941],[-15.63587330244308,90],[-18.721998818789388,-11.66880646480822],[60.158611185675326,-36.966763960486929],[19.166418765782844,-53.662915804391695],[-19.049573405176112,-22.46036923493498],[-20.486971473666468,-69.813772479288062],[-29.749353943865881,-73.586348060869426]],[[-19.049573405176112,-22.46036923493498],[-18.721998818789388,-11.66880646480822],[-30.750049689226596,-7.8112663207986941],[-31.449272112025476,-12.336278393150847],[-19.049573405176112,-22.46036923493498]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[-17.906614911617162,-53.670186894017093],[-72.687715727164573,-90],[-77.889582483879749,90],[-47.149885004784061,16.372797801863811],[-58.874489264131405,8.3403055152440846],[-44.017112148517498,8.8692333739436133],[-43.760297522359615,8.2541153357643502],[-48.398890069305921,4.7201397602360009],[-38.665987052649818,-3.9476907252248874],[-17.906614911617162,-53.670186894017093]],[[-2.7387498969355368,-90],[-17.906614911617162,-53.670186894017093],[-6.8038688963847829,-46.30705103709559],[-2.7387498969355368,-90]],[[-6.8038688963847829,-46.30705103709559],[-8.2224486207861638,-31.0597897622158],[2.1962303277340673,-40.338351652092697],[-6.8038688963847829,-46.30705103709559]],[[-8.2224486207861638,-31.0597897622158],[-38.665987052649818,-3.9476907252248874],[-43.760297522359615,8.2541153357643502],[-42.90074612601282,8.9089763975751382],[-44.017112148517498,8.8692333739436133],[-47.149885004784061,16.372797801863811],[45.190674429223662,79.635046572817728],[40.490070954305672,72.441418146356597],[63.53694979672099,90],[75.056911135062407,13.108310545642606],[-0.027204347469059975,10.435289586728711],[-10.580480746811602,-5.715051428780245],[-8.2224486207861638,-31.0597897622158]],[[-42.90074612601282,8.9089763975751382],[-0.027204347469059975,10.435289586728711],[40.490070954305672,72.441418146356597],[-42.90074612601282,8.9089763975751382]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(polygon, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//		}

		[NUnit.Framework.Test]
		public static void TestPolylines()
		{
			com.epl.geometry.OperatorConvexHull bounding = (com.epl.geometry.OperatorConvexHull)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ConvexHull);
			com.epl.geometry.OperatorDensifyByLength densify = (com.epl.geometry.OperatorDensifyByLength)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.DensifyByLength);
			com.epl.geometry.OperatorDifference difference = (com.epl.geometry.OperatorDifference)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Difference);
			com.epl.geometry.OperatorContains contains = (com.epl.geometry.OperatorContains)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains);
			{
				com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
				poly.StartPath(0, 0);
				poly.LineTo(0, 10);
				poly.LineTo(0, 20);
				poly.LineTo(0, 40);
				poly.LineTo(0, 500);
				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(poly, 10.0, null));
				com.epl.geometry.Polyline convex_hull = (com.epl.geometry.Polyline)(bounding.Execute(densified, null));
				com.epl.geometry.Polyline differenced = (com.epl.geometry.Polyline)(difference.Execute(densified, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
			}
			{
				com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
				polyline.StartPath(-200, -90);
				polyline.LineTo(-180, -85);
				polyline.LineTo(-90, -70);
				polyline.LineTo(0, 0);
				polyline.LineTo(100, 25);
				polyline.LineTo(170, 45);
				polyline.LineTo(225, 65);
				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 10.0, null));
				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(densified, null));
				bool bcontains = contains.Execute(convex_hull, densified, com.epl.geometry.SpatialReference.Create(4326), null);
				NUnit.Framework.Assert.IsTrue(bcontains);
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
			}
		}

		[NUnit.Framework.Test]
		public static void TestNonSimpleShape()
		{
			com.epl.geometry.OperatorConvexHull bounding = (com.epl.geometry.OperatorConvexHull)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ConvexHull);
			com.epl.geometry.OperatorDensifyByLength densify = (com.epl.geometry.OperatorDensifyByLength)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.DensifyByLength);
			com.epl.geometry.OperatorDifference difference = (com.epl.geometry.OperatorDifference)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Difference);
			com.epl.geometry.OperatorContains contains = (com.epl.geometry.OperatorContains)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains);
//			{
//				com.epl.geometry.Polygon shape = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[6.7260599916745036,-90],[15.304037095218971,-18.924146439950675],[6.3163297788539232,-90],[5.2105387036445823,-59.980719950158637],[5.1217504663506981,-60.571174400735508],[8.2945138368731044,-27.967042187979146],[15.76606600357545,28.594953216378414],[8.4365340991447919,66.880924521951329],[10.115022726199774,65.247385313781265],[12.721206966604395,-23.793016208456883],[12.051875868947576,-11.368909618476637],[11.867118776841318,44.968896646914239],[7.5326099218274614,35.095776924526533],[8.86765609460479,90],[3.2036592678446922,55.507964789691712],[0.23585282258761486,-42.620591380394039],[-1.2660432762142744,90],[5.5580612840503001,-9.4879902323389196],[12.258387597532487,-35.945231749575591],[-48.746716054894101,90],[7.2294405148356846,-15.719232058488402],[13.798313011339591,-10.467172541381753],[7.4430022048746718,6.3951685161785656],[6.4876332898327815,31.10016146737189],[9.3645424359058911,47.123308099298804],[13.398605254542668,-6.4398318586014325],[-90,90],[13.360786277212718,82.971274676174545],[7.9405631778693566,90],[10.512482079680538,90],[16.994982794293946,19.60673041736408],[16.723893839323615,22.728853852102926],[23.178783416627525,90],[6.7260599916745036,-90]],[[26.768777234301993,90],[20.949797955126346,90],[11.967758262201434,-0.45048849056049711],[17.535751576687339,52.767528591651441],[26.768777234301993,90]],[[18.677765775891793,12.559680067559942],[19.060218406331451,90],[17.123595624401705,90],[-2.3805299720687887,-90],[-11.882782057881979,-90],[21.640575461689693,90],[11.368255808198477,85.501555553904794],[17.390084032215348,90],[23.999392897519989,78.255909006554603],[-6.8860811786563101,69.49189433189926],[29.232578855788898,90],[25.951412073846683,90],[-5.5572284181160772,-16.763772082849457],[18.677765775891793,12.559680067559942]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(shape, 10.0, null));
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(densified, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(densified, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon shape = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[-13.630596027421603,3.6796011190640709],[-10.617275202716886,-28.133054337834409],[-81.617315194491695,90],[-4.0763362539824293,-90],[2.8213537979804073,-90],[-5.1515857979973365,-11.605767592612787],[43.477754021411123,35.507543731267589],[-45.818261267516704,-90],[-4.8545715514870018,-64.204371906322223],[-1.744951154293144,-30.257848194381509],[-7.8524748309267149,15.513561279453089],[-10.657563385538953,-81.785061432086309],[-6.3487369893289411,-31.849779201388415],[-14.768278524737962,-12.004393281111058],[-27.001635582579841,90],[-14.967554248940855,-78.970629918591811],[-12.999635147475825,-38.584472796107939],[-13.630596027421603,3.6796011190640709]],[[-16.338143621861352,-37.415690513288375],[-21.553879270366266,-90],[-18.649338100909404,-90],[-24.880584966233631,1.3133858590648728],[-16.483464632078249,-53.979692212288882],[-24.836979215403964,-68.69859399640147],[-29.708282990385214,-90],[-27.469962102507036,-1.6392995673644872],[-20.405051753708271,61.943199597870034],[-18.242567838912599,24.405109362934219],[-66.334547696572528,-52.678390155566603],[-13.471083255903507,-33.782708412943229],[-7.092757068096085,33.673785662500464],[-2.7427100969018205,74.386868339212668],[-8.2174861339989675,90],[-15.699459164009667,90],[-9.5910045204059156,90],[-8.4504603287557369,90],[-1.5498862802092637,2.5144190340747681],[-6.5326327868410639,-17.428029961128306],[-10.947786354404593,31.516236387466538],[-7.4777936485986354,12.486727826508769],[-13.89052186883092,12.397126427870356],[-10.530672679779606,-55.463541447339118],[-8.7161833631330374,-90],[-4.7231067612639519,-90],[-3.9692500849117041,-32.204677519048822],[3.740804266163555,32.88191805391007],[6.2021313886056246,76.617541950091564],[6.1183997672398194,90],[0.59730820015390673,90],[7.3242950674530753,18.030401540676614],[1.8252371571535342,90],[-16.338143621861352,-37.415690513288375]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(shape, 10.0, null));
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(densified, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(densified, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
//			{
//				com.epl.geometry.Polygon shape = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[-11.752662474672046,-90],[-76.236530072126513,7.3247326417920817],[18.933251883215579,90],[51.538924439116798,90],[52.253017336758049,80.352482145105284],[41.767201918260639,51.890297432229289],[21.697252770910882,-1.3185641048567049],[45.112193442818935,60.758441021743636],[48.457184967377231,69.626584611257954],[49.531808284502759,70.202152706968036],[52.394797054144334,71.533541126234581],[ 52.9671102343993,70.704964290210626],[58.527850348069251,16.670036266565845],[62.310807912773328,-34.249918700039238],[62.775020703241523,-43.541598916699364],[64.631871865114277,-80.708319783339874],[65.096084655582459,-90],[-11.752662474672046,-90]]]}"
//					).GetGeometry());
//				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(shape, null));
//				com.epl.geometry.Polygon differenced = (com.epl.geometry.Polygon)(difference.Execute(shape, convex_hull, com.epl.geometry.SpatialReference.Create(4326), null));
//				NUnit.Framework.Assert.IsTrue(differenced.IsEmpty());
//				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
//			}
			{
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				polygon.StartPath(0, 0);
				polygon.LineTo(0, 10);
				polygon.LineTo(4, 10);
				polygon.LineTo(9, 1);
				polygon.LineTo(1, 1);
				polygon.LineTo(5, 10);
				polygon.LineTo(10, 10);
				polygon.LineTo(10, 0);
				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1, null));
				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(densified, null));
				double area = convex_hull.CalculateArea2D();
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				NUnit.Framework.Assert.IsTrue(area == 100.0);
				com.epl.geometry.Point2D p1 = convex_hull.GetXY(0);
				com.epl.geometry.Point2D p2 = convex_hull.GetXY(1);
				com.epl.geometry.Point2D p3 = convex_hull.GetXY(2);
				com.epl.geometry.Point2D p4 = convex_hull.GetXY(3);
				NUnit.Framework.Assert.IsTrue(p1.x == 0.0 && p1.y == 0.0);
				NUnit.Framework.Assert.IsTrue(p2.x == 0.0 && p2.y == 10.0);
				NUnit.Framework.Assert.IsTrue(p3.x == 10.0 && p3.y == 10.0);
				NUnit.Framework.Assert.IsTrue(p4.x == 10.0 && p4.y == 0.0);
			}
			{
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				polygon.StartPath(0, 0);
				polygon.LineTo(0, 10);
				polygon.LineTo(8, 10);
				polygon.LineTo(10, 8);
				polygon.LineTo(10, 0);
				polygon.LineTo(0, 0);
				polygon.LineTo(0, 10);
				polygon.LineTo(10, 10);
				polygon.LineTo(10, 0);
				// Polygon densified = (Polygon)(densify.execute(polygon, 1, null));
				com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(polygon, null));
				double area = convex_hull.CalculateArea2D();
				NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
				NUnit.Framework.Assert.IsTrue(area == 100.0);
				com.epl.geometry.Point2D p1 = convex_hull.GetXY(0);
				com.epl.geometry.Point2D p2 = convex_hull.GetXY(1);
				com.epl.geometry.Point2D p3 = convex_hull.GetXY(2);
				com.epl.geometry.Point2D p4 = convex_hull.GetXY(3);
				NUnit.Framework.Assert.IsTrue(p1.x == 0.0 && p1.y == 0.0);
				NUnit.Framework.Assert.IsTrue(p2.x == 0.0 && p2.y == 10.0);
				NUnit.Framework.Assert.IsTrue(p3.x == 10.0 && p3.y == 10.0);
				NUnit.Framework.Assert.IsTrue(p4.x == 10.0 && p4.y == 0.0);
			}
		}

		[NUnit.Framework.Test]
		public static void TestStar()
		{
			com.epl.geometry.OperatorConvexHull bounding = (com.epl.geometry.OperatorConvexHull)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ConvexHull);
			com.epl.geometry.OperatorDensifyByLength densify = (com.epl.geometry.OperatorDensifyByLength)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.DensifyByLength);
			com.epl.geometry.Polygon star = new com.epl.geometry.Polygon();
			star.StartPath(0, 0);
			star.LineTo(0, 0);
			star.LineTo(5, 10);
			star.LineTo(5, 10);
			star.LineTo(10, 0);
			star.LineTo(10, 0);
			star.StartPath(0, 5);
			star.LineTo(0, 5);
			star.LineTo(10, 5);
			star.LineTo(10, 5);
			star.LineTo(5, -5);
			star.LineTo(5, -5);
			star.ReversePath(1);
			com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(star, 1.0, null));
			com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(bounding.Execute(densified, null));
			NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
			com.epl.geometry.Point2D p1 = convex_hull.GetXY(0);
			com.epl.geometry.Point2D p2 = convex_hull.GetXY(1);
			com.epl.geometry.Point2D p3 = convex_hull.GetXY(2);
			com.epl.geometry.Point2D p4 = convex_hull.GetXY(3);
			com.epl.geometry.Point2D p5 = convex_hull.GetXY(4);
			com.epl.geometry.Point2D p6 = convex_hull.GetXY(5);
			NUnit.Framework.Assert.IsTrue(p1.x == 0.0 && p1.y == 0.0);
			NUnit.Framework.Assert.IsTrue(p2.x == 0.0 && p2.y == 5.0);
			NUnit.Framework.Assert.IsTrue(p3.x == 5.0 && p3.y == 10.0);
			NUnit.Framework.Assert.IsTrue(p4.x == 10.0 && p4.y == 5.0);
			NUnit.Framework.Assert.IsTrue(p5.x == 10.0 && p5.y == 0.0);
			NUnit.Framework.Assert.IsTrue(p6.x == 5.0 && p6.y == -5.0);
		}

		[NUnit.Framework.Test]
		public static void TestPointsArray()
		{
			com.epl.geometry.Point2D[] points = new com.epl.geometry.Point2D[6];
			int[] convex_hull = new int[6];
			for (int i = 0; i < 6; i++)
			{
				points[i] = new com.epl.geometry.Point2D();
			}
			points[0].x = 0;
			points[0].y = 0;
			points[1].x = 5;
			points[1].y = 10;
			points[2].x = 10;
			points[2].y = 0;
			points[3].x = 0;
			points[3].y = 5;
			points[4].x = 10;
			points[4].y = 5;
			points[5].x = 5;
			points[5].y = -5;
			com.epl.geometry.ConvexHull.Construct(points, 6, convex_hull);
			NUnit.Framework.Assert.IsTrue(convex_hull[0] == 0);
			NUnit.Framework.Assert.IsTrue(convex_hull[1] == 3);
			NUnit.Framework.Assert.IsTrue(convex_hull[2] == 1);
			NUnit.Framework.Assert.IsTrue(convex_hull[3] == 4);
			NUnit.Framework.Assert.IsTrue(convex_hull[4] == 2);
			NUnit.Framework.Assert.IsTrue(convex_hull[5] == 5);
		}

		[NUnit.Framework.Test]
		public static void TestMergeCursor()
		{
			com.epl.geometry.OperatorConvexHull bounding = (com.epl.geometry.OperatorConvexHull)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ConvexHull);
			com.epl.geometry.Polygon geom1 = new com.epl.geometry.Polygon();
			com.epl.geometry.Polygon geom2 = new com.epl.geometry.Polygon();
			com.epl.geometry.Point geom3 = new com.epl.geometry.Point();
			com.epl.geometry.Line geom4 = new com.epl.geometry.Line();
			com.epl.geometry.Envelope geom5 = new com.epl.geometry.Envelope();
			com.epl.geometry.MultiPoint geom6 = new com.epl.geometry.MultiPoint();
			// polygon
			geom1.StartPath(0, 0);
			geom1.LineTo(0, 0);
			geom1.LineTo(5, 11);
			geom1.LineTo(5, 11);
			geom1.LineTo(10, 0);
			geom1.LineTo(10, 0);
			// polygon
			geom2.StartPath(0, 5);
			geom2.LineTo(0, 5);
			geom2.LineTo(10, 5);
			geom2.LineTo(10, 5);
			geom2.LineTo(5, -5);
			geom2.LineTo(5, -5);
			// point
			geom3.SetXY(15, 1.25);
			// segment
			geom4.SetEndXY(-5, 1.25);
			geom4.SetStartXY(0, 0);
			// envelope
			geom5.SetCoords(0, 0, 5, 10);
			// multi_point
			geom6.Add(10, 5);
			geom6.Add(10, 10);
			// create cursor
			com.epl.geometry.Geometry[] geoms = new com.epl.geometry.Geometry[6];
			geoms[0] = geom1;
			geoms[1] = geom2;
			geoms[2] = geom3;
			geoms[3] = geom4;
			geoms[4] = geom5;
			geoms[5] = geom6;
			com.epl.geometry.GeometryCursor cursor = new com.epl.geometry.SimpleGeometryCursor(geoms);
			// create convex hull from the cursor with b_merge set to true
			com.epl.geometry.GeometryCursor convex_hull_curs = bounding.Execute(cursor, true, null);
			com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)(convex_hull_curs.Next());
			NUnit.Framework.Assert.IsTrue(convex_hull_curs.Next() == null);
			NUnit.Framework.Assert.IsTrue(bounding.IsConvex(convex_hull, null));
			com.epl.geometry.Point2D p1 = convex_hull.GetXY(0);
			com.epl.geometry.Point2D p2 = convex_hull.GetXY(1);
			com.epl.geometry.Point2D p3 = convex_hull.GetXY(2);
			com.epl.geometry.Point2D p4 = convex_hull.GetXY(3);
			com.epl.geometry.Point2D p5 = convex_hull.GetXY(4);
			com.epl.geometry.Point2D p6 = convex_hull.GetXY(5);
			NUnit.Framework.Assert.IsTrue(p1.x == 5.0 && p1.y == 11.0);
			NUnit.Framework.Assert.IsTrue(p2.x == 10.0 && p2.y == 10);
			NUnit.Framework.Assert.IsTrue(p3.x == 15.0 && p3.y == 1.25);
			NUnit.Framework.Assert.IsTrue(p4.x == 5.0 && p4.y == -5.0);
			NUnit.Framework.Assert.IsTrue(p5.x == -5.0 && p5.y == 1.25);
			NUnit.Framework.Assert.IsTrue(p6.x == 0.0 && p6.y == 10.0);
			// Test GeometryEngine
			com.epl.geometry.Geometry[] merged_hull = com.epl.geometry.GeometryEngine.ConvexHull(geoms, true);
			convex_hull = (com.epl.geometry.Polygon)merged_hull[0];
			p1 = convex_hull.GetXY(0);
			p2 = convex_hull.GetXY(1);
			p3 = convex_hull.GetXY(2);
			p4 = convex_hull.GetXY(3);
			p5 = convex_hull.GetXY(4);
			p6 = convex_hull.GetXY(5);
			NUnit.Framework.Assert.IsTrue(p1.x == 5.0 && p1.y == 11.0);
			NUnit.Framework.Assert.IsTrue(p2.x == 10.0 && p2.y == 10);
			NUnit.Framework.Assert.IsTrue(p3.x == 15.0 && p3.y == 1.25);
			NUnit.Framework.Assert.IsTrue(p4.x == 5.0 && p4.y == -5.0);
			NUnit.Framework.Assert.IsTrue(p5.x == -5.0 && p5.y == 1.25);
			NUnit.Framework.Assert.IsTrue(p6.x == 0.0 && p6.y == 10.0);
		}

		[NUnit.Framework.Test]
		public virtual void TestHullTickTock()
		{
			com.epl.geometry.Polygon geom1 = new com.epl.geometry.Polygon();
			com.epl.geometry.Polygon geom2 = new com.epl.geometry.Polygon();
			com.epl.geometry.Point geom3 = new com.epl.geometry.Point();
			com.epl.geometry.Line geom4 = new com.epl.geometry.Line();
			com.epl.geometry.Envelope geom5 = new com.epl.geometry.Envelope();
			com.epl.geometry.MultiPoint geom6 = new com.epl.geometry.MultiPoint();
			// polygon
			geom1.StartPath(0, 0);
			geom1.LineTo(0, 0);
			geom1.LineTo(5, 11);
			geom1.LineTo(5, 11);
			geom1.LineTo(10, 0);
			geom1.LineTo(10, 0);
			// polygon
			geom2.StartPath(0, 5);
			geom2.LineTo(0, 5);
			geom2.LineTo(10, 5);
			geom2.LineTo(10, 5);
			geom2.LineTo(5, -5);
			geom2.LineTo(5, -5);
			// point
			geom3.SetXY(15, 1.25);
			// segment
			geom4.SetEndXY(-5, 1.25);
			geom4.SetStartXY(0, 0);
			// envelope
			geom5.SetCoords(0, 0, 5, 10);
			// multi_point
			geom6.Add(10, 5);
			geom6.Add(10, 10);
			// Create
			com.epl.geometry.ListeningGeometryCursor gc = new com.epl.geometry.ListeningGeometryCursor();
			com.epl.geometry.GeometryCursor ticktock = com.epl.geometry.OperatorConvexHull.Local().Execute(gc, true, null);
			// Use tick-tock to push a geometry and do a piece of work.
			gc.Tick(geom1);
			ticktock.Tock();
			gc.Tick(geom2);
			ticktock.Tock();
			gc.Tick(geom3);
			// skiped one tock just for testing.
			ticktock.Tock();
			gc.Tick(geom4);
			ticktock.Tock();
			gc.Tick(geom5);
			ticktock.Tock();
			gc.Tick(geom6);
			ticktock.Tock();
			// Get the result
			com.epl.geometry.Geometry result = ticktock.Next();
			com.epl.geometry.Polygon convex_hull = (com.epl.geometry.Polygon)result;
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.OperatorConvexHull.Local().IsConvex(convex_hull, null));
			com.epl.geometry.Point2D p1 = convex_hull.GetXY(0);
			com.epl.geometry.Point2D p2 = convex_hull.GetXY(1);
			com.epl.geometry.Point2D p3 = convex_hull.GetXY(2);
			com.epl.geometry.Point2D p4 = convex_hull.GetXY(3);
			com.epl.geometry.Point2D p5 = convex_hull.GetXY(4);
			com.epl.geometry.Point2D p6 = convex_hull.GetXY(5);
			NUnit.Framework.Assert.IsTrue(p1.x == 5.0 && p1.y == 11.0);
			NUnit.Framework.Assert.IsTrue(p2.x == 10.0 && p2.y == 10);
			NUnit.Framework.Assert.IsTrue(p3.x == 15.0 && p3.y == 1.25);
			NUnit.Framework.Assert.IsTrue(p4.x == 5.0 && p4.y == -5.0);
			NUnit.Framework.Assert.IsTrue(p5.x == -5.0 && p5.y == 1.25);
			NUnit.Framework.Assert.IsTrue(p6.x == 0.0 && p6.y == 10.0);
		}
	}
}
