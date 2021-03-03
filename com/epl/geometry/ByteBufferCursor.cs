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
	/// <summary>An abstract ByteBuffer Cursor class.</summary>
	internal abstract class ByteBufferCursor
	{
		/// <summary>Moves the cursor to the next ByteBuffer.</summary>
		/// <remarks>
		/// Moves the cursor to the next ByteBuffer. Returns null when reached the
		/// end.
		/// </remarks>
		public abstract System.IO.MemoryStream Next();

		/// <summary>Returns the ID of the current ByteBuffer.</summary>
		/// <remarks>
		/// Returns the ID of the current ByteBuffer. The ID is propagated across the
		/// operations (when possible).
		/// Returns an ID associated with the current Geometry. The ID is passed
		/// along and is returned by some operators to preserve relationship between
		/// the input and output geometry classes. It is not always possible to
		/// preserve an ID during an operation.
		/// </remarks>
		public abstract int GetByteBufferID();
	}
}
