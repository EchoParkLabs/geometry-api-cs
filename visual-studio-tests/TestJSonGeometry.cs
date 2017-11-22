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
		private sealed class _Dictionary_36 : System.Collections.Generic.Dictionary<int, string>
		{
			public _Dictionary_36()
			{
				{
					this[4035] = "GEOGCS[\"GCS_Sphere\",DATUM[\"D_Sphere\"," + "SPHEROID[\"Sphere\",6371000.0,0.0]],PRIMEM[\"Greenwich\",0.0]," + "UNIT[\"Degree\",0.0174532925199433]]";
				}
				this.serialVersionUID = 8630934425353750539L;
			}

			/// <summary>added to get rid of warning</summary>
			private const long serialVersionUID;
		}

		internal static System.Collections.Generic.IDictionary<int, string> SR_WKI_WKTs = new _Dictionary_36();
	}
}
