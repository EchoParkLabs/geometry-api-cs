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
	public class TestRelation : NUnit.Framework.TestFixtureAttribute
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
		public virtual void TestCreation()
		{
			{
				com.epl.geometry.OperatorFactoryLocal projEnv = com.epl.geometry.OperatorFactoryLocal.GetInstance();
				com.epl.geometry.SpatialReference inputSR = com.epl.geometry.SpatialReference.Create(3857);
				com.epl.geometry.Polygon poly1 = new com.epl.geometry.Polygon();
				com.epl.geometry.Envelope2D env1 = new com.epl.geometry.Envelope2D();
				env1.SetCoords(855277, 3892059, 855277 + 100, 3892059 + 100);
				poly1.AddEnvelope(env1, false);
				com.epl.geometry.Polygon poly2 = new com.epl.geometry.Polygon();
				com.epl.geometry.Envelope2D env2 = new com.epl.geometry.Envelope2D();
				env2.SetCoords(855277, 3892059, 855277 + 300, 3892059 + 200);
				poly2.AddEnvelope(env2, false);
				{
					com.epl.geometry.OperatorEquals operatorEquals = (com.epl.geometry.OperatorEquals)(projEnv.GetOperator(com.epl.geometry.Operator.Type.Equals));
					bool result = operatorEquals.Execute(poly1, poly2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					com.epl.geometry.Polygon poly11 = new com.epl.geometry.Polygon();
					poly1.CopyTo(poly11);
					result = operatorEquals.Execute(poly1, poly11, inputSR, null);
					NUnit.Framework.Assert.IsTrue(result);
				}
				{
					com.epl.geometry.OperatorCrosses operatorCrosses = (com.epl.geometry.OperatorCrosses)(projEnv.GetOperator(com.epl.geometry.Operator.Type.Crosses));
					bool result = operatorCrosses.Execute(poly1, poly2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
				}
				{
					com.epl.geometry.OperatorWithin operatorWithin = (com.epl.geometry.OperatorWithin)(projEnv.GetOperator(com.epl.geometry.Operator.Type.Within));
					bool result = operatorWithin.Execute(poly1, poly2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(result);
				}
				{
					com.epl.geometry.OperatorDisjoint operatorDisjoint = (com.epl.geometry.OperatorDisjoint)(projEnv.GetOperator(com.epl.geometry.Operator.Type.Disjoint));
					com.epl.geometry.OperatorIntersects operatorIntersects = (com.epl.geometry.OperatorIntersects)(projEnv.GetOperator(com.epl.geometry.Operator.Type.Intersects));
					bool result = operatorDisjoint.Execute(poly1, poly2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					{
						result = operatorIntersects.Execute(poly1, poly2, inputSR, null);
						NUnit.Framework.Assert.IsTrue(result);
					}
				}
				{
					com.epl.geometry.OperatorDisjoint operatorDisjoint = (com.epl.geometry.OperatorDisjoint)(projEnv.GetOperator(com.epl.geometry.Operator.Type.Disjoint));
					com.epl.geometry.OperatorIntersects operatorIntersects = (com.epl.geometry.OperatorIntersects)(projEnv.GetOperator(com.epl.geometry.Operator.Type.Intersects));
					com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
					poly2.QueryEnvelope2D(env2D);
					com.epl.geometry.Envelope envelope = new com.epl.geometry.Envelope(env2D);
					bool result = operatorDisjoint.Execute(envelope, poly2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					{
						result = operatorIntersects.Execute(envelope, poly2, inputSR, null);
						NUnit.Framework.Assert.IsTrue(result);
					}
				}
				{
					com.epl.geometry.OperatorDisjoint operatorDisjoint = (com.epl.geometry.OperatorDisjoint)(projEnv.GetOperator(com.epl.geometry.Operator.Type.Disjoint));
					com.epl.geometry.OperatorIntersects operatorIntersects = (com.epl.geometry.OperatorIntersects)(projEnv.GetOperator(com.epl.geometry.Operator.Type.Intersects));
					com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
					com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
					env2D.SetCoords(855277, 3892059, 855277 + 100, 3892059 + 100);
					poly.AddEnvelope(env2D, false);
					env2D.SetCoords(855277 + 10, 3892059 + 10, 855277 + 90, 3892059 + 90);
					poly.AddEnvelope(env2D, true);
					env2D.SetCoords(855277 + 20, 3892059 + 20, 855277 + 200, 3892059 + 80);
					com.epl.geometry.Envelope envelope = new com.epl.geometry.Envelope(env2D);
					bool result = operatorDisjoint.Execute(envelope, poly, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					{
						result = operatorIntersects.Execute(envelope, poly, inputSR, null);
						NUnit.Framework.Assert.IsTrue(result);
					}
				}
				{
					com.epl.geometry.OperatorTouches operatorTouches = (com.epl.geometry.OperatorTouches)(projEnv.GetOperator(com.epl.geometry.Operator.Type.Touches));
					bool result = operatorTouches.Execute(poly1, poly2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
				}
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestOperatorDisjoint()
		{
			{
				com.epl.geometry.OperatorFactoryLocal projEnv = com.epl.geometry.OperatorFactoryLocal.GetInstance();
				com.epl.geometry.SpatialReference inputSR = com.epl.geometry.SpatialReference.Create(3857);
				com.epl.geometry.Polygon poly1 = new com.epl.geometry.Polygon();
				com.epl.geometry.Envelope2D env1 = new com.epl.geometry.Envelope2D();
				env1.SetCoords(855277, 3892059, 855277 + 100, 3892059 + 100);
				poly1.AddEnvelope(env1, false);
				com.epl.geometry.Polygon poly2 = new com.epl.geometry.Polygon();
				com.epl.geometry.Envelope2D env2 = new com.epl.geometry.Envelope2D();
				env2.SetCoords(855277, 3892059, 855277 + 300, 3892059 + 200);
				poly2.AddEnvelope(env2, false);
				com.epl.geometry.Polygon poly3 = new com.epl.geometry.Polygon();
				com.epl.geometry.Envelope2D env3 = new com.epl.geometry.Envelope2D();
				env3.SetCoords(855277 + 100, 3892059 + 100, 855277 + 100 + 100, 3892059 + 100 + 100);
				poly3.AddEnvelope(env3, false);
				com.epl.geometry.Polygon poly4 = new com.epl.geometry.Polygon();
				com.epl.geometry.Envelope2D env4 = new com.epl.geometry.Envelope2D();
				env4.SetCoords(855277 + 200, 3892059 + 200, 855277 + 200 + 100, 3892059 + 200 + 100);
				poly4.AddEnvelope(env4, false);
				com.epl.geometry.Point point1 = new com.epl.geometry.Point(855277, 3892059);
				com.epl.geometry.Point point2 = new com.epl.geometry.Point(855277 + 2, 3892059 + 3);
				com.epl.geometry.Point point3 = new com.epl.geometry.Point(855277 - 2, 3892059 - 3);
				{
					com.epl.geometry.OperatorDisjoint operatorDisjoint = (com.epl.geometry.OperatorDisjoint)(projEnv.GetOperator(com.epl.geometry.Operator.Type.Disjoint));
					bool result = operatorDisjoint.Execute(poly1, poly2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					result = operatorDisjoint.Execute(poly1, poly3, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					result = operatorDisjoint.Execute(poly1, poly4, inputSR, null);
					NUnit.Framework.Assert.IsTrue(result);
					result = operatorDisjoint.Execute(poly1, point1, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					result = operatorDisjoint.Execute(point1, poly1, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					result = operatorDisjoint.Execute(poly1, point2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					result = operatorDisjoint.Execute(point2, poly1, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					result = operatorDisjoint.Execute(poly1, point3, inputSR, null);
					NUnit.Framework.Assert.IsTrue(result);
					result = operatorDisjoint.Execute(point3, poly1, inputSR, null);
					NUnit.Framework.Assert.IsTrue(result);
				}
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestTouchPointLineCR183227()
		{
			// Tests CR 183227
			com.epl.geometry.OperatorTouches operatorTouches = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
			com.epl.geometry.Geometry baseGeom = new com.epl.geometry.Point(-130, 10);
			com.epl.geometry.Polyline pl = new com.epl.geometry.Polyline();
			// pl.startPath(std::make_shared<Point>(-130, 10));
			pl.StartPath(-130, 10);
			pl.LineTo(-131, 15);
			pl.LineTo(-140, 20);
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			bool isTouched;
			bool isTouched2;
			isTouched = operatorTouches.Execute(baseGeom, pl, sr, null);
			isTouched2 = operatorTouches.Execute(pl, baseGeom, sr, null);
			NUnit.Framework.Assert.IsTrue(isTouched && isTouched2);
			{
				baseGeom = new com.epl.geometry.Point(-131, 15);
				isTouched = operatorTouches.Execute(baseGeom, pl, sr, null);
				isTouched2 = operatorTouches.Execute(pl, baseGeom, sr, null);
				NUnit.Framework.Assert.IsTrue(!isTouched && !isTouched2);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestTouchPointLineClosed()
		{
			// Tests CR 183227
			com.epl.geometry.OperatorTouches operatorTouches = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
			com.epl.geometry.Geometry baseGeom = new com.epl.geometry.Point(-130, 10);
			com.epl.geometry.Polyline pl = new com.epl.geometry.Polyline();
			pl.StartPath(-130, 10);
			pl.LineTo(-131, 15);
			pl.LineTo(-140, 20);
			pl.LineTo(-130, 10);
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			bool isTouched;
			bool isTouched2;
			isTouched = operatorTouches.Execute(baseGeom, pl, sr, null);
			isTouched2 = operatorTouches.Execute(pl, baseGeom, sr, null);
			NUnit.Framework.Assert.IsTrue(!isTouched && !isTouched2);
			{
				// this may change in future
				baseGeom = new com.epl.geometry.Point(-131, 15);
				isTouched = operatorTouches.Execute(baseGeom, pl, sr, null);
				isTouched2 = operatorTouches.Execute(pl, baseGeom, sr, null);
				NUnit.Framework.Assert.IsTrue(!isTouched && !isTouched2);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestTouchPolygonPolygon()
		{
			com.epl.geometry.OperatorTouches operatorTouches = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
			com.epl.geometry.Polygon pg = new com.epl.geometry.Polygon();
			pg.StartPath(-130, 10);
			pg.LineTo(-131, 15);
			pg.LineTo(-140, 20);
			com.epl.geometry.Polygon pg2 = new com.epl.geometry.Polygon();
			pg2.StartPath(-130, 10);
			pg2.LineTo(-131, 15);
			pg2.LineTo(-120, 20);
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			bool isTouched;
			bool isTouched2;
			isTouched = operatorTouches.Execute(pg, pg2, sr, null);
			isTouched2 = operatorTouches.Execute(pg2, pg, sr, null);
			NUnit.Framework.Assert.IsTrue(isTouched && isTouched2);
		}

//		[NUnit.Framework.Test]
//		public virtual void TestContainsFailureCR186456()
//		{
//			{
//				com.epl.geometry.OperatorContains op = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
//				string str = "{\"rings\":[[[406944.399999999,287461.450000001],[406947.750000011,287462.299999997],[406946.44999999,287467.450000001],[406943.050000005,287466.550000005],[406927.799999992,287456.849999994],[406926.949999996,287456.599999995],[406924.800000005,287455.999999998],[406924.300000007,287455.849999999],[406924.200000008,287456.099999997],[406923.450000011,287458.449999987],[406922.999999987,287459.800000008],[406922.29999999,287462.099999998],[406921.949999991,287463.449999992],[406921.449999993,287465.050000011],[406920.749999996,287466.700000004],[406919.800000001,287468.599999996],[406919.050000004,287469.99999999],[406917.800000009,287471.800000008],[406916.04999999,287473.550000001],[406915.449999993,287473.999999999],[406913.700000001,287475.449999993],[406913.300000002,287475.899999991],[406912.050000008,287477.250000011],[406913.450000002,287478.150000007],[406915.199999994,287478.650000005],[406915.999999991,287478.800000005],[406918.300000007,287479.200000003],[406920.649999997,287479.450000002],[406923.100000013,287479.550000001],[406925.750000001,287479.450000002],[406928.39999999,287479.150000003],[406929.80000001,287478.950000004],[406932.449999998,287478.350000006],[406935.099999987,287477.60000001],[406938.699999998,287476.349999989],[406939.649999994,287473.949999999],[406939.799999993,287473.949999999],[406941.249999987,287473.75],[406942.700000007,287473.250000002],[406943.100000005,287473.100000003],[406943.950000001,287472.750000004],[406944.799999998,287472.300000006],[406944.999999997,287472.200000007],[406946.099999992,287471.200000011],[406946.299999991,287470.950000012],[406948.00000001,287468.599999996],[406948.10000001,287468.399999997],[406950.100000001,287465.050000011],[406951.949999993,287461.450000001],[406952.049999993,287461.300000001],[406952.69999999,287459.900000007],[406953.249999987,287458.549999987],[406953.349999987,287458.299999988],[406953.650000012,287457.299999992],[406953.900000011,287456.349999996],[406954.00000001,287455.300000001],[406954.00000001,287454.750000003],[406953.850000011,287453.750000008],[406953.550000012,287452.900000011],[406953.299999987,287452.299999988],[406954.500000008,287450.299999996],[406954.00000001,287449.000000002],[406953.399999987,287447.950000006],[406953.199999988,287447.550000008],[406952.69999999,287446.850000011],[406952.149999992,287446.099999988],[406951.499999995,287445.499999991],[406951.149999996,287445.249999992],[406950.449999999,287444.849999994],[406949.600000003,287444.599999995],[406949.350000004,287444.549999995],[406948.250000009,287444.499999995],[406947.149999987,287444.699999994],[406946.849999989,287444.749999994],[406945.899999993,287444.949999993],[406944.999999997,287445.349999991],[406944.499999999,287445.64999999],[406943.650000003,287446.349999987],[406942.900000006,287447.10000001],[406942.500000008,287447.800000007],[406942.00000001,287448.700000003],[406941.600000011,287449.599999999],[406941.350000013,287450.849999994],[406941.350000013,287451.84999999],[406941.450000012,287452.850000012],[406941.750000011,287453.850000007],[406941.800000011,287454.000000007],[406942.150000009,287454.850000003],[406942.650000007,287455.6],[406943.150000005,287456.299999997],[406944.499999999,287457.299999992],[406944.899999997,287457.599999991],[406945.299999995,287457.949999989],[406944.399999999,287461.450000001],[406941.750000011,287461.999999998],[406944.399999999,287461.450000001]],[[406944.399999999,287461.450000001],[406947.750000011,287462.299999997],[406946.44999999,287467.450000001],[406943.050000005,287466.550000005],[406927.799999992,287456.849999994],[406944.399999999,287461.450000001]]]}";
//				com.epl.geometry.MapGeometry mg = com.epl.geometry.TestCommonMethods.FromJson(str);
//				bool res = op.Execute((mg.GetGeometry()), (mg.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//			}
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestWithin()
//		{
//			{
//				com.epl.geometry.OperatorWithin op = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
//				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[0,0],[0,0],[0,0]]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				string str2 = "{\"x\":100,\"y\":100}";
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str2);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//			{
//				// polygon
//				com.epl.geometry.OperatorWithin op = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
//				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[100,0]]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				string str2 = "{\"rings\":[[[10,10],[10,100],[100,100],[100,10]]]}";
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str2);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//			{
//				// Multi_point
//				com.epl.geometry.OperatorWithin op = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
//				string str1 = "{\"points\":[[0,0],[0,200],[200,200]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				string str2 = "{\"points\":[[0,0],[0,200]]}";
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str2);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//			{
//				// Multi_point
//				com.epl.geometry.OperatorWithin op = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
//				string str1 = "{\"points\":[[0,0],[0,200]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				string str2 = "{\"points\":[[0,0],[0,200]]}";
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str2);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//			}
//			{
//				// Multi_point
//				com.epl.geometry.OperatorWithin op = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
//				string str1 = "{\"points\":[[0,0],[0,200],[200,200]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				string str2 = "{\"points\":[[0,0],[0,200], [1, 1]]}";
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str2);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestContains()
//		{
//			{
//				com.epl.geometry.OperatorContains op = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
//				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[0,0],[0,0],[0,0]]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				string str2 = "{\"x\":100,\"y\":100}";
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str2);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//			}
//			{
//				// polygon
//				com.epl.geometry.OperatorContains op = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
//				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[0,0]]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				string str2 = "{\"rings\":[[[10,10],[10,100],[100,100],[10,10]]]}";
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str2);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//			}
//			{
//				// Multi_point
//				com.epl.geometry.OperatorContains op = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
//				string str1 = "{\"points\":[[0,0],[0,200],[200,200]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				string str2 = "{\"points\":[[0,0],[0,200]]}";
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str2);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//			}
//			{
//				// Multi_point
//				com.epl.geometry.OperatorContains op = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
//				string str1 = "{\"points\":[[0,0],[0,200]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				string str2 = "{\"points\":[[0,0],[0,200]]}";
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str2);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//			}
//			{
//				// Multi_point
//				com.epl.geometry.OperatorContains op = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
//				string str1 = "{\"points\":[[0,0],[0,200],[200,200]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				string str2 = "{\"points\":[[0,0],[0,200], [1, 1]]}";
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str2);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//		}
//
		[NUnit.Framework.Test]
		public virtual void TestOverlaps()
		{
			{
				// empty polygon
				com.epl.geometry.OperatorOverlaps op = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
				com.epl.geometry.Polygon poly1 = new com.epl.geometry.Polygon();
				com.epl.geometry.Polygon poly2 = new com.epl.geometry.Polygon();
				bool res = op.Execute(poly1, poly2, null, null);
				NUnit.Framework.Assert.IsTrue(!res);
				res = op.Execute(poly1, poly2, null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
//			{
//				// polygon
//				com.epl.geometry.OperatorOverlaps op = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[0,0],[0,0],[0,0]]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				string str2 = "{\"x\":100,\"y\":100}";
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str2);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//			{
//				// polygon
//				com.epl.geometry.OperatorOverlaps op = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[0,0]]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				com.epl.geometry.Transformation2D trans = new com.epl.geometry.Transformation2D();
//				trans.SetShift(300, 0);
//				mg2.GetGeometry().ApplyTransformation(trans);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//			{
//				// polygon
//				com.epl.geometry.OperatorOverlaps op = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[0,0]]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				com.epl.geometry.Transformation2D trans = new com.epl.geometry.Transformation2D();
//				trans.SetShift(30, 0);
//				mg2.GetGeometry().ApplyTransformation(trans);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//			}
//			{
//				// polygon
//				com.epl.geometry.OperatorOverlaps op = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[0,0]]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				com.epl.geometry.Transformation2D trans = new com.epl.geometry.Transformation2D();
//				trans.SetShift(0, 0);
//				mg2.GetGeometry().ApplyTransformation(trans);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//			{
//				// polyline
//				com.epl.geometry.OperatorOverlaps op = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//				string str1 = "{\"paths\":[[[0,0],[100,0],[200,0]]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				com.epl.geometry.Transformation2D trans = new com.epl.geometry.Transformation2D();
//				trans.SetShift(0, 0);
//				mg2.GetGeometry().ApplyTransformation(trans);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//			{
//				// polyline
//				com.epl.geometry.OperatorOverlaps op = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//				string str1 = "{\"paths\":[[[0,0],[100,0],[200,0]]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				com.epl.geometry.Transformation2D trans = new com.epl.geometry.Transformation2D();
//				trans.SetShift(10, 0);
//				mg2.GetGeometry().ApplyTransformation(trans);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//			}
//			{
//				// polyline
//				com.epl.geometry.OperatorOverlaps op = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//				string str1 = "{\"paths\":[[[0,0],[100,0],[200,0]]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				com.epl.geometry.Transformation2D trans = new com.epl.geometry.Transformation2D();
//				trans.SetShift(200, 0);
//				mg2.GetGeometry().ApplyTransformation(trans);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//			{
//				// Multi_point
//				com.epl.geometry.OperatorOverlaps op = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//				string str1 = "{\"points\":[[0,0],[0,200],[200,200],[200,0]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				com.epl.geometry.Transformation2D trans = new com.epl.geometry.Transformation2D();
//				trans.SetShift(0, 0);
//				mg2.GetGeometry().ApplyTransformation(trans);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//			{
//				// Multi_point
//				com.epl.geometry.OperatorOverlaps op = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//				string str1 = "{\"points\":[[0,0],[0,200],[200,200]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				string str2 = "{\"points\":[[0,0],[0,200]]}";
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str2);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(!res);
//			}
//			{
//				// Multi_point
//				com.epl.geometry.OperatorOverlaps op = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//				string str1 = "{\"points\":[[0,0],[0,200],[200,200]]}";
//				com.epl.geometry.MapGeometry mg1 = com.epl.geometry.TestCommonMethods.FromJson(str1);
//				string str2 = "{\"points\":[[0,0],[0,200], [0,2]]}";
//				com.epl.geometry.MapGeometry mg2 = com.epl.geometry.TestCommonMethods.FromJson(str2);
//				bool res = op.Execute((mg2.GetGeometry()), (mg1.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//				res = op.Execute((mg1.GetGeometry()), (mg2.GetGeometry()), null, null);
//				NUnit.Framework.Assert.IsTrue(res);
//			}
		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestPolygonPolygonEquals()
//		{
//			com.epl.geometry.OperatorEquals equals = (com.epl.geometry.OperatorEquals)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Equals));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
//			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
//			// Polygon1 and Polygon2 are topologically equal, but have differing
//			// number of vertices
//			string str1 = "{\"rings\":[[[0,0],[0,5],[0,7],[0,10],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			string str2 = "{\"rings\":[[[0,10],[10,10],[10,0],[0,0],[0,10]],[[9,1],[9,6],[9,9],[1,9],[1,1],[1,1],[9,1]]]}";
//			com.epl.geometry.Polygon polygon1 = (com.epl.geometry.Polygon)com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry();
//			com.epl.geometry.Polygon polygon2 = (com.epl.geometry.Polygon)com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry();
//			// wiggleGeometry(polygon1, tolerance, 1982);
//			// wiggleGeometry(polygon2, tolerance, 511);
//			equals.AccelerateGeometry(polygon1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
//			equals.AccelerateGeometry(polygon2, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
//			bool res = equals.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			equals.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			// The outer rings of Polygon1 and Polygon2 are equal, but Polygon1 has
//			// a hole.
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"rings\":[[[0,10],[10,10],[5,10],[10,10],[10,0],[0,0],[0,10]]]}";
//			polygon1 = (com.epl.geometry.Polygon)com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry();
//			polygon2 = (com.epl.geometry.Polygon)com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry();
//			res = equals.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = equals.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// The rings are equal but rotated
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]]]}";
//			str2 = "{\"rings\":[[[0,10],[10,10],[10,0],[0,0],[0,10]]]}";
//			polygon1 = (com.epl.geometry.Polygon)com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry();
//			polygon2 = (com.epl.geometry.Polygon)com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry();
//			res = equals.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = equals.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			// The rings are equal but opposite orientation
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]]]}";
//			str2 = "{\"rings\":[[[0,0],[10,0],[10,10],[0,10],[0,0]]]}";
//			polygon1 = (com.epl.geometry.Polygon)com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry();
//			polygon2 = (com.epl.geometry.Polygon)com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry();
//			res = equals.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = equals.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// The rings are equal but first polygon has two rings stacked
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[0,10],[10,10],[10,0],[0,0],[0,10]]]}";
//			str2 = "{\"rings\":[[[0,10],[10,10],[10,0],[0,0],[0,10]]]}";
//			polygon1 = (com.epl.geometry.Polygon)com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry();
//			polygon2 = (com.epl.geometry.Polygon)com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry();
//			res = equals.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = equals.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPointMultiPointEquals()
		{
			com.epl.geometry.OperatorEquals equals = (com.epl.geometry.OperatorEquals)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Equals);
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.MultiPoint multipoint1 = new com.epl.geometry.MultiPoint();
			com.epl.geometry.MultiPoint multipoint2 = new com.epl.geometry.MultiPoint();
			multipoint1.Add(0, 0);
			multipoint1.Add(1, 1);
			multipoint1.Add(2, 2);
			multipoint1.Add(3, 3);
			multipoint1.Add(4, 4);
			multipoint1.Add(1, 1);
			multipoint1.Add(0, 0);
			multipoint2.Add(4, 4);
			multipoint2.Add(3, 3);
			multipoint2.Add(2, 2);
			multipoint2.Add(1, 1);
			multipoint2.Add(0, 0);
			multipoint2.Add(2, 2);
			WiggleGeometry(multipoint1, 0.001, 123);
			WiggleGeometry(multipoint2, 0.001, 5937);
			bool res = equals.Execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = equals.Execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint1.Add(1, 2);
			res = equals.Execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = equals.Execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPointPointEquals()
		{
			com.epl.geometry.OperatorEquals equals = (com.epl.geometry.OperatorEquals)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Equals));
			com.epl.geometry.OperatorWithin within = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.MultiPoint multipoint1 = new com.epl.geometry.MultiPoint();
			com.epl.geometry.Point point2 = new com.epl.geometry.Point();
			multipoint1.Add(2, 2);
			multipoint1.Add(2, 2);
			point2.SetXY(2, 2);
			WiggleGeometry(multipoint1, 0.001, 123);
			bool res = equals.Execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = equals.Execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = within.Execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = within.Execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint1.Add(4, 4);
			res = equals.Execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = equals.Execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.Execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.Execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPointPointEquals()
		{
			com.epl.geometry.OperatorEquals equals = (com.epl.geometry.OperatorEquals)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Equals));
			com.epl.geometry.OperatorWithin within = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
			com.epl.geometry.OperatorContains contains = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
			com.epl.geometry.OperatorDisjoint disjoint = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.Point point1 = new com.epl.geometry.Point();
			com.epl.geometry.Point point2 = new com.epl.geometry.Point();
			point1.SetXY(2, 2);
			point2.SetXY(2, 2);
			bool res = equals.Execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = equals.Execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = within.Execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = within.Execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = contains.Execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = contains.Execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.Execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.Execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			point2.SetXY(2, 3);
			res = equals.Execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = equals.Execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.Execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.Execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.Execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.Execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.Execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.Execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

