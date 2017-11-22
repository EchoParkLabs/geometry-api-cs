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
	public class QuadTree
	{
		public sealed class QuadTreeIterator
		{
			/// <summary>Resets the iterator to an starting state on the Quad_tree.</summary>
			/// <remarks>
			/// Resets the iterator to an starting state on the Quad_tree. If the
			/// input Geometry is a Line segment, then the query will be the segment.
			/// Otherwise the query will be the Envelope_2D bounding the Geometry.
			/// \param query The Geometry used for the query. \param tolerance The
			/// tolerance used for the intersection tests.
			/// </remarks>
			public void ResetIterator(com.epl.geometry.Geometry query, double tolerance)
			{
				m_impl.ResetIterator(query, tolerance);
			}

			/// <summary>
			/// Resets the iterator to a starting state on the Quad_tree using the
			/// input Envelope_2D as the query.
			/// </summary>
			/// <remarks>
			/// Resets the iterator to a starting state on the Quad_tree using the
			/// input Envelope_2D as the query. \param query The Envelope_2D used for
			/// the query. \param tolerance The tolerance used for the intersection
			/// tests.
			/// </remarks>
			public void ResetIterator(com.epl.geometry.Envelope2D query, double tolerance)
			{
				m_impl.ResetIterator(query, tolerance);
			}

			/// <summary>
			/// Moves the iterator to the next Element_handle and returns the
			/// Element_handle.
			/// </summary>
			public int Next()
			{
				return m_impl.Next();
			}

			/// <summary>Returns a void* to the impl class.</summary>
			internal object GetImpl_()
			{
				return m_impl;
			}

			internal QuadTreeIterator(object obj)
			{
				// Creates an iterator on the input Quad_tree_impl. The query will be
				// the Envelope_2D bounding the input Geometry.
				m_impl = (com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl)obj;
			}

			private com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl m_impl;
		}

		/// <summary>
		/// Creates a Quad_tree with the root having the extent of the input
		/// Envelope_2D, and height of the input height, where the root starts at
		/// height 0.
		/// </summary>
		/// <remarks>
		/// Creates a Quad_tree with the root having the extent of the input
		/// Envelope_2D, and height of the input height, where the root starts at
		/// height 0. Note that the height cannot be larger than 16 if on a 32 bit
		/// platform and 32 if on a 64 bit platform. \param extent The extent of the
		/// Quad_tree. \param height The max height of the Quad_tree.
		/// </remarks>
		public QuadTree(com.epl.geometry.Envelope2D extent, int height)
		{
			m_impl = new com.epl.geometry.QuadTreeImpl(extent, height);
		}

		/// <summary>Inserts the element and bounding_box into the Quad_tree.</summary>
		/// <remarks>
		/// Inserts the element and bounding_box into the Quad_tree. Note that a copy
		/// will me made of the input bounding_box. Note that this will invalidate
		/// any active iterator on the Quad_tree. Returns an Element_handle
		/// corresponding to the element and bounding_box. \param element The element
		/// of the Geometry to be inserted. \param bounding_box The bounding_box of
		/// the Geometry to be inserted.
		/// </remarks>
		public virtual int Insert(int element, com.epl.geometry.Envelope2D bounding_box)
		{
			return m_impl.Insert(element, bounding_box);
		}

		/// <summary>
		/// Inserts the element and bounding_box into the Quad_tree at the given
		/// quad_handle.
		/// </summary>
		/// <remarks>
		/// Inserts the element and bounding_box into the Quad_tree at the given
		/// quad_handle. Note that a copy will me made of the input bounding_box.
		/// Note that this will invalidate any active iterator on the Quad_tree.
		/// Returns an Element_handle corresponding to the element and bounding_box.
		/// \param element The element of the Geometry to be inserted. \param
		/// bounding_box The bounding_box of the Geometry to be inserted. \param
		/// hint_index A handle used as a hint where to place the element. This can
		/// be a handle obtained from a previous insertion and is useful on data
		/// having strong locality such as segments of a Polygon.
		/// </remarks>
		public virtual int Insert(int element, com.epl.geometry.Envelope2D bounding_box, int hint_index)
		{
			return m_impl.Insert(element, bounding_box, hint_index);
		}

		/// <summary>Removes the element and bounding_box at the given element_handle.</summary>
		/// <remarks>
		/// Removes the element and bounding_box at the given element_handle. Note
		/// that this will invalidate any active iterator on the Quad_tree. \param
		/// element_handle The handle corresponding to the element and bounding_box
		/// to be removed.
		/// </remarks>
		public virtual void RemoveElement(int element_handle)
		{
			m_impl.RemoveElement(element_handle);
		}

		/// <summary>Returns the element at the given element_handle.</summary>
		/// <remarks>
		/// Returns the element at the given element_handle. \param element_handle
		/// The handle corresponding to the element to be retrieved.
		/// </remarks>
		public virtual int GetElement(int element_handle)
		{
			return m_impl.GetElement(element_handle);
		}

		/// <summary>Returns the height of the quad at the given quad_handle.</summary>
		/// <remarks>
		/// Returns the height of the quad at the given quad_handle. \param
		/// quad_handle The handle corresponding to the quad.
		/// </remarks>
		public virtual int GetHeight(int quad_handle)
		{
			return m_impl.GetHeight(quad_handle);
		}

		/// <summary>Returns the extent of the quad at the given quad_handle.</summary>
		/// <remarks>
		/// Returns the extent of the quad at the given quad_handle. \param
		/// quad_handle The handle corresponding to the quad.
		/// </remarks>
		public virtual com.epl.geometry.Envelope2D GetExtent(int quad_handle)
		{
			return m_impl.GetExtent(quad_handle);
		}

		/// <summary>Returns the Quad_handle of the quad containing the given element_handle.</summary>
		/// <remarks>
		/// Returns the Quad_handle of the quad containing the given element_handle.
		/// \param element_handle The handle corresponding to the element.
		/// </remarks>
		public virtual int GetQuad(int element_handle)
		{
			return m_impl.GetQuad(element_handle);
		}

		/// <summary>Returns the number of elements in the Quad_tree.</summary>
		public virtual int GetElementCount()
		{
			return m_impl.GetElementCount();
		}

		/// <summary>Gets an iterator on the Quad_tree.</summary>
		/// <remarks>
		/// Gets an iterator on the Quad_tree. The query will be the Envelope_2D that
		/// bounds the input Geometry. To reuse the existing iterator on the same
		/// Quad_tree but with a new query, use the reset_iterator function on the
		/// Quad_tree_iterator. \param query The Geometry used for the query. If the
		/// Geometry is a Line segment, then the query will be the segment. Otherwise
		/// the query will be the Envelope_2D bounding the Geometry. \param tolerance
		/// The tolerance used for the intersection tests.
		/// </remarks>
		public virtual com.epl.geometry.QuadTree.QuadTreeIterator GetIterator(com.epl.geometry.Geometry query, double tolerance)
		{
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl iterator = m_impl.GetIterator(query, tolerance);
			return new com.epl.geometry.QuadTree.QuadTreeIterator(iterator);
		}

		/// <summary>
		/// Gets an iterator on the Quad_tree using the input Envelope_2D as the
		/// query.
		/// </summary>
		/// <remarks>
		/// Gets an iterator on the Quad_tree using the input Envelope_2D as the
		/// query. To reuse the existing iterator on the same Quad_tree but with a
		/// new query, use the reset_iterator function on the Quad_tree_iterator.
		/// \param query The Envelope_2D used for the query. \param tolerance The
		/// tolerance used for the intersection tests.
		/// </remarks>
		public virtual com.epl.geometry.QuadTree.QuadTreeIterator GetIterator(com.epl.geometry.Envelope2D query, double tolerance)
		{
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl iterator = m_impl.GetIterator(query, tolerance);
			return new com.epl.geometry.QuadTree.QuadTreeIterator(iterator);
		}

		/// <summary>Gets an iterator on the Quad_tree.</summary>
		public virtual com.epl.geometry.QuadTree.QuadTreeIterator GetIterator()
		{
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl iterator = m_impl.GetIterator();
			return new com.epl.geometry.QuadTree.QuadTreeIterator(iterator);
		}

		/// <summary>Returns a void* to the impl class.</summary>
		internal virtual object GetImpl_()
		{
			return m_impl;
		}

		private com.epl.geometry.QuadTreeImpl m_impl;
	}
}
