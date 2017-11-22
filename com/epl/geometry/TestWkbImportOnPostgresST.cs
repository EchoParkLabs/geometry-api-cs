

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestWkbImportOnPostgresST
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

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public static void testWkbImportOnPostgresST()
		{
			try
			{
				java.sql.Connection con = java.sql.DriverManager.getConnection("jdbc:postgresql://tb.esri.com:5432/new_gdb"
					, "tb", "tb");
				com.esri.core.geometry.OperatorFactoryLocal factory = com.esri.core.geometry.OperatorFactoryLocal
					.getInstance();
				com.esri.core.geometry.OperatorImportFromWkb operatorImport = (com.esri.core.geometry.OperatorImportFromWkb
					)factory.getOperator(com.esri.core.geometry.Operator.Type.ImportFromWkb);
				string stmt = "SELECT objectid,sde.st_asbinary(shape) FROM new_gdb.tb.interstates a WHERE objectid IN (2) AND (a.shape IS NULL OR sde.st_geometrytype(shape)::text IN ('ST_MULTILINESTRING','ST_LINESTRING'))  LIMIT 1000";
				java.sql.PreparedStatement ps = con.prepareStatement(stmt);
				java.sql.ResultSet rs = ps.executeQuery();
				while (rs.next())
				{
					byte[] rsWkbGeom = rs.getBytes(2);
					com.esri.core.geometry.Geometry geomBorg = null;
					if (rsWkbGeom != null)
					{
						geomBorg = operatorImport.execute(0, com.esri.core.geometry.Geometry.Type.Unknown
							, java.nio.ByteBuffer.wrap(rsWkbGeom), null);
					}
				}
				ps.close();
				con.close();
			}
			catch (System.Exception)
			{
			}
		}
	}
}
