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
	/// <summary>A collection of strides of Index_type elements.</summary>
	/// <remarks>
	/// A collection of strides of Index_type elements. To be used when one needs a
	/// collection of homogeneous elements that contain only integer fields (i.e.
	/// structs with Index_type members) Recycles the strides. Allows for constant
	/// time creation and deletion of an element.
	/// </remarks>
	internal sealed class StridedIndexTypeCollection
	{
		private int[][] m_buffer = null;

		private int m_firstFree = -1;

		private int m_last = 0;

		private int m_size = 0;

		private int m_capacity = 0;

		private int m_bufferSize = 0;

		private int m_stride;

		private int m_realStride;

		private int m_blockSize;

		private const int m_realBlockSize = 16384;

		private const int m_blockMask = unchecked((int)(0x3FFF));

		private const int m_blockPower = 14;

		private static readonly int[] st_sizes = new int[] { 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };

		internal StridedIndexTypeCollection(int stride)
		{
			/*
			private final static int m_realBlockSize = 2048;//if you change this, change m_blockSize, m_blockPower, m_blockMask, and st_sizes
			private final static int m_blockMask = 0x7FF;
			private final static int m_blockPower = 11;
			private final static int[] st_sizes = {16, 32, 64, 128, 256, 512, 1024, 2048};
			*/
			// if you change this,
			// change m_blockSize,
			// m_blockPower,
			// m_blockMask, and
			// st_sizes
			m_stride = stride;
			m_realStride = stride;
			m_blockSize = m_realBlockSize / m_realStride;
		}

		private bool Dbgdelete_(int element)
		{
			m_buffer[element >> m_blockPower][(element & m_blockMask) + 1] = -unchecked((int)(0x7eadbeed));
			return true;
		}

		internal void DeleteElement(int element)
		{
			System.Diagnostics.Debug.Assert((Dbgdelete_(element)));
			int totalStrides = (element >> m_blockPower) * m_blockSize * m_realStride + (element & m_blockMask);
			if (totalStrides < m_last * m_realStride)
			{
				m_buffer[element >> m_blockPower][element & m_blockMask] = m_firstFree;
				m_firstFree = element;
			}
			else
			{
				System.Diagnostics.Debug.Assert((totalStrides == m_last * m_realStride));
				m_last--;
			}
			m_size--;
		}

		// Returns the given field of the element.
		internal int GetField(int element, int field)
		{
			System.Diagnostics.Debug.Assert((m_buffer[element >> m_blockPower][(element & m_blockMask) + 1] != -unchecked((int)(0x7eadbeed))));
			return m_buffer[element >> m_blockPower][(element & m_blockMask) + field];
		}

		// Sets the given field of the element.
		internal void SetField(int element, int field, int value)
		{
			System.Diagnostics.Debug.Assert((m_buffer[element >> m_blockPower][(element & m_blockMask) + 1] != -unchecked((int)(0x7eadbeed))));
			m_buffer[element >> m_blockPower][(element & m_blockMask) + field] = value;
		}

		// Returns the stride size
		internal int GetStride()
		{
			return m_stride;
		}

		// Creates the new element. This is a constant time operation.
		// All fields are initialized to -1.
		internal int NewElement()
		{
			int element = m_firstFree;
			if (element == -1)
			{
				if (m_last == m_capacity)
				{
					long newcap = m_capacity != 0 ? (((long)m_capacity + 1) * 3 / 2) : (long)1;
					if (newcap > int.MaxValue)
					{
						newcap = int.MaxValue;
					}
					// cannot grow past 2gb elements
					// presently
					if (newcap == m_capacity)
					{
						throw new System.IndexOutOfRangeException();
					}
					Grow_(newcap);
				}
				element = ((m_last / m_blockSize) << m_blockPower) + (m_last % m_blockSize) * m_realStride;
				m_last++;
			}
			else
			{
				m_firstFree = m_buffer[element >> m_blockPower][element & m_blockMask];
			}
			m_size++;
			int[] ar = m_buffer[element >> m_blockPower];
			int ind = element & m_blockMask;
			for (int i = 0; i < m_stride; i++)
			{
				ar[ind + i] = -1;
			}
			return element;
		}

		internal int ElementToIndex(int element)
		{
			return (element >> m_blockPower) * m_blockSize + (element & m_blockMask) / m_realStride;
		}

		// Deletes all elements and frees all the memory if b_free_memory is True.
		internal void DeleteAll(bool b_free_memory)
		{
			m_firstFree = -1;
			m_last = 0;
			m_size = 0;
			if (b_free_memory)
			{
				m_buffer = null;
				m_capacity = 0;
			}
		}

		// Returns the count of existing elements
		internal int Size()
		{
			return m_size;
		}

		// Sets the capcity of the collection. Only applied if current capacity is
		// smaller.
		internal void SetCapacity(int capacity)
		{
			if (capacity > m_capacity)
			{
				Grow_(capacity);
			}
		}

		// Returns the capacity of the collection
		internal int Capacity()
		{
			return m_capacity;
		}

		// Swaps content of two elements (each field of the stride)
		internal void Swap(int element1, int element2)
		{
			int[] ar1 = m_buffer[element1 >> m_blockPower];
			int[] ar2 = m_buffer[element2 >> m_blockPower];
			int ind1 = element1 & m_blockMask;
			int ind2 = element2 & m_blockMask;
			for (int i = 0; i < m_stride; i++)
			{
				int tmp = ar1[ind1 + i];
				ar1[ind1 + i] = ar2[ind2 + i];
				ar2[ind2 + i] = tmp;
			}
		}

		// Swaps content of two fields
		internal void SwapField(int element1, int element2, int field)
		{
			int[] ar1 = m_buffer[element1 >> m_blockPower];
			int[] ar2 = m_buffer[element2 >> m_blockPower];
			int ind1 = (element1 & m_blockMask) + field;
			int ind2 = (element2 & m_blockMask) + field;
			int tmp = ar1[ind1];
			ar1[ind1] = ar2[ind2];
			ar2[ind2] = tmp;
		}

		// Returns a value of the index, that never will be returned by new_element
		// and is neither -1 nor impossible_index_3.
		internal static int ImpossibleIndex2()
		{
			return -2;
		}

		// Returns a value of the index, that never will be returned by new_element
		// and is neither -1 nor impossible_index_2.
		internal static int ImpossibleIndex3()
		{
			return -3;
		}

		internal static bool IsValidElement(int element)
		{
			return element >= 0;
		}

		private void EnsureBufferBlocksCapacity(int blocks)
		{
			if (m_buffer.Length < blocks)
			{
				int[][] newBuffer = new int[blocks][];
				for (int i = 0; i < m_buffer.Length; i++)
				{
					newBuffer[i] = m_buffer[i];
				}
				m_buffer = newBuffer;
			}
		}

		private void Grow_(long newsize)
		{
			if (m_buffer == null)
			{
				m_bufferSize = 0;
				m_buffer = new int[8][];
			}
			System.Diagnostics.Debug.Assert((newsize > m_capacity));
			long nblocks = (newsize + m_blockSize - 1) / m_blockSize;
			if (nblocks > int.MaxValue)
			{
				throw new System.IndexOutOfRangeException();
			}
			EnsureBufferBlocksCapacity((int)nblocks);
			if (nblocks == 1)
			{
				// When less than one block is needed we allocate smaller arrays
				// than m_realBlockSize to avoid initialization cost.
				int oldsz = m_capacity > 0 ? m_capacity : 0;
				System.Diagnostics.Debug.Assert((oldsz < newsize));
				int i = 0;
				int realnewsize = (int)newsize * m_realStride;
				while (realnewsize > st_sizes[i])
				{
					// get the size to allocate. Using fixed sizes to reduce
					// fragmentation.
					i++;
				}
				int[] b = new int[st_sizes[i]];
				if (m_bufferSize == 1)
				{
					System.Array.Copy(m_buffer[0], 0, b, 0, m_buffer[0].Length);
					m_buffer[0] = b;
				}
				else
				{
					m_buffer[m_bufferSize] = b;
					m_bufferSize++;
				}
				m_capacity = b.Length / m_realStride;
			}
			else
			{
				if (nblocks * m_blockSize > int.MaxValue)
				{
					throw new System.IndexOutOfRangeException();
				}
				if (m_bufferSize == 1)
				{
					if (m_buffer[0].Length < m_realBlockSize)
					{
						// resize the first buffer to ensure it is equal the
						// m_realBlockSize.
						int[] b = new int[m_realBlockSize];
						System.Array.Copy(m_buffer[0], 0, b, 0, m_buffer[0].Length);
						m_buffer[0] = b;
						m_capacity = m_blockSize;
					}
				}
				while (m_bufferSize < nblocks)
				{
					m_buffer[m_bufferSize++] = new int[m_realBlockSize];
					m_capacity += m_blockSize;
				}
			}
		}
	}
}
