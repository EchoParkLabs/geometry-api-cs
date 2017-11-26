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
	public class TestQuadTree : NUnit.Framework.TestFixtureAttribute
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
		public static void Test1()
		{
			{
				com.epl.geometry.QuadTree quad_tree = new com.epl.geometry.QuadTree(com.epl.geometry.Envelope2D.Construct(-10, -10, 10, 10), 8);
				com.epl.geometry.QuadTree.QuadTreeIterator qt = quad_tree.GetIterator(true);
				NUnit.Framework.Assert.IsTrue(qt.Next() == -1);
				qt.ResetIterator(com.epl.geometry.Envelope2D.Construct(0, 0, 0, 0), 0);
				NUnit.Framework.Assert.IsTrue(quad_tree.GetIntersectionCount(com.epl.geometry.Envelope2D.Construct(0, 0, 0, 0), 0, 10) == 0);
				NUnit.Framework.Assert.IsTrue(quad_tree.GetElementCount() == 0);
			}
			com.epl.geometry.Polyline polyline;
			polyline = MakePolyline();
			com.epl.geometry.MultiPathImpl polylineImpl = (com.epl.geometry.MultiPathImpl)polyline._getImpl();
			com.epl.geometry.QuadTree quadtree = BuildQuadTree_(polylineImpl, false);
			com.epl.geometry.Line queryline = new com.epl.geometry.Line(34, 9, 66, 46);
			com.epl.geometry.QuadTree.QuadTreeIterator qtIter = quadtree.GetIterator();
			NUnit.Framework.Assert.IsTrue(qtIter.Next() == -1);
			qtIter.ResetIterator(queryline, 0.0);
			int element_handle = qtIter.Next();
			while (element_handle > 0)
			{
				int index = quadtree.GetElement(element_handle);
				NUnit.Framework.Assert.IsTrue(index == 6 || index == 8 || index == 14);
				element_handle = qtIter.Next();
			}
			com.epl.geometry.Envelope2D envelope = new com.epl.geometry.Envelope2D(34, 9, 66, 46);
			com.epl.geometry.Polygon queryPolygon = new com.epl.geometry.Polygon();
			queryPolygon.AddEnvelope(envelope, true);
			qtIter.ResetIterator(queryline, 0.0);
			element_handle = qtIter.Next();
			while (element_handle > 0)
			{
				int index = quadtree.GetElement(element_handle);
				NUnit.Framework.Assert.IsTrue(index == 6 || index == 8 || index == 14);
				element_handle = qtIter.Next();
			}
		}

		[NUnit.Framework.Test]
		public static void TestQuadTreeWithDuplicates()
		{
			int pass_count = 10;
			int figure_size = 400;
			int figure_size2 = 100;
			com.epl.geometry.Envelope extent1 = new com.epl.geometry.Envelope();
			extent1.SetCoords(-100000, -100000, 100000, 100000);
			com.epl.geometry.RandomCoordinateGenerator generator1 = new com.epl.geometry.RandomCoordinateGenerator(System.Math.Max(figure_size, 10000), extent1, 0.001);
			System.Random random = new System.Random(2013);
			int rand_max = 32;
			com.epl.geometry.Polygon poly_red = new com.epl.geometry.Polygon();
			com.epl.geometry.Polygon poly_blue = new com.epl.geometry.Polygon();
			int r = figure_size;
			for (int c = 0; c < pass_count; c++)
			{
				com.epl.geometry.Point pt;
				for (int j = 0; j < r; j++)
				{
					int rand = random.Next(rand_max);
					bool b_random_new = r > 10 && ((1.0 * rand) / rand_max > 0.95);
					pt = generator1.GetRandomCoord();
					if (j == 0 || b_random_new)
					{
						poly_blue.StartPath(pt);
					}
					else
					{
						poly_blue.LineTo(pt);
					}
				}
				com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
				com.epl.geometry.QuadTree quad_tree_blue = BuildQuadTree_((com.epl.geometry.MultiPathImpl)poly_blue._getImpl(), false);
				com.epl.geometry.QuadTree quad_tree_blue_duplicates = BuildQuadTree_((com.epl.geometry.MultiPathImpl)poly_blue._getImpl(), true);
				com.epl.geometry.Envelope2D e1 = quad_tree_blue.GetDataExtent();
				com.epl.geometry.Envelope2D e2 = quad_tree_blue_duplicates.GetDataExtent();
				NUnit.Framework.Assert.IsTrue(e1.Equals(e2));
				NUnit.Framework.Assert.IsTrue(quad_tree_blue.GetElementCount() == poly_blue.GetSegmentCount());
				com.epl.geometry.SegmentIterator seg_iter_blue = poly_blue.QuerySegmentIterator();
				poly_red.SetEmpty();
				r = figure_size2;
				if (r < 3)
				{
					continue;
				}
				for (int j_1 = 0; j_1 < r; j_1++)
				{
					int rand = random.Next(rand_max);
					bool b_random_new = r > 10 && ((1.0 * rand) / rand_max > 0.95);
					pt = generator1.GetRandomCoord();
					if (j_1 == 0 || b_random_new)
					{
						poly_red.StartPath(pt);
					}
					else
					{
						poly_red.LineTo(pt);
					}
				}
				com.epl.geometry.QuadTree.QuadTreeIterator iterator = quad_tree_blue.GetIterator();
				com.epl.geometry.SegmentIteratorImpl seg_iter_red = ((com.epl.geometry.MultiPathImpl)poly_red._getImpl()).QuerySegmentIterator();
				System.Collections.Generic.Dictionary<int, bool> map1 = new System.Collections.Generic.Dictionary<int, bool>(0);
				int count = 0;
				int intersections_per_query = 0;
				while (seg_iter_red.NextPath())
				{
					while (seg_iter_red.HasNextSegment())
					{
						com.epl.geometry.Segment segment_red = seg_iter_red.NextSegment();
						segment_red.QueryEnvelope2D(env);
						iterator.ResetIterator(env, 0.0);
						int count_upper = 0;
						int element_handle;
						while ((element_handle = iterator.Next()) != -1)
						{
							count_upper++;
							int index = quad_tree_blue.GetElement(element_handle);
							bool iter = map1.ContainsKey(index);
							if (!iter)
							{
								count++;
								map1[index] = true;
							}
							intersections_per_query++;
						}
						int intersection_count = quad_tree_blue.GetIntersectionCount(env, 0.0, -1);
						NUnit.Framework.Assert.IsTrue(intersection_count == count_upper);
					}
				}
				seg_iter_red.ResetToFirstPath();
				System.Collections.Generic.Dictionary<int, bool> map2 = new System.Collections.Generic.Dictionary<int, bool>(0);
				com.epl.geometry.QuadTree.QuadTreeIterator iterator_duplicates = quad_tree_blue_duplicates.GetIterator();
				int count_duplicates = 0;
				int intersections_per_query_duplicates = 0;
				while (seg_iter_red.NextPath())
				{
					while (seg_iter_red.HasNextSegment())
					{
						com.epl.geometry.Segment segment_red = seg_iter_red.NextSegment();
						segment_red.QueryEnvelope2D(env);
						iterator_duplicates.ResetIterator(env, 0.0);
						int count_lower = 0;
						System.Collections.Generic.Dictionary<int, bool> map_per_query = new System.Collections.Generic.Dictionary<int, bool>(0);
						int count_upper = 0;
						int element_handle;
						while ((element_handle = iterator_duplicates.Next()) != -1)
						{
							count_upper++;
							int index = quad_tree_blue_duplicates.GetElement(element_handle);
							bool iter = map2.ContainsKey(index);
							if (!iter)
							{
								count_duplicates++;
								map2[index] = true;
							}
							bool iter_lower = map_per_query.ContainsKey(index);
							if (!iter_lower)
							{
								count_lower++;
								intersections_per_query_duplicates++;
								map_per_query[index] = true;
							}
							int q = quad_tree_blue_duplicates.GetQuad(element_handle);
							NUnit.Framework.Assert.IsTrue(quad_tree_blue_duplicates.GetSubTreeElementCount(q) >= quad_tree_blue_duplicates.GetContainedSubTreeElementCount(q));
						}
						int intersection_count = quad_tree_blue_duplicates.GetIntersectionCount(env, 0.0, -1);
						bool b_has_data = quad_tree_blue_duplicates.HasData(env, 0.0);
						NUnit.Framework.Assert.IsTrue(b_has_data || intersection_count == 0);
						NUnit.Framework.Assert.IsTrue(count_lower <= intersection_count && intersection_count <= count_upper);
						NUnit.Framework.Assert.IsTrue(count_upper <= 4 * count_lower);
					}
				}
				NUnit.Framework.Assert.IsTrue(count == count_duplicates);
				NUnit.Framework.Assert.IsTrue(intersections_per_query == intersections_per_query_duplicates);
			}
		}

		[NUnit.Framework.Test]
		public static void TestSortedIterator()
		{
			int pass_count = 10;
			int figure_size = 400;
			int figure_size2 = 100;
			com.epl.geometry.Envelope extent1 = new com.epl.geometry.Envelope();
			extent1.SetCoords(-100000, -100000, 100000, 100000);
			com.epl.geometry.RandomCoordinateGenerator generator1 = new com.epl.geometry.RandomCoordinateGenerator(System.Math.Max(figure_size, 10000), extent1, 0.001);
			System.Random random = new System.Random(2013);
			int rand_max = 32;
			com.epl.geometry.Polygon poly_red = new com.epl.geometry.Polygon();
			com.epl.geometry.Polygon poly_blue = new com.epl.geometry.Polygon();
			int r = figure_size;
			for (int c = 0; c < pass_count; c++)
			{
				com.epl.geometry.Point pt;
				for (int j = 0; j < r; j++)
				{
					int rand = random.Next(rand_max);
					bool b_random_new = r > 10 && ((1.0 * rand) / rand_max > 0.95);
					pt = generator1.GetRandomCoord();
					if (j == 0 || b_random_new)
					{
						poly_blue.StartPath(pt);
					}
					else
					{
						poly_blue.LineTo(pt);
					}
				}
				com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
				com.epl.geometry.QuadTree quad_tree_blue = BuildQuadTree_((com.epl.geometry.MultiPathImpl)poly_blue._getImpl(), false);
				com.epl.geometry.Envelope2D e1 = quad_tree_blue.GetDataExtent();
				NUnit.Framework.Assert.IsTrue(quad_tree_blue.GetElementCount() == poly_blue.GetSegmentCount());
				com.epl.geometry.SegmentIterator seg_iter_blue = poly_blue.QuerySegmentIterator();
				poly_red.SetEmpty();
				r = figure_size2;
				if (r < 3)
				{
					continue;
				}
				for (int j_1 = 0; j_1 < r; j_1++)
				{
					int rand = random.Next(rand_max);
					bool b_random_new = r > 10 && ((1.0 * rand) / rand_max > 0.95);
					pt = generator1.GetRandomCoord();
					if (j_1 == 0 || b_random_new)
					{
						poly_red.StartPath(pt);
					}
					else
					{
						poly_red.LineTo(pt);
					}
				}
				com.epl.geometry.QuadTree.QuadTreeIterator iterator = quad_tree_blue.GetIterator();
				com.epl.geometry.SegmentIteratorImpl seg_iter_red = ((com.epl.geometry.MultiPathImpl)poly_red._getImpl()).QuerySegmentIterator();
				System.Collections.Generic.Dictionary<int, bool> map1 = new System.Collections.Generic.Dictionary<int, bool>(0);
				int count = 0;
				int intersections_per_query = 0;
				while (seg_iter_red.NextPath())
				{
					while (seg_iter_red.HasNextSegment())
					{
						com.epl.geometry.Segment segment_red = seg_iter_red.NextSegment();
						segment_red.QueryEnvelope2D(env);
						iterator.ResetIterator(env, 0.0);
						int count_upper = 0;
						int element_handle;
						while ((element_handle = iterator.Next()) != -1)
						{
							count_upper++;
							int index = quad_tree_blue.GetElement(element_handle);
							bool iter = map1.ContainsKey(index);
							if (!iter)
							{
								count++;
								map1[index] = true;
							}
							intersections_per_query++;
						}
						int intersection_count = quad_tree_blue.GetIntersectionCount(env, 0.0, -1);
						NUnit.Framework.Assert.IsTrue(intersection_count == count_upper);
					}
				}
				seg_iter_red.ResetToFirstPath();
				System.Collections.Generic.Dictionary<int, bool> map2 = new System.Collections.Generic.Dictionary<int, bool>(0);
				com.epl.geometry.QuadTree.QuadTreeIterator sorted_iterator = quad_tree_blue.GetIterator(true);
				int count_sorted = 0;
				int intersections_per_query_sorted = 0;
				while (seg_iter_red.NextPath())
				{
					while (seg_iter_red.HasNextSegment())
					{
						com.epl.geometry.Segment segment_red = seg_iter_red.NextSegment();
						segment_red.QueryEnvelope2D(env);
						sorted_iterator.ResetIterator(env, 0.0);
						int count_upper_sorted = 0;
						int element_handle;
						int last_index = -1;
						while ((element_handle = sorted_iterator.Next()) != -1)
						{
							count_upper_sorted++;
							int index = quad_tree_blue.GetElement(element_handle);
							NUnit.Framework.Assert.IsTrue(last_index < index);
							// ensure the element handles are returned in sorted order
							last_index = index;
							bool iter = map2.ContainsKey(index);
							if (!iter)
							{
								count_sorted++;
								map2[index] = true;
							}
							intersections_per_query_sorted++;
						}
						int intersection_count = quad_tree_blue.GetIntersectionCount(env, 0.0, -1);
						NUnit.Framework.Assert.IsTrue(intersection_count == count_upper_sorted);
					}
				}
				NUnit.Framework.Assert.IsTrue(count == count_sorted);
				NUnit.Framework.Assert.IsTrue(intersections_per_query == intersections_per_query_sorted);
			}
		}

		[NUnit.Framework.Test]
		public static void Test_perf_quad_tree()
		{
			com.epl.geometry.Envelope extent1 = new com.epl.geometry.Envelope();
			extent1.SetCoords(-1000, -1000, 1000, 1000);
			com.epl.geometry.RandomCoordinateGenerator generator1 = new com.epl.geometry.RandomCoordinateGenerator(1000, extent1, 0.001);
			//HiResTimer timer;
			for (int N = 16; N <= 1024; N *= 2)
			{
				//timer.StartMeasurement();
				com.epl.geometry.Envelope2D extent = new com.epl.geometry.Envelope2D();
				extent.SetCoords(-1000, -1000, 1000, 1000);
				System.Collections.Generic.Dictionary<int, com.epl.geometry.Envelope2D> data = new System.Collections.Generic.Dictionary<int, com.epl.geometry.Envelope2D>(0);
				com.epl.geometry.QuadTree qt = new com.epl.geometry.QuadTree(extent, 10);
				for (int i = 0; i < N; i++)
				{
					com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
					com.epl.geometry.Point2D center = generator1.GetRandomCoord().GetXY();
					double w = 10;
					env.SetCoords(center, w, w);
					env.Intersect(extent);
					if (env.IsEmpty())
					{
						continue;
					}
					int h = qt.Insert(i, env);
					data[h] = env;
				}
				int ecount = 0;
				com.epl.geometry.AttributeStreamOfInt32 handles = new com.epl.geometry.AttributeStreamOfInt32(0);
				com.epl.geometry.QuadTree.QuadTreeIterator iter = qt.GetIterator();
				System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int, com.epl.geometry.Envelope2D>> pairs = data.GetEnumerator();
				while (pairs.MoveNext())
				{
					System.Collections.Generic.KeyValuePair<int, com.epl.geometry.Envelope2D> entry = pairs.Current;
					iter.ResetIterator((com.epl.geometry.Envelope2D)entry.Value, 0.001);
					bool remove_self = false;
					for (int h = iter.Next(); h != -1; h = iter.Next())
					{
						if (h != entry.Key)
						{
							handles.Add(h);
						}
						else
						{
							remove_self = true;
						}
						ecount++;
					}
					for (int i_1 = 0; i_1 < handles.Size(); i_1++)
					{
						qt.RemoveElement(handles.Get(i_1));
					}
					//remove elements that were selected.
					if (remove_self)
					{
						qt.RemoveElement(entry.Key);
					}
					handles.Resize(0);
				}
			}
		}

		//printf("%d %0.3f (%I64d, %f, mem %I64d)\n", N, timer.GetMilliseconds(), ecount, ecount / double(N * N), memsize);
		[NUnit.Framework.Test]
		public static void Test2()
		{
			com.epl.geometry.MultiPoint multipoint = new com.epl.geometry.MultiPoint();
			for (int i = 0; i < 100; i++)
			{
				for (int j = 0; j < 100; j++)
				{
					multipoint.Add(i, j);
				}
			}
			com.epl.geometry.Envelope2D extent = new com.epl.geometry.Envelope2D();
			multipoint.QueryEnvelope2D(extent);
			com.epl.geometry.MultiPointImpl multipointImpl = (com.epl.geometry.MultiPointImpl)multipoint._getImpl();
			com.epl.geometry.QuadTree quadtree = BuildQuadTree_(multipointImpl);
			com.epl.geometry.QuadTree.QuadTreeIterator qtIter = quadtree.GetIterator();
			NUnit.Framework.Assert.IsTrue(qtIter.Next() == -1);
			int count = 0;
			qtIter.ResetIterator(extent, 0.0);
			while (qtIter.Next() != -1)
			{
				count++;
			}
			NUnit.Framework.Assert.IsTrue(count == 10000);
		}

		public static com.epl.geometry.Polyline MakePolyline()
		{
			com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
			// 0
			poly.StartPath(0, 40);
			poly.LineTo(30, 0);
			// 1
			poly.StartPath(20, 70);
			poly.LineTo(45, 100);
			// 2
			poly.StartPath(50, 100);
			poly.LineTo(50, 60);
			// 3
			poly.StartPath(35, 25);
			poly.LineTo(65, 45);
			// 4
			poly.StartPath(60, 10);
			poly.LineTo(65, 35);
			// 5
			poly.StartPath(60, 60);
			poly.LineTo(100, 60);
			// 6
			poly.StartPath(80, 10);
			poly.LineTo(80, 99);
			// 7
			poly.StartPath(60, 60);
			poly.LineTo(65, 35);
			return poly;
		}

		internal static com.epl.geometry.QuadTree BuildQuadTree_(com.epl.geometry.MultiPathImpl multipathImpl, bool bStoreDuplicates)
		{
			com.epl.geometry.Envelope2D extent = new com.epl.geometry.Envelope2D();
			multipathImpl.QueryEnvelope2D(extent);
			com.epl.geometry.QuadTree quadTree = new com.epl.geometry.QuadTree(extent, 8, bStoreDuplicates);
			int hint_index = -1;
			com.epl.geometry.Envelope2D boundingbox = new com.epl.geometry.Envelope2D();
			com.epl.geometry.SegmentIteratorImpl seg_iter = multipathImpl.QuerySegmentIterator();
			while (seg_iter.NextPath())
			{
				while (seg_iter.HasNextSegment())
				{
					com.epl.geometry.Segment segment = seg_iter.NextSegment();
					int index = seg_iter.GetStartPointIndex();
					segment.QueryEnvelope2D(boundingbox);
					hint_index = quadTree.Insert(index, boundingbox, hint_index);
				}
			}
			return quadTree;
		}

		internal static com.epl.geometry.QuadTree BuildQuadTree_(com.epl.geometry.MultiPointImpl multipointImpl)
		{
			com.epl.geometry.Envelope2D extent = new com.epl.geometry.Envelope2D();
			multipointImpl.QueryEnvelope2D(extent);
			com.epl.geometry.QuadTree quadTree = new com.epl.geometry.QuadTree(extent, 8);
			com.epl.geometry.Envelope2D boundingbox = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Point2D pt;
			for (int i = 0; i < multipointImpl.GetPointCount(); i++)
			{
				pt = multipointImpl.GetXY(i);
				boundingbox.SetCoords(pt.x, pt.y, pt.x, pt.y);
				quadTree.Insert(i, boundingbox, -1);
			}
			return quadTree;
		}
	}
}
