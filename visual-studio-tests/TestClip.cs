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
	public class TestClip : NUnit.Framework.TestFixtureAttribute
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
		public static void TestClipGeometries()
		{
			// RandomTest();
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorClip clipOp = (com.epl.geometry.OperatorClip)engine.GetOperator(com.epl.geometry.Operator.Type.Clip);
			com.epl.geometry.Polygon polygon = MakePolygon();
			com.epl.geometry.SimpleGeometryCursor polygonCurs = new com.epl.geometry.SimpleGeometryCursor(polygon);
			com.epl.geometry.Polyline polyline = MakePolyline();
			com.epl.geometry.SimpleGeometryCursor polylineCurs = new com.epl.geometry.SimpleGeometryCursor(polyline);
			com.epl.geometry.MultiPoint multipoint = MakeMultiPoint();
			com.epl.geometry.SimpleGeometryCursor multipointCurs = new com.epl.geometry.SimpleGeometryCursor(multipoint);
			com.epl.geometry.Point point = MakePoint();
			com.epl.geometry.SimpleGeometryCursor pointCurs = new com.epl.geometry.SimpleGeometryCursor(point);
			com.epl.geometry.SpatialReference spatialRef = com.epl.geometry.SpatialReference.Create(3857);
			com.epl.geometry.Envelope2D envelope = new com.epl.geometry.Envelope2D();
			envelope.xmin = 0;
			envelope.xmax = 20;
			envelope.ymin = 5;
			envelope.ymax = 15;
			// Cursor implementation
			com.epl.geometry.GeometryCursor clipPolygonCurs = clipOp.Execute(polygonCurs, envelope, spatialRef, null);
			com.epl.geometry.Polygon clippedPolygon = (com.epl.geometry.Polygon)clipPolygonCurs.Next();
			double area = clippedPolygon.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - 25) < 0.00001);
			// Single Geometry implementation
			clippedPolygon = (com.epl.geometry.Polygon)clipOp.Execute(polygon, envelope, spatialRef, null);
			area = clippedPolygon.CalculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(area - 25) < 0.00001);
			// Cursor implementation
			com.epl.geometry.GeometryCursor clipPolylineCurs = clipOp.Execute(polylineCurs, envelope, spatialRef, null);
			com.epl.geometry.Polyline clippedPolyline = (com.epl.geometry.Polyline)clipPolylineCurs.Next();
			double length = clippedPolyline.CalculateLength2D();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(length - 10 * System.Math.Sqrt(2.0)) < 1e-10);
			// Single Geometry implementation
			clippedPolyline = (com.epl.geometry.Polyline)clipOp.Execute(polyline, envelope, spatialRef, null);
			length = clippedPolyline.CalculateLength2D();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(length - 10 * System.Math.Sqrt(2.0)) < 1e-10);
			// Cursor implementation
			com.epl.geometry.GeometryCursor clipMulti_pointCurs = clipOp.Execute(multipointCurs, envelope, spatialRef, null);
			com.epl.geometry.MultiPoint clipped_multi_point = (com.epl.geometry.MultiPoint)clipMulti_pointCurs.Next();
			int pointCount = clipped_multi_point.GetPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 2);
			// Cursor implementation
			com.epl.geometry.GeometryCursor clipPointCurs = clipOp.Execute(pointCurs, envelope, spatialRef, null);
			com.epl.geometry.Point clippedPoint = (com.epl.geometry.Point)clipPointCurs.Next();
			NUnit.Framework.Assert.IsTrue(clippedPoint != null);
			// RandomTest();
			com.epl.geometry.Polyline _poly = new com.epl.geometry.Polyline();
			_poly.StartPath(2, 2);
			_poly.LineTo(0, 0);
			com.epl.geometry.Envelope2D _env = new com.epl.geometry.Envelope2D();
			_env.SetCoords(2, 1, 5, 3);
			com.epl.geometry.Polyline _clippedPolyline = (com.epl.geometry.Polyline)clipOp.Execute(_poly, _env, spatialRef, null);
			NUnit.Framework.Assert.IsTrue(_clippedPolyline.IsEmpty());
			{
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
				poly.AddEnvelope(new com.epl.geometry.Envelope2D(0, 0, 100, 100), false);
				poly.AddEnvelope(new com.epl.geometry.Envelope2D(5, 5, 95, 95), true);
				com.epl.geometry.Polygon clippedPoly = (com.epl.geometry.Polygon)clipOp.Execute(poly, new com.epl.geometry.Envelope2D(-10, -10, 110, 50), spatialRef, null);
				NUnit.Framework.Assert.IsTrue(clippedPoly.GetPathCount() == 1);
				NUnit.Framework.Assert.IsTrue(clippedPoly.GetPointCount() == 8);
			}
		}

		internal static com.epl.geometry.Polygon MakePolygon()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(10, 10);
			poly.LineTo(20, 0);
			return poly;
		}

		internal static com.epl.geometry.Polyline MakePolyline()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(0, 0);
			poly.LineTo(10, 10);
			poly.LineTo(20, 0);
			return poly;
		}

		[NUnit.Framework.Test]
		public static void TestGetXCorrectCR185697()
		{
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorClip clipOp = (com.epl.geometry.OperatorClip)engine.GetOperator(com.epl.geometry.Operator.Type.Clip);
			com.epl.geometry.Polyline polylineCR = MakePolylineCR();
			com.epl.geometry.SimpleGeometryCursor polylineCursCR = new com.epl.geometry.SimpleGeometryCursor(polylineCR);
			com.epl.geometry.SpatialReference gcsWGS84 = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.Envelope2D envelopeCR = new com.epl.geometry.Envelope2D();
			envelopeCR.xmin = -180;
			envelopeCR.xmax = 180;
			envelopeCR.ymin = -90;
			envelopeCR.ymax = 90;
			// CR
			com.epl.geometry.Polyline clippedPolylineCR = (com.epl.geometry.Polyline)clipOp.Execute(polylineCR, envelopeCR, gcsWGS84, null);
			com.epl.geometry.Point pointResult = new com.epl.geometry.Point();
			clippedPolylineCR.GetPointByVal(0, pointResult);
			NUnit.Framework.Assert.IsTrue(pointResult.GetX() == -180);
			clippedPolylineCR.GetPointByVal(1, pointResult);
			NUnit.Framework.Assert.IsTrue(pointResult.GetX() == -90);
			clippedPolylineCR.GetPointByVal(2, pointResult);
			NUnit.Framework.Assert.IsTrue(pointResult.GetX() == 0);
			clippedPolylineCR.GetPointByVal(3, pointResult);
			NUnit.Framework.Assert.IsTrue(pointResult.GetX() == 100);
			clippedPolylineCR.GetPointByVal(4, pointResult);
			NUnit.Framework.Assert.IsTrue(pointResult.GetX() == 170);
			clippedPolylineCR.GetPointByVal(5, pointResult);
			NUnit.Framework.Assert.IsTrue(pointResult.GetX() == 180);
		}

		[NUnit.Framework.Test]
		public static void TestArcObjectsFailureCR196492()
		{
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorClip clipOp = (com.epl.geometry.OperatorClip)engine.GetOperator(com.epl.geometry.Operator.Type.Clip);
			com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
			polygon.AddEnvelope(new com.epl.geometry.Envelope2D(0, 0, 600, 600), false);
			polygon.StartPath(30, 300);
			polygon.LineTo(20, 310);
			polygon.LineTo(10, 300);
			com.epl.geometry.SpatialReference gcsWGS84 = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.Envelope2D envelopeCR = new com.epl.geometry.Envelope2D(10, 10, 500, 500);
			com.epl.geometry.Polygon clippedPolygon = (com.epl.geometry.Polygon)clipOp.Execute(polygon, envelopeCR, gcsWGS84, null);
			NUnit.Framework.Assert.IsTrue(clippedPolygon.GetPointCount() == 7);
		}

		// ((MultiPathImpl::SPtr)clippedPolygon._GetImpl()).SaveToTextFileDbg("c:\\temp\\test_ArcObjects_failure_CR196492.txt");
		internal static com.epl.geometry.Polyline MakePolylineCR()
		{
			com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
			polyline.StartPath(-200, -90);
			polyline.LineTo(-180, -85);
			polyline.LineTo(-90, -70);
			polyline.LineTo(0, 0);
			polyline.LineTo(100, 25);
			polyline.LineTo(170, 45);
			polyline.LineTo(225, 65);
			return polyline;
		}

		internal static com.epl.geometry.MultiPoint MakeMultiPoint()
		{
			com.epl.geometry.MultiPoint mpoint = new com.epl.geometry.MultiPoint();
			com.epl.geometry.Point2D pt1 = new com.epl.geometry.Point2D();
			pt1.x = 10;
			pt1.y = 10;
			com.epl.geometry.Point2D pt2 = new com.epl.geometry.Point2D();
			pt2.x = 15;
			pt2.y = 10;
			com.epl.geometry.Point2D pt3 = new com.epl.geometry.Point2D();
			pt3.x = 10;
			pt3.y = 20;
			mpoint.Add(pt1.x, pt1.y);
			mpoint.Add(pt2.x, pt2.y);
			mpoint.Add(pt3.x, pt3.y);
			return mpoint;
		}

		internal static com.epl.geometry.Point MakePoint()
		{
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			pt.SetCoords(10, 20);
			point.SetXY(pt);
			return point;
		}

		[NUnit.Framework.Test]
		public static void TestClipOfCoinciding()
		{
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorClip clipOp = (com.epl.geometry.OperatorClip)engine.GetOperator(com.epl.geometry.Operator.Type.Clip);
			com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
			com.epl.geometry.Envelope2D envelopeCR = new com.epl.geometry.Envelope2D();
			envelopeCR.xmin = -180;
			envelopeCR.xmax = 180;
			envelopeCR.ymin = -90;
			envelopeCR.ymax = 90;
			polygon.AddEnvelope(envelopeCR, false);
			com.epl.geometry.SpatialReference gcsWGS84 = com.epl.geometry.SpatialReference.Create(4326);
			// CR
			com.epl.geometry.Polygon clippedPolygon = (com.epl.geometry.Polygon)clipOp.Execute(polygon, envelopeCR, gcsWGS84, null);
			NUnit.Framework.Assert.IsTrue(clippedPolygon.GetPathCount() == 1);
			NUnit.Framework.Assert.IsTrue(clippedPolygon.GetPointCount() == 4);
			com.epl.geometry.OperatorDensifyByLength densifyOp = (com.epl.geometry.OperatorDensifyByLength)engine.GetOperator(com.epl.geometry.Operator.Type.DensifyByLength);
			polygon.SetEmpty();
			polygon.AddEnvelope(envelopeCR, false);
			polygon = (com.epl.geometry.Polygon)densifyOp.Execute(polygon, 1, null);
			int pc = polygon.GetPointCount();
			int pathc = polygon.GetPathCount();
			NUnit.Framework.Assert.IsTrue(pc == 1080);
			NUnit.Framework.Assert.IsTrue(pathc == 1);
			clippedPolygon = (com.epl.geometry.Polygon)clipOp.Execute(polygon, envelopeCR, gcsWGS84, null);
			int _pathc = clippedPolygon.GetPathCount();
			int _pc = clippedPolygon.GetPointCount();
			NUnit.Framework.Assert.IsTrue(_pathc == 1);
			NUnit.Framework.Assert.IsTrue(_pc == pc);
		}

		[NUnit.Framework.Test]
		public static void TestClipAttributes()
		{
			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorClip clipOp = (com.epl.geometry.OperatorClip)engine.GetOperator(com.epl.geometry.Operator.Type.Clip);
			{
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				polygon.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
				polygon.StartPath(0, 0);
				polygon.LineTo(30, 30);
				polygon.LineTo(60, 0);
				polygon.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 0, 0);
				polygon.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, 60);
				polygon.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 2, 0, 120);
				com.epl.geometry.Envelope2D clipper = new com.epl.geometry.Envelope2D();
				clipper.SetCoords(10, 0, 50, 20);
				com.epl.geometry.Polygon clippedPolygon = (com.epl.geometry.Polygon)clipOp.Execute(polygon, clipper, com.epl.geometry.SpatialReference.Create(4326), null);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0) == 100);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == 19.999999999999996);
				// 20.0
				NUnit.Framework.Assert.IsTrue(clippedPolygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0) == 20);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 3, 0) == 40);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 4, 0) == 80);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 5, 0) == 100);
			}
			{
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				polygon.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
				polygon.StartPath(0, 0);
				polygon.LineTo(0, 40);
				polygon.LineTo(20, 40);
				polygon.LineTo(20, 0);
				polygon.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 0, 0);
				polygon.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, 60);
				polygon.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 2, 0, 120);
				polygon.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 3, 0, 180);
				com.epl.geometry.Envelope2D clipper = new com.epl.geometry.Envelope2D();
				clipper.SetCoords(0, 10, 20, 20);
				com.epl.geometry.Polygon clippedPolygon = (com.epl.geometry.Polygon)clipOp.Execute(polygon, clipper, com.epl.geometry.SpatialReference.Create(4326), null);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0) == 15);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == 30);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0) == 150);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 3, 0) == 165);
			}
		}

		[NUnit.Framework.Test]
		public static void TestClipIssue258243()
		{
			com.epl.geometry.Polygon poly1 = new com.epl.geometry.Polygon();
			poly1.StartPath(21.476191371901479, 41.267022001907215);
			poly1.LineTo(59.669186665158051, 36.62700518555863);
			poly1.LineTo(20.498578117352313, 30.363180148246094);
			poly1.LineTo(18.342565836615044, 46.303295352085627);
			poly1.LineTo(17.869569458621626, 23.886816966894159);
			poly1.LineTo(19.835465558090434, 20);
			poly1.LineTo(18.83911285048551, 43.515995498114791);
			poly1.LineTo(20.864485260298004, 20.235921201027757);
			poly1.LineTo(18.976127544787012, 20);
			poly1.LineTo(34.290201277718218, 61.801369014954794);
			poly1.LineTo(20.734727419368866, 20);
			poly1.LineTo(18.545865698148113, 20);
			poly1.LineTo(19.730260558565515, 20);
			poly1.LineTo(19.924806216827005, 23.780315893949187);
			poly1.LineTo(21.675168105421452, 36.699924873001258);
			poly1.LineTo(22.500527828912158, 43.703424859922983);
			poly1.LineTo(42.009527116514818, 36.995486982256089);
			poly1.LineTo(24.469729873835782, 58.365871758247039);
			poly1.LineTo(24.573736036545878, 36.268390409195824);
			poly1.LineTo(22.726502169802746, 20);
			poly1.LineTo(23.925834885228145, 20);
			poly1.LineTo(25.495346880936729, 20);
			poly1.LineTo(23.320941499288317, 20);
			poly1.LineTo(24.05655665646276, 28.659578774758632);
			poly1.LineTo(23.205940789341135, 38.491506888710504);
			poly1.LineTo(21.472847203385509, 53.057228182018044);
			poly1.LineTo(25.04257681654104, 20);
			poly1.LineTo(25.880572351149542, 25.16102863979474);
			poly1.LineTo(26.756283333879658, 20);
			poly1.LineTo(21.476191371901479, 41.267022001907215);
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
			env.SetCoords(24.269517325186033, 19.999998900000001, 57.305574253225409, 61.801370114954793);
			try
			{
				com.epl.geometry.Geometry output_geom = com.epl.geometry.OperatorClip.Local().Execute(poly1, env, com.epl.geometry.SpatialReference.Create(4326), null);
				com.epl.geometry.Envelope envPoly = new com.epl.geometry.Envelope();
				poly1.QueryEnvelope(envPoly);
				com.epl.geometry.Envelope e = new com.epl.geometry.Envelope(env);
				e.Intersect(envPoly);
				com.epl.geometry.Envelope clippedEnv = new com.epl.geometry.Envelope();
				output_geom.QueryEnvelope(clippedEnv);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(clippedEnv.GetXMin() - e.GetXMin()) < 1e-10 && System.Math.Abs(clippedEnv.GetYMin() - e.GetYMin()) < 1e-10 && System.Math.Abs(clippedEnv.GetXMax() - e.GetXMax()) < 1e-10 && System.Math.Abs(clippedEnv.GetYMax() - e.GetYMax()) < 1e-10);
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false);
			}
		}
	}
}
