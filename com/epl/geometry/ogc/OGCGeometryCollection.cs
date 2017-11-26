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
	public abstract class OGCGeometryCollection : com.epl.geometry.ogc.OGCGeometry
	{
		/// <summary>Returns the number of geometries in this GeometryCollection.</summary>
		public abstract int NumGeometries();

		/// <summary>Returns the Nth geometry in this GeometryCollection.</summary>
		/// <param name="n">The 0 based index of the geometry.</param>
		public abstract com.epl.geometry.ogc.OGCGeometry GeometryN(int n);
	}
}
