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


namespace com.epl.geometry
{
	internal class Boundary
	{
		internal static bool HasNonEmptyBoundary(com.epl.geometry.Geometry geom, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (geom.IsEmpty())
			{
				return false;
			}
			com.epl.geometry.Geometry.Type gt = geom.GetType();
			if (gt == com.epl.geometry.Geometry.Type.Polygon)
			{
				if (geom.CalculateArea2D() == 0)
				{
					return false;
				}
				return true;
			}
			else
			{
				if (gt == com.epl.geometry.Geometry.Type.Polyline)
				{
					bool[] b = new bool[1];
					b[0] = false;
					CalculatePolylineBoundary_(geom._getImpl(), progress_tracker, true, b);
					return b[0];
				}
				else
				{
					if (gt == com.epl.geometry.Geometry.Type.Envelope)
					{
						return true;
					}
					else
					{
						if (com.epl.geometry.Geometry.IsSegment(gt.Value()))
						{
							if (!((com.epl.geometry.Segment)geom).IsClosed())
							{
								return true;
							}
							return false;
						}
						else
						{
							if (com.epl.geometry.Geometry.IsPoint(gt.Value()))
							{
								return false;
							}
						}
					}
				}
			}
			return false;
		}

		internal static com.epl.geometry.Geometry Calculate(com.epl.geometry.Geometry geom, com.epl.geometry.ProgressTracker progress_tracker)
		{
			int gt = geom.GetType().Value();
			if (gt == com.epl.geometry.Geometry.GeometryType.Polygon)
			{
				com.epl.geometry.Polyline dst = new com.epl.geometry.Polyline(geom.GetDescription());
				if (!geom.IsEmpty())
				{
					((com.epl.geometry.MultiPathImpl)geom._getImpl())._copyToUnsafe((com.epl.geometry.MultiPathImpl)dst._getImpl());
				}
				return dst;
			}
			else
			{
				if (gt == com.epl.geometry.Geometry.GeometryType.Polyline)
				{
					return CalculatePolylineBoundary_(geom._getImpl(), progress_tracker, false, null);
				}
				else
				{
					if (gt == com.epl.geometry.Geometry.GeometryType.Envelope)
					{
						com.epl.geometry.Polyline dst = new com.epl.geometry.Polyline(geom.GetDescription());
						if (!geom.IsEmpty())
						{
							dst.AddEnvelope((com.epl.geometry.Envelope)geom, false);
						}
						return dst;
					}
					else
					{
						if (com.epl.geometry.Geometry.IsSegment(gt))
						{
							com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint(geom.GetDescription());
							if (!geom.IsEmpty() && !((com.epl.geometry.Segment)geom).IsClosed())
							{
								com.epl.geometry.Point pt = new com.epl.geometry.Point();
								((com.epl.geometry.Segment)geom).QueryStart(pt);
								mp.Add(pt);
								((com.epl.geometry.Segment)geom).QueryEnd(pt);
								mp.Add(pt);
							}
							return mp;
						}
						else
						{
							if (com.epl.geometry.Geometry.IsPoint(gt))
							{
								// returns empty point for points and multipoints.
								return null;
							}
						}
					}
				}
			}
			throw new System.ArgumentException();
		}

		private sealed class MultiPathImplBoundarySorter : com.epl.geometry.ClassicSort
		{
			internal com.epl.geometry.AttributeStreamOfDbl m_xy;

			internal sealed class CompareIndices : com.epl.geometry.AttributeStreamOfInt32.IntComparator
			{
				internal com.epl.geometry.AttributeStreamOfDbl m_xy;

				internal com.epl.geometry.Point2D pt1_helper;

				internal com.epl.geometry.Point2D pt2_helper;

				internal CompareIndices(com.epl.geometry.AttributeStreamOfDbl xy)
				{
					m_xy = xy;
					pt1_helper = new com.epl.geometry.Point2D();
					pt2_helper = new com.epl.geometry.Point2D();
				}

				public override int Compare(int v1, int v2)
				{
					m_xy.Read(2 * v1, pt1_helper);
					m_xy.Read(2 * v2, pt2_helper);
					return pt1_helper.Compare(pt2_helper);
				}
			}

			internal MultiPathImplBoundarySorter(com.epl.geometry.AttributeStreamOfDbl xy)
			{
				m_xy = xy;
			}

