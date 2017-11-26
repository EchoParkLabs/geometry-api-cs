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
	public abstract class OGCMultiSurface : com.epl.geometry.ogc.OGCGeometryCollection
	{
		public virtual double Area()
		{
			return GetEsriGeometry().CalculateArea2D();
		}

		public virtual com.epl.geometry.ogc.OGCPoint Centroid()
		{
			// TODO
			throw new System.NotSupportedException();
		}

		public virtual com.epl.geometry.ogc.OGCPoint PointOnSurface()
		{
			// TODO
			throw new System.NotSupportedException();
		}
	}
}
