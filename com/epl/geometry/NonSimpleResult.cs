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
	/// <summary>The result of the IsSimpleXXX.</summary>
	public class NonSimpleResult
	{
		public enum Reason
		{
			NotDetermined,
			Structure,
			DegenerateSegments,
			Clustering,
			Cracking,
			CrossOver,
			RingOrientation,
			RingOrder,
			OGCPolylineSelfTangency,
			OGCPolygonSelfTangency,
			OGCDisconnectedInterior
		}

		public com.epl.geometry.NonSimpleResult.Reason m_reason;

		public int m_vertexIndex1;

		public int m_vertexIndex2;

		public NonSimpleResult()
		{
			m_reason = com.epl.geometry.NonSimpleResult.Reason.NotDetermined;
			m_vertexIndex1 = -1;
			m_vertexIndex2 = -1;
		}

		internal virtual void Assign(com.epl.geometry.NonSimpleResult src)
		{
			m_reason = src.m_reason;
			m_vertexIndex1 = src.m_vertexIndex1;
			m_vertexIndex2 = src.m_vertexIndex2;
		}

		internal NonSimpleResult(com.epl.geometry.NonSimpleResult.Reason reason, int index1, int index2)
		{
			m_reason = reason;
			m_vertexIndex1 = index1;
			m_vertexIndex2 = index2;
		}
	}
}
