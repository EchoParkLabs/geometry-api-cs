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
	/// <summary>Base class for AttributeStream instances.</summary>
	internal abstract class AttributeStreamBase
	{
		protected internal bool m_bLockedInSize;

		protected internal bool m_bReadonly;

		public AttributeStreamBase()
		{
			m_bReadonly = false;
			m_bLockedInSize = false;
		}

		/// <summary>Returns the number of elements in the stream.</summary>
		public abstract int VirtualSize();

		/// <summary>Returns the Persistence type of the stream.</summary>
		public abstract int GetPersistence();

		/// <summary>Reads given element and returns it as double.</summary>
		public abstract double ReadAsDbl(int offset);

		/// <summary>Writes given element as double.</summary>
		/// <remarks>
		/// Writes given element as double. The double is cast to the internal
		/// representation (truncated when int).
		/// </remarks>
		public abstract void WriteAsDbl(int offset, double d);

		/// <summary>Reads given element and returns it as int (truncated if double).</summary>
		public abstract int ReadAsInt(int offset);

		/// <summary>Writes given element as int.</summary>
		/// <remarks>
		/// Writes given element as int. The int is cast to the internal
		/// representation.
		/// </remarks>
		public abstract void WriteAsInt(int offset, int d);

		/// <summary>Reads given element and returns it as int (truncated if double).</summary>
		public abstract long ReadAsInt64(int offset);

		/// <summary>Writes given element as int.</summary>
		/// <remarks>
		/// Writes given element as int. The int is cast to the internal
		/// representation.
		/// </remarks>
		public abstract void WriteAsInt64(int offset, long d);

		/// <summary>Resizes the AttributeStream to the new size.</summary>
		public abstract void Resize(int newSize, double defaultValue);

		/// <summary>Resizes the AttributeStream to the new size.</summary>
		public abstract void Resize(int newSize);

		/// <summary>Resizes the AttributeStream to the new size.</summary>
		/// <remarks>
		/// Resizes the AttributeStream to the new size. Does not change the capacity
		/// of the stream.
		/// </remarks>
		public abstract void ResizePreserveCapacity(int newSize);

		// java only method
		/// <summary>Same as resize(0)</summary>
		internal virtual void Clear(bool bFreeMemory)
		{
			if (bFreeMemory)
			{
				Resize(0);
			}
			else
			{
				ResizePreserveCapacity(0);
			}
		}

		/// <summary>Adds a range of elements from the source stream.</summary>
		/// <remarks>
		/// Adds a range of elements from the source stream. The streams must be of
		/// the same type.
		/// </remarks>
		/// <param name="src">The source stream to read elements from.</param>
		/// <param name="srcStart">
		/// The index of the element in the source stream to start reading
		/// from.
		/// </param>
		/// <param name="count">The number of elements to add.</param>
		/// <param name="bForward">
		/// True if adding the elements in order of the incoming source
		/// stream. False if adding the elements in reverse.
		/// </param>
		/// <param name="stride">
		/// The number of elements to be grouped together if adding the
		/// elements in reverse.
		/// </param>
		public abstract void AddRange(com.epl.geometry.AttributeStreamBase src, int srcStart, int count, bool bForward, int stride);

		/// <summary>Inserts a range of elements from the source stream.</summary>
		/// <remarks>
		/// Inserts a range of elements from the source stream. The streams must be
		/// of the same type.
		/// </remarks>
		/// <param name="start">The index where to start the insert.</param>
		/// <param name="src">The source stream to read elements from.</param>
		/// <param name="srcStart">
		/// The index of the element in the source stream to start reading
		/// from.
		/// </param>
		/// <param name="count">The number of elements to read from the source stream.</param>
		/// <param name="validSize">The number of valid elements in this stream.</param>
		public abstract void InsertRange(int start, com.epl.geometry.AttributeStreamBase src, int srcStart, int count, bool bForward, int stride, int validSize);

		/// <summary>Inserts a range of elements of the given value.</summary>
		/// <param name="start">The index where to start the insert.</param>
		/// <param name="value">The value to be inserted.</param>
		/// <param name="count">The number of elements to be inserted.</param>
		/// <param name="validSize">The number of valid elements in this stream.</param>
		public abstract void InsertRange(int start, double value, int count, int validSize);

		/// <summary>Inserts the attributes of a given semantics from a Point geometry.</summary>
		/// <param name="start">The index where to start the insert.</param>
		/// <param name="pt">The Point geometry holding the attributes to be inserted.</param>
		/// <param name="semantics">The attribute semantics that are being inserted.</param>
		/// <param name="validSize">The number of valid elements in this stream.</param>
		public abstract void InsertAttributes(int start, com.epl.geometry.Point pt, int semantics, int validSize);

		/// <summary>Sets a range of values to given value.</summary>
		/// <param name="value">The value to set stream elements to.</param>
		/// <param name="start">The index of the element to start writing to.</param>
		/// <param name="count">The number of elements to set.</param>
		public abstract void SetRange(double value, int start, int count);

		/// <summary>Adds a range of elements from the source byte buffer.</summary>
		/// <remarks>
		/// Adds a range of elements from the source byte buffer. This stream is
		/// resized automatically to accomodate required number of elements.
		/// </remarks>
		/// <param name="startElement">
		/// the index of the element in this stream to start setting
		/// elements from.
		/// </param>
		/// <param name="count">The number of AttributeStream elements to read.</param>
		/// <param name="src">The source ByteBuffer to read elements from.</param>
		/// <param name="sourceStart">The offset from the start of the ByteBuffer in bytes.</param>
		/// <param name="bForward">When False, the source is written in reversed order.</param>
		/// <param name="stride">
		/// Used for reversed writing only to indicate the unit of
		/// writing. elements inside a stride are not reversed. Only the
		/// strides are reversed.
		/// </param>
		public abstract void WriteRange(int startElement, int count, com.epl.geometry.AttributeStreamBase src, int sourceStart, bool bForward, int stride);

		/// <summary>Adds a range of elements from the source byte buffer.</summary>
		/// <remarks>
		/// Adds a range of elements from the source byte buffer. The stream is
		/// resized automatically to accomodate required number of elements.
		/// </remarks>
		/// <param name="startElement">
		/// the index of the element in this stream to start setting
		/// elements from.
		/// </param>
		/// <param name="count">The number of AttributeStream elements to read.</param>
		/// <param name="src">The source ByteBuffer to read elements from.</param>
		/// <param name="offsetBytes">The offset from the start of the ByteBuffer in bytes.</param>
		public abstract void WriteRange(int startElement, int count, System.IO.BinaryReader src, int offsetBytes, bool bForward);

		/// <summary>Write a range of elements to the source byte buffer.</summary>
		/// <param name="srcStart">The element index to start writing from.</param>
		/// <param name="count">The number of AttributeStream elements to write.</param>
		/// <param name="dst">
		/// The destination ByteBuffer. The buffer must be large enough or
		/// it will throw.
		/// </param>
		/// <param name="dstOffsetBytes">
		/// The offset in the destination ByteBuffer to start write
		/// elements from.
		/// </param>
		public abstract void ReadRange(int srcStart, int count, System.IO.BinaryWriter dst, int dstOffset, bool bForward);

		/// <summary>Erases a range from the buffer and defragments the result.</summary>
		/// <param name="index">The index in this stream where the erasing starts.</param>
		/// <param name="count">The number of elements to be erased.</param>
		/// <param name="validSize">The number of valid elements in this stream.</param>
		public abstract void EraseRange(int index, int count, int validSize);

		/// <summary>Reverses a range from the buffer.</summary>
		/// <param name="index">The index in this stream where the reversing starts.</param>
		/// <param name="count">The number of elements to be reversed.</param>
		/// <param name="stride">
		/// The number of elements to be grouped together when doing the
		/// reverse.
		/// </param>
		public abstract void ReverseRange(int index, int count, int stride);

		/// <summary>Creates a new attribute stream for storing bytes.</summary>
		/// <param name="size">The number of elements in the stream.</param>
		public static com.epl.geometry.AttributeStreamBase CreateByteStream(int size)
		{
			com.epl.geometry.AttributeStreamBase newStream = new com.epl.geometry.AttributeStreamOfInt8(size);
			return newStream;
		}

		/// <summary>Creates a new attribute stream for storing bytes.</summary>
		/// <param name="size">The number of elements in the stream.</param>
		/// <param name="defaultValue">The default value to fill the stream with.</param>
		public static com.epl.geometry.AttributeStreamBase CreateByteStream(int size, byte defaultValue)
		{
			com.epl.geometry.AttributeStreamBase newStream = new com.epl.geometry.AttributeStreamOfInt8(size, defaultValue);
			return newStream;
		}

		/// <summary>Creates a new attribute stream for storing doubles.</summary>
		/// <param name="size">The number of elements in the stream.</param>
		public static com.epl.geometry.AttributeStreamBase CreateDoubleStream(int size)
		{
			com.epl.geometry.AttributeStreamBase newStream = new com.epl.geometry.AttributeStreamOfDbl(size);
			return newStream;
		}

		/// <summary>Creates a new attribute stream for storing doubles.</summary>
		/// <param name="size">The number of elements in the stream.</param>
		/// <param name="defaultValue">The default value to fill the stream with.</param>
		public static com.epl.geometry.AttributeStreamBase CreateDoubleStream(int size, double defaultValue)
		{
			com.epl.geometry.AttributeStreamBase newStream = new com.epl.geometry.AttributeStreamOfDbl(size, defaultValue);
			return newStream;
		}

		/// <summary>Creats a copy of the stream that contains upto maxsize elements.</summary>
		public abstract com.epl.geometry.AttributeStreamBase RestrictedClone(int maxsize);

		/// <summary>Makes the stream to be readonly.</summary>
		/// <remarks>
		/// Makes the stream to be readonly. Any operation that changes the content
		/// or size of the stream will throw.
		/// </remarks>
		public virtual void SetReadonly()
		{
			m_bReadonly = true;
			m_bLockedInSize = true;
		}

		public virtual bool IsReadonly()
		{
			return m_bReadonly;
		}

		/// <summary>Lock the size of the stream.</summary>
		/// <remarks>
		/// Lock the size of the stream. Any operation that changes the size of the
		/// stream will throw.
		/// </remarks>
		public virtual void LockSize()
		{
			m_bLockedInSize = true;
		}

		public virtual bool IsLockedSize()
		{
			return m_bLockedInSize;
		}

		/// <summary>Creates a new attribute stream of given persistence type and size.</summary>
		/// <param name="persistence">The persistence type of the stream (see VertexDescription).</param>
		/// <param name="size">
		/// The number of elements (floats, doubles, or 32 bit integers)
		/// of the given type in the stream.
		/// </param>
		public static com.epl.geometry.AttributeStreamBase CreateAttributeStreamWithPersistence(int persistence, int size)
		{
			com.epl.geometry.AttributeStreamBase newStream;
			switch (persistence)
			{
				case (com.epl.geometry.VertexDescription.Persistence.enumFloat):
				{
					newStream = new com.epl.geometry.AttributeStreamOfFloat(size);
					break;
				}

				case (com.epl.geometry.VertexDescription.Persistence.enumDouble):
				{
					newStream = new com.epl.geometry.AttributeStreamOfDbl(size);
					break;
				}

				case (com.epl.geometry.VertexDescription.Persistence.enumInt32):
				{
					newStream = new com.epl.geometry.AttributeStreamOfInt32(size);
					break;
				}

				case (com.epl.geometry.VertexDescription.Persistence.enumInt64):
				{
					newStream = new com.epl.geometry.AttributeStreamOfInt64(size);
					break;
				}

				case (com.epl.geometry.VertexDescription.Persistence.enumInt8):
				{
					newStream = new com.epl.geometry.AttributeStreamOfInt8(size);
					break;
				}

				case (com.epl.geometry.VertexDescription.Persistence.enumInt16):
				{
					newStream = new com.epl.geometry.AttributeStreamOfInt16(size);
					break;
				}

				default:
				{
					throw new com.epl.geometry.GeometryException("Internal Error");
				}
			}
			return newStream;
		}

		/// <summary>Creates a new attribute stream of given persistence type and size.</summary>
		/// <param name="persistence">The persistence type of the stream (see VertexDescription).</param>
		/// <param name="size">
		/// The number of elements (floats, doubles, or 32 bit integers)
		/// of the given type in the stream.
		/// </param>
		/// <param name="defaultValue">The default value to fill the stream with.</param>
		public static com.epl.geometry.AttributeStreamBase CreateAttributeStreamWithPersistence(int persistence, int size, double defaultValue)
		{
			com.epl.geometry.AttributeStreamBase newStream;
			switch (persistence)
			{
				case (com.epl.geometry.VertexDescription.Persistence.enumFloat):
				{
					newStream = new com.epl.geometry.AttributeStreamOfFloat(size, (float)defaultValue);
					break;
				}

				case (com.epl.geometry.VertexDescription.Persistence.enumDouble):
				{
					newStream = new com.epl.geometry.AttributeStreamOfDbl(size, (double)defaultValue);
					break;
				}

				case (com.epl.geometry.VertexDescription.Persistence.enumInt32):
				{
					newStream = new com.epl.geometry.AttributeStreamOfInt32(size, (int)defaultValue);
					break;
				}

				case (com.epl.geometry.VertexDescription.Persistence.enumInt64):
				{
					newStream = new com.epl.geometry.AttributeStreamOfInt64(size, (long)defaultValue);
					break;
				}

				case (com.epl.geometry.VertexDescription.Persistence.enumInt8):
				{
					newStream = new com.epl.geometry.AttributeStreamOfInt8(size, unchecked((byte)defaultValue));
					break;
				}

				case (com.epl.geometry.VertexDescription.Persistence.enumInt16):
				{
					newStream = new com.epl.geometry.AttributeStreamOfInt16(size, (short)defaultValue);
					break;
				}

				default:
				{
					throw new com.epl.geometry.GeometryException("Internal Error");
				}
			}
			return newStream;
		}

		/// <summary>Creates a new attribute stream for the given semantics and vertex count.</summary>
		/// <param name="semantics">The semantics of the attribute (see VertexDescription).</param>
		/// <param name="vertexCount">
		/// The number of vertices in the geometry. The actual number of
		/// elements in the stream is vertexCount * ncomponents.
		/// </param>
		public static com.epl.geometry.AttributeStreamBase CreateAttributeStreamWithSemantics(int semantics, int vertexCount)
		{
			int ncomps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
			int persistence = com.epl.geometry.VertexDescription.GetPersistence(semantics);
			return CreateAttributeStreamWithPersistence(persistence, vertexCount * ncomps, com.epl.geometry.VertexDescription.GetDefaultValue(semantics));
		}

		/// <summary>Creates a new attribute stream for storing vertex indices.</summary>
		/// <param name="size">The number of elements in the stream.</param>
		public static com.epl.geometry.AttributeStreamBase CreateIndexStream(int size)
		{
			int persistence = com.epl.geometry.VertexDescription.Persistence.enumInt32;
			// VertexDescription.getPersistenceFromInt(NumberUtils::SizeOf((int)0));
			com.epl.geometry.AttributeStreamBase newStream;
			switch (persistence)
			{
				case (com.epl.geometry.VertexDescription.Persistence.enumInt32):
				{
					newStream = new com.epl.geometry.AttributeStreamOfInt32(size);
					break;
				}

				case (com.epl.geometry.VertexDescription.Persistence.enumInt64):
				{
					newStream = new com.epl.geometry.AttributeStreamOfInt64(size);
					break;
				}

				default:
				{
					throw new com.epl.geometry.GeometryException("Internal Error");
				}
			}
			return newStream;
		}

		/// <summary>Creates a new attribute stream for storing vertex indices.</summary>
		/// <param name="size">The number of elements in the stream.</param>
		/// <param name="defaultValue">The default value to fill the stream with.</param>
		public static com.epl.geometry.AttributeStreamBase CreateIndexStream(int size, int defaultValue)
		{
			int persistence = com.epl.geometry.VertexDescription.Persistence.enumInt32;
			// VertexDescription.getPersistenceFromInt(NumberUtils::SizeOf((int)0));
			com.epl.geometry.AttributeStreamBase newStream;
			switch (persistence)
			{
				case (com.epl.geometry.VertexDescription.Persistence.enumInt32):
				{
					newStream = new com.epl.geometry.AttributeStreamOfInt32(size, (int)defaultValue);
					break;
				}

				case (com.epl.geometry.VertexDescription.Persistence.enumInt64):
				{
					newStream = new com.epl.geometry.AttributeStreamOfInt64(size, (long)defaultValue);
					break;
				}

				default:
				{
					throw new com.epl.geometry.GeometryException("Internal Error");
				}
			}
			return newStream;
		}

		public abstract int CalculateHashImpl(int hashCode, int start, int end);

		public abstract bool Equals(com.epl.geometry.AttributeStreamBase other, int start, int end);
	}
}
