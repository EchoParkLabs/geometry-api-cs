

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestEnvelope2DIntersector
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
		public static void testEnvelope2Dintersector()
		{
			System.Collections.Generic.List<com.esri.core.geometry.Envelope2D> envelopes = new 
				System.Collections.Generic.List<com.esri.core.geometry.Envelope2D>(0);
			com.esri.core.geometry.Envelope2D env0 = new com.esri.core.geometry.Envelope2D(2, 
				3, 4, 4);
			com.esri.core.geometry.Envelope2D env1 = new com.esri.core.geometry.Envelope2D(5, 
				13, 9, 15);
			com.esri.core.geometry.Envelope2D env2 = new com.esri.core.geometry.Envelope2D(6, 
				9, 11, 12);
			com.esri.core.geometry.Envelope2D env3 = new com.esri.core.geometry.Envelope2D(8, 
				10, 9, 17);
			com.esri.core.geometry.Envelope2D env4 = new com.esri.core.geometry.Envelope2D(11.001
				, 12, 14, 14);
			com.esri.core.geometry.Envelope2D env5 = new com.esri.core.geometry.Envelope2D(1, 
				3, 3, 4);
			com.esri.core.geometry.Envelope2D env6 = new com.esri.core.geometry.Envelope2D(0, 
				2, 5, 10);
			com.esri.core.geometry.Envelope2D env7 = new com.esri.core.geometry.Envelope2D(4, 
				7, 5, 10);
			com.esri.core.geometry.Envelope2D env8 = new com.esri.core.geometry.Envelope2D(3, 
				15, 15, 15);
			com.esri.core.geometry.Envelope2D env9 = new com.esri.core.geometry.Envelope2D(0, 
				9, 14, 9);
			com.esri.core.geometry.Envelope2D env10 = new com.esri.core.geometry.Envelope2D(0
				, 8.999, 14, 8.999);
			envelopes.add(env0);
			envelopes.add(env1);
			envelopes.add(env2);
			envelopes.add(env3);
			envelopes.add(env4);
			envelopes.add(env5);
			envelopes.add(env6);
			envelopes.add(env7);
			envelopes.add(env8);
			envelopes.add(env9);
			envelopes.add(env10);
			com.esri.core.geometry.Envelope2DIntersectorImpl intersector = new com.esri.core.geometry.Envelope2DIntersectorImpl
				();
			intersector.setTolerance(0.001);
			intersector.startConstruction();
			for (int i = 0; i < envelopes.Count; i++)
			{
				intersector.addEnvelope(i, envelopes[i]);
			}
			intersector.endConstruction();
			int count = 0;
			while (intersector.next())
			{
				int env_a = intersector.getHandleA();
				int env_b = intersector.getHandleB();
				count++;
				com.esri.core.geometry.Envelope2D env = new com.esri.core.geometry.Envelope2D();
				env.setCoords(envelopes[env_a]);
				env.inflate(0.001, 0.001);
				NUnit.Framework.Assert.IsTrue(env.isIntersecting(envelopes[env_b]));
			}
			System.Diagnostics.Debug.Assert((count == 16));
			com.esri.core.geometry.Envelope2DIntersectorImpl intersector2 = new com.esri.core.geometry.Envelope2DIntersectorImpl
				();
			intersector2.setTolerance(0.0);
			intersector2.startConstruction();
			for (int i_1 = 0; i_1 < envelopes.Count; i_1++)
			{
				intersector2.addEnvelope(i_1, envelopes[i_1]);
			}
			intersector2.endConstruction();
			count = 0;
			while (intersector2.next())
			{
				int env_a = intersector2.getHandleA();
				int env_b = intersector2.getHandleB();
				count++;
				com.esri.core.geometry.Envelope2D env = new com.esri.core.geometry.Envelope2D();
				env.setCoords(envelopes[env_a]);
				NUnit.Framework.Assert.IsTrue(env.isIntersecting(envelopes[env_b]));
			}
			System.Diagnostics.Debug.Assert((count == 13));
			env0 = new com.esri.core.geometry.Envelope2D(0, 0, 0, 10);
			env1 = new com.esri.core.geometry.Envelope2D(0, 10, 10, 10);
			env2 = new com.esri.core.geometry.Envelope2D(10, 0, 10, 10);
			env3 = new com.esri.core.geometry.Envelope2D(0, 0, 10, 0);
			envelopes.clear();
			envelopes.add(env0);
			envelopes.add(env1);
			envelopes.add(env2);
			envelopes.add(env3);
			com.esri.core.geometry.Envelope2DIntersectorImpl intersector3 = new com.esri.core.geometry.Envelope2DIntersectorImpl
				();
			intersector3.setTolerance(0.001);
			intersector3.startConstruction();
			for (int i_2 = 0; i_2 < envelopes.Count; i_2++)
			{
				intersector3.addEnvelope(i_2, envelopes[i_2]);
			}
			intersector3.endConstruction();
			count = 0;
			while (intersector3.next())
			{
				int env_a = intersector3.getHandleA();
				int env_b = intersector3.getHandleB();
				count++;
				com.esri.core.geometry.Envelope2D env = new com.esri.core.geometry.Envelope2D();
				env.setCoords(envelopes[env_a]);
				NUnit.Framework.Assert.IsTrue(env.isIntersecting(envelopes[env_b]));
			}
			NUnit.Framework.Assert.IsTrue(count == 4);
			env0 = new com.esri.core.geometry.Envelope2D(0, 0, 0, 10);
			envelopes.clear();
			envelopes.add(env0);
			envelopes.add(env0);
			envelopes.add(env0);
			envelopes.add(env0);
			com.esri.core.geometry.Envelope2DIntersectorImpl intersector4 = new com.esri.core.geometry.Envelope2DIntersectorImpl
				();
			intersector4.setTolerance(0.001);
			intersector4.startConstruction();
			for (int i_3 = 0; i_3 < envelopes.Count; i_3++)
			{
				intersector4.addEnvelope(i_3, envelopes[i_3]);
			}
			intersector4.endConstruction();
			count = 0;
			while (intersector4.next())
			{
				int env_a = intersector4.getHandleA();
				int env_b = intersector4.getHandleB();
				count++;
				com.esri.core.geometry.Envelope2D env = new com.esri.core.geometry.Envelope2D();
				env.setCoords(envelopes[env_a]);
				NUnit.Framework.Assert.IsTrue(env.isIntersecting(envelopes[env_b]));
			}
			System.Diagnostics.Debug.Assert((count == 6));
			env0 = new com.esri.core.geometry.Envelope2D(0, 10, 10, 10);
			envelopes.clear();
			envelopes.add(env0);
			envelopes.add(env0);
			envelopes.add(env0);
			envelopes.add(env0);
			com.esri.core.geometry.Envelope2DIntersectorImpl intersector5 = new com.esri.core.geometry.Envelope2DIntersectorImpl
				();
			intersector5.setTolerance(0.001);
			intersector5.startConstruction();
			for (int i_4 = 0; i_4 < envelopes.Count; i_4++)
			{
				intersector5.addEnvelope(i_4, envelopes[i_4]);
			}
			intersector5.endConstruction();
			count = 0;
			while (intersector5.next())
			{
				int env_a = intersector5.getHandleA();
				int env_b = intersector5.getHandleB();
				count++;
				com.esri.core.geometry.Envelope2D env = new com.esri.core.geometry.Envelope2D();
				env.setCoords(envelopes[env_a]);
				NUnit.Framework.Assert.IsTrue(env.isIntersecting(envelopes[env_b]));
			}
			NUnit.Framework.Assert.IsTrue(count == 6);
		}

		[NUnit.Framework.Test]
		public static void testRandom()
		{
			int passcount = 10;
			int figureSize = 100;
			int figureSize2 = 100;
			com.esri.core.geometry.Envelope extent1 = new com.esri.core.geometry.Envelope();
			com.esri.core.geometry.Envelope extent2 = new com.esri.core.geometry.Envelope();
			com.esri.core.geometry.Envelope extent3 = new com.esri.core.geometry.Envelope();
			com.esri.core.geometry.Envelope extent4 = new com.esri.core.geometry.Envelope();
			extent1.setCoords(-10000, 5000, 10000, 25000);
			// red
			extent2.setCoords(-10000, 2000, 10000, 8000);
			// blue
			extent3.setCoords(-10000, -8000, 10000, -2000);
			// blue
			extent4.setCoords(-10000, -25000, 10000, -5000);
			// red
			com.esri.core.geometry.RandomCoordinateGenerator generator1 = new com.esri.core.geometry.RandomCoordinateGenerator
				(System.Math.max(figureSize, 10000), extent1, 0.001);
			com.esri.core.geometry.RandomCoordinateGenerator generator2 = new com.esri.core.geometry.RandomCoordinateGenerator
				(System.Math.max(figureSize, 10000), extent2, 0.001);
			com.esri.core.geometry.RandomCoordinateGenerator generator3 = new com.esri.core.geometry.RandomCoordinateGenerator
				(System.Math.max(figureSize, 10000), extent3, 0.001);
			com.esri.core.geometry.RandomCoordinateGenerator generator4 = new com.esri.core.geometry.RandomCoordinateGenerator
				(System.Math.max(figureSize, 10000), extent4, 0.001);
			java.util.Random random = new java.util.Random(1982);
			int rand_max = 511;
			int qCount = 0;
			int eCount = 0;
			int bCount = 0;
			for (int c = 0; c < passcount; c++)
			{
				com.esri.core.geometry.Polygon polyRed = new com.esri.core.geometry.Polygon();
				com.esri.core.geometry.Polygon polyBlue = new com.esri.core.geometry.Polygon();
				int r = figureSize;
				if (r < 3)
				{
					continue;
				}
				com.esri.core.geometry.Point pt;
				for (int j = 0; j < r; j++)
				{
					int rand = random.nextInt(rand_max);
					bool bRandomNew = (r > 10) && ((1.0 * rand) / rand_max > 0.95);
					pt = generator1.GetRandomCoord();
					if (j == 0 || bRandomNew)
					{
						polyRed.startPath(pt);
					}
					else
					{
						polyRed.lineTo(pt);
					}
				}
				for (int j_1 = 0; j_1 < r; j_1++)
				{
					int rand = random.nextInt(rand_max);
					bool bRandomNew = (r > 10) && ((1.0 * rand) / rand_max > 0.95);
					pt = generator4.GetRandomCoord();
					if (j_1 == 0 || bRandomNew)
					{
						polyRed.startPath(pt);
					}
					else
					{
						polyRed.lineTo(pt);
					}
				}
				r = figureSize2;
				if (r < 3)
				{
					continue;
				}
				for (int j_2 = 0; j_2 < r; j_2++)
				{
					int rand = random.nextInt(rand_max);
					bool bRandomNew = (r > 10) && ((1.0 * rand) / rand_max > 0.95);
					pt = generator2.GetRandomCoord();
					if (j_2 == 0 || bRandomNew)
					{
						polyBlue.startPath(pt);
					}
					else
					{
						polyBlue.lineTo(pt);
					}
				}
				for (int j_3 = 0; j_3 < r; j_3++)
				{
					int rand = random.nextInt(rand_max);
					bool bRandomNew = (r > 10) && ((1.0 * rand) / rand_max > 0.95);
					pt = generator3.GetRandomCoord();
					if (j_3 == 0 || bRandomNew)
					{
						polyBlue.startPath(pt);
					}
					else
					{
						polyBlue.lineTo(pt);
					}
				}
				com.esri.core.geometry.Envelope2D env = new com.esri.core.geometry.Envelope2D();
				// Quad_tree
				com.esri.core.geometry.QuadTree quadTree = buildQuadTree(polyBlue);
				com.esri.core.geometry.QuadTree.QuadTreeIterator iterator = quadTree.getIterator(
					);
				com.esri.core.geometry.SegmentIteratorImpl _segIterRed = ((com.esri.core.geometry.MultiPathImpl
					)polyRed._getImpl()).querySegmentIterator();
				while (_segIterRed.nextPath())
				{
					while (_segIterRed.hasNextSegment())
					{
						com.esri.core.geometry.Segment segmentRed = _segIterRed.nextSegment();
						segmentRed.queryEnvelope2D(env);
						iterator.resetIterator(env, 0.001);
						while (iterator.next() != -1)
						{
							qCount++;
						}
					}
				}
				// Envelope_2D_intersector
				System.Collections.Generic.List<com.esri.core.geometry.Envelope2D> envelopes_red = 
					new System.Collections.Generic.List<com.esri.core.geometry.Envelope2D>();
				System.Collections.Generic.List<com.esri.core.geometry.Envelope2D> envelopes_blue
					 = new System.Collections.Generic.List<com.esri.core.geometry.Envelope2D>();
				com.esri.core.geometry.SegmentIterator segIterRed = polyRed.querySegmentIterator(
					);
				while (segIterRed.nextPath())
				{
					while (segIterRed.hasNextSegment())
					{
						com.esri.core.geometry.Segment segment = segIterRed.nextSegment();
						env = new com.esri.core.geometry.Envelope2D();
						segment.queryEnvelope2D(env);
						envelopes_red.add(env);
					}
				}
				com.esri.core.geometry.SegmentIterator segIterBlue = polyBlue.querySegmentIterator
					();
				while (segIterBlue.nextPath())
				{
					while (segIterBlue.hasNextSegment())
					{
						com.esri.core.geometry.Segment segment = segIterBlue.nextSegment();
						env = new com.esri.core.geometry.Envelope2D();
						segment.queryEnvelope2D(env);
						envelopes_blue.add(env);
					}
				}
				com.esri.core.geometry.Envelope2DIntersectorImpl intersector = new com.esri.core.geometry.Envelope2DIntersectorImpl
					();
				intersector.setTolerance(0.001);
				intersector.startRedConstruction();
				for (int i = 0; i < envelopes_red.Count; i++)
				{
					intersector.addRedEnvelope(i, envelopes_red[i]);
				}
				intersector.endRedConstruction();
				intersector.startBlueConstruction();
				for (int i_1 = 0; i_1 < envelopes_blue.Count; i_1++)
				{
					intersector.addBlueEnvelope(i_1, envelopes_blue[i_1]);
				}
				intersector.endBlueConstruction();
				while (intersector.next())
				{
					eCount++;
				}
				NUnit.Framework.Assert.IsTrue(qCount == eCount);
			}
		}

		public static com.esri.core.geometry.QuadTree buildQuadTree(com.esri.core.geometry.MultiPath
			 multipath)
		{
			com.esri.core.geometry.Envelope2D extent = new com.esri.core.geometry.Envelope2D(
				);
			multipath.queryEnvelope2D(extent);
			com.esri.core.geometry.QuadTree quadTree = new com.esri.core.geometry.QuadTree(extent
				, 8);
			int hint_index = -1;
			com.esri.core.geometry.SegmentIterator seg_iter = multipath.querySegmentIterator(
				);
			while (seg_iter.nextPath())
			{
				while (seg_iter.hasNextSegment())
				{
					com.esri.core.geometry.Segment segment = seg_iter.nextSegment();
					int index = seg_iter.getStartPointIndex();
					com.esri.core.geometry.Envelope2D boundingbox = new com.esri.core.geometry.Envelope2D
						();
					segment.queryEnvelope2D(boundingbox);
					hint_index = quadTree.insert(index, boundingbox, hint_index);
				}
			}
			return quadTree;
		}
	}
}
