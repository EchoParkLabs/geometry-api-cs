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
	/// <summary>Values for use in Geodetic length and area calculations</summary>
	internal abstract class GeodeticCurveType
	{
		/// <summary>Shortest distance between two points on an ellipsoide</summary>
		public const int Geodesic = 0;

		/// <summary>A line of constant bearing or azimuth.</summary>
		/// <remarks>A line of constant bearing or azimuth. Also known as a rhmub line</remarks>
		public const int Loxodrome = 1;

		/// <summary>
		/// The line on a spheroid defined along the intersection at the surface by a
		/// plane that passes through the center of the spheroid.
		/// </summary>
		/// <remarks>
		/// The line on a spheroid defined along the intersection at the surface by a
		/// plane that passes through the center of the spheroid. When the spheroid
		/// flattening is equal to zero (sphere) then a Great Elliptic is a Great
		/// Circle
		/// </remarks>
		public const int GreatElliptic = 2;

		public const int NormalSection = 3;

		public const int ShapePreserving = 4;
		/*The ShapePreserving type means the segments shapes are preserved in the spatial reference where they are defined.
		*The behavior of the ShapePreserving type can be emulated by densifying the geometry with a small step, and then calling a geodetic method
		*using Geodesic or GreatElliptic curve types.
		*/
	}

	internal static class GeodeticCurveTypeConstants
	{
	}
}
