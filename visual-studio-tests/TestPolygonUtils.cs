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
using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestPolygonUtils : NUnit.Framework.TestFixtureAttribute
	{
		/// <exception cref="System.Exception"/>
		[SetUp]
        protected void SetUp()
		{
			
		}

		/// <exception cref="System.Exception"/>
		protected void TearDown()
		{
			
		}

		[NUnit.Framework.Test]
		public static void TestPointInAnyOuterRing()
		{
			com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
			// outer ring1
			polygon.StartPath(-200, -100);
			polygon.LineTo(200, -100);
			polygon.LineTo(200, 100);
			polygon.LineTo(-190, 100);
			polygon.LineTo(-190, 90);
			polygon.LineTo(-200, 90);
			// hole
			polygon.StartPath(-100, 50);
			polygon.LineTo(100, 50);
			polygon.LineTo(100, -40);
			polygon.LineTo(90, -40);
			polygon.LineTo(90, -50);
			polygon.LineTo(-100, -50);
			// island
			polygon.StartPath(-10, -10);
			polygon.LineTo(10, -10);
			polygon.LineTo(10, 10);
			polygon.LineTo(-10, 10);
			// outer ring2
			polygon.StartPath(300, 300);
			polygon.LineTo(310, 300);
			polygon.LineTo(310, 310);
			polygon.LineTo(300, 310);
			polygon.ReverseAllPaths();
			com.epl.geometry.Point2D testPointIn1 = new com.epl.geometry.Point2D(1, 2);
			// inside the island
			com.epl.geometry.Point2D testPointIn2 = new com.epl.geometry.Point2D(190, 90);
			// inside, betwen outer
			// ring1 and the hole
			com.epl.geometry.Point2D testPointIn3 = new com.epl.geometry.Point2D(305, 305);
			// inside the outer ring2
			com.epl.geometry.Point2D testPointOut1 = new com.epl.geometry.Point2D(300, 2);
			// outside any
			com.epl.geometry.Point2D testPointOut2 = new com.epl.geometry.Point2D(-195, 95);
			// outside any (in the
			// concave area of outer
			// ring 2)
			com.epl.geometry.Point2D testPointOut3 = new com.epl.geometry.Point2D(99, 49);
			// outside (in the hole)
			com.epl.geometry.PolygonUtils.PiPResult res;
			// is_point_in_polygon_2D
			res = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon, testPointIn1, 0);
			NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPInside);
			res = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon, testPointIn2, 0);
			NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPInside);
			res = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon, testPointIn3, 0);
			NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPInside);
			res = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon, testPointOut1, 0);
			NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside);
			res = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon, testPointOut2, 0);
			NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside);
			res = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon, testPointOut3, 0);
			NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside);
			// Ispoint_in_any_outer_ring
			res = com.epl.geometry.PolygonUtils.IsPointInAnyOuterRing(polygon, testPointIn1, 0);
			NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPInside);
			res = com.epl.geometry.PolygonUtils.IsPointInAnyOuterRing(polygon, testPointIn2, 0);
			NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPInside);
			res = com.epl.geometry.PolygonUtils.IsPointInAnyOuterRing(polygon, testPointIn3, 0);
			NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPInside);
			res = com.epl.geometry.PolygonUtils.IsPointInAnyOuterRing(polygon, testPointOut1, 0);
			NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside);
			res = com.epl.geometry.PolygonUtils.IsPointInAnyOuterRing(polygon, testPointOut2, 0);
			NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside);
			res = com.epl.geometry.PolygonUtils.IsPointInAnyOuterRing(polygon, testPointOut3, 0);
			NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPInside);
		}

		// inside of outer
		// ring
		[NUnit.Framework.Test]
		public static void TestPointInPolygonBugCR181840()
		{
			com.epl.geometry.PolygonUtils.PiPResult res;
			{
				// pointInPolygonBugCR181840 - point in polygon bug
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				// outer ring1
				polygon.StartPath(0, 0);
				polygon.LineTo(10, 10);
				polygon.LineTo(20, 0);
				res = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon, com.epl.geometry.Point2D.Construct(15, 10), 0);
				NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside);
				res = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon, com.epl.geometry.Point2D.Construct(2, 10), 0);
				NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside);
				res = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon, com.epl.geometry.Point2D.Construct(5, 5), 0);
				NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPInside);
			}
			{
				// CR181840 - point in polygon bug
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				// outer ring1
				polygon.StartPath(10, 10);
				polygon.LineTo(20, 0);
				polygon.LineTo(0, 0);
				res = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon, com.epl.geometry.Point2D.Construct(15, 10), 0);
				NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside);
				res = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon, com.epl.geometry.Point2D.Construct(2, 10), 0);
				NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside);
				res = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon, com.epl.geometry.Point2D.Construct(5, 5), 0);
				NUnit.Framework.Assert.IsTrue(res == com.epl.geometry.PolygonUtils.PiPResult.PiPInside);
			}
		}
	}
}
