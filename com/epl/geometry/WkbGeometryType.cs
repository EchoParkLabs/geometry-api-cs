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
	internal abstract class WkbGeometryType
	{
		public const int wkbPoint = 1;

		public const int wkbPointZ = 1001;

		public const int wkbPointM = 2001;

		public const int wkbPointZM = 3001;

		public const int wkbLineString = 2;

		public const int wkbLineStringZ = 1002;

		public const int wkbLineStringM = 2002;

		public const int wkbLineStringZM = 3002;

		public const int wkbPolygon = 3;

		public const int wkbPolygonZ = 1003;

		public const int wkbPolygonM = 2003;

		public const int wkbPolygonZM = 3003;

		public const int wkbMultiPoint = 4;

		public const int wkbMultiPointZ = 1004;

		public const int wkbMultiPointM = 2004;

		public const int wkbMultiPointZM = 3004;

		public const int wkbMultiLineString = 5;

		public const int wkbMultiLineStringZ = 1005;

		public const int wkbMultiLineStringM = 2005;

		public const int wkbMultiLineStringZM = 3005;

		public const int wkbMultiPolygon = 6;

		public const int wkbMultiPolygonZ = 1006;

		public const int wkbMultiPolygonM = 2006;

		public const int wkbMultiPolygonZM = 3006;

		public const int wkbGeometryCollection = 7;

		public const int wkbGeometryCollectionZ = 1007;

		public const int wkbGeometryCollectionM = 2007;

		public const int wkbGeometryCollectionZM = 3007;

		public const int wkbMultiPatch = 8;
	}

	internal static class WkbGeometryTypeConstants
	{
	}
}
