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
	public class QuadTree
	{
		public sealed class QuadTreeIterator
		{
			/// <summary>Resets the iterator to an starting state on the QuadTree.</summary>
			/// <remarks>
			/// Resets the iterator to an starting state on the QuadTree. If the
			/// input Geometry is a Line segment, then the query will be the segment.
			/// Otherwise the query will be the Envelope2D bounding the Geometry.
			/// \param query The Geometry used for the query.
			/// \param tolerance The tolerance used for the intersection tests.
			/// </remarks>
			public void ResetIterator(com.epl.geometry.Geometry query, double tolerance)
			{
				if (!m_b_sorted)
				{
					((com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl)m_impl).ResetIterator(query, tolerance);
				}
				else
				{
					((com.epl.geometry.QuadTreeImpl.QuadTreeSortedIteratorImpl)m_impl).ResetIterator(query, tolerance);
				}
			}

			/// <summary>
			/// Resets the iterator to a starting state on the QuadTree using the
			/// input Envelope2D as the query.
			/// </summary>
			/// <remarks>
			/// Resets the iterator to a starting state on the QuadTree using the
			/// input Envelope2D as the query.
			/// \param query The Envelope2D used for the query.
			/// \param tolerance The tolerance used for the intersection
			/// tests.
			/// </remarks>
			public void ResetIterator(com.epl.geometry.Envelope2D query, double tolerance)
			{
				if (!m_b_sorted)
				{
					((com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl)m_impl).ResetIterator(query, tolerance);
				}
				else
				{
					((com.epl.geometry.QuadTreeImpl.QuadTreeSortedIteratorImpl)m_impl).ResetIterator(query, tolerance);
				}
			}

			/// <summary>
			/// Moves the iterator to the next Element_handle and returns the
			/// Element_handle.
			/// </summary>
			public int Next()
			{
				if (!m_b_sorted)
				{
					return ((com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl)m_impl).Next();
				}
				else
				{
					return ((com.epl.geometry.QuadTreeImpl.QuadTreeSortedIteratorImpl)m_impl).Next();
				}
			}

			/// <summary>Returns a void* to the impl class.</summary>
			internal object GetImpl_()
			{
				return m_impl;
			}

			internal QuadTreeIterator(object obj, bool bSorted)
			{
				// Creates an iterator on the input QuadTreeImpl. The query will be
				// the Envelope2D bounding the input Geometry.
				m_impl = obj;
				m_b_sorted = bSorted;
			}

			private object m_impl;

			private bool m_b_sorted;
		}

		/// <summary>
		/// Creates a QuadTree with the root having the extent of the input
		/// Envelope2D, and height of the input height, where the root starts at height 0.
		/// </summary>
		/// <remarks>
		/// Creates a QuadTree with the root having the extent of the input
		/// Envelope2D, and height of the input height, where the root starts at height 0.
		/// \param extent The extent of the QuadTree.
		/// \param height The max height of the QuadTree.
		/// </remarks>
		public QuadTree(com.epl.geometry.Envelope2D extent, int height)
		{
			m_impl = new com.epl.geometry.QuadTreeImpl(extent, height);
		}

		/// <summary>Creates a QuadTree with the root having the extent of the input Envelope2D, and height of the input height, where the root starts at height 0.</summary>
		/// <remarks>
		/// Creates a QuadTree with the root having the extent of the input Envelope2D, and height of the input height, where the root starts at height 0.
		/// \param extent The extent of the QuadTreeImpl.
		/// \param height The max height of the QuadTreeImpl.
		/// \param bStoreDuplicates Put true to place elements deeper into the quad tree at intesecting quads, duplicates will be stored. Put false to only place elements into quads that can contain it..
		/// </remarks>
		public QuadTree(com.epl.geometry.Envelope2D extent, int height, bool bStoreDuplicates)
		{
			m_impl = new com.epl.geometry.QuadTreeImpl(extent, height, bStoreDuplicates);
		}

		/// <summary>Inserts the element and bounding_box into the QuadTree.</summary>
		/// <remarks>
		/// Inserts the element and bounding_box into the QuadTree. Note that a copy
		/// will me made of the input bounding_box. Note that this will invalidate
		/// any active iterator on the QuadTree. Returns an Element_handle
		/// corresponding to the element and bounding_box.
		/// \param element The element of the Geometry to be inserted.
		/// \param bounding_box The bounding_box of
		/// the Geometry to be inserted.
		/// </remarks>
		public virtual int Insert(int element, com.epl.geometry.Envelope2D boundingBox)
		{
			return m_impl.Insert(element, boundingBox);
		}

		/// <summary>
		/// Inserts the element and bounding_box into the QuadTree at the given
		/// quad_handle.
		/// </summary>
		/// <remarks>
		/// Inserts the element and bounding_box into the QuadTree at the given
		/// quad_handle. Note that a copy will me made of the input bounding_box.
		/// Note that this will invalidate any active iterator on the QuadTree.
		/// Returns an Element_handle corresponding to the element and bounding_box.
		/// \param element The element of the Geometry to be inserted.
		/// \param bounding_box The bounding_box of the Geometry to be inserted.
		/// \param hint_index A handle used as a hint where to place the element. This can
		/// be a handle obtained from a previous insertion and is useful on data
		/// having strong locality such as segments of a Polygon.
		/// </remarks>
		public virtual int Insert(int element, com.epl.geometry.Envelope2D boundingBox, int hintIndex)
		{
			return m_impl.Insert(element, boundingBox, hintIndex);
		}

		/// <summary>Removes the element and bounding_box at the given element_handle.</summary>
		/// <remarks>
		/// Removes the element and bounding_box at the given element_handle. Note
		/// that this will invalidate any active iterator on the QuadTree.
		/// \param element_handle The handle corresponding to the element and bounding_box
		/// to be removed.
		/// </remarks>
		public virtual void RemoveElement(int elementHandle)
		{
			m_impl.RemoveElement(elementHandle);
		}

		/// <summary>Returns the element at the given element_handle.</summary>
		/// <remarks>
		/// Returns the element at the given element_handle.
		/// \param element_handle The handle corresponding to the element to be retrieved.
		/// </remarks>
		public virtual int GetElement(int elementHandle)
		{
			return m_impl.GetElement(elementHandle);
		}

		/// <summary>Returns the element extent at the given element_handle.</summary>
		/// <remarks>
		/// Returns the element extent at the given element_handle.
		/// \param element_handle The handle corresponding to the element extent to be retrieved.
		/// </remarks>
		public virtual com.epl.geometry.Envelope2D GetElementExtent(int elementHandle)
		{
			return m_impl.GetElementExtent(elementHandle);
		}

		/// <summary>Returns the extent of all elements in the quad tree.</summary>
		public virtual com.epl.geometry.Envelope2D GetDataExtent()
		{
			return m_impl.GetDataExtent();
		}

		/// <summary>Returns the extent of the quad tree.</summary>
		public virtual com.epl.geometry.Envelope2D GetQuadTreeExtent()
		{
			return m_impl.GetQuadTreeExtent();
		}

		/// <summary>Returns the number of elements in the subtree rooted at the given quad_handle.</summary>
		/// <remarks>
		/// Returns the number of elements in the subtree rooted at the given quad_handle.
		/// \param quad_handle The handle corresponding to the quad.
		/// </remarks>
		public virtual int GetSubTreeElementCount(int quadHandle)
		{
			return m_impl.GetSubTreeElementCount(quadHandle);
		}

		/// <summary>Returns the number of elements contained in the subtree rooted at the given quad_handle.</summary>
		/// <remarks>
		/// Returns the number of elements contained in the subtree rooted at the given quad_handle.
		/// \param quad_handle The handle corresponding to the quad.
		/// </remarks>
		public virtual int GetContainedSubTreeElementCount(int quadHandle)
		{
			return m_impl.GetContainedSubTreeElementCount(quadHandle);
		}

		/// <summary>Returns the number of elements in the quad tree that intersect the qiven query.</summary>
		/// <remarks>
		/// Returns the number of elements in the quad tree that intersect the qiven query. Some elements may be duplicated if the quad tree stores duplicates.
		/// \param query The Envelope2D used for the query.
		/// \param tolerance The tolerance used for the intersection tests.
		/// \param max_count If the intersection count becomes greater than or equal to the max_count, then max_count is returned.
		/// </remarks>
		public virtual int GetIntersectionCount(com.epl.geometry.Envelope2D query, double tolerance, int maxCount)
		{
			return m_impl.GetIntersectionCount(query, tolerance, maxCount);
		}

		/// <summary>Returns true if the quad tree has data intersecting the given query.</summary>
		/// <remarks>
		/// Returns true if the quad tree has data intersecting the given query.
		/// \param query The Envelope2D used for the query.
		/// \param tolerance The tolerance used for the intersection tests.
		/// </remarks>
		public virtual bool HasData(com.epl.geometry.Envelope2D query, double tolerance)
		{
			return m_impl.HasData(query, tolerance);
		}

		/// <summary>Returns the height of the quad at the given quad_handle.</summary>
		/// <remarks>
		/// Returns the height of the quad at the given quad_handle. \param
		/// quad_handle The handle corresponding to the quad.
		/// </remarks>
		public virtual int GetHeight(int quadHandle)
		{
			return m_impl.GetHeight(quadHandle);
		}

		/// <summary>Returns the max height the quad tree can grow to.</summary>
		public virtual int GetMaxHeight()
		{
			return m_impl.GetMaxHeight();
		}

		/// <summary>Returns the extent of the quad at the given quad_handle.</summary>
		/// <remarks>
		/// Returns the extent of the quad at the given quad_handle.
		/// \param quad_handle The handle corresponding to the quad.
		/// </remarks>
		public virtual com.epl.geometry.Envelope2D GetExtent(int quadHandle)
		{
			return m_impl.GetExtent(quadHandle);
		}

		/// <summary>Returns the Quad_handle of the quad containing the given element_handle.</summary>
		/// <remarks>
		/// Returns the Quad_handle of the quad containing the given element_handle.
		/// \param element_handle The handle corresponding to the element.
		/// </remarks>
		public virtual int GetQuad(int elementHandle)
		{
			return m_impl.GetQuad(elementHandle);
		}

		/// <summary>Returns the number of elements in the QuadTree.</summary>
		public virtual int GetElementCount()
		{
			return m_impl.GetElementCount();
		}

		/// <summary>Gets an iterator on the QuadTree.</summary>
		/// <remarks>
		/// Gets an iterator on the QuadTree. The query will be the Envelope2D that
		/// bounds the input Geometry. To reuse the existing iterator on the same
		/// QuadTree but with a new query, use the reset_iterator function on the
		/// QuadTree_iterator.
		/// \param query The Geometry used for the query. If the
		/// Geometry is a Line segment, then the query will be the segment. Otherwise
		/// the query will be the Envelope2D bounding the Geometry.
		/// \param tolerance The tolerance used for the intersection tests.
		/// </remarks>
		public virtual com.epl.geometry.QuadTree.QuadTreeIterator GetIterator(com.epl.geometry.Geometry query, double tolerance)
		{
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl iterator = m_impl.GetIterator(query, tolerance);
			return new com.epl.geometry.QuadTree.QuadTreeIterator(iterator, false);
		}

		/// <summary>
		/// Gets an iterator on the QuadTree using the input Envelope2D as the
		/// query.
		/// </summary>
		/// <remarks>
		/// Gets an iterator on the QuadTree using the input Envelope2D as the
		/// query. To reuse the existing iterator on the same QuadTree but with a
		/// new query, use the reset_iterator function on the QuadTree_iterator.
		/// \param query The Envelope2D used for the query.
		/// \param tolerance The tolerance used for the intersection tests.
		/// </remarks>
		public virtual com.epl.geometry.QuadTree.QuadTreeIterator GetIterator(com.epl.geometry.Envelope2D query, double tolerance)
		{
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl iterator = m_impl.GetIterator(query, tolerance);
			return new com.epl.geometry.QuadTree.QuadTreeIterator(iterator, false);
		}

		/// <summary>Gets an iterator on the QuadTree.</summary>
		public virtual com.epl.geometry.QuadTree.QuadTreeIterator GetIterator()
		{
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl iterator = m_impl.GetIterator();
			return new com.epl.geometry.QuadTree.QuadTreeIterator(iterator, false);
		}

		/// <summary>Gets an iterator on the QuadTree.</summary>
		/// <remarks>
		/// Gets an iterator on the QuadTree. The query will be the Envelope2D that bounds the input Geometry.
		/// To reuse the existing iterator on the same QuadTree but with a new query, use the reset_iterator function on the QuadTree_iterator.
		/// \param query The Geometry used for the query. If the Geometry is a Line segment, then the query will be the segment. Otherwise the query will be the Envelope2D bounding the Geometry.
		/// \param tolerance The tolerance used for the intersection tests.
		/// \param bSorted Put true to iterate the quad tree in the order of the Element_types.
		/// </remarks>
		public virtual com.epl.geometry.QuadTree.QuadTreeIterator GetIterator(com.epl.geometry.Geometry query, double tolerance, bool bSorted)
		{
			if (!bSorted)
			{
				com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl iterator = m_impl.GetIterator(query, tolerance);
				return new com.epl.geometry.QuadTree.QuadTreeIterator(iterator, false);
			}
			else
			{
				com.epl.geometry.QuadTreeImpl.QuadTreeSortedIteratorImpl iterator = m_impl.GetSortedIterator(query, tolerance);
				return new com.epl.geometry.QuadTree.QuadTreeIterator(iterator, true);
			}
		}

		/// <summary>Gets an iterator on the QuadTree using the input Envelope2D as the query.</summary>
		/// <remarks>
		/// Gets an iterator on the QuadTree using the input Envelope2D as the query.
		/// To reuse the existing iterator on the same QuadTree but with a new query, use the reset_iterator function on the QuadTree_iterator.
		/// \param query The Envelope2D used for the query.
		/// \param tolerance The tolerance used for the intersection tests.
		/// \param bSorted Put true to iterate the quad tree in the order of the Element_types.
		/// </remarks>
		public virtual com.epl.geometry.QuadTree.QuadTreeIterator GetIterator(com.epl.geometry.Envelope2D query, double tolerance, bool bSorted)
		{
			if (!bSorted)
			{
				com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl iterator = m_impl.GetIterator(query, tolerance);
				return new com.epl.geometry.QuadTree.QuadTreeIterator(iterator, false);
			}
			else
			{
				com.epl.geometry.QuadTreeImpl.QuadTreeSortedIteratorImpl iterator = m_impl.GetSortedIterator(query, tolerance);
				return new com.epl.geometry.QuadTree.QuadTreeIterator(iterator, true);
			}
		}

		/// <summary>Gets an iterator on the QuadTree.</summary>
		/// <remarks>
		/// Gets an iterator on the QuadTree.
		/// \param bSorted Put true to iterate the quad tree in the order of the Element_types.
		/// </remarks>
		public virtual com.epl.geometry.QuadTree.QuadTreeIterator GetIterator(bool bSorted)
		{
			if (!bSorted)
			{
				com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl iterator = m_impl.GetIterator();
				return new com.epl.geometry.QuadTree.QuadTreeIterator(iterator, false);
			}
			else
			{
				com.epl.geometry.QuadTreeImpl.QuadTreeSortedIteratorImpl iterator = m_impl.GetSortedIterator();
				return new com.epl.geometry.QuadTree.QuadTreeIterator(iterator, true);
			}
		}

		/// <summary>Returns a void* to the impl class.</summary>
		internal virtual object GetImpl_()
		{
			return m_impl;
		}

		private com.epl.geometry.QuadTreeImpl m_impl;
	}
}
