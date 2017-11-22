

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestOffset
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
		public virtual void testOffsetPoint()
		{
			try
			{
				com.esri.core.geometry.Point point = new com.esri.core.geometry.Point();
				point.setXY(0, 0);
				com.esri.core.geometry.OperatorOffset offset = (com.esri.core.geometry.OperatorOffset
					)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Offset);
				com.esri.core.geometry.Geometry outputGeom = offset.execute(point, null, 2, com.esri.core.geometry.OperatorOffset.JoinType
					.Round, 2, 0, null);
				NUnit.Framework.Assert.IsNull(outputGeom);
			}
			catch (System.Exception)
			{
			}
			try
			{
				com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
				mp.add(0, 0);
				mp.add(10, 10);
				com.esri.core.geometry.OperatorOffset offset = (com.esri.core.geometry.OperatorOffset
					)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
					.Offset);
				com.esri.core.geometry.Geometry outputGeom = offset.execute(mp, null, 2, com.esri.core.geometry.OperatorOffset.JoinType
					.Round, 2, 0, null);
				NUnit.Framework.Assert.IsNull(outputGeom);
			}
			catch (System.Exception)
			{
			}
		}

		[NUnit.Framework.Test]
		public virtual void testOffsetPolyline()
		{
			for (long i = -5; i <= 5; i++)
			{
				try
				{
					OffsetPolyline_(i, com.esri.core.geometry.OperatorOffset.JoinType.Round);
				}
				catch (System.Exception)
				{
					fail("OffsetPolyline(Round) failure");
				}
				try
				{
					OffsetPolyline_(i, com.esri.core.geometry.OperatorOffset.JoinType.Miter);
				}
				catch (System.Exception)
				{
					fail("OffsetPolyline(Miter) failure");
				}
				try
				{
					OffsetPolyline_(i, com.esri.core.geometry.OperatorOffset.JoinType.Bevel);
				}
				catch (System.Exception)
				{
					fail("OffsetPolyline(Bevel) failure");
				}
				try
				{
					OffsetPolyline_(i, com.esri.core.geometry.OperatorOffset.JoinType.Square);
				}
				catch (System.Exception)
				{
					fail("OffsetPolyline(Square) failure");
				}
			}
		}

		public virtual void OffsetPolyline_(double distance, com.esri.core.geometry.OperatorOffset.JoinType
			 joins)
		{
			com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
			polyline.startPath(0, 0);
			polyline.lineTo(6, 0);
			polyline.lineTo(6, 1);
			polyline.lineTo(4, 1);
			polyline.lineTo(4, 2);
			polyline.lineTo(10, 2);
			com.esri.core.geometry.OperatorOffset offset = (com.esri.core.geometry.OperatorOffset
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Offset);
			com.esri.core.geometry.Geometry outputGeom = offset.execute(polyline, null, distance
				, joins, 2, 0, null);
			NUnit.Framework.Assert.IsNotNull(outputGeom);
		}

		[NUnit.Framework.Test]
		public virtual void testOffsetPolygon()
		{
			for (long i = -5; i <= 5; i++)
			{
				try
				{
					OffsetPolygon_(i, com.esri.core.geometry.OperatorOffset.JoinType.Round);
				}
				catch (System.Exception)
				{
					fail("OffsetPolyline(Round) failure");
				}
				try
				{
					OffsetPolygon_(i, com.esri.core.geometry.OperatorOffset.JoinType.Miter);
				}
				catch (System.Exception)
				{
					fail("OffsetPolyline(Miter) failure");
				}
				try
				{
					OffsetPolygon_(i, com.esri.core.geometry.OperatorOffset.JoinType.Bevel);
				}
				catch (System.Exception)
				{
					fail("OffsetPolyline(Bevel) failure");
				}
				try
				{
					OffsetPolygon_(i, com.esri.core.geometry.OperatorOffset.JoinType.Square);
				}
				catch (System.Exception)
				{
					fail("OffsetPolyline(Square) failure");
				}
			}
		}

		public virtual void OffsetPolygon_(double distance, com.esri.core.geometry.OperatorOffset.JoinType
			 joins)
		{
			com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
			polygon.startPath(0, 0);
			polygon.lineTo(0, 16);
			polygon.lineTo(16, 16);
			polygon.lineTo(16, 11);
			polygon.lineTo(10, 10);
			polygon.lineTo(10, 12);
			polygon.lineTo(3, 12);
			polygon.lineTo(3, 4);
			polygon.lineTo(10, 4);
			polygon.lineTo(10, 6);
			polygon.lineTo(16, 5);
			polygon.lineTo(16, 0);
			com.esri.core.geometry.OperatorOffset offset = (com.esri.core.geometry.OperatorOffset
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Offset);
			com.esri.core.geometry.Geometry outputGeom = offset.execute(polygon, null, distance
				, joins, 2, 0, null);
			NUnit.Framework.Assert.IsNotNull(outputGeom);
			if (distance > 2)
			{
				NUnit.Framework.Assert.IsTrue(outputGeom.isEmpty());
			}
		}
	}
}
