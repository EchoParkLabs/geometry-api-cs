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
	internal sealed class AttributeStreamOfDbl : com.epl.geometry.AttributeStreamBase
	{
		private double[] m_buffer = null;

		private int m_size;

		public int Size()
		{
			return m_size;
		}

		public void Reserve(int reserve)
		{
			if (reserve <= 0)
			{
				return;
			}
			if (m_buffer == null)
			{
				m_buffer = new double[reserve];
			}
			else
			{
				if (reserve <= m_buffer.Length)
				{
					return;
				}
				double[] buf = new double[reserve];
				System.Array.Copy(m_buffer, 0, buf, 0, m_size);
				m_buffer = buf;
			}
		}

		public int Capacity()
		{
			return m_buffer != null ? m_buffer.Length : 0;
		}

		public AttributeStreamOfDbl(int size)
		{
			int sz = size;
			if (sz < 2)
			{
				sz = 2;
			}
			m_buffer = new double[sz];
			m_size = size;
		}

		public AttributeStreamOfDbl(int size, double defaultValue)
		{
			int sz = size;
			if (sz < 2)
			{
				sz = 2;
			}
			m_buffer = new double[sz];
			m_size = size;
			java.util.Arrays.Fill(m_buffer, 0, size, defaultValue);
		}

		public AttributeStreamOfDbl(com.epl.geometry.AttributeStreamOfDbl other)
		{
			m_buffer = (double[])other.m_buffer.Clone();
			m_size = other.m_size;
		}

		public AttributeStreamOfDbl(com.epl.geometry.AttributeStreamOfDbl other, int maxSize)
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
			m_buffer = new double[sz];
			System.Array.Copy(other.m_buffer, 0, m_buffer, 0, m_size);
		}

		/// <summary>Reads a value from the buffer at given offset.</summary>
		/// <param name="offset">is the element number in the stream.</param>
		public double Read(int offset)
		{
			return m_buffer[offset];
		}

		public double Get(int offset)
		{
			return m_buffer[offset];
		}

		/// <summary>Overwrites given element with new value.</summary>
		/// <param name="offset">is the element number in the stream.</param>
		/// <param name="value">is the value to write.</param>
		public void Write(int offset, double value)
		{
			if (m_bReadonly)
			{
				throw new System.Exception("invalid_call");
			}
			m_buffer[offset] = value;
		}

		public void Set(int offset, double value)
		{
			if (m_bReadonly)
			{
				throw new System.Exception("invalid_call");
			}
			m_buffer[offset] = value;
		}

		/// <summary>Reads a value from the buffer at given offset.</summary>
		/// <param name="offset">is the element number in the stream.</param>
		public void Read(int offset, com.epl.geometry.Point2D outPoint)
		{
			outPoint.x = m_buffer[offset];
			outPoint.y = m_buffer[offset + 1];
		}

		/// <summary>Overwrites given element with new value.</summary>
		/// <param name="offset">is the element number in the stream.</param>
		/// <param name="value">is the value to write.</param>
		internal void Write(int offset, com.epl.geometry.Point2D point)
		{
			if (m_bReadonly)
			{
				throw new System.Exception("invalid_call");
			}
			m_buffer[offset] = point.x;
			m_buffer[offset + 1] = point.y;
		}

		/// <summary>Adds a new value at the end of the stream.</summary>
		public void Add(double v)
		{
			Resize(m_size + 1);
			m_buffer[m_size - 1] = v;
		}

		public override com.epl.geometry.AttributeStreamBase RestrictedClone(int maxsize)
		{
			com.epl.geometry.AttributeStreamOfDbl clone = new com.epl.geometry.AttributeStreamOfDbl(this, maxsize);
			return clone;
		}

		public override int VirtualSize()
		{
			return Size();
		}

		// @Override
		// public void addRange(AttributeStreamBase src, int srcStartIndex, int
		// count) {
		// if ((src == this) || !(src instanceof AttributeStreamOfDbl))
		// throw new IllegalArgumentException();
		//
		// AttributeStreamOfDbl as = (AttributeStreamOfDbl) src;
		//
		// int len = as.size();
		// int oldSize = m_size;
		// resize(oldSize + len, 0);
		// for (int i = 0; i < len; i++) {
		// m_buffer[oldSize + i] = as.read(i);
		// }
		// }
		public override int GetPersistence()
		{
			return com.epl.geometry.VertexDescription.Persistence.enumDouble;
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
					double[] newBuffer = new double[newSize];
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
					double[] newBuffer = new double[sz];
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
					double[] newBuffer = new double[newSize];
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
					double[] newBuffer = new double[sz];
					System.Array.Copy(m_buffer, 0, newBuffer, 0, m_size);
					m_buffer = newBuffer;
				}
				java.util.Arrays.Fill(m_buffer, m_size, newSize, defaultValue);
				m_size = newSize;
			}
		}

		public override void WriteAsDbl(int offset, double d)
		{
			Write(offset, d);
		}

		public override void WriteAsInt64(int offset, long d)
		{
			Write(offset, (double)d);
		}

		public override void WriteAsInt(int offset, int d)
		{
			Write(offset, (double)d);
		}

		/// <summary>Sets the envelope from the attribute stream.</summary>
		/// <remarks>
		/// Sets the envelope from the attribute stream. The attribute stream stores
		/// interleaved x and y. The envelope will be set to empty if the pointCount
		/// is zero.
		/// </remarks>
		public void SetEnvelopeFromPoints(int pointCount, com.epl.geometry.Envelope2D inOutEnv)
		{
			if (pointCount == 0)
			{
				inOutEnv.SetEmpty();
				return;
			}
			if (pointCount < 0)
			{
				pointCount = Size() / 2;
			}
			else
			{
				if (pointCount * 2 > Size())
				{
					throw new System.ArgumentException();
				}
			}
			inOutEnv.SetCoords(Read(0), Read(1));
			for (int i = 1; i < pointCount; i++)
			{
				inOutEnv.MergeNE(Read(i * 2), Read(i * 2 + 1));
			}
		}

		public override int CalculateHashImpl(int hashCodeIn, int start, int end)
		{
			int hashCode = hashCodeIn;
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
			if (!(other is com.epl.geometry.AttributeStreamOfDbl))
			{
				return false;
			}
			com.epl.geometry.AttributeStreamOfDbl _other = (com.epl.geometry.AttributeStreamOfDbl)other;
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
				System.Array.Copy(((com.epl.geometry.AttributeStreamOfDbl)src).m_buffer, start, m_buffer, oldSize, count);
			}
			else
			{
				int n = count;
				for (int i = 0; i < count; i += stride)
				{
					n -= stride;
					for (int s = 0; s < stride; s++)
					{
						m_buffer[oldSize + i + s] = ((com.epl.geometry.AttributeStreamOfDbl)src).m_buffer[start + n + s];
					}
				}
			}
		}

		// public void addRange(AttributeStreamBase src, int start,
		// int count, boolean bForward, int stride) {
		//
		// if (m_bReadonly)
		// throw new GeometryException("invalid_call");
		//
		// if (!bForward && (stride < 1 || count % stride != 0))
		// throw new IllegalArgumentException();
		//
		// if (bForward)
		// {
		// double[] otherbuffer = ((AttributeStreamOfDbl) src).m_buffer;
		// // int newSize = size() + count;
		// // resize(newSize);
		// // System.arraycopy(otherbuffer, start, m_buffer, pos, count);
		// for (int i = 0; i < count; i++) {
		// add(otherbuffer[start + i]);
		// }
		// } else {
		// throw new GeometryException("not implemented for reverse add");
		// }
		// }
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
			int excess_space = m_size - validSize;
			if (excess_space < count)
			{
				int original_size = m_size;
				Resize(original_size + count - excess_space);
			}
			System.Array.Copy(m_buffer, start, m_buffer, start + count, validSize - start);
			if (m_buffer == ((com.epl.geometry.AttributeStreamOfDbl)src).m_buffer)
			{
				if (start < srcStart)
				{
					srcStart += count;
				}
			}
			if (bForward)
			{
				System.Array.Copy(((com.epl.geometry.AttributeStreamOfDbl)src).m_buffer, srcStart, m_buffer, start, count);
			}
			else
			{
				int n = count;
				for (int i = 0; i < count; i += stride)
				{
					n -= stride;
					for (int s = 0; s < stride; s++)
					{
						m_buffer[start + i + s] = ((com.epl.geometry.AttributeStreamOfDbl)src).m_buffer[srcStart + n + s];
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
			for (int i = 0; i < count; i++)
			{
				m_buffer[start + i] = value;
			}
		}

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
				m_buffer[start + c] = pt.GetAttributeAsDbl(semantics, c);
			}
		}

		public void Insert(int index, com.epl.geometry.Point2D point, int validSize)
		{
			if (m_bReadonly)
			{
				throw new com.epl.geometry.GeometryException("invalid_call");
			}
			System.Array.Copy(m_buffer, index, m_buffer, index + 2, validSize - index);
			m_buffer[index] = point.x;
			m_buffer[index + 1] = point.y;
		}

		// special case for .net 2d array syntax [,]
		// writes count doubles, 2 at a time, into this stream. count is assumed to
		// be even, arrayOffset is an index of the zeroth dimension (i.e. you can't
		// start writing from dst[0,1])
		public void WriteRange(int streamOffset, int count, double[][] src, int arrayOffset, bool bForward)
		{
			if (streamOffset < 0 || count < 0 || arrayOffset < 0 || count > com.epl.geometry.NumberUtils.IntMax())
			{
				throw new System.ArgumentException();
			}
			if (src.Length * 2 < (int)((arrayOffset << 1) + count))
			{
				throw new System.ArgumentException();
			}
			if (count == 0)
			{
				return;
			}
			if (Size() < count + streamOffset)
			{
				Resize(count + streamOffset);
			}
			int j = streamOffset;
			if (!bForward)
			{
				j += count - 1;
			}
			int dj = bForward ? 2 : -2;
			int end = arrayOffset + (count >> 1);
			for (int i = arrayOffset; i < end; i++)
			{
				m_buffer[j] = (double)src[i][0];
				m_buffer[j + 1] = (double)src[i][1];
				j += dj;
			}
		}

		public void WriteRange(int streamOffset, int count, double[] src, int arrayOffset, bool bForward)
		{
			if (streamOffset < 0 || count < 0 || arrayOffset < 0)
			{
				throw new System.ArgumentException();
			}
			if (src.Length < arrayOffset + count)
			{
				throw new System.ArgumentException();
			}
			if (count == 0)
			{
				return;
			}
			if (Size() < count + streamOffset)
			{
				Resize(count + streamOffset);
			}
			if (bForward)
			{
				System.Array.Copy(src, arrayOffset, m_buffer, streamOffset, count);
			}
			else
			{
				int j = streamOffset;
				if (!bForward)
				{
					j += count - 1;
				}
				int end = arrayOffset + count;
				for (int i = arrayOffset; i < end; i++)
				{
					m_buffer[j] = src[i];
					j--;
				}
			}
		}

		// reads count doubles, 2 at a time, into dst. count is assumed to be even,
		// arrayOffset is an index of the zeroth dimension (i.e. you can't start
		// reading into dst[0,1])
		// void AttributeStreamOfDbl::ReadRange(int streamOffset, int count,
		// array<double,2>^ dst, int arrayOffset, bool bForward)
		public void ReadRange(int streamOffset, int count, double[][] dst, int arrayOffset, bool bForward)
		{
			if (streamOffset < 0 || count < 0 || arrayOffset < 0 || count > com.epl.geometry.NumberUtils.IntMax() || Size() < count + streamOffset)
			{
				throw new System.ArgumentException();
			}
			if (dst.Length * 2 < (int)((arrayOffset << 1) + count))
			{
				throw new System.ArgumentException();
			}
			if (count == 0)
			{
				return;
			}
			int j = streamOffset;
			if (!bForward)
			{
				j += count - 1;
			}
			int dj = bForward ? 2 : -2;
			int end = arrayOffset + (count >> 1);
			for (int i = arrayOffset; i < end; i++)
			{
				dst[i][0] = m_buffer[j];
				dst[i][1] = m_buffer[j + 1];
				j += dj;
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
			if (validSize - (index + count) > 0)
			{
				System.Array.Copy(m_buffer, index + count, m_buffer, index, validSize - (index + count));
			}
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
					double temp = m_buffer[index + i + s];
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
			double v = value;
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
			com.epl.geometry.AttributeStreamOfDbl src = (com.epl.geometry.AttributeStreamOfDbl)_src;
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
				int j = startElement;
				int offset = srcStart;
				for (int i = 0; i < count; i++)
				{
					m_buffer[j] = src.m_buffer[offset];
					j++;
					offset++;
				}
			}
			else
			{
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
					double v = m_buffer[j + k];
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
				m_buffer[j] = src.ReadDouble();
				j += dj;
			}
		}

		public void WriteRange(int streamOffset, int pointCount, com.epl.geometry.Point2D[] src, int arrayOffset, bool bForward)
		{
			if (streamOffset < 0 || pointCount < 0 || arrayOffset < 0)
			{
				throw new System.ArgumentException();
			}
			// if (src->Length < (int)(arrayOffset + pointCount)) jt: we have lost
			// the length check, not sure about this
			// GEOMTHROW(invalid_argument);
			if (pointCount == 0)
			{
				return;
			}
			if (Size() < (pointCount << 1) + streamOffset)
			{
				Resize((pointCount << 1) + streamOffset);
			}
			int j = streamOffset;
			if (!bForward)
			{
				j += (pointCount - 1) << 1;
			}
			int dj = bForward ? 2 : -2;
			// TODO: refactor to take advantage of the known block array structure
			int i0 = arrayOffset;
			pointCount += i0;
			for (int i = i0; i < pointCount; i++)
			{
				m_buffer[j] = src[i].x;
				m_buffer[j + 1] = src[i].y;
				j += dj;
			}
		}

		// Less efficient as boolean bForward set to false, as it is looping through
		// half
		// of the elements of the array
		public void ReadRange(int srcStart, int count, double[] dst, int dstOffset, bool bForward)
		{
			if (srcStart < 0 || count < 0 || dstOffset < 0 || Size() < count + srcStart)
			{
				throw new System.ArgumentException();
			}
			if (bForward)
			{
				System.Array.Copy(m_buffer, srcStart, dst, dstOffset, count);
			}
			else
			{
				int j = dstOffset + count - 1;
				for (int i = srcStart; i < count; i++)
				{
					dst[j] = m_buffer[i];
					j--;
				}
			}
		}

		public void Sort(int start, int end)
		{
			System.Array.Sort(m_buffer, start, end - start);
		}
	}
}
