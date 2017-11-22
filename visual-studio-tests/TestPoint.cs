using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestPoint : NUnit.Framework.TestFixtureAttribute
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
		public virtual void TestPt()
		{
			com.epl.geometry.Point pt = new com.epl.geometry.Point();
			NUnit.Framework.Assert.IsTrue(pt.IsEmpty());
			pt.SetXY(10, 2);
			NUnit.Framework.Assert.IsFalse(pt.IsEmpty());
			pt.ToString();
		}

//		[NUnit.Framework.Test]
//		public virtual void TestEnvelope2000()
//		{
//			com.epl.geometry.Point[] points = new com.epl.geometry.Point[2000];
//			System.Random random = new System.Random(69);
//			for (int i = 0; i < 2000; i++)
//			{
//				points[i] = new com.epl.geometry.Point();
//				points[i].SetX(random.NextDouble() * 100);
//				points[i].SetY(random.NextDouble() * 100);
//			}
//			for (int iter = 0; iter < 2; iter++)
//			{
//				long startTime = Sharpen.Runtime.NanoTime();
//				com.epl.geometry.Envelope geomExtent = new com.epl.geometry.Envelope();
//				com.epl.geometry.Envelope fullExtent = new com.epl.geometry.Envelope();
//				for (int i_1 = 0; i_1 < 2000; i_1++)
//				{
//					points[i_1].QueryEnvelope(geomExtent);
//					fullExtent.Merge(geomExtent);
//				}
//				long endTime = Sharpen.Runtime.NanoTime();
//			}
//		}

		[NUnit.Framework.Test]
		public virtual void TestBasic()
		{
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.GetDimensionFromType(com.epl.geometry.Geometry.Type.Polygon.Value()) == 2);
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.GetDimensionFromType(com.epl.geometry.Geometry.Type.Polyline.Value()) == 1);
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.GetDimensionFromType(com.epl.geometry.Geometry.Type.Envelope.Value()) == 2);
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.GetDimensionFromType(com.epl.geometry.Geometry.Type.Line.Value()) == 1);
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.GetDimensionFromType(com.epl.geometry.Geometry.Type.Point.Value()) == 0);
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.GetDimensionFromType(com.epl.geometry.Geometry.Type.MultiPoint.Value()) == 0);
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.IsLinear(com.epl.geometry.Geometry.Type.Polygon.Value()));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.IsLinear(com.epl.geometry.Geometry.Type.Polyline.Value()));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.IsLinear(com.epl.geometry.Geometry.Type.Envelope.Value()));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.IsLinear(com.epl.geometry.Geometry.Type.Line.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsLinear(com.epl.geometry.Geometry.Type.Point.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsLinear(com.epl.geometry.Geometry.Type.MultiPoint.Value()));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.IsArea(com.epl.geometry.Geometry.Type.Polygon.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsArea(com.epl.geometry.Geometry.Type.Polyline.Value()));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.IsArea(com.epl.geometry.Geometry.Type.Envelope.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsArea(com.epl.geometry.Geometry.Type.Line.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsArea(com.epl.geometry.Geometry.Type.Point.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsArea(com.epl.geometry.Geometry.Type.MultiPoint.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsPoint(com.epl.geometry.Geometry.Type.Polygon.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsPoint(com.epl.geometry.Geometry.Type.Polyline.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsPoint(com.epl.geometry.Geometry.Type.Envelope.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsPoint(com.epl.geometry.Geometry.Type.Line.Value()));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.IsPoint(com.epl.geometry.Geometry.Type.Point.Value()));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.IsPoint(com.epl.geometry.Geometry.Type.MultiPoint.Value()));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.IsMultiVertex(com.epl.geometry.Geometry.Type.Polygon.Value()));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.IsMultiVertex(com.epl.geometry.Geometry.Type.Polyline.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsMultiVertex(com.epl.geometry.Geometry.Type.Envelope.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsMultiVertex(com.epl.geometry.Geometry.Type.Line.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsMultiVertex(com.epl.geometry.Geometry.Type.Point.Value()));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.IsMultiVertex(com.epl.geometry.Geometry.Type.MultiPoint.Value()));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.IsMultiPath(com.epl.geometry.Geometry.Type.Polygon.Value()));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.IsMultiPath(com.epl.geometry.Geometry.Type.Polyline.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsMultiPath(com.epl.geometry.Geometry.Type.Envelope.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsMultiPath(com.epl.geometry.Geometry.Type.Line.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsMultiPath(com.epl.geometry.Geometry.Type.Point.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsMultiPath(com.epl.geometry.Geometry.Type.MultiPoint.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsSegment(com.epl.geometry.Geometry.Type.Polygon.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsSegment(com.epl.geometry.Geometry.Type.Polyline.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsSegment(com.epl.geometry.Geometry.Type.Envelope.Value()));
			NUnit.Framework.Assert.IsTrue(com.epl.geometry.Geometry.IsSegment(com.epl.geometry.Geometry.Type.Line.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsSegment(com.epl.geometry.Geometry.Type.Point.Value()));
			NUnit.Framework.Assert.IsTrue(!com.epl.geometry.Geometry.IsSegment(com.epl.geometry.Geometry.Type.MultiPoint.Value()));
		}

		[NUnit.Framework.Test]
		public virtual void TestCopy()
		{
			com.epl.geometry.Point pt = new com.epl.geometry.Point();
			com.epl.geometry.Point copyPt = (com.epl.geometry.Point)pt.Copy();
			NUnit.Framework.Assert.IsTrue(copyPt.Equals(pt));
			pt.SetXY(11, 13);
			copyPt = (com.epl.geometry.Point)pt.Copy();
			NUnit.Framework.Assert.IsTrue(copyPt.Equals(pt));
			NUnit.Framework.Assert.IsTrue(copyPt.GetXY().IsEqual(new com.epl.geometry.Point2D(11, 13)));
			NUnit.Framework.Assert.IsTrue(copyPt.GetXY().Equals((object)new com.epl.geometry.Point2D(11, 13)));
		}

		[NUnit.Framework.Test]
		public virtual void TestEnvelope2D_corners()
		{
			com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D(0, 1, 2, 3);
			NUnit.Framework.Assert.IsFalse(env.Equals(null));
			NUnit.Framework.Assert.IsTrue(env.Equals((object)new com.epl.geometry.Envelope2D(0, 1, 2, 3)));
			com.epl.geometry.Point2D pt2D = env.GetLowerLeft();
			NUnit.Framework.Assert.IsTrue(pt2D.Equals(com.epl.geometry.Point2D.Construct(0, 1)));
			pt2D = env.GetUpperLeft();
			NUnit.Framework.Assert.IsTrue(pt2D.Equals(com.epl.geometry.Point2D.Construct(0, 3)));
			pt2D = env.GetUpperRight();
			NUnit.Framework.Assert.IsTrue(pt2D.Equals(com.epl.geometry.Point2D.Construct(2, 3)));
			pt2D = env.GetLowerRight();
			NUnit.Framework.Assert.IsTrue(pt2D.Equals(com.epl.geometry.Point2D.Construct(2, 1)));
			{
				com.epl.geometry.Point2D[] corners = new com.epl.geometry.Point2D[4];
				env.QueryCorners(corners);
				NUnit.Framework.Assert.IsTrue(corners[0].Equals(com.epl.geometry.Point2D.Construct(0, 1)));
				NUnit.Framework.Assert.IsTrue(corners[1].Equals(com.epl.geometry.Point2D.Construct(0, 3)));
				NUnit.Framework.Assert.IsTrue(corners[2].Equals(com.epl.geometry.Point2D.Construct(2, 3)));
				NUnit.Framework.Assert.IsTrue(corners[3].Equals(com.epl.geometry.Point2D.Construct(2, 1)));
				env.QueryCorners(corners);
				NUnit.Framework.Assert.IsTrue(corners[0].Equals(env.QueryCorner(0)));
				NUnit.Framework.Assert.IsTrue(corners[1].Equals(env.QueryCorner(1)));
				NUnit.Framework.Assert.IsTrue(corners[2].Equals(env.QueryCorner(2)));
				NUnit.Framework.Assert.IsTrue(corners[3].Equals(env.QueryCorner(3)));
			}
			{
				com.epl.geometry.Point2D[] corners = new com.epl.geometry.Point2D[4];
				env.QueryCornersReversed(corners);
				NUnit.Framework.Assert.IsTrue(corners[0].Equals(com.epl.geometry.Point2D.Construct(0, 1)));
				NUnit.Framework.Assert.IsTrue(corners[1].Equals(com.epl.geometry.Point2D.Construct(2, 1)));
				NUnit.Framework.Assert.IsTrue(corners[2].Equals(com.epl.geometry.Point2D.Construct(2, 3)));
				NUnit.Framework.Assert.IsTrue(corners[3].Equals(com.epl.geometry.Point2D.Construct(0, 3)));
				env.QueryCornersReversed(corners);
				NUnit.Framework.Assert.IsTrue(corners[0].Equals(env.QueryCorner(0)));
				NUnit.Framework.Assert.IsTrue(corners[1].Equals(env.QueryCorner(3)));
				NUnit.Framework.Assert.IsTrue(corners[2].Equals(env.QueryCorner(2)));
				NUnit.Framework.Assert.IsTrue(corners[3].Equals(env.QueryCorner(1)));
			}
			NUnit.Framework.Assert.IsTrue(env.GetCenter().Equals(com.epl.geometry.Point2D.Construct(1, 2)));
			NUnit.Framework.Assert.IsFalse(env.ContainsExclusive(env.GetUpperLeft()));
			NUnit.Framework.Assert.IsTrue(env.Contains(env.GetUpperLeft()));
			NUnit.Framework.Assert.IsTrue(env.ContainsExclusive(env.GetCenter()));
		}

		[NUnit.Framework.Test]
		public virtual void TestReplaceNaNs()
		{
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope();
			com.epl.geometry.Point pt = new com.epl.geometry.Point();
			pt.SetXY(1, 2);
			pt.SetZ(double.NaN);
			pt.QueryEnvelope(env);
			pt.ReplaceNaNs(com.epl.geometry.VertexDescription.Semantics.Z, 5);
			NUnit.Framework.Assert.IsTrue(pt.Equals(new com.epl.geometry.Point(1, 2, 5)));
			NUnit.Framework.Assert.IsTrue(env.HasZ());
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0).IsEmpty());
			env.ReplaceNaNs(com.epl.geometry.VertexDescription.Semantics.Z, 5);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0).Equals(new com.epl.geometry.Envelope1D(5, 5)));
		}
	}
}
