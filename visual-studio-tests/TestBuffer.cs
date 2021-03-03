/*
Copyright 2017-2021 David Raleigh

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

email: davidraleigh@gmail.com
*/
using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestBuffer : NUnit.Framework.TestFixtureAttribute
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
		public virtual void TestBufferPolytonWKT()
		{
			string wkt = "MULTIPOLYGON (((-98.42049 46.08456, -98.42052 46.08682, -98.40509 46.08681, -98.40511 46.08456, -98.42049 46.08456)))";
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.Geometry geom = com.epl.geometry.OperatorImportFromWkt.Local().Execute(0, com.epl.geometry.Geometry.Type.Unknown, wkt, null);
			com.epl.geometry.Geometry buffered = com.epl.geometry.OperatorBuffer.Local().Execute(geom, sr, 2.0, null);
			string exportedGeom = com.epl.geometry.OperatorExportToWkt.Local().Execute(0, buffered, null);
			int position = exportedGeom.IndexOf('(');
			NUnit.Framework.Assert.AreEqual("MULTIPOLYGON", exportedGeom.Substring(0, position - 1 - 0));
		}

		[NUnit.Framework.Test]
		public virtual void TestBufferPoint()
		{
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.Point inputGeom = new com.epl.geometry.Point(12, 120);
			com.epl.geometry.OperatorBuffer buffer = (com.epl.geometry.OperatorBuffer)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Buffer);
			com.epl.geometry.OperatorSimplify simplify = (com.epl.geometry.OperatorSimplify)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Simplify);
			com.epl.geometry.Geometry result = buffer.Execute(inputGeom, sr, 40.0, null);
			NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
			com.epl.geometry.Polygon poly = (com.epl.geometry.Polygon)result;
			int pathCount = poly.GetPathCount();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			int pointCount = poly.GetPointCount();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(pointCount - 100.0) < 10);
			com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
			result.QueryEnvelope2D(env2D);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetWidth() - 80) < 0.01 && System.Math.Abs(env2D.GetHeight() - 80) < 0.01);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetCenterX() - 12) < 0.001 && System.Math.Abs(env2D.GetCenterY() - 120) < 0.001);
			com.epl.geometry.NonSimpleResult nsr = new com.epl.geometry.NonSimpleResult();
			bool is_simple = simplify.IsSimpleAsFeature(result, sr, true, nsr, null);
			NUnit.Framework.Assert.IsTrue(is_simple);
			{
				result = buffer.Execute(inputGeom, sr, 0, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				NUnit.Framework.Assert.IsTrue(result.IsEmpty());
			}
			{
				result = buffer.Execute(inputGeom, sr, -1, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				NUnit.Framework.Assert.IsTrue(result.IsEmpty());
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestBufferEnvelope()
		{
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.Envelope inputGeom = new com.epl.geometry.Envelope(1, 0, 200, 400);
			com.epl.geometry.OperatorBuffer buffer = (com.epl.geometry.OperatorBuffer)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Buffer);
			com.epl.geometry.OperatorSimplify simplify = (com.epl.geometry.OperatorSimplify)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Simplify);
			com.epl.geometry.Geometry result = buffer.Execute(inputGeom, sr, 40.0, null);
			NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
			com.epl.geometry.Polygon poly = (com.epl.geometry.Polygon)(result);
			com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
			result.QueryEnvelope2D(env2D);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetWidth() - (80 + 199)) < 0.001 && System.Math.Abs(env2D.GetHeight() - (80 + 400)) < 0.001);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetCenterX() - 201.0 / 2) < 0.001 && System.Math.Abs(env2D.GetCenterY() - 400 / 2.0) < 0.001);
			int pathCount = poly.GetPathCount();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			int pointCount = poly.GetPointCount();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(pointCount - 104.0) < 10);
			com.epl.geometry.NonSimpleResult nsr = new com.epl.geometry.NonSimpleResult();
			NUnit.Framework.Assert.IsTrue(simplify.IsSimpleAsFeature(result, sr, true, nsr, null));
			{
				result = buffer.Execute(inputGeom, sr, -200.0, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				NUnit.Framework.Assert.IsTrue(result.IsEmpty());
			}
			{
				result = buffer.Execute(inputGeom, sr, -200.0, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				NUnit.Framework.Assert.IsTrue(result.IsEmpty());
			}
			{
				result = buffer.Execute(inputGeom, sr, -199 / 2.0, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				NUnit.Framework.Assert.IsTrue(result.IsEmpty());
			}
			{
				result = buffer.Execute(inputGeom, sr, -50.0, null);
				poly = (com.epl.geometry.Polygon)(result);
				result.QueryEnvelope2D(env2D);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetWidth() - (199 - 100)) < 0.001 && System.Math.Abs(env2D.GetHeight() - (400 - 100)) < 0.001);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetCenterX() - 201.0 / 2) < 0.001 && System.Math.Abs(env2D.GetCenterY() - 400 / 2.0) < 0.001);
				pathCount = poly.GetPathCount();
				NUnit.Framework.Assert.IsTrue(pathCount == 1);
				pointCount = poly.GetPointCount();
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(pointCount - 4.0) < 10);
				NUnit.Framework.Assert.IsTrue(simplify.IsSimpleAsFeature(result, sr, null));
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestBufferMultiPoint()
		{
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.OperatorBuffer buffer = (com.epl.geometry.OperatorBuffer)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Buffer);
			com.epl.geometry.OperatorSimplify simplify = (com.epl.geometry.OperatorSimplify)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Simplify);
			com.epl.geometry.MultiPoint inputGeom = new com.epl.geometry.MultiPoint();
			inputGeom.Add(12, 120);
			inputGeom.Add(20, 120);
			com.epl.geometry.Geometry result = buffer.Execute(inputGeom, sr, 40.0, null);
			NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
			com.epl.geometry.Polygon poly = (com.epl.geometry.Polygon)(result);
			com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
			result.QueryEnvelope2D(env2D);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetWidth() - 80 - 8) < 0.001 && System.Math.Abs(env2D.GetHeight() - 80) < 0.001);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetCenterX() - 16) < 0.001 && System.Math.Abs(env2D.GetCenterY() - 120) < 0.001);
			int pathCount = poly.GetPathCount();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			int pointCount = poly.GetPointCount();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(pointCount - 108.0) < 10);
			NUnit.Framework.Assert.IsTrue(simplify.IsSimpleAsFeature(result, sr, null));
			{
				result = buffer.Execute(inputGeom, sr, 0, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				NUnit.Framework.Assert.IsTrue(result.IsEmpty());
			}
			{
				result = buffer.Execute(inputGeom, sr, -1, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				NUnit.Framework.Assert.IsTrue(result.IsEmpty());
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestBufferLine()
		{
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.Line inputGeom = new com.epl.geometry.Line(12, 120, 20, 120);
			com.epl.geometry.OperatorBuffer buffer = (com.epl.geometry.OperatorBuffer)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Buffer);
			com.epl.geometry.OperatorSimplify simplify = (com.epl.geometry.OperatorSimplify)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Simplify);
			com.epl.geometry.Geometry result = buffer.Execute(inputGeom, sr, 40.0, null);
			NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
			com.epl.geometry.Polygon poly = (com.epl.geometry.Polygon)(result);
			com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
			result.QueryEnvelope2D(env2D);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetWidth() - 80 - 8) < 0.001 && System.Math.Abs(env2D.GetHeight() - 80) < 0.001);
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetCenterX() - 16) < 0.001 && System.Math.Abs(env2D.GetCenterY() - 120) < 0.001);
			int pathCount = poly.GetPathCount();
			NUnit.Framework.Assert.IsTrue(pathCount == 1);
			int pointCount = poly.GetPointCount();
			NUnit.Framework.Assert.IsTrue(System.Math.Abs(pointCount - 100.0) < 10);
			NUnit.Framework.Assert.IsTrue(simplify.IsSimpleAsFeature(result, sr, null));
			{
				result = buffer.Execute(inputGeom, sr, 0, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				NUnit.Framework.Assert.IsTrue(result.IsEmpty());
			}
			{
				result = buffer.Execute(inputGeom, sr, -1, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				NUnit.Framework.Assert.IsTrue(result.IsEmpty());
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestBufferPolyline()
		{
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.Polyline inputGeom = new com.epl.geometry.Polyline();
			com.epl.geometry.OperatorBuffer buffer = (com.epl.geometry.OperatorBuffer)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Buffer);
			com.epl.geometry.OperatorSimplify simplify = (com.epl.geometry.OperatorSimplify)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Simplify);
			inputGeom.StartPath(0, 0);
			inputGeom.LineTo(50, 50);
			inputGeom.LineTo(50, 0);
			inputGeom.LineTo(0, 50);
			{
				com.epl.geometry.Geometry result = buffer.Execute(inputGeom, sr, 0, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				NUnit.Framework.Assert.IsTrue(result.IsEmpty());
			}
			{
				com.epl.geometry.Geometry result = buffer.Execute(inputGeom, sr, -1, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				NUnit.Framework.Assert.IsTrue(result.IsEmpty());
			}
			{
				com.epl.geometry.Geometry result = buffer.Execute(inputGeom, sr, 40.0, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				com.epl.geometry.Polygon poly = (com.epl.geometry.Polygon)(result);
				com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
				result.QueryEnvelope2D(env2D);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetWidth() - 80 - 50) < 0.1 && System.Math.Abs(env2D.GetHeight() - 80 - 50) < 0.1);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetCenterX() - 25) < 0.1 && System.Math.Abs(env2D.GetCenterY() - 25) < 0.1);
				int pathCount = poly.GetPathCount();
				NUnit.Framework.Assert.IsTrue(pathCount == 1);
				int pointCount = poly.GetPointCount();
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(pointCount - 171.0) < 10);
				NUnit.Framework.Assert.IsTrue(simplify.IsSimpleAsFeature(result, sr, null));
			}
			{
				com.epl.geometry.Geometry result = buffer.Execute(inputGeom, sr, 4.0, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				com.epl.geometry.Polygon poly = (com.epl.geometry.Polygon)(result);
				com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
				result.QueryEnvelope2D(env2D);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetWidth() - 8 - 50) < 0.1 && System.Math.Abs(env2D.GetHeight() - 8 - 50) < 0.1);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetCenterX() - 25) < 0.1 && System.Math.Abs(env2D.GetCenterY() - 25) < 0.1);
				int pathCount = poly.GetPathCount();
				NUnit.Framework.Assert.IsTrue(pathCount == 2);
				int pointCount = poly.GetPointCount();
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(pointCount - 186.0) < 10);
				NUnit.Framework.Assert.IsTrue(simplify.IsSimpleAsFeature(result, sr, null));
			}
			{
				inputGeom = new com.epl.geometry.Polyline();
				inputGeom.StartPath(0, 0);
				inputGeom.LineTo(50, 50);
				inputGeom.StartPath(50, 0);
				inputGeom.LineTo(0, 50);
				com.epl.geometry.Geometry result = buffer.Execute(inputGeom, sr, 4.0, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				com.epl.geometry.Polygon poly = (com.epl.geometry.Polygon)(result);
				com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
				result.QueryEnvelope2D(env2D);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetWidth() - 8 - 50) < 0.1 && System.Math.Abs(env2D.GetHeight() - 8 - 50) < 0.1);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetCenterX() - 25) < 0.1 && System.Math.Abs(env2D.GetCenterY() - 25) < 0.1);
				int pathCount = poly.GetPathCount();
				NUnit.Framework.Assert.IsTrue(pathCount == 1);
				int pointCount = poly.GetPointCount();
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(pointCount - 208.0) < 10);
				NUnit.Framework.Assert.IsTrue(simplify.IsSimpleAsFeature(result, sr, null));
			}
			{
				inputGeom = new com.epl.geometry.Polyline();
				inputGeom.StartPath(1.762614, 0.607368);
				inputGeom.LineTo(1.762414, 0.606655);
				inputGeom.LineTo(1.763006, 0.607034);
				inputGeom.LineTo(1.762548, 0.607135);
				com.epl.geometry.Geometry result = buffer.Execute(inputGeom, sr, 0.005, null);
				NUnit.Framework.Assert.IsTrue(simplify.IsSimpleAsFeature(result, sr, null));
			}
		}

		[NUnit.Framework.Test]
		public virtual void TestBufferPolygon()
		{
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.Polygon inputGeom = new com.epl.geometry.Polygon();
			com.epl.geometry.OperatorBuffer buffer = (com.epl.geometry.OperatorBuffer)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Buffer);
			com.epl.geometry.OperatorSimplify simplify = (com.epl.geometry.OperatorSimplify)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Simplify);
			inputGeom.StartPath(0, 0);
			inputGeom.LineTo(50, 50);
			inputGeom.LineTo(50, 0);
			{
				com.epl.geometry.Geometry result = buffer.Execute(inputGeom, sr, 0, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				NUnit.Framework.Assert.IsTrue(result == inputGeom);
			}
			{
				com.epl.geometry.Geometry result = buffer.Execute(inputGeom, sr, 10, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				com.epl.geometry.Polygon poly = (com.epl.geometry.Polygon)(result);
				com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
				result.QueryEnvelope2D(env2D);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetWidth() - 20 - 50) < 0.1 && System.Math.Abs(env2D.GetHeight() - 20 - 50) < 0.1);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetCenterX() - 25) < 0.1 && System.Math.Abs(env2D.GetCenterY() - 25) < 0.1);
				int pathCount = poly.GetPathCount();
				NUnit.Framework.Assert.IsTrue(pathCount == 1);
				int pointCount = poly.GetPointCount();
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(pointCount - 104.0) < 10);
				NUnit.Framework.Assert.IsTrue(simplify.IsSimpleAsFeature(result, sr, null));
			}
			{
				sr = com.epl.geometry.SpatialReference.Create(4326);
				inputGeom = new com.epl.geometry.Polygon();
				inputGeom.StartPath(0, 0);
				inputGeom.LineTo(50, 50);
				inputGeom.LineTo(50, 0);
				com.epl.geometry.Geometry result = buffer.Execute(inputGeom, sr, -10, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				com.epl.geometry.Polygon poly = (com.epl.geometry.Polygon)(result);
				com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
				result.QueryEnvelope2D(env2D);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetWidth() - 15.85) < 0.1 && System.Math.Abs(env2D.GetHeight() - 15.85) < 0.1);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetCenterX() - 32.07) < 0.1 && System.Math.Abs(env2D.GetCenterY() - 17.93) < 0.1);
				int pathCount = poly.GetPathCount();
				NUnit.Framework.Assert.IsTrue(pathCount == 1);
				int pointCount = poly.GetPointCount();
				NUnit.Framework.Assert.IsTrue(pointCount == 3);
				NUnit.Framework.Assert.IsTrue(simplify.IsSimpleAsFeature(result, sr, null));
			}
			{
				sr = com.epl.geometry.SpatialReference.Create(4326);
				inputGeom = new com.epl.geometry.Polygon();
				inputGeom.StartPath(0, 0);
				inputGeom.LineTo(0, 50);
				inputGeom.LineTo(50, 50);
				inputGeom.LineTo(50, 0);
				inputGeom.StartPath(10, 10);
				inputGeom.LineTo(40, 10);
				inputGeom.LineTo(40, 40);
				inputGeom.LineTo(10, 40);
				com.epl.geometry.Geometry result = buffer.Execute(inputGeom, sr, -2, null);
				NUnit.Framework.Assert.IsTrue(result.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon);
				com.epl.geometry.Polygon poly = (com.epl.geometry.Polygon)(result);
				com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
				result.QueryEnvelope2D(env2D);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetWidth() + 4 - 50) < 0.1 && System.Math.Abs(env2D.GetHeight() + 4 - 50) < 0.1);
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(env2D.GetCenterX() - 25) < 0.1 && System.Math.Abs(env2D.GetCenterY() - 25) < 0.1);
				int pathCount = poly.GetPathCount();
				NUnit.Framework.Assert.IsTrue(pathCount == 2);
				int pointCount = poly.GetPointCount();
				NUnit.Framework.Assert.IsTrue(System.Math.Abs(pointCount - 108) < 10);
				NUnit.Framework.Assert.IsTrue(simplify.IsSimpleAsFeature(result, sr, null));
			}
		}
	}
}
