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
	internal sealed class BucketSort
	{
		internal com.epl.geometry.AttributeStreamOfInt32 m_buckets;

		internal com.epl.geometry.AttributeStreamOfInt32 m_bucketed_indices;

		internal double m_min_value;

		internal double m_max_value;

		internal double m_dy;

		internal static int MAXBUCKETS = 65536;

		public BucketSort()
		{
			m_buckets = new com.epl.geometry.AttributeStreamOfInt32(0);
			m_bucketed_indices = new com.epl.geometry.AttributeStreamOfInt32(0);
			m_min_value = 1;
			m_max_value = -1;
			m_dy = com.epl.geometry.NumberUtils.TheNaN;
		}

		/// <summary>Executes sort on the Bucket_sort.</summary>
		/// <remarks>
		/// Executes sort on the Bucket_sort. The result is fed into the indices
		/// array in the range between begin (inclusive) and end (exclusive). Uses
		/// user supplied sorter to execute sort on each bucket. Users either supply
		/// the sorter and use this method of Bucket_sort class, or use other methods
		/// to form the buckets and take care of bucket sorting themselves.
		/// </remarks>
		public void Sort(com.epl.geometry.AttributeStreamOfInt32 indices, int begin, int end, com.epl.geometry.ClassicSort sorter)
		{
			if (end - begin < 32)
			{
				sorter.UserSort(begin, end, indices);
				return;
			}
			bool b_fallback = true;
			try
			{
				double miny = com.epl.geometry.NumberUtils.PositiveInf();
				double maxy = com.epl.geometry.NumberUtils.NegativeInf();
				for (int i = begin; i < end; i++)
				{
					double y = sorter.GetValue(indices.Get(i));
					if (y < miny)
					{
						miny = y;
					}
					if (y > maxy)
					{
						maxy = y;
					}
				}
				if (Reset(end - begin, miny, maxy, end - begin))
				{
					for (int i_1 = begin; i_1 < end; i_1++)
					{
						int vertex = indices.Get(i_1);
						double y = sorter.GetValue(vertex);
						int bucket = GetBucket(y);
						m_buckets.Set(bucket, m_buckets.Get(bucket) + 1);
						// counting
						// values
						// in a
						// bucket.
						m_bucketed_indices.Write(i_1 - begin, vertex);
					}
					// Recalculate buckets to contain start positions of buckets.
					int c = m_buckets.Get(0);
					m_buckets.Set(0, 0);
					for (int i_2 = 1, n = m_buckets.Size(); i_2 < n; i_2++)
					{
						int b = m_buckets.Get(i_2);
						m_buckets.Set(i_2, c);
						c += b;
					}
					for (int i_3 = begin; i_3 < end; i_3++)
					{
						int vertex = m_bucketed_indices.Read(i_3 - begin);
						double y = sorter.GetValue(vertex);
						int bucket = GetBucket(y);
						int bucket_index = m_buckets.Get(bucket);
						indices.Set(bucket_index + begin, vertex);
						m_buckets.Set(bucket, bucket_index + 1);
					}
					b_fallback = false;
				}
			}
			catch (System.Exception)
			{
				m_buckets.Resize(0);
				m_bucketed_indices.Resize(0);
			}
			if (b_fallback)
			{
				sorter.UserSort(begin, end, indices);
				return;
			}
			int j = 0;
			for (int i_4 = 0, n = m_buckets.Size(); i_4 < n; i_4++)
			{
				int j0 = j;
				j = m_buckets.Get(i_4);
				if (j > j0)
				{
					sorter.UserSort(begin + j0, begin + j, indices);
				}
			}
			System.Diagnostics.Debug.Assert((j == end));
			if (GetBucketCount() > 100)
			{
				// some heuristics to preserve memory
				m_buckets.Resize(0);
				m_bucketed_indices.Resize(0);
			}
		}

		/// <summary>
		/// Clears and resets Bucket_sort to the new state, preparing for the
		/// accumulation of new data.
		/// </summary>
		/// <param name="bucket_count">
		/// - the number of buckets. Usually equal to the number of
		/// elements to sort.
		/// </param>
		/// <param name="min_value">- the minimum value of elements to sort.</param>
		/// <param name="max_value">- the maximum value of elements to sort.</param>
		/// <param name="capacity">
		/// - the number of elements to sort (-1 if not known). The
		/// bucket_count are usually equal.
		/// </param>
		/// <returns>
		/// Returns False, if the bucket sort cannot be used with the given
		/// parameters. The method also can throw out of memory exception. In
		/// the later case, one should fall back to the regular sort.
		/// </returns>
		private bool Reset(int bucket_count, double min_value, double max_value, int capacity)
		{
			if (bucket_count < 2 || max_value == min_value)
			{
				return false;
			}
			int bc = System.Math.Min(MAXBUCKETS, bucket_count);
			m_buckets.Reserve(bc);
			m_buckets.Resize(bc);
			m_buckets.SetRange(0, 0, m_buckets.Size());
			m_min_value = min_value;
			m_max_value = max_value;
			m_bucketed_indices.Resize(capacity);
			m_dy = (max_value - min_value) / (bc - 1);
			return true;
		}

		/// <summary>Adds new element to the bucket builder.</summary>
		/// <remarks>
		/// Adds new element to the bucket builder. The value must be between
		/// min_value and max_value.
		/// </remarks>
		/// <param name="The">value used for bucketing.</param>
		/// <param name="The">
		/// index of the element to store in the buffer. Usually it is an
		/// index into some array, where the real elements are stored.
		/// </param>
		private int GetBucket(double value)
		{
			System.Diagnostics.Debug.Assert((value >= m_min_value && value <= m_max_value));
			int bucket = (int)((value - m_min_value) / m_dy);
			return bucket;
		}

		/// <summary>Returns the bucket count.</summary>
		private int GetBucketCount()
		{
			return m_buckets.Size();
		}
	}
}
