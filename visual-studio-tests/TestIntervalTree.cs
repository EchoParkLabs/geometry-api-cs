/*
Copyright 2017 Echo Park Labs

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

email: info@echoparklabs.io
*/
using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestIntervalTree : NUnit.Framework.TestFixtureAttribute
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

		internal static void Construct(com.epl.geometry.IntervalTreeImpl interval_tree, System.Collections.Generic.List<com.epl.geometry.Envelope1D> intervals)
		{
			interval_tree.StartConstruction();
			for (int i = 0; i < intervals.Count; i++)
			{
				interval_tree.AddInterval(intervals[i]);
			}
			interval_tree.EndConstruction();
		}

		[NUnit.Framework.Test]
		public static void TestIntervalTree__1()
		{
			System.Collections.Generic.List<com.epl.geometry.Envelope1D> intervals = new System.Collections.Generic.List<com.epl.geometry.Envelope1D>(0);
			com.epl.geometry.Envelope1D env0 = new com.epl.geometry.Envelope1D(2, 3);
			com.epl.geometry.Envelope1D env1 = new com.epl.geometry.Envelope1D(5, 13);
			com.epl.geometry.Envelope1D env2 = new com.epl.geometry.Envelope1D(6, 9);
			com.epl.geometry.Envelope1D env3 = new com.epl.geometry.Envelope1D(8, 10);
			com.epl.geometry.Envelope1D env4 = new com.epl.geometry.Envelope1D(11, 12);
			com.epl.geometry.Envelope1D env5 = new com.epl.geometry.Envelope1D(1, 3);
			com.epl.geometry.Envelope1D env6 = new com.epl.geometry.Envelope1D(0, 2);
			com.epl.geometry.Envelope1D env7 = new com.epl.geometry.Envelope1D(4, 7);
			com.epl.geometry.Envelope1D env8;
			intervals.Add(env0);
			intervals.Add(env1);
			intervals.Add(env2);
			intervals.Add(env3);
			intervals.Add(env4);
			intervals.Add(env5);
			intervals.Add(env6);
			intervals.Add(env7);
			int counter;
			com.epl.geometry.IntervalTreeImpl intervalTree = new com.epl.geometry.IntervalTreeImpl(false);
			Construct(intervalTree, intervals);
			com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl iterator = intervalTree.GetIterator(new com.epl.geometry.Envelope1D(-1, 14), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 8);
			iterator.ResetIterator(new com.epl.geometry.Envelope1D(2.5, 10.5), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 6);
			iterator.ResetIterator(5.0, 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 2);
			iterator.ResetIterator(7, 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 3);
			iterator.ResetIterator(new com.epl.geometry.Envelope1D(2.0, 10.5), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 7);
			iterator.ResetIterator(new com.epl.geometry.Envelope1D(2.5, 11), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 7);
			iterator.ResetIterator(new com.epl.geometry.Envelope1D(2.1, 2.5), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 2);
			iterator.ResetIterator(new com.epl.geometry.Envelope1D(2.1, 5), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 4);
			iterator.ResetIterator(new com.epl.geometry.Envelope1D(2.0, 5), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 5);
			iterator.ResetIterator(new com.epl.geometry.Envelope1D(5.0, 11), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 5);
			iterator.ResetIterator(new com.epl.geometry.Envelope1D(8, 10.5), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 3);
			iterator.ResetIterator(new com.epl.geometry.Envelope1D(10, 11), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 3);
			iterator.ResetIterator(new com.epl.geometry.Envelope1D(10, 10.9), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 2);
			iterator.ResetIterator(new com.epl.geometry.Envelope1D(11.5, 12), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 2);
			env0 = new com.epl.geometry.Envelope1D(0, 4);
			env1 = new com.epl.geometry.Envelope1D(6, 7);
			env2 = new com.epl.geometry.Envelope1D(9, 10);
			env3 = new com.epl.geometry.Envelope1D(9, 11);
			env4 = new com.epl.geometry.Envelope1D(7, 12);
			env5 = new com.epl.geometry.Envelope1D(13, 15);
			env6 = new com.epl.geometry.Envelope1D(1, 6);
			env7 = new com.epl.geometry.Envelope1D(3, 3);
			env8 = new com.epl.geometry.Envelope1D(8, 8);
			intervals.Clear();
			intervals.Add(env0);
			intervals.Add(env1);
			intervals.Add(env2);
			intervals.Add(env3);
			intervals.Add(env4);
			intervals.Add(env5);
			intervals.Add(env6);
			intervals.Add(env7);
			intervals.Add(env8);
			com.epl.geometry.IntervalTreeImpl intervalTree2 = new com.epl.geometry.IntervalTreeImpl(true);
			Construct(intervalTree2, intervals);
			intervalTree2.Insert(0);
			intervalTree2.Insert(1);
			intervalTree2.Insert(2);
			intervalTree2.Insert(3);
			intervalTree2.Insert(4);
			intervalTree2.Insert(5);
			intervalTree2.Insert(6);
			intervalTree2.Insert(7);
			intervalTree2.Insert(8);
			iterator = intervalTree2.GetIterator(new com.epl.geometry.Envelope1D(8, 8), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 2);
			iterator.ResetIterator(new com.epl.geometry.Envelope1D(3, 7), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 5);
			iterator.ResetIterator(new com.epl.geometry.Envelope1D(1, 3), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 3);
			iterator.ResetIterator(new com.epl.geometry.Envelope1D(6, 9), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 6);
			iterator.ResetIterator(new com.epl.geometry.Envelope1D(10, 14), 0.0);
			counter = 0;
			while (iterator.Next() != -1)
			{
				counter++;
			}
			NUnit.Framework.Assert.IsTrue(counter == 4);
			env0 = new com.epl.geometry.Envelope1D(11, 14);
			env1 = new com.epl.geometry.Envelope1D(21, 36);
			env2 = new com.epl.geometry.Envelope1D(15, 19);
			env3 = new com.epl.geometry.Envelope1D(3, 8);
			env4 = new com.epl.geometry.Envelope1D(34, 38);
			env5 = new com.epl.geometry.Envelope1D(23, 27);
			env6 = new com.epl.geometry.Envelope1D(6, 36);
			intervals.Clear();
			intervals.Add(env0);
			intervals.Add(env1);
			intervals.Add(env2);
			intervals.Add(env3);
			intervals.Add(env4);
			intervals.Add(env5);
			intervals.Add(env6);
			com.epl.geometry.IntervalTreeImpl intervalTree3 = new com.epl.geometry.IntervalTreeImpl(false);
			Construct(intervalTree3, intervals);
			iterator = intervalTree3.GetIterator(new com.epl.geometry.Envelope1D(50, 50), 0.0);
			System.Diagnostics.Debug.Assert((iterator.Next() == -1));
		}

		[NUnit.Framework.Test]
		public static void TestIntervalTree_RandomConstruction()
		{
			int pointcount = 0;
			int passcount = 1000;
			int figureSize = 50;
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope();
			env.SetCoords(-10000, -10000, 10000, 10000);
			com.epl.geometry.RandomCoordinateGenerator generator = new com.epl.geometry.RandomCoordinateGenerator(System.Math.Max(figureSize, 10000), env, 0.001);
			System.Random random = new System.Random(2013);
			int rand_max = 98765;
			System.Collections.Generic.List<com.epl.geometry.Envelope1D> intervals = new System.Collections.Generic.List<com.epl.geometry.Envelope1D>();
			com.epl.geometry.AttributeStreamOfInt8 intervalsFound = new com.epl.geometry.AttributeStreamOfInt8(0);
			for (int i = 0; i < passcount; i++)
			{
				int r = figureSize;
				if (r < 3)
				{
					continue;
				}
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
				com.epl.geometry.Point pt;
				for (int j = 0; j < r; j++)
				{
					int rand = random.Next(rand_max);
					bool bRandomNew = (r > 10) && ((1.0 * rand) / rand_max > 0.95);
					pt = generator.GetRandomCoord();
					if (j == 0 || bRandomNew)
					{
						poly.StartPath(pt);
					}
					else
					{
						poly.LineTo(pt);
					}
				}
				{
					intervals.Clear();
					com.epl.geometry.SegmentIterator seg_iter = poly.QuerySegmentIterator();
					com.epl.geometry.Envelope1D interval;
					com.epl.geometry.Envelope1D range = poly.QueryInterval(com.epl.geometry.VertexDescription.Semantics.POSITION, 0);
					range.vmin -= 0.01;
					range.vmax += 0.01;
					while (seg_iter.NextPath())
					{
						while (seg_iter.HasNextSegment())
						{
							com.epl.geometry.Segment segment = seg_iter.NextSegment();
							interval = segment.QueryInterval(com.epl.geometry.VertexDescription.Semantics.POSITION, 0);
							intervals.Add(interval);
						}
					}
					intervalsFound.Resize(intervals.Count, 0);
					// Just test construction for assertions
					com.epl.geometry.IntervalTreeImpl intervalTree = new com.epl.geometry.IntervalTreeImpl(true);
					Construct(intervalTree, intervals);
					for (int j_1 = 0; j_1 < intervals.Count; j_1++)
					{
						intervalTree.Insert(j_1);
					}
					com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl iterator = intervalTree.GetIterator(range, 0.0);
					int count = 0;
					int handle;
					while ((handle = iterator.Next()) != -1)
					{
						count++;
						intervalsFound.Write(handle, unchecked((byte)1));
					}
					NUnit.Framework.Assert.IsTrue(count == intervals.Count);
					for (int j_2 = 0; j_2 < intervalsFound.Size(); j_2++)
					{
						interval = intervals[j_2];
						NUnit.Framework.Assert.IsTrue(intervalsFound.Read(j_2) == 1);
					}
					for (int j_3 = 0; j_3 < intervals.Count >> 1; j_3++)
					{
						intervalTree.Remove(j_3);
					}
					iterator.ResetIterator(range, 0.0);
					count = 0;
					while ((handle = iterator.Next()) != -1)
					{
						count++;
						intervalsFound.Write(handle, unchecked((byte)1));
					}
					NUnit.Framework.Assert.IsTrue(count == intervals.Count - (intervals.Count >> 1));
					for (int j_4 = (intervals.Count >> 1); j_4 < intervals.Count; j_4++)
					{
						intervalTree.Remove(j_4);
					}
				}
			}
		}
	}
}
