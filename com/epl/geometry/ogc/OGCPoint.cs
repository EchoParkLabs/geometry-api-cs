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
	public sealed class OGCPoint : com.epl.geometry.ogc.OGCGeometry
	{
		public OGCPoint(com.epl.geometry.Point pt, com.epl.geometry.SpatialReference sr)
		{
			point = pt;
			esriSR = sr;
		}

		public override string AsText()
		{
			return com.epl.geometry.GeometryEngine.GeometryToWkt(GetEsriGeometry(), com.epl.geometry.WktExportFlags.wktExportPoint);
		}

		public override System.IO.MemoryStream AsBinary()
		{
			com.epl.geometry.OperatorExportToWkb op = (com.epl.geometry.OperatorExportToWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkb);
			return op.Execute(com.epl.geometry.WkbExportFlags.wkbExportPoint, GetEsriGeometry(), null);
		}

		public double X()
		{
			return point.GetX();
		}

		public double Y()
		{
			return point.GetY();
		}

		public double Z()
		{
			return point.GetZ();
		}

		public double M()
		{
			return point.GetM();
		}

		public override string GeometryType()
		{
			return "Point";
		}

		public override com.epl.geometry.ogc.OGCGeometry Boundary()
		{
			return new com.epl.geometry.ogc.OGCMultiPoint(new com.epl.geometry.MultiPoint(GetEsriGeometry().GetDescription()), esriSR);
		}

		// return empty point
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
			return point;
		}

		public override com.epl.geometry.ogc.OGCGeometry ConvertToMulti()
		{
			return new com.epl.geometry.ogc.OGCMultiPoint(point, esriSR);
		}

		internal com.epl.geometry.Point point;
	}
}
