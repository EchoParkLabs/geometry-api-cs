using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestImportExport : NUnit.Framework.TestFixtureAttribute
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
		public static void TestImportExportShapePolygon()
		{
			com.epl.geometry.OperatorExportToESRIShape exporterShape = (com.epl.geometry.OperatorExportToESRIShape)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToESRIShape);
			com.epl.geometry.OperatorImportFromESRIShape importerShape = (com.epl.geometry.OperatorImportFromESRIShape)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromESRIShape);
			com.epl.geometry.Polygon polygon = MakePolygon();
			byte[] esriShape = com.epl.geometry.GeometryEngine.GeometryToEsriShape(polygon);
			com.epl.geometry.Geometry imported = com.epl.geometry.GeometryEngine.GeometryFromEsriShape(esriShape, com.epl.geometry.Geometry.Type.Unknown);
			//com.epl.geometry.TestCommonMethods.CompareGeometryContent((com.epl.geometry.MultiPath)imported, polygon);
			// Test Import Polygon from Polygon
			System.IO.MemoryStream polygonShapeBuffer = exporterShape.Execute(0, polygon);
			com.epl.geometry.Geometry polygonShapeGeometry = importerShape.Execute(0, com.epl.geometry.Geometry.Type.Polygon, polygonShapeBuffer);
			//com.epl.geometry.TestCommonMethods.CompareGeometryContent((com.epl.geometry.MultiPath)polygonShapeGeometry, polygon);
			// Test Import Envelope from Polygon
			com.epl.geometry.Geometry envelopeShapeGeometry = importerShape.Execute(0, com.epl.geometry.Geometry.Type.Envelope, polygonShapeBuffer);
			com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)envelopeShapeGeometry;
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope();
			com.epl.geometry.Envelope otherenv = new com.epl.geometry.Envelope();
			polygon.QueryEnvelope(otherenv);
			NUnit.Framework.Assert.IsTrue(envelope.GetXMin() == otherenv.GetXMin());
			NUnit.Framework.Assert.IsTrue(envelope.GetXMax() == otherenv.GetXMax());
			NUnit.Framework.Assert.IsTrue(envelope.GetYMin() == otherenv.GetYMin());
			NUnit.Framework.Assert.IsTrue(envelope.GetYMax() == otherenv.GetYMax());
			com.epl.geometry.Envelope1D interval;
			com.epl.geometry.Envelope1D otherinterval;
			interval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
			otherinterval = polygon.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
			NUnit.Framework.Assert.IsTrue(interval.vmin == otherinterval.vmin);
			NUnit.Framework.Assert.IsTrue(interval.vmax == otherinterval.vmax);
		}

		[NUnit.Framework.Test]
		public static void TestImportExportShapePolyline()
		{
			com.epl.geometry.OperatorExportToESRIShape exporterShape = (com.epl.geometry.OperatorExportToESRIShape)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToESRIShape);
			com.epl.geometry.OperatorImportFromESRIShape importerShape = (com.epl.geometry.OperatorImportFromESRIShape)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromESRIShape);
			com.epl.geometry.Polyline polyline = MakePolyline();
			// Test Import Polyline from Polyline
			System.IO.MemoryStream polylineShapeBuffer = exporterShape.Execute(0, polyline);
			com.epl.geometry.Geometry polylineShapeGeometry = importerShape.Execute(0, com.epl.geometry.Geometry.Type.Polyline, polylineShapeBuffer);
			// TODO test this
			//com.epl.geometry.TestCommonMethods.CompareGeometryContent((com.epl.geometry.MultiPath)polylineShapeGeometry, polyline);
			// Test Import Envelope from Polyline;
			com.epl.geometry.Geometry envelopeShapeGeometry = importerShape.Execute(0, com.epl.geometry.Geometry.Type.Envelope, polylineShapeBuffer);
			com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)envelopeShapeGeometry;
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope();
			com.epl.geometry.Envelope otherenv = new com.epl.geometry.Envelope();
			envelope.QueryEnvelope(env);
			polyline.QueryEnvelope(otherenv);
			NUnit.Framework.Assert.IsTrue(env.GetXMin() == otherenv.GetXMin());
			NUnit.Framework.Assert.IsTrue(env.GetXMax() == otherenv.GetXMax());
			NUnit.Framework.Assert.IsTrue(env.GetYMin() == otherenv.GetYMin());
			NUnit.Framework.Assert.IsTrue(env.GetYMax() == otherenv.GetYMax());
			com.epl.geometry.Envelope1D interval;
			com.epl.geometry.Envelope1D otherinterval;
			interval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
			otherinterval = polyline.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
			NUnit.Framework.Assert.IsTrue(interval.vmin == otherinterval.vmin);
			NUnit.Framework.Assert.IsTrue(interval.vmax == otherinterval.vmax);
		}

		[NUnit.Framework.Test]
		public static void TestImportExportShapeMultiPoint()
		{
			com.epl.geometry.OperatorExportToESRIShape exporterShape = (com.epl.geometry.OperatorExportToESRIShape)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToESRIShape);
			com.epl.geometry.OperatorImportFromESRIShape importerShape = (com.epl.geometry.OperatorImportFromESRIShape)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromESRIShape);
			com.epl.geometry.MultiPoint multipoint = MakeMultiPoint();
			// Test Import MultiPoint from MultiPoint
			System.IO.MemoryStream multipointShapeBuffer = exporterShape.Execute(0, multipoint);
			com.epl.geometry.MultiPoint multipointShapeGeometry = (com.epl.geometry.MultiPoint)importerShape.Execute(0, com.epl.geometry.Geometry.Type.MultiPoint, multipointShapeBuffer);
			//com.epl.geometry.TestCommonMethods.CompareGeometryContent((com.epl.geometry.MultiPoint)multipointShapeGeometry, multipoint);
			// Test Import Envelope from MultiPoint
			com.epl.geometry.Geometry envelopeShapeGeometry = importerShape.Execute(0, com.epl.geometry.Geometry.Type.Envelope, multipointShapeBuffer);
			com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)envelopeShapeGeometry;
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope();
			com.epl.geometry.Envelope otherenv = new com.epl.geometry.Envelope();
			envelope.QueryEnvelope(env);
			multipoint.QueryEnvelope(otherenv);
			NUnit.Framework.Assert.IsTrue(env.GetXMin() == otherenv.GetXMin());
			NUnit.Framework.Assert.IsTrue(env.GetXMax() == otherenv.GetXMax());
			NUnit.Framework.Assert.IsTrue(env.GetYMin() == otherenv.GetYMin());
			NUnit.Framework.Assert.IsTrue(env.GetYMax() == otherenv.GetYMax());
			com.epl.geometry.Envelope1D interval;
			com.epl.geometry.Envelope1D otherinterval;
			interval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
			otherinterval = multipoint.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
			NUnit.Framework.Assert.IsTrue(interval.vmin == otherinterval.vmin);
			NUnit.Framework.Assert.IsTrue(interval.vmax == otherinterval.vmax);
			interval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0);
			otherinterval = multipoint.QueryInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0);
			NUnit.Framework.Assert.IsTrue(interval.vmin == otherinterval.vmin);
			NUnit.Framework.Assert.IsTrue(interval.vmax == otherinterval.vmax);
		}

		[NUnit.Framework.Test]
		public static void TestImportExportShapePoint()
		{
			com.epl.geometry.OperatorExportToESRIShape exporterShape = (com.epl.geometry.OperatorExportToESRIShape)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToESRIShape);
			com.epl.geometry.OperatorImportFromESRIShape importerShape = (com.epl.geometry.OperatorImportFromESRIShape)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromESRIShape);
			// Point
			com.epl.geometry.Point point = MakePoint();
			// Test Import Point from Point
			System.IO.MemoryStream pointShapeBuffer = exporterShape.Execute(0, point);
			com.epl.geometry.Point pointShapeGeometry = (com.epl.geometry.Point)importerShape.Execute(0, com.epl.geometry.Geometry.Type.Point, pointShapeBuffer);
			double x1 = point.GetX();
			double x2 = pointShapeGeometry.GetX();
			NUnit.Framework.Assert.IsTrue(x1 == x2);
			double y1 = point.GetY();
			double y2 = pointShapeGeometry.GetY();
			NUnit.Framework.Assert.IsTrue(y1 == y2);
			double z1 = point.GetZ();
			double z2 = pointShapeGeometry.GetZ();
			NUnit.Framework.Assert.IsTrue(z1 == z2);
			double m1 = point.GetM();
			double m2 = pointShapeGeometry.GetM();
			NUnit.Framework.Assert.IsTrue(m1 == m2);
			int id1 = point.GetID();
			int id2 = pointShapeGeometry.GetID();
			NUnit.Framework.Assert.IsTrue(id1 == id2);
			// Test Import Multipoint from Point
			com.epl.geometry.MultiPoint multipointShapeGeometry = (com.epl.geometry.MultiPoint)importerShape.Execute(0, com.epl.geometry.Geometry.Type.MultiPoint, pointShapeBuffer);
			com.epl.geometry.Point point2d = multipointShapeGeometry.GetPoint(0);
			NUnit.Framework.Assert.IsTrue(x1 == point2d.GetX() && y1 == point2d.GetY());
			int pointCount = multipointShapeGeometry.GetPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 1);
			z2 = multipointShapeGeometry.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0);
			NUnit.Framework.Assert.IsTrue(z1 == z2);
			m2 = multipointShapeGeometry.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0);
			NUnit.Framework.Assert.IsTrue(m1 == m2);
			id2 = multipointShapeGeometry.GetAttributeAsInt(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0);
			NUnit.Framework.Assert.IsTrue(id1 == id2);
			// Test Import Envelope from Point
			com.epl.geometry.Geometry envelopeShapeGeometry = importerShape.Execute(0, com.epl.geometry.Geometry.Type.Envelope, pointShapeBuffer);
			com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)envelopeShapeGeometry;
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope();
			com.epl.geometry.Envelope otherenv = new com.epl.geometry.Envelope();
			envelope.QueryEnvelope(env);
			point.QueryEnvelope(otherenv);
			NUnit.Framework.Assert.IsTrue(env.GetXMin() == otherenv.GetXMin());
			NUnit.Framework.Assert.IsTrue(env.GetXMax() == otherenv.GetXMax());
			NUnit.Framework.Assert.IsTrue(env.GetYMin() == otherenv.GetYMin());
			NUnit.Framework.Assert.IsTrue(env.GetYMax() == otherenv.GetYMax());
			com.epl.geometry.Envelope1D interval;
			com.epl.geometry.Envelope1D otherinterval;
			interval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
			otherinterval = point.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
			NUnit.Framework.Assert.IsTrue(interval.vmin == otherinterval.vmin);
			NUnit.Framework.Assert.IsTrue(interval.vmax == otherinterval.vmax);
			interval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0);
			otherinterval = point.QueryInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0);
			NUnit.Framework.Assert.IsTrue(interval.vmin == otherinterval.vmin);
			NUnit.Framework.Assert.IsTrue(interval.vmax == otherinterval.vmax);
		}

		[NUnit.Framework.Test]
		public static void TestImportExportShapeEnvelope()
		{
			com.epl.geometry.OperatorExportToESRIShape exporterShape = (com.epl.geometry.OperatorExportToESRIShape)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToESRIShape);
			com.epl.geometry.OperatorImportFromESRIShape importerShape = (com.epl.geometry.OperatorImportFromESRIShape)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromESRIShape);
			// Test Export Envelope to Polygon
			com.epl.geometry.Envelope envelope = MakeEnvelope();
			System.IO.MemoryStream polygonShapeBuffer = exporterShape.Execute(0, envelope);
			com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)importerShape.Execute(0, com.epl.geometry.Geometry.Type.Polygon, polygonShapeBuffer);
			int pointCount = polygon.GetPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 4);
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope();
			envelope.QueryEnvelope(env);
			// interval = envelope.queryInterval(VertexDescription.Semantics.Z, 0);
			com.epl.geometry.Point point3d;
			point3d = polygon.GetPoint(0);
			NUnit.Framework.Assert.IsTrue(point3d.GetX() == env.GetXMin() && point3d.GetY() == env.GetYMin());
			// && point3d.z ==
			// interval.vmin);
			point3d = polygon.GetPoint(1);
			NUnit.Framework.Assert.IsTrue(point3d.GetX() == env.GetXMin() && point3d.GetY() == env.GetYMax());
			// && point3d.z ==
			// interval.vmax);
			point3d = polygon.GetPoint(2);
			NUnit.Framework.Assert.IsTrue(point3d.GetX() == env.GetXMax() && point3d.GetY() == env.GetYMax());
			// && point3d.z ==
			// interval.vmin);
			point3d = polygon.GetPoint(3);
			NUnit.Framework.Assert.IsTrue(point3d.GetX() == env.GetXMax() && point3d.GetY() == env.GetYMin());
			// && point3d.z ==
			// interval.vmax);
			com.epl.geometry.Envelope1D interval;
			interval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0);
			double m = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmin);
			m = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmax);
			m = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmin);
			m = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 3, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmax);
			interval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0);
			double id = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0);
			NUnit.Framework.Assert.IsTrue(id == interval.vmin);
			id = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 1, 0);
			NUnit.Framework.Assert.IsTrue(id == interval.vmax);
			id = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 2, 0);
			NUnit.Framework.Assert.IsTrue(id == interval.vmin);
			id = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 3, 0);
			NUnit.Framework.Assert.IsTrue(id == interval.vmax);
		}

