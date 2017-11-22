

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestJSonGeometry
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
		public virtual void testGetSpatialReferenceFor4326()
		{
			string completeStr = "GEOGCS[\"GCS_Sphere\",DATUM[\"D_Sphere\"," + "SPHEROID[\"Sphere\",6371000.0,0.0]],PRIMEM[\"Greenwich\",0.0],"
				 + "UNIT[\"Degree\",0.0174532925199433]]";
			// 4326 GCS_WGS_1984
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(completeStr);
			NUnit.Framework.Assert.IsNotNull(sr);
		}
	}

	internal sealed class HashMapClassForTesting
	{
		private sealed class _Dictionary_36 : System.Collections.Generic.Dictionary<int, 
			string>
		{
			public _Dictionary_36()
			{
				{
					this[4035] = "GEOGCS[\"GCS_Sphere\",DATUM[\"D_Sphere\"," + "SPHEROID[\"Sphere\",6371000.0,0.0]],PRIMEM[\"Greenwich\",0.0],"
						 + "UNIT[\"Degree\",0.0174532925199433]]";
				}
				this.serialVersionUID = 8630934425353750539L;
			}

			/// <summary>added to get rid of warning</summary>
			private const long serialVersionUID;
		}

		internal static System.Collections.Generic.IDictionary<int, string> SR_WKI_WKTs = 
			new _Dictionary_36();
	}
}
