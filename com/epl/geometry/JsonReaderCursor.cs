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
Environmental Systems Research Institute, Inc.
Attn: Contracts Dept
380 New York Street
Redlands, California, USA 92373

email: contracts@esri.com
*/
/*
COPYRIGHT 1995-2017 ESRI

TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
Unpublished material - all rights reserved under the
Copyright Laws of the United States.

For additional information, contact:

email: info@echoparklabs.io
*/


namespace com.epl.geometry
{
	/// <summary>An abstract JsonReader Cursor class.</summary>
	internal abstract class JsonReaderCursor
	{
		/// <summary>Moves the cursor to the next JsonReader.</summary>
		/// <remarks>
		/// Moves the cursor to the next JsonReader. Returns null when reached the
		/// end.
		/// </remarks>
		public abstract com.epl.geometry.JsonReader Next();

		/// <summary>Returns the ID of the current geometry.</summary>
		/// <remarks>
		/// Returns the ID of the current geometry. The ID is propagated across the
		/// operations (when possible).
		/// Returns an ID associated with the current Geometry. The ID is passed
		/// along and is returned by some operators to preserve relationship between
		/// the input and output geometry classes. It is not always possible to
		/// preserve an ID during an operation.
		/// </remarks>
		public abstract int GetID();
	}
}
