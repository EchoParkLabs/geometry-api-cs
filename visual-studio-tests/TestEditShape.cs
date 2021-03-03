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
	public class TestEditShape : NUnit.Framework.TestFixtureAttribute
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
		public static void TestEditShape__()
		{
			{
				// Single part polygon
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
				poly.StartPath(10, 10);
				poly.LineTo(10, 12);
				poly.LineTo(14, 15);
				poly.LineTo(10, 11);
				com.epl.geometry.EditShape editShape = new com.epl.geometry.EditShape();
				int geom = editShape.AddGeometry(poly);
				com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)editShape.GetGeometry(geom);
				NUnit.Framework.Assert.IsTrue(poly.Equals(poly2));
			}
			{
				// Two part poly
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
				poly.StartPath(10, 10);
				poly.LineTo(10, 12);
				poly.LineTo(14, 15);
				poly.LineTo(10, 11);
				poly.StartPath(100, 10);
				poly.LineTo(100, 12);
				poly.LineTo(14, 150);
				poly.LineTo(10, 101);
				poly.LineTo(100, 11);
				com.epl.geometry.EditShape editShape = new com.epl.geometry.EditShape();
				int geom = editShape.AddGeometry(poly);
				com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)editShape.GetGeometry(geom);
				NUnit.Framework.Assert.IsTrue(poly.Equals(poly2));
			}
			{
				// Single part polyline
				com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
				poly.StartPath(10, 10);
				poly.LineTo(10, 12);
				poly.LineTo(14, 15);
				poly.LineTo(10, 11);
				com.epl.geometry.EditShape editShape = new com.epl.geometry.EditShape();
				int geom = editShape.AddGeometry(poly);
				com.epl.geometry.Polyline poly2 = (com.epl.geometry.Polyline)editShape.GetGeometry(geom);
				NUnit.Framework.Assert.IsTrue(poly.Equals(poly2));
			}
			{
				// Two part poly
				com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
				poly.StartPath(10, 10);
				poly.LineTo(10, 12);
				poly.LineTo(14, 15);
				poly.LineTo(10, 11);
				poly.StartPath(100, 10);
				poly.LineTo(100, 12);
				poly.LineTo(14, 150);
				poly.LineTo(10, 101);
				poly.LineTo(100, 11);
				com.epl.geometry.EditShape editShape = new com.epl.geometry.EditShape();
				int geom = editShape.AddGeometry(poly);
				com.epl.geometry.Polyline poly2 = (com.epl.geometry.Polyline)editShape.GetGeometry(geom);
				NUnit.Framework.Assert.IsTrue(poly.Equals(poly2));
			}
			{
				// Five part poly. Close one of parts to test if it works.
				com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
				poly.StartPath(10, 10);
				poly.LineTo(10, 12);
				poly.LineTo(14, 15);
				poly.LineTo(10, 11);
				poly.StartPath(100, 10);
				poly.LineTo(100, 12);
				poly.LineTo(14, 150);
				poly.LineTo(10, 101);
				poly.LineTo(100, 11);
				poly.StartPath(1100, 101);
				poly.LineTo(1300, 132);
				poly.LineTo(144, 150);
				poly.LineTo(106, 1051);
				poly.LineTo(1600, 161);
				poly.StartPath(100, 190);
				poly.LineTo(1800, 192);
				poly.LineTo(184, 8150);
				poly.LineTo(1080, 181);
				poly.StartPath(1030, 10);
				poly.LineTo(1300, 132);
				poly.LineTo(314, 3150);
				poly.LineTo(310, 1301);
				poly.LineTo(3100, 311);
				com.epl.geometry.EditShape editShape = new com.epl.geometry.EditShape();
				int geom = editShape.AddGeometry(poly);
				editShape.SetClosedPath(editShape.GetNextPath(editShape.GetFirstPath(geom)), true);
				((com.epl.geometry.MultiPathImpl)poly._getImpl()).ClosePathWithLine(1);
				com.epl.geometry.Polyline poly2 = (com.epl.geometry.Polyline)editShape.GetGeometry(geom);
				NUnit.Framework.Assert.IsTrue(poly.Equals(poly2));
			}
			{
				// Test erase
				com.epl.geometry.Polyline poly = new com.epl.geometry.Polyline();
				poly.StartPath(10, 10);
				poly.LineTo(10, 12);
				poly.LineTo(314, 3150);
				poly.LineTo(310, 1301);
				poly.LineTo(3100, 311);
				com.epl.geometry.EditShape editShape = new com.epl.geometry.EditShape();
				int geom = editShape.AddGeometry(poly);
				int vertex = editShape.GetFirstVertex(editShape.GetFirstPath(geom));
				vertex = editShape.RemoveVertex(vertex, true);
				vertex = editShape.GetNextVertex(vertex);
				editShape.RemoveVertex(vertex, true);
				com.epl.geometry.Polyline poly2 = (com.epl.geometry.Polyline)editShape.GetGeometry(geom);
				poly.SetEmpty();
				poly.StartPath(10, 12);
				poly.LineTo(310, 1301);
				poly.LineTo(3100, 311);
				NUnit.Framework.Assert.IsTrue(poly.Equals(poly2));
			}
			{
				// Test erase
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
				poly.StartPath(10, 10);
				poly.LineTo(10, 12);
				poly.LineTo(314, 3150);
				poly.LineTo(310, 1301);
				poly.LineTo(3100, 311);
				com.epl.geometry.EditShape editShape = new com.epl.geometry.EditShape();
				int geom = editShape.AddGeometry(poly);
				int vertex = editShape.GetFirstVertex(editShape.GetFirstPath(geom));
				vertex = editShape.RemoveVertex(vertex, true);
				vertex = editShape.GetNextVertex(vertex);
				editShape.RemoveVertex(vertex, true);
				com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)editShape.GetGeometry(geom);
				poly.SetEmpty();
				poly.StartPath(10, 12);
				poly.LineTo(310, 1301);
				poly.LineTo(3100, 311);
				NUnit.Framework.Assert.IsTrue(poly.Equals(poly2));
			}
			{
				// Test Filter Close Points
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
				poly.StartPath(10, 10);
				poly.LineTo(10, 10.001);
				poly.LineTo(10.001, 10);
				com.epl.geometry.EditShape editShape = new com.epl.geometry.EditShape();
				int geom = editShape.AddGeometry(poly);
				editShape.FilterClosePoints(0.002, true, false);
				com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)editShape.GetGeometry(geom);
				NUnit.Framework.Assert.IsTrue(poly2.IsEmpty());
			}
			{
				// Test Filter Close Points
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
				poly.StartPath(10, 10);
				poly.LineTo(10, 10.0025);
				poly.LineTo(11.0, 10);
				com.epl.geometry.EditShape editShape = new com.epl.geometry.EditShape();
				int geom = editShape.AddGeometry(poly);
				editShape.FilterClosePoints(0.002, true, false);
				com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)editShape.GetGeometry(geom);
				NUnit.Framework.Assert.IsTrue(!poly2.IsEmpty());
			}
			{
				// Test Filter Close Points
				com.epl.geometry.Polygon poly = new com.epl.geometry.Polygon();
				poly.StartPath(10, 10);
				poly.LineTo(10, 10.001);
				poly.LineTo(11.0, 10);
				com.epl.geometry.EditShape editShape = new com.epl.geometry.EditShape();
				int geom = editShape.AddGeometry(poly);
				editShape.FilterClosePoints(0.002, true, false);
				com.epl.geometry.Polygon poly2 = (com.epl.geometry.Polygon)editShape.GetGeometry(geom);
				NUnit.Framework.Assert.IsTrue(poly2.IsEmpty());
			}
			{
				// Test attribute splitting 1
				com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline();
				polyline.StartPath(0, 0);
				polyline.LineTo(1, 1);
				polyline.LineTo(2, 2);
				polyline.LineTo(3, 3);
				polyline.LineTo(4, 4);
				polyline.StartPath(5, 5);
				polyline.LineTo(6, 6);
				polyline.LineTo(7, 7);
				polyline.LineTo(8, 8);
				polyline.LineTo(9, 9);
				polyline.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
				polyline.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0, 4);
				polyline.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0, 8);
				polyline.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0, 12);
				polyline.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 3, 0, 16);
				polyline.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 4, 0, 20);
				polyline.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 5, 0, 22);
				polyline.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 6, 0, 26);
				polyline.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 7, 0, 30);
				polyline.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 8, 0, 34);
				polyline.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 9, 0, 38);
				com.epl.geometry.EditShape shape = new com.epl.geometry.EditShape();
				int geometry = shape.AddGeometry(polyline);
				com.epl.geometry.AttributeStreamOfInt32 vertex_handles = new com.epl.geometry.AttributeStreamOfInt32(0);
				for (int path = shape.GetFirstPath(geometry); path != -1; path = shape.GetNextPath(path))
				{
					for (int vertex = shape.GetFirstVertex(path); vertex != -1; vertex = shape.GetNextVertex(vertex))
					{
						if (vertex != shape.GetLastVertex(path))
						{
							vertex_handles.Add(vertex);
						}
					}
				}
				double[] t = new double[1];
				for (int i = 0; i < vertex_handles.Size(); i++)
				{
					int vertex = vertex_handles.Read(i);
					t[0] = 0.5;
					shape.SplitSegment(vertex, t, 1);
				}
				com.epl.geometry.Polyline chopped_polyline = (com.epl.geometry.Polyline)shape.GetGeometry(geometry);
				NUnit.Framework.Assert.IsTrue(chopped_polyline.GetPointCount() == 18);
				double att_ = 4;
				for (int i_1 = 0; i_1 < 18; i_1++)
				{
					double att = chopped_polyline.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, i_1, 0);
					NUnit.Framework.Assert.IsTrue(att == att_);
					att_ += 2;
				}
			}
			{
				// Test attribute splitting 2
				com.epl.geometry.Polyline line1 = new com.epl.geometry.Polyline();
				com.epl.geometry.Polyline line2 = new com.epl.geometry.Polyline();
				line1.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
				line2.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
				line1.StartPath(0, 0);
				line1.LineTo(10, 10);
				line2.StartPath(10, 0);
				line2.LineTo(0, 10);
				line1.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 0, 7);
				line1.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, 17);
				line2.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 0, 5);
				line2.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, 15);
				com.epl.geometry.EditShape shape = new com.epl.geometry.EditShape();
				int g1 = shape.AddGeometry(line1);
				int g2 = shape.AddGeometry(line2);
				com.epl.geometry.CrackAndCluster.Execute(shape, 0.001, null, true);
				com.epl.geometry.Polyline chopped_line1 = (com.epl.geometry.Polyline)shape.GetGeometry(g1);
				com.epl.geometry.Polyline chopped_line2 = (com.epl.geometry.Polyline)shape.GetGeometry(g2);
				double att1 = chopped_line1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0);
				double att2 = chopped_line2.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0);
				NUnit.Framework.Assert.IsTrue(att1 == 12);
				NUnit.Framework.Assert.IsTrue(att2 == 10);
			}
			{
				// Test attribute splitting 3
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
				polygon.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
				polygon.StartPath(0, 0);
				polygon.LineTo(0, 10);
				polygon.LineTo(10, 10);
				polygon.LineTo(10, 0);
				polygon.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 0, 7);
				polygon.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, 17);
				polygon.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 2, 0, 23);
				polygon.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 3, 0, 43);
				com.epl.geometry.EditShape shape = new com.epl.geometry.EditShape();
				int geometry = shape.AddGeometry(polygon);
				com.epl.geometry.AttributeStreamOfInt32 vertex_handles = new com.epl.geometry.AttributeStreamOfInt32(0);
				int start_v = shape.GetFirstVertex(shape.GetFirstPath(geometry));
				int v = start_v;
				do
				{
					vertex_handles.Add(v);
					v = shape.GetNextVertex(v);
				}
				while (v != start_v);
				double[] t = new double[1];
				for (int i = 0; i < vertex_handles.Size(); i++)
				{
					int v1 = vertex_handles.Read(i);
					t[0] = 0.5;
					shape.SplitSegment(v1, t, 1);
				}
				com.epl.geometry.Polygon cut_polygon = (com.epl.geometry.Polygon)shape.GetGeometry(geometry);
				NUnit.Framework.Assert.IsTrue(cut_polygon.GetPointCount() == 8);
				com.epl.geometry.Point2D pt0 = cut_polygon.GetXY(0);
				double a0 = cut_polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0);
				NUnit.Framework.Assert.IsTrue(a0 == 25);
				com.epl.geometry.Point2D pt1 = cut_polygon.GetXY(1);
				double a1 = cut_polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0);
				NUnit.Framework.Assert.IsTrue(a1 == 7);
				com.epl.geometry.Point2D pt2 = cut_polygon.GetXY(2);
				double a2 = cut_polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0);
				NUnit.Framework.Assert.IsTrue(a2 == 12);
				com.epl.geometry.Point2D pt3 = cut_polygon.GetXY(3);
				double a3 = cut_polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 3, 0);
				NUnit.Framework.Assert.IsTrue(a3 == 17);
				com.epl.geometry.Point2D pt4 = cut_polygon.GetXY(4);
				double a4 = cut_polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 4, 0);
				NUnit.Framework.Assert.IsTrue(a4 == 20);
				com.epl.geometry.Point2D pt5 = cut_polygon.GetXY(5);
				double a5 = cut_polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 5, 0);
				NUnit.Framework.Assert.IsTrue(a5 == 23);
				com.epl.geometry.Point2D pt6 = cut_polygon.GetXY(6);
				double a6 = cut_polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 6, 0);
				NUnit.Framework.Assert.IsTrue(a6 == 33);
				com.epl.geometry.Point2D pt7 = cut_polygon.GetXY(7);
				double a7 = cut_polygon.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 7, 0);
				NUnit.Framework.Assert.IsTrue(a7 == 43);
			}
		}
	}
}
