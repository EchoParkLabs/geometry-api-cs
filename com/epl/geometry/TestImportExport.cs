

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestImportExport
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
		public static void testImportExportShapePolygon()
		{
			com.esri.core.geometry.OperatorExportToESRIShape exporterShape = (com.esri.core.geometry.OperatorExportToESRIShape
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToESRIShape);
			com.esri.core.geometry.OperatorImportFromESRIShape importerShape = (com.esri.core.geometry.OperatorImportFromESRIShape
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromESRIShape);
			com.esri.core.geometry.Polygon polygon = makePolygon();
			byte[] esriShape = com.esri.core.geometry.GeometryEngine.geometryToEsriShape(polygon
				);
			com.esri.core.geometry.Geometry imported = com.esri.core.geometry.GeometryEngine.
				geometryFromEsriShape(esriShape, com.esri.core.geometry.Geometry.Type.Unknown);
			com.esri.core.geometry.TestCommonMethods.compareGeometryContent((com.esri.core.geometry.MultiPath
				)imported, polygon);
			// Test Import Polygon from Polygon
			java.nio.ByteBuffer polygonShapeBuffer = exporterShape.execute(0, polygon);
			com.esri.core.geometry.Geometry polygonShapeGeometry = importerShape.execute(0, com.esri.core.geometry.Geometry.Type
				.Polygon, polygonShapeBuffer);
			com.esri.core.geometry.TestCommonMethods.compareGeometryContent((com.esri.core.geometry.MultiPath
				)polygonShapeGeometry, polygon);
			// Test Import Envelope from Polygon
			com.esri.core.geometry.Geometry envelopeShapeGeometry = importerShape.execute(0, 
				com.esri.core.geometry.Geometry.Type.Envelope, polygonShapeBuffer);
			com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)envelopeShapeGeometry;
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope();
			com.esri.core.geometry.Envelope otherenv = new com.esri.core.geometry.Envelope();
			polygon.queryEnvelope(otherenv);
			NUnit.Framework.Assert.IsTrue(envelope.getXMin() == otherenv.getXMin());
			NUnit.Framework.Assert.IsTrue(envelope.getXMax() == otherenv.getXMax());
			NUnit.Framework.Assert.IsTrue(envelope.getYMin() == otherenv.getYMin());
			NUnit.Framework.Assert.IsTrue(envelope.getYMax() == otherenv.getYMax());
			com.esri.core.geometry.Envelope1D interval;
			com.esri.core.geometry.Envelope1D otherinterval;
			interval = envelope.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0);
			otherinterval = polygon.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0);
			NUnit.Framework.Assert.IsTrue(interval.vmin == otherinterval.vmin);
			NUnit.Framework.Assert.IsTrue(interval.vmax == otherinterval.vmax);
		}

		[NUnit.Framework.Test]
		public static void testImportExportShapePolyline()
		{
			com.esri.core.geometry.OperatorExportToESRIShape exporterShape = (com.esri.core.geometry.OperatorExportToESRIShape
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToESRIShape);
			com.esri.core.geometry.OperatorImportFromESRIShape importerShape = (com.esri.core.geometry.OperatorImportFromESRIShape
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromESRIShape);
			com.esri.core.geometry.Polyline polyline = makePolyline();
			// Test Import Polyline from Polyline
			java.nio.ByteBuffer polylineShapeBuffer = exporterShape.execute(0, polyline);
			com.esri.core.geometry.Geometry polylineShapeGeometry = importerShape.execute(0, 
				com.esri.core.geometry.Geometry.Type.Polyline, polylineShapeBuffer);
			// TODO test this
			com.esri.core.geometry.TestCommonMethods.compareGeometryContent((com.esri.core.geometry.MultiPath
				)polylineShapeGeometry, polyline);
			// Test Import Envelope from Polyline;
			com.esri.core.geometry.Geometry envelopeShapeGeometry = importerShape.execute(0, 
				com.esri.core.geometry.Geometry.Type.Envelope, polylineShapeBuffer);
			com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)envelopeShapeGeometry;
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope();
			com.esri.core.geometry.Envelope otherenv = new com.esri.core.geometry.Envelope();
			envelope.queryEnvelope(env);
			polyline.queryEnvelope(otherenv);
			NUnit.Framework.Assert.IsTrue(env.getXMin() == otherenv.getXMin());
			NUnit.Framework.Assert.IsTrue(env.getXMax() == otherenv.getXMax());
			NUnit.Framework.Assert.IsTrue(env.getYMin() == otherenv.getYMin());
			NUnit.Framework.Assert.IsTrue(env.getYMax() == otherenv.getYMax());
			com.esri.core.geometry.Envelope1D interval;
			com.esri.core.geometry.Envelope1D otherinterval;
			interval = envelope.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0);
			otherinterval = polyline.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0);
			NUnit.Framework.Assert.IsTrue(interval.vmin == otherinterval.vmin);
			NUnit.Framework.Assert.IsTrue(interval.vmax == otherinterval.vmax);
		}

		[NUnit.Framework.Test]
		public static void testImportExportShapeMultiPoint()
		{
			com.esri.core.geometry.OperatorExportToESRIShape exporterShape = (com.esri.core.geometry.OperatorExportToESRIShape
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToESRIShape);
			com.esri.core.geometry.OperatorImportFromESRIShape importerShape = (com.esri.core.geometry.OperatorImportFromESRIShape
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromESRIShape);
			com.esri.core.geometry.MultiPoint multipoint = makeMultiPoint();
			// Test Import MultiPoint from MultiPoint
			java.nio.ByteBuffer multipointShapeBuffer = exporterShape.execute(0, multipoint);
			com.esri.core.geometry.MultiPoint multipointShapeGeometry = (com.esri.core.geometry.MultiPoint
				)importerShape.execute(0, com.esri.core.geometry.Geometry.Type.MultiPoint, multipointShapeBuffer
				);
			com.esri.core.geometry.TestCommonMethods.compareGeometryContent((com.esri.core.geometry.MultiPoint
				)multipointShapeGeometry, multipoint);
			// Test Import Envelope from MultiPoint
			com.esri.core.geometry.Geometry envelopeShapeGeometry = importerShape.execute(0, 
				com.esri.core.geometry.Geometry.Type.Envelope, multipointShapeBuffer);
			com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)envelopeShapeGeometry;
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope();
			com.esri.core.geometry.Envelope otherenv = new com.esri.core.geometry.Envelope();
			envelope.queryEnvelope(env);
			multipoint.queryEnvelope(otherenv);
			NUnit.Framework.Assert.IsTrue(env.getXMin() == otherenv.getXMin());
			NUnit.Framework.Assert.IsTrue(env.getXMax() == otherenv.getXMax());
			NUnit.Framework.Assert.IsTrue(env.getYMin() == otherenv.getYMin());
			NUnit.Framework.Assert.IsTrue(env.getYMax() == otherenv.getYMax());
			com.esri.core.geometry.Envelope1D interval;
			com.esri.core.geometry.Envelope1D otherinterval;
			interval = envelope.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0);
			otherinterval = multipoint.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0);
			NUnit.Framework.Assert.IsTrue(interval.vmin == otherinterval.vmin);
			NUnit.Framework.Assert.IsTrue(interval.vmax == otherinterval.vmax);
			interval = envelope.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0);
			otherinterval = multipoint.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0);
			NUnit.Framework.Assert.IsTrue(interval.vmin == otherinterval.vmin);
			NUnit.Framework.Assert.IsTrue(interval.vmax == otherinterval.vmax);
		}

		[NUnit.Framework.Test]
		public static void testImportExportShapePoint()
		{
			com.esri.core.geometry.OperatorExportToESRIShape exporterShape = (com.esri.core.geometry.OperatorExportToESRIShape
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToESRIShape);
			com.esri.core.geometry.OperatorImportFromESRIShape importerShape = (com.esri.core.geometry.OperatorImportFromESRIShape
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromESRIShape);
			// Point
			com.esri.core.geometry.Point point = makePoint();
			// Test Import Point from Point
			java.nio.ByteBuffer pointShapeBuffer = exporterShape.execute(0, point);
			com.esri.core.geometry.Point pointShapeGeometry = (com.esri.core.geometry.Point)importerShape
				.execute(0, com.esri.core.geometry.Geometry.Type.Point, pointShapeBuffer);
			double x1 = point.getX();
			double x2 = pointShapeGeometry.getX();
			NUnit.Framework.Assert.IsTrue(x1 == x2);
			double y1 = point.getY();
			double y2 = pointShapeGeometry.getY();
			NUnit.Framework.Assert.IsTrue(y1 == y2);
			double z1 = point.getZ();
			double z2 = pointShapeGeometry.getZ();
			NUnit.Framework.Assert.IsTrue(z1 == z2);
			double m1 = point.getM();
			double m2 = pointShapeGeometry.getM();
			NUnit.Framework.Assert.IsTrue(m1 == m2);
			int id1 = point.getID();
			int id2 = pointShapeGeometry.getID();
			NUnit.Framework.Assert.IsTrue(id1 == id2);
			// Test Import Multipoint from Point
			com.esri.core.geometry.MultiPoint multipointShapeGeometry = (com.esri.core.geometry.MultiPoint
				)importerShape.execute(0, com.esri.core.geometry.Geometry.Type.MultiPoint, pointShapeBuffer
				);
			com.esri.core.geometry.Point point2d = multipointShapeGeometry.getPoint(0);
			NUnit.Framework.Assert.IsTrue(x1 == point2d.getX() && y1 == point2d.getY());
			int pointCount = multipointShapeGeometry.getPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 1);
			z2 = multipointShapeGeometry.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0, 0);
			NUnit.Framework.Assert.IsTrue(z1 == z2);
			m2 = multipointShapeGeometry.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0);
			NUnit.Framework.Assert.IsTrue(m1 == m2);
			id2 = multipointShapeGeometry.getAttributeAsInt(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0, 0);
			NUnit.Framework.Assert.IsTrue(id1 == id2);
			// Test Import Envelope from Point
			com.esri.core.geometry.Geometry envelopeShapeGeometry = importerShape.execute(0, 
				com.esri.core.geometry.Geometry.Type.Envelope, pointShapeBuffer);
			com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)envelopeShapeGeometry;
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope();
			com.esri.core.geometry.Envelope otherenv = new com.esri.core.geometry.Envelope();
			envelope.queryEnvelope(env);
			point.queryEnvelope(otherenv);
			NUnit.Framework.Assert.IsTrue(env.getXMin() == otherenv.getXMin());
			NUnit.Framework.Assert.IsTrue(env.getXMax() == otherenv.getXMax());
			NUnit.Framework.Assert.IsTrue(env.getYMin() == otherenv.getYMin());
			NUnit.Framework.Assert.IsTrue(env.getYMax() == otherenv.getYMax());
			com.esri.core.geometry.Envelope1D interval;
			com.esri.core.geometry.Envelope1D otherinterval;
			interval = envelope.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0);
			otherinterval = point.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0);
			NUnit.Framework.Assert.IsTrue(interval.vmin == otherinterval.vmin);
			NUnit.Framework.Assert.IsTrue(interval.vmax == otherinterval.vmax);
			interval = envelope.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0);
			otherinterval = point.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0);
			NUnit.Framework.Assert.IsTrue(interval.vmin == otherinterval.vmin);
			NUnit.Framework.Assert.IsTrue(interval.vmax == otherinterval.vmax);
		}

		[NUnit.Framework.Test]
		public static void testImportExportShapeEnvelope()
		{
			com.esri.core.geometry.OperatorExportToESRIShape exporterShape = (com.esri.core.geometry.OperatorExportToESRIShape
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToESRIShape);
			com.esri.core.geometry.OperatorImportFromESRIShape importerShape = (com.esri.core.geometry.OperatorImportFromESRIShape
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromESRIShape);
			// Test Export Envelope to Polygon
			com.esri.core.geometry.Envelope envelope = makeEnvelope();
			java.nio.ByteBuffer polygonShapeBuffer = exporterShape.execute(0, envelope);
			com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)importerShape
				.execute(0, com.esri.core.geometry.Geometry.Type.Polygon, polygonShapeBuffer);
			int pointCount = polygon.getPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount == 4);
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope();
			envelope.queryEnvelope(env);
			// interval = envelope.queryInterval(VertexDescription.Semantics.Z, 0);
			com.esri.core.geometry.Point point3d;
			point3d = polygon.getPoint(0);
			NUnit.Framework.Assert.IsTrue(point3d.getX() == env.getXMin() && point3d.getY() ==
				 env.getYMin());
			// && point3d.z ==
			// interval.vmin);
			point3d = polygon.getPoint(1);
			NUnit.Framework.Assert.IsTrue(point3d.getX() == env.getXMin() && point3d.getY() ==
				 env.getYMax());
			// && point3d.z ==
			// interval.vmax);
			point3d = polygon.getPoint(2);
			NUnit.Framework.Assert.IsTrue(point3d.getX() == env.getXMax() && point3d.getY() ==
				 env.getYMax());
			// && point3d.z ==
			// interval.vmin);
			point3d = polygon.getPoint(3);
			NUnit.Framework.Assert.IsTrue(point3d.getX() == env.getXMax() && point3d.getY() ==
				 env.getYMin());
			// && point3d.z ==
			// interval.vmax);
			com.esri.core.geometry.Envelope1D interval;
			interval = envelope.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0);
			double m = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmin);
			m = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics.
				M, 1, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmax);
			m = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics.
				M, 2, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmin);
			m = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics.
				M, 3, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmax);
			interval = envelope.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0);
			double id = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0, 0);
			NUnit.Framework.Assert.IsTrue(id == interval.vmin);
			id = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 1, 0);
			NUnit.Framework.Assert.IsTrue(id == interval.vmax);
			id = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 2, 0);
			NUnit.Framework.Assert.IsTrue(id == interval.vmin);
			id = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 3, 0);
			NUnit.Framework.Assert.IsTrue(id == interval.vmax);
		}

		[NUnit.Framework.Test]
		public static void testImportExportWkbGeometryCollection()
		{
			com.esri.core.geometry.OperatorImportFromWkb importerWKB = (com.esri.core.geometry.OperatorImportFromWkb
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromWkb);
			int offset = 0;
			java.nio.ByteBuffer wkbBuffer = java.nio.ByteBuffer.allocate(600).order(java.nio.ByteOrder
				.nativeOrder());
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			// byte order
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbGeometryCollection
				);
			offset += 4;
			// type
			wkbBuffer.putInt(offset, 3);
			// 3 geometries
			offset += 4;
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbPoint);
			offset += 4;
			wkbBuffer.putDouble(offset, 0);
			offset += 8;
			wkbBuffer.putDouble(offset, 0);
			offset += 8;
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			// byte order
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbGeometryCollection
				);
			offset += 4;
			// type
			wkbBuffer.putInt(offset, 7);
			// 7 empty geometries
			offset += 4;
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbLineString);
			offset += 4;
			wkbBuffer.putInt(offset, 0);
			// 0 points, for empty linestring
			offset += 4;
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbPolygon);
			offset += 4;
			wkbBuffer.putInt(offset, 0);
			// 0 points, for empty polygon
			offset += 4;
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbMultiPolygon);
			offset += 4;
			wkbBuffer.putInt(offset, 0);
			// 0 points, for empty multipolygon
			offset += 4;
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbMultiLineString
				);
			offset += 4;
			wkbBuffer.putInt(offset, 0);
			// 0 points, for empty multilinestring
			offset += 4;
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbGeometryCollection
				);
			offset += 4;
			wkbBuffer.putInt(offset, 0);
			// 0 geometries, for empty
			// geometrycollection
			offset += 4;
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbMultiPoint);
			offset += 4;
			wkbBuffer.putInt(offset, 0);
			// 0 points, for empty multipoint
			offset += 4;
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbPoint);
			offset += 4;
			wkbBuffer.putDouble(offset, 66);
			offset += 8;
			wkbBuffer.putDouble(offset, 88);
			offset += 8;
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbPoint);
			offset += 4;
			wkbBuffer.putDouble(offset, 13);
			offset += 8;
			wkbBuffer.putDouble(offset, 17);
			offset += 8;
			// "GeometryCollection( Point (0 0),  GeometryCollection( LineString empty, Polygon empty, MultiPolygon empty, MultiLineString empty, MultiPoint empty ), Point (13 17) )";
			com.esri.core.geometry.OGCStructure structure = importerWKB.executeOGC(0, wkbBuffer
				, null).m_structures[0];
			NUnit.Framework.Assert.IsTrue(structure.m_type == 7);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[0].m_type == 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_type == 7);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[2].m_type == 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[0].m_type ==
				 2);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[1].m_type ==
				 3);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[2].m_type ==
				 6);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[3].m_type ==
				 5);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[4].m_type ==
				 7);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[5].m_type ==
				 4);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[6].m_type ==
				 1);
			com.esri.core.geometry.Point p = (com.esri.core.geometry.Point)structure.m_structures
				[1].m_structures[6].m_geometry;
			NUnit.Framework.Assert.IsTrue(p.getX() == 66);
			NUnit.Framework.Assert.IsTrue(p.getY() == 88);
			p = (com.esri.core.geometry.Point)structure.m_structures[2].m_geometry;
			NUnit.Framework.Assert.IsTrue(p.getX() == 13);
			NUnit.Framework.Assert.IsTrue(p.getY() == 17);
		}

		[NUnit.Framework.Test]
		public static void testImportExportWKBPolygon()
		{
			com.esri.core.geometry.OperatorExportToWkb exporterWKB = (com.esri.core.geometry.OperatorExportToWkb
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToWkb);
			com.esri.core.geometry.OperatorExportToWkt exporterWKT = (com.esri.core.geometry.OperatorExportToWkt
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToWkt);
			com.esri.core.geometry.OperatorImportFromWkb importerWKB = (com.esri.core.geometry.OperatorImportFromWkb
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromWkb);
			// Test Import Polygon with bad rings
			int offset = 0;
			java.nio.ByteBuffer wkbBuffer = java.nio.ByteBuffer.allocate(500).order(java.nio.ByteOrder
				.nativeOrder());
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			// byte order
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbPolygon);
			offset += 4;
			// type
			wkbBuffer.putInt(offset, 8);
			offset += 4;
			// num rings
			wkbBuffer.putInt(offset, 4);
			offset += 4;
			// num points
			wkbBuffer.putDouble(offset, 0.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 0.0);
			offset += 8;
			// y
			wkbBuffer.putDouble(offset, 0.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 10.0);
			offset += 8;
			// y
			wkbBuffer.putDouble(offset, 10.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 10.0);
			offset += 8;
			// y
			wkbBuffer.putDouble(offset, 0.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 0.0);
			offset += 8;
			// y
			wkbBuffer.putInt(offset, 1);
			offset += 4;
			// num points
			wkbBuffer.putDouble(offset, 36.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 17.0);
			offset += 8;
			// y
			wkbBuffer.putInt(offset, 2);
			offset += 4;
			// num points
			wkbBuffer.putDouble(offset, 19.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 19.0);
			offset += 8;
			// y
			wkbBuffer.putDouble(offset, -19.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, -19.0);
			offset += 8;
			// y
			wkbBuffer.putInt(offset, 4);
			offset += 4;
			// num points
			wkbBuffer.putDouble(offset, 23.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 88);
			offset += 8;
			// y
			wkbBuffer.putDouble(offset, 13.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 43.0);
			offset += 8;
			// y
			wkbBuffer.putDouble(offset, 59.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 79.0);
			offset += 8;
			// y
			wkbBuffer.putDouble(offset, 83.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 87.0);
			offset += 8;
			// y
			wkbBuffer.putInt(offset, 3);
			offset += 4;
			// num points
			wkbBuffer.putDouble(offset, 23.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 88);
			offset += 8;
			// y
			wkbBuffer.putDouble(offset, 88);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 43.0);
			offset += 8;
			// y
			wkbBuffer.putDouble(offset, 67.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 79.0);
			offset += 8;
			// y
			wkbBuffer.putInt(offset, 0);
			offset += 4;
			// num points
			wkbBuffer.putInt(offset, 3);
			offset += 4;
			// num points
			wkbBuffer.putDouble(offset, 23.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 88);
			offset += 8;
			// y
			wkbBuffer.putDouble(offset, 88);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 43.0);
			offset += 8;
			// y
			wkbBuffer.putDouble(offset, 67.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 88);
			offset += 8;
			// y
			wkbBuffer.putInt(offset, 2);
			offset += 4;
			// num points
			wkbBuffer.putDouble(offset, 23.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 67.0);
			offset += 8;
			// y
			wkbBuffer.putDouble(offset, 43.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 67.0);
			offset += 8;
			// y
			com.esri.core.geometry.Geometry p = importerWKB.execute(0, com.esri.core.geometry.Geometry.Type
				.Polygon, wkbBuffer, null);
			int pc = ((com.esri.core.geometry.Polygon)p).getPathCount();
			string wktString = exporterWKT.execute(0, p, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOLYGON (((0 0, 10 10, 0 10, 0 0), (36 17, 36 17, 36 17), (19 19, -19 -19, 19 19), (23 88, 83 87, 59 79, 13 43, 23 88), (23 88, 67 79, 88 43, 23 88), (23 88, 67 88, 88 43, 23 88), (23 67, 43 67, 23 67)))"
				));
			wktString = exporterWKT.execute(com.esri.core.geometry.WktExportFlags.wktExportPolygon
				, p, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("POLYGON ((0 0, 10 10, 0 10, 0 0), (36 17, 36 17, 36 17), (19 19, -19 -19, 19 19), (23 88, 83 87, 59 79, 13 43, 23 88), (23 88, 67 79, 88 43, 23 88), (23 88, 67 88, 88 43, 23 88), (23 67, 43 67, 23 67))"
				));
			com.esri.core.geometry.Polygon polygon = makePolygon();
			// Test Import Polygon from Polygon8
			java.nio.ByteBuffer polygonWKBBuffer = exporterWKB.execute(0, polygon, null);
			int wkbType = polygonWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbMultiPolygonZM
				);
			com.esri.core.geometry.Geometry polygonWKBGeometry = importerWKB.execute(0, com.esri.core.geometry.Geometry.Type
				.Polygon, polygonWKBBuffer, null);
			com.esri.core.geometry.TestCommonMethods.compareGeometryContent((com.esri.core.geometry.MultiVertexGeometry
				)polygonWKBGeometry, polygon);
			// Test WKB_export_multi_polygon on nonempty single part polygon
			com.esri.core.geometry.Polygon polygon2 = makePolygon2();
			NUnit.Framework.Assert.IsTrue(polygon2.getPathCount() == 1);
			polygonWKBBuffer = exporterWKB.execute(com.esri.core.geometry.WkbExportFlags.wkbExportMultiPolygon
				, polygon2, null);
			polygonWKBGeometry = importerWKB.execute(0, com.esri.core.geometry.Geometry.Type.
				Polygon, polygonWKBBuffer, null);
			com.esri.core.geometry.TestCommonMethods.compareGeometryContent((com.esri.core.geometry.MultiVertexGeometry
				)polygonWKBGeometry, polygon2);
			wkbType = polygonWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbMultiPolygonZM
				);
			// Test WKB_export_polygon on nonempty single part polygon
			NUnit.Framework.Assert.IsTrue(polygon2.getPathCount() == 1);
			polygonWKBBuffer = exporterWKB.execute(com.esri.core.geometry.WkbExportFlags.wkbExportPolygon
				, polygon2, null);
			polygonWKBGeometry = importerWKB.execute(0, com.esri.core.geometry.Geometry.Type.
				Polygon, polygonWKBBuffer, null);
			com.esri.core.geometry.TestCommonMethods.compareGeometryContent((com.esri.core.geometry.MultiVertexGeometry
				)polygonWKBGeometry, polygon2);
			wkbType = polygonWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbPolygonZM
				);
			// Test WKB_export_polygon on empty polygon
			com.esri.core.geometry.Polygon polygon3 = new com.esri.core.geometry.Polygon();
			polygonWKBBuffer = exporterWKB.execute(com.esri.core.geometry.WkbExportFlags.wkbExportPolygon
				, polygon3, null);
			polygonWKBGeometry = importerWKB.execute(0, com.esri.core.geometry.Geometry.Type.
				Polygon, polygonWKBBuffer, null);
			NUnit.Framework.Assert.IsTrue(polygonWKBGeometry.isEmpty() == true);
			wkbType = polygonWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbPolygon
				);
			// Test WKB_export_defaults on empty polygon
			polygonWKBBuffer = exporterWKB.execute(0, polygon3, null);
			polygonWKBGeometry = importerWKB.execute(0, com.esri.core.geometry.Geometry.Type.
				Polygon, polygonWKBBuffer, null);
			NUnit.Framework.Assert.IsTrue(polygonWKBGeometry.isEmpty() == true);
			wkbType = polygonWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbMultiPolygon
				);
		}

		[NUnit.Framework.Test]
		public static void testImportExportWKBPolyline()
		{
			com.esri.core.geometry.OperatorExportToWkb exporterWKB = (com.esri.core.geometry.OperatorExportToWkb
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToWkb);
			com.esri.core.geometry.OperatorExportToWkt exporterWKT = (com.esri.core.geometry.OperatorExportToWkt
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToWkt);
			com.esri.core.geometry.OperatorImportFromWkb importerWKB = (com.esri.core.geometry.OperatorImportFromWkb
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromWkb);
			// Test Import Polyline with bad paths (i.e. paths with one point or
			// zero points)
			int offset = 0;
			java.nio.ByteBuffer wkbBuffer = java.nio.ByteBuffer.allocate(500).order(java.nio.ByteOrder
				.nativeOrder());
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			// byte order
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbMultiLineString
				);
			offset += 4;
			// type
			wkbBuffer.putInt(offset, 4);
			offset += 4;
			// num paths
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			// byte order
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbLineString);
			offset += 4;
			// type
			wkbBuffer.putInt(offset, 1);
			offset += 4;
			// num points
			wkbBuffer.putDouble(offset, 36.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 17.0);
			offset += 8;
			// y
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			// byte order
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbLineString);
			offset += 4;
			// type
			wkbBuffer.putInt(offset, 0);
			offset += 4;
			// num points
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			// byte order
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbLineString);
			offset += 4;
			// type
			wkbBuffer.putInt(offset, 1);
			offset += 4;
			// num points
			wkbBuffer.putDouble(offset, 19.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 19.0);
			offset += 8;
			// y
			wkbBuffer.put(offset, unchecked((byte)com.esri.core.geometry.WkbByteOrder.wkbNDR)
				);
			offset += 1;
			// byte order
			wkbBuffer.putInt(offset, com.esri.core.geometry.WkbGeometryType.wkbLineString);
			offset += 4;
			// type
			wkbBuffer.putInt(offset, 3);
			offset += 4;
			// num points
			wkbBuffer.putDouble(offset, 88);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 29.0);
			offset += 8;
			// y
			wkbBuffer.putDouble(offset, 13.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 43.0);
			offset += 8;
			// y
			wkbBuffer.putDouble(offset, 59.0);
			offset += 8;
			// x
			wkbBuffer.putDouble(offset, 88);
			offset += 8;
			// y
			com.esri.core.geometry.Polyline p = (com.esri.core.geometry.Polyline)(importerWKB
				.execute(0, com.esri.core.geometry.Geometry.Type.Polyline, wkbBuffer, null));
			int pc = p.getPointCount();
			int pac = p.getPathCount();
			NUnit.Framework.Assert.IsTrue(p.getPointCount() == 7);
			NUnit.Framework.Assert.IsTrue(p.getPathCount() == 3);
			string wktString = exporterWKT.execute(0, p, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTILINESTRING ((36 17, 36 17), (19 19, 19 19), (88 29, 13 43, 59 88))"
				));
			com.esri.core.geometry.Polyline polyline = makePolyline();
			polyline.dropAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID);
			// Test Import Polyline from Polyline
			java.nio.ByteBuffer polylineWKBBuffer = exporterWKB.execute(0, polyline, null);
			int wkbType = polylineWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbMultiLineStringZM
				);
			com.esri.core.geometry.Geometry polylineWKBGeometry = importerWKB.execute(0, com.esri.core.geometry.Geometry.Type
				.Polyline, polylineWKBBuffer, null);
			com.esri.core.geometry.TestCommonMethods.compareGeometryContent((com.esri.core.geometry.MultiVertexGeometry
				)polylineWKBGeometry, polyline);
			// Test wkbExportMultiPolyline on nonempty single part polyline
			com.esri.core.geometry.Polyline polyline2 = makePolyline2();
			NUnit.Framework.Assert.IsTrue(polyline2.getPathCount() == 1);
			polylineWKBBuffer = exporterWKB.execute(com.esri.core.geometry.WkbExportFlags.wkbExportMultiLineString
				, polyline2, null);
			polylineWKBGeometry = importerWKB.execute(0, com.esri.core.geometry.Geometry.Type
				.Polyline, polylineWKBBuffer, null);
			com.esri.core.geometry.TestCommonMethods.compareGeometryContent((com.esri.core.geometry.MultiVertexGeometry
				)polylineWKBGeometry, polyline2);
			wkbType = polylineWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbMultiLineStringZM
				);
			// Test wkbExportPolyline on nonempty single part polyline
			NUnit.Framework.Assert.IsTrue(polyline2.getPathCount() == 1);
			polylineWKBBuffer = exporterWKB.execute(com.esri.core.geometry.WkbExportFlags.wkbExportLineString
				, polyline2, null);
			polylineWKBGeometry = importerWKB.execute(0, com.esri.core.geometry.Geometry.Type
				.Polyline, polylineWKBBuffer, null);
			com.esri.core.geometry.TestCommonMethods.compareGeometryContent((com.esri.core.geometry.MultiVertexGeometry
				)polylineWKBGeometry, polyline2);
			wkbType = polylineWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbLineStringZM
				);
			// Test wkbExportPolyline on empty polyline
			com.esri.core.geometry.Polyline polyline3 = new com.esri.core.geometry.Polyline();
			polylineWKBBuffer = exporterWKB.execute(com.esri.core.geometry.WkbExportFlags.wkbExportLineString
				, polyline3, null);
			polylineWKBGeometry = importerWKB.execute(0, com.esri.core.geometry.Geometry.Type
				.Polyline, polylineWKBBuffer, null);
			NUnit.Framework.Assert.IsTrue(polylineWKBGeometry.isEmpty() == true);
			wkbType = polylineWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbLineString
				);
			// Test WKB_export_defaults on empty polyline
			polylineWKBBuffer = exporterWKB.execute(0, polyline3, null);
			polylineWKBGeometry = importerWKB.execute(0, com.esri.core.geometry.Geometry.Type
				.Polyline, polylineWKBBuffer, null);
			NUnit.Framework.Assert.IsTrue(polylineWKBGeometry.isEmpty() == true);
			wkbType = polylineWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbMultiLineString
				);
		}

		[NUnit.Framework.Test]
		public static void testImportExportWKBMultiPoint()
		{
			com.esri.core.geometry.OperatorExportToWkb exporterWKB = (com.esri.core.geometry.OperatorExportToWkb
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToWkb);
			com.esri.core.geometry.OperatorImportFromWkb importerWKB = (com.esri.core.geometry.OperatorImportFromWkb
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromWkb);
			com.esri.core.geometry.MultiPoint multipoint = makeMultiPoint();
			multipoint.dropAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID);
			// Test Import Multi_point from Multi_point
			java.nio.ByteBuffer multipointWKBBuffer = exporterWKB.execute(0, multipoint, null
				);
			int wkbType = multipointWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbMultiPointZ
				);
			com.esri.core.geometry.MultiPoint multipointWKBGeometry = (com.esri.core.geometry.MultiPoint
				)(importerWKB.execute(0, com.esri.core.geometry.Geometry.Type.MultiPoint, multipointWKBBuffer
				, null));
			com.esri.core.geometry.TestCommonMethods.compareGeometryContent((com.esri.core.geometry.MultiVertexGeometry
				)multipointWKBGeometry, multipoint);
			// Test WKB_export_point on nonempty single point Multi_point
			com.esri.core.geometry.MultiPoint multipoint2 = makeMultiPoint2();
			NUnit.Framework.Assert.IsTrue(multipoint2.getPointCount() == 1);
			java.nio.ByteBuffer pointWKBBuffer = exporterWKB.execute(com.esri.core.geometry.WkbExportFlags
				.wkbExportPoint, multipoint2, null);
			com.esri.core.geometry.Point pointWKBGeometry = (com.esri.core.geometry.Point)(importerWKB
				.execute(0, com.esri.core.geometry.Geometry.Type.Point, pointWKBBuffer, null));
			com.esri.core.geometry.Point3D point3d;
			com.esri.core.geometry.Point3D mpoint3d;
			point3d = pointWKBGeometry.getXYZ();
			mpoint3d = multipoint2.getXYZ(0);
			NUnit.Framework.Assert.IsTrue(point3d.x == mpoint3d.x && point3d.y == mpoint3d.y 
				&& point3d.z == mpoint3d.z);
			wkbType = pointWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbPointZ
				);
			// Test WKB_export_point on empty Multi_point
			com.esri.core.geometry.MultiPoint multipoint3 = new com.esri.core.geometry.MultiPoint
				();
			pointWKBBuffer = exporterWKB.execute(com.esri.core.geometry.WkbExportFlags.wkbExportPoint
				, multipoint3, null);
			pointWKBGeometry = (com.esri.core.geometry.Point)(importerWKB.execute(0, com.esri.core.geometry.Geometry.Type
				.Point, pointWKBBuffer, null));
			NUnit.Framework.Assert.IsTrue(pointWKBGeometry.isEmpty() == true);
			wkbType = pointWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbPoint
				);
			// Test WKB_export_defaults on empty Multi_point
			multipointWKBBuffer = exporterWKB.execute(0, multipoint3, null);
			multipointWKBGeometry = (com.esri.core.geometry.MultiPoint)(importerWKB.execute(0
				, com.esri.core.geometry.Geometry.Type.MultiPoint, multipointWKBBuffer, null));
			NUnit.Framework.Assert.IsTrue(multipointWKBGeometry.isEmpty() == true);
			wkbType = multipointWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbMultiPoint
				);
		}

		[NUnit.Framework.Test]
		public static void testImportExportWKBPoint()
		{
			com.esri.core.geometry.OperatorExportToWkb exporterWKB = (com.esri.core.geometry.OperatorExportToWkb
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToWkb);
			com.esri.core.geometry.OperatorImportFromWkb importerWKB = (com.esri.core.geometry.OperatorImportFromWkb
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromWkb);
			// Point
			com.esri.core.geometry.Point point = makePoint();
			// Test Import Point from Point
			java.nio.ByteBuffer pointWKBBuffer = exporterWKB.execute(0, point, null);
			int wkbType = pointWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbPointZM
				);
			com.esri.core.geometry.Point pointWKBGeometry = (com.esri.core.geometry.Point)(importerWKB
				.execute(0, com.esri.core.geometry.Geometry.Type.Point, pointWKBBuffer, null));
			double x_1 = point.getX();
			double x2 = pointWKBGeometry.getX();
			NUnit.Framework.Assert.IsTrue(x_1 == x2);
			double y1 = point.getY();
			double y2 = pointWKBGeometry.getY();
			NUnit.Framework.Assert.IsTrue(y1 == y2);
			double z_1 = point.getZ();
			double z_2 = pointWKBGeometry.getZ();
			NUnit.Framework.Assert.IsTrue(z_1 == z_2);
			double m1 = point.getM();
			double m2 = pointWKBGeometry.getM();
			NUnit.Framework.Assert.IsTrue(m1 == m2);
			// Test WKB_export_defaults on empty point
			com.esri.core.geometry.Point point2 = new com.esri.core.geometry.Point();
			pointWKBBuffer = exporterWKB.execute(0, point2, null);
			pointWKBGeometry = (com.esri.core.geometry.Point)(importerWKB.execute(0, com.esri.core.geometry.Geometry.Type
				.Point, pointWKBBuffer, null));
			NUnit.Framework.Assert.IsTrue(pointWKBGeometry.isEmpty() == true);
			wkbType = pointWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbPoint
				);
			// Test WKB_export_point on empty point
			pointWKBBuffer = exporterWKB.execute(com.esri.core.geometry.WkbExportFlags.wkbExportPoint
				, point2, null);
			pointWKBGeometry = (com.esri.core.geometry.Point)(importerWKB.execute(0, com.esri.core.geometry.Geometry.Type
				.Point, pointWKBBuffer, null));
			NUnit.Framework.Assert.IsTrue(pointWKBGeometry.isEmpty() == true);
			wkbType = pointWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbPoint
				);
			// Test WKB_export_multi_point on empty point
			com.esri.core.geometry.MultiPoint multipoint = new com.esri.core.geometry.MultiPoint
				();
			java.nio.ByteBuffer multipointWKBBuffer = exporterWKB.execute(com.esri.core.geometry.WkbExportFlags
				.wkbExportMultiPoint, multipoint, null);
			com.esri.core.geometry.MultiPoint multipointWKBGeometry = (com.esri.core.geometry.MultiPoint
				)(importerWKB.execute(0, com.esri.core.geometry.Geometry.Type.MultiPoint, multipointWKBBuffer
				, null));
			NUnit.Framework.Assert.IsTrue(multipointWKBGeometry.isEmpty() == true);
			wkbType = multipointWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbMultiPoint
				);
			// Test WKB_export_point on nonempty single point Multi_point
			com.esri.core.geometry.MultiPoint multipoint2 = makeMultiPoint2();
			NUnit.Framework.Assert.IsTrue(multipoint2.getPointCount() == 1);
			pointWKBBuffer = exporterWKB.execute(com.esri.core.geometry.WkbExportFlags.wkbExportPoint
				, multipoint2, null);
			pointWKBGeometry = (com.esri.core.geometry.Point)(importerWKB.execute(0, com.esri.core.geometry.Geometry.Type
				.Point, pointWKBBuffer, null));
			com.esri.core.geometry.Point3D point3d;
			com.esri.core.geometry.Point3D mpoint3d;
			point3d = pointWKBGeometry.getXYZ();
			mpoint3d = multipoint2.getXYZ(0);
			NUnit.Framework.Assert.IsTrue(point3d.x == mpoint3d.x && point3d.y == mpoint3d.y 
				&& point3d.z == mpoint3d.z);
			wkbType = pointWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbPointZ
				);
		}

		[NUnit.Framework.Test]
		public static void testImportExportWKBEnvelope()
		{
			com.esri.core.geometry.OperatorExportToWkb exporterWKB = (com.esri.core.geometry.OperatorExportToWkb
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToWkb);
			com.esri.core.geometry.OperatorImportFromWkb importerWKB = (com.esri.core.geometry.OperatorImportFromWkb
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromWkb);
			// Test Export Envelope to Polygon (WKB_export_defaults)
			com.esri.core.geometry.Envelope envelope = makeEnvelope();
			envelope.dropAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID);
			java.nio.ByteBuffer polygonWKBBuffer = exporterWKB.execute(0, envelope, null);
			int wkbType = polygonWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbPolygonZM
				);
			com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(importerWKB
				.execute(0, com.esri.core.geometry.Geometry.Type.Polygon, polygonWKBBuffer, null
				));
			int point_count = polygon.getPointCount();
			NUnit.Framework.Assert.IsTrue(point_count == 4);
			com.esri.core.geometry.Envelope2D env = new com.esri.core.geometry.Envelope2D();
			com.esri.core.geometry.Envelope1D interval;
			envelope.queryEnvelope2D(env);
			interval = envelope.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0);
			com.esri.core.geometry.Point3D point3d;
			point3d = polygon.getXYZ(0);
			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmin && point3d.y == env.ymin && point3d
				.z == interval.vmin);
			point3d = polygon.getXYZ(1);
			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmin && point3d.y == env.ymax && point3d
				.z == interval.vmax);
			point3d = polygon.getXYZ(2);
			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmax && point3d.y == env.ymax && point3d
				.z == interval.vmin);
			point3d = polygon.getXYZ(3);
			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmax && point3d.y == env.ymin && point3d
				.z == interval.vmax);
			interval = envelope.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0);
			double m = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmin);
			m = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics.
				M, 1, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmax);
			m = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics.
				M, 2, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmin);
			m = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics.
				M, 3, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmax);
			// Test WKB_export_multi_polygon on nonempty Envelope
			polygonWKBBuffer = exporterWKB.execute(com.esri.core.geometry.WkbExportFlags.wkbExportMultiPolygon
				, envelope, null);
			wkbType = polygonWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbMultiPolygonZM
				);
			polygon = (com.esri.core.geometry.Polygon)(importerWKB.execute(0, com.esri.core.geometry.Geometry.Type
				.Polygon, polygonWKBBuffer, null));
			point_count = polygon.getPointCount();
			NUnit.Framework.Assert.IsTrue(point_count == 4);
			envelope.queryEnvelope2D(env);
			interval = envelope.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0);
			point3d = polygon.getXYZ(0);
			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmin && point3d.y == env.ymin && point3d
				.z == interval.vmin);
			point3d = polygon.getXYZ(1);
			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmin && point3d.y == env.ymax && point3d
				.z == interval.vmax);
			point3d = polygon.getXYZ(2);
			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmax && point3d.y == env.ymax && point3d
				.z == interval.vmin);
			point3d = polygon.getXYZ(3);
			NUnit.Framework.Assert.IsTrue(point3d.x == env.xmax && point3d.y == env.ymin && point3d
				.z == interval.vmax);
			interval = envelope.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0);
			m = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics.
				M, 0, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmin);
			m = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics.
				M, 1, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmax);
			m = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics.
				M, 2, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmin);
			m = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics.
				M, 3, 0);
			NUnit.Framework.Assert.IsTrue(m == interval.vmax);
			// Test WKB_export_defaults on empty Envelope
			com.esri.core.geometry.Envelope envelope2 = new com.esri.core.geometry.Envelope();
			polygonWKBBuffer = exporterWKB.execute(0, envelope2, null);
			wkbType = polygonWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbPolygon
				);
			polygon = (com.esri.core.geometry.Polygon)(importerWKB.execute(0, com.esri.core.geometry.Geometry.Type
				.Polygon, polygonWKBBuffer, null));
			NUnit.Framework.Assert.IsTrue(polygon.isEmpty());
			// Test WKB_export_polygon on empty Envelope
			polygonWKBBuffer = exporterWKB.execute(com.esri.core.geometry.WkbExportFlags.wkbExportPolygon
				, envelope2, null);
			wkbType = polygonWKBBuffer.getInt(1);
			NUnit.Framework.Assert.IsTrue(wkbType == com.esri.core.geometry.WkbGeometryType.wkbPolygon
				);
			polygon = (com.esri.core.geometry.Polygon)(importerWKB.execute(0, com.esri.core.geometry.Geometry.Type
				.Polygon, polygonWKBBuffer, null));
			NUnit.Framework.Assert.IsTrue(polygon.isEmpty());
		}

		[NUnit.Framework.Test]
		public static void testImportExportWktGeometryCollection()
		{
			com.esri.core.geometry.OperatorImportFromWkt importerWKT = (com.esri.core.geometry.OperatorImportFromWkt
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromWkt);
			com.esri.core.geometry.OperatorExportToWkt exporterWKT = (com.esri.core.geometry.OperatorExportToWkt
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToWkt);
			string wktString;
			com.esri.core.geometry.Envelope2D envelope = new com.esri.core.geometry.Envelope2D
				();
			com.esri.core.geometry.WktParser wktParser = new com.esri.core.geometry.WktParser
				();
			wktString = "GeometryCollection( Point (0 0),  GeometryCollection( Point (0 0) ,  Point (1 1) , Point (2 2), LineString empty ), Point (1 1),  Point (2 2) )";
			com.esri.core.geometry.OGCStructure structure = importerWKT.executeOGC(0, wktString
				, null).m_structures[0];
			NUnit.Framework.Assert.IsTrue(structure.m_type == 7);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[0].m_type == 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[0].m_type == 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_type == 7);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[2].m_type == 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[3].m_type == 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[0].m_type ==
				 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[1].m_type ==
				 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[2].m_type ==
				 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[3].m_type ==
				 2);
		}

		[NUnit.Framework.Test]
		public static void testImportExportWktMultiPolygon()
		{
			com.esri.core.geometry.OperatorImportFromWkt importerWKT = (com.esri.core.geometry.OperatorImportFromWkt
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromWkt);
			com.esri.core.geometry.OperatorExportToWkt exporterWKT = (com.esri.core.geometry.OperatorExportToWkt
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToWkt);
			com.esri.core.geometry.Polygon polygon;
			string wktString;
			com.esri.core.geometry.Envelope2D envelope = new com.esri.core.geometry.Envelope2D
				();
			com.esri.core.geometry.WktParser wktParser = new com.esri.core.geometry.WktParser
				();
			// Test Import from MultiPolygon
			wktString = "Multipolygon M empty";
			polygon = (com.esri.core.geometry.Polygon)importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Polygon, wktString, null);
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.isEmpty());
			NUnit.Framework.Assert.IsTrue(polygon.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			polygon = (com.esri.core.geometry.Polygon)com.esri.core.geometry.GeometryEngine.geometryFromWkt
				(wktString, 0, com.esri.core.geometry.Geometry.Type.Unknown);
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.isEmpty());
			NUnit.Framework.Assert.IsTrue(polygon.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			wktString = exporterWKT.execute(0, polygon, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOLYGON M EMPTY"));
			wktString = com.esri.core.geometry.GeometryEngine.geometryToWkt(polygon, 0);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOLYGON M EMPTY"));
			wktString = "Multipolygon Z (empty, (empty, (10 10 5, 20 10 5, 20 20 5, 10 20 5, 10 10 5), (12 12 3), empty, (10 10 1, 12 12 1)), empty, ((90 90 88, 60 90 7, 60 60 7), empty, (70 70 7, 80 80 7, 70 80 7, 70 70 7)), empty)";
			polygon = (com.esri.core.geometry.Polygon)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Polygon, wktString, null));
			NUnit.Framework.Assert.IsTrue(polygon != null);
			polygon.queryEnvelope2D(envelope);
			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 90 && envelope
				.ymin == 10 && envelope.ymax == 90);
			NUnit.Framework.Assert.IsTrue(polygon.getPointCount() == 14);
			NUnit.Framework.Assert.IsTrue(polygon.getPathCount() == 5);
			// assertTrue(polygon.calculate_area_2D() > 0.0);
			NUnit.Framework.Assert.IsTrue(polygon.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			double z = polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0, 0);
			NUnit.Framework.Assert.IsTrue(z == 5);
			// Test Export to WKT MultiPolygon
			wktString = exporterWKT.execute(0, polygon, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOLYGON Z (((10 10 5, 20 10 5, 20 20 5, 10 20 5, 10 10 5), (12 12 3, 12 12 3, 12 12 3), (10 10 1, 12 12 1, 10 10 1)), ((90 90 88, 60 90 7, 60 60 7, 90 90 88), (70 70 7, 70 80 7, 80 80 7, 70 70 7)))"
				));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			// Test import Polygon
			wktString = "POLYGON z (EMPTY, EMPTY, (10 10 5, 10 20 5, 20 20 5, 20 10 5), (12 12 3), EMPTY, (10 10 1, 12 12 1), EMPTY, (60 60 7, 60 90 7, 90 90 7, 60 60 7), EMPTY, (70 70 7, 70 80 7, 80 80 7), EMPTY)";
			polygon = (com.esri.core.geometry.Polygon)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Polygon, wktString, null));
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.getPointCount() == 14);
			NUnit.Framework.Assert.IsTrue(polygon.getPathCount() == 5);
			NUnit.Framework.Assert.IsTrue(polygon.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			// Test Export to WKT Polygon
			wktString = exporterWKT.execute(com.esri.core.geometry.WktExportFlags.wktExportPolygon
				, polygon, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("POLYGON Z ((10 10 5, 20 10 5, 20 20 5, 10 20 5, 10 10 5), (12 12 3, 12 12 3, 12 12 3), (10 10 1, 12 12 1, 10 10 1), (60 60 7, 60 90 7, 90 90 7, 60 60 7), (70 70 7, 70 80 7, 80 80 7, 70 70 7))"
				));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope();
			env.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
			polygon.queryEnvelope(env);
			wktString = exporterWKT.execute(0, env, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("POLYGON Z ((10 10 1, 90 10 7, 90 90 1, 10 90 7, 10 10 1))"
				));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			wktString = exporterWKT.execute(com.esri.core.geometry.WktExportFlags.wktExportMultiPolygon
				, env, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOLYGON Z (((10 10 1, 90 10 7, 90 90 1, 10 90 7, 10 10 1)))"
				));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			env.setEmpty();
			wktString = exporterWKT.execute(0, env, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("POLYGON Z EMPTY"));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			wktString = exporterWKT.execute(com.esri.core.geometry.WktExportFlags.wktExportMultiPolygon
				, env, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOLYGON Z EMPTY"));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			wktString = "MULTIPOLYGON (((5 10, 8 10, 10 10, 10 0, 0 0, 0 10, 2 10, 5 10)))";
			// ring
			// is
			// oriented
			// clockwise
			polygon = (com.esri.core.geometry.Polygon)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Polygon, wktString, null));
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.calculateArea2D() > 0);
			wktString = "MULTIPOLYGON Z (((90 10 7, 10 10 1, 10 90 7, 90 90 1, 90 10 7)))";
			// ring
			// is
			// oriented
			// clockwise
			polygon = (com.esri.core.geometry.Polygon)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Polygon, wktString, null));
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.getPointCount() == 4);
			NUnit.Framework.Assert.IsTrue(polygon.getPathCount() == 1);
			NUnit.Framework.Assert.IsTrue(polygon.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(polygon.calculateArea2D() > 0);
			wktString = exporterWKT.execute(com.esri.core.geometry.WktExportFlags.wktExportMultiPolygon
				, polygon, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOLYGON Z (((90 10 7, 90 90 1, 10 90 7, 10 10 1, 90 10 7)))"
				));
		}

		[NUnit.Framework.Test]
		public static void testImportExportWktPolygon()
		{
			com.esri.core.geometry.OperatorImportFromWkt importerWKT = (com.esri.core.geometry.OperatorImportFromWkt
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromWkt);
			// OperatorExportToWkt exporterWKT =
			// (OperatorExportToWkt)OperatorFactoryLocal.getInstance().getOperator(Operator.Type.ExportToWkt);
			com.esri.core.geometry.Polygon polygon;
			string wktString;
			com.esri.core.geometry.Envelope2D envelope = new com.esri.core.geometry.Envelope2D
				();
			// Test Import from Polygon
			wktString = "Polygon ZM empty";
			polygon = (com.esri.core.geometry.Polygon)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.isEmpty());
			NUnit.Framework.Assert.IsTrue(polygon.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(polygon.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			wktString = "Polygon z (empty, (10 10 5, 20 10 5, 20 20 5, 10 20 5, 10 10 5), (12 12 3), empty, (10 10 1, 12 12 1))";
			polygon = (com.esri.core.geometry.Polygon)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polygon != null);
			polygon.queryEnvelope2D(envelope);
			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 20 && envelope
				.ymin == 10 && envelope.ymax == 20);
			NUnit.Framework.Assert.IsTrue(polygon.getPointCount() == 8);
			NUnit.Framework.Assert.IsTrue(polygon.getPathCount() == 3);
			NUnit.Framework.Assert.IsTrue(polygon.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			wktString = "polygon ((35 10, 10 20, 15 40, 45 45, 35 10), (20 30, 35 35, 30 20, 20 30))";
			com.esri.core.geometry.Polygon polygon2 = (com.esri.core.geometry.Polygon)(importerWKT
				.execute(0, com.esri.core.geometry.Geometry.Type.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polygon2 != null);
		}

		// wktString = exporterWKT.execute(0, *polygon2, null);
		[NUnit.Framework.Test]
		public static void testImportExportWktLineString()
		{
			com.esri.core.geometry.OperatorImportFromWkt importerWKT = (com.esri.core.geometry.OperatorImportFromWkt
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromWkt);
			// OperatorExportToWkt exporterWKT =
			// (OperatorExportToWkt)OperatorFactoryLocal.getInstance().getOperator(Operator.Type.ExportToWkt);
			com.esri.core.geometry.Polyline polyline;
			string wktString;
			com.esri.core.geometry.Envelope2D envelope = new com.esri.core.geometry.Envelope2D
				();
			// Test Import from LineString
			wktString = "LineString ZM empty";
			polyline = (com.esri.core.geometry.Polyline)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polyline != null);
			NUnit.Framework.Assert.IsTrue(polyline.isEmpty());
			NUnit.Framework.Assert.IsTrue(polyline.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(polyline.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			wktString = "LineString m (10 10 5, 10 20 5, 20 20 5, 20 10 5)";
			polyline = (com.esri.core.geometry.Polyline)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polyline != null);
			polyline.queryEnvelope2D(envelope);
			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 20 && envelope
				.ymin == 10 && envelope.ymax == 20);
			NUnit.Framework.Assert.IsTrue(polyline.getPointCount() == 4);
			NUnit.Framework.Assert.IsTrue(polyline.getPathCount() == 1);
			NUnit.Framework.Assert.IsTrue(polyline.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
		}

		[NUnit.Framework.Test]
		public static void testImportExportWktMultiLineString()
		{
			com.esri.core.geometry.OperatorImportFromWkt importerWKT = (com.esri.core.geometry.OperatorImportFromWkt
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromWkt);
			com.esri.core.geometry.OperatorExportToWkt exporterWKT = (com.esri.core.geometry.OperatorExportToWkt
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToWkt);
			com.esri.core.geometry.Polyline polyline;
			string wktString;
			com.esri.core.geometry.Envelope2D envelope = new com.esri.core.geometry.Envelope2D
				();
			com.esri.core.geometry.WktParser wktParser = new com.esri.core.geometry.WktParser
				();
			// Test Import from MultiLineString
			wktString = "MultiLineStringZMempty";
			polyline = (com.esri.core.geometry.Polyline)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polyline != null);
			NUnit.Framework.Assert.IsTrue(polyline.isEmpty());
			NUnit.Framework.Assert.IsTrue(polyline.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(polyline.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			wktString = "MultiLineStringm(empty, empty, (10 10 5, 10 20 5, 20 88 5, 20 10 5), (12 88 3), empty, (10 10 1, 12 12 1), empty, (88 60 7, 60 90 7, 90 90 7), empty, (70 70 7, 70 80 7, 80 80 7), empty)";
			polyline = (com.esri.core.geometry.Polyline)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polyline != null);
			polyline.queryEnvelope2D(envelope);
			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 90 && envelope
				.ymin == 10 && envelope.ymax == 90);
			NUnit.Framework.Assert.IsTrue(polyline.getPointCount() == 14);
			NUnit.Framework.Assert.IsTrue(polyline.getPathCount() == 5);
			NUnit.Framework.Assert.IsTrue(polyline.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			wktString = exporterWKT.execute(0, polyline, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTILINESTRING M ((10 10 5, 10 20 5, 20 88 5, 20 10 5), (12 88 3, 12 88 3), (10 10 1, 12 12 1), (88 60 7, 60 90 7, 90 90 7), (70 70 7, 70 80 7, 80 80 7))"
				));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			// Test Import LineString
			wktString = "Linestring Z(10 10 5, 10 20 5, 20 20 5, 20 10 5)";
			polyline = (com.esri.core.geometry.Polyline)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(polyline.getPointCount() == 4);
			wktString = exporterWKT.execute(com.esri.core.geometry.WktExportFlags.wktExportLineString
				, polyline, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("LINESTRING Z (10 10 5, 10 20 5, 20 20 5, 20 10 5)"
				));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			wktString = exporterWKT.execute(0, polyline, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTILINESTRING Z ((10 10 5, 10 20 5, 20 20 5, 20 10 5))"
				));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
		}

		[NUnit.Framework.Test]
		public static void testImportExportWktMultiPoint()
		{
			com.esri.core.geometry.OperatorImportFromWkt importerWKT = (com.esri.core.geometry.OperatorImportFromWkt
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromWkt);
			com.esri.core.geometry.OperatorExportToWkt exporterWKT = (com.esri.core.geometry.OperatorExportToWkt
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToWkt);
			com.esri.core.geometry.MultiPoint multipoint;
			string wktString;
			com.esri.core.geometry.Envelope2D envelope = new com.esri.core.geometry.Envelope2D
				();
			com.esri.core.geometry.WktParser wktParser = new com.esri.core.geometry.WktParser
				();
			// Test Import from Multi_point
			wktString = "  MultiPoint ZM empty";
			multipoint = (com.esri.core.geometry.MultiPoint)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(multipoint != null);
			NUnit.Framework.Assert.IsTrue(multipoint.isEmpty());
			NUnit.Framework.Assert.IsTrue(multipoint.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(multipoint.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			wktString = exporterWKT.execute(0, multipoint, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOINT ZM EMPTY"));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			wktString = exporterWKT.execute(com.esri.core.geometry.WktExportFlags.wktExportPoint
				, multipoint, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("POINT ZM EMPTY"));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			multipoint = new com.esri.core.geometry.MultiPoint();
			multipoint.add(118.15114354234563, 33.82234433423462345);
			multipoint.add(88, 88);
			wktString = exporterWKT.execute(com.esri.core.geometry.WktExportFlags.wktExportPrecision10
				, multipoint, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOINT ((118.1511435 33.82234433), (88 88))"
				));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			multipoint = new com.esri.core.geometry.MultiPoint();
			multipoint.add(88, 2);
			multipoint.add(88, 88);
			wktString = exporterWKT.execute(0, multipoint, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOINT ((88 2), (88 88))"));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			wktString = "Multipoint zm (empty, empty, (10 88 88 33), (10 20 5 33), (20 20 5 33), (20 10 5 33), (12 12 3 33), empty, (10 10 1 33), (12 12 1 33), empty, (60 60 7 33), (60 90.1 7 33), (90 90 7 33), empty, (70 70 7 33), (70 80 7 33), (80 80 7 33), empty)";
			multipoint = (com.esri.core.geometry.MultiPoint)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(multipoint != null);
			multipoint.queryEnvelope2D(envelope);
			// assertTrue(envelope.xmin == 10 && envelope.xmax == 90 &&
			// envelope.ymin == 10 && Math.abs(envelope.ymax - 90.1) <= 0.001);
			NUnit.Framework.Assert.IsTrue(multipoint.getPointCount() == 13);
			NUnit.Framework.Assert.IsTrue(multipoint.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(multipoint.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			wktString = "Multipoint zm (10 88 88 33, 10 20 5 33, 20 20 5 33, 20 10 5 33, 12 12 3 33, 10 10 1 33, 12 12 1 33, 60 60 7 33, 60 90.1 7 33, 90 90 7 33, 70 70 7 33, 70 80 7 33, 80 80 7 33)";
			multipoint = (com.esri.core.geometry.MultiPoint)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(multipoint != null);
			// assertTrue(envelope.xmin == 10 && envelope.xmax == 90 &&
			// envelope.ymin == 10 && ::fabs(envelope.ymax - 90.1) <= 0.001);
			NUnit.Framework.Assert.IsTrue(multipoint.getPointCount() == 13);
			NUnit.Framework.Assert.IsTrue(multipoint.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(multipoint.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			wktString = exporterWKT.execute(com.esri.core.geometry.WktExportFlags.wktExportPrecision15
				, multipoint, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOINT ZM ((10 88 88 33), (10 20 5 33), (20 20 5 33), (20 10 5 33), (12 12 3 33), (10 10 1 33), (12 12 1 33), (60 60 7 33), (60 90.1 7 33), (90 90 7 33), (70 70 7 33), (70 80 7 33), (80 80 7 33))"
				));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			wktString = "Multipoint zm (empty, empty, (10 10 5 33))";
			multipoint = (com.esri.core.geometry.MultiPoint)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, wktString, null));
			wktString = exporterWKT.execute(com.esri.core.geometry.WktExportFlags.wktExportPoint
				, multipoint, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("POINT ZM (10 10 5 33)"));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
		}

		[NUnit.Framework.Test]
		public static void testImportExportWktPoint()
		{
			com.esri.core.geometry.OperatorImportFromWkt importerWKT = (com.esri.core.geometry.OperatorImportFromWkt
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromWkt);
			com.esri.core.geometry.OperatorExportToWkt exporterWKT = (com.esri.core.geometry.OperatorExportToWkt
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ExportToWkt);
			com.esri.core.geometry.Point point;
			string wktString;
			com.esri.core.geometry.WktParser wktParser = new com.esri.core.geometry.WktParser
				();
			// Test Import from Point
			wktString = "Point ZM empty";
			point = (com.esri.core.geometry.Point)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(point != null);
			NUnit.Framework.Assert.IsTrue(point.isEmpty());
			NUnit.Framework.Assert.IsTrue(point.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(point.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			wktString = exporterWKT.execute(0, point, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("POINT ZM EMPTY"));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			wktString = exporterWKT.execute(com.esri.core.geometry.WktExportFlags.wktExportMultiPoint
				, point, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOINT ZM EMPTY"));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			wktString = "Point zm (30.1 10.6 5.1 33.1)";
			point = (com.esri.core.geometry.Point)(importerWKT.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, wktString, null));
			NUnit.Framework.Assert.IsTrue(point != null);
			NUnit.Framework.Assert.IsTrue(point.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(point.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			double x = point.getX();
			double y = point.getY();
			double z = point.getZ();
			double m = point.getM();
			NUnit.Framework.Assert.IsTrue(x == 30.1);
			NUnit.Framework.Assert.IsTrue(y == 10.6);
			NUnit.Framework.Assert.IsTrue(z == 5.1);
			NUnit.Framework.Assert.IsTrue(m == 33.1);
			wktString = exporterWKT.execute(com.esri.core.geometry.WktExportFlags.wktExportPrecision15
				, point, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("POINT ZM (30.1 10.6 5.1 33.1)"));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
			wktString = exporterWKT.execute(com.esri.core.geometry.WktExportFlags.wktExportMultiPoint
				 | com.esri.core.geometry.WktExportFlags.wktExportPrecision15, point, null);
			NUnit.Framework.Assert.IsTrue(wktString.Equals("MULTIPOINT ZM ((30.1 10.6 5.1 33.1))"
				));
			wktParser.resetParser(wktString);
			while (wktParser.nextToken() != com.esri.core.geometry.WktParser.WktToken.not_available
				)
			{
			}
		}

		/// <exception cref="org.json.JSONException"/>
		[NUnit.Framework.Test]
		public static void testImportGeoJsonGeometryCollection()
		{
			com.esri.core.geometry.OperatorImportFromGeoJson importer = (com.esri.core.geometry.OperatorImportFromGeoJson
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromGeoJson);
			string geoJsonString;
			com.esri.core.geometry.Envelope2D envelope = new com.esri.core.geometry.Envelope2D
				();
			com.esri.core.geometry.WktParser wktParser = new com.esri.core.geometry.WktParser
				();
			geoJsonString = "{\"type\" : \"GeometryCollection\", \"geometries\" : [{\"type\" : \"Point\", \"coordinates\": [0,0]},  {\"type\" : \"GeometryCollection\" , \"geometries\" : [ {\"type\" : \"Point\", \"coordinates\" : [0, 0]} ,  {\"type\" : \"Point\", \"coordinates\" : [1, 1]} ,{ \"type\" : \"Point\", \"coordinates\" : [2, 2]}, {\"type\" : \"LineString\", \"coordinates\" :  []}]} , {\"type\" : \"Point\", \"coordinates\" : [1, 1]},  {\"type\" : \"Point\" , \"coordinates\" : [2, 2]} ] }";
			com.esri.core.geometry.OGCStructure structure = importer.executeOGC(0, geoJsonString
				, null).m_ogcStructure.m_structures[0];
			NUnit.Framework.Assert.IsTrue(structure.m_type == 7);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[0].m_type == 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[0].m_type == 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_type == 7);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[2].m_type == 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[3].m_type == 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[0].m_type ==
				 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[1].m_type ==
				 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[2].m_type ==
				 1);
			NUnit.Framework.Assert.IsTrue(structure.m_structures[1].m_structures[3].m_type ==
				 2);
		}

		/// <exception cref="org.json.JSONException"/>
		[NUnit.Framework.Test]
		public static void testImportGeoJsonMultiPolygon()
		{
			com.esri.core.geometry.OperatorImportFromGeoJson importerGeoJson = (com.esri.core.geometry.OperatorImportFromGeoJson
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromGeoJson);
			com.esri.core.geometry.Polygon polygon;
			string geoJsonString;
			com.esri.core.geometry.Envelope2D envelope = new com.esri.core.geometry.Envelope2D
				();
			// Test Import from MultiPolygon
			geoJsonString = "{\"type\": \"Multipolygon\", \"coordinates\": []}";
			polygon = (com.esri.core.geometry.Polygon)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Polygon, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.isEmpty());
			NUnit.Framework.Assert.IsTrue(!polygon.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.GeometryEngine.
				geometryFromGeoJson(geoJsonString, 0, com.esri.core.geometry.Geometry.Type.Unknown
				).getGeometry());
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.isEmpty());
			NUnit.Framework.Assert.IsTrue(!polygon.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			geoJsonString = "{\"type\": \"Multipolygon\", \"coordinates\": [[], [[], [[10, 10, 5], [20, 10, 5], [20, 20, 5], [10, 20, 5], [10, 10, 5]], [[12, 12, 3]], [], [[10, 10, 1], [12, 12, 1]]], [], [[[90, 90, 88], [60, 90, 7], [60, 60, 7]], [], [[70, 70, 7], [80, 80, 7], [70, 80, 7], [70, 70, 7]]], []]}";
			polygon = (com.esri.core.geometry.Polygon)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Polygon, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(polygon != null);
			polygon.queryEnvelope2D(envelope);
			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 90 && envelope
				.ymin == 10 && envelope.ymax == 90);
			NUnit.Framework.Assert.IsTrue(polygon.getPointCount() == 14);
			NUnit.Framework.Assert.IsTrue(polygon.getPathCount() == 5);
			// assertTrue(polygon.calculate_area_2D() > 0.0);
			// assertTrue(polygon.hasAttribute(VertexDescription.Semantics.Z));
			// double z = polygon.getAttributeAsDbl(VertexDescription.Semantics.Z,
			// 0, 0);
			// assertTrue(z == 5);
			// Test import Polygon
			geoJsonString = "{\"type\": \"POLYGON\", \"coordinates\": [[], [], [[10, 10, 5], [10, 20, 5], [20, 20, 5], [20, 10, 5]], [[12, 12, 3]], [], [[10, 10, 1], [12, 12, 1]], [], [[60, 60, 7], [60, 90, 7], [90, 90, 7], [60, 60, 7]], [], [[70, 70, 7], [70, 80, 7], [80, 80, 7]], []] }";
			polygon = (com.esri.core.geometry.Polygon)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Polygon, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.getPointCount() == 14);
			NUnit.Framework.Assert.IsTrue(polygon.getPathCount() == 5);
			// assertTrue(polygon.hasAttribute(VertexDescription.Semantics.Z));
			geoJsonString = "{\"type\": \"MULTIPOLYGON\", \"coordinates\": [[[[90, 10, 7], [10, 10, 1], [10, 90, 7], [90, 90, 1], [90, 10, 7]]]] }";
			// ring
			// is
			// oriented
			// clockwise
			polygon = (com.esri.core.geometry.Polygon)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Polygon, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.getPointCount() == 4);
			NUnit.Framework.Assert.IsTrue(polygon.getPathCount() == 1);
			// assertTrue(polygon.hasAttribute(VertexDescription.Semantics.Z));
			NUnit.Framework.Assert.IsTrue(polygon.calculateArea2D() > 0);
		}

		/// <exception cref="org.json.JSONException"/>
		[NUnit.Framework.Test]
		public static void testImportGeoJsonMultiLineString()
		{
			com.esri.core.geometry.OperatorImportFromGeoJson importerGeoJson = (com.esri.core.geometry.OperatorImportFromGeoJson
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromGeoJson);
			com.esri.core.geometry.Polyline polyline;
			string geoJsonString;
			com.esri.core.geometry.Envelope2D envelope = new com.esri.core.geometry.Envelope2D
				();
			// Test Import from MultiLineString
			geoJsonString = "{\"type\": \"MultiLineString\", \"coordinates\": []}";
			polyline = (com.esri.core.geometry.Polyline)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(polyline != null);
			NUnit.Framework.Assert.IsTrue(polyline.isEmpty());
			NUnit.Framework.Assert.IsTrue(!polyline.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(!polyline.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			geoJsonString = "{\"type\": \"MultiLineString\", \"coordinates\": [[], [], [[10, 10, 5], [10, 20, 5], [20, 88, 5], [20, 10, 5]], [[12, 88, 3]], [], [[10, 10, 1], [12, 12, 1]], [], [[88, 60, 7], [60, 90, 7], [90, 90, 7]], [], [[70, 70, 7], [70, 80, 7], [80, 80, 7]], []]}";
			polyline = (com.esri.core.geometry.Polyline)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(polyline != null);
			polyline.queryEnvelope2D(envelope);
			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 90 && envelope
				.ymin == 10 && envelope.ymax == 90);
			NUnit.Framework.Assert.IsTrue(polyline.getPointCount() == 14);
			NUnit.Framework.Assert.IsTrue(polyline.getPathCount() == 5);
			// assertTrue(!polyline.hasAttribute(VertexDescription.Semantics.M));
			// Test Import LineString
			geoJsonString = "{\"type\": \"Linestring\", \"coordinates\": [[10, 10, 5], [10, 20, 5], [20, 20, 5], [20, 10, 5]]}";
			polyline = (com.esri.core.geometry.Polyline)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(polyline.getPointCount() == 4);
			// assertTrue(polyline.hasAttribute(VertexDescription.Semantics.Z));
			geoJsonString = "{\"type\": \"Linestring\", \"coordinates\": [[10, 10, 5], [10, 20, 5, 3], [20, 20, 5], [20, 10, 5]]}";
			polyline = (com.esri.core.geometry.Polyline)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(polyline.getPointCount() == 4);
		}

		// assertTrue(polyline.hasAttribute(VertexDescription.Semantics.Z));
		// assertTrue(polyline.hasAttribute(VertexDescription.Semantics.M));
		/// <exception cref="org.json.JSONException"/>
		[NUnit.Framework.Test]
		public static void testImportGeoJsonMultiPoint()
		{
			com.esri.core.geometry.OperatorImportFromGeoJson importerGeoJson = (com.esri.core.geometry.OperatorImportFromGeoJson
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromGeoJson);
			com.esri.core.geometry.MultiPoint multipoint;
			string geoJsonString;
			com.esri.core.geometry.Envelope2D envelope = new com.esri.core.geometry.Envelope2D
				();
			// Test Import from Multi_point
			geoJsonString = "{\"type\": \"MultiPoint\", \"coordinates\": []}";
			multipoint = (com.esri.core.geometry.MultiPoint)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(multipoint != null);
			NUnit.Framework.Assert.IsTrue(multipoint.isEmpty());
			NUnit.Framework.Assert.IsTrue(!multipoint.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(!multipoint.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			multipoint = new com.esri.core.geometry.MultiPoint();
			multipoint.add(118.15114354234563, 33.82234433423462345);
			multipoint.add(88, 88);
			multipoint = new com.esri.core.geometry.MultiPoint();
			multipoint.add(88, 2);
			multipoint.add(88, 88);
			geoJsonString = "{\"type\": \"Multipoint\", \"coordinates\": [[], [], [10, 88, 88, 33], [10, 20, 5, 33], [20, 20, 5, 33], [20, 10, 5, 33], [12, 12, 3, 33], [], [10, 10, 1, 33], [12, 12, 1, 33], [], [60, 60, 7, 33], [60, 90.1, 7, 33], [90, 90, 7, 33], [], [70, 70, 7, 33], [70, 80, 7, 33], [80, 80, 7, 33], []]}";
			multipoint = (com.esri.core.geometry.MultiPoint)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(multipoint != null);
			multipoint.queryEnvelope2D(envelope);
			// assertTrue(envelope.xmin == 10 && envelope.xmax == 90 &&
			// envelope.ymin == 10 && Math.abs(envelope.ymax - 90.1) <= 0.001);
			NUnit.Framework.Assert.IsTrue(multipoint.getPointCount() == 13);
			// assertTrue(multipoint.hasAttribute(VertexDescription.Semantics.Z));
			// assertTrue(multipoint.hasAttribute(VertexDescription.Semantics.M));
			geoJsonString = "{\"type\": \"Multipoint\", \"coordinates\": [[10, 88, 88, 33], [10, 20, 5, 33], [20, 20, 5, 33], [20, 10, 5, 33], [12, 12, 3, 33], [10, 10, 1, 33], [12, 12, 1, 33], [60, 60, 7, 33], [60, 90.1, 7, 33], [90, 90, 7, 33], [70, 70, 7, 33], [70, 80, 7, 33], [80, 80, 7, 33]]}";
			multipoint = (com.esri.core.geometry.MultiPoint)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(multipoint != null);
			// assertTrue(envelope.xmin == 10 && envelope.xmax == 90 &&
			// envelope.ymin == 10 && ::fabs(envelope.ymax - 90.1) <= 0.001);
			NUnit.Framework.Assert.IsTrue(multipoint.getPointCount() == 13);
			// assertTrue(multipoint.hasAttribute(VertexDescription.Semantics.Z));
			// assertTrue(multipoint.hasAttribute(VertexDescription.Semantics.M));
			geoJsonString = "{\"type\": \"Multipoint\", \"coordinates\": [[], [], [10, 10, 5, 33]]}";
			multipoint = (com.esri.core.geometry.MultiPoint)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString, null).getGeometry());
		}

		/// <exception cref="org.json.JSONException"/>
		[NUnit.Framework.Test]
		public static void testImportGeoJsonPolygon()
		{
			com.esri.core.geometry.OperatorImportFromGeoJson importerGeoJson = (com.esri.core.geometry.OperatorImportFromGeoJson
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromGeoJson);
			com.esri.core.geometry.Polygon polygon;
			string geoJsonString;
			com.esri.core.geometry.Envelope2D envelope = new com.esri.core.geometry.Envelope2D
				();
			// Test Import from Polygon
			geoJsonString = "{\"type\": \"Polygon\", \"coordinates\": []}";
			polygon = (com.esri.core.geometry.Polygon)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(polygon != null);
			NUnit.Framework.Assert.IsTrue(polygon.isEmpty());
			NUnit.Framework.Assert.IsTrue(!polygon.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(!polygon.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			geoJsonString = "{\"type\": \"Polygon\", \"coordinates\": [[], [[10, 10, 5], [20, 10, 5], [20, 20, 5], [10, 20, 5], [10, 10, 5]], [[12, 12, 3]], [], [[10, 10, 1], [12, 12, 1]]]}";
			polygon = (com.esri.core.geometry.Polygon)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(polygon != null);
			polygon.queryEnvelope2D(envelope);
			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 20 && envelope
				.ymin == 10 && envelope.ymax == 20);
			NUnit.Framework.Assert.IsTrue(polygon.getPointCount() == 8);
			NUnit.Framework.Assert.IsTrue(polygon.getPathCount() == 3);
			// assertTrue(polygon.hasAttribute(VertexDescription.Semantics.Z));
			geoJsonString = "{\"type\": \"polygon\", \"coordinates\": [[[35, 10], [10, 20], [15, 40], [45, 45], [35, 10]], [[20, 30], [35, 35], [30, 20], [20, 30]]]}";
			com.esri.core.geometry.Polygon polygon2 = (com.esri.core.geometry.Polygon)(importerGeoJson
				.execute(0, com.esri.core.geometry.Geometry.Type.Unknown, geoJsonString, null).getGeometry
				());
			NUnit.Framework.Assert.IsTrue(polygon2 != null);
		}

		/// <exception cref="org.json.JSONException"/>
		[NUnit.Framework.Test]
		public static void testImportGeoJsonLineString()
		{
			com.esri.core.geometry.OperatorImportFromGeoJson importerGeoJson = (com.esri.core.geometry.OperatorImportFromGeoJson
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromGeoJson);
			com.esri.core.geometry.Polyline polyline;
			string geoJsonString;
			com.esri.core.geometry.Envelope2D envelope = new com.esri.core.geometry.Envelope2D
				();
			// Test Import from LineString
			geoJsonString = "{\"type\": \"LineString\", \"coordinates\": []}";
			polyline = (com.esri.core.geometry.Polyline)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(polyline != null);
			NUnit.Framework.Assert.IsTrue(polyline.isEmpty());
			NUnit.Framework.Assert.IsTrue(!polyline.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(!polyline.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			geoJsonString = "{\"type\": \"LineString\", \"coordinates\": [[10, 10, 5], [10, 20, 5], [20, 20, 5], [20, 10, 5]]}";
			polyline = (com.esri.core.geometry.Polyline)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(polyline != null);
			polyline.queryEnvelope2D(envelope);
			NUnit.Framework.Assert.IsTrue(envelope.xmin == 10 && envelope.xmax == 20 && envelope
				.ymin == 10 && envelope.ymax == 20);
			NUnit.Framework.Assert.IsTrue(polyline.getPointCount() == 4);
			NUnit.Framework.Assert.IsTrue(polyline.getPathCount() == 1);
		}

		// assertTrue(!polyline.hasAttribute(VertexDescription.Semantics.M));
		/// <exception cref="org.json.JSONException"/>
		[NUnit.Framework.Test]
		public static void testImportGeoJsonPoint()
		{
			com.esri.core.geometry.OperatorImportFromGeoJson importerGeoJson = (com.esri.core.geometry.OperatorImportFromGeoJson
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromGeoJson);
			com.esri.core.geometry.Point point;
			string geoJsonString;
			// Test Import from Point
			geoJsonString = "{\"type\": \"Point\", \"coordinates\": []}";
			point = (com.esri.core.geometry.Point)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(point != null);
			NUnit.Framework.Assert.IsTrue(point.isEmpty());
			NUnit.Framework.Assert.IsTrue(!point.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(!point.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			geoJsonString = "{\"type\": \"Point\", \"coordinates\": [30.1, 10.6, 5.1, 33.1]}";
			point = (com.esri.core.geometry.Point)(importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString, null).getGeometry());
			NUnit.Framework.Assert.IsTrue(point != null);
			// assertTrue(point.hasAttribute(VertexDescription.Semantics.Z));
			// assertTrue(point.hasAttribute(VertexDescription.Semantics.M));
			double x = point.getX();
			double y = point.getY();
			// double z = point.getZ();
			// double m = point.getM();
			NUnit.Framework.Assert.IsTrue(x == 30.1);
			NUnit.Framework.Assert.IsTrue(y == 10.6);
		}

		// assertTrue(z == 5.1);
		// assertTrue(m == 33.1);
		/// <exception cref="org.json.JSONException"/>
		[NUnit.Framework.Test]
		public static void testImportGeoJsonSpatialReference()
		{
			com.esri.core.geometry.OperatorImportFromGeoJson importerGeoJson = (com.esri.core.geometry.OperatorImportFromGeoJson
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.ImportFromGeoJson);
			string geoJsonString4326;
			string geoJsonString3857;
			// Test Import from Point
			geoJsonString4326 = "{\"type\": \"Point\", \"coordinates\": [3.0, 5.0], \"crs\": \"EPSG:4326\"}";
			geoJsonString3857 = "{\"type\": \"Point\", \"coordinates\": [3.0, 5.0], \"crs\": \"EPSG:3857\"}";
			com.esri.core.geometry.MapGeometry mapGeometry4326 = importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString4326, null);
			com.esri.core.geometry.MapGeometry mapGeometry3857 = importerGeoJson.execute(0, com.esri.core.geometry.Geometry.Type
				.Unknown, geoJsonString3857, null);
			NUnit.Framework.Assert.IsTrue(mapGeometry4326.Equals(mapGeometry3857) == false);
			NUnit.Framework.Assert.IsTrue(mapGeometry4326.getGeometry().Equals(mapGeometry3857
				.getGeometry()));
		}

		public static com.esri.core.geometry.Polygon makePolygon()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 10);
			poly.lineTo(10, 10);
			poly.lineTo(10, 0);
			poly.startPath(3, 3);
			poly.lineTo(7, 3);
			poly.lineTo(7, 7);
			poly.lineTo(3, 7);
			poly.startPath(15, 0);
			poly.lineTo(15, 15);
			poly.lineTo(30, 15);
			poly.lineTo(30, 0);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 2);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 1, 0, 3);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 2, 0, 5);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 3, 0, 7);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 4, 0, 11);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 5, 0, 13);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 6, 0, 17);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 7, 0, 19);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 8, 0, 23);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 9, 0, 29);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 10, 0, 31
				);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 11, 0, 37
				);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 0, 2);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, 4);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 2, 0, 8);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 3, 0, 16);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 4, 0, 32);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 5, 0, 64);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 6, 0, 128
				);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 7, 0, 256
				);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 8, 0, 512
				);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 9, 0, 1024
				);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 10, 0, 2048
				);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 11, 0, 4096
				);
			return poly;
		}

		public static com.esri.core.geometry.Polygon makePolygon2()
		{
			com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
			poly.startPath(0, 0);
			poly.lineTo(0, 10);
			poly.lineTo(10, 10);
			poly.lineTo(10, 0);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 2);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 1, 0, 3);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 2, 0, 5);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 3, 0, 7);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 0, 2);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, 4);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 2, 0, 8);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 3, 0, 16);
			return poly;
		}

		public static com.esri.core.geometry.Polyline makePolyline()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(30, 14);
			poly.lineTo(60, 144);
			poly.startPath(20, 13);
			poly.lineTo(150, 120);
			poly.lineTo(300, 414);
			poly.lineTo(610, 14);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 2);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 1, 0, 3);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 2, 0, 5);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 3, 0, 7);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 4, 0, 11);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 5, 0, 13);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 6, 0, 17);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 7, 0, 19);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 0, 2);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, 4);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 2, 0, 8);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 3, 0, 16);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 4, 0, 32);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 5, 0, 64);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 6, 0, 128
				);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 7, 0, 256
				);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 0, 0, 1);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 1, 0, 2);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 2, 0, 3);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 3, 0, 5);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 4, 0, 8);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 5, 0, 13
				);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 6, 0, 21
				);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 7, 0, 34
				);
			return poly;
		}

		public static com.esri.core.geometry.Polyline makePolyline2()
		{
			com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
			poly.startPath(10, 1);
			poly.lineTo(15, 20);
			poly.lineTo(30, 14);
			poly.lineTo(60, 144);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 2);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 1, 0, 3);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 2, 0, 5);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 3, 0, 7);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 0, 2);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, 4);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 2, 0, 8);
			poly.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 3, 0, 16);
			return poly;
		}

		public static com.esri.core.geometry.Point makePoint()
		{
			com.esri.core.geometry.Point point = new com.esri.core.geometry.Point();
			point.setXY(11, 13);
			point.setZ(32);
			point.setM(243);
			point.setID(1024);
			return point;
		}

		public static com.esri.core.geometry.MultiPoint makeMultiPoint()
		{
			com.esri.core.geometry.MultiPoint mpoint = new com.esri.core.geometry.MultiPoint(
				);
			com.esri.core.geometry.Point pt1 = new com.esri.core.geometry.Point();
			pt1.setXY(0, 0);
			pt1.setZ(-1);
			com.esri.core.geometry.Point pt2 = new com.esri.core.geometry.Point();
			pt2.setXY(0, 0);
			pt2.setZ(1);
			com.esri.core.geometry.Point pt3 = new com.esri.core.geometry.Point();
			pt3.setXY(0, 1);
			pt3.setZ(1);
			mpoint.add(pt1);
			mpoint.add(pt2);
			mpoint.add(pt3);
			mpoint.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 0, 0, 
				7);
			mpoint.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 1, 0, 
				11);
			mpoint.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 2, 0, 
				13);
			return mpoint;
		}

		public static com.esri.core.geometry.MultiPoint makeMultiPoint2()
		{
			com.esri.core.geometry.MultiPoint mpoint = new com.esri.core.geometry.MultiPoint(
				);
			com.esri.core.geometry.Point pt1 = new com.esri.core.geometry.Point();
			pt1.setX(0.0);
			pt1.setY(0.0);
			pt1.setZ(-1.0);
			mpoint.add(pt1);
			return mpoint;
		}

		public static com.esri.core.geometry.Envelope makeEnvelope()
		{
			com.esri.core.geometry.Envelope envelope;
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope(0.0, 0.0
				, 5.0, 5.0);
			envelope = env;
			com.esri.core.geometry.Envelope1D interval = new com.esri.core.geometry.Envelope1D
				();
			interval.vmin = -3.0;
			interval.vmax = -7.0;
			envelope.setInterval(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, interval
				);
			interval.vmin = 16.0;
			interval.vmax = 32.0;
			envelope.setInterval(com.esri.core.geometry.VertexDescription.Semantics.M, 0, interval
				);
			interval.vmin = 5.0;
			interval.vmax = 11.0;
			envelope.setInterval(com.esri.core.geometry.VertexDescription.Semantics.ID, 0, interval
				);
			return envelope;
		}
	}
}
