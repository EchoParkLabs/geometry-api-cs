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
	public class TestJSonGeometry : NUnit.Framework.TestFixtureAttribute
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
		public virtual void TestGetSpatialReferenceFor4326()
		{
			string completeStr = "GEOGCS[\"GCS_Sphere\",DATUM[\"D_Sphere\"," + "SPHEROID[\"Sphere\",6371000.0,0.0]],PRIMEM[\"Greenwich\",0.0]," + "UNIT[\"Degree\",0.0174532925199433]]";
			// 4326 GCS_WGS_1984
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(completeStr);
			NUnit.Framework.Assert.IsNotNull(sr);
		}
	}

	internal sealed class HashMapClassForTesting
	{
		private sealed class _Dictionary_59 : System.Collections.Generic.Dictionary<int, string>
		{
			public _Dictionary_59()
			{
				{
					this[4035] = "GEOGCS[\"GCS_Sphere\",DATUM[\"D_Sphere\"," + "SPHEROID[\"Sphere\",6371000.0,0.0]],PRIMEM[\"Greenwich\",0.0]," + "UNIT[\"Degree\",0.0174532925199433]]";
				}
				this.serialVersionUID = 8630934425353750539L;
			}

			/// <summary>added to get rid of warning</summary>
			private const long serialVersionUID;
		}

		internal static System.Collections.Generic.IDictionary<int, string> SR_WKI_WKTs = new _Dictionary_59();
	}
}