//		[NUnit.Framework.Test]
//		public virtual void TestPolygonPolygonDisjoint()
//		{
//			com.epl.geometry.OperatorDisjoint disjoint = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
//			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
//			// Polygon1 and Polygon2 are topologically equal, but have differing
//			// number of vertices
//			string str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			string str2 = "{\"rings\":[[[0,10],[10,10],[10,0],[0,0],[0,10]],[[9,1],[9,6],[9,9],[1,9],[1,1],[1,1],[9,1]]]}";
//			com.epl.geometry.Polygon polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			com.epl.geometry.Polygon polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			bool res = disjoint.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = disjoint.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// Polygon1 and Polygon2 touch at a point
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"rings\":[[[10,10],[10,15],[15,15],[15,10],[10,10]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polygon1, tolerance, 1982);
//			WiggleGeometry(polygon2, tolerance, 511);
//			res = disjoint.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = disjoint.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// Polygon1 and Polygon2 touch along the boundary
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"rings\":[[[10,0],[10,10],[15,10],[15,0],[10,0]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polygon1, tolerance, 1982);
//			WiggleGeometry(polygon2, tolerance, 511);
//			res = disjoint.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = disjoint.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// Polygon2 is inside of the hole of polygon1
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[8,2],[2,2]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = disjoint.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = disjoint.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			// Polygon2 is inside of the hole of polygon1
//			str1 = "{\"rings\":[[[0,0],[0,5],[5,5],[5,0]],[[10,0],[10,10],[20,10],[20,0]]]}";
//			str2 = "{\"rings\":[[[0,-10],[0,-5],[5,-5],[5,-10]],[[11,1],[11,9],[19,9],[19,1]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = disjoint.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = disjoint.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			polygon1 = (com.epl.geometry.Polygon)com.epl.geometry.OperatorDensifyByLength.Local().Execute(polygon1, 0.5, null);
//			disjoint.AccelerateGeometry(polygon1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
//			res = disjoint.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = disjoint.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			polygon1.ReverseAllPaths();
//			polygon2.ReverseAllPaths();
//			res = disjoint.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = disjoint.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// Polygon1 contains polygon2, but polygon2 is counterclockwise.
//			str1 = "{\"rings\":[[[0,0],[10,0],[10,10],[0,10],[0,0]],[[11,0],[11,10],[21,10],[21,0],[11,0]]]}";
//			str2 = "{\"rings\":[[[2,2],[8,2],[8,8],[2,8],[2,2]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = disjoint.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = disjoint.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			polygon1 = (com.epl.geometry.Polygon)com.epl.geometry.OperatorDensifyByLength.Local().Execute(polygon1, 0.5, null);
//			disjoint.AccelerateGeometry(polygon1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
//			res = disjoint.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = disjoint.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[0,20],[0,30],[10,30],[10,20],[0,20]],[[20,20],[20,30],[30,30],[30,20],[20,20]],[[20,0],[20,10],[30,10],[30,0],[20,0]]]}";
//			str2 = "{\"rings\":[[[14,14],[14,16],[16,16],[16,14],[14,14]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = disjoint.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = disjoint.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			polygon1 = (com.epl.geometry.Polygon)com.epl.geometry.OperatorDensifyByLength.Local().Execute(polygon1, 0.5, null);
//			disjoint.AccelerateGeometry(polygon1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
//			res = disjoint.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = disjoint.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestPolylinePolylineDisjoint()
//		{
//			com.epl.geometry.OperatorDisjoint disjoint = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
//			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
//			// Polyline1 and Polyline2 touch at a point
//			string str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			string str2 = "{\"paths\":[[[10,10],[10,15],[15,15],[15,10],[10,10]]]}";
//			com.epl.geometry.Polyline polyline1 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			com.epl.geometry.Polyline polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polyline1, tolerance, 1982);
//			WiggleGeometry(polyline2, tolerance, 511);
//			bool res = disjoint.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = disjoint.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// Polyline1 and Polyline2 touch along the boundary
//			str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"paths\":[[[10,0],[10,10],[15,10],[15,0],[10,0]]]}";
//			polyline1 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polyline1, tolerance, 1982);
//			WiggleGeometry(polyline2, tolerance, 511);
//			res = disjoint.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = disjoint.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// Polyline2 does not intersect with Polyline1
//			str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"paths\":[[[2,2],[2,8],[8,8],[8,2],[2,2]]]}";
//			polyline1 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = disjoint.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = disjoint.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//		}

		[NUnit.Framework.Test]
		public virtual void TestPolygonPolylineDisjoint()
		{
			com.epl.geometry.OperatorDisjoint disjoint = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.Polygon polygon1 = new com.epl.geometry.Polygon();
			com.epl.geometry.Polyline polyline2 = new com.epl.geometry.Polyline();
			polygon1.StartPath(0, 0);
			polygon1.LineTo(0, 10);
			polygon1.LineTo(10, 10);
			polygon1.LineTo(10, 0);
			polygon1.StartPath(1, 1);
			polygon1.LineTo(9, 1);
			polygon1.LineTo(9, 9);
			polygon1.LineTo(1, 9);
			polyline2.StartPath(3, 3);
			polyline2.LineTo(6, 6);
			bool res = disjoint.Execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.Execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline2.StartPath(0, 0);
			polyline2.LineTo(0, 5);
			res = disjoint.Execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.Execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polygon1.SetEmpty();
			polyline2.SetEmpty();
			polygon1.StartPath(0, 0);
			polygon1.LineTo(0, 10);
			polygon1.LineTo(10, 10);
			polygon1.LineTo(10, 0);
			polyline2.StartPath(2, 2);
			polyline2.LineTo(4, 4);
			com.epl.geometry.OperatorFactoryLocal factory = com.epl.geometry.OperatorFactoryLocal.GetInstance();
			com.epl.geometry.OperatorSimplify simplify_op = (com.epl.geometry.OperatorSimplify)factory.GetOperator(com.epl.geometry.Operator.Type.Simplify);
			simplify_op.IsSimpleAsFeature(polygon1, sr, null);
			simplify_op.IsSimpleAsFeature(polyline2, sr, null);
			res = disjoint.Execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.Execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolylineMultiPointDisjoint()
		{
			com.epl.geometry.OperatorDisjoint disjoint = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.Polyline polyline1 = new com.epl.geometry.Polyline();
			com.epl.geometry.MultiPoint multipoint2 = new com.epl.geometry.MultiPoint();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(2, 0);
			polyline1.LineTo(4, 2);
			multipoint2.Add(1, 1);
			multipoint2.Add(2, 2);
			multipoint2.Add(3, 0);
			bool res = disjoint.Execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.Execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.Add(3, 1);
			res = disjoint.Execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.Execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.StartPath(1, -4);
			polyline1.LineTo(1, -3);
			polyline1.LineTo(1, -2);
			polyline1.LineTo(1, -1);
			polyline1.LineTo(1, 0);
			polyline1.LineTo(1, 1);
			disjoint.AccelerateGeometry(polyline1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
			res = disjoint.Execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.Execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolylinePointDisjoint()
		{
			com.epl.geometry.OperatorDisjoint disjoint = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
			com.epl.geometry.OperatorContains contains = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.Polyline polyline1 = new com.epl.geometry.Polyline();
			com.epl.geometry.Point point2 = new com.epl.geometry.Point();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(2, 0);
			polyline1.LineTo(4, 2);
			point2.SetXY(1, 1);
			bool res = disjoint.Execute(polyline1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.Execute(point2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = contains.Execute(polyline1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.Execute(point2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			point2.SetXY(4, 2);
			polyline1 = (com.epl.geometry.Polyline)com.epl.geometry.OperatorDensifyByLength.Local().Execute(polyline1, 0.1, null);
			disjoint.AccelerateGeometry(polyline1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
			res = disjoint.Execute(polyline1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.Execute(point2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.Execute(polyline1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.Execute(point2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.SetEmpty();
			point2.SetEmpty();
			polyline1.StartPath(659062.37370000035, 153070.85220000148);
			polyline1.LineTo(660916.47940000147, 151481.10269999877);
			point2.SetXY(659927.85020000115, 152328.77430000156);
			res = contains.Execute(polyline1, point2, com.epl.geometry.SpatialReference.Create(54004), null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPointMultiPointDisjoint()
		{
			com.epl.geometry.OperatorDisjoint disjoint = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.MultiPoint multipoint1 = new com.epl.geometry.MultiPoint();
			com.epl.geometry.MultiPoint multipoint2 = new com.epl.geometry.MultiPoint();
			multipoint1.Add(2, 2);
			multipoint1.Add(2, 5);
			multipoint1.Add(4, 1);
			multipoint1.Add(4, 4);
			multipoint1.Add(4, 7);
			multipoint1.Add(6, 2);
			multipoint1.Add(6, 6);
			multipoint1.Add(4, 1);
			multipoint1.Add(6, 6);
			multipoint2.Add(0, 1);
			multipoint2.Add(0, 7);
			multipoint2.Add(4, 2);
			multipoint2.Add(4, 6);
			multipoint2.Add(6, 4);
			multipoint2.Add(4, 2);
			multipoint2.Add(0, 1);
			bool res = disjoint.Execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.Execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.Add(2, 2);
			res = disjoint.Execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.Execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPointPointDisjoint()
		{
			com.epl.geometry.OperatorDisjoint disjoint = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
			com.epl.geometry.OperatorContains contains = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.MultiPoint multipoint1 = new com.epl.geometry.MultiPoint();
			com.epl.geometry.Point point2 = new com.epl.geometry.Point();
			multipoint1.Add(2, 2);
			multipoint1.Add(2, 5);
			multipoint1.Add(4, 1);
			multipoint1.Add(4, 4);
			multipoint1.Add(4, 7);
			multipoint1.Add(6, 2);
			multipoint1.Add(6, 6);
			multipoint1.Add(4, 1);
			multipoint1.Add(6, 6);
			point2.SetXY(2, 6);
			bool res = disjoint.Execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.Execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = contains.Execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.Execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			multipoint1.Add(2, 6);
			res = disjoint.Execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.Execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.Execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = contains.Execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygonMultiPointDisjoint()
		{
			com.epl.geometry.OperatorDisjoint disjoint = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.Polygon polygon1 = new com.epl.geometry.Polygon();
			com.epl.geometry.MultiPoint multipoint2 = new com.epl.geometry.MultiPoint();
			polygon1.StartPath(0, 0);
			polygon1.LineTo(0, 10);
			polygon1.LineTo(10, 10);
			multipoint2.Add(-1, 5);
			multipoint2.Add(5, 11);
			multipoint2.Add(11, 5);
			multipoint2.Add(5, -1);
			bool res = disjoint.Execute(polygon1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.Execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon1.StartPath(15, 0);
			polygon1.LineTo(15, 10);
			polygon1.LineTo(25, 10);
			polygon1.LineTo(25, 0);
			multipoint2.Add(14, 5);
			multipoint2.Add(20, 11);
			multipoint2.Add(26, 5);
			multipoint2.Add(20, -1);
			res = disjoint.Execute(polygon1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.Execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.Add(20, 5);
			res = disjoint.Execute(polygon1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.Execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygonMultiPointTouches()
		{
			com.epl.geometry.OperatorTouches touches = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.Polygon polygon1 = new com.epl.geometry.Polygon();
			com.epl.geometry.MultiPoint multipoint2 = new com.epl.geometry.MultiPoint();
			polygon1.StartPath(0, 0);
			polygon1.LineTo(0, 10);
			polygon1.LineTo(10, 10);
			polygon1.LineTo(10, 0);
			multipoint2.Add(-1, 5);
			multipoint2.Add(5, 11);
			multipoint2.Add(11, 5);
			multipoint2.Add(5, -1);
			bool res = touches.Execute(polygon1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.Execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			multipoint2.Add(5, 10);
			res = touches.Execute(polygon1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.Execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.Add(5, 5);
			res = touches.Execute(polygon1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.Execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

//		[NUnit.Framework.Test]
//		public virtual void TestPolygonPointTouches()
//		{
//			com.epl.geometry.OperatorTouches touches = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
//			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
//			com.epl.geometry.Polygon polygon1 = new com.epl.geometry.Polygon();
//			com.epl.geometry.Point point2 = new com.epl.geometry.Point();
//			polygon1.StartPath(0, 0);
//			polygon1.LineTo(0, 10);
//			polygon1.LineTo(10, 10);
//			polygon1.LineTo(10, 0);
//			point2.SetXY(5, 5);
//			bool res = touches.Execute(polygon1, point2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = touches.Execute(point2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			point2.SetXY(5, 10);
//			res = touches.Execute(polygon1, point2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = touches.Execute(point2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestPolygonPolygonTouches()
//		{
//			com.epl.geometry.OperatorTouches touches = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
//			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
//			// Polygon1 and Polygon2 touch at a point
//			string str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			string str2 = "{\"rings\":[[[10,10],[10,15],[15,15],[15,10],[10,10]]]}";
//			com.epl.geometry.Polygon polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			com.epl.geometry.Polygon polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polygon1, tolerance, 1982);
//			WiggleGeometry(polygon2, tolerance, 511);
//			bool res = touches.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = touches.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			// Polygon1 and Polygon2 touch along the boundary
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"rings\":[[[10,0],[10,10],[15,10],[15,0],[10,0]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polygon1, tolerance, 1982);
//			WiggleGeometry(polygon2, tolerance, 511);
//			res = touches.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = touches.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			// Polygon1 and Polygon2 touch at a corner of Polygon1 and a diagonal of
//			// Polygon2
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"rings\":[[[15,5],[5,15],[15,15],[15,5]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polygon1, tolerance, 1982);
//			WiggleGeometry(polygon2, tolerance, 511);
//			res = touches.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = touches.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			// Polygon1 and Polygon2 do not touch
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"rings\":[[[5,5],[5,15],[15,15],[15,5],[5,5]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = touches.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = touches.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			polygon1.SetEmpty();
//			polygon2.SetEmpty();
//			polygon1.StartPath(0, 0);
//			polygon1.LineTo(0, 1);
//			polygon1.LineTo(-1, 0);
//			polygon2.StartPath(0, 0);
//			polygon2.LineTo(0, 1);
//			polygon2.LineTo(1, 0);
//			res = touches.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = touches.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestPolygonPolylineTouches()
//		{
//			com.epl.geometry.OperatorTouches touches = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
//			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
//			// Polygon1 and Polyline2 touch at a point
//			string str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			string str2 = "{\"paths\":[[[10,10],[10,15],[15,15],[15,10]]]}";
//			com.epl.geometry.Polygon polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			com.epl.geometry.Polyline polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polygon1, tolerance, 1982);
//			WiggleGeometry(polyline2, tolerance, 511);
//			bool res = touches.Execute(polygon1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = touches.Execute(polyline2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			// Polygon1 and Polyline2 overlap along the boundary
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"paths\":[[[10,0],[10,10],[15,10],[15,0]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polygon1, tolerance, 1982);
//			WiggleGeometry(polyline2, tolerance, 511);
//			res = touches.Execute(polygon1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = touches.Execute(polyline2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"paths\":[[[15,5],[5,15],[15,15],[15,5]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polygon1, tolerance, 1982);
//			WiggleGeometry(polyline2, tolerance, 511);
//			res = touches.Execute(polygon1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = touches.Execute(polyline2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			str1 = "{\"rings\":[[[10,10],[10,0],[0,0],[0,10],[10,10]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"paths\":[[[15,5],[5,15],[15,15]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polygon1, tolerance, 1982);
//			WiggleGeometry(polyline2, tolerance, 511);
//			res = touches.Execute(polygon1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = touches.Execute(polyline2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestPolylinePolylineTouches()
//		{
//			com.epl.geometry.OperatorTouches touches = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
//			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
//			// Polyline1 and Polyline2 touch at a point
//			string str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			string str2 = "{\"paths\":[[[10,10],[10,15],[15,15],[15,10]]]}";
//			com.epl.geometry.Polyline polyline1 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			com.epl.geometry.Polyline polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			bool res = touches.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = touches.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			// Polyline1 and Polyline2 overlap along the boundary
//			str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"paths\":[[[10,0],[10,10],[15,10],[15,0],[10,0]]]}";
//			polyline1 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = touches.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = touches.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// Polyline1 and Polyline2 intersect at interiors
//			str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"paths\":[[[15,5],[5,15],[15,15],[15,5]]]}";
//			polyline1 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = touches.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = touches.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// Polyline1 and Polyline2 touch at an endpoint of Polyline1 and
//			// interior of Polyline2 (but Polyline1 is closed)
//			str1 = "{\"paths\":[[[10,10],[10,0],[0,0],[0,10],[10,10]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"paths\":[[[15,5],[5,15],[15,15]]]}";
//			polyline1 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = touches.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = touches.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// Polyline1 and Polyline2 touch at an endpoint of Polyline1 and
//			// interior of Polyline2 (same as previous case, but Polyline1 is not
//			// closed)
//			str1 = "{\"paths\":[[[10,10],[10,0],[0,0],[0,10]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"paths\":[[[15,5],[5,15],[15,15]]]}";
//			polyline1 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = touches.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = touches.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			str1 = "{\"paths\":[[[10,10],[10,0],[0,0],[0,10]],[[1,1],[9,1],[9,9],[1,9],[6, 9]]]}";
//			str2 = "{\"paths\":[[[15,5],[5,15],[15,15],[15,5]]]}";
//			polyline1 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = touches.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = touches.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			polyline1.SetEmpty();
//			polyline2.SetEmpty();
//			polyline1.StartPath(-2, -2);
//			polyline1.LineTo(-1, -1);
//			polyline1.LineTo(1, 1);
//			polyline1.LineTo(2, 2);
//			polyline2.StartPath(-2, 2);
//			polyline2.LineTo(-1, 1);
//			polyline2.LineTo(1, -1);
//			polyline2.LineTo(2, -2);
//			res = touches.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			polyline1.SetEmpty();
//			polyline2.SetEmpty();
//			polyline1.StartPath(-2, -2);
//			polyline1.LineTo(-1, -1);
//			polyline1.LineTo(1, 1);
//			polyline1.LineTo(2, 2);
//			polyline2.StartPath(-2, 2);
//			polyline2.LineTo(-1, 1);
//			polyline2.LineTo(1, -1);
//			res = touches.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			polyline1.SetEmpty();
//			polyline2.SetEmpty();
//			polyline1.StartPath(-1, -1);
//			polyline1.LineTo(0, 0);
//			polyline1.LineTo(1, 1);
//			polyline2.StartPath(-1, 1);
//			polyline2.LineTo(0, 0);
//			polyline2.LineTo(1, -1);
//			res = touches.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			polyline1.SetEmpty();
//			polyline2.SetEmpty();
//			polyline1.StartPath(0, 0);
//			polyline1.LineTo(0, 1);
//			polyline1.LineTo(0, 0);
//			polyline2.StartPath(0, 1);
//			polyline2.LineTo(0, 2);
//			polyline2.LineTo(0, 1);
//			res = touches.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = touches.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//		}

		[NUnit.Framework.Test]
		public virtual void TestPolylineMultiPointTouches()
		{
			com.epl.geometry.OperatorTouches touches = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.Polyline polyline1 = new com.epl.geometry.Polyline();
			com.epl.geometry.MultiPoint multipoint2 = new com.epl.geometry.MultiPoint();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(2, 0);
			polyline1.LineTo(4, 2);
			multipoint2.Add(1, 1);
			multipoint2.Add(2, 2);
			multipoint2.Add(3, 0);
			bool res = touches.Execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.Execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.StartPath(1, -4);
			polyline1.LineTo(1, -3);
			polyline1.LineTo(1, -2);
			polyline1.LineTo(1, -1);
			polyline1.LineTo(1, 0);
			polyline1.LineTo(1, 1);
			touches.AccelerateGeometry(polyline1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
			res = touches.Execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.Execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.Add(3, 1);
			res = touches.Execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.Execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.StartPath(0, 0);
			polyline1.LineTo(2, 0);
			polyline1.StartPath(2, 1);
			polyline1.LineTo(2, -1);
			multipoint2.Add(2, 0);
			res = touches.Execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.Execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolylineMultiPointCrosses()
		{
			com.epl.geometry.OperatorCrosses crosses = (com.epl.geometry.OperatorCrosses)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Crosses));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.Polyline polyline1 = new com.epl.geometry.Polyline();
			com.epl.geometry.MultiPoint multipoint2 = new com.epl.geometry.MultiPoint();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(2, 0);
			polyline1.LineTo(4, 2);
			multipoint2.Add(1, 1);
			multipoint2.Add(2, 2);
			multipoint2.Add(3, 0);
			multipoint2.Add(0, 0);
			bool res = crosses.Execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = crosses.Execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.StartPath(1, -4);
			polyline1.LineTo(1, -3);
			polyline1.LineTo(1, -2);
			polyline1.LineTo(1, -1);
			polyline1.LineTo(1, 0);
			polyline1.LineTo(1, 1);
			res = crosses.Execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = crosses.Execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			crosses.AccelerateGeometry(polyline1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
			multipoint2.Add(1, 0);
			res = crosses.Execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = crosses.Execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.Add(3, 1);
			res = crosses.Execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = crosses.Execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

//		[NUnit.Framework.Test]
//		public virtual void TestPolylinePointTouches()
//		{
//			com.epl.geometry.OperatorTouches touches = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
//			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
//			com.epl.geometry.Polyline polyline1 = new com.epl.geometry.Polyline();
//			com.epl.geometry.Point point2 = new com.epl.geometry.Point();
//			polyline1.StartPath(0, 0);
//			polyline1.LineTo(2, 0);
//			polyline1.StartPath(2, 1);
//			polyline1.LineTo(2, -1);
//			point2.SetXY(2, 0);
//			bool res = touches.Execute(polyline1, point2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = touches.Execute(point2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestPolygonPolygonOverlaps()
//		{
//			com.epl.geometry.OperatorOverlaps overlaps = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
//			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
//			// Polygon1 and Polygon2 touch at a point
//			string str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			string str2 = "{\"rings\":[[[10,10],[10,15],[15,15],[15,10],[10,10]]]}";
//			com.epl.geometry.Polygon polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			com.epl.geometry.Polygon polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polygon1, tolerance, 1982);
//			WiggleGeometry(polygon2, tolerance, 511);
//			bool res = overlaps.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = overlaps.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// Polygon1 and Polygon2 touch along the boundary
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"rings\":[[[10,0],[10,10],[15,10],[15,0],[10,0]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polygon1, tolerance, 1982);
//			WiggleGeometry(polygon2, tolerance, 511);
//			res = overlaps.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = overlaps.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// Polygon1 and Polygon2 touch at a corner of Polygon1 and a diagonal of
//			// Polygon2
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"rings\":[[[15,5],[5,15],[15,15],[15,5]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polygon1, tolerance, 1982);
//			WiggleGeometry(polygon2, tolerance, 511);
//			res = overlaps.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = overlaps.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// Polygon1 and Polygon2 overlap at the upper right corner
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"rings\":[[[5,5],[5,15],[15,15],[15,5],[5,5]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = overlaps.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = overlaps.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[4,4],[6,4],[6,6],[4,6],[4,4],[4,4]]]}";
//			str2 = "{\"rings\":[[[1,1],[1,9],[9,9],[9,1],[1,1]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = overlaps.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = overlaps.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//		}

		[NUnit.Framework.Test]
		public virtual void TestPolygonPolylineWithin()
		{
			com.epl.geometry.OperatorWithin within = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.Polygon polygon1 = new com.epl.geometry.Polygon();
			com.epl.geometry.Polyline polyline2 = new com.epl.geometry.Polyline();
			polygon1.StartPath(0, 0);
			polygon1.LineTo(0, 10);
			polygon1.LineTo(10, 10);
			polygon1.LineTo(10, 0);
			polyline2.StartPath(5, 0);
			polyline2.LineTo(5, 10);
			bool res = within.Execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.Execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline2.SetEmpty();
			polyline2.StartPath(0, 1);
			polyline2.LineTo(0, 9);
			res = within.Execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPointMultiPointWithin()
		{
			com.epl.geometry.OperatorWithin within = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.MultiPoint multipoint1 = new com.epl.geometry.MultiPoint();
			com.epl.geometry.MultiPoint multipoint2 = new com.epl.geometry.MultiPoint();
			multipoint1.Add(0, 0);
			multipoint1.Add(3, 3);
			multipoint1.Add(0, 0);
			multipoint1.Add(5, 5);
			multipoint1.Add(3, 3);
			multipoint1.Add(2, 4);
			multipoint1.Add(2, 8);
			multipoint2.Add(0, 0);
			multipoint2.Add(3, 3);
			multipoint2.Add(2, 4);
			multipoint2.Add(2, 8);
			multipoint2.Add(5, 5);
			bool res = within.Execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = within.Execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.Add(10, 10);
			multipoint2.Add(10, 10);
			res = within.Execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = within.Execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			multipoint1.Add(10, 10);
			res = within.Execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = within.Execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint1.Add(-10, -10);
			res = within.Execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.Execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolylinePolylineOverlaps()
		{
			com.epl.geometry.OperatorOverlaps overlaps = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.Polyline polyline1 = new com.epl.geometry.Polyline();
			com.epl.geometry.Polyline polyline2 = new com.epl.geometry.Polyline();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(2, 0);
			polyline2.StartPath(1, 0);
			polyline2.LineTo(3, 0);
			polyline2.LineTo(1, 1);
			polyline2.LineTo(1, -1);
			WiggleGeometry(polyline1, tolerance, 1982);
			WiggleGeometry(polyline2, tolerance, 511);
			bool res = overlaps.Execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = overlaps.Execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.SetEmpty();
			polyline2.SetEmpty();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(2, 0);
			polyline2.StartPath(1.9989, 0);
			polyline2.LineTo(2.0011, 0);
			// wiggleGeometry(polyline1, tolerance, 1982);
			// wiggleGeometry(polyline2, tolerance, 511);
			res = overlaps.Execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = overlaps.Execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.SetEmpty();
			polyline2.SetEmpty();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(2, 0);
			polyline2.StartPath(1.9989, 0);
			polyline2.LineTo(2.0009, 0);
			WiggleGeometry(polyline1, tolerance, 1982);
			WiggleGeometry(polyline2, tolerance, 511);
			res = overlaps.Execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = overlaps.Execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.SetEmpty();
			polyline2.SetEmpty();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(2, 0);
			polyline2.StartPath(0, 0);
			polyline2.LineTo(2, 0);
			polyline2.StartPath(0, -1);
			polyline2.LineTo(2, -1);
			res = overlaps.Execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = overlaps.Execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPointMultiPointOverlaps()
		{
			com.epl.geometry.OperatorOverlaps overlaps = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.MultiPoint multipoint1 = new com.epl.geometry.MultiPoint();
			com.epl.geometry.MultiPoint multipoint2 = new com.epl.geometry.MultiPoint();
			multipoint1.Add(4, 4);
			multipoint1.Add(6, 4);
			multipoint2.Add(6, 2);
			multipoint2.Add(2, 6);
			bool res = overlaps.Execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = overlaps.Execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			multipoint1.Add(10, 10);
			multipoint2.Add(6, 2);
			res = overlaps.Execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = overlaps.Execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			multipoint1.Add(6, 2);
			res = overlaps.Execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = overlaps.Execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint1.Add(2, 6);
			res = overlaps.Execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = overlaps.Execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			multipoint2.Add(1, 1);
			res = overlaps.Execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = overlaps.Execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.Add(10, 10);
			multipoint2.Add(4, 4);
			multipoint2.Add(6, 4);
			res = overlaps.Execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = overlaps.Execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

//		[NUnit.Framework.Test]
//		public virtual void TestPolygonPolygonWithin()
//		{
//			com.epl.geometry.OperatorWithin within = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
//			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
//			// Polygon1 is within Polygon2
//			string str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			string str2 = "{\"rings\":[[[-1,-1],[-1,11],[11,11],[11,-1],[-1,-1]]]}";
//			com.epl.geometry.Polygon polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			com.epl.geometry.Polygon polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			bool res = within.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = within.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// Polygon1 is within Polygon2, and the boundaries intersect
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[4,4],[6,4],[6,6],[4,6],[4,4],[4,4]]]}";
//			str2 = "{\"rings\":[[[1,1],[1,9],[9,9],[9,1],[1,1]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polygon1, tolerance, 1982);
//			WiggleGeometry(polygon2, tolerance, 511);
//			res = within.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// Polygon1 is within Polygon2, and the boundaries intersect
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"rings\":[[[-1,0],[-1,11],[11,11],[11,0],[-1,0]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			WiggleGeometry(polygon1, tolerance, 1982);
//			WiggleGeometry(polygon2, tolerance, 511);
//			res = within.Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			// Polygon2 is inside of the hole of polygon1
//			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[8,2],[2,2]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			str1 = "{\"rings\":[[[0,0],[10,0],[10,10],[0,10]]]}";
//			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[8,2],[2,2],[8,2],[8,8],[2,8],[2,2]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0]],[[12,8],[12,10],[18,10],[18,8],[12,8]]]}";
//			str2 = "{\"paths\":[[[2,2],[2,8],[8,8],[8,2]],[[12,2],[12,4],[18,4],[18,2]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			com.epl.geometry.Polyline polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polyline2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[4,4],[6,4],[6,6],[4,6],[4,4]]]}";
//			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[8,2],[2,2],[2,8],[8,8],[8,2],[2,2]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			// Same as above, but winding fill rule
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[4,4],[6,4],[6,6],[4,6],[4,4]]]}";
//			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[8,2],[2,2],[2,8],[8,8],[8,2],[2,2]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			polygon1.SetFillRule(com.epl.geometry.Polygon.FillRule.enumFillRuleWinding);
//			polygon2.SetFillRule(com.epl.geometry.Polygon.FillRule.enumFillRuleWinding);
//			res = within.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]]]}";
//			str2 = "{\"paths\":[[[2,2],[2,2]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polyline2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[11,11],[11,20],[20,20],[20,11],[11,11]]]}";
//			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[15,15],[8,8],[8,2],[2,2]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[10,10],[10,20],[20,20],[20,10],[10,10]]]}";
//			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[15,15],[8,8],[8,2],[2,2]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]]]}";
//			str2 = "{\"rings\":[[[9.9999999925,4],[9.9999999925,6],[10.0000000075,6],[10.0000000075,4],[9.9999999925,4]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = com.epl.geometry.OperatorOverlaps.Local().Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = com.epl.geometry.OperatorTouches.Local().Execute(polygon1, polygon2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[10,10],[10,20],[20,20],[20,10],[10,10]]]}";
//			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[15,15],[8,8],[8,2],[2,2]],[[15,5],[15,5],[15,5]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]]]}";
//			str2 = "{\"rings\":[[[2,2],[2,2],[2,2]],[[3,3],[3,3],[3,3]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]]]}";
//			str2 = "{\"rings\":[[[2,2],[2,2],[2,2],[2,2]],[[3,3],[3,3],[3,3],[3,3]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polygon2 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polygon2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]]]}";
//			str2 = "{\"paths\":[[[2,2],[2,2]],[[3,3],[3,3]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polyline2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[10,10],[10,20],[20,20],[20,10],[10,10]]]}";
//			str2 = "{\"paths\":[[[2,2],[2,8]],[[15,5],[15,5]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polyline2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[10,10],[10,20],[20,20],[20,10],[10,10]]]}";
//			str2 = "{\"paths\":[[[2,2],[2,8]],[[15,5],[15,5],[15,5],[15,5]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polyline2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[10,10],[10,20],[20,20],[20,10],[10,10]]]}";
//			str2 = "{\"paths\":[[[2,2],[2,2]],[[15,5],[15,6]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polyline2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[10,10],[10,20],[20,20],[20,10],[10,10]]]}";
//			str2 = "{\"paths\":[[[2,2],[2,2],[2,2],[2,2]],[[15,5],[15,6]]]}";
//			polygon1 = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = within.Execute(polyline2, polygon1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//		}

		[NUnit.Framework.Test]
		public virtual void TestPolylinePolylineWithin()
		{
			com.epl.geometry.OperatorWithin within = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
			com.epl.geometry.OperatorContains contains = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.Polyline polyline1 = new com.epl.geometry.Polyline();
			com.epl.geometry.Polyline polyline2 = new com.epl.geometry.Polyline();
			polyline1.SetEmpty();
			polyline2.SetEmpty();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(2, 0);
			polyline2.StartPath(1.9989, 0);
			polyline2.LineTo(2.0011, 0);
			bool res = within.Execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.Execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.SetEmpty();
			polyline2.SetEmpty();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(2, 0);
			polyline2.StartPath(1.9989, 0);
			polyline2.LineTo(2.001, 0);
			res = within.Execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = contains.Execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.SetEmpty();
			polyline2.SetEmpty();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(2, 0);
			polyline1.LineTo(3, 0);
			polyline1.LineTo(4, 0);
			polyline1.LineTo(5, 0);
			polyline1.LineTo(6, 0);
			polyline1.LineTo(7, 0);
			polyline1.LineTo(8, 0);
			polyline2.StartPath(0, 0);
			polyline2.LineTo(.1, 0);
			polyline2.LineTo(.2, 0);
			polyline2.LineTo(.4, 0);
			polyline2.LineTo(1.1, 0);
			polyline2.LineTo(2.5, 0);
			polyline2.StartPath(2.7, 0);
			polyline2.LineTo(4, 0);
			res = within.Execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = contains.Execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolylineMultiPointWithin()
		{
			com.epl.geometry.OperatorWithin within = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.Polyline polyline1 = new com.epl.geometry.Polyline();
			com.epl.geometry.MultiPoint multipoint2 = new com.epl.geometry.MultiPoint();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(2, 0);
			polyline1.LineTo(4, 2);
			multipoint2.Add(1, 0);
			multipoint2.Add(2, 0);
			multipoint2.Add(3, 1);
			multipoint2.Add(2, 0);
			bool res = within.Execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.Execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.StartPath(1, -2);
			polyline1.LineTo(1, -1);
			polyline1.LineTo(1, 0);
			polyline1.LineTo(1, 1);
			res = within.Execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.Execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.Add(1, 2);
			res = within.Execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			multipoint2.Add(-1, -1);
			multipoint2.Add(4, 2);
			multipoint2.Add(0, 0);
			res = within.Execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygonMultiPointWithin()
		{
			com.epl.geometry.OperatorWithin within = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.Polygon polygon1 = new com.epl.geometry.Polygon();
			com.epl.geometry.MultiPoint multipoint2 = new com.epl.geometry.MultiPoint();
			polygon1.StartPath(0, 0);
			polygon1.LineTo(0, 10);
			polygon1.LineTo(10, 10);
			polygon1.LineTo(10, 0);
			multipoint2.Add(5, 0);
			multipoint2.Add(5, 10);
			multipoint2.Add(5, 5);
			bool res = within.Execute(polygon1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.Execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.Add(5, 11);
			res = within.Execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygonPolylineCrosses()
		{
			com.epl.geometry.OperatorCrosses crosses = (com.epl.geometry.OperatorCrosses)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Crosses));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
			com.epl.geometry.Polygon polygon1 = new com.epl.geometry.Polygon();
			com.epl.geometry.Polyline polyline2 = new com.epl.geometry.Polyline();
			polygon1.StartPath(0, 0);
			polygon1.LineTo(0, 10);
			polygon1.LineTo(10, 10);
			polygon1.LineTo(10, 0);
			polyline2.StartPath(5, -5);
			polyline2.LineTo(5, 15);
			bool res = crosses.Execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = crosses.Execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline2.SetEmpty();
			polyline2.StartPath(5, 0);
			polyline2.LineTo(5, 10);
			res = crosses.Execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = crosses.Execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polygon1.SetEmpty();
			polyline2.SetEmpty();
			polygon1.StartPath(0, 0);
			polygon1.LineTo(0, 10);
			polygon1.LineTo(10, 10);
			polygon1.LineTo(10, 8);
			polygon1.LineTo(15, 5);
			polygon1.LineTo(10, 2);
			polygon1.LineTo(10, 0);
			polyline2.StartPath(10, 15);
			polyline2.LineTo(10, -5);
			res = crosses.Execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = crosses.Execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

//		[NUnit.Framework.Test]
//		public virtual void TestPolylinePolylineCrosses()
//		{
//			com.epl.geometry.OperatorCrosses crosses = (com.epl.geometry.OperatorCrosses)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Crosses));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
//			double tolerance = sr.GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
//			// Polyline1 and Polyline2 touch at a point
//			string str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			string str2 = "{\"paths\":[[[10,10],[10,15],[15,15],[15,10]]]}";
//			com.epl.geometry.Polyline polyline1 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			com.epl.geometry.Polyline polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			bool res = crosses.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = crosses.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			// Polyline1 and Polyline2 intersect at interiors
//			str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"paths\":[[[15,5],[5,15],[15,15],[15,5]]]}";
//			polyline1 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = crosses.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = crosses.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			// Polyline1 and Polyline2 touch at an endpoint of Polyline1 and
//			// interior of Polyline2 (but Polyline1 is closed)
//			str1 = "{\"paths\":[[[10,10],[10,0],[0,0],[0,10],[10,10]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"paths\":[[[15,5],[5,15],[15,15]]]}";
//			polyline1 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = crosses.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = crosses.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			// Polyline1 and Polyline2 touch at an endpoint of Polyline1 and
//			// interior of Polyline2 (same as previous case, but Polyline1 is not
//			// closed)
//			str1 = "{\"paths\":[[[10,10],[10,0],[0,0],[0,10]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
//			str2 = "{\"paths\":[[[15,5],[5,15],[15,15]]]}";
//			polyline1 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = crosses.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			res = crosses.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(!res);
//			str1 = "{\"paths\":[[[10,11],[10,0],[0,0],[0,10]],[[1,1],[9,1],[9,9],[1,9],[6, 9]]]}";
//			str2 = "{\"paths\":[[[15,5],[5,15],[15,15],[15,5]]]}";
//			polyline1 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str1).GetGeometry());
//			polyline2 = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson(str2).GetGeometry());
//			res = crosses.Execute(polyline1, polyline2, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			res = crosses.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//			polyline1.SetEmpty();
//			polyline2.SetEmpty();
//			polyline1.StartPath(-2, -2);
//			polyline1.LineTo(-1, -1);
//			polyline1.LineTo(1, 1);
//			polyline1.LineTo(2, 2);
//			polyline2.StartPath(-2, 2);
//			polyline2.LineTo(-1, 1);
//			polyline2.LineTo(1, -1);
//			polyline2.LineTo(2, -2);
//			res = crosses.Execute(polyline2, polyline1, sr, null);
//			NUnit.Framework.Assert.IsTrue(res);
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestPolygonEnvelope()
//		{
//			com.epl.geometry.OperatorEquals equals = (com.epl.geometry.OperatorEquals)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Equals));
//			com.epl.geometry.OperatorContains contains = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
//			com.epl.geometry.OperatorDisjoint disjoint = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
//			com.epl.geometry.OperatorCrosses crosses = (com.epl.geometry.OperatorCrosses)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Crosses));
//			com.epl.geometry.OperatorWithin within = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
//			com.epl.geometry.OperatorOverlaps overlaps = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//			com.epl.geometry.OperatorTouches touches = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
//			com.epl.geometry.OperatorDensifyByLength densify = (com.epl.geometry.OperatorDensifyByLength)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.DensifyByLength));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(equals.Execute(envelope, densified, sr, null));
//				// they
//				// cover
//				// the
//				// same
//				// space
//				NUnit.Framework.Assert.IsTrue(contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":5,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				// the
//				// polygon
//				// contains
//				// the
//				// envelope,
//				// but
//				// they
//				// aren't
//				// equal
//				NUnit.Framework.Assert.IsTrue(contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":15,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				// the
//				// envelope
//				// sticks
//				// outside
//				// of
//				// the
//				// polygon
//				// but
//				// they
//				// intersect
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":5,\"ymin\":0,\"xmax\":15,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				// the
//				// envelope
//				// sticks
//				// outside
//				// of
//				// the
//				// polygon
//				// but
//				// they
//				// intersect
//				// and
//				// overlap
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":10,\"ymin\":0,\"xmax\":15,\"ymax\":5}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				// the
//				// envelope
//				// rides
//				// the
//				// side
//				// of
//				// the
//				// polygon
//				// (they
//				// touch)
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(contains.Execute(densified, envelope, sr, null));
//				// polygon
//				// and
//				// envelope
//				// cover
//				// the
//				// same
//				// space
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				// envelope
//				// sticks
//				// outside
//				// of
//				// polygon,
//				// but
//				// the
//				// envelopes
//				// are
//				// equal
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":15,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				// the
//				// polygon
//				// envelope
//				// doesn't
//				// contain
//				// the
//				// envelope,
//				// but
//				// they
//				// intersect
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":0,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				// envelope
//				// degenerate
//				// to
//				// a
//				// point
//				// and
//				// is
//				// on
//				// border
//				// (i.e.
//				// touches)
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":1,\"ymin\":1,\"xmax\":1,\"ymax\":1}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(densified, envelope, sr, null));
//				// envelope
//				// degenerate
//				// to
//				// a
//				// point
//				// and
//				// is
//				// properly
//				// inside
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":-1,\"ymin\":-1,\"xmax\":-1,\"ymax\":-1}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				// envelope
//				// degenerate
//				// to
//				// a
//				// point
//				// and
//				// is
//				// properly
//				// outside
//				NUnit.Framework.Assert.IsTrue(disjoint.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":1,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				// envelope
//				// degenerate
//				// to
//				// a
//				// line
//				// and
//				// rides
//				// the
//				// bottom
//				// of
//				// the
//				// polygon
//				// (no
//				// interior
//				// intersection)
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":1,\"xmax\":1,\"ymax\":1}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(densified, envelope, sr, null));
//				// envelope
//				// degenerate
//				// to
//				// a
//				// line,
//				// touches
//				// the
//				// border
//				// on
//				// the
//				// inside
//				// yet
//				// has
//				// interior
//				// intersection
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":5,\"ymin\":5,\"xmax\":6,\"ymax\":5}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				// envelope
//				// degenerate
//				// to
//				// a
//				// line,
//				// touches
//				// the
//				// boundary,
//				// and
//				// is
//				// outside
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":6,\"ymin\":5,\"xmax\":7,\"ymax\":5}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				// envelope
//				// degenerate
//				// to
//				// a
//				// line,
//				// and
//				// is
//				// outside
//				NUnit.Framework.Assert.IsTrue(disjoint.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polygon polygon = (com.epl.geometry.Polygon)(com.epl.geometry.TestCommonMethods.FromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,0],[0,0]]]}").GetGeometry());
//				com.epl.geometry.Polygon densified = (com.epl.geometry.Polygon)(densify.Execute(polygon, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":4,\"ymin\":5,\"xmax\":7,\"ymax\":5}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				// envelope
//				// degenerate
//				// to
//				// a
//				// line,
//				// and
//				// crosses
//				// polygon
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(crosses.Execute(envelope, densified, sr, null));
//			}
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestPolylineEnvelope()
//		{
//			com.epl.geometry.OperatorEquals equals = (com.epl.geometry.OperatorEquals)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Equals));
//			com.epl.geometry.OperatorContains contains = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
//			com.epl.geometry.OperatorDisjoint disjoint = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
//			com.epl.geometry.OperatorCrosses crosses = (com.epl.geometry.OperatorCrosses)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Crosses));
//			com.epl.geometry.OperatorWithin within = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
//			com.epl.geometry.OperatorOverlaps overlaps = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//			com.epl.geometry.OperatorTouches touches = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
//			com.epl.geometry.OperatorDensifyByLength densify = (com.epl.geometry.OperatorDensifyByLength)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.DensifyByLength));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				// polyline
//				// straddles
//				// the
//				// envelope
//				// like
//				// a hat
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[-10,0],[0,10]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[-11,0],[1,12]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[5,5],[6,6]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				// polyline
//				// properly
//				// inside
//				NUnit.Framework.Assert.IsTrue(contains.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[5,5],[10,10]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[-5,5],[15,5]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(crosses.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(crosses.Execute(densified, envelope, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[5,5],[5,15]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				// polyline
//				// slices
//				// through
//				// the
//				// envelope
//				// (interior
//				// and
//				// exterior
//				// intersection)
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[5,11],[5,15]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				// polyline
//				// outside
//				// of
//				// envelope
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":10,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				// polyline
//				// straddles
//				// the
//				// degenerate
//				// envelope
//				// like
//				// a hat
//				NUnit.Framework.Assert.IsTrue(contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":-5,\"xmax\":0,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":0,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 511);
//				WiggleGeometry(envelope, 0.00000001, 1982);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				// degenerate
//				// envelope
//				// is at
//				// the
//				// end
//				// point
//				// of
//				// polyline
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":5,\"xmax\":0,\"ymax\":5}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				// degenerate
//				// envelope
//				// is at
//				// the
//				// interior
//				// of
//				// polyline
//				NUnit.Framework.Assert.IsTrue(contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[2,-2],[2,2]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":5,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				// degenerate
//				// envelope
//				// crosses
//				// polyline
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[2,0],[2,2]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":5,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				// degenerate
//				// envelope
//				// crosses
//				// polyline
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[2,0],[2,2]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":2,\"ymin\":0,\"xmax\":2,\"ymax\":3}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				// degenerate
//				// envelope
//				// contains
//				// polyline
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(densified, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[5,5],[6,6]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":5,\"ymax\":5}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, densified, sr, null));
//				// polyline
//				// properly
//				// inside
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//			{
//				com.epl.geometry.Polyline polyline = (com.epl.geometry.Polyline)(com.epl.geometry.TestCommonMethods.FromJson("{\"paths\":[[[5,5],[5,10]]]}").GetGeometry());
//				com.epl.geometry.Polyline densified = (com.epl.geometry.Polyline)(densify.Execute(polyline, 1.0, null));
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":5,\"ymin\":5,\"xmax\":5,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(densified, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(equals.Execute(envelope, densified, sr, null));
//				// polyline
//				// properly
//				// inside
//				NUnit.Framework.Assert.IsTrue(contains.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, densified, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, densified, sr, null));
//			}
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestMultiPointEnvelope()
//		{
//			com.epl.geometry.OperatorEquals equals = (com.epl.geometry.OperatorEquals)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Equals));
//			com.epl.geometry.OperatorContains contains = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
//			com.epl.geometry.OperatorDisjoint disjoint = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
//			com.epl.geometry.OperatorCrosses crosses = (com.epl.geometry.OperatorCrosses)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Crosses));
//			com.epl.geometry.OperatorWithin within = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
//			com.epl.geometry.OperatorOverlaps overlaps = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//			com.epl.geometry.OperatorTouches touches = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
//			com.epl.geometry.OperatorDensifyByLength densify = (com.epl.geometry.OperatorDensifyByLength)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.DensifyByLength));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
//			{
//				com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)(com.epl.geometry.TestCommonMethods.FromJson("{\"points\":[[0,0],[0,10],[10,10],[10,0]]}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(multi_point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, multi_point, sr, null));
//				// all
//				// points
//				// on
//				// boundary
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, multi_point, sr, null));
//			}
//			{
//				com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)(com.epl.geometry.TestCommonMethods.FromJson("{\"points\":[[0,0],[0,10],[10,10],[5,5]]}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(multi_point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, multi_point, sr, null));
//				// points
//				// on
//				// boundary
//				// and
//				// one
//				// point
//				// in
//				// interior
//				NUnit.Framework.Assert.IsTrue(contains.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, multi_point, sr, null));
//			}
//			{
//				com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)(com.epl.geometry.TestCommonMethods.FromJson("{\"points\":[[0,0],[0,10],[10,10],[5,5],[15,15]]}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(multi_point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, multi_point, sr, null));
//				// points
//				// on
//				// boundary,
//				// one
//				// interior,
//				// one
//				// exterior
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(crosses.Execute(envelope, multi_point, sr, null));
//			}
//			{
//				com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)(com.epl.geometry.TestCommonMethods.FromJson("{\"points\":[[0,0],[0,10],[10,10],[15,15]]}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(multi_point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, multi_point, sr, null));
//				// points
//				// on
//				// boundary,
//				// one
//				// exterior
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, multi_point, sr, null));
//			}
//			{
//				com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)(com.epl.geometry.TestCommonMethods.FromJson("{\"points\":[[0,-1],[0,11],[11,11],[15,15]]}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(multi_point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, multi_point, sr, null));
//				// all
//				// points
//				// exterior
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(disjoint.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, multi_point, sr, null));
//			}
//			{
//				com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)(com.epl.geometry.TestCommonMethods.FromJson("{\"points\":[[0,0],[0,10],[10,10],[10,0]]}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":10,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(multi_point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, multi_point, sr, null));
//				// degenerate
//				// envelope
//				// slices
//				// through
//				// some
//				// points,
//				// but
//				// some
//				// points
//				// are
//				// off
//				// the
//				// line
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, multi_point, sr, null));
//			}
//			{
//				com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)(com.epl.geometry.TestCommonMethods.FromJson("{\"points\":[[0,0],[1,10],[10,10],[10,0]]}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":10,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(multi_point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, multi_point, sr, null));
//				// degenerate
//				// envelope
//				// slices
//				// through
//				// some
//				// points,
//				// but
//				// some
//				// points
//				// are
//				// off
//				// the
//				// line
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(crosses.Execute(envelope, multi_point, sr, null));
//			}
//			{
//				com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)(com.epl.geometry.TestCommonMethods.FromJson("{\"points\":[[0,10],[10,10]]}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":10,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(multi_point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, multi_point, sr, null));
//				// degenerate
//				// envelopes
//				// slices
//				// through
//				// all
//				// the
//				// points,
//				// and
//				// they
//				// are
//				// at
//				// the
//				// end
//				// points
//				// of
//				// the
//				// line
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, multi_point, sr, null));
//			}
//			{
//				com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)(com.epl.geometry.TestCommonMethods.FromJson("{\"points\":[[1,10],[9,10]]}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":10,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(multi_point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, multi_point, sr, null));
//				// degenerate
//				// envelopes
//				// slices
//				// through
//				// all
//				// the
//				// points,
//				// and
//				// they
//				// are
//				// in
//				// the
//				// interior
//				// of
//				// the
//				// line
//				NUnit.Framework.Assert.IsTrue(contains.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, multi_point, sr, null));
//			}
//			{
//				com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)(com.epl.geometry.TestCommonMethods.FromJson("{\"points\":[[0,-1],[0,11],[11,11],[15,15]]}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":10,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(multi_point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, multi_point, sr, null));
//				// all
//				// points
//				// exterior
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(disjoint.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, multi_point, sr, null));
//			}
//			{
//				com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)(com.epl.geometry.TestCommonMethods.FromJson("{\"points\":[[0,-1],[0,11],[11,11],[15,15]]}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":10,\"ymin\":10,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(multi_point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, multi_point, sr, null));
//				// all
//				// points
//				// exterior
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(disjoint.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, multi_point, sr, null));
//			}
//			{
//				com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)(com.epl.geometry.TestCommonMethods.FromJson("{\"points\":[[0,-1],[0,11],[11,11],[15,15]]}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":11,\"ymax\":11}").GetGeometry());
//				WiggleGeometry(multi_point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, multi_point, sr, null));
//				// all
//				// points
//				// exterior
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(multi_point, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, multi_point, sr, null));
//			}
//			{
//				com.epl.geometry.MultiPoint multi_point = (com.epl.geometry.MultiPoint)(com.epl.geometry.TestCommonMethods.FromJson("{\"points\":[[0,-1],[0,-1]]}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":-1,\"xmax\":0,\"ymax\":-1}").GetGeometry());
//				WiggleGeometry(multi_point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(equals.Execute(envelope, multi_point, sr, null));
//				// all
//				// points
//				// exterior
//				NUnit.Framework.Assert.IsTrue(contains.Execute(multi_point, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, multi_point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, multi_point, sr, null));
//			}
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestPointEnvelope()
//		{
//			com.epl.geometry.OperatorEquals equals = (com.epl.geometry.OperatorEquals)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Equals));
//			com.epl.geometry.OperatorContains contains = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
//			com.epl.geometry.OperatorDisjoint disjoint = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
//			com.epl.geometry.OperatorCrosses crosses = (com.epl.geometry.OperatorCrosses)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Crosses));
//			com.epl.geometry.OperatorWithin within = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
//			com.epl.geometry.OperatorOverlaps overlaps = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//			com.epl.geometry.OperatorTouches touches = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
//			{
//				com.epl.geometry.Point point = (com.epl.geometry.Point)(com.epl.geometry.TestCommonMethods.FromJson("{\"x\":5,\"y\":6}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(point, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, point, sr, null));
//			}
//			{
//				com.epl.geometry.Point point = (com.epl.geometry.Point)(com.epl.geometry.TestCommonMethods.FromJson("{\"x\":0,\"y\":10}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(point, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, point, sr, null));
//			}
//			{
//				com.epl.geometry.Point point = (com.epl.geometry.Point)(com.epl.geometry.TestCommonMethods.FromJson("{\"x\":0,\"y\":11}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(point, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(disjoint.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, point, sr, null));
//			}
//			{
//				com.epl.geometry.Point point = (com.epl.geometry.Point)(com.epl.geometry.TestCommonMethods.FromJson("{\"x\":0,\"y\":0}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(point, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, point, sr, null));
//			}
//			{
//				com.epl.geometry.Point point = (com.epl.geometry.Point)(com.epl.geometry.TestCommonMethods.FromJson("{\"x\":5,\"y\":0}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(point, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, point, sr, null));
//			}
//			{
//				com.epl.geometry.Point point = (com.epl.geometry.Point)(com.epl.geometry.TestCommonMethods.FromJson("{\"x\":11,\"y\":0}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(point, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(disjoint.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, point, sr, null));
//			}
//			{
//				com.epl.geometry.Point point = (com.epl.geometry.Point)(com.epl.geometry.TestCommonMethods.FromJson("{\"x\":0,\"y\":0}").GetGeometry());
//				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":0,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(point, 0.00000001, 1982);
//				WiggleGeometry(envelope, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(equals.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(point, envelope, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(envelope, point, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(envelope, point, sr, null));
//			}
//		}
//
//		[NUnit.Framework.Test]
//		public virtual void TestEnvelopeEnvelope()
//		{
//			com.epl.geometry.OperatorEquals equals = (com.epl.geometry.OperatorEquals)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Equals));
//			com.epl.geometry.OperatorContains contains = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
//			com.epl.geometry.OperatorDisjoint disjoint = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
//			com.epl.geometry.OperatorCrosses crosses = (com.epl.geometry.OperatorCrosses)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Crosses));
//			com.epl.geometry.OperatorWithin within = (com.epl.geometry.OperatorWithin)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Within));
//			com.epl.geometry.OperatorOverlaps overlaps = (com.epl.geometry.OperatorOverlaps)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Overlaps));
//			com.epl.geometry.OperatorTouches touches = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
//			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":5,\"ymin\":5,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":5,\"ymin\":5,\"xmax\":15,\"ymax\":15}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":10,\"ymin\":0,\"xmax\":20,\"ymax\":10}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":10,\"ymin\":0,\"xmax\":20,\"ymax\":10}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":10,\"ymin\":0,\"xmax\":20,\"ymax\":10}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":10,\"xmax\":10,\"ymax\":20}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":15,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":-5,\"ymin\":5,\"xmax\":0,\"ymax\":5}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":-5,\"ymin\":5,\"xmax\":5,\"ymax\":5}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":3,\"ymin\":5,\"xmax\":5,\"ymax\":5}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":3,\"ymin\":5,\"xmax\":10,\"ymax\":5}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":-5,\"ymin\":0,\"xmax\":0,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":15,\"ymax\":0}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":15,\"ymax\":0}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":-5,\"ymin\":0,\"xmax\":10,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":5,\"ymax\":0}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":-5,\"ymin\":0,\"xmax\":0,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":5,\"ymin\":-5,\"xmax\":5,\"ymax\":5}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":10,\"ymin\":0,\"xmax\":20,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":5,\"ymin\":0,\"xmax\":5,\"ymax\":5}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":0,\"ymax\":0}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":5,\"ymin\":0,\"xmax\":5,\"ymax\":0}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":5,\"ymin\":5,\"xmax\":5,\"ymax\":5}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":0,\"ymax\":0}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(touches.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":5,\"ymin\":0,\"xmax\":5,\"ymax\":0}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(!equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//			{
//				com.epl.geometry.Envelope env1 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":5,\"ymin\":0,\"xmax\":5,\"ymax\":0}").GetGeometry());
//				com.epl.geometry.Envelope env2 = (com.epl.geometry.Envelope)(com.epl.geometry.TestCommonMethods.FromJson("{\"xmin\":5,\"ymin\":0,\"xmax\":5,\"ymax\":0}").GetGeometry());
//				WiggleGeometry(env1, 0.00000001, 1982);
//				WiggleGeometry(env2, 0.00000001, 511);
//				NUnit.Framework.Assert.IsTrue(equals.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(contains.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!disjoint.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!touches.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!overlaps.Execute(env2, env1, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env1, env2, sr, null));
//				NUnit.Framework.Assert.IsTrue(!crosses.Execute(env2, env1, sr, null));
//			}
//		}

		internal static void WiggleGeometry(com.epl.geometry.Geometry geometry, double tolerance, int rand)
		{
			int type = geometry.GetType().Value();
			if (type == com.epl.geometry.Geometry.GeometryType.Polygon || type == com.epl.geometry.Geometry.GeometryType.Polyline || type == com.epl.geometry.Geometry.GeometryType.MultiPoint)
			{
				com.epl.geometry.MultiVertexGeometry mvGeom = (com.epl.geometry.MultiVertexGeometry)geometry;
				for (int i = 0; i < mvGeom.GetPointCount(); i++)
				{
					com.epl.geometry.Point2D pt = mvGeom.GetXY(i);
					// create random vector and normalize it to 0.49 * tolerance
					com.epl.geometry.Point2D randomV = new com.epl.geometry.Point2D();
					rand = com.epl.geometry.NumberUtils.NextRand(rand);
					randomV.x = 1.0 * rand / com.epl.geometry.NumberUtils.IntMax() - 0.5;
					rand = com.epl.geometry.NumberUtils.NextRand(rand);
					randomV.y = 1.0 * rand / com.epl.geometry.NumberUtils.IntMax() - 0.5;
					randomV.Normalize();
					randomV.Scale(0.45 * tolerance);
					pt.Add(randomV);
					mvGeom.SetXY(i, pt);
				}
			}
			else
			{
				if (type == com.epl.geometry.Geometry.GeometryType.Point)
				{
					com.epl.geometry.Point ptGeom = (com.epl.geometry.Point)(geometry);
					com.epl.geometry.Point2D pt = ptGeom.GetXY();
					// create random vector and normalize it to 0.49 * tolerance
					com.epl.geometry.Point2D randomV = new com.epl.geometry.Point2D();
					rand = com.epl.geometry.NumberUtils.NextRand(rand);
					randomV.x = 1.0 * rand / com.epl.geometry.NumberUtils.IntMax() - 0.5;
					rand = com.epl.geometry.NumberUtils.NextRand(rand);
					randomV.y = 1.0 * rand / com.epl.geometry.NumberUtils.IntMax() - 0.5;
					randomV.Normalize();
					randomV.Scale(0.45 * tolerance);
					pt.Add(randomV);
					ptGeom.SetXY(pt);
				}
				else
				{
					if (type == com.epl.geometry.Geometry.GeometryType.Envelope)
					{
						com.epl.geometry.Envelope envGeom = (com.epl.geometry.Envelope)(geometry);
						com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
						envGeom.QueryEnvelope2D(env);
						double xmin;
						double xmax;
						double ymin;
						double ymax;
						com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
						env.QueryLowerLeft(pt);
						// create random vector and normalize it to 0.49 * tolerance
						com.epl.geometry.Point2D randomV = new com.epl.geometry.Point2D();
						rand = com.epl.geometry.NumberUtils.NextRand(rand);
						randomV.x = 1.0 * rand / com.epl.geometry.NumberUtils.IntMax() - 0.5;
						rand = com.epl.geometry.NumberUtils.NextRand(rand);
						randomV.y = 1.0 * rand / com.epl.geometry.NumberUtils.IntMax() - 0.5;
						randomV.Normalize();
						randomV.Scale(0.45 * tolerance);
						xmin = (pt.x + randomV.x);
						ymin = (pt.y + randomV.y);
						env.QueryUpperRight(pt);
						// create random vector and normalize it to 0.49 * tolerance
						rand = com.epl.geometry.NumberUtils.NextRand(rand);
						randomV.x = 1.0 * rand / com.epl.geometry.NumberUtils.IntMax() - 0.5;
						rand = com.epl.geometry.NumberUtils.NextRand(rand);
						randomV.y = 1.0 * rand / com.epl.geometry.NumberUtils.IntMax() - 0.5;
						randomV.Normalize();
						randomV.Scale(0.45 * tolerance);
						xmax = (pt.x + randomV.x);
						ymax = (pt.y + randomV.y);
						if (xmin > xmax)
						{
							double swap = xmin;
							xmin = xmax;
							xmax = swap;
						}
						if (ymin > ymax)
						{
							double swap = ymin;
							ymin = ymax;
							ymax = swap;
						}
						envGeom.SetCoords(xmin, ymin, xmax, ymax);
					}
				}
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestDisjointRelationFalse()
		{
			{
				com.epl.geometry.OperatorDisjoint op = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
				com.epl.geometry.Envelope env1 = new com.epl.geometry.Envelope(50, 50, 150, 150);
				com.epl.geometry.Envelope env2 = new com.epl.geometry.Envelope(25, 25, 175, 175);
				bool result = op.Execute(env1, env2, com.epl.geometry.SpatialReference.Create(4326), null);
				NUnit.Framework.Assert.IsTrue(!result);
			}
			{
				com.epl.geometry.OperatorIntersects op = (com.epl.geometry.OperatorIntersects)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Intersects));
				com.epl.geometry.Envelope env1 = new com.epl.geometry.Envelope(50, 50, 150, 150);
				com.epl.geometry.Envelope env2 = new com.epl.geometry.Envelope(25, 25, 175, 175);
				bool result = op.Execute(env1, env2, com.epl.geometry.SpatialReference.Create(4326), null);
				NUnit.Framework.Assert.IsTrue(result);
			}
			{
				com.epl.geometry.OperatorContains op = (com.epl.geometry.OperatorContains)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains));
				com.epl.geometry.Envelope env1 = new com.epl.geometry.Envelope(100, 175, 200, 225);
				com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
				polyline.StartPath(200, 175);
				polyline.LineTo(200, 225);
				polyline.LineTo(125, 200);
				bool result = op.Execute(env1, polyline, com.epl.geometry.SpatialReference.Create(4326), null);
				NUnit.Framework.Assert.IsTrue(result);
			}
			{
				com.epl.geometry.OperatorTouches op = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
				com.epl.geometry.Envelope env1 = new com.epl.geometry.Envelope(100, 200, 400, 400);
				com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
				polyline.StartPath(300, 60);
				polyline.LineTo(300, 200);
				polyline.LineTo(400, 50);
				bool result = op.Execute(env1, polyline, com.epl.geometry.SpatialReference.Create(4326), null);
				NUnit.Framework.Assert.IsTrue(result);
			}
			{
				com.epl.geometry.OperatorTouches op = (com.epl.geometry.OperatorTouches)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Touches));
				com.epl.geometry.Envelope env1 = new com.epl.geometry.Envelope(50, 50, 150, 150);
				com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
				polyline.StartPath(100, 20);
				polyline.LineTo(100, 50);
				polyline.LineTo(150, 10);
				bool result = op.Execute(polyline, env1, com.epl.geometry.SpatialReference.Create(4326), null);
				NUnit.Framework.Assert.IsTrue(result);
			}
			{
				com.epl.geometry.OperatorDisjoint op = (com.epl.geometry.OperatorDisjoint)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Disjoint));
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
				polygon.StartPath(0, 0);
				polygon.LineTo(0, 10);
				polygon.LineTo(10, 10);
				polygon.LineTo(10, 0);
				polyline.StartPath(-5, 4);
				polyline.LineTo(5, -6);
				bool result = op.Execute(polyline, polygon, com.epl.geometry.SpatialReference.Create(4326), null);
				NUnit.Framework.Assert.IsTrue(result);
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestPolylinePolylineRelate()
		{
			com.epl.geometry.OperatorRelate op = com.epl.geometry.OperatorRelate.Local();
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			bool res;
			string scl;
			com.epl.geometry.Polyline polyline1 = new com.epl.geometry.Polyline();
			com.epl.geometry.Polyline polyline2 = new com.epl.geometry.Polyline();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(1, 1);
			polyline2.StartPath(1, 1);
			polyline2.LineTo(2, 0);
			scl = "FF1FT01T2";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "****TF*T*";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "****F****";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "**1*0*T**";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "****1****";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "**T*001*T";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "T********";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "F********";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.SetEmpty();
			polyline2.SetEmpty();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(1, 0);
			polyline2.StartPath(0, 0);
			polyline2.LineTo(1, 0);
			scl = "1FFFTFFFT";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "1*T*T****";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "1T**T****";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.SetEmpty();
			polyline2.SetEmpty();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(0.5, 0.5);
			polyline1.LineTo(1, 1);
			polyline2.StartPath(1, 0);
			polyline2.LineTo(0.5, 0.5);
			polyline2.LineTo(0, 1);
			scl = "0F1FFTT0T";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "*T*******";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "*F*F*****";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.SetEmpty();
			polyline2.SetEmpty();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(1, 0);
			polyline2.StartPath(1, -1);
			polyline2.LineTo(1, 1);
			scl = "FT1TF01TT";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "***T*****";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.SetEmpty();
			polyline2.SetEmpty();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(0, 20);
			polyline1.LineTo(20, 20);
			polyline1.LineTo(20, 0);
			polyline1.LineTo(0, 0);
			// has no boundary
			polyline2.StartPath(3, 3);
			polyline2.LineTo(5, 5);
			op.AccelerateGeometry(polyline1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
			scl = "FF1FFF102";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.SetEmpty();
			polyline2.SetEmpty();
			polyline1.StartPath(4, 0);
			polyline1.LineTo(0, 4);
			polyline1.LineTo(4, 8);
			polyline1.LineTo(8, 4);
			polyline2.StartPath(8, 1);
			polyline2.LineTo(8, 2);
			op.AccelerateGeometry(polyline1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
			scl = "FF1FF0102";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.SetEmpty();
			polyline2.SetEmpty();
			polyline1.StartPath(4, 0);
			polyline1.LineTo(0, 4);
			polyline2.StartPath(3, 2);
			polyline2.LineTo(3, 2);
			NUnit.Framework.Assert.IsTrue(polyline2.GetBoundary().IsEmpty());
			scl = "******0F*";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline2.LineTo(3, 2);
			NUnit.Framework.Assert.IsTrue(polyline2.GetBoundary().IsEmpty());
			scl = "******0F*";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "******0F*";
			polyline2.LineTo(3, 2);
			NUnit.Framework.Assert.IsTrue(polyline2.GetBoundary().IsEmpty());
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.SetEmpty();
			polyline2.SetEmpty();
			polyline1.StartPath(3, 3);
			polyline1.LineTo(3, 4);
			polyline1.LineTo(3, 3);
			polyline2.StartPath(1, 1);
			polyline2.LineTo(1, 1);
			scl = "FF1FFF0F2";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "FF0FFF1F2";
			res = op.Execute(polyline2, polyline1, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.SetEmpty();
			polyline2.SetEmpty();
			polyline1.StartPath(4, 0);
			polyline1.LineTo(0, 4);
			polyline2.StartPath(2, 2);
			polyline2.LineTo(2, 2);
			scl = "0F*******";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline2.LineTo(2, 2);
			scl = "0F*******";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "0F*******";
			res = op.Execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygonPolylineRelate()
		{
			com.epl.geometry.OperatorRelate op = com.epl.geometry.OperatorRelate.Local();
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			bool res;
			string scl;
			com.epl.geometry.Polygon polygon1 = new com.epl.geometry.Polygon();
			com.epl.geometry.Polyline polyline2 = new com.epl.geometry.Polyline();
			polygon1.StartPath(0, 0);
			polygon1.LineTo(0, 10);
			polygon1.LineTo(10, 10);
			polygon1.LineTo(10, 0);
			polyline2.StartPath(-10, 0);
			polyline2.LineTo(0, 0);
			scl = "FF2F01102";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "**1*0110*";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "T***T****";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "FF2FT****";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline2.SetEmpty();
			polyline2.StartPath(0, 0);
			polyline2.LineTo(10, 0);
			scl = "**21*1FF*";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "F*21*1FF*";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "0**1*1FF*";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "F**1*1TF*";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline2.SetEmpty();
			polyline2.StartPath(1, 1);
			polyline2.LineTo(5, 5);
			scl = "TT2******";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "1T2FF1FF*";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "1T1FF1FF*";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline2.SetEmpty();
			polyline2.StartPath(5, 5);
			polyline2.LineTo(15, 5);
			scl = "1T20F*T0T";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon1.SetEmpty();
			polyline2.SetEmpty();
			polygon1.StartPath(2, 0);
			polygon1.LineTo(0, 2);
			polygon1.LineTo(2, 4);
			polygon1.LineTo(4, 2);
			polyline2.StartPath(1, 2);
			polyline2.LineTo(3, 2);
			op.AccelerateGeometry(polygon1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
			scl = "TTTFF****";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline2.SetEmpty();
			polyline2.StartPath(5, 2);
			polyline2.LineTo(7, 2);
			scl = "FF2FFT***";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon1.SetEmpty();
			polyline2.SetEmpty();
			polygon1.StartPath(0, 0);
			polygon1.LineTo(0, 1);
			polygon1.LineTo(1, 0);
			polyline2.StartPath(0, 10);
			polyline2.LineTo(0, 9);
			polyline2.StartPath(10, 0);
			polyline2.LineTo(9, 0);
			polyline2.StartPath(0, -10);
			polyline2.LineTo(0, -9);
			scl = "**2******";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon1.SetEmpty();
			polyline2.SetEmpty();
			polygon1.StartPath(0, 0);
			polygon1.LineTo(0, 1);
			polygon1.LineTo(0, 0);
			polyline2.StartPath(0, 10);
			polyline2.LineTo(0, 9);
			scl = "**1******";
			res = op.Execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygonPolygonRelate()
		{
			com.epl.geometry.OperatorRelate op = (com.epl.geometry.OperatorRelate)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Relate));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			bool res;
			string scl;
			com.epl.geometry.Polygon polygon1 = new com.epl.geometry.Polygon();
			com.epl.geometry.Polygon polygon2 = new com.epl.geometry.Polygon();
			polygon1.StartPath(0, 0);
			polygon1.LineTo(0, 10);
			polygon1.LineTo(10, 10);
			polygon1.LineTo(10, 0);
			polygon2.StartPath(15, 0);
			polygon2.LineTo(15, 10);
			polygon2.LineTo(25, 10);
			polygon2.LineTo(25, 0);
			scl = "FFTFFT21T";
			res = op.Execute(polygon1, polygon2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "FFTFFT11T";
			res = op.Execute(polygon1, polygon2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polygon2.SetEmpty();
			polygon2.StartPath(5, 0);
			polygon2.LineTo(5, 10);
			polygon2.LineTo(15, 10);
			polygon2.LineTo(15, 0);
			scl = "21TT1121T";
			res = op.Execute(polygon1, polygon2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon2.SetEmpty();
			polygon2.StartPath(1, 1);
			polygon2.LineTo(1, 9);
			polygon2.LineTo(9, 9);
			polygon2.LineTo(9, 1);
			scl = "212FF1FFT";
			res = op.Execute(polygon1, polygon2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon1.SetEmpty();
			polygon2.SetEmpty();
			polygon1.StartPath(3, 3);
			polygon1.LineTo(3, 4);
			polygon1.LineTo(3, 3);
			polygon2.StartPath(1, 1);
			polygon2.LineTo(1, 1);
			scl = "FF1FFF0F2";
			res = op.Execute(polygon1, polygon2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "FF0FFF1F2";
			res = op.Execute(polygon2, polygon1, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon1.SetEmpty();
			polygon2.SetEmpty();
			polygon1.StartPath(0, 0);
			polygon1.LineTo(0, 100);
			polygon1.LineTo(100, 100);
			polygon1.LineTo(100, 0);
			polygon2.StartPath(50, 50);
			polygon2.LineTo(50, 50);
			polygon2.LineTo(50, 50);
			op.AccelerateGeometry(polygon1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
			scl = "0F2FF1FF2";
			res = op.Execute(polygon1, polygon2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon2.LineTo(51, 50);
			scl = "1F2FF1FF2";
			res = op.Execute(polygon1, polygon2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPointPointRelate()
		{
			com.epl.geometry.OperatorRelate op = (com.epl.geometry.OperatorRelate)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Relate));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			bool res;
			string scl;
			com.epl.geometry.MultiPoint m1 = new com.epl.geometry.MultiPoint();
			com.epl.geometry.Point p2 = new com.epl.geometry.Point();
			m1.Add(0, 0);
			p2.SetXY(0, 0);
			scl = "T*F***F**";
			res = op.Execute(m1, p2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "T*T***F**";
			res = op.Execute(m1, p2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			m1.Add(1, 1);
			res = op.Execute(m1, p2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			m1.SetEmpty();
			m1.Add(1, 1);
			m1.Add(2, 2);
			scl = "FF0FFFTF2";
			res = op.Execute(m1, p2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPointPointRelate()
		{
			com.epl.geometry.OperatorRelate op = (com.epl.geometry.OperatorRelate)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Relate));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			bool res;
			string scl;
			com.epl.geometry.Point p1 = new com.epl.geometry.Point();
			com.epl.geometry.Point p2 = new com.epl.geometry.Point();
			p1.SetXY(0, 0);
			p2.SetXY(0, 0);
			scl = "T********";
			res = op.Execute(p1, p2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			p1.SetXY(0, 0);
			p2.SetXY(1, 0);
			res = op.Execute(p1, p2, null, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			p1.SetEmpty();
			p2.SetEmpty();
			scl = "*********";
			res = op.Execute(p1, p2, null, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "FFFFFFFFF";
			res = op.Execute(p1, p2, null, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "FFFFFFFFT";
			res = op.Execute(p1, p2, null, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygonMultiPointRelate()
		{
			com.epl.geometry.OperatorRelate op = (com.epl.geometry.OperatorRelate)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Relate));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			bool res;
			string scl;
			com.epl.geometry.Polygon polygon1 = new com.epl.geometry.Polygon();
			com.epl.geometry.MultiPoint multipoint2 = new com.epl.geometry.MultiPoint();
			polygon1.StartPath(0, 0);
			polygon1.LineTo(0, 10);
			polygon1.LineTo(10, 10);
			polygon1.LineTo(10, 0);
			multipoint2.Add(0, 0);
			multipoint2.Add(5, 5);
			scl = "TFT0F1FFT";
			res = op.Execute(polygon1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "T0FFFFT1T";
			// transpose of above
			res = op.Execute(multipoint2, polygon1, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.Add(11, 11);
			scl = "TFT0F10FT";
			res = op.Execute(polygon1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.Add(0, 5);
			scl = "TFT0F10FT";
			res = op.Execute(polygon1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "TFF0F10FT";
			res = op.Execute(polygon1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polygon1.SetEmpty();
			multipoint2.SetEmpty();
			polygon1.StartPath(0, 0);
			polygon1.LineTo(0, 20);
			polygon1.LineTo(20, 20);
			polygon1.LineTo(20, 0);
			multipoint2.Add(3, 3);
			multipoint2.Add(5, 5);
			op.AccelerateGeometry(polygon1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
			scl = "TF2FF****";
			res = op.Execute(polygon1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon1.SetEmpty();
			multipoint2.SetEmpty();
			polygon1.StartPath(4, 0);
			polygon1.LineTo(0, 4);
			polygon1.LineTo(4, 8);
			polygon1.LineTo(8, 4);
			multipoint2.Add(8, 1);
			multipoint2.Add(8, 2);
			op.AccelerateGeometry(polygon1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
			scl = "FF2FF10F2";
			res = op.Execute(polygon1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygonPointRelate()
		{
			com.epl.geometry.OperatorRelate op = (com.epl.geometry.OperatorRelate)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Relate));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			bool res;
			string scl;
			com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			polygon.StartPath(0, 0);
			polygon.LineTo(0, 10);
			polygon.LineTo(10, 10);
			polygon.LineTo(10, 0);
			point.SetXY(0, 0);
			scl = "FF20FTFFT";
			res = op.Execute(polygon, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon.SetEmpty();
			polygon.StartPath(0, 0);
			polygon.LineTo(0, 0);
			polygon.LineTo(0, 0);
			scl = "0FFFFFFF2";
			res = op.Execute(polygon, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon.SetEmpty();
			polygon.StartPath(0, 0);
			polygon.LineTo(0, 1);
			polygon.LineTo(0, 0);
			scl = "0F1FFFFF2";
			res = op.Execute(polygon, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			point.SetXY(-1, 0);
			scl = "FF1FFF0F2";
			res = op.Execute(polygon, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon.SetEmpty();
			polygon.StartPath(0, 0);
			polygon.LineTo(0, 10);
			polygon.LineTo(0, 0);
			scl = "FF1FFFTFT";
			res = op.Execute(polygon, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon.SetEmpty();
			polygon.StartPath(0, 0);
			polygon.LineTo(0, 0);
			polygon.LineTo(0, 0);
			scl = "FF0FFF0F2";
			res = op.Execute(polygon, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolylineMultiPointRelate()
		{
			com.epl.geometry.OperatorRelate op = com.epl.geometry.OperatorRelate.Local();
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			bool res;
			string scl;
			com.epl.geometry.Polyline polyline1 = new com.epl.geometry.Polyline();
			com.epl.geometry.MultiPoint multipoint2 = new com.epl.geometry.MultiPoint();
			polyline1.StartPath(0, 0);
			polyline1.LineTo(10, 0);
			multipoint2.Add(0, 0);
			multipoint2.Add(5, 5);
			scl = "FF10F00F2";
			res = op.Execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.Add(5, 0);
			scl = "0F10F00F2";
			res = op.Execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "0F11F00F2";
			res = op.Execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.SetEmpty();
			multipoint2.SetEmpty();
			polyline1.StartPath(4, 0);
			polyline1.LineTo(0, 4);
			polyline1.LineTo(4, 8);
			polyline1.LineTo(8, 4);
			polyline1.LineTo(4, 0);
			// has no boundary
			multipoint2.Add(8, 1);
			multipoint2.Add(8, 2);
			op.AccelerateGeometry(polyline1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
			scl = "FF1FFF0F2";
			res = op.Execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.SetEmpty();
			multipoint2.SetEmpty();
			polyline1.StartPath(4, 0);
			polyline1.LineTo(4, 0);
			multipoint2.Add(8, 1);
			multipoint2.Add(8, 2);
			scl = "FF0FFF0F2";
			res = op.Execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.Add(-2, 0);
			res = op.Execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			op.AccelerateGeometry(polyline1, sr, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
			res = op.Execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.SetEmpty();
			multipoint2.SetEmpty();
			polyline1.StartPath(10, 10);
			polyline1.LineTo(10, 10);
			multipoint2.Add(10, 10);
			scl = "0FFFFFFF2";
			res = op.Execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.StartPath(12, 12);
			polyline1.LineTo(12, 12);
			scl = "0F0FFFFF2";
			res = op.Execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.SetEmpty();
			multipoint2.SetEmpty();
			polyline1.StartPath(10, 10);
			polyline1.LineTo(10, 10);
			multipoint2.Add(0, 0);
			scl = "FF0FFF0F2";
			res = op.Execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPointMultipointRelate()
		{
			com.epl.geometry.OperatorRelate op = (com.epl.geometry.OperatorRelate)(com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Relate));
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			bool res;
			string scl;
			com.epl.geometry.MultiPoint multipoint1 = new com.epl.geometry.MultiPoint();
			com.epl.geometry.MultiPoint multipoint2 = new com.epl.geometry.MultiPoint();
			multipoint1.Add(0, 0);
			multipoint2.Add(0, 0);
			scl = "TFFFFFFF2";
			res = op.Execute(multipoint1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.Add(5, 5);
			scl = "TFFFFFTF2";
			res = op.Execute(multipoint1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint1.Add(-5, 0);
			scl = "0FTFFFTF2";
			res = op.Execute(multipoint1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = com.epl.geometry.GeometryEngine.Relate(multipoint1, multipoint2, sr, scl);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint1.SetEmpty();
			multipoint2.SetEmpty();
			multipoint1.Add(0, 0);
			multipoint2.Add(1, 1);
			scl = "FFTFFF0FT";
			res = op.Execute(multipoint1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolylinePointRelate()
		{
			com.epl.geometry.OperatorRelate op = com.epl.geometry.OperatorRelate.Local();
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			bool res;
			string scl;
			com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			polyline.StartPath(0, 2);
			polyline.LineTo(0, 4);
			point.SetXY(0, 3);
			scl = "0F1FF0FF2";
			res = op.Execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			point.SetXY(1, 3);
			scl = "FF1FF00F2";
			res = op.Execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline.LineTo(4, 4);
			polyline.LineTo(4, 2);
			polyline.LineTo(0, 2);
			// no bounadry
			point.SetXY(0, 3);
			scl = "0F1FFFFF2";
			res = op.Execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "0F1FFFFF2";
			res = op.Execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			point.SetXY(1, 3);
			scl = "FF1FFF0F2";
			res = op.Execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			point.SetXY(10, 10);
			scl = "FF1FFF0F2";
			res = op.Execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline.SetEmpty();
			point.SetEmpty();
			polyline.StartPath(10, 10);
			polyline.LineTo(10, 10);
			point.SetXY(10, 10);
			scl = "0FFFFFFF2";
			res = op.Execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline.StartPath(12, 12);
			polyline.LineTo(12, 12);
			scl = "0F0FFFFF2";
			res = op.Execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline.SetEmpty();
			point.SetEmpty();
			polyline.StartPath(10, 10);
			polyline.LineTo(10, 10);
			point.SetXY(0, 0);
			scl = "FF0FFF0F2";
			res = op.Execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void TestCrosses_github_issue_40()
		{
			// Issue 40: Acceleration without a spatial reference changes the result
			// of relation operators
			com.epl.geometry.Geometry geom1 = com.epl.geometry.OperatorImportFromWkt.Local().Execute(0, com.epl.geometry.Geometry.Type.Unknown, "LINESTRING (2 0, 2 3)", null);
			com.epl.geometry.Geometry geom2 = com.epl.geometry.OperatorImportFromWkt.Local().Execute(0, com.epl.geometry.Geometry.Type.Unknown, "POLYGON ((1 1, 4 1, 4 4, 1 4, 1 1))", null);
			bool answer1 = com.epl.geometry.OperatorCrosses.Local().Execute(geom1, geom2, null, null);
			NUnit.Framework.Assert.IsTrue(answer1);
			com.epl.geometry.OperatorCrosses.Local().AccelerateGeometry(geom1, null, com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
			bool answer2 = com.epl.geometry.OperatorCrosses.Local().Execute(geom1, geom2, null, null);
			NUnit.Framework.Assert.IsTrue(answer2);
		}

		[NUnit.Framework.Test]
		public virtual void TestDisjointCrash()
		{
			com.epl.geometry.Polygon g1 = new com.epl.geometry.Polygon();
			g1.AddEnvelope(com.epl.geometry.Envelope2D.Construct(0, 0, 10, 10), false);
			com.epl.geometry.Polygon g2 = new com.epl.geometry.Polygon();
			g2.AddEnvelope(com.epl.geometry.Envelope2D.Construct(10, 1, 21, 21), false);
			g1 = (com.epl.geometry.Polygon)com.epl.geometry.OperatorDensifyByLength.Local().Execute(g1, 0.1, null);
			com.epl.geometry.OperatorDisjoint.Local().AccelerateGeometry(g1, com.epl.geometry.SpatialReference.Create(4267), com.epl.geometry.Geometry.GeometryAccelerationDegree.enumHot);
			bool res = com.epl.geometry.OperatorDisjoint.Local().Execute(g1, g2, com.epl.geometry.SpatialReference.Create(4267), null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

//		[NUnit.Framework.Test]
//		public virtual void TestDisjointFail()
//		{
//			com.epl.geometry.MapGeometry geometry1 = com.epl.geometry.OperatorImportFromJson.Local().Execute(com.epl.geometry.Geometry.Type.Unknown, "{\"paths\":[[[3,3],[3,3]]],\"spatialReference\":{\"wkid\":4326}}");
//			com.epl.geometry.MapGeometry geometry2 = com.epl.geometry.OperatorImportFromJson.Local().Execute(com.epl.geometry.Geometry.Type.Unknown, "{\"rings\":[[[2,2],[2,4],[4,4],[4,2],[2,2]]],\"spatialReference\":{\"wkid\":4326}}");
//			com.epl.geometry.OperatorDisjoint.Local().AccelerateGeometry(geometry1.GetGeometry(), geometry1.GetSpatialReference(), com.epl.geometry.Geometry.GeometryAccelerationDegree.enumMedium);
//			bool res = com.epl.geometry.OperatorDisjoint.Local().Execute(geometry1.GetGeometry(), geometry2.GetGeometry(), geometry1.GetSpatialReference(), null);
//			NUnit.Framework.Assert.IsTrue(!res);
//		}
	}
}
