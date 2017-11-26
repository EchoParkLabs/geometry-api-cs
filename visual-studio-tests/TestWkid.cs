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
	public class TestWkid : NUnit.Framework.TestFixtureAttribute
	{
		[NUnit.Framework.Test]
		public virtual void Test()
		{
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(102100);
			NUnit.Framework.Assert.IsTrue(sr.GetID() == 102100);
			NUnit.Framework.Assert.IsTrue(sr.GetLatestID() == 3857);
			NUnit.Framework.Assert.IsTrue(sr.GetOldID() == 102100);
			NUnit.Framework.Assert.IsTrue(sr.GetTolerance() == 0.001);
			com.epl.geometry.SpatialReference sr84 = com.epl.geometry.SpatialReference.Create(4326);
			double tol84 = sr84.GetTolerance();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(tol84 - 1e-8) < 1e-8 * 1e-8);
		}

		[NUnit.Framework.Test]
		public virtual void Test_80()
		{
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(3857);
			NUnit.Framework.Assert.IsTrue(sr.GetID() == 3857);
			NUnit.Framework.Assert.IsTrue(sr.GetLatestID() == 3857);
			NUnit.Framework.Assert.IsTrue(sr.GetOldID() == 102100);
			NUnit.Framework.Assert.IsTrue(sr.GetTolerance() == 0.001);
		}
	}
}
