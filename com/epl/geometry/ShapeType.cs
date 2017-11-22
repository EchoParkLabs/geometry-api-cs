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
	internal abstract class ShapeType
	{
		public const int ShapeNull = 0;

		public const int ShapePoint = 1;

		public const int ShapePointM = 21;

		public const int ShapePointZM = 11;

		public const int ShapePointZ = 9;

		public const int ShapeMultiPoint = 8;

		public const int ShapeMultiPointM = 28;

		public const int ShapeMultiPointZM = 18;

		public const int ShapeMultiPointZ = 20;

		public const int ShapePolyline = 3;

		public const int ShapePolylineM = 23;

		public const int ShapePolylineZM = 13;

		public const int ShapePolylineZ = 10;

		public const int ShapePolygon = 5;

		public const int ShapePolygonM = 25;

		public const int ShapePolygonZM = 15;

		public const int ShapePolygonZ = 19;

		public const int ShapeMultiPatchM = 31;

		public const int ShapeMultiPatch = 32;

		public const int ShapeGeneralPolyline = 50;

		public const int ShapeGeneralPolygon = 51;

		public const int ShapeGeneralPoint = 52;

		public const int ShapeGeneralMultiPoint = 53;

		public const int ShapeGeneralMultiPatch = 54;

		public const int ShapeTypeLast = 55;
	}

	internal static class ShapeTypeConstants
	{
	}
}
