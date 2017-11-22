

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestIntersection
	{
		internal static com.esri.core.geometry.OperatorFactoryLocal projEnv = com.esri.core.geometry.OperatorFactoryLocal
			.getInstance();

		internal static int codeIn = 26910;

		internal static int codeOut = 32610;

		internal static com.esri.core.geometry.SpatialReference inputSR;

		internal static com.esri.core.geometry.SpatialReference outputSR;

		//import java.util.Random;
		// NAD_1983_UTM_Zone_10N : GCS 6269
		// WGS_1984_UTM_Zone_10N; : GCS 4326
		/// <exception cref="System.Exception"/>
		protected override void setUp()
		{
			base.setUp();
			projEnv = com.esri.core.geometry.OperatorFactoryLocal.getInstance();
			inputSR = com.esri.core.geometry.SpatialReference.create(codeIn);
			outputSR = com.esri.core.geometry.SpatialReference.create(codeOut);
		}

		/// <exception cref="System.Exception"/>
		protected override void tearDown()
		{
			base.tearDown();
		}

		[NUnit.Framework.Test]
		public virtual void testIntersection1()
		{
			// OperatorFactoryLocal projEnv = OperatorFactoryLocal.getInstance();
			// int codeIn = 26910;//NAD_1983_UTM_Zone_10N : GCS 6269
			// int codeOut = 32610;//WGS_1984_UTM_Zone_10N; : GCS 4326
			// int codeIn = SpatialReference::PCS_WGS_1984_UTM_10N;
			// int codeOut = SpatialReference::PCS_WORLD_MOLLWEIDE;
			// int codeOut = 102100;
			inputSR = com.esri.core.geometry.SpatialReference.create(codeIn);
			NUnit.Framework.Assert.IsTrue(inputSR.getID() == codeIn);
			outputSR = com.esri.core.geometry.SpatialReference.create(codeOut);
			NUnit.Framework.Assert.IsTrue(outputSR.getID() == codeOut);
			com.esri.core.geometry.OperatorIntersection operatorIntersection = (com.esri.core.geometry.OperatorIntersection
				)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
			com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.Envelope env1 = new com.esri.core.geometry.Envelope(855277
				, 3892059, 855277 + 100, 3892059 + 100);
			// Envelope env1 = new Envelope(-1000000, -1000000, 1000000, 1000000);
			// env1.SetCoords(-8552770, -3892059, 855277 + 100, 3892059 + 100);
			poly1.addEnvelope(env1, false);
			com.esri.core.geometry.Polygon poly2 = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.Envelope env2 = new com.esri.core.geometry.Envelope(855277
				 + 1, 3892059 + 1, 855277 + 30, 3892059 + 20);
			poly2.addEnvelope(env2, false);
			com.esri.core.geometry.GeometryCursor cursor1 = new com.esri.core.geometry.SimpleGeometryCursor
				(poly1);
			com.esri.core.geometry.GeometryCursor cursor2 = new com.esri.core.geometry.SimpleGeometryCursor
				(poly2);
			com.esri.core.geometry.GeometryCursor outputGeoms = operatorIntersection.execute(
				cursor1, cursor2, inputSR, null);
			com.esri.core.geometry.Geometry geomr = outputGeoms.next();
			NUnit.Framework.Assert.IsNotNull(geomr);
			NUnit.Framework.Assert.IsTrue(geomr.getType() == com.esri.core.geometry.Geometry.Type
				.Polygon);
			com.esri.core.geometry.Polygon geom = (com.esri.core.geometry.Polygon)geomr;
			NUnit.Framework.Assert.IsTrue(geom.getPointCount() == 4);
			com.esri.core.geometry.Point[] points = com.esri.core.geometry.TestCommonMethods.
				pointsFromMultiPath(geom);
			// SPtrOfArrayOf(Point2D)
			// pts =
			// geom.get.getCoordinates2D();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(points[0].getX() - 855278.000000000
				) < 1e-7);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(points[0].getY() - 3892060.0000000000
				) < 1e-7);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(points[2].getX() - 855307.00000000093
				) < 1e-7);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(points[2].getY() - 3892079.0000000000
				) < 1e-7);
			geomr = operatorIntersection.execute(poly1, poly2, inputSR, null);
			NUnit.Framework.Assert.IsNotNull(geomr);
			NUnit.Framework.Assert.IsTrue(geomr.getType() == com.esri.core.geometry.Geometry.Type
				.Polygon);
			com.esri.core.geometry.Polygon outputGeom = (com.esri.core.geometry.Polygon)geomr;
			NUnit.Framework.Assert.IsTrue(outputGeom.getPointCount() == 4);
			points = com.esri.core.geometry.TestCommonMethods.pointsFromMultiPath(outputGeom);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(points[0].getX() - 855278.000000000
				) < 1e-7);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(points[0].getY() - 3892060.0000000000
				) < 1e-7);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(points[2].getX() - 855307.00000000093
				) < 1e-7);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(points[2].getY() - 3892079.0000000000
				) < 1e-7);
		}

		[NUnit.Framework.Test]
		public virtual void testSelfIntersecting()
		{
			// Test that we do not fail if there is
			// self-intersection
			// OperatorFactoryLocal projEnv =
			// OperatorFactoryLocal.getInstance();
			com.esri.core.geometry.OperatorIntersection operatorIntersection = (com.esri.core.geometry.OperatorIntersection
				)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.Envelope2D env1 = new com.esri.core.geometry.Envelope2D();
			env1.setCoords(0, 0, 20, 30);
			poly1.addEnvelope(env1, false);
			com.esri.core.geometry.Polygon poly2 = new com.esri.core.geometry.Polygon();
			poly2.startPath(0, 0);
			poly2.lineTo(10, 10);
			poly2.lineTo(0, 10);
			poly2.lineTo(10, 0);
			com.esri.core.geometry.Polygon res = (com.esri.core.geometry.Polygon)(operatorIntersection
				.execute(poly1, poly2, sr, null));
		}

		// Operator_equals equals =
		// (Operator_equals)projEnv.get_operator(Operator::equals);
		// assertTrue(equals.execute(res, poly2, sr, NULL) == true);
		[NUnit.Framework.Test]
		public virtual void testMultipoint()
		{
			com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.Envelope env1 = new com.esri.core.geometry.Envelope(855277
				, 3892059, 855277 + 100, 3892059 + 100);
			poly1.addEnvelope(env1, false);
			com.esri.core.geometry.MultiPoint multiPoint = new com.esri.core.geometry.MultiPoint
				();
			multiPoint.add(855277 + 10, 3892059 + 10);
			multiPoint.add(855277, 3892059);
			multiPoint.add(855277 + 100, 3892059 + 100);
			multiPoint.add(855277 + 100, 3892059 + 101);
			multiPoint.add(855277 + 101, 3892059 + 100);
			multiPoint.add(855277 + 101, 3892059 + 101);
			com.esri.core.geometry.OperatorIntersection operatorIntersection = (com.esri.core.geometry.OperatorIntersection
				)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
			com.esri.core.geometry.MultiPoint mpResult = (com.esri.core.geometry.MultiPoint)operatorIntersection
				.execute(poly1, multiPoint, inputSR, null);
			NUnit.Framework.Assert.IsTrue(mpResult.getPointCount() == 3);
			NUnit.Framework.Assert.IsTrue(mpResult.getPoint(0).getX() == 855277 + 10 && mpResult
				.getPoint(0).getY() == 3892059 + 10);
			NUnit.Framework.Assert.IsTrue(mpResult.getPoint(1).getX() == 855277 && mpResult.getPoint
				(1).getY() == 3892059);
			NUnit.Framework.Assert.IsTrue(mpResult.getPoint(2).getX() == 855277 + 100 && mpResult
				.getPoint(2).getY() == 3892059 + 100);
			// Test intersection of Polygon with Envelope (calls Clip)
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(10, 10);
			poly.lineTo(20, 0);
			env1.setXMin(0);
			env1.setXMax(20);
			env1.setYMin(5);
			env1.setYMax(15);
			com.esri.core.geometry.Envelope envelope1 = env1;
			com.esri.core.geometry.Polygon clippedPoly = (com.esri.core.geometry.Polygon)operatorIntersection
				.execute(poly, envelope1, inputSR, null);
			double area = clippedPoly.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - 25) < 0.00001);
			// Geometry res = GeometryEngine.difference(poly, envelope1, inputSR);
			com.esri.core.geometry.Envelope env2 = new com.esri.core.geometry.Envelope(855277
				 + 1, 3892059 + 1, 855277 + 30, 3892059 + 20);
			env2.setXMin(5);
			env2.setXMax(10);
			env2.setYMin(0);
			env2.setYMax(20);
			com.esri.core.geometry.Envelope envelope2 = env2;
			com.esri.core.geometry.Envelope clippedEnvelope = (com.esri.core.geometry.Envelope
				)operatorIntersection.execute(envelope1, envelope2, inputSR, null);
			area = clippedEnvelope.calculateArea2D();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(area - 50) < 0.00001);
		}

		[NUnit.Framework.Test]
		public virtual void testDifferenceOnPolyline()
		{
			com.esri.core.geometry.Polyline basePl = new com.esri.core.geometry.Polyline();
			basePl.startPath(-117, 20);
			basePl.lineTo(-130, 10);
			basePl.lineTo(-120, 50);
			com.esri.core.geometry.Polyline compPl = new com.esri.core.geometry.Polyline();
			compPl.startPath(-116, 20);
			compPl.lineTo(-131, 10);
			compPl.lineTo(-121, 50);
			// OperatorFactoryLocal projEnv = OperatorFactoryLocal.getInstance();
			com.esri.core.geometry.OperatorDifference op = (com.esri.core.geometry.OperatorDifference
				)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Difference);
			com.esri.core.geometry.Polyline diffGeom = (com.esri.core.geometry.Polyline)(op.execute
				(basePl, compPl, com.esri.core.geometry.SpatialReference.create(4326), null));
			int pc = diffGeom.getPointCount();
			NUnit.Framework.Assert.IsTrue(pc == 5);
		}

		[NUnit.Framework.Test]
		public virtual void testDifferenceOnPolyline2()
		{
			com.esri.core.geometry.Polyline basePl = new com.esri.core.geometry.Polyline();
			basePl.startPath(0, 0);
			basePl.lineTo(10, 10);
			basePl.lineTo(20, 20);
			basePl.lineTo(10, 0);
			basePl.lineTo(20, 10);
			com.esri.core.geometry.Polyline compPl = new com.esri.core.geometry.Polyline();
			compPl.startPath(5, 0);
			compPl.lineTo(5, 10);
			compPl.lineTo(0, 10);
			compPl.lineTo(7.5, 2.5);
			// Operator_factory_local::SaveJSONToTextFileDbg("c:/temp/basePl.txt",
			// *basePl, null);
			// Operator_factory_local::SaveJSONToTextFileDbg("c:/temp/compPl.txt",
			// *compPl, null);
			// OperatorFactoryLocal projEnv = OperatorFactoryLocal.getInstance();
			com.esri.core.geometry.OperatorDifference op = (com.esri.core.geometry.OperatorDifference
				)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Difference);
			com.esri.core.geometry.Polyline diffGeom = (com.esri.core.geometry.Polyline)(op.execute
				(basePl, compPl, com.esri.core.geometry.SpatialReference.create(4326), null));
			// Operator_factory_local::SaveJSONToTextFileDbg("c:/temp/diffGeom.txt",
			// *diffGeom, null);
			int pathc = diffGeom.getPathCount();
			NUnit.Framework.Assert.IsTrue(pathc == 1);
			int pc = diffGeom.getPointCount();
			NUnit.Framework.Assert.IsTrue(pc == 6);
			com.esri.core.geometry.Polyline resPl = new com.esri.core.geometry.Polyline();
			resPl.startPath(0, 0);
			resPl.lineTo(5, 5);
			resPl.lineTo(10, 10);
			resPl.lineTo(20, 20);
			resPl.lineTo(10, 0);
			resPl.lineTo(20, 10);
			// Operator_factory_local::SaveJSONToTextFileDbg("c:/temp/resPl.txt",
			// *resPl, null);
			NUnit.Framework.Assert.IsTrue(resPl.Equals(diffGeom));
		}

		[NUnit.Framework.Test]
		public virtual void testDifferencePointPolyline()
		{
			com.esri.core.geometry.Polyline basePl = new com.esri.core.geometry.Polyline();
			basePl.startPath(0, 0);
			basePl.lineTo(10, 10);
			basePl.lineTo(20, 20);
			basePl.lineTo(10, 0);
			basePl.lineTo(20, 10);
			com.esri.core.geometry.Point compPl = new com.esri.core.geometry.Point(5, 5);
			// OperatorFactoryLocal projEnv = OperatorFactoryLocal.getInstance();
			com.esri.core.geometry.OperatorDifference op = (com.esri.core.geometry.OperatorDifference
				)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Difference);
			com.esri.core.geometry.Polyline diffGeom = (com.esri.core.geometry.Polyline)(op.execute
				(basePl, compPl, com.esri.core.geometry.SpatialReference.create(4326), null));
			int pathc = diffGeom.getPathCount();
			NUnit.Framework.Assert.IsTrue(pathc == 1);
			int pc = diffGeom.getPointCount();
			NUnit.Framework.Assert.IsTrue(pc == 5);
			com.esri.core.geometry.Polyline resPl = new com.esri.core.geometry.Polyline();
			resPl.startPath(0, 0);
			resPl.lineTo(10, 10);
			resPl.lineTo(20, 20);
			resPl.lineTo(10, 0);
			resPl.lineTo(20, 10);
			NUnit.Framework.Assert.IsTrue(resPl.Equals(diffGeom));
		}

		// no change happens to the original
		// polyline
		[NUnit.Framework.Test]
		public virtual void testIntersectionPolylinePolygon()
		{
			{
				com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
				polygon.startPath(0, 0);
				polygon.lineTo(0, 10);
				polygon.lineTo(20, 10);
				polygon.lineTo(20, 0);
				polygon.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 
					3);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 3, 0, 
					3);
				polygon.interpolateAttributes(0, 0, 3);
				com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
				polyline.startPath(0, 10);
				polyline.lineTo(5, 5);
				polyline.lineTo(6, 4);
				polyline.lineTo(7, -1);
				polyline.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 
					5);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 3, 0, 
					5);
				polyline.interpolateAttributes(0, 0, 0, 3);
				// OperatorFactoryLocal projEnv =
				// OperatorFactoryLocal.getInstance();
				com.esri.core.geometry.OperatorIntersection operatorIntersection = (com.esri.core.geometry.OperatorIntersection
					)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
				com.esri.core.geometry.Geometry geom = operatorIntersection.execute(polyline, polygon
					, null, null);
				NUnit.Framework.Assert.IsTrue(!geom.isEmpty());
				com.esri.core.geometry.Polyline poly = (com.esri.core.geometry.Polyline)(geom);
				for (int i = 0; i < poly.getPointCount(); i++)
				{
					NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
						.Z, i, 0) == 5);
				}
			}
			{
				// std::shared_ptr<Operator_export_to_JSON> jsonExport =
				// (Operator_export_to_JSON>)Operator_factory_local::get_instance().get_operator(Operator::Operator_type::export_to_JSON);
				// std::string str = jsonExport.execute(0, geom, null, null);
				// OutputDebugStringA(str.c_str());
				// OutputDebugString(L"\n");
				// assertTrue(str=="{\"paths\":[[[0,10],[5,5],[6,4],[6.7999999999999998,4.4408922169635528e-016]]]}");
				com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
				polygon.startPath(0, 0);
				polygon.lineTo(0, 10);
				polygon.lineTo(20, 10);
				polygon.lineTo(20, 0);
				polygon.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 
					3);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 3, 0, 
					3);
				polygon.interpolateAttributes(0, 0, 3);
				com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
				polyline.startPath(0, 10);
				polyline.lineTo(20, 0);
				polyline.lineTo(5, 5);
				polyline.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 
					5);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 1, 0, 
					5);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 2, 0, 
					5);
				// OperatorFactoryLocal projEnv =
				// OperatorFactoryLocal.getInstance();
				com.esri.core.geometry.OperatorIntersection operatorIntersection = (com.esri.core.geometry.OperatorIntersection
					)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
				com.esri.core.geometry.Geometry geom = operatorIntersection.execute(polyline, polygon
					, null, null);
				NUnit.Framework.Assert.IsTrue(!geom.isEmpty());
				com.esri.core.geometry.Polyline poly = (com.esri.core.geometry.Polyline)(geom);
				for (int i = 0; i < poly.getPointCount(); i++)
				{
					NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
						.Z, i, 0) == 5);
				}
			}
			{
				// Operator_export_to_JSON> jsonExport =
				// (Operator_export_to_JSON>)Operator_factory_local::get_instance().get_operator(Operator::Operator_type::export_to_JSON);
				// std::string str = jsonExport.execute(0, geom, null, null);
				// OutputDebugStringA(str.c_str());
				// OutputDebugString(L"\n");
				// assertTrue(str=="{\"paths\":[[[0,10],[20,0],[5,5]]]}");
				com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
				polygon.startPath(0, 0);
				polygon.lineTo(0, 10);
				polygon.lineTo(20, 10);
				polygon.lineTo(20, 0);
				polygon.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 
					3);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 3, 0, 
					3);
				polygon.interpolateAttributes(0, 0, 3);
				com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
				polyline.startPath(0, 0);
				polyline.lineTo(0, 10);
				polyline.lineTo(20, 10);
				polyline.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 
					5);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 1, 0, 
					5);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 2, 0, 
					5);
				// OperatorFactoryLocal projEnv =
				// OperatorFactoryLocal.getInstance();
				com.esri.core.geometry.OperatorIntersection operatorIntersection = (com.esri.core.geometry.OperatorIntersection
					)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
				com.esri.core.geometry.Geometry geom = operatorIntersection.execute(polyline, polygon
					, null, null);
				NUnit.Framework.Assert.IsTrue(!geom.isEmpty());
				com.esri.core.geometry.Polyline poly = (com.esri.core.geometry.Polyline)(geom);
				for (int i = 0; i < poly.getPointCount(); i++)
				{
					NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
						.Z, i, 0) == 5);
				}
			}
			{
				// Operator_export_to_JSON> jsonExport =
				// (Operator_export_to_JSON>)Operator_factory_local::get_instance().get_operator(Operator::Operator_type::export_to_JSON);
				// std::string str = jsonExport.execute(0, geom, null, null);
				// OutputDebugStringA(str.c_str());
				// OutputDebugString(L"\n");
				// assertTrue(str=="{\"paths\":[[[0,0],[0,10],[20,10]]]}");
				com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
				polygon.startPath(0, 0);
				polygon.lineTo(0, 10);
				polygon.lineTo(20, 10);
				polygon.lineTo(20, 0);
				com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
				polyline.startPath(3, -1);
				polyline.lineTo(17, 1);
				polyline.lineTo(10, 8);
				polyline.lineTo(-1, 5);
				polyline.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 
					5);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 1, 0, 
					5);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 2, 0, 
					5);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 3, 0, 
					5);
				// OperatorFactoryLocal projEnv =
				// OperatorFactoryLocal.getInstance();
				com.esri.core.geometry.OperatorIntersection operatorIntersection = (com.esri.core.geometry.OperatorIntersection
					)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
				com.esri.core.geometry.Geometry geom = operatorIntersection.execute(polyline, polygon
					, null, null);
				NUnit.Framework.Assert.IsTrue(!geom.isEmpty());
				com.esri.core.geometry.Polyline poly = (com.esri.core.geometry.Polyline)geom;
				for (int i = 0; i < poly.getPointCount(); i++)
				{
					NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
						.Z, i, 0) == 5);
				}
			}
			{
				// Operator_export_to_JSON> jsonExport =
				// (Operator_export_to_JSON>)Operator_factory_local::get_instance().get_operator(Operator::Operator_type::export_to_JSON);
				// std::string str = jsonExport.execute(0, geom, null, null);
				// OutputDebugStringA(str.c_str());
				// OutputDebugString(L"\n");
				// assertTrue(str=="{\"paths\":[[[10,0],[17,1],[10,8],[4.7377092701401439e-024,5.2727272727272734]]]}");
				com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
				polygon.startPath(0, 0);
				polygon.lineTo(0, 10);
				polygon.lineTo(20, 10);
				polygon.lineTo(20, 0);
				com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
				polyline.startPath(0, 15);
				polyline.lineTo(3, -1);
				polyline.lineTo(17, 1);
				polyline.lineTo(10, 8);
				polyline.lineTo(-1, 5);
				polyline.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 
					5);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 4, 0, 
					5);
				polyline.interpolateAttributes(0, 0, 0, 4);
				com.esri.core.geometry.OperatorIntersection operatorIntersection = (com.esri.core.geometry.OperatorIntersection
					)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
				com.esri.core.geometry.Geometry geom = operatorIntersection.execute(polyline, polygon
					, null, null);
				NUnit.Framework.Assert.IsTrue(!geom.isEmpty());
				com.esri.core.geometry.Polyline poly = (com.esri.core.geometry.Polyline)geom;
				for (int i = 0; i < poly.getPointCount(); i++)
				{
					NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
						.Z, i, 0) == 5);
				}
			}
			{
				// Operator_export_to_JSON> jsonExport =
				// (Operator_export_to_JSON>)Operator_factory_local::get_instance().get_operator(Operator::Operator_type::export_to_JSON);
				// std::string str = jsonExport.execute(0, geom, null, null);
				// OutputDebugStringA(str.c_str());
				// OutputDebugString(L"\n");
				// assertTrue(str=="{\"paths\":[[[0.9375,10],[2.8125,9.476226333847234e-024]],[[10,0],[17,1],[10,8],[4.7377092701401439e-024,5.2727272727272734]]]}");
				com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
				polygon.startPath(0, 0);
				polygon.lineTo(0, 10);
				polygon.lineTo(20, 10);
				polygon.lineTo(20, 0);
				com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
				polyline.startPath(5, 5);
				polyline.lineTo(1, 1);
				polyline.lineTo(-1, 1);
				polyline.lineTo(-1, 10);
				polyline.lineTo(0, 10);
				polyline.lineTo(6, 6);
				polyline.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 
					5);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 5, 0, 
					5);
				polyline.interpolateAttributes(0, 0, 0, 5);
				// OperatorFactoryLocal projEnv =
				// OperatorFactoryLocal.getInstance();
				com.esri.core.geometry.OperatorIntersection operatorIntersection = (com.esri.core.geometry.OperatorIntersection
					)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
				com.esri.core.geometry.Geometry geom = operatorIntersection.execute(polyline, polygon
					, null, null);
				NUnit.Framework.Assert.IsTrue(!geom.isEmpty());
				com.esri.core.geometry.Polyline poly = (com.esri.core.geometry.Polyline)geom;
				for (int i = 0; i < poly.getPointCount(); i++)
				{
					NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
						.Z, i, 0) == 5);
				}
			}
			{
				// Operator_export_to_JSON> jsonExport =
				// (Operator_export_to_JSON>)Operator_factory_local::get_instance().get_operator(Operator::Operator_type::export_to_JSON);
				// std::string str = jsonExport.execute(0, geom, null, null);
				// OutputDebugStringA(str.c_str());
				// OutputDebugString(L"\n");
				// assertTrue(str=="{\"paths\":[[[5,5],[1,1],[4.738113166923617e-023,1]],[[0,10],[6,6]]]}");
				com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
				polygon.startPath(0, 0);
				polygon.lineTo(0, 10);
				polygon.lineTo(20, 10);
				polygon.lineTo(20, 0);
				com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
				polyline.startPath(0, 15);
				polyline.lineTo(3, -1);
				polyline.lineTo(17, 1);
				polyline.lineTo(10, 8);
				polyline.lineTo(-1, 5);
				polyline.startPath(19, 15);
				polyline.lineTo(29, 9);
				polyline.startPath(19, 15);
				polyline.lineTo(29, 9);
				polyline.startPath(5, 5);
				polyline.lineTo(1, 1);
				polyline.lineTo(-1, 1);
				polyline.lineTo(-1, 10);
				polyline.lineTo(0, 10);
				polyline.lineTo(6, 6);
				polyline.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 
					5);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 14, 0
					, 5);
				polyline.interpolateAttributes(0, 0, 3, 5);
				// OperatorFactoryLocal projEnv =
				// OperatorFactoryLocal.getInstance();
				com.esri.core.geometry.OperatorIntersection operatorIntersection = (com.esri.core.geometry.OperatorIntersection
					)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
				com.esri.core.geometry.Geometry geom = operatorIntersection.execute(polyline, polygon
					, null, null);
				NUnit.Framework.Assert.IsTrue(!geom.isEmpty());
				com.esri.core.geometry.Polyline poly = (com.esri.core.geometry.Polyline)geom;
				for (int i = 0; i < poly.getPointCount(); i++)
				{
					NUnit.Framework.Assert.IsTrue(poly.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
						.Z, i, 0) == 5);
				}
			}
		}

		// Operator_export_to_JSON> jsonExport =
		// (Operator_export_to_JSON>)Operator_factory_local::get_instance().get_operator(Operator::Operator_type::export_to_JSON);
		// std::string str = jsonExport.execute(0, geom, null, null);
		// OutputDebugStringA(str.c_str());
		// OutputDebugString(L"\n");
		// assertTrue(str=="{\"paths\":[[[0.9375,10],[2.8125,9.476226333847234e-024]],[[10,0],[17,1],[10,8],[4.7377092701401439e-024,5.2727272727272734]],[[5,5],[1,1],[4.738113166923617e-023,1]],[[0,10],[6,6]]]}");
		[NUnit.Framework.Test]
		public virtual void testMultiPointPolyline()
		{
			com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
			polyline.startPath(0, 0);
			polyline.lineTo(0, 10);
			polyline.lineTo(20, 10);
			polyline.lineTo(20, 0);
			com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
			mp.add(0, 10, 7);
			mp.add(0, 5, 7);
			mp.add(1, 5, 7);
			// OperatorFactoryLocal projEnv = OperatorFactoryLocal.getInstance();
			com.esri.core.geometry.OperatorIntersection operatorIntersection = (com.esri.core.geometry.OperatorIntersection
				)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
			com.esri.core.geometry.OperatorDifference operatorDifference = (com.esri.core.geometry.OperatorDifference
				)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Difference);
			{
				// intersect
				com.esri.core.geometry.Geometry geom = operatorIntersection.execute(polyline, mp, 
					null, null);
				com.esri.core.geometry.MultiPoint res = (com.esri.core.geometry.MultiPoint)geom;
				NUnit.Framework.Assert.IsTrue(res.getPointCount() == 2);
				com.esri.core.geometry.Point2D pt_1 = res.getXY(0);
				com.esri.core.geometry.Point2D pt_2 = res.getXY(1);
				NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Point2D.distance(pt_1, new com.esri.core.geometry.Point2D
					(0, 10)) < 1e-10 && com.esri.core.geometry.Point2D.distance(pt_2, new com.esri.core.geometry.Point2D
					(0, 5)) < 1e-10 || com.esri.core.geometry.Point2D.distance(pt_2, new com.esri.core.geometry.Point2D
					(0, 10)) < 1e-10 && com.esri.core.geometry.Point2D.distance(pt_1, new com.esri.core.geometry.Point2D
					(0, 5)) < 1e-10);
				NUnit.Framework.Assert.IsTrue(res.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.Z, 0, 0) == 7);
				NUnit.Framework.Assert.IsTrue(res.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.Z, 1, 0) == 7);
			}
			{
				// difference
				com.esri.core.geometry.Geometry geom = operatorDifference.execute(polyline, mp, null
					, null);
				// assertTrue(geom.getGeometryType() ==
				// Geometry.GeometryType.Polyline);
				com.esri.core.geometry.Polyline res = (com.esri.core.geometry.Polyline)geom;
				NUnit.Framework.Assert.IsTrue(res.getPointCount() == 4);
			}
			{
				// difference
				com.esri.core.geometry.Geometry geom = operatorDifference.execute(mp, polyline, null
					, null);
				// assertTrue(geom.getType() == Geometry.GeometryType.MultiPoint);
				com.esri.core.geometry.MultiPoint res = (com.esri.core.geometry.MultiPoint)geom;
				NUnit.Framework.Assert.IsTrue(res.getPointCount() == 1);
				com.esri.core.geometry.Point2D pt_1 = res.getXY(0);
				NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Point2D.distance(pt_1, new com.esri.core.geometry.Point2D
					(1, 5)) < 1e-10);
			}
			{
				// difference (subtract empty)
				com.esri.core.geometry.Geometry geom = operatorDifference.execute(mp, new com.esri.core.geometry.Polyline
					(), null, null);
				// assertTrue(geom.getGeometryType() ==
				// Geometry.GeometryType.MultiPoint);
				com.esri.core.geometry.MultiPoint res = (com.esri.core.geometry.MultiPoint)geom;
				NUnit.Framework.Assert.IsTrue(res.getPointCount() == 3);
				com.esri.core.geometry.Point2D pt_1 = res.getXY(0);
				NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Point2D.distance(pt_1, new com.esri.core.geometry.Point2D
					(0, 10)) < 1e-10);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testPointPolyline()
		{
			com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
			polyline.startPath(0, 0);
			polyline.lineTo(0, 10);
			polyline.lineTo(20, 10);
			polyline.lineTo(20, 0);
			com.esri.core.geometry.Point p_1 = new com.esri.core.geometry.Point(0, 5, 7);
			com.esri.core.geometry.Point p_2 = new com.esri.core.geometry.Point(0, 10, 7);
			com.esri.core.geometry.Point p3 = new com.esri.core.geometry.Point(1, 5, 7);
			// OperatorFactoryLocal projEnv = OperatorFactoryLocal.getInstance();
			com.esri.core.geometry.OperatorIntersection operatorIntersection = (com.esri.core.geometry.OperatorIntersection
				)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
			com.esri.core.geometry.OperatorDifference operatorDiff = (com.esri.core.geometry.OperatorDifference
				)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Difference);
			com.esri.core.geometry.OperatorUnion operatorUnion = (com.esri.core.geometry.OperatorUnion
				)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Union);
			com.esri.core.geometry.OperatorSymmetricDifference operatorSymDiff = (com.esri.core.geometry.OperatorSymmetricDifference
				)projEnv.getOperator(com.esri.core.geometry.Operator.Type.SymmetricDifference);
			{
				// intersect case1
				com.esri.core.geometry.Geometry geom = operatorIntersection.execute(polyline, p_1
					, null, null);
				// assertTrue(geom.getType() == Geometry::enum_point);
				com.esri.core.geometry.Point res = (com.esri.core.geometry.Point)geom;
				com.esri.core.geometry.Point2D pt_1 = res.getXY();
				NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Point2D.distance(pt_1, new com.esri.core.geometry.Point2D
					(0, 5)) < 1e-10);
				NUnit.Framework.Assert.IsTrue(res.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.Z, 0) == 7);
			}
			{
				// intersect case2
				com.esri.core.geometry.Geometry geom = operatorIntersection.execute(polyline, p_2
					, null, null);
				// assertTrue(geom.getType() == Geometry::enum_point);
				com.esri.core.geometry.Point res = (com.esri.core.geometry.Point)geom;
				com.esri.core.geometry.Point2D pt_1 = res.getXY();
				NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Point2D.distance(pt_1, new com.esri.core.geometry.Point2D
					(0, 10)) < 1e-10);
				NUnit.Framework.Assert.IsTrue(res.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.Z, 0) == 7);
			}
			{
				// intersect case3
				com.esri.core.geometry.Geometry geom = operatorIntersection.execute(polyline, p3, 
					null, null);
				// assertTrue(geom.getType() == Geometry::enum_point);
				NUnit.Framework.Assert.IsTrue(geom.isEmpty());
				NUnit.Framework.Assert.IsTrue(geom.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
					.Z));
			}
			{
				// difference case1
				com.esri.core.geometry.Geometry geom = operatorDiff.execute(polyline, p_1, null, 
					null);
				// assertTrue(geom.getType() == Geometry.GeometryType.Polyline);
				com.esri.core.geometry.Polyline res = (com.esri.core.geometry.Polyline)geom;
				NUnit.Framework.Assert.IsTrue(res.getPointCount() == 4);
			}
			{
				// difference case2
				com.esri.core.geometry.Geometry geom = operatorDiff.execute(p_1, polyline, null, 
					null);
				// assertTrue(geom.getType() == Geometry::enum_point);
				com.esri.core.geometry.Point res = (com.esri.core.geometry.Point)geom;
				NUnit.Framework.Assert.IsTrue(res.isEmpty());
			}
			{
				// difference case3
				com.esri.core.geometry.Geometry geom = operatorDiff.execute(p_2, polyline, null, 
					null);
				// assertTrue(geom.getType() == Geometry::enum_point);
				com.esri.core.geometry.Point res = (com.esri.core.geometry.Point)geom;
				NUnit.Framework.Assert.IsTrue(res.isEmpty());
			}
			{
				// difference case4
				com.esri.core.geometry.Geometry geom = operatorDiff.execute(p3, polyline, null, null
					);
				// assertTrue(geom.getType() == Geometry::enum_point);
				com.esri.core.geometry.Point res = (com.esri.core.geometry.Point)geom;
				com.esri.core.geometry.Point2D pt_1 = res.getXY();
				NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Point2D.distance(pt_1, new com.esri.core.geometry.Point2D
					(1, 5)) < 1e-10);
			}
			{
				// union case1
				com.esri.core.geometry.Geometry geom = operatorUnion.execute(p_1, polyline, null, 
					null);
				// assertTrue(geom.getType() == Geometry.GeometryType.Polyline);
				com.esri.core.geometry.Polyline res = (com.esri.core.geometry.Polyline)geom;
				NUnit.Framework.Assert.IsTrue(!res.isEmpty());
			}
			{
				// union case2
				com.esri.core.geometry.Geometry geom = operatorUnion.execute(polyline, p_1, null, 
					null);
				// assertTrue(geom.getType() == Geometry.GeometryType.Polyline);
				com.esri.core.geometry.Polyline res = (com.esri.core.geometry.Polyline)geom;
				NUnit.Framework.Assert.IsTrue(!res.isEmpty());
			}
			{
				// symmetric difference case1
				com.esri.core.geometry.Geometry geom = operatorSymDiff.execute(polyline, p_1, null
					, null);
				NUnit.Framework.Assert.IsTrue(geom.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polyline);
				com.esri.core.geometry.Polyline res = (com.esri.core.geometry.Polyline)(geom);
				NUnit.Framework.Assert.IsTrue(!res.isEmpty());
			}
			{
				// symmetric difference case2
				com.esri.core.geometry.Geometry geom = operatorSymDiff.execute(p_1, polyline, null
					, null);
				NUnit.Framework.Assert.IsTrue(geom.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polyline);
				com.esri.core.geometry.Polyline res = (com.esri.core.geometry.Polyline)(geom);
				NUnit.Framework.Assert.IsTrue(!res.isEmpty());
			}
		}

		[NUnit.Framework.Test]
		public virtual void testPolylinePolylineIntersectionExtended()
		{
			{
				// crossing intersection
				com.esri.core.geometry.Polyline basePl = new com.esri.core.geometry.Polyline();
				basePl.startPath(0, 10);
				basePl.lineTo(100, 10);
				com.esri.core.geometry.Polyline compPl = new com.esri.core.geometry.Polyline();
				compPl.startPath(50, 0);
				compPl.lineTo(50, 100);
				com.esri.core.geometry.OperatorIntersection op = (com.esri.core.geometry.OperatorIntersection
					)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
				com.esri.core.geometry.GeometryCursor result_cursor = op.execute(new com.esri.core.geometry.SimpleGeometryCursor
					(basePl), new com.esri.core.geometry.SimpleGeometryCursor(compPl), com.esri.core.geometry.SpatialReference
					.create(4326), null, 3);
				// dimension is 3, means it has to return a point and a polyline
				com.esri.core.geometry.Geometry geom1 = result_cursor.next();
				NUnit.Framework.Assert.IsTrue(geom1 != null);
				NUnit.Framework.Assert.IsTrue(geom1.getDimension() == 0);
				NUnit.Framework.Assert.IsTrue(geom1.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.MultiPoint);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.MultiPoint)geom1).getPointCount
					() == 1);
				com.esri.core.geometry.Geometry geom2 = result_cursor.next();
				NUnit.Framework.Assert.IsTrue(geom2 != null);
				NUnit.Framework.Assert.IsTrue(geom2.getDimension() == 1);
				NUnit.Framework.Assert.IsTrue(geom2.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polyline);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polyline)geom2).getPointCount
					() == 0);
				com.esri.core.geometry.Geometry geom3 = result_cursor.next();
				NUnit.Framework.Assert.IsTrue(geom3 == null);
			}
			{
				// crossing + overlapping intersection
				com.esri.core.geometry.Polyline basePl = new com.esri.core.geometry.Polyline();
				basePl.startPath(0, 10);
				basePl.lineTo(100, 10);
				com.esri.core.geometry.Polyline compPl = new com.esri.core.geometry.Polyline();
				compPl.startPath(50, 0);
				compPl.lineTo(50, 100);
				compPl.lineTo(70, 10);
				compPl.lineTo(100, 10);
				com.esri.core.geometry.OperatorIntersection op = (com.esri.core.geometry.OperatorIntersection
					)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
				com.esri.core.geometry.GeometryCursor result_cursor = op.execute(new com.esri.core.geometry.SimpleGeometryCursor
					(basePl), new com.esri.core.geometry.SimpleGeometryCursor(compPl), com.esri.core.geometry.SpatialReference
					.create(4326), null, 3);
				// dimension is 3, means it has to return a point and a polyline
				com.esri.core.geometry.Geometry geom1 = result_cursor.next();
				NUnit.Framework.Assert.IsTrue(geom1 != null);
				NUnit.Framework.Assert.IsTrue(geom1.getDimension() == 0);
				NUnit.Framework.Assert.IsTrue(geom1.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.MultiPoint);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.MultiPoint)geom1).getPointCount
					() == 1);
				com.esri.core.geometry.Geometry geom2 = result_cursor.next();
				NUnit.Framework.Assert.IsTrue(geom2 != null);
				NUnit.Framework.Assert.IsTrue(geom2.getDimension() == 1);
				NUnit.Framework.Assert.IsTrue(geom2.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polyline);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polyline)geom2).getPathCount
					() == 1);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polyline)geom2).getPointCount
					() == 2);
				com.esri.core.geometry.Geometry geom3 = result_cursor.next();
				NUnit.Framework.Assert.IsTrue(geom3 == null);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonPolygonIntersectionExtended()
		{
			{
				// crossing intersection
				com.esri.core.geometry.Polygon basePl = new com.esri.core.geometry.Polygon();
				basePl.startPath(0, 0);
				basePl.lineTo(100, 0);
				basePl.lineTo(100, 100);
				basePl.lineTo(0, 100);
				com.esri.core.geometry.Polygon compPl = new com.esri.core.geometry.Polygon();
				compPl.startPath(100, 100);
				compPl.lineTo(200, 100);
				compPl.lineTo(200, 200);
				compPl.lineTo(100, 200);
				com.esri.core.geometry.OperatorIntersection op = (com.esri.core.geometry.OperatorIntersection
					)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
				com.esri.core.geometry.GeometryCursor result_cursor = op.execute(new com.esri.core.geometry.SimpleGeometryCursor
					(basePl), new com.esri.core.geometry.SimpleGeometryCursor(compPl), com.esri.core.geometry.SpatialReference
					.create(4326), null, 7);
				com.esri.core.geometry.Geometry geom1 = result_cursor.next();
				NUnit.Framework.Assert.IsTrue(geom1 != null);
				NUnit.Framework.Assert.IsTrue(geom1.getDimension() == 0);
				NUnit.Framework.Assert.IsTrue(geom1.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.MultiPoint);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.MultiPoint)geom1).getPointCount
					() == 1);
				com.esri.core.geometry.Geometry geom2 = result_cursor.next();
				NUnit.Framework.Assert.IsTrue(geom2 != null);
				NUnit.Framework.Assert.IsTrue(geom2.getDimension() == 1);
				NUnit.Framework.Assert.IsTrue(geom2.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polyline);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polyline)geom2).getPointCount
					() == 0);
				com.esri.core.geometry.Geometry geom3 = result_cursor.next();
				NUnit.Framework.Assert.IsTrue(geom3 != null);
				NUnit.Framework.Assert.IsTrue(geom3.getDimension() == 2);
				NUnit.Framework.Assert.IsTrue(geom3.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polygon)geom3).getPointCount
					() == 0);
				com.esri.core.geometry.Geometry geom4 = result_cursor.next();
				NUnit.Framework.Assert.IsTrue(geom4 == null);
			}
			{
				// crossing + overlapping intersection
				com.esri.core.geometry.Polygon basePl = new com.esri.core.geometry.Polygon();
				basePl.startPath(0, 0);
				basePl.lineTo(100, 0);
				basePl.lineTo(100, 100);
				basePl.lineTo(0, 100);
				com.esri.core.geometry.Polygon compPl = new com.esri.core.geometry.Polygon();
				compPl.startPath(100, 100);
				compPl.lineTo(200, 100);
				compPl.lineTo(200, 200);
				compPl.lineTo(100, 200);
				compPl.startPath(100, 20);
				compPl.lineTo(200, 20);
				compPl.lineTo(200, 40);
				compPl.lineTo(100, 40);
				compPl.startPath(-10, -10);
				compPl.lineTo(-10, 10);
				compPl.lineTo(10, 10);
				compPl.lineTo(10, -10);
				com.esri.core.geometry.OperatorIntersection op = (com.esri.core.geometry.OperatorIntersection
					)projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersection);
				com.esri.core.geometry.GeometryCursor result_cursor = op.execute(new com.esri.core.geometry.SimpleGeometryCursor
					(basePl), new com.esri.core.geometry.SimpleGeometryCursor(compPl), com.esri.core.geometry.SpatialReference
					.create(4326), null, 7);
				// dimension is 3, means it has to return a point and a polyline
				com.esri.core.geometry.Geometry geom1 = result_cursor.next();
				NUnit.Framework.Assert.IsTrue(geom1 != null);
				NUnit.Framework.Assert.IsTrue(geom1.getDimension() == 0);
				NUnit.Framework.Assert.IsTrue(geom1.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.MultiPoint);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.MultiPoint)geom1).getPointCount
					() == 1);
				com.esri.core.geometry.Geometry geom2 = result_cursor.next();
				NUnit.Framework.Assert.IsTrue(geom2 != null);
				NUnit.Framework.Assert.IsTrue(geom2.getDimension() == 1);
				NUnit.Framework.Assert.IsTrue(geom2.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polyline);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polyline)geom2).getPathCount
					() == 1);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polyline)geom2).getPointCount
					() == 2);
				com.esri.core.geometry.Geometry geom3 = result_cursor.next();
				NUnit.Framework.Assert.IsTrue(geom3 != null);
				NUnit.Framework.Assert.IsTrue(geom3.getDimension() == 2);
				NUnit.Framework.Assert.IsTrue(geom3.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polygon)geom3).getPathCount
					() == 1);
				NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polygon)geom3).getPointCount
					() == 4);
				com.esri.core.geometry.Geometry geom4 = result_cursor.next();
				NUnit.Framework.Assert.IsTrue(geom4 == null);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testFromProjection()
		{
			com.esri.core.geometry.MultiPoint multiPointInitial = new com.esri.core.geometry.MultiPoint
				();
			multiPointInitial.add(-20037508.342789244, 3360107.7777777780);
			multiPointInitial.add(-18924313.434856508, 3360107.7777777780);
			multiPointInitial.add(-18924313.434856508, -3360107.7777777780);
			multiPointInitial.add(-20037508.342789244, -3360107.7777777780);
			com.esri.core.geometry.Geometry geom1 = ((com.esri.core.geometry.MultiPoint)multiPointInitial
				);
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			com.esri.core.geometry.Envelope2D env = new com.esri.core.geometry.Envelope2D();
			env.setCoords(-20037508.342788246, -30240971.958386172, 20037508.342788246, 30240971.958386205
				);
			/* xmin */
			/* ymin */
			/* xmax */
			/* ymax */
			// /*xmin*/ -20037508.342788246
			// /*ymin*/ -30240971.958386172
			// /*xmax*/ 20037508.342788246
			// /*ymax*/ 30240971.958386205
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(env.xmin, env.ymin);
			poly.lineTo(env.xmin, env.ymax);
			poly.lineTo(env.xmax, env.ymax);
			poly.lineTo(env.xmax, env.ymin);
			com.esri.core.geometry.Geometry geom2 = new com.esri.core.geometry.Envelope(env);
			// Geometry geom2 = poly;
			com.esri.core.geometry.OperatorIntersection operatorIntersection = (com.esri.core.geometry.OperatorIntersection
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Intersection);
			com.esri.core.geometry.MultiPoint multiPointOut = (com.esri.core.geometry.MultiPoint
				)(operatorIntersection.execute(geom1, geom2, sr, null));
			NUnit.Framework.Assert.IsTrue(multiPointOut.getCoordinates2D().Length == 2);
			NUnit.Framework.Assert.IsTrue(multiPointOut.getCoordinates2D()[0].x == -18924313.434856508
				);
			NUnit.Framework.Assert.IsTrue(multiPointOut.getCoordinates2D()[0].y == 3360107.7777777780
				);
			NUnit.Framework.Assert.IsTrue(multiPointOut.getCoordinates2D()[1].x == -18924313.434856508
				);
			NUnit.Framework.Assert.IsTrue(multiPointOut.getCoordinates2D()[1].y == -3360107.7777777780
				);
		}

		[NUnit.Framework.Test]
		public virtual void testIssue258128()
		{
			com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
			poly1.startPath(0, 0);
			poly1.lineTo(0, 10);
			poly1.lineTo(10, 10);
			poly1.lineTo(10, 0);
			com.esri.core.geometry.Polygon poly2 = new com.esri.core.geometry.Polygon();
			poly2.startPath(10.5, 4);
			poly2.lineTo(10.5, 8);
			poly2.lineTo(14, 10);
			try
			{
				com.esri.core.geometry.GeometryCursor result_cursor = com.esri.core.geometry.OperatorIntersection
					.local().execute(new com.esri.core.geometry.SimpleGeometryCursor(poly1), new com.esri.core.geometry.SimpleGeometryCursor
					(poly2), com.esri.core.geometry.SpatialReference.create(4326), null, 1);
				while (result_cursor.next() != null)
				{
				}
			}
			catch (System.Exception)
			{
				NUnit.Framework.Assert.IsTrue(false);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testUnionTickTock()
		{
			com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
			poly1.startPath(0, 0);
			poly1.lineTo(0, 10);
			poly1.lineTo(10, 10);
			poly1.lineTo(10, 0);
			com.esri.core.geometry.Polygon poly2 = new com.esri.core.geometry.Polygon();
			poly2.startPath(10.5, 4);
			poly2.lineTo(10.5, 8);
			poly2.lineTo(14, 10);
			com.esri.core.geometry.Transformation2D trans = new com.esri.core.geometry.Transformation2D
				();
			com.esri.core.geometry.Polygon poly3 = (com.esri.core.geometry.Polygon)poly1.copy
				();
			trans.setShift(2, 3);
			poly3.applyTransformation(trans);
			com.esri.core.geometry.Polygon poly4 = (com.esri.core.geometry.Polygon)poly1.copy
				();
			trans.setShift(-2, -3);
			poly4.applyTransformation(trans);
			// Create
			com.esri.core.geometry.ListeningGeometryCursor gc = new com.esri.core.geometry.ListeningGeometryCursor
				();
			com.esri.core.geometry.GeometryCursor ticktock = com.esri.core.geometry.OperatorUnion
				.local().execute(gc, null, null);
			// Use tick-tock to push a geometry and do a piece of work.
			gc.tick(poly1);
			ticktock.tock();
			gc.tick(poly2);
			gc.tick(poly3);
			// skiped one tock just for testing.
			ticktock.tock();
			gc.tick(poly4);
			ticktock.tock();
			// Get the result
			com.esri.core.geometry.Geometry result = ticktock.next();
			// Use ListeningGeometryCursor to put all geometries in.
			com.esri.core.geometry.ListeningGeometryCursor gc2 = new com.esri.core.geometry.ListeningGeometryCursor
				();
			gc2.tick(poly1);
			gc2.tick(poly2);
			gc2.tick(poly3);
			gc2.tick(poly4);
			com.esri.core.geometry.GeometryCursor res = com.esri.core.geometry.OperatorUnion.
				local().execute(gc2, null, null);
			// Calling next will process all geometries at once.
			com.esri.core.geometry.Geometry result2 = res.next();
			NUnit.Framework.Assert.IsTrue(result.Equals(result2));
		}

		[NUnit.Framework.Test]
		public virtual void testIntersectionIssueLinePoly1()
		{
			string wkt1 = new string("polygon((0 0, 10 0, 10 10, 0 10, 0 0))");
			string wkt2 = new string("linestring(9 5, 10 5, 9 4, 8 3)");
			com.esri.core.geometry.Geometry g1 = com.esri.core.geometry.OperatorImportFromWkt
				.local().execute(0, com.esri.core.geometry.Geometry.Type.Unknown, wkt1, null);
			com.esri.core.geometry.Geometry g2 = com.esri.core.geometry.OperatorImportFromWkt
				.local().execute(0, com.esri.core.geometry.Geometry.Type.Unknown, wkt2, null);
			com.esri.core.geometry.Geometry res = com.esri.core.geometry.OperatorIntersection
				.local().execute(g1, g2, null, null);
			NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polyline)res).getPathCount
				() == 1);
			NUnit.Framework.Assert.IsTrue(((com.esri.core.geometry.Polyline)res).getPointCount
				() == 4);
		}

		[NUnit.Framework.Test]
		public virtual void testSharedEdgeIntersection_13()
		{
			string s1 = "{\"rings\":[[[0.099604024000029767,0.2107958250000479],[0.14626826900007472,0.2107958250000479],[0.14626826900007472,0.18285316400005058],[0.099604024000029767,0.18285316400005058],[0.099604024000029767,0.2107958250000479]]]}";
			string s2 = "{\"paths\":[[[0.095692051000071388,0.15910190100004229],[0.10324853600002371,0.18285316400004228],[0.12359292700006108,0.18285316400004228],[0.12782611200003657,0.1705583920000322],[0.13537063000007138,0.18285316400004228]]]}";
			com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)com.esri.core.geometry.TestCommonMethods
				.fromJson(s1).getGeometry();
			com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)com.esri.core.geometry.TestCommonMethods
				.fromJson(s2).getGeometry();
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Geometry g = com.esri.core.geometry.OperatorIntersection.local
				().execute(polygon, polyline, sr, null);
			NUnit.Framework.Assert.IsTrue(!g.isEmpty());
		}

		[NUnit.Framework.Test]
		public virtual void testIntersectionIssue2()
		{
			string s1 = "{\"rings\":[[[-97.174860352323378,48.717174479818425],[-97.020624513410553,58.210155436624177],[-94.087641114245969,58.210155436624177],[-94.087641114245969,48.639781902013226],[-97.174860352323378,48.717174479818425]]]}";
			string s2 = "{\"rings\":[[[-94.08764111399995,52.68342763000004],[-94.08764111399995,56.835188018000053],[-90.285921520999977,62.345706350000057],[-94.08764111399995,52.68342763000004]]]}";
			com.esri.core.geometry.Polygon polygon1 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.TestCommonMethods
				.fromJson(s1).getGeometry();
			com.esri.core.geometry.Polygon polygon2 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.TestCommonMethods
				.fromJson(s2).getGeometry();
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.GeometryCursor res = com.esri.core.geometry.OperatorIntersection
				.local().execute(new com.esri.core.geometry.SimpleGeometryCursor(polygon1), new 
				com.esri.core.geometry.SimpleGeometryCursor(polygon2), sr, null, 2);
			com.esri.core.geometry.Geometry g = res.next();
			NUnit.Framework.Assert.IsTrue(g != null);
			NUnit.Framework.Assert.IsTrue(!g.isEmpty());
			com.esri.core.geometry.Geometry g2 = res.next();
			NUnit.Framework.Assert.IsTrue(g2 == null);
			string ss = "{\"paths\":[[[-94.08764111412296,52.68342763000004],[-94.08764111410767,56.83518801800005]]]}";
			com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)com.esri.core.geometry.TestCommonMethods
				.fromJson(ss).getGeometry();
			bool eq = com.esri.core.geometry.OperatorEquals.local().execute(g, polyline, sr, 
				null);
			NUnit.Framework.Assert.IsTrue(eq);
		}
	}
}
