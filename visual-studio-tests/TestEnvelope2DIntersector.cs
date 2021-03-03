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
	public class TestEnvelope2DIntersector : NUnit.Framework.TestFixtureAttribute
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
		public static void TestEnvelope2Dintersector()
		{
			System.Collections.Generic.List<com.epl.geometry.Envelope2D> envelopes = new System.Collections.Generic.List<com.epl.geometry.Envelope2D>(0);
			com.epl.geometry.Envelope2D env0 = new com.epl.geometry.Envelope2D(2, 3, 4, 4);
			com.epl.geometry.Envelope2D env1 = new com.epl.geometry.Envelope2D(5, 13, 9, 15);
			com.epl.geometry.Envelope2D env2 = new com.epl.geometry.Envelope2D(6, 9, 11, 12);
			com.epl.geometry.Envelope2D env3 = new com.epl.geometry.Envelope2D(8, 10, 9, 17);
			com.epl.geometry.Envelope2D env4 = new com.epl.geometry.Envelope2D(11.001, 12, 14, 14);
			com.epl.geometry.Envelope2D env5 = new com.epl.geometry.Envelope2D(1, 3, 3, 4);
			com.epl.geometry.Envelope2D env6 = new com.epl.geometry.Envelope2D(0, 2, 5, 10);
			com.epl.geometry.Envelope2D env7 = new com.epl.geometry.Envelope2D(4, 7, 5, 10);
			com.epl.geometry.Envelope2D env8 = new com.epl.geometry.Envelope2D(3, 15, 15, 15);
			com.epl.geometry.Envelope2D env9 = new com.epl.geometry.Envelope2D(0, 9, 14, 9);
			com.epl.geometry.Envelope2D env10 = new com.epl.geometry.Envelope2D(0, 8.999, 14, 8.999);
			envelopes.Add(env0);
			envelopes.Add(env1);
			envelopes.Add(env2);
			envelopes.Add(env3);
			envelopes.Add(env4);
			envelopes.Add(env5);
			envelopes.Add(env6);
			envelopes.Add(env7);
			envelopes.Add(env8);
			envelopes.Add(env9);
			envelopes.Add(env10);
			com.epl.geometry.Envelope2DIntersectorImpl intersector = new com.epl.geometry.Envelope2DIntersectorImpl();
			intersector.SetTolerance(0.001);
			intersector.StartConstruction();
			for (int i = 0; i < envelopes.Count; i++)
			{
				intersector.AddEnvelope(i, envelopes[i]);
			}
			intersector.EndConstruction();
			int count = 0;
			while (intersector.Next())
			{
				int env_a = intersector.GetHandleA();
				int env_b = intersector.GetHandleB();
				count++;
				com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
				env.SetCoords(envelopes[env_a]);
				env.Inflate(0.001, 0.001);
				NUnit.Framework.Assert.IsTrue(env.IsIntersecting(envelopes[env_b]));
			}
			System.Diagnostics.Debug.Assert((count == 16));
			com.epl.geometry.Envelope2DIntersectorImpl intersector2 = new com.epl.geometry.Envelope2DIntersectorImpl();
			intersector2.SetTolerance(0.0);
			intersector2.StartConstruction();
			for (int i_1 = 0; i_1 < envelopes.Count; i_1++)
			{
				intersector2.AddEnvelope(i_1, envelopes[i_1]);
			}
			intersector2.EndConstruction();
			count = 0;
			while (intersector2.Next())
			{
				int env_a = intersector2.GetHandleA();
				int env_b = intersector2.GetHandleB();
				count++;
				com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
				env.SetCoords(envelopes[env_a]);
				NUnit.Framework.Assert.IsTrue(env.IsIntersecting(envelopes[env_b]));
			}
			System.Diagnostics.Debug.Assert((count == 13));
			env0 = new com.epl.geometry.Envelope2D(0, 0, 0, 10);
			env1 = new com.epl.geometry.Envelope2D(0, 10, 10, 10);
			env2 = new com.epl.geometry.Envelope2D(10, 0, 10, 10);
			env3 = new com.epl.geometry.Envelope2D(0, 0, 10, 0);
			envelopes.Clear();
			envelopes.Add(env0);
			envelopes.Add(env1);
			envelopes.Add(env2);
			envelopes.Add(env3);
			com.epl.geometry.Envelope2DIntersectorImpl intersector3 = new com.epl.geometry.Envelope2DIntersectorImpl();
			intersector3.SetTolerance(0.001);
			intersector3.StartConstruction();
			for (int i_2 = 0; i_2 < envelopes.Count; i_2++)
			{
				intersector3.AddEnvelope(i_2, envelopes[i_2]);
			}
			intersector3.EndConstruction();
			count = 0;
			while (intersector3.Next())
			{
				int env_a = intersector3.GetHandleA();
				int env_b = intersector3.GetHandleB();
				count++;
				com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
				env.SetCoords(envelopes[env_a]);
				NUnit.Framework.Assert.IsTrue(env.IsIntersecting(envelopes[env_b]));
			}
			NUnit.Framework.Assert.IsTrue(count == 4);
			env0 = new com.epl.geometry.Envelope2D(0, 0, 0, 10);
			envelopes.Clear();
			envelopes.Add(env0);
			envelopes.Add(env0);
			envelopes.Add(env0);
			envelopes.Add(env0);
			com.epl.geometry.Envelope2DIntersectorImpl intersector4 = new com.epl.geometry.Envelope2DIntersectorImpl();
			intersector4.SetTolerance(0.001);
			intersector4.StartConstruction();
			for (int i_3 = 0; i_3 < envelopes.Count; i_3++)
			{
				intersector4.AddEnvelope(i_3, envelopes[i_3]);
			}
			intersector4.EndConstruction();
			count = 0;
			while (intersector4.Next())
			{
				int env_a = intersector4.GetHandleA();
				int env_b = intersector4.GetHandleB();
				count++;
				com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
				env.SetCoords(envelopes[env_a]);
				NUnit.Framework.Assert.IsTrue(env.IsIntersecting(envelopes[env_b]));
			}
			System.Diagnostics.Debug.Assert((count == 6));
			env0 = new com.epl.geometry.Envelope2D(0, 10, 10, 10);
			envelopes.Clear();
			envelopes.Add(env0);
			envelopes.Add(env0);
			envelopes.Add(env0);
			envelopes.Add(env0);
			com.epl.geometry.Envelope2DIntersectorImpl intersector5 = new com.epl.geometry.Envelope2DIntersectorImpl();
			intersector5.SetTolerance(0.001);
			intersector5.StartConstruction();
			for (int i_4 = 0; i_4 < envelopes.Count; i_4++)
			{
				intersector5.AddEnvelope(i_4, envelopes[i_4]);
			}
			intersector5.EndConstruction();
			count = 0;
			while (intersector5.Next())
			{
				int env_a = intersector5.GetHandleA();
				int env_b = intersector5.GetHandleB();
				count++;
				com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
				env.SetCoords(envelopes[env_a]);
				NUnit.Framework.Assert.IsTrue(env.IsIntersecting(envelopes[env_b]));
			}
			NUnit.Framework.Assert.IsTrue(count == 6);
		}

		[NUnit.Framework.Test]
		public static void TestRandom()
		{
			int passcount = 10;
			int figureSize = 100;
			int figureSize2 = 100;
			com.epl.geometry.Envelope extent1 = new com.epl.geometry.Envelope();
			com.epl.geometry.Envelope extent2 = new com.epl.geometry.Envelope();
			com.epl.geometry.Envelope extent3 = new com.epl.geometry.Envelope();
			com.epl.geometry.Envelope extent4 = new com.epl.geometry.Envelope();
			extent1.SetCoords(-10000, 5000, 10000, 25000);
			// red
			extent2.SetCoords(-10000, 2000, 10000, 8000);
			// blue
			extent3.SetCoords(-10000, -8000, 10000, -2000);
			// blue
			extent4.SetCoords(-10000, -25000, 10000, -5000);
			// red
			com.epl.geometry.RandomCoordinateGenerator generator1 = new com.epl.geometry.RandomCoordinateGenerator(System.Math.Max(figureSize, 10000), extent1, 0.001);
			com.epl.geometry.RandomCoordinateGenerator generator2 = new com.epl.geometry.RandomCoordinateGenerator(System.Math.Max(figureSize, 10000), extent2, 0.001);
			com.epl.geometry.RandomCoordinateGenerator generator3 = new com.epl.geometry.RandomCoordinateGenerator(System.Math.Max(figureSize, 10000), extent3, 0.001);
			com.epl.geometry.RandomCoordinateGenerator generator4 = new com.epl.geometry.RandomCoordinateGenerator(System.Math.Max(figureSize, 10000), extent4, 0.001);
			System.Random random = new System.Random(1982);
			int rand_max = 511;
			int qCount = 0;
			int eCount = 0;
			int bCount = 0;
			for (int c = 0; c < passcount; c++)
			{
				com.epl.geometry.Polygon polyRed = new com.epl.geometry.Polygon();
				com.epl.geometry.Polygon polyBlue = new com.epl.geometry.Polygon();
				int r = figureSize;
				if (r < 3)
				{
					continue;
				}
				com.epl.geometry.Point pt;
				for (int j = 0; j < r; j++)
				{
					int rand = random.Next(rand_max);
					bool bRandomNew = (r > 10) && ((1.0 * rand) / rand_max > 0.95);
					pt = generator1.GetRandomCoord();
					if (j == 0 || bRandomNew)
					{
						polyRed.StartPath(pt);
					}
					else
					{
						polyRed.LineTo(pt);
					}
				}
				for (int j_1 = 0; j_1 < r; j_1++)
				{
					int rand = random.Next(rand_max);
					bool bRandomNew = (r > 10) && ((1.0 * rand) / rand_max > 0.95);
					pt = generator4.GetRandomCoord();
					if (j_1 == 0 || bRandomNew)
					{
						polyRed.StartPath(pt);
					}
					else
					{
						polyRed.LineTo(pt);
					}
				}
				r = figureSize2;
				if (r < 3)
				{
					continue;
				}
				for (int j_2 = 0; j_2 < r; j_2++)
				{
					int rand = random.Next(rand_max);
					bool bRandomNew = (r > 10) && ((1.0 * rand) / rand_max > 0.95);
					pt = generator2.GetRandomCoord();
					if (j_2 == 0 || bRandomNew)
					{
						polyBlue.StartPath(pt);
					}
					else
					{
						polyBlue.LineTo(pt);
					}
				}
				for (int j_3 = 0; j_3 < r; j_3++)
				{
					int rand = random.Next(rand_max);
					bool bRandomNew = (r > 10) && ((1.0 * rand) / rand_max > 0.95);
					pt = generator3.GetRandomCoord();
					if (j_3 == 0 || bRandomNew)
					{
						polyBlue.StartPath(pt);
					}
					else
					{
						polyBlue.LineTo(pt);
					}
				}
				com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
				// Quad_tree
				com.epl.geometry.QuadTree quadTree = BuildQuadTree(polyBlue);
				com.epl.geometry.QuadTree.QuadTreeIterator iterator = quadTree.GetIterator();
				com.epl.geometry.SegmentIteratorImpl _segIterRed = ((com.epl.geometry.MultiPathImpl)polyRed._getImpl()).QuerySegmentIterator();
				while (_segIterRed.NextPath())
				{
					while (_segIterRed.HasNextSegment())
					{
						com.epl.geometry.Segment segmentRed = _segIterRed.NextSegment();
						segmentRed.QueryEnvelope2D(env);
						iterator.ResetIterator(env, 0.001);
						while (iterator.Next() != -1)
						{
							qCount++;
						}
					}
				}
				// Envelope_2D_intersector
				System.Collections.Generic.List<com.epl.geometry.Envelope2D> envelopes_red = new System.Collections.Generic.List<com.epl.geometry.Envelope2D>();
				System.Collections.Generic.List<com.epl.geometry.Envelope2D> envelopes_blue = new System.Collections.Generic.List<com.epl.geometry.Envelope2D>();
				com.epl.geometry.SegmentIterator segIterRed = polyRed.QuerySegmentIterator();
				while (segIterRed.NextPath())
				{
					while (segIterRed.HasNextSegment())
					{
						com.epl.geometry.Segment segment = segIterRed.NextSegment();
						env = new com.epl.geometry.Envelope2D();
						segment.QueryEnvelope2D(env);
						envelopes_red.Add(env);
					}
				}
				com.epl.geometry.SegmentIterator segIterBlue = polyBlue.QuerySegmentIterator();
				while (segIterBlue.NextPath())
				{
					while (segIterBlue.HasNextSegment())
					{
						com.epl.geometry.Segment segment = segIterBlue.NextSegment();
						env = new com.epl.geometry.Envelope2D();
						segment.QueryEnvelope2D(env);
						envelopes_blue.Add(env);
					}
				}
				com.epl.geometry.Envelope2DIntersectorImpl intersector = new com.epl.geometry.Envelope2DIntersectorImpl();
				intersector.SetTolerance(0.001);
				intersector.StartRedConstruction();
				for (int i = 0; i < envelopes_red.Count; i++)
				{
					intersector.AddRedEnvelope(i, envelopes_red[i]);
				}
				intersector.EndRedConstruction();
				intersector.StartBlueConstruction();
				for (int i_1 = 0; i_1 < envelopes_blue.Count; i_1++)
				{
					intersector.AddBlueEnvelope(i_1, envelopes_blue[i_1]);
				}
				intersector.EndBlueConstruction();
				while (intersector.Next())
				{
					eCount++;
				}
				NUnit.Framework.Assert.IsTrue(qCount == eCount);
			}
		}

		public static com.epl.geometry.QuadTree BuildQuadTree(com.epl.geometry.MultiPath multipath)
		{
			com.epl.geometry.Envelope2D extent = new com.epl.geometry.Envelope2D();
			multipath.QueryEnvelope2D(extent);
			com.epl.geometry.QuadTree quadTree = new com.epl.geometry.QuadTree(extent, 8);
			int hint_index = -1;
			com.epl.geometry.SegmentIterator seg_iter = multipath.QuerySegmentIterator();
			while (seg_iter.NextPath())
			{
				while (seg_iter.HasNextSegment())
				{
					com.epl.geometry.Segment segment = seg_iter.NextSegment();
					int index = seg_iter.GetStartPointIndex();
					com.epl.geometry.Envelope2D boundingbox = new com.epl.geometry.Envelope2D();
					segment.QueryEnvelope2D(boundingbox);
					hint_index = quadTree.Insert(index, boundingbox, hint_index);
				}
			}
			return quadTree;
		}
	}
}
