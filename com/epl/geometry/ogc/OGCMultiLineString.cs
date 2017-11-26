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
	public class OGCMultiLineString : com.epl.geometry.ogc.OGCMultiCurve
	{
		public OGCMultiLineString(com.epl.geometry.Polyline poly, com.epl.geometry.SpatialReference sr)
		{
			polyline = poly;
			esriSR = sr;
		}

		public override string AsText()
		{
			return com.epl.geometry.GeometryEngine.GeometryToWkt(GetEsriGeometry(), com.epl.geometry.WktExportFlags.wktExportMultiLineString);
		}

		public override string AsGeoJson()
		{
			com.epl.geometry.OperatorExportToGeoJson op = (com.epl.geometry.OperatorExportToGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			return op.Execute(com.epl.geometry.GeoJsonExportFlags.geoJsonExportPreferMultiGeometry, null, GetEsriGeometry());
		}

		public override System.IO.MemoryStream AsBinary()
		{
			com.epl.geometry.OperatorExportToWkb op = (com.epl.geometry.OperatorExportToWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkb);
			return op.Execute(com.epl.geometry.WkbExportFlags.wkbExportMultiLineString, GetEsriGeometry(), null);
		}

		public override com.epl.geometry.ogc.OGCGeometry GeometryN(int n)
		{
			com.epl.geometry.ogc.OGCLineString ls = new com.epl.geometry.ogc.OGCLineString(polyline, n, esriSR);
			return ls;
		}

		public override string GeometryType()
		{
			return "MultiLineString";
		}

		public override com.epl.geometry.ogc.OGCGeometry Boundary()
		{
			com.epl.geometry.OperatorBoundary op = (com.epl.geometry.OperatorBoundary)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Boundary);
			com.epl.geometry.Geometry g = op.Execute(polyline, null);
			return com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(g, esriSR, true);
		}

		public override com.epl.geometry.ogc.OGCGeometry LocateAlong(double mValue)
		{
			// TODO Auto-generated method stub
			throw new System.NotSupportedException();
		}

		public override com.epl.geometry.ogc.OGCGeometry LocateBetween(double mStart, double mEnd)
		{
			// TODO Auto-generated method stub
			throw new System.NotSupportedException();
		}

		public override com.epl.geometry.Geometry GetEsriGeometry()
		{
			return polyline;
		}

		public override com.epl.geometry.ogc.OGCGeometry ConvertToMulti()
		{
			return this;
		}

		internal com.epl.geometry.Polyline polyline;
	}
}
