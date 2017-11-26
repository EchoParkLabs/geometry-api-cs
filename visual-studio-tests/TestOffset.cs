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
	public class TestOffset : NUnit.Framework.TestFixtureAttribute
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
		public virtual void TestOffsetPoint()
		{
			try
			{
				com.epl.geometry.Point point = new com.epl.geometry.Point();
				point.SetXY(0, 0);
				com.epl.geometry.OperatorOffset offset = (com.epl.geometry.OperatorOffset)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Offset);
				com.epl.geometry.Geometry outputGeom = offset.Execute(point, null, 2, com.epl.geometry.OperatorOffset.JoinType.Round, 2, 0, null);
				NUnit.Framework.Assert.IsNull(outputGeom);
			}
			catch (System.Exception)
			{
			}
			try
			{
				com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
				mp.Add(0, 0);
				mp.Add(10, 10);
				com.epl.geometry.OperatorOffset offset = (com.epl.geometry.OperatorOffset)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Offset);
				com.epl.geometry.Geometry outputGeom = offset.Execute(mp, null, 2, com.epl.geometry.OperatorOffset.JoinType.Round, 2, 0, null);
				NUnit.Framework.Assert.IsNull(outputGeom);
			}
			catch (System.Exception)
			{
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestOffsetPolyline()
		{
			for (long i = -5; i <= 5; i++)
			{
				try
				{
					OffsetPolyline_(i, com.epl.geometry.OperatorOffset.JoinType.Round);
				}
				catch (System.Exception)
				{
					//Fail("OffsetPolyline(Round) failure");
				}
				try
				{
					OffsetPolyline_(i, com.epl.geometry.OperatorOffset.JoinType.Miter);
				}
				catch (System.Exception)
				{
					//Fail("OffsetPolyline(Miter) failure");
				}
				try
				{
					OffsetPolyline_(i, com.epl.geometry.OperatorOffset.JoinType.Bevel);
				}
				catch (System.Exception)
				{
					//Fail("OffsetPolyline(Bevel) failure");
				}
				try
				{
					OffsetPolyline_(i, com.epl.geometry.OperatorOffset.JoinType.Square);
				}
				catch (System.Exception)
				{
					//Fail("OffsetPolyline(Square) failure");
				}
			}
		}

		public virtual void OffsetPolyline_(double distance, com.epl.geometry.OperatorOffset.JoinType joins)
		{
			com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
			polyline.StartPath(0, 0);
			polyline.LineTo(6, 0);
			polyline.LineTo(6, 1);
			polyline.LineTo(4, 1);
			polyline.LineTo(4, 2);
			polyline.LineTo(10, 2);
			com.epl.geometry.OperatorOffset offset = (com.epl.geometry.OperatorOffset)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Offset);
			com.epl.geometry.Geometry outputGeom = offset.Execute(polyline, null, distance, joins, 2, 0, null);
			NUnit.Framework.Assert.IsNotNull(outputGeom);
		}

		[NUnit.Framework.Test]
		public virtual void TestOffsetPolygon()
		{
			for (long i = -5; i <= 5; i++)
			{
				try
				{
					OffsetPolygon_(i, com.epl.geometry.OperatorOffset.JoinType.Round);
				}
				catch (System.Exception)
				{
					//Fail("OffsetPolyline(Round) failure");
				}
				try
				{
					OffsetPolygon_(i, com.epl.geometry.OperatorOffset.JoinType.Miter);
				}
				catch (System.Exception)
				{
					//Fail("OffsetPolyline(Miter) failure");
				}
				try
				{
					OffsetPolygon_(i, com.epl.geometry.OperatorOffset.JoinType.Bevel);
				}
				catch (System.Exception)
				{
					//Fail("OffsetPolyline(Bevel) failure");
				}
				try
				{
					OffsetPolygon_(i, com.epl.geometry.OperatorOffset.JoinType.Square);
				}
				catch (System.Exception)
				{
					//Fail("OffsetPolyline(Square) failure");
				}
			}
		}

		public virtual void OffsetPolygon_(double distance, com.epl.geometry.OperatorOffset.JoinType joins)
		{
			com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
			polygon.StartPath(0, 0);
			polygon.LineTo(0, 16);
			polygon.LineTo(16, 16);
			polygon.LineTo(16, 11);
			polygon.LineTo(10, 10);
			polygon.LineTo(10, 12);
			polygon.LineTo(3, 12);
			polygon.LineTo(3, 4);
			polygon.LineTo(10, 4);
			polygon.LineTo(10, 6);
			polygon.LineTo(16, 5);
			polygon.LineTo(16, 0);
			com.epl.geometry.OperatorOffset offset = (com.epl.geometry.OperatorOffset)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Offset);
			com.epl.geometry.Geometry outputGeom = offset.Execute(polygon, null, distance, joins, 2, 0, null);
			NUnit.Framework.Assert.IsNotNull(outputGeom);
			if (distance > 2)
			{
				NUnit.Framework.Assert.IsTrue(outputGeom.IsEmpty());
			}
		}
	}
}
