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
	internal class QuadTreeImpl
	{
		internal sealed class QuadTreeIteratorImpl
		{
			/// <summary>Resets the iterator to an starting state on the Quad_tree_impl.</summary>
			/// <remarks>
			/// Resets the iterator to an starting state on the Quad_tree_impl. If
			/// the input Geometry is a Line segment, then the query will be the
			/// segment. Otherwise the query will be the Envelope_2D bounding the
			/// Geometry. \param query The Geometry used for the query. \param
			/// tolerance The tolerance used for the intersection tests. \param
			/// tolerance The tolerance used for the intersection tests.
			/// </remarks>
			internal void ResetIterator(com.epl.geometry.Geometry query, double tolerance)
			{
				m_quads_stack.Resize(0);
				m_extents_stack.Clear();
				m_current_element_handle = -1;
				query.QueryLooseEnvelope2D(m_query_box);
				m_query_box.Inflate(tolerance, tolerance);
				if (m_query_box.IsIntersecting(m_quad_tree.m_extent))
				{
					int type = query.GetType().Value();
					m_b_linear = com.epl.geometry.Geometry.IsSegment(type);
					if (m_b_linear)
					{
						com.epl.geometry.Segment segment = (com.epl.geometry.Segment)query;
						m_query_start = segment.GetStartXY();
						m_query_end = segment.GetEndXY();
						m_tolerance = tolerance;
					}
					else
					{
						m_tolerance = com.epl.geometry.NumberUtils.NaN();
					}
					// we don't need it
					m_quads_stack.Add(m_quad_tree.m_root);
					m_extents_stack.Add(m_quad_tree.m_extent);
					m_next_element_handle = m_quad_tree.GetFirstElement_(m_quad_tree.m_root);
				}
				else
				{
					m_next_element_handle = -1;
				}
			}

			/// <summary>
			/// Resets the iterator to a starting state on the Quad_tree_impl using
			/// the input Envelope_2D as the query.
			/// </summary>
			/// <remarks>
			/// Resets the iterator to a starting state on the Quad_tree_impl using
			/// the input Envelope_2D as the query. \param query The Envelope_2D used
			/// for the query. \param tolerance The tolerance used for the
			/// intersection tests.
			/// </remarks>
			internal void ResetIterator(com.epl.geometry.Envelope2D query, double tolerance)
			{
				m_quads_stack.Resize(0);
				m_extents_stack.Clear();
				m_current_element_handle = -1;
				m_query_box.SetCoords(query);
				m_query_box.Inflate(tolerance, tolerance);
				m_tolerance = com.epl.geometry.NumberUtils.NaN();
				// we don't need it
				if (m_query_box.IsIntersecting(m_quad_tree.m_extent))
				{
					m_quads_stack.Add(m_quad_tree.m_root);
					m_extents_stack.Add(m_quad_tree.m_extent);
					m_next_element_handle = m_quad_tree.GetFirstElement_(m_quad_tree.m_root);
					m_b_linear = false;
				}
				else
				{
					m_next_element_handle = -1;
				}
			}

			/// <summary>Moves the iterator to the next int and returns the int.</summary>
			internal int Next()
			{
				// If the node stack is empty, then we've exhausted our search
				if (m_quads_stack.Size() == 0)
				{
					return -1;
				}
				m_current_element_handle = m_next_element_handle;
				com.epl.geometry.Point2D start = null;
				com.epl.geometry.Point2D end = null;
				com.epl.geometry.Envelope2D bounding_box;
				com.epl.geometry.Envelope2D extent_inf = null;
				com.epl.geometry.Envelope2D[] child_extents = null;
				if (m_b_linear)
				{
					// Should this memory be cached for reuse?
					start = new com.epl.geometry.Point2D();
					end = new com.epl.geometry.Point2D();
					extent_inf = new com.epl.geometry.Envelope2D();
				}
				bool b_found_hit = false;
				while (!b_found_hit)
				{
					while (m_current_element_handle != -1)
					{
						int current_box_handle = m_quad_tree.GetBoxHandle_(m_current_element_handle);
						bounding_box = m_quad_tree.GetBoundingBox_(current_box_handle);
						if (bounding_box.IsIntersecting(m_query_box))
						{
							if (m_b_linear)
							{
								start.SetCoords(m_query_start);
								end.SetCoords(m_query_end);
								extent_inf.SetCoords(bounding_box);
								extent_inf.Inflate(m_tolerance, m_tolerance);
								if (extent_inf.ClipLine(start, end) > 0)
								{
									b_found_hit = true;
									break;
								}
							}
							else
							{
								b_found_hit = true;
								break;
							}
						}
						// get next element_handle
						m_current_element_handle = m_quad_tree.GetNextElement_(m_current_element_handle);
					}
					// If m_current_element_handle equals -1, then we've exhausted
					// our search in the current quadtree node
					if (m_current_element_handle == -1)
					{
						// get the last node from the stack and add the children
						// whose extent intersects m_query_box
						int current_quad = m_quads_stack.GetLast();
						com.epl.geometry.Envelope2D current_extent = m_extents_stack[m_extents_stack.Count - 1];
						double x_mid = 0.5 * (current_extent.xmin + current_extent.xmax);
						double y_mid = 0.5 * (current_extent.ymin + current_extent.ymax);
						if (child_extents == null)
						{
							child_extents = new com.epl.geometry.Envelope2D[4];
							child_extents[0] = new com.epl.geometry.Envelope2D();
							child_extents[1] = new com.epl.geometry.Envelope2D();
							child_extents[2] = new com.epl.geometry.Envelope2D();
							child_extents[3] = new com.epl.geometry.Envelope2D();
						}
						SetChildExtents_(current_extent, child_extents);
						m_quads_stack.RemoveLast();
						m_extents_stack.RemoveAt(m_extents_stack.Count - 1);
						for (int quadrant = 0; quadrant < 4; quadrant++)
						{
							int child_handle = m_quad_tree.GetChild_(current_quad, quadrant);
							if (child_handle != -1 && m_quad_tree.GetSubTreeElementCount(child_handle) > 0)
							{
								if (child_extents[quadrant].IsIntersecting(m_query_box))
								{
									if (m_b_linear)
									{
										start.SetCoords(m_query_start);
										end.SetCoords(m_query_end);
										extent_inf.SetCoords(child_extents[quadrant]);
										extent_inf.Inflate(m_tolerance, m_tolerance);
										if (extent_inf.ClipLine(start, end) > 0)
										{
											com.epl.geometry.Envelope2D child_extent = new com.epl.geometry.Envelope2D();
											child_extent.SetCoords(child_extents[quadrant]);
											m_quads_stack.Add(child_handle);
											m_extents_stack.Add(child_extent);
										}
									}
									else
									{
										com.epl.geometry.Envelope2D child_extent = new com.epl.geometry.Envelope2D();
										child_extent.SetCoords(child_extents[quadrant]);
										m_quads_stack.Add(child_handle);
										m_extents_stack.Add(child_extent);
									}
								}
							}
						}
						System.Diagnostics.Debug.Assert((m_quads_stack.Size() <= 4 * (m_quad_tree.m_height - 1)));
						if (m_quads_stack.Size() == 0)
						{
							return -1;
						}
						m_current_element_handle = m_quad_tree.GetFirstElement_(m_quads_stack.Get(m_quads_stack.Size() - 1));
					}
				}
				// We did not exhaust our search in the current node, so we return
				// the element at m_current_element_handle in m_element_nodes
				m_next_element_handle = m_quad_tree.GetNextElement_(m_current_element_handle);
				return m_current_element_handle;
			}

			internal QuadTreeIteratorImpl(com.epl.geometry.QuadTreeImpl quad_tree_impl, com.epl.geometry.Geometry query, double tolerance)
			{
				// Creates an iterator on the input Quad_tree_impl. The query will be
				// the Envelope_2D bounding the input Geometry.
				m_quad_tree = quad_tree_impl;
				m_query_box = new com.epl.geometry.Envelope2D();
				m_quads_stack = new com.epl.geometry.AttributeStreamOfInt32(0);
				m_extents_stack = new System.Collections.Generic.List<com.epl.geometry.Envelope2D>(0);
				ResetIterator(query, tolerance);
			}

			internal QuadTreeIteratorImpl(com.epl.geometry.QuadTreeImpl quad_tree_impl, com.epl.geometry.Envelope2D query, double tolerance)
			{
				// Creates an iterator on the input Quad_tree_impl using the input
				// Envelope_2D as the query.
				m_quad_tree = quad_tree_impl;
				m_query_box = new com.epl.geometry.Envelope2D();
				m_quads_stack = new com.epl.geometry.AttributeStreamOfInt32(0);
				m_extents_stack = new System.Collections.Generic.List<com.epl.geometry.Envelope2D>(0);
				ResetIterator(query, tolerance);
			}

			internal QuadTreeIteratorImpl(com.epl.geometry.QuadTreeImpl quad_tree_impl)
			{
				// Creates an iterator on the input Quad_tree_impl.
				m_quad_tree = quad_tree_impl;
				m_query_box = new com.epl.geometry.Envelope2D();
				m_quads_stack = new com.epl.geometry.AttributeStreamOfInt32(0);
				m_extents_stack = new System.Collections.Generic.List<com.epl.geometry.Envelope2D>(0);
			}

			private bool m_b_linear;

			private com.epl.geometry.Point2D m_query_start;

			private com.epl.geometry.Point2D m_query_end;

			private com.epl.geometry.Envelope2D m_query_box;

			private double m_tolerance;

			private int m_current_element_handle;

			private int m_next_element_handle;

			private com.epl.geometry.QuadTreeImpl m_quad_tree;

			private com.epl.geometry.AttributeStreamOfInt32 m_quads_stack;

			private System.Collections.Generic.List<com.epl.geometry.Envelope2D> m_extents_stack;
			// this won't grow bigger
			// than 4 *
			// (m_quad_tree->m_height
			// - 1)
		}

		/// <summary>
		/// Creates a Quad_tree_impl with the root having the extent of the input
		/// Envelope_2D, and height of the input height, where the root starts at
		/// height 0.
		/// </summary>
		/// <remarks>
		/// Creates a Quad_tree_impl with the root having the extent of the input
		/// Envelope_2D, and height of the input height, where the root starts at
		/// height 0. Note that the height cannot be larger than 16 if on a 32 bit
		/// platform and 32 if on a 64 bit platform. \param extent The extent of the
		/// Quad_tree_impl. \param height The max height of the Quad_tree_impl.
		/// </remarks>
		internal QuadTreeImpl(com.epl.geometry.Envelope2D extent, int height)
		{
			m_quad_tree_nodes = new com.epl.geometry.StridedIndexTypeCollection(11);
			m_element_nodes = new com.epl.geometry.StridedIndexTypeCollection(5);
			m_boxes = new System.Collections.Generic.List<com.epl.geometry.Envelope2D>(0);
			m_free_boxes = new com.epl.geometry.AttributeStreamOfInt32(0);
			m_extent = new com.epl.geometry.Envelope2D();
			Reset_(extent, height);
		}

		/// <summary>Resets the Quad_tree_impl to the given extent and height.</summary>
		/// <remarks>
		/// Resets the Quad_tree_impl to the given extent and height. \param extent
		/// The extent of the Quad_tree_impl. \param height The max height of the
		/// Quad_tree_impl.
		/// </remarks>
		internal virtual void Reset(com.epl.geometry.Envelope2D extent, int height)
		{
			m_quad_tree_nodes.DeleteAll(false);
			m_element_nodes.DeleteAll(false);
			m_boxes.Clear();
			m_free_boxes.Clear(false);
			Reset_(extent, height);
		}

		/// <summary>Inserts the element and bounding_box into the Quad_tree_impl.</summary>
		/// <remarks>
		/// Inserts the element and bounding_box into the Quad_tree_impl. Note that
		/// this will invalidate any active iterator on the Quad_tree_impl. Returns
		/// an int corresponding to the element and bounding_box. \param element The
		/// element of the Geometry to be inserted. \param bounding_box The
		/// bounding_box of the Geometry to be inserted.
		/// </remarks>
		internal virtual int Insert(int element, com.epl.geometry.Envelope2D bounding_box)
		{
			return Insert_(element, bounding_box, 0, m_extent, m_root, false, -1);
		}

		/// <summary>
		/// Inserts the element and bounding_box into the Quad_tree_impl at the given
		/// quad_handle.
		/// </summary>
		/// <remarks>
		/// Inserts the element and bounding_box into the Quad_tree_impl at the given
		/// quad_handle. Note that this will invalidate any active iterator on the
		/// Quad_tree_impl. Returns an int corresponding to the element and
		/// bounding_box. \param element The element of the Geometry to be inserted.
		/// \param bounding_box The bounding_box of the Geometry to be inserted.
		/// \param hint_index A handle used as a hint where to place the element.
		/// This can be a handle obtained from a previous insertion and is useful on
		/// data having strong locality such as segments of a Polygon.
		/// </remarks>
		internal virtual int Insert(int element, com.epl.geometry.Envelope2D bounding_box, int hint_index)
		{
			int quad_handle;
			if (hint_index == -1)
			{
				quad_handle = m_root;
			}
			else
			{
				quad_handle = GetQuad_(hint_index);
			}
			int quad_height = GetHeight(quad_handle);
			com.epl.geometry.Envelope2D quad_extent = GetExtent(quad_handle);
			return Insert_(element, bounding_box, quad_height, quad_extent, quad_handle, false, -1);
		}

		/// <summary>Removes the element and bounding_box at the given element_handle.</summary>
		/// <remarks>
		/// Removes the element and bounding_box at the given element_handle. Note
		/// that this will invalidate any active iterator on the Quad_tree_impl.
		/// \param element_handle The handle corresponding to the element and
		/// bounding_box to be removed.
		/// </remarks>
		internal virtual void RemoveElement(int element_handle)
		{
			int quad_handle = GetQuad_(element_handle);
			int nextElementHandle = DisconnectElementHandle_(element_handle);
			FreeElementAndBoxNode_(element_handle);
			for (int q = quad_handle; q != -1; q = GetParent_(q))
			{
				SetSubTreeElementCount_(q, GetSubTreeElementCount_(q) - 1);
				System.Diagnostics.Debug.Assert((GetSubTreeElementCount_(q) >= 0));
			}
		}

		/// <summary>Returns the element at the given element_handle.</summary>
		/// <remarks>
		/// Returns the element at the given element_handle.
		/// \param element_handle
		/// The handle corresponding to the element to be retrieved.
		/// </remarks>
		internal virtual int GetElement(int element_handle)
		{
			return GetElement_(element_handle);
		}

		/// <summary>Returns a reference to the element extent at the given element_handle.</summary>
		/// <remarks>
		/// Returns a reference to the element extent at the given element_handle.
		/// \param element_handle
		/// The handle corresponding to the element to be retrieved.
		/// </remarks>
		internal virtual com.epl.geometry.Envelope2D GetElementExtent(int element_handle)
		{
			int box_handle = GetBoxHandle_(element_handle);
			return GetBoundingBox_(box_handle);
		}

		/// <summary>Returns the height of the quad at the given quad_handle.</summary>
		/// <remarks>
		/// Returns the height of the quad at the given quad_handle. \param
		/// quad_handle The handle corresponding to the quad.
		/// </remarks>
		internal virtual int GetHeight(int quad_handle)
		{
			return GetHeight_(quad_handle);
		}

		/// <summary>Returns the extent of the quad at the given quad_handle.</summary>
		/// <remarks>
		/// Returns the extent of the quad at the given quad_handle. \param
		/// quad_handle The handle corresponding to the quad.
		/// </remarks>
		internal virtual com.epl.geometry.Envelope2D GetExtent(int quad_handle)
		{
			com.epl.geometry.Envelope2D quad_extent = new com.epl.geometry.Envelope2D();
			quad_extent.SetCoords(m_extent);
			int height = GetHeight_(quad_handle);
			int morten_number = GetMortenNumber_(quad_handle);
			int mask = 3;
			for (int i = 0; i < 2 * height; i += 2)
			{
				int child = (int)(mask & (morten_number >> i));
				if (child == 0)
				{
					// northeast
					quad_extent.xmin = 0.5 * (quad_extent.xmin + quad_extent.xmax);
					quad_extent.ymin = 0.5 * (quad_extent.ymin + quad_extent.ymax);
				}
				else
				{
					if (child == 1)
					{
						// northwest
						quad_extent.xmax = 0.5 * (quad_extent.xmin + quad_extent.xmax);
						quad_extent.ymin = 0.5 * (quad_extent.ymin + quad_extent.ymax);
					}
					else
					{
						if (child == 2)
						{
							// southwest
							quad_extent.xmax = 0.5 * (quad_extent.xmin + quad_extent.xmax);
							quad_extent.ymax = 0.5 * (quad_extent.ymin + quad_extent.ymax);
						}
						else
						{
							// southeast
							quad_extent.xmin = 0.5 * (quad_extent.xmin + quad_extent.xmax);
							quad_extent.ymax = 0.5 * (quad_extent.ymin + quad_extent.ymax);
						}
					}
				}
			}
			return quad_extent;
		}

		/// <summary>Returns the int of the quad containing the given element_handle.</summary>
		/// <remarks>
		/// Returns the int of the quad containing the given element_handle. \param
		/// element_handle The handle corresponding to the element.
		/// </remarks>
		internal virtual int GetQuad(int element_handle)
		{
			return GetQuad_(element_handle);
		}

		/// <summary>Returns the number of elements in the Quad_tree_impl.</summary>
		internal virtual int GetElementCount()
		{
			return GetSubTreeElementCount_(m_root);
		}

		/// <summary>
		/// Returns the number of elements in the subtree rooted at the given
		/// quad_handle.
		/// </summary>
		/// <remarks>
		/// Returns the number of elements in the subtree rooted at the given
		/// quad_handle. \param quad_handle The handle corresponding to the quad.
		/// </remarks>
		internal virtual int GetSubTreeElementCount(int quad_handle)
		{
			return GetSubTreeElementCount_(quad_handle);
		}

		/// <summary>Gets an iterator on the Quad_tree_impl.</summary>
		/// <remarks>
		/// Gets an iterator on the Quad_tree_impl. The query will be the Envelope_2D
		/// that bounds the input Geometry. To reuse the existing iterator on the
		/// same Quad_tree_impl but with a new query, use the reset_iterator function
		/// on the Quad_tree_iterator_impl. \param query The Geometry used for the
		/// query. If the Geometry is a Line segment, then the query will be the
		/// segment. Otherwise the query will be the Envelope_2D bounding the
		/// Geometry. \param tolerance The tolerance used for the intersection tests.
		/// </remarks>
		internal virtual com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl GetIterator(com.epl.geometry.Geometry query, double tolerance)
		{
			return new com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl(this, query, tolerance);
		}

		/// <summary>
		/// Gets an iterator on the Quad_tree_impl using the input Envelope_2D as the
		/// query.
		/// </summary>
		/// <remarks>
		/// Gets an iterator on the Quad_tree_impl using the input Envelope_2D as the
		/// query. To reuse the existing iterator on the same Quad_tree_impl but with
		/// a new query, use the reset_iterator function on the
		/// Quad_tree_iterator_impl. \param query The Envelope_2D used for the query.
		/// \param tolerance The tolerance used for the intersection tests.
		/// </remarks>
		internal virtual com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl GetIterator(com.epl.geometry.Envelope2D query, double tolerance)
		{
			return new com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl(this, query, tolerance);
		}

		/// <summary>Gets an iterator on the Quad_tree.</summary>
		internal virtual com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl GetIterator()
		{
			return new com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl(this);
		}

		private void Reset_(com.epl.geometry.Envelope2D extent, int height)
		{
			// We need 2 * height bits for the morten number, which is of type
			// Index_type (more than enough).
			if (height < 0 || 2 * height > 8 * 4)
			{
				throw new System.ArgumentException("invalid height");
			}
			m_height = height;
			m_extent.SetCoords(extent);
			m_root = m_quad_tree_nodes.NewElement();
			SetSubTreeElementCount_(m_root, 0);
			SetLocalElementCount_(m_root, 0);
			SetMortenNumber_(m_root, 0);
			SetHeight_(m_root, 0);
		}

		private int Insert_(int element, com.epl.geometry.Envelope2D bounding_box, int height, com.epl.geometry.Envelope2D quad_extent, int quad_handle, bool b_flushing, int flushed_element_handle)
		{
			if (!quad_extent.Contains(bounding_box))
			{
				System.Diagnostics.Debug.Assert((!b_flushing));
				if (height == 0)
				{
					return -1;
				}
				return Insert_(element, bounding_box, 0, m_extent, m_root, b_flushing, flushed_element_handle);
			}
			if (!b_flushing)
			{
				for (int q = quad_handle; q != -1; q = GetParent_(q))
				{
					SetSubTreeElementCount_(q, GetSubTreeElementCount_(q) + 1);
				}
			}
			com.epl.geometry.Envelope2D current_extent = new com.epl.geometry.Envelope2D();
			current_extent.SetCoords(quad_extent);
			int current_quad_handle = quad_handle;
			com.epl.geometry.Envelope2D[] child_extents = new com.epl.geometry.Envelope2D[4];
			child_extents[0] = new com.epl.geometry.Envelope2D();
			child_extents[1] = new com.epl.geometry.Envelope2D();
			child_extents[2] = new com.epl.geometry.Envelope2D();
			child_extents[3] = new com.epl.geometry.Envelope2D();
			int current_height;
			for (current_height = height; current_height < m_height && CanPushDown_(current_quad_handle); current_height++)
			{
				SetChildExtents_(current_extent, child_extents);
				bool b_contains = false;
				for (int i = 0; i < 4; i++)
				{
					if (child_extents[i].Contains(bounding_box))
					{
						b_contains = true;
						int child_handle = GetChild_(current_quad_handle, i);
						if (child_handle == -1)
						{
							child_handle = CreateChild_(current_quad_handle, i);
						}
						SetSubTreeElementCount_(child_handle, GetSubTreeElementCount_(child_handle) + 1);
						current_quad_handle = child_handle;
						current_extent.SetCoords(child_extents[i]);
						break;
					}
				}
				if (!b_contains)
				{
					break;
				}
			}
			return InsertAtQuad_(element, bounding_box, current_height, current_extent, current_quad_handle, b_flushing, quad_handle, flushed_element_handle);
		}

		private int InsertAtQuad_(int element, com.epl.geometry.Envelope2D bounding_box, int current_height, com.epl.geometry.Envelope2D current_extent, int current_quad_handle, bool b_flushing, int quad_handle, int flushed_element_handle)
		{
			// If the bounding box is not contained in any of the current_node's
			// children, or if the current_height is m_height, then insert the
			// element and
			// bounding box into the current_node
			int head_element_handle = GetFirstElement_(current_quad_handle);
			int tail_element_handle = GetLastElement_(current_quad_handle);
			int element_handle = -1;
			if (b_flushing)
			{
				System.Diagnostics.Debug.Assert((flushed_element_handle != -1));
				if (current_quad_handle == quad_handle)
				{
					return flushed_element_handle;
				}
				DisconnectElementHandle_(flushed_element_handle);
				// Take it out of
				// the incoming
				// quad_handle,
				// and place in
				// current_quad_handle
				element_handle = flushed_element_handle;
			}
			else
			{
				element_handle = CreateElementAndBoxNode_();
				SetElement_(element_handle, element);
				// insert element at the new
				// tail of the list
				// (next_element_handle).
				SetBoundingBox_(GetBoxHandle_(element_handle), bounding_box);
			}
			// insert
			// bounding_box
			System.Diagnostics.Debug.Assert((!b_flushing || element_handle == flushed_element_handle));
			SetQuad_(element_handle, current_quad_handle);
			// set parent quad
			// (needed for removal
			// of element)
			// assign the prev pointer of the new tail to point at the old tail
			// (tail_element_handle)
			// assign the next pointer of the old tail to point at the new tail
			// (next_element_handle)
			if (tail_element_handle != -1)
			{
				SetPrevElement_(element_handle, tail_element_handle);
				SetNextElement_(tail_element_handle, element_handle);
			}
			else
			{
				System.Diagnostics.Debug.Assert((head_element_handle == -1));
				SetFirstElement_(current_quad_handle, element_handle);
			}
			// assign the new tail
			SetLastElement_(current_quad_handle, element_handle);
			SetLocalElementCount_(current_quad_handle, GetLocalElementCount_(current_quad_handle) + 1);
			if (CanFlush_(current_quad_handle))
			{
				Flush_(current_height, current_extent, current_quad_handle);
			}
			return element_handle;
		}

		private int DisconnectElementHandle_(int element_handle)
		{
			System.Diagnostics.Debug.Assert((element_handle != -1));
			int quad_handle = GetQuad_(element_handle);
			int head_element_handle = GetFirstElement_(quad_handle);
			int tail_element_handle = GetLastElement_(quad_handle);
			int prev_element_handle = GetPrevElement_(element_handle);
			int next_element_handle = GetNextElement_(element_handle);
			System.Diagnostics.Debug.Assert((head_element_handle != -1 && tail_element_handle != -1));
			if (head_element_handle == element_handle)
			{
				if (next_element_handle != -1)
				{
					SetPrevElement_(next_element_handle, -1);
				}
				else
				{
					System.Diagnostics.Debug.Assert((head_element_handle == tail_element_handle));
					System.Diagnostics.Debug.Assert((GetLocalElementCount_(quad_handle) == 1));
					SetLastElement_(quad_handle, -1);
				}
				SetFirstElement_(quad_handle, next_element_handle);
			}
			else
			{
				if (tail_element_handle == element_handle)
				{
					System.Diagnostics.Debug.Assert((prev_element_handle != -1));
					System.Diagnostics.Debug.Assert((GetLocalElementCount_(quad_handle) >= 2));
					SetNextElement_(prev_element_handle, -1);
					SetLastElement_(quad_handle, prev_element_handle);
				}
				else
				{
					System.Diagnostics.Debug.Assert((next_element_handle != -1 && prev_element_handle != -1));
					System.Diagnostics.Debug.Assert((GetLocalElementCount_(quad_handle) >= 3));
					SetPrevElement_(next_element_handle, prev_element_handle);
					SetNextElement_(prev_element_handle, next_element_handle);
				}
			}
			SetPrevElement_(element_handle, -1);
			SetNextElement_(element_handle, -1);
			SetLocalElementCount_(quad_handle, GetLocalElementCount_(quad_handle) - 1);
			System.Diagnostics.Debug.Assert((GetLocalElementCount_(quad_handle) >= 0));
			return next_element_handle;
		}

		private static void SetChildExtents_(com.epl.geometry.Envelope2D current_extent, com.epl.geometry.Envelope2D[] child_extents)
		{
			double x_mid = 0.5 * (current_extent.xmin + current_extent.xmax);
			double y_mid = 0.5 * (current_extent.ymin + current_extent.ymax);
			child_extents[0].SetCoords(x_mid, y_mid, current_extent.xmax, current_extent.ymax);
			// northeast
			child_extents[1].SetCoords(current_extent.xmin, y_mid, x_mid, current_extent.ymax);
			// northwest
			child_extents[2].SetCoords(current_extent.xmin, current_extent.ymin, x_mid, y_mid);
			// southwest
			child_extents[3].SetCoords(x_mid, current_extent.ymin, current_extent.xmax, y_mid);
		}

		// southeast
		private bool CanFlush_(int quad_handle)
		{
			return GetLocalElementCount_(quad_handle) == 8 && !HasChildren_(quad_handle);
		}

		private void Flush_(int height, com.epl.geometry.Envelope2D extent, int quad_handle)
		{
			int element;
			com.epl.geometry.Envelope2D bounding_box;
			System.Diagnostics.Debug.Assert((quad_handle != -1));
			int element_handle = GetFirstElement_(quad_handle);
			int next_handle;
			int box_handle;
			System.Diagnostics.Debug.Assert((element_handle != -1));
			do
			{
				box_handle = GetBoxHandle_(element_handle);
				element = m_element_nodes.GetField(element_handle, 0);
				bounding_box = GetBoundingBox_(box_handle);
				Insert_(element, bounding_box, height, extent, quad_handle, true, element_handle);
				next_handle = GetNextElement_(element_handle);
				element_handle = next_handle;
			}
			while (element_handle != -1);
		}

		internal virtual bool CanPushDown_(int quad_handle)
		{
			return GetLocalElementCount_(quad_handle) >= 8 || HasChildren_(quad_handle);
		}

		internal virtual bool HasChildren_(int parent)
		{
			return GetChild_(parent, 0) != -1 || GetChild_(parent, 1) != -1 || GetChild_(parent, 2) != -1 || GetChild_(parent, 3) != -1;
		}

		private int CreateChild_(int parent, int quadrant)
		{
			int child = m_quad_tree_nodes.NewElement();
			SetChild_(parent, quadrant, child);
			SetSubTreeElementCount_(child, 0);
			SetLocalElementCount_(child, 0);
			SetParent_(child, parent);
			SetHeight_(child, GetHeight_(parent) + 1);
			SetMortenNumber_(child, (quadrant << (2 * GetHeight_(parent))) | GetMortenNumber_(parent));
			return child;
		}

		private int CreateElementAndBoxNode_()
		{
			int element_handle = m_element_nodes.NewElement();
			int box_handle;
			if (m_free_boxes.Size() > 0)
			{
				box_handle = m_free_boxes.GetLast();
				m_free_boxes.RemoveLast();
			}
			else
			{
				box_handle = m_boxes.Count;
				m_boxes.Add(new com.epl.geometry.Envelope2D());
			}
			SetBoxHandle_(element_handle, box_handle);
			return element_handle;
		}

		private void FreeElementAndBoxNode_(int element_handle)
		{
			m_free_boxes.Add(GetBoxHandle_(element_handle));
			m_element_nodes.DeleteElement(element_handle);
		}

		private int GetChild_(int quad_handle, int quadrant)
		{
			return m_quad_tree_nodes.GetField(quad_handle, quadrant);
		}

		private void SetChild_(int parent, int quadrant, int child)
		{
			m_quad_tree_nodes.SetField(parent, quadrant, child);
		}

		private int GetFirstElement_(int quad_handle)
		{
			return m_quad_tree_nodes.GetField(quad_handle, 4);
		}

		private void SetFirstElement_(int quad_handle, int head)
		{
			m_quad_tree_nodes.SetField(quad_handle, 4, head);
		}

		private int GetLastElement_(int quad_handle)
		{
			return m_quad_tree_nodes.GetField(quad_handle, 5);
		}

		private void SetLastElement_(int quad_handle, int tail)
		{
			m_quad_tree_nodes.SetField(quad_handle, 5, tail);
		}

		private int GetMortenNumber_(int quad_handle)
		{
			return m_quad_tree_nodes.GetField(quad_handle, 6);
		}

		private void SetMortenNumber_(int quad_handle, int morten_number)
		{
			m_quad_tree_nodes.SetField(quad_handle, 6, morten_number);
		}

		private int GetLocalElementCount_(int quad_handle)
		{
			return m_quad_tree_nodes.GetField(quad_handle, 7);
		}

		private int GetSubTreeElementCount_(int quad_handle)
		{
			return m_quad_tree_nodes.GetField(quad_handle, 8);
		}

		private void SetLocalElementCount_(int quad_handle, int count)
		{
			m_quad_tree_nodes.SetField(quad_handle, 7, count);
		}

		private void SetSubTreeElementCount_(int quad_handle, int count)
		{
			m_quad_tree_nodes.SetField(quad_handle, 8, count);
		}

		private int GetParent_(int child)
		{
			return m_quad_tree_nodes.GetField(child, 9);
		}

		private void SetParent_(int child, int parent)
		{
			m_quad_tree_nodes.SetField(child, 9, parent);
		}

		private int GetHeight_(int quad_handle)
		{
			return (int)m_quad_tree_nodes.GetField(quad_handle, 10);
		}

		private void SetHeight_(int quad_handle, int height)
		{
			m_quad_tree_nodes.SetField(quad_handle, 10, height);
		}

		private int GetElement_(int element_handle)
		{
			return m_element_nodes.GetField(element_handle, 0);
		}

		private void SetElement_(int element_handle, int element)
		{
			m_element_nodes.SetField(element_handle, 0, element);
		}

		private int GetPrevElement_(int element_handle)
		{
			return m_element_nodes.GetField(element_handle, 1);
		}

		private int GetNextElement_(int element_handle)
		{
			return m_element_nodes.GetField(element_handle, 2);
		}

		private void SetPrevElement_(int element_handle, int prev_handle)
		{
			m_element_nodes.SetField(element_handle, 1, prev_handle);
		}

		private void SetNextElement_(int element_handle, int next_handle)
		{
			m_element_nodes.SetField(element_handle, 2, next_handle);
		}

		private int GetQuad_(int element_handle)
		{
			return m_element_nodes.GetField(element_handle, 3);
		}

		private void SetQuad_(int element_handle, int parent)
		{
			m_element_nodes.SetField(element_handle, 3, parent);
		}

		private int GetBoxHandle_(int element_handle)
		{
			return m_element_nodes.GetField(element_handle, 4);
		}

		private void SetBoxHandle_(int element_handle, int box_handle)
		{
			m_element_nodes.SetField(element_handle, 4, box_handle);
		}

		private com.epl.geometry.Envelope2D GetBoundingBox_(int box_handle)
		{
			return m_boxes[box_handle];
		}

		private void SetBoundingBox_(int box_handle, com.epl.geometry.Envelope2D bounding_box)
		{
			m_boxes[box_handle].SetCoords(bounding_box);
		}

		private int m_root;

		private com.epl.geometry.Envelope2D m_extent;

		private int m_height;

		private com.epl.geometry.StridedIndexTypeCollection m_quad_tree_nodes;

		private com.epl.geometry.StridedIndexTypeCollection m_element_nodes;

		private System.Collections.Generic.List<com.epl.geometry.Envelope2D> m_boxes;

		private com.epl.geometry.AttributeStreamOfInt32 m_free_boxes;
	}
}
