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
	public class TestEquals : NUnit.Framework.TestFixtureAttribute
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
		public virtual void TestEqualsOnEnvelopes()
		{
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.Point p = new com.epl.geometry.Point(-130, 10);
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope(p, 12, 12);
			com.epl.geometry.Envelope env2 = new com.epl.geometry.Envelope(-136, 4, -124, 16);
			bool isEqual;
			try
			{
				isEqual = com.epl.geometry.GeometryEngine.Equals(env, env2, sr);
			}
			catch (System.ArgumentException)
			{
				isEqual = false;
			}
			NUnit.Framework.Assert.IsTrue(isEqual);
		}

		[NUnit.Framework.Test]
		public virtual void TestEqualsOnPoints()
		{
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.Point p1 = new com.epl.geometry.Point(-116, 40);
			com.epl.geometry.Point p2 = new com.epl.geometry.Point(-120, 39);
			com.epl.geometry.Point p3 = new com.epl.geometry.Point(-121, 10);
			com.epl.geometry.Point p4 = new com.epl.geometry.Point(-130, 12);
			com.epl.geometry.Point p5 = new com.epl.geometry.Point(-108, 25);
			com.epl.geometry.Point p12 = new com.epl.geometry.Point(-116, 40);
			com.epl.geometry.Point p22 = new com.epl.geometry.Point(-120, 39);
			com.epl.geometry.Point p32 = new com.epl.geometry.Point(-121, 10);
			com.epl.geometry.Point p42 = new com.epl.geometry.Point(-130, 12);
			com.epl.geometry.Point p52 = new com.epl.geometry.Point(-108, 25);
			bool isEqual1 = false;
			bool isEqual2 = false;
			bool isEqual3 = false;
			bool isEqual4 = false;
			bool isEqual5 = false;
			try
			{
				isEqual1 = com.epl.geometry.GeometryEngine.Equals(p1, p12, sr);
				isEqual2 = com.epl.geometry.GeometryEngine.Equals(p1, p12, sr);
				isEqual3 = com.epl.geometry.GeometryEngine.Equals(p1, p12, sr);
				isEqual4 = com.epl.geometry.GeometryEngine.Equals(p1, p12, sr);
				isEqual5 = com.epl.geometry.GeometryEngine.Equals(p1, p12, sr);
			}
			catch (System.ArgumentException)
			{
			}
			NUnit.Framework.Assert.IsTrue(isEqual1 && isEqual2 && isEqual3 && isEqual4 && isEqual5);
		}

		[NUnit.Framework.Test]
		public virtual void TestEqualsOnPolygons()
		{
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.Polygon baseMp = new com.epl.geometry.Polygon();
			com.epl.geometry.Polygon compMp = new com.epl.geometry.Polygon();
			baseMp.StartPath(-116, 40);
			baseMp.LineTo(-120, 39);
			baseMp.LineTo(-121, 10);
			baseMp.LineTo(-130, 12);
			baseMp.LineTo(-108, 25);
			compMp.StartPath(-116, 40);
			compMp.LineTo(-120, 39);
			compMp.LineTo(-121, 10);
			compMp.LineTo(-130, 12);
			compMp.LineTo(-108, 25);
			bool isEqual;
			try
			{
				isEqual = com.epl.geometry.GeometryEngine.Equals(baseMp, compMp, sr);
			}
			catch (System.ArgumentException)
			{
				isEqual = false;
			}
			NUnit.Framework.Assert.IsTrue(isEqual);
		}

		[NUnit.Framework.Test]
		public virtual void TestEqualsOnPolylines()
		{
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.Polyline baseMp = new com.epl.geometry.Polyline();
			com.epl.geometry.Polyline compMp = new com.epl.geometry.Polyline();
			baseMp.StartPath(-116, 40);
			baseMp.LineTo(-120, 39);
			baseMp.LineTo(-121, 10);
			baseMp.LineTo(-130, 12);
			baseMp.LineTo(-108, 25);
			compMp.StartPath(-116, 40);
			compMp.LineTo(-120, 39);
			compMp.LineTo(-121, 10);
			compMp.LineTo(-130, 12);
			compMp.LineTo(-108, 25);
			bool isEqual;
			try
			{
				isEqual = com.epl.geometry.GeometryEngine.Equals(baseMp, compMp, sr);
			}
			catch (System.ArgumentException)
			{
				isEqual = false;
			}
			NUnit.Framework.Assert.IsTrue(isEqual);
		}

		[NUnit.Framework.Test]
		public virtual void TestEqualsOnMultiPoints()
		{
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.MultiPoint baseMp = new com.epl.geometry.MultiPoint();
			com.epl.geometry.MultiPoint compMp = new com.epl.geometry.MultiPoint();
			baseMp.Add(new com.epl.geometry.Point(-116, 40));
			baseMp.Add(new com.epl.geometry.Point(-120, 39));
			baseMp.Add(new com.epl.geometry.Point(-121, 10));
			baseMp.Add(new com.epl.geometry.Point(-130, 12));
			baseMp.Add(new com.epl.geometry.Point(-108, 25));
			compMp.Add(new com.epl.geometry.Point(-116, 40));
			compMp.Add(new com.epl.geometry.Point(-120, 39));
			compMp.Add(new com.epl.geometry.Point(-121, 10));
			compMp.Add(new com.epl.geometry.Point(-130, 12));
			compMp.Add(new com.epl.geometry.Point(-108, 25));
			bool isEqual;
			try
			{
				isEqual = com.epl.geometry.GeometryEngine.Equals(baseMp, compMp, sr);
			}
			catch (System.ArgumentException)
			{
				isEqual = false;
			}
			NUnit.Framework.Assert.IsTrue(isEqual);
		}
	}
}
