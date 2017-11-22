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
	internal abstract class DirtyFlags
	{
		public const int dirtyIsKnownSimple = 1;

		public const int isWeakSimple = 2;

		public const int isStrongSimple = 4;

		public const int dirtyOGCFlags = 8;

		public const int dirtyVerifiedStreams = 32;

		public const int dirtyExactIntervals = 64;

		public const int dirtyLooseIntervals = 128;

		public const int dirtyIntervals = dirtyExactIntervals | dirtyLooseIntervals;

		public const int dirtyIsEnvelope = 256;

		public const int dirtyLength2D = 512;

		public const int dirtyRingAreas2D = 1024;

		public const int dirtyCoordinates = dirtyIsKnownSimple | dirtyIntervals | dirtyIsEnvelope | dirtyLength2D | dirtyRingAreas2D | dirtyOGCFlags;

		public const int dirtyAllInternal = unchecked((int)(0xFFFF));

		public const int dirtyAll = unchecked((int)(0xFFFFFF));
		// !<0 when is_weak_simple
		// or is_strong_simple flag
		// is valid
		// !<when dirty_is_known_simple is
		// 0, this flag indicates
		// whether the geometry is weak
		// simple or not
		// !<when dirty_is_known_simple
		// is 0, this flag indicates
		// whether the geometry is
		// strong simple or not
		// !<OGCFlags are set by simplify
		// or WKB/WKT import.
		// < at least one stream
		// is unverified
		// < exact envelope is
		// dirty
		// < loose envelope is
		// dirty
		// < loose and dirty envelopes are loose
		// < the geometry is not
		// known to be an envelope
		// < the geometry length needs
		// update
		// < m_cached_ring_areas_2D
		// need update
		// there has been no
		// change to the streams
		// from outside.
		// there has been a change to
		// one of attribute streams
		// from the outside.
	}

	internal static class DirtyFlagsConstants
	{
	}
}
