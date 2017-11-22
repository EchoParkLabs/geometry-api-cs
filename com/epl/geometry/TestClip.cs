

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestClip
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
		public static void testClipGeometries()
		{
			// RandomTest();
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorClip clipOp = (com.esri.core.geometry.OperatorClip
				)engine.getOperator(com.esri.core.geometry.Operator.Type.Clip);
			com.esri.core.geometry.Polygon polygon = makePolygon();
			com.esri.core.geometry.SimpleGeometryCursor polygonCurs = new com.esri.core.geometry.SimpleGeometryCursor
				(polygon);
			com.esri.core.geometry.Polyline polyline = makePolyline();
			com.esri.core.geometry.SimpleGeometryCursor polylineCurs = new com.esri.core.geometry.SimpleGeometryCursor
				(polyline);
			com.esri.core.geometry.MultiPoint multipoint = makeMultiPoint();
			com.esri.core.geometry.SimpleGeometryCursor multipointCurs = new com.esri.core.geometry.SimpleGeometryCursor
				(multipoint);
			com.esri.core.geometry.Point point = makePoint();
			com.esri.core.geometry.SimpleGeometryCursor pointCurs = new com.esri.core.geometry.SimpleGeometryCursor
				(point);
			com.esri.core.geometry.SpatialReference spatialRef = com.esri.core.geometry.SpatialReference
				.create(3857);
			com.esri.core.geometry.Envelope2D envelope = new com.esri.core.geometry.Envelope2D
				();
			envelope.xmin = 0;
			envelope.xmax = 20;
			envelope.ymin = 5;
			envelope.ymax = 15;
			// Cursor implementation
			com.esri.core.geometry.GeometryCursor clipPolygonCurs = clipOp.execute(polygonCurs
				, envelope, spatialRef, null);
			com.esri.core.geometry.Polygon clippedPolygon = (com.esri.core.geometry.Polygon)clipPolygonCurs
				.next();
			double area = clippedPolygon.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - 25) < 0.00001);
			// Single Geometry implementation
			clippedPolygon = (com.esri.core.geometry.Polygon)clipOp.execute(polygon, envelope
				, spatialRef, null);
			area = clippedPolygon.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - 25) < 0.00001);
			// Cursor implementation
			com.esri.core.geometry.GeometryCursor clipPolylineCurs = clipOp.execute(polylineCurs
				, envelope, spatialRef, null);
			com.esri.core.geometry.Polyline clippedPolyline = (com.esri.core.geometry.Polyline
				)clipPolylineCurs.next();
			double length = clippedPolyline.calculateLength2D();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(length - 10 * System.Math.sqrt(2.0)
				) < 1e-10);
			// Single Geometry implementation
			clippedPolyline = (com.esri.core.geometry.Polyline)clipOp.execute(polyline, envelope
				, spatialRef, null);
			length = clippedPolyline.calculateLength2D();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(length - 10 * System.Math.sqrt(2.0)
				) < 1e-10);
			// Cursor implementation
			com.esri.core.geometry.GeometryCursor clipMulti_pointCurs = clipOp.execute(multipointCurs
				, envelope, spatialRef, null);
			com.esri.core.geometry.MultiPoint clipped_multi_point = (com.esri.core.geometry.MultiPoint
				)clipMulti_pointCurs.next();
			int pointCount = clipped_multi_point.getPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 2);
			// Cursor implementation
			com.esri.core.geometry.GeometryCursor clipPointCurs = clipOp.execute(pointCurs, envelope
				, spatialRef, null);
			com.esri.core.geometry.Point clippedPoint = (com.esri.core.geometry.Point)clipPointCurs
				.next();
			NUnit.Framework.Assert.IsTrue(clippedPoint != null);
			// RandomTest();
			com.esri.core.geometry.Polyline _poly = new com.esri.core.geometry.Polyline();
			_poly.startPath(2, 2);
			_poly.lineTo(0, 0);
			com.esri.core.geometry.Envelope2D _env = new com.esri.core.geometry.Envelope2D();
			_env.setCoords(2, 1, 5, 3);
			com.esri.core.geometry.Polyline _clippedPolyline = (com.esri.core.geometry.Polyline
				)clipOp.execute(_poly, _env, spatialRef, null);
			NUnit.Framework.Assert.IsTrue(_clippedPolyline.isEmpty());
			{
				com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
				poly.addEnvelope(new com.esri.core.geometry.Envelope2D(0, 0, 100, 100), false);
				poly.addEnvelope(new com.esri.core.geometry.Envelope2D(5, 5, 95, 95), true);
				com.esri.core.geometry.Polygon clippedPoly = (com.esri.core.geometry.Polygon)clipOp
					.execute(poly, new com.esri.core.geometry.Envelope2D(-10, -10, 110, 50), spatialRef
					, null);
				NUnit.Framework.Assert.IsTrue(clippedPoly.getPathCount() == 1);
				NUnit.Framework.Assert.IsTrue(clippedPoly.getPointCount() == 8);
			}
		}

		[NUnit.Framework.Test]
		public static com.esri.core.geometry.Polygon makePolygon()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(10, 10);
			poly.lineTo(20, 0);
			return poly;
		}

		[NUnit.Framework.Test]
		public static com.esri.core.geometry.Polyline makePolyline()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(0, 0);
			poly.lineTo(10, 10);
			poly.lineTo(20, 0);
			return poly;
		}

		[NUnit.Framework.Test]
		public static void testGetXCorrectCR185697()
		{
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorClip clipOp = (com.esri.core.geometry.OperatorClip
				)engine.getOperator(com.esri.core.geometry.Operator.Type.Clip);
			com.esri.core.geometry.Polyline polylineCR = makePolylineCR();
			com.esri.core.geometry.SimpleGeometryCursor polylineCursCR = new com.esri.core.geometry.SimpleGeometryCursor
				(polylineCR);
			com.esri.core.geometry.SpatialReference gcsWGS84 = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Envelope2D envelopeCR = new com.esri.core.geometry.Envelope2D
				();
			envelopeCR.xmin = -180;
			envelopeCR.xmax = 180;
			envelopeCR.ymin = -90;
			envelopeCR.ymax = 90;
			// CR
			com.esri.core.geometry.Polyline clippedPolylineCR = (com.esri.core.geometry.Polyline
				)clipOp.execute(polylineCR, envelopeCR, gcsWGS84, null);
			com.esri.core.geometry.Point pointResult = new com.esri.core.geometry.Point();
			clippedPolylineCR.getPointByVal(0, pointResult);
			NUnit.Framework.Assert.IsTrue(pointResult.getX() == -180);
			clippedPolylineCR.getPointByVal(1, pointResult);
			NUnit.Framework.Assert.IsTrue(pointResult.getX() == -90);
			clippedPolylineCR.getPointByVal(2, pointResult);
			NUnit.Framework.Assert.IsTrue(pointResult.getX() == 0);
			clippedPolylineCR.getPointByVal(3, pointResult);
			NUnit.Framework.Assert.IsTrue(pointResult.getX() == 100);
			clippedPolylineCR.getPointByVal(4, pointResult);
			NUnit.Framework.Assert.IsTrue(pointResult.getX() == 170);
			clippedPolylineCR.getPointByVal(5, pointResult);
			NUnit.Framework.Assert.IsTrue(pointResult.getX() == 180);
		}

		[NUnit.Framework.Test]
		public static void testArcObjectsFailureCR196492()
		{
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorClip clipOp = (com.esri.core.geometry.OperatorClip
				)engine.getOperator(com.esri.core.geometry.Operator.Type.Clip);
			com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
			polygon.addEnvelope(new com.esri.core.geometry.Envelope2D(0, 0, 600, 600), false);
			polygon.startPath(30, 300);
			polygon.lineTo(20, 310);
			polygon.lineTo(10, 300);
			com.esri.core.geometry.SpatialReference gcsWGS84 = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Envelope2D envelopeCR = new com.esri.core.geometry.Envelope2D
				(10, 10, 500, 500);
			com.esri.core.geometry.Polygon clippedPolygon = (com.esri.core.geometry.Polygon)clipOp
				.execute(polygon, envelopeCR, gcsWGS84, null);
			NUnit.Framework.Assert.IsTrue(clippedPolygon.getPointCount() == 7);
		}

		// ((MultiPathImpl::SPtr)clippedPolygon._GetImpl()).SaveToTextFileDbg("c:\\temp\\test_ArcObjects_failure_CR196492.txt");
		[NUnit.Framework.Test]
		public static com.esri.core.geometry.Polyline makePolylineCR()
		{
			com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
			polyline.startPath(-200, -90);
			polyline.lineTo(-180, -85);
			polyline.lineTo(-90, -70);
			polyline.lineTo(0, 0);
			polyline.lineTo(100, 25);
			polyline.lineTo(170, 45);
			polyline.lineTo(225, 65);
			return polyline;
		}

		[NUnit.Framework.Test]
		public static com.esri.core.geometry.MultiPoint makeMultiPoint()
		{
			com.esri.core.geometry.MultiPoint mpoint = new com.esri.core.geometry.MultiPoint(
				);
			com.esri.core.geometry.Point2D pt1 = new com.esri.core.geometry.Point2D();
			pt1.x = 10;
			pt1.y = 10;
			com.esri.core.geometry.Point2D pt2 = new com.esri.core.geometry.Point2D();
			pt2.x = 15;
			pt2.y = 10;
			com.esri.core.geometry.Point2D pt3 = new com.esri.core.geometry.Point2D();
			pt3.x = 10;
			pt3.y = 20;
			mpoint.add(pt1.x, pt1.y);
			mpoint.add(pt2.x, pt2.y);
			mpoint.add(pt3.x, pt3.y);
			return mpoint;
		}

		[NUnit.Framework.Test]
		public static com.esri.core.geometry.Point makePoint()
		{
			com.esri.core.geometry.Point point = new com.esri.core.geometry.Point();
			com.esri.core.geometry.Point2D pt = new com.esri.core.geometry.Point2D();
			pt.setCoords(10, 20);
			point.setXY(pt);
			return point;
		}

		[NUnit.Framework.Test]
		public static void testClipOfCoinciding()
		{
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorClip clipOp = (com.esri.core.geometry.OperatorClip
				)engine.getOperator(com.esri.core.geometry.Operator.Type.Clip);
			com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.Envelope2D envelopeCR = new com.esri.core.geometry.Envelope2D
				();
			envelopeCR.xmin = -180;
			envelopeCR.xmax = 180;
			envelopeCR.ymin = -90;
			envelopeCR.ymax = 90;
			polygon.addEnvelope(envelopeCR, false);
			com.esri.core.geometry.SpatialReference gcsWGS84 = com.esri.core.geometry.SpatialReference
				.create(4326);
			// CR
			com.esri.core.geometry.Polygon clippedPolygon = (com.esri.core.geometry.Polygon)clipOp
				.execute(polygon, envelopeCR, gcsWGS84, null);
			NUnit.Framework.Assert.IsTrue(clippedPolygon.getPathCount() == 1);
			NUnit.Framework.Assert.IsTrue(clippedPolygon.getPointCount() == 4);
			com.esri.core.geometry.OperatorDensifyByLength densifyOp = (com.esri.core.geometry.OperatorDensifyByLength
				)engine.getOperator(com.esri.core.geometry.Operator.Type.DensifyByLength);
			polygon.setEmpty();
			polygon.addEnvelope(envelopeCR, false);
			polygon = (com.esri.core.geometry.Polygon)densifyOp.execute(polygon, 1, null);
			int pc = polygon.getPointCount();
			int pathc = polygon.getPathCount();
			NUnit.Framework.Assert.IsTrue(pc == 1080);
			NUnit.Framework.Assert.IsTrue(pathc == 1);
			clippedPolygon = (com.esri.core.geometry.Polygon)clipOp.execute(polygon, envelopeCR
				, gcsWGS84, null);
			int _pathc = clippedPolygon.getPathCount();
			int _pc = clippedPolygon.getPointCount();
			NUnit.Framework.Assert.IsTrue(_pathc == 1);
			NUnit.Framework.Assert.IsTrue(_pc == pc);
		}

		[NUnit.Framework.Test]
		public static void testClipAttributes()
		{
			com.esri.core.geometry.OperatorFactoryLocal engine = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorClip clipOp = (com.esri.core.geometry.OperatorClip
				)engine.getOperator(com.esri.core.geometry.Operator.Type.Clip);
			{
				com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
				polygon.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
				polygon.startPath(0, 0);
				polygon.lineTo(30, 30);
				polygon.lineTo(60, 0);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 0, 
					0);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, 
					60);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 2, 0, 
					120);
				com.esri.core.geometry.Envelope2D clipper = new com.esri.core.geometry.Envelope2D
					();
				clipper.setCoords(10, 0, 50, 20);
				com.esri.core.geometry.Polygon clippedPolygon = (com.esri.core.geometry.Polygon)clipOp
					.execute(polygon, clipper, com.esri.core.geometry.SpatialReference.create(4326), 
					null);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 0, 0) == 100);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 1, 0) == 19.999999999999996);
				// 20.0
				NUnit.Framework.Assert.IsTrue(clippedPolygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 2, 0) == 20);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 3, 0) == 40);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 4, 0) == 80);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 5, 0) == 100);
			}
			{
				com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
				polygon.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
				polygon.startPath(0, 0);
				polygon.lineTo(0, 40);
				polygon.lineTo(20, 40);
				polygon.lineTo(20, 0);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 0, 
					0);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, 
					60);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 2, 0, 
					120);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 3, 0, 
					180);
				com.esri.core.geometry.Envelope2D clipper = new com.esri.core.geometry.Envelope2D
					();
				clipper.setCoords(0, 10, 20, 20);
				com.esri.core.geometry.Polygon clippedPolygon = (com.esri.core.geometry.Polygon)clipOp
					.execute(polygon, clipper, com.esri.core.geometry.SpatialReference.create(4326), 
					null);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 0, 0) == 15);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 1, 0) == 30);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 2, 0) == 150);
				NUnit.Framework.Assert.IsTrue(clippedPolygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 3, 0) == 165);
			}
		}

		[NUnit.Framework.Test]
		public static void testClipIssue258243()
		{
			com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
			poly1.startPath(21.476191371901479, 41.267022001907215);
			poly1.lineTo(59.669186665158051, 36.62700518555863);
			poly1.lineTo(20.498578117352313, 30.363180148246094);
			poly1.lineTo(18.342565836615044, 46.303295352085627);
			poly1.lineTo(17.869569458621626, 23.886816966894159);
			poly1.lineTo(19.835465558090434, 20);
			poly1.lineTo(18.83911285048551, 43.515995498114791);
			poly1.lineTo(20.864485260298004, 20.235921201027757);
			poly1.lineTo(18.976127544787012, 20);
			poly1.lineTo(34.290201277718218, 61.801369014954794);
			poly1.lineTo(20.734727419368866, 20);
			poly1.lineTo(18.545865698148113, 20);
			poly1.lineTo(19.730260558565515, 20);
			poly1.lineTo(19.924806216827005, 23.780315893949187);
			poly1.lineTo(21.675168105421452, 36.699924873001258);
			poly1.lineTo(22.500527828912158, 43.703424859922983);
			poly1.lineTo(42.009527116514818, 36.995486982256089);
			poly1.lineTo(24.469729873835782, 58.365871758247039);
			poly1.lineTo(24.573736036545878, 36.268390409195824);
			poly1.lineTo(22.726502169802746, 20);
			poly1.lineTo(23.925834885228145, 20);
			poly1.lineTo(25.495346880936729, 20);
			poly1.lineTo(23.320941499288317, 20);
			poly1.lineTo(24.05655665646276, 28.659578774758632);
			poly1.lineTo(23.205940789341135, 38.491506888710504);
			poly1.lineTo(21.472847203385509, 53.057228182018044);
			poly1.lineTo(25.04257681654104, 20);
			poly1.lineTo(25.880572351149542, 25.16102863979474);
			poly1.lineTo(26.756283333879658, 20);
			poly1.lineTo(21.476191371901479, 41.267022001907215);
			com.esri.core.geometry.Envelope2D env = new com.esri.core.geometry.Envelope2D();
			env.setCoords(24.269517325186033, 19.999998900000001, 57.305574253225409, 61.801370114954793
				);
			try
			{
				com.esri.core.geometry.Geometry output_geom = com.esri.core.geometry.OperatorClip
					.local().execute(poly1, env, com.esri.core.geometry.SpatialReference.create(4326
					), null);
				com.esri.core.geometry.Envelope envPoly = new com.esri.core.geometry.Envelope();
				poly1.queryEnvelope(envPoly);
				com.esri.core.geometry.Envelope e = new com.esri.core.geometry.Envelope(env);
				e.intersect(envPoly);
				com.esri.core.geometry.Envelope clippedEnv = new com.esri.core.geometry.Envelope(
					);
				output_geom.queryEnvelope(clippedEnv);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(clippedEnv.getXMin() - e.getXMin())
					 < 1e-10 && System.Math.abs(clippedEnv.getYMin() - e.getYMin()) < 1e-10 && System.Math
					.abs(clippedEnv.getXMax() - e.getXMax()) < 1e-10 && System.Math.abs(clippedEnv.getYMax
					() - e.getYMax()) < 1e-10);
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false);
			}
		}
	}
}
