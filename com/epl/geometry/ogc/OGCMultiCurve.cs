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
	public abstract class OGCMultiCurve : com.epl.geometry.ogc.OGCGeometryCollection
	{
		public override int NumGeometries()
		{
			com.epl.geometry.MultiPath mp = (com.epl.geometry.MultiPath)GetEsriGeometry();
			return mp.GetPathCount();
		}

		public virtual bool IsClosed()
		{
			com.epl.geometry.MultiPath mp = (com.epl.geometry.MultiPath)GetEsriGeometry();
			for (int i = 0, n = mp.GetPathCount(); i < n; i++)
			{
				if (!mp.IsClosedPathInXYPlane(i))
				{
					return false;
				}
			}
			return true;
		}

		public virtual double Length()
		{
			return GetEsriGeometry().CalculateLength2D();
		}
	}
}
