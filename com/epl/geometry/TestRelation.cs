

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestRelation
	{
		/// <exception cref="System.Exception"/>
		protected override void setUp()
		{
			base.setUp();
		}

		/// <exception cref="System.Exception"/>
		protected override void tearDown()
		{
			base.tearDown();
		}

		[NUnit.Framework.Test]
		public virtual void testCreation()
		{
			{
				com.esri.core.geometry.OperatorFactoryLocal projEnv = com.esri.core.geometry.OperatorFactoryLocal
					.getInstance();
				com.esri.core.geometry.SpatialReference inputSR = com.esri.core.geometry.SpatialReference
					.create(3857);
				com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
				com.esri.core.geometry.Envelope2D env1 = new com.esri.core.geometry.Envelope2D();
				env1.setCoords(855277, 3892059, 855277 + 100, 3892059 + 100);
				poly1.addEnvelope(env1, false);
				com.esri.core.geometry.Polygon poly2 = new com.esri.core.geometry.Polygon();
				com.esri.core.geometry.Envelope2D env2 = new com.esri.core.geometry.Envelope2D();
				env2.setCoords(855277, 3892059, 855277 + 300, 3892059 + 200);
				poly2.addEnvelope(env2, false);
				{
					com.esri.core.geometry.OperatorEquals operatorEquals = (com.esri.core.geometry.OperatorEquals
						)(projEnv.getOperator(com.esri.core.geometry.Operator.Type.Equals));
					bool result = operatorEquals.execute(poly1, poly2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					com.esri.core.geometry.Polygon poly11 = new com.esri.core.geometry.Polygon();
					poly1.copyTo(poly11);
					result = operatorEquals.execute(poly1, poly11, inputSR, null);
					NUnit.Framework.Assert.IsTrue(result);
				}
				{
					com.esri.core.geometry.OperatorCrosses operatorCrosses = (com.esri.core.geometry.OperatorCrosses
						)(projEnv.getOperator(com.esri.core.geometry.Operator.Type.Crosses));
					bool result = operatorCrosses.execute(poly1, poly2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
				}
				{
					com.esri.core.geometry.OperatorWithin operatorWithin = (com.esri.core.geometry.OperatorWithin
						)(projEnv.getOperator(com.esri.core.geometry.Operator.Type.Within));
					bool result = operatorWithin.execute(poly1, poly2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(result);
				}
				{
					com.esri.core.geometry.OperatorDisjoint operatorDisjoint = (com.esri.core.geometry.OperatorDisjoint
						)(projEnv.getOperator(com.esri.core.geometry.Operator.Type.Disjoint));
					com.esri.core.geometry.OperatorIntersects operatorIntersects = (com.esri.core.geometry.OperatorIntersects
						)(projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersects));
					bool result = operatorDisjoint.execute(poly1, poly2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					{
						result = operatorIntersects.execute(poly1, poly2, inputSR, null);
						NUnit.Framework.Assert.IsTrue(result);
					}
				}
				{
					com.esri.core.geometry.OperatorDisjoint operatorDisjoint = (com.esri.core.geometry.OperatorDisjoint
						)(projEnv.getOperator(com.esri.core.geometry.Operator.Type.Disjoint));
					com.esri.core.geometry.OperatorIntersects operatorIntersects = (com.esri.core.geometry.OperatorIntersects
						)(projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersects));
					com.esri.core.geometry.Envelope2D env2D = new com.esri.core.geometry.Envelope2D();
					poly2.queryEnvelope2D(env2D);
					com.esri.core.geometry.Envelope envelope = new com.esri.core.geometry.Envelope(env2D
						);
					bool result = operatorDisjoint.execute(envelope, poly2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					{
						result = operatorIntersects.execute(envelope, poly2, inputSR, null);
						NUnit.Framework.Assert.IsTrue(result);
					}
				}
				{
					com.esri.core.geometry.OperatorDisjoint operatorDisjoint = (com.esri.core.geometry.OperatorDisjoint
						)(projEnv.getOperator(com.esri.core.geometry.Operator.Type.Disjoint));
					com.esri.core.geometry.OperatorIntersects operatorIntersects = (com.esri.core.geometry.OperatorIntersects
						)(projEnv.getOperator(com.esri.core.geometry.Operator.Type.Intersects));
					com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
					com.esri.core.geometry.Envelope2D env2D = new com.esri.core.geometry.Envelope2D();
					env2D.setCoords(855277, 3892059, 855277 + 100, 3892059 + 100);
					poly.addEnvelope(env2D, false);
					env2D.setCoords(855277 + 10, 3892059 + 10, 855277 + 90, 3892059 + 90);
					poly.addEnvelope(env2D, true);
					env2D.setCoords(855277 + 20, 3892059 + 20, 855277 + 200, 3892059 + 80);
					com.esri.core.geometry.Envelope envelope = new com.esri.core.geometry.Envelope(env2D
						);
					bool result = operatorDisjoint.execute(envelope, poly, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					{
						result = operatorIntersects.execute(envelope, poly, inputSR, null);
						NUnit.Framework.Assert.IsTrue(result);
					}
				}
				{
					com.esri.core.geometry.OperatorTouches operatorTouches = (com.esri.core.geometry.OperatorTouches
						)(projEnv.getOperator(com.esri.core.geometry.Operator.Type.Touches));
					bool result = operatorTouches.execute(poly1, poly2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
				}
			}
		}

		[NUnit.Framework.Test]
		public virtual void testOperatorDisjoint()
		{
			{
				com.esri.core.geometry.OperatorFactoryLocal projEnv = com.esri.core.geometry.OperatorFactoryLocal
					.getInstance();
				com.esri.core.geometry.SpatialReference inputSR = com.esri.core.geometry.SpatialReference
					.create(3857);
				com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
				com.esri.core.geometry.Envelope2D env1 = new com.esri.core.geometry.Envelope2D();
				env1.setCoords(855277, 3892059, 855277 + 100, 3892059 + 100);
				poly1.addEnvelope(env1, false);
				com.esri.core.geometry.Polygon poly2 = new com.esri.core.geometry.Polygon();
				com.esri.core.geometry.Envelope2D env2 = new com.esri.core.geometry.Envelope2D();
				env2.setCoords(855277, 3892059, 855277 + 300, 3892059 + 200);
				poly2.addEnvelope(env2, false);
				com.esri.core.geometry.Polygon poly3 = new com.esri.core.geometry.Polygon();
				com.esri.core.geometry.Envelope2D env3 = new com.esri.core.geometry.Envelope2D();
				env3.setCoords(855277 + 100, 3892059 + 100, 855277 + 100 + 100, 3892059 + 100 + 100
					);
				poly3.addEnvelope(env3, false);
				com.esri.core.geometry.Polygon poly4 = new com.esri.core.geometry.Polygon();
				com.esri.core.geometry.Envelope2D env4 = new com.esri.core.geometry.Envelope2D();
				env4.setCoords(855277 + 200, 3892059 + 200, 855277 + 200 + 100, 3892059 + 200 + 100
					);
				poly4.addEnvelope(env4, false);
				com.esri.core.geometry.Point point1 = new com.esri.core.geometry.Point(855277, 3892059
					);
				com.esri.core.geometry.Point point2 = new com.esri.core.geometry.Point(855277 + 2
					, 3892059 + 3);
				com.esri.core.geometry.Point point3 = new com.esri.core.geometry.Point(855277 - 2
					, 3892059 - 3);
				{
					com.esri.core.geometry.OperatorDisjoint operatorDisjoint = (com.esri.core.geometry.OperatorDisjoint
						)(projEnv.getOperator(com.esri.core.geometry.Operator.Type.Disjoint));
					bool result = operatorDisjoint.execute(poly1, poly2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					result = operatorDisjoint.execute(poly1, poly3, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					result = operatorDisjoint.execute(poly1, poly4, inputSR, null);
					NUnit.Framework.Assert.IsTrue(result);
					result = operatorDisjoint.execute(poly1, point1, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					result = operatorDisjoint.execute(point1, poly1, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					result = operatorDisjoint.execute(poly1, point2, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					result = operatorDisjoint.execute(point2, poly1, inputSR, null);
					NUnit.Framework.Assert.IsTrue(!result);
					result = operatorDisjoint.execute(poly1, point3, inputSR, null);
					NUnit.Framework.Assert.IsTrue(result);
					result = operatorDisjoint.execute(point3, poly1, inputSR, null);
					NUnit.Framework.Assert.IsTrue(result);
				}
			}
		}

		[NUnit.Framework.Test]
		public virtual void testTouchPointLineCR183227()
		{
			// Tests CR 183227
			com.esri.core.geometry.OperatorTouches operatorTouches = (com.esri.core.geometry.OperatorTouches
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Touches));
			com.esri.core.geometry.Geometry baseGeom = new com.esri.core.geometry.Point(-130, 
				10);
			com.esri.core.geometry.Polyline pl = new com.esri.core.geometry.Polyline();
			// pl.startPath(std::make_shared<Point>(-130, 10));
			pl.startPath(-130, 10);
			pl.lineTo(-131, 15);
			pl.lineTo(-140, 20);
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool isTouched;
			bool isTouched2;
			isTouched = operatorTouches.execute(baseGeom, pl, sr, null);
			isTouched2 = operatorTouches.execute(pl, baseGeom, sr, null);
			NUnit.Framework.Assert.IsTrue(isTouched && isTouched2);
			{
				baseGeom = new com.esri.core.geometry.Point(-131, 15);
				isTouched = operatorTouches.execute(baseGeom, pl, sr, null);
				isTouched2 = operatorTouches.execute(pl, baseGeom, sr, null);
				NUnit.Framework.Assert.IsTrue(!isTouched && !isTouched2);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testTouchPointLineClosed()
		{
			// Tests CR 183227
			com.esri.core.geometry.OperatorTouches operatorTouches = (com.esri.core.geometry.OperatorTouches
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Touches));
			com.esri.core.geometry.Geometry baseGeom = new com.esri.core.geometry.Point(-130, 
				10);
			com.esri.core.geometry.Polyline pl = new com.esri.core.geometry.Polyline();
			pl.startPath(-130, 10);
			pl.lineTo(-131, 15);
			pl.lineTo(-140, 20);
			pl.lineTo(-130, 10);
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool isTouched;
			bool isTouched2;
			isTouched = operatorTouches.execute(baseGeom, pl, sr, null);
			isTouched2 = operatorTouches.execute(pl, baseGeom, sr, null);
			NUnit.Framework.Assert.IsTrue(!isTouched && !isTouched2);
			{
				// this may change in future
				baseGeom = new com.esri.core.geometry.Point(-131, 15);
				isTouched = operatorTouches.execute(baseGeom, pl, sr, null);
				isTouched2 = operatorTouches.execute(pl, baseGeom, sr, null);
				NUnit.Framework.Assert.IsTrue(!isTouched && !isTouched2);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testTouchPolygonPolygon()
		{
			com.esri.core.geometry.OperatorTouches operatorTouches = (com.esri.core.geometry.OperatorTouches
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Touches));
			com.esri.core.geometry.Polygon pg = new com.esri.core.geometry.Polygon();
			pg.startPath(-130, 10);
			pg.lineTo(-131, 15);
			pg.lineTo(-140, 20);
			com.esri.core.geometry.Polygon pg2 = new com.esri.core.geometry.Polygon();
			pg2.startPath(-130, 10);
			pg2.lineTo(-131, 15);
			pg2.lineTo(-120, 20);
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool isTouched;
			bool isTouched2;
			isTouched = operatorTouches.execute(pg, pg2, sr, null);
			isTouched2 = operatorTouches.execute(pg2, pg, sr, null);
			NUnit.Framework.Assert.IsTrue(isTouched && isTouched2);
		}

		[NUnit.Framework.Test]
		public virtual void testContainsFailureCR186456()
		{
			{
				com.esri.core.geometry.OperatorContains op = (com.esri.core.geometry.OperatorContains
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Contains));
				string str = "{\"rings\":[[[406944.399999999,287461.450000001],[406947.750000011,287462.299999997],[406946.44999999,287467.450000001],[406943.050000005,287466.550000005],[406927.799999992,287456.849999994],[406926.949999996,287456.599999995],[406924.800000005,287455.999999998],[406924.300000007,287455.849999999],[406924.200000008,287456.099999997],[406923.450000011,287458.449999987],[406922.999999987,287459.800000008],[406922.29999999,287462.099999998],[406921.949999991,287463.449999992],[406921.449999993,287465.050000011],[406920.749999996,287466.700000004],[406919.800000001,287468.599999996],[406919.050000004,287469.99999999],[406917.800000009,287471.800000008],[406916.04999999,287473.550000001],[406915.449999993,287473.999999999],[406913.700000001,287475.449999993],[406913.300000002,287475.899999991],[406912.050000008,287477.250000011],[406913.450000002,287478.150000007],[406915.199999994,287478.650000005],[406915.999999991,287478.800000005],[406918.300000007,287479.200000003],[406920.649999997,287479.450000002],[406923.100000013,287479.550000001],[406925.750000001,287479.450000002],[406928.39999999,287479.150000003],[406929.80000001,287478.950000004],[406932.449999998,287478.350000006],[406935.099999987,287477.60000001],[406938.699999998,287476.349999989],[406939.649999994,287473.949999999],[406939.799999993,287473.949999999],[406941.249999987,287473.75],[406942.700000007,287473.250000002],[406943.100000005,287473.100000003],[406943.950000001,287472.750000004],[406944.799999998,287472.300000006],[406944.999999997,287472.200000007],[406946.099999992,287471.200000011],[406946.299999991,287470.950000012],[406948.00000001,287468.599999996],[406948.10000001,287468.399999997],[406950.100000001,287465.050000011],[406951.949999993,287461.450000001],[406952.049999993,287461.300000001],[406952.69999999,287459.900000007],[406953.249999987,287458.549999987],[406953.349999987,287458.299999988],[406953.650000012,287457.299999992],[406953.900000011,287456.349999996],[406954.00000001,287455.300000001],[406954.00000001,287454.750000003],[406953.850000011,287453.750000008],[406953.550000012,287452.900000011],[406953.299999987,287452.299999988],[406954.500000008,287450.299999996],[406954.00000001,287449.000000002],[406953.399999987,287447.950000006],[406953.199999988,287447.550000008],[406952.69999999,287446.850000011],[406952.149999992,287446.099999988],[406951.499999995,287445.499999991],[406951.149999996,287445.249999992],[406950.449999999,287444.849999994],[406949.600000003,287444.599999995],[406949.350000004,287444.549999995],[406948.250000009,287444.499999995],[406947.149999987,287444.699999994],[406946.849999989,287444.749999994],[406945.899999993,287444.949999993],[406944.999999997,287445.349999991],[406944.499999999,287445.64999999],[406943.650000003,287446.349999987],[406942.900000006,287447.10000001],[406942.500000008,287447.800000007],[406942.00000001,287448.700000003],[406941.600000011,287449.599999999],[406941.350000013,287450.849999994],[406941.350000013,287451.84999999],[406941.450000012,287452.850000012],[406941.750000011,287453.850000007],[406941.800000011,287454.000000007],[406942.150000009,287454.850000003],[406942.650000007,287455.6],[406943.150000005,287456.299999997],[406944.499999999,287457.299999992],[406944.899999997,287457.599999991],[406945.299999995,287457.949999989],[406944.399999999,287461.450000001],[406941.750000011,287461.999999998],[406944.399999999,287461.450000001]],[[406944.399999999,287461.450000001],[406947.750000011,287462.299999997],[406946.44999999,287467.450000001],[406943.050000005,287466.550000005],[406927.799999992,287456.849999994],[406944.399999999,287461.450000001]]]}";
				com.esri.core.geometry.MapGeometry mg = com.esri.core.geometry.TestCommonMethods.
					fromJson(str);
				bool res = op.execute((mg.getGeometry()), (mg.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testWithin()
		{
			{
				com.esri.core.geometry.OperatorWithin op = (com.esri.core.geometry.OperatorWithin
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Within));
				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[0,0],[0,0],[0,0]]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				string str2 = "{\"x\":100,\"y\":100}";
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str2);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
			{
				// polygon
				com.esri.core.geometry.OperatorWithin op = (com.esri.core.geometry.OperatorWithin
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Within));
				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[100,0]]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				string str2 = "{\"rings\":[[[10,10],[10,100],[100,100],[100,10]]]}";
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str2);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
			{
				// Multi_point
				com.esri.core.geometry.OperatorWithin op = (com.esri.core.geometry.OperatorWithin
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Within));
				string str1 = "{\"points\":[[0,0],[0,200],[200,200]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				string str2 = "{\"points\":[[0,0],[0,200]]}";
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str2);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
			{
				// Multi_point
				com.esri.core.geometry.OperatorWithin op = (com.esri.core.geometry.OperatorWithin
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Within));
				string str1 = "{\"points\":[[0,0],[0,200]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				string str2 = "{\"points\":[[0,0],[0,200]]}";
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str2);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
			}
			{
				// Multi_point
				com.esri.core.geometry.OperatorWithin op = (com.esri.core.geometry.OperatorWithin
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Within));
				string str1 = "{\"points\":[[0,0],[0,200],[200,200]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				string str2 = "{\"points\":[[0,0],[0,200], [1, 1]]}";
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str2);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testContains()
		{
			{
				com.esri.core.geometry.OperatorContains op = (com.esri.core.geometry.OperatorContains
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Contains));
				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[0,0],[0,0],[0,0]]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				string str2 = "{\"x\":100,\"y\":100}";
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str2);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
			}
			{
				// polygon
				com.esri.core.geometry.OperatorContains op = (com.esri.core.geometry.OperatorContains
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Contains));
				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[0,0]]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				string str2 = "{\"rings\":[[[10,10],[10,100],[100,100],[10,10]]]}";
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str2);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
			}
			{
				// Multi_point
				com.esri.core.geometry.OperatorContains op = (com.esri.core.geometry.OperatorContains
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Contains));
				string str1 = "{\"points\":[[0,0],[0,200],[200,200]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				string str2 = "{\"points\":[[0,0],[0,200]]}";
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str2);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
			}
			{
				// Multi_point
				com.esri.core.geometry.OperatorContains op = (com.esri.core.geometry.OperatorContains
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Contains));
				string str1 = "{\"points\":[[0,0],[0,200]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				string str2 = "{\"points\":[[0,0],[0,200]]}";
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str2);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
			}
			{
				// Multi_point
				com.esri.core.geometry.OperatorContains op = (com.esri.core.geometry.OperatorContains
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Contains));
				string str1 = "{\"points\":[[0,0],[0,200],[200,200]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				string str2 = "{\"points\":[[0,0],[0,200], [1, 1]]}";
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str2);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testOverlaps()
		{
			{
				// empty polygon
				com.esri.core.geometry.OperatorOverlaps op = (com.esri.core.geometry.OperatorOverlaps
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Overlaps));
				com.esri.core.geometry.Polygon poly1 = new com.esri.core.geometry.Polygon();
				com.esri.core.geometry.Polygon poly2 = new com.esri.core.geometry.Polygon();
				bool res = op.execute(poly1, poly2, null, null);
				NUnit.Framework.Assert.IsTrue(!res);
				res = op.execute(poly1, poly2, null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
			{
				// polygon
				com.esri.core.geometry.OperatorOverlaps op = (com.esri.core.geometry.OperatorOverlaps
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Overlaps));
				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[0,0],[0,0],[0,0]]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				string str2 = "{\"x\":100,\"y\":100}";
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str2);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
			{
				// polygon
				com.esri.core.geometry.OperatorOverlaps op = (com.esri.core.geometry.OperatorOverlaps
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Overlaps));
				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[0,0]]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				com.esri.core.geometry.Transformation2D trans = new com.esri.core.geometry.Transformation2D
					();
				trans.setShift(300, 0);
				mg2.getGeometry().applyTransformation(trans);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
			{
				// polygon
				com.esri.core.geometry.OperatorOverlaps op = (com.esri.core.geometry.OperatorOverlaps
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Overlaps));
				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[0,0]]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				com.esri.core.geometry.Transformation2D trans = new com.esri.core.geometry.Transformation2D
					();
				trans.setShift(30, 0);
				mg2.getGeometry().applyTransformation(trans);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
			}
			{
				// polygon
				com.esri.core.geometry.OperatorOverlaps op = (com.esri.core.geometry.OperatorOverlaps
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Overlaps));
				string str1 = "{\"rings\":[[[0,0],[0,200],[200,200],[200,0],[0,0]]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				com.esri.core.geometry.Transformation2D trans = new com.esri.core.geometry.Transformation2D
					();
				trans.setShift(0, 0);
				mg2.getGeometry().applyTransformation(trans);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
			{
				// polyline
				com.esri.core.geometry.OperatorOverlaps op = (com.esri.core.geometry.OperatorOverlaps
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Overlaps));
				string str1 = "{\"paths\":[[[0,0],[100,0],[200,0]]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				com.esri.core.geometry.Transformation2D trans = new com.esri.core.geometry.Transformation2D
					();
				trans.setShift(0, 0);
				mg2.getGeometry().applyTransformation(trans);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
			{
				// polyline
				com.esri.core.geometry.OperatorOverlaps op = (com.esri.core.geometry.OperatorOverlaps
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Overlaps));
				string str1 = "{\"paths\":[[[0,0],[100,0],[200,0]]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				com.esri.core.geometry.Transformation2D trans = new com.esri.core.geometry.Transformation2D
					();
				trans.setShift(10, 0);
				mg2.getGeometry().applyTransformation(trans);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
			}
			{
				// polyline
				com.esri.core.geometry.OperatorOverlaps op = (com.esri.core.geometry.OperatorOverlaps
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Overlaps));
				string str1 = "{\"paths\":[[[0,0],[100,0],[200,0]]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				com.esri.core.geometry.Transformation2D trans = new com.esri.core.geometry.Transformation2D
					();
				trans.setShift(200, 0);
				mg2.getGeometry().applyTransformation(trans);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
			{
				// Multi_point
				com.esri.core.geometry.OperatorOverlaps op = (com.esri.core.geometry.OperatorOverlaps
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Overlaps));
				string str1 = "{\"points\":[[0,0],[0,200],[200,200],[200,0]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				com.esri.core.geometry.Transformation2D trans = new com.esri.core.geometry.Transformation2D
					();
				trans.setShift(0, 0);
				mg2.getGeometry().applyTransformation(trans);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
			{
				// Multi_point
				com.esri.core.geometry.OperatorOverlaps op = (com.esri.core.geometry.OperatorOverlaps
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Overlaps));
				string str1 = "{\"points\":[[0,0],[0,200],[200,200]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				string str2 = "{\"points\":[[0,0],[0,200]]}";
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str2);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(!res);
			}
			{
				// Multi_point
				com.esri.core.geometry.OperatorOverlaps op = (com.esri.core.geometry.OperatorOverlaps
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Overlaps));
				string str1 = "{\"points\":[[0,0],[0,200],[200,200]]}";
				com.esri.core.geometry.MapGeometry mg1 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str1);
				string str2 = "{\"points\":[[0,0],[0,200], [0,2]]}";
				com.esri.core.geometry.MapGeometry mg2 = com.esri.core.geometry.TestCommonMethods
					.fromJson(str2);
				bool res = op.execute((mg2.getGeometry()), (mg1.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
				res = op.execute((mg1.getGeometry()), (mg2.getGeometry()), null, null);
				NUnit.Framework.Assert.IsTrue(res);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonPolygonEquals()
		{
			com.esri.core.geometry.OperatorEquals equals = (com.esri.core.geometry.OperatorEquals
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Equals));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			// Polygon1 and Polygon2 are topologically equal, but have differing
			// number of vertices
			string str1 = "{\"rings\":[[[0,0],[0,5],[0,7],[0,10],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			string str2 = "{\"rings\":[[[0,10],[10,10],[10,0],[0,0],[0,10]],[[9,1],[9,6],[9,9],[1,9],[1,1],[1,1],[9,1]]]}";
			com.esri.core.geometry.Polygon polygon1 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry();
			com.esri.core.geometry.Polygon polygon2 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry();
			// wiggleGeometry(polygon1, tolerance, 1982);
			// wiggleGeometry(polygon2, tolerance, 511);
			equals.accelerateGeometry(polygon1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			equals.accelerateGeometry(polygon2, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			bool res = equals.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			equals.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			// The outer rings of Polygon1 and Polygon2 are equal, but Polygon1 has
			// a hole.
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"rings\":[[[0,10],[10,10],[5,10],[10,10],[10,0],[0,0],[0,10]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry();
			polygon2 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry();
			res = equals.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = equals.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// The rings are equal but rotated
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]]]}";
			str2 = "{\"rings\":[[[0,10],[10,10],[10,0],[0,0],[0,10]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry();
			polygon2 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry();
			res = equals.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = equals.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			// The rings are equal but opposite orientation
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]]]}";
			str2 = "{\"rings\":[[[0,0],[10,0],[10,10],[0,10],[0,0]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry();
			polygon2 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry();
			res = equals.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = equals.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// The rings are equal but first polygon has two rings stacked
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[0,10],[10,10],[10,0],[0,0],[0,10]]]}";
			str2 = "{\"rings\":[[[0,10],[10,10],[10,0],[0,0],[0,10]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry();
			polygon2 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry();
			res = equals.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = equals.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPointMultiPointEquals()
		{
			com.esri.core.geometry.OperatorEquals equals = (com.esri.core.geometry.OperatorEquals
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Equals);
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.MultiPoint multipoint1 = new com.esri.core.geometry.MultiPoint
				();
			com.esri.core.geometry.MultiPoint multipoint2 = new com.esri.core.geometry.MultiPoint
				();
			multipoint1.add(0, 0);
			multipoint1.add(1, 1);
			multipoint1.add(2, 2);
			multipoint1.add(3, 3);
			multipoint1.add(4, 4);
			multipoint1.add(1, 1);
			multipoint1.add(0, 0);
			multipoint2.add(4, 4);
			multipoint2.add(3, 3);
			multipoint2.add(2, 2);
			multipoint2.add(1, 1);
			multipoint2.add(0, 0);
			multipoint2.add(2, 2);
			wiggleGeometry(multipoint1, 0.001, 123);
			wiggleGeometry(multipoint2, 0.001, 5937);
			bool res = equals.execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = equals.execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint1.add(1, 2);
			res = equals.execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = equals.execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPointPointEquals()
		{
			com.esri.core.geometry.OperatorEquals equals = (com.esri.core.geometry.OperatorEquals
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Equals));
			com.esri.core.geometry.OperatorWithin within = (com.esri.core.geometry.OperatorWithin
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Within));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.MultiPoint multipoint1 = new com.esri.core.geometry.MultiPoint
				();
			com.esri.core.geometry.Point point2 = new com.esri.core.geometry.Point();
			multipoint1.add(2, 2);
			multipoint1.add(2, 2);
			point2.setXY(2, 2);
			wiggleGeometry(multipoint1, 0.001, 123);
			bool res = equals.execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = equals.execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = within.execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = within.execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint1.add(4, 4);
			res = equals.execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = equals.execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPointPointEquals()
		{
			com.esri.core.geometry.OperatorEquals equals = (com.esri.core.geometry.OperatorEquals
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Equals));
			com.esri.core.geometry.OperatorWithin within = (com.esri.core.geometry.OperatorWithin
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Within));
			com.esri.core.geometry.OperatorContains contains = (com.esri.core.geometry.OperatorContains
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Contains));
			com.esri.core.geometry.OperatorDisjoint disjoint = (com.esri.core.geometry.OperatorDisjoint
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Disjoint));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Point point1 = new com.esri.core.geometry.Point();
			com.esri.core.geometry.Point point2 = new com.esri.core.geometry.Point();
			point1.setXY(2, 2);
			point2.setXY(2, 2);
			bool res = equals.execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = equals.execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = within.execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = within.execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = contains.execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = contains.execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			point2.setXY(2, 3);
			res = equals.execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = equals.execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(point1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.execute(point2, point1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonPolygonDisjoint()
		{
			com.esri.core.geometry.OperatorDisjoint disjoint = (com.esri.core.geometry.OperatorDisjoint
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Disjoint));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			// Polygon1 and Polygon2 are topologically equal, but have differing
			// number of vertices
			string str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			string str2 = "{\"rings\":[[[0,10],[10,10],[10,0],[0,0],[0,10]],[[9,1],[9,6],[9,9],[1,9],[1,1],[1,1],[9,1]]]}";
			com.esri.core.geometry.Polygon polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			com.esri.core.geometry.Polygon polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			bool res = disjoint.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// Polygon1 and Polygon2 touch at a point
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"rings\":[[[10,10],[10,15],[15,15],[15,10],[10,10]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polygon1, tolerance, 1982);
			wiggleGeometry(polygon2, tolerance, 511);
			res = disjoint.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// Polygon1 and Polygon2 touch along the boundary
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"rings\":[[[10,0],[10,10],[15,10],[15,0],[10,0]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polygon1, tolerance, 1982);
			wiggleGeometry(polygon2, tolerance, 511);
			res = disjoint.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// Polygon2 is inside of the hole of polygon1
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[8,2],[2,2]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = disjoint.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			// Polygon2 is inside of the hole of polygon1
			str1 = "{\"rings\":[[[0,0],[0,5],[5,5],[5,0]],[[10,0],[10,10],[20,10],[20,0]]]}";
			str2 = "{\"rings\":[[[0,-10],[0,-5],[5,-5],[5,-10]],[[11,1],[11,9],[19,9],[19,1]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = disjoint.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polygon1 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.OperatorDensifyByLength
				.local().execute(polygon1, 0.5, null);
			disjoint.accelerateGeometry(polygon1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			res = disjoint.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polygon1.reverseAllPaths();
			polygon2.reverseAllPaths();
			res = disjoint.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// Polygon1 contains polygon2, but polygon2 is counterclockwise.
			str1 = "{\"rings\":[[[0,0],[10,0],[10,10],[0,10],[0,0]],[[11,0],[11,10],[21,10],[21,0],[11,0]]]}";
			str2 = "{\"rings\":[[[2,2],[8,2],[8,8],[2,8],[2,2]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = disjoint.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polygon1 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.OperatorDensifyByLength
				.local().execute(polygon1, 0.5, null);
			disjoint.accelerateGeometry(polygon1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			res = disjoint.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[0,20],[0,30],[10,30],[10,20],[0,20]],[[20,20],[20,30],[30,30],[30,20],[20,20]],[[20,0],[20,10],[30,10],[30,0],[20,0]]]}";
			str2 = "{\"rings\":[[[14,14],[14,16],[16,16],[16,14],[14,14]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = disjoint.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon1 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.OperatorDensifyByLength
				.local().execute(polygon1, 0.5, null);
			disjoint.accelerateGeometry(polygon1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			res = disjoint.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolylinePolylineDisjoint()
		{
			com.esri.core.geometry.OperatorDisjoint disjoint = (com.esri.core.geometry.OperatorDisjoint
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Disjoint));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			// Polyline1 and Polyline2 touch at a point
			string str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			string str2 = "{\"paths\":[[[10,10],[10,15],[15,15],[15,10],[10,10]]]}";
			com.esri.core.geometry.Polyline polyline1 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			com.esri.core.geometry.Polyline polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polyline1, tolerance, 1982);
			wiggleGeometry(polyline2, tolerance, 511);
			bool res = disjoint.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// Polyline1 and Polyline2 touch along the boundary
			str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"paths\":[[[10,0],[10,10],[15,10],[15,0],[10,0]]]}";
			polyline1 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polyline1, tolerance, 1982);
			wiggleGeometry(polyline2, tolerance, 511);
			res = disjoint.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// Polyline2 does not intersect with Polyline1
			str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"paths\":[[[2,2],[2,8],[8,8],[8,2],[2,2]]]}";
			polyline1 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = disjoint.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonPolylineDisjoint()
		{
			com.esri.core.geometry.OperatorDisjoint disjoint = (com.esri.core.geometry.OperatorDisjoint
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Disjoint));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Polygon polygon1 = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.Polyline polyline2 = new com.esri.core.geometry.Polyline();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 10);
			polygon1.lineTo(10, 10);
			polygon1.lineTo(10, 0);
			polygon1.startPath(1, 1);
			polygon1.lineTo(9, 1);
			polygon1.lineTo(9, 9);
			polygon1.lineTo(1, 9);
			polyline2.startPath(3, 3);
			polyline2.lineTo(6, 6);
			bool res = disjoint.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline2.startPath(0, 0);
			polyline2.lineTo(0, 5);
			res = disjoint.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polygon1.setEmpty();
			polyline2.setEmpty();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 10);
			polygon1.lineTo(10, 10);
			polygon1.lineTo(10, 0);
			polyline2.startPath(2, 2);
			polyline2.lineTo(4, 4);
			com.esri.core.geometry.OperatorFactoryLocal factory = com.esri.core.geometry.OperatorFactoryLocal
				.getInstance();
			com.esri.core.geometry.OperatorSimplify simplify_op = (com.esri.core.geometry.OperatorSimplify
				)factory.getOperator(com.esri.core.geometry.Operator.Type.Simplify);
			simplify_op.isSimpleAsFeature(polygon1, sr, null);
			simplify_op.isSimpleAsFeature(polyline2, sr, null);
			res = disjoint.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolylineMultiPointDisjoint()
		{
			com.esri.core.geometry.OperatorDisjoint disjoint = (com.esri.core.geometry.OperatorDisjoint
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Disjoint));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Polyline polyline1 = new com.esri.core.geometry.Polyline();
			com.esri.core.geometry.MultiPoint multipoint2 = new com.esri.core.geometry.MultiPoint
				();
			polyline1.startPath(0, 0);
			polyline1.lineTo(2, 0);
			polyline1.lineTo(4, 2);
			multipoint2.add(1, 1);
			multipoint2.add(2, 2);
			multipoint2.add(3, 0);
			bool res = disjoint.execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.add(3, 1);
			res = disjoint.execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.startPath(1, -4);
			polyline1.lineTo(1, -3);
			polyline1.lineTo(1, -2);
			polyline1.lineTo(1, -1);
			polyline1.lineTo(1, 0);
			polyline1.lineTo(1, 1);
			disjoint.accelerateGeometry(polyline1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			res = disjoint.execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolylinePointDisjoint()
		{
			com.esri.core.geometry.OperatorDisjoint disjoint = (com.esri.core.geometry.OperatorDisjoint
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Disjoint));
			com.esri.core.geometry.OperatorContains contains = (com.esri.core.geometry.OperatorContains
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Contains));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Polyline polyline1 = new com.esri.core.geometry.Polyline();
			com.esri.core.geometry.Point point2 = new com.esri.core.geometry.Point();
			polyline1.startPath(0, 0);
			polyline1.lineTo(2, 0);
			polyline1.lineTo(4, 2);
			point2.setXY(1, 1);
			bool res = disjoint.execute(polyline1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.execute(point2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = contains.execute(polyline1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.execute(point2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			point2.setXY(4, 2);
			polyline1 = (com.esri.core.geometry.Polyline)com.esri.core.geometry.OperatorDensifyByLength
				.local().execute(polyline1, 0.1, null);
			disjoint.accelerateGeometry(polyline1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			res = disjoint.execute(polyline1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(point2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.execute(polyline1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.execute(point2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.setEmpty();
			point2.setEmpty();
			polyline1.startPath(659062.37370000035, 153070.85220000148);
			polyline1.lineTo(660916.47940000147, 151481.10269999877);
			point2.setXY(659927.85020000115, 152328.77430000156);
			res = contains.execute(polyline1, point2, com.esri.core.geometry.SpatialReference
				.create(54004), null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPointMultiPointDisjoint()
		{
			com.esri.core.geometry.OperatorDisjoint disjoint = (com.esri.core.geometry.OperatorDisjoint
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Disjoint));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.MultiPoint multipoint1 = new com.esri.core.geometry.MultiPoint
				();
			com.esri.core.geometry.MultiPoint multipoint2 = new com.esri.core.geometry.MultiPoint
				();
			multipoint1.add(2, 2);
			multipoint1.add(2, 5);
			multipoint1.add(4, 1);
			multipoint1.add(4, 4);
			multipoint1.add(4, 7);
			multipoint1.add(6, 2);
			multipoint1.add(6, 6);
			multipoint1.add(4, 1);
			multipoint1.add(6, 6);
			multipoint2.add(0, 1);
			multipoint2.add(0, 7);
			multipoint2.add(4, 2);
			multipoint2.add(4, 6);
			multipoint2.add(6, 4);
			multipoint2.add(4, 2);
			multipoint2.add(0, 1);
			bool res = disjoint.execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.add(2, 2);
			res = disjoint.execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPointPointDisjoint()
		{
			com.esri.core.geometry.OperatorDisjoint disjoint = (com.esri.core.geometry.OperatorDisjoint
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Disjoint));
			com.esri.core.geometry.OperatorContains contains = (com.esri.core.geometry.OperatorContains
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Contains));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.MultiPoint multipoint1 = new com.esri.core.geometry.MultiPoint
				();
			com.esri.core.geometry.Point point2 = new com.esri.core.geometry.Point();
			multipoint1.add(2, 2);
			multipoint1.add(2, 5);
			multipoint1.add(4, 1);
			multipoint1.add(4, 4);
			multipoint1.add(4, 7);
			multipoint1.add(6, 2);
			multipoint1.add(6, 6);
			multipoint1.add(4, 1);
			multipoint1.add(6, 6);
			point2.setXY(2, 6);
			bool res = disjoint.execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = contains.execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			multipoint1.add(2, 6);
			res = disjoint.execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.execute(multipoint1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = contains.execute(point2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonMultiPointDisjoint()
		{
			com.esri.core.geometry.OperatorDisjoint disjoint = (com.esri.core.geometry.OperatorDisjoint
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Disjoint));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Polygon polygon1 = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.MultiPoint multipoint2 = new com.esri.core.geometry.MultiPoint
				();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 10);
			polygon1.lineTo(10, 10);
			multipoint2.add(-1, 5);
			multipoint2.add(5, 11);
			multipoint2.add(11, 5);
			multipoint2.add(5, -1);
			bool res = disjoint.execute(polygon1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon1.startPath(15, 0);
			polygon1.lineTo(15, 10);
			polygon1.lineTo(25, 10);
			polygon1.lineTo(25, 0);
			multipoint2.add(14, 5);
			multipoint2.add(20, 11);
			multipoint2.add(26, 5);
			multipoint2.add(20, -1);
			res = disjoint.execute(polygon1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = disjoint.execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.add(20, 5);
			res = disjoint.execute(polygon1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = disjoint.execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonMultiPointTouches()
		{
			com.esri.core.geometry.OperatorTouches touches = (com.esri.core.geometry.OperatorTouches
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Touches));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Polygon polygon1 = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.MultiPoint multipoint2 = new com.esri.core.geometry.MultiPoint
				();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 10);
			polygon1.lineTo(10, 10);
			polygon1.lineTo(10, 0);
			multipoint2.add(-1, 5);
			multipoint2.add(5, 11);
			multipoint2.add(11, 5);
			multipoint2.add(5, -1);
			bool res = touches.execute(polygon1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			multipoint2.add(5, 10);
			res = touches.execute(polygon1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.add(5, 5);
			res = touches.execute(polygon1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonPointTouches()
		{
			com.esri.core.geometry.OperatorTouches touches = (com.esri.core.geometry.OperatorTouches
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Touches));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Polygon polygon1 = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.Point point2 = new com.esri.core.geometry.Point();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 10);
			polygon1.lineTo(10, 10);
			polygon1.lineTo(10, 0);
			point2.setXY(5, 5);
			bool res = touches.execute(polygon1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.execute(point2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			point2.setXY(5, 10);
			res = touches.execute(polygon1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.execute(point2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonPolygonTouches()
		{
			com.esri.core.geometry.OperatorTouches touches = (com.esri.core.geometry.OperatorTouches
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Touches));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			// Polygon1 and Polygon2 touch at a point
			string str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			string str2 = "{\"rings\":[[[10,10],[10,15],[15,15],[15,10],[10,10]]]}";
			com.esri.core.geometry.Polygon polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			com.esri.core.geometry.Polygon polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polygon1, tolerance, 1982);
			wiggleGeometry(polygon2, tolerance, 511);
			bool res = touches.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			// Polygon1 and Polygon2 touch along the boundary
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"rings\":[[[10,0],[10,10],[15,10],[15,0],[10,0]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polygon1, tolerance, 1982);
			wiggleGeometry(polygon2, tolerance, 511);
			res = touches.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			// Polygon1 and Polygon2 touch at a corner of Polygon1 and a diagonal of
			// Polygon2
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"rings\":[[[15,5],[5,15],[15,15],[15,5]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polygon1, tolerance, 1982);
			wiggleGeometry(polygon2, tolerance, 511);
			res = touches.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			// Polygon1 and Polygon2 do not touch
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"rings\":[[[5,5],[5,15],[15,15],[15,5],[5,5]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = touches.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polygon1.setEmpty();
			polygon2.setEmpty();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 1);
			polygon1.lineTo(-1, 0);
			polygon2.startPath(0, 0);
			polygon2.lineTo(0, 1);
			polygon2.lineTo(1, 0);
			res = touches.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonPolylineTouches()
		{
			com.esri.core.geometry.OperatorTouches touches = (com.esri.core.geometry.OperatorTouches
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Touches));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			// Polygon1 and Polyline2 touch at a point
			string str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			string str2 = "{\"paths\":[[[10,10],[10,15],[15,15],[15,10]]]}";
			com.esri.core.geometry.Polygon polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			com.esri.core.geometry.Polyline polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polygon1, tolerance, 1982);
			wiggleGeometry(polyline2, tolerance, 511);
			bool res = touches.execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			// Polygon1 and Polyline2 overlap along the boundary
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"paths\":[[[10,0],[10,10],[15,10],[15,0]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polygon1, tolerance, 1982);
			wiggleGeometry(polyline2, tolerance, 511);
			res = touches.execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"paths\":[[[15,5],[5,15],[15,15],[15,5]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polygon1, tolerance, 1982);
			wiggleGeometry(polyline2, tolerance, 511);
			res = touches.execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			str1 = "{\"rings\":[[[10,10],[10,0],[0,0],[0,10],[10,10]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"paths\":[[[15,5],[5,15],[15,15]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polygon1, tolerance, 1982);
			wiggleGeometry(polyline2, tolerance, 511);
			res = touches.execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolylinePolylineTouches()
		{
			com.esri.core.geometry.OperatorTouches touches = (com.esri.core.geometry.OperatorTouches
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Touches));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			// Polyline1 and Polyline2 touch at a point
			string str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			string str2 = "{\"paths\":[[[10,10],[10,15],[15,15],[15,10]]]}";
			com.esri.core.geometry.Polyline polyline1 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			com.esri.core.geometry.Polyline polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			bool res = touches.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			// Polyline1 and Polyline2 overlap along the boundary
			str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"paths\":[[[10,0],[10,10],[15,10],[15,0],[10,0]]]}";
			polyline1 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = touches.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// Polyline1 and Polyline2 intersect at interiors
			str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"paths\":[[[15,5],[5,15],[15,15],[15,5]]]}";
			polyline1 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = touches.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// Polyline1 and Polyline2 touch at an endpoint of Polyline1 and
			// interior of Polyline2 (but Polyline1 is closed)
			str1 = "{\"paths\":[[[10,10],[10,0],[0,0],[0,10],[10,10]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"paths\":[[[15,5],[5,15],[15,15]]]}";
			polyline1 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = touches.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// Polyline1 and Polyline2 touch at an endpoint of Polyline1 and
			// interior of Polyline2 (same as previous case, but Polyline1 is not
			// closed)
			str1 = "{\"paths\":[[[10,10],[10,0],[0,0],[0,10]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"paths\":[[[15,5],[5,15],[15,15]]]}";
			polyline1 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = touches.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			str1 = "{\"paths\":[[[10,10],[10,0],[0,0],[0,10]],[[1,1],[9,1],[9,9],[1,9],[6, 9]]]}";
			str2 = "{\"paths\":[[[15,5],[5,15],[15,15],[15,5]]]}";
			polyline1 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = touches.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(-2, -2);
			polyline1.lineTo(-1, -1);
			polyline1.lineTo(1, 1);
			polyline1.lineTo(2, 2);
			polyline2.startPath(-2, 2);
			polyline2.lineTo(-1, 1);
			polyline2.lineTo(1, -1);
			polyline2.lineTo(2, -2);
			res = touches.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(-2, -2);
			polyline1.lineTo(-1, -1);
			polyline1.lineTo(1, 1);
			polyline1.lineTo(2, 2);
			polyline2.startPath(-2, 2);
			polyline2.lineTo(-1, 1);
			polyline2.lineTo(1, -1);
			res = touches.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(-1, -1);
			polyline1.lineTo(0, 0);
			polyline1.lineTo(1, 1);
			polyline2.startPath(-1, 1);
			polyline2.lineTo(0, 0);
			polyline2.lineTo(1, -1);
			res = touches.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(0, 0);
			polyline1.lineTo(0, 1);
			polyline1.lineTo(0, 0);
			polyline2.startPath(0, 1);
			polyline2.lineTo(0, 2);
			polyline2.lineTo(0, 1);
			res = touches.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolylineMultiPointTouches()
		{
			com.esri.core.geometry.OperatorTouches touches = (com.esri.core.geometry.OperatorTouches
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Touches));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Polyline polyline1 = new com.esri.core.geometry.Polyline();
			com.esri.core.geometry.MultiPoint multipoint2 = new com.esri.core.geometry.MultiPoint
				();
			polyline1.startPath(0, 0);
			polyline1.lineTo(2, 0);
			polyline1.lineTo(4, 2);
			multipoint2.add(1, 1);
			multipoint2.add(2, 2);
			multipoint2.add(3, 0);
			bool res = touches.execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.startPath(1, -4);
			polyline1.lineTo(1, -3);
			polyline1.lineTo(1, -2);
			polyline1.lineTo(1, -1);
			polyline1.lineTo(1, 0);
			polyline1.lineTo(1, 1);
			touches.accelerateGeometry(polyline1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			res = touches.execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.add(3, 1);
			res = touches.execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.startPath(0, 0);
			polyline1.lineTo(2, 0);
			polyline1.startPath(2, 1);
			polyline1.lineTo(2, -1);
			multipoint2.add(2, 0);
			res = touches.execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = touches.execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolylineMultiPointCrosses()
		{
			com.esri.core.geometry.OperatorCrosses crosses = (com.esri.core.geometry.OperatorCrosses
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Crosses));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Polyline polyline1 = new com.esri.core.geometry.Polyline();
			com.esri.core.geometry.MultiPoint multipoint2 = new com.esri.core.geometry.MultiPoint
				();
			polyline1.startPath(0, 0);
			polyline1.lineTo(2, 0);
			polyline1.lineTo(4, 2);
			multipoint2.add(1, 1);
			multipoint2.add(2, 2);
			multipoint2.add(3, 0);
			multipoint2.add(0, 0);
			bool res = crosses.execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = crosses.execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.startPath(1, -4);
			polyline1.lineTo(1, -3);
			polyline1.lineTo(1, -2);
			polyline1.lineTo(1, -1);
			polyline1.lineTo(1, 0);
			polyline1.lineTo(1, 1);
			res = crosses.execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = crosses.execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			crosses.accelerateGeometry(polyline1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			multipoint2.add(1, 0);
			res = crosses.execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = crosses.execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.add(3, 1);
			res = crosses.execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = crosses.execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolylinePointTouches()
		{
			com.esri.core.geometry.OperatorTouches touches = (com.esri.core.geometry.OperatorTouches
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Touches));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Polyline polyline1 = new com.esri.core.geometry.Polyline();
			com.esri.core.geometry.Point point2 = new com.esri.core.geometry.Point();
			polyline1.startPath(0, 0);
			polyline1.lineTo(2, 0);
			polyline1.startPath(2, 1);
			polyline1.lineTo(2, -1);
			point2.setXY(2, 0);
			bool res = touches.execute(polyline1, point2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = touches.execute(point2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonPolygonOverlaps()
		{
			com.esri.core.geometry.OperatorOverlaps overlaps = (com.esri.core.geometry.OperatorOverlaps
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Overlaps));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			// Polygon1 and Polygon2 touch at a point
			string str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			string str2 = "{\"rings\":[[[10,10],[10,15],[15,15],[15,10],[10,10]]]}";
			com.esri.core.geometry.Polygon polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			com.esri.core.geometry.Polygon polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polygon1, tolerance, 1982);
			wiggleGeometry(polygon2, tolerance, 511);
			bool res = overlaps.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = overlaps.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// Polygon1 and Polygon2 touch along the boundary
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"rings\":[[[10,0],[10,10],[15,10],[15,0],[10,0]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polygon1, tolerance, 1982);
			wiggleGeometry(polygon2, tolerance, 511);
			res = overlaps.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = overlaps.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// Polygon1 and Polygon2 touch at a corner of Polygon1 and a diagonal of
			// Polygon2
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"rings\":[[[15,5],[5,15],[15,15],[15,5]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polygon1, tolerance, 1982);
			wiggleGeometry(polygon2, tolerance, 511);
			res = overlaps.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = overlaps.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// Polygon1 and Polygon2 overlap at the upper right corner
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"rings\":[[[5,5],[5,15],[15,15],[15,5],[5,5]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = overlaps.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = overlaps.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[4,4],[6,4],[6,6],[4,6],[4,4],[4,4]]]}";
			str2 = "{\"rings\":[[[1,1],[1,9],[9,9],[9,1],[1,1]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = overlaps.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = overlaps.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonPolylineWithin()
		{
			com.esri.core.geometry.OperatorWithin within = (com.esri.core.geometry.OperatorWithin
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Within));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Polygon polygon1 = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.Polyline polyline2 = new com.esri.core.geometry.Polyline();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 10);
			polygon1.lineTo(10, 10);
			polygon1.lineTo(10, 0);
			polyline2.startPath(5, 0);
			polyline2.lineTo(5, 10);
			bool res = within.execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline2.setEmpty();
			polyline2.startPath(0, 1);
			polyline2.lineTo(0, 9);
			res = within.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPointMultiPointWithin()
		{
			com.esri.core.geometry.OperatorWithin within = (com.esri.core.geometry.OperatorWithin
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Within));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.MultiPoint multipoint1 = new com.esri.core.geometry.MultiPoint
				();
			com.esri.core.geometry.MultiPoint multipoint2 = new com.esri.core.geometry.MultiPoint
				();
			multipoint1.add(0, 0);
			multipoint1.add(3, 3);
			multipoint1.add(0, 0);
			multipoint1.add(5, 5);
			multipoint1.add(3, 3);
			multipoint1.add(2, 4);
			multipoint1.add(2, 8);
			multipoint2.add(0, 0);
			multipoint2.add(3, 3);
			multipoint2.add(2, 4);
			multipoint2.add(2, 8);
			multipoint2.add(5, 5);
			bool res = within.execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = within.execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.add(10, 10);
			multipoint2.add(10, 10);
			res = within.execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = within.execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			multipoint1.add(10, 10);
			res = within.execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = within.execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint1.add(-10, -10);
			res = within.execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolylinePolylineOverlaps()
		{
			com.esri.core.geometry.OperatorOverlaps overlaps = (com.esri.core.geometry.OperatorOverlaps
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Overlaps));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Polyline polyline1 = new com.esri.core.geometry.Polyline();
			com.esri.core.geometry.Polyline polyline2 = new com.esri.core.geometry.Polyline();
			polyline1.startPath(0, 0);
			polyline1.lineTo(2, 0);
			polyline2.startPath(1, 0);
			polyline2.lineTo(3, 0);
			polyline2.lineTo(1, 1);
			polyline2.lineTo(1, -1);
			wiggleGeometry(polyline1, tolerance, 1982);
			wiggleGeometry(polyline2, tolerance, 511);
			bool res = overlaps.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = overlaps.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(0, 0);
			polyline1.lineTo(2, 0);
			polyline2.startPath(1.9989, 0);
			polyline2.lineTo(2.0011, 0);
			// wiggleGeometry(polyline1, tolerance, 1982);
			// wiggleGeometry(polyline2, tolerance, 511);
			res = overlaps.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = overlaps.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(0, 0);
			polyline1.lineTo(2, 0);
			polyline2.startPath(1.9989, 0);
			polyline2.lineTo(2.0009, 0);
			wiggleGeometry(polyline1, tolerance, 1982);
			wiggleGeometry(polyline2, tolerance, 511);
			res = overlaps.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = overlaps.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(0, 0);
			polyline1.lineTo(2, 0);
			polyline2.startPath(0, 0);
			polyline2.lineTo(2, 0);
			polyline2.startPath(0, -1);
			polyline2.lineTo(2, -1);
			res = overlaps.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = overlaps.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPointMultiPointOverlaps()
		{
			com.esri.core.geometry.OperatorOverlaps overlaps = (com.esri.core.geometry.OperatorOverlaps
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Overlaps));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.MultiPoint multipoint1 = new com.esri.core.geometry.MultiPoint
				();
			com.esri.core.geometry.MultiPoint multipoint2 = new com.esri.core.geometry.MultiPoint
				();
			multipoint1.add(4, 4);
			multipoint1.add(6, 4);
			multipoint2.add(6, 2);
			multipoint2.add(2, 6);
			bool res = overlaps.execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = overlaps.execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			multipoint1.add(10, 10);
			multipoint2.add(6, 2);
			res = overlaps.execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = overlaps.execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			multipoint1.add(6, 2);
			res = overlaps.execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = overlaps.execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint1.add(2, 6);
			res = overlaps.execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = overlaps.execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			multipoint2.add(1, 1);
			res = overlaps.execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = overlaps.execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.add(10, 10);
			multipoint2.add(4, 4);
			multipoint2.add(6, 4);
			res = overlaps.execute(multipoint1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = overlaps.execute(multipoint2, multipoint1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonPolygonWithin()
		{
			com.esri.core.geometry.OperatorWithin within = (com.esri.core.geometry.OperatorWithin
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Within));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			// Polygon1 is within Polygon2
			string str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			string str2 = "{\"rings\":[[[-1,-1],[-1,11],[11,11],[11,-1],[-1,-1]]]}";
			com.esri.core.geometry.Polygon polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			com.esri.core.geometry.Polygon polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			bool res = within.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = within.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// Polygon1 is within Polygon2, and the boundaries intersect
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[4,4],[6,4],[6,6],[4,6],[4,4],[4,4]]]}";
			str2 = "{\"rings\":[[[1,1],[1,9],[9,9],[9,1],[1,1]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polygon1, tolerance, 1982);
			wiggleGeometry(polygon2, tolerance, 511);
			res = within.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// Polygon1 is within Polygon2, and the boundaries intersect
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"rings\":[[[-1,0],[-1,11],[11,11],[11,0],[-1,0]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			wiggleGeometry(polygon1, tolerance, 1982);
			wiggleGeometry(polygon2, tolerance, 511);
			res = within.execute(polygon1, polygon2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			// Polygon2 is inside of the hole of polygon1
			str1 = "{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[8,2],[2,2]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			str1 = "{\"rings\":[[[0,0],[10,0],[10,10],[0,10]]]}";
			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[8,2],[2,2],[8,2],[8,8],[2,8],[2,2]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0]],[[12,8],[12,10],[18,10],[18,8],[12,8]]]}";
			str2 = "{\"paths\":[[[2,2],[2,8],[8,8],[8,2]],[[12,2],[12,4],[18,4],[18,2]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			com.esri.core.geometry.Polyline polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[4,4],[6,4],[6,6],[4,6],[4,4]]]}";
			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[8,2],[2,2],[2,8],[8,8],[8,2],[2,2]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			// Same as above, but winding fill rule
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[4,4],[6,4],[6,6],[4,6],[4,4]]]}";
			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[8,2],[2,2],[2,8],[8,8],[8,2],[2,2]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			polygon1.setFillRule(com.esri.core.geometry.Polygon.FillRule.enumFillRuleWinding);
			polygon2.setFillRule(com.esri.core.geometry.Polygon.FillRule.enumFillRuleWinding);
			res = within.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]]]}";
			str2 = "{\"paths\":[[[2,2],[2,2]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[11,11],[11,20],[20,20],[20,11],[11,11]]]}";
			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[15,15],[8,8],[8,2],[2,2]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[10,10],[10,20],[20,20],[20,10],[10,10]]]}";
			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[15,15],[8,8],[8,2],[2,2]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]]]}";
			str2 = "{\"rings\":[[[9.9999999925,4],[9.9999999925,6],[10.0000000075,6],[10.0000000075,4],[9.9999999925,4]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = com.esri.core.geometry.OperatorOverlaps.local().execute(polygon1, polygon2, 
				sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = com.esri.core.geometry.OperatorTouches.local().execute(polygon1, polygon2, 
				sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[10,10],[10,20],[20,20],[20,10],[10,10]]]}";
			str2 = "{\"rings\":[[[2,2],[2,8],[8,8],[15,15],[8,8],[8,2],[2,2]],[[15,5],[15,5],[15,5]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]]]}";
			str2 = "{\"rings\":[[[2,2],[2,2],[2,2]],[[3,3],[3,3],[3,3]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]]]}";
			str2 = "{\"rings\":[[[2,2],[2,2],[2,2],[2,2]],[[3,3],[3,3],[3,3],[3,3]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polygon2 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polygon2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]]]}";
			str2 = "{\"paths\":[[[2,2],[2,2]],[[3,3],[3,3]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[10,10],[10,20],[20,20],[20,10],[10,10]]]}";
			str2 = "{\"paths\":[[[2,2],[2,8]],[[15,5],[15,5]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[10,10],[10,20],[20,20],[20,10],[10,10]]]}";
			str2 = "{\"paths\":[[[2,2],[2,8]],[[15,5],[15,5],[15,5],[15,5]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[10,10],[10,20],[20,20],[20,10],[10,10]]]}";
			str2 = "{\"paths\":[[[2,2],[2,2]],[[15,5],[15,6]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			str1 = "{\"rings\":[[[0,0],[0,10],[10,10],[10,0],[0,0]],[[10,10],[10,20],[20,20],[20,10],[10,10]]]}";
			str2 = "{\"paths\":[[[2,2],[2,2],[2,2],[2,2]],[[15,5],[15,6]]]}";
			polygon1 = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = within.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolylinePolylineWithin()
		{
			com.esri.core.geometry.OperatorWithin within = (com.esri.core.geometry.OperatorWithin
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Within));
			com.esri.core.geometry.OperatorContains contains = (com.esri.core.geometry.OperatorContains
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Contains));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Polyline polyline1 = new com.esri.core.geometry.Polyline();
			com.esri.core.geometry.Polyline polyline2 = new com.esri.core.geometry.Polyline();
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(0, 0);
			polyline1.lineTo(2, 0);
			polyline2.startPath(1.9989, 0);
			polyline2.lineTo(2.0011, 0);
			bool res = within.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = contains.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(0, 0);
			polyline1.lineTo(2, 0);
			polyline2.startPath(1.9989, 0);
			polyline2.lineTo(2.001, 0);
			res = within.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = contains.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(0, 0);
			polyline1.lineTo(2, 0);
			polyline1.lineTo(3, 0);
			polyline1.lineTo(4, 0);
			polyline1.lineTo(5, 0);
			polyline1.lineTo(6, 0);
			polyline1.lineTo(7, 0);
			polyline1.lineTo(8, 0);
			polyline2.startPath(0, 0);
			polyline2.lineTo(.1, 0);
			polyline2.lineTo(.2, 0);
			polyline2.lineTo(.4, 0);
			polyline2.lineTo(1.1, 0);
			polyline2.lineTo(2.5, 0);
			polyline2.startPath(2.7, 0);
			polyline2.lineTo(4, 0);
			res = within.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = contains.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolylineMultiPointWithin()
		{
			com.esri.core.geometry.OperatorWithin within = (com.esri.core.geometry.OperatorWithin
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Within));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Polyline polyline1 = new com.esri.core.geometry.Polyline();
			com.esri.core.geometry.MultiPoint multipoint2 = new com.esri.core.geometry.MultiPoint
				();
			polyline1.startPath(0, 0);
			polyline1.lineTo(2, 0);
			polyline1.lineTo(4, 2);
			multipoint2.add(1, 0);
			multipoint2.add(2, 0);
			multipoint2.add(3, 1);
			multipoint2.add(2, 0);
			bool res = within.execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.startPath(1, -2);
			polyline1.lineTo(1, -1);
			polyline1.lineTo(1, 0);
			polyline1.lineTo(1, 1);
			res = within.execute(polyline1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.add(1, 2);
			res = within.execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			multipoint2.add(-1, -1);
			multipoint2.add(4, 2);
			multipoint2.add(0, 0);
			res = within.execute(multipoint2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonMultiPointWithin()
		{
			com.esri.core.geometry.OperatorWithin within = (com.esri.core.geometry.OperatorWithin
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Within));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Polygon polygon1 = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.MultiPoint multipoint2 = new com.esri.core.geometry.MultiPoint
				();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 10);
			polygon1.lineTo(10, 10);
			polygon1.lineTo(10, 0);
			multipoint2.add(5, 0);
			multipoint2.add(5, 10);
			multipoint2.add(5, 5);
			bool res = within.execute(polygon1, multipoint2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = within.execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.add(5, 11);
			res = within.execute(multipoint2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonPolylineCrosses()
		{
			com.esri.core.geometry.OperatorCrosses crosses = (com.esri.core.geometry.OperatorCrosses
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Crosses));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			com.esri.core.geometry.Polygon polygon1 = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.Polyline polyline2 = new com.esri.core.geometry.Polyline();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 10);
			polygon1.lineTo(10, 10);
			polygon1.lineTo(10, 0);
			polyline2.startPath(5, -5);
			polyline2.lineTo(5, 15);
			bool res = crosses.execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = crosses.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline2.setEmpty();
			polyline2.startPath(5, 0);
			polyline2.lineTo(5, 10);
			res = crosses.execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = crosses.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polygon1.setEmpty();
			polyline2.setEmpty();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 10);
			polygon1.lineTo(10, 10);
			polygon1.lineTo(10, 8);
			polygon1.lineTo(15, 5);
			polygon1.lineTo(10, 2);
			polygon1.lineTo(10, 0);
			polyline2.startPath(10, 15);
			polyline2.lineTo(10, -5);
			res = crosses.execute(polygon1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = crosses.execute(polyline2, polygon1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolylinePolylineCrosses()
		{
			com.esri.core.geometry.OperatorCrosses crosses = (com.esri.core.geometry.OperatorCrosses
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Crosses));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(102100);
			double tolerance = sr.getTolerance(com.esri.core.geometry.VertexDescription.Semantics
				.POSITION);
			// Polyline1 and Polyline2 touch at a point
			string str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			string str2 = "{\"paths\":[[[10,10],[10,15],[15,15],[15,10]]]}";
			com.esri.core.geometry.Polyline polyline1 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			com.esri.core.geometry.Polyline polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			bool res = crosses.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = crosses.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			// Polyline1 and Polyline2 intersect at interiors
			str1 = "{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"paths\":[[[15,5],[5,15],[15,15],[15,5]]]}";
			polyline1 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = crosses.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = crosses.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			// Polyline1 and Polyline2 touch at an endpoint of Polyline1 and
			// interior of Polyline2 (but Polyline1 is closed)
			str1 = "{\"paths\":[[[10,10],[10,0],[0,0],[0,10],[10,10]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"paths\":[[[15,5],[5,15],[15,15]]]}";
			polyline1 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = crosses.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = crosses.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			// Polyline1 and Polyline2 touch at an endpoint of Polyline1 and
			// interior of Polyline2 (same as previous case, but Polyline1 is not
			// closed)
			str1 = "{\"paths\":[[[10,10],[10,0],[0,0],[0,10]],[[1,1],[9,1],[9,9],[1,9],[1,1],[1,1]]]}";
			str2 = "{\"paths\":[[[15,5],[5,15],[15,15]]]}";
			polyline1 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = crosses.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = crosses.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(!res);
			str1 = "{\"paths\":[[[10,11],[10,0],[0,0],[0,10]],[[1,1],[9,1],[9,9],[1,9],[6, 9]]]}";
			str2 = "{\"paths\":[[[15,5],[5,15],[15,15],[15,5]]]}";
			polyline1 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str1).getGeometry());
			polyline2 = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
				.fromJson(str2).getGeometry());
			res = crosses.execute(polyline1, polyline2, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = crosses.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(-2, -2);
			polyline1.lineTo(-1, -1);
			polyline1.lineTo(1, 1);
			polyline1.lineTo(2, 2);
			polyline2.startPath(-2, 2);
			polyline2.lineTo(-1, 1);
			polyline2.lineTo(1, -1);
			polyline2.lineTo(2, -2);
			res = crosses.execute(polyline2, polyline1, sr, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonEnvelope()
		{
			com.esri.core.geometry.OperatorEquals equals = (com.esri.core.geometry.OperatorEquals
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Equals));
			com.esri.core.geometry.OperatorContains contains = (com.esri.core.geometry.OperatorContains
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Contains));
			com.esri.core.geometry.OperatorDisjoint disjoint = (com.esri.core.geometry.OperatorDisjoint
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Disjoint));
			com.esri.core.geometry.OperatorCrosses crosses = (com.esri.core.geometry.OperatorCrosses
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Crosses));
			com.esri.core.geometry.OperatorWithin within = (com.esri.core.geometry.OperatorWithin
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Within));
			com.esri.core.geometry.OperatorOverlaps overlaps = (com.esri.core.geometry.OperatorOverlaps
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Overlaps));
			com.esri.core.geometry.OperatorTouches touches = (com.esri.core.geometry.OperatorTouches
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Touches));
			com.esri.core.geometry.OperatorDensifyByLength densify = (com.esri.core.geometry.OperatorDensifyByLength
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.DensifyByLength));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").getGeometry
					());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(equals.execute(envelope, densified, sr, null));
				// they
				// cover
				// the
				// same
				// space
				NUnit.Framework.Assert.IsTrue(contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").getGeometry
					());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":5,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				// the
				// polygon
				// contains
				// the
				// envelope,
				// but
				// they
				// aren't
				// equal
				NUnit.Framework.Assert.IsTrue(contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").getGeometry
					());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":15,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				// the
				// envelope
				// sticks
				// outside
				// of
				// the
				// polygon
				// but
				// they
				// intersect
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").getGeometry
					());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":5,\"ymin\":0,\"xmax\":15,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				// the
				// envelope
				// sticks
				// outside
				// of
				// the
				// polygon
				// but
				// they
				// intersect
				// and
				// overlap
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").getGeometry
					());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":10,\"ymin\":0,\"xmax\":15,\"ymax\":5}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				// the
				// envelope
				// rides
				// the
				// side
				// of
				// the
				// polygon
				// (they
				// touch)
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").getGeometry
					());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(contains.execute(densified, envelope, sr, null));
				// polygon
				// and
				// envelope
				// cover
				// the
				// same
				// space
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,0],[0,0]]]}").getGeometry());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				// envelope
				// sticks
				// outside
				// of
				// polygon,
				// but
				// the
				// envelopes
				// are
				// equal
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,0],[0,0]]]}").getGeometry());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":15,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				// the
				// polygon
				// envelope
				// doesn't
				// contain
				// the
				// envelope,
				// but
				// they
				// intersect
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").getGeometry
					());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":0,\"ymax\":0}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				// envelope
				// degenerate
				// to
				// a
				// point
				// and
				// is
				// on
				// border
				// (i.e.
				// touches)
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").getGeometry
					());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":1,\"ymin\":1,\"xmax\":1,\"ymax\":1}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(densified, envelope, sr, null));
				// envelope
				// degenerate
				// to
				// a
				// point
				// and
				// is
				// properly
				// inside
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").getGeometry
					());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":-1,\"ymin\":-1,\"xmax\":-1,\"ymax\":-1}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				// envelope
				// degenerate
				// to
				// a
				// point
				// and
				// is
				// properly
				// outside
				NUnit.Framework.Assert.IsTrue(disjoint.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").getGeometry
					());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":1,\"ymax\":0}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				// envelope
				// degenerate
				// to
				// a
				// line
				// and
				// rides
				// the
				// bottom
				// of
				// the
				// polygon
				// (no
				// interior
				// intersection)
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,10],[10,0],[0,0]]]}").getGeometry
					());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":1,\"xmax\":1,\"ymax\":1}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(densified, envelope, sr, null));
				// envelope
				// degenerate
				// to
				// a
				// line,
				// touches
				// the
				// border
				// on
				// the
				// inside
				// yet
				// has
				// interior
				// intersection
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,0],[0,0]]]}").getGeometry());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":5,\"ymin\":5,\"xmax\":6,\"ymax\":5}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				// envelope
				// degenerate
				// to
				// a
				// line,
				// touches
				// the
				// boundary,
				// and
				// is
				// outside
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,0],[0,0]]]}").getGeometry());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":6,\"ymin\":5,\"xmax\":7,\"ymax\":5}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				// envelope
				// degenerate
				// to
				// a
				// line,
				// and
				// is
				// outside
				NUnit.Framework.Assert.IsTrue(disjoint.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polygon polygon = (com.esri.core.geometry.Polygon)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"rings\":[[[0,0],[0,5],[0,10],[10,0],[0,0]]]}").getGeometry());
				com.esri.core.geometry.Polygon densified = (com.esri.core.geometry.Polygon)(densify
					.execute(polygon, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":4,\"ymin\":5,\"xmax\":7,\"ymax\":5}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				// envelope
				// degenerate
				// to
				// a
				// line,
				// and
				// crosses
				// polygon
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(crosses.execute(envelope, densified, sr, null));
			}
		}

		[NUnit.Framework.Test]
		public virtual void testPolylineEnvelope()
		{
			com.esri.core.geometry.OperatorEquals equals = (com.esri.core.geometry.OperatorEquals
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Equals));
			com.esri.core.geometry.OperatorContains contains = (com.esri.core.geometry.OperatorContains
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Contains));
			com.esri.core.geometry.OperatorDisjoint disjoint = (com.esri.core.geometry.OperatorDisjoint
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Disjoint));
			com.esri.core.geometry.OperatorCrosses crosses = (com.esri.core.geometry.OperatorCrosses
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Crosses));
			com.esri.core.geometry.OperatorWithin within = (com.esri.core.geometry.OperatorWithin
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Within));
			com.esri.core.geometry.OperatorOverlaps overlaps = (com.esri.core.geometry.OperatorOverlaps
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Overlaps));
			com.esri.core.geometry.OperatorTouches touches = (com.esri.core.geometry.OperatorTouches
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Touches));
			com.esri.core.geometry.OperatorDensifyByLength densify = (com.esri.core.geometry.OperatorDensifyByLength
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.DensifyByLength));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				// polyline
				// straddles
				// the
				// envelope
				// like
				// a hat
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[-10,0],[0,10]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[-11,0],[1,12]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[5,5],[6,6]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				// polyline
				// properly
				// inside
				NUnit.Framework.Assert.IsTrue(contains.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[5,5],[10,10]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[-5,5],[15,5]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(crosses.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(crosses.execute(densified, envelope, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[5,5],[5,15]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				// polyline
				// slices
				// through
				// the
				// envelope
				// (interior
				// and
				// exterior
				// intersection)
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[5,11],[5,15]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				// polyline
				// outside
				// of
				// envelope
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":10,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				// polyline
				// straddles
				// the
				// degenerate
				// envelope
				// like
				// a hat
				NUnit.Framework.Assert.IsTrue(contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":-5,\"xmax\":0,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":0,\"ymax\":0}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 511);
				wiggleGeometry(envelope, 0.00000001, 1982);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				// degenerate
				// envelope
				// is at
				// the
				// end
				// point
				// of
				// polyline
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[0,0],[0,5],[0,10],[10,10],[10,0]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":5,\"xmax\":0,\"ymax\":5}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				// degenerate
				// envelope
				// is at
				// the
				// interior
				// of
				// polyline
				NUnit.Framework.Assert.IsTrue(contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[2,-2],[2,2]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":5,\"ymax\":0}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				// degenerate
				// envelope
				// crosses
				// polyline
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[2,0],[2,2]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":5,\"ymax\":0}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				// degenerate
				// envelope
				// crosses
				// polyline
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[2,0],[2,2]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":2,\"ymin\":0,\"xmax\":2,\"ymax\":3}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				// degenerate
				// envelope
				// contains
				// polyline
				NUnit.Framework.Assert.IsTrue(!contains.execute(densified, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[5,5],[6,6]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":5,\"ymax\":5}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, densified, sr, null));
				// polyline
				// properly
				// inside
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
			{
				com.esri.core.geometry.Polyline polyline = (com.esri.core.geometry.Polyline)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"paths\":[[[5,5],[5,10]]]}").getGeometry());
				com.esri.core.geometry.Polyline densified = (com.esri.core.geometry.Polyline)(densify
					.execute(polyline, 1.0, null));
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":5,\"ymin\":5,\"xmax\":5,\"ymax\":10}").getGeometry());
				wiggleGeometry(densified, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(equals.execute(envelope, densified, sr, null));
				// polyline
				// properly
				// inside
				NUnit.Framework.Assert.IsTrue(contains.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, densified, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, densified, sr, null));
			}
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPointEnvelope()
		{
			com.esri.core.geometry.OperatorEquals equals = (com.esri.core.geometry.OperatorEquals
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Equals));
			com.esri.core.geometry.OperatorContains contains = (com.esri.core.geometry.OperatorContains
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Contains));
			com.esri.core.geometry.OperatorDisjoint disjoint = (com.esri.core.geometry.OperatorDisjoint
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Disjoint));
			com.esri.core.geometry.OperatorCrosses crosses = (com.esri.core.geometry.OperatorCrosses
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Crosses));
			com.esri.core.geometry.OperatorWithin within = (com.esri.core.geometry.OperatorWithin
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Within));
			com.esri.core.geometry.OperatorOverlaps overlaps = (com.esri.core.geometry.OperatorOverlaps
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Overlaps));
			com.esri.core.geometry.OperatorTouches touches = (com.esri.core.geometry.OperatorTouches
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Touches));
			com.esri.core.geometry.OperatorDensifyByLength densify = (com.esri.core.geometry.OperatorDensifyByLength
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.DensifyByLength));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			{
				com.esri.core.geometry.MultiPoint multi_point = (com.esri.core.geometry.MultiPoint
					)(com.esri.core.geometry.TestCommonMethods.fromJson("{\"points\":[[0,0],[0,10],[10,10],[10,0]]}"
					).getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(multi_point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, multi_point, sr, null));
				// all
				// points
				// on
				// boundary
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, multi_point, sr, null));
			}
			{
				com.esri.core.geometry.MultiPoint multi_point = (com.esri.core.geometry.MultiPoint
					)(com.esri.core.geometry.TestCommonMethods.fromJson("{\"points\":[[0,0],[0,10],[10,10],[5,5]]}"
					).getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(multi_point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, multi_point, sr, null));
				// points
				// on
				// boundary
				// and
				// one
				// point
				// in
				// interior
				NUnit.Framework.Assert.IsTrue(contains.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, multi_point, sr, null));
			}
			{
				com.esri.core.geometry.MultiPoint multi_point = (com.esri.core.geometry.MultiPoint
					)(com.esri.core.geometry.TestCommonMethods.fromJson("{\"points\":[[0,0],[0,10],[10,10],[5,5],[15,15]]}"
					).getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(multi_point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, multi_point, sr, null));
				// points
				// on
				// boundary,
				// one
				// interior,
				// one
				// exterior
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(crosses.execute(envelope, multi_point, sr, null));
			}
			{
				com.esri.core.geometry.MultiPoint multi_point = (com.esri.core.geometry.MultiPoint
					)(com.esri.core.geometry.TestCommonMethods.fromJson("{\"points\":[[0,0],[0,10],[10,10],[15,15]]}"
					).getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(multi_point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, multi_point, sr, null));
				// points
				// on
				// boundary,
				// one
				// exterior
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, multi_point, sr, null));
			}
			{
				com.esri.core.geometry.MultiPoint multi_point = (com.esri.core.geometry.MultiPoint
					)(com.esri.core.geometry.TestCommonMethods.fromJson("{\"points\":[[0,-1],[0,11],[11,11],[15,15]]}"
					).getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(multi_point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, multi_point, sr, null));
				// all
				// points
				// exterior
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(disjoint.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, multi_point, sr, null));
			}
			{
				com.esri.core.geometry.MultiPoint multi_point = (com.esri.core.geometry.MultiPoint
					)(com.esri.core.geometry.TestCommonMethods.fromJson("{\"points\":[[0,0],[0,10],[10,10],[10,0]]}"
					).getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":10,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(multi_point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, multi_point, sr, null));
				// degenerate
				// envelope
				// slices
				// through
				// some
				// points,
				// but
				// some
				// points
				// are
				// off
				// the
				// line
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, multi_point, sr, null));
			}
			{
				com.esri.core.geometry.MultiPoint multi_point = (com.esri.core.geometry.MultiPoint
					)(com.esri.core.geometry.TestCommonMethods.fromJson("{\"points\":[[0,0],[1,10],[10,10],[10,0]]}"
					).getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":10,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(multi_point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, multi_point, sr, null));
				// degenerate
				// envelope
				// slices
				// through
				// some
				// points,
				// but
				// some
				// points
				// are
				// off
				// the
				// line
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(crosses.execute(envelope, multi_point, sr, null));
			}
			{
				com.esri.core.geometry.MultiPoint multi_point = (com.esri.core.geometry.MultiPoint
					)(com.esri.core.geometry.TestCommonMethods.fromJson("{\"points\":[[0,10],[10,10]]}"
					).getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":10,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(multi_point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, multi_point, sr, null));
				// degenerate
				// envelopes
				// slices
				// through
				// all
				// the
				// points,
				// and
				// they
				// are
				// at
				// the
				// end
				// points
				// of
				// the
				// line
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, multi_point, sr, null));
			}
			{
				com.esri.core.geometry.MultiPoint multi_point = (com.esri.core.geometry.MultiPoint
					)(com.esri.core.geometry.TestCommonMethods.fromJson("{\"points\":[[1,10],[9,10]]}"
					).getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":10,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(multi_point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, multi_point, sr, null));
				// degenerate
				// envelopes
				// slices
				// through
				// all
				// the
				// points,
				// and
				// they
				// are
				// in
				// the
				// interior
				// of
				// the
				// line
				NUnit.Framework.Assert.IsTrue(contains.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, multi_point, sr, null));
			}
			{
				com.esri.core.geometry.MultiPoint multi_point = (com.esri.core.geometry.MultiPoint
					)(com.esri.core.geometry.TestCommonMethods.fromJson("{\"points\":[[0,-1],[0,11],[11,11],[15,15]]}"
					).getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":10,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(multi_point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, multi_point, sr, null));
				// all
				// points
				// exterior
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(disjoint.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, multi_point, sr, null));
			}
			{
				com.esri.core.geometry.MultiPoint multi_point = (com.esri.core.geometry.MultiPoint
					)(com.esri.core.geometry.TestCommonMethods.fromJson("{\"points\":[[0,-1],[0,11],[11,11],[15,15]]}"
					).getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":10,\"ymin\":10,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(multi_point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, multi_point, sr, null));
				// all
				// points
				// exterior
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(disjoint.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, multi_point, sr, null));
			}
			{
				com.esri.core.geometry.MultiPoint multi_point = (com.esri.core.geometry.MultiPoint
					)(com.esri.core.geometry.TestCommonMethods.fromJson("{\"points\":[[0,-1],[0,11],[11,11],[15,15]]}"
					).getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":11,\"ymax\":11}").getGeometry());
				wiggleGeometry(multi_point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, multi_point, sr, null));
				// all
				// points
				// exterior
				NUnit.Framework.Assert.IsTrue(!contains.execute(multi_point, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, multi_point, sr, null));
			}
			{
				com.esri.core.geometry.MultiPoint multi_point = (com.esri.core.geometry.MultiPoint
					)(com.esri.core.geometry.TestCommonMethods.fromJson("{\"points\":[[0,-1],[0,-1]]}"
					).getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":-1,\"xmax\":0,\"ymax\":-1}").getGeometry());
				wiggleGeometry(multi_point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(equals.execute(envelope, multi_point, sr, null));
				// all
				// points
				// exterior
				NUnit.Framework.Assert.IsTrue(contains.execute(multi_point, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, multi_point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, multi_point, sr, null));
			}
		}

		[NUnit.Framework.Test]
		public virtual void testPointEnvelope()
		{
			com.esri.core.geometry.OperatorEquals equals = (com.esri.core.geometry.OperatorEquals
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Equals));
			com.esri.core.geometry.OperatorContains contains = (com.esri.core.geometry.OperatorContains
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Contains));
			com.esri.core.geometry.OperatorDisjoint disjoint = (com.esri.core.geometry.OperatorDisjoint
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Disjoint));
			com.esri.core.geometry.OperatorCrosses crosses = (com.esri.core.geometry.OperatorCrosses
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Crosses));
			com.esri.core.geometry.OperatorWithin within = (com.esri.core.geometry.OperatorWithin
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Within));
			com.esri.core.geometry.OperatorOverlaps overlaps = (com.esri.core.geometry.OperatorOverlaps
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Overlaps));
			com.esri.core.geometry.OperatorTouches touches = (com.esri.core.geometry.OperatorTouches
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Touches));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			{
				com.esri.core.geometry.Point point = (com.esri.core.geometry.Point)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"x\":5,\"y\":6}").getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(point, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, point, sr, null));
			}
			{
				com.esri.core.geometry.Point point = (com.esri.core.geometry.Point)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"x\":0,\"y\":10}").getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(point, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, point, sr, null));
			}
			{
				com.esri.core.geometry.Point point = (com.esri.core.geometry.Point)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"x\":0,\"y\":11}").getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(point, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(disjoint.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, point, sr, null));
			}
			{
				com.esri.core.geometry.Point point = (com.esri.core.geometry.Point)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"x\":0,\"y\":0}").getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").getGeometry());
				wiggleGeometry(point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(point, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, point, sr, null));
			}
			{
				com.esri.core.geometry.Point point = (com.esri.core.geometry.Point)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"x\":5,\"y\":0}").getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").getGeometry());
				wiggleGeometry(point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(point, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, point, sr, null));
			}
			{
				com.esri.core.geometry.Point point = (com.esri.core.geometry.Point)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"x\":11,\"y\":0}").getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").getGeometry());
				wiggleGeometry(point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(point, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(disjoint.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, point, sr, null));
			}
			{
				com.esri.core.geometry.Point point = (com.esri.core.geometry.Point)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"x\":0,\"y\":0}").getGeometry());
				com.esri.core.geometry.Envelope envelope = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":0,\"ymax\":0}").getGeometry());
				wiggleGeometry(point, 0.00000001, 1982);
				wiggleGeometry(envelope, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(equals.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(point, envelope, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(envelope, point, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(envelope, point, sr, null));
			}
		}

		[NUnit.Framework.Test]
		public virtual void testEnvelopeEnvelope()
		{
			com.esri.core.geometry.OperatorEquals equals = (com.esri.core.geometry.OperatorEquals
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Equals));
			com.esri.core.geometry.OperatorContains contains = (com.esri.core.geometry.OperatorContains
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Contains));
			com.esri.core.geometry.OperatorDisjoint disjoint = (com.esri.core.geometry.OperatorDisjoint
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Disjoint));
			com.esri.core.geometry.OperatorCrosses crosses = (com.esri.core.geometry.OperatorCrosses
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Crosses));
			com.esri.core.geometry.OperatorWithin within = (com.esri.core.geometry.OperatorWithin
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Within));
			com.esri.core.geometry.OperatorOverlaps overlaps = (com.esri.core.geometry.OperatorOverlaps
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Overlaps));
			com.esri.core.geometry.OperatorTouches touches = (com.esri.core.geometry.OperatorTouches
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Touches));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":5,\"ymin\":5,\"xmax\":10,\"ymax\":10}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":5,\"ymin\":5,\"xmax\":15,\"ymax\":15}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":10,\"ymin\":0,\"xmax\":20,\"ymax\":10}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":10,\"ymin\":0,\"xmax\":20,\"ymax\":10}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":10,\"ymin\":0,\"xmax\":20,\"ymax\":10}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":10,\"xmax\":10,\"ymax\":20}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":15,\"ymax\":0}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":-5,\"ymin\":5,\"xmax\":0,\"ymax\":5}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":-5,\"ymin\":5,\"xmax\":5,\"ymax\":5}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":3,\"ymin\":5,\"xmax\":5,\"ymax\":5}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":3,\"ymin\":5,\"xmax\":10,\"ymax\":5}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":-5,\"ymin\":0,\"xmax\":0,\"ymax\":0}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":15,\"ymax\":0}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":15,\"ymax\":0}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":-5,\"ymin\":0,\"xmax\":10,\"ymax\":0}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":5,\"ymax\":0}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":-5,\"ymin\":0,\"xmax\":0,\"ymax\":0}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":5,\"ymin\":-5,\"xmax\":5,\"ymax\":5}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":10,\"ymin\":0,\"xmax\":20,\"ymax\":0}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":5,\"ymin\":0,\"xmax\":5,\"ymax\":5}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":0,\"ymax\":0}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":5,\"ymin\":0,\"xmax\":5,\"ymax\":0}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":5,\"ymin\":5,\"xmax\":5,\"ymax\":5}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":10}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":0,\"ymax\":0}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(touches.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":5,\"ymin\":0,\"xmax\":5,\"ymax\":0}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":0,\"ymin\":0,\"xmax\":10,\"ymax\":0}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(!equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
			{
				com.esri.core.geometry.Envelope env1 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":5,\"ymin\":0,\"xmax\":5,\"ymax\":0}").getGeometry());
				com.esri.core.geometry.Envelope env2 = (com.esri.core.geometry.Envelope)(com.esri.core.geometry.TestCommonMethods
					.fromJson("{\"xmin\":5,\"ymin\":0,\"xmax\":5,\"ymax\":0}").getGeometry());
				wiggleGeometry(env1, 0.00000001, 1982);
				wiggleGeometry(env2, 0.00000001, 511);
				NUnit.Framework.Assert.IsTrue(equals.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(contains.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!disjoint.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!touches.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!overlaps.execute(env2, env1, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env1, env2, sr, null));
				NUnit.Framework.Assert.IsTrue(!crosses.execute(env2, env1, sr, null));
			}
		}

		internal static void wiggleGeometry(com.esri.core.geometry.Geometry geometry, double
			 tolerance, int rand)
		{
			int type = geometry.getType().value();
			if (type == com.esri.core.geometry.Geometry.GeometryType.Polygon || type == com.esri.core.geometry.Geometry.GeometryType
				.Polyline || type == com.esri.core.geometry.Geometry.GeometryType.MultiPoint)
			{
				com.esri.core.geometry.MultiVertexGeometry mvGeom = (com.esri.core.geometry.MultiVertexGeometry
					)geometry;
				for (int i = 0; i < mvGeom.getPointCount(); i++)
				{
					com.esri.core.geometry.Point2D pt = mvGeom.getXY(i);
					// create random vector and normalize it to 0.49 * tolerance
					com.esri.core.geometry.Point2D randomV = new com.esri.core.geometry.Point2D();
					rand = com.esri.core.geometry.NumberUtils.nextRand(rand);
					randomV.x = 1.0 * rand / com.esri.core.geometry.NumberUtils.intMax() - 0.5;
					rand = com.esri.core.geometry.NumberUtils.nextRand(rand);
					randomV.y = 1.0 * rand / com.esri.core.geometry.NumberUtils.intMax() - 0.5;
					randomV.normalize();
					randomV.scale(0.45 * tolerance);
					pt.add(randomV);
					mvGeom.setXY(i, pt);
				}
			}
			else
			{
				if (type == com.esri.core.geometry.Geometry.GeometryType.Point)
				{
					com.esri.core.geometry.Point ptGeom = (com.esri.core.geometry.Point)(geometry);
					com.esri.core.geometry.Point2D pt = ptGeom.getXY();
					// create random vector and normalize it to 0.49 * tolerance
					com.esri.core.geometry.Point2D randomV = new com.esri.core.geometry.Point2D();
					rand = com.esri.core.geometry.NumberUtils.nextRand(rand);
					randomV.x = 1.0 * rand / com.esri.core.geometry.NumberUtils.intMax() - 0.5;
					rand = com.esri.core.geometry.NumberUtils.nextRand(rand);
					randomV.y = 1.0 * rand / com.esri.core.geometry.NumberUtils.intMax() - 0.5;
					randomV.normalize();
					randomV.scale(0.45 * tolerance);
					pt.add(randomV);
					ptGeom.setXY(pt);
				}
				else
				{
					if (type == com.esri.core.geometry.Geometry.GeometryType.Envelope)
					{
						com.esri.core.geometry.Envelope envGeom = (com.esri.core.geometry.Envelope)(geometry
							);
						com.esri.core.geometry.Envelope2D env = new com.esri.core.geometry.Envelope2D();
						envGeom.queryEnvelope2D(env);
						double xmin;
						double xmax;
						double ymin;
						double ymax;
						com.esri.core.geometry.Point2D pt = new com.esri.core.geometry.Point2D();
						env.queryLowerLeft(pt);
						// create random vector and normalize it to 0.49 * tolerance
						com.esri.core.geometry.Point2D randomV = new com.esri.core.geometry.Point2D();
						rand = com.esri.core.geometry.NumberUtils.nextRand(rand);
						randomV.x = 1.0 * rand / com.esri.core.geometry.NumberUtils.intMax() - 0.5;
						rand = com.esri.core.geometry.NumberUtils.nextRand(rand);
						randomV.y = 1.0 * rand / com.esri.core.geometry.NumberUtils.intMax() - 0.5;
						randomV.normalize();
						randomV.scale(0.45 * tolerance);
						xmin = (pt.x + randomV.x);
						ymin = (pt.y + randomV.y);
						env.queryUpperRight(pt);
						// create random vector and normalize it to 0.49 * tolerance
						rand = com.esri.core.geometry.NumberUtils.nextRand(rand);
						randomV.x = 1.0 * rand / com.esri.core.geometry.NumberUtils.intMax() - 0.5;
						rand = com.esri.core.geometry.NumberUtils.nextRand(rand);
						randomV.y = 1.0 * rand / com.esri.core.geometry.NumberUtils.intMax() - 0.5;
						randomV.normalize();
						randomV.scale(0.45 * tolerance);
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
						envGeom.setCoords(xmin, ymin, xmax, ymax);
					}
				}
			}
		}

		[NUnit.Framework.Test]
		public virtual void testDisjointRelationFalse()
		{
			{
				com.esri.core.geometry.OperatorDisjoint op = (com.esri.core.geometry.OperatorDisjoint
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Disjoint));
				com.esri.core.geometry.Envelope env1 = new com.esri.core.geometry.Envelope(50, 50
					, 150, 150);
				com.esri.core.geometry.Envelope env2 = new com.esri.core.geometry.Envelope(25, 25
					, 175, 175);
				bool result = op.execute(env1, env2, com.esri.core.geometry.SpatialReference.create
					(4326), null);
				NUnit.Framework.Assert.IsTrue(!result);
			}
			{
				com.esri.core.geometry.OperatorIntersects op = (com.esri.core.geometry.OperatorIntersects
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Intersects));
				com.esri.core.geometry.Envelope env1 = new com.esri.core.geometry.Envelope(50, 50
					, 150, 150);
				com.esri.core.geometry.Envelope env2 = new com.esri.core.geometry.Envelope(25, 25
					, 175, 175);
				bool result = op.execute(env1, env2, com.esri.core.geometry.SpatialReference.create
					(4326), null);
				NUnit.Framework.Assert.IsTrue(result);
			}
			{
				com.esri.core.geometry.OperatorContains op = (com.esri.core.geometry.OperatorContains
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Contains));
				com.esri.core.geometry.Envelope env1 = new com.esri.core.geometry.Envelope(100, 175
					, 200, 225);
				com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
				polyline.startPath(200, 175);
				polyline.lineTo(200, 225);
				polyline.lineTo(125, 200);
				bool result = op.execute(env1, polyline, com.esri.core.geometry.SpatialReference.
					create(4326), null);
				NUnit.Framework.Assert.IsTrue(result);
			}
			{
				com.esri.core.geometry.OperatorTouches op = (com.esri.core.geometry.OperatorTouches
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Touches));
				com.esri.core.geometry.Envelope env1 = new com.esri.core.geometry.Envelope(100, 200
					, 400, 400);
				com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
				polyline.startPath(300, 60);
				polyline.lineTo(300, 200);
				polyline.lineTo(400, 50);
				bool result = op.execute(env1, polyline, com.esri.core.geometry.SpatialReference.
					create(4326), null);
				NUnit.Framework.Assert.IsTrue(result);
			}
			{
				com.esri.core.geometry.OperatorTouches op = (com.esri.core.geometry.OperatorTouches
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Touches));
				com.esri.core.geometry.Envelope env1 = new com.esri.core.geometry.Envelope(50, 50
					, 150, 150);
				com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
				polyline.startPath(100, 20);
				polyline.lineTo(100, 50);
				polyline.lineTo(150, 10);
				bool result = op.execute(polyline, env1, com.esri.core.geometry.SpatialReference.
					create(4326), null);
				NUnit.Framework.Assert.IsTrue(result);
			}
			{
				com.esri.core.geometry.OperatorDisjoint op = (com.esri.core.geometry.OperatorDisjoint
					)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Disjoint));
				com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
				com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
				polygon.startPath(0, 0);
				polygon.lineTo(0, 10);
				polygon.lineTo(10, 10);
				polygon.lineTo(10, 0);
				polyline.startPath(-5, 4);
				polyline.lineTo(5, -6);
				bool result = op.execute(polyline, polygon, com.esri.core.geometry.SpatialReference
					.create(4326), null);
				NUnit.Framework.Assert.IsTrue(result);
			}
		}

		[NUnit.Framework.Test]
		public virtual void testPolylinePolylineRelate()
		{
			com.esri.core.geometry.OperatorRelate op = com.esri.core.geometry.OperatorRelate.
				local();
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool res;
			string scl;
			com.esri.core.geometry.Polyline polyline1 = new com.esri.core.geometry.Polyline();
			com.esri.core.geometry.Polyline polyline2 = new com.esri.core.geometry.Polyline();
			polyline1.startPath(0, 0);
			polyline1.lineTo(1, 1);
			polyline2.startPath(1, 1);
			polyline2.lineTo(2, 0);
			scl = "FF1FT01T2";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "****TF*T*";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "****F****";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "**1*0*T**";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "****1****";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "**T*001*T";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "T********";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "F********";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(0, 0);
			polyline1.lineTo(1, 0);
			polyline2.startPath(0, 0);
			polyline2.lineTo(1, 0);
			scl = "1FFFTFFFT";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "1*T*T****";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "1T**T****";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(0, 0);
			polyline1.lineTo(0.5, 0.5);
			polyline1.lineTo(1, 1);
			polyline2.startPath(1, 0);
			polyline2.lineTo(0.5, 0.5);
			polyline2.lineTo(0, 1);
			scl = "0F1FFTT0T";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "*T*******";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "*F*F*****";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(0, 0);
			polyline1.lineTo(1, 0);
			polyline2.startPath(1, -1);
			polyline2.lineTo(1, 1);
			scl = "FT1TF01TT";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "***T*****";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(0, 0);
			polyline1.lineTo(0, 20);
			polyline1.lineTo(20, 20);
			polyline1.lineTo(20, 0);
			polyline1.lineTo(0, 0);
			// has no boundary
			polyline2.startPath(3, 3);
			polyline2.lineTo(5, 5);
			op.accelerateGeometry(polyline1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			scl = "FF1FFF102";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(4, 0);
			polyline1.lineTo(0, 4);
			polyline1.lineTo(4, 8);
			polyline1.lineTo(8, 4);
			polyline2.startPath(8, 1);
			polyline2.lineTo(8, 2);
			op.accelerateGeometry(polyline1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			scl = "FF1FF0102";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(4, 0);
			polyline1.lineTo(0, 4);
			polyline2.startPath(3, 2);
			polyline2.lineTo(3, 2);
			NUnit.Framework.Assert.IsTrue(polyline2.getBoundary().isEmpty());
			scl = "******0F*";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline2.lineTo(3, 2);
			NUnit.Framework.Assert.IsTrue(polyline2.getBoundary().isEmpty());
			scl = "******0F*";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "******0F*";
			polyline2.lineTo(3, 2);
			NUnit.Framework.Assert.IsTrue(polyline2.getBoundary().isEmpty());
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(3, 3);
			polyline1.lineTo(3, 4);
			polyline1.lineTo(3, 3);
			polyline2.startPath(1, 1);
			polyline2.lineTo(1, 1);
			scl = "FF1FFF0F2";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "FF0FFF1F2";
			res = op.execute(polyline2, polyline1, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.setEmpty();
			polyline2.setEmpty();
			polyline1.startPath(4, 0);
			polyline1.lineTo(0, 4);
			polyline2.startPath(2, 2);
			polyline2.lineTo(2, 2);
			scl = "0F*******";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline2.lineTo(2, 2);
			scl = "0F*******";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "0F*******";
			res = op.execute(polyline1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonPolylineRelate()
		{
			com.esri.core.geometry.OperatorRelate op = com.esri.core.geometry.OperatorRelate.
				local();
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool res;
			string scl;
			com.esri.core.geometry.Polygon polygon1 = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.Polyline polyline2 = new com.esri.core.geometry.Polyline();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 10);
			polygon1.lineTo(10, 10);
			polygon1.lineTo(10, 0);
			polyline2.startPath(-10, 0);
			polyline2.lineTo(0, 0);
			scl = "FF2F01102";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "**1*0110*";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "T***T****";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "FF2FT****";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline2.setEmpty();
			polyline2.startPath(0, 0);
			polyline2.lineTo(10, 0);
			scl = "**21*1FF*";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "F*21*1FF*";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "0**1*1FF*";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			scl = "F**1*1TF*";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline2.setEmpty();
			polyline2.startPath(1, 1);
			polyline2.lineTo(5, 5);
			scl = "TT2******";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "1T2FF1FF*";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "1T1FF1FF*";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline2.setEmpty();
			polyline2.startPath(5, 5);
			polyline2.lineTo(15, 5);
			scl = "1T20F*T0T";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon1.setEmpty();
			polyline2.setEmpty();
			polygon1.startPath(2, 0);
			polygon1.lineTo(0, 2);
			polygon1.lineTo(2, 4);
			polygon1.lineTo(4, 2);
			polyline2.startPath(1, 2);
			polyline2.lineTo(3, 2);
			op.accelerateGeometry(polygon1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			scl = "TTTFF****";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline2.setEmpty();
			polyline2.startPath(5, 2);
			polyline2.lineTo(7, 2);
			scl = "FF2FFT***";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon1.setEmpty();
			polyline2.setEmpty();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 1);
			polygon1.lineTo(1, 0);
			polyline2.startPath(0, 10);
			polyline2.lineTo(0, 9);
			polyline2.startPath(10, 0);
			polyline2.lineTo(9, 0);
			polyline2.startPath(0, -10);
			polyline2.lineTo(0, -9);
			scl = "**2******";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon1.setEmpty();
			polyline2.setEmpty();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 1);
			polygon1.lineTo(0, 0);
			polyline2.startPath(0, 10);
			polyline2.lineTo(0, 9);
			scl = "**1******";
			res = op.execute(polygon1, polyline2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonPolygonRelate()
		{
			com.esri.core.geometry.OperatorRelate op = (com.esri.core.geometry.OperatorRelate
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Relate));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool res;
			string scl;
			com.esri.core.geometry.Polygon polygon1 = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.Polygon polygon2 = new com.esri.core.geometry.Polygon();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 10);
			polygon1.lineTo(10, 10);
			polygon1.lineTo(10, 0);
			polygon2.startPath(15, 0);
			polygon2.lineTo(15, 10);
			polygon2.lineTo(25, 10);
			polygon2.lineTo(25, 0);
			scl = "FFTFFT21T";
			res = op.execute(polygon1, polygon2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "FFTFFT11T";
			res = op.execute(polygon1, polygon2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polygon2.setEmpty();
			polygon2.startPath(5, 0);
			polygon2.lineTo(5, 10);
			polygon2.lineTo(15, 10);
			polygon2.lineTo(15, 0);
			scl = "21TT1121T";
			res = op.execute(polygon1, polygon2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon2.setEmpty();
			polygon2.startPath(1, 1);
			polygon2.lineTo(1, 9);
			polygon2.lineTo(9, 9);
			polygon2.lineTo(9, 1);
			scl = "212FF1FFT";
			res = op.execute(polygon1, polygon2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon1.setEmpty();
			polygon2.setEmpty();
			polygon1.startPath(3, 3);
			polygon1.lineTo(3, 4);
			polygon1.lineTo(3, 3);
			polygon2.startPath(1, 1);
			polygon2.lineTo(1, 1);
			scl = "FF1FFF0F2";
			res = op.execute(polygon1, polygon2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "FF0FFF1F2";
			res = op.execute(polygon2, polygon1, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon1.setEmpty();
			polygon2.setEmpty();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 100);
			polygon1.lineTo(100, 100);
			polygon1.lineTo(100, 0);
			polygon2.startPath(50, 50);
			polygon2.lineTo(50, 50);
			polygon2.lineTo(50, 50);
			op.accelerateGeometry(polygon1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			scl = "0F2FF1FF2";
			res = op.execute(polygon1, polygon2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon2.lineTo(51, 50);
			scl = "1F2FF1FF2";
			res = op.execute(polygon1, polygon2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPointPointRelate()
		{
			com.esri.core.geometry.OperatorRelate op = (com.esri.core.geometry.OperatorRelate
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Relate));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool res;
			string scl;
			com.esri.core.geometry.MultiPoint m1 = new com.esri.core.geometry.MultiPoint();
			com.esri.core.geometry.Point p2 = new com.esri.core.geometry.Point();
			m1.add(0, 0);
			p2.setXY(0, 0);
			scl = "T*F***F**";
			res = op.execute(m1, p2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "T*T***F**";
			res = op.execute(m1, p2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			m1.add(1, 1);
			res = op.execute(m1, p2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			m1.setEmpty();
			m1.add(1, 1);
			m1.add(2, 2);
			scl = "FF0FFFTF2";
			res = op.execute(m1, p2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPointPointRelate()
		{
			com.esri.core.geometry.OperatorRelate op = (com.esri.core.geometry.OperatorRelate
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Relate));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool res;
			string scl;
			com.esri.core.geometry.Point p1 = new com.esri.core.geometry.Point();
			com.esri.core.geometry.Point p2 = new com.esri.core.geometry.Point();
			p1.setXY(0, 0);
			p2.setXY(0, 0);
			scl = "T********";
			res = op.execute(p1, p2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			p1.setXY(0, 0);
			p2.setXY(1, 0);
			res = op.execute(p1, p2, null, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			p1.setEmpty();
			p2.setEmpty();
			scl = "*********";
			res = op.execute(p1, p2, null, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "FFFFFFFFF";
			res = op.execute(p1, p2, null, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "FFFFFFFFT";
			res = op.execute(p1, p2, null, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonMultiPointRelate()
		{
			com.esri.core.geometry.OperatorRelate op = (com.esri.core.geometry.OperatorRelate
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Relate));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool res;
			string scl;
			com.esri.core.geometry.Polygon polygon1 = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.MultiPoint multipoint2 = new com.esri.core.geometry.MultiPoint
				();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 10);
			polygon1.lineTo(10, 10);
			polygon1.lineTo(10, 0);
			multipoint2.add(0, 0);
			multipoint2.add(5, 5);
			scl = "TFT0F1FFT";
			res = op.execute(polygon1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "T0FFFFT1T";
			// transpose of above
			res = op.execute(multipoint2, polygon1, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.add(11, 11);
			scl = "TFT0F10FT";
			res = op.execute(polygon1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.add(0, 5);
			scl = "TFT0F10FT";
			res = op.execute(polygon1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "TFF0F10FT";
			res = op.execute(polygon1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polygon1.setEmpty();
			multipoint2.setEmpty();
			polygon1.startPath(0, 0);
			polygon1.lineTo(0, 20);
			polygon1.lineTo(20, 20);
			polygon1.lineTo(20, 0);
			multipoint2.add(3, 3);
			multipoint2.add(5, 5);
			op.accelerateGeometry(polygon1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			scl = "TF2FF****";
			res = op.execute(polygon1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon1.setEmpty();
			multipoint2.setEmpty();
			polygon1.startPath(4, 0);
			polygon1.lineTo(0, 4);
			polygon1.lineTo(4, 8);
			polygon1.lineTo(8, 4);
			multipoint2.add(8, 1);
			multipoint2.add(8, 2);
			op.accelerateGeometry(polygon1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			scl = "FF2FF10F2";
			res = op.execute(polygon1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygonPointRelate()
		{
			com.esri.core.geometry.OperatorRelate op = (com.esri.core.geometry.OperatorRelate
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Relate));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool res;
			string scl;
			com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.Point point = new com.esri.core.geometry.Point();
			polygon.startPath(0, 0);
			polygon.lineTo(0, 10);
			polygon.lineTo(10, 10);
			polygon.lineTo(10, 0);
			point.setXY(0, 0);
			scl = "FF20FTFFT";
			res = op.execute(polygon, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon.setEmpty();
			polygon.startPath(0, 0);
			polygon.lineTo(0, 0);
			polygon.lineTo(0, 0);
			scl = "0FFFFFFF2";
			res = op.execute(polygon, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon.setEmpty();
			polygon.startPath(0, 0);
			polygon.lineTo(0, 1);
			polygon.lineTo(0, 0);
			scl = "0F1FFFFF2";
			res = op.execute(polygon, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			point.setXY(-1, 0);
			scl = "FF1FFF0F2";
			res = op.execute(polygon, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon.setEmpty();
			polygon.startPath(0, 0);
			polygon.lineTo(0, 10);
			polygon.lineTo(0, 0);
			scl = "FF1FFFTFT";
			res = op.execute(polygon, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polygon.setEmpty();
			polygon.startPath(0, 0);
			polygon.lineTo(0, 0);
			polygon.lineTo(0, 0);
			scl = "FF0FFF0F2";
			res = op.execute(polygon, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolylineMultiPointRelate()
		{
			com.esri.core.geometry.OperatorRelate op = com.esri.core.geometry.OperatorRelate.
				local();
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool res;
			string scl;
			com.esri.core.geometry.Polyline polyline1 = new com.esri.core.geometry.Polyline();
			com.esri.core.geometry.MultiPoint multipoint2 = new com.esri.core.geometry.MultiPoint
				();
			polyline1.startPath(0, 0);
			polyline1.lineTo(10, 0);
			multipoint2.add(0, 0);
			multipoint2.add(5, 5);
			scl = "FF10F00F2";
			res = op.execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.add(5, 0);
			scl = "0F10F00F2";
			res = op.execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "0F11F00F2";
			res = op.execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(!res);
			polyline1.setEmpty();
			multipoint2.setEmpty();
			polyline1.startPath(4, 0);
			polyline1.lineTo(0, 4);
			polyline1.lineTo(4, 8);
			polyline1.lineTo(8, 4);
			polyline1.lineTo(4, 0);
			// has no boundary
			multipoint2.add(8, 1);
			multipoint2.add(8, 2);
			op.accelerateGeometry(polyline1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			scl = "FF1FFF0F2";
			res = op.execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.setEmpty();
			multipoint2.setEmpty();
			polyline1.startPath(4, 0);
			polyline1.lineTo(4, 0);
			multipoint2.add(8, 1);
			multipoint2.add(8, 2);
			scl = "FF0FFF0F2";
			res = op.execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.add(-2, 0);
			res = op.execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			op.accelerateGeometry(polyline1, sr, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			res = op.execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.setEmpty();
			multipoint2.setEmpty();
			polyline1.startPath(10, 10);
			polyline1.lineTo(10, 10);
			multipoint2.add(10, 10);
			scl = "0FFFFFFF2";
			res = op.execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.startPath(12, 12);
			polyline1.lineTo(12, 12);
			scl = "0F0FFFFF2";
			res = op.execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline1.setEmpty();
			multipoint2.setEmpty();
			polyline1.startPath(10, 10);
			polyline1.lineTo(10, 10);
			multipoint2.add(0, 0);
			scl = "FF0FFF0F2";
			res = op.execute(polyline1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPointMultipointRelate()
		{
			com.esri.core.geometry.OperatorRelate op = (com.esri.core.geometry.OperatorRelate
				)(com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Relate));
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool res;
			string scl;
			com.esri.core.geometry.MultiPoint multipoint1 = new com.esri.core.geometry.MultiPoint
				();
			com.esri.core.geometry.MultiPoint multipoint2 = new com.esri.core.geometry.MultiPoint
				();
			multipoint1.add(0, 0);
			multipoint2.add(0, 0);
			scl = "TFFFFFFF2";
			res = op.execute(multipoint1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint2.add(5, 5);
			scl = "TFFFFFTF2";
			res = op.execute(multipoint1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint1.add(-5, 0);
			scl = "0FTFFFTF2";
			res = op.execute(multipoint1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			res = com.esri.core.geometry.GeometryEngine.relate(multipoint1, multipoint2, sr, 
				scl);
			NUnit.Framework.Assert.IsTrue(res);
			multipoint1.setEmpty();
			multipoint2.setEmpty();
			multipoint1.add(0, 0);
			multipoint2.add(1, 1);
			scl = "FFTFFF0FT";
			res = op.execute(multipoint1, multipoint2, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testPolylinePointRelate()
		{
			com.esri.core.geometry.OperatorRelate op = com.esri.core.geometry.OperatorRelate.
				local();
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			bool res;
			string scl;
			com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
			com.esri.core.geometry.Point point = new com.esri.core.geometry.Point();
			polyline.startPath(0, 2);
			polyline.lineTo(0, 4);
			point.setXY(0, 3);
			scl = "0F1FF0FF2";
			res = op.execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			point.setXY(1, 3);
			scl = "FF1FF00F2";
			res = op.execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline.lineTo(4, 4);
			polyline.lineTo(4, 2);
			polyline.lineTo(0, 2);
			// no bounadry
			point.setXY(0, 3);
			scl = "0F1FFFFF2";
			res = op.execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			scl = "0F1FFFFF2";
			res = op.execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			point.setXY(1, 3);
			scl = "FF1FFF0F2";
			res = op.execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			point.setXY(10, 10);
			scl = "FF1FFF0F2";
			res = op.execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline.setEmpty();
			point.setEmpty();
			polyline.startPath(10, 10);
			polyline.lineTo(10, 10);
			point.setXY(10, 10);
			scl = "0FFFFFFF2";
			res = op.execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline.startPath(12, 12);
			polyline.lineTo(12, 12);
			scl = "0F0FFFFF2";
			res = op.execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
			polyline.setEmpty();
			point.setEmpty();
			polyline.startPath(10, 10);
			polyline.lineTo(10, 10);
			point.setXY(0, 0);
			scl = "FF0FFF0F2";
			res = op.execute(polyline, point, sr, scl, null);
			NUnit.Framework.Assert.IsTrue(res);
		}

		[NUnit.Framework.Test]
		public virtual void testCrosses_github_issue_40()
		{
			// Issue 40: Acceleration without a spatial reference changes the result
			// of relation operators
			com.esri.core.geometry.Geometry geom1 = com.esri.core.geometry.OperatorImportFromWkt
				.local().execute(0, com.esri.core.geometry.Geometry.Type.Unknown, "LINESTRING (2 0, 2 3)"
				, null);
			com.esri.core.geometry.Geometry geom2 = com.esri.core.geometry.OperatorImportFromWkt
				.local().execute(0, com.esri.core.geometry.Geometry.Type.Unknown, "POLYGON ((1 1, 4 1, 4 4, 1 4, 1 1))"
				, null);
			bool answer1 = com.esri.core.geometry.OperatorCrosses.local().execute(geom1, geom2
				, null, null);
			NUnit.Framework.Assert.IsTrue(answer1);
			com.esri.core.geometry.OperatorCrosses.local().accelerateGeometry(geom1, null, com.esri.core.geometry.Geometry.GeometryAccelerationDegree
				.enumHot);
			bool answer2 = com.esri.core.geometry.OperatorCrosses.local().execute(geom1, geom2
				, null, null);
			NUnit.Framework.Assert.IsTrue(answer2);
		}

		[NUnit.Framework.Test]
		public virtual void testDisjointCrash()
		{
			com.esri.core.geometry.Polygon g1 = new com.esri.core.geometry.Polygon();
			g1.addEnvelope(com.esri.core.geometry.Envelope2D.construct(0, 0, 10, 10), false);
			com.esri.core.geometry.Polygon g2 = new com.esri.core.geometry.Polygon();
			g2.addEnvelope(com.esri.core.geometry.Envelope2D.construct(10, 1, 21, 21), false);
			g1 = (com.esri.core.geometry.Polygon)com.esri.core.geometry.OperatorDensifyByLength
				.local().execute(g1, 0.1, null);
			com.esri.core.geometry.OperatorDisjoint.local().accelerateGeometry(g1, com.esri.core.geometry.SpatialReference
				.create(4267), com.esri.core.geometry.Geometry.GeometryAccelerationDegree.enumHot
				);
			bool res = com.esri.core.geometry.OperatorDisjoint.local().execute(g1, g2, com.esri.core.geometry.SpatialReference
				.create(4267), null);
			NUnit.Framework.Assert.IsTrue(!res);
		}
	}
}
