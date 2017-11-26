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
		private void SortEndIndices_(com.epl.geometry.AttributeStreamOfInt32 end_indices, int begin_, int end_)
		{
			com.epl.geometry.IntervalTreeImpl.IntervalTreeBucketSortHelper sorter = new com.epl.geometry.IntervalTreeImpl.IntervalTreeBucketSortHelper(this, this);
			com.epl.geometry.BucketSort bucket_sort = new com.epl.geometry.BucketSort();
			bucket_sort.Sort(end_indices, begin_, end_, sorter);
		}

		private void SortEndIndicesHelper_(com.epl.geometry.AttributeStreamOfInt32 end_indices, int begin_, int end_)
		{
			end_indices.Sort(begin_, end_, new com.epl.geometry.IntervalTreeImpl.EndPointsComparer(this));
		}

		private double GetValue_(int e)
		{
			if (!m_b_envelopes_ref)
			{
				com.epl.geometry.Envelope1D interval = m_intervals[e >> 1];
				double v = (IsLeft_(e) ? interval.vmin : interval.vmax);
				return v;
			}
			com.epl.geometry.Envelope2D interval_1 = m_envelopes_ref[e >> 1];
			double v_1 = (IsLeft_(e) ? interval_1.xmin : interval_1.xmax);
			return v_1;
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
				// For bucket sort
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

		internal IntervalTreeImpl(bool b_offline_dynamic)
		{
			m_b_envelopes_ref = false;
			m_b_offline_dynamic = b_offline_dynamic;
			m_b_constructing = false;
			m_b_construction_ended = false;
		}

		internal void AddEnvelopesRef(System.Collections.Generic.List<com.epl.geometry.Envelope2D> envelopes)
		{
			Reset_(true, true);
			m_b_envelopes_ref = true;
			m_envelopes_ref = envelopes;
			m_b_constructing = false;
			m_b_construction_ended = true;
			if (!m_b_offline_dynamic)
			{
				InsertIntervalsStatic_();
				m_c_count = m_envelopes_ref.Count;
			}
		}

		internal void StartConstruction()
		{
			Reset_(true, false);
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
			Reset_(false, m_b_envelopes_ref);
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

		private bool m_b_envelopes_ref;

		private bool m_b_offline_dynamic;

		private System.Collections.Generic.List<com.epl.geometry.Envelope1D> m_intervals;

		private System.Collections.Generic.List<com.epl.geometry.Envelope2D> m_envelopes_ref;

		private com.epl.geometry.StridedIndexTypeCollection m_tertiary_nodes;

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

		// 5 elements for offline dynamic case, 4 elements for static case
		// 3 elements
		// for offline dynamic// case
		// for static case
		// for off-line dynamic case
		// for both offline dynamic and static cases
		/* m_tertiary_nodes
		* 0: m_discriminant_index_1
		* 1: m_secondary
		* 2: m_lptr
		* 3: m_rptr
		* 4: m_pptr
		*/
		private void QuerySortedEndPointIndices_(com.epl.geometry.AttributeStreamOfInt32 end_indices)
		{
			int size = (!m_b_envelopes_ref ? m_intervals.Count : m_envelopes_ref.Count);
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

		internal void Insert(int index)
		{
			if (!m_b_offline_dynamic || !m_b_construction_ended)
			{
				throw new System.ArgumentException("invalid call");
			}
			if (m_root == -1)
			{
				int size = (!m_b_envelopes_ref ? m_intervals.Count : m_envelopes_ref.Count);
				if (m_b_sort_intervals)
				{
					// sort
					com.epl.geometry.AttributeStreamOfInt32 end_point_indices_sorted = new com.epl.geometry.AttributeStreamOfInt32(0);
					end_point_indices_sorted.Reserve(2 * size);
					QuerySortedEndPointIndices_(end_point_indices_sorted);
					// remove duplicates
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
				m_root = CreateRoot_();
			}
			int interval_handle = InsertIntervalEnd_(index << 1, m_root);
			int secondary_handle = GetSecondaryFromInterval_(interval_handle);
			int right_end_handle = m_secondary_treaps.AddElement((index << 1) + 1, secondary_handle);
			SetRightEnd_(interval_handle, right_end_handle);
			m_interval_handles.Set(index, interval_handle);
			m_c_count++;
		}

		// assert(check_validation_());
		private void InsertIntervalsStatic_()
		{
			int size = (!m_b_envelopes_ref ? m_intervals.Count : m_envelopes_ref.Count);
			System.Diagnostics.Debug.Assert((m_b_sort_intervals));
			// sort
			com.epl.geometry.AttributeStreamOfInt32 end_indices_sorted = new com.epl.geometry.AttributeStreamOfInt32(0);
			end_indices_sorted.Reserve(2 * size);
			QuerySortedEndPointIndices_(end_indices_sorted);
			// remove duplicates
			m_end_indices_unique.Resize(0);
			QuerySortedDuplicatesRemoved_(end_indices_sorted);
			System.Diagnostics.Debug.Assert((m_tertiary_nodes.Size() == 0));
			m_interval_nodes.SetCapacity(size);
			// one for each interval being inserted. each element contains a tertiary node, a left secondary node, and a right secondary node.
			m_secondary_lists.ReserveNodes(2 * size);
			// one for each end point of the original interval set (not the unique set)
			com.epl.geometry.AttributeStreamOfInt32 interval_handles = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(size);
			interval_handles.SetRange(-1, 0, size);
			m_root = CreateRoot_();
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

		private int CreateRoot_()
		{
			int discriminant_index_1 = CalculateDiscriminantIndex1_(0, m_end_indices_unique.Size() - 1);
			return CreateTertiaryNode_(discriminant_index_1);
		}

		private int InsertIntervalEnd_(int end_index, int root)
		{
			System.Diagnostics.Debug.Assert((IsLeft_(end_index)));
			int pptr = -1;
			int ptr = root;
			int secondary_handle = -1;
			int interval_handle = -1;
			int il = 0;
			int ir = m_end_indices_unique.Size() - 1;
			int im = 0;
			int index = end_index >> 1;
			double discriminant_pptr = com.epl.geometry.NumberUtils.NaN();
			double discriminant_ptr = com.epl.geometry.NumberUtils.NaN();
			bool bSearching = true;
			double min = GetMin_(index);
			double max = GetMax_(index);
			int discriminant_index_1 = -1;
			while (bSearching)
			{
				im = il + (ir - il) / 2;
				System.Diagnostics.Debug.Assert((il != ir || min == max));
				discriminant_index_1 = CalculateDiscriminantIndex1_(il, ir);
				double discriminant = GetDiscriminantFromIndex1_(discriminant_index_1);
				System.Diagnostics.Debug.Assert((!com.epl.geometry.NumberUtils.IsNaN(discriminant)));
				if (max < discriminant)
				{
					if (ptr != -1)
					{
						if (discriminant_index_1 == GetDiscriminantIndex1_(ptr))
						{
							System.Diagnostics.Debug.Assert((GetDiscriminantFromIndex1_(discriminant_index_1) == GetDiscriminant_(ptr)));
							pptr = ptr;
							discriminant_pptr = discriminant;
							ptr = GetLPTR_(ptr);
							if (ptr != -1)
							{
								discriminant_ptr = GetDiscriminant_(ptr);
							}
							else
							{
								discriminant_ptr = com.epl.geometry.NumberUtils.NaN();
							}
						}
						else
						{
							if (discriminant_ptr > discriminant)
							{
								int tertiary_handle = CreateTertiaryNode_(discriminant_index_1);
								if (discriminant < discriminant_pptr)
								{
									SetLPTR_(pptr, tertiary_handle);
								}
								else
								{
									SetRPTR_(pptr, tertiary_handle);
								}
								SetRPTR_(tertiary_handle, ptr);
								if (m_b_offline_dynamic)
								{
									SetPPTR_(tertiary_handle, pptr);
									SetPPTR_(ptr, tertiary_handle);
								}
								pptr = tertiary_handle;
								discriminant_pptr = discriminant;
								ptr = -1;
								discriminant_ptr = com.epl.geometry.NumberUtils.NaN();
							}
						}
					}
					ir = im;
					continue;
				}
				if (min > discriminant)
				{
					if (ptr != -1)
					{
						if (discriminant_index_1 == GetDiscriminantIndex1_(ptr))
						{
							System.Diagnostics.Debug.Assert((GetDiscriminantFromIndex1_(discriminant_index_1) == GetDiscriminant_(ptr)));
							pptr = ptr;
							discriminant_pptr = discriminant;
							ptr = GetRPTR_(ptr);
							if (ptr != -1)
							{
								discriminant_ptr = GetDiscriminant_(ptr);
							}
							else
							{
								discriminant_ptr = com.epl.geometry.NumberUtils.NaN();
							}
						}
						else
						{
							if (discriminant_ptr < discriminant)
							{
								int tertiary_handle = CreateTertiaryNode_(discriminant_index_1);
								if (discriminant < discriminant_pptr)
								{
									SetLPTR_(pptr, tertiary_handle);
								}
								else
								{
									SetRPTR_(pptr, tertiary_handle);
								}
								SetLPTR_(tertiary_handle, ptr);
								if (m_b_offline_dynamic)
								{
									SetPPTR_(tertiary_handle, pptr);
									SetPPTR_(ptr, tertiary_handle);
								}
								pptr = tertiary_handle;
								discriminant_pptr = discriminant;
								ptr = -1;
								discriminant_ptr = com.epl.geometry.NumberUtils.NaN();
							}
						}
					}
					il = im + 1;
					continue;
				}
				int tertiary_handle_1 = -1;
				if (ptr == -1 || discriminant_index_1 != GetDiscriminantIndex1_(ptr))
				{
					tertiary_handle_1 = CreateTertiaryNode_(discriminant_index_1);
				}
				else
				{
					tertiary_handle_1 = ptr;
				}
				secondary_handle = GetSecondaryFromTertiary_(tertiary_handle_1);
				if (secondary_handle == -1)
				{
					secondary_handle = CreateSecondary_(tertiary_handle_1);
					SetSecondaryToTertiary_(tertiary_handle_1, secondary_handle);
				}
				int left_end_handle = AddEndIndex_(secondary_handle, end_index);
				interval_handle = CreateIntervalNode_();
				SetSecondaryToInterval_(interval_handle, secondary_handle);
				SetLeftEnd_(interval_handle, left_end_handle);
				if (ptr == -1 || discriminant_index_1 != GetDiscriminantIndex1_(ptr))
				{
					System.Diagnostics.Debug.Assert((tertiary_handle_1 != -1));
					System.Diagnostics.Debug.Assert((GetLPTR_(tertiary_handle_1) == -1 && GetRPTR_(tertiary_handle_1) == -1 && (!m_b_offline_dynamic || GetPPTR_(tertiary_handle_1) == -1)));
					if (discriminant < discriminant_pptr)
					{
						SetLPTR_(pptr, tertiary_handle_1);
					}
					else
					{
						SetRPTR_(pptr, tertiary_handle_1);
					}
					if (m_b_offline_dynamic)
					{
						SetPPTR_(tertiary_handle_1, pptr);
					}
					if (ptr != -1)
					{
						if (discriminant_ptr < discriminant)
						{
							SetLPTR_(tertiary_handle_1, ptr);
						}
						else
						{
							SetRPTR_(tertiary_handle_1, ptr);
						}
						if (m_b_offline_dynamic)
						{
							SetPPTR_(ptr, tertiary_handle_1);
						}
					}
				}
				bSearching = false;
				break;
			}
			return interval_handle;
		}

		internal void Remove(int index)
		{
			if (!m_b_offline_dynamic || !m_b_construction_ended)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			int interval_handle = m_interval_handles.Get(index);
			if (interval_handle == -1)
			{
				throw new com.epl.geometry.GeometryException("the interval does not exist in the interval tree");
			}
			m_interval_handles.Set(index, -1);
			System.Diagnostics.Debug.Assert((GetSecondaryFromInterval_(interval_handle) != -1));
			System.Diagnostics.Debug.Assert((GetLeftEnd_(interval_handle) != -1));
			System.Diagnostics.Debug.Assert((GetRightEnd_(interval_handle) != -1));
			m_c_count--;
			int size;
			int secondary_handle = GetSecondaryFromInterval_(interval_handle);
			int tertiary_handle = -1;
			tertiary_handle = m_secondary_treaps.GetTreapData(secondary_handle);
			m_secondary_treaps.DeleteNode(GetLeftEnd_(interval_handle), secondary_handle);
			m_secondary_treaps.DeleteNode(GetRightEnd_(interval_handle), secondary_handle);
			size = m_secondary_treaps.Size(secondary_handle);
			if (size == 0)
			{
				m_secondary_treaps.DeleteTreap(secondary_handle);
				SetSecondaryToTertiary_(tertiary_handle, -1);
			}
			m_interval_nodes.DeleteElement(interval_handle);
			int pptr = GetPPTR_(tertiary_handle);
			int lptr = GetLPTR_(tertiary_handle);
			int rptr = GetRPTR_(tertiary_handle);
			int iterations = 0;
			while (!(size > 0 || tertiary_handle == m_root || (lptr != -1 && rptr != -1)))
			{
				System.Diagnostics.Debug.Assert((size == 0));
				System.Diagnostics.Debug.Assert((lptr == -1 || rptr == -1));
				System.Diagnostics.Debug.Assert((tertiary_handle != 0));
				if (tertiary_handle == GetLPTR_(pptr))
				{
					if (lptr != -1)
					{
						SetLPTR_(pptr, lptr);
						SetPPTR_(lptr, pptr);
						SetLPTR_(tertiary_handle, -1);
						SetPPTR_(tertiary_handle, -1);
					}
					else
					{
						if (rptr != -1)
						{
							SetLPTR_(pptr, rptr);
							SetPPTR_(rptr, pptr);
							SetRPTR_(tertiary_handle, -1);
							SetPPTR_(tertiary_handle, -1);
						}
						else
						{
							SetLPTR_(pptr, -1);
							SetPPTR_(tertiary_handle, -1);
						}
					}
				}
				else
				{
					if (lptr != -1)
					{
						SetRPTR_(pptr, lptr);
						SetPPTR_(lptr, pptr);
						SetLPTR_(tertiary_handle, -1);
						SetPPTR_(tertiary_handle, -1);
					}
					else
					{
						if (rptr != -1)
						{
							SetRPTR_(pptr, rptr);
							SetPPTR_(rptr, pptr);
							SetRPTR_(tertiary_handle, -1);
							SetPPTR_(tertiary_handle, -1);
						}
						else
						{
							SetRPTR_(pptr, -1);
							SetPPTR_(tertiary_handle, -1);
						}
					}
				}
				m_tertiary_nodes.DeleteElement(tertiary_handle);
				iterations++;
				tertiary_handle = pptr;
				secondary_handle = GetSecondaryFromTertiary_(tertiary_handle);
				size = (secondary_handle != -1 ? m_secondary_treaps.Size(secondary_handle) : 0);
				lptr = GetLPTR_(tertiary_handle);
				rptr = GetRPTR_(tertiary_handle);
				pptr = GetPPTR_(tertiary_handle);
			}
			System.Diagnostics.Debug.Assert((iterations <= 2));
		}

		//assert(check_validation_());
		private void Reset_(bool b_new_intervals, bool b_envelopes_ref)
		{
			if (b_new_intervals)
			{
				m_b_envelopes_ref = false;
				m_envelopes_ref = null;
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
				if (!b_envelopes_ref)
				{
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
					if (m_intervals != null)
					{
						m_intervals.Clear();
					}
					m_b_envelopes_ref = true;
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
			if (m_tertiary_nodes == null)
			{
				m_interval_nodes = new com.epl.geometry.StridedIndexTypeCollection(3);
				m_tertiary_nodes = new com.epl.geometry.StridedIndexTypeCollection(m_b_offline_dynamic ? 5 : 4);
			}
			else
			{
				m_interval_nodes.DeleteAll(false);
				m_tertiary_nodes.DeleteAll(false);
			}
			m_root = -1;
			m_c_count = 0;
		}

		private double GetDiscriminant_(int tertiary_handle)
		{
			int discriminant_index_1 = GetDiscriminantIndex1_(tertiary_handle);
			return GetDiscriminantFromIndex1_(discriminant_index_1);
		}

		private double GetDiscriminantFromIndex1_(int discriminant_index_1)
		{
			if (discriminant_index_1 == -1)
			{
				return com.epl.geometry.NumberUtils.NaN();
			}
			if (discriminant_index_1 > 0)
			{
				int j = discriminant_index_1 - 2;
				int e_1 = m_end_indices_unique.Get(j);
				int e_2 = m_end_indices_unique.Get(j + 1);
				double v_1 = GetValue_(e_1);
				double v_2 = GetValue_(e_2);
				System.Diagnostics.Debug.Assert((v_1 < v_2));
				return 0.5 * (v_1 + v_2);
			}
			int j_1 = -discriminant_index_1 - 2;
			System.Diagnostics.Debug.Assert((j_1 >= 0));
			int e = m_end_indices_unique.Get(j_1);
			double v = GetValue_(e);
			return v;
		}

		private int CalculateDiscriminantIndex1_(int il, int ir)
		{
			int discriminant_index_1;
			if (il < ir)
			{
				int im = il + (ir - il) / 2;
				discriminant_index_1 = im + 2;
			}
			else
			{
				// positive discriminant means use average of im and im + 1
				discriminant_index_1 = -(il + 2);
			}
			// negative discriminant just means use il (-(il + 2) will never be -1)
			return discriminant_index_1;
		}

		internal sealed class IntervalTreeIteratorImpl
		{
			private com.epl.geometry.IntervalTreeImpl m_interval_tree;

			private com.epl.geometry.Envelope1D m_query = new com.epl.geometry.Envelope1D();

			private int m_tertiary_handle;

			private int m_next_tertiary_handle;

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

			private bool Initialize_()
			{
				m_tertiary_handle = -1;
				m_next_tertiary_handle = -1;
				m_forked_handle = -1;
				m_current_end_handle = -1;
				if (m_interval_tree.m_tertiary_nodes != null && m_interval_tree.m_tertiary_nodes.Size() > 0)
				{
					m_function_stack[0] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.pIn;
					// overwrite initialize
					m_next_tertiary_handle = m_interval_tree.m_root;
					return true;
				}
				m_function_index = -1;
				return false;
			}

			private bool PIn_()
			{
				m_tertiary_handle = m_next_tertiary_handle;
				if (m_tertiary_handle == -1)
				{
					m_function_index = -1;
					m_current_end_handle = -1;
					return false;
				}
				double discriminant = m_interval_tree.GetDiscriminant_(m_tertiary_handle);
				if (m_query.vmax < discriminant)
				{
					int secondary_handle = m_interval_tree.GetSecondaryFromTertiary_(m_tertiary_handle);
					m_next_tertiary_handle = m_interval_tree.GetLPTR_(m_tertiary_handle);
					if (secondary_handle != -1)
					{
						m_next_end_handle = m_interval_tree.GetFirst_(secondary_handle);
						m_function_stack[++m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.left;
					}
					return true;
				}
				if (discriminant < m_query.vmin)
				{
					int secondary_handle = m_interval_tree.GetSecondaryFromTertiary_(m_tertiary_handle);
					m_next_tertiary_handle = m_interval_tree.GetRPTR_(m_tertiary_handle);
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
				m_forked_handle = m_tertiary_handle;
				int secondary_handle_1 = m_interval_tree.GetSecondaryFromTertiary_(m_tertiary_handle);
				m_next_tertiary_handle = m_interval_tree.GetLPTR_(m_tertiary_handle);
				if (secondary_handle_1 != -1)
				{
					m_next_end_handle = m_interval_tree.GetFirst_(secondary_handle_1);
					m_function_stack[++m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.all;
				}
				return true;
			}

			private bool PL_()
			{
				m_tertiary_handle = m_next_tertiary_handle;
				if (m_tertiary_handle == -1)
				{
					m_function_stack[m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.pR;
					// overwrite pL
					m_next_tertiary_handle = m_interval_tree.GetRPTR_(m_forked_handle);
					return true;
				}
				double discriminant = m_interval_tree.GetDiscriminant_(m_tertiary_handle);
				if (discriminant < m_query.vmin)
				{
					int secondary_handle = m_interval_tree.GetSecondaryFromTertiary_(m_tertiary_handle);
					m_next_tertiary_handle = m_interval_tree.GetRPTR_(m_tertiary_handle);
					if (secondary_handle != -1)
					{
						m_next_end_handle = m_interval_tree.GetLast_(secondary_handle);
						m_function_stack[++m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.right;
					}
					return true;
				}
				System.Diagnostics.Debug.Assert((m_query.Contains(discriminant)));
				int secondary_handle_1 = m_interval_tree.GetSecondaryFromTertiary_(m_tertiary_handle);
				m_next_tertiary_handle = m_interval_tree.GetLPTR_(m_tertiary_handle);
				if (secondary_handle_1 != -1)
				{
					m_next_end_handle = m_interval_tree.GetFirst_(secondary_handle_1);
					m_function_stack[++m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.all;
				}
				int rptr = m_interval_tree.GetRPTR_(m_tertiary_handle);
				if (rptr != -1)
				{
					m_tertiary_stack.Add(rptr);
				}
				// we'll search this in the pT state
				return true;
			}

			private bool PR_()
			{
				m_tertiary_handle = m_next_tertiary_handle;
				if (m_tertiary_handle == -1)
				{
					m_function_stack[m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.pT;
					// overwrite pR
					return true;
				}
				double discriminant = m_interval_tree.GetDiscriminant_(m_tertiary_handle);
				if (m_query.vmax < discriminant)
				{
					int secondary_handle = m_interval_tree.GetSecondaryFromTertiary_(m_tertiary_handle);
					m_next_tertiary_handle = m_interval_tree.GetLPTR_(m_tertiary_handle);
					if (secondary_handle != -1)
					{
						m_next_end_handle = m_interval_tree.GetFirst_(secondary_handle);
						m_function_stack[++m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.left;
					}
					return true;
				}
				System.Diagnostics.Debug.Assert((m_query.Contains(discriminant)));
				int secondary_handle_1 = m_interval_tree.GetSecondaryFromTertiary_(m_tertiary_handle);
				m_next_tertiary_handle = m_interval_tree.GetRPTR_(m_tertiary_handle);
				if (secondary_handle_1 != -1)
				{
					m_next_end_handle = m_interval_tree.GetFirst_(secondary_handle_1);
					m_function_stack[++m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.all;
				}
				int lptr = m_interval_tree.GetLPTR_(m_tertiary_handle);
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
				m_tertiary_handle = m_tertiary_stack.Get(m_tertiary_stack.Size() - 1);
				m_tertiary_stack.Resize(m_tertiary_stack.Size() - 1);
				int secondary_handle = m_interval_tree.GetSecondaryFromTertiary_(m_tertiary_handle);
				if (secondary_handle != -1)
				{
					m_next_end_handle = m_interval_tree.GetFirst_(secondary_handle);
					m_function_stack[++m_function_index] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.all;
				}
				if (m_interval_tree.GetLPTR_(m_tertiary_handle) != -1)
				{
					m_tertiary_stack.Add(m_interval_tree.GetLPTR_(m_tertiary_handle));
				}
				if (m_interval_tree.GetRPTR_(m_tertiary_handle) != -1)
				{
					m_tertiary_stack.Add(m_interval_tree.GetRPTR_(m_tertiary_handle));
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

			internal IntervalTreeIteratorImpl(com.epl.geometry.IntervalTreeImpl interval_tree, com.epl.geometry.Envelope1D query, double tolerance)
			{
				m_interval_tree = interval_tree;
				m_tertiary_stack.Reserve(20);
				ResetIterator(query, tolerance);
			}

			internal IntervalTreeIteratorImpl(com.epl.geometry.IntervalTreeImpl interval_tree, double query, double tolerance)
			{
				m_interval_tree = interval_tree;
				m_tertiary_stack.Reserve(20);
				ResetIterator(query, tolerance);
			}

			internal IntervalTreeIteratorImpl(com.epl.geometry.IntervalTreeImpl interval_tree)
			{
				m_interval_tree = interval_tree;
				m_tertiary_stack.Reserve(20);
				m_function_index = -1;
			}

			internal void ResetIterator(com.epl.geometry.Envelope1D query, double tolerance)
			{
				m_query.vmin = query.vmin - tolerance;
				m_query.vmax = query.vmax + tolerance;
				m_tertiary_stack.Resize(0);
				m_function_index = 0;
				m_function_stack[0] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.initialize;
			}

			internal void ResetIterator(double query_min, double query_max, double tolerance)
			{
				m_query.vmin = query_min - tolerance;
				m_query.vmax = query_max + tolerance;
				m_tertiary_stack.Resize(0);
				m_function_index = 0;
				m_function_stack[0] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.initialize;
			}

			internal void ResetIterator(double query, double tolerance)
			{
				m_query.vmin = query - tolerance;
				m_query.vmax = query + tolerance;
				m_tertiary_stack.Resize(0);
				m_function_index = 0;
				m_function_stack[0] = com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl.State.initialize;
			}
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

		private int CreateTertiaryNode_(int discriminant_index_1)
		{
			int tertiary_handle = m_tertiary_nodes.NewElement();
			SetDiscriminantIndex1_(tertiary_handle, discriminant_index_1);
			return tertiary_handle;
		}

		private int CreateSecondary_(int tertiary_handle)
		{
			if (!m_b_offline_dynamic)
			{
				return m_secondary_lists.CreateList(tertiary_handle);
			}
			return m_secondary_treaps.CreateTreap(tertiary_handle);
		}

		private int CreateIntervalNode_()
		{
			return m_interval_nodes.NewElement();
		}

		private void SetDiscriminantIndex1_(int tertiary_handle, int end_index)
		{
			m_tertiary_nodes.SetField(tertiary_handle, 0, end_index);
		}

		private void SetSecondaryToTertiary_(int tertiary_handle, int secondary_handle)
		{
			m_tertiary_nodes.SetField(tertiary_handle, 1, secondary_handle);
		}

		private void SetLPTR_(int tertiary_handle, int lptr)
		{
			m_tertiary_nodes.SetField(tertiary_handle, 2, lptr);
		}

		private void SetRPTR_(int tertiary_handle, int rptr)
		{
			m_tertiary_nodes.SetField(tertiary_handle, 3, rptr);
		}

		private void SetPPTR_(int tertiary_handle, int pptr)
		{
			m_tertiary_nodes.SetField(tertiary_handle, 4, pptr);
		}

		private void SetSecondaryToInterval_(int interval_handle, int secondary_handle)
		{
			m_interval_nodes.SetField(interval_handle, 0, secondary_handle);
		}

		private int AddEndIndex_(int secondary_handle, int end_index)
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

		private int GetDiscriminantIndex1_(int tertiary_handle)
		{
			return m_tertiary_nodes.GetField(tertiary_handle, 0);
		}

		private int GetSecondaryFromTertiary_(int tertiary_handle)
		{
			return m_tertiary_nodes.GetField(tertiary_handle, 1);
		}

		private int GetLPTR_(int tertiary_handle)
		{
			return m_tertiary_nodes.GetField(tertiary_handle, 2);
		}

		private int GetRPTR_(int tertiary_handle)
		{
			return m_tertiary_nodes.GetField(tertiary_handle, 3);
		}

		private int GetPPTR_(int tertiary_handle)
		{
			return m_tertiary_nodes.GetField(tertiary_handle, 4);
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
			return (!m_b_envelopes_ref ? m_intervals[i].vmin : m_envelopes_ref[i].xmin);
		}

		private double GetMax_(int i)
		{
			return (!m_b_envelopes_ref ? m_intervals[i].vmax : m_envelopes_ref[i].xmax);
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
	}
}
