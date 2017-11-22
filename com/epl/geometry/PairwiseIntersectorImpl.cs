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


namespace com.epl.geometry
{
	internal class PairwiseIntersectorImpl
	{
		private com.epl.geometry.MultiPathImpl m_multi_path_impl_a;

		private com.epl.geometry.MultiPathImpl m_multi_path_impl_b;

		private bool m_b_paths;

		private bool m_b_quad_tree;

		private bool m_b_done;

		private bool m_b_swap_elements;

		private double m_tolerance;

		private int m_path_index;

		private int m_element_handle;

		private com.epl.geometry.Envelope2D m_paths_query = new com.epl.geometry.Envelope2D();

		private com.epl.geometry.QuadTreeImpl m_quad_tree;

		private com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl m_qt_iter;

		private com.epl.geometry.SegmentIteratorImpl m_seg_iter;

		private com.epl.geometry.Envelope2DIntersectorImpl m_intersector;

		private int m_function;

		private abstract class State
		{
			public const int nextPath = 0;

			public const int nextSegment = 1;

			public const int iterate = 2;
			// Quad_tree
			// only used for m_b_paths == true case
			// Envelope_2D_intersector
		}

		private static class StateConstants
		{
		}

		internal PairwiseIntersectorImpl(com.epl.geometry.MultiPathImpl multi_path_impl_a, com.epl.geometry.MultiPathImpl multi_path_impl_b, double tolerance, bool b_paths)
		{
			m_multi_path_impl_a = multi_path_impl_a;
			m_multi_path_impl_b = multi_path_impl_b;
			m_b_paths = b_paths;
			m_path_index = -1;
			m_b_quad_tree = false;
			com.epl.geometry.GeometryAccelerators geometry_accelerators_a = multi_path_impl_a._getAccelerators();
			if (geometry_accelerators_a != null)
			{
				com.epl.geometry.QuadTreeImpl qtree_a = (!b_paths ? geometry_accelerators_a.GetQuadTree() : geometry_accelerators_a.GetQuadTreeForPaths());
				if (qtree_a != null)
				{
					m_b_done = false;
					m_tolerance = tolerance;
					m_quad_tree = qtree_a;
					m_qt_iter = m_quad_tree.GetIterator();
					m_b_quad_tree = true;
					m_b_swap_elements = true;
					m_function = com.epl.geometry.PairwiseIntersectorImpl.State.nextPath;
					if (!b_paths)
					{
						m_seg_iter = multi_path_impl_b.QuerySegmentIterator();
					}
					else
					{
						m_path_index = multi_path_impl_b.GetPathCount();
					}
				}
			}
			// we will iterate backwards until we hit -1
			if (!m_b_quad_tree)
			{
				com.epl.geometry.GeometryAccelerators geometry_accelerators_b = multi_path_impl_b._getAccelerators();
				if (geometry_accelerators_b != null)
				{
					com.epl.geometry.QuadTreeImpl qtree_b = (!b_paths ? geometry_accelerators_b.GetQuadTree() : geometry_accelerators_b.GetQuadTreeForPaths());
					if (qtree_b != null)
					{
						m_b_done = false;
						m_tolerance = tolerance;
						m_quad_tree = qtree_b;
						m_qt_iter = m_quad_tree.GetIterator();
						m_b_quad_tree = true;
						m_b_swap_elements = false;
						m_function = com.epl.geometry.PairwiseIntersectorImpl.State.nextPath;
						if (!b_paths)
						{
							m_seg_iter = multi_path_impl_a.QuerySegmentIterator();
						}
						else
						{
							m_path_index = multi_path_impl_a.GetPathCount();
						}
					}
				}
			}
			// we will iterate backwards until we hit -1
			if (!m_b_quad_tree)
			{
				if (!b_paths)
				{
					m_intersector = com.epl.geometry.InternalUtils.GetEnvelope2DIntersector(multi_path_impl_a, multi_path_impl_b, tolerance);
				}
				else
				{
					bool b_simple_a = multi_path_impl_a.GetIsSimple(0.0) >= 1;
					bool b_simple_b = multi_path_impl_b.GetIsSimple(0.0) >= 1;
					m_intersector = com.epl.geometry.InternalUtils.GetEnvelope2DIntersectorForParts(multi_path_impl_a, multi_path_impl_b, tolerance, b_simple_a, b_simple_b);
				}
			}
		}

