

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestCommonMethods
	{
		public static bool compareDouble(double a, double b, double tol)
		{
			double diff = System.Math.abs(a - b);
			return diff <= tol * System.Math.abs(a) + tol;
		}

		public static com.esri.core.geometry.Point[] pointsFromMultiPath(com.esri.core.geometry.MultiPath
			 geom)
		{
			int numberOfPoints = geom.getPointCount();
			com.esri.core.geometry.Point[] points = new com.esri.core.geometry.Point[numberOfPoints
				];
			for (int i = 0; i < geom.getPointCount(); i++)
			{
				points[i] = geom.getPoint(i);
			}
			return points;
		}

		[NUnit.Framework.Test]
		public static void compareGeometryContent(com.esri.core.geometry.MultiVertexGeometry
			 geom1, com.esri.core.geometry.MultiVertexGeometry geom2)
		{
			// Geometry types
			NUnit.Framework.Assert.IsTrue(geom1.getType().value() == geom2.getType().value());
			// Envelopes
			com.esri.core.geometry.Envelope2D env1 = new com.esri.core.geometry.Envelope2D();
			geom1.queryEnvelope2D(env1);
			com.esri.core.geometry.Envelope2D env2 = new com.esri.core.geometry.Envelope2D();
			geom2.queryEnvelope2D(env2);
			NUnit.Framework.Assert.IsTrue(env1.xmin == env2.xmin && env1.xmax == env2.xmax &&
				 env1.ymin == env2.ymin && env1.ymax == env2.ymax);
			int type = geom1.getType().value();
			if (type == com.esri.core.geometry.Geometry.GeometryType.Polyline || type == com.esri.core.geometry.Geometry.GeometryType
				.Polygon)
			{
				// Part Count
				int partCount1 = ((com.esri.core.geometry.MultiPath)geom1).getPathCount();
				int partCount2 = ((com.esri.core.geometry.MultiPath)geom2).getPathCount();
				NUnit.Framework.Assert.IsTrue(partCount1 == partCount2);
				// Part indices
				for (int i = 0; i < partCount1; i++)
				{
					int start1 = ((com.esri.core.geometry.MultiPath)geom1).getPathStart(i);
					int start2 = ((com.esri.core.geometry.MultiPath)geom2).getPathStart(i);
					NUnit.Framework.Assert.IsTrue(start1 == start2);
					int end1 = ((com.esri.core.geometry.MultiPath)geom1).getPathEnd(i);
					int end2 = ((com.esri.core.geometry.MultiPath)geom2).getPathEnd(i);
					NUnit.Framework.Assert.IsTrue(end1 == end2);
				}
			}
			// Point count
			int pointCount1 = geom1.getPointCount();
			int pointCount2 = geom2.getPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount1 == pointCount2);
			if (type == com.esri.core.geometry.Geometry.GeometryType.MultiPoint || type == com.esri.core.geometry.Geometry.GeometryType
				.Polyline || type == com.esri.core.geometry.Geometry.GeometryType.Polygon)
			{
				// POSITION
				com.esri.core.geometry.AttributeStreamBase positionStream1 = ((com.esri.core.geometry.MultiVertexGeometryImpl
					)geom1._getImpl()).getAttributeStreamRef(com.esri.core.geometry.VertexDescription.Semantics
					.POSITION);
				com.esri.core.geometry.AttributeStreamOfDbl position1 = (com.esri.core.geometry.AttributeStreamOfDbl
					)(positionStream1);
				com.esri.core.geometry.AttributeStreamBase positionStream2 = ((com.esri.core.geometry.MultiVertexGeometryImpl
					)geom2._getImpl()).getAttributeStreamRef(com.esri.core.geometry.VertexDescription.Semantics
					.POSITION);
				com.esri.core.geometry.AttributeStreamOfDbl position2 = (com.esri.core.geometry.AttributeStreamOfDbl
					)(positionStream2);
				for (int i = 0; i < pointCount1; i++)
				{
					double x1 = position1.read(2 * i);
					double x2 = position2.read(2 * i);
					NUnit.Framework.Assert.IsTrue(x1 == x2);
					double y1 = position1.read(2 * i + 1);
					double y2 = position2.read(2 * i + 1);
					NUnit.Framework.Assert.IsTrue(y1 == y2);
				}
				// Zs
				bool bHasZs1 = geom1.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
					.Z);
				bool bHasZs2 = geom2.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
					.Z);
				NUnit.Framework.Assert.IsTrue(bHasZs1 == bHasZs2);
				if (bHasZs1)
				{
					com.esri.core.geometry.AttributeStreamBase zStream1 = ((com.esri.core.geometry.MultiVertexGeometryImpl
						)geom1._getImpl()).getAttributeStreamRef(com.esri.core.geometry.VertexDescription.Semantics
						.Z);
					com.esri.core.geometry.AttributeStreamOfDbl zs1 = (com.esri.core.geometry.AttributeStreamOfDbl
						)(zStream1);
					com.esri.core.geometry.AttributeStreamBase zStream2 = ((com.esri.core.geometry.MultiVertexGeometryImpl
						)geom2._getImpl()).getAttributeStreamRef(com.esri.core.geometry.VertexDescription.Semantics
						.Z);
					com.esri.core.geometry.AttributeStreamOfDbl zs2 = (com.esri.core.geometry.AttributeStreamOfDbl
						)(zStream2);
					for (int i_1 = 0; i_1 < pointCount1; i_1++)
					{
						double z1 = zs1.read(i_1);
						double z2 = zs2.read(i_1);
						NUnit.Framework.Assert.IsTrue(z1 == z2);
					}
				}
				// Ms
				bool bHasMs1 = geom1.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
					.M);
				bool bHasMs2 = geom2.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
					.M);
				NUnit.Framework.Assert.IsTrue(bHasMs1 == bHasMs2);
				if (bHasMs1)
				{
					com.esri.core.geometry.AttributeStreamBase mStream1 = ((com.esri.core.geometry.MultiVertexGeometryImpl
						)geom1._getImpl()).getAttributeStreamRef(com.esri.core.geometry.VertexDescription.Semantics
						.M);
					com.esri.core.geometry.AttributeStreamOfDbl ms1 = (com.esri.core.geometry.AttributeStreamOfDbl
						)(mStream1);
					com.esri.core.geometry.AttributeStreamBase mStream2 = ((com.esri.core.geometry.MultiVertexGeometryImpl
						)geom2._getImpl()).getAttributeStreamRef(com.esri.core.geometry.VertexDescription.Semantics
						.M);
					com.esri.core.geometry.AttributeStreamOfDbl ms2 = (com.esri.core.geometry.AttributeStreamOfDbl
						)(mStream2);
					for (int i_1 = 0; i_1 < pointCount1; i_1++)
					{
						double m1 = ms1.read(i_1);
						double m2 = ms2.read(i_1);
						NUnit.Framework.Assert.IsTrue(m1 == m2);
					}
				}
				// IDs
				bool bHasIDs1 = geom1.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
					.ID);
				bool bHasIDs2 = geom2.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
					.ID);
				NUnit.Framework.Assert.IsTrue(bHasIDs1 == bHasIDs2);
				if (bHasIDs1)
				{
					com.esri.core.geometry.AttributeStreamBase idStream1 = ((com.esri.core.geometry.MultiVertexGeometryImpl
						)geom1._getImpl()).getAttributeStreamRef(com.esri.core.geometry.VertexDescription.Semantics
						.ID);
					com.esri.core.geometry.AttributeStreamOfInt32 ids1 = (com.esri.core.geometry.AttributeStreamOfInt32
						)(idStream1);
					com.esri.core.geometry.AttributeStreamBase idStream2 = ((com.esri.core.geometry.MultiVertexGeometryImpl
						)geom2._getImpl()).getAttributeStreamRef(com.esri.core.geometry.VertexDescription.Semantics
						.ID);
					com.esri.core.geometry.AttributeStreamOfInt32 ids2 = (com.esri.core.geometry.AttributeStreamOfInt32
						)(idStream2);
					for (int i_1 = 0; i_1 < pointCount1; i_1++)
					{
						int id1 = ids1.read(i_1);
						int id2 = ids2.read(i_1);
						NUnit.Framework.Assert.IsTrue(id1 == id2);
					}
				}
			}
		}

		[NUnit.Framework.Test]
		public static void compareGeometryContent(com.esri.core.geometry.MultiPoint geom1
			, com.esri.core.geometry.MultiPoint geom2)
		{
			// Geometry types
			NUnit.Framework.Assert.IsTrue(geom1.getType().value() == geom2.getType().value());
			// Envelopes
			com.esri.core.geometry.Envelope env1 = new com.esri.core.geometry.Envelope();
			geom1.queryEnvelope(env1);
			com.esri.core.geometry.Envelope env2 = new com.esri.core.geometry.Envelope();
			geom2.queryEnvelope(env2);
			NUnit.Framework.Assert.IsTrue(env1.getXMin() == env2.getXMin() && env1.getXMax() 
				== env2.getXMax() && env1.getYMin() == env2.getYMin() && env1.getYMax() == env2.
				getYMax());
			// Point count
			int pointCount1 = geom1.getPointCount();
			int pointCount2 = geom2.getPointCount();
			NUnit.Framework.Assert.IsTrue(pointCount1 == pointCount2);
			com.esri.core.geometry.Point point1;
			com.esri.core.geometry.Point point2;
			for (int i = 0; i < pointCount1; i++)
			{
				point1 = geom1.getPoint(i);
				point2 = geom2.getPoint(i);
				double x1 = point1.getX();
				double x2 = point2.getX();
				NUnit.Framework.Assert.IsTrue(x1 == x2);
				double y1 = point1.getY();
				double y2 = point2.getY();
				NUnit.Framework.Assert.IsTrue(y1 == y2);
			}
		}

		[NUnit.Framework.Test]
		public static void testNothing()
		{
		}

		public static bool writeObjectToFile(string fileName, object obj)
		{
			try
			{
				java.io.File f = new java.io.File(fileName);
				f.setWritable(true);
				java.io.FileOutputStream fout = new java.io.FileOutputStream(f);
				java.io.ObjectOutputStream oo = new java.io.ObjectOutputStream(fout);
				oo.writeObject(obj);
				fout.close();
				return true;
			}
			catch (System.Exception)
			{
				return false;
			}
		}

		public static object readObjectFromFile(string fileName)
		{
			try
			{
				java.io.File f = new java.io.File(fileName);
				f.setReadable(true);
				java.io.FileInputStream streamIn = new java.io.FileInputStream(f);
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(streamIn);
				object obj = ii.readObject();
				streamIn.close();
				return obj;
			}
			catch (System.Exception)
			{
				return null;
			}
		}

		public static com.esri.core.geometry.MapGeometry fromJson(string jsonString)
		{
			org.codehaus.jackson.JsonFactory factory = new org.codehaus.jackson.JsonFactory();
			try
			{
				org.codehaus.jackson.JsonParser jsonParser = factory.createJsonParser(jsonString);
				jsonParser.nextToken();
				com.esri.core.geometry.OperatorImportFromJson importer = (com.esri.core.geometry.OperatorImportFromJson
					)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.ImportFromJson);
				return importer.execute(com.esri.core.geometry.Geometry.Type.Unknown, jsonParser);
			}
			catch (System.Exception)
			{
			}
			return null;
		}
	}
}