//		[NUnit.Framework.Test]
//		public static void TestImportExportWkbGeometryCollection()
//		{
//			com.epl.geometry.OperatorImportFromWkb importerWKB = (com.epl.geometry.OperatorImportFromWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkb);
//			int offset = 0;
//			System.IO.MemoryStream wkbBuffer = System.IO.MemoryStream.Allocate(600).Order(java.nio.ByteOrder.NativeOrder());
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			// byte order
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbGeometryCollection);
//			offset += 4;
//			// type
//			wkbBuffer.PutInt(offset, 3);
//			// 3 geometries
//			offset += 4;
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbPoint);
//			offset += 4;
//			wkbBuffer.PutDouble(offset, 0);
//			offset += 8;
//			wkbBuffer.PutDouble(offset, 0);
//			offset += 8;
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			// byte order
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbGeometryCollection);
//			offset += 4;
//			// type
//			wkbBuffer.PutInt(offset, 7);
//			// 7 empty geometries
//			offset += 4;
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbLineString);
//			offset += 4;
//			wkbBuffer.PutInt(offset, 0);
//			// 0 points, for empty linestring
//			offset += 4;
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbPolygon);
//			offset += 4;
//			wkbBuffer.PutInt(offset, 0);
//			// 0 points, for empty polygon
//			offset += 4;
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPolygon);
//			offset += 4;
//			wkbBuffer.PutInt(offset, 0);
//			// 0 points, for empty multipolygon
//			offset += 4;
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiLineString);
//			offset += 4;
//			wkbBuffer.PutInt(offset, 0);
//			// 0 points, for empty multilinestring
//			offset += 4;
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbGeometryCollection);
//			offset += 4;
//			wkbBuffer.PutInt(offset, 0);
//			// 0 geometries, for empty
//			// geometrycollection
//			offset += 4;
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiPoint);
//			offset += 4;
//			wkbBuffer.PutInt(offset, 0);
//			// 0 points, for empty multipoint
//			offset += 4;
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbPoint);
//			offset += 4;
//			wkbBuffer.PutDouble(offset, 66);
//			offset += 8;
//			wkbBuffer.PutDouble(offset, 88);
//			offset += 8;
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbPoint);
//			offset += 4;
//			wkbBuffer.PutDouble(offset, 13);
//			offset += 8;
//			wkbBuffer.PutDouble(offset, 17);
//			offset += 8;
//			// "GeometryCollection( Point (0 0),  GeometryCollection( LineString empty, Polygon empty, MultiPolygon empty, MultiLineString empty, MultiPoint empty ), Point (13 17) )";
//			com.epl.geometry.OGCStructure structure = importerWKB.ExecuteOGC(0, wkbBuffer, null).m_structures[0];
//			NUnit.Framework.Assert.IsTrue(structure.m_type == 7);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[0].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_type == 7);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[2].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[0].m_type == 2);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[1].m_type == 3);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[2].m_type == 6);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[3].m_type == 5);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[4].m_type == 7);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[5].m_type == 4);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[6].m_type == 1);
//			com.epl.geometry.Point p = (com.epl.geometry.Point)structure.m_structures[1].m_structures[6].m_geometry;
//			NUnit.Framework.Assert.IsTrue(p.GetX() == 66);
//			NUnit.Framework.Assert.IsTrue(p.GetY() == 88);
//			p = (com.epl.geometry.Point)structure.m_structures[2].m_geometry;
//			NUnit.Framework.Assert.IsTrue(p.GetX() == 13);
//			NUnit.Framework.Assert.IsTrue(p.GetY() == 17);
//		}
//
//		[NUnit.Framework.Test]
//		public static void TestImportExportWKBPolygon()
//		{
//			com.epl.geometry.OperatorExportToWkb exporterWKB = (com.epl.geometry.OperatorExportToWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkb);
//			com.epl.geometry.OperatorExportToWkt exporterWKT = (com.epl.geometry.OperatorExportToWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkt);
//			com.epl.geometry.OperatorImportFromWkb importerWKB = (com.epl.geometry.OperatorImportFromWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkb);
//			// Test Import Polygon with bad rings
//			int offset = 0;
//			System.IO.MemoryStream wkbBuffer = System.IO.MemoryStream.Allocate(500).Order(java.nio.ByteOrder.NativeOrder());
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			// byte order
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbPolygon);
//			offset += 4;
//			// type
//			wkbBuffer.PutInt(offset, 8);
//			offset += 4;
//			// num rings
//			wkbBuffer.PutInt(offset, 4);
//			offset += 4;
//			// num points
//			wkbBuffer.PutDouble(offset, 0.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 0.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutDouble(offset, 0.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 10.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutDouble(offset, 10.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 10.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutDouble(offset, 0.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 0.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutInt(offset, 1);
//			offset += 4;
//			// num points
//			wkbBuffer.PutDouble(offset, 36.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 17.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutInt(offset, 2);
//			offset += 4;
//			// num points
//			wkbBuffer.PutDouble(offset, 19.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 19.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutDouble(offset, -19.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, -19.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutInt(offset, 4);
//			offset += 4;
//			// num points
//			wkbBuffer.PutDouble(offset, 23.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 88);
//			offset += 8;
//			// y
//			wkbBuffer.PutDouble(offset, 13.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 43.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutDouble(offset, 59.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 79.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutDouble(offset, 83.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 87.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutInt(offset, 3);
//			offset += 4;
//			// num points
//			wkbBuffer.PutDouble(offset, 23.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 88);
//			offset += 8;
//			// y
//			wkbBuffer.PutDouble(offset, 88);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 43.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutDouble(offset, 67.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 79.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutInt(offset, 0);
//			offset += 4;
//			// num points
//			wkbBuffer.PutInt(offset, 3);
//			offset += 4;
//			// num points
//			wkbBuffer.PutDouble(offset, 23.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 88);
//			offset += 8;
//			// y
//			wkbBuffer.PutDouble(offset, 88);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 43.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutDouble(offset, 67.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 88);
//			offset += 8;
//			// y
//			wkbBuffer.PutInt(offset, 2);
//			offset += 4;
//			// num points
//			wkbBuffer.PutDouble(offset, 23.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 67.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutDouble(offset, 43.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 67.0);
//			offset += 8;
//			// y
//			com.epl.geometry.Geometry p = importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polygon, wkbBuffer, null);
//			int pc = ((com.epl.geometry.Polygon)p).GetPathCount();
//			string wktString = exporterWKT.Execute(0, p, null);
//			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOLYGON (((0 0, 10 10, 0 10, 0 0), (36 17, 36 17, 36 17), (19 19, -19 -19, 19 19), (23 88, 83 87, 59 79, 13 43, 23 88), (23 88, 67 79, 88 43, 23 88), (23 88, 67 88, 88 43, 23 88), (23 67, 43 67, 23 67)))"));
//			wktString = exporterWKT.Execute(com.epl.geometry.WktExportFlags.wktExportPolygon, p, null);
//			NUnit.Framework.Assert.IsTrue(wktString.Equals("POLYGON ((0 0, 10 10, 0 10, 0 0), (36 17, 36 17, 36 17), (19 19, -19 -19, 19 19), (23 88, 83 87, 59 79, 13 43, 23 88), (23 88, 67 79, 88 43, 23 88), (23 88, 67 88, 88 43, 23 88), (23 67, 43 67, 23 67))"));
//			com.epl.geometry.Polygon polygon = MakePolygon();
//			// Test Import Polygon from Polygon8
//			System.IO.MemoryStream polygonWKBBuffer = exporterWKB.Execute(0, polygon, null);
//			int wkbType = polygonWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbMultiPolygonZM);
//			com.epl.geometry.Geometry polygonWKBGeometry = importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polygon, polygonWKBBuffer, null);
//			//com.epl.geometry.TestCommonMethods.CompareGeometryContent((com.epl.geometry.MultiVertexGeometry)polygonWKBGeometry, polygon);
//			// Test WKB_export_multi_polygon on nonempty single part polygon
//			com.epl.geometry.Polygon polygon2 = MakePolygon2();
//			NUnit.Framework.Assert.IsTrue(polygon2.GetPathCount() == 1);
//			polygonWKBBuffer = exporterWKB.Execute(com.epl.geometry.WkbExportFlags.wkbExportMultiPolygon, polygon2, null);
//			polygonWKBGeometry = importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polygon, polygonWKBBuffer, null);
//			//com.epl.geometry.TestCommonMethods.CompareGeometryContent((com.epl.geometry.MultiVertexGeometry)polygonWKBGeometry, polygon2);
//			wkbType = polygonWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbMultiPolygonZM);
//			// Test WKB_export_polygon on nonempty single part polygon
//			NUnit.Framework.Assert.IsTrue(polygon2.GetPathCount() == 1);
//			polygonWKBBuffer = exporterWKB.Execute(com.epl.geometry.WkbExportFlags.wkbExportPolygon, polygon2, null);
//			polygonWKBGeometry = importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polygon, polygonWKBBuffer, null);
//			//com.epl.geometry.TestCommonMethods.CompareGeometryContent((com.epl.geometry.MultiVertexGeometry)polygonWKBGeometry, polygon2);
//			wkbType = polygonWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbPolygonZM);
//			// Test WKB_export_polygon on empty polygon
//			com.epl.geometry.Polygon polygon3 = new com.epl.geometry.Polygon();
//			polygonWKBBuffer = exporterWKB.Execute(com.epl.geometry.WkbExportFlags.wkbExportPolygon, polygon3, null);
//			polygonWKBGeometry = importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polygon, polygonWKBBuffer, null);
//			NUnit.Framework.Assert.IsTrue(polygonWKBGeometry.IsEmpty() == true);
//			wkbType = polygonWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbPolygon);
//			// Test WKB_export_defaults on empty polygon
//			polygonWKBBuffer = exporterWKB.Execute(0, polygon3, null);
//			polygonWKBGeometry = importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polygon, polygonWKBBuffer, null);
//			NUnit.Framework.Assert.IsTrue(polygonWKBGeometry.IsEmpty() == true);
//			wkbType = polygonWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbMultiPolygon);
//		}
//
//		[NUnit.Framework.Test]
//		public static void TestImportExportWKBPolyline()
//		{
//			com.epl.geometry.OperatorExportToWkb exporterWKB = (com.epl.geometry.OperatorExportToWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkb);
//			com.epl.geometry.OperatorExportToWkt exporterWKT = (com.epl.geometry.OperatorExportToWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkt);
//			com.epl.geometry.OperatorImportFromWkb importerWKB = (com.epl.geometry.OperatorImportFromWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkb);
//			// Test Import Polyline with bad paths (i.e. paths with one point or
//			// zero points)
//			int offset = 0;
//			System.IO.MemoryStream wkbBuffer = System.IO.MemoryStream.Allocate(500).Order(java.nio.ByteOrder.NativeOrder());
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			// byte order
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbMultiLineString);
//			offset += 4;
//			// type
//			wkbBuffer.PutInt(offset, 4);
//			offset += 4;
//			// num paths
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			// byte order
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbLineString);
//			offset += 4;
//			// type
//			wkbBuffer.PutInt(offset, 1);
//			offset += 4;
//			// num points
//			wkbBuffer.PutDouble(offset, 36.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 17.0);
//			offset += 8;
//			// y
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			// byte order
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbLineString);
//			offset += 4;
//			// type
//			wkbBuffer.PutInt(offset, 0);
//			offset += 4;
//			// num points
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			// byte order
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbLineString);
//			offset += 4;
//			// type
//			wkbBuffer.PutInt(offset, 1);
//			offset += 4;
//			// num points
//			wkbBuffer.PutDouble(offset, 19.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 19.0);
//			offset += 8;
//			// y
//			wkbBuffer.Put(offset, unchecked((byte)com.epl.geometry.WkbByteOrder.wkbNDR));
//			offset += 1;
//			// byte order
//			wkbBuffer.PutInt(offset, com.epl.geometry.WkbGeometryType.wkbLineString);
//			offset += 4;
//			// type
//			wkbBuffer.PutInt(offset, 3);
//			offset += 4;
//			// num points
//			wkbBuffer.PutDouble(offset, 88);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 29.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutDouble(offset, 13.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 43.0);
//			offset += 8;
//			// y
//			wkbBuffer.PutDouble(offset, 59.0);
//			offset += 8;
//			// x
//			wkbBuffer.PutDouble(offset, 88);
//			offset += 8;
//			// y
//			com.epl.geometry.Polyline p = (com.epl.geometry.Polyline)(importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polyline, wkbBuffer, null));
//			int pc = p.GetPointCount();
//			int pac = p.GetPathCount();
//			NUnit.Framework.Assert.IsTrue(p.GetPointCount() == 7);
//			NUnit.Framework.Assert.IsTrue(p.GetPathCount() == 3);
//			string wktString = exporterWKT.Execute(0, p, null);
//			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTILINESTRING ((36 17, 36 17), (19 19, 19 19), (88 29, 13 43, 59 88))"));
//			com.epl.geometry.Polyline polyline = MakePolyline();
//			polyline.DropAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
//			// Test Import Polyline from Polyline
//			System.IO.MemoryStream polylineWKBBuffer = exporterWKB.Execute(0, polyline, null);
//			int wkbType = polylineWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbMultiLineStringZM);
//			com.epl.geometry.Geometry polylineWKBGeometry = importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polyline, polylineWKBBuffer, null);
//			//com.epl.geometry.TestCommonMethods.CompareGeometryContent((com.epl.geometry.MultiVertexGeometry)polylineWKBGeometry, polyline);
//			// Test wkbExportMultiPolyline on nonempty single part polyline
//			com.epl.geometry.Polyline polyline2 = MakePolyline2();
//			NUnit.Framework.Assert.IsTrue(polyline2.GetPathCount() == 1);
//			polylineWKBBuffer = exporterWKB.Execute(com.epl.geometry.WkbExportFlags.wkbExportMultiLineString, polyline2, null);
//			polylineWKBGeometry = importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polyline, polylineWKBBuffer, null);
//			//com.epl.geometry.TestCommonMethods.CompareGeometryContent((com.epl.geometry.MultiVertexGeometry)polylineWKBGeometry, polyline2);
//			wkbType = polylineWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbMultiLineStringZM);
//			// Test wkbExportPolyline on nonempty single part polyline
//			NUnit.Framework.Assert.IsTrue(polyline2.GetPathCount() == 1);
//			polylineWKBBuffer = exporterWKB.Execute(com.epl.geometry.WkbExportFlags.wkbExportLineString, polyline2, null);
//			polylineWKBGeometry = importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polyline, polylineWKBBuffer, null);
//			//com.epl.geometry.TestCommonMethods.CompareGeometryContent((com.epl.geometry.MultiVertexGeometry)polylineWKBGeometry, polyline2);
//			wkbType = polylineWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbLineStringZM);
//			// Test wkbExportPolyline on empty polyline
//			com.epl.geometry.Polyline polyline3 = new com.epl.geometry.Polyline();
//			polylineWKBBuffer = exporterWKB.Execute(com.epl.geometry.WkbExportFlags.wkbExportLineString, polyline3, null);
//			polylineWKBGeometry = importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polyline, polylineWKBBuffer, null);
//			NUnit.Framework.Assert.IsTrue(polylineWKBGeometry.IsEmpty() == true);
//			wkbType = polylineWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbLineString);
//			// Test WKB_export_defaults on empty polyline
//			polylineWKBBuffer = exporterWKB.Execute(0, polyline3, null);
//			polylineWKBGeometry = importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polyline, polylineWKBBuffer, null);
//			NUnit.Framework.Assert.IsTrue(polylineWKBGeometry.IsEmpty() == true);
//			wkbType = polylineWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbMultiLineString);
//		}
//
//		[NUnit.Framework.Test]
//		public static void TestImportExportWKBMultiPoint()
//		{
//			com.epl.geometry.OperatorExportToWkb exporterWKB = (com.epl.geometry.OperatorExportToWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkb);
//			com.epl.geometry.OperatorImportFromWkb importerWKB = (com.epl.geometry.OperatorImportFromWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkb);
//			com.epl.geometry.MultiPoint multipoint = MakeMultiPoint();
//			multipoint.DropAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
//			// Test Import Multi_point from Multi_point
//			System.IO.MemoryStream multipointWKBBuffer = exporterWKB.Execute(0, multipoint, null);
//			int wkbType = multipointWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbMultiPointZ);
//			com.epl.geometry.MultiPoint multipointWKBGeometry = (com.epl.geometry.MultiPoint)(importerWKB.Execute(0, com.epl.geometry.Geometry.Type.MultiPoint, multipointWKBBuffer, null));
//			//com.epl.geometry.TestCommonMethods.CompareGeometryContent((com.epl.geometry.MultiVertexGeometry)multipointWKBGeometry, multipoint);
//			// Test WKB_export_point on nonempty single point Multi_point
//			com.epl.geometry.MultiPoint multipoint2 = MakeMultiPoint2();
//			NUnit.Framework.Assert.IsTrue(multipoint2.GetPointCount() == 1);
//			System.IO.MemoryStream pointWKBBuffer = exporterWKB.Execute(com.epl.geometry.WkbExportFlags.wkbExportPoint, multipoint2, null);
//			com.epl.geometry.Point pointWKBGeometry = (com.epl.geometry.Point)(importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Point, pointWKBBuffer, null));
//			com.epl.geometry.Point3D point3d;
//			com.epl.geometry.Point3D mpoint3d;
//			point3d = pointWKBGeometry.GetXYZ();
//			mpoint3d = multipoint2.GetXYZ(0);
//			NUnit.Framework.Assert.IsTrue(point3d.x == mpoint3d.x && point3d.y == mpoint3d.y && point3d.z == mpoint3d.z);
//			wkbType = pointWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbPointZ);
//			// Test WKB_export_point on empty Multi_point
//			com.epl.geometry.MultiPoint multipoint3 = new com.epl.geometry.MultiPoint();
//			pointWKBBuffer = exporterWKB.Execute(com.epl.geometry.WkbExportFlags.wkbExportPoint, multipoint3, null);
//			pointWKBGeometry = (com.epl.geometry.Point)(importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Point, pointWKBBuffer, null));
//			NUnit.Framework.Assert.IsTrue(pointWKBGeometry.IsEmpty() == true);
//			wkbType = pointWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbPoint);
//			// Test WKB_export_defaults on empty Multi_point
//			multipointWKBBuffer = exporterWKB.Execute(0, multipoint3, null);
//			multipointWKBGeometry = (com.epl.geometry.MultiPoint)(importerWKB.Execute(0, com.epl.geometry.Geometry.Type.MultiPoint, multipointWKBBuffer, null));
//			NUnit.Framework.Assert.IsTrue(multipointWKBGeometry.IsEmpty() == true);
//			wkbType = multipointWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbMultiPoint);
//		}
//
//		[NUnit.Framework.Test]
//		public static void TestImportExportWKBPoint()
//		{
//			com.epl.geometry.OperatorExportToWkb exporterWKB = (com.epl.geometry.OperatorExportToWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkb);
//			com.epl.geometry.OperatorImportFromWkb importerWKB = (com.epl.geometry.OperatorImportFromWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkb);
//			// Point
//			com.epl.geometry.Point point = MakePoint();
//			// Test Import Point from Point
//			System.IO.MemoryStream pointWKBBuffer = exporterWKB.Execute(0, point, null);
//			int wkbType = pointWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbPointZM);
//			com.epl.geometry.Point pointWKBGeometry = (com.epl.geometry.Point)(importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Point, pointWKBBuffer, null));
//			double x_1 = point.GetX();
//			double x2 = pointWKBGeometry.GetX();
//			NUnit.Framework.Assert.IsTrue(x_1 == x2);
//			double y1 = point.GetY();
//			double y2 = pointWKBGeometry.GetY();
//			NUnit.Framework.Assert.IsTrue(y1 == y2);
//			double z_1 = point.GetZ();
//			double z_2 = pointWKBGeometry.GetZ();
//			NUnit.Framework.Assert.IsTrue(z_1 == z_2);
//			double m1 = point.GetM();
//			double m2 = pointWKBGeometry.GetM();
//			NUnit.Framework.Assert.IsTrue(m1 == m2);
//			// Test WKB_export_defaults on empty point
//			com.epl.geometry.Point point2 = new com.epl.geometry.Point();
//			pointWKBBuffer = exporterWKB.Execute(0, point2, null);
//			pointWKBGeometry = (com.epl.geometry.Point)(importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Point, pointWKBBuffer, null));
//			NUnit.Framework.Assert.IsTrue(pointWKBGeometry.IsEmpty() == true);
//			wkbType = pointWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbPoint);
//			// Test WKB_export_point on empty point
//			pointWKBBuffer = exporterWKB.Execute(com.epl.geometry.WkbExportFlags.wkbExportPoint, point2, null);
//			pointWKBGeometry = (com.epl.geometry.Point)(importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Point, pointWKBBuffer, null));
//			NUnit.Framework.Assert.IsTrue(pointWKBGeometry.IsEmpty() == true);
//			wkbType = pointWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbPoint);
//			// Test WKB_export_multi_point on empty point
//			com.epl.geometry.MultiPoint multipoint = new com.epl.geometry.MultiPoint();
//			System.IO.MemoryStream multipointWKBBuffer = exporterWKB.Execute(com.epl.geometry.WkbExportFlags.wkbExportMultiPoint, multipoint, null);
//			com.epl.geometry.MultiPoint multipointWKBGeometry = (com.epl.geometry.MultiPoint)(importerWKB.Execute(0, com.epl.geometry.Geometry.Type.MultiPoint, multipointWKBBuffer, null));
//			NUnit.Framework.Assert.IsTrue(multipointWKBGeometry.IsEmpty() == true);
//			wkbType = multipointWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbMultiPoint);
//			// Test WKB_export_point on nonempty single point Multi_point
//			com.epl.geometry.MultiPoint multipoint2 = MakeMultiPoint2();
//			NUnit.Framework.Assert.IsTrue(multipoint2.GetPointCount() == 1);
//			pointWKBBuffer = exporterWKB.Execute(com.epl.geometry.WkbExportFlags.wkbExportPoint, multipoint2, null);
//			pointWKBGeometry = (com.epl.geometry.Point)(importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Point, pointWKBBuffer, null));
//			com.epl.geometry.Point3D point3d;
//			com.epl.geometry.Point3D mpoint3d;
//			point3d = pointWKBGeometry.GetXYZ();
//			mpoint3d = multipoint2.GetXYZ(0);
//			NUnit.Framework.Assert.IsTrue(point3d.x == mpoint3d.x && point3d.y == mpoint3d.y && point3d.z == mpoint3d.z);
//			wkbType = pointWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbPointZ);
//		}
//
//		[NUnit.Framework.Test]
//		public static void TestImportExportWKBEnvelope()
//		{
//			com.epl.geometry.OperatorExportToWkb exporterWKB = (com.epl.geometry.OperatorExportToWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkb);
//			com.epl.geometry.OperatorImportFromWkb importerWKB = (com.epl.geometry.OperatorImportFromWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkb);
//			// Test Export Envelope to Polygon (WKB_export_defaults)
//			com.epl.geometry.Envelope envelope = MakeEnvelope();
//			envelope.DropAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
//			System.IO.MemoryStream polygonWKBBuffer = exporterWKB.Execute(0, envelope, null);
//			int wkbType = polygonWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbPolygonZM);
//			com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polygon, polygonWKBBuffer, null));
//			int point_count = polygon.GetPointCount();
//			NUnit.Framework.Assert.IsTrue(point_count == 4);
//			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
//			com.epl.geometry.Envelope1D interval;
//			envelope.QueryEnvelope2D(env);
//			interval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
//			com.epl.geometry.Point3D point3d;
//			point3d = polygon.GetXYZ(0);
//			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmin && point3d.y == env.ymin && point3d.z == interval.vmin);
//			point3d = polygon.GetXYZ(1);
//			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmin && point3d.y == env.ymax && point3d.z == interval.vmax);
//			point3d = polygon.GetXYZ(2);
//			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmax && point3d.y == env.ymax && point3d.z == interval.vmin);
//			point3d = polygon.GetXYZ(3);
//			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmax && point3d.y == env.ymin && point3d.z == interval.vmax);
//			interval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0);
//			double m = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0);
//			NUnit.Framework.Assert.IsTrue(m == interval.vmin);
//			m = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0);
//			NUnit.Framework.Assert.IsTrue(m == interval.vmax);
//			m = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0);
//			NUnit.Framework.Assert.IsTrue(m == interval.vmin);
//			m = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 3, 0);
//			NUnit.Framework.Assert.IsTrue(m == interval.vmax);
//			// Test WKB_export_multi_polygon on nonempty Envelope
//			polygonWKBBuffer = exporterWKB.Execute(com.epl.geometry.WkbExportFlags.wkbExportMultiPolygon, envelope, null);
//			wkbType = polygonWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbMultiPolygonZM);
//			polygon = (com.epl.geometry.Polygon)(importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polygon, polygonWKBBuffer, null));
//			point_count = polygon.GetPointCount();
//			NUnit.Framework.Assert.IsTrue(point_count == 4);
//			envelope.QueryEnvelope2D(env);
//			interval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
//			point3d = polygon.GetXYZ(0);
//			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmin && point3d.y == env.ymin && point3d.z == interval.vmin);
//			point3d = polygon.GetXYZ(1);
//			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmin && point3d.y == env.ymax && point3d.z == interval.vmax);
//			point3d = polygon.GetXYZ(2);
//			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmax && point3d.y == env.ymax && point3d.z == interval.vmin);
//			point3d = polygon.GetXYZ(3);
//			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmax && point3d.y == env.ymin && point3d.z == interval.vmax);
//			interval = envelope.QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0);
//			m = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0);
//			NUnit.Framework.Assert.IsTrue(m == interval.vmin);
//			m = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0);
//			NUnit.Framework.Assert.IsTrue(m == interval.vmax);
//			m = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0);
//			NUnit.Framework.Assert.IsTrue(m == interval.vmin);
//			m = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 3, 0);
//			NUnit.Framework.Assert.IsTrue(m == interval.vmax);
//			// Test WKB_export_defaults on empty Envelope
//			com.epl.geometry.Envelope envelope2 = new com.epl.geometry.Envelope();
//			polygonWKBBuffer = exporterWKB.Execute(0, envelope2, null);
//			wkbType = polygonWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbPolygon);
//			polygon = (com.epl.geometry.Polygon)(importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polygon, polygonWKBBuffer, null));
//			NUnit.Framework.Assert.IsTrue(polygon.IsEmpty());
//			// Test WKB_export_polygon on empty Envelope
//			polygonWKBBuffer = exporterWKB.Execute(com.epl.geometry.WkbExportFlags.wkbExportPolygon, envelope2, null);
//			wkbType = polygonWKBBuffer.GetInt(1);
//			NUnit.Framework.Assert.IsTrue(wkbType == com.epl.geometry.WkbGeometryType.wkbPolygon);
//			polygon = (com.epl.geometry.Polygon)(importerWKB.Execute(0, com.epl.geometry.Geometry.Type.Polygon, polygonWKBBuffer, null));
//			NUnit.Framework.Assert.IsTrue(polygon.IsEmpty());
//		}
//
//		[NUnit.Framework.Test]
//		public static void TestImportExportWktGeometryCollection()
//		{
//			com.epl.geometry.OperatorImportFromWkt importerWKT = (com.epl.geometry.OperatorImportFromWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkt);
//			com.epl.geometry.OperatorExportToWkt exporterWKT = (com.epl.geometry.OperatorExportToWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkt);
//			string wktString;
//			com.epl.geometry.Envelope2D envelope = new com.epl.geometry.Envelope2D();
//			com.epl.geometry.WktParser wktParser = new com.epl.geometry.WktParser();
//			wktString = "GeometryCollection( Point (0 0),  GeometryCollection( Point (0 0) ,  Point (1 1) , Point (2 2), LineString empty ), Point (1 1),  Point (2 2) )";
//			com.epl.geometry.OGCStructure structure = importerWKT.ExecuteOGC(0, wktString, null).m_structures[0];
//			NUnit.Framework.Assert.IsTrue(structure.m_type == 7);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[0].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[0].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_type == 7);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[2].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[3].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[0].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[1].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[2].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[3].m_type == 2);
//		}

		[NUnit.Framework.Test]
		public static void TestImportExportWktMultiPolygon()
		{
			com.epl.geometry.OperatorImportFromWkt importerWKT = (com.epl.geometry.OperatorImportFromWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkt);
			com.epl.geometry.OperatorExportToWkt exporterWKT = (com.epl.geometry.OperatorExportToWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkt);
			com.epl.geometry.Polygon polygon;
			string wktString;
			com.epl.geometry.Envelope2D envelope = new com.epl.geometry.Envelope2D();
			com.epl.geometry.WktParser wktParser = new com.epl.geometry.WktParser();
			// Test Import from MultiPolygon
			wktString = "Multipolygon M empty";
			polygon = (com.epl.geometry.Polygon)importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Polygon, wktString, null);
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.IsEmpty());
			NUnit.Framework.Assert.IsTrue(polygon.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			polygon = (com.epl.geometry.Polygon)com.epl.geometry.GeometryEngine.GeometryFromWkt(wktString, 0, com.epl.geometry.Geometry.Type.Unknown);
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.IsEmpty());
			NUnit.Framework.Assert.IsTrue(polygon.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			wktString = exporterWKT.Execute(0, polygon, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOLYGON M EMPTY"));
			wktString = com.epl.geometry.GeometryEngine.GeometryToWkt(polygon, 0);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOLYGON M EMPTY"));
			wktString = "Multipolygon Z (empty, (empty, (10 10 5, 20 10 5, 20 20 5, 10 20 5, 10 10 5), (12 12 3), empty, (10 10 1, 12 12 1)), empty, ((90 90 88, 60 90 7, 60 60 7), empty, (70 70 7, 80 80 7, 70 80 7, 70 70 7)), empty)";
			polygon = (com.epl.geometry.Polygon)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Polygon, wktString, null));
			NUnit.Framework.Assert.IsTrue(polygon != null);
			polygon.QueryEnvelope2D(envelope);
			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 90 && envelope.ymin == 10 && envelope.ymax == 90);
			NUnit.Framework.Assert.IsTrue(polygon.GetPointCount() == 14);
			NUnit.Framework.Assert.IsTrue(polygon.GetPathCount() == 5);
			// assertTrue(polygon.calculate_area_2D() > 0.0);
			NUnit.Framework.Assert.IsTrue(polygon.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			double z = polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0);
			NUnit.Framework.Assert.IsTrue(z == 5);
			// Test Export to WKT MultiPolygon
			wktString = exporterWKT.Execute(0, polygon, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOLYGON Z (((10 10 5, 20 10 5, 20 20 5, 10 20 5, 10 10 5), (12 12 3, 12 12 3, 12 12 3), (10 10 1, 12 12 1, 10 10 1)), ((90 90 88, 60 90 7, 60 60 7, 90 90 88), (70 70 7, 70 80 7, 80 80 7, 70 70 7)))"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			// Test import Polygon
			wktString = "POLYGON z (EMPTY, EMPTY, (10 10 5, 10 20 5, 20 20 5, 20 10 5), (12 12 3), EMPTY, (10 10 1, 12 12 1), EMPTY, (60 60 7, 60 90 7, 90 90 7, 60 60 7), EMPTY, (70 70 7, 70 80 7, 80 80 7), EMPTY)";
			polygon = (com.epl.geometry.Polygon)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Polygon, wktString, null));
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.GetPointCount() == 14);
			NUnit.Framework.Assert.IsTrue(polygon.GetPathCount() == 5);
			NUnit.Framework.Assert.IsTrue(polygon.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			// Test Export to WKT Polygon
			wktString = exporterWKT.Execute(com.epl.geometry.WktExportFlags.wktExportPolygon, polygon, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("POLYGON Z ((10 10 5, 20 10 5, 20 20 5, 10 20 5, 10 10 5), (12 12 3, 12 12 3, 12 12 3), (10 10 1, 12 12 1, 10 10 1), (60 60 7, 60 90 7, 90 90 7, 60 60 7), (70 70 7, 70 80 7, 80 80 7, 70 70 7))"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope();
			env.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			polygon.QueryEnvelope(env);
			wktString = exporterWKT.Execute(0, env, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("POLYGON Z ((10 10 1, 90 10 7, 90 90 1, 10 90 7, 10 10 1))"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			wktString = exporterWKT.Execute(com.epl.geometry.WktExportFlags.wktExportMultiPolygon, env, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOLYGON Z (((10 10 1, 90 10 7, 90 90 1, 10 90 7, 10 10 1)))"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			env.SetEmpty();
			wktString = exporterWKT.Execute(0, env, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("POLYGON Z EMPTY"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			wktString = exporterWKT.Execute(com.epl.geometry.WktExportFlags.wktExportMultiPolygon, env, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOLYGON Z EMPTY"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			wktString = "MULTIPOLYGON (((5 10, 8 10, 10 10, 10 0, 0 0, 0 10, 2 10, 5 10)))";
			// ring
			// is
			// oriented
			// clockwise
			polygon = (com.epl.geometry.Polygon)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Polygon, wktString, null));
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.CalculateArea2D() > 0);
			wktString = "MULTIPOLYGON Z (((90 10 7, 10 10 1, 10 90 7, 90 90 1, 90 10 7)))";
			// ring
			// is
			// oriented
			// clockwise
			polygon = (com.epl.geometry.Polygon)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Polygon, wktString, null));
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.GetPointCount() == 4);
			NUnit.Framework.Assert.IsTrue(polygon.GetPathCount() == 1);
			NUnit.Framework.Assert.IsTrue(polygon.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			NUnit.Framework.Assert.IsTrue(polygon.CalculateArea2D() > 0);
			wktString = exporterWKT.Execute(com.epl.geometry.WktExportFlags.wktExportMultiPolygon, polygon, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOLYGON Z (((90 10 7, 90 90 1, 10 90 7, 10 10 1, 90 10 7)))"));
		}

		[NUnit.Framework.Test]
		public static void TestImportExportWktPolygon()
		{
			com.epl.geometry.OperatorImportFromWkt importerWKT = (com.epl.geometry.OperatorImportFromWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkt);
			// OperatorExportToWkt exporterWKT =
			// (OperatorExportToWkt)OperatorFactoryLocal.getInstance().getOperator(Operator.Type.ExportToWkt);
			com.epl.geometry.Polygon polygon;
			string wktString;
			com.epl.geometry.Envelope2D envelope = new com.epl.geometry.Envelope2D();
			// Test Import from Polygon
			wktString = "Polygon ZM empty";
			polygon = (com.epl.geometry.Polygon)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.IsEmpty());
			NUnit.Framework.Assert.IsTrue(polygon.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			NUnit.Framework.Assert.IsTrue(polygon.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			wktString = "Polygon z (empty, (10 10 5, 20 10 5, 20 20 5, 10 20 5, 10 10 5), (12 12 3), empty, (10 10 1, 12 12 1))";
			polygon = (com.epl.geometry.Polygon)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polygon != null);
			polygon.QueryEnvelope2D(envelope);
			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 20 && envelope.ymin == 10 && envelope.ymax == 20);
			NUnit.Framework.Assert.IsTrue(polygon.GetPointCount() == 8);
			NUnit.Framework.Assert.IsTrue(polygon.GetPathCount() == 3);
			NUnit.Framework.Assert.IsTrue(polygon.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			wktString = "polygon ((35 10, 10 20, 15 40, 45 45, 35 10), (20 30, 35 35, 30 20, 20 30))";
			com.epl.geometry.Polygon polygon2 = (com.epl.geometry.Polygon)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polygon2 != null);
		}

		// wktString = exporterWKT.execute(0, *polygon2, null);
		[NUnit.Framework.Test]
		public static void TestImportExportWktLineString()
		{
			com.epl.geometry.OperatorImportFromWkt importerWKT = (com.epl.geometry.OperatorImportFromWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkt);
			// OperatorExportToWkt exporterWKT =
			// (OperatorExportToWkt)OperatorFactoryLocal.getInstance().getOperator(Operator.Type.ExportToWkt);
			com.epl.geometry.Polyline polyline;
			string wktString;
			com.epl.geometry.Envelope2D envelope = new com.epl.geometry.Envelope2D();
			// Test Import from LineString
			wktString = "LineString ZM empty";
			polyline = (com.epl.geometry.Polyline)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polyline != null);
			NUnit.Framework.Assert.IsTrue(polyline.IsEmpty());
			NUnit.Framework.Assert.IsTrue(polyline.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			NUnit.Framework.Assert.IsTrue(polyline.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			wktString = "LineString m (10 10 5, 10 20 5, 20 20 5, 20 10 5)";
			polyline = (com.epl.geometry.Polyline)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polyline != null);
			polyline.QueryEnvelope2D(envelope);
			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 20 && envelope.ymin == 10 && envelope.ymax == 20);
			NUnit.Framework.Assert.IsTrue(polyline.GetPointCount() == 4);
			NUnit.Framework.Assert.IsTrue(polyline.GetPathCount() == 1);
			NUnit.Framework.Assert.IsTrue(polyline.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
		}

		[NUnit.Framework.Test]
		public static void TestImportExportWktMultiLineString()
		{
			com.epl.geometry.OperatorImportFromWkt importerWKT = (com.epl.geometry.OperatorImportFromWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkt);
			com.epl.geometry.OperatorExportToWkt exporterWKT = (com.epl.geometry.OperatorExportToWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkt);
			com.epl.geometry.Polyline polyline;
			string wktString;
			com.epl.geometry.Envelope2D envelope = new com.epl.geometry.Envelope2D();
			com.epl.geometry.WktParser wktParser = new com.epl.geometry.WktParser();
			// Test Import from MultiLineString
			wktString = "MultiLineStringZMempty";
			polyline = (com.epl.geometry.Polyline)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polyline != null);
			NUnit.Framework.Assert.IsTrue(polyline.IsEmpty());
			NUnit.Framework.Assert.IsTrue(polyline.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			NUnit.Framework.Assert.IsTrue(polyline.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			wktString = "MultiLineStringm(empty, empty, (10 10 5, 10 20 5, 20 88 5, 20 10 5), (12 88 3), empty, (10 10 1, 12 12 1), empty, (88 60 7, 60 90 7, 90 90 7), empty, (70 70 7, 70 80 7, 80 80 7), empty)";
			polyline = (com.epl.geometry.Polyline)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polyline != null);
			polyline.QueryEnvelope2D(envelope);
			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 90 && envelope.ymin == 10 && envelope.ymax == 90);
			NUnit.Framework.Assert.IsTrue(polyline.GetPointCount() == 14);
			NUnit.Framework.Assert.IsTrue(polyline.GetPathCount() == 5);
			NUnit.Framework.Assert.IsTrue(polyline.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			wktString = exporterWKT.Execute(0, polyline, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTILINESTRING M ((10 10 5, 10 20 5, 20 88 5, 20 10 5), (12 88 3, 12 88 3), (10 10 1, 12 12 1), (88 60 7, 60 90 7, 90 90 7), (70 70 7, 70 80 7, 80 80 7))"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			// Test Import LineString
			wktString = "Linestring Z(10 10 5, 10 20 5, 20 20 5, 20 10 5)";
			polyline = (com.epl.geometry.Polyline)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polyline.GetPointCount() == 4);
			wktString = exporterWKT.Execute(com.epl.geometry.WktExportFlags.wktExportLineString, polyline, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("LINESTRING Z (10 10 5, 10 20 5, 20 20 5, 20 10 5)"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			wktString = exporterWKT.Execute(0, polyline, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTILINESTRING Z ((10 10 5, 10 20 5, 20 20 5, 20 10 5))"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
		}

		[NUnit.Framework.Test]
		public static void TestImportExportWktMultiPoint()
		{
			com.epl.geometry.OperatorImportFromWkt importerWKT = (com.epl.geometry.OperatorImportFromWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkt);
			com.epl.geometry.OperatorExportToWkt exporterWKT = (com.epl.geometry.OperatorExportToWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkt);
			com.epl.geometry.MultiPoint multipoint;
			string wktString;
			com.epl.geometry.Envelope2D envelope = new com.epl.geometry.Envelope2D();
			com.epl.geometry.WktParser wktParser = new com.epl.geometry.WktParser();
			// Test Import from Multi_point
			wktString = "  MultiPoint ZM empty";
			multipoint = (com.epl.geometry.MultiPoint)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(multipoint != null);
			NUnit.Framework.Assert.IsTrue(multipoint.IsEmpty());
			NUnit.Framework.Assert.IsTrue(multipoint.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			NUnit.Framework.Assert.IsTrue(multipoint.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			wktString = exporterWKT.Execute(0, multipoint, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOINT ZM EMPTY"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			wktString = exporterWKT.Execute(com.epl.geometry.WktExportFlags.wktExportPoint, multipoint, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("POINT ZM EMPTY"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			multipoint = new com.epl.geometry.MultiPoint();
			multipoint.Add(118.15114354234563, 33.82234433423462345);
			multipoint.Add(88, 88);
			wktString = exporterWKT.Execute(com.epl.geometry.WktExportFlags.wktExportPrecision10, multipoint, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOINT ((118.1511435 33.82234433), (88 88))"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			multipoint = new com.epl.geometry.MultiPoint();
			multipoint.Add(88, 2);
			multipoint.Add(88, 88);
			wktString = exporterWKT.Execute(0, multipoint, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOINT ((88 2), (88 88))"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			wktString = "Multipoint zm (empty, empty, (10 88 88 33), (10 20 5 33), (20 20 5 33), (20 10 5 33), (12 12 3 33), empty, (10 10 1 33), (12 12 1 33), empty, (60 60 7 33), (60 90.1 7 33), (90 90 7 33), empty, (70 70 7 33), (70 80 7 33), (80 80 7 33), empty)";
			multipoint = (com.epl.geometry.MultiPoint)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(multipoint != null);
			multipoint.QueryEnvelope2D(envelope);
			// assertTrue(envelope.xmin == 10 && envelope.xmax == 90 &&
			// envelope.ymin == 10 && Math.abs(envelope.ymax - 90.1) <= 0.001);
			NUnit.Framework.Assert.IsTrue(multipoint.GetPointCount() == 13);
			NUnit.Framework.Assert.IsTrue(multipoint.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			NUnit.Framework.Assert.IsTrue(multipoint.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			wktString = "Multipoint zm (10 88 88 33, 10 20 5 33, 20 20 5 33, 20 10 5 33, 12 12 3 33, 10 10 1 33, 12 12 1 33, 60 60 7 33, 60 90.1 7 33, 90 90 7 33, 70 70 7 33, 70 80 7 33, 80 80 7 33)";
			multipoint = (com.epl.geometry.MultiPoint)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(multipoint != null);
			// assertTrue(envelope.xmin == 10 && envelope.xmax == 90 &&
			// envelope.ymin == 10 && ::fabs(envelope.ymax - 90.1) <= 0.001);
			NUnit.Framework.Assert.IsTrue(multipoint.GetPointCount() == 13);
			NUnit.Framework.Assert.IsTrue(multipoint.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			NUnit.Framework.Assert.IsTrue(multipoint.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			wktString = exporterWKT.Execute(com.epl.geometry.WktExportFlags.wktExportPrecision15, multipoint, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOINT ZM ((10 88 88 33), (10 20 5 33), (20 20 5 33), (20 10 5 33), (12 12 3 33), (10 10 1 33), (12 12 1 33), (60 60 7 33), (60 90.1 7 33), (90 90 7 33), (70 70 7 33), (70 80 7 33), (80 80 7 33))"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			wktString = "Multipoint zm (empty, empty, (10 10 5 33))";
			multipoint = (com.epl.geometry.MultiPoint)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Unknown, wktString, null));
			wktString = exporterWKT.Execute(com.epl.geometry.WktExportFlags.wktExportPoint, multipoint, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("POINT ZM (10 10 5 33)"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
		}

		[NUnit.Framework.Test]
		public static void TestImportExportWktPoint()
		{
			com.epl.geometry.OperatorImportFromWkt importerWKT = (com.epl.geometry.OperatorImportFromWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkt);
			com.epl.geometry.OperatorExportToWkt exporterWKT = (com.epl.geometry.OperatorExportToWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkt);
			com.epl.geometry.Point point;
			string wktString;
			com.epl.geometry.WktParser wktParser = new com.epl.geometry.WktParser();
			// Test Import from Point
			wktString = "Point ZM empty";
			point = (com.epl.geometry.Point)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(point != null);
			NUnit.Framework.Assert.IsTrue(point.IsEmpty());
			NUnit.Framework.Assert.IsTrue(point.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			NUnit.Framework.Assert.IsTrue(point.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			wktString = exporterWKT.Execute(0, point, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("POINT ZM EMPTY"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			wktString = exporterWKT.Execute(com.epl.geometry.WktExportFlags.wktExportMultiPoint, point, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOINT ZM EMPTY"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			wktString = "Point zm (30.1 10.6 5.1 33.1)";
			point = (com.epl.geometry.Point)(importerWKT.Execute(0, com.epl.geometry.Geometry.Type.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(point != null);
			NUnit.Framework.Assert.IsTrue(point.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			NUnit.Framework.Assert.IsTrue(point.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			double x = point.GetX();
			double y = point.GetY();
			double z = point.GetZ();
			double m = point.GetM();
			NUnit.Framework.Assert.IsTrue(x == 30.1);
			NUnit.Framework.Assert.IsTrue(y == 10.6);
			NUnit.Framework.Assert.IsTrue(z == 5.1);
			NUnit.Framework.Assert.IsTrue(m == 33.1);
			wktString = exporterWKT.Execute(com.epl.geometry.WktExportFlags.wktExportPrecision15, point, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("POINT ZM (30.1 10.6 5.1 33.1)"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
			wktString = exporterWKT.Execute(com.epl.geometry.WktExportFlags.wktExportMultiPoint | com.epl.geometry.WktExportFlags.wktExportPrecision15, point, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOINT ZM ((30.1 10.6 5.1 33.1))"));
			wktParser.ResetParser(wktString);
			while (wktParser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
			}
		}

//		/// <exception cref="org.json.JSONException"/>
//		[NUnit.Framework.Test]
//		public static void TestImportGeoJsonGeometryCollection()
//		{
//			com.epl.geometry.OperatorImportFromGeoJson importer = (com.epl.geometry.OperatorImportFromGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromGeoJson);
//			string geoJsonString;
//			com.epl.geometry.Envelope2D envelope = new com.epl.geometry.Envelope2D();
//			com.epl.geometry.WktParser wktParser = new com.epl.geometry.WktParser();
//			geoJsonString = "{\"type\" : \"GeometryCollection\", \"geometries\" : [{\"type\" : \"Point\", \"coordinates\": [0,0]},  {\"type\" : \"GeometryCollection\" , \"geometries\" : [ {\"type\" : \"Point\", \"coordinates\" : [0, 0]} ,  {\"type\" : \"Point\", \"coordinates\" : [1, 1]} ,{ \"type\" : \"Point\", \"coordinates\" : [2, 2]}, {\"type\" : \"LineString\", \"coordinates\" :  []}]} , {\"type\" : \"Point\", \"coordinates\" : [1, 1]},  {\"type\" : \"Point\" , \"coordinates\" : [2, 2]} ] }";
//			com.epl.geometry.OGCStructure structure = importer.ExecuteOGC(0, geoJsonString, null).m_ogcStructure.m_structures[0];
//			NUnit.Framework.Assert.IsTrue(structure.m_type == 7);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[0].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[0].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_type == 7);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[2].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[3].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[0].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[1].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[2].m_type == 1);
//			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[3].m_type == 2);
//		}
//
//		/// <exception cref="org.json.JSONException"/>
//		[NUnit.Framework.Test]
//		public static void TestImportGeoJsonMultiPolygon()
//		{
//			com.epl.geometry.OperatorImportFromGeoJson importerGeoJson = (com.epl.geometry.OperatorImportFromGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromGeoJson);
//			com.epl.geometry.Polygon polygon;
//			string geoJsonString;
//			com.epl.geometry.Envelope2D envelope = new com.epl.geometry.Envelope2D();
//			// Test Import from MultiPolygon
//			geoJsonString = "{\"type\": \"Multipolygon\", \"coordinates\": []}";
//			polygon = (com.epl.geometry.Polygon)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Polygon, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(polygon != null);
//			NUnit.Framework.Assert.IsTrue(polygon.IsEmpty());
//			NUnit.Framework.Assert.IsTrue(!polygon.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
//			polygon = (com.epl.geometry.Polygon)(com.epl.geometry.GeometryEngine.GeometryFromGeoJson(geoJsonString, 0, com.epl.geometry.Geometry.Type.Unknown).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(polygon != null);
//			NUnit.Framework.Assert.IsTrue(polygon.IsEmpty());
//			NUnit.Framework.Assert.IsTrue(!polygon.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
//			geoJsonString = "{\"type\": \"Multipolygon\", \"coordinates\": [[], [[], [[10, 10, 5], [20, 10, 5], [20, 20, 5], [10, 20, 5], [10, 10, 5]], [[12, 12, 3]], [], [[10, 10, 1], [12, 12, 1]]], [], [[[90, 90, 88], [60, 90, 7], [60, 60, 7]], [], [[70, 70, 7], [80, 80, 7], [70, 80, 7], [70, 70, 7]]], []]}";
//			polygon = (com.epl.geometry.Polygon)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Polygon, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(polygon != null);
//			polygon.QueryEnvelope2D(envelope);
//			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 90 && envelope.ymin == 10 && envelope.ymax == 90);
//			NUnit.Framework.Assert.IsTrue(polygon.GetPointCount() == 14);
//			NUnit.Framework.Assert.IsTrue(polygon.GetPathCount() == 5);
//			// assertTrue(polygon.calculate_area_2D() > 0.0);
//			// assertTrue(polygon.hasAttribute(VertexDescription.Semantics.Z));
//			// double z = polygon.getAttributeAsDbl(VertexDescription.Semantics.Z,
//			// 0, 0);
//			// assertTrue(z == 5);
//			// Test import Polygon
//			geoJsonString = "{\"type\": \"POLYGON\", \"coordinates\": [[], [], [[10, 10, 5], [10, 20, 5], [20, 20, 5], [20, 10, 5]], [[12, 12, 3]], [], [[10, 10, 1], [12, 12, 1]], [], [[60, 60, 7], [60, 90, 7], [90, 90, 7], [60, 60, 7]], [], [[70, 70, 7], [70, 80, 7], [80, 80, 7]], []] }";
//			polygon = (com.epl.geometry.Polygon)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Polygon, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(polygon != null);
//			NUnit.Framework.Assert.IsTrue(polygon.GetPointCount() == 14);
//			NUnit.Framework.Assert.IsTrue(polygon.GetPathCount() == 5);
//			// assertTrue(polygon.hasAttribute(VertexDescription.Semantics.Z));
//			geoJsonString = "{\"type\": \"MULTIPOLYGON\", \"coordinates\": [[[[90, 10, 7], [10, 10, 1], [10, 90, 7], [90, 90, 1], [90, 10, 7]]]] }";
//			// ring
//			// is
//			// oriented
//			// clockwise
//			polygon = (com.epl.geometry.Polygon)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Polygon, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(polygon != null);
//			NUnit.Framework.Assert.IsTrue(polygon.GetPointCount() == 4);
//			NUnit.Framework.Assert.IsTrue(polygon.GetPathCount() == 1);
//			// assertTrue(polygon.hasAttribute(VertexDescription.Semantics.Z));
//			NUnit.Framework.Assert.IsTrue(polygon.CalculateArea2D() > 0);
//		}
//
//		/// <exception cref="org.json.JSONException"/>
//		[NUnit.Framework.Test]
//		public static void TestImportGeoJsonMultiLineString()
//		{
//			com.epl.geometry.OperatorImportFromGeoJson importerGeoJson = (com.epl.geometry.OperatorImportFromGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromGeoJson);
//			com.epl.geometry.Polyline polyline;
//			string geoJsonString;
//			com.epl.geometry.Envelope2D envelope = new com.epl.geometry.Envelope2D();
//			// Test Import from MultiLineString
//			geoJsonString = "{\"type\": \"MultiLineString\", \"coordinates\": []}";
//			polyline = (com.epl.geometry.Polyline)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(polyline != null);
//			NUnit.Framework.Assert.IsTrue(polyline.IsEmpty());
//			NUnit.Framework.Assert.IsTrue(!polyline.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
//			NUnit.Framework.Assert.IsTrue(!polyline.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
//			geoJsonString = "{\"type\": \"MultiLineString\", \"coordinates\": [[], [], [[10, 10, 5], [10, 20, 5], [20, 88, 5], [20, 10, 5]], [[12, 88, 3]], [], [[10, 10, 1], [12, 12, 1]], [], [[88, 60, 7], [60, 90, 7], [90, 90, 7]], [], [[70, 70, 7], [70, 80, 7], [80, 80, 7]], []]}";
//			polyline = (com.epl.geometry.Polyline)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(polyline != null);
//			polyline.QueryEnvelope2D(envelope);
//			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 90 && envelope.ymin == 10 && envelope.ymax == 90);
//			NUnit.Framework.Assert.IsTrue(polyline.GetPointCount() == 14);
//			NUnit.Framework.Assert.IsTrue(polyline.GetPathCount() == 5);
//			// assertTrue(!polyline.hasAttribute(VertexDescription.Semantics.M));
//			// Test Import LineString
//			geoJsonString = "{\"type\": \"Linestring\", \"coordinates\": [[10, 10, 5], [10, 20, 5], [20, 20, 5], [20, 10, 5]]}";
//			polyline = (com.epl.geometry.Polyline)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(polyline.GetPointCount() == 4);
//			// assertTrue(polyline.hasAttribute(VertexDescription.Semantics.Z));
//			geoJsonString = "{\"type\": \"Linestring\", \"coordinates\": [[10, 10, 5], [10, 20, 5, 3], [20, 20, 5], [20, 10, 5]]}";
//			polyline = (com.epl.geometry.Polyline)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(polyline.GetPointCount() == 4);
//		}
//
//		// assertTrue(polyline.hasAttribute(VertexDescription.Semantics.Z));
//		// assertTrue(polyline.hasAttribute(VertexDescription.Semantics.M));
//		/// <exception cref="org.json.JSONException"/>
//		[NUnit.Framework.Test]
//		public static void TestImportGeoJsonMultiPoint()
//		{
//			com.epl.geometry.OperatorImportFromGeoJson importerGeoJson = (com.epl.geometry.OperatorImportFromGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromGeoJson);
//			com.epl.geometry.MultiPoint multipoint;
//			string geoJsonString;
//			com.epl.geometry.Envelope2D envelope = new com.epl.geometry.Envelope2D();
//			// Test Import from Multi_point
//			geoJsonString = "{\"type\": \"MultiPoint\", \"coordinates\": []}";
//			multipoint = (com.epl.geometry.MultiPoint)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(multipoint != null);
//			NUnit.Framework.Assert.IsTrue(multipoint.IsEmpty());
//			NUnit.Framework.Assert.IsTrue(!multipoint.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
//			NUnit.Framework.Assert.IsTrue(!multipoint.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
//			multipoint = new com.epl.geometry.MultiPoint();
//			multipoint.Add(118.15114354234563, 33.82234433423462345);
//			multipoint.Add(88, 88);
//			multipoint = new com.epl.geometry.MultiPoint();
//			multipoint.Add(88, 2);
//			multipoint.Add(88, 88);
//			geoJsonString = "{\"type\": \"Multipoint\", \"coordinates\": [[], [], [10, 88, 88, 33], [10, 20, 5, 33], [20, 20, 5, 33], [20, 10, 5, 33], [12, 12, 3, 33], [], [10, 10, 1, 33], [12, 12, 1, 33], [], [60, 60, 7, 33], [60, 90.1, 7, 33], [90, 90, 7, 33], [], [70, 70, 7, 33], [70, 80, 7, 33], [80, 80, 7, 33], []]}";
//			multipoint = (com.epl.geometry.MultiPoint)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(multipoint != null);
//			multipoint.QueryEnvelope2D(envelope);
//			// assertTrue(envelope.xmin == 10 && envelope.xmax == 90 &&
//			// envelope.ymin == 10 && Math.abs(envelope.ymax - 90.1) <= 0.001);
//			NUnit.Framework.Assert.IsTrue(multipoint.GetPointCount() == 13);
//			// assertTrue(multipoint.hasAttribute(VertexDescription.Semantics.Z));
//			// assertTrue(multipoint.hasAttribute(VertexDescription.Semantics.M));
//			geoJsonString = "{\"type\": \"Multipoint\", \"coordinates\": [[10, 88, 88, 33], [10, 20, 5, 33], [20, 20, 5, 33], [20, 10, 5, 33], [12, 12, 3, 33], [10, 10, 1, 33], [12, 12, 1, 33], [60, 60, 7, 33], [60, 90.1, 7, 33], [90, 90, 7, 33], [70, 70, 7, 33], [70, 80, 7, 33], [80, 80, 7, 33]]}";
//			multipoint = (com.epl.geometry.MultiPoint)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(multipoint != null);
//			// assertTrue(envelope.xmin == 10 && envelope.xmax == 90 &&
//			// envelope.ymin == 10 && ::fabs(envelope.ymax - 90.1) <= 0.001);
//			NUnit.Framework.Assert.IsTrue(multipoint.GetPointCount() == 13);
//			// assertTrue(multipoint.hasAttribute(VertexDescription.Semantics.Z));
//			// assertTrue(multipoint.hasAttribute(VertexDescription.Semantics.M));
//			geoJsonString = "{\"type\": \"Multipoint\", \"coordinates\": [[], [], [10, 10, 5, 33]]}";
//			multipoint = (com.epl.geometry.MultiPoint)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString, null).GetGeometry());
//		}
//
//		/// <exception cref="org.json.JSONException"/>
//		[NUnit.Framework.Test]
//		public static void TestImportGeoJsonPolygon()
//		{
//			com.epl.geometry.OperatorImportFromGeoJson importerGeoJson = (com.epl.geometry.OperatorImportFromGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromGeoJson);
//			com.epl.geometry.Polygon polygon;
//			string geoJsonString;
//			com.epl.geometry.Envelope2D envelope = new com.epl.geometry.Envelope2D();
//			// Test Import from Polygon
//			geoJsonString = "{\"type\": \"Polygon\", \"coordinates\": []}";
//			polygon = (com.epl.geometry.Polygon)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(polygon != null);
//			NUnit.Framework.Assert.IsTrue(polygon.IsEmpty());
//			NUnit.Framework.Assert.IsTrue(!polygon.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
//			NUnit.Framework.Assert.IsTrue(!polygon.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
//			geoJsonString = "{\"type\": \"Polygon\", \"coordinates\": [[], [[10, 10, 5], [20, 10, 5], [20, 20, 5], [10, 20, 5], [10, 10, 5]], [[12, 12, 3]], [], [[10, 10, 1], [12, 12, 1]]]}";
//			polygon = (com.epl.geometry.Polygon)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(polygon != null);
//			polygon.QueryEnvelope2D(envelope);
//			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 20 && envelope.ymin == 10 && envelope.ymax == 20);
//			NUnit.Framework.Assert.IsTrue(polygon.GetPointCount() == 8);
//			NUnit.Framework.Assert.IsTrue(polygon.GetPathCount() == 3);
//			// assertTrue(polygon.hasAttribute(VertexDescription.Semantics.Z));
//			geoJsonString = "{\"type\": \"polygon\", \"coordinates\": [[[35, 10], [10, 20], [15, 40], [45, 45], [35, 10]], [[20, 30], [35, 35], [30, 20], [20, 30]]]}";
//			com.epl.geometry.Polygon polygon2 = (com.epl.geometry.Polygon)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(polygon2 != null);
//		}
//
//		/// <exception cref="org.json.JSONException"/>
//		[NUnit.Framework.Test]
//		public static void TestImportGeoJsonLineString()
//		{
//			com.epl.geometry.OperatorImportFromGeoJson importerGeoJson = (com.epl.geometry.OperatorImportFromGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromGeoJson);
//			com.epl.geometry.Polyline polyline;
//			string geoJsonString;
//			com.epl.geometry.Envelope2D envelope = new com.epl.geometry.Envelope2D();
//			// Test Import from LineString
//			geoJsonString = "{\"type\": \"LineString\", \"coordinates\": []}";
//			polyline = (com.epl.geometry.Polyline)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(polyline != null);
//			NUnit.Framework.Assert.IsTrue(polyline.IsEmpty());
//			NUnit.Framework.Assert.IsTrue(!polyline.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
//			NUnit.Framework.Assert.IsTrue(!polyline.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
//			geoJsonString = "{\"type\": \"LineString\", \"coordinates\": [[10, 10, 5], [10, 20, 5], [20, 20, 5], [20, 10, 5]]}";
//			polyline = (com.epl.geometry.Polyline)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(polyline != null);
//			polyline.QueryEnvelope2D(envelope);
//			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 20 && envelope.ymin == 10 && envelope.ymax == 20);
//			NUnit.Framework.Assert.IsTrue(polyline.GetPointCount() == 4);
//			NUnit.Framework.Assert.IsTrue(polyline.GetPathCount() == 1);
//		}
//
//		// assertTrue(!polyline.hasAttribute(VertexDescription.Semantics.M));
//		/// <exception cref="org.json.JSONException"/>
//		[NUnit.Framework.Test]
//		public static void TestImportGeoJsonPoint()
//		{
//			com.epl.geometry.OperatorImportFromGeoJson importerGeoJson = (com.epl.geometry.OperatorImportFromGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromGeoJson);
//			com.epl.geometry.Point point;
//			string geoJsonString;
//			// Test Import from Point
//			geoJsonString = "{\"type\": \"Point\", \"coordinates\": []}";
//			point = (com.epl.geometry.Point)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(point != null);
//			NUnit.Framework.Assert.IsTrue(point.IsEmpty());
//			NUnit.Framework.Assert.IsTrue(!point.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
//			NUnit.Framework.Assert.IsTrue(!point.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
//			geoJsonString = "{\"type\": \"Point\", \"coordinates\": [30.1, 10.6, 5.1, 33.1]}";
//			point = (com.epl.geometry.Point)(importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString, null).GetGeometry());
//			NUnit.Framework.Assert.IsTrue(point != null);
//			// assertTrue(point.hasAttribute(VertexDescription.Semantics.Z));
//			// assertTrue(point.hasAttribute(VertexDescription.Semantics.M));
//			double x = point.GetX();
//			double y = point.GetY();
//			// double z = point.getZ();
//			// double m = point.getM();
//			NUnit.Framework.Assert.IsTrue(x == 30.1);
//			NUnit.Framework.Assert.IsTrue(y == 10.6);
//		}
//
//		// assertTrue(z == 5.1);
//		// assertTrue(m == 33.1);
//		/// <exception cref="org.json.JSONException"/>
//		[NUnit.Framework.Test]
//		public static void TestImportGeoJsonSpatialReference()
//		{
//			com.epl.geometry.OperatorImportFromGeoJson importerGeoJson = (com.epl.geometry.OperatorImportFromGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromGeoJson);
//			string geoJsonString4326;
//			string geoJsonString3857;
//			// Test Import from Point
//			geoJsonString4326 = "{\"type\": \"Point\", \"coordinates\": [3.0, 5.0], \"crs\": \"EPSG:4326\"}";
//			geoJsonString3857 = "{\"type\": \"Point\", \"coordinates\": [3.0, 5.0], \"crs\": \"EPSG:3857\"}";
//			com.epl.geometry.MapGeometry mapGeometry4326 = importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString4326, null);
//			com.epl.geometry.MapGeometry mapGeometry3857 = importerGeoJson.Execute(0, com.epl.geometry.Geometry.Type.Unknown, geoJsonString3857, null);
//			NUnit.Framework.Assert.IsTrue(mapGeometry4326.Equals(mapGeometry3857) == false);
//			NUnit.Framework.Assert.IsTrue(mapGeometry4326.GetGeometry().Equals(mapGeometry3857.GetGeometry()));
//		}

		public static com.epl.geometry.Polygon MakePolygon()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 10);
			poly.LineTo(10, 10);
			poly.LineTo(10, 0);
			poly.StartPath(3, 3);
			poly.LineTo(7, 3);
			poly.LineTo(7, 7);
			poly.LineTo(3, 7);
			poly.StartPath(15, 0);
			poly.LineTo(15, 15);
			poly.LineTo(30, 15);
			poly.LineTo(30, 0);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0, 2);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0, 3);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0, 5);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 3, 0, 7);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 4, 0, 11);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 5, 0, 13);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 6, 0, 17);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 7, 0, 19);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 8, 0, 23);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 9, 0, 29);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 10, 0, 31);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 11, 0, 37);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 0, 2);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, 4);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 2, 0, 8);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 3, 0, 16);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 4, 0, 32);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 5, 0, 64);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 6, 0, 128);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 7, 0, 256);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 8, 0, 512);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 9, 0, 1024);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 10, 0, 2048);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 11, 0, 4096);
			return poly;
		}

		public static com.epl.geometry.Polygon MakePolygon2()
		{
			com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
			poly.StartPath(0, 0);
			poly.LineTo(0, 10);
			poly.LineTo(10, 10);
			poly.LineTo(10, 0);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0, 2);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0, 3);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0, 5);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 3, 0, 7);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 0, 2);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, 4);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 2, 0, 8);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 3, 0, 16);
			return poly;
		}

		public static com.epl.geometry.Polyline MakePolyline()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(30, 14);
			poly.LineTo(60, 144);
			poly.StartPath(20, 13);
			poly.LineTo(150, 120);
			poly.LineTo(300, 414);
			poly.LineTo(610, 14);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0, 2);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0, 3);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0, 5);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 3, 0, 7);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 4, 0, 11);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 5, 0, 13);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 6, 0, 17);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 7, 0, 19);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 0, 2);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, 4);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 2, 0, 8);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 3, 0, 16);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 4, 0, 32);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 5, 0, 64);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 6, 0, 128);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 7, 0, 256);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0, 1);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 1, 0, 2);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 2, 0, 3);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 3, 0, 5);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 4, 0, 8);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 5, 0, 13);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 6, 0, 21);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 7, 0, 34);
			return poly;
		}

		public static com.epl.geometry.Polyline MakePolyline2()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			poly.StartPath(10, 1);
			poly.LineTo(15, 20);
			poly.LineTo(30, 14);
			poly.LineTo(60, 144);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0, 2);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0, 3);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0, 5);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 3, 0, 7);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 0, 2);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, 4);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 2, 0, 8);
			poly.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 3, 0, 16);
			return poly;
		}

		public static com.epl.geometry.Point MakePoint()
		{
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			point.SetXY(11, 13);
			point.SetZ(32);
			point.SetM(243);
			point.SetID(1024);
			return point;
		}

		public static com.epl.geometry.MultiPoint MakeMultiPoint()
		{
			com.epl.geometry.MultiPoint mpoint = new com.epl.geometry.MultiPoint();
			com.epl.geometry.Point pt1 = new com.epl.geometry.Point();
			pt1.SetXY(0, 0);
			pt1.SetZ(-1);
			com.epl.geometry.Point pt2 = new com.epl.geometry.Point();
			pt2.SetXY(0, 0);
			pt2.SetZ(1);
			com.epl.geometry.Point pt3 = new com.epl.geometry.Point();
			pt3.SetXY(0, 1);
			pt3.SetZ(1);
			mpoint.Add(pt1);
			mpoint.Add(pt2);
			mpoint.Add(pt3);
			mpoint.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0, 7);
			mpoint.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 1, 0, 11);
			mpoint.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 2, 0, 13);
			return mpoint;
		}

		public static com.epl.geometry.MultiPoint MakeMultiPoint2()
		{
			com.epl.geometry.MultiPoint mpoint = new com.epl.geometry.MultiPoint();
			com.epl.geometry.Point pt1 = new com.epl.geometry.Point();
			pt1.SetX(0.0);
			pt1.SetY(0.0);
			pt1.SetZ(-1.0);
			mpoint.Add(pt1);
			return mpoint;
		}

		public static com.epl.geometry.Envelope MakeEnvelope()
		{
			com.epl.geometry.Envelope envelope;
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope(0.0, 0.0, 5.0, 5.0);
			envelope = env;
			com.epl.geometry.Envelope1D interval = new com.epl.geometry.Envelope1D();
			interval.vmin = -3.0;
			interval.vmax = -7.0;
			envelope.SetInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0, interval);
			interval.vmin = 16.0;
			interval.vmax = 32.0;
			envelope.SetInterval(com.epl.geometry.VertexDescription.Semantics.M, 0, interval);
			interval.vmin = 5.0;
			interval.vmax = 11.0;
			envelope.SetInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0, interval);
			return envelope;
		}
	}
}
