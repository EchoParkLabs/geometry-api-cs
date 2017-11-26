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
	public abstract class OGCCurve : com.epl.geometry.ogc.OGCGeometry
	{
		public abstract double Length();

		public abstract com.epl.geometry.ogc.OGCPoint StartPoint();

		public abstract com.epl.geometry.ogc.OGCPoint EndPoint();

		public abstract bool IsClosed();

		public virtual bool IsRing()
		{
			return IsSimple() && IsClosed();
		}

		public override com.epl.geometry.ogc.OGCGeometry Boundary()
		{
			if (IsClosed())
			{
				return new com.epl.geometry.ogc.OGCMultiPoint(new com.epl.geometry.MultiPoint(GetEsriGeometry().GetDescription()), esriSR);
			}
			else
			{
				// return empty multipoint;
				return new com.epl.geometry.ogc.OGCMultiPoint(StartPoint(), EndPoint());
			}
		}
	}
}
