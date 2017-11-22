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


namespace com.epl.geometry
{
	internal class RelationalOperationsMatrix
	{
		private com.epl.geometry.TopoGraph m_topo_graph;

		private int[] m_matrix;

		private int[] m_max_dim;

		private bool[] m_perform_predicates;

		private string m_scl;

		private int m_predicates_half_edge;

		private int m_predicates_cluster;

		private int m_predicate_count;

		private int m_cluster_index_a;

		private int m_cluster_index_b;

		private int m_visited_index;

		private abstract class MatrixPredicate
		{
			public const int InteriorInterior = 0;

			public const int InteriorBoundary = 1;

			public const int InteriorExterior = 2;

			public const int BoundaryInterior = 3;

			public const int BoundaryBoundary = 4;

			public const int BoundaryExterior = 5;

			public const int ExteriorInterior = 6;

			public const int ExteriorBoundary = 7;

			public const int ExteriorExterior = 8;
		}

		private static class MatrixPredicateConstants
		{
		}

		private abstract class Predicates
		{
			public const int AreaAreaPredicates = 0;

			public const int AreaLinePredicates = 1;

			public const int LineLinePredicates = 2;

			public const int AreaPointPredicates = 3;

			public const int LinePointPredicates = 4;

			public const int PointPointPredicates = 5;
		}

		private static class PredicatesConstants
		{
		}

