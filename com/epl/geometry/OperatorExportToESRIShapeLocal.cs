/*
Copyright 2017-2021 David Raleigh

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

email: davidraleigh@gmail.com
*/


namespace com.epl.geometry
{
	/// <summary>OperatorExportToESRIShape implementation.</summary>
	internal class OperatorExportToESRIShapeLocal : com.epl.geometry.OperatorExportToESRIShape
	{
		internal override com.epl.geometry.ByteBufferCursor Execute(int exportFlags, com.epl.geometry.GeometryCursor geometryCursor)
		{
			return new com.epl.geometry.OperatorExportToESRIShapeCursor(exportFlags, geometryCursor);
		}

		public override System.IO.MemoryStream Execute(int exportFlags, com.epl.geometry.Geometry geometry)
		{
			System.IO.MemoryStream shapeBuffer = null;
			int size = com.epl.geometry.OperatorExportToESRIShapeCursor.ExportToESRIShape(exportFlags, geometry, null);
			shapeBuffer = new System.IO.MemoryStream(size);
            com.epl.geometry.OperatorExportToESRIShapeCursor.ExportToESRIShape(exportFlags, geometry, new System.IO.BinaryWriter(shapeBuffer));
			return shapeBuffer;
		}

		public override int Execute(int exportFlags, com.epl.geometry.Geometry geometry, System.IO.MemoryStream shapeBuffer)
		{
			//shapeBuffer.Order(java.nio.ByteOrder.LITTLE_ENDIAN);
            return com.epl.geometry.OperatorExportToESRIShapeCursor.ExportToESRIShape(exportFlags, geometry, new System.IO.BinaryWriter(shapeBuffer));
		}
	}
}
