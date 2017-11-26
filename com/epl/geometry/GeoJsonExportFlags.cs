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
	public abstract class GeoJsonExportFlags
	{
		public const int geoJsonExportDefaults = 0;

		/// <summary>Export MultiXXX geometries every time, by default it will export the minimum required type.</summary>
		public const int geoJsonExportPreferMultiGeometry = 1;

		public const int geoJsonExportStripZs = 2;

		public const int geoJsonExportStripMs = 4;

		public const int geoJsonExportSkipCRS = 8;

		public const int geoJsonExportFailIfNotSimple = 16;

		public const int geoJsonExportPrecision16 = unchecked((int)(0x02000));

		public const int geoJsonExportPrecision15 = unchecked((int)(0x04000));

		public const int geoJsonExportPrecision14 = unchecked((int)(0x06000));

		public const int geoJsonExportPrecision13 = unchecked((int)(0x08000));

		public const int geoJsonExportPrecision12 = unchecked((int)(0x0a000));

		public const int geoJsonExportPrecision11 = unchecked((int)(0x0c000));

		public const int geoJsonExportPrecision10 = unchecked((int)(0x0e000));

		public const int geoJsonExportPrecision9 = unchecked((int)(0x10000));

		public const int geoJsonExportPrecision8 = unchecked((int)(0x12000));

		public const int geoJsonExportPrecision7 = unchecked((int)(0x14000));

		public const int geoJsonExportPrecision6 = unchecked((int)(0x16000));

		public const int geoJsonExportPrecision5 = unchecked((int)(0x18000));

		public const int geoJsonExportPrecision4 = unchecked((int)(0x1a000));

		public const int geoJsonExportPrecision3 = unchecked((int)(0x1c000));

		public const int geoJsonExportPrecision2 = unchecked((int)(0x1e000));

		public const int geoJsonExportPrecision1 = unchecked((int)(0x20000));

		public const int geoJsonExportPrecision0 = unchecked((int)(0x22000));

		public const int geoJsonExportPrecisionFixedPoint = unchecked((int)(0x40000));
	}

	public static class GeoJsonExportFlagsConstants
	{
	}
}
