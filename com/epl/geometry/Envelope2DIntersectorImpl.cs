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
	internal class Envelope2DIntersectorImpl
	{
		internal Envelope2DIntersectorImpl()
		{
			/*
			* Constructor for Envelope_2D_intersector.
			*/
			m_function = -1;
			m_tolerance = 0.0;
			Reset_();
		}

		internal virtual void StartConstruction()
		{
			Reset_();
			m_b_add_red_red = true;
			if (m_envelopes_red == null)
			{
				m_elements_red = new com.epl.geometry.AttributeStreamOfInt32(0);
				m_envelopes_red = new System.Collections.Generic.List<com.epl.geometry.Envelope2D>(0);
			}
			else
			{
				m_elements_red.ResizePreserveCapacity(0);
				m_envelopes_red.Clear();
			}
		}

		internal virtual void AddEnvelope(int element, com.epl.geometry.Envelope2D envelope)
		{
			if (!m_b_add_red_red)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			com.epl.geometry.Envelope2D e = new com.epl.geometry.Envelope2D();
			e.SetCoords(envelope);
			m_elements_red.Add(element);
			m_envelopes_red.Add(e);
		}

		internal virtual void EndConstruction()
		{
			if (!m_b_add_red_red)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			m_b_add_red_red = false;
			if (m_envelopes_red != null && m_envelopes_red.Count > 0)
			{
				m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.initialize;
				m_b_done = false;
			}
		}

		internal virtual void StartRedConstruction()
		{
			Reset_();
			m_b_add_red = true;
			if (m_envelopes_red == null)
			{
				m_elements_red = new com.epl.geometry.AttributeStreamOfInt32(0);
				m_envelopes_red = new System.Collections.Generic.List<com.epl.geometry.Envelope2D>(0);
			}
			else
			{
				m_elements_red.ResizePreserveCapacity(0);
				m_envelopes_red.Clear();
			}
		}

		internal virtual void AddRedEnvelope(int element, com.epl.geometry.Envelope2D red_envelope)
		{
			if (!m_b_add_red)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			com.epl.geometry.Envelope2D e = new com.epl.geometry.Envelope2D();
			e.SetCoords(red_envelope);
			m_elements_red.Add(element);
			m_envelopes_red.Add(e);
		}

		internal virtual void EndRedConstruction()
		{
			if (!m_b_add_red)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			m_b_add_red = false;
			if (m_envelopes_red != null && m_envelopes_red.Count > 0 && m_envelopes_blue != null && m_envelopes_blue.Count > 0)
			{
				if (m_function == -1)
				{
					m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.initializeRedBlue;
				}
				else
				{
					if (m_function == com.epl.geometry.Envelope2DIntersectorImpl.State.initializeBlue)
					{
						m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.initializeRedBlue;
					}
					else
					{
						if (m_function != com.epl.geometry.Envelope2DIntersectorImpl.State.initializeRedBlue)
						{
							m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.initializeRed;
						}
					}
				}
				m_b_done = false;
			}
		}

		internal virtual void StartBlueConstruction()
		{
			Reset_();
			m_b_add_blue = true;
			if (m_envelopes_blue == null)
			{
				m_elements_blue = new com.epl.geometry.AttributeStreamOfInt32(0);
				m_envelopes_blue = new System.Collections.Generic.List<com.epl.geometry.Envelope2D>(0);
			}
			else
			{
				m_elements_blue.ResizePreserveCapacity(0);
				m_envelopes_blue.Clear();
			}
		}

		internal virtual void AddBlueEnvelope(int element, com.epl.geometry.Envelope2D blue_envelope)
		{
			if (!m_b_add_blue)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			com.epl.geometry.Envelope2D e = new com.epl.geometry.Envelope2D();
			e.SetCoords(blue_envelope);
			m_elements_blue.Add(element);
			m_envelopes_blue.Add(e);
		}

		internal virtual void EndBlueConstruction()
		{
			if (!m_b_add_blue)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
			m_b_add_blue = false;
			if (m_envelopes_red != null && m_envelopes_red.Count > 0 && m_envelopes_blue != null && m_envelopes_blue.Count > 0)
			{
				if (m_function == -1)
				{
					m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.initializeRedBlue;
				}
				else
				{
					if (m_function == com.epl.geometry.Envelope2DIntersectorImpl.State.initializeRed)
					{
						m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.initializeRedBlue;
					}
					else
					{
						if (m_function != com.epl.geometry.Envelope2DIntersectorImpl.State.initializeRedBlue)
						{
							m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.initializeBlue;
						}
					}
				}
				m_b_done = false;
			}
		}

		/*
		* Moves the iterator to the next intersecting pair of envelopes.Returns
		* true if an intersecting pair is found. You can call get_handle_a() and
		* get_handle_b() to get the index of each envelope in the current
		* intersection. Otherwise if false is returned, then are no more
		* intersections (if at all).
		*/
		internal virtual bool Next()
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
					case com.epl.geometry.Envelope2DIntersectorImpl.State.initialize:
					{
						b_searching = Initialize_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.initializeRed:
					{
						b_searching = InitializeRed_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.initializeBlue:
					{
						b_searching = InitializeBlue_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.initializeRedBlue:
					{
						b_searching = InitializeRedBlue_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.sweep:
					{
						b_searching = Sweep_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.sweepBruteForce:
					{
						b_searching = SweepBruteForce_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.sweepRedBlueBruteForce:
					{
						b_searching = SweepRedBlueBruteForce_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.sweepRedBlue:
					{
						b_searching = SweepRedBlue_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.sweepRed:
					{
						b_searching = SweepRed_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.sweepBlue:
					{
						b_searching = SweepBlue_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.iterate:
					{
						b_searching = Iterate_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.iterateRed:
					{
						b_searching = IterateRed_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.iterateBlue:
					{
						b_searching = IterateBlue_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.iterateBruteForce:
					{
						b_searching = IterateBruteForce_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.iterateRedBlueBruteForce:
					{
						b_searching = IterateRedBlueBruteForce_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.resetRed:
					{
						b_searching = ResetRed_();
						break;
					}

					case com.epl.geometry.Envelope2DIntersectorImpl.State.resetBlue:
					{
						b_searching = ResetBlue_();
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

		/*
		* Returns the index of the first envelope in the intersection. In the
		* red/blue case, this will be an index to the red envelopes.
		*/
		internal virtual int GetHandleA()
		{
			return m_envelope_handle_a;
		}

		/*
		* Returns the index of the second envelope in the intersection. In the
		* red/blue case, this will be an index to the blue envelopes.
		*/
		internal virtual int GetHandleB()
		{
			return m_envelope_handle_b;
		}

		/*
		* Sets the tolerance used for the intersection tests.\param tolerance The
		* tolerance used to determine intersection.
		*/
		internal virtual void SetTolerance(double tolerance)
		{
			m_tolerance = tolerance;
		}

		/*
		* Returns a reference to the envelope at the given handle. Use this for the red/red intersection case.
		*/
		internal virtual com.epl.geometry.Envelope2D GetEnvelope(int handle)
		{
			return m_envelopes_red[handle];
		}

		/*
		* Returns the user element associated with handle. Use this for the red/red intersection case.
		*/
		internal virtual int GetElement(int handle)
		{
			return m_elements_red.Read(handle);
		}

		/*
		* Returns a reference to the red envelope at handle_a.
		*/
		internal virtual com.epl.geometry.Envelope2D GetRedEnvelope(int handle_a)
		{
			return m_envelopes_red[handle_a];
		}

		/*
		* Returns a reference to the blue envelope at handle_b.
		*/
		internal virtual com.epl.geometry.Envelope2D GetBlueEnvelope(int handle_b)
		{
			return m_envelopes_blue[handle_b];
		}

		/*
		* Returns the user element associated with handle_a.
		*/
		internal virtual int GetRedElement(int handle_a)
		{
			return m_elements_red.Read(handle_a);
		}

		/*
		* Returns the user element associated with handle_b.
		*/
		internal virtual int GetBlueElement(int handle_b)
		{
			return m_elements_blue.Read(handle_b);
		}

		private double m_tolerance;

		private int m_sweep_index_red;

		private int m_sweep_index_blue;

		private int m_envelope_handle_a;

		private int m_envelope_handle_b;

		private com.epl.geometry.IntervalTreeImpl m_interval_tree_red;

		private com.epl.geometry.IntervalTreeImpl m_interval_tree_blue;

		private com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl m_iterator_red;

		private com.epl.geometry.IntervalTreeImpl.IntervalTreeIteratorImpl m_iterator_blue;

		private com.epl.geometry.Envelope2D m_envelope_helper = new com.epl.geometry.Envelope2D();

		private System.Collections.Generic.List<com.epl.geometry.Envelope2D> m_envelopes_red;

		private System.Collections.Generic.List<com.epl.geometry.Envelope2D> m_envelopes_blue;

		private com.epl.geometry.AttributeStreamOfInt32 m_elements_red;

		private com.epl.geometry.AttributeStreamOfInt32 m_elements_blue;

		private com.epl.geometry.AttributeStreamOfInt32 m_sorted_end_indices_red;

		private com.epl.geometry.AttributeStreamOfInt32 m_sorted_end_indices_blue;

		private int m_queued_list_red;

		private int m_queued_list_blue;

		private com.epl.geometry.IndexMultiDCList m_queued_envelopes;

		private com.epl.geometry.AttributeStreamOfInt32 m_queued_indices_red;

		private com.epl.geometry.AttributeStreamOfInt32 m_queued_indices_blue;

		private bool m_b_add_red;

		private bool m_b_add_blue;

		private bool m_b_add_red_red;

		private bool m_b_done;

		private static bool IsTop_(int y_end_point_handle)
		{
			return (y_end_point_handle & unchecked((int)(0x1))) == 1;
		}

		private static bool IsBottom_(int y_end_point_handle)
		{
			return (y_end_point_handle & unchecked((int)(0x1))) == 0;
		}

		private void Reset_()
		{
			m_b_add_red = false;
			m_b_add_blue = false;
			m_b_add_red_red = false;
			m_sweep_index_red = -1;
			m_sweep_index_blue = -1;
			m_queued_list_red = -1;
			m_queued_list_blue = -1;
			m_b_done = true;
		}

		private bool Initialize_()
		{
			m_envelope_handle_a = -1;
			m_envelope_handle_b = -1;
			if (m_envelopes_red.Count < 10)
			{
				m_sweep_index_red = m_envelopes_red.Count;
				m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.sweepBruteForce;
				return true;
			}
			if (m_interval_tree_red == null)
			{
				m_interval_tree_red = new com.epl.geometry.IntervalTreeImpl(true);
				m_sorted_end_indices_red = new com.epl.geometry.AttributeStreamOfInt32(0);
			}
			m_interval_tree_red.AddEnvelopesRef(m_envelopes_red);
			if (m_iterator_red == null)
			{
				m_iterator_red = m_interval_tree_red.GetIterator();
			}
			m_sorted_end_indices_red.Reserve(2 * m_envelopes_red.Count);
			m_sorted_end_indices_red.Resize(0);
			for (int i = 0; i < 2 * m_envelopes_red.Count; i++)
			{
				m_sorted_end_indices_red.Add(i);
			}
			SortYEndIndices_(m_sorted_end_indices_red, 0, 2 * m_envelopes_red.Count, true);
			m_sweep_index_red = 2 * m_envelopes_red.Count;
			m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.sweep;
			// overwrite initialize_
			return true;
		}

		private bool InitializeRed_()
		{
			m_envelope_handle_a = -1;
			m_envelope_handle_b = -1;
			if (m_envelopes_red.Count < 10 || m_envelopes_blue.Count < 10)
			{
				m_sweep_index_red = m_envelopes_red.Count;
				m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.sweepRedBlueBruteForce;
				return true;
			}
			if (m_interval_tree_red == null)
			{
				m_interval_tree_red = new com.epl.geometry.IntervalTreeImpl(true);
				m_sorted_end_indices_red = new com.epl.geometry.AttributeStreamOfInt32(0);
			}
			m_interval_tree_red.AddEnvelopesRef(m_envelopes_red);
			if (m_iterator_red == null)
			{
				m_iterator_red = m_interval_tree_red.GetIterator();
			}
			m_sorted_end_indices_red.Reserve(2 * m_envelopes_red.Count);
			m_sorted_end_indices_red.Resize(0);
			for (int i = 0; i < 2 * m_envelopes_red.Count; i++)
			{
				m_sorted_end_indices_red.Add(i);
			}
			SortYEndIndices_(m_sorted_end_indices_red, 0, m_sorted_end_indices_red.Size(), true);
			m_sweep_index_red = m_sorted_end_indices_red.Size();
			if (m_queued_list_red != -1)
			{
				m_queued_envelopes.DeleteList(m_queued_list_red);
				m_queued_indices_red.Resize(0);
				m_queued_list_red = -1;
			}
			m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.sweepRedBlue;
			// overwrite initialize_
			return ResetBlue_();
		}

		private bool InitializeBlue_()
		{
			m_envelope_handle_a = -1;
			m_envelope_handle_b = -1;
			if (m_envelopes_red.Count < 10 || m_envelopes_blue.Count < 10)
			{
				m_sweep_index_red = m_envelopes_red.Count;
				m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.sweepRedBlueBruteForce;
				return true;
			}
			if (m_interval_tree_blue == null)
			{
				m_interval_tree_blue = new com.epl.geometry.IntervalTreeImpl(true);
				m_sorted_end_indices_blue = new com.epl.geometry.AttributeStreamOfInt32(0);
			}
			m_interval_tree_blue.AddEnvelopesRef(m_envelopes_blue);
			if (m_iterator_blue == null)
			{
				m_iterator_blue = m_interval_tree_blue.GetIterator();
			}
			m_sorted_end_indices_blue.Reserve(2 * m_envelopes_blue.Count);
			m_sorted_end_indices_blue.Resize(0);
			for (int i = 0; i < 2 * m_envelopes_blue.Count; i++)
			{
				m_sorted_end_indices_blue.Add(i);
			}
			SortYEndIndices_(m_sorted_end_indices_blue, 0, m_sorted_end_indices_blue.Size(), false);
			m_sweep_index_blue = m_sorted_end_indices_blue.Size();
			if (m_queued_list_blue != -1)
			{
				m_queued_envelopes.DeleteList(m_queued_list_blue);
				m_queued_indices_blue.Resize(0);
				m_queued_list_blue = -1;
			}
			m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.sweepRedBlue;
			// overwrite initialize_
			return ResetRed_();
		}

		private bool InitializeRedBlue_()
		{
			m_envelope_handle_a = -1;
			m_envelope_handle_b = -1;
			if (m_envelopes_red.Count < 10 || m_envelopes_blue.Count < 10)
			{
				m_sweep_index_red = m_envelopes_red.Count;
				m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.sweepRedBlueBruteForce;
				return true;
			}
			if (m_interval_tree_red == null)
			{
				m_interval_tree_red = new com.epl.geometry.IntervalTreeImpl(true);
				m_sorted_end_indices_red = new com.epl.geometry.AttributeStreamOfInt32(0);
			}
			if (m_interval_tree_blue == null)
			{
				m_interval_tree_blue = new com.epl.geometry.IntervalTreeImpl(true);
				m_sorted_end_indices_blue = new com.epl.geometry.AttributeStreamOfInt32(0);
			}
			m_interval_tree_red.AddEnvelopesRef(m_envelopes_red);
			m_interval_tree_blue.AddEnvelopesRef(m_envelopes_blue);
			if (m_iterator_red == null)
			{
				m_iterator_red = m_interval_tree_red.GetIterator();
			}
			if (m_iterator_blue == null)
			{
				m_iterator_blue = m_interval_tree_blue.GetIterator();
			}
			m_sorted_end_indices_red.Reserve(2 * m_envelopes_red.Count);
			m_sorted_end_indices_blue.Reserve(2 * m_envelopes_blue.Count);
			m_sorted_end_indices_red.Resize(0);
			m_sorted_end_indices_blue.Resize(0);
			for (int i = 0; i < 2 * m_envelopes_red.Count; i++)
			{
				m_sorted_end_indices_red.Add(i);
			}
			for (int i_1 = 0; i_1 < 2 * m_envelopes_blue.Count; i_1++)
			{
				m_sorted_end_indices_blue.Add(i_1);
			}
			SortYEndIndices_(m_sorted_end_indices_red, 0, m_sorted_end_indices_red.Size(), true);
			SortYEndIndices_(m_sorted_end_indices_blue, 0, m_sorted_end_indices_blue.Size(), false);
			m_sweep_index_red = m_sorted_end_indices_red.Size();
			m_sweep_index_blue = m_sorted_end_indices_blue.Size();
			if (m_queued_list_red != -1)
			{
				m_queued_envelopes.DeleteList(m_queued_list_red);
				m_queued_indices_red.Resize(0);
				m_queued_list_red = -1;
			}
			if (m_queued_list_blue != -1)
			{
				m_queued_envelopes.DeleteList(m_queued_list_blue);
				m_queued_indices_blue.Resize(0);
				m_queued_list_blue = -1;
			}
			m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.sweepRedBlue;
			// overwrite initialize_
			return true;
		}

		private bool Sweep_()
		{
			int y_end_point_handle = m_sorted_end_indices_red.Get(--m_sweep_index_red);
			int envelope_handle = y_end_point_handle >> 1;
			if (IsBottom_(y_end_point_handle))
			{
				m_interval_tree_red.Remove(envelope_handle);
				if (m_sweep_index_red == 0)
				{
					m_envelope_handle_a = -1;
					m_envelope_handle_b = -1;
					m_b_done = true;
					return false;
				}
				return true;
			}
			m_iterator_red.ResetIterator(m_envelopes_red[envelope_handle].xmin, m_envelopes_red[envelope_handle].xmax, m_tolerance);
			m_envelope_handle_a = envelope_handle;
			m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.iterate;
			return true;
		}

		private bool SweepBruteForce_()
		{
			// this isn't really a sweep, it just walks along the array of red envelopes backward.
			if (--m_sweep_index_red == -1)
			{
				m_envelope_handle_a = -1;
				m_envelope_handle_b = -1;
				m_b_done = true;
				return false;
			}
			m_envelope_handle_a = m_sweep_index_red;
			m_sweep_index_blue = m_sweep_index_red;
			m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.iterateBruteForce;
			return true;
		}

		private bool SweepRedBlueBruteForce_()
		{
			// this isn't really a sweep, it just walks along the array of red envelopes backward.
			if (--m_sweep_index_red == -1)
			{
				m_envelope_handle_a = -1;
				m_envelope_handle_b = -1;
				m_b_done = true;
				return false;
			}
			m_envelope_handle_a = m_sweep_index_red;
			m_sweep_index_blue = m_envelopes_blue.Count;
			m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.iterateRedBlueBruteForce;
			return true;
		}

		private bool SweepRedBlue_()
		{
			// controls whether we want to sweep the red envelopes or sweep the blue envelopes
			int y_end_point_handle_red = m_sorted_end_indices_red.Get(m_sweep_index_red - 1);
			int y_end_point_handle_blue = m_sorted_end_indices_blue.Get(m_sweep_index_blue - 1);
			double y_red = GetAdjustedValue_(y_end_point_handle_red, true);
			double y_blue = GetAdjustedValue_(y_end_point_handle_blue, false);
			if (y_red > y_blue)
			{
				return SweepRed_();
			}
			if (y_red < y_blue)
			{
				return SweepBlue_();
			}
			if (IsTop_(y_end_point_handle_red))
			{
				return SweepRed_();
			}
			if (IsTop_(y_end_point_handle_blue))
			{
				return SweepBlue_();
			}
			return SweepRed_();
		}

		// arbitrary. can call sweep_blue_ instead and would also work correctly
		private bool SweepRed_()
		{
			int y_end_point_handle_red = m_sorted_end_indices_red.Get(--m_sweep_index_red);
			int envelope_handle_red = y_end_point_handle_red >> 1;
			if (IsBottom_(y_end_point_handle_red))
			{
				if (m_queued_list_red != -1 && m_queued_indices_red.Get(envelope_handle_red) != -1)
				{
					m_queued_envelopes.DeleteElement(m_queued_list_red, m_queued_indices_red.Get(envelope_handle_red));
					m_queued_indices_red.Set(envelope_handle_red, -1);
				}
				else
				{
					m_interval_tree_red.Remove(envelope_handle_red);
				}
				if (m_sweep_index_red == 0)
				{
					m_envelope_handle_a = -1;
					m_envelope_handle_b = -1;
					m_b_done = true;
					return false;
				}
				return true;
			}
			if (m_queued_list_blue != -1 && m_queued_envelopes.GetListSize(m_queued_list_blue) > 0)
			{
				int node = m_queued_envelopes.GetFirst(m_queued_list_blue);
				while (node != -1)
				{
					int e = m_queued_envelopes.GetData(node);
					m_interval_tree_blue.Insert(e);
					m_queued_indices_blue.Set(e, -1);
					int next_node = m_queued_envelopes.GetNext(node);
					m_queued_envelopes.DeleteElement(m_queued_list_blue, node);
					node = next_node;
				}
			}
			if (m_interval_tree_blue.Size() > 0)
			{
				m_iterator_blue.ResetIterator(m_envelopes_red[envelope_handle_red].xmin, m_envelopes_red[envelope_handle_red].xmax, m_tolerance);
				m_envelope_handle_a = envelope_handle_red;
				m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.iterateBlue;
			}
			else
			{
				if (m_queued_list_red == -1)
				{
					if (m_queued_envelopes == null)
					{
						m_queued_envelopes = new com.epl.geometry.IndexMultiDCList();
					}
					m_queued_indices_red = new com.epl.geometry.AttributeStreamOfInt32(0);
					m_queued_indices_red.Resize(m_envelopes_red.Count, -1);
					m_queued_indices_red.SetRange(-1, 0, m_envelopes_red.Count);
					m_queued_list_red = m_queued_envelopes.CreateList(1);
				}
				m_queued_indices_red.Set(envelope_handle_red, m_queued_envelopes.AddElement(m_queued_list_red, envelope_handle_red));
				m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.sweepRedBlue;
			}
			return true;
		}

		private bool SweepBlue_()
		{
			int y_end_point_handle_blue = m_sorted_end_indices_blue.Get(--m_sweep_index_blue);
			int envelope_handle_blue = y_end_point_handle_blue >> 1;
			if (IsBottom_(y_end_point_handle_blue))
			{
				if (m_queued_list_blue != -1 && m_queued_indices_blue.Get(envelope_handle_blue) != -1)
				{
					m_queued_envelopes.DeleteElement(m_queued_list_blue, m_queued_indices_blue.Get(envelope_handle_blue));
					m_queued_indices_blue.Set(envelope_handle_blue, -1);
				}
				else
				{
					m_interval_tree_blue.Remove(envelope_handle_blue);
				}
				if (m_sweep_index_blue == 0)
				{
					m_envelope_handle_a = -1;
					m_envelope_handle_b = -1;
					m_b_done = true;
					return false;
				}
				return true;
			}
			if (m_queued_list_red != -1 && m_queued_envelopes.GetListSize(m_queued_list_red) > 0)
			{
				int node = m_queued_envelopes.GetFirst(m_queued_list_red);
				while (node != -1)
				{
					int e = m_queued_envelopes.GetData(node);
					m_interval_tree_red.Insert(e);
					m_queued_indices_red.Set(e, -1);
					int next_node = m_queued_envelopes.GetNext(node);
					m_queued_envelopes.DeleteElement(m_queued_list_red, node);
					node = next_node;
				}
			}
			if (m_interval_tree_red.Size() > 0)
			{
				m_iterator_red.ResetIterator(m_envelopes_blue[envelope_handle_blue].xmin, m_envelopes_blue[envelope_handle_blue].xmax, m_tolerance);
				m_envelope_handle_b = envelope_handle_blue;
				m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.iterateRed;
			}
			else
			{
				if (m_queued_list_blue == -1)
				{
					if (m_queued_envelopes == null)
					{
						m_queued_envelopes = new com.epl.geometry.IndexMultiDCList();
					}
					m_queued_indices_blue = new com.epl.geometry.AttributeStreamOfInt32(0);
					m_queued_indices_blue.Resize(m_envelopes_blue.Count, -1);
					m_queued_indices_blue.SetRange(-1, 0, m_envelopes_blue.Count);
					m_queued_list_blue = m_queued_envelopes.CreateList(0);
				}
				m_queued_indices_blue.Set(envelope_handle_blue, m_queued_envelopes.AddElement(m_queued_list_blue, envelope_handle_blue));
				m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.sweepRedBlue;
			}
			return true;
		}

		private bool Iterate_()
		{
			m_envelope_handle_b = m_iterator_red.Next();
			if (m_envelope_handle_b != -1)
			{
				return false;
			}
			int envelope_handle = m_sorted_end_indices_red.Get(m_sweep_index_red) >> 1;
			m_interval_tree_red.Insert(envelope_handle);
			m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.sweep;
			return true;
		}

		private bool IterateRed_()
		{
			m_envelope_handle_a = m_iterator_red.Next();
			if (m_envelope_handle_a != -1)
			{
				return false;
			}
			m_envelope_handle_a = -1;
			m_envelope_handle_b = -1;
			int envelope_handle_blue = m_sorted_end_indices_blue.Get(m_sweep_index_blue) >> 1;
			m_interval_tree_blue.Insert(envelope_handle_blue);
			m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.sweepRedBlue;
			return true;
		}

		private bool IterateBlue_()
		{
			m_envelope_handle_b = m_iterator_blue.Next();
			if (m_envelope_handle_b != -1)
			{
				return false;
			}
			int envelope_handle_red = m_sorted_end_indices_red.Get(m_sweep_index_red) >> 1;
			m_interval_tree_red.Insert(envelope_handle_red);
			m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.sweepRedBlue;
			return true;
		}

		private bool IterateBruteForce_()
		{
			if (--m_sweep_index_blue == -1)
			{
				m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.sweepBruteForce;
				return true;
			}
			m_envelope_helper.SetCoords(m_envelopes_red[m_sweep_index_red]);
			com.epl.geometry.Envelope2D envelope_b = m_envelopes_red[m_sweep_index_blue];
			m_envelope_helper.Inflate(m_tolerance, m_tolerance);
			if (m_envelope_helper.IsIntersecting(envelope_b))
			{
				m_envelope_handle_b = m_sweep_index_blue;
				return false;
			}
			return true;
		}

		private bool IterateRedBlueBruteForce_()
		{
			if (--m_sweep_index_blue == -1)
			{
				m_function = com.epl.geometry.Envelope2DIntersectorImpl.State.sweepRedBlueBruteForce;
				return true;
			}
			m_envelope_helper.SetCoords(m_envelopes_red[m_sweep_index_red]);
			com.epl.geometry.Envelope2D envelope_b = m_envelopes_blue[m_sweep_index_blue];
			m_envelope_helper.Inflate(m_tolerance, m_tolerance);
			if (m_envelope_helper.IsIntersecting(envelope_b))
			{
				m_envelope_handle_b = m_sweep_index_blue;
				return false;
			}
			return true;
		}

		private bool ResetRed_()
		{
			if (m_interval_tree_red == null)
			{
				m_b_done = true;
				return false;
			}
			m_sweep_index_red = m_sorted_end_indices_red.Size();
			if (m_interval_tree_red.Size() > 0)
			{
				m_interval_tree_red.Reset();
			}
			if (m_queued_list_red != -1)
			{
				m_queued_envelopes.DeleteList(m_queued_list_red);
				m_queued_indices_red.Resize(0);
				m_queued_list_red = -1;
			}
			m_b_done = false;
			return true;
		}

		private bool ResetBlue_()
		{
			if (m_interval_tree_blue == null)
			{
				m_b_done = true;
				return false;
			}
			m_sweep_index_blue = m_sorted_end_indices_blue.Size();
			if (m_interval_tree_blue.Size() > 0)
			{
				m_interval_tree_blue.Reset();
			}
			if (m_queued_list_blue != -1)
			{
				m_queued_envelopes.DeleteList(m_queued_list_blue);
				m_queued_indices_blue.Resize(0);
				m_queued_list_blue = -1;
			}
			m_b_done = false;
			return true;
		}

		private int m_function;

		private abstract class State
		{
			public const int initialize = 0;

			public const int initializeRed = 1;

			public const int initializeBlue = 2;

			public const int initializeRedBlue = 3;

			public const int sweep = 4;

			public const int sweepBruteForce = 5;

			public const int sweepRedBlueBruteForce = 6;

			public const int sweepRedBlue = 7;

			public const int sweepRed = 8;

			public const int sweepBlue = 9;

			public const int iterate = 10;

			public const int iterateRed = 11;

			public const int iterateBlue = 12;

			public const int iterateBruteForce = 13;

			public const int iterateRedBlueBruteForce = 14;

			public const int resetRed = 15;

			public const int resetBlue = 16;
		}

		private static class StateConstants
		{
		}

		private com.epl.geometry.BucketSort m_bucket_sort;

		// *********** Helpers for Bucket sort**************
		private void SortYEndIndices_(com.epl.geometry.AttributeStreamOfInt32 end_indices, int begin_, int end_, bool b_red)
		{
			if (m_bucket_sort == null)
			{
				m_bucket_sort = new com.epl.geometry.BucketSort();
			}
			com.epl.geometry.Envelope2DIntersectorImpl.Envelope2DBucketSortHelper sorter = new com.epl.geometry.Envelope2DIntersectorImpl.Envelope2DBucketSortHelper(this, b_red);
			m_bucket_sort.Sort(end_indices, begin_, end_, sorter);
		}

		private void SortYEndIndicesHelper_(com.epl.geometry.AttributeStreamOfInt32 end_indices, int begin_, int end_, bool b_red)
		{
			end_indices.Sort(begin_, end_, new com.epl.geometry.Envelope2DIntersectorImpl.EndPointsComparer(this, b_red));
		}

		private double GetAdjustedValue_(int e, bool b_red)
		{
			double dy = 0.5 * m_tolerance;
			if (b_red)
			{
				com.epl.geometry.Envelope2D envelope_red = m_envelopes_red[e >> 1];
				double y = (IsBottom_(e) ? envelope_red.ymin - dy : envelope_red.ymax + dy);
				return y;
			}
			com.epl.geometry.Envelope2D envelope_blue = m_envelopes_blue[e >> 1];
			double y_1 = (IsBottom_(e) ? envelope_blue.ymin - dy : envelope_blue.ymax + dy);
			return y_1;
		}

		private sealed class EndPointsComparer : com.epl.geometry.AttributeStreamOfInt32.IntComparator
		{
			internal EndPointsComparer(com.epl.geometry.Envelope2DIntersectorImpl intersector, bool b_red)
			{
				// For user sort
				m_intersector = intersector;
				m_b_red = b_red;
			}

			public override int Compare(int e_1, int e_2)
			{
				double y1 = m_intersector.GetAdjustedValue_(e_1, m_b_red);
				double y2 = m_intersector.GetAdjustedValue_(e_2, m_b_red);
				if (y1 < y2 || (y1 == y2 && IsBottom_(e_1) && IsTop_(e_2)))
				{
					return -1;
				}
				return 1;
			}

			private com.epl.geometry.Envelope2DIntersectorImpl m_intersector;

			private bool m_b_red;
		}

		private sealed class Envelope2DBucketSortHelper : com.epl.geometry.ClassicSort
		{
			internal Envelope2DBucketSortHelper(com.epl.geometry.Envelope2DIntersectorImpl intersector, bool b_red)
			{
				// For
				// bucket
				// sort
				m_intersector = intersector;
				m_b_red = b_red;
			}

			public override void UserSort(int begin, int end, com.epl.geometry.AttributeStreamOfInt32 indices)
			{
				m_intersector.SortYEndIndicesHelper_(indices, begin, end, m_b_red);
			}

			public override double GetValue(int index)
			{
				return m_intersector.GetAdjustedValue_(index, m_b_red);
			}

			private com.epl.geometry.Envelope2DIntersectorImpl m_intersector;

			private bool m_b_red;
		}
	}
}
