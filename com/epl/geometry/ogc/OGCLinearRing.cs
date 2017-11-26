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
using Sharpen;

namespace com.epl.geometry.ogc
{
	public class OGCLinearRing : com.epl.geometry.ogc.OGCLineString
	{
		public OGCLinearRing(com.epl.geometry.MultiPath mp, int pathIndex, com.epl.geometry.SpatialReference sr, bool reversed)
			: base(mp, pathIndex, sr, reversed)
		{
			if (!mp.IsClosedPath(0))
			{
				throw new System.ArgumentException("LinearRing path must be closed");
			}
		}

		public override int NumPoints()
		{
			if (multiPath.IsEmpty())
			{
				return 0;
			}
			return multiPath.GetPointCount() + 1;
		}

		public override bool IsClosed()
		{
			return true;
		}

		public override bool IsRing()
		{
			return true;
		}

		public override com.epl.geometry.ogc.OGCPoint PointN(int n)
		{
			int nn;
			if (n == multiPath.GetPathSize(0))
			{
				nn = multiPath.GetPathStart(0);
			}
			else
			{
				nn = multiPath.GetPathStart(0) + n;
			}
			return (com.epl.geometry.ogc.OGCPoint)com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(multiPath.GetPoint(nn), esriSR);
		}
	}
}