		internal virtual bool Next()
		{
			if (m_b_quad_tree)
			{
				if (m_b_done)
				{
					return false;
				}
				bool b_searching = true;
				while (b_searching)
				{
					switch (m_function)
					{
						case com.epl.geometry.PairwiseIntersectorImpl.State.nextPath:
						{
							b_searching = NextPath_();
							break;
						}

						case com.epl.geometry.PairwiseIntersectorImpl.State.nextSegment:
						{
							b_searching = NextSegment_();
							break;
						}

						case com.epl.geometry.PairwiseIntersectorImpl.State.iterate:
						{
							b_searching = Iterate_();
							break;
						}

						default:
						{
							throw com.epl.geometry.GeometryException.GeometryInternalError();
						}
					}
				}
				if (m_b_done)
				{
					return false;
				}
				return true;
			}
			if (m_intersector == null)
			{
				return false;
			}
			return m_intersector.Next();
		}

		internal virtual int GetRedElement()
		{
			if (m_b_quad_tree)
			{
				if (!m_b_swap_elements)
				{
					return (!m_b_paths ? m_seg_iter.GetStartPointIndex() : m_path_index);
				}
				return m_quad_tree.GetElement(m_element_handle);
			}
			return m_intersector.GetRedElement(m_intersector.GetHandleA());
		}

		internal virtual int GetBlueElement()
		{
			if (m_b_quad_tree)
			{
				if (m_b_swap_elements)
				{
					return (!m_b_paths ? m_seg_iter.GetStartPointIndex() : m_path_index);
				}
				return m_quad_tree.GetElement(m_element_handle);
			}
			return m_intersector.GetBlueElement(m_intersector.GetHandleB());
		}

		internal virtual com.epl.geometry.Envelope2D GetRedEnvelope()
		{
			if (!m_b_paths)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			if (m_b_quad_tree)
			{
				if (!m_b_swap_elements)
				{
					return m_paths_query;
				}
				return m_quad_tree.GetElementExtent(m_element_handle);
			}
			return m_intersector.GetRedEnvelope(m_intersector.GetHandleA());
		}

		internal virtual com.epl.geometry.Envelope2D GetBlueEnvelope()
		{
			if (!m_b_paths)
			{
				throw com.epl.geometry.GeometryException.GeometryInternalError();
			}
			if (m_b_quad_tree)
			{
				if (m_b_swap_elements)
				{
					return m_paths_query;
				}
				return m_quad_tree.GetElementExtent(m_element_handle);
			}
			return m_intersector.GetBlueEnvelope(m_intersector.GetHandleB());
		}

		internal virtual bool NextPath_()
		{
			if (!m_b_paths)
			{
				if (!m_seg_iter.NextPath())
				{
					m_b_done = true;
					return false;
				}
				m_function = com.epl.geometry.PairwiseIntersectorImpl.State.nextSegment;
				return true;
			}
			if (--m_path_index == -1)
			{
				m_b_done = true;
				return false;
			}
			if (m_b_swap_elements)
			{
				m_multi_path_impl_b.QueryPathEnvelope2D(m_path_index, m_paths_query);
			}
			else
			{
				m_multi_path_impl_a.QueryPathEnvelope2D(m_path_index, m_paths_query);
			}
			m_qt_iter.ResetIterator(m_paths_query, m_tolerance);
			m_function = com.epl.geometry.PairwiseIntersectorImpl.State.iterate;
			return true;
		}

		internal virtual bool NextSegment_()
		{
			if (!m_seg_iter.HasNextSegment())
			{
				m_function = com.epl.geometry.PairwiseIntersectorImpl.State.nextPath;
				return true;
			}
			com.epl.geometry.Segment segment = m_seg_iter.NextSegment();
			m_qt_iter.ResetIterator(segment, m_tolerance);
			m_function = com.epl.geometry.PairwiseIntersectorImpl.State.iterate;
			return true;
		}

		internal virtual bool Iterate_()
		{
			m_element_handle = m_qt_iter.Next();
			if (m_element_handle == -1)
			{
				m_function = (!m_b_paths ? com.epl.geometry.PairwiseIntersectorImpl.State.nextSegment : com.epl.geometry.PairwiseIntersectorImpl.State.nextPath);
				return true;
			}
			return false;
		}
	}
}
