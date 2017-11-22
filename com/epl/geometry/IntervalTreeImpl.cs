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
	internal sealed class IntervalTreeImpl
	{
		internal sealed class IntervalTreeIteratorImpl
		{
			/// <summary>
			/// Resets the iterator to a starting state on the Interval_tree_impl
			/// using the input Envelope_1D interval as the query \param query The
			/// Envelope_1D interval used for the query.
			/// </summary>
			/// <remarks>
			/// Resets the iterator to a starting state on the Interval_tree_impl
			/// using the input Envelope_1D interval as the query \param query The
			/// Envelope_1D interval used for the query. \param tolerance The
			/// tolerance used for the intersection tests.
			/// </remarks>
			internal void ResetIterator(com.epl.geometry.Envelope1D query, double tolerance)
			{
				m_query.vmin = query.vmin - tolerance;
				m_query.vmax = query.vmax + tolerance;
				m_tertiary_stack.Resize(0);
				m_function_index = 0;
				m_function_stack[0] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.initialize;
			}

			/// <summary>
			/// Resets the iterator to a starting state on the Interval_tree_impl
			/// using the input Envelope_1D interval as the query \param query The
			/// Envelope_1D interval used for the query.
			/// </summary>
			/// <remarks>
			/// Resets the iterator to a starting state on the Interval_tree_impl
			/// using the input Envelope_1D interval as the query \param query The
			/// Envelope_1D interval used for the query. \param tolerance The
			/// tolerance used for the intersection tests.
			/// </remarks>
			internal void ResetIterator(double query_min, double query_max, double tolerance)
			{
				if (query_min > query_max)
				{
					throw new System.ArgumentException();
				}
				m_query.vmin = query_min - tolerance;
				m_query.vmax = query_max + tolerance;
				m_tertiary_stack.Resize(0);
				m_function_index = 0;
				m_function_stack[0] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.initialize;
			}

			/// <summary>
			/// Resets the iterator to a starting state on the Interval_tree_impl
			/// using the input double as the stabbing query \param query The double
			/// used for the query.
			/// </summary>
			/// <remarks>
			/// Resets the iterator to a starting state on the Interval_tree_impl
			/// using the input double as the stabbing query \param query The double
			/// used for the query. \param tolerance The tolerance used for the
			/// intersection tests.
			/// </remarks>
			internal void ResetIterator(double query, double tolerance)
			{
				m_query.vmin = query - tolerance;
				m_query.vmax = query + tolerance;
				m_tertiary_stack.Resize(0);
				m_function_index = 0;
				m_function_stack[0] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.initialize;
			}

			/// <summary>Iterates over all intervals which interset the query interval.</summary>
			/// <remarks>
			/// Iterates over all intervals which interset the query interval.
			/// Returns an index to an interval that intersects the query.
			/// </remarks>
			internal int Next()
			{
				if (!m_interval_tree.m_b_construction_ended)
				{
					throw new com.epl.geometry.GeometryException("invalid call");
				}
				if (m_function_index < 0)
				{
					return -1;
				}
				bool b_searching = true;
				while (b_searching)
				{
					switch (m_function_stack[m_function_index])
					{
						case com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.pIn:
						{
							b_searching = PIn_();
							break;
						}

						case com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.pL:
						{
							b_searching = PL_();
							break;
						}

						case com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.pR:
						{
							b_searching = PR_();
							break;
						}

						case com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.pT:
						{
							b_searching = PT_();
							break;
						}

						case com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.right:
						{
							b_searching = Right_();
							break;
						}

						case com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.left:
						{
							b_searching = Left_();
							break;
						}

						case com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.all:
						{
							b_searching = All_();
							break;
						}

						case com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.initialize:
						{
							b_searching = Initialize_();
							break;
						}

						default:
						{
							throw com.epl.geometry.GeometryException.GeometryInternalError();
						}
					}
				}
				if (m_current_end_handle != -1)
				{
					return GetCurrentEndIndex_() >> 1;
				}
				return -1;
			}

			internal IntervalTreeIteratorImpl(com.epl.geometry.IntervalTreeImpl interval_tree, com.epl.geometry.Envelope1D query, double tolerance)
			{
				// Creates an iterator on the input Interval_tree using the input
				// Envelope_1D interval as the query.
				m_interval_tree = interval_tree;
				m_tertiary_stack.Reserve(20);
				ResetIterator(query, tolerance);
			}

			internal IntervalTreeIteratorImpl(com.epl.geometry.IntervalTreeImpl interval_tree, double query, double tolerance)
			{
				// Creates an iterator on the input Interval_tree using the input double
				// as the stabbing query.
				m_interval_tree = interval_tree;
				m_tertiary_stack.Reserve(20);
				ResetIterator(query, tolerance);
			}

			internal IntervalTreeIteratorImpl(com.epl.geometry.IntervalTreeImpl interval_tree)
			{
				// Creates an iterator on the input Interval_tree.
				m_interval_tree = interval_tree;
				m_tertiary_stack.Reserve(20);
				m_function_index = -1;
			}

			private com.epl.geometry.IntervalTreeImpl m_interval_tree;

			private com.epl.geometry.Envelope1D m_query = new com.epl.geometry.Envelope1D();

			private int m_primary_handle;

			private int m_next_primary_handle;

			private int m_forked_handle;

			private int m_current_end_handle;

			private int m_next_end_handle;

			private com.epl.geometry.AttributeStreamOfInt32 m_tertiary_stack = new com.epl.geometry.AttributeStreamOfInt32(0);

			private int m_function_index;

			private int[] m_function_stack = new int[2];

			private abstract class State
			{
				public const int initialize = 0;

				public const int pIn = 1;

				public const int pL = 2;

				public const int pR = 3;

				public const int pT = 4;

				public const int right = 5;

				public const int left = 6;

				public const int all = 7;
			}

			private static class StateConstants
			{
			}

			private bool Initialize_()
			{
				m_primary_handle = -1;
				m_next_primary_handle = -1;
				m_forked_handle = -1;
				m_current_end_handle = -1;
				if (m_interval_tree.m_primary_nodes != null && m_interval_tree.m_primary_nodes.Size() > 0)
				{
					m_function_stack[0] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.pIn;
					// overwrite initialize
					m_next_primary_handle = m_interval_tree.m_root;
					return true;
				}
				m_function_index = -1;
				return false;
			}

			private bool PIn_()
			{
				m_primary_handle = m_next_primary_handle;
				if (m_primary_handle == -1)
				{
					m_function_index = -1;
					m_current_end_handle = -1;
					return false;
				}
				double discriminant = m_interval_tree.GetDiscriminant_(m_primary_handle);
				if (m_query.vmax < discriminant)
				{
					int secondary_handle = m_interval_tree.GetSecondaryFromPrimary(m_primary_handle);
					m_next_primary_handle = m_interval_tree.GetLPTR_(m_primary_handle);
					if (secondary_handle != -1)
					{
						m_next_end_handle = m_interval_tree.GetFirst_(secondary_handle);
						m_function_stack[++m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.left;
					}
					return true;
				}
				if (discriminant < m_query.vmin)
				{
					int secondary_handle = m_interval_tree.GetSecondaryFromPrimary(m_primary_handle);
					m_next_primary_handle = m_interval_tree.GetRPTR_(m_primary_handle);
					if (secondary_handle != -1)
					{
						m_next_end_handle = m_interval_tree.GetLast_(secondary_handle);
						m_function_stack[++m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.right;
					}
					return true;
				}
				System.Diagnostics.Debug.Assert((m_query.Contains(discriminant)));
				m_function_stack[m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.pL;
				// overwrite pIn
				m_forked_handle = m_primary_handle;
				int secondary_handle_1 = m_interval_tree.GetSecondaryFromPrimary(m_primary_handle);
				m_next_primary_handle = m_interval_tree.GetLPTR_(m_primary_handle);
				if (secondary_handle_1 != -1)
				{
					m_next_end_handle = m_interval_tree.GetFirst_(secondary_handle_1);
					m_function_stack[++m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.all;
				}
				return true;
			}

			private bool PL_()
			{
				m_primary_handle = m_next_primary_handle;
				if (m_primary_handle == -1)
				{
					m_function_stack[m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.pR;
					// overwrite pL
					m_next_primary_handle = m_interval_tree.GetRPTR_(m_forked_handle);
					return true;
				}
				double discriminant = m_interval_tree.GetDiscriminant_(m_primary_handle);
				if (discriminant < m_query.vmin)
				{
					int secondary_handle = m_interval_tree.GetSecondaryFromPrimary(m_primary_handle);
					m_next_primary_handle = m_interval_tree.GetRPTR_(m_primary_handle);
					if (secondary_handle != -1)
					{
						m_next_end_handle = m_interval_tree.GetLast_(secondary_handle);
						m_function_stack[++m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.right;
					}
					return true;
				}
				System.Diagnostics.Debug.Assert((m_query.Contains(discriminant)));
				int secondary_handle_1 = m_interval_tree.GetSecondaryFromPrimary(m_primary_handle);
				m_next_primary_handle = m_interval_tree.GetLPTR_(m_primary_handle);
				if (secondary_handle_1 != -1)
				{
					m_next_end_handle = m_interval_tree.GetFirst_(secondary_handle_1);
					m_function_stack[++m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.all;
				}
				int rptr = m_interval_tree.GetRPTR_(m_primary_handle);
				if (rptr != -1)
				{
					m_tertiary_stack.Add(rptr);
				}
				// we'll search this in the pT state
				return true;
			}

			private bool PR_()
			{
				m_primary_handle = m_next_primary_handle;
				if (m_primary_handle == -1)
				{
					m_function_stack[m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.pT;
					// overwrite pR
					return true;
				}
				double discriminant = m_interval_tree.GetDiscriminant_(m_primary_handle);
				if (m_query.vmax < discriminant)
				{
					int secondary_handle = m_interval_tree.GetSecondaryFromPrimary(m_primary_handle);
					m_next_primary_handle = m_interval_tree.GetLPTR_(m_primary_handle);
					if (secondary_handle != -1)
					{
						m_next_end_handle = m_interval_tree.GetFirst_(secondary_handle);
						m_function_stack[++m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.left;
					}
					return true;
				}
				System.Diagnostics.Debug.Assert((m_query.Contains(discriminant)));
				int secondary_handle_1 = m_interval_tree.GetSecondaryFromPrimary(m_primary_handle);
				m_next_primary_handle = m_interval_tree.GetRPTR_(m_primary_handle);
				if (secondary_handle_1 != -1)
				{
					m_next_end_handle = m_interval_tree.GetFirst_(secondary_handle_1);
					m_function_stack[++m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.all;
				}
				int lptr = m_interval_tree.GetLPTR_(m_primary_handle);
				if (lptr != -1)
				{
					m_tertiary_stack.Add(lptr);
				}
				// we'll search this in the pT state
				return true;
			}

			private bool PT_()
			{
				if (m_tertiary_stack.Size() == 0)
				{
					m_function_index = -1;
					m_current_end_handle = -1;
					return false;
				}
				m_primary_handle = m_tertiary_stack.Get(m_tertiary_stack.Size() - 1);
				m_tertiary_stack.Resize(m_tertiary_stack.Size() - 1);
				int secondary_handle = m_interval_tree.GetSecondaryFromPrimary(m_primary_handle);
				if (secondary_handle != -1)
				{
					m_next_end_handle = m_interval_tree.GetFirst_(secondary_handle);
					m_function_stack[++m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.all;
				}
				if (m_interval_tree.GetLPTR_(m_primary_handle) != -1)
				{
					m_tertiary_stack.Add(m_interval_tree.GetLPTR_(m_primary_handle));
				}
				if (m_interval_tree.GetRPTR_(m_primary_handle) != -1)
				{
					m_tertiary_stack.Add(m_interval_tree.GetRPTR_(m_primary_handle));
				}
				return true;
			}

			private bool Left_()
			{
				m_current_end_handle = m_next_end_handle;
				if (m_current_end_handle != -1 && com.epl.geometry.IntervalTreeImpl.IsLeft_(GetCurrentEndIndex_()) && m_interval_tree.GetValue_(GetCurrentEndIndex_()) <= m_query.vmax)
				{
					m_next_end_handle = GetNext_();
					return false;
				}
				m_function_index--;
				return true;
			}

			private bool Right_()
			{
				m_current_end_handle = m_next_end_handle;
				if (m_current_end_handle != -1 && com.epl.geometry.IntervalTreeImpl.IsRight_(GetCurrentEndIndex_()) && m_interval_tree.GetValue_(GetCurrentEndIndex_()) >= m_query.vmin)
				{
					m_next_end_handle = GetPrev_();
					return false;
				}
				m_function_index--;
				return true;
			}

			private bool All_()
			{
				m_current_end_handle = m_next_end_handle;
				if (m_current_end_handle != -1 && com.epl.geometry.IntervalTreeImpl.IsLeft_(GetCurrentEndIndex_()))
				{
					m_next_end_handle = GetNext_();
					return false;
				}
				m_function_index--;
				return true;
			}

			private int GetNext_()
			{
				if (!m_interval_tree.m_b_offline_dynamic)
				{
					return m_interval_tree.m_secondary_lists.GetNext(m_current_end_handle);
				}
				return m_interval_tree.m_secondary_treaps.GetNext(m_current_end_handle);
			}

			private int GetPrev_()
			{
				if (!m_interval_tree.m_b_offline_dynamic)
				{
					return m_interval_tree.m_secondary_lists.GetPrev(m_current_end_handle);
				}
				return m_interval_tree.m_secondary_treaps.GetPrev(m_current_end_handle);
			}

			private int GetCurrentEndIndex_()
			{
				if (!m_interval_tree.m_b_offline_dynamic)
				{
					return m_interval_tree.m_secondary_lists.GetData(m_current_end_handle);
				}
				return m_interval_tree.m_secondary_treaps.GetElement(m_current_end_handle);
			}
		}

		internal IntervalTreeImpl(bool b_offline_dynamic)
		{
			m_b_offline_dynamic = b_offline_dynamic;
			m_b_constructing = false;
			m_b_construction_ended = false;
		}

		internal void StartConstruction()
		{
			Reset_(true);
		}

		internal void AddInterval(com.epl.geometry.Envelope1D interval)
		{
			if (!m_b_constructing)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			m_intervals.Add(interval);
		}

		internal void AddInterval(double min, double max)
		{
			if (!m_b_constructing)
			{
				throw new com.epl.geometry.GeometryException("invald call");
			}
			m_intervals.Add(new com.epl.geometry.Envelope1D(min, max));
		}

		internal void EndConstruction()
		{
			if (!m_b_constructing)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			m_b_constructing = false;
			m_b_construction_ended = true;
			if (!m_b_offline_dynamic)
			{
				InsertIntervalsStatic_();
				m_c_count = m_intervals.Count;
			}
		}

		/// <summary>Inserts the interval from the given index into the Interval_tree_impl.</summary>
		/// <remarks>
		/// Inserts the interval from the given index into the Interval_tree_impl.
		/// This operation can only be performed in the offline dynamic case. \param
		/// index The index containing the interval to be inserted.
		/// </remarks>
		internal void Insert(int index)
		{
			if (!m_b_offline_dynamic || !m_b_construction_ended)
			{
				throw new System.ArgumentException("invalid call");
			}
			if (m_root == -1)
			{
				int size = m_intervals.Count;
				if (m_b_sort_intervals)
				{
					// sort
					com.epl.geometry.AttributeStreamOfInt32 end_point_indices_sorted = new com.epl.geometry.AttributeStreamOfInt32(0);
					end_point_indices_sorted.Reserve(2 * size);
					QuerySortedEndPointIndices_(end_point_indices_sorted);
					// remove duplicates
					m_end_indices_unique.Reserve(2 * size);
					m_end_indices_unique.Resize(0);
					QuerySortedDuplicatesRemoved_(end_point_indices_sorted);
					m_interval_handles.Resize(size, -1);
					m_interval_handles.SetRange(-1, 0, size);
					m_b_sort_intervals = false;
				}
				else
				{
					m_interval_handles.SetRange(-1, 0, size);
				}
				m_root = CreatePrimaryNode_();
			}
			int interval_handle = InsertIntervalEnd_(index << 1, m_root);
			int secondary_handle = GetSecondaryFromInterval_(interval_handle);
			int right_end_handle = m_secondary_treaps.AddElement((index << 1) + 1, secondary_handle);
			SetRightEnd_(interval_handle, right_end_handle);
			m_interval_handles.Set(index, interval_handle);
			m_c_count++;
		}

		// assert(check_validation_());
		/// <summary>Deletes the interval from the Interval_tree_impl.</summary>
		/// <remarks>
		/// Deletes the interval from the Interval_tree_impl. \param index The index
		/// containing the interval to be deleted from the Interval_tree_impl.
		/// </remarks>
		internal void Remove(int index)
		{
			if (!m_b_offline_dynamic || !m_b_construction_ended)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			int interval_handle = m_interval_handles.Get(index);
			if (interval_handle == -1)
			{
				throw new System.ArgumentException("the interval does not exist in the interval tree");
			}
			m_interval_handles.Set(index, -1);
			System.Diagnostics.Debug.Assert((GetSecondaryFromInterval_(interval_handle) != -1));
			System.Diagnostics.Debug.Assert((GetLeftEnd_(interval_handle) != -1));
			System.Diagnostics.Debug.Assert((GetRightEnd_(interval_handle) != -1));
			m_c_count--;
			int size;
			int secondary_handle = GetSecondaryFromInterval_(interval_handle);
			int primary_handle;
			primary_handle = m_secondary_treaps.GetTreapData(secondary_handle);
			m_secondary_treaps.DeleteNode(GetLeftEnd_(interval_handle), secondary_handle);
			m_secondary_treaps.DeleteNode(GetRightEnd_(interval_handle), secondary_handle);
			size = m_secondary_treaps.Size(secondary_handle);
			if (size == 0)
			{
				m_secondary_treaps.DeleteTreap(secondary_handle);
				SetSecondaryToPrimary_(primary_handle, -1);
			}
			m_interval_nodes.DeleteElement(interval_handle);
			int tertiary_handle = GetPPTR_(primary_handle);
			int lptr = GetLPTR_(primary_handle);
			int rptr = GetRPTR_(primary_handle);
			int iterations = 0;
			while (!(size > 0 || primary_handle == m_root || (lptr != -1 && rptr != -1)))
			{
				System.Diagnostics.Debug.Assert((size == 0));
				System.Diagnostics.Debug.Assert((lptr == -1 || rptr == -1));
				System.Diagnostics.Debug.Assert((primary_handle != 0));
				if (primary_handle == GetLPTR_(tertiary_handle))
				{
					if (lptr != -1)
					{
						SetLPTR_(tertiary_handle, lptr);
						SetPPTR_(lptr, tertiary_handle);
						SetLPTR_(primary_handle, -1);
						SetPPTR_(primary_handle, -1);
					}
					else
					{
						if (rptr != -1)
						{
							SetLPTR_(tertiary_handle, rptr);
							SetPPTR_(rptr, tertiary_handle);
							SetRPTR_(primary_handle, -1);
							SetPPTR_(primary_handle, -1);
						}
						else
						{
							SetLPTR_(tertiary_handle, -1);
							SetPPTR_(primary_handle, -1);
						}
					}
				}
				else
				{
					if (lptr != -1)
					{
						SetRPTR_(tertiary_handle, lptr);
						SetPPTR_(lptr, tertiary_handle);
						SetLPTR_(primary_handle, -1);
						SetPPTR_(primary_handle, -1);
					}
					else
					{
						if (rptr != -1)
						{
							SetRPTR_(tertiary_handle, rptr);
							SetPPTR_(rptr, tertiary_handle);
							SetRPTR_(primary_handle, -1);
							SetPPTR_(primary_handle, -1);
						}
						else
						{
							SetRPTR_(tertiary_handle, -1);
							SetPPTR_(primary_handle, -1);
						}
					}
				}
				iterations++;
				primary_handle = tertiary_handle;
				secondary_handle = GetSecondaryFromPrimary(primary_handle);
				size = (secondary_handle != -1 ? m_secondary_treaps.Size(secondary_handle) : 0);
				lptr = GetLPTR_(primary_handle);
				rptr = GetRPTR_(primary_handle);
				tertiary_handle = GetPPTR_(primary_handle);
			}
			System.Diagnostics.Debug.Assert((iterations <= 2));
		}

		// assert(check_validation_());
		/*
		* Resets the Interval_tree_impl to an empty state, but maintains a handle
		* on the current intervals.
		*/
		internal void Reset()
		{
			if (!m_b_offline_dynamic || !m_b_construction_ended)
			{
				throw new System.ArgumentException("invalid call");
			}
			Reset_(false);
		}

		/// <summary>Returns the number of intervals stored in the Interval_tree_impl</summary>
		internal int Size()
		{
			return m_c_count;
		}

		/// <summary>
		/// Gets an iterator on the Interval_tree_impl using the input Envelope_1D
		/// interval as the query.
		/// </summary>
		/// <remarks>
		/// Gets an iterator on the Interval_tree_impl using the input Envelope_1D
		/// interval as the query. To reuse the existing iterator on the same
		/// Interval_tree_impl but with a new query, use the reset_iterator function
		/// on the Interval_tree_iterator_impl. \param query The Envelope_1D interval
		/// used for the query. \param tolerance The tolerance used for the
		/// intersection tests.
		/// </remarks>
		internal com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl GetIterator(com.epl.geometry.Envelope1D query, double tolerance)
		{
			return new com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl(this, query, tolerance);
		}

		/// <summary>
		/// Gets an iterator on the Interval_tree_impl using the input double as the
		/// stabbing query.
		/// </summary>
		/// <remarks>
		/// Gets an iterator on the Interval_tree_impl using the input double as the
		/// stabbing query. To reuse the existing iterator on the same
		/// Interval_tree_impl but with a new query, use the reset_iterator function
		/// on the Interval_tree_iterator_impl. \param query The double used for the
		/// stabbing query. \param tolerance The tolerance used for the intersection
		/// tests.
		/// </remarks>
		internal com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl GetIterator(double query, double tolerance)
		{
			return new com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl(this, query, tolerance);
		}

		/// <summary>Gets an iterator on the Interval_tree_impl.</summary>
		internal com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl GetIterator()
		{
			return new com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl(this);
		}

		private sealed class SecondaryComparator : com.epl.geometry.Treap.Comparator
		{
			internal SecondaryComparator(com.epl.geometry.IntervalTreeImpl interval_tree)
			{
				m_interval_tree = interval_tree;
			}

			internal override int Compare(com.epl.geometry.Treap treap, int e_1, int node)
			{
				int e_2 = treap.GetElement(node);
				double v_1 = m_interval_tree.GetValue_(e_1);
				double v_2 = m_interval_tree.GetValue_(e_2);
				if (v_1 < v_2)
				{
					return -1;
				}
				if (v_1 == v_2)
				{
					if (IsLeft_(e_1) && IsRight_(e_2))
					{
						return -1;
					}
					if (IsLeft_(e_2) && IsRight_(e_1))
					{
						return 1;
					}
					return 0;
				}
				return 1;
			}

			private com.epl.geometry.IntervalTreeImpl m_interval_tree;
		}

		private bool m_b_offline_dynamic;

		private System.Collections.Generic.List<com.epl.geometry.Envelope1D> m_intervals;

		private com.epl.geometry.StridedIndexTypeCollection m_primary_nodes;

		private com.epl.geometry.StridedIndexTypeCollection m_interval_nodes;

		private com.epl.geometry.AttributeStreamOfInt32 m_interval_handles;

		private com.epl.geometry.IndexMultiDCList m_secondary_lists;

		private com.epl.geometry.Treap m_secondary_treaps;

		private com.epl.geometry.AttributeStreamOfInt32 m_end_indices_unique;

		private int m_c_count;

		private int m_root;

		private bool m_b_sort_intervals;

		private bool m_b_constructing;

		private bool m_b_construction_ended;

		// 8 elements for
		// offline dynamic case,
		// 7 elements for static
		// case
		// 3 elements
		// for offline dynamic
		// case
		// for static case
		// for off-line dynamic case
		// for both offline
		// dynamic and
		// static cases
		private void QuerySortedEndPointIndices_(com.epl.geometry.AttributeStreamOfInt32 end_indices)
		{
			int size = m_intervals.Count;
			for (int i = 0; i < 2 * size; i++)
			{
				end_indices.Add(i);
			}
			SortEndIndices_(end_indices, 0, 2 * size);
		}

		private void QuerySortedDuplicatesRemoved_(com.epl.geometry.AttributeStreamOfInt32 end_indices_sorted)
		{
			// remove duplicates
			double prev = com.epl.geometry.NumberUtils.TheNaN;
			for (int i = 0; i < end_indices_sorted.Size(); i++)
			{
				int e = end_indices_sorted.Get(i);
				double v = GetValue_(e);
				if (v != prev)
				{
					m_end_indices_unique.Add(e);
					prev = v;
				}
			}
		}

		private void InsertIntervalsStatic_()
		{
			int size = m_intervals.Count;
			System.Diagnostics.Debug.Assert((m_b_sort_intervals));
			// sort
			com.epl.geometry.AttributeStreamOfInt32 end_indices_sorted = new com.epl.geometry.AttributeStreamOfInt32(0);
			end_indices_sorted.Reserve(2 * size);
			QuerySortedEndPointIndices_(end_indices_sorted);
			// remove duplicates
			m_end_indices_unique.Reserve(2 * size);
			m_end_indices_unique.Resize(0);
			QuerySortedDuplicatesRemoved_(end_indices_sorted);
			System.Diagnostics.Debug.Assert((m_primary_nodes.Size() == 0));
			m_interval_nodes.SetCapacity(size);
			// one for each interval being
			// inserted. each element contains a
			// primary node, a left secondary
			// node, and a right secondary node.
			m_secondary_lists.ReserveNodes(2 * size);
			// one for each end point of
			// the original interval set
			// (not the unique set)
			com.epl.geometry.AttributeStreamOfInt32 interval_handles = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(size);
			interval_handles.SetRange(-1, 0, size);
			m_root = CreatePrimaryNode_();
			for (int i = 0; i < end_indices_sorted.Size(); i++)
			{
				int e = end_indices_sorted.Get(i);
				int interval_handle = interval_handles.Get(e >> 1);
				if (interval_handle != -1)
				{
					// insert the right end point
					System.Diagnostics.Debug.Assert((IsRight_(e)));
					int secondary_handle = GetSecondaryFromInterval_(interval_handle);
					SetRightEnd_(interval_handle, m_secondary_lists.AddElement(secondary_handle, e));
				}
				else
				{
					// insert the left end point
					System.Diagnostics.Debug.Assert((IsLeft_(e)));
					interval_handle = InsertIntervalEnd_(e, m_root);
					interval_handles.Set(e >> 1, interval_handle);
				}
			}
			System.Diagnostics.Debug.Assert((m_secondary_lists.GetNodeCount() == 2 * size));
		}

		private int InsertIntervalEnd_(int end_index, int root)
		{
			System.Diagnostics.Debug.Assert((IsLeft_(end_index)));
			int primary_handle = root;
			int tertiary_handle = root;
			int ptr = root;
			int secondary_handle;
			int interval_handle = -1;
			int il = 0;
			int ir = m_end_indices_unique.Size() - 1;
			int im = 0;
			int index = end_index >> 1;
			double discriminant_tertiary = com.epl.geometry.NumberUtils.TheNaN;
			double discriminant_ptr = com.epl.geometry.NumberUtils.TheNaN;
			bool bSearching = true;
			double min = GetMin_(index);
			double max = GetMax_(index);
			while (bSearching)
			{
				if (il < ir)
				{
					im = il + (ir - il) / 2;
					if (GetDiscriminantIndex1_(primary_handle) == -1)
					{
						SetDiscriminantIndices_(primary_handle, m_end_indices_unique.Get(im), m_end_indices_unique.Get(im + 1));
					}
				}
				else
				{
					System.Diagnostics.Debug.Assert((il == ir));
					System.Diagnostics.Debug.Assert((min == max));
					if (GetDiscriminantIndex1_(primary_handle) == -1)
					{
						SetDiscriminantIndices_(primary_handle, m_end_indices_unique.Get(il), m_end_indices_unique.Get(il));
					}
				}
				double discriminant = GetDiscriminant_(primary_handle);
				System.Diagnostics.Debug.Assert((!com.epl.geometry.NumberUtils.IsNaN(discriminant)));
				if (max < discriminant)
				{
					if (ptr != -1)
					{
						if (ptr == primary_handle)
						{
							tertiary_handle = primary_handle;
							discriminant_tertiary = discriminant;
							ptr = GetLPTR_(primary_handle);
							if (ptr != -1)
							{
								discriminant_ptr = GetDiscriminant_(ptr);
							}
							else
							{
								discriminant_ptr = com.epl.geometry.NumberUtils.TheNaN;
							}
						}
						else
						{
							if (discriminant_ptr > discriminant)
							{
								if (discriminant < discriminant_tertiary)
								{
									SetLPTR_(tertiary_handle, primary_handle);
								}
								else
								{
									SetRPTR_(tertiary_handle, primary_handle);
								}
								SetRPTR_(primary_handle, ptr);
								if (m_b_offline_dynamic)
								{
									SetPPTR_(primary_handle, tertiary_handle);
									SetPPTR_(ptr, primary_handle);
								}
								tertiary_handle = primary_handle;
								discriminant_tertiary = discriminant;
								ptr = -1;
								discriminant_ptr = com.epl.geometry.NumberUtils.TheNaN;
								System.Diagnostics.Debug.Assert((GetLPTR_(primary_handle) == -1));
								System.Diagnostics.Debug.Assert((GetRightPrimary_(primary_handle) != -1));
							}
						}
					}
					int left_handle = GetLeftPrimary_(primary_handle);
					if (left_handle == -1)
					{
						left_handle = CreatePrimaryNode_();
						SetLeftPrimary_(primary_handle, left_handle);
					}
					primary_handle = left_handle;
					ir = im;
					continue;
				}
				if (min > discriminant)
				{
					if (ptr != -1)
					{
						if (ptr == primary_handle)
						{
							tertiary_handle = primary_handle;
							discriminant_tertiary = discriminant;
							ptr = GetRPTR_(primary_handle);
							if (ptr != -1)
							{
								discriminant_ptr = GetDiscriminant_(ptr);
							}
							else
							{
								discriminant_ptr = com.epl.geometry.NumberUtils.TheNaN;
							}
						}
						else
						{
							if (discriminant_ptr < discriminant)
							{
								if (discriminant < discriminant_tertiary)
								{
									SetLPTR_(tertiary_handle, primary_handle);
								}
								else
								{
									SetRPTR_(tertiary_handle, primary_handle);
								}
								SetLPTR_(primary_handle, ptr);
								if (m_b_offline_dynamic)
								{
									SetPPTR_(primary_handle, tertiary_handle);
									SetPPTR_(ptr, primary_handle);
								}
								tertiary_handle = primary_handle;
								discriminant_tertiary = discriminant;
								ptr = -1;
								discriminant_ptr = com.epl.geometry.NumberUtils.TheNaN;
								System.Diagnostics.Debug.Assert((GetRPTR_(primary_handle) == -1));
								System.Diagnostics.Debug.Assert((GetLeftPrimary_(primary_handle) != -1));
							}
						}
					}
					int right_handle = GetRightPrimary_(primary_handle);
					if (right_handle == -1)
					{
						right_handle = CreatePrimaryNode_();
						SetRightPrimary_(primary_handle, right_handle);
					}
					primary_handle = right_handle;
					il = im + 1;
					continue;
				}
				secondary_handle = GetSecondaryFromPrimary(primary_handle);
				if (secondary_handle == -1)
				{
					secondary_handle = CreateSecondary_(primary_handle);
					SetSecondaryToPrimary_(primary_handle, secondary_handle);
				}
				int left_end_handle = AddEndIndex(secondary_handle, end_index);
				interval_handle = CreateIntervalNode_();
				SetSecondaryToInterval_(interval_handle, secondary_handle);
				SetLeftEnd_(interval_handle, left_end_handle);
				if (primary_handle != ptr)
				{
					System.Diagnostics.Debug.Assert((primary_handle != -1));
					System.Diagnostics.Debug.Assert((GetLPTR_(primary_handle) == -1 && GetRPTR_(primary_handle) == -1 && (!m_b_offline_dynamic || GetPPTR_(primary_handle) == -1)));
					if (discriminant < discriminant_tertiary)
					{
						SetLPTR_(tertiary_handle, primary_handle);
					}
					else
					{
						SetRPTR_(tertiary_handle, primary_handle);
					}
					if (m_b_offline_dynamic)
					{
						SetPPTR_(primary_handle, tertiary_handle);
					}
					if (ptr != -1)
					{
						if (discriminant_ptr < discriminant)
						{
							SetLPTR_(primary_handle, ptr);
						}
						else
						{
							SetRPTR_(primary_handle, ptr);
						}
						if (m_b_offline_dynamic)
						{
							SetPPTR_(ptr, primary_handle);
						}
					}
				}
				bSearching = false;
			}
			return interval_handle;
		}

		private int CreatePrimaryNode_()
		{
			return m_primary_nodes.NewElement();
		}

		private int CreateSecondary_(int primary_handle)
		{
			if (!m_b_offline_dynamic)
			{
				return m_secondary_lists.CreateList(primary_handle);
			}
			return m_secondary_treaps.CreateTreap(primary_handle);
		}

		private int CreateIntervalNode_()
		{
			return m_interval_nodes.NewElement();
		}

		private void Reset_(bool b_new_intervals)
		{
			if (b_new_intervals)
			{
				m_b_sort_intervals = true;
				m_b_constructing = true;
				m_b_construction_ended = false;
				if (m_end_indices_unique == null)
				{
					m_end_indices_unique = (com.epl.geometry.AttributeStreamOfInt32)(com.epl.geometry.AttributeStreamBase.CreateIndexStream(0));
				}
				else
				{
					m_end_indices_unique.Resize(0);
				}
				if (m_intervals == null)
				{
					m_intervals = new System.Collections.Generic.List<com.epl.geometry.Envelope1D>(0);
				}
				else
				{
					m_intervals.Clear();
				}
			}
			else
			{
				System.Diagnostics.Debug.Assert((m_b_offline_dynamic && m_b_construction_ended));
				m_b_sort_intervals = false;
			}
			if (m_b_offline_dynamic)
			{
				if (m_interval_handles == null)
				{
					m_interval_handles = (com.epl.geometry.AttributeStreamOfInt32)(com.epl.geometry.AttributeStreamBase.CreateIndexStream(0));
					m_secondary_treaps = new com.epl.geometry.Treap();
					m_secondary_treaps.SetComparator(new com.epl.geometry.IntervalTreeImpl.SecondaryComparator(this));
				}
				else
				{
					m_secondary_treaps.Clear();
				}
			}
			else
			{
				if (m_secondary_lists == null)
				{
					m_secondary_lists = new com.epl.geometry.IndexMultiDCList();
				}
				else
				{
					m_secondary_lists.Clear();
				}
			}
			if (m_primary_nodes == null)
			{
				m_interval_nodes = new com.epl.geometry.StridedIndexTypeCollection(3);
				m_primary_nodes = new com.epl.geometry.StridedIndexTypeCollection(m_b_offline_dynamic ? 8 : 7);
			}
			else
			{
				m_interval_nodes.DeleteAll(false);
				m_primary_nodes.DeleteAll(false);
			}
			m_root = -1;
			m_c_count = 0;
		}

		private void SetDiscriminantIndices_(int primary_handle, int e_1, int e_2)
		{
			SetDiscriminantIndex1_(primary_handle, e_1);
			SetDiscriminantIndex2_(primary_handle, e_2);
		}

		private double GetDiscriminant_(int primary_handle)
		{
			int e_1 = GetDiscriminantIndex1_(primary_handle);
			if (e_1 == -1)
			{
				return com.epl.geometry.NumberUtils.TheNaN;
			}
			int e_2 = GetDiscriminantIndex2_(primary_handle);
			System.Diagnostics.Debug.Assert((e_2 != -1));
			double v_1 = GetValue_(e_1);
			double v_2 = GetValue_(e_2);
			if (v_1 == v_2)
			{
				return v_1;
			}
			return 0.5 * (v_1 + v_2);
		}

		private bool IsActive_(int primary_handle)
		{
			int secondary_handle = GetSecondaryFromPrimary(primary_handle);
			if (secondary_handle != -1)
			{
				return true;
			}
			int left_handle = GetLeftPrimary_(primary_handle);
			if (left_handle == -1)
			{
				return false;
			}
			int right_handle = GetRightPrimary_(primary_handle);
			if (right_handle == -1)
			{
				return false;
			}
			return true;
		}

		private void SetDiscriminantIndex1_(int primary_handle, int end_index)
		{
			m_primary_nodes.SetField(primary_handle, 0, end_index);
		}

		private void SetDiscriminantIndex2_(int primary_handle, int end_index)
		{
			m_primary_nodes.SetField(primary_handle, 1, end_index);
		}

		private void SetLeftPrimary_(int primary_handle, int left_handle)
		{
			m_primary_nodes.SetField(primary_handle, 3, left_handle);
		}

		private void SetRightPrimary_(int primary_handle, int right_handle)
		{
			m_primary_nodes.SetField(primary_handle, 4, right_handle);
		}

		private void SetSecondaryToPrimary_(int primary_handle, int secondary_handle)
		{
			m_primary_nodes.SetField(primary_handle, 2, secondary_handle);
		}

		private void SetLPTR_(int primary_handle, int lptr)
		{
			m_primary_nodes.SetField(primary_handle, 5, lptr);
		}

		private void SetRPTR_(int primary_handle, int rptr)
		{
			m_primary_nodes.SetField(primary_handle, 6, rptr);
		}

		private void SetPPTR_(int primary_handle, int pptr)
		{
			m_primary_nodes.SetField(primary_handle, 7, pptr);
		}

		private void SetSecondaryToInterval_(int interval_handle, int secondary_handle)
		{
			m_interval_nodes.SetField(interval_handle, 0, secondary_handle);
		}

		private int AddEndIndex(int secondary_handle, int end_index)
		{
			int end_index_handle;
			if (!m_b_offline_dynamic)
			{
				end_index_handle = m_secondary_lists.AddElement(secondary_handle, end_index);
			}
			else
			{
				end_index_handle = m_secondary_treaps.AddElement(end_index, secondary_handle);
			}
			return end_index_handle;
		}

		private void SetLeftEnd_(int interval_handle, int left_end_handle)
		{
			m_interval_nodes.SetField(interval_handle, 1, left_end_handle);
		}

		private void SetRightEnd_(int interval_handle, int right_end_handle)
		{
			m_interval_nodes.SetField(interval_handle, 2, right_end_handle);
		}

		private int GetFirst_(int secondary_handle)
		{
			if (!m_b_offline_dynamic)
			{
				return m_secondary_lists.GetFirst(secondary_handle);
			}
			return m_secondary_treaps.GetFirst(secondary_handle);
		}

		private int GetLast_(int secondary_handle)
		{
			if (!m_b_offline_dynamic)
			{
				return m_secondary_lists.GetLast(secondary_handle);
			}
			return m_secondary_treaps.GetLast(secondary_handle);
		}

		private static bool IsLeft_(int end_index)
		{
			return (end_index & unchecked((int)(0x1))) == 0;
		}

		private static bool IsRight_(int end_index)
		{
			return (end_index & unchecked((int)(0x1))) == 1;
		}

		private int GetDiscriminantIndex1_(int primary_handle)
		{
			return m_primary_nodes.GetField(primary_handle, 0);
		}

		private int GetDiscriminantIndex2_(int primary_handle)
		{
			return m_primary_nodes.GetField(primary_handle, 1);
		}

		private int GetSecondaryFromPrimary(int primary_handle)
		{
			return m_primary_nodes.GetField(primary_handle, 2);
		}

		private int GetLeftPrimary_(int primary_handle)
		{
			return m_primary_nodes.GetField(primary_handle, 3);
		}

		private int GetRightPrimary_(int primary_handle)
		{
			return m_primary_nodes.GetField(primary_handle, 4);
		}

		private int GetLPTR_(int primary_handle)
		{
			return m_primary_nodes.GetField(primary_handle, 5);
		}

		private int GetRPTR_(int primary_handle)
		{
			return m_primary_nodes.GetField(primary_handle, 6);
		}

		private int GetPPTR_(int primary_handle)
		{
			return m_primary_nodes.GetField(primary_handle, 7);
		}

		private int GetSecondaryFromInterval_(int interval_handle)
		{
			return m_interval_nodes.GetField(interval_handle, 0);
		}

		private int GetLeftEnd_(int interval_handle)
		{
			return m_interval_nodes.GetField(interval_handle, 1);
		}

		private int GetRightEnd_(int interval_handle)
		{
			return m_interval_nodes.GetField(interval_handle, 2);
		}

		private double GetMin_(int i)
		{
			com.epl.geometry.Envelope1D interval = m_intervals[i];
			return interval.vmin;
		}

		private double GetMax_(int i)
		{
			com.epl.geometry.Envelope1D interval = m_intervals[i];
			return interval.vmax;
		}

		private com.epl.geometry.BucketSort m_bucket_sort;

		// *********** Helpers for Bucket sort**************
		private void SortEndIndices_(com.epl.geometry.AttributeStreamOfInt32 end_indices, int begin_, int end_)
		{
			if (m_bucket_sort == null)
			{
				m_bucket_sort = new com.epl.geometry.BucketSort();
			}
			com.epl.geometry.IntervalTreeImpl.IntervalTreeBucketSortHelper sorter = new com.epl.geometry.IntervalTreeImpl.IntervalTreeBucketSortHelper(this, this);
			m_bucket_sort.Sort(end_indices, begin_, end_, sorter);
		}

		private void SortEndIndicesHelper_(com.epl.geometry.AttributeStreamOfInt32 end_indices, int begin_, int end_)
		{
			end_indices.Sort(begin_, end_, new com.epl.geometry.IntervalTreeImpl.EndPointsComparer(this));
		}

		private double GetValue_(int e)
		{
			com.epl.geometry.Envelope1D interval = m_intervals[e >> 1];
			double v = (IsLeft_(e) ? interval.vmin : interval.vmax);
			return v;
		}

		private sealed class EndPointsComparer : com.epl.geometry.AttributeStreamOfInt32.IntComparator
		{
			internal EndPointsComparer(com.epl.geometry.IntervalTreeImpl interval_tree)
			{
				// For user sort
				m_interval_tree = interval_tree;
			}

			public override int Compare(int e_1, int e_2)
			{
				double v_1 = m_interval_tree.GetValue_(e_1);
				double v_2 = m_interval_tree.GetValue_(e_2);
				if (v_1 < v_2 || (v_1 == v_2 && IsLeft_(e_1) && IsRight_(e_2)))
				{
					return -1;
				}
				return 1;
			}

			private com.epl.geometry.IntervalTreeImpl m_interval_tree;
		}

		private class IntervalTreeBucketSortHelper : com.epl.geometry.ClassicSort
		{
			internal IntervalTreeBucketSortHelper(IntervalTreeImpl _enclosing, com.epl.geometry.IntervalTreeImpl interval_tree)
			{
				this._enclosing = _enclosing;
				// For
				// bucket
				// sort
				this.m_interval_tree = interval_tree;
			}

			public override void UserSort(int begin, int end, com.epl.geometry.AttributeStreamOfInt32 indices)
			{
				this.m_interval_tree.SortEndIndicesHelper_(indices, begin, end);
			}

			public override double GetValue(int e)
			{
				return this.m_interval_tree.GetValue_(e);
			}

			private com.epl.geometry.IntervalTreeImpl m_interval_tree;

			private readonly IntervalTreeImpl _enclosing;
		}
	}
}
