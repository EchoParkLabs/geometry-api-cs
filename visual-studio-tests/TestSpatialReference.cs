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
	public class TestSpatialReference : NUnit.Framework.TestFixtureAttribute
	{
		[NUnit.Framework.Test]
		public virtual void TestEquals()
		{
			string wktext1 = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137.0,298.257223563]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]]";
			string wktext2 = "PROJCS[\"WGS_1984_Web_Mercator_Auxiliary_Sphere\",GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137.0,298.257223563]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Mercator_Auxiliary_Sphere\"],PARAMETER[\"False_Easting\",0.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",0.0],PARAMETER[\"Standard_Parallel_1\",0.0],PARAMETER[\"Auxiliary_Sphere_Type\",0.0],UNIT[\"Meter\",1.0]]";
			com.epl.geometry.SpatialReference a1 = com.epl.geometry.SpatialReference.Create(wktext1);
			com.epl.geometry.SpatialReference b = com.epl.geometry.SpatialReference.Create(wktext2);
			com.epl.geometry.SpatialReference a2 = com.epl.geometry.SpatialReference.Create(wktext1);
			NUnit.Framework.Assert.IsTrue(a1.Equals(a1));
			NUnit.Framework.Assert.IsTrue(b.Equals(b));
			NUnit.Framework.Assert.IsTrue(a1.Equals(a2));
			NUnit.Framework.Assert.IsFalse(a1.Equals(b));
			NUnit.Framework.Assert.IsFalse(b.Equals(a1));
		}
	}
}
