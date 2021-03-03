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
	public class TestFailed : NUnit.Framework.TestFixtureAttribute
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
		public virtual void TestCenterXY()
		{
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope(-130, 30, -70, 50);
			NUnit.Framework.Assert.AreEqual(-100, env.GetCenterX(), 0);
			NUnit.Framework.Assert.AreEqual(40, env.GetCenterY(), 0);
		}

		[NUnit.Framework.Test]
		public virtual void TestGeometryOperationSupport()
		{
			com.epl.geometry.Geometry baseGeom = new com.epl.geometry.Point(-130, 10);
			com.epl.geometry.Geometry comparisonGeom = new com.epl.geometry.Point(-130, 10);
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.Geometry diffGeom = null;
			int noException = 1;
			// no exception
			try
			{
				diffGeom = com.epl.geometry.GeometryEngine.Difference(baseGeom, comparisonGeom, sr);
			}
			catch (System.ArgumentException)
			{
				noException = 0;
			}
			catch (com.epl.geometry.GeometryException)
			{
				noException = 0;
			}
			NUnit.Framework.Assert.AreEqual(noException, 1);
		}

		[NUnit.Framework.Test]
		public virtual void TestIntersection()
		{
			com.epl.geometry.OperatorIntersects op = (com.epl.geometry.OperatorIntersects)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Intersects);
			com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
			// outer ring1
			polygon.StartPath(0, 0);
			polygon.LineTo(10, 10);
			polygon.LineTo(20, 0);
			com.epl.geometry.Point point1 = new com.epl.geometry.Point(15, 10);
			com.epl.geometry.Point point2 = new com.epl.geometry.Point(2, 10);
			com.epl.geometry.Point point3 = new com.epl.geometry.Point(5, 5);
			bool res = op.Execute(polygon, point1, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = op.Execute(polygon, point2, null, null);
			NUnit.Framework.Assert.IsTrue(!res);
			res = op.Execute(polygon, point3, null, null);
			NUnit.Framework.Assert.IsTrue(res);
		}
	}
}
