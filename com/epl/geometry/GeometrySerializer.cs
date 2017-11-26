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
	internal sealed class GeometrySerializer
	{
		private const long serialVersionUID = 1L;

		[System.Serializable]
		internal class BaseGeometryData
		{
			internal com.epl.geometry.Geometry.Type geometryType;

			internal byte[] esriShape = null;
			//Left here for backward compatibility. Use GenericGeometrySerializer instead
		}

		[System.Serializable]
		internal class MultiVertexData : com.epl.geometry.GeometrySerializer.BaseGeometryData
		{
			internal int simpleFlag = 0;

			internal double tolerance = 0;
		}

		[System.Serializable]
		internal class MultiPathData : com.epl.geometry.GeometrySerializer.MultiVertexData
		{
			internal bool[] ogcFlags = null;
		}

		internal com.epl.geometry.GeometrySerializer.BaseGeometryData geometryData;

		/// <exception cref="java.io.ObjectStreamException"/>
		internal object ReadResolve()
		{
			com.epl.geometry.Geometry geometry = null;
			try
			{
				geometry = com.epl.geometry.GeometryEngine.GeometryFromEsriShape(geometryData.esriShape, geometryData.geometryType);
				if (com.epl.geometry.Geometry.IsMultiVertex(geometry.GetType().Value()))
				{
					com.epl.geometry.GeometrySerializer.MultiVertexData mvd = (com.epl.geometry.GeometrySerializer.MultiVertexData)geometryData;
					com.epl.geometry.MultiVertexGeometryImpl mvImpl = (com.epl.geometry.MultiVertexGeometryImpl)geometry._getImpl();
					if (!geometry.IsEmpty() && com.epl.geometry.Geometry.IsMultiPath(geometry.GetType().Value()))
					{
						com.epl.geometry.GeometrySerializer.MultiPathData mpd = (com.epl.geometry.GeometrySerializer.MultiPathData)geometryData;
						com.epl.geometry.MultiPathImpl mpImpl = (com.epl.geometry.MultiPathImpl)geometry._getImpl();
						com.epl.geometry.AttributeStreamOfInt8 pathFlags = mpImpl.GetPathFlagsStreamRef();
						for (int i = 0, n = mpImpl.GetPathCount(); i < n; i++)
						{
							if (mpd.ogcFlags[i])
							{
								pathFlags.SetBits(i, unchecked((byte)com.epl.geometry.PathFlags.enumOGCStartPolygon));
							}
						}
					}
					mvImpl.SetIsSimple(mvd.simpleFlag, mvd.tolerance, false);
				}
			}
			catch (System.Exception)
			{
				throw new System.IO.InvalidDataException("Cannot read geometry from stream");
			}
			return geometry;
		}

		/// <exception cref="java.io.ObjectStreamException"/>
		public void SetGeometryByValue(com.epl.geometry.Geometry geometry)
		{
			try
			{
				if (com.epl.geometry.Geometry.IsMultiPath(geometry.GetType().Value()))
				{
					geometryData = new com.epl.geometry.GeometrySerializer.MultiPathData();
				}
				else
				{
					if (com.epl.geometry.Geometry.IsMultiVertex(geometry.GetType().Value()))
					{
						geometryData = new com.epl.geometry.GeometrySerializer.MultiVertexData();
					}
					else
					{
						geometryData = new com.epl.geometry.GeometrySerializer.BaseGeometryData();
					}
				}
				geometryData.esriShape = com.epl.geometry.GeometryEngine.GeometryToEsriShape(geometry);
				geometryData.geometryType = geometry.GetType();
				if (com.epl.geometry.Geometry.IsMultiVertex(geometryData.geometryType.Value()))
				{
					com.epl.geometry.GeometrySerializer.MultiVertexData mvd = (com.epl.geometry.GeometrySerializer.MultiVertexData)geometryData;
					com.epl.geometry.MultiVertexGeometryImpl mvImpl = (com.epl.geometry.MultiVertexGeometryImpl)geometry._getImpl();
					mvd.tolerance = mvImpl.m_simpleTolerance;
					mvd.simpleFlag = mvImpl.GetIsSimple(0);
					if (!geometry.IsEmpty() && com.epl.geometry.Geometry.IsMultiPath(geometryData.geometryType.Value()))
					{
						com.epl.geometry.GeometrySerializer.MultiPathData mpd = (com.epl.geometry.GeometrySerializer.MultiPathData)geometryData;
						com.epl.geometry.MultiPathImpl mpImpl = (com.epl.geometry.MultiPathImpl)geometry._getImpl();
						mpd.ogcFlags = new bool[mpImpl.GetPathCount()];
						com.epl.geometry.AttributeStreamOfInt8 pathFlags = mpImpl.GetPathFlagsStreamRef();
						for (int i = 0, n = mpImpl.GetPathCount(); i < n; i++)
						{
							mpd.ogcFlags[i] = (pathFlags.Read(i) & unchecked((byte)com.epl.geometry.PathFlags.enumOGCStartPolygon)) != 0;
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