			public override void UserSort(int begin, int end, com.epl.geometry.AttributeStreamOfInt32 indices)
			{
				indices.Sort(begin, end, new com.epl.geometry.Boundary.MultiPathImplBoundarySorter.CompareIndices(m_xy));
			}

			public override double GetValue(int index)
			{
				return m_xy.Read(2 * index + 1);
			}
		}

		internal static com.epl.geometry.MultiPoint CalculatePolylineBoundary_(object impl, com.epl.geometry.ProgressTracker progress_tracker, bool only_check_non_empty_boundary, bool[] not_empty)
		{
			if (not_empty != null)
			{
				not_empty[0] = false;
			}
			com.epl.geometry.MultiPathImpl mpImpl = (com.epl.geometry.MultiPathImpl)impl;
			com.epl.geometry.MultiPoint dst = null;
			if (!only_check_non_empty_boundary)
			{
				dst = new com.epl.geometry.MultiPoint(mpImpl.GetDescription());
			}
			if (!mpImpl.IsEmpty())
			{
				com.epl.geometry.AttributeStreamOfInt32 indices = new com.epl.geometry.AttributeStreamOfInt32(0);
				indices.Reserve(mpImpl.GetPathCount() * 2);
				for (int ipath = 0, nPathCount = mpImpl.GetPathCount(); ipath < nPathCount; ipath++)
				{
					int path_size = mpImpl.GetPathSize(ipath);
					if (path_size > 0 && !mpImpl.IsClosedPathInXYPlane(ipath))
					{
						// closed
						// paths
						// of
						// polyline
						// do
						// not
						// contribute
						// to
						// the
						// boundary.
						int start = mpImpl.GetPathStart(ipath);
						indices.Add(start);
						int end = mpImpl.GetPathEnd(ipath) - 1;
						indices.Add(end);
					}
				}
				if (indices.Size() > 0)
				{
					com.epl.geometry.BucketSort sorter = new com.epl.geometry.BucketSort();
					com.epl.geometry.AttributeStreamOfDbl xy = (com.epl.geometry.AttributeStreamOfDbl)(mpImpl.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION));
					sorter.Sort(indices, 0, indices.Size(), new com.epl.geometry.Boundary.MultiPathImplBoundarySorter(xy));
					com.epl.geometry.Point2D ptPrev = new com.epl.geometry.Point2D();
					xy.Read(2 * indices.Get(0), ptPrev);
					int ind = 0;
					int counter = 1;
					com.epl.geometry.Point point = new com.epl.geometry.Point();
					com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
					for (int i = 1, n = indices.Size(); i < n; i++)
					{
						xy.Read(2 * indices.Get(i), pt);
						if (pt.IsEqual(ptPrev))
						{
							if (indices.Get(ind) > indices.Get(i))
							{
								// remove duplicate point
								indices.Set(ind, com.epl.geometry.NumberUtils.IntMax());
								ind = i;
							}
							else
							{
								// just for the heck of it, have the first
								// point in the order to be added to the
								// boundary.
								indices.Set(i, com.epl.geometry.NumberUtils.IntMax());
							}
							counter++;
						}
						else
						{
							if ((counter & 1) == 0)
							{
								// remove boundary point
								indices.Set(ind, com.epl.geometry.NumberUtils.IntMax());
							}
							else
							{
								if (only_check_non_empty_boundary)
								{
									if (not_empty != null)
									{
										not_empty[0] = true;
									}
									return null;
								}
							}
							ptPrev.SetCoords(pt);
							ind = i;
							counter = 1;
						}
					}
					if ((counter & 1) == 0)
					{
						// remove the point
						indices.Set(ind, com.epl.geometry.NumberUtils.IntMax());
					}
					else
					{
						if (only_check_non_empty_boundary)
						{
							if (not_empty != null)
							{
								not_empty[0] = true;
							}
							return null;
						}
					}
					if (!only_check_non_empty_boundary)
					{
						indices.Sort(0, indices.Size());
						for (int i_1 = 0, n = indices.Size(); i_1 < n; i_1++)
						{
							if (indices.Get(i_1) == com.epl.geometry.NumberUtils.IntMax())
							{
								break;
							}
							mpImpl.GetPointByVal(indices.Get(i_1), point);
							dst.Add(point);
						}
					}
				}
			}
			if (only_check_non_empty_boundary)
			{
				return null;
			}
			return dst;
		}
	}
}
