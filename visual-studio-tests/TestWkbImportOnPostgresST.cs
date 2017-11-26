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
