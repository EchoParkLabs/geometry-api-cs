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
using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestGeodetic : NUnit.Framework.TestFixtureAttribute
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
		public virtual void TestTriangleLength()
		{
			com.epl.geometry.Point pt_0 = new com.epl.geometry.Point(10, 10);
			com.epl.geometry.Point pt_1 = new com.epl.geometry.Point(20, 20);
			com.epl.geometry.Point pt_2 = new com.epl.geometry.Point(20, 10);
			double length = 0.0;
			length += com.epl.geometry.GeometryEngine.GeodesicDistanceOnWGS84(pt_0, pt_1);
			length += com.epl.geometry.GeometryEngine.GeodesicDistanceOnWGS84(pt_1, pt_2);
			length += com.epl.geometry.GeometryEngine.GeodesicDistanceOnWGS84(pt_2, pt_0);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(length - 3744719.4094597572) < 1e-12 * 3744719.4094597572);
		}

		[NUnit.Framework.Test]
		public virtual void TestRotationInvariance()
		{
			com.epl.geometry.Point pt_0 = new com.epl.geometry.Point(10, 40);
			com.epl.geometry.Point pt_1 = new com.epl.geometry.Point(20, 60);
			com.epl.geometry.Point pt_2 = new com.epl.geometry.Point(20, 40);
			double length = 0.0;
			length += com.epl.geometry.GeometryEngine.GeodesicDistanceOnWGS84(pt_0, pt_1);
			length += com.epl.geometry.GeometryEngine.GeodesicDistanceOnWGS84(pt_1, pt_2);
			length += com.epl.geometry.GeometryEngine.GeodesicDistanceOnWGS84(pt_2, pt_0);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(length - 5409156.3896271614) < 1e-12 * 5409156.3896271614);
			for (int i = -540; i < 540; i += 5)
			{
				pt_0.SetXY(i + 10, 40);
				pt_1.SetXY(i + 20, 60);
				pt_2.SetXY(i + 20, 40);
				length = 0.0;
				length += com.epl.geometry.GeometryEngine.GeodesicDistanceOnWGS84(pt_0, pt_1);
				length += com.epl.geometry.GeometryEngine.GeodesicDistanceOnWGS84(pt_1, pt_2);
				length += com.epl.geometry.GeometryEngine.GeodesicDistanceOnWGS84(pt_2, pt_0);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(length - 5409156.3896271614) < 1e-12 * 5409156.3896271614);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestDistanceFailure()
		{
			{
				com.epl.geometry.Point p1 = new com.epl.geometry.Point(-60.668485, -31.996013333333334);
				com.epl.geometry.Point p2 = new com.epl.geometry.Point(119.13731666666666, 32.251583333333336);
				double d = com.epl.geometry.GeometryEngine.GeodesicDistanceOnWGS84(p1, p2);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(d - 19973410.50579736) < 1e-13 * 19973410.50579736);
			}
			{
				com.epl.geometry.Point p1 = new com.epl.geometry.Point(121.27343833333333, 27.467438333333334);
				com.epl.geometry.Point p2 = new com.epl.geometry.Point(-58.55804833333333, -27.035613333333334);
				double d = com.epl.geometry.GeometryEngine.GeodesicDistanceOnWGS84(p1, p2);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(d - 19954707.428360686) < 1e-13 * 19954707.428360686);
			}
			{
				com.epl.geometry.Point p1 = new com.epl.geometry.Point(-53.329865, -36.08110166666667);
				com.epl.geometry.Point p2 = new com.epl.geometry.Point(126.52895166666667, 35.97385);
				double d = com.epl.geometry.GeometryEngine.GeodesicDistanceOnWGS84(p1, p2);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(d - 19990586.700431127) < 1e-13 * 19990586.700431127);
			}
			{
				com.epl.geometry.Point p1 = new com.epl.geometry.Point(-4.7181166667, 36.1160166667);
				com.epl.geometry.Point p2 = new com.epl.geometry.Point(175.248925, -35.7606716667);
				double d = com.epl.geometry.GeometryEngine.GeodesicDistanceOnWGS84(p1, p2);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(d - 19964450.206594173) < 1e-12 * 19964450.206594173);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestLengthAccurateCR191313()
		{
		}
		/*
		* // random_test(); OperatorFactoryLocal engine =
		* OperatorFactoryLocal.getInstance(); //TODO: Make this:
		* OperatorShapePreservingLength geoLengthOp =
		* (OperatorShapePreservingLength)
		* factory.getOperator(Operator.Type.ShapePreservingLength);
		* SpatialReference spatialRef = SpatialReference.create(102631);
		* //[6097817.59407673
		* ,17463475.2931517],[-1168053.34617516,11199801.3734424
		* ]]],"spatialReference":{"wkid":102631}
		*
		* Polyline polyline = new Polyline();
		* polyline.startPath(6097817.59407673, 17463475.2931517);
		* polyline.lineTo(-1168053.34617516, 11199801.3734424); double length =
		* geoLengthOp.execute(polyline, spatialRef, null);
		* assertTrue(Math.abs(length - 2738362.3249366437) < 2e-9 * length);
		*/
	}
}
