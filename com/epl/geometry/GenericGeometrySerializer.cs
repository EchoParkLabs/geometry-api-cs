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
	[System.Serializable]
	public class GenericGeometrySerializer
	{
		private const long serialVersionUID = 1L;

		internal int geometryType;

		internal byte[] esriShape = null;

		internal int simpleFlag = 0;

		internal double tolerance = 0;

		internal bool[] ogcFlags = null;

		//This is a writeReplace class for MultiPoint, Polyline, and Polygon
		/// <exception cref="java.io.ObjectStreamException"/>
		public virtual object ReadResolve()
		{
			com.epl.geometry.Geometry geometry = null;
			try
			{
				geometry = com.epl.geometry.GeometryEngine.GeometryFromEsriShape(esriShape, com.epl.geometry.Geometry.Type.IntToType(geometryType));
				if (com.epl.geometry.Geometry.IsMultiVertex(geometryType))
				{
					com.epl.geometry.MultiVertexGeometryImpl mvImpl = (com.epl.geometry.MultiVertexGeometryImpl)geometry._getImpl();
					if (!geometry.IsEmpty() && com.epl.geometry.Geometry.IsMultiPath(geometryType))
					{
						com.epl.geometry.MultiPathImpl mpImpl = (com.epl.geometry.MultiPathImpl)geometry._getImpl();
						com.epl.geometry.AttributeStreamOfInt8 pathFlags = mpImpl.GetPathFlagsStreamRef();
						for (int i = 0, n = mpImpl.GetPathCount(); i < n; i++)
						{
							if (ogcFlags[i])
							{
								pathFlags.SetBits(i, unchecked((byte)com.epl.geometry.PathFlags.enumOGCStartPolygon));
							}
						}
					}
					mvImpl.SetIsSimple(simpleFlag, tolerance, false);
				}
			}
			catch (System.Exception)
			{
				throw new System.IO.InvalidDataException("Cannot read geometry from stream");
			}
			return geometry;
		}

		/// <exception cref="java.io.ObjectStreamException"/>
		public virtual void SetGeometryByValue(com.epl.geometry.Geometry geometry)
		{
			try
			{
				esriShape = com.epl.geometry.GeometryEngine.GeometryToEsriShape(geometry);
				geometryType = geometry.GetType().Value();
				if (com.epl.geometry.Geometry.IsMultiVertex(geometryType))
				{
					com.epl.geometry.MultiVertexGeometryImpl mvImpl = (com.epl.geometry.MultiVertexGeometryImpl)geometry._getImpl();
					tolerance = mvImpl.m_simpleTolerance;
					simpleFlag = mvImpl.GetIsSimple(0);
					if (!geometry.IsEmpty() && com.epl.geometry.Geometry.IsMultiPath(geometryType))
					{
						com.epl.geometry.MultiPathImpl mpImpl = (com.epl.geometry.MultiPathImpl)geometry._getImpl();
						ogcFlags = new bool[mpImpl.GetPathCount()];
						com.epl.geometry.AttributeStreamOfInt8 pathFlags = mpImpl.GetPathFlagsStreamRef();
						for (int i = 0, n = mpImpl.GetPathCount(); i < n; i++)
						{
							ogcFlags[i] = (pathFlags.Read(i) & unchecked((byte)com.epl.geometry.PathFlags.enumOGCStartPolygon)) != 0;
						}
					}
				}
			}
			catch (System.Exception)
			{
				throw new System.IO.InvalidDataException("Cannot serialize this geometry");
			}
		}
	}
}