		// Computes the necessary 9 intersection relationships of boundary,
		// interior, and exterior of geometry_a vs geometry_b for the given scl
		// string.
		internal static bool Relate(com.epl.geometry.Geometry geometry_a, com.epl.geometry.Geometry geometry_b, com.epl.geometry.SpatialReference sr, string scl, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (scl.Length != 9)
			{
				throw new com.epl.geometry.GeometryException("relation string length has to be 9 characters");
			}
			for (int i = 0; i < 9; i++)
			{
				char c = scl[i];
				if (c != '*' && c != 'T' && c != 'F' && c != '0' && c != '1' && c != '2')
				{
					throw new com.epl.geometry.GeometryException("relation string");
				}
			}
			int relation = GetPredefinedRelation_(scl, geometry_a.GetDimension(), geometry_b.GetDimension());
			if (relation != com.epl.geometry.RelationalOperations.Relation.unknown)
			{
				return com.epl.geometry.RelationalOperations.Relate(geometry_a, geometry_b, sr, relation, progress_tracker);
			}
			com.epl.geometry.Envelope2D env1 = new com.epl.geometry.Envelope2D();
			geometry_a.QueryEnvelope2D(env1);
			com.epl.geometry.Envelope2D env2 = new com.epl.geometry.Envelope2D();
			geometry_b.QueryEnvelope2D(env2);
			com.epl.geometry.Envelope2D envMerged = new com.epl.geometry.Envelope2D();
			envMerged.SetCoords(env1);
			envMerged.Merge(env2);
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(sr, envMerged, false);
			com.epl.geometry.Geometry _geometryA = ConvertGeometry_(geometry_a, tolerance);
			com.epl.geometry.Geometry _geometryB = ConvertGeometry_(geometry_b, tolerance);
			if (_geometryA.IsEmpty() || _geometryB.IsEmpty())
			{
				return RelateEmptyGeometries_(_geometryA, _geometryB, scl);
			}
			int typeA = _geometryA.GetType().Value();
			int typeB = _geometryB.GetType().Value();
			bool bRelation = false;
			switch (typeA)
			{
				case com.epl.geometry.Geometry.GeometryType.Polygon:
				{
					switch (typeB)
					{
						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							bRelation = PolygonRelatePolygon_((com.epl.geometry.Polygon)(_geometryA), (com.epl.geometry.Polygon)(_geometryB), tolerance, scl, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Polyline:
						{
							bRelation = PolygonRelatePolyline_((com.epl.geometry.Polygon)(_geometryA), (com.epl.geometry.Polyline)(_geometryB), tolerance, scl, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Point:
						{
							bRelation = PolygonRelatePoint_((com.epl.geometry.Polygon)(_geometryA), (com.epl.geometry.Point)(_geometryB), tolerance, scl, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.MultiPoint:
						{
							bRelation = PolygonRelateMultiPoint_((com.epl.geometry.Polygon)(_geometryA), (com.epl.geometry.MultiPoint)(_geometryB), tolerance, scl, progress_tracker);
							break;
						}

						default:
						{
							break;
						}
					}
					// warning fix
					break;
				}

				case com.epl.geometry.Geometry.GeometryType.Polyline:
				{
					switch (typeB)
					{
						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							bRelation = PolygonRelatePolyline_((com.epl.geometry.Polygon)(_geometryB), (com.epl.geometry.Polyline)(_geometryA), tolerance, GetTransposeMatrix_(scl), progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Polyline:
						{
							bRelation = PolylineRelatePolyline_((com.epl.geometry.Polyline)(_geometryA), (com.epl.geometry.Polyline)(_geometryB), tolerance, scl, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Point:
						{
							bRelation = PolylineRelatePoint_((com.epl.geometry.Polyline)(_geometryA), (com.epl.geometry.Point)(_geometryB), tolerance, scl, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.MultiPoint:
						{
							bRelation = PolylineRelateMultiPoint_((com.epl.geometry.Polyline)(_geometryA), (com.epl.geometry.MultiPoint)(_geometryB), tolerance, scl, progress_tracker);
							break;
						}

						default:
						{
							break;
						}
					}
					// warning fix
					break;
				}

				case com.epl.geometry.Geometry.GeometryType.Point:
				{
					switch (typeB)
					{
						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							bRelation = PolygonRelatePoint_((com.epl.geometry.Polygon)(_geometryB), (com.epl.geometry.Point)(_geometryA), tolerance, GetTransposeMatrix_(scl), progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Polyline:
						{
							bRelation = PolylineRelatePoint_((com.epl.geometry.Polyline)(_geometryB), (com.epl.geometry.Point)(_geometryA), tolerance, GetTransposeMatrix_(scl), progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Point:
						{
							bRelation = PointRelatePoint_((com.epl.geometry.Point)(_geometryA), (com.epl.geometry.Point)(_geometryB), tolerance, scl, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.MultiPoint:
						{
							bRelation = MultiPointRelatePoint_((com.epl.geometry.MultiPoint)(_geometryB), (com.epl.geometry.Point)(_geometryA), tolerance, GetTransposeMatrix_(scl), progress_tracker);
							break;
						}

						default:
						{
							break;
						}
					}
					// warning fix
					break;
				}

				case com.epl.geometry.Geometry.GeometryType.MultiPoint:
				{
					switch (typeB)
					{
						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							bRelation = PolygonRelateMultiPoint_((com.epl.geometry.Polygon)(_geometryB), (com.epl.geometry.MultiPoint)(_geometryA), tolerance, GetTransposeMatrix_(scl), progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Polyline:
						{
							bRelation = PolylineRelateMultiPoint_((com.epl.geometry.Polyline)(_geometryB), (com.epl.geometry.MultiPoint)(_geometryA), tolerance, GetTransposeMatrix_(scl), progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.MultiPoint:
						{
							bRelation = MultiPointRelateMultiPoint_((com.epl.geometry.MultiPoint)(_geometryA), (com.epl.geometry.MultiPoint)(_geometryB), tolerance, scl, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Point:
						{
							bRelation = MultiPointRelatePoint_((com.epl.geometry.MultiPoint)(_geometryA), (com.epl.geometry.Point)(_geometryB), tolerance, scl, progress_tracker);
							break;
						}

						default:
						{
							break;
						}
					}
					// warning fix
					break;
				}

				default:
				{
					bRelation = false;
					break;
				}
			}
			return bRelation;
		}

		private RelationalOperationsMatrix()
		{
			m_predicate_count = 0;
			m_topo_graph = new com.epl.geometry.TopoGraph();
			m_matrix = new int[9];
			m_max_dim = new int[9];
			m_perform_predicates = new bool[9];
			m_predicates_half_edge = -1;
			m_predicates_cluster = -1;
		}

		// Returns true if the relation holds.
		internal static bool PolygonRelatePolygon_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polygon polygon_b, double tolerance, string scl, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.RelationalOperationsMatrix relOps = new com.epl.geometry.RelationalOperationsMatrix();
			relOps.ResetMatrix_();
			relOps.SetPredicates_(scl);
			relOps.SetAreaAreaPredicates_();
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			polygon_b.QueryEnvelope2D(env_b);
			bool bRelationKnown = false;
			bool b_disjoint = com.epl.geometry.RelationalOperations.EnvelopeDisjointEnvelope_(env_a, env_b, tolerance, progress_tracker);
			if (b_disjoint)
			{
				relOps.AreaAreaDisjointPredicates_(polygon_a, polygon_b);
				bRelationKnown = true;
			}
			if (!bRelationKnown)
			{
				// Quick rasterize test to see whether the the geometries are
				// disjoint, or if one is contained in the other.
				int relation = com.epl.geometry.RelationalOperations.TryRasterizedContainsOrDisjoint_(polygon_a, polygon_b, tolerance, false);
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					relOps.AreaAreaDisjointPredicates_(polygon_a, polygon_b);
					bRelationKnown = true;
				}
				else
				{
					if (relation == com.epl.geometry.RelationalOperations.Relation.contains)
					{
						relOps.AreaAreaContainsPredicates_(polygon_b);
						bRelationKnown = true;
					}
					else
					{
						if (relation == com.epl.geometry.RelationalOperations.Relation.within)
						{
							relOps.AreaAreaWithinPredicates_(polygon_a);
							bRelationKnown = true;
						}
					}
				}
			}
			if (!bRelationKnown)
			{
				com.epl.geometry.EditShape edit_shape = new com.epl.geometry.EditShape();
				int geom_a = edit_shape.AddGeometry(polygon_a);
				int geom_b = edit_shape.AddGeometry(polygon_b);
				relOps.SetEditShapeCrackAndCluster_(edit_shape, tolerance, progress_tracker);
				relOps.ComputeMatrixTopoGraphHalfEdges_(geom_a, geom_b);
				relOps.m_topo_graph.RemoveShape();
			}
			bool bRelation = RelationCompare_(relOps.m_matrix, relOps.m_scl);
			return bRelation;
		}

		// The relation is based on the simplified-Polygon A containing Polygon B, which may be non-simple.
		internal static bool PolygonContainsPolygon_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polygon polygon_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			System.Diagnostics.Debug.Assert((!polygon_a.IsEmpty()));
			System.Diagnostics.Debug.Assert((!polygon_b.IsEmpty()));
			com.epl.geometry.RelationalOperationsMatrix relOps = new com.epl.geometry.RelationalOperationsMatrix();
			relOps.ResetMatrix_();
			relOps.SetPredicates_("T*****F**");
			relOps.SetAreaAreaPredicates_();
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			polygon_b.QueryEnvelope2D(env_b);
			bool bRelationKnown = false;
			bool b_disjoint = com.epl.geometry.RelationalOperations.EnvelopeDisjointEnvelope_(env_a, env_b, tolerance, progress_tracker);
			if (b_disjoint)
			{
				relOps.AreaAreaDisjointPredicates_(polygon_a, polygon_b);
				bRelationKnown = true;
			}
			if (!bRelationKnown)
			{
				// Quick rasterize test to see whether the the geometries are disjoint, or if one is contained in the other.
				int relation = com.epl.geometry.RelationalOperations.TryRasterizedContainsOrDisjoint_(polygon_a, polygon_b, tolerance, false);
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					relOps.AreaAreaDisjointPredicates_(polygon_a, polygon_b);
					bRelationKnown = true;
				}
				else
				{
					if (relation == com.epl.geometry.RelationalOperations.Relation.contains)
					{
						relOps.AreaAreaContainsPredicates_(polygon_b);
						bRelationKnown = true;
					}
					else
					{
						if (relation == com.epl.geometry.RelationalOperations.Relation.within)
						{
							relOps.AreaAreaWithinPredicates_(polygon_a);
							bRelationKnown = true;
						}
					}
				}
			}
			if (bRelationKnown)
			{
				bool bContains = RelationCompare_(relOps.m_matrix, relOps.m_scl);
				return bContains;
			}
			com.epl.geometry.EditShape edit_shape = new com.epl.geometry.EditShape();
			int geom_a = edit_shape.AddGeometry(polygon_a);
			int geom_b = edit_shape.AddGeometry(polygon_b);
			com.epl.geometry.CrackAndCluster.Execute(edit_shape, tolerance, progress_tracker, false);
			com.epl.geometry.Polyline boundary_b = (com.epl.geometry.Polyline)edit_shape.GetGeometry(geom_b).GetBoundary();
			edit_shape.FilterClosePoints(0, true, true);
			com.epl.geometry.Simplificator.Execute(edit_shape, geom_a, -1, false, progress_tracker);
			// Make sure Polygon A has exterior
			// If the simplified Polygon A does not have interior, then it cannot contain anything.
			if (edit_shape.GetPointCount(geom_a) == 0)
			{
				return false;
			}
			com.epl.geometry.Simplificator.Execute(edit_shape, geom_b, -1, false, progress_tracker);
			relOps.SetEditShape_(edit_shape, progress_tracker);
			// We see if the simplified Polygon A contains the simplified Polygon B.
			bool b_empty = edit_shape.GetPointCount(geom_b) == 0;
			if (!b_empty)
			{
				//geom_b has interior
				relOps.ComputeMatrixTopoGraphHalfEdges_(geom_a, geom_b);
				relOps.m_topo_graph.RemoveShape();
				bool bContains = RelationCompare_(relOps.m_matrix, relOps.m_scl);
				if (!bContains)
				{
					return bContains;
				}
			}
			com.epl.geometry.Polygon polygon_simple_a = (com.epl.geometry.Polygon)edit_shape.GetGeometry(geom_a);
			edit_shape = new com.epl.geometry.EditShape();
			geom_a = edit_shape.AddGeometry(polygon_simple_a);
			geom_b = edit_shape.AddGeometry(boundary_b);
			relOps.SetEditShape_(edit_shape, progress_tracker);
			// Check no interior lines of the boundary intersect the exterior
			relOps.m_predicate_count = 0;
			relOps.ResetMatrix_();
			relOps.SetPredicates_(b_empty ? "T*****F**" : "******F**");
			relOps.SetAreaLinePredicates_();
			relOps.ComputeMatrixTopoGraphHalfEdges_(geom_a, geom_b);
			relOps.m_topo_graph.RemoveShape();
			bool bContains_1 = RelationCompare_(relOps.m_matrix, relOps.m_scl);
			return bContains_1;
		}

		// Returns true if the relation holds.
		internal static bool PolygonRelatePolyline_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polyline polyline_b, double tolerance, string scl, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.RelationalOperationsMatrix relOps = new com.epl.geometry.RelationalOperationsMatrix();
			relOps.ResetMatrix_();
			relOps.SetPredicates_(scl);
			relOps.SetAreaLinePredicates_();
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			polyline_b.QueryEnvelope2D(env_b);
			bool bRelationKnown = false;
			bool b_disjoint = com.epl.geometry.RelationalOperations.EnvelopeDisjointEnvelope_(env_a, env_b, tolerance, progress_tracker);
			if (b_disjoint)
			{
				relOps.AreaLineDisjointPredicates_(polygon_a, polyline_b);
				// passing polyline
				// to get boundary
				// information
				bRelationKnown = true;
			}
			if (!bRelationKnown)
			{
				// Quick rasterize test to see whether the the geometries are
				// disjoint, or if one is contained in the other.
				int relation = com.epl.geometry.RelationalOperations.TryRasterizedContainsOrDisjoint_(polygon_a, polyline_b, tolerance, false);
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					relOps.AreaLineDisjointPredicates_(polygon_a, polyline_b);
					// passing
					// polyline to
					// get boundary
					// information
					bRelationKnown = true;
				}
				else
				{
					if (relation == com.epl.geometry.RelationalOperations.Relation.contains)
					{
						relOps.AreaLineContainsPredicates_(polygon_a, polyline_b);
						// passing
						// polyline to
						// get boundary
						// information
						bRelationKnown = true;
					}
				}
			}
			if (!bRelationKnown)
			{
				com.epl.geometry.EditShape edit_shape = new com.epl.geometry.EditShape();
				int geom_a = edit_shape.AddGeometry(polygon_a);
				int geom_b = edit_shape.AddGeometry(polyline_b);
				relOps.SetEditShapeCrackAndCluster_(edit_shape, tolerance, progress_tracker);
				relOps.m_cluster_index_b = relOps.m_topo_graph.CreateUserIndexForClusters();
				MarkClusterEndPoints_(geom_b, relOps.m_topo_graph, relOps.m_cluster_index_b);
				relOps.ComputeMatrixTopoGraphHalfEdges_(geom_a, geom_b);
				relOps.m_topo_graph.DeleteUserIndexForClusters(relOps.m_cluster_index_b);
				relOps.m_topo_graph.RemoveShape();
			}
			bool bRelation = RelationCompare_(relOps.m_matrix, relOps.m_scl);
			return bRelation;
		}

		internal static bool PolygonContainsPolyline_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polyline polyline_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.RelationalOperationsMatrix relOps = new com.epl.geometry.RelationalOperationsMatrix();
			relOps.ResetMatrix_();
			relOps.SetPredicates_("T*****F**");
			relOps.SetAreaLinePredicates_();
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			polyline_b.QueryEnvelope2D(env_b);
			bool bRelationKnown = false;
			bool b_disjoint = com.epl.geometry.RelationalOperations.EnvelopeDisjointEnvelope_(env_a, env_b, tolerance, progress_tracker);
			if (b_disjoint)
			{
				relOps.AreaLineDisjointPredicates_(polygon_a, polyline_b);
				// passing polyline to get boundary information
				bRelationKnown = true;
			}
			if (!bRelationKnown)
			{
				// Quick rasterize test to see whether the the geometries are disjoint, or if one is contained in the other.
				int relation = com.epl.geometry.RelationalOperations.TryRasterizedContainsOrDisjoint_(polygon_a, polyline_b, tolerance, false);
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					relOps.AreaLineDisjointPredicates_(polygon_a, polyline_b);
					// passing polyline to get boundary information
					bRelationKnown = true;
				}
				else
				{
					if (relation == com.epl.geometry.RelationalOperations.Relation.contains)
					{
						relOps.AreaLineContainsPredicates_(polygon_a, polyline_b);
						// passing polyline to get boundary information
						bRelationKnown = true;
					}
				}
			}
			if (bRelationKnown)
			{
				bool bContains = RelationCompare_(relOps.m_matrix, relOps.m_scl);
				return bContains;
			}
			com.epl.geometry.EditShape edit_shape = new com.epl.geometry.EditShape();
			int geom_a = edit_shape.AddGeometry(polygon_a);
			int geom_b = edit_shape.AddGeometry(polyline_b);
			relOps.SetEditShapeCrackAndCluster_(edit_shape, tolerance, progress_tracker);
			// Make sure Polygon A has exterior
			// If the simplified Polygon A does not have interior, then it cannot contain anything.
			if (edit_shape.GetPointCount(geom_a) == 0)
			{
				return false;
			}
			relOps.ComputeMatrixTopoGraphHalfEdges_(geom_a, geom_b);
			relOps.m_topo_graph.RemoveShape();
			bool bContains_1 = RelationCompare_(relOps.m_matrix, relOps.m_scl);
			return bContains_1;
		}

		// Returns true if the relation holds
		internal static bool PolygonRelateMultiPoint_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, string scl, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.RelationalOperationsMatrix relOps = new com.epl.geometry.RelationalOperationsMatrix();
			relOps.ResetMatrix_();
			relOps.SetPredicates_(scl);
			relOps.SetAreaPointPredicates_();
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			multipoint_b.QueryEnvelope2D(env_b);
			bool bRelationKnown = false;
			bool b_disjoint = com.epl.geometry.RelationalOperations.EnvelopeDisjointEnvelope_(env_a, env_b, tolerance, progress_tracker);
			if (b_disjoint)
			{
				relOps.AreaPointDisjointPredicates_(polygon_a);
				bRelationKnown = true;
			}
			if (!bRelationKnown)
			{
				// Quick rasterize test to see whether the the geometries are
				// disjoint, or if one is contained in the other.
				int relation = com.epl.geometry.RelationalOperations.TryRasterizedContainsOrDisjoint_(polygon_a, multipoint_b, tolerance, false);
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					relOps.AreaPointDisjointPredicates_(polygon_a);
					bRelationKnown = true;
				}
				else
				{
					if (relation == com.epl.geometry.RelationalOperations.Relation.contains)
					{
						relOps.AreaPointContainsPredicates_(polygon_a);
						bRelationKnown = true;
					}
				}
			}
			if (!bRelationKnown)
			{
				com.epl.geometry.EditShape edit_shape = new com.epl.geometry.EditShape();
				int geom_a = edit_shape.AddGeometry(polygon_a);
				int geom_b = edit_shape.AddGeometry(multipoint_b);
				relOps.SetEditShapeCrackAndCluster_(edit_shape, tolerance, progress_tracker);
				relOps.ComputeMatrixTopoGraphClusters_(geom_a, geom_b);
				relOps.m_topo_graph.RemoveShape();
			}
			bool bRelation = RelationCompare_(relOps.m_matrix, relOps.m_scl);
			return bRelation;
		}

		// Returns true if the relation holds.
		internal static bool PolylineRelatePolyline_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Polyline polyline_b, double tolerance, string scl, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.RelationalOperationsMatrix relOps = new com.epl.geometry.RelationalOperationsMatrix();
			relOps.ResetMatrix_();
			relOps.SetPredicates_(scl);
			relOps.SetLineLinePredicates_();
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polyline_a.QueryEnvelope2D(env_a);
			polyline_b.QueryEnvelope2D(env_b);
			bool bRelationKnown = false;
			bool b_disjoint = com.epl.geometry.RelationalOperations.EnvelopeDisjointEnvelope_(env_a, env_b, tolerance, progress_tracker);
			if (b_disjoint)
			{
				relOps.LineLineDisjointPredicates_(polyline_a, polyline_b);
				bRelationKnown = true;
			}
			if (!bRelationKnown)
			{
				// Quick rasterize test to see whether the the geometries are
				// disjoint, or if one is contained in the other.
				int relation = com.epl.geometry.RelationalOperations.TryRasterizedContainsOrDisjoint_(polyline_a, polyline_b, tolerance, false);
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					relOps.LineLineDisjointPredicates_(polyline_a, polyline_b);
					bRelationKnown = true;
				}
			}
			if (!bRelationKnown)
			{
				com.epl.geometry.EditShape edit_shape = new com.epl.geometry.EditShape();
				int geom_a = edit_shape.AddGeometry(polyline_a);
				int geom_b = edit_shape.AddGeometry(polyline_b);
				relOps.SetEditShapeCrackAndCluster_(edit_shape, tolerance, progress_tracker);
				relOps.m_cluster_index_a = relOps.m_topo_graph.CreateUserIndexForClusters();
				relOps.m_cluster_index_b = relOps.m_topo_graph.CreateUserIndexForClusters();
				MarkClusterEndPoints_(geom_a, relOps.m_topo_graph, relOps.m_cluster_index_a);
				MarkClusterEndPoints_(geom_b, relOps.m_topo_graph, relOps.m_cluster_index_b);
				relOps.ComputeMatrixTopoGraphHalfEdges_(geom_a, geom_b);
				relOps.m_topo_graph.DeleteUserIndexForClusters(relOps.m_cluster_index_a);
				relOps.m_topo_graph.DeleteUserIndexForClusters(relOps.m_cluster_index_b);
				relOps.m_topo_graph.RemoveShape();
			}
			bool bRelation = RelationCompare_(relOps.m_matrix, relOps.m_scl);
			return bRelation;
		}

		// Returns true if the relation holds.
		internal static bool PolylineRelateMultiPoint_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, string scl, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.RelationalOperationsMatrix relOps = new com.epl.geometry.RelationalOperationsMatrix();
			relOps.ResetMatrix_();
			relOps.SetPredicates_(scl);
			relOps.SetLinePointPredicates_();
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polyline_a.QueryEnvelope2D(env_a);
			multipoint_b.QueryEnvelope2D(env_b);
			bool bRelationKnown = false;
			bool b_disjoint = com.epl.geometry.RelationalOperations.EnvelopeDisjointEnvelope_(env_a, env_b, tolerance, progress_tracker);
			if (b_disjoint)
			{
				relOps.LinePointDisjointPredicates_(polyline_a);
				bRelationKnown = true;
			}
			if (!bRelationKnown)
			{
				// Quick rasterize test to see whether the the geometries are
				// disjoint, or if one is contained in the other.
				int relation = com.epl.geometry.RelationalOperations.TryRasterizedContainsOrDisjoint_(polyline_a, multipoint_b, tolerance, false);
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					relOps.LinePointDisjointPredicates_(polyline_a);
					bRelationKnown = true;
				}
			}
			if (!bRelationKnown)
			{
				com.epl.geometry.EditShape edit_shape = new com.epl.geometry.EditShape();
				int geom_a = edit_shape.AddGeometry(polyline_a);
				int geom_b = edit_shape.AddGeometry(multipoint_b);
				relOps.SetEditShapeCrackAndCluster_(edit_shape, tolerance, progress_tracker);
				relOps.m_cluster_index_a = relOps.m_topo_graph.CreateUserIndexForClusters();
				MarkClusterEndPoints_(geom_a, relOps.m_topo_graph, relOps.m_cluster_index_a);
				relOps.ComputeMatrixTopoGraphClusters_(geom_a, geom_b);
				relOps.m_topo_graph.DeleteUserIndexForClusters(relOps.m_cluster_index_a);
				relOps.m_topo_graph.RemoveShape();
			}
			bool bRelation = RelationCompare_(relOps.m_matrix, relOps.m_scl);
			return bRelation;
		}

		// Returns true if the relation holds.
		internal static bool MultiPointRelateMultiPoint_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, string scl, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.RelationalOperationsMatrix relOps = new com.epl.geometry.RelationalOperationsMatrix();
			relOps.ResetMatrix_();
			relOps.SetPredicates_(scl);
			relOps.SetPointPointPredicates_();
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			multipoint_a.QueryEnvelope2D(env_a);
			multipoint_b.QueryEnvelope2D(env_b);
			bool bRelationKnown = false;
			bool b_disjoint = com.epl.geometry.RelationalOperations.EnvelopeDisjointEnvelope_(env_a, env_b, tolerance, progress_tracker);
			if (b_disjoint)
			{
				relOps.PointPointDisjointPredicates_();
				bRelationKnown = true;
			}
			if (!bRelationKnown)
			{
				com.epl.geometry.EditShape edit_shape = new com.epl.geometry.EditShape();
				int geom_a = edit_shape.AddGeometry(multipoint_a);
				int geom_b = edit_shape.AddGeometry(multipoint_b);
				relOps.SetEditShapeCrackAndCluster_(edit_shape, tolerance, progress_tracker);
				relOps.ComputeMatrixTopoGraphClusters_(geom_a, geom_b);
				relOps.m_topo_graph.RemoveShape();
			}
			bool bRelation = RelationCompare_(relOps.m_matrix, relOps.m_scl);
			return bRelation;
		}

		// Returns true if the relation holds.
		internal static bool PolygonRelatePoint_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Point point_b, double tolerance, string scl, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.RelationalOperationsMatrix relOps = new com.epl.geometry.RelationalOperationsMatrix();
			relOps.ResetMatrix_();
			relOps.SetPredicates_(scl);
			relOps.SetAreaPointPredicates_();
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			com.epl.geometry.Point2D pt_b = point_b.GetXY();
			bool bRelationKnown = false;
			bool b_disjoint = com.epl.geometry.RelationalOperations.PointDisjointEnvelope_(pt_b, env_a, tolerance, progress_tracker);
			if (b_disjoint)
			{
				relOps.AreaPointDisjointPredicates_(polygon_a);
				bRelationKnown = true;
			}
			if (!bRelationKnown)
			{
				com.epl.geometry.PolygonUtils.PiPResult res = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon_a, pt_b, tolerance);
				// uses accelerator
				if (res == com.epl.geometry.PolygonUtils.PiPResult.PiPInside)
				{
					// polygon must have area
					relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 0;
					relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 2;
					relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = -1;
					relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = 1;
					relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = -1;
				}
				else
				{
					if (res == com.epl.geometry.PolygonUtils.PiPResult.PiPBoundary)
					{
						relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = -1;
						double area = polygon_a.CalculateArea2D();
						if (area != 0)
						{
							relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = -1;
							relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = 0;
							relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 2;
							relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = 1;
						}
						else
						{
							relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 0;
							relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = -1;
							relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = -1;
							com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
							polygon_a.QueryEnvelope2D(env);
							relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = (env.GetHeight() == 0.0 && env.GetWidth() == 0.0 ? -1 : 1);
						}
					}
					else
					{
						relOps.AreaPointDisjointPredicates_(polygon_a);
					}
				}
			}
			bool bRelation = RelationCompare_(relOps.m_matrix, scl);
			return bRelation;
		}

		// Returns true if the relation holds.
		internal static bool PolylineRelatePoint_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Point point_b, double tolerance, string scl, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.RelationalOperationsMatrix relOps = new com.epl.geometry.RelationalOperationsMatrix();
			relOps.ResetMatrix_();
			relOps.SetPredicates_(scl);
			relOps.SetLinePointPredicates_();
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			polyline_a.QueryEnvelope2D(env_a);
			com.epl.geometry.Point2D pt_b = point_b.GetXY();
			bool bRelationKnown = false;
			bool b_disjoint = com.epl.geometry.RelationalOperations.PointDisjointEnvelope_(pt_b, env_a, tolerance, progress_tracker);
			if (b_disjoint)
			{
				relOps.LinePointDisjointPredicates_(polyline_a);
				bRelationKnown = true;
			}
			if (!bRelationKnown)
			{
				com.epl.geometry.MultiPoint boundary_a = null;
				bool b_boundary_contains_point_known = false;
				bool b_boundary_contains_point = false;
				if (relOps.m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] || relOps.m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior])
				{
					bool b_intersects = com.epl.geometry.RelationalOperations.LinearPathIntersectsPoint_(polyline_a, pt_b, tolerance);
					if (b_intersects)
					{
						if (relOps.m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior])
						{
							boundary_a = (com.epl.geometry.MultiPoint)com.epl.geometry.Boundary.Calculate(polyline_a, progress_tracker);
							b_boundary_contains_point = !com.epl.geometry.RelationalOperations.MultiPointDisjointPointImpl_(boundary_a, pt_b, tolerance, progress_tracker);
							b_boundary_contains_point_known = true;
							if (b_boundary_contains_point)
							{
								relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = -1;
							}
							else
							{
								relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 0;
							}
						}
						relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = -1;
					}
					else
					{
						relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = -1;
						relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = 0;
					}
				}
				if (relOps.m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior])
				{
					if (boundary_a != null && boundary_a.IsEmpty())
					{
						relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = -1;
					}
					else
					{
						if (!b_boundary_contains_point_known)
						{
							if (boundary_a == null)
							{
								boundary_a = (com.epl.geometry.MultiPoint)com.epl.geometry.Boundary.Calculate(polyline_a, progress_tracker);
							}
							b_boundary_contains_point = !com.epl.geometry.RelationalOperations.MultiPointDisjointPointImpl_(boundary_a, pt_b, tolerance, progress_tracker);
							b_boundary_contains_point_known = true;
						}
						relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = (b_boundary_contains_point ? 0 : -1);
					}
				}
				if (relOps.m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior])
				{
					if (boundary_a != null && boundary_a.IsEmpty())
					{
						relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = -1;
					}
					else
					{
						if (b_boundary_contains_point_known && !b_boundary_contains_point)
						{
							relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = 0;
						}
						else
						{
							if (boundary_a == null)
							{
								boundary_a = (com.epl.geometry.MultiPoint)com.epl.geometry.Boundary.Calculate(polyline_a, progress_tracker);
							}
							bool b_boundary_equals_point = com.epl.geometry.RelationalOperations.MultiPointEqualsPoint_(boundary_a, point_b, tolerance, progress_tracker);
							relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = (b_boundary_equals_point ? -1 : 0);
						}
					}
				}
				if (relOps.m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior])
				{
					bool b_has_length = polyline_a.CalculateLength2D() != 0;
					if (b_has_length)
					{
						relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 1;
					}
					else
					{
						// all points are interior
						com.epl.geometry.MultiPoint interior_a = new com.epl.geometry.MultiPoint(polyline_a.GetDescription());
						interior_a.Add(polyline_a, 0, polyline_a.GetPointCount());
						bool b_interior_equals_point = com.epl.geometry.RelationalOperations.MultiPointEqualsPoint_(interior_a, point_b, tolerance, progress_tracker);
						relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = (b_interior_equals_point ? -1 : 0);
					}
				}
			}
			bool bRelation = RelationCompare_(relOps.m_matrix, relOps.m_scl);
			return bRelation;
		}

		// Returns true if the relation holds.
		internal static bool MultiPointRelatePoint_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.Point point_b, double tolerance, string scl, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.RelationalOperationsMatrix relOps = new com.epl.geometry.RelationalOperationsMatrix();
			relOps.ResetMatrix_();
			relOps.SetPredicates_(scl);
			relOps.SetPointPointPredicates_();
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			multipoint_a.QueryEnvelope2D(env_a);
			com.epl.geometry.Point2D pt_b = point_b.GetXY();
			bool bRelationKnown = false;
			bool b_disjoint = com.epl.geometry.RelationalOperations.PointDisjointEnvelope_(pt_b, env_a, tolerance, progress_tracker);
			if (b_disjoint)
			{
				relOps.PointPointDisjointPredicates_();
				bRelationKnown = true;
			}
			if (!bRelationKnown)
			{
				bool b_intersects = false;
				bool b_multipoint_contained = true;
				double tolerance_sq = tolerance * tolerance;
				for (int i = 0; i < multipoint_a.GetPointCount(); i++)
				{
					com.epl.geometry.Point2D pt_a = multipoint_a.GetXY(i);
					if (com.epl.geometry.Point2D.SqrDistance(pt_a, pt_b) <= tolerance_sq)
					{
						b_intersects = true;
					}
					else
					{
						b_multipoint_contained = false;
					}
					if (b_intersects && !b_multipoint_contained)
					{
						break;
					}
				}
				if (b_intersects)
				{
					relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 0;
					if (!b_multipoint_contained)
					{
						relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 0;
					}
					else
					{
						relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = -1;
					}
					relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = -1;
				}
				else
				{
					relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = -1;
					relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 0;
					relOps.m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = 0;
				}
			}
			bool bRelation = RelationCompare_(relOps.m_matrix, scl);
			return bRelation;
		}

		// Returns true if the relation holds.
		internal static bool PointRelatePoint_(com.epl.geometry.Point point_a, com.epl.geometry.Point point_b, double tolerance, string scl, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Point2D pt_a = point_a.GetXY();
			com.epl.geometry.Point2D pt_b = point_b.GetXY();
			int[] matrix = new int[9];
			for (int i = 0; i < 9; i++)
			{
				matrix[i] = -1;
			}
			if (com.epl.geometry.Point2D.SqrDistance(pt_a, pt_b) <= tolerance * tolerance)
			{
				matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 0;
			}
			else
			{
				matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 0;
				matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = 0;
			}
			matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = 2;
			bool bRelation = RelationCompare_(matrix, scl);
			return bRelation;
		}

		// Compares the DE-9I matrix against the scl string.
		private static bool RelationCompare_(int[] matrix, string scl)
		{
			for (int i = 0; i < 9; i++)
			{
				switch (scl[i])
				{
					case 'T':
					{
						System.Diagnostics.Debug.Assert((matrix[i] != -2));
						if (matrix[i] == -1)
						{
							return false;
						}
						break;
					}

					case 'F':
					{
						System.Diagnostics.Debug.Assert((matrix[i] != -2));
						if (matrix[i] != -1)
						{
							return false;
						}
						break;
					}

					case '0':
					{
						System.Diagnostics.Debug.Assert((matrix[i] != -2));
						if (matrix[i] != 0)
						{
							return false;
						}
						break;
					}

					case '1':
					{
						System.Diagnostics.Debug.Assert((matrix[i] != -2));
						if (matrix[i] != 1)
						{
							return false;
						}
						break;
					}

					case '2':
					{
						System.Diagnostics.Debug.Assert((matrix[i] != -2));
						if (matrix[i] != 2)
						{
							return false;
						}
						break;
					}

					default:
					{
						break;
					}
				}
			}
			return true;
		}

		internal static bool RelateEmptyGeometries_(com.epl.geometry.Geometry geometry_a, com.epl.geometry.Geometry geometry_b, string scl)
		{
			int[] matrix = new int[9];
			if (geometry_a.IsEmpty() && geometry_b.IsEmpty())
			{
				for (int i = 0; i < 9; i++)
				{
					matrix[i] = -1;
				}
				return RelationCompare_(matrix, scl);
			}
			bool b_transpose = false;
			com.epl.geometry.Geometry g_a;
			com.epl.geometry.Geometry g_b;
			if (!geometry_a.IsEmpty())
			{
				g_a = geometry_a;
				g_b = geometry_b;
			}
			else
			{
				g_a = geometry_b;
				g_b = geometry_a;
				b_transpose = true;
			}
			matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = -1;
			matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = -1;
			matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = -1;
			matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = -1;
			matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = -1;
			matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = -1;
			matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = 2;
			int type = g_a.GetType().Value();
			if (com.epl.geometry.Geometry.IsMultiPath(type))
			{
				if (type == com.epl.geometry.Geometry.GeometryType.Polygon)
				{
					double area = ((com.epl.geometry.Polygon)g_a).CalculateArea2D();
					if (area != 0)
					{
						matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 2;
						matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = 1;
					}
					else
					{
						matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = -1;
						com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
						g_a.QueryEnvelope2D(env);
						matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = (env.GetHeight() == 0.0 && env.GetWidth() == 0.0 ? 0 : 1);
					}
				}
				else
				{
					bool b_has_length = ((com.epl.geometry.Polyline)g_a).CalculateLength2D() != 0;
					matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = (b_has_length ? 1 : 0);
					matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = (com.epl.geometry.Boundary.HasNonEmptyBoundary((com.epl.geometry.Polyline)g_a, null) ? 0 : -1);
				}
			}
			else
			{
				matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 0;
				matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = -1;
			}
			if (b_transpose)
			{
				TransposeMatrix_(matrix);
			}
			return RelationCompare_(matrix, scl);
		}

		// Checks whether scl string is a predefined relation.
		private static int GetPredefinedRelation_(string scl, int dim_a, int dim_b)
		{
			if (Equals_(scl))
			{
				return com.epl.geometry.RelationalOperations.Relation.equals;
			}
			if (Disjoint_(scl))
			{
				return com.epl.geometry.RelationalOperations.Relation.disjoint;
			}
			if (Touches_(scl, dim_a, dim_b))
			{
				return com.epl.geometry.RelationalOperations.Relation.touches;
			}
			if (Crosses_(scl, dim_a, dim_b))
			{
				return com.epl.geometry.RelationalOperations.Relation.crosses;
			}
			if (Contains_(scl))
			{
				return com.epl.geometry.RelationalOperations.Relation.contains;
			}
			if (Overlaps_(scl, dim_a, dim_b))
			{
				return com.epl.geometry.RelationalOperations.Relation.overlaps;
			}
			return com.epl.geometry.RelationalOperations.Relation.unknown;
		}

		// Checks whether the scl string is the equals relation.
		private static bool Equals_(string scl)
		{
			// Valid for all
			if (scl[0] == 'T' && scl[1] == '*' && scl[2] == 'F' && scl[3] == '*' && scl[4] == '*' && scl[5] == 'F' && scl[6] == 'F' && scl[7] == 'F' && scl[8] == '*')
			{
				return true;
			}
			return false;
		}

		// Checks whether the scl string is the disjoint relation.
		private static bool Disjoint_(string scl)
		{
			if (scl[0] == 'F' && scl[1] == 'F' && scl[2] == '*' && scl[3] == 'F' && scl[4] == 'F' && scl[5] == '*' && scl[6] == '*' && scl[7] == '*' && scl[8] == '*')
			{
				return true;
			}
			return false;
		}

		// Checks whether the scl string is the touches relation.
		private static bool Touches_(string scl, int dim_a, int dim_b)
		{
			// Points cant touch
			if (dim_a == 0 && dim_b == 0)
			{
				return false;
			}
			if (!(dim_a == 2 && dim_b == 2))
			{
				// Valid for area-Line, Line-Line, area-Point, and Line-Point
				if (scl[0] == 'F' && scl[1] == '*' && scl[2] == '*' && scl[3] == 'T' && scl[4] == '*' && scl[5] == '*' && scl[6] == '*' && scl[7] == '*' && scl[8] == '*')
				{
					return true;
				}
				if (dim_a == 1 && dim_b == 1)
				{
					// Valid for Line-Line
					if (scl[0] == 'F' && scl[1] == 'T' && scl[2] == '*' && scl[3] == '*' && scl[4] == '*' && scl[5] == '*' && scl[6] == '*' && scl[7] == '*' && scl[8] == '*')
					{
						return true;
					}
				}
			}
			// Valid for area-area, area-Line, Line-Line
			if (dim_b != 0)
			{
				if (scl[0] == 'F' && scl[1] == '*' && scl[2] == '*' && scl[3] == '*' && scl[4] == 'T' && scl[5] == '*' && scl[6] == '*' && scl[7] == '*' && scl[8] == '*')
				{
					return true;
				}
			}
			return false;
		}

		// Checks whether the scl string is the crosses relation.
		private static bool Crosses_(string scl, int dim_a, int dim_b)
		{
			if (dim_a > dim_b)
			{
				// Valid for area-Line, area-Point, Line-Point
				if (scl[0] == 'T' && scl[1] == '*' && scl[2] == '*' && scl[3] == '*' && scl[4] == '*' && scl[5] == '*' && scl[6] == 'T' && scl[7] == '*' && scl[8] == '*')
				{
					return true;
				}
				return false;
			}
			if (dim_a == 1 && dim_b == 1)
			{
				// Valid for Line-Line
				if (scl[0] == '0' && scl[1] == '*' && scl[2] == '*' && scl[3] == '*' && scl[4] == '*' && scl[5] == '*' && scl[6] == '*' && scl[7] == '*' && scl[8] == '*')
				{
					return true;
				}
			}
			return false;
		}

		// Checks whether the scl string is the contains relation.
		private static bool Contains_(string scl)
		{
			// Valid for all
			if (scl[0] == 'T' && scl[1] == '*' && scl[2] == '*' && scl[3] == '*' && scl[4] == '*' && scl[5] == '*' && scl[6] == 'F' && scl[7] == 'F' && scl[8] == '*')
			{
				return true;
			}
			return false;
		}

		// Checks whether the scl string is the overlaps relation.
		private static bool Overlaps_(string scl, int dim_a, int dim_b)
		{
			if (dim_a == dim_b)
			{
				if (dim_a != 1)
				{
					// Valid for area-area, Point-Point
					if (scl[0] == 'T' && scl[1] == '*' && scl[2] == 'T' && scl[3] == '*' && scl[4] == '*' && scl[5] == '*' && scl[6] == 'T' && scl[7] == '*' && scl[8] == '*')
					{
						return true;
					}
					return false;
				}
				// Valid for Line-Line
				if (scl[0] == '1' && scl[1] == '*' && scl[2] == 'T' && scl[3] == '*' && scl[4] == '*' && scl[5] == '*' && scl[6] == 'T' && scl[7] == '*' && scl[8] == '*')
				{
					return true;
				}
			}
			return false;
		}

		// Marks each cluster of the topoGraph as belonging to an interior vertex of
		// the geometry and/or a boundary index of the geometry.
		private static void MarkClusterEndPoints_(int geometry, com.epl.geometry.TopoGraph topoGraph, int clusterIndex)
		{
			int id = topoGraph.GetGeometryID(geometry);
			for (int cluster = topoGraph.GetFirstCluster(); cluster != -1; cluster = topoGraph.GetNextCluster(cluster))
			{
				int cluster_parentage = topoGraph.GetClusterParentage(cluster);
				if ((cluster_parentage & id) == 0)
				{
					continue;
				}
				int first_half_edge = topoGraph.GetClusterHalfEdge(cluster);
				if (first_half_edge == -1)
				{
					topoGraph.SetClusterUserIndex(cluster, clusterIndex, 0);
					continue;
				}
				int next_half_edge = first_half_edge;
				int index = 0;
				do
				{
					int half_edge = next_half_edge;
					int half_edge_parentage = topoGraph.GetHalfEdgeParentage(half_edge);
					if ((half_edge_parentage & id) != 0)
					{
						index++;
					}
					next_half_edge = topoGraph.GetHalfEdgeNext(topoGraph.GetHalfEdgeTwin(half_edge));
				}
				while (next_half_edge != first_half_edge);
				topoGraph.SetClusterUserIndex(cluster, clusterIndex, index);
			}
			return;
		}

		private static string GetTransposeMatrix_(string scl)
		{
			string transpose = "";
			transpose += scl[0];
			transpose += scl[3];
			transpose += scl[6];
			transpose += scl[1];
			transpose += scl[4];
			transpose += scl[7];
			transpose += scl[2];
			transpose += scl[5];
			transpose += scl[8];
			return transpose;
		}

		// Allocates the matrix array if need be, and sets all entries to -2.
		// -2: Not Computed
		// -1: No intersection
		// 0: 0-dimension intersection
		// 1: 1-dimension intersection
		// 2: 2-dimension intersection
		private void ResetMatrix_()
		{
			for (int i = 0; i < 9; i++)
			{
				m_matrix[i] = -2;
				m_max_dim[i] = -2;
			}
		}

		private static void TransposeMatrix_(int[] matrix)
		{
			int temp1 = matrix[1];
			int temp2 = matrix[2];
			int temp5 = matrix[5];
			matrix[1] = matrix[3];
			matrix[2] = matrix[6];
			matrix[5] = matrix[7];
			matrix[3] = temp1;
			matrix[6] = temp2;
			matrix[7] = temp5;
		}

		// Sets the relation predicates from the scl string.
		private void SetPredicates_(string scl)
		{
			m_scl = scl;
			for (int i = 0; i < 9; i++)
			{
				if (m_scl[i] != '*')
				{
					m_perform_predicates[i] = true;
					m_predicate_count++;
				}
				else
				{
					m_perform_predicates[i] = false;
				}
			}
		}

		// Sets the remaining predicates to false
		private void SetRemainingPredicatesToFalse_()
		{
			for (int i = 0; i < 9; i++)
			{
				if (m_perform_predicates[i] && m_matrix[i] == -2)
				{
					m_matrix[i] = -1;
					m_perform_predicates[i] = false;
				}
			}
		}

		// Checks whether the predicate is known.
		private bool IsPredicateKnown_(int predicate)
		{
			System.Diagnostics.Debug.Assert((m_scl[predicate] != '*'));
			if (m_matrix[predicate] == -2)
			{
				return false;
			}
			if (m_matrix[predicate] == -1)
			{
				m_perform_predicates[predicate] = false;
				m_predicate_count--;
				return true;
			}
			if (m_scl[predicate] != 'T' && m_scl[predicate] != 'F')
			{
				if (m_matrix[predicate] < m_max_dim[predicate])
				{
					return false;
				}
				else
				{
					m_perform_predicates[predicate] = false;
					m_predicate_count--;
					return true;
				}
			}
			else
			{
				m_perform_predicates[predicate] = false;
				m_predicate_count--;
				return true;
			}
		}

		// Sets the area-area predicates function.
		private void SetAreaAreaPredicates_()
		{
			m_predicates_half_edge = com.epl.geometry.RelationalOperationsMatrix.Predicates.AreaAreaPredicates;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 2;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = 1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 2;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = 1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = 1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = 1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = 2;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = 1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = 2;
			// set predicates that are always true/false
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = 2;
				// Always true
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = false;
				m_predicate_count--;
			}
		}

		// Sets the area-line predicate function.
		private void SetAreaLinePredicates_()
		{
			m_predicates_half_edge = com.epl.geometry.RelationalOperationsMatrix.Predicates.AreaLinePredicates;
			m_predicates_cluster = com.epl.geometry.RelationalOperationsMatrix.Predicates.AreaPointPredicates;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 2;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = 1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = 1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = 1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = 2;
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = 2;
				// Always true
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = false;
				m_predicate_count--;
			}
		}

		// Sets the line-line predicates function.
		private void SetLineLinePredicates_()
		{
			m_predicates_half_edge = com.epl.geometry.RelationalOperationsMatrix.Predicates.LineLinePredicates;
			m_predicates_cluster = com.epl.geometry.RelationalOperationsMatrix.Predicates.LinePointPredicates;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = 1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = 2;
			// set predicates that are always true/false
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = 2;
				// Always true
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = false;
				m_predicate_count--;
			}
		}

		// Sets the area-point predicate function.
		private void SetAreaPointPredicates_()
		{
			m_predicates_cluster = com.epl.geometry.RelationalOperationsMatrix.Predicates.AreaPointPredicates;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = -1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 2;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = -1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = 1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = -1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = 2;
			// set predicates that are always true/false
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = -1;
				// Always false
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = false;
				m_predicate_count--;
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = -1;
				// Always false
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = false;
				m_predicate_count--;
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = -1;
				// Always false
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = false;
				m_predicate_count--;
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = 2;
				// Always true
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = false;
				m_predicate_count--;
			}
		}

		// Sets the line-point predicates function.
		private void SetLinePointPredicates_()
		{
			m_predicates_cluster = com.epl.geometry.RelationalOperationsMatrix.Predicates.LinePointPredicates;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = -1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = -1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = -1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = 2;
			// set predicates that are always true/false
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = -1;
				// Always false
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = false;
				m_predicate_count--;
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = -1;
				// Always false
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = false;
				m_predicate_count--;
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = -1;
				// Always false
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = false;
				m_predicate_count--;
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = 2;
				// Always true
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = false;
				m_predicate_count--;
			}
		}

		// Sets the point-point predicates function.
		private void SetPointPointPredicates_()
		{
			m_predicates_cluster = com.epl.geometry.RelationalOperationsMatrix.Predicates.PointPointPredicates;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = -1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = -1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = -1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = -1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = 0;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = -1;
			m_max_dim[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = 2;
			// set predicates that are always true/false
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = -1;
				// Always false
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = false;
				m_predicate_count--;
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = -1;
				// Always false
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = false;
				m_predicate_count--;
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = -1;
				// Always false
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = false;
				m_predicate_count--;
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = -1;
				// Always false
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = false;
				m_predicate_count--;
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = -1;
				// Always false
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = false;
				m_predicate_count--;
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior])
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = 2;
				// Always true
				m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorExterior] = false;
				m_predicate_count--;
			}
		}

		// Invokes the 9 relational predicates of area vs area.
		private bool AreaAreaPredicates_(int half_edge, int id_a, int id_b)
		{
			bool bRelationKnown = true;
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior])
			{
				InteriorAreaInteriorArea_(half_edge, id_a, id_b);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary])
			{
				InteriorAreaBoundaryArea_(half_edge, id_a, com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior])
			{
				InteriorAreaExteriorArea_(half_edge, id_a, id_b, com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior])
			{
				InteriorAreaBoundaryArea_(half_edge, id_b, com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary])
			{
				BoundaryAreaBoundaryArea_(half_edge, id_a, id_b);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior])
			{
				BoundaryAreaExteriorArea_(half_edge, id_a, id_b, com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior])
			{
				InteriorAreaExteriorArea_(half_edge, id_b, id_a, com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary])
			{
				BoundaryAreaExteriorArea_(half_edge, id_b, id_a, com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary);
			}
			return bRelationKnown;
		}

		private void AreaAreaDisjointPredicates_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polygon polygon_b)
		{
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = -1;
			AreaGeomContainsOrDisjointPredicates_(polygon_a, m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] ? com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior : -1, m_scl[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate
				.InteriorExterior], m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] ? com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior : -1, m_scl[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior
				]);
			AreaGeomContainsOrDisjointPredicates_(polygon_b, m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] ? com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior : -1, m_scl[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate
				.ExteriorInterior], m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] ? com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary : -1, m_scl[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary
				]);
		}

		private void AreaGeomContainsOrDisjointPredicates_(com.epl.geometry.Polygon polygon, int matrix_interior, char c1, int matrix_boundary, char c2)
		{
			if (matrix_interior != -1 || matrix_boundary != -1)
			{
				bool has_area = ((c1 != 'T' && c1 != 'F' && matrix_interior != -1) || (c2 != 'T' && c2 != 'F' && matrix_boundary != -1) ? polygon.CalculateArea2D() != 0 : true);
				if (has_area)
				{
					if (matrix_interior != -1)
					{
						m_matrix[matrix_interior] = 2;
					}
					if (matrix_boundary != -1)
					{
						m_matrix[matrix_boundary] = 1;
					}
				}
				else
				{
					if (matrix_boundary != -1)
					{
						m_matrix[matrix_boundary] = -1;
					}
					if (matrix_interior != -1)
					{
						com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
						polygon.QueryEnvelope2D(env);
						m_matrix[matrix_interior] = (env.GetHeight() == 0.0 && env.GetWidth() == 0.0 ? 0 : 1);
					}
				}
			}
		}

		private void AreaAreaContainsPredicates_(com.epl.geometry.Polygon polygon_b)
		{
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 2;
			// im assuming its a proper contains.
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = 1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = -1;
			AreaGeomContainsOrDisjointPredicates_(polygon_b, m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] ? com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior : -1, m_scl[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate
				.InteriorInterior], m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] ? com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary : -1, m_scl[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary
				]);
		}

		// all other predicates should already be set by set_area_area_predicates
		private void AreaAreaWithinPredicates_(com.epl.geometry.Polygon polygon_a)
		{
			AreaAreaContainsPredicates_(polygon_a);
			TransposeMatrix_(m_matrix);
		}

		private void AreaLineDisjointPredicates_(com.epl.geometry.Polygon polygon, com.epl.geometry.Polyline polyline)
		{
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = -1;
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior])
			{
				char c = m_scl[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior];
				bool b_has_length = (c != 'T' && c != 'F' ? polyline.CalculateLength2D() != 0 : true);
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = (b_has_length ? 1 : 0);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary])
			{
				bool has_non_empty_boundary = com.epl.geometry.Boundary.HasNonEmptyBoundary(polyline, null);
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = has_non_empty_boundary ? 0 : -1;
			}
			AreaGeomContainsOrDisjointPredicates_(polygon, m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] ? com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior : -1, m_scl[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate
				.InteriorExterior], m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] ? com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior : -1, m_scl[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior
				]);
		}

		private void AreaLineContainsPredicates_(com.epl.geometry.Polygon polygon, com.epl.geometry.Polyline polyline)
		{
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior])
			{
				char c = m_scl[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior];
				bool b_has_length = (c != 'T' && c != 'F' ? polyline.CalculateLength2D() != 0 : true);
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = (b_has_length ? 1 : 0);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary])
			{
				bool has_non_empty_boundary = com.epl.geometry.Boundary.HasNonEmptyBoundary(polyline, null);
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = has_non_empty_boundary ? 0 : -1;
			}
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 2;
			//assume polygon has area
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = 1;
			//assume polygon has area
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = -1;
		}

		private void AreaPointDisjointPredicates_(com.epl.geometry.Polygon polygon)
		{
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = 0;
			AreaGeomContainsOrDisjointPredicates_(polygon, m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] ? com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior : -1, m_scl[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate
				.InteriorExterior], m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] ? com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior : -1, m_scl[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior
				]);
		}

		private void AreaPointContainsPredicates_(com.epl.geometry.Polygon polygon)
		{
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 0;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 2;
			//assume polygon has area
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = 1;
			//assume polygon has area
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = -1;
		}

		private void LineLineDisjointPredicates_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Polyline polyline_b)
		{
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = -1;
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior])
			{
				char c = m_scl[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior];
				bool b_has_length = (c != 'T' && c != 'F' ? polyline_a.CalculateLength2D() != 0 : true);
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = (b_has_length ? 1 : 0);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior])
			{
				bool has_non_empty_boundary_a = com.epl.geometry.Boundary.HasNonEmptyBoundary(polyline_a, null);
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = has_non_empty_boundary_a ? 0 : -1;
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior])
			{
				char c = m_scl[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior];
				bool b_has_length = (c != 'T' && c != 'F' ? polyline_b.CalculateLength2D() != 0 : true);
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = (b_has_length ? 1 : 0);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary])
			{
				bool has_non_empty_boundary_b = com.epl.geometry.Boundary.HasNonEmptyBoundary(polyline_b, null);
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = has_non_empty_boundary_b ? 0 : -1;
			}
		}

		private void LinePointDisjointPredicates_(com.epl.geometry.Polyline polyline)
		{
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = -1;
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior])
			{
				char c = m_scl[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior];
				bool b_has_length = (c != 'T' && c != 'F' ? polyline.CalculateLength2D() != 0 : true);
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = (b_has_length ? 1 : 0);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior])
			{
				bool has_non_empty_boundary = com.epl.geometry.Boundary.HasNonEmptyBoundary(polyline, null);
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = (has_non_empty_boundary ? 0 : -1);
			}
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = 0;
		}

		private void PointPointDisjointPredicates_()
		{
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = -1;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 0;
			m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = 0;
		}

		// Invokes the 9 relational predicates of area vs Line.
		private bool AreaLinePredicates_(int half_edge, int id_a, int id_b)
		{
			bool bRelationKnown = true;
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior])
			{
				InteriorAreaInteriorLine_(half_edge, id_a, id_b);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary])
			{
				InteriorAreaBoundaryLine_(half_edge, id_a, id_b, m_cluster_index_b);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior])
			{
				InteriorAreaExteriorLine_(half_edge, id_a, id_b);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior])
			{
				BoundaryAreaInteriorLine_(half_edge, id_a, id_b, m_cluster_index_b);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary])
			{
				BoundaryAreaBoundaryLine_(half_edge, id_a, id_b, m_cluster_index_b);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior])
			{
				BoundaryAreaExteriorLine_(half_edge, id_a, id_b);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior])
			{
				ExteriorAreaInteriorLine_(half_edge, id_a);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary])
			{
				ExteriorAreaBoundaryLine_(half_edge, id_a, id_b, m_cluster_index_b);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary);
			}
			return bRelationKnown;
		}

		// Invokes the 9 relational predicates of Line vs Line.
		private bool LineLinePredicates_(int half_edge, int id_a, int id_b)
		{
			bool bRelationKnown = true;
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior])
			{
				InteriorLineInteriorLine_(half_edge, id_a, id_b, m_cluster_index_a, m_cluster_index_b);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary])
			{
				InteriorLineBoundaryLine_(half_edge, id_a, id_b, m_cluster_index_a, m_cluster_index_b, com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior])
			{
				InteriorLineExteriorLine_(half_edge, id_a, id_b, com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior])
			{
				InteriorLineBoundaryLine_(half_edge, id_b, id_a, m_cluster_index_b, m_cluster_index_a, com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary])
			{
				BoundaryLineBoundaryLine_(half_edge, id_a, id_b, m_cluster_index_a, m_cluster_index_b);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior])
			{
				BoundaryLineExteriorLine_(half_edge, id_a, id_b, m_cluster_index_a, com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior])
			{
				InteriorLineExteriorLine_(half_edge, id_b, id_a, com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary])
			{
				BoundaryLineExteriorLine_(half_edge, id_b, id_a, m_cluster_index_b, com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary);
			}
			return bRelationKnown;
		}

		// Invokes the 9 relational predicates of area vs Point.
		private bool AreaPointPredicates_(int cluster, int id_a, int id_b)
		{
			bool bRelationKnown = true;
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior])
			{
				InteriorAreaInteriorPoint_(cluster, id_a);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior])
			{
				InteriorAreaExteriorPoint_(cluster, id_a);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior])
			{
				BoundaryAreaInteriorPoint_(cluster, id_a, id_b);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior])
			{
				BoundaryAreaExteriorPoint_(cluster, id_a);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior])
			{
				ExteriorAreaInteriorPoint_(cluster, id_a);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior);
			}
			return bRelationKnown;
		}

		// Invokes the 9 relational predicates of Line vs Point.
		private bool LinePointPredicates_(int cluster, int id_a, int id_b)
		{
			bool bRelationKnown = true;
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior])
			{
				InteriorLineInteriorPoint_(cluster, id_a, id_b, m_cluster_index_a);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior])
			{
				InteriorLineExteriorPoint_(cluster, id_a, id_b, m_cluster_index_a);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior])
			{
				BoundaryLineInteriorPoint_(cluster, id_a, id_b, m_cluster_index_a);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior])
			{
				BoundaryLineExteriorPoint_(cluster, id_a, id_b, m_cluster_index_a);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior])
			{
				ExteriorLineInteriorPoint_(cluster, id_a, id_b);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior);
			}
			return bRelationKnown;
		}

		// Invokes the 9 relational predicates of Point vs Point.
		private bool PointPointPredicates_(int cluster, int id_a, int id_b)
		{
			bool bRelationKnown = true;
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior])
			{
				InteriorPointInteriorPoint_(cluster, id_a, id_b);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior])
			{
				InteriorPointExteriorPoint_(cluster, id_a, id_b, com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior);
			}
			if (m_perform_predicates[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior])
			{
				InteriorPointExteriorPoint_(cluster, id_b, id_a, com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior);
				bRelationKnown &= IsPredicateKnown_(com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior);
			}
			return bRelationKnown;
		}

		// Relational predicate to determine if the interior of area A intersects
		// with the interior of area B.
		private void InteriorAreaInteriorArea_(int half_edge, int id_a, int id_b)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] == 2)
			{
				return;
			}
			int faceParentage = m_topo_graph.GetHalfEdgeFaceParentage(half_edge);
			if ((faceParentage & id_a) != 0 && (faceParentage & id_b) != 0)
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 2;
			}
		}

		// Relational predicate to determine if the interior of area A intersects
		// with the boundary of area B.
		private void InteriorAreaBoundaryArea_(int half_edge, int id_a, int predicate)
		{
			if (m_matrix[predicate] == 1)
			{
				return;
			}
			int faceParentage = m_topo_graph.GetHalfEdgeFaceParentage(half_edge);
			int twinFaceParentage = m_topo_graph.GetHalfEdgeFaceParentage(m_topo_graph.GetHalfEdgeTwin(half_edge));
			if ((faceParentage & id_a) != 0 && (twinFaceParentage & id_a) != 0)
			{
				m_matrix[predicate] = 1;
			}
		}

		// Relational predicate to determine if the interior of area A intersects
		// with the exterior of area B.
		private void InteriorAreaExteriorArea_(int half_edge, int id_a, int id_b, int predicate)
		{
			if (m_matrix[predicate] == 2)
			{
				return;
			}
			int faceParentage = m_topo_graph.GetHalfEdgeFaceParentage(half_edge);
			if ((faceParentage & id_a) != 0 && (faceParentage & id_b) == 0)
			{
				m_matrix[predicate] = 2;
			}
		}

		// Relational predicate to determine if the boundary of area A intersects
		// with the boundary of area B.
		private void BoundaryAreaBoundaryArea_(int half_edge, int id_a, int id_b)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] == 1)
			{
				return;
			}
			int parentage = m_topo_graph.GetHalfEdgeParentage(half_edge);
			if ((parentage & id_a) != 0 && (parentage & id_b) != 0)
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = 1;
				return;
			}
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] != 0)
			{
				if (m_topo_graph.GetHalfEdgeUserIndex(m_topo_graph.GetHalfEdgePrev(m_topo_graph.GetHalfEdgeTwin(half_edge)), m_visited_index) != 1)
				{
					int cluster = m_topo_graph.GetHalfEdgeTo(half_edge);
					int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
					if ((clusterParentage & id_a) != 0 && (clusterParentage & id_b) != 0)
					{
						m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = 0;
					}
				}
			}
		}

		// Relational predicate to determine if the boundary of area A intersects
		// with the exterior of area B.
		private void BoundaryAreaExteriorArea_(int half_edge, int id_a, int id_b, int predicate)
		{
			if (m_matrix[predicate] == 1)
			{
				return;
			}
			int faceParentage = m_topo_graph.GetHalfEdgeFaceParentage(half_edge);
			int twinFaceParentage = m_topo_graph.GetHalfEdgeFaceParentage(m_topo_graph.GetHalfEdgeTwin(half_edge));
			if ((faceParentage & id_b) == 0 && (twinFaceParentage & id_b) == 0)
			{
				m_matrix[predicate] = 1;
			}
		}

		// Relational predicate to determine if the interior of area A intersects
		// with the interior of Line B.
		private void InteriorAreaInteriorLine_(int half_edge, int id_a, int id_b)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] == 1)
			{
				return;
			}
			int faceParentage = m_topo_graph.GetHalfEdgeFaceParentage(half_edge);
			int twinFaceParentage = m_topo_graph.GetHalfEdgeFaceParentage(m_topo_graph.GetHalfEdgeTwin(half_edge));
			if ((faceParentage & id_a) != 0 && (twinFaceParentage & id_a) != 0)
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 1;
			}
		}

		// Relational predicate to determine if the interior of area A intersects
		// with the boundary of Line B.
		private void InteriorAreaBoundaryLine_(int half_edge, int id_a, int id_b, int cluster_index_b)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] == 0)
			{
				return;
			}
			if (m_topo_graph.GetHalfEdgeUserIndex(m_topo_graph.GetHalfEdgePrev(m_topo_graph.GetHalfEdgeTwin(half_edge)), m_visited_index) != 1)
			{
				int cluster = m_topo_graph.GetHalfEdgeTo(half_edge);
				int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
				if ((clusterParentage & id_a) == 0)
				{
					int faceParentage = m_topo_graph.GetHalfEdgeFaceParentage(half_edge);
					if ((faceParentage & id_a) != 0)
					{
						int index = m_topo_graph.GetClusterUserIndex(cluster, cluster_index_b);
						if ((clusterParentage & id_b) != 0 && (index % 2 != 0))
						{
							System.Diagnostics.Debug.Assert((index != -1));
							m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorBoundary] = 0;
						}
					}
				}
			}
		}

		private void InteriorAreaExteriorLine_(int half_edge, int id_a, int id_b)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] == 2)
			{
				return;
			}
			int half_edge_parentage = m_topo_graph.GetHalfEdgeParentage(half_edge);
			if ((half_edge_parentage & id_a) != 0)
			{
				//half edge of polygon
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 2;
			}
		}

		// Relational predicate to determine if the boundary of area A intersects
		// with the interior of Line B.
		private void BoundaryAreaInteriorLine_(int half_edge, int id_a, int id_b, int cluster_index_b)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] == 1)
			{
				return;
			}
			int parentage = m_topo_graph.GetHalfEdgeParentage(half_edge);
			if ((parentage & id_a) != 0 && (parentage & id_b) != 0)
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = 1;
				return;
			}
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] != 0)
			{
				if (m_topo_graph.GetHalfEdgeUserIndex(m_topo_graph.GetHalfEdgePrev(m_topo_graph.GetHalfEdgeTwin(half_edge)), m_visited_index) != 1)
				{
					int cluster = m_topo_graph.GetHalfEdgeTo(half_edge);
					int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
					if ((clusterParentage & id_a) != 0)
					{
						int index = m_topo_graph.GetClusterUserIndex(cluster, cluster_index_b);
						if ((clusterParentage & id_b) != 0 && (index % 2 == 0))
						{
							System.Diagnostics.Debug.Assert((index != -1));
							m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = 0;
						}
					}
				}
			}
		}

		// Relational predicate to determine if the boundary of area A intersects
		// with the boundary of Line B.
		private void BoundaryAreaBoundaryLine_(int half_edge, int id_a, int id_b, int cluster_index_b)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] == 0)
			{
				return;
			}
			if (m_topo_graph.GetHalfEdgeUserIndex(m_topo_graph.GetHalfEdgePrev(m_topo_graph.GetHalfEdgeTwin(half_edge)), m_visited_index) != 1)
			{
				int cluster = m_topo_graph.GetHalfEdgeTo(half_edge);
				int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
				if ((clusterParentage & id_a) != 0)
				{
					int index = m_topo_graph.GetClusterUserIndex(cluster, cluster_index_b);
					if ((clusterParentage & id_b) != 0 && (index % 2 != 0))
					{
						System.Diagnostics.Debug.Assert((index != -1));
						m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = 0;
					}
				}
			}
		}

		// Relational predicate to determine if the boundary of area A intersects
		// with the exterior of Line B.
		private void BoundaryAreaExteriorLine_(int half_edge, int id_a, int id_b)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] == 1)
			{
				return;
			}
			int parentage = m_topo_graph.GetHalfEdgeParentage(half_edge);
			if ((parentage & id_a) != 0 && (parentage & id_b) == 0)
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = 1;
			}
		}

		// Relational predicate to determine if the exterior of area A intersects
		// with the interior of Line B.
		private void ExteriorAreaInteriorLine_(int half_edge, int id_a)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] == 1)
			{
				return;
			}
			int faceParentage = m_topo_graph.GetHalfEdgeFaceParentage(half_edge);
			int twinFaceParentage = m_topo_graph.GetHalfEdgeFaceParentage(m_topo_graph.GetHalfEdgeTwin(half_edge));
			if ((faceParentage & id_a) == 0 && (twinFaceParentage & id_a) == 0)
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = 1;
			}
		}

		// Relational predicate to determine if the exterior of area A intersects
		// with the boundary of Line B.
		private void ExteriorAreaBoundaryLine_(int half_edge, int id_a, int id_b, int cluster_index_b)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] == 0)
			{
				return;
			}
			if (m_topo_graph.GetHalfEdgeUserIndex(m_topo_graph.GetHalfEdgePrev(m_topo_graph.GetHalfEdgeTwin(half_edge)), m_visited_index) != 1)
			{
				int cluster = m_topo_graph.GetHalfEdgeTo(half_edge);
				int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
				if ((clusterParentage & id_a) == 0)
				{
					int faceParentage = m_topo_graph.GetHalfEdgeFaceParentage(half_edge);
					if ((faceParentage & id_a) == 0)
					{
						System.Diagnostics.Debug.Assert(((m_topo_graph.GetHalfEdgeParentage(m_topo_graph.GetHalfEdgeTwin(half_edge)) & id_a) == 0));
						int index = m_topo_graph.GetClusterUserIndex(cluster, cluster_index_b);
						if ((clusterParentage & id_b) != 0 && (index % 2 != 0))
						{
							System.Diagnostics.Debug.Assert((index != -1));
							m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorBoundary] = 0;
						}
					}
				}
			}
		}

		// Relational predicate to determine if the interior of Line A intersects
		// with the interior of Line B.
		private void InteriorLineInteriorLine_(int half_edge, int id_a, int id_b, int cluster_index_a, int cluster_index_b)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] == 1)
			{
				return;
			}
			int parentage = m_topo_graph.GetHalfEdgeParentage(half_edge);
			if ((parentage & id_a) != 0 && (parentage & id_b) != 0)
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 1;
				return;
			}
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] != 0)
			{
				if (m_topo_graph.GetHalfEdgeUserIndex(m_topo_graph.GetHalfEdgePrev(m_topo_graph.GetHalfEdgeTwin(half_edge)), m_visited_index) != 1)
				{
					int cluster = m_topo_graph.GetHalfEdgeTo(half_edge);
					int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
					if ((clusterParentage & id_a) != 0 && (clusterParentage & id_b) != 0)
					{
						int index_a = m_topo_graph.GetClusterUserIndex(cluster, cluster_index_a);
						int index_b = m_topo_graph.GetClusterUserIndex(cluster, cluster_index_b);
						System.Diagnostics.Debug.Assert((index_a != -1));
						System.Diagnostics.Debug.Assert((index_b != -1));
						if ((index_a % 2 == 0) && (index_b % 2 == 0))
						{
							System.Diagnostics.Debug.Assert(((m_topo_graph.GetClusterParentage(cluster) & id_a) != 0 && (m_topo_graph.GetClusterParentage(cluster) & id_b) != 0));
							m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 0;
						}
					}
				}
			}
		}

		// Relational predicate to determine of the interior of LineA intersects
		// with the boundary of Line B.
		private void InteriorLineBoundaryLine_(int half_edge, int id_a, int id_b, int cluster_index_a, int cluster_index_b, int predicate)
		{
			if (m_matrix[predicate] == 0)
			{
				return;
			}
			if (m_topo_graph.GetHalfEdgeUserIndex(m_topo_graph.GetHalfEdgePrev(m_topo_graph.GetHalfEdgeTwin(half_edge)), m_visited_index) != 1)
			{
				int cluster = m_topo_graph.GetHalfEdgeTo(half_edge);
				int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
				if ((clusterParentage & id_a) != 0 && (clusterParentage & id_b) != 0)
				{
					int index_a = m_topo_graph.GetClusterUserIndex(cluster, cluster_index_a);
					int index_b = m_topo_graph.GetClusterUserIndex(cluster, cluster_index_b);
					System.Diagnostics.Debug.Assert((index_a != -1));
					System.Diagnostics.Debug.Assert((index_b != -1));
					if ((index_a % 2 == 0) && (index_b % 2 != 0))
					{
						System.Diagnostics.Debug.Assert(((m_topo_graph.GetClusterParentage(cluster) & id_a) != 0 && (m_topo_graph.GetClusterParentage(cluster) & id_b) != 0));
						m_matrix[predicate] = 0;
					}
				}
			}
		}

		// Relational predicate to determine if the interior of Line A intersects
		// with the exterior of Line B.
		private void InteriorLineExteriorLine_(int half_edge, int id_a, int id_b, int predicate)
		{
			if (m_matrix[predicate] == 1)
			{
				return;
			}
			int parentage = m_topo_graph.GetHalfEdgeParentage(half_edge);
			if ((parentage & id_a) != 0 && (parentage & id_b) == 0)
			{
				m_matrix[predicate] = 1;
			}
		}

		// Relational predicate to determine if the boundary of Line A intersects
		// with the boundary of Line B.
		private void BoundaryLineBoundaryLine_(int half_edge, int id_a, int id_b, int cluster_index_a, int cluster_index_b)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] == 0)
			{
				return;
			}
			if (m_topo_graph.GetHalfEdgeUserIndex(m_topo_graph.GetHalfEdgePrev(m_topo_graph.GetHalfEdgeTwin(half_edge)), m_visited_index) != 1)
			{
				int cluster = m_topo_graph.GetHalfEdgeTo(half_edge);
				int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
				if ((clusterParentage & id_a) != 0 && (clusterParentage & id_b) != 0)
				{
					int index_a = m_topo_graph.GetClusterUserIndex(cluster, cluster_index_a);
					int index_b = m_topo_graph.GetClusterUserIndex(cluster, cluster_index_b);
					System.Diagnostics.Debug.Assert((index_a != -1));
					System.Diagnostics.Debug.Assert((index_b != -1));
					if ((index_a % 2 != 0) && (index_b % 2 != 0))
					{
						System.Diagnostics.Debug.Assert(((m_topo_graph.GetClusterParentage(cluster) & id_a) != 0 && (m_topo_graph.GetClusterParentage(cluster) & id_b) != 0));
						m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryBoundary] = 0;
					}
				}
			}
		}

		// Relational predicate to determine if the boundary of Line A intersects
		// with the exterior of Line B.
		private void BoundaryLineExteriorLine_(int half_edge, int id_a, int id_b, int cluster_index_a, int predicate)
		{
			if (m_matrix[predicate] == 0)
			{
				return;
			}
			if (m_topo_graph.GetHalfEdgeUserIndex(m_topo_graph.GetHalfEdgePrev(m_topo_graph.GetHalfEdgeTwin(half_edge)), m_visited_index) != 1)
			{
				int cluster = m_topo_graph.GetHalfEdgeTo(half_edge);
				int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
				if ((clusterParentage & id_b) == 0)
				{
					int index = m_topo_graph.GetClusterUserIndex(cluster, cluster_index_a);
					System.Diagnostics.Debug.Assert((index != -1));
					if (index % 2 != 0)
					{
						System.Diagnostics.Debug.Assert(((m_topo_graph.GetClusterParentage(cluster) & id_a) != 0));
						m_matrix[predicate] = 0;
					}
				}
			}
		}

		// Relational predicate to determine if the interior of area A intersects
		// with the interior of Point B.
		private void InteriorAreaInteriorPoint_(int cluster, int id_a)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] == 0)
			{
				return;
			}
			int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
			if ((clusterParentage & id_a) == 0)
			{
				int chain = m_topo_graph.GetClusterChain(cluster);
				int chainParentage = m_topo_graph.GetChainParentage(chain);
				if ((chainParentage & id_a) != 0)
				{
					m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 0;
				}
			}
		}

		private void InteriorAreaExteriorPoint_(int cluster, int id_a)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] == 2)
			{
				return;
			}
			int cluster_parentage = m_topo_graph.GetClusterParentage(cluster);
			if ((cluster_parentage & id_a) != 0)
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 2;
			}
		}

		// Relational predicate to determine if the boundary of area A intersects
		// with the interior of Point B.
		private void BoundaryAreaInteriorPoint_(int cluster, int id_a, int id_b)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] == 0)
			{
				return;
			}
			int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
			if ((clusterParentage & id_a) != 0 && (clusterParentage & id_b) != 0)
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = 0;
			}
		}

		private void BoundaryAreaExteriorPoint_(int cluster, int id_a)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] == 1)
			{
				return;
			}
			int cluster_parentage = m_topo_graph.GetClusterParentage(cluster);
			if ((cluster_parentage & id_a) != 0)
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = 1;
			}
		}

		// Relational predicate to determine if the exterior of area A intersects
		// with the interior of Point B.
		private void ExteriorAreaInteriorPoint_(int cluster, int id_a)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] == 0)
			{
				return;
			}
			int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
			if ((clusterParentage & id_a) == 0)
			{
				int chain = m_topo_graph.GetClusterChain(cluster);
				int chainParentage = m_topo_graph.GetChainParentage(chain);
				if ((chainParentage & id_a) == 0)
				{
					m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = 0;
				}
			}
		}

		// Relational predicate to determine if the interior of Line A intersects
		// with the interior of Point B.
		private void InteriorLineInteriorPoint_(int cluster, int id_a, int id_b, int cluster_index_a)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] == 0)
			{
				return;
			}
			int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
			if ((clusterParentage & id_a) != 0 && (clusterParentage & id_b) != 0)
			{
				int index = m_topo_graph.GetClusterUserIndex(cluster, cluster_index_a);
				if (index % 2 == 0)
				{
					m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 0;
				}
			}
		}

		private void InteriorLineExteriorPoint_(int cluster, int id_a, int id_b, int cluster_index_a)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] == 1)
			{
				return;
			}
			int half_edge_a = m_topo_graph.GetClusterHalfEdge(cluster);
			if (half_edge_a != -1)
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 1;
				return;
			}
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] != 0)
			{
				int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
				if ((clusterParentage & id_b) == 0)
				{
					System.Diagnostics.Debug.Assert((m_topo_graph.GetClusterUserIndex(cluster, cluster_index_a) % 2 == 0));
					m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorExterior] = 0;
					return;
				}
			}
			return;
		}

		// Relational predicate to determine if the boundary of Line A intersects
		// with the interior of Point B.
		private void BoundaryLineInteriorPoint_(int cluster, int id_a, int id_b, int cluster_index_a)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] == 0)
			{
				return;
			}
			int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
			if ((clusterParentage & id_a) != 0 && (clusterParentage & id_b) != 0)
			{
				int index = m_topo_graph.GetClusterUserIndex(cluster, cluster_index_a);
				if (index % 2 != 0)
				{
					m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryInterior] = 0;
				}
			}
		}

		// Relational predicate to determine if the boundary of Line A intersects
		// with the exterior of Point B.
		private void BoundaryLineExteriorPoint_(int cluster, int id_a, int id_b, int cluster_index_a)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] == 0)
			{
				return;
			}
			int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
			if ((clusterParentage & id_a) != 0 && (clusterParentage & id_b) == 0)
			{
				int index = m_topo_graph.GetClusterUserIndex(cluster, cluster_index_a);
				if (index % 2 != 0)
				{
					m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.BoundaryExterior] = 0;
				}
			}
		}

		// Relational predicate to determine if the exterior of Line A intersects
		// with the interior of Point B.
		private void ExteriorLineInteriorPoint_(int cluster, int id_a, int id_b)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] == 0)
			{
				return;
			}
			int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
			if ((clusterParentage & id_a) == 0 && (clusterParentage & id_b) != 0)
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.ExteriorInterior] = 0;
			}
		}

		// Relational predicate to determine if the interior of Point A intersects
		// with the interior of Point B.
		private void InteriorPointInteriorPoint_(int cluster, int id_a, int id_b)
		{
			if (m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] == 0)
			{
				return;
			}
			int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
			if ((clusterParentage & id_a) != 0 && (clusterParentage & id_b) != 0)
			{
				m_matrix[com.epl.geometry.RelationalOperationsMatrix.MatrixPredicate.InteriorInterior] = 0;
			}
		}

		// Relational predicate to determine if the interior of Point A intersects
		// with the exterior of Point B.
		private void InteriorPointExteriorPoint_(int cluster, int id_a, int id_b, int predicate)
		{
			if (m_matrix[predicate] == 0)
			{
				return;
			}
			int clusterParentage = m_topo_graph.GetClusterParentage(cluster);
			if ((clusterParentage & id_a) != 0 && (clusterParentage & id_b) == 0)
			{
				m_matrix[predicate] = 0;
			}
		}

		// Computes the 9 intersection relationships of boundary, interior, and
		// exterior of geometry_a vs geometry_b using the Topo_graph for area/area,
		// area/Line, and Line/Line relations
		private void ComputeMatrixTopoGraphHalfEdges_(int geometry_a, int geometry_b)
		{
			bool bRelationKnown = false;
			int id_a = m_topo_graph.GetGeometryID(geometry_a);
			int id_b = m_topo_graph.GetGeometryID(geometry_b);
			m_visited_index = m_topo_graph.CreateUserIndexForHalfEdges();
			for (int cluster = m_topo_graph.GetFirstCluster(); cluster != -1; cluster = m_topo_graph.GetNextCluster(cluster))
			{
				int first_half_edge = m_topo_graph.GetClusterHalfEdge(cluster);
				if (first_half_edge == -1)
				{
					if (m_predicates_cluster != -1)
					{
						switch (m_predicates_cluster)
						{
							case com.epl.geometry.RelationalOperationsMatrix.Predicates.AreaPointPredicates:
							{
								// Treat cluster as an interior point
								bRelationKnown = AreaPointPredicates_(cluster, id_a, id_b);
								break;
							}

							case com.epl.geometry.RelationalOperationsMatrix.Predicates.LinePointPredicates:
							{
								bRelationKnown = LinePointPredicates_(cluster, id_a, id_b);
								break;
							}

							default:
							{
								throw com.epl.geometry.GeometryException.GeometryInternalError();
							}
						}
					}
					continue;
				}
				int next_half_edge = first_half_edge;
				do
				{
					int half_edge = next_half_edge;
					int visited = m_topo_graph.GetHalfEdgeUserIndex(half_edge, m_visited_index);
					if (visited != 1)
					{
						do
						{
							switch (m_predicates_half_edge)
							{
								case com.epl.geometry.RelationalOperationsMatrix.Predicates.AreaAreaPredicates:
								{
									// Invoke relational predicates
									bRelationKnown = AreaAreaPredicates_(half_edge, id_a, id_b);
									break;
								}

								case com.epl.geometry.RelationalOperationsMatrix.Predicates.AreaLinePredicates:
								{
									bRelationKnown = AreaLinePredicates_(half_edge, id_a, id_b);
									break;
								}

								case com.epl.geometry.RelationalOperationsMatrix.Predicates.LineLinePredicates:
								{
									bRelationKnown = LineLinePredicates_(half_edge, id_a, id_b);
									break;
								}

								default:
								{
									throw com.epl.geometry.GeometryException.GeometryInternalError();
								}
							}
							if (bRelationKnown)
							{
								break;
							}
							m_topo_graph.SetHalfEdgeUserIndex(half_edge, m_visited_index, 1);
							half_edge = m_topo_graph.GetHalfEdgeNext(half_edge);
						}
						while (half_edge != next_half_edge && !bRelationKnown);
					}
					if (bRelationKnown)
					{
						break;
					}
					next_half_edge = m_topo_graph.GetHalfEdgeNext(m_topo_graph.GetHalfEdgeTwin(half_edge));
				}
				while (next_half_edge != first_half_edge);
				if (bRelationKnown)
				{
					break;
				}
			}
			if (!bRelationKnown)
			{
				SetRemainingPredicatesToFalse_();
			}
			m_topo_graph.DeleteUserIndexForHalfEdges(m_visited_index);
		}

		// Computes the 9 intersection relationships of boundary, interior, and
		// exterior of geometry_a vs geometry_b using the Topo_graph for area/Point,
		// Line/Point, and Point/Point relations
		private void ComputeMatrixTopoGraphClusters_(int geometry_a, int geometry_b)
		{
			bool bRelationKnown = false;
			int id_a = m_topo_graph.GetGeometryID(geometry_a);
			int id_b = m_topo_graph.GetGeometryID(geometry_b);
			for (int cluster = m_topo_graph.GetFirstCluster(); cluster != -1; cluster = m_topo_graph.GetNextCluster(cluster))
			{
				switch (m_predicates_cluster)
				{
					case com.epl.geometry.RelationalOperationsMatrix.Predicates.AreaPointPredicates:
					{
						// Invoke relational predicates
						bRelationKnown = AreaPointPredicates_(cluster, id_a, id_b);
						break;
					}

					case com.epl.geometry.RelationalOperationsMatrix.Predicates.LinePointPredicates:
					{
						bRelationKnown = LinePointPredicates_(cluster, id_a, id_b);
						break;
					}

					case com.epl.geometry.RelationalOperationsMatrix.Predicates.PointPointPredicates:
					{
						bRelationKnown = PointPointPredicates_(cluster, id_a, id_b);
						break;
					}

					default:
					{
						throw com.epl.geometry.GeometryException.GeometryInternalError();
					}
				}
				if (bRelationKnown)
				{
					break;
				}
			}
			if (!bRelationKnown)
			{
				SetRemainingPredicatesToFalse_();
			}
		}

		// Call this method to set the edit shape, if the edit shape has been
		// cracked and clustered already.
		private void SetEditShape_(com.epl.geometry.EditShape shape, com.epl.geometry.ProgressTracker progressTracker)
		{
			m_topo_graph.SetEditShape(shape, progressTracker);
		}

		private void SetEditShapeCrackAndCluster_(com.epl.geometry.EditShape shape, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			EditShapeCrackAndCluster_(shape, tolerance, progress_tracker);
			SetEditShape_(shape, progress_tracker);
		}

		private void EditShapeCrackAndCluster_(com.epl.geometry.EditShape shape, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.CrackAndCluster.Execute(shape, tolerance, progress_tracker, false);
			//do not filter degenerate segments.
			shape.FilterClosePoints(0, true, true);
			//remove degeneracies from polygon geometries.
			for (int geometry = shape.GetFirstGeometry(); geometry != -1; geometry = shape.GetNextGeometry(geometry))
			{
				if (shape.GetGeometryType(geometry) == com.epl.geometry.Geometry.Type.Polygon.Value())
				{
					com.epl.geometry.Simplificator.Execute(shape, geometry, -1, false, progress_tracker);
				}
			}
		}

		// Upgrades the geometry to a feature geometry.
		private static com.epl.geometry.Geometry ConvertGeometry_(com.epl.geometry.Geometry geometry, double tolerance)
		{
			int gt = geometry.GetType().Value();
			if (com.epl.geometry.Geometry.IsSegment(gt))
			{
				com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline(geometry.GetDescription());
				polyline.AddSegment((com.epl.geometry.Segment)geometry, true);
				return polyline;
			}
			if (gt == com.epl.geometry.Geometry.GeometryType.Envelope)
			{
				com.epl.geometry.Envelope envelope = (com.epl.geometry.Envelope)(geometry);
				com.epl.geometry.Envelope2D env = new com.epl.geometry.Envelope2D();
				geometry.QueryEnvelope2D(env);
				if (env.GetHeight() <= tolerance && env.GetWidth() <= tolerance)
				{
					// treat
					// as
					// point
					com.epl.geometry.Point point = new com.epl.geometry.Point(geometry.GetDescription());
					envelope.GetCenter(point);
					return point;
				}
				if (env.GetHeight() <= tolerance || env.GetWidth() <= tolerance)
				{
					// treat
					// as
					// line
					com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline(geometry.GetDescription());
					com.epl.geometry.Point p = new com.epl.geometry.Point();
					envelope.QueryCornerByVal(0, p);
					polyline.StartPath(p);
					envelope.QueryCornerByVal(2, p);
					polyline.LineTo(p);
					return polyline;
				}
				// treat as polygon
				com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon(geometry.GetDescription());
				polygon.AddEnvelope(envelope, false);
				return polygon;
			}
			return geometry;
		}
	}
}
