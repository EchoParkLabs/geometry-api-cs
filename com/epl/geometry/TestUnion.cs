

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestUnion
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
		public static void testUnion()
		{
			com.esri.core.geometry.Point pt = new com.esri.core.geometry.Point(10, 20);
			com.esri.core.geometry.Point pt2 = new com.esri.core.geometry.Point();
			pt2.setXY(10, 10);
			com.esri.core.geometry.Envelope env1 = new com.esri.core.geometry.Envelope(10, 10
				, 30, 50);
			com.esri.core.geometry.Envelope env2 = new com.esri.core.geometry.Envelope(30, 10
				, 60, 50);
			com.esri.core.geometry.Geometry[] geomArray = new com.esri.core.geometry.Geometry
				[] { env1, env2 };
			com.esri.core.geometry.SimpleGeometryCursor inputGeometries = new com.esri.core.geometry.SimpleGeometryCursor
				(geomArray);
			com.esri.core.geometry.OperatorUnion union = (com.esri.core.geometry.OperatorUnion
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Union);
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.GeometryCursor outputCursor = union.execute(inputGeometries
				, sr, null);
			com.esri.core.geometry.Geometry result = outputCursor.next();
		}
	}
}
