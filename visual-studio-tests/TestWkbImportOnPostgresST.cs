using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestWkbImportOnPostgresST : NUnit.Framework.TestFixtureAttribute
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

		/// <exception cref="System.Exception"/>
		[NUnit.Framework.Test]
		public static void TestWkbImportOnPostgresST()
		{
			try
			{
				java.sql.Connection con = java.sql.DriverManager.GetConnection("jdbc:postgresql://tb.esri.com:5432/new_gdb", "tb", "tb");
				com.epl.geometry.OperatorFactoryLocal factory = com.epl.geometry.OperatorFactoryLocal.GetInstance();
				com.epl.geometry.OperatorImportFromWkb operatorImport = (com.epl.geometry.OperatorImportFromWkb)factory.GetOperator(com.epl.geometry.Operator.Type.ImportFromWkb);
				string stmt = "SELECT objectid,sde.st_asbinary(shape) FROM new_gdb.tb.interstates a WHERE objectid IN (2) AND (a.shape IS NULL OR sde.st_geometrytype(shape)::text IN ('ST_MULTILINESTRING','ST_LINESTRING'))  LIMIT 1000";
				java.sql.PreparedStatement ps = con.PrepareStatement(stmt);
				java.sql.ResultSet rs = ps.ExecuteQuery();
				while (rs.Next())
				{
					byte[] rsWkbGeom = rs.GetBytes(2);
					com.epl.geometry.Geometry geomBorg = null;
					if (rsWkbGeom != null)
					{
						geomBorg = operatorImport.Execute(0, com.epl.geometry.Geometry.Type.Unknown, System.IO.MemoryStream.Wrap(rsWkbGeom), null);
					}
				}
				ps.Close();
				con.Close();
			}
			catch (System.Exception)
			{
			}
		}
	}
}
