

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestPoint
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
		public virtual void testPt()
		{
			com.esri.core.geometry.Point pt = new com.esri.core.geometry.Point();
			NUnit.Framework.Assert.IsTrue(pt.isEmpty());
			pt.setXY(10, 2);
			NUnit.Framework.Assert.IsFalse(pt.isEmpty());
			pt.ToString();
		}

		[NUnit.Framework.Test]
		public virtual void testEnvelope2000()
		{
			com.esri.core.geometry.Point[] points = new com.esri.core.geometry.Point[2000];
			java.util.Random random = new java.util.Random(69);
			for (int i = 0; i < 2000; i++)
			{
				points[i] = new com.esri.core.geometry.Point();
				points[i].setX(random.nextDouble() * 100);
				points[i].setY(random.nextDouble() * 100);
			}
			for (int iter = 0; iter < 2; iter++)
			{
				long startTime = Sharpen.Runtime.nanoTime();
				com.esri.core.geometry.Envelope geomExtent = new com.esri.core.geometry.Envelope(
					);
				com.esri.core.geometry.Envelope fullExtent = new com.esri.core.geometry.Envelope(
					);
				for (int i_1 = 0; i_1 < 2000; i_1++)
				{
					points[i_1].queryEnvelope(geomExtent);
					fullExtent.merge(geomExtent);
				}
				long endTime = Sharpen.Runtime.nanoTime();
			}
		}

		[NUnit.Framework.Test]
		public virtual void testBasic()
		{
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.getDimensionFromType
				(com.esri.core.geometry.Geometry.Type.Polygon.value()) == 2);
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.getDimensionFromType
				(com.esri.core.geometry.Geometry.Type.Polyline.value()) == 1);
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.getDimensionFromType
				(com.esri.core.geometry.Geometry.Type.Envelope.value()) == 2);
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.getDimensionFromType
				(com.esri.core.geometry.Geometry.Type.Line.value()) == 1);
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.getDimensionFromType
				(com.esri.core.geometry.Geometry.Type.Point.value()) == 0);
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.getDimensionFromType
				(com.esri.core.geometry.Geometry.Type.MultiPoint.value()) == 0);
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.isLinear(com.esri.core.geometry.Geometry.Type
				.Polygon.value()));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.isLinear(com.esri.core.geometry.Geometry.Type
				.Polyline.value()));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.isLinear(com.esri.core.geometry.Geometry.Type
				.Envelope.value()));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.isLinear(com.esri.core.geometry.Geometry.Type
				.Line.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isLinear(com.esri.core.geometry.Geometry.Type
				.Point.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isLinear(com.esri.core.geometry.Geometry.Type
				.MultiPoint.value()));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.isArea(com.esri.core.geometry.Geometry.Type
				.Polygon.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isArea(com.esri.core.geometry.Geometry.Type
				.Polyline.value()));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.isArea(com.esri.core.geometry.Geometry.Type
				.Envelope.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isArea(com.esri.core.geometry.Geometry.Type
				.Line.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isArea(com.esri.core.geometry.Geometry.Type
				.Point.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isArea(com.esri.core.geometry.Geometry.Type
				.MultiPoint.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isPoint(com.esri.core.geometry.Geometry.Type
				.Polygon.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isPoint(com.esri.core.geometry.Geometry.Type
				.Polyline.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isPoint(com.esri.core.geometry.Geometry.Type
				.Envelope.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isPoint(com.esri.core.geometry.Geometry.Type
				.Line.value()));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.isPoint(com.esri.core.geometry.Geometry.Type
				.Point.value()));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.isPoint(com.esri.core.geometry.Geometry.Type
				.MultiPoint.value()));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.isMultiVertex(com.esri.core.geometry.Geometry.Type
				.Polygon.value()));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.isMultiVertex(com.esri.core.geometry.Geometry.Type
				.Polyline.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isMultiVertex(com.esri.core.geometry.Geometry.Type
				.Envelope.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isMultiVertex(com.esri.core.geometry.Geometry.Type
				.Line.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isMultiVertex(com.esri.core.geometry.Geometry.Type
				.Point.value()));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.isMultiVertex(com.esri.core.geometry.Geometry.Type
				.MultiPoint.value()));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.isMultiPath(com.esri.core.geometry.Geometry.Type
				.Polygon.value()));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.isMultiPath(com.esri.core.geometry.Geometry.Type
				.Polyline.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isMultiPath(com.esri.core.geometry.Geometry.Type
				.Envelope.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isMultiPath(com.esri.core.geometry.Geometry.Type
				.Line.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isMultiPath(com.esri.core.geometry.Geometry.Type
				.Point.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isMultiPath(com.esri.core.geometry.Geometry.Type
				.MultiPoint.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isSegment(com.esri.core.geometry.Geometry.Type
				.Polygon.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isSegment(com.esri.core.geometry.Geometry.Type
				.Polyline.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isSegment(com.esri.core.geometry.Geometry.Type
				.Envelope.value()));
			NUnit.Framework.Assert.IsTrue(com.esri.core.geometry.Geometry.isSegment(com.esri.core.geometry.Geometry.Type
				.Line.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isSegment(com.esri.core.geometry.Geometry.Type
				.Point.value()));
			NUnit.Framework.Assert.IsTrue(!com.esri.core.geometry.Geometry.isSegment(com.esri.core.geometry.Geometry.Type
				.MultiPoint.value()));
		}

		[NUnit.Framework.Test]
		public virtual void testCopy()
		{
			com.esri.core.geometry.Point pt = new com.esri.core.geometry.Point();
			com.esri.core.geometry.Point copyPt = (com.esri.core.geometry.Point)pt.copy();
			NUnit.Framework.Assert.IsTrue(copyPt.Equals(pt));
			pt.setXY(11, 13);
			copyPt = (com.esri.core.geometry.Point)pt.copy();
			NUnit.Framework.Assert.IsTrue(copyPt.Equals(pt));
			NUnit.Framework.Assert.IsTrue(copyPt.getXY().isEqual(new com.esri.core.geometry.Point2D
				(11, 13)));
			NUnit.Framework.Assert.IsTrue(copyPt.getXY().Equals((object)new com.esri.core.geometry.Point2D
				(11, 13)));
		}

		[NUnit.Framework.Test]
		public virtual void testEnvelope2D_corners()
		{
			com.esri.core.geometry.Envelope2D env = new com.esri.core.geometry.Envelope2D(0, 
				1, 2, 3);
			NUnit.Framework.Assert.IsFalse(env.Equals(null));
			NUnit.Framework.Assert.IsTrue(env.Equals((object)new com.esri.core.geometry.Envelope2D
				(0, 1, 2, 3)));
			com.esri.core.geometry.Point2D pt2D = env.getLowerLeft();
			NUnit.Framework.Assert.IsTrue(pt2D.equals(com.esri.core.geometry.Point2D.construct
				(0, 1)));
			pt2D = env.getUpperLeft();
			NUnit.Framework.Assert.IsTrue(pt2D.equals(com.esri.core.geometry.Point2D.construct
				(0, 3)));
			pt2D = env.getUpperRight();
			NUnit.Framework.Assert.IsTrue(pt2D.equals(com.esri.core.geometry.Point2D.construct
				(2, 3)));
			pt2D = env.getLowerRight();
			NUnit.Framework.Assert.IsTrue(pt2D.equals(com.esri.core.geometry.Point2D.construct
				(2, 1)));
			{
				com.esri.core.geometry.Point2D[] corners = new com.esri.core.geometry.Point2D[4];
				env.queryCorners(corners);
				NUnit.Framework.Assert.IsTrue(corners[0].equals(com.esri.core.geometry.Point2D.construct
					(0, 1)));
				NUnit.Framework.Assert.IsTrue(corners[1].equals(com.esri.core.geometry.Point2D.construct
					(0, 3)));
				NUnit.Framework.Assert.IsTrue(corners[2].equals(com.esri.core.geometry.Point2D.construct
					(2, 3)));
				NUnit.Framework.Assert.IsTrue(corners[3].equals(com.esri.core.geometry.Point2D.construct
					(2, 1)));
				env.queryCorners(corners);
				NUnit.Framework.Assert.IsTrue(corners[0].equals(env.queryCorner(0)));
				NUnit.Framework.Assert.IsTrue(corners[1].equals(env.queryCorner(1)));
				NUnit.Framework.Assert.IsTrue(corners[2].equals(env.queryCorner(2)));
				NUnit.Framework.Assert.IsTrue(corners[3].equals(env.queryCorner(3)));
			}
			{
				com.esri.core.geometry.Point2D[] corners = new com.esri.core.geometry.Point2D[4];
				env.queryCornersReversed(corners);
				NUnit.Framework.Assert.IsTrue(corners[0].equals(com.esri.core.geometry.Point2D.construct
					(0, 1)));
				NUnit.Framework.Assert.IsTrue(corners[1].equals(com.esri.core.geometry.Point2D.construct
					(2, 1)));
				NUnit.Framework.Assert.IsTrue(corners[2].equals(com.esri.core.geometry.Point2D.construct
					(2, 3)));
				NUnit.Framework.Assert.IsTrue(corners[3].equals(com.esri.core.geometry.Point2D.construct
					(0, 3)));
				env.queryCornersReversed(corners);
				NUnit.Framework.Assert.IsTrue(corners[0].equals(env.queryCorner(0)));
				NUnit.Framework.Assert.IsTrue(corners[1].equals(env.queryCorner(3)));
				NUnit.Framework.Assert.IsTrue(corners[2].equals(env.queryCorner(2)));
				NUnit.Framework.Assert.IsTrue(corners[3].equals(env.queryCorner(1)));
			}
			NUnit.Framework.Assert.IsTrue(env.getCenter().equals(com.esri.core.geometry.Point2D
				.construct(1, 2)));
			NUnit.Framework.Assert.IsFalse(env.containsExclusive(env.getUpperLeft()));
			NUnit.Framework.Assert.IsTrue(env.contains(env.getUpperLeft()));
			NUnit.Framework.Assert.IsTrue(env.containsExclusive(env.getCenter()));
		}

		[NUnit.Framework.Test]
		public virtual void testReplaceNaNs()
		{
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope();
			com.esri.core.geometry.Point pt = new com.esri.core.geometry.Point();
			pt.setXY(1, 2);
			pt.setZ(double.NaN);
			pt.queryEnvelope(env);
			pt.replaceNaNs(com.esri.core.geometry.VertexDescription.Semantics.Z, 5);
			NUnit.Framework.Assert.IsTrue(pt.Equals(new com.esri.core.geometry.Point(1, 2, 5)
				));
			NUnit.Framework.Assert.IsTrue(env.hasZ());
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0).isEmpty());
			env.replaceNaNs(com.esri.core.geometry.VertexDescription.Semantics.Z, 5);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0).Equals(new com.esri.core.geometry.Envelope1D(5, 5)));
		}
	}
}
