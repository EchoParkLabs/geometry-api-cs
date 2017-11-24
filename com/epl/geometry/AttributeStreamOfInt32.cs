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
	internal sealed class AttributeStreamOfInt32 : com.epl.geometry.AttributeStreamBase
	{
		private int[] m_buffer = null;

		private int m_size;

		public void Reserve(int reserve)
		{
			if (reserve <= 0)
			{
				return;
			}
			if (m_buffer == null)
			{
				m_buffer = new int[reserve];
			}
			else
			{
				if (reserve <= m_buffer.Length)
				{
					return;
				}
				int[] buf = new int[reserve];
				System.Array.Copy(m_buffer, 0, buf, 0, m_buffer.Length);
				m_buffer = buf;
			}
		}

		public int Size()
		{
			return m_size;
		}

		public int Capacity()
		{
			return m_buffer != null ? m_buffer.Length : 0;
		}

		public AttributeStreamOfInt32(int size)
		{
			int sz = size;
			if (sz < 2)
			{
				sz = 2;
			}
			m_buffer = new int[sz];
			m_size = size;
		}

		public AttributeStreamOfInt32(int size, int defaultValue)
		{
			int sz = size;
			if (sz < 2)
			{
				sz = 2;
			}
			m_buffer = new int[sz];
			m_size = size;
			java.util.Arrays.Fill(m_buffer, 0, size, defaultValue);
		}

		public AttributeStreamOfInt32(com.epl.geometry.AttributeStreamOfInt32 other)
		{
			// for (int i = 0; i < size; i++)
			// m_buffer[i] = defaultValue;
			m_buffer = (int[])other.m_buffer.Clone();
			m_size = other.m_size;
		}

		public AttributeStreamOfInt32(com.epl.geometry.AttributeStreamOfInt32 other, int maxSize)
		{
			m_size = other.Size();
			if (m_size > maxSize)
			{
				m_size = maxSize;
			}
			int sz = m_size;
			if (sz < 2)
			{
				sz = 2;
			}
			m_buffer = new int[sz];
			System.Array.Copy(other.m_buffer, 0, m_buffer, 0, m_size);
		}

		/// <summary>Reads a value from the buffer at given offset.</summary>
		/// <param name="offset">is the element number in the stream.</param>
		public int Read(int offset)
		{
			return m_buffer[offset];
		}

		public int Get(int offset)
		{
			return m_buffer[offset];
		}

		/// <summary>Overwrites given element with new value.</summary>
		/// <param name="offset">is the element number in the stream.</param>
		/// <param name="value">is the value to write.</param>
		public void Write(int offset, int value)
		{
			if (m_bReadonly)
			{
				throw new System.Exception("invalid_call");
			}
			m_buffer[offset] = value;
		}

		public void Set(int offset, int value)
		{
			if (m_bReadonly)
			{
				throw new System.Exception("invalid_call");
			}
			m_buffer[offset] = value;
		}

		/// <summary>Adds a new value at the end of the stream.</summary>
		/// <param name="offset">is the element number in the stream.</param>
		/// <param name="value">is the value to write.</param>
		public void Add(int v)
		{
			Resize(m_size + 1);
			m_buffer[m_size - 1] = v;
		}

		public override com.epl.geometry.AttributeStreamBase RestrictedClone(int maxsize)
		{
			int len = m_size;
			int newSize = maxsize < len ? maxsize : len;
			int[] newBuffer = new int[newSize];
			System.Array.Copy(m_buffer, 0, newBuffer, 0, newSize);
			m_buffer = newBuffer;
			m_size = newSize;
			return this;
		}

		public override int VirtualSize()
		{
			return Size();
		}

		public override int GetPersistence()
		{
			return com.epl.geometry.VertexDescription.Persistence.enumInt32;
		}

		public override double ReadAsDbl(int offset)
		{
			return Read(offset);
		}

		public override int ReadAsInt(int offset)
		{
			return (int)Read(offset);
		}

		public override long ReadAsInt64(int offset)
		{
			return (long)Read(offset);
		}

		public override void Resize(int newSize)
		{
			if (m_bLockedInSize)
			{
				throw new com.epl.geometry.GeometryException("invalid call. Attribute Stream is locked and cannot be resized.");
			}
			if (newSize <= m_size)
			{
				if ((newSize * 5) / 4 < m_buffer.Length)
				{
					// decrease when the 25%
					// margin is exceeded
					int[] newBuffer = new int[newSize];
					System.Array.Copy(m_buffer, 0, newBuffer, 0, newSize);
					m_buffer = newBuffer;
				}
				m_size = newSize;
			}
			else
			{
				if (newSize > m_buffer.Length)
				{
					int sz = (newSize < 64) ? System.Math.Max(newSize * 2, 4) : (newSize * 5) / 4;
					int[] newBuffer = new int[sz];
					System.Array.Copy(m_buffer, 0, newBuffer, 0, m_size);
					m_buffer = newBuffer;
				}
				m_size = newSize;
			}
		}

		public override void ResizePreserveCapacity(int newSize)
		{
			// java only method
			if (m_buffer == null || newSize > m_buffer.Length)
			{
				Resize(newSize);
			}
			if (m_bLockedInSize)
			{
				throw new com.epl.geometry.GeometryException("invalid call. Attribute Stream is locked and cannot be resized.");
			}
			m_size = newSize;
		}

		public override void Resize(int newSize, double defaultValue)
		{
			if (m_bLockedInSize)
			{
				throw new com.epl.geometry.GeometryException("invalid call. Attribute Stream is locked and cannot be resized.");
			}
			if (newSize <= m_size)
			{
				if ((newSize * 5) / 4 < m_buffer.Length)
				{
					// decrease when the 25%
					// margin is exceeded
					int[] newBuffer = new int[newSize];
					System.Array.Copy(m_buffer, 0, newBuffer, 0, newSize);
					m_buffer = newBuffer;
				}
				m_size = newSize;
			}
			else
			{
				if (newSize > m_buffer.Length)
				{
					int sz = (newSize < 64) ? System.Math.Max(newSize * 2, 4) : (newSize * 5) / 4;
					int[] newBuffer = new int[sz];
					System.Array.Copy(m_buffer, 0, newBuffer, 0, m_size);
					m_buffer = newBuffer;
				}
				java.util.Arrays.Fill(m_buffer, m_size, newSize, (int)defaultValue);
				// for (int i = m_size; i < newSize; i++)
				// m_buffer[i] = (int)defaultValue;
				m_size = newSize;
			}
		}

		public override void WriteAsDbl(int offset, double d)
		{
			Write(offset, (int)d);
		}

		public override void WriteAsInt64(int offset, long d)
		{
			Write(offset, (int)d);
		}

		public override void WriteAsInt(int offset, int d)
		{
			Write(offset, (int)d);
		}

		// @Override
		// public void writeRange(int srcStart, int count, ByteBuffer dst,
		// int dstOffsetBytes) {
		// // TODO Auto-generated method stub
		//
		// }
		public override int CalculateHashImpl(int hashCode, int start, int end)
		{
			for (int i = start, n = Size(); i < n && i < end; i++)
			{
				hashCode = com.epl.geometry.NumberUtils.Hash(hashCode, Read(i));
			}
			return hashCode;
		}

		public override bool Equals(com.epl.geometry.AttributeStreamBase other, int start, int end)
		{
			if (other == null)
			{
				return false;
			}
			if (!(other is com.epl.geometry.AttributeStreamOfInt32))
			{
				return false;
			}
			com.epl.geometry.AttributeStreamOfInt32 _other = (com.epl.geometry.AttributeStreamOfInt32)other;
			int size = Size();
			int sizeOther = _other.Size();
			if (end > size || end > sizeOther && (size != sizeOther))
			{
				return false;
			}
			if (end > size)
			{
				end = size;
			}
			for (int i = start; i < end; i++)
			{
				if (Read(i) != _other.Read(i))
				{
					return false;
				}
			}
			return true;
		}

		public int GetLast()
		{
			return m_buffer[m_size - 1];
		}

		public void SetLast(int v)
		{
			m_buffer[m_size - 1] = v;
		}

		public void RemoveLast()
		{
			Resize(m_size - 1);
		}

		// Finds element in the unsorted array and returns its index. Returns -1 if
		// the element could not be found.
		internal int FindElement(int value)
		{
			for (int i = 0, n = m_size; i < n; i++)
			{
				if (m_buffer[i] == value)
				{
					return i;
				}
			}
			return -1;
		}

		// Returns True if element could be found in the array.
		internal bool HasElement(int value)
		{
			return FindElement(value) >= 0;
		}

		// Removes the element from the array in constant time.
		// It moves the last element of the array to the index and decrements the
		// array size by 1.
		internal void PopElement(int index)
		{
			System.Diagnostics.Debug.Assert((index >= 0 && index < m_size));
			if (index < m_size - 1)
			{
				m_buffer[index] = m_buffer[m_size - 1];
			}
			Resize(m_size - 1);
		}

		public override void AddRange(com.epl.geometry.AttributeStreamBase src, int start, int count, bool bForward, int stride)
		{
			if (m_bReadonly)
			{
				throw new com.epl.geometry.GeometryException("invalid_call");
			}
			if (!bForward && (stride < 1 || count % stride != 0))
			{
				throw new System.ArgumentException();
			}
			int oldSize = m_size;
			int newSize = oldSize + count;
			Resize(newSize);
			if (bForward)
			{
				System.Array.Copy(((com.epl.geometry.AttributeStreamOfInt32)src).m_buffer, start, m_buffer, oldSize, count);
			}
			else
			{
				int n = count;
				for (int i = 0; i < count; i += stride)
				{
					n -= stride;
					for (int s = 0; s < stride; s++)
					{
						m_buffer[oldSize + i + s] = ((com.epl.geometry.AttributeStreamOfInt32)src).m_buffer[start + n + s];
					}
				}
			}
		}

		public override void InsertRange(int start, com.epl.geometry.AttributeStreamBase src, int srcStart, int count, bool bForward, int stride, int validSize)
		{
			if (m_bReadonly)
			{
				throw new com.epl.geometry.GeometryException("invalid_call");
			}
			if (!bForward && (stride < 1 || count % stride != 0))
			{
				throw new System.ArgumentException();
			}
			System.Array.Copy(m_buffer, start, m_buffer, start + count, validSize - start);
			if (m_buffer == ((com.epl.geometry.AttributeStreamOfInt32)src).m_buffer)
			{
				if (start < srcStart)
				{
					srcStart += count;
				}
			}
			if (bForward)
			{
				System.Array.Copy(((com.epl.geometry.AttributeStreamOfInt32)src).m_buffer, srcStart, m_buffer, start, count);
			}
			else
			{
				int n = count;
				for (int i = 0; i < count; i += stride)
				{
					n -= stride;
					for (int s = 0; s < stride; s++)
					{
						m_buffer[start + i + s] = ((com.epl.geometry.AttributeStreamOfInt32)src).m_buffer[srcStart + n + s];
					}
				}
			}
		}

		public override void InsertRange(int start, double value, int count, int validSize)
		{
			if (m_bReadonly)
			{
				throw new com.epl.geometry.GeometryException("invalid_call");
			}
			System.Array.Copy(m_buffer, start, m_buffer, start + count, validSize - start);
			int v = (int)value;
			java.util.Arrays.Fill(m_buffer, start, start + count, v);
		}

		// for (int i = 0; i < count; i++)
		// {
		// m_buffer[start + i] = v;
		// }
		public override void InsertAttributes(int start, com.epl.geometry.Point pt, int semantics, int validSize)
		{
			if (m_bReadonly)
			{
				throw new com.epl.geometry.GeometryException("invalid_call");
			}
			int comp = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			System.Array.Copy(m_buffer, start, m_buffer, start + comp, validSize - start);
			for (int c = 0; c < comp; c++)
			{
				m_buffer[start + c] = (int)pt.GetAttributeAsDbl(semantics, c);
			}
		}

		public override void EraseRange(int index, int count, int validSize)
		{
			if (m_bReadonly)
			{
				throw new com.epl.geometry.GeometryException("invalid_call");
			}
			if (index + count > m_size)
			{
				throw new com.epl.geometry.GeometryException("invalid_call");
			}
			System.Array.Copy(m_buffer, index + count, m_buffer, index, validSize - (index + count));
			m_size -= count;
		}

		public override void ReadRange(int srcStart, int count, System.IO.BinaryWriter dst, int dstOffset, bool bForward)
		{
			if (srcStart < 0 || count < 0 || dstOffset < 0 || Size() < count + srcStart)
			{
				throw new System.ArgumentException();
			}
			int elmSize = com.epl.geometry.NumberUtils.SizeOf((double)0);
			if (dst.BaseStream.Length < (int)(dstOffset + elmSize * count))
			{
				throw new System.ArgumentException();
			}
			if (count == 0)
			{
				return;
			}
			int j = srcStart;
			if (!bForward)
			{
				j += count - 1;
			}
			int dj = bForward ? 1 : -1;
			int offset = dstOffset;
			for (int i = 0; i < count; i++, offset += elmSize)
			{
				dst.Write(m_buffer[j]);
				j += dj;
			}
		}

		public override void ReverseRange(int index, int count, int stride)
		{
			if (m_bReadonly)
			{
				throw new com.epl.geometry.GeometryException("invalid_call");
			}
			if (stride < 1 || count % stride != 0)
			{
				throw new com.epl.geometry.GeometryException("invalid_call");
			}
			int cIterations = count >> 1;
			int n = count;
			for (int i = 0; i < cIterations; i += stride)
			{
				n -= stride;
				for (int s = 0; s < stride; s++)
				{
					int temp = m_buffer[index + i + s];
					m_buffer[index + i + s] = m_buffer[index + n + s];
					m_buffer[index + n + s] = temp;
				}
			}
		}

		public override void SetRange(double value, int start, int count)
		{
			if (start < 0 || count < 0 || start < 0 || count + start > Size())
			{
				throw new System.ArgumentException();
			}
			int v = (int)value;
			java.util.Arrays.Fill(m_buffer, start, start + count, v);
		}

		// for (int i = start, n = start + count; i < n; i++)
		// write(i, v);
		public override void WriteRange(int startElement, int count, com.epl.geometry.AttributeStreamBase _src, int srcStart, bool bForward, int stride)
		{
			if (startElement < 0 || count < 0 || srcStart < 0)
			{
				throw new System.ArgumentException();
			}
			if (!bForward && (stride <= 0 || (count % stride != 0)))
			{
				throw new System.ArgumentException();
			}
			com.epl.geometry.AttributeStreamOfInt32 src = (com.epl.geometry.AttributeStreamOfInt32)_src;
			// the input
			// type must
			// match
			if (src.Size() < (int)(srcStart + count))
			{
				throw new System.ArgumentException();
			}
			if (count == 0)
			{
				return;
			}
			if (Size() < count + startElement)
			{
				Resize(count + startElement);
			}
			if (_src == (com.epl.geometry.AttributeStreamBase)this)
			{
				_selfWriteRangeImpl(startElement, count, srcStart, bForward, stride);
				return;
			}
			if (bForward)
			{
				System.Array.Copy(src.m_buffer, srcStart, m_buffer, startElement, count);
			}
			else
			{
				// int j = startElement;
				// int offset = srcStart;
				// for (int i = 0; i < count; i++)
				// {
				// m_buffer[j] = src.m_buffer[offset];
				// j++;
				// offset++;
				// }
				int j = startElement;
				int offset = srcStart + count - stride;
				if (stride == 1)
				{
					for (int i = 0; i < count; i++)
					{
						m_buffer[j] = src.m_buffer[offset];
						j++;
						offset--;
					}
				}
				else
				{
					for (int i = 0, n = count / stride; i < n; i++)
					{
						for (int k = 0; k < stride; k++)
						{
							m_buffer[j + k] = src.m_buffer[offset + k];
						}
						j += stride;
						offset -= stride;
					}
				}
			}
		}

		private void _selfWriteRangeImpl(int toElement, int count, int fromElement, bool bForward, int stride)
		{
			// writing from to this stream.
			if (bForward)
			{
				if (toElement == fromElement)
				{
					return;
				}
			}
			System.Array.Copy(m_buffer, fromElement, m_buffer, toElement, count);
			if (bForward)
			{
				return;
			}
			// reverse what we written
			int j = toElement;
			int offset = toElement + count - stride;
			int dj = stride;
			for (int i = 0, n = count / 2; i < n; i++)
			{
				for (int k = 0; k < stride; k++)
				{
					int v = m_buffer[j + k];
					m_buffer[j + k] = m_buffer[offset + k];
					m_buffer[offset + k] = v;
				}
				j += stride;
				offset -= stride;
			}
		}

		public override void WriteRange(int startElement, int count, System.IO.BinaryReader src, int offsetBytes, bool bForward)
		{
			if (startElement < 0 || count < 0 || offsetBytes < 0)
			{
				throw new System.ArgumentException();
			}
			int elmSize = com.epl.geometry.NumberUtils.SizeOf((double)0);
			if (src.BaseStream.Length < (int)(offsetBytes + elmSize * count))
			{
				throw new System.ArgumentException();
			}
			if (count == 0)
			{
				return;
			}
			if (Size() < count + startElement)
			{
				Resize(count + startElement);
			}
			int j = startElement;
			if (!bForward)
			{
				j += count - 1;
			}
			int dj = bForward ? 1 : -1;
			int offset = offsetBytes;
			for (int i = 0; i < count; i++, offset += elmSize)
			{
				m_buffer[j] = src.ReadInt32();
				j += dj;
			}
		}

		public abstract class IntComparator
		{
			public abstract int Compare(int v1, int v2);
		}

		internal class RandomSeed
		{
			public int random;

			public RandomSeed()
			{
				random = 1973;
			}
		}

		public void Sort(int start, int end, com.epl.geometry.AttributeStreamOfInt32.IntComparator compare)
		{
			if (end - start < 10)
			{
				Insertionsort(start, end, compare);
			}
			else
			{
				Quicksort(start, end - 1, compare, new com.epl.geometry.AttributeStreamOfInt32.RandomSeed());
			}
		}

		internal void Insertionsort(int start, int end, com.epl.geometry.AttributeStreamOfInt32.IntComparator compare)
		{
			for (int j = start; j < end; j++)
			{
				// insertion sort
				int key = m_buffer[j];
				int i = j - 1;
				while (i >= start && compare.Compare(m_buffer[i], key) > 0)
				{
					m_buffer[i + 1] = m_buffer[i];
					i--;
				}
				m_buffer[i + 1] = key;
			}
		}

		internal void Swap(int left, int right)
		{
			int tmp = m_buffer[right];
			m_buffer[right] = m_buffer[left];
			m_buffer[left] = tmp;
		}

		internal void Quicksort(int leftIn, int rightIn, com.epl.geometry.AttributeStreamOfInt32.IntComparator compare, com.epl.geometry.AttributeStreamOfInt32.RandomSeed seed)
		{
			if (leftIn >= rightIn)
			{
				return;
			}
			int left = leftIn;
			int right = rightIn;
			while (true)
			{
				// tail recursion loop
				if (right - left < 9)
				{
					Insertionsort(left, right + 1, compare);
					return;
				}
				// Select random index for the pivot
				seed.random = com.epl.geometry.NumberUtils.NextRand(seed.random);
				long nom = ((long)(right - left)) * seed.random;
				int pivotIndex = (int)(nom / com.epl.geometry.NumberUtils.IntMax()) + left;
				// Get the pivot value
				int pivotValue = m_buffer[pivotIndex];
				// Start partition
				// Move pivot to the right
				Swap(pivotIndex, right);
				int storeIndex = left;
				for (int i = left; i < right; i++)
				{
					int elm = m_buffer[i];
					if (compare.Compare(elm, pivotValue) <= 0)
					{
						Swap(storeIndex, i);
						storeIndex = storeIndex + 1;
					}
				}
				// Move pivot to its final place
				Swap(storeIndex, right);
				// End partition
				// Shorter part is regular recursion
				// Longer part is tail recursion
				if (storeIndex - left < right - storeIndex)
				{
					Quicksort(left, storeIndex - 1, compare, seed);
					left = storeIndex + 1;
				}
				else
				{
					Quicksort(storeIndex + 1, right, compare, seed);
					right = storeIndex - 1;
				}
			}
		}

		public void Sort(int start, int end)
		{
			System.Array.Sort(m_buffer, start, end);
		}
	}
}
