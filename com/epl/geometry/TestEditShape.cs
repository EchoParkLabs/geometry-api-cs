

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestEditShape
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
		public static void testEditShape()
		{
			{
				// std::shared_ptr<Esri_runtimecore::Geometry::Polygon> poly_base_6
				// = std::make_shared<Esri_runtimecore::Geometry::Polygon>();
				// Single part polygon
				com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
				poly.startPath(10, 10);
				poly.lineTo(10, 12);
				poly.lineTo(14, 15);
				poly.lineTo(10, 11);
				com.esri.core.geometry.EditShape editShape = new com.esri.core.geometry.EditShape
					();
				int geom = editShape.addGeometry(poly);
				com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)editShape.
					getGeometry(geom);
				NUnit.Framework.Assert.IsTrue(poly.Equals(poly2));
			}
			{
				// Two part poly
				com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
				poly.startPath(10, 10);
				poly.lineTo(10, 12);
				poly.lineTo(14, 15);
				poly.lineTo(10, 11);
				poly.startPath(100, 10);
				poly.lineTo(100, 12);
				poly.lineTo(14, 150);
				poly.lineTo(10, 101);
				poly.lineTo(100, 11);
				com.esri.core.geometry.EditShape editShape = new com.esri.core.geometry.EditShape
					();
				int geom = editShape.addGeometry(poly);
				com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)editShape.
					getGeometry(geom);
				NUnit.Framework.Assert.IsTrue(poly.Equals(poly2));
			}
			{
				// Single part polyline
				com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
				poly.startPath(10, 10);
				poly.lineTo(10, 12);
				poly.lineTo(14, 15);
				poly.lineTo(10, 11);
				com.esri.core.geometry.EditShape editShape = new com.esri.core.geometry.EditShape
					();
				int geom = editShape.addGeometry(poly);
				com.esri.core.geometry.Polyline poly2 = (com.esri.core.geometry.Polyline)editShape
					.getGeometry(geom);
				NUnit.Framework.Assert.IsTrue(poly.Equals(poly2));
			}
			{
				// Two part poly
				com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
				poly.startPath(10, 10);
				poly.lineTo(10, 12);
				poly.lineTo(14, 15);
				poly.lineTo(10, 11);
				poly.startPath(100, 10);
				poly.lineTo(100, 12);
				poly.lineTo(14, 150);
				poly.lineTo(10, 101);
				poly.lineTo(100, 11);
				com.esri.core.geometry.EditShape editShape = new com.esri.core.geometry.EditShape
					();
				int geom = editShape.addGeometry(poly);
				com.esri.core.geometry.Polyline poly2 = (com.esri.core.geometry.Polyline)editShape
					.getGeometry(geom);
				NUnit.Framework.Assert.IsTrue(poly.Equals(poly2));
			}
			{
				// Five part poly. Close one of parts to test if it works.
				com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
				poly.startPath(10, 10);
				poly.lineTo(10, 12);
				poly.lineTo(14, 15);
				poly.lineTo(10, 11);
				poly.startPath(100, 10);
				poly.lineTo(100, 12);
				poly.lineTo(14, 150);
				poly.lineTo(10, 101);
				poly.lineTo(100, 11);
				poly.startPath(1100, 101);
				poly.lineTo(1300, 132);
				poly.lineTo(144, 150);
				poly.lineTo(106, 1051);
				poly.lineTo(1600, 161);
				poly.startPath(100, 190);
				poly.lineTo(1800, 192);
				poly.lineTo(184, 8150);
				poly.lineTo(1080, 181);
				poly.startPath(1030, 10);
				poly.lineTo(1300, 132);
				poly.lineTo(314, 3150);
				poly.lineTo(310, 1301);
				poly.lineTo(3100, 311);
				com.esri.core.geometry.EditShape editShape = new com.esri.core.geometry.EditShape
					();
				int geom = editShape.addGeometry(poly);
				editShape.setClosedPath(editShape.getNextPath(editShape.getFirstPath(geom)), true
					);
				((com.esri.core.geometry.MultiPathImpl)poly._getImpl()).closePathWithLine(1);
				com.esri.core.geometry.Polyline poly2 = (com.esri.core.geometry.Polyline)editShape
					.getGeometry(geom);
				NUnit.Framework.Assert.IsTrue(poly.Equals(poly2));
			}
			{
				// Test erase
				com.esri.core.geometry.Polyline poly = new com.esri.core.geometry.Polyline();
				poly.startPath(10, 10);
				poly.lineTo(10, 12);
				poly.lineTo(314, 3150);
				poly.lineTo(310, 1301);
				poly.lineTo(3100, 311);
				com.esri.core.geometry.EditShape editShape = new com.esri.core.geometry.EditShape
					();
				int geom = editShape.addGeometry(poly);
				int vertex = editShape.getFirstVertex(editShape.getFirstPath(geom));
				vertex = editShape.removeVertex(vertex, true);
				vertex = editShape.getNextVertex(vertex);
				editShape.removeVertex(vertex, true);
				com.esri.core.geometry.Polyline poly2 = (com.esri.core.geometry.Polyline)editShape
					.getGeometry(geom);
				poly.setEmpty();
				poly.startPath(10, 12);
				poly.lineTo(310, 1301);
				poly.lineTo(3100, 311);
				NUnit.Framework.Assert.IsTrue(poly.Equals(poly2));
			}
			{
				// Test erase
				com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
				poly.startPath(10, 10);
				poly.lineTo(10, 12);
				poly.lineTo(314, 3150);
				poly.lineTo(310, 1301);
				poly.lineTo(3100, 311);
				com.esri.core.geometry.EditShape editShape = new com.esri.core.geometry.EditShape
					();
				int geom = editShape.addGeometry(poly);
				int vertex = editShape.getFirstVertex(editShape.getFirstPath(geom));
				vertex = editShape.removeVertex(vertex, true);
				vertex = editShape.getNextVertex(vertex);
				editShape.removeVertex(vertex, true);
				com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)editShape.
					getGeometry(geom);
				poly.setEmpty();
				poly.startPath(10, 12);
				poly.lineTo(310, 1301);
				poly.lineTo(3100, 311);
				NUnit.Framework.Assert.IsTrue(poly.Equals(poly2));
			}
			{
				// Test Filter Close Points
				com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
				poly.startPath(10, 10);
				poly.lineTo(10, 10.001);
				poly.lineTo(10.001, 10);
				com.esri.core.geometry.EditShape editShape = new com.esri.core.geometry.EditShape
					();
				int geom = editShape.addGeometry(poly);
				editShape.filterClosePoints(0.002, true, false);
				com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)editShape.
					getGeometry(geom);
				NUnit.Framework.Assert.IsTrue(poly2.isEmpty());
			}
			{
				// Test Filter Close Points
				com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
				poly.startPath(10, 10);
				poly.lineTo(10, 10.0025);
				poly.lineTo(11.0, 10);
				com.esri.core.geometry.EditShape editShape = new com.esri.core.geometry.EditShape
					();
				int geom = editShape.addGeometry(poly);
				editShape.filterClosePoints(0.002, true, false);
				com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)editShape.
					getGeometry(geom);
				NUnit.Framework.Assert.IsTrue(!poly2.isEmpty());
			}
			{
				// Test Filter Close Points
				com.esri.core.geometry.Polygon poly = new com.esri.core.geometry.Polygon();
				poly.startPath(10, 10);
				poly.lineTo(10, 10.001);
				poly.lineTo(11.0, 10);
				com.esri.core.geometry.EditShape editShape = new com.esri.core.geometry.EditShape
					();
				int geom = editShape.addGeometry(poly);
				editShape.filterClosePoints(0.002, true, false);
				com.esri.core.geometry.Polygon poly2 = (com.esri.core.geometry.Polygon)editShape.
					getGeometry(geom);
				NUnit.Framework.Assert.IsTrue(poly2.isEmpty());
			}
			{
				// Test attribute splitting 1
				com.esri.core.geometry.Polyline polyline = new com.esri.core.geometry.Polyline();
				polyline.startPath(0, 0);
				polyline.lineTo(1, 1);
				polyline.lineTo(2, 2);
				polyline.lineTo(3, 3);
				polyline.lineTo(4, 4);
				polyline.startPath(5, 5);
				polyline.lineTo(6, 6);
				polyline.lineTo(7, 7);
				polyline.lineTo(8, 8);
				polyline.lineTo(9, 9);
				polyline.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 
					4);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 1, 0, 
					8);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 2, 0, 
					12);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 3, 0, 
					16);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 4, 0, 
					20);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 5, 0, 
					22);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 6, 0, 
					26);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 7, 0, 
					30);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 8, 0, 
					34);
				polyline.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 9, 0, 
					38);
				com.esri.core.geometry.EditShape shape = new com.esri.core.geometry.EditShape();
				int geometry = shape.addGeometry(polyline);
				com.esri.core.geometry.AttributeStreamOfInt32 vertex_handles = new com.esri.core.geometry.AttributeStreamOfInt32
					(0);
				for (int path = shape.getFirstPath(geometry); path != -1; path = shape.getNextPath
					(path))
				{
					for (int vertex = shape.getFirstVertex(path); vertex != -1; vertex = shape.getNextVertex
						(vertex))
					{
						if (vertex != shape.getLastVertex(path))
						{
							vertex_handles.add(vertex);
						}
					}
				}
				double[] t = new double[1];
				for (int i = 0; i < vertex_handles.size(); i++)
				{
					int vertex = vertex_handles.read(i);
					t[0] = 0.5;
					shape.splitSegment(vertex, t, 1);
				}
				com.esri.core.geometry.Polyline chopped_polyline = (com.esri.core.geometry.Polyline
					)shape.getGeometry(geometry);
				NUnit.Framework.Assert.IsTrue(chopped_polyline.getPointCount() == 18);
				double att_ = 4;
				for (int i_1 = 0; i_1 < 18; i_1++)
				{
					double att = chopped_polyline.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
						.Z, i_1, 0);
					NUnit.Framework.Assert.IsTrue(att == att_);
					att_ += 2;
				}
			}
			{
				// Test attribute splitting 2
				com.esri.core.geometry.Polyline line1 = new com.esri.core.geometry.Polyline();
				com.esri.core.geometry.Polyline line2 = new com.esri.core.geometry.Polyline();
				line1.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
				line2.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
				line1.startPath(0, 0);
				line1.lineTo(10, 10);
				line2.startPath(10, 0);
				line2.lineTo(0, 10);
				line1.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 0, 7);
				line1.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, 17
					);
				line2.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 0, 5);
				line2.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, 15
					);
				com.esri.core.geometry.EditShape shape = new com.esri.core.geometry.EditShape();
				int g1 = shape.addGeometry(line1);
				int g2 = shape.addGeometry(line2);
				com.esri.core.geometry.CrackAndCluster.execute(shape, 0.001, null, true);
				com.esri.core.geometry.Polyline chopped_line1 = (com.esri.core.geometry.Polyline)
					shape.getGeometry(g1);
				com.esri.core.geometry.Polyline chopped_line2 = (com.esri.core.geometry.Polyline)
					shape.getGeometry(g2);
				double att1 = chopped_line1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 1, 0);
				double att2 = chopped_line2.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 1, 0);
				NUnit.Framework.Assert.IsTrue(att1 == 12);
				NUnit.Framework.Assert.IsTrue(att2 == 10);
			}
			{
				// Test attribute splitting 3
				com.esri.core.geometry.Polygon polygon = new com.esri.core.geometry.Polygon();
				polygon.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
				polygon.startPath(0, 0);
				polygon.lineTo(0, 10);
				polygon.lineTo(10, 10);
				polygon.lineTo(10, 0);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 0, 
					7);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, 
					17);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 2, 0, 
					23);
				polygon.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 3, 0, 
					43);
				com.esri.core.geometry.EditShape shape = new com.esri.core.geometry.EditShape();
				int geometry = shape.addGeometry(polygon);
				com.esri.core.geometry.AttributeStreamOfInt32 vertex_handles = new com.esri.core.geometry.AttributeStreamOfInt32
					(0);
				int start_v = shape.getFirstVertex(shape.getFirstPath(geometry));
				int v = start_v;
				do
				{
					vertex_handles.add(v);
					v = shape.getNextVertex(v);
				}
				while (v != start_v);
				double[] t = new double[1];
				for (int i = 0; i < vertex_handles.size(); i++)
				{
					int v1 = vertex_handles.read(i);
					t[0] = 0.5;
					shape.splitSegment(v1, t, 1);
				}
				com.esri.core.geometry.Polygon cut_polygon = (com.esri.core.geometry.Polygon)shape
					.getGeometry(geometry);
				NUnit.Framework.Assert.IsTrue(cut_polygon.getPointCount() == 8);
				com.esri.core.geometry.Point2D pt0 = cut_polygon.getXY(0);
				double a0 = cut_polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 0, 0);
				NUnit.Framework.Assert.IsTrue(a0 == 25);
				com.esri.core.geometry.Point2D pt1 = cut_polygon.getXY(1);
				double a1 = cut_polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 1, 0);
				NUnit.Framework.Assert.IsTrue(a1 == 7);
				com.esri.core.geometry.Point2D pt2 = cut_polygon.getXY(2);
				double a2 = cut_polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 2, 0);
				NUnit.Framework.Assert.IsTrue(a2 == 12);
				com.esri.core.geometry.Point2D pt3 = cut_polygon.getXY(3);
				double a3 = cut_polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 3, 0);
				NUnit.Framework.Assert.IsTrue(a3 == 17);
				com.esri.core.geometry.Point2D pt4 = cut_polygon.getXY(4);
				double a4 = cut_polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 4, 0);
				NUnit.Framework.Assert.IsTrue(a4 == 20);
				com.esri.core.geometry.Point2D pt5 = cut_polygon.getXY(5);
				double a5 = cut_polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 5, 0);
				NUnit.Framework.Assert.IsTrue(a5 == 23);
				com.esri.core.geometry.Point2D pt6 = cut_polygon.getXY(6);
				double a6 = cut_polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 6, 0);
				NUnit.Framework.Assert.IsTrue(a6 == 33);
				com.esri.core.geometry.Point2D pt7 = cut_polygon.getXY(7);
				double a7 = cut_polygon.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
					.M, 7, 0);
				NUnit.Framework.Assert.IsTrue(a7 == 43);
			}
		}
	}
}
