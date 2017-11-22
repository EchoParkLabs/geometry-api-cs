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
	/// <summary>Flags used by the OperatorExportToEsriShape.</summary>
	public abstract class ShapeExportFlags
	{
		public const int ShapeExportDefaults = 0;

		public const int ShapeExportNoSwap = 1;

		public const int ShapeExportAngularDensify = 2;

		public const int ShapeExportDistanceDensify = 4;

		public const int ShapeExportTrueNaNs = 8;

		public const int ShapeExportStripZs = 16;

		public const int ShapeExportStripMs = 32;

		public const int ShapeExportStripIDs = 64;

		public const int ShapeExportStripTextures = 128;

		public const int ShapeExportStripNormals = 256;

		public const int ShapeExportStripMaterials = 512;

		public const int ShapeExportNewArcFormat = 1024;

		public const int ShapeExportNoCompress = 2048;
		//!<Default export flags
	}

	public static class ShapeExportFlagsConstants
	{
	}
}
