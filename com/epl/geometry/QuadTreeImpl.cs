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
				if (m_quad_tree.m_root != -1 && m_query_box.IsIntersecting(m_quad_tree.m_extent))
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
					m_next_element_handle = m_quad_tree.Get_first_element_(m_quad_tree.m_root);
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
				if (m_quad_tree.m_root != -1 && m_query_box.IsIntersecting(m_quad_tree.m_extent))
				{
					m_quads_stack.Add(m_quad_tree.m_root);
					m_extents_stack.Add(m_quad_tree.m_extent);
					m_next_element_handle = m_quad_tree.Get_first_element_(m_quad_tree.m_root);
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
					start = new com.epl.geometry.Point2D();
					end = new com.epl.geometry.Point2D();
					extent_inf = new com.epl.geometry.Envelope2D();
				}
				bool b_found_hit = false;
				while (!b_found_hit)
				{
					while (m_current_element_handle != -1)
					{
						int current_data_handle = m_quad_tree.Get_data_(m_current_element_handle);
						bounding_box = m_quad_tree.Get_bounding_box_value_(current_data_handle);
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
						m_current_element_handle = m_quad_tree.Get_next_element_(m_current_element_handle);
					}
					// If m_current_element_handle equals -1, then we've exhausted our search in the current quadtree node
					if (m_current_element_handle == -1)
					{
						// get the last node from the stack and add the children whose extent intersects m_query_box
						int current_quad = m_quads_stack.GetLast();
						com.epl.geometry.Envelope2D current_extent = m_extents_stack[m_extents_stack.Count - 1];
						if (child_extents == null)
						{
							child_extents = new com.epl.geometry.Envelope2D[4];
							child_extents[0] = new com.epl.geometry.Envelope2D();
							child_extents[1] = new com.epl.geometry.Envelope2D();
							child_extents[2] = new com.epl.geometry.Envelope2D();
							child_extents[3] = new com.epl.geometry.Envelope2D();
						}
						Set_child_extents_(current_extent, child_extents);
						m_quads_stack.RemoveLast();
						m_extents_stack.RemoveAt(m_extents_stack.Count - 1);
						for (int quadrant = 0; quadrant < 4; quadrant++)
						{
							int child_handle = m_quad_tree.Get_child_(current_quad, quadrant);
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
						m_current_element_handle = m_quad_tree.Get_first_element_(m_quads_stack.Get(m_quads_stack.Size() - 1));
					}
				}
				// We did not exhaust our search in the current node, so we return
				// the element at m_current_element_handle in m_element_nodes
				m_next_element_handle = m_quad_tree.Get_next_element_(m_current_element_handle);
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

			internal com.epl.geometry.QuadTreeImpl m_quad_tree;

			private com.epl.geometry.AttributeStreamOfInt32 m_quads_stack;

			private System.Collections.Generic.List<com.epl.geometry.Envelope2D> m_extents_stack;
			// this won't grow bigger than 4 * (m_quad_tree->m_height - 1)
		}

		internal sealed class QuadTreeSortedIteratorImpl
		{
			/// <summary>Resets the iterator to a starting state on the Quad_tree_impl.</summary>
			/// <remarks>
			/// Resets the iterator to a starting state on the Quad_tree_impl. If the input Geometry is a Line segment, then the query will be the segment. Otherwise the query will be the Envelope_2D bounding the Geometry.
			/// \param query The Geometry used for the query.
			/// \param tolerance The tolerance used for the intersection tests.
			/// \param tolerance The tolerance used for the intersection tests.
			/// </remarks>
			internal void ResetIterator(com.epl.geometry.Geometry query, double tolerance)
			{
				m_quad_tree_iterator_impl.ResetIterator(query, tolerance);
				m_sorted_handles.Resize(0);
				m_index = -1;
			}

			/// <summary>Resets the iterator to a starting state on the Quad_tree_impl using the input Envelope_2D as the query.</summary>
			/// <remarks>
			/// Resets the iterator to a starting state on the Quad_tree_impl using the input Envelope_2D as the query.
			/// \param query The Envelope_2D used for the query.
			/// \param tolerance The tolerance used for the intersection tests.
			/// </remarks>
			internal void ResetIterator(com.epl.geometry.Envelope2D query, double tolerance)
			{
				m_quad_tree_iterator_impl.ResetIterator(query, tolerance);
				m_sorted_handles.Resize(0);
				m_index = -1;
			}

			/// <summary>Moves the iterator to the next Element_handle and returns the Element_handle.</summary>
			internal int Next()
			{
				if (m_index == -1)
				{
					int element_handle = -1;
					while ((element_handle = m_quad_tree_iterator_impl.Next()) != -1)
					{
						m_sorted_handles.Add(element_handle);
					}
					m_bucket_sort.Sort(m_sorted_handles, 0, m_sorted_handles.Size(), new com.epl.geometry.QuadTreeImpl.QuadTreeSortedIteratorImpl.Sorter(this, m_quad_tree_iterator_impl.m_quad_tree));
				}
				if (m_index == m_sorted_handles.Size() - 1)
				{
					return -1;
				}
				m_index++;
				return m_sorted_handles.Get(m_index);
			}

			internal QuadTreeSortedIteratorImpl(com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl quad_tree_iterator_impl)
			{
				//Creates a sorted iterator on the input Quad_tree_iterator_impl
				m_bucket_sort = new com.epl.geometry.BucketSort();
				m_sorted_handles = new com.epl.geometry.AttributeStreamOfInt32(0);
				m_quad_tree_iterator_impl = quad_tree_iterator_impl;
				m_index = -1;
			}

			private class Sorter : com.epl.geometry.ClassicSort
			{
				public Sorter(QuadTreeSortedIteratorImpl _enclosing, com.epl.geometry.QuadTreeImpl quad_tree)
				{
					this._enclosing = _enclosing;
					this.m_quad_tree = quad_tree;
				}

				public override void UserSort(int begin, int end, com.epl.geometry.AttributeStreamOfInt32 indices)
				{
					indices.Sort(begin, end);
				}

				public override double GetValue(int e)
				{
					return this.m_quad_tree.GetElement(e);
				}

				internal com.epl.geometry.QuadTreeImpl m_quad_tree;

				private readonly QuadTreeSortedIteratorImpl _enclosing;
			}

			private com.epl.geometry.BucketSort m_bucket_sort;

			private com.epl.geometry.AttributeStreamOfInt32 m_sorted_handles;

			private com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl m_quad_tree_iterator_impl;

			internal int m_index;
		}

		/// <summary>Creates a Quad_tree_impl with the root having the extent of the input Envelope_2D, and height of the input height, where the root starts at height 0.</summary>
		/// <remarks>
		/// Creates a Quad_tree_impl with the root having the extent of the input Envelope_2D, and height of the input height, where the root starts at height 0.
		/// \param extent The extent of the Quad_tree_impl.
		/// \param height The max height of the Quad_tree_impl.
		/// </remarks>
		internal QuadTreeImpl(com.epl.geometry.Envelope2D extent, int height)
		{
			m_quad_tree_nodes = new com.epl.geometry.StridedIndexTypeCollection(10);
			m_element_nodes = new com.epl.geometry.StridedIndexTypeCollection(4);
			m_data = new System.Collections.Generic.List<com.epl.geometry.QuadTreeImpl.Data>(0);
			m_free_data = new com.epl.geometry.AttributeStreamOfInt32(0);
			m_b_store_duplicates = false;
			m_extent = new com.epl.geometry.Envelope2D();
			m_data_extent = new com.epl.geometry.Envelope2D();
			Reset_(extent, height);
		}

		/// <summary>Creates a Quad_tree_impl with the root having the extent of the input Envelope_2D, and height of the input height, where the root starts at height 0.</summary>
		/// <remarks>
		/// Creates a Quad_tree_impl with the root having the extent of the input Envelope_2D, and height of the input height, where the root starts at height 0.
		/// \param extent The extent of the Quad_tree_impl.
		/// \param height The max height of the Quad_tree_impl.
		/// \param b_store_duplicates Put true to place elements deeper into the quad tree at intesecting quads, duplicates will be stored. Put false to only place elements into quads that can contain it.
		/// </remarks>
		internal QuadTreeImpl(com.epl.geometry.Envelope2D extent, int height, bool b_store_duplicates)
		{
			m_quad_tree_nodes = (b_store_duplicates ? new com.epl.geometry.StridedIndexTypeCollection(11) : new com.epl.geometry.StridedIndexTypeCollection(10));
			m_element_nodes = new com.epl.geometry.StridedIndexTypeCollection(4);
			m_data = new System.Collections.Generic.List<com.epl.geometry.QuadTreeImpl.Data>(0);
			m_free_data = new com.epl.geometry.AttributeStreamOfInt32(0);
			m_b_store_duplicates = b_store_duplicates;
			m_extent = new com.epl.geometry.Envelope2D();
			m_data_extent = new com.epl.geometry.Envelope2D();
			Reset_(extent, height);
		}

		/// <summary>Resets the Quad_tree_impl to the given extent and height.</summary>
		/// <remarks>
		/// Resets the Quad_tree_impl to the given extent and height.
		/// \param extent The extent of the Quad_tree_impl.
		/// \param height The max height of the Quad_tree_impl.
		/// </remarks>
		internal virtual void Reset(com.epl.geometry.Envelope2D extent, int height)
		{
			m_quad_tree_nodes.DeleteAll(false);
			m_element_nodes.DeleteAll(false);
			m_data.Clear();
			m_free_data.Clear(false);
			Reset_(extent, height);
		}

		/// <summary>Inserts the element and bounding_box into the Quad_tree_impl.</summary>
		/// <remarks>
		/// Inserts the element and bounding_box into the Quad_tree_impl.
		/// Note that this will invalidate any active iterator on the Quad_tree_impl.
		/// Returns an Element_handle corresponding to the element and bounding_box.
		/// \param element The element of the Geometry to be inserted.
		/// \param bounding_box The bounding_box of the Geometry to be inserted.
		/// </remarks>
		internal virtual int Insert(int element, com.epl.geometry.Envelope2D bounding_box)
		{
			if (m_root == -1)
			{
				Create_root_();
			}
			if (m_b_store_duplicates)
			{
				int success = Insert_duplicates_(element, bounding_box, 0, m_extent, m_root, false, -1);
				if (success != -1)
				{
					if (m_data_extent.IsEmpty())
					{
						m_data_extent.SetCoords(bounding_box);
					}
					else
					{
						m_data_extent.Merge(bounding_box);
					}
				}
				return success;
			}
			int element_handle = Insert_(element, bounding_box, 0, m_extent, m_root, false, -1);
			if (element_handle != -1)
			{
				if (m_data_extent.IsEmpty())
				{
					m_data_extent.SetCoords(bounding_box);
				}
				else
				{
					m_data_extent.Merge(bounding_box);
				}
			}
			return element_handle;
		}

		/// <summary>Inserts the element and bounding_box into the Quad_tree_impl at the given quad_handle.</summary>
		/// <remarks>
		/// Inserts the element and bounding_box into the Quad_tree_impl at the given quad_handle.
		/// Note that this will invalidate any active iterator on the Quad_tree_impl.
		/// Returns an Element_handle corresponding to the element and bounding_box.
		/// \param element The element of the Geometry to be inserted.
		/// \param bounding_box The bounding_box of the Geometry to be inserted.
		/// \param hint_index A handle used as a hint where to place the element. This can be a handle obtained from a previous insertion and is useful on data having strong locality such as segments of a Polygon.
		/// </remarks>
		internal virtual int Insert(int element, com.epl.geometry.Envelope2D bounding_box, int hint_index)
		{
			if (m_root == -1)
			{
				Create_root_();
			}
			if (m_b_store_duplicates)
			{
				int success = Insert_duplicates_(element, bounding_box, 0, m_extent, m_root, false, -1);
				if (success != -1)
				{
					if (m_data_extent.IsEmpty())
					{
						m_data_extent.SetCoords(bounding_box);
					}
					else
					{
						m_data_extent.Merge(bounding_box);
					}
				}
				return success;
			}
			int quad_handle;
			if (hint_index == -1)
			{
				quad_handle = m_root;
			}
			else
			{
				quad_handle = Get_quad_(hint_index);
			}
			int quad_height = GetHeight(quad_handle);
			com.epl.geometry.Envelope2D quad_extent = GetExtent(quad_handle);
			int element_handle = Insert_(element, bounding_box, quad_height, quad_extent, quad_handle, false, -1);
			if (element_handle != -1)
			{
				if (m_data_extent.IsEmpty())
				{
					m_data_extent.SetCoords(bounding_box);
				}
				else
				{
					m_data_extent.Merge(bounding_box);
				}
			}
			return element_handle;
		}

		/// <summary>Removes the element and bounding_box at the given element_handle.</summary>
		/// <remarks>
		/// Removes the element and bounding_box at the given element_handle.
		/// Note that this will invalidate any active iterator on the Quad_tree_impl.
		/// \param element_handle The handle corresponding to the element and bounding_box to be removed.
		/// </remarks>
		internal virtual void RemoveElement(int element_handle)
		{
			if (m_b_store_duplicates)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			int quad_handle = Get_quad_(element_handle);
			Disconnect_element_handle_(element_handle);
			Free_element_and_box_node_(element_handle);
			int q = quad_handle;
			while (q != -1)
			{
				Set_sub_tree_element_count_(q, Get_sub_tree_element_count_(q) - 1);
				int parent = Get_parent_(q);
				if (Get_sub_tree_element_count_(q) == 0)
				{
					System.Diagnostics.Debug.Assert((Get_local_element_count_(q) == 0));
					if (q != m_root)
					{
						int quadrant = Get_quadrant_(q);
						m_quad_tree_nodes.DeleteElement(q);
						Set_child_(parent, quadrant, -1);
					}
				}
				q = parent;
			}
		}

		/// <summary>Returns the element at the given element_handle.</summary>
		/// <remarks>
		/// Returns the element at the given element_handle.
		/// \param element_handle The handle corresponding to the element to be retrieved.
		/// </remarks>
		internal virtual int GetElement(int element_handle)
		{
			return Get_element_value_(Get_data_(element_handle));
		}

		/// <summary>Returns the ith unique element.</summary>
		/// <remarks>
		/// Returns the ith unique element.
		/// \param i The index corresponding to the ith unique element.
		/// </remarks>
		internal virtual int GetElementAtIndex(int i)
		{
			return m_data[i].element;
		}

		/// <summary>Returns the element extent at the given element_handle.</summary>
		/// <remarks>
		/// Returns the element extent at the given element_handle.
		/// \param element_handle The handle corresponding to the element extent to be retrieved.
		/// </remarks>
		internal virtual com.epl.geometry.Envelope2D GetElementExtent(int element_handle)
		{
			int data_handle = Get_data_(element_handle);
			return Get_bounding_box_value_(data_handle);
		}

		/// <summary>Returns the extent of the ith unique element.</summary>
		/// <remarks>
		/// Returns the extent of the ith unique element.
		/// \param i The index corresponding to the ith unique element.
		/// </remarks>
		internal virtual com.epl.geometry.Envelope2D GetElementExtentAtIndex(int i)
		{
			return m_data[i].box;
		}

		/// <summary>Returns the extent of all elements in the quad tree.</summary>
		internal virtual com.epl.geometry.Envelope2D GetDataExtent()
		{
			return m_data_extent;
		}

		/// <summary>Returns the extent of the quad tree.</summary>
		internal virtual com.epl.geometry.Envelope2D GetQuadTreeExtent()
		{
			return m_extent;
		}

		/// <summary>Returns the height of the quad at the given quad_handle.</summary>
		/// <remarks>
		/// Returns the height of the quad at the given quad_handle.
		/// \param quad_handle The handle corresponding to the quad.
		/// </remarks>
		internal virtual int GetHeight(int quad_handle)
		{
			return Get_height_(quad_handle);
		}

		internal virtual int GetMaxHeight()
		{
			return m_height;
		}

		/// <summary>Returns the extent of the quad at the given quad_handle.</summary>
		/// <remarks>
		/// Returns the extent of the quad at the given quad_handle.
		/// \param quad_handle The handle corresponding to the quad.
		/// </remarks>
		internal virtual com.epl.geometry.Envelope2D GetExtent(int quad_handle)
		{
			com.epl.geometry.Envelope2D quad_extent = new com.epl.geometry.Envelope2D();
			quad_extent.SetCoords(m_extent);
			if (quad_handle == m_root)
			{
				return quad_extent;
			}
			com.epl.geometry.AttributeStreamOfInt32 quadrants = new com.epl.geometry.AttributeStreamOfInt32(0);
			int q = quad_handle;
			do
			{
				quadrants.Add(Get_quadrant_(q));
				q = Get_parent_(q);
			}
			while (q != m_root);
			int sz = quadrants.Size();
			System.Diagnostics.Debug.Assert((sz == GetHeight(quad_handle)));
			for (int i = 0; i < sz; i++)
			{
				int child = quadrants.GetLast();
				quadrants.RemoveLast();
				if (child == 0)
				{
					//northeast
					quad_extent.xmin = 0.5 * (quad_extent.xmin + quad_extent.xmax);
					quad_extent.ymin = 0.5 * (quad_extent.ymin + quad_extent.ymax);
				}
				else
				{
					if (child == 1)
					{
						//northwest
						quad_extent.xmax = 0.5 * (quad_extent.xmin + quad_extent.xmax);
						quad_extent.ymin = 0.5 * (quad_extent.ymin + quad_extent.ymax);
					}
					else
					{
						if (child == 2)
						{
							//southwest
							quad_extent.xmax = 0.5 * (quad_extent.xmin + quad_extent.xmax);
							quad_extent.ymax = 0.5 * (quad_extent.ymin + quad_extent.ymax);
						}
						else
						{
							//southeast
							quad_extent.xmin = 0.5 * (quad_extent.xmin + quad_extent.xmax);
							quad_extent.ymax = 0.5 * (quad_extent.ymin + quad_extent.ymax);
						}
					}
				}
			}
			return quad_extent;
		}

		/// <summary>Returns the Quad_handle of the quad containing the given element_handle.</summary>
		/// <remarks>
		/// Returns the Quad_handle of the quad containing the given element_handle.
		/// \param element_handle The handle corresponding to the element.
		/// </remarks>
		internal virtual int GetQuad(int element_handle)
		{
			return Get_quad_(element_handle);
		}

		/// <summary>Returns the number of elements in the Quad_tree_impl.</summary>
		internal virtual int GetElementCount()
		{
			if (m_root == -1)
			{
				return 0;
			}
			System.Diagnostics.Debug.Assert((Get_sub_tree_element_count_(m_root) == m_data.Count));
			return Get_sub_tree_element_count_(m_root);
		}

		/// <summary>Returns the number of elements in the subtree rooted at the given quad_handle.</summary>
		/// <remarks>
		/// Returns the number of elements in the subtree rooted at the given quad_handle.
		/// \param quad_handle The handle corresponding to the quad.
		/// </remarks>
		internal virtual int GetSubTreeElementCount(int quad_handle)
		{
			return Get_sub_tree_element_count_(quad_handle);
		}

		/// <summary>Returns the number of elements contained in the subtree rooted at the given quad_handle.</summary>
		/// <remarks>
		/// Returns the number of elements contained in the subtree rooted at the given quad_handle.
		/// \param quad_handle The handle corresponding to the quad.
		/// </remarks>
		internal virtual int GetContainedSubTreeElementCount(int quad_handle)
		{
			if (!m_b_store_duplicates)
			{
				return Get_sub_tree_element_count_(quad_handle);
			}
			return Get_contained_sub_tree_element_count_(quad_handle);
		}

		/// <summary>Returns the number of elements in the quad tree that intersect the qiven query.</summary>
		/// <remarks>
		/// Returns the number of elements in the quad tree that intersect the qiven query. Some elements may be duplicated if the quad tree stores duplicates.
		/// \param query The Envelope_2D used for the query.
		/// \param tolerance The tolerance used for the intersection tests.
		/// \param max_count If the intersection count becomes greater than or equal to the max_count, then max_count is returned.
		/// </remarks>
		internal virtual int GetIntersectionCount(com.epl.geometry.Envelope2D query, double tolerance, int max_count)
		{
			if (m_root == -1)
			{
				return 0;
			}
			com.epl.geometry.Envelope2D query_inflated = new com.epl.geometry.Envelope2D();
			query_inflated.SetCoords(query);
			query_inflated.Inflate(tolerance, tolerance);
			com.epl.geometry.AttributeStreamOfInt32 quads_stack = new com.epl.geometry.AttributeStreamOfInt32(0);
			System.Collections.Generic.List<com.epl.geometry.Envelope2D> extents_stack = new System.Collections.Generic.List<com.epl.geometry.Envelope2D>(0);
			quads_stack.Add(m_root);
			extents_stack.Add(new com.epl.geometry.Envelope2D(m_extent.xmin, m_extent.ymin, m_extent.xmax, m_extent.ymax));
			com.epl.geometry.Envelope2D[] child_extents = new com.epl.geometry.Envelope2D[4];
			child_extents[0] = new com.epl.geometry.Envelope2D();
			child_extents[1] = new com.epl.geometry.Envelope2D();
			child_extents[2] = new com.epl.geometry.Envelope2D();
			child_extents[3] = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D current_extent = new com.epl.geometry.Envelope2D();
			int intersection_count = 0;
			while (quads_stack.Size() > 0)
			{
				bool b_subdivide = false;
				int current_quad_handle = quads_stack.GetLast();
				current_extent.SetCoords(extents_stack[extents_stack.Count - 1]);
				quads_stack.RemoveLast();
				extents_stack.RemoveAt(extents_stack.Count - 1);
				if (query_inflated.Contains(current_extent))
				{
					intersection_count += GetSubTreeElementCount(current_quad_handle);
					if (max_count > 0 && intersection_count >= max_count)
					{
						return max_count;
					}
				}
				else
				{
					if (query_inflated.IsIntersecting(current_extent))
					{
						for (int element_handle = Get_first_element_(current_quad_handle); element_handle != -1; element_handle = Get_next_element_(element_handle))
						{
							int data_handle = Get_data_(element_handle);
							com.epl.geometry.Envelope2D env = Get_bounding_box_value_(data_handle);
							if (env.IsIntersecting(query_inflated))
							{
								intersection_count++;
								if (max_count > 0 && intersection_count >= max_count)
								{
									return max_count;
								}
							}
						}
						b_subdivide = GetHeight(current_quad_handle) + 1 <= m_height;
					}
				}
				if (b_subdivide)
				{
					Set_child_extents_(current_extent, child_extents);
					for (int i = 0; i < 4; i++)
					{
						int child_handle = Get_child_(current_quad_handle, i);
						if (child_handle != -1 && GetSubTreeElementCount(child_handle) > 0)
						{
							bool b_is_intersecting = query_inflated.IsIntersecting(child_extents[i]);
							if (b_is_intersecting)
							{
								quads_stack.Add(child_handle);
								extents_stack.Add(new com.epl.geometry.Envelope2D(child_extents[i].xmin, child_extents[i].ymin, child_extents[i].xmax, child_extents[i].ymax));
							}
						}
					}
				}
			}
			return intersection_count;
		}

		/// <summary>Returns true if the quad tree has data intersecting the given query.</summary>
		/// <remarks>
		/// Returns true if the quad tree has data intersecting the given query.
		/// \param query The Envelope_2D used for the query.
		/// \param tolerance The tolerance used for the intersection tests.
		/// </remarks>
		internal virtual bool HasData(com.epl.geometry.Envelope2D query, double tolerance)
		{
			int count = GetIntersectionCount(query, tolerance, 1);
			return count >= 1;
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

		/// <summary>Gets a sorted iterator on the Quad_tree_impl.</summary>
		/// <remarks>
		/// Gets a sorted iterator on the Quad_tree_impl. The Element_handles will be returned in increasing order of their corresponding Element_types.
		/// The query will be the Envelope_2D that bounds the input Geometry.
		/// To reuse the existing iterator on the same Quad_tree_impl but with a new query, use the reset_iterator function on the Quad_tree_sorted_iterator_impl.
		/// \param query The Geometry used for the query. If the Geometry is a Line segment, then the query will be the segment. Otherwise the query will be the Envelope_2D bounding the Geometry.
		/// \param tolerance The tolerance used for the intersection tests.
		/// </remarks>
		internal virtual com.epl.geometry.QuadTreeImpl.QuadTreeSortedIteratorImpl GetSortedIterator(com.epl.geometry.Geometry query, double tolerance)
		{
			return new com.epl.geometry.QuadTreeImpl.QuadTreeSortedIteratorImpl(GetIterator(query, tolerance));
		}

		/// <summary>Gets a sorted iterator on the Quad_tree_impl using the input Envelope_2D as the query.</summary>
		/// <remarks>
		/// Gets a sorted iterator on the Quad_tree_impl using the input Envelope_2D as the query. The Element_handles will be returned in increasing order of their corresponding Element_types.
		/// To reuse the existing iterator on the same Quad_tree_impl but with a new query, use the reset_iterator function on the Quad_tree_iterator_impl.
		/// \param query The Envelope_2D used for the query.
		/// \param tolerance The tolerance used for the intersection tests.
		/// </remarks>
		internal virtual com.epl.geometry.QuadTreeImpl.QuadTreeSortedIteratorImpl GetSortedIterator(com.epl.geometry.Envelope2D query, double tolerance)
		{
			return new com.epl.geometry.QuadTreeImpl.QuadTreeSortedIteratorImpl(GetIterator(query, tolerance));
		}

		/// <summary>Gets a sorted iterator on the Quad_tree.</summary>
		/// <remarks>Gets a sorted iterator on the Quad_tree. The Element_handles will be returned in increasing order of their corresponding Element_types</remarks>
		internal virtual com.epl.geometry.QuadTreeImpl.QuadTreeSortedIteratorImpl GetSortedIterator()
		{
			return new com.epl.geometry.QuadTreeImpl.QuadTreeSortedIteratorImpl(GetIterator());
		}

		private void Reset_(com.epl.geometry.Envelope2D extent, int height)
		{
			if (height < 0 || height > 127)
			{
				throw new System.ArgumentException("invalid height");
			}
			m_height = height;
			m_extent.SetCoords(extent);
			m_root = m_quad_tree_nodes.NewElement();
			m_data_extent.SetEmpty();
			m_root = -1;
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
				for (int q = quad_handle; q != -1; q = Get_parent_(q))
				{
					Set_sub_tree_element_count_(q, Get_sub_tree_element_count_(q) + 1);
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
			for (current_height = height; current_height < m_height && Can_push_down_(current_quad_handle); current_height++)
			{
				Set_child_extents_(current_extent, child_extents);
				bool b_contains = false;
				for (int i = 0; i < 4; i++)
				{
					if (child_extents[i].Contains(bounding_box))
					{
						b_contains = true;
						int child_handle = Get_child_(current_quad_handle, i);
						if (child_handle == -1)
						{
							child_handle = Create_child_(current_quad_handle, i);
						}
						Set_sub_tree_element_count_(child_handle, Get_sub_tree_element_count_(child_handle) + 1);
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
			return Insert_at_quad_(element, bounding_box, current_height, current_extent, current_quad_handle, b_flushing, quad_handle, flushed_element_handle, -1);
		}

		private int Insert_duplicates_(int element, com.epl.geometry.Envelope2D bounding_box, int height, com.epl.geometry.Envelope2D quad_extent, int quad_handle, bool b_flushing, int flushed_element_handle)
		{
			System.Diagnostics.Debug.Assert((b_flushing || m_root == quad_handle));
			if (!b_flushing)
			{
				// If b_flushing is true, then the sub tree element counts are already accounted for since the element already lies in the current incoming quad
				if (!quad_extent.Contains(bounding_box))
				{
					return -1;
				}
				Set_sub_tree_element_count_(quad_handle, Get_sub_tree_element_count_(quad_handle) + 1);
				Set_contained_sub_tree_element_count_(quad_handle, Get_contained_sub_tree_element_count_(quad_handle) + 1);
			}
			double bounding_box_max_dim = System.Math.Max(bounding_box.GetWidth(), bounding_box.GetHeight());
			int element_handle = -1;
			com.epl.geometry.AttributeStreamOfInt32 quads_stack = new com.epl.geometry.AttributeStreamOfInt32(0);
			System.Collections.Generic.List<com.epl.geometry.Envelope2D> extents_stack = new System.Collections.Generic.List<com.epl.geometry.Envelope2D>(0);
			com.epl.geometry.AttributeStreamOfInt32 heights_stack = new com.epl.geometry.AttributeStreamOfInt32(0);
			quads_stack.Add(quad_handle);
			extents_stack.Add(new com.epl.geometry.Envelope2D(quad_extent.xmin, quad_extent.ymin, quad_extent.xmax, quad_extent.ymax));
			heights_stack.Add(height);
			com.epl.geometry.Envelope2D[] child_extents = new com.epl.geometry.Envelope2D[4];
			child_extents[0] = new com.epl.geometry.Envelope2D();
			child_extents[1] = new com.epl.geometry.Envelope2D();
			child_extents[2] = new com.epl.geometry.Envelope2D();
			child_extents[3] = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D current_extent = new com.epl.geometry.Envelope2D();
			while (quads_stack.Size() > 0)
			{
				bool b_subdivide = false;
				int current_quad_handle = quads_stack.GetLast();
				current_extent.SetCoords(extents_stack[extents_stack.Count - 1]);
				int current_height = heights_stack.GetLast();
				quads_stack.RemoveLast();
				extents_stack.RemoveAt(extents_stack.Count - 1);
				heights_stack.RemoveLast();
				if (current_height + 1 < m_height && Can_push_down_(current_quad_handle))
				{
					double current_extent_max_dim = System.Math.Max(current_extent.GetWidth(), current_extent.GetHeight());
					if (bounding_box_max_dim <= current_extent_max_dim / 2.0)
					{
						b_subdivide = true;
					}
				}
				if (b_subdivide)
				{
					Set_child_extents_(current_extent, child_extents);
					bool b_contains = false;
					for (int i = 0; i < 4; i++)
					{
						b_contains = child_extents[i].Contains(bounding_box);
						if (b_contains)
						{
							int child_handle = Get_child_(current_quad_handle, i);
							if (child_handle == -1)
							{
								child_handle = Create_child_(current_quad_handle, i);
							}
							quads_stack.Add(child_handle);
							extents_stack.Add(new com.epl.geometry.Envelope2D(child_extents[i].xmin, child_extents[i].ymin, child_extents[i].xmax, child_extents[i].ymax));
							heights_stack.Add(current_height + 1);
							Set_sub_tree_element_count_(child_handle, Get_sub_tree_element_count_(child_handle) + 1);
							Set_contained_sub_tree_element_count_(child_handle, Get_contained_sub_tree_element_count_(child_handle) + 1);
							break;
						}
					}
					if (!b_contains)
					{
						for (int i_1 = 0; i_1 < 4; i_1++)
						{
							bool b_intersects = child_extents[i_1].IsIntersecting(bounding_box);
							if (b_intersects)
							{
								int child_handle = Get_child_(current_quad_handle, i_1);
								if (child_handle == -1)
								{
									child_handle = Create_child_(current_quad_handle, i_1);
								}
								quads_stack.Add(child_handle);
								extents_stack.Add(new com.epl.geometry.Envelope2D(child_extents[i_1].xmin, child_extents[i_1].ymin, child_extents[i_1].xmax, child_extents[i_1].ymax));
								heights_stack.Add(current_height + 1);
								Set_sub_tree_element_count_(child_handle, Get_sub_tree_element_count_(child_handle) + 1);
							}
						}
					}
				}
				else
				{
					element_handle = Insert_at_quad_(element, bounding_box, current_height, current_extent, current_quad_handle, b_flushing, quad_handle, flushed_element_handle, element_handle);
					b_flushing = false;
				}
			}
			// flushing is false after the first inserted element has been flushed down, all subsequent inserts will be new
			return 0;
		}

		private int Insert_at_quad_(int element, com.epl.geometry.Envelope2D bounding_box, int current_height, com.epl.geometry.Envelope2D current_extent, int current_quad_handle, bool b_flushing, int quad_handle, int flushed_element_handle, int duplicate_element_handle)
		{
			// If the bounding box is not contained in any of the current_node's children, or if the current_height is m_height, then insert the element and
			// bounding box into the current_node
			int head_element_handle = Get_first_element_(current_quad_handle);
			int tail_element_handle = Get_last_element_(current_quad_handle);
			int element_handle = -1;
			if (b_flushing)
			{
				System.Diagnostics.Debug.Assert((flushed_element_handle != -1));
				if (current_quad_handle == quad_handle)
				{
					return flushed_element_handle;
				}
				Disconnect_element_handle_(flushed_element_handle);
				// Take it out of the incoming quad_handle, and place in current_quad_handle
				element_handle = flushed_element_handle;
			}
			else
			{
				if (duplicate_element_handle == -1)
				{
					element_handle = Create_element_();
					Set_data_values_(Get_data_(element_handle), element, bounding_box);
				}
				else
				{
					System.Diagnostics.Debug.Assert((m_b_store_duplicates));
					element_handle = Create_element_from_duplicate_(duplicate_element_handle);
				}
			}
			System.Diagnostics.Debug.Assert((!b_flushing || element_handle == flushed_element_handle));
			Set_quad_(element_handle, current_quad_handle);
			// set parent quad (needed for removal of element)
			// assign the prev pointer of the new tail to point at the old tail (tail_element_handle)
			// assign the next pointer of the old tail to point at the new tail (next_element_handle)
			if (tail_element_handle != -1)
			{
				Set_prev_element_(element_handle, tail_element_handle);
				Set_next_element_(tail_element_handle, element_handle);
			}
			else
			{
				System.Diagnostics.Debug.Assert((head_element_handle == -1));
				Set_first_element_(current_quad_handle, element_handle);
			}
			// assign the new tail
			Set_last_element_(current_quad_handle, element_handle);
			Set_local_element_count_(current_quad_handle, Get_local_element_count_(current_quad_handle) + 1);
			if (Can_flush_(current_quad_handle))
			{
				Flush_(current_height, current_extent, current_quad_handle);
			}
			return element_handle;
		}

		private static void Set_child_extents_(com.epl.geometry.Envelope2D current_extent, com.epl.geometry.Envelope2D[] child_extents)
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
		private void Disconnect_element_handle_(int element_handle)
		{
			System.Diagnostics.Debug.Assert((element_handle != -1));
			int quad_handle = Get_quad_(element_handle);
			int head_element_handle = Get_first_element_(quad_handle);
			int tail_element_handle = Get_last_element_(quad_handle);
			int prev_element_handle = Get_prev_element_(element_handle);
			int next_element_handle = Get_next_element_(element_handle);
			System.Diagnostics.Debug.Assert((head_element_handle != -1 && tail_element_handle != -1));
			if (head_element_handle == element_handle)
			{
				if (next_element_handle != -1)
				{
					Set_prev_element_(next_element_handle, -1);
				}
				else
				{
					System.Diagnostics.Debug.Assert((head_element_handle == tail_element_handle));
					System.Diagnostics.Debug.Assert((Get_local_element_count_(quad_handle) == 1));
					Set_last_element_(quad_handle, -1);
				}
				Set_first_element_(quad_handle, next_element_handle);
			}
			else
			{
				if (tail_element_handle == element_handle)
				{
					System.Diagnostics.Debug.Assert((prev_element_handle != -1));
					System.Diagnostics.Debug.Assert((Get_local_element_count_(quad_handle) >= 2));
					Set_next_element_(prev_element_handle, -1);
					Set_last_element_(quad_handle, prev_element_handle);
				}
				else
				{
					System.Diagnostics.Debug.Assert((next_element_handle != -1 && prev_element_handle != -1));
					System.Diagnostics.Debug.Assert((Get_local_element_count_(quad_handle) >= 3));
					Set_prev_element_(next_element_handle, prev_element_handle);
					Set_next_element_(prev_element_handle, next_element_handle);
				}
			}
			Set_prev_element_(element_handle, -1);
			Set_next_element_(element_handle, -1);
			Set_local_element_count_(quad_handle, Get_local_element_count_(quad_handle) - 1);
			System.Diagnostics.Debug.Assert((Get_local_element_count_(quad_handle) >= 0));
		}

		private bool Can_flush_(int quad_handle)
		{
			return Get_local_element_count_(quad_handle) == m_flushing_count && !Has_children_(quad_handle);
		}

		private void Flush_(int height, com.epl.geometry.Envelope2D extent, int quad_handle)
		{
			int element;
			com.epl.geometry.Envelope2D bounding_box = new com.epl.geometry.Envelope2D();
			System.Diagnostics.Debug.Assert((quad_handle != -1));
			int element_handle = Get_first_element_(quad_handle);
			int next_handle = -1;
			int data_handle = -1;
			System.Diagnostics.Debug.Assert((element_handle != -1));
			do
			{
				data_handle = Get_data_(element_handle);
				element = Get_element_value_(data_handle);
				bounding_box.SetCoords(Get_bounding_box_value_(data_handle));
				next_handle = Get_next_element_(element_handle);
				if (!m_b_store_duplicates)
				{
					Insert_(element, bounding_box, height, extent, quad_handle, true, element_handle);
				}
				else
				{
					Insert_duplicates_(element, bounding_box, height, extent, quad_handle, true, element_handle);
				}
				element_handle = next_handle;
			}
			while (element_handle != -1);
		}

		private bool Can_push_down_(int quad_handle)
		{
			return Get_local_element_count_(quad_handle) >= m_flushing_count || Has_children_(quad_handle);
		}

		private bool Has_children_(int parent)
		{
			return Get_child_(parent, 0) != -1 || Get_child_(parent, 1) != -1 || Get_child_(parent, 2) != -1 || Get_child_(parent, 3) != -1;
		}

		private int Create_child_(int parent, int quadrant)
		{
			int child = m_quad_tree_nodes.NewElement();
			Set_child_(parent, quadrant, child);
			Set_sub_tree_element_count_(child, 0);
			Set_local_element_count_(child, 0);
			Set_parent_(child, parent);
			Set_height_and_quadrant_(child, Get_height_(parent) + 1, quadrant);
			if (m_b_store_duplicates)
			{
				Set_contained_sub_tree_element_count_(child, 0);
			}
			return child;
		}

		private void Create_root_()
		{
			m_root = m_quad_tree_nodes.NewElement();
			Set_sub_tree_element_count_(m_root, 0);
			Set_local_element_count_(m_root, 0);
			Set_height_and_quadrant_(m_root, 0, 0);
			if (m_b_store_duplicates)
			{
				Set_contained_sub_tree_element_count_(m_root, 0);
			}
		}

		private int Create_element_()
		{
			int element_handle = m_element_nodes.NewElement();
			int data_handle;
			if (m_free_data.Size() > 0)
			{
				data_handle = m_free_data.Get(m_free_data.Size() - 1);
				m_free_data.RemoveLast();
			}
			else
			{
				data_handle = m_data.Count;
				m_data.Add(null);
			}
			Set_data_(element_handle, data_handle);
			return element_handle;
		}

		private int Create_element_from_duplicate_(int duplicate_element_handle)
		{
			int element_handle = m_element_nodes.NewElement();
			int data_handle = Get_data_(duplicate_element_handle);
			Set_data_(element_handle, data_handle);
			return element_handle;
		}

		private void Free_element_and_box_node_(int element_handle)
		{
			int data_handle = Get_data_(element_handle);
			m_free_data.Add(data_handle);
			m_element_nodes.DeleteElement(element_handle);
		}

		private int Get_child_(int quad_handle, int quadrant)
		{
			return m_quad_tree_nodes.GetField(quad_handle, quadrant);
		}

		private void Set_child_(int parent, int quadrant, int child)
		{
			m_quad_tree_nodes.SetField(parent, quadrant, child);
		}

		private int Get_first_element_(int quad_handle)
		{
			return m_quad_tree_nodes.GetField(quad_handle, 4);
		}

		private void Set_first_element_(int quad_handle, int head)
		{
			m_quad_tree_nodes.SetField(quad_handle, 4, head);
		}

		private int Get_last_element_(int quad_handle)
		{
			return m_quad_tree_nodes.GetField(quad_handle, 5);
		}

		private void Set_last_element_(int quad_handle, int tail)
		{
			m_quad_tree_nodes.SetField(quad_handle, 5, tail);
		}

		private int Get_quadrant_(int quad_handle)
		{
			int height_quadrant_hybrid = m_quad_tree_nodes.GetField(quad_handle, 6);
			int quadrant = height_quadrant_hybrid & m_quadrant_mask;
			return quadrant;
		}

		private int Get_height_(int quad_handle)
		{
			int height_quadrant_hybrid = m_quad_tree_nodes.GetField(quad_handle, 6);
			int height = height_quadrant_hybrid >> m_height_bit_shift;
			return height;
		}

		private void Set_height_and_quadrant_(int quad_handle, int height, int quadrant)
		{
			System.Diagnostics.Debug.Assert((quadrant >= 0 && quadrant <= 3));
			int height_quadrant_hybrid = (int)((height << m_height_bit_shift) | quadrant);
			m_quad_tree_nodes.SetField(quad_handle, 6, height_quadrant_hybrid);
		}

		private int Get_local_element_count_(int quad_handle)
		{
			return m_quad_tree_nodes.GetField(quad_handle, 7);
		}

		private void Set_local_element_count_(int quad_handle, int count)
		{
			m_quad_tree_nodes.SetField(quad_handle, 7, count);
		}

		private int Get_sub_tree_element_count_(int quad_handle)
		{
			return m_quad_tree_nodes.GetField(quad_handle, 8);
		}

		private void Set_sub_tree_element_count_(int quad_handle, int count)
		{
			m_quad_tree_nodes.SetField(quad_handle, 8, count);
		}

		private int Get_parent_(int child)
		{
			return m_quad_tree_nodes.GetField(child, 9);
		}

		private void Set_parent_(int child, int parent)
		{
			m_quad_tree_nodes.SetField(child, 9, parent);
		}

		private int Get_contained_sub_tree_element_count_(int quad_handle)
		{
			return m_quad_tree_nodes.GetField(quad_handle, 10);
		}

		private void Set_contained_sub_tree_element_count_(int quad_handle, int count)
		{
			m_quad_tree_nodes.SetField(quad_handle, 10, count);
		}

		private int Get_data_(int element_handle)
		{
			return m_element_nodes.GetField(element_handle, 0);
		}

		private void Set_data_(int element_handle, int data_handle)
		{
			m_element_nodes.SetField(element_handle, 0, data_handle);
		}

		private int Get_prev_element_(int element_handle)
		{
			return m_element_nodes.GetField(element_handle, 1);
		}

		private int Get_next_element_(int element_handle)
		{
			return m_element_nodes.GetField(element_handle, 2);
		}

		private void Set_prev_element_(int element_handle, int prev_handle)
		{
			m_element_nodes.SetField(element_handle, 1, prev_handle);
		}

		private void Set_next_element_(int element_handle, int next_handle)
		{
			m_element_nodes.SetField(element_handle, 2, next_handle);
		}

		private int Get_quad_(int element_handle)
		{
			return m_element_nodes.GetField(element_handle, 3);
		}

		private void Set_quad_(int element_handle, int parent)
		{
			m_element_nodes.SetField(element_handle, 3, parent);
		}

		private int Get_element_value_(int data_handle)
		{
			return m_data[data_handle].element;
		}

		private com.epl.geometry.Envelope2D Get_bounding_box_value_(int data_handle)
		{
			return m_data[data_handle].box;
		}

		private void Set_data_values_(int data_handle, int element, com.epl.geometry.Envelope2D bounding_box)
		{
			m_data[data_handle] = new com.epl.geometry.QuadTreeImpl.Data(element, bounding_box);
		}

		private com.epl.geometry.Envelope2D m_extent;

		private com.epl.geometry.Envelope2D m_data_extent;

		private com.epl.geometry.StridedIndexTypeCollection m_quad_tree_nodes;

		private com.epl.geometry.StridedIndexTypeCollection m_element_nodes;

		private System.Collections.Generic.List<com.epl.geometry.QuadTreeImpl.Data> m_data;

		private com.epl.geometry.AttributeStreamOfInt32 m_free_data;

		private int m_root;

		private int m_height;

		private bool m_b_store_duplicates;

		private int m_quadrant_mask = 3;

		private int m_height_bit_shift = 2;

		private int m_flushing_count = 5;

		internal sealed class Data
		{
			internal int element;

			internal com.epl.geometry.Envelope2D box;

			internal Data(int element_, com.epl.geometry.Envelope2D box_)
			{
				element = element_;
				box = new com.epl.geometry.Envelope2D();
				box.SetCoords(box_);
			}
		}
		/* m_quad_tree_nodes
		* 0: m_north_east_child
		* 1: m_north_west_child
		* 2: m_south_west_child
		* 3: m_south_east_child
		* 4: m_head_element
		* 5: m_tail_element
		* 6: m_quadrant_and_height
		* 7: m_local_element_count
		* 8: m_sub_tree_element_count
		* 9: m_parent_quad
		* 10: m_height
		*/
		/* m_element_nodes
		* 0: m_data_handle
		* 1: m_prev
		* 2: m_next
		* 3: m_parent_quad
		*/
		/* m_data
		* element
		* box
		*/
	}
}
