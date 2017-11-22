

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestBuffer
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
		public virtual void testBufferPoint()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Point inputGeom = new com.esri.core.geometry.Point(12, 120
				);
			com.esri.core.geometry.OperatorBuffer buffer = (com.esri.core.geometry.OperatorBuffer
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Buffer);
			com.esri.core.geometry.OperatorSimplify simplify = (com.esri.core.geometry.OperatorSimplify
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Simplify);
			com.esri.core.geometry.Geometry result = buffer.execute(inputGeom, sr, 40.0, null
				);
			NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
				.Polygon);
			com.esri.core.geometry.Polygon poly = (com.esri.core.geometry.Polygon)result;
			int pathCount = poly.getPathCount();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			int pointCount = poly.getPointCount();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(pointCount - 100.0) < 10);
			com.esri.core.geometry.Envelope2D env2D = new com.esri.core.geometry.Envelope2D();
			result.queryEnvelope2D(env2D);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getWidth() - 80) < 0.01 && System.Math
				.abs(env2D.getHeight() - 80) < 0.01);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getCenterX() - 12) < 0.001 &&
				 System.Math.abs(env2D.getCenterY() - 120) < 0.001);
			com.esri.core.geometry.NonSimpleResult nsr = new com.esri.core.geometry.NonSimpleResult
				();
			bool is_simple = simplify.isSimpleAsFeature(result, sr, true, nsr, null);
			NUnit.Framework.Assert.IsTrue(is_simple);
			{
				result = buffer.execute(inputGeom, sr, 0, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				NUnit.Framework.Assert.IsTrue(result.isEmpty());
			}
			{
				result = buffer.execute(inputGeom, sr, -1, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				NUnit.Framework.Assert.IsTrue(result.isEmpty());
			}
		}

		[NUnit.Framework.Test]
		public virtual void testBufferEnvelope()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Envelope inputGeom = new com.esri.core.geometry.Envelope(1
				, 0, 200, 400);
			com.esri.core.geometry.OperatorBuffer buffer = (com.esri.core.geometry.OperatorBuffer
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Buffer);
			com.esri.core.geometry.OperatorSimplify simplify = (com.esri.core.geometry.OperatorSimplify
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Simplify);
			com.esri.core.geometry.Geometry result = buffer.execute(inputGeom, sr, 40.0, null
				);
			NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
				.Polygon);
			com.esri.core.geometry.Polygon poly = (com.esri.core.geometry.Polygon)(result);
			com.esri.core.geometry.Envelope2D env2D = new com.esri.core.geometry.Envelope2D();
			result.queryEnvelope2D(env2D);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getWidth() - (80 + 199)) < 0.001
				 && System.Math.abs(env2D.getHeight() - (80 + 400)) < 0.001);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getCenterX() - 201.0 / 2) < 0.001
				 && System.Math.abs(env2D.getCenterY() - 400 / 2.0) < 0.001);
			int pathCount = poly.getPathCount();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			int pointCount = poly.getPointCount();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(pointCount - 104.0) < 10);
			com.esri.core.geometry.NonSimpleResult nsr = new com.esri.core.geometry.NonSimpleResult
				();
			NUnit.Framework.Assert.IsTrue(simplify.isSimpleAsFeature(result, sr, true, nsr, null
				));
			{
				result = buffer.execute(inputGeom, sr, -200.0, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				NUnit.Framework.Assert.IsTrue(result.isEmpty());
			}
			{
				result = buffer.execute(inputGeom, sr, -200.0, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				NUnit.Framework.Assert.IsTrue(result.isEmpty());
			}
			{
				result = buffer.execute(inputGeom, sr, -199 / 2.0, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				NUnit.Framework.Assert.IsTrue(result.isEmpty());
			}
			{
				result = buffer.execute(inputGeom, sr, -50.0, null);
				poly = (com.esri.core.geometry.Polygon)(result);
				result.queryEnvelope2D(env2D);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getWidth() - (199 - 100)) < 0.001
					 && System.Math.abs(env2D.getHeight() - (400 - 100)) < 0.001);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getCenterX() - 201.0 / 2) < 0.001
					 && System.Math.abs(env2D.getCenterY() - 400 / 2.0) < 0.001);
				pathCount = poly.getPathCount();
				NUnit.Framework.Assert.IsTrue(pathCount == 1);
				pointCount = poly.getPointCount();
				NUnit.Framework.Assert.IsTrue(System.Math.abs(pointCount - 4.0) < 10);
				NUnit.Framework.Assert.IsTrue(simplify.isSimpleAsFeature(result, sr, null));
			}
		}

		[NUnit.Framework.Test]
		public virtual void testBufferMultiPoint()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.OperatorBuffer buffer = (com.esri.core.geometry.OperatorBuffer
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Buffer);
			com.esri.core.geometry.OperatorSimplify simplify = (com.esri.core.geometry.OperatorSimplify
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Simplify);
			com.esri.core.geometry.MultiPoint inputGeom = new com.esri.core.geometry.MultiPoint
				();
			inputGeom.add(12, 120);
			inputGeom.add(20, 120);
			com.esri.core.geometry.Geometry result = buffer.execute(inputGeom, sr, 40.0, null
				);
			NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
				.Polygon);
			com.esri.core.geometry.Polygon poly = (com.esri.core.geometry.Polygon)(result);
			com.esri.core.geometry.Envelope2D env2D = new com.esri.core.geometry.Envelope2D();
			result.queryEnvelope2D(env2D);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getWidth() - 80 - 8) < 0.001 
				&& System.Math.abs(env2D.getHeight() - 80) < 0.001);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getCenterX() - 16) < 0.001 &&
				 System.Math.abs(env2D.getCenterY() - 120) < 0.001);
			int pathCount = poly.getPathCount();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			int pointCount = poly.getPointCount();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(pointCount - 108.0) < 10);
			NUnit.Framework.Assert.IsTrue(simplify.isSimpleAsFeature(result, sr, null));
			{
				result = buffer.execute(inputGeom, sr, 0, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				NUnit.Framework.Assert.IsTrue(result.isEmpty());
			}
			{
				result = buffer.execute(inputGeom, sr, -1, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				NUnit.Framework.Assert.IsTrue(result.isEmpty());
			}
		}

		[NUnit.Framework.Test]
		public virtual void testBufferLine()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Line inputGeom = new com.esri.core.geometry.Line(12, 120, 
				20, 120);
			com.esri.core.geometry.OperatorBuffer buffer = (com.esri.core.geometry.OperatorBuffer
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Buffer);
			com.esri.core.geometry.OperatorSimplify simplify = (com.esri.core.geometry.OperatorSimplify
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Simplify);
			com.esri.core.geometry.Geometry result = buffer.execute(inputGeom, sr, 40.0, null
				);
			NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
				.Polygon);
			com.esri.core.geometry.Polygon poly = (com.esri.core.geometry.Polygon)(result);
			com.esri.core.geometry.Envelope2D env2D = new com.esri.core.geometry.Envelope2D();
			result.queryEnvelope2D(env2D);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getWidth() - 80 - 8) < 0.001 
				&& System.Math.abs(env2D.getHeight() - 80) < 0.001);
			NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getCenterX() - 16) < 0.001 &&
				 System.Math.abs(env2D.getCenterY() - 120) < 0.001);
			int pathCount = poly.getPathCount();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			int pointCount = poly.getPointCount();
			NUnit.Framework.Assert.IsTrue(System.Math.abs(pointCount - 100.0) < 10);
			NUnit.Framework.Assert.IsTrue(simplify.isSimpleAsFeature(result, sr, null));
			{
				result = buffer.execute(inputGeom, sr, 0, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				NUnit.Framework.Assert.IsTrue(result.isEmpty());
			}
			{
				result = buffer.execute(inputGeom, sr, -1, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				NUnit.Framework.Assert.IsTrue(result.isEmpty());
			}
		}

		[NUnit.Framework.Test]
		public virtual void testBufferPolyline()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Polyline inputGeom = new com.esri.core.geometry.Polyline();
			com.esri.core.geometry.OperatorBuffer buffer = (com.esri.core.geometry.OperatorBuffer
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Buffer);
			com.esri.core.geometry.OperatorSimplify simplify = (com.esri.core.geometry.OperatorSimplify
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Simplify);
			inputGeom.startPath(0, 0);
			inputGeom.lineTo(50, 50);
			inputGeom.lineTo(50, 0);
			inputGeom.lineTo(0, 50);
			{
				com.esri.core.geometry.Geometry result = buffer.execute(inputGeom, sr, 0, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				NUnit.Framework.Assert.IsTrue(result.isEmpty());
			}
			{
				com.esri.core.geometry.Geometry result = buffer.execute(inputGeom, sr, -1, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				NUnit.Framework.Assert.IsTrue(result.isEmpty());
			}
			{
				com.esri.core.geometry.Geometry result = buffer.execute(inputGeom, sr, 40.0, null
					);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				com.esri.core.geometry.Polygon poly = (com.esri.core.geometry.Polygon)(result);
				com.esri.core.geometry.Envelope2D env2D = new com.esri.core.geometry.Envelope2D();
				result.queryEnvelope2D(env2D);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getWidth() - 80 - 50) < 0.1 &&
					 System.Math.abs(env2D.getHeight() - 80 - 50) < 0.1);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getCenterX() - 25) < 0.1 && System.Math
					.abs(env2D.getCenterY() - 25) < 0.1);
				int pathCount = poly.getPathCount();
				NUnit.Framework.Assert.IsTrue(pathCount == 1);
				int pointCount = poly.getPointCount();
				NUnit.Framework.Assert.IsTrue(System.Math.abs(pointCount - 171.0) < 10);
				NUnit.Framework.Assert.IsTrue(simplify.isSimpleAsFeature(result, sr, null));
			}
			{
				com.esri.core.geometry.Geometry result = buffer.execute(inputGeom, sr, 4.0, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				com.esri.core.geometry.Polygon poly = (com.esri.core.geometry.Polygon)(result);
				com.esri.core.geometry.Envelope2D env2D = new com.esri.core.geometry.Envelope2D();
				result.queryEnvelope2D(env2D);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getWidth() - 8 - 50) < 0.1 &&
					 System.Math.abs(env2D.getHeight() - 8 - 50) < 0.1);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getCenterX() - 25) < 0.1 && System.Math
					.abs(env2D.getCenterY() - 25) < 0.1);
				int pathCount = poly.getPathCount();
				NUnit.Framework.Assert.IsTrue(pathCount == 2);
				int pointCount = poly.getPointCount();
				NUnit.Framework.Assert.IsTrue(System.Math.abs(pointCount - 186.0) < 10);
				NUnit.Framework.Assert.IsTrue(simplify.isSimpleAsFeature(result, sr, null));
			}
			{
				inputGeom = new com.esri.core.geometry.Polyline();
				inputGeom.startPath(0, 0);
				inputGeom.lineTo(50, 50);
				inputGeom.startPath(50, 0);
				inputGeom.lineTo(0, 50);
				com.esri.core.geometry.Geometry result = buffer.execute(inputGeom, sr, 4.0, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				com.esri.core.geometry.Polygon poly = (com.esri.core.geometry.Polygon)(result);
				com.esri.core.geometry.Envelope2D env2D = new com.esri.core.geometry.Envelope2D();
				result.queryEnvelope2D(env2D);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getWidth() - 8 - 50) < 0.1 &&
					 System.Math.abs(env2D.getHeight() - 8 - 50) < 0.1);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getCenterX() - 25) < 0.1 && System.Math
					.abs(env2D.getCenterY() - 25) < 0.1);
				int pathCount = poly.getPathCount();
				NUnit.Framework.Assert.IsTrue(pathCount == 1);
				int pointCount = poly.getPointCount();
				NUnit.Framework.Assert.IsTrue(System.Math.abs(pointCount - 208.0) < 10);
				NUnit.Framework.Assert.IsTrue(simplify.isSimpleAsFeature(result, sr, null));
			}
		}

		[NUnit.Framework.Test]
		public virtual void testBufferPolygon()
		{
			com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
				.create(4326);
			com.esri.core.geometry.Polygon inputGeom = new com.esri.core.geometry.Polygon();
			com.esri.core.geometry.OperatorBuffer buffer = (com.esri.core.geometry.OperatorBuffer
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Buffer);
			com.esri.core.geometry.OperatorSimplify simplify = (com.esri.core.geometry.OperatorSimplify
				)com.esri.core.geometry.OperatorFactoryLocal.getInstance().getOperator(com.esri.core.geometry.Operator.Type
				.Simplify);
			inputGeom.startPath(0, 0);
			inputGeom.lineTo(50, 50);
			inputGeom.lineTo(50, 0);
			{
				com.esri.core.geometry.Geometry result = buffer.execute(inputGeom, sr, 0, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				NUnit.Framework.Assert.IsTrue(result == inputGeom);
			}
			{
				com.esri.core.geometry.Geometry result = buffer.execute(inputGeom, sr, 10, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				com.esri.core.geometry.Polygon poly = (com.esri.core.geometry.Polygon)(result);
				com.esri.core.geometry.Envelope2D env2D = new com.esri.core.geometry.Envelope2D();
				result.queryEnvelope2D(env2D);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getWidth() - 20 - 50) < 0.1 &&
					 System.Math.abs(env2D.getHeight() - 20 - 50) < 0.1);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getCenterX() - 25) < 0.1 && System.Math
					.abs(env2D.getCenterY() - 25) < 0.1);
				int pathCount = poly.getPathCount();
				NUnit.Framework.Assert.IsTrue(pathCount == 1);
				int pointCount = poly.getPointCount();
				NUnit.Framework.Assert.IsTrue(System.Math.abs(pointCount - 104.0) < 10);
				NUnit.Framework.Assert.IsTrue(simplify.isSimpleAsFeature(result, sr, null));
			}
			{
				sr = com.esri.core.geometry.SpatialReference.create(4326);
				inputGeom = new com.esri.core.geometry.Polygon();
				inputGeom.startPath(0, 0);
				inputGeom.lineTo(50, 50);
				inputGeom.lineTo(50, 0);
				com.esri.core.geometry.Geometry result = buffer.execute(inputGeom, sr, -10, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				com.esri.core.geometry.Polygon poly = (com.esri.core.geometry.Polygon)(result);
				com.esri.core.geometry.Envelope2D env2D = new com.esri.core.geometry.Envelope2D();
				result.queryEnvelope2D(env2D);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getWidth() - 15.85) < 0.1 && 
					System.Math.abs(env2D.getHeight() - 15.85) < 0.1);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getCenterX() - 32.07) < 0.1 &&
					 System.Math.abs(env2D.getCenterY() - 17.93) < 0.1);
				int pathCount = poly.getPathCount();
				NUnit.Framework.Assert.IsTrue(pathCount == 1);
				int pointCount = poly.getPointCount();
				NUnit.Framework.Assert.IsTrue(pointCount == 3);
				NUnit.Framework.Assert.IsTrue(simplify.isSimpleAsFeature(result, sr, null));
			}
			{
				sr = com.esri.core.geometry.SpatialReference.create(4326);
				inputGeom = new com.esri.core.geometry.Polygon();
				inputGeom.startPath(0, 0);
				inputGeom.lineTo(0, 50);
				inputGeom.lineTo(50, 50);
				inputGeom.lineTo(50, 0);
				inputGeom.startPath(10, 10);
				inputGeom.lineTo(40, 10);
				inputGeom.lineTo(40, 40);
				inputGeom.lineTo(10, 40);
				com.esri.core.geometry.Geometry result = buffer.execute(inputGeom, sr, -2, null);
				NUnit.Framework.Assert.IsTrue(result.getType().value() == com.esri.core.geometry.Geometry.GeometryType
					.Polygon);
				com.esri.core.geometry.Polygon poly = (com.esri.core.geometry.Polygon)(result);
				com.esri.core.geometry.Envelope2D env2D = new com.esri.core.geometry.Envelope2D();
				result.queryEnvelope2D(env2D);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getWidth() + 4 - 50) < 0.1 &&
					 System.Math.abs(env2D.getHeight() + 4 - 50) < 0.1);
				NUnit.Framework.Assert.IsTrue(System.Math.abs(env2D.getCenterX() - 25) < 0.1 && System.Math
					.abs(env2D.getCenterY() - 25) < 0.1);
				int pathCount = poly.getPathCount();
				NUnit.Framework.Assert.IsTrue(pathCount == 2);
				int pointCount = poly.getPointCount();
				NUnit.Framework.Assert.IsTrue(System.Math.abs(pointCount - 108) < 10);
				NUnit.Framework.Assert.IsTrue(simplify.isSimpleAsFeature(result, sr, null));
			}
		}
	}
}
