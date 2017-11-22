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
	/// <summary>Flags used by the OperatorExportToWkt</summary>
	public abstract class WktExportFlags
	{
		public const int wktExportDefaults = 0;

		public const int wktExportPoint = 1;

		public const int wktExportMultiPoint = 2;

		public const int wktExportLineString = 4;

		public const int wktExportMultiLineString = 8;

		public const int wktExportPolygon = 16;

		public const int wktExportMultiPolygon = 32;

		public const int wktExportStripZs = 64;

		public const int wktExportStripMs = 128;

		public const int wktExportFailIfNotSimple = 4096;

		public const int wktExportPrecision16 = unchecked((int)(0x2000));

		public const int wktExportPrecision15 = unchecked((int)(0x4000));

		public const int wktExportPrecision14 = unchecked((int)(0x6000));

		public const int wktExportPrecision13 = unchecked((int)(0x8000));

		public const int wktExportPrecision12 = unchecked((int)(0xa000));

		public const int wktExportPrecision11 = unchecked((int)(0xc000));

		public const int wktExportPrecision10 = unchecked((int)(0xe000));
	}

	public static class WktExportFlagsConstants
	{
	}
}
