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


namespace com.epl.geometry
{
	internal class RelationalOperations
	{
		internal abstract class Relation
		{
			public const int contains = 1;

			public const int within = 2;

			public const int equals = 3;

			public const int disjoint = 4;

			public const int touches = 8;

			public const int crosses = 16;

			public const int overlaps = 32;

			public const int unknown = 0;

			public const int intersects = unchecked((int)(0x40000000));
		}

		internal static class RelationConstants
		{
		}

		internal static bool Relate(com.epl.geometry.Geometry geometry_a, com.epl.geometry.Geometry geometry_b, com.epl.geometry.SpatialReference sr, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			int type_a = geometry_a.GetType().Value();
			int type_b = geometry_b.GetType().Value();
			// Give preference to the Point vs Envelope, Envelope vs Envelope and
			// Point vs Point realtions:
			if (type_a == com.epl.geometry.Geometry.GeometryType.Envelope)
			{
				if (type_b == com.epl.geometry.Geometry.GeometryType.Envelope)
				{
					return Relate((com.epl.geometry.Envelope)geometry_a, (com.epl.geometry.Envelope)geometry_b, sr, relation, progress_tracker);
				}
				else
				{
					if (type_b == com.epl.geometry.Geometry.GeometryType.Point)
					{
						if (relation == com.epl.geometry.RelationalOperations.Relation.within)
						{
							relation = com.epl.geometry.RelationalOperations.Relation.contains;
						}
						else
						{
							if (relation == com.epl.geometry.RelationalOperations.Relation.contains)
							{
								relation = com.epl.geometry.RelationalOperations.Relation.within;
							}
						}
						return Relate((com.epl.geometry.Point)geometry_b, (com.epl.geometry.Envelope)geometry_a, sr, relation, progress_tracker);
					}
				}
			}
			else
			{
				// proceed below
				if (type_a == com.epl.geometry.Geometry.GeometryType.Point)
				{
					if (type_b == com.epl.geometry.Geometry.GeometryType.Envelope)
					{
						return Relate((com.epl.geometry.Point)geometry_a, (com.epl.geometry.Envelope)geometry_b, sr, relation, progress_tracker);
					}
					else
					{
						if (type_b == com.epl.geometry.Geometry.GeometryType.Point)
						{
							return Relate((com.epl.geometry.Point)geometry_a, (com.epl.geometry.Point)geometry_b, sr, relation, progress_tracker);
						}
					}
				}
			}
			// proceed below
			// proceed below
			if (geometry_a.IsEmpty() || geometry_b.IsEmpty())
			{
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					return true;
				}
				// Always true
				return false;
			}
			// Always false
			com.epl.geometry.Envelope2D env1 = new com.epl.geometry.Envelope2D();
			geometry_a.QueryEnvelope2D(env1);
			com.epl.geometry.Envelope2D env2 = new com.epl.geometry.Envelope2D();
			geometry_b.QueryEnvelope2D(env2);
			com.epl.geometry.Envelope2D envMerged = new com.epl.geometry.Envelope2D();
			envMerged.SetCoords(env1);
			envMerged.Merge(env2);
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(sr, envMerged, false);
			if (EnvelopeDisjointEnvelope_(env1, env2, tolerance, progress_tracker))
			{
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					return true;
				}
				return false;
			}
			bool bRelation = false;
			com.epl.geometry.Geometry _geometry_a;
			com.epl.geometry.Geometry _geometry_b;
			com.epl.geometry.Polyline polyline_a;
			com.epl.geometry.Polyline polyline_b;
			if (com.epl.geometry.MultiPath.IsSegment(type_a))
			{
				polyline_a = new com.epl.geometry.Polyline(geometry_a.GetDescription());
				polyline_a.AddSegment((com.epl.geometry.Segment)geometry_a, true);
				_geometry_a = polyline_a;
				type_a = com.epl.geometry.Geometry.GeometryType.Polyline;
			}
			else
			{
				_geometry_a = geometry_a;
			}
			if (com.epl.geometry.MultiPath.IsSegment(type_b))
			{
				polyline_b = new com.epl.geometry.Polyline(geometry_b.GetDescription());
				polyline_b.AddSegment((com.epl.geometry.Segment)geometry_b, true);
				_geometry_b = polyline_b;
				type_b = com.epl.geometry.Geometry.GeometryType.Polyline;
			}
			else
			{
				_geometry_b = geometry_b;
			}
			if (type_a != com.epl.geometry.Geometry.GeometryType.Envelope && type_b != com.epl.geometry.Geometry.GeometryType.Envelope)
			{
				if (_geometry_a.GetDimension() < _geometry_b.GetDimension() || (type_a == com.epl.geometry.Geometry.GeometryType.Point && type_b == com.epl.geometry.Geometry.GeometryType.MultiPoint))
				{
					// we
					// will
					// switch
					// the
					// order
					// of
					// the
					// geometries
					// below.
					if (relation == com.epl.geometry.RelationalOperations.Relation.within)
					{
						relation = com.epl.geometry.RelationalOperations.Relation.contains;
					}
					else
					{
						if (relation == com.epl.geometry.RelationalOperations.Relation.contains)
						{
							relation = com.epl.geometry.RelationalOperations.Relation.within;
						}
					}
				}
			}
			else
			{
				if (type_a != com.epl.geometry.Geometry.GeometryType.Polygon && type_b != com.epl.geometry.Geometry.GeometryType.Envelope)
				{
					// we will
					// switch
					// the order
					// of the
					// geometries
					// below.
					if (relation == com.epl.geometry.RelationalOperations.Relation.within)
					{
						relation = com.epl.geometry.RelationalOperations.Relation.contains;
					}
					else
					{
						if (relation == com.epl.geometry.RelationalOperations.Relation.contains)
						{
							relation = com.epl.geometry.RelationalOperations.Relation.within;
						}
					}
				}
			}
			switch (type_a)
			{
				case com.epl.geometry.Geometry.GeometryType.Polygon:
				{
					switch (type_b)
					{
						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							bRelation = PolygonRelatePolygon_((com.epl.geometry.Polygon)(_geometry_a), (com.epl.geometry.Polygon)(_geometry_b), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Polyline:
						{
							bRelation = PolygonRelatePolyline_((com.epl.geometry.Polygon)(_geometry_a), (com.epl.geometry.Polyline)(_geometry_b), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Point:
						{
							bRelation = PolygonRelatePoint_((com.epl.geometry.Polygon)(_geometry_a), (com.epl.geometry.Point)(_geometry_b), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.MultiPoint:
						{
							bRelation = PolygonRelateMultiPoint_((com.epl.geometry.Polygon)(_geometry_a), (com.epl.geometry.MultiPoint)(_geometry_b), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Envelope:
						{
							bRelation = PolygonRelateEnvelope_((com.epl.geometry.Polygon)(_geometry_a), (com.epl.geometry.Envelope)(_geometry_b), tolerance, relation, progress_tracker);
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
					switch (type_b)
					{
						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							bRelation = PolygonRelatePolyline_((com.epl.geometry.Polygon)(_geometry_b), (com.epl.geometry.Polyline)(_geometry_a), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Polyline:
						{
							bRelation = PolylineRelatePolyline_((com.epl.geometry.Polyline)(_geometry_a), (com.epl.geometry.Polyline)(_geometry_b), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Point:
						{
							bRelation = PolylineRelatePoint_((com.epl.geometry.Polyline)(_geometry_a), (com.epl.geometry.Point)(_geometry_b), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.MultiPoint:
						{
							bRelation = PolylineRelateMultiPoint_((com.epl.geometry.Polyline)(_geometry_a), (com.epl.geometry.MultiPoint)(_geometry_b), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Envelope:
						{
							bRelation = PolylineRelateEnvelope_((com.epl.geometry.Polyline)(_geometry_a), (com.epl.geometry.Envelope)(_geometry_b), tolerance, relation, progress_tracker);
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
					switch (type_b)
					{
						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							bRelation = PolygonRelatePoint_((com.epl.geometry.Polygon)(_geometry_b), (com.epl.geometry.Point)(_geometry_a), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Polyline:
						{
							bRelation = PolylineRelatePoint_((com.epl.geometry.Polyline)(_geometry_b), (com.epl.geometry.Point)(_geometry_a), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.MultiPoint:
						{
							bRelation = MultiPointRelatePoint_((com.epl.geometry.MultiPoint)(_geometry_b), (com.epl.geometry.Point)(_geometry_a), tolerance, relation, progress_tracker);
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
					switch (type_b)
					{
						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							bRelation = PolygonRelateMultiPoint_((com.epl.geometry.Polygon)(_geometry_b), (com.epl.geometry.MultiPoint)(_geometry_a), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Polyline:
						{
							bRelation = PolylineRelateMultiPoint_((com.epl.geometry.Polyline)(_geometry_b), (com.epl.geometry.MultiPoint)(_geometry_a), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.MultiPoint:
						{
							bRelation = MultiPointRelateMultiPoint_((com.epl.geometry.MultiPoint)(_geometry_a), (com.epl.geometry.MultiPoint)(_geometry_b), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Point:
						{
							bRelation = MultiPointRelatePoint_((com.epl.geometry.MultiPoint)(_geometry_a), (com.epl.geometry.Point)(_geometry_b), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Envelope:
						{
							bRelation = MultiPointRelateEnvelope_((com.epl.geometry.MultiPoint)(_geometry_a), (com.epl.geometry.Envelope)(_geometry_b), tolerance, relation, progress_tracker);
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

				case com.epl.geometry.Geometry.GeometryType.Envelope:
				{
					switch (type_b)
					{
						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							bRelation = PolygonRelateEnvelope_((com.epl.geometry.Polygon)(_geometry_b), (com.epl.geometry.Envelope)(_geometry_a), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.Polyline:
						{
							bRelation = PolylineRelateEnvelope_((com.epl.geometry.Polyline)(_geometry_b), (com.epl.geometry.Envelope)(_geometry_a), tolerance, relation, progress_tracker);
							break;
						}

						case com.epl.geometry.Geometry.GeometryType.MultiPoint:
						{
							bRelation = MultiPointRelateEnvelope_((com.epl.geometry.MultiPoint)(_geometry_b), (com.epl.geometry.Envelope)(_geometry_a), tolerance, relation, progress_tracker);
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
					break;
				}
			}
			// warning fix
			return bRelation;
		}

		// Computes the necessary 9 intersection relationships of boundary,
		// interior, and exterior of envelope_a vs envelope_b for the given
		// relation.
		private static bool Relate(com.epl.geometry.Envelope envelope_a, com.epl.geometry.Envelope envelope_b, com.epl.geometry.SpatialReference sr, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (envelope_a.IsEmpty() || envelope_b.IsEmpty())
			{
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					return true;
				}
				// Always true
				return false;
			}
			// Always false
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_merged = new com.epl.geometry.Envelope2D();
			envelope_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			env_merged.SetCoords(env_a);
			env_merged.Merge(env_b);
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(sr, env_merged, false);
			switch (relation)
			{
				case com.epl.geometry.RelationalOperations.Relation.disjoint:
				{
					return EnvelopeDisjointEnvelope_(env_a, env_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.within:
				{
					return EnvelopeContainsEnvelope_(env_b, env_a, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.contains:
				{
					return EnvelopeContainsEnvelope_(env_a, env_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.equals:
				{
					return EnvelopeEqualsEnvelope_(env_a, env_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.touches:
				{
					return EnvelopeTouchesEnvelope_(env_a, env_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.overlaps:
				{
					return EnvelopeOverlapsEnvelope_(env_a, env_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.crosses:
				{
					return EnvelopeCrossesEnvelope_(env_a, env_b, tolerance, progress_tracker);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return false;
		}

		// Computes the necessary 9 intersection relationships of boundary,
		// interior, and exterior of point_a vs envelope_b for the given relation.
		private static bool Relate(com.epl.geometry.Point point_a, com.epl.geometry.Envelope envelope_b, com.epl.geometry.SpatialReference sr, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (point_a.IsEmpty() || envelope_b.IsEmpty())
			{
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					return true;
				}
				// Always true
				return false;
			}
			// Always false
			com.epl.geometry.Point2D pt_a = point_a.GetXY();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_merged = new com.epl.geometry.Envelope2D();
			envelope_b.QueryEnvelope2D(env_b);
			env_merged.SetCoords(pt_a);
			env_merged.Merge(env_b);
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(sr, env_merged, false);
			switch (relation)
			{
				case com.epl.geometry.RelationalOperations.Relation.disjoint:
				{
					return PointDisjointEnvelope_(pt_a, env_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.within:
				{
					return PointWithinEnvelope_(pt_a, env_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.contains:
				{
					return PointContainsEnvelope_(pt_a, env_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.equals:
				{
					return PointEqualsEnvelope_(pt_a, env_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.touches:
				{
					return PointTouchesEnvelope_(pt_a, env_b, tolerance, progress_tracker);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return false;
		}

		// Computes the necessary 9 intersection relationships of boundary,
		// interior, and exterior of point_a vs point_b for the given relation.
		private static bool Relate(com.epl.geometry.Point point_a, com.epl.geometry.Point point_b, com.epl.geometry.SpatialReference sr, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (point_a.IsEmpty() || point_b.IsEmpty())
			{
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					return true;
				}
				// Always true
				return false;
			}
			// Always false
			com.epl.geometry.Point2D pt_a = point_a.GetXY();
			com.epl.geometry.Point2D pt_b = point_b.GetXY();
			com.epl.geometry.Envelope2D env_merged = new com.epl.geometry.Envelope2D();
			env_merged.SetCoords(pt_a);
			env_merged.Merge(pt_b);
			double tolerance = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(sr, env_merged, false);
			switch (relation)
			{
				case com.epl.geometry.RelationalOperations.Relation.disjoint:
				{
					return PointDisjointPoint_(pt_a, pt_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.within:
				{
					return PointContainsPoint_(pt_b, pt_a, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.contains:
				{
					return PointContainsPoint_(pt_a, pt_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.equals:
				{
					return PointEqualsPoint_(pt_a, pt_b, tolerance, progress_tracker);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return false;
		}

		// Returns true if the relation holds.
		private static bool PolygonRelatePolygon_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polygon polygon_b, double tolerance, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			switch (relation)
			{
				case com.epl.geometry.RelationalOperations.Relation.disjoint:
				{
					return PolygonDisjointPolygon_(polygon_a, polygon_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.within:
				{
					return PolygonContainsPolygon_(polygon_b, polygon_a, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.contains:
				{
					return PolygonContainsPolygon_(polygon_a, polygon_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.equals:
				{
					return PolygonEqualsPolygon_(polygon_a, polygon_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.touches:
				{
					return PolygonTouchesPolygon_(polygon_a, polygon_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.overlaps:
				{
					return PolygonOverlapsPolygon_(polygon_a, polygon_b, tolerance, progress_tracker);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return false;
		}

		// Returns true if the relation holds.
		private static bool PolygonRelatePolyline_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polyline polyline_b, double tolerance, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			switch (relation)
			{
				case com.epl.geometry.RelationalOperations.Relation.disjoint:
				{
					return PolygonDisjointPolyline_(polygon_a, polyline_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.contains:
				{
					return PolygonContainsPolyline_(polygon_a, polyline_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.touches:
				{
					return PolygonTouchesPolyline_(polygon_a, polyline_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.crosses:
				{
					return PolygonCrossesPolyline_(polygon_a, polyline_b, tolerance, progress_tracker);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return false;
		}

		// Returns true if the relation holds.
		private static bool PolygonRelatePoint_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Point point_b, double tolerance, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			switch (relation)
			{
				case com.epl.geometry.RelationalOperations.Relation.disjoint:
				{
					return PolygonDisjointPoint_(polygon_a, point_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.contains:
				{
					return PolygonContainsPoint_(polygon_a, point_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.touches:
				{
					return PolygonTouchesPoint_(polygon_a, point_b, tolerance, progress_tracker);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return false;
		}

		// Returns true if the relation holds
		private static bool PolygonRelateMultiPoint_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			switch (relation)
			{
				case com.epl.geometry.RelationalOperations.Relation.disjoint:
				{
					return PolygonDisjointMultiPoint_(polygon_a, multipoint_b, tolerance, true, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.contains:
				{
					return PolygonContainsMultiPoint_(polygon_a, multipoint_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.touches:
				{
					return PolygonTouchesMultiPoint_(polygon_a, multipoint_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.crosses:
				{
					return PolygonCrossesMultiPoint_(polygon_a, multipoint_b, tolerance, progress_tracker);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return false;
		}

		// Returns true if the relation holds
		private static bool PolygonRelateEnvelope_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Envelope envelope_b, double tolerance, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (PolygonDisjointEnvelope_(polygon_a, envelope_b, tolerance, progress_tracker))
			{
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					return true;
				}
				return false;
			}
			else
			{
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					return false;
				}
			}
			switch (relation)
			{
				case com.epl.geometry.RelationalOperations.Relation.within:
				{
					return PolygonWithinEnvelope_(polygon_a, envelope_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.contains:
				{
					return PolygonContainsEnvelope_(polygon_a, envelope_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.equals:
				{
					return PolygonEqualsEnvelope_(polygon_a, envelope_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.touches:
				{
					return PolygonTouchesEnvelope_(polygon_a, envelope_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.overlaps:
				{
					return PolygonOverlapsEnvelope_(polygon_a, envelope_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.crosses:
				{
					return PolygonCrossesEnvelope_(polygon_a, envelope_b, tolerance, progress_tracker);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return false;
		}

		// Returns true if the relation holds.
		private static bool PolylineRelatePolyline_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Polyline polyline_b, double tolerance, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			switch (relation)
			{
				case com.epl.geometry.RelationalOperations.Relation.disjoint:
				{
					return PolylineDisjointPolyline_(polyline_a, polyline_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.within:
				{
					return PolylineContainsPolyline_(polyline_b, polyline_a, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.contains:
				{
					return PolylineContainsPolyline_(polyline_a, polyline_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.equals:
				{
					return PolylineEqualsPolyline_(polyline_a, polyline_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.touches:
				{
					return PolylineTouchesPolyline_(polyline_a, polyline_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.overlaps:
				{
					return PolylineOverlapsPolyline_(polyline_a, polyline_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.crosses:
				{
					return PolylineCrossesPolyline_(polyline_a, polyline_b, tolerance, progress_tracker);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return false;
		}

		// Returns true if the relation holds.
		private static bool PolylineRelatePoint_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Point point_b, double tolerance, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			switch (relation)
			{
				case com.epl.geometry.RelationalOperations.Relation.disjoint:
				{
					return PolylineDisjointPoint_(polyline_a, point_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.contains:
				{
					return PolylineContainsPoint_(polyline_a, point_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.touches:
				{
					return PolylineTouchesPoint_(polyline_a, point_b, tolerance, progress_tracker);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return false;
		}

		// Returns true if the relation holds.
		private static bool PolylineRelateMultiPoint_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			switch (relation)
			{
				case com.epl.geometry.RelationalOperations.Relation.disjoint:
				{
					return PolylineDisjointMultiPoint_(polyline_a, multipoint_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.contains:
				{
					return PolylineContainsMultiPoint_(polyline_a, multipoint_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.touches:
				{
					return PolylineTouchesMultiPoint_(polyline_a, multipoint_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.crosses:
				{
					return PolylineCrossesMultiPoint_(polyline_a, multipoint_b, tolerance, progress_tracker);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return false;
		}

		// Returns true if the relation holds.
		private static bool PolylineRelateEnvelope_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Envelope envelope_b, double tolerance, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (PolylineDisjointEnvelope_(polyline_a, envelope_b, tolerance, progress_tracker))
			{
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					return true;
				}
				return false;
			}
			else
			{
				if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
				{
					return false;
				}
			}
			switch (relation)
			{
				case com.epl.geometry.RelationalOperations.Relation.within:
				{
					return PolylineWithinEnvelope_(polyline_a, envelope_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.contains:
				{
					return PolylineContainsEnvelope_(polyline_a, envelope_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.equals:
				{
					return PolylineEqualsEnvelope_(polyline_a, envelope_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.touches:
				{
					return PolylineTouchesEnvelope_(polyline_a, envelope_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.overlaps:
				{
					return PolylineOverlapsEnvelope_(polyline_a, envelope_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.crosses:
				{
					return PolylineCrossesEnvelope_(polyline_a, envelope_b, tolerance, progress_tracker);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return false;
		}

		// Returns true if the relation holds.
		private static bool MultiPointRelateMultiPoint_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			switch (relation)
			{
				case com.epl.geometry.RelationalOperations.Relation.disjoint:
				{
					return MultiPointDisjointMultiPoint_(multipoint_a, multipoint_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.within:
				{
					return MultiPointContainsMultiPoint_(multipoint_b, multipoint_a, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.contains:
				{
					return MultiPointContainsMultiPoint_(multipoint_a, multipoint_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.equals:
				{
					return MultiPointEqualsMultiPoint_(multipoint_a, multipoint_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.overlaps:
				{
					return MultiPointOverlapsMultiPoint_(multipoint_a, multipoint_b, tolerance, progress_tracker);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return false;
		}

		// Returns true if the relation holds.
		private static bool MultiPointRelatePoint_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.Point point_b, double tolerance, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			switch (relation)
			{
				case com.epl.geometry.RelationalOperations.Relation.disjoint:
				{
					return MultiPointDisjointPoint_(multipoint_a, point_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.within:
				{
					return MultiPointWithinPoint_(multipoint_a, point_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.contains:
				{
					return MultiPointContainsPoint_(multipoint_a, point_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.equals:
				{
					return MultiPointEqualsPoint_(multipoint_a, point_b, tolerance, progress_tracker);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return false;
		}

		// Returns true if the relation holds.
		private static bool MultiPointRelateEnvelope_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.Envelope envelope_b, double tolerance, int relation, com.epl.geometry.ProgressTracker progress_tracker)
		{
			switch (relation)
			{
				case com.epl.geometry.RelationalOperations.Relation.disjoint:
				{
					return MultiPointDisjointEnvelope_(multipoint_a, envelope_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.within:
				{
					return MultiPointWithinEnvelope_(multipoint_a, envelope_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.contains:
				{
					return MultiPointContainsEnvelope_(multipoint_a, envelope_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.equals:
				{
					return MultiPointEqualsEnvelope_(multipoint_a, envelope_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.touches:
				{
					return MultiPointTouchesEnvelope_(multipoint_a, envelope_b, tolerance, progress_tracker);
				}

				case com.epl.geometry.RelationalOperations.Relation.crosses:
				{
					return MultiPointCrossesEnvelope_(multipoint_a, envelope_b, tolerance, progress_tracker);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return false;
		}

		// Returns true if polygon_a equals polygon_b.
		private static bool PolygonEqualsPolygon_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polygon polygon_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			polygon_b.QueryEnvelope2D(env_b);
			// Quick envelope rejection test for false equality.
			if (!EnvelopeEqualsEnvelope_(env_a, env_b, tolerance, progress_tracker))
			{
				return false;
			}
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, polygon_b, tolerance, false);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint || relation == com.epl.geometry.RelationalOperations.Relation.contains || relation == com.epl.geometry.RelationalOperations.Relation.within)
			{
				return false;
			}
			// Quick point equality check for true equality. This just checks if all
			// the points in each ring are the same (within a tolerance) and in the
			// same order
			if (MultiPathExactlyEqualsMultiPath_(polygon_a, polygon_b, tolerance, progress_tracker))
			{
				return true;
			}
			double length_a = polygon_a.CalculateLength2D();
			double length_b = polygon_b.CalculateLength2D();
			int max_vertices = System.Math.Max(polygon_a.GetPointCount(), polygon_b.GetPointCount());
			if (System.Math.Abs(length_a - length_b) > max_vertices * 4.0 * tolerance)
			{
				return false;
			}
			return LinearPathEqualsLinearPath_(polygon_a, polygon_b, tolerance, true);
		}

		// Returns true if polygon_a is disjoint from polygon_b.
		private static bool PolygonDisjointPolygon_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polygon polygon_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, polygon_b, tolerance, true);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return true;
			}
			if (relation == com.epl.geometry.RelationalOperations.Relation.contains || relation == com.epl.geometry.RelationalOperations.Relation.within || relation == com.epl.geometry.RelationalOperations.Relation.intersects)
			{
				return false;
			}
			return PolygonDisjointMultiPath_(polygon_a, polygon_b, tolerance, progress_tracker);
		}

		// Returns true if polygon_a touches polygon_b.
		private static bool PolygonTouchesPolygon_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polygon polygon_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, polygon_b, tolerance, false);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint || relation == com.epl.geometry.RelationalOperations.Relation.contains || relation == com.epl.geometry.RelationalOperations.Relation.within)
			{
				return false;
			}
			return PolygonTouchesPolygonImpl_(polygon_a, polygon_b, tolerance, null);
		}

		// Returns true if polygon_a overlaps polygon_b.
		private static bool PolygonOverlapsPolygon_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polygon polygon_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, polygon_b, tolerance, false);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint || relation == com.epl.geometry.RelationalOperations.Relation.contains || relation == com.epl.geometry.RelationalOperations.Relation.within)
			{
				return false;
			}
			return PolygonOverlapsPolygonImpl_(polygon_a, polygon_b, tolerance, progress_tracker);
		}

		// Returns true if polygon_a contains polygon_b.
		private static bool PolygonContainsPolygon_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polygon polygon_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			polygon_b.QueryEnvelope2D(env_b);
			// Quick envelope rejection test for false equality.
			if (!EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance))
			{
				return false;
			}
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, polygon_b, tolerance, false);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint || relation == com.epl.geometry.RelationalOperations.Relation.within)
			{
				return false;
			}
			if (relation == com.epl.geometry.RelationalOperations.Relation.contains)
			{
				return true;
			}
			return PolygonContainsPolygonImpl_(polygon_a, polygon_b, tolerance, progress_tracker);
		}

		// Returns true if polygon_a is disjoint from polyline_b.
		private static bool PolygonDisjointPolyline_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polyline polyline_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, polyline_b, tolerance, true);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return true;
			}
			if (relation == com.epl.geometry.RelationalOperations.Relation.contains || relation == com.epl.geometry.RelationalOperations.Relation.intersects)
			{
				return false;
			}
			return PolygonDisjointMultiPath_(polygon_a, polyline_b, tolerance, progress_tracker);
		}

		// Returns true if polygon_a touches polyline_b.
		private static bool PolygonTouchesPolyline_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polyline polyline_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, polyline_b, tolerance, false);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint || relation == com.epl.geometry.RelationalOperations.Relation.contains)
			{
				return false;
			}
			return PolygonTouchesPolylineImpl_(polygon_a, polyline_b, tolerance, progress_tracker);
		}

		// Returns true if polygon_a crosses polyline_b.
		private static bool PolygonCrossesPolyline_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polyline polyline_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, polyline_b, tolerance, false);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint || relation == com.epl.geometry.RelationalOperations.Relation.contains)
			{
				return false;
			}
			return PolygonCrossesPolylineImpl_(polygon_a, polyline_b, tolerance, null);
		}

		// Returns true if polygon_a contains polyline_b.
		private static bool PolygonContainsPolyline_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polyline polyline_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			polyline_b.QueryEnvelope2D(env_b);
			// Quick envelope rejection test for false equality.
			if (!EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance))
			{
				return false;
			}
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, polyline_b, tolerance, false);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return false;
			}
			if (relation == com.epl.geometry.RelationalOperations.Relation.contains)
			{
				return true;
			}
			return PolygonContainsPolylineImpl_(polygon_a, polyline_b, tolerance, progress_tracker);
		}

		// Returns true if polygon_a is disjoint from point_b.
		private static bool PolygonDisjointPoint_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Point point_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.PolygonUtils.PiPResult result = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon_a, point_b, tolerance);
			if (result == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
			{
				return true;
			}
			return false;
		}

		// Returns true of polygon_a touches point_b.
		private static bool PolygonTouchesPoint_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Point point_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Point2D pt_b = point_b.GetXY();
			return PolygonTouchesPointImpl_(polygon_a, pt_b, tolerance, null);
		}

		// Returns true if polygon_a contains point_b.
		private static bool PolygonContainsPoint_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Point point_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Point2D pt_b = point_b.GetXY();
			return PolygonContainsPointImpl_(polygon_a, pt_b, tolerance, progress_tracker);
		}

		// Returns true if polygon_a is disjoint from multipoint_b.
		private static bool PolygonDisjointMultiPoint_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, bool bIncludeBoundaryA, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, multipoint_b, tolerance, false);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return true;
			}
			if (relation == com.epl.geometry.RelationalOperations.Relation.contains)
			{
				return false;
			}
			com.epl.geometry.Envelope2D env_a_inflated = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a_inflated);
			env_a_inflated.Inflate(tolerance, tolerance);
			com.epl.geometry.Point2D ptB = new com.epl.geometry.Point2D();
			for (int i = 0; i < multipoint_b.GetPointCount(); i++)
			{
				multipoint_b.GetXY(i, ptB);
				if (!env_a_inflated.Contains(ptB))
				{
					continue;
				}
				com.epl.geometry.PolygonUtils.PiPResult result = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon_a, ptB, tolerance);
				if (result == com.epl.geometry.PolygonUtils.PiPResult.PiPInside || (bIncludeBoundaryA && result == com.epl.geometry.PolygonUtils.PiPResult.PiPBoundary))
				{
					return false;
				}
			}
			return true;
		}

		// Returns true if polygon_a touches multipoint_b.
		private static bool PolygonTouchesMultiPoint_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, multipoint_b, tolerance, false);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint || relation == com.epl.geometry.RelationalOperations.Relation.contains)
			{
				return false;
			}
			com.epl.geometry.Envelope2D env_a_inflated = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a_inflated);
			env_a_inflated.Inflate(tolerance, tolerance);
			com.epl.geometry.Point2D ptB;
			bool b_boundary = false;
			com.epl.geometry.MultiPathImpl polygon_a_impl = (com.epl.geometry.MultiPathImpl)polygon_a._getImpl();
			com.epl.geometry.Polygon pa = null;
			com.epl.geometry.Polygon p_polygon_a = polygon_a;
			bool b_checked_polygon_a_quad_tree = false;
			for (int i = 0; i < multipoint_b.GetPointCount(); i++)
			{
				ptB = multipoint_b.GetXY(i);
				if (env_a_inflated.Contains(ptB))
				{
					com.epl.geometry.PolygonUtils.PiPResult result = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(p_polygon_a, ptB, tolerance);
					if (result == com.epl.geometry.PolygonUtils.PiPResult.PiPBoundary)
					{
						b_boundary = true;
					}
					else
					{
						if (result == com.epl.geometry.PolygonUtils.PiPResult.PiPInside)
						{
							return false;
						}
					}
				}
				if (!b_checked_polygon_a_quad_tree)
				{
					if (com.epl.geometry.PointInPolygonHelper.QuadTreeWillHelp(polygon_a, multipoint_b.GetPointCount() - 1) && (polygon_a_impl._getAccelerators() == null || polygon_a_impl._getAccelerators().GetQuadTree() == null))
					{
						pa = new com.epl.geometry.Polygon();
						polygon_a.CopyTo(pa);
						((com.epl.geometry.MultiPathImpl)pa._getImpl())._buildQuadTreeAccelerator(com.epl.geometry.Geometry.GeometryAccelerationDegree.enumMedium);
						p_polygon_a = pa;
					}
					else
					{
						p_polygon_a = polygon_a;
					}
					b_checked_polygon_a_quad_tree = true;
				}
			}
			if (b_boundary)
			{
				return true;
			}
			return false;
		}

		// Returns true if polygon_a crosses multipoint_b.
		private static bool PolygonCrossesMultiPoint_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, multipoint_b, tolerance, false);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint || relation == com.epl.geometry.RelationalOperations.Relation.contains)
			{
				return false;
			}
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_a_inflated = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			multipoint_b.QueryEnvelope2D(env_b);
			env_a_inflated.SetCoords(env_a);
			env_a_inflated.Inflate(tolerance, tolerance);
			bool b_interior = false;
			bool b_exterior = false;
			com.epl.geometry.Point2D pt_b;
			com.epl.geometry.MultiPathImpl polygon_a_impl = (com.epl.geometry.MultiPathImpl)polygon_a._getImpl();
			com.epl.geometry.Polygon pa = null;
			com.epl.geometry.Polygon p_polygon_a = polygon_a;
			bool b_checked_polygon_a_quad_tree = false;
			for (int i = 0; i < multipoint_b.GetPointCount(); i++)
			{
				pt_b = multipoint_b.GetXY(i);
				if (!env_a_inflated.Contains(pt_b))
				{
					b_exterior = true;
				}
				else
				{
					com.epl.geometry.PolygonUtils.PiPResult result = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(p_polygon_a, pt_b, tolerance);
					if (result == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
					{
						b_exterior = true;
					}
					else
					{
						if (result == com.epl.geometry.PolygonUtils.PiPResult.PiPInside)
						{
							b_interior = true;
						}
					}
				}
				if (b_interior && b_exterior)
				{
					return true;
				}
				if (!b_checked_polygon_a_quad_tree)
				{
					if (com.epl.geometry.PointInPolygonHelper.QuadTreeWillHelp(polygon_a, multipoint_b.GetPointCount() - 1) && (polygon_a_impl._getAccelerators() == null || polygon_a_impl._getAccelerators().GetQuadTree() == null))
					{
						pa = new com.epl.geometry.Polygon();
						polygon_a.CopyTo(pa);
						((com.epl.geometry.MultiPathImpl)pa._getImpl())._buildQuadTreeAccelerator(com.epl.geometry.Geometry.GeometryAccelerationDegree.enumMedium);
						p_polygon_a = pa;
					}
					else
					{
						p_polygon_a = polygon_a;
					}
					b_checked_polygon_a_quad_tree = true;
				}
			}
			return false;
		}

		// Returns true if polygon_a contains multipoint_b.
		private static bool PolygonContainsMultiPoint_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			multipoint_b.QueryEnvelope2D(env_b);
			// Quick envelope rejection test for false equality.
			if (!EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance))
			{
				return false;
			}
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, multipoint_b, tolerance, false);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return false;
			}
			if (relation == com.epl.geometry.RelationalOperations.Relation.contains)
			{
				return true;
			}
			bool b_interior = false;
			com.epl.geometry.Point2D ptB;
			com.epl.geometry.MultiPathImpl polygon_a_impl = (com.epl.geometry.MultiPathImpl)polygon_a._getImpl();
			com.epl.geometry.Polygon pa = null;
			com.epl.geometry.Polygon p_polygon_a = polygon_a;
			bool b_checked_polygon_a_quad_tree = false;
			for (int i = 0; i < multipoint_b.GetPointCount(); i++)
			{
				ptB = multipoint_b.GetXY(i);
				if (!env_a.Contains(ptB))
				{
					return false;
				}
				com.epl.geometry.PolygonUtils.PiPResult result = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(p_polygon_a, ptB, tolerance);
				if (result == com.epl.geometry.PolygonUtils.PiPResult.PiPInside)
				{
					b_interior = true;
				}
				else
				{
					if (result == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
					{
						return false;
					}
				}
				if (!b_checked_polygon_a_quad_tree)
				{
					if (com.epl.geometry.PointInPolygonHelper.QuadTreeWillHelp(polygon_a, multipoint_b.GetPointCount() - 1) && (polygon_a_impl._getAccelerators() == null || polygon_a_impl._getAccelerators().GetQuadTree() == null))
					{
						pa = new com.epl.geometry.Polygon();
						polygon_a.CopyTo(pa);
						((com.epl.geometry.MultiPathImpl)pa._getImpl())._buildQuadTreeAccelerator(com.epl.geometry.Geometry.GeometryAccelerationDegree.enumMedium);
						p_polygon_a = pa;
					}
					else
					{
						p_polygon_a = polygon_a;
					}
					b_checked_polygon_a_quad_tree = true;
				}
			}
			return b_interior;
		}

		// Returns true if polygon_a equals envelope_b.
		private static bool PolygonEqualsEnvelope_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			// Quick envelope rejection test for false equality.
			// This check will correctly handle degenerate envelope cases (i.e.
			// degenerate to point or line)
			if (!EnvelopeEqualsEnvelope_(env_a, env_b, tolerance, progress_tracker))
			{
				return false;
			}
			com.epl.geometry.Polygon polygon_b = new com.epl.geometry.Polygon();
			polygon_b.AddEnvelope(envelope_b, false);
			return LinearPathEqualsLinearPath_(polygon_a, polygon_b, tolerance, true);
		}

		// Returns true if polygon_a is disjoint from envelope_b.
		private static bool PolygonDisjointEnvelope_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, envelope_b, tolerance, false);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return true;
			}
			if (relation == com.epl.geometry.RelationalOperations.Relation.contains || relation == com.epl.geometry.RelationalOperations.Relation.within)
			{
				return false;
			}
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance))
			{
				return false;
			}
			com.epl.geometry.PolygonUtils.PiPResult pipres;
			com.epl.geometry.Point2D pt_b = new com.epl.geometry.Point2D();
			env_b.QueryLowerLeft(pt_b);
			pipres = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon_a, pt_b, tolerance);
			if (pipres != com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
			{
				return false;
			}
			env_b.QueryLowerRight(pt_b);
			pipres = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon_a, pt_b, tolerance);
			if (pipres != com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
			{
				return false;
			}
			env_b.QueryUpperRight(pt_b);
			pipres = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon_a, pt_b, tolerance);
			if (pipres != com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
			{
				return false;
			}
			env_b.QueryUpperLeft(pt_b);
			pipres = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon_a, pt_b, tolerance);
			if (pipres != com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
			{
				return false;
			}
			com.epl.geometry.MultiPathImpl mimpl_a = (com.epl.geometry.MultiPathImpl)polygon_a._getImpl();
			com.epl.geometry.AttributeStreamOfDbl pos = (com.epl.geometry.AttributeStreamOfDbl)(mimpl_a.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION));
			com.epl.geometry.Envelope2D env_b_inflated = new com.epl.geometry.Envelope2D();
			env_b_inflated.SetCoords(env_b);
			env_b_inflated.Inflate(tolerance, tolerance);
			for (int ptIndex = 0, n = mimpl_a.GetPointCount(); ptIndex < n; ptIndex++)
			{
				double x = pos.Read(2 * ptIndex);
				double y = pos.Read(2 * ptIndex + 1);
				if (env_b_inflated.Contains(x, y))
				{
					return false;
				}
			}
			return !LinearPathIntersectsEnvelope_(polygon_a, env_b, tolerance, progress_tracker);
		}

		// Returns true if polygon_a touches envelope_b.
		private static bool PolygonTouchesEnvelope_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, envelope_b, tolerance, false);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint || relation == com.epl.geometry.RelationalOperations.Relation.contains || relation == com.epl.geometry.RelationalOperations.Relation.within)
			{
				return false;
			}
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance))
			{
				return false;
			}
			if (env_b.GetWidth() <= tolerance && env_b.GetHeight() <= tolerance)
			{
				// treat
				// as
				// point
				com.epl.geometry.Point2D pt_b = envelope_b.GetCenterXY();
				return PolygonTouchesPointImpl_(polygon_a, pt_b, tolerance, progress_tracker);
			}
			if (env_b.GetWidth() <= tolerance || env_b.GetHeight() <= tolerance)
			{
				// treat
				// as
				// polyline
				com.epl.geometry.Polyline polyline_b = new com.epl.geometry.Polyline();
				com.epl.geometry.Point p = new com.epl.geometry.Point();
				envelope_b.QueryCornerByVal(0, p);
				polyline_b.StartPath(p);
				envelope_b.QueryCornerByVal(2, p);
				polyline_b.LineTo(p);
				return PolygonTouchesPolylineImpl_(polygon_a, polyline_b, tolerance, progress_tracker);
			}
			// treat as polygon
			com.epl.geometry.Polygon polygon_b = new com.epl.geometry.Polygon();
			polygon_b.AddEnvelope(envelope_b, false);
			return PolygonTouchesPolygonImpl_(polygon_a, polygon_b, tolerance, progress_tracker);
		}

		// Returns true if polygon_a overlaps envelope_b.
		private static bool PolygonOverlapsEnvelope_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, envelope_b, tolerance, false);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint || relation == com.epl.geometry.RelationalOperations.Relation.contains || relation == com.epl.geometry.RelationalOperations.Relation.within)
			{
				return false;
			}
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance))
			{
				return false;
			}
			if (env_b.GetWidth() <= tolerance || env_b.GetHeight() <= tolerance)
			{
				return false;
			}
			// has no interior
			com.epl.geometry.Polygon polygon_b = new com.epl.geometry.Polygon();
			polygon_b.AddEnvelope(envelope_b, false);
			return PolygonOverlapsPolygonImpl_(polygon_a, polygon_b, tolerance, progress_tracker);
		}

		// Returns true if polygon_a is within envelope_b
		private static bool PolygonWithinEnvelope_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			return EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance);
		}

		// Returns true if polygon_a contains envelope_b.
		private static bool PolygonContainsEnvelope_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick envelope rejection test
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (!EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance))
			{
				return false;
			}
			// Quick rasterize test to see whether the the geometries are disjoint,
			// or if one is contained in the other.
			int relation = TryRasterizedContainsOrDisjoint_(polygon_a, envelope_b, tolerance, false);
			if (relation == com.epl.geometry.RelationalOperations.Relation.disjoint || relation == com.epl.geometry.RelationalOperations.Relation.within)
			{
				return false;
			}
			if (relation == com.epl.geometry.RelationalOperations.Relation.contains)
			{
				return true;
			}
			if (env_b.GetWidth() <= tolerance && env_b.GetHeight() <= tolerance)
			{
				// treat
				// as
				// point
				com.epl.geometry.Point2D pt_b = envelope_b.GetCenterXY();
				return PolygonContainsPointImpl_(polygon_a, pt_b, tolerance, progress_tracker);
			}
			if (env_b.GetWidth() <= tolerance || env_b.GetHeight() <= tolerance)
			{
				// treat
				// as
				// polyline
				com.epl.geometry.Polyline polyline_b = new com.epl.geometry.Polyline();
				com.epl.geometry.Point p = new com.epl.geometry.Point();
				envelope_b.QueryCornerByVal(0, p);
				polyline_b.StartPath(p);
				envelope_b.QueryCornerByVal(2, p);
				polyline_b.LineTo(p);
				return PolygonContainsPolylineImpl_(polygon_a, polyline_b, tolerance, null);
			}
			// treat as polygon
			com.epl.geometry.Polygon polygon_b = new com.epl.geometry.Polygon();
			polygon_b.AddEnvelope(envelope_b, false);
			return PolygonContainsPolygonImpl_(polygon_a, polygon_b, tolerance, null);
		}

		// Returns true if polygon_a crosses envelope_b.
		private static bool PolygonCrossesEnvelope_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance))
			{
				return false;
			}
			if (env_b.GetHeight() > tolerance && env_b.GetWidth() > tolerance)
			{
				return false;
			}
			// when treated as an area, areas cannot cross areas.
			if (env_b.GetHeight() <= tolerance && env_b.GetWidth() <= tolerance)
			{
				return false;
			}
			// when treated as a point, areas cannot cross points.
			// Treat as polyline
			com.epl.geometry.Polyline polyline_b = new com.epl.geometry.Polyline();
			com.epl.geometry.Point p = new com.epl.geometry.Point();
			envelope_b.QueryCornerByVal(0, p);
			polyline_b.StartPath(p);
			envelope_b.QueryCornerByVal(2, p);
			polyline_b.LineTo(p);
			return PolygonCrossesPolylineImpl_(polygon_a, polyline_b, tolerance, progress_tracker);
		}

		// Returns true if polyline_a equals polyline_b.
		private static bool PolylineEqualsPolyline_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Polyline polyline_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polyline_a.QueryEnvelope2D(env_a);
			polyline_b.QueryEnvelope2D(env_b);
			// Quick envelope rejection test for false equality.
			if (!EnvelopeEqualsEnvelope_(env_a, env_b, tolerance, progress_tracker))
			{
				return false;
			}
			// Quick rasterize test to see whether the the geometries are disjoint.
			if (TryRasterizedContainsOrDisjoint_(polyline_a, polyline_b, tolerance, false) == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return false;
			}
			// Quick point equality check for true equality. This just checks if all
			// the points in each ring are the same (within a tolerance) and in the
			// same order
			if (MultiPathExactlyEqualsMultiPath_(polyline_a, polyline_b, tolerance, progress_tracker))
			{
				return true;
			}
			return LinearPathEqualsLinearPath_(polyline_a, polyline_b, tolerance, false);
		}

		// Returns true if polyline_a is disjoint from polyline_b.
		private static bool PolylineDisjointPolyline_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Polyline polyline_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint.
			if (TryRasterizedContainsOrDisjoint_(polyline_a, polyline_b, tolerance, false) == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return true;
			}
			com.epl.geometry.MultiPathImpl multi_path_impl_a = (com.epl.geometry.MultiPathImpl)polyline_a._getImpl();
			com.epl.geometry.MultiPathImpl multi_path_impl_b = (com.epl.geometry.MultiPathImpl)polyline_b._getImpl();
			com.epl.geometry.PairwiseIntersectorImpl intersector_paths = new com.epl.geometry.PairwiseIntersectorImpl(multi_path_impl_a, multi_path_impl_b, tolerance, true);
			if (!intersector_paths.Next())
			{
				return false;
			}
			return !LinearPathIntersectsLinearPath_(polyline_a, polyline_b, tolerance);
		}

		// Returns true if polyline_a touches polyline_b.
		private static bool PolylineTouchesPolyline_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Polyline polyline_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint.
			if (TryRasterizedContainsOrDisjoint_(polyline_a, polyline_b, tolerance, false) == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return false;
			}
			com.epl.geometry.AttributeStreamOfDbl intersections = new com.epl.geometry.AttributeStreamOfDbl(0);
			int dim = LinearPathIntersectsLinearPathMaxDim_(polyline_a, polyline_b, tolerance, intersections);
			if (dim != 0)
			{
				return false;
			}
			com.epl.geometry.MultiPoint intersection = new com.epl.geometry.MultiPoint();
			for (int i = 0; i < intersections.Size(); i += 2)
			{
				double x = intersections.Read(i);
				double y = intersections.Read(i + 1);
				intersection.Add(x, y);
			}
			com.epl.geometry.MultiPoint boundary_a_b = (com.epl.geometry.MultiPoint)(polyline_a.GetBoundary());
			com.epl.geometry.MultiPoint boundary_b = (com.epl.geometry.MultiPoint)(polyline_b.GetBoundary());
			boundary_a_b.Add(boundary_b, 0, boundary_b.GetPointCount());
			return MultiPointContainsMultiPointBrute_(boundary_a_b, intersection, tolerance);
		}

		// Returns true if polyline_a crosses polyline_b.
		private static bool PolylineCrossesPolyline_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Polyline polyline_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint.
			if (TryRasterizedContainsOrDisjoint_(polyline_a, polyline_b, tolerance, false) == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return false;
			}
			com.epl.geometry.AttributeStreamOfDbl intersections = new com.epl.geometry.AttributeStreamOfDbl(0);
			int dim = LinearPathIntersectsLinearPathMaxDim_(polyline_a, polyline_b, tolerance, intersections);
			if (dim != 0)
			{
				return false;
			}
			com.epl.geometry.MultiPoint intersection = new com.epl.geometry.MultiPoint();
			for (int i = 0; i < intersections.Size(); i += 2)
			{
				double x = intersections.Read(i);
				double y = intersections.Read(i + 1);
				intersection.Add(x, y);
			}
			com.epl.geometry.MultiPoint boundary_a_b = (com.epl.geometry.MultiPoint)(polyline_a.GetBoundary());
			com.epl.geometry.MultiPoint boundary_b = (com.epl.geometry.MultiPoint)(polyline_b.GetBoundary());
			boundary_a_b.Add(boundary_b, 0, boundary_b.GetPointCount());
			return !MultiPointContainsMultiPointBrute_(boundary_a_b, intersection, tolerance);
		}

		// Returns true if polyline_a overlaps polyline_b.
		private static bool PolylineOverlapsPolyline_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Polyline polyline_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint.
			if (TryRasterizedContainsOrDisjoint_(polyline_a, polyline_b, tolerance, false) == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return false;
			}
			return LinearPathOverlapsLinearPath_(polyline_a, polyline_b, tolerance);
		}

		// Returns true if polyline_a contains polyline_b.
		private static bool PolylineContainsPolyline_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Polyline polyline_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polyline_a.QueryEnvelope2D(env_a);
			polyline_b.QueryEnvelope2D(env_b);
			// Quick envelope rejection test for false equality.
			if (!EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance))
			{
				return false;
			}
			// Quick rasterize test to see whether the the geometries are disjoint.
			if (TryRasterizedContainsOrDisjoint_(polyline_a, polyline_b, tolerance, false) == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return false;
			}
			return LinearPathWithinLinearPath_(polyline_b, polyline_a, tolerance, false);
		}

		// Returns true if polyline_a is disjoint from point_b.
		private static bool PolylineDisjointPoint_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Point point_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint.
			if (TryRasterizedContainsOrDisjoint_(polyline_a, point_b, tolerance, false) == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return true;
			}
			com.epl.geometry.Point2D pt_b = point_b.GetXY();
			return !LinearPathIntersectsPoint_(polyline_a, pt_b, tolerance);
		}

		// Returns true if polyline_a touches point_b.
		private static bool PolylineTouchesPoint_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Point point_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint.
			if (TryRasterizedContainsOrDisjoint_(polyline_a, point_b, tolerance, false) == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return false;
			}
			com.epl.geometry.Point2D pt_b = point_b.GetXY();
			return LinearPathTouchesPointImpl_(polyline_a, pt_b, tolerance);
		}

		// Returns true of polyline_a contains point_b.
		private static bool PolylineContainsPoint_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Point point_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint.
			if (TryRasterizedContainsOrDisjoint_(polyline_a, point_b, tolerance, false) == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return false;
			}
			com.epl.geometry.Point2D pt_b = point_b.GetXY();
			return LinearPathContainsPoint_(polyline_a, pt_b, tolerance);
		}

		// Returns true if polyline_a is disjoint from multipoint_b.
		private static bool PolylineDisjointMultiPoint_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint.
			if (TryRasterizedContainsOrDisjoint_(polyline_a, multipoint_b, tolerance, false) == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return true;
			}
			return !LinearPathIntersectsMultiPoint_(polyline_a, multipoint_b, tolerance, false);
		}

		// Returns true if polyline_a touches multipoint_b.
		private static bool PolylineTouchesMultiPoint_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint.
			if (TryRasterizedContainsOrDisjoint_(polyline_a, multipoint_b, tolerance, false) == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return false;
			}
			com.epl.geometry.SegmentIteratorImpl segIterA = ((com.epl.geometry.MultiPathImpl)polyline_a._getImpl()).QuerySegmentIterator();
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D envInter = new com.epl.geometry.Envelope2D();
			polyline_a.QueryEnvelope2D(env_a);
			multipoint_b.QueryEnvelope2D(env_b);
			env_a.Inflate(tolerance, tolerance);
			env_b.Inflate(tolerance, tolerance);
			envInter.SetCoords(env_a);
			envInter.Intersect(env_b);
			com.epl.geometry.QuadTreeImpl qtA = null;
			com.epl.geometry.QuadTreeImpl quadTreeA = null;
			com.epl.geometry.QuadTreeImpl quadTreePathsA = null;
			com.epl.geometry.GeometryAccelerators accel = ((com.epl.geometry.MultiPathImpl)(polyline_a._getImpl()))._getAccelerators();
			if (accel != null)
			{
				quadTreeA = accel.GetQuadTree();
				quadTreePathsA = accel.GetQuadTreeForPaths();
				if (quadTreeA == null)
				{
					qtA = com.epl.geometry.InternalUtils.BuildQuadTree((com.epl.geometry.MultiPathImpl)polyline_a._getImpl(), envInter);
					quadTreeA = qtA;
				}
			}
			else
			{
				qtA = com.epl.geometry.InternalUtils.BuildQuadTree((com.epl.geometry.MultiPathImpl)polyline_a._getImpl(), envInter);
				quadTreeA = qtA;
			}
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl qtIterA = quadTreeA.GetIterator();
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl qtIterPathsA = null;
			if (quadTreePathsA != null)
			{
				qtIterPathsA = quadTreePathsA.GetIterator();
			}
			com.epl.geometry.Point2D ptB = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D closest = new com.epl.geometry.Point2D();
			bool b_intersects = false;
			double toleranceSq = tolerance * tolerance;
			com.epl.geometry.AttributeStreamOfInt8 intersects = new com.epl.geometry.AttributeStreamOfInt8(multipoint_b.GetPointCount());
			for (int i = 0; i < multipoint_b.GetPointCount(); i++)
			{
				intersects.Write(i, unchecked((byte)0));
			}
			for (int i_1 = 0; i_1 < multipoint_b.GetPointCount(); i_1++)
			{
				multipoint_b.GetXY(i_1, ptB);
				if (!envInter.Contains(ptB))
				{
					continue;
				}
				env_b.SetCoords(ptB.x, ptB.y, ptB.x, ptB.y);
				if (qtIterPathsA != null)
				{
					qtIterPathsA.ResetIterator(env_b, tolerance);
					if (qtIterPathsA.Next() == -1)
					{
						continue;
					}
				}
				qtIterA.ResetIterator(env_b, tolerance);
				for (int elementHandleA = qtIterA.Next(); elementHandleA != -1; elementHandleA = qtIterA.Next())
				{
					int vertex_a = quadTreeA.GetElement(elementHandleA);
					segIterA.ResetToVertex(vertex_a);
					com.epl.geometry.Segment segmentA = segIterA.NextSegment();
					double t = segmentA.GetClosestCoordinate(ptB, false);
					segmentA.GetCoord2D(t, closest);
					if (com.epl.geometry.Point2D.SqrDistance(ptB, closest) <= toleranceSq)
					{
						intersects.Write(i_1, unchecked((byte)1));
						b_intersects = true;
						break;
					}
				}
			}
			if (!b_intersects)
			{
				return false;
			}
			com.epl.geometry.MultiPoint boundary_a = (com.epl.geometry.MultiPoint)(polyline_a.GetBoundary());
			com.epl.geometry.MultiPoint multipoint_b_inter = new com.epl.geometry.MultiPoint();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			for (int i_2 = 0; i_2 < multipoint_b.GetPointCount(); i_2++)
			{
				if (intersects.Read(i_2) == 0)
				{
					continue;
				}
				multipoint_b.GetXY(i_2, pt);
				multipoint_b_inter.Add(pt.x, pt.y);
			}
			return MultiPointContainsMultiPointBrute_(boundary_a, multipoint_b_inter, tolerance);
		}

		// Returns true if polyline_a crosses multipoint_b.
		private static bool PolylineCrossesMultiPoint_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			// Quick rasterize test to see whether the the geometries are disjoint.
			if (TryRasterizedContainsOrDisjoint_(polyline_a, multipoint_b, tolerance, false) == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return false;
			}
			com.epl.geometry.SegmentIteratorImpl segIterA = ((com.epl.geometry.MultiPathImpl)polyline_a._getImpl()).QuerySegmentIterator();
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D envInter = new com.epl.geometry.Envelope2D();
			polyline_a.QueryEnvelope2D(env_a);
			multipoint_b.QueryEnvelope2D(env_b);
			env_a.Inflate(tolerance, tolerance);
			env_b.Inflate(tolerance, tolerance);
			envInter.SetCoords(env_a);
			envInter.Intersect(env_b);
			com.epl.geometry.QuadTreeImpl qtA = null;
			com.epl.geometry.QuadTreeImpl quadTreeA = null;
			com.epl.geometry.QuadTreeImpl quadTreePathsA = null;
			com.epl.geometry.GeometryAccelerators accel = ((com.epl.geometry.MultiPathImpl)(polyline_a._getImpl()))._getAccelerators();
			if (accel != null)
			{
				quadTreeA = accel.GetQuadTree();
				quadTreePathsA = accel.GetQuadTreeForPaths();
				if (quadTreeA == null)
				{
					qtA = com.epl.geometry.InternalUtils.BuildQuadTree((com.epl.geometry.MultiPathImpl)polyline_a._getImpl(), envInter);
					quadTreeA = qtA;
				}
			}
			else
			{
				qtA = com.epl.geometry.InternalUtils.BuildQuadTree((com.epl.geometry.MultiPathImpl)polyline_a._getImpl(), envInter);
				quadTreeA = qtA;
			}
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl qtIterA = quadTreeA.GetIterator();
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl qtIterPathsA = null;
			if (quadTreePathsA != null)
			{
				qtIterPathsA = quadTreePathsA.GetIterator();
			}
			com.epl.geometry.Point2D ptB = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D closest = new com.epl.geometry.Point2D();
			bool b_intersects = false;
			bool b_exterior_found = false;
			double toleranceSq = tolerance * tolerance;
			com.epl.geometry.AttributeStreamOfInt8 intersects = new com.epl.geometry.AttributeStreamOfInt8(multipoint_b.GetPointCount());
			for (int i = 0; i < multipoint_b.GetPointCount(); i++)
			{
				intersects.Write(i, unchecked((byte)0));
			}
			for (int i_1 = 0; i_1 < multipoint_b.GetPointCount(); i_1++)
			{
				multipoint_b.GetXY(i_1, ptB);
				if (!envInter.Contains(ptB))
				{
					b_exterior_found = true;
					continue;
				}
				env_b.SetCoords(ptB.x, ptB.y, ptB.x, ptB.y);
				if (qtIterPathsA != null)
				{
					qtIterPathsA.ResetIterator(env_b, tolerance);
					if (qtIterPathsA.Next() == -1)
					{
						b_exterior_found = true;
						continue;
					}
				}
				qtIterA.ResetIterator(env_b, tolerance);
				bool b_covered = false;
				for (int elementHandleA = qtIterA.Next(); elementHandleA != -1; elementHandleA = qtIterA.Next())
				{
					int vertex_a = quadTreeA.GetElement(elementHandleA);
					segIterA.ResetToVertex(vertex_a);
					com.epl.geometry.Segment segmentA = segIterA.NextSegment();
					double t = segmentA.GetClosestCoordinate(ptB, false);
					segmentA.GetCoord2D(t, closest);
					if (com.epl.geometry.Point2D.SqrDistance(ptB, closest) <= toleranceSq)
					{
						intersects.Write(i_1, unchecked((byte)1));
						b_intersects = true;
						b_covered = true;
						break;
					}
				}
				if (!b_covered)
				{
					b_exterior_found = true;
				}
			}
			if (!b_intersects || !b_exterior_found)
			{
				return false;
			}
			com.epl.geometry.MultiPoint boundary_a = (com.epl.geometry.MultiPoint)(polyline_a.GetBoundary());
			com.epl.geometry.MultiPoint multipoint_b_inter = new com.epl.geometry.MultiPoint();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			for (int i_2 = 0; i_2 < multipoint_b.GetPointCount(); i_2++)
			{
				if (intersects.Read(i_2) == 0)
				{
					continue;
				}
				multipoint_b.GetXY(i_2, pt);
				multipoint_b_inter.Add(pt.x, pt.y);
			}
			return !MultiPointContainsMultiPointBrute_(boundary_a, multipoint_b_inter, tolerance);
		}

		// Returns true if polyline_a contains multipoint_b.
		private static bool PolylineContainsMultiPoint_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polyline_a.QueryEnvelope2D(env_a);
			multipoint_b.QueryEnvelope2D(env_b);
			if (!EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance))
			{
				return false;
			}
			// Quick rasterize test to see whether the the geometries are disjoint.
			if (TryRasterizedContainsOrDisjoint_(polyline_a, multipoint_b, tolerance, false) == com.epl.geometry.RelationalOperations.Relation.disjoint)
			{
				return false;
			}
			if (!LinearPathIntersectsMultiPoint_(polyline_a, multipoint_b, tolerance, true))
			{
				return false;
			}
			com.epl.geometry.MultiPoint boundary_a = (com.epl.geometry.MultiPoint)(polyline_a.GetBoundary());
			return !MultiPointIntersectsMultiPoint_(boundary_a, multipoint_b, tolerance, progress_tracker);
		}

		// Returns true if polyline_a equals envelope_b.
		private static bool PolylineEqualsEnvelope_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polyline_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (env_b.GetHeight() > tolerance && env_b.GetWidth() > tolerance)
			{
				return false;
			}
			// area cannot equal a line
			return EnvelopeEqualsEnvelope_(env_a, env_b, tolerance, progress_tracker);
		}

		// Returns true if polyline_a is disjoint from envelope_b.
		private static bool PolylineDisjointEnvelope_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polyline_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance))
			{
				return false;
			}
			return !LinearPathIntersectsEnvelope_(polyline_a, env_b, tolerance, progress_tracker);
		}

		// Returns true if polyline_a touches envelope_b.
		private static bool PolylineTouchesEnvelope_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polyline_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (env_b.GetHeight() <= tolerance && env_b.GetWidth() <= tolerance)
			{
				// Treat
				// as
				// point
				com.epl.geometry.Point2D pt_b = envelope_b.GetCenterXY();
				return LinearPathTouchesPointImpl_(polyline_a, pt_b, tolerance);
			}
			if (env_b.GetHeight() <= tolerance || env_b.GetWidth() <= tolerance)
			{
				// Treat
				// as
				// polyline
				com.epl.geometry.Polyline polyline_b = new com.epl.geometry.Polyline();
				com.epl.geometry.Point p = new com.epl.geometry.Point();
				envelope_b.QueryCornerByVal(0, p);
				polyline_b.StartPath(p);
				envelope_b.QueryCornerByVal(2, p);
				polyline_b.LineTo(p);
				return PolylineTouchesPolyline_(polyline_a, polyline_b, tolerance, progress_tracker);
			}
			// Treat env_b as area
			com.epl.geometry.SegmentIterator seg_iter_a = polyline_a.QuerySegmentIterator();
			com.epl.geometry.Envelope2D env_b_deflated = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b_inflated = new com.epl.geometry.Envelope2D();
			env_b_deflated.SetCoords(env_b);
			env_b_inflated.SetCoords(env_b);
			env_b_deflated.Inflate(-tolerance, -tolerance);
			env_b_inflated.Inflate(tolerance, tolerance);
			bool b_boundary = false;
			com.epl.geometry.Envelope2D env_segment_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_inter = new com.epl.geometry.Envelope2D();
			while (seg_iter_a.NextPath())
			{
				while (seg_iter_a.HasNextSegment())
				{
					com.epl.geometry.Segment segment_a = seg_iter_a.NextSegment();
					segment_a.QueryEnvelope2D(env_segment_a);
					env_inter.SetCoords(env_b_deflated);
					env_inter.Intersect(env_segment_a);
					if (!env_inter.IsEmpty() && (env_inter.GetHeight() > tolerance || env_inter.GetWidth() > tolerance))
					{
						return false;
					}
					// consider segment within
					env_inter.SetCoords(env_b_inflated);
					env_inter.Intersect(env_segment_a);
					if (!env_inter.IsEmpty())
					{
						b_boundary = true;
					}
				}
			}
			return b_boundary;
		}

		// Returns true if polyline_a overlaps envelope_b.
		private static bool PolylineOverlapsEnvelope_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polyline_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance) || EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance))
			{
				return false;
			}
			if (EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance))
			{
				return false;
			}
			if (env_b.GetHeight() > tolerance && env_b.GetWidth() > tolerance)
			{
				return false;
			}
			// lines cannot overlap areas
			if (env_b.GetHeight() <= tolerance && env_b.GetWidth() <= tolerance)
			{
				return false;
			}
			// lines cannot overlap points
			// Treat as polyline
			com.epl.geometry.Polyline polyline_b = new com.epl.geometry.Polyline();
			com.epl.geometry.Point p = new com.epl.geometry.Point();
			envelope_b.QueryCornerByVal(0, p);
			polyline_b.StartPath(p);
			envelope_b.QueryCornerByVal(2, p);
			polyline_b.LineTo(p);
			return LinearPathOverlapsLinearPath_(polyline_a, polyline_b, tolerance);
		}

		// Returns true if polyline_a is within envelope_b.
		private static bool PolylineWithinEnvelope_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polyline_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (!EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance))
			{
				return false;
			}
			if (env_b.GetHeight() <= tolerance && env_b.GetWidth() <= tolerance)
			{
				return false;
			}
			if (env_b.GetHeight() <= tolerance || env_b.GetWidth() <= tolerance)
			{
				return EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance);
			}
			com.epl.geometry.SegmentIterator seg_iter_a = polyline_a.QuerySegmentIterator();
			com.epl.geometry.Envelope2D env_b_deflated = new com.epl.geometry.Envelope2D();
			env_b_deflated.SetCoords(env_b);
			env_b_deflated.Inflate(-tolerance, -tolerance);
			bool b_interior = false;
			com.epl.geometry.Envelope2D env_segment_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_inter = new com.epl.geometry.Envelope2D();
			while (seg_iter_a.NextPath())
			{
				while (seg_iter_a.HasNextSegment())
				{
					com.epl.geometry.Segment segment_a = seg_iter_a.NextSegment();
					segment_a.QueryEnvelope2D(env_segment_a);
					if (env_b_deflated.ContainsExclusive(env_segment_a))
					{
						b_interior = true;
						continue;
					}
					env_inter.SetCoords(env_b_deflated);
					env_inter.Intersect(env_segment_a);
					if (!env_inter.IsEmpty() && (env_inter.GetHeight() > tolerance || env_inter.GetWidth() > tolerance))
					{
						b_interior = true;
					}
				}
			}
			return b_interior;
		}

		// Returns true if polyline_a contains envelope_b.
		private static bool PolylineContainsEnvelope_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			envelope_b.QueryEnvelope2D(env_b);
			polyline_a.QueryEnvelope2D(env_a);
			if (!EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance))
			{
				return false;
			}
			if (env_b.GetHeight() > tolerance && env_b.GetWidth() > tolerance)
			{
				return false;
			}
			// when treated as an area, lines cannot contain
			// areas.
			if (env_b.GetHeight() <= tolerance && env_b.GetWidth() <= tolerance)
			{
				// Treat
				// as
				// point
				com.epl.geometry.Point2D pt_b = envelope_b.GetCenterXY();
				return LinearPathContainsPoint_(polyline_a, pt_b, tolerance);
			}
			// Treat as polyline
			com.epl.geometry.Polyline polyline_b = new com.epl.geometry.Polyline();
			com.epl.geometry.Point p = new com.epl.geometry.Point();
			envelope_b.QueryCornerByVal(0, p);
			polyline_b.StartPath(p);
			envelope_b.QueryCornerByVal(2, p);
			polyline_b.LineTo(p);
			return LinearPathWithinLinearPath_(polyline_b, polyline_a, tolerance, false);
		}

		// Returns true if polyline_a crosses envelope_b.
		private static bool PolylineCrossesEnvelope_(com.epl.geometry.Polyline polyline_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			polyline_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance))
			{
				return false;
			}
			if (env_b.GetHeight() <= tolerance && env_b.GetWidth() <= tolerance)
			{
				return false;
			}
			// when treated as a point, lines cannot cross points.
			if (env_b.GetHeight() <= tolerance || env_b.GetWidth() <= tolerance)
			{
				// Treat
				// as
				// polyline
				com.epl.geometry.Polyline polyline_b = new com.epl.geometry.Polyline();
				com.epl.geometry.Point p = new com.epl.geometry.Point();
				envelope_b.QueryCornerByVal(0, p);
				polyline_b.StartPath(p);
				envelope_b.QueryCornerByVal(2, p);
				polyline_b.LineTo(p);
				return PolylineCrossesPolyline_(polyline_a, polyline_b, tolerance, progress_tracker);
			}
			// Treat env_b as area
			com.epl.geometry.SegmentIterator seg_iter_a = polyline_a.QuerySegmentIterator();
			com.epl.geometry.Envelope2D env_b_inflated = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b_deflated = new com.epl.geometry.Envelope2D();
			env_b_deflated.SetCoords(env_b);
			env_b_inflated.SetCoords(env_b);
			env_b_deflated.Inflate(-tolerance, -tolerance);
			env_b_inflated.Inflate(tolerance, tolerance);
			bool b_interior = false;
			bool b_exterior = false;
			com.epl.geometry.Envelope2D env_segment_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_inter = new com.epl.geometry.Envelope2D();
			while (seg_iter_a.NextPath())
			{
				while (seg_iter_a.HasNextSegment())
				{
					com.epl.geometry.Segment segment_a = seg_iter_a.NextSegment();
					segment_a.QueryEnvelope2D(env_segment_a);
					if (!b_exterior)
					{
						if (!env_b_inflated.Contains(env_segment_a))
						{
							b_exterior = true;
						}
					}
					if (!b_interior)
					{
						env_inter.SetCoords(env_b_deflated);
						env_inter.Intersect(env_segment_a);
						if (!env_inter.IsEmpty() && (env_inter.GetHeight() > tolerance || env_inter.GetWidth() > tolerance))
						{
							b_interior = true;
						}
					}
					if (b_interior && b_exterior)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Returns true if multipoint_a equals multipoint_b.
		private static bool MultiPointEqualsMultiPoint_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			multipoint_a.QueryEnvelope2D(env_a);
			multipoint_b.QueryEnvelope2D(env_b);
			if (!EnvelopeEqualsEnvelope_(env_a, env_b, tolerance, progress_tracker))
			{
				return false;
			}
			if (MultiPointExactlyEqualsMultiPoint_(multipoint_a, multipoint_b, tolerance, progress_tracker))
			{
				return true;
			}
			return MultiPointCoverageMultiPoint_(multipoint_a, multipoint_b, tolerance, false, true, false, progress_tracker);
		}

		// Returns true if multipoint_a is disjoint from multipoint_b.
		private static bool MultiPointDisjointMultiPoint_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			return !MultiPointIntersectsMultiPoint_(multipoint_a, multipoint_b, tolerance, progress_tracker);
		}

		// Returns true if multipoint_a overlaps multipoint_b.
		private static bool MultiPointOverlapsMultiPoint_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			return MultiPointCoverageMultiPoint_(multipoint_a, multipoint_b, tolerance, false, false, true, progress_tracker);
		}

		// Returns true if multipoint_a contains multipoint_b.
		private static bool MultiPointContainsMultiPoint_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			multipoint_a.QueryEnvelope2D(env_a);
			multipoint_b.QueryEnvelope2D(env_b);
			if (!EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance))
			{
				return false;
			}
			return MultiPointCoverageMultiPoint_(multipoint_b, multipoint_a, tolerance, true, false, false, progress_tracker);
		}

		private static bool MultiPointContainsMultiPointBrute_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance)
		{
			double tolerance_sq = tolerance * tolerance;
			com.epl.geometry.Point2D pt_a = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D pt_b = new com.epl.geometry.Point2D();
			for (int i = 0; i < multipoint_b.GetPointCount(); i++)
			{
				multipoint_b.GetXY(i, pt_b);
				bool b_contained = false;
				for (int j = 0; j < multipoint_a.GetPointCount(); j++)
				{
					multipoint_a.GetXY(j, pt_a);
					if (com.epl.geometry.Point2D.SqrDistance(pt_a, pt_b) <= tolerance_sq)
					{
						b_contained = true;
						break;
					}
				}
				if (!b_contained)
				{
					return false;
				}
			}
			return true;
		}

		// Returns true if multipoint_a equals point_b.
		internal static bool MultiPointEqualsPoint_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.Point point_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			multipoint_a.QueryEnvelope2D(env_a);
			point_b.QueryEnvelope2D(env_b);
			return EnvelopeEqualsEnvelope_(env_a, env_b, tolerance, progress_tracker);
		}

		// Returns true if multipoint_a is disjoint from point_b.
		private static bool MultiPointDisjointPoint_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.Point point_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Point2D pt_b = point_b.GetXY();
			return MultiPointDisjointPointImpl_(multipoint_a, pt_b, tolerance, progress_tracker);
		}

		// Returns true if multipoint_a is within point_b.
		private static bool MultiPointWithinPoint_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.Point point_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			return MultiPointEqualsPoint_(multipoint_a, point_b, tolerance, progress_tracker);
		}

		// Returns true if multipoint_a contains point_b.
		private static bool MultiPointContainsPoint_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.Point point_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			return !MultiPointDisjointPoint_(multipoint_a, point_b, tolerance, progress_tracker);
		}

		// Returns true if multipoint_a equals envelope_b.
		private static bool MultiPointEqualsEnvelope_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			multipoint_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (env_b.GetHeight() > tolerance || env_b.GetWidth() > tolerance)
			{
				return false;
			}
			// only true if all the points of the multi_point degenerate to a point
			// equal to the envelope
			return EnvelopeEqualsEnvelope_(env_a, env_b, tolerance, progress_tracker);
		}

		// Returns true if multipoint_a is disjoint from envelope_b.
		private static bool MultiPointDisjointEnvelope_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			multipoint_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance))
			{
				return false;
			}
			com.epl.geometry.Envelope2D env_b_inflated = new com.epl.geometry.Envelope2D();
			env_b_inflated.SetCoords(env_b);
			env_b_inflated.Inflate(tolerance, tolerance);
			com.epl.geometry.Point2D pt_a = new com.epl.geometry.Point2D();
			for (int i = 0; i < multipoint_a.GetPointCount(); i++)
			{
				multipoint_a.GetXY(i, pt_a);
				if (!env_b_inflated.Contains(pt_a))
				{
					continue;
				}
				return false;
			}
			return true;
		}

		// Returns true if multipoint_a touches envelope_b.
		private static bool MultiPointTouchesEnvelope_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b_inflated = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b_deflated = new com.epl.geometry.Envelope2D();
			envelope_b.QueryEnvelope2D(env_b);
			if (env_b.GetHeight() <= tolerance && env_b.GetWidth() <= tolerance)
			{
				return false;
			}
			// there are no boundaries to intersect
			if (env_b.GetHeight() <= tolerance || env_b.GetWidth() <= tolerance)
			{
				// treat
				// as
				// line
				com.epl.geometry.Point2D pt_a = new com.epl.geometry.Point2D();
				bool b_boundary = false;
				env_b_inflated.SetCoords(env_b);
				env_b_deflated.SetCoords(env_b);
				env_b_inflated.Inflate(tolerance, tolerance);
				if (env_b.GetHeight() > tolerance)
				{
					env_b_deflated.Inflate(0, -tolerance);
				}
				else
				{
					env_b_deflated.Inflate(-tolerance, 0);
				}
				for (int i = 0; i < multipoint_a.GetPointCount(); i++)
				{
					multipoint_a.GetXY(i, pt_a);
					if (!env_b_inflated.Contains(pt_a))
					{
						continue;
					}
					if (env_b.GetHeight() > tolerance)
					{
						if (pt_a.y > env_b_deflated.ymin && pt_a.y < env_b_deflated.ymax)
						{
							return false;
						}
						b_boundary = true;
					}
					else
					{
						if (pt_a.x > env_b_deflated.xmin && pt_a.x < env_b_deflated.xmax)
						{
							return false;
						}
						b_boundary = true;
					}
				}
				return b_boundary;
			}
			// treat as area
			env_b_inflated.SetCoords(env_b);
			env_b_deflated.SetCoords(env_b);
			env_b_inflated.Inflate(tolerance, tolerance);
			env_b_deflated.Inflate(-tolerance, -tolerance);
			com.epl.geometry.Point2D pt_a_1 = new com.epl.geometry.Point2D();
			bool b_boundary_1 = false;
			for (int i_1 = 0; i_1 < multipoint_a.GetPointCount(); i_1++)
			{
				multipoint_a.GetXY(i_1, pt_a_1);
				if (!env_b_inflated.Contains(pt_a_1))
				{
					continue;
				}
				if (env_b_deflated.ContainsExclusive(pt_a_1))
				{
					return false;
				}
				b_boundary_1 = true;
			}
			return b_boundary_1;
		}

		// Returns true if multipoint_a is within envelope_b.
		private static bool MultiPointWithinEnvelope_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			multipoint_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (!EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance))
			{
				return false;
			}
			if (env_b.GetHeight() <= tolerance && env_b.GetWidth() <= tolerance)
			{
				return EnvelopeEqualsEnvelope_(env_a, env_b, tolerance, progress_tracker);
			}
			// treat as point
			if (env_b.GetHeight() <= tolerance || env_b.GetWidth() <= tolerance)
			{
				// treat
				// as
				// line
				bool b_interior = false;
				com.epl.geometry.Envelope2D env_b_deflated = new com.epl.geometry.Envelope2D();
				com.epl.geometry.Envelope2D env_b_inflated = new com.epl.geometry.Envelope2D();
				env_b_deflated.SetCoords(env_b);
				env_b_inflated.SetCoords(env_b);
				if (env_b.GetHeight() > tolerance)
				{
					env_b_deflated.Inflate(0, -tolerance);
				}
				else
				{
					env_b_deflated.Inflate(-tolerance, 0);
				}
				env_b_inflated.Inflate(tolerance, tolerance);
				com.epl.geometry.Point2D pt_a = new com.epl.geometry.Point2D();
				for (int i = 0; i < multipoint_a.GetPointCount(); i++)
				{
					multipoint_a.GetXY(i, pt_a);
					if (!env_b_inflated.Contains(pt_a))
					{
						return false;
					}
					if (env_b.GetHeight() > tolerance)
					{
						if (pt_a.y > env_b_deflated.ymin && pt_a.y < env_b_deflated.ymax)
						{
							b_interior = true;
						}
					}
					else
					{
						if (pt_a.x > env_b_deflated.xmin && pt_a.x < env_b_deflated.xmax)
						{
							b_interior = true;
						}
					}
				}
				return b_interior;
			}
			// treat as area
			bool b_interior_1 = false;
			com.epl.geometry.Envelope2D env_b_deflated_1 = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b_inflated_1 = new com.epl.geometry.Envelope2D();
			env_b_deflated_1.SetCoords(env_b);
			env_b_inflated_1.SetCoords(env_b);
			env_b_deflated_1.Inflate(-tolerance, -tolerance);
			env_b_inflated_1.Inflate(tolerance, tolerance);
			com.epl.geometry.Point2D pt_a_1 = new com.epl.geometry.Point2D();
			// we loop to find a proper interior intersection (i.e. something inside
			// instead of just on the boundary)
			for (int i_1 = 0; i_1 < multipoint_a.GetPointCount(); i_1++)
			{
				multipoint_a.GetXY(i_1, pt_a_1);
				if (!env_b_inflated_1.Contains(pt_a_1))
				{
					return false;
				}
				if (env_b_deflated_1.ContainsExclusive(pt_a_1))
				{
					b_interior_1 = true;
				}
			}
			return b_interior_1;
		}

		// Returns true if multipoint_a contains envelope_b.
		private static bool MultiPointContainsEnvelope_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			multipoint_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (!EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance))
			{
				return false;
			}
			if (env_b.GetHeight() > tolerance || env_b.GetWidth() > tolerance)
			{
				return false;
			}
			com.epl.geometry.Point2D pt_b = envelope_b.GetCenterXY();
			return !MultiPointDisjointPointImpl_(multipoint_a, pt_b, tolerance, progress_tracker);
		}

		// Returns true if multipoint_a crosses envelope_b.
		internal static bool MultiPointCrossesEnvelope_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.Envelope envelope_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			multipoint_a.QueryEnvelope2D(env_a);
			envelope_b.QueryEnvelope2D(env_b);
			if (EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance))
			{
				return false;
			}
			if (env_b.GetHeight() <= tolerance && env_b.GetWidth() <= tolerance)
			{
				return false;
			}
			if (env_b.GetHeight() <= tolerance || env_b.GetWidth() <= tolerance)
			{
				// treat
				// as
				// line
				com.epl.geometry.Envelope2D env_b_deflated = new com.epl.geometry.Envelope2D();
				com.epl.geometry.Envelope2D env_b_inflated = new com.epl.geometry.Envelope2D();
				env_b_deflated.SetCoords(env_b);
				if (env_b.GetHeight() > tolerance)
				{
					env_b_deflated.Inflate(0, -tolerance);
				}
				else
				{
					env_b_deflated.Inflate(-tolerance, 0);
				}
				env_b_inflated.SetCoords(env_b);
				env_b_inflated.Inflate(tolerance, tolerance);
				com.epl.geometry.Point2D pt_a = new com.epl.geometry.Point2D();
				bool b_interior = false;
				bool b_exterior = false;
				for (int i = 0; i < multipoint_a.GetPointCount(); i++)
				{
					multipoint_a.GetXY(i, pt_a);
					if (!b_interior)
					{
						if (env_b.GetHeight() > tolerance)
						{
							if (pt_a.y > env_b_deflated.ymin && pt_a.y < env_b_deflated.ymax)
							{
								b_interior = true;
							}
						}
						else
						{
							if (pt_a.x > env_b_deflated.xmin && pt_a.x < env_b_deflated.xmax)
							{
								b_interior = true;
							}
						}
					}
					if (!b_exterior && !env_b_inflated.Contains(pt_a))
					{
						b_exterior = true;
					}
					if (b_interior && b_exterior)
					{
						return true;
					}
				}
				return false;
			}
			com.epl.geometry.Envelope2D env_b_deflated_1 = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b_inflated_1 = new com.epl.geometry.Envelope2D();
			env_b_deflated_1.SetCoords(env_b);
			env_b_deflated_1.Inflate(-tolerance, -tolerance);
			System.Diagnostics.Debug.Assert((!env_b_deflated_1.IsEmpty()));
			env_b_inflated_1.SetCoords(env_b);
			env_b_inflated_1.Inflate(tolerance, tolerance);
			com.epl.geometry.Point2D pt_a_1 = new com.epl.geometry.Point2D();
			bool b_interior_1 = false;
			bool b_exterior_1 = false;
			for (int i_1 = 0; i_1 < multipoint_a.GetPointCount(); i_1++)
			{
				multipoint_a.GetXY(i_1, pt_a_1);
				if (!b_interior_1 && env_b_deflated_1.ContainsExclusive(pt_a_1))
				{
					b_interior_1 = true;
				}
				if (!b_exterior_1 && !env_b_inflated_1.Contains(pt_a_1))
				{
					b_exterior_1 = true;
				}
				if (b_interior_1 && b_exterior_1)
				{
					return true;
				}
			}
			return false;
		}

		// Returns true if pt_a equals pt_b.
		private static bool PointEqualsPoint_(com.epl.geometry.Point2D pt_a, com.epl.geometry.Point2D pt_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (com.epl.geometry.Point2D.SqrDistance(pt_a, pt_b) <= tolerance * tolerance)
			{
				return true;
			}
			return false;
		}

		// Returns true if pt_a is disjoint from pt_b.
		private static bool PointDisjointPoint_(com.epl.geometry.Point2D pt_a, com.epl.geometry.Point2D pt_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (com.epl.geometry.Point2D.SqrDistance(pt_a, pt_b) > tolerance * tolerance)
			{
				return true;
			}
			return false;
		}

		// Returns true if pt_a contains pt_b.
		private static bool PointContainsPoint_(com.epl.geometry.Point2D pt_a, com.epl.geometry.Point2D pt_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			return PointEqualsPoint_(pt_a, pt_b, tolerance, progress_tracker);
		}

		// Returns true if pt_a equals enve_b.
		private static bool PointEqualsEnvelope_(com.epl.geometry.Point2D pt_a, com.epl.geometry.Envelope2D env_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			env_a.SetCoords(pt_a);
			return EnvelopeEqualsEnvelope_(env_a, env_b, tolerance, progress_tracker);
		}

		// Returns true if pt_a is disjoint from env_b.
		internal static bool PointDisjointEnvelope_(com.epl.geometry.Point2D pt_a, com.epl.geometry.Envelope2D env_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_b_inflated = new com.epl.geometry.Envelope2D();
			env_b_inflated.SetCoords(env_b);
			env_b_inflated.Inflate(tolerance, tolerance);
			return !env_b_inflated.Contains(pt_a);
		}

		// Returns true if pt_a touches env_b.
		private static bool PointTouchesEnvelope_(com.epl.geometry.Point2D pt_a, com.epl.geometry.Envelope2D env_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (env_b.GetHeight() <= tolerance && env_b.GetWidth() <= tolerance)
			{
				return false;
			}
			// when treates as a point, points cannot touch points
			com.epl.geometry.Envelope2D env_b_inflated = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b_deflated = new com.epl.geometry.Envelope2D();
			env_b_inflated.SetCoords(env_b);
			env_b_inflated.Inflate(tolerance, tolerance);
			if (!env_b_inflated.Contains(pt_a))
			{
				return false;
			}
			if (env_b.GetHeight() <= tolerance || env_b.GetWidth() <= tolerance)
			{
				env_b_deflated.SetCoords(env_b);
				if (env_b.GetHeight() > tolerance)
				{
					env_b_deflated.Inflate(0, -tolerance);
				}
				else
				{
					env_b_deflated.Inflate(-tolerance, 0);
				}
				if (env_b.GetHeight() > tolerance)
				{
					if (pt_a.y > env_b_deflated.ymin && pt_a.y < env_b_deflated.ymax)
					{
						return false;
					}
				}
				else
				{
					if (pt_a.x > env_b_deflated.xmin && pt_a.x < env_b_deflated.xmax)
					{
						return false;
					}
				}
				return true;
			}
			env_b_deflated.SetCoords(env_b);
			env_b_deflated.Inflate(-tolerance, -tolerance);
			if (env_b_deflated.ContainsExclusive(pt_a))
			{
				return false;
			}
			return true;
		}

		// Returns true if pt_a is within env_b.
		private static bool PointWithinEnvelope_(com.epl.geometry.Point2D pt_a, com.epl.geometry.Envelope2D env_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (env_b.GetHeight() <= tolerance && env_b.GetWidth() <= tolerance)
			{
				// assert(env_b_inflated.contains(pt_a)); // should contain if we
				// got to here (i.e. not disjoint)
				return true;
			}
			if (env_b.GetHeight() <= tolerance || env_b.GetWidth() <= tolerance)
			{
				// treat
				// as
				// line
				com.epl.geometry.Envelope2D env_b_deflated = new com.epl.geometry.Envelope2D();
				env_b_deflated.SetCoords(env_b);
				if (env_b.GetHeight() > tolerance)
				{
					env_b_deflated.Inflate(0, -tolerance);
				}
				else
				{
					env_b_deflated.Inflate(-tolerance, 0);
				}
				bool b_interior = false;
				if (env_b.GetHeight() > tolerance)
				{
					if (pt_a.y > env_b_deflated.ymin && pt_a.y < env_b_deflated.ymax)
					{
						b_interior = true;
					}
				}
				else
				{
					if (pt_a.x > env_b_deflated.xmin && pt_a.x < env_b_deflated.xmax)
					{
						b_interior = true;
					}
				}
				return b_interior;
			}
			// treat as area
			com.epl.geometry.Envelope2D env_b_deflated_1 = new com.epl.geometry.Envelope2D();
			env_b_deflated_1.SetCoords(env_b);
			env_b_deflated_1.Inflate(-tolerance, -tolerance);
			return env_b_deflated_1.ContainsExclusive(pt_a);
		}

		// Returns true if pt_a contains env_b.
		private static bool PointContainsEnvelope_(com.epl.geometry.Point2D pt_a, com.epl.geometry.Envelope2D env_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			return PointEqualsEnvelope_(pt_a, env_b, tolerance, progress_tracker);
		}

		// Returns true if env_a equals env_b.
		private static bool EnvelopeEqualsEnvelope_(com.epl.geometry.Envelope2D env_a, com.epl.geometry.Envelope2D env_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			return EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance) && EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance);
		}

		// Returns true if env_a is disjoint from env_b.
		internal static bool EnvelopeDisjointEnvelope_(com.epl.geometry.Envelope2D env_a, com.epl.geometry.Envelope2D env_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Envelope2D env_b_inflated = new com.epl.geometry.Envelope2D();
			env_b_inflated.SetCoords(env_b);
			env_b_inflated.Inflate(tolerance, tolerance);
			return !env_a.IsIntersecting(env_b_inflated);
		}

		// Returns true if env_a touches env_b.
		private static bool EnvelopeTouchesEnvelope_(com.epl.geometry.Envelope2D env_a, com.epl.geometry.Envelope2D env_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (env_a.GetHeight() <= tolerance && env_a.GetWidth() <= tolerance)
			{
				// treat
				// env_a
				// as
				// point
				com.epl.geometry.Point2D pt_a = env_a.GetCenter();
				return PointTouchesEnvelope_(pt_a, env_b, tolerance, progress_tracker);
			}
			if (env_b.GetHeight() <= tolerance && env_b.GetWidth() <= tolerance)
			{
				// treat
				// env_b
				// as
				// point
				com.epl.geometry.Point2D pt_b = env_b.GetCenter();
				return PointTouchesEnvelope_(pt_b, env_a, tolerance, progress_tracker);
			}
			com.epl.geometry.Envelope2D _env_a;
			com.epl.geometry.Envelope2D _env_b;
			if (env_a.GetHeight() > tolerance && env_a.GetWidth() > tolerance && (env_b.GetHeight() <= tolerance || env_b.GetWidth() <= tolerance))
			{
				// swap a and b
				_env_a = env_b;
				_env_b = env_a;
			}
			else
			{
				_env_a = env_a;
				_env_b = env_b;
			}
			if (_env_a.GetHeight() <= tolerance || _env_a.GetWidth() <= tolerance)
			{
				// treat
				// env_a
				// as
				// line
				if (_env_b.GetHeight() <= tolerance || _env_b.GetWidth() <= tolerance)
				{
					// treat env_b as line
					com.epl.geometry.Line line_a = new com.epl.geometry.Line();
					com.epl.geometry.Line line_b = new com.epl.geometry.Line();
					double[] scalars_a = new double[2];
					double[] scalars_b = new double[2];
					com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
					_env_a.QueryLowerLeft(pt);
					line_a.SetStartXY(pt);
					_env_a.QueryUpperRight(pt);
					line_a.SetEndXY(pt);
					_env_b.QueryLowerLeft(pt);
					line_b.SetStartXY(pt);
					_env_b.QueryUpperRight(pt);
					line_b.SetEndXY(pt);
					line_a.Intersect(line_b, null, scalars_a, scalars_b, tolerance);
					int count = line_a.Intersect(line_b, null, null, null, tolerance);
					if (count != 1)
					{
						return false;
					}
					return scalars_a[0] == 0.0 || scalars_a[1] == 1.0 || scalars_b[0] == 0.0 || scalars_b[1] == 1.0;
				}
				// treat env_b as area
				com.epl.geometry.Envelope2D env_b_deflated = new com.epl.geometry.Envelope2D();
				com.epl.geometry.Envelope2D env_inter = new com.epl.geometry.Envelope2D();
				env_b_deflated.SetCoords(_env_b);
				env_b_deflated.Inflate(-tolerance, -tolerance);
				env_inter.SetCoords(env_b_deflated);
				env_inter.Intersect(_env_a);
				if (!env_inter.IsEmpty() && (env_inter.GetHeight() > tolerance || env_inter.GetWidth() > tolerance))
				{
					return false;
				}
				System.Diagnostics.Debug.Assert((!EnvelopeDisjointEnvelope_(_env_a, _env_b, tolerance, progress_tracker)));
				return true;
			}
			// we already know they intersect within a tolerance
			com.epl.geometry.Envelope2D env_inter_1 = new com.epl.geometry.Envelope2D();
			env_inter_1.SetCoords(_env_a);
			env_inter_1.Intersect(_env_b);
			if (!env_inter_1.IsEmpty() && env_inter_1.GetHeight() > tolerance && env_inter_1.GetWidth() > tolerance)
			{
				return false;
			}
			return true;
		}

		// we already know they intersect within a tolerance
		// Returns true if env_a overlaps env_b.
		private static bool EnvelopeOverlapsEnvelope_(com.epl.geometry.Envelope2D env_a, com.epl.geometry.Envelope2D env_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance) || EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance))
			{
				return false;
			}
			if (env_a.GetHeight() <= tolerance && env_a.GetWidth() <= tolerance)
			{
				return false;
			}
			// points cannot overlap
			if (env_b.GetHeight() <= tolerance && env_b.GetWidth() <= tolerance)
			{
				return false;
			}
			// points cannot overlap
			if (env_a.GetHeight() <= tolerance || env_a.GetWidth() <= tolerance)
			{
				// treat
				// env_a
				// as
				// a
				// line
				if (env_b.GetHeight() > tolerance && env_b.GetWidth() > tolerance)
				{
					return false;
				}
				// lines cannot overlap areas
				// treat both as lines
				com.epl.geometry.Line line_a = new com.epl.geometry.Line();
				com.epl.geometry.Line line_b = new com.epl.geometry.Line();
				double[] scalars_a = new double[2];
				double[] scalars_b = new double[2];
				com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
				env_a.QueryLowerLeft(pt);
				line_a.SetStartXY(pt);
				env_a.QueryUpperRight(pt);
				line_a.SetEndXY(pt);
				env_b.QueryLowerLeft(pt);
				line_b.SetStartXY(pt);
				env_b.QueryUpperRight(pt);
				line_b.SetEndXY(pt);
				line_a.Intersect(line_b, null, scalars_a, scalars_b, tolerance);
				int count = line_a.Intersect(line_b, null, null, null, tolerance);
				if (count != 2)
				{
					return false;
				}
				return (scalars_a[0] > 0.0 || scalars_a[1] < 1.0) && (scalars_b[0] > 0.0 || scalars_b[1] < 1.0);
			}
			if (env_b.GetHeight() <= tolerance || env_b.GetWidth() <= tolerance)
			{
				return false;
			}
			// lines cannot overlap areas
			// treat both as areas
			com.epl.geometry.Envelope2D env_inter = new com.epl.geometry.Envelope2D();
			env_inter.SetCoords(env_a);
			env_inter.Intersect(env_b);
			if (env_inter.IsEmpty())
			{
				return false;
			}
			if (env_inter.GetHeight() <= tolerance || env_inter.GetWidth() <= tolerance)
			{
				return false;
			}
			// not an area
			System.Diagnostics.Debug.Assert((!EnvelopeInfContainsEnvelope_(env_inter, env_a, tolerance) && !EnvelopeInfContainsEnvelope_(env_inter, env_b, tolerance)));
			return true;
		}

		// Returns true if env_a contains env_b.
		private static bool EnvelopeContainsEnvelope_(com.epl.geometry.Envelope2D env_a, com.epl.geometry.Envelope2D env_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (!EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance))
			{
				return false;
			}
			if (env_a.GetHeight() <= tolerance && env_a.GetWidth() <= tolerance)
			{
				com.epl.geometry.Point2D pt_a = env_a.GetCenter();
				return PointWithinEnvelope_(pt_a, env_b, tolerance, progress_tracker);
			}
			if (env_b.GetHeight() <= tolerance && env_b.GetWidth() <= tolerance)
			{
				com.epl.geometry.Point2D pt_b = env_b.GetCenter();
				return PointWithinEnvelope_(pt_b, env_a, tolerance, progress_tracker);
			}
			if (env_a.GetHeight() <= tolerance || env_a.GetWidth() <= tolerance)
			{
				return EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance);
			}
			// treat
			// env_b
			// as
			// line
			// treat env_a as area
			if (env_b.GetHeight() <= tolerance || env_b.GetWidth() <= tolerance)
			{
				// treat
				// env_b
				// as
				// line
				com.epl.geometry.Envelope2D env_a_deflated = new com.epl.geometry.Envelope2D();
				env_a_deflated.SetCoords(env_a);
				env_a_deflated.Inflate(-tolerance, -tolerance);
				if (env_a_deflated.ContainsExclusive(env_b))
				{
					return true;
				}
				com.epl.geometry.Envelope2D env_inter = new com.epl.geometry.Envelope2D();
				env_inter.SetCoords(env_a_deflated);
				env_inter.Intersect(env_b);
				if (env_inter.IsEmpty() || (env_inter.GetHeight() <= tolerance && env_inter.GetWidth() <= tolerance))
				{
					return false;
				}
				return true;
			}
			return EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance);
		}

		// Returns true if env_a crosses env_b.
		private static bool EnvelopeCrossesEnvelope_(com.epl.geometry.Envelope2D env_a, com.epl.geometry.Envelope2D env_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (EnvelopeInfContainsEnvelope_(env_a, env_b, tolerance) || EnvelopeInfContainsEnvelope_(env_b, env_a, tolerance))
			{
				return false;
			}
			if (env_a.GetHeight() <= tolerance && env_a.GetWidth() <= tolerance)
			{
				return false;
			}
			// points cannot cross
			if (env_b.GetHeight() <= tolerance && env_b.GetWidth() <= tolerance)
			{
				return false;
			}
			// points cannot cross
			if (env_b.GetHeight() > tolerance && env_b.GetWidth() > tolerance)
			{
				if (env_a.GetHeight() > tolerance && env_a.GetWidth() > tolerance)
				{
					return false;
				}
			}
			// areas cannot cross
			com.epl.geometry.Envelope2D _env_a;
			com.epl.geometry.Envelope2D _env_b;
			if (env_a.GetHeight() > tolerance && env_a.GetWidth() > tolerance)
			{
				// swap b and a
				_env_a = env_b;
				_env_b = env_a;
			}
			else
			{
				_env_a = env_a;
				_env_b = env_b;
			}
			if (_env_b.GetHeight() > tolerance && _env_b.GetWidth() > tolerance)
			{
				// treat
				// env_b
				// as
				// an
				// area
				// (env_a
				// as
				// a
				// line);
				com.epl.geometry.Envelope2D env_inter = new com.epl.geometry.Envelope2D();
				com.epl.geometry.Envelope2D env_b_deflated = new com.epl.geometry.Envelope2D();
				env_b_deflated.SetCoords(_env_b);
				env_b_deflated.Inflate(-tolerance, -tolerance);
				env_inter.SetCoords(env_b_deflated);
				env_inter.Intersect(_env_a);
				if (env_inter.IsEmpty())
				{
					return false;
				}
				if (env_inter.GetHeight() <= tolerance && env_inter.GetWidth() <= tolerance)
				{
					return false;
				}
				// not a line
				System.Diagnostics.Debug.Assert((!EnvelopeInfContainsEnvelope_(env_inter, _env_a, tolerance)));
				return true;
			}
			// treat both as lines
			com.epl.geometry.Line line_a = new com.epl.geometry.Line();
			com.epl.geometry.Line line_b = new com.epl.geometry.Line();
			double[] scalars_a = new double[2];
			double[] scalars_b = new double[2];
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			_env_a.QueryLowerLeft(pt);
			line_a.SetStartXY(pt);
			_env_a.QueryUpperRight(pt);
			line_a.SetEndXY(pt);
			_env_b.QueryLowerLeft(pt);
			line_b.SetStartXY(pt);
			_env_b.QueryUpperRight(pt);
			line_b.SetEndXY(pt);
			line_a.Intersect(line_b, null, scalars_a, scalars_b, tolerance);
			int count = line_a.Intersect(line_b, null, null, null, tolerance);
			if (count != 1)
			{
				return false;
			}
			return scalars_a[0] > 0.0 && scalars_a[1] < 1.0 && scalars_b[0] > 0.0 && scalars_b[1] < 1.0;
		}

		// Returns true if polygon_a is disjoint from multipath_b.
		private static bool PolygonDisjointMultiPath_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.MultiPath multipath_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.Point2D pt_a;
			com.epl.geometry.Point2D pt_b;
			com.epl.geometry.Envelope2D env_a_inf = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b_inf = new com.epl.geometry.Envelope2D();
			com.epl.geometry.MultiPathImpl multi_path_impl_a = (com.epl.geometry.MultiPathImpl)polygon_a._getImpl();
			com.epl.geometry.MultiPathImpl multi_path_impl_b = (com.epl.geometry.MultiPathImpl)multipath_b._getImpl();
			com.epl.geometry.PairwiseIntersectorImpl intersector = new com.epl.geometry.PairwiseIntersectorImpl(multi_path_impl_a, multi_path_impl_b, tolerance, true);
			if (!intersector.Next())
			{
				return true;
			}
			// no rings intersect
			bool b_intersects = LinearPathIntersectsLinearPath_(polygon_a, multipath_b, tolerance);
			if (b_intersects)
			{
				return false;
			}
			com.epl.geometry.Polygon pa = null;
			com.epl.geometry.Polygon p_polygon_a = polygon_a;
			com.epl.geometry.Polygon pb = null;
			com.epl.geometry.Polygon p_polygon_b = null;
			if (multipath_b.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon)
			{
				p_polygon_b = (com.epl.geometry.Polygon)multipath_b;
			}
			bool b_checked_polygon_a_quad_tree = false;
			bool b_checked_polygon_b_quad_tree = false;
			do
			{
				int path_a = intersector.GetRedElement();
				int path_b = intersector.GetBlueElement();
				pt_b = multipath_b.GetXY(multipath_b.GetPathStart(path_b));
				env_a_inf.SetCoords(intersector.GetRedEnvelope());
				env_a_inf.Inflate(tolerance, tolerance);
				if (env_a_inf.Contains(pt_b))
				{
					com.epl.geometry.PolygonUtils.PiPResult result = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(p_polygon_a, pt_b, 0.0);
					if (result != com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
					{
						return false;
					}
				}
				if (multipath_b.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon)
				{
					pt_a = polygon_a.GetXY(polygon_a.GetPathStart(path_a));
					env_b_inf.SetCoords(intersector.GetBlueEnvelope());
					env_b_inf.Inflate(tolerance, tolerance);
					if (env_b_inf.Contains(pt_a))
					{
						com.epl.geometry.PolygonUtils.PiPResult result = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(p_polygon_b, pt_a, 0.0);
						if (result != com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
						{
							return false;
						}
					}
				}
				if (!b_checked_polygon_a_quad_tree)
				{
					if (com.epl.geometry.PointInPolygonHelper.QuadTreeWillHelp(polygon_a, multipath_b.GetPathCount() - 1) && (multi_path_impl_a._getAccelerators() == null || multi_path_impl_a._getAccelerators().GetQuadTree() == null))
					{
						pa = new com.epl.geometry.Polygon();
						polygon_a.CopyTo(pa);
						((com.epl.geometry.MultiPathImpl)pa._getImpl())._buildQuadTreeAccelerator(com.epl.geometry.Geometry.GeometryAccelerationDegree.enumMedium);
						p_polygon_a = pa;
					}
					else
					{
						p_polygon_a = polygon_a;
					}
					b_checked_polygon_a_quad_tree = true;
				}
				if (multipath_b.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polygon)
				{
					if (!b_checked_polygon_b_quad_tree)
					{
						com.epl.geometry.Polygon polygon_b = (com.epl.geometry.Polygon)multipath_b;
						if (com.epl.geometry.PointInPolygonHelper.QuadTreeWillHelp(polygon_b, polygon_a.GetPathCount() - 1) && (multi_path_impl_b._getAccelerators() == null || multi_path_impl_b._getAccelerators().GetQuadTree() == null))
						{
							pb = new com.epl.geometry.Polygon();
							polygon_b.CopyTo(pb);
							((com.epl.geometry.MultiPathImpl)pb._getImpl())._buildQuadTreeAccelerator(com.epl.geometry.Geometry.GeometryAccelerationDegree.enumMedium);
							p_polygon_b = pb;
						}
						else
						{
							p_polygon_b = (com.epl.geometry.Polygon)multipath_b;
						}
						b_checked_polygon_b_quad_tree = true;
					}
				}
			}
			while (intersector.Next());
			return true;
		}

		// Returns true if env_a inflated contains env_b.
		private static bool EnvelopeInfContainsEnvelope_(com.epl.geometry.Envelope2D env_a, com.epl.geometry.Envelope2D env_b, double tolerance)
		{
			com.epl.geometry.Envelope2D env_a_inflated = new com.epl.geometry.Envelope2D();
			env_a_inflated.SetCoords(env_a);
			env_a_inflated.Inflate(tolerance, tolerance);
			return env_a_inflated.Contains(env_b);
		}

		// Returns true if a coordinate of envelope A is outside of envelope B.
		private static bool InteriorEnvExteriorEnv_(com.epl.geometry.Envelope2D env_a, com.epl.geometry.Envelope2D env_b, double tolerance)
		{
			com.epl.geometry.Envelope2D envBInflated = new com.epl.geometry.Envelope2D();
			envBInflated.SetCoords(env_b);
			envBInflated.Inflate(tolerance, tolerance);
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			env_a.QueryLowerLeft(pt);
			if (!envBInflated.Contains(pt))
			{
				return true;
			}
			env_a.QueryLowerRight(pt);
			if (!envBInflated.Contains(pt))
			{
				return true;
			}
			env_a.QueryUpperLeft(pt);
			if (!envBInflated.Contains(pt))
			{
				return true;
			}
			env_a.QueryUpperRight(pt);
			if (!envBInflated.Contains(pt))
			{
				return true;
			}
			System.Diagnostics.Debug.Assert((envBInflated.Contains(env_a)));
			return false;
		}

		// Returns true if the points in each path of multipathA are the same as
		// those in multipathB, within a tolerance, and in the same order.
		private static bool MultiPathExactlyEqualsMultiPath_(com.epl.geometry.MultiPath multipathA, com.epl.geometry.MultiPath multipathB, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (multipathA.GetPathCount() != multipathB.GetPathCount() || multipathA.GetPointCount() != multipathB.GetPointCount())
			{
				return false;
			}
			com.epl.geometry.Point2D ptA = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D ptB = new com.epl.geometry.Point2D();
			bool bAllPointsEqual = true;
			double tolerance_sq = tolerance * tolerance;
			for (int ipath = 0; ipath < multipathA.GetPathCount(); ipath++)
			{
				if (multipathA.GetPathEnd(ipath) != multipathB.GetPathEnd(ipath))
				{
					bAllPointsEqual = false;
					break;
				}
				for (int i = multipathA.GetPathStart(ipath); i < multipathB.GetPathEnd(ipath); i++)
				{
					multipathA.GetXY(i, ptA);
					multipathB.GetXY(i, ptB);
					if (com.epl.geometry.Point2D.SqrDistance(ptA, ptB) > tolerance_sq)
					{
						bAllPointsEqual = false;
						break;
					}
				}
				if (!bAllPointsEqual)
				{
					break;
				}
			}
			if (!bAllPointsEqual)
			{
				return false;
			}
			return true;
		}

		// Returns true if the points of multipoint_a are the same as those in
		// multipoint_b, within a tolerance, and in the same order.
		private static bool MultiPointExactlyEqualsMultiPoint_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.MultiPoint multipoint_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (multipoint_a.GetPointCount() != multipoint_b.GetPointCount())
			{
				return false;
			}
			com.epl.geometry.Point2D ptA = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D ptB = new com.epl.geometry.Point2D();
			bool bAllPointsEqual = true;
			double tolerance_sq = tolerance * tolerance;
			for (int i = 0; i < multipoint_a.GetPointCount(); i++)
			{
				multipoint_a.GetXY(i, ptA);
				multipoint_b.GetXY(i, ptB);
				if (com.epl.geometry.Point2D.SqrDistance(ptA, ptB) > tolerance_sq)
				{
					bAllPointsEqual = false;
					break;
				}
			}
			if (!bAllPointsEqual)
			{
				return false;
			}
			return true;
		}

		// By default this will perform the within operation if bEquals is false.
		// Otherwise it will do equals.
		private static bool MultiPointCoverageMultiPoint_(com.epl.geometry.MultiPoint _multipointA, com.epl.geometry.MultiPoint _multipointB, double tolerance, bool bPerformWithin, bool bPerformEquals, bool bPerformOverlaps, com.epl.geometry.ProgressTracker progress_tracker)
		{
			bool bPerformContains = false;
			com.epl.geometry.MultiPoint multipoint_a;
			com.epl.geometry.MultiPoint multipoint_b;
			if (_multipointA.GetPointCount() > _multipointB.GetPointCount())
			{
				if (bPerformWithin)
				{
					bPerformWithin = false;
					bPerformContains = true;
				}
				multipoint_a = _multipointB;
				multipoint_b = _multipointA;
			}
			else
			{
				multipoint_a = _multipointA;
				multipoint_b = _multipointB;
			}
			com.epl.geometry.AttributeStreamOfInt8 contained = null;
			if (bPerformEquals || bPerformOverlaps || bPerformContains)
			{
				contained = new com.epl.geometry.AttributeStreamOfInt8(multipoint_b.GetPointCount());
				for (int i = 0; i < multipoint_b.GetPointCount(); i++)
				{
					contained.Write(i, unchecked((byte)0));
				}
			}
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D envInter = new com.epl.geometry.Envelope2D();
			multipoint_a.QueryEnvelope2D(env_a);
			multipoint_b.QueryEnvelope2D(env_b);
			env_a.Inflate(tolerance, tolerance);
			env_b.Inflate(tolerance, tolerance);
			envInter.SetCoords(env_a);
			envInter.Intersect(env_b);
			com.epl.geometry.Point2D ptA = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D ptB = new com.epl.geometry.Point2D();
			bool bWithin = true;
			// starts off true by default
			com.epl.geometry.QuadTreeImpl quadTreeB = com.epl.geometry.InternalUtils.BuildQuadTree((com.epl.geometry.MultiPointImpl)(multipoint_b._getImpl()), envInter);
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl qtIterB = quadTreeB.GetIterator();
			double tolerance_sq = tolerance * tolerance;
			for (int vertex_a = 0; vertex_a < multipoint_a.GetPointCount(); vertex_a++)
			{
				multipoint_a.GetXY(vertex_a, ptA);
				if (!envInter.Contains(ptA))
				{
					if (bPerformEquals || bPerformWithin)
					{
						return false;
					}
					else
					{
						bWithin = false;
						continue;
					}
				}
				bool bPtACovered = false;
				env_a.SetCoords(ptA.x, ptA.y, ptA.x, ptA.y);
				qtIterB.ResetIterator(env_a, tolerance);
				for (int elementHandleB = qtIterB.Next(); elementHandleB != -1; elementHandleB = qtIterB.Next())
				{
					int vertex_b = quadTreeB.GetElement(elementHandleB);
					multipoint_b.GetXY(vertex_b, ptB);
					if (com.epl.geometry.Point2D.SqrDistance(ptA, ptB) <= tolerance_sq)
					{
						if (bPerformEquals || bPerformOverlaps || bPerformContains)
						{
							contained.Write(vertex_b, unchecked((byte)1));
						}
						bPtACovered = true;
						if (bPerformWithin)
						{
							break;
						}
					}
				}
				if (!bPtACovered)
				{
					bWithin = false;
					if (bPerformEquals || bPerformWithin)
					{
						return false;
					}
				}
			}
			if (bPerformOverlaps && bWithin)
			{
				return false;
			}
			if (bPerformWithin)
			{
				return true;
			}
			for (int i_1 = 0; i_1 < multipoint_b.GetPointCount(); i_1++)
			{
				if (contained.Read(i_1) == 1)
				{
					if (bPerformOverlaps)
					{
						return true;
					}
				}
				else
				{
					if (bPerformEquals || bPerformContains)
					{
						return false;
					}
				}
			}
			if (bPerformOverlaps)
			{
				return false;
			}
			return true;
		}

		// Returns true if multipoint_a intersects multipoint_b.
		private static bool MultiPointIntersectsMultiPoint_(com.epl.geometry.MultiPoint _multipointA, com.epl.geometry.MultiPoint _multipointB, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.MultiPoint multipoint_a;
			com.epl.geometry.MultiPoint multipoint_b;
			if (_multipointA.GetPointCount() > _multipointB.GetPointCount())
			{
				multipoint_a = _multipointB;
				multipoint_b = _multipointA;
			}
			else
			{
				multipoint_a = _multipointA;
				multipoint_b = _multipointB;
			}
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D envInter = new com.epl.geometry.Envelope2D();
			multipoint_a.QueryEnvelope2D(env_a);
			multipoint_b.QueryEnvelope2D(env_b);
			env_a.Inflate(tolerance, tolerance);
			env_b.Inflate(tolerance, tolerance);
			envInter.SetCoords(env_a);
			envInter.Intersect(env_b);
			com.epl.geometry.Point2D ptA = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D ptB = new com.epl.geometry.Point2D();
			double tolerance_sq = tolerance * tolerance;
			com.epl.geometry.QuadTreeImpl quadTreeB = com.epl.geometry.InternalUtils.BuildQuadTree((com.epl.geometry.MultiPointImpl)(multipoint_b._getImpl()), envInter);
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl qtIterB = quadTreeB.GetIterator();
			for (int vertex_a = 0; vertex_a < multipoint_a.GetPointCount(); vertex_a++)
			{
				multipoint_a.GetXY(vertex_a, ptA);
				if (!envInter.Contains(ptA))
				{
					continue;
				}
				env_a.SetCoords(ptA.x, ptA.y, ptA.x, ptA.y);
				qtIterB.ResetIterator(env_a, tolerance);
				for (int elementHandleB = qtIterB.Next(); elementHandleB != -1; elementHandleB = qtIterB.Next())
				{
					int vertex_b = quadTreeB.GetElement(elementHandleB);
					multipoint_b.GetXY(vertex_b, ptB);
					if (com.epl.geometry.Point2D.SqrDistance(ptA, ptB) <= tolerance_sq)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Returns true if multipathA equals multipathB.
		private static bool LinearPathEqualsLinearPath_(com.epl.geometry.MultiPath multipathA, com.epl.geometry.MultiPath multipathB, double tolerance, bool bEnforceOrientation)
		{
			return LinearPathWithinLinearPath_(multipathA, multipathB, tolerance, bEnforceOrientation) && LinearPathWithinLinearPath_(multipathB, multipathA, tolerance, bEnforceOrientation);
		}

		// Returns true if the segments of multipathA are within the segments of
		// multipathB.
		private static bool LinearPathWithinLinearPath_(com.epl.geometry.MultiPath multipathA, com.epl.geometry.MultiPath multipathB, double tolerance, bool bEnforceOrientation)
		{
			bool bWithin = true;
			double[] scalarsA = new double[2];
			double[] scalarsB = new double[2];
			int ievent = 0;
			com.epl.geometry.AttributeStreamOfInt32 eventIndices = new com.epl.geometry.AttributeStreamOfInt32(0);
			com.epl.geometry.RelationalOperations relOps = new com.epl.geometry.RelationalOperations();
			com.epl.geometry.RelationalOperations.OverlapComparer overlapComparer = new com.epl.geometry.RelationalOperations.OverlapComparer(relOps);
			com.epl.geometry.RelationalOperations.OverlapEvent overlapEvent;
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D envInter = new com.epl.geometry.Envelope2D();
			multipathA.QueryEnvelope2D(env_a);
			multipathB.QueryEnvelope2D(env_b);
			env_a.Inflate(tolerance, tolerance);
			env_b.Inflate(tolerance, tolerance);
			envInter.SetCoords(env_a);
			envInter.Intersect(env_b);
			com.epl.geometry.SegmentIteratorImpl segIterA = ((com.epl.geometry.MultiPathImpl)multipathA._getImpl()).QuerySegmentIterator();
			com.epl.geometry.SegmentIteratorImpl segIterB = ((com.epl.geometry.MultiPathImpl)multipathB._getImpl()).QuerySegmentIterator();
			com.epl.geometry.QuadTreeImpl qtB = null;
			com.epl.geometry.QuadTreeImpl quadTreeB = null;
			com.epl.geometry.QuadTreeImpl quadTreePathsB = null;
			com.epl.geometry.GeometryAccelerators accel = ((com.epl.geometry.MultiPathImpl)multipathB._getImpl())._getAccelerators();
			if (accel != null)
			{
				quadTreeB = accel.GetQuadTree();
				quadTreePathsB = accel.GetQuadTreeForPaths();
				if (quadTreeB == null)
				{
					qtB = com.epl.geometry.InternalUtils.BuildQuadTree((com.epl.geometry.MultiPathImpl)multipathB._getImpl(), envInter);
					quadTreeB = qtB;
				}
			}
			else
			{
				qtB = com.epl.geometry.InternalUtils.BuildQuadTree((com.epl.geometry.MultiPathImpl)multipathB._getImpl(), envInter);
				quadTreeB = qtB;
			}
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl qtIterB = quadTreeB.GetIterator();
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl qtIterPathsB = null;
			if (quadTreePathsB != null)
			{
				qtIterPathsB = quadTreePathsB.GetIterator();
			}
			while (segIterA.NextPath())
			{
				while (segIterA.HasNextSegment())
				{
					bool bStringOfSegmentAsCovered = false;
					com.epl.geometry.Segment segmentA = segIterA.NextSegment();
					segmentA.QueryEnvelope2D(env_a);
					if (!env_a.IsIntersecting(envInter))
					{
						return false;
					}
					// bWithin = false
					if (qtIterPathsB != null)
					{
						qtIterPathsB.ResetIterator(env_a, tolerance);
						if (qtIterPathsB.Next() == -1)
						{
							bWithin = false;
							return false;
						}
					}
					double lengthA = segmentA.CalculateLength2D();
					qtIterB.ResetIterator(segmentA, tolerance);
					for (int elementHandleB = qtIterB.Next(); elementHandleB != -1; elementHandleB = qtIterB.Next())
					{
						int vertex_b = quadTreeB.GetElement(elementHandleB);
						segIterB.ResetToVertex(vertex_b);
						com.epl.geometry.Segment segmentB = segIterB.NextSegment();
						int result = segmentA.Intersect(segmentB, null, scalarsA, scalarsB, tolerance);
						if (result == 2 && (!bEnforceOrientation || scalarsB[0] <= scalarsB[1]))
						{
							double scalar_a_0 = scalarsA[0];
							double scalar_a_1 = scalarsA[1];
							double scalar_b_0 = scalarsB[0];
							double scalar_b_1 = scalarsB[1];
							// Performance enhancement for nice cases where
							// localization occurs. Increment segIterA as far as we
							// can while the current segmentA is covered.
							if (scalar_a_0 * lengthA <= tolerance && (1.0 - scalar_a_1) * lengthA <= tolerance)
							{
								bStringOfSegmentAsCovered = true;
								ievent = 0;
								eventIndices.Resize(0);
								relOps.m_overlap_events.Clear();
								int ivertex_a = segIterA.GetStartPointIndex();
								bool bSegmentACovered = true;
								while (bSegmentACovered)
								{
									// keep going while the
									// current segmentA is
									// covered.
									if (segIterA.HasNextSegment())
									{
										segmentA = segIterA.NextSegment();
										lengthA = segmentA.CalculateLength2D();
										result = segmentA.Intersect(segmentB, null, scalarsA, scalarsB, tolerance);
										if (result == 2 && (!bEnforceOrientation || scalarsB[0] <= scalarsB[1]))
										{
											scalar_a_0 = scalarsA[0];
											scalar_a_1 = scalarsA[1];
											if (scalar_a_0 * lengthA <= tolerance && (1.0 - scalar_a_1) * lengthA <= tolerance)
											{
												ivertex_a = segIterA.GetStartPointIndex();
												continue;
											}
										}
										if (segIterB.HasNextSegment())
										{
											segmentB = segIterB.NextSegment();
											result = segmentA.Intersect(segmentB, null, scalarsA, scalarsB, tolerance);
											if (result == 2 && (!bEnforceOrientation || scalarsB[0] <= scalarsB[1]))
											{
												scalar_a_0 = scalarsA[0];
												scalar_a_1 = scalarsA[1];
												if (scalar_a_0 * lengthA <= tolerance && (1.0 - scalar_a_1) * lengthA <= tolerance)
												{
													ivertex_a = segIterA.GetStartPointIndex();
													continue;
												}
											}
										}
									}
									bSegmentACovered = false;
								}
								if (ivertex_a != segIterA.GetStartPointIndex())
								{
									segIterA.ResetToVertex(ivertex_a);
									segIterA.NextSegment();
								}
								break;
							}
							else
							{
								int ivertex_a = segIterA.GetStartPointIndex();
								int ipath_a = segIterA.GetPathIndex();
								int ivertex_b = segIterB.GetStartPointIndex();
								int ipath_b = segIterB.GetPathIndex();
								overlapEvent = com.epl.geometry.RelationalOperations.OverlapEvent.Construct(ivertex_a, ipath_a, scalar_a_0, scalar_a_1, ivertex_b, ipath_b, scalar_b_0, scalar_b_1);
								relOps.m_overlap_events.Add(overlapEvent);
								eventIndices.Add(eventIndices.Size());
							}
						}
					}
					if (bStringOfSegmentAsCovered)
					{
						continue;
					}
					// no need to check that segmentA is covered
					if (ievent == relOps.m_overlap_events.Count)
					{
						return false;
					}
					// bWithin = false
					if (eventIndices.Size() - ievent > 1)
					{
						eventIndices.Sort(ievent, eventIndices.Size(), overlapComparer);
					}
					double lastScalar = 0.0;
					for (int i = ievent; i < relOps.m_overlap_events.Count; i++)
					{
						overlapEvent = relOps.m_overlap_events[eventIndices.Get(i)];
						if (overlapEvent.m_scalar_a_0 < lastScalar && overlapEvent.m_scalar_a_1 < lastScalar)
						{
							continue;
						}
						if (lengthA * (overlapEvent.m_scalar_a_0 - lastScalar) > tolerance)
						{
							return false;
						}
						else
						{
							// bWithin = false
							lastScalar = overlapEvent.m_scalar_a_1;
							if (lengthA * (1.0 - lastScalar) <= tolerance || lastScalar == 1.0)
							{
								break;
							}
						}
					}
					if (lengthA * (1.0 - lastScalar) > tolerance)
					{
						return false;
					}
					// bWithin = false
					ievent = 0;
					eventIndices.Resize(0);
					relOps.m_overlap_events.Clear();
				}
			}
			return bWithin;
		}

		// Returns true if the segments of multipathA overlap the segments of
		// multipathB.
		private static bool LinearPathOverlapsLinearPath_(com.epl.geometry.MultiPath multipathA, com.epl.geometry.MultiPath multipathB, double tolerance)
		{
			int dim = LinearPathIntersectsLinearPathMaxDim_(multipathA, multipathB, tolerance, null);
			if (dim < 1)
			{
				return false;
			}
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			multipathA.QueryEnvelope2D(env_a);
			multipathB.QueryEnvelope2D(env_b);
			bool bIntAExtB = InteriorEnvExteriorEnv_(env_a, env_b, tolerance);
			bool bIntBExtA = InteriorEnvExteriorEnv_(env_b, env_a, tolerance);
			if (bIntAExtB && bIntBExtA)
			{
				return true;
			}
			if (bIntAExtB && !bIntBExtA)
			{
				return !LinearPathWithinLinearPath_(multipathB, multipathA, tolerance, false);
			}
			if (bIntBExtA && !bIntAExtB)
			{
				return !LinearPathWithinLinearPath_(multipathA, multipathB, tolerance, false);
			}
			return !LinearPathWithinLinearPath_(multipathA, multipathB, tolerance, false) && !LinearPathWithinLinearPath_(multipathB, multipathA, tolerance, false);
		}

		// Returns true the dimension of intersection of _multipathA and
		// _multipathB.
		internal static int LinearPathIntersectsLinearPathMaxDim_(com.epl.geometry.MultiPath _multipathA, com.epl.geometry.MultiPath _multipathB, double tolerance, com.epl.geometry.AttributeStreamOfDbl intersections)
		{
			com.epl.geometry.MultiPath multipathA;
			com.epl.geometry.MultiPath multipathB;
			if (_multipathA.GetSegmentCount() > _multipathB.GetSegmentCount())
			{
				multipathA = _multipathB;
				multipathB = _multipathA;
			}
			else
			{
				multipathA = _multipathA;
				multipathB = _multipathB;
			}
			com.epl.geometry.SegmentIteratorImpl segIterA = ((com.epl.geometry.MultiPathImpl)multipathA._getImpl()).QuerySegmentIterator();
			com.epl.geometry.SegmentIteratorImpl segIterB = ((com.epl.geometry.MultiPathImpl)multipathB._getImpl()).QuerySegmentIterator();
			double[] scalarsA = new double[2];
			double[] scalarsB = new double[2];
			int dim = -1;
			int ievent = 0;
			double overlapLength;
			com.epl.geometry.AttributeStreamOfInt32 eventIndices = new com.epl.geometry.AttributeStreamOfInt32(0);
			com.epl.geometry.RelationalOperations relOps = new com.epl.geometry.RelationalOperations();
			com.epl.geometry.RelationalOperations.OverlapComparer overlapComparer = new com.epl.geometry.RelationalOperations.OverlapComparer(relOps);
			com.epl.geometry.RelationalOperations.OverlapEvent overlapEvent;
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D envInter = new com.epl.geometry.Envelope2D();
			multipathA.QueryEnvelope2D(env_a);
			multipathB.QueryEnvelope2D(env_b);
			env_a.Inflate(tolerance, tolerance);
			env_b.Inflate(tolerance, tolerance);
			envInter.SetCoords(env_a);
			envInter.Intersect(env_b);
			com.epl.geometry.Point2D int_point = null;
			if (intersections != null)
			{
				int_point = new com.epl.geometry.Point2D();
			}
			com.epl.geometry.QuadTreeImpl qtB = null;
			com.epl.geometry.QuadTreeImpl quadTreeB = null;
			com.epl.geometry.QuadTreeImpl quadTreePathsB = null;
			com.epl.geometry.GeometryAccelerators accel = ((com.epl.geometry.MultiPathImpl)multipathB._getImpl())._getAccelerators();
			if (accel != null)
			{
				quadTreeB = accel.GetQuadTree();
				quadTreePathsB = accel.GetQuadTreeForPaths();
				if (quadTreeB == null)
				{
					qtB = com.epl.geometry.InternalUtils.BuildQuadTree((com.epl.geometry.MultiPathImpl)multipathB._getImpl(), envInter);
					quadTreeB = qtB;
				}
			}
			else
			{
				qtB = com.epl.geometry.InternalUtils.BuildQuadTree((com.epl.geometry.MultiPathImpl)multipathB._getImpl(), envInter);
				quadTreeB = qtB;
			}
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl qtIterB = quadTreeB.GetIterator();
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl qtIterPathsB = null;
			if (quadTreePathsB != null)
			{
				qtIterPathsB = quadTreePathsB.GetIterator();
			}
			while (segIterA.NextPath())
			{
				overlapLength = 0.0;
				while (segIterA.HasNextSegment())
				{
					com.epl.geometry.Segment segmentA = segIterA.NextSegment();
					segmentA.QueryEnvelope2D(env_a);
					if (!env_a.IsIntersecting(envInter))
					{
						continue;
					}
					if (qtIterPathsB != null)
					{
						qtIterPathsB.ResetIterator(env_a, tolerance);
						if (qtIterPathsB.Next() == -1)
						{
							continue;
						}
					}
					double lengthA = segmentA.CalculateLength2D();
					qtIterB.ResetIterator(segmentA, tolerance);
					for (int elementHandleB = qtIterB.Next(); elementHandleB != -1; elementHandleB = qtIterB.Next())
					{
						int vertex_b = quadTreeB.GetElement(elementHandleB);
						segIterB.ResetToVertex(vertex_b);
						com.epl.geometry.Segment segmentB = segIterB.NextSegment();
						double lengthB = segmentB.CalculateLength2D();
						int result = segmentA.Intersect(segmentB, null, scalarsA, scalarsB, tolerance);
						if (result > 0)
						{
							double scalar_a_0 = scalarsA[0];
							double scalar_b_0 = scalarsB[0];
							double scalar_a_1 = (result == 2 ? scalarsA[1] : com.epl.geometry.NumberUtils.TheNaN);
							double scalar_b_1 = (result == 2 ? scalarsB[1] : com.epl.geometry.NumberUtils.TheNaN);
							if (result == 2)
							{
								if (lengthA * (scalar_a_1 - scalar_a_0) > tolerance)
								{
									dim = 1;
									return dim;
								}
								// Quick neighbor check
								double length = lengthA * (scalar_a_1 - scalar_a_0);
								if (segIterB.HasNextSegment())
								{
									segmentB = segIterB.NextSegment();
									result = segmentA.Intersect(segmentB, null, scalarsA, null, tolerance);
									if (result == 2)
									{
										double nextScalarA0 = scalarsA[0];
										double nextScalarA1 = scalarsA[1];
										double lengthNext = lengthA * (nextScalarA1 - nextScalarA0);
										if (length + lengthNext > tolerance)
										{
											dim = 1;
											return dim;
										}
									}
									segIterB.ResetToVertex(vertex_b);
									segIterB.NextSegment();
								}
								if (!segIterB.IsFirstSegmentInPath())
								{
									segIterB.PreviousSegment();
									segmentB = segIterB.PreviousSegment();
									result = segmentA.Intersect(segmentB, null, scalarsA, null, tolerance);
									if (result == 2)
									{
										double nextScalarA0 = scalarsA[0];
										double nextScalarA1 = scalarsA[1];
										double lengthPrevious = lengthA * (nextScalarA1 - nextScalarA0);
										if (length + lengthPrevious > tolerance)
										{
											dim = 1;
											return dim;
										}
									}
									segIterB.ResetToVertex(vertex_b);
									segIterB.NextSegment();
								}
								if (segIterA.HasNextSegment())
								{
									int vertex_a = segIterA.GetStartPointIndex();
									segmentA = segIterA.NextSegment();
									result = segmentA.Intersect(segmentB, null, scalarsA, null, tolerance);
									if (result == 2)
									{
										double nextScalarA0 = scalarsA[0];
										double nextScalarA1 = scalarsA[1];
										double lengthNext = lengthA * (nextScalarA1 - nextScalarA0);
										if (length + lengthNext > tolerance)
										{
											dim = 1;
											return dim;
										}
									}
									segIterA.ResetToVertex(vertex_a);
									segIterA.NextSegment();
								}
								if (!segIterA.IsFirstSegmentInPath())
								{
									int vertex_a = segIterA.GetStartPointIndex();
									segIterA.PreviousSegment();
									segmentA = segIterA.PreviousSegment();
									result = segmentA.Intersect(segmentB, null, scalarsA, null, tolerance);
									if (result == 2)
									{
										double nextScalarA0 = scalarsA[0];
										double nextScalarA1 = scalarsA[1];
										double lengthPrevious = lengthB * (nextScalarA1 - nextScalarA0);
										if (length + lengthPrevious > tolerance)
										{
											dim = 1;
											return dim;
										}
									}
									segIterA.ResetToVertex(vertex_a);
									segIterA.NextSegment();
								}
								int ivertex_a = segIterA.GetStartPointIndex();
								int ipath_a = segIterA.GetPathIndex();
								int ivertex_b = segIterB.GetStartPointIndex();
								int ipath_b = segIterB.GetPathIndex();
								overlapEvent = com.epl.geometry.RelationalOperations.OverlapEvent.Construct(ivertex_a, ipath_a, scalar_a_0, scalar_a_1, ivertex_b, ipath_b, scalar_b_0, scalar_b_1);
								relOps.m_overlap_events.Add(overlapEvent);
								eventIndices.Add(eventIndices.Size());
							}
							dim = 0;
							if (intersections != null)
							{
								segmentA.GetCoord2D(scalar_a_0, int_point);
								intersections.Add(int_point.x);
								intersections.Add(int_point.y);
							}
						}
					}
					if (ievent < relOps.m_overlap_events.Count)
					{
						eventIndices.Sort(ievent, eventIndices.Size(), overlapComparer);
						double lastScalar = 0.0;
						int lastPath = relOps.m_overlap_events[eventIndices.Get(ievent)].m_ipath_a;
						for (int i = ievent; i < relOps.m_overlap_events.Count; i++)
						{
							overlapEvent = relOps.m_overlap_events[eventIndices.Get(i)];
							if (overlapEvent.m_scalar_a_0 < lastScalar && overlapEvent.m_scalar_a_1 < lastScalar)
							{
								continue;
							}
							if (lengthA * (overlapEvent.m_scalar_a_0 - lastScalar) > tolerance)
							{
								overlapLength = lengthA * (overlapEvent.m_scalar_a_1 - overlapEvent.m_scalar_a_0);
								// reset
								lastScalar = overlapEvent.m_scalar_a_1;
								lastPath = overlapEvent.m_ipath_a;
							}
							else
							{
								if (overlapEvent.m_ipath_a != lastPath)
								{
									overlapLength = lengthA * (overlapEvent.m_scalar_a_1 - overlapEvent.m_scalar_a_0);
									// reset
									lastPath = overlapEvent.m_ipath_a;
								}
								else
								{
									overlapLength += lengthA * (overlapEvent.m_scalar_a_1 - overlapEvent.m_scalar_a_0);
								}
								// accumulate
								if (overlapLength > tolerance)
								{
									dim = 1;
									return dim;
								}
								lastScalar = overlapEvent.m_scalar_a_1;
								if (lastScalar == 1.0)
								{
									break;
								}
							}
						}
						if (lengthA * (1.0 - lastScalar) > tolerance)
						{
							overlapLength = 0.0;
						}
						// reset
						ievent = 0;
						eventIndices.Resize(0);
						relOps.m_overlap_events.Clear();
					}
				}
			}
			return dim;
		}

		// Returns true if the line segments of _multipathA intersect the line
		// segments of _multipathB.
		private static bool LinearPathIntersectsLinearPath_(com.epl.geometry.MultiPath multipathA, com.epl.geometry.MultiPath multipathB, double tolerance)
		{
			com.epl.geometry.MultiPathImpl multi_path_impl_a = (com.epl.geometry.MultiPathImpl)multipathA._getImpl();
			com.epl.geometry.MultiPathImpl multi_path_impl_b = (com.epl.geometry.MultiPathImpl)multipathB._getImpl();
			com.epl.geometry.SegmentIteratorImpl segIterA = multi_path_impl_a.QuerySegmentIterator();
			com.epl.geometry.SegmentIteratorImpl segIterB = multi_path_impl_b.QuerySegmentIterator();
			com.epl.geometry.PairwiseIntersectorImpl intersector = new com.epl.geometry.PairwiseIntersectorImpl(multi_path_impl_a, multi_path_impl_b, tolerance, false);
			while (intersector.Next())
			{
				int vertex_a = intersector.GetRedElement();
				int vertex_b = intersector.GetBlueElement();
				segIterA.ResetToVertex(vertex_a);
				segIterB.ResetToVertex(vertex_b);
				com.epl.geometry.Segment segmentA = segIterA.NextSegment();
				com.epl.geometry.Segment segmentB = segIterB.NextSegment();
				int result = segmentB.Intersect(segmentA, null, null, null, tolerance);
				if (result > 0)
				{
					return true;
				}
			}
			return false;
		}

		// Returns true if the relation intersects, crosses, or contains holds
		// between multipathA and multipoint_b. multipathA is put in the
		// Quad_tree_impl.
		private static bool LinearPathIntersectsMultiPoint_(com.epl.geometry.MultiPath multipathA, com.epl.geometry.MultiPoint multipoint_b, double tolerance, bool b_intersects_all)
		{
			com.epl.geometry.SegmentIteratorImpl segIterA = ((com.epl.geometry.MultiPathImpl)multipathA._getImpl()).QuerySegmentIterator();
			bool bContained = true;
			bool bInteriorHitFound = false;
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D envInter = new com.epl.geometry.Envelope2D();
			multipathA.QueryEnvelope2D(env_a);
			multipoint_b.QueryEnvelope2D(env_b);
			env_a.Inflate(tolerance, tolerance);
			if (!env_a.Contains(env_b))
			{
				bContained = false;
			}
			env_b.Inflate(tolerance, tolerance);
			envInter.SetCoords(env_a);
			envInter.Intersect(env_b);
			com.epl.geometry.QuadTreeImpl qtA = null;
			com.epl.geometry.QuadTreeImpl quadTreeA = null;
			com.epl.geometry.QuadTreeImpl quadTreePathsA = null;
			com.epl.geometry.GeometryAccelerators accel = ((com.epl.geometry.MultiPathImpl)multipathA._getImpl())._getAccelerators();
			if (accel != null)
			{
				quadTreeA = accel.GetQuadTree();
				if (quadTreeA == null)
				{
					qtA = com.epl.geometry.InternalUtils.BuildQuadTree((com.epl.geometry.MultiPathImpl)multipathA._getImpl(), envInter);
					quadTreeA = qtA;
				}
			}
			else
			{
				qtA = com.epl.geometry.InternalUtils.BuildQuadTree((com.epl.geometry.MultiPathImpl)multipathA._getImpl(), envInter);
				quadTreeA = qtA;
			}
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl qtIterA = quadTreeA.GetIterator();
			com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl qtIterPathsA = null;
			if (quadTreePathsA != null)
			{
				qtIterPathsA = quadTreePathsA.GetIterator();
			}
			com.epl.geometry.Point2D ptB = new com.epl.geometry.Point2D();
			com.epl.geometry.Point2D closest = new com.epl.geometry.Point2D();
			bool b_intersects = false;
			double toleranceSq = tolerance * tolerance;
			for (int i = 0; i < multipoint_b.GetPointCount(); i++)
			{
				multipoint_b.GetXY(i, ptB);
				if (!envInter.Contains(ptB))
				{
					continue;
				}
				env_b.SetCoords(ptB.x, ptB.y, ptB.x, ptB.y);
				if (qtIterPathsA != null)
				{
					qtIterPathsA.ResetIterator(env_b, tolerance);
					if (qtIterPathsA.Next() == -1)
					{
						continue;
					}
				}
				qtIterA.ResetIterator(env_b, tolerance);
				bool b_covered = false;
				for (int elementHandleA = qtIterA.Next(); elementHandleA != -1; elementHandleA = qtIterA.Next())
				{
					int vertex_a = quadTreeA.GetElement(elementHandleA);
					segIterA.ResetToVertex(vertex_a);
					com.epl.geometry.Segment segmentA = segIterA.NextSegment();
					double t = segmentA.GetClosestCoordinate(ptB, false);
					segmentA.GetCoord2D(t, closest);
					if (com.epl.geometry.Point2D.SqrDistance(closest, ptB) <= toleranceSq)
					{
						b_covered = true;
						break;
					}
				}
				if (b_intersects_all)
				{
					if (!b_covered)
					{
						return false;
					}
				}
				else
				{
					if (b_covered)
					{
						return true;
					}
				}
			}
			if (b_intersects_all)
			{
				return true;
			}
			return false;
		}

		// Returns true if a segment of multipathA intersects point_b.
		internal static bool LinearPathIntersectsPoint_(com.epl.geometry.MultiPath multipathA, com.epl.geometry.Point2D ptB, double tolerance)
		{
			com.epl.geometry.Point2D closest = new com.epl.geometry.Point2D();
			double toleranceSq = tolerance * tolerance;
			com.epl.geometry.SegmentIteratorImpl segIterA = ((com.epl.geometry.MultiPathImpl)multipathA._getImpl()).QuerySegmentIterator();
			com.epl.geometry.GeometryAccelerators accel = ((com.epl.geometry.MultiPathImpl)multipathA._getImpl())._getAccelerators();
			if (accel != null)
			{
				com.epl.geometry.QuadTreeImpl quadTreeA = accel.GetQuadTree();
				if (quadTreeA != null)
				{
					com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
					env_b.SetCoords(ptB);
					com.epl.geometry.QuadTreeImpl.QuadTreeIteratorImpl qt_iter = quadTreeA.GetIterator(env_b, tolerance);
					for (int e = qt_iter.Next(); e != -1; e = qt_iter.Next())
					{
						segIterA.ResetToVertex(quadTreeA.GetElement(e));
						if (segIterA.HasNextSegment())
						{
							com.epl.geometry.Segment segmentA = segIterA.NextSegment();
							double t = segmentA.GetClosestCoordinate(ptB, false);
							segmentA.GetCoord2D(t, closest);
							if (com.epl.geometry.Point2D.SqrDistance(ptB, closest) <= toleranceSq)
							{
								return true;
							}
						}
					}
					return false;
				}
			}
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			while (segIterA.NextPath())
			{
				while (segIterA.HasNextSegment())
				{
					com.epl.geometry.Segment segmentA = segIterA.NextSegment();
					segmentA.QueryEnvelope2D(env_a);
					env_a.Inflate(tolerance, tolerance);
					if (!env_a.Contains(ptB))
					{
						continue;
					}
					double t = segmentA.GetClosestCoordinate(ptB, false);
					segmentA.GetCoord2D(t, closest);
					if (com.epl.geometry.Point2D.SqrDistance(ptB, closest) <= toleranceSq)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool LinearPathContainsPoint_(com.epl.geometry.MultiPath multipathA, com.epl.geometry.Point2D pt_b, double tolerance)
		{
			return LinearPathIntersectsPoint_(multipathA, pt_b, tolerance) && !LinearPathTouchesPointImpl_(multipathA, pt_b, tolerance);
		}

		private static bool LinearPathTouchesPointImpl_(com.epl.geometry.MultiPath multipathA, com.epl.geometry.Point2D ptB, double tolerance)
		{
			com.epl.geometry.MultiPoint boundary = (com.epl.geometry.MultiPoint)(multipathA.GetBoundary());
			return !MultiPointDisjointPointImpl_(boundary, ptB, tolerance, null);
		}

		// Returns true if the segments of multipathA intersects env_b
		private static bool LinearPathIntersectsEnvelope_(com.epl.geometry.MultiPath multipath_a, com.epl.geometry.Envelope2D env_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			if (!multipath_a.HasNonLinearSegments())
			{
				com.epl.geometry.Envelope2D env_b_inflated = new com.epl.geometry.Envelope2D();
				env_b_inflated.SetCoords(env_b);
				env_b_inflated.Inflate(tolerance, tolerance);
				com.epl.geometry.MultiPathImpl mimpl_a = (com.epl.geometry.MultiPathImpl)multipath_a._getImpl();
				com.epl.geometry.AttributeStreamOfDbl xy = (com.epl.geometry.AttributeStreamOfDbl)(mimpl_a.GetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION));
				com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
				com.epl.geometry.Point2D pt_prev = new com.epl.geometry.Point2D();
				com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
				com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();
				for (int ipath = 0, npath = mimpl_a.GetPathCount(); ipath < npath; ipath++)
				{
					bool b_first = true;
					for (int i = mimpl_a.GetPathStart(ipath), n = mimpl_a.GetPathEnd(ipath); i < n; i++)
					{
						if (b_first)
						{
							xy.Read(2 * i, pt_prev);
							b_first = false;
							continue;
						}
						xy.Read(2 * i, pt);
						pt_1.SetCoords(pt_prev);
						pt_2.SetCoords(pt);
						if (env_b_inflated.ClipLine(pt_1, pt_2) != 0)
						{
							return true;
						}
						pt_prev.SetCoords(pt);
					}
				}
			}
			else
			{
				com.epl.geometry.Line line_1 = new com.epl.geometry.Line(env_b.xmin, env_b.ymin, env_b.xmin, env_b.ymax);
				com.epl.geometry.Line line_2 = new com.epl.geometry.Line(env_b.xmin, env_b.ymax, env_b.xmax, env_b.ymax);
				com.epl.geometry.Line line3 = new com.epl.geometry.Line(env_b.xmax, env_b.ymax, env_b.xmax, env_b.ymin);
				com.epl.geometry.Line line4 = new com.epl.geometry.Line(env_b.xmax, env_b.ymin, env_b.xmin, env_b.ymin);
				com.epl.geometry.SegmentIterator iter = multipath_a.QuerySegmentIterator();
				while (iter.NextPath())
				{
					while (iter.HasNextSegment())
					{
						com.epl.geometry.Segment polySeg = iter.NextSegment();
						if (polySeg.IsIntersecting(line_1, tolerance))
						{
							return true;
						}
						if (polySeg.IsIntersecting(line_2, tolerance))
						{
							return true;
						}
						if (polySeg.IsIntersecting(line3, tolerance))
						{
							return true;
						}
						if (polySeg.IsIntersecting(line4, tolerance))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Returns contains, disjoint, or within if the relationship can be
		// determined from the rasterized tests.
		// When bExtraTestForIntersects is true performs extra tests and can return
		// "intersects".
		internal static int TryRasterizedContainsOrDisjoint_(com.epl.geometry.Geometry geom_a, com.epl.geometry.Geometry geom_b, double tolerance, bool bExtraTestForIntersects)
		{
			int gtA = geom_a.GetType().Value();
			int gtB = geom_b.GetType().Value();
			do
			{
				if (com.epl.geometry.Geometry.IsMultiVertex(gtA))
				{
					com.epl.geometry.MultiVertexGeometryImpl impl = (com.epl.geometry.MultiVertexGeometryImpl)geom_a._getImpl();
					com.epl.geometry.GeometryAccelerators accel = impl._getAccelerators();
					if (accel != null)
					{
						com.epl.geometry.RasterizedGeometry2D rgeom = accel.GetRasterizedGeometry();
						if (rgeom != null)
						{
							if (gtB == com.epl.geometry.Geometry.GeometryType.Point)
							{
								com.epl.geometry.Point2D ptB = ((com.epl.geometry.Point)geom_b).GetXY();
								com.epl.geometry.RasterizedGeometry2D.HitType hit = rgeom.QueryPointInGeometry(ptB.x, ptB.y);
								if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Inside)
								{
									return com.epl.geometry.RelationalOperations.Relation.contains;
								}
								else
								{
									if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Outside)
									{
										return com.epl.geometry.RelationalOperations.Relation.disjoint;
									}
								}
								break;
							}
							com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
							geom_b.QueryEnvelope2D(env_b);
							com.epl.geometry.RasterizedGeometry2D.HitType hit_1 = rgeom.QueryEnvelopeInGeometry(env_b);
							if (hit_1 == com.epl.geometry.RasterizedGeometry2D.HitType.Inside)
							{
								return com.epl.geometry.RelationalOperations.Relation.contains;
							}
							else
							{
								if (hit_1 == com.epl.geometry.RasterizedGeometry2D.HitType.Outside)
								{
									return com.epl.geometry.RelationalOperations.Relation.disjoint;
								}
								else
								{
									if (bExtraTestForIntersects && com.epl.geometry.Geometry.IsMultiVertex(gtB))
									{
										if (CheckVerticesForIntersection_((com.epl.geometry.MultiVertexGeometryImpl)geom_b._getImpl(), rgeom))
										{
											return com.epl.geometry.RelationalOperations.Relation.intersects;
										}
									}
								}
							}
							break;
						}
					}
				}
			}
			while (false);
			do
			{
				if (com.epl.geometry.Geometry.IsMultiVertex(gtB))
				{
					com.epl.geometry.MultiVertexGeometryImpl impl = (com.epl.geometry.MultiVertexGeometryImpl)geom_b._getImpl();
					com.epl.geometry.GeometryAccelerators accel = impl._getAccelerators();
					if (accel != null)
					{
						com.epl.geometry.RasterizedGeometry2D rgeom = accel.GetRasterizedGeometry();
						if (rgeom != null)
						{
							if (gtA == com.epl.geometry.Geometry.GeometryType.Point)
							{
								com.epl.geometry.Point2D ptA = ((com.epl.geometry.Point)geom_a).GetXY();
								com.epl.geometry.RasterizedGeometry2D.HitType hit = rgeom.QueryPointInGeometry(ptA.x, ptA.y);
								if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Inside)
								{
									return com.epl.geometry.RelationalOperations.Relation.within;
								}
								else
								{
									if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Outside)
									{
										return com.epl.geometry.RelationalOperations.Relation.disjoint;
									}
								}
								break;
							}
							com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
							geom_a.QueryEnvelope2D(env_a);
							com.epl.geometry.RasterizedGeometry2D.HitType hit_1 = rgeom.QueryEnvelopeInGeometry(env_a);
							if (hit_1 == com.epl.geometry.RasterizedGeometry2D.HitType.Inside)
							{
								return com.epl.geometry.RelationalOperations.Relation.within;
							}
							else
							{
								if (hit_1 == com.epl.geometry.RasterizedGeometry2D.HitType.Outside)
								{
									return com.epl.geometry.RelationalOperations.Relation.disjoint;
								}
								else
								{
									if (bExtraTestForIntersects && com.epl.geometry.Geometry.IsMultiVertex(gtA))
									{
										if (CheckVerticesForIntersection_((com.epl.geometry.MultiVertexGeometryImpl)geom_a._getImpl(), rgeom))
										{
											return com.epl.geometry.RelationalOperations.Relation.intersects;
										}
									}
								}
							}
							break;
						}
					}
				}
			}
			while (false);
			return com.epl.geometry.RelationalOperations.Relation.unknown;
		}

		// Returns true if intersects and false if nothing can be determined.
		private static bool CheckVerticesForIntersection_(com.epl.geometry.MultiVertexGeometryImpl geom, com.epl.geometry.RasterizedGeometry2D rgeom)
		{
			// Do a quick raster test for each point. If any point is inside, then
			// there is an intersection.
			int pointCount = geom.GetPointCount();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			for (int ipoint = 0; ipoint < pointCount; ipoint++)
			{
				geom.GetXY(ipoint, pt);
				com.epl.geometry.RasterizedGeometry2D.HitType hit = rgeom.QueryPointInGeometry(pt.x, pt.y);
				if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Inside)
				{
					return true;
				}
			}
			return false;
		}

		private static bool PolygonTouchesPolygonImpl_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polygon polygon_b, double tolerance, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.MultiPathImpl polygon_impl_a = (com.epl.geometry.MultiPathImpl)polygon_a._getImpl();
			com.epl.geometry.MultiPathImpl polygon_impl_b = (com.epl.geometry.MultiPathImpl)polygon_b._getImpl();
			// double geom_tolerance;
			bool b_geometries_simple = polygon_impl_a.GetIsSimple(0.0) >= 1 && polygon_impl_b.GetIsSimple(0.0) >= 1;
			com.epl.geometry.SegmentIteratorImpl segIterA = polygon_impl_a.QuerySegmentIterator();
			com.epl.geometry.SegmentIteratorImpl segIterB = polygon_impl_b.QuerySegmentIterator();
			double[] scalarsA = new double[2];
			double[] scalarsB = new double[2];
			com.epl.geometry.PairwiseIntersectorImpl intersector = new com.epl.geometry.PairwiseIntersectorImpl(polygon_impl_a, polygon_impl_b, tolerance, false);
			bool b_boundaries_intersect = false;
			while (intersector.Next())
			{
				int vertex_a = intersector.GetRedElement();
				int vertex_b = intersector.GetBlueElement();
				segIterA.ResetToVertex(vertex_a);
				segIterB.ResetToVertex(vertex_b);
				com.epl.geometry.Segment segmentA = segIterA.NextSegment();
				com.epl.geometry.Segment segmentB = segIterB.NextSegment();
				int result = segmentB.Intersect(segmentA, null, scalarsB, scalarsA, tolerance);
				if (result == 2)
				{
					double scalar_a_0 = scalarsA[0];
					double scalar_a_1 = scalarsA[1];
					double length_a = segmentA.CalculateLength2D();
					if (b_geometries_simple && (scalar_a_1 - scalar_a_0) * length_a > tolerance)
					{
						// If the line segments overlap along the same direction,
						// then we have an Interior-Interior intersection
						return false;
					}
					b_boundaries_intersect = true;
				}
				else
				{
					if (result != 0)
					{
						double scalar_a_0 = scalarsA[0];
						double scalar_b_0 = scalarsB[0];
						if (scalar_a_0 > 0.0 && scalar_a_0 < 1.0 && scalar_b_0 > 0.0 && scalar_b_0 < 1.0)
						{
							return false;
						}
						b_boundaries_intersect = true;
					}
				}
			}
			if (!b_boundaries_intersect)
			{
				return false;
			}
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D envInter = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			polygon_b.QueryEnvelope2D(env_b);
			env_a.Inflate(1000.0 * tolerance, 1000.0 * tolerance);
			env_b.Inflate(1000.0 * tolerance, 1000.0 * tolerance);
			envInter.SetCoords(env_a);
			envInter.Intersect(env_b);
			com.epl.geometry.Polygon _polygonA;
			com.epl.geometry.Polygon _polygonB;
			if (polygon_a.GetPointCount() > 10)
			{
				_polygonA = (com.epl.geometry.Polygon)(com.epl.geometry.Clipper.Clip(polygon_a, envInter, tolerance, 0.0));
				if (_polygonA.IsEmpty())
				{
					return false;
				}
			}
			else
			{
				_polygonA = polygon_a;
			}
			if (polygon_b.GetPointCount() > 10)
			{
				_polygonB = (com.epl.geometry.Polygon)(com.epl.geometry.Clipper.Clip(polygon_b, envInter, tolerance, 0.0));
				if (_polygonB.IsEmpty())
				{
					return false;
				}
			}
			else
			{
				_polygonB = polygon_b;
			}
			// We just need to determine whether interior_interior is false
			string scl = "F********";
			bool bRelation = com.epl.geometry.RelationalOperationsMatrix.PolygonRelatePolygon_(_polygonA, _polygonB, tolerance, scl, progressTracker);
			return bRelation;
		}

		private static bool PolygonOverlapsPolygonImpl_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polygon polygon_b, double tolerance, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.MultiPathImpl polygon_impl_a = (com.epl.geometry.MultiPathImpl)polygon_a._getImpl();
			com.epl.geometry.MultiPathImpl polygon_impl_b = (com.epl.geometry.MultiPathImpl)polygon_b._getImpl();
			// double geom_tolerance;
			bool b_geometries_simple = polygon_impl_a.GetIsSimple(0.0) >= 1 && polygon_impl_b.GetIsSimple(0.0) >= 1;
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D envInter = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			polygon_b.QueryEnvelope2D(env_b);
			bool bInteriorIntersectionKnown = false;
			bool bIntAExtB = InteriorEnvExteriorEnv_(env_a, env_b, tolerance);
			bool bExtAIntB = InteriorEnvExteriorEnv_(env_b, env_a, tolerance);
			com.epl.geometry.SegmentIteratorImpl segIterA = polygon_impl_a.QuerySegmentIterator();
			com.epl.geometry.SegmentIteratorImpl segIterB = polygon_impl_b.QuerySegmentIterator();
			double[] scalarsA = new double[2];
			double[] scalarsB = new double[2];
			com.epl.geometry.PairwiseIntersectorImpl intersector = new com.epl.geometry.PairwiseIntersectorImpl(polygon_impl_a, polygon_impl_b, tolerance, false);
			while (intersector.Next())
			{
				int vertex_a = intersector.GetRedElement();
				int vertex_b = intersector.GetBlueElement();
				segIterA.ResetToVertex(vertex_a);
				segIterB.ResetToVertex(vertex_b);
				com.epl.geometry.Segment segmentA = segIterA.NextSegment();
				com.epl.geometry.Segment segmentB = segIterB.NextSegment();
				int result = segmentB.Intersect(segmentA, null, scalarsB, scalarsA, tolerance);
				if (result == 2)
				{
					double scalar_a_0 = scalarsA[0];
					double scalar_a_1 = scalarsA[1];
					double length_a = segmentA.CalculateLength2D();
					if (b_geometries_simple && (scalar_a_1 - scalar_a_0) * length_a > tolerance)
					{
						// When the line segments intersect along the same
						// direction, then we have an interior-interior intersection
						bInteriorIntersectionKnown = true;
						if (bIntAExtB && bExtAIntB)
						{
							return true;
						}
					}
				}
				else
				{
					if (result != 0)
					{
						double scalar_a_0 = scalarsA[0];
						double scalar_b_0 = scalarsB[0];
						if (scalar_a_0 > 0.0 && scalar_a_0 < 1.0 && scalar_b_0 > 0.0 && scalar_b_0 < 1.0)
						{
							return true;
						}
					}
				}
			}
			com.epl.geometry.Envelope2D envAInflated = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D envBInflated = new com.epl.geometry.Envelope2D();
			envAInflated.SetCoords(env_a);
			envAInflated.Inflate(1000.0 * tolerance, 1000.0 * tolerance);
			envBInflated.SetCoords(env_b);
			envBInflated.Inflate(1000.0 * tolerance, 1000.0 * tolerance);
			envInter.SetCoords(envAInflated);
			envInter.Intersect(envBInflated);
			com.epl.geometry.Polygon _polygonA;
			com.epl.geometry.Polygon _polygonB;
			System.Text.StringBuilder scl = new System.Text.StringBuilder();
			if (!bInteriorIntersectionKnown)
			{
				scl.Append("T*");
			}
			else
			{
				scl.Append("**");
			}
			if (bIntAExtB)
			{
				if (polygon_b.GetPointCount() > 10)
				{
					_polygonB = (com.epl.geometry.Polygon)(com.epl.geometry.Clipper.Clip(polygon_b, envInter, tolerance, 0.0));
					if (_polygonB.IsEmpty())
					{
						return false;
					}
				}
				else
				{
					_polygonB = polygon_b;
				}
				scl.Append("****");
			}
			else
			{
				_polygonB = polygon_b;
				scl.Append("T***");
			}
			if (bExtAIntB)
			{
				if (polygon_a.GetPointCount() > 10)
				{
					_polygonA = (com.epl.geometry.Polygon)(com.epl.geometry.Clipper.Clip(polygon_a, envInter, tolerance, 0.0));
					if (_polygonA.IsEmpty())
					{
						return false;
					}
				}
				else
				{
					_polygonA = polygon_a;
				}
				scl.Append("***");
			}
			else
			{
				_polygonA = polygon_a;
				scl.Append("T**");
			}
			bool bRelation = com.epl.geometry.RelationalOperationsMatrix.PolygonRelatePolygon_(_polygonA, _polygonB, tolerance, scl.ToString(), progressTracker);
			return bRelation;
		}

		private static bool PolygonContainsPolygonImpl_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polygon polygon_b, double tolerance, com.epl.geometry.ProgressTracker progressTracker)
		{
			bool[] b_result_known = new bool[1];
			b_result_known[0] = false;
			bool res = PolygonContainsMultiPath_(polygon_a, polygon_b, tolerance, b_result_known, progressTracker);
			if (b_result_known[0])
			{
				return res;
			}
			// We can clip polygon_a to the extent of polyline_b
			com.epl.geometry.Envelope2D envBInflated = new com.epl.geometry.Envelope2D();
			polygon_b.QueryEnvelope2D(envBInflated);
			envBInflated.Inflate(1000.0 * tolerance, 1000.0 * tolerance);
			com.epl.geometry.Polygon _polygonA = null;
			if (polygon_a.GetPointCount() > 10)
			{
				_polygonA = (com.epl.geometry.Polygon)com.epl.geometry.Clipper.Clip(polygon_a, envBInflated, tolerance, 0.0);
				if (_polygonA.IsEmpty())
				{
					return false;
				}
			}
			else
			{
				_polygonA = polygon_a;
			}
			bool bContains = com.epl.geometry.RelationalOperationsMatrix.PolygonContainsPolygon_(_polygonA, polygon_b, tolerance, progressTracker);
			return bContains;
		}

		private static bool PolygonContainsMultiPath_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.MultiPath multi_path_b, double tolerance, bool[] b_result_known, com.epl.geometry.ProgressTracker progress_tracker)
		{
			b_result_known[0] = false;
			com.epl.geometry.MultiPathImpl polygon_impl_a = (com.epl.geometry.MultiPathImpl)polygon_a._getImpl();
			com.epl.geometry.MultiPathImpl multi_path_impl_b = (com.epl.geometry.MultiPathImpl)multi_path_b._getImpl();
			com.epl.geometry.SegmentIteratorImpl segIterA = polygon_impl_a.QuerySegmentIterator();
			com.epl.geometry.SegmentIteratorImpl segIterB = multi_path_impl_b.QuerySegmentIterator();
			double[] scalarsA = new double[2];
			double[] scalarsB = new double[2];
			com.epl.geometry.PairwiseIntersectorImpl intersector = new com.epl.geometry.PairwiseIntersectorImpl(polygon_impl_a, multi_path_impl_b, tolerance, false);
			bool b_boundaries_intersect = false;
			while (intersector.Next())
			{
				int vertex_a = intersector.GetRedElement();
				int vertex_b = intersector.GetBlueElement();
				segIterA.ResetToVertex(vertex_a, -1);
				segIterB.ResetToVertex(vertex_b, -1);
				com.epl.geometry.Segment segmentA = segIterA.NextSegment();
				com.epl.geometry.Segment segmentB = segIterB.NextSegment();
				int result = segmentB.Intersect(segmentA, null, scalarsB, scalarsA, tolerance);
				if (result != 0)
				{
					b_boundaries_intersect = true;
					if (result == 1)
					{
						double scalar_a_0 = scalarsA[0];
						double scalar_b_0 = scalarsB[0];
						if (scalar_a_0 > 0.0 && scalar_a_0 < 1.0 && scalar_b_0 > 0.0 && scalar_b_0 < 1.0)
						{
							b_result_known[0] = true;
							return false;
						}
					}
				}
			}
			if (!b_boundaries_intersect)
			{
				b_result_known[0] = true;
				//boundaries do not intersect
				com.epl.geometry.Envelope2D env_a_inflated = new com.epl.geometry.Envelope2D();
				polygon_a.QueryEnvelope2D(env_a_inflated);
				env_a_inflated.Inflate(tolerance, tolerance);
				com.epl.geometry.Polygon pa = null;
				com.epl.geometry.Polygon p_polygon_a = polygon_a;
				bool b_checked_polygon_a_quad_tree = false;
				com.epl.geometry.Envelope2D path_env_b = new com.epl.geometry.Envelope2D();
				for (int ipath = 0, npath = multi_path_b.GetPathCount(); ipath < npath; ipath++)
				{
					if (multi_path_b.GetPathSize(ipath) > 0)
					{
						multi_path_b.QueryPathEnvelope2D(ipath, path_env_b);
						if (env_a_inflated.IsIntersecting(path_env_b))
						{
							com.epl.geometry.Point2D anyPoint = multi_path_b.GetXY(multi_path_b.GetPathStart(ipath));
							int res = com.epl.geometry.PointInPolygonHelper.IsPointInPolygon(p_polygon_a, anyPoint, 0);
							if (res == 0)
							{
								return false;
							}
						}
						else
						{
							return false;
						}
						if (!b_checked_polygon_a_quad_tree)
						{
							if (com.epl.geometry.PointInPolygonHelper.QuadTreeWillHelp(polygon_a, multi_path_b.GetPathCount() - 1) && (polygon_impl_a._getAccelerators() == null || polygon_impl_a._getAccelerators().GetQuadTree() == null))
							{
								pa = new com.epl.geometry.Polygon();
								polygon_a.CopyTo(pa);
								((com.epl.geometry.MultiPathImpl)pa._getImpl())._buildQuadTreeAccelerator(com.epl.geometry.Geometry.GeometryAccelerationDegree.enumMedium);
								p_polygon_a = pa;
							}
							else
							{
								p_polygon_a = polygon_a;
							}
							b_checked_polygon_a_quad_tree = true;
						}
					}
				}
				if (polygon_a.GetPathCount() == 1 || multi_path_b.GetType().Value() == com.epl.geometry.Geometry.GeometryType.Polyline)
				{
					return true;
				}
				//boundaries do not intersect. all paths of b are inside of a
				// Polygon A has multiple rings, and Multi_path B is a polygon.
				com.epl.geometry.Polygon polygon_b = (com.epl.geometry.Polygon)multi_path_b;
				com.epl.geometry.Envelope2D env_b_inflated = new com.epl.geometry.Envelope2D();
				polygon_b.QueryEnvelope2D(env_b_inflated);
				env_b_inflated.Inflate(tolerance, tolerance);
				com.epl.geometry.Polygon pb = null;
				com.epl.geometry.Polygon p_polygon_b = polygon_b;
				bool b_checked_polygon_b_quad_tree = false;
				com.epl.geometry.Envelope2D path_env_a = new com.epl.geometry.Envelope2D();
				for (int ipath_1 = 0, npath = polygon_a.GetPathCount(); ipath_1 < npath; ipath_1++)
				{
					if (polygon_a.GetPathSize(ipath_1) > 0)
					{
						polygon_a.QueryPathEnvelope2D(ipath_1, path_env_a);
						if (env_b_inflated.IsIntersecting(path_env_a))
						{
							com.epl.geometry.Point2D anyPoint = polygon_a.GetXY(polygon_a.GetPathStart(ipath_1));
							int res = com.epl.geometry.PointInPolygonHelper.IsPointInPolygon(p_polygon_b, anyPoint, 0);
							if (res == 1)
							{
								return false;
							}
						}
						if (!b_checked_polygon_b_quad_tree)
						{
							if (com.epl.geometry.PointInPolygonHelper.QuadTreeWillHelp(polygon_b, polygon_a.GetPathCount() - 1) && (multi_path_impl_b._getAccelerators() == null || multi_path_impl_b._getAccelerators().GetQuadTree() == null))
							{
								pb = new com.epl.geometry.Polygon();
								polygon_b.CopyTo(pb);
								((com.epl.geometry.MultiPathImpl)pb._getImpl())._buildQuadTreeAccelerator(com.epl.geometry.Geometry.GeometryAccelerationDegree.enumMedium);
								p_polygon_b = pb;
							}
							else
							{
								p_polygon_b = polygon_b;
							}
							b_checked_polygon_b_quad_tree = true;
						}
					}
				}
				return true;
			}
			return false;
		}

		private static bool PolygonTouchesPolylineImpl_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polyline polyline_b, double tolerance, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.MultiPathImpl polygon_impl_a = (com.epl.geometry.MultiPathImpl)polygon_a._getImpl();
			com.epl.geometry.MultiPathImpl polyline_impl_b = (com.epl.geometry.MultiPathImpl)polyline_b._getImpl();
			com.epl.geometry.SegmentIteratorImpl segIterA = polygon_impl_a.QuerySegmentIterator();
			com.epl.geometry.SegmentIteratorImpl segIterB = polyline_impl_b.QuerySegmentIterator();
			double[] scalarsA = new double[2];
			double[] scalarsB = new double[2];
			com.epl.geometry.PairwiseIntersectorImpl intersector = new com.epl.geometry.PairwiseIntersectorImpl(polygon_impl_a, polyline_impl_b, tolerance, false);
			bool b_boundaries_intersect = false;
			while (intersector.Next())
			{
				int vertex_a = intersector.GetRedElement();
				int vertex_b = intersector.GetBlueElement();
				segIterA.ResetToVertex(vertex_a);
				segIterB.ResetToVertex(vertex_b);
				com.epl.geometry.Segment segmentA = segIterA.NextSegment();
				com.epl.geometry.Segment segmentB = segIterB.NextSegment();
				int result = segmentB.Intersect(segmentA, null, scalarsB, scalarsA, tolerance);
				if (result == 2)
				{
					b_boundaries_intersect = true;
				}
				else
				{
					if (result != 0)
					{
						double scalar_a_0 = scalarsA[0];
						double scalar_b_0 = scalarsB[0];
						if (scalar_a_0 > 0.0 && scalar_a_0 < 1.0 && scalar_b_0 > 0.0 && scalar_b_0 < 1.0)
						{
							return false;
						}
						b_boundaries_intersect = true;
					}
				}
			}
			if (!b_boundaries_intersect)
			{
				return false;
			}
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D envInter = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			polyline_b.QueryEnvelope2D(env_b);
			env_a.Inflate(1000.0 * tolerance, 1000.0 * tolerance);
			env_b.Inflate(1000.0 * tolerance, 1000.0 * tolerance);
			envInter.SetCoords(env_a);
			envInter.Intersect(env_b);
			com.epl.geometry.Polygon _polygonA;
			com.epl.geometry.Polyline _polylineB;
			if (polygon_a.GetPointCount() > 10)
			{
				_polygonA = (com.epl.geometry.Polygon)(com.epl.geometry.Clipper.Clip(polygon_a, envInter, tolerance, 0.0));
				if (_polygonA.IsEmpty())
				{
					return false;
				}
			}
			else
			{
				_polygonA = polygon_a;
			}
			if (polyline_b.GetPointCount() > 10)
			{
				_polylineB = (com.epl.geometry.Polyline)com.epl.geometry.Clipper.Clip(polyline_b, envInter, tolerance, 0.0);
				if (_polylineB.IsEmpty())
				{
					return false;
				}
			}
			else
			{
				_polylineB = polyline_b;
			}
			// We just need to determine that interior_interior is false
			string scl = "F********";
			bool bRelation = com.epl.geometry.RelationalOperationsMatrix.PolygonRelatePolyline_(_polygonA, _polylineB, tolerance, scl, progressTracker);
			return bRelation;
		}

		private static bool PolygonCrossesPolylineImpl_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polyline polyline_b, double tolerance, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.MultiPathImpl polygon_impl_a = (com.epl.geometry.MultiPathImpl)polygon_a._getImpl();
			com.epl.geometry.MultiPathImpl polyline_impl_b = (com.epl.geometry.MultiPathImpl)polyline_b._getImpl();
			com.epl.geometry.SegmentIteratorImpl segIterA = polygon_impl_a.QuerySegmentIterator();
			com.epl.geometry.SegmentIteratorImpl segIterB = polyline_impl_b.QuerySegmentIterator();
			double[] scalarsA = new double[2];
			double[] scalarsB = new double[2];
			com.epl.geometry.PairwiseIntersectorImpl intersector = new com.epl.geometry.PairwiseIntersectorImpl(polygon_impl_a, polyline_impl_b, tolerance, false);
			bool b_boundaries_intersect = false;
			while (intersector.Next())
			{
				int vertex_a = intersector.GetRedElement();
				int vertex_b = intersector.GetBlueElement();
				segIterA.ResetToVertex(vertex_a);
				segIterB.ResetToVertex(vertex_b);
				com.epl.geometry.Segment segmentA = segIterA.NextSegment();
				com.epl.geometry.Segment segmentB = segIterB.NextSegment();
				int result = segmentB.Intersect(segmentA, null, scalarsB, scalarsA, tolerance);
				if (result == 2)
				{
					b_boundaries_intersect = true;
				}
				else
				{
					if (result != 0)
					{
						double scalar_a_0 = scalarsA[0];
						double scalar_b_0 = scalarsB[0];
						if (scalar_a_0 > 0.0 && scalar_a_0 < 1.0 && scalar_b_0 > 0.0 && scalar_b_0 < 1.0)
						{
							return true;
						}
						b_boundaries_intersect = true;
					}
				}
			}
			if (!b_boundaries_intersect)
			{
				return false;
			}
			com.epl.geometry.Envelope2D env_a = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D env_b = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D envAInflated = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D envBInflated = new com.epl.geometry.Envelope2D();
			com.epl.geometry.Envelope2D envInter = new com.epl.geometry.Envelope2D();
			polygon_a.QueryEnvelope2D(env_a);
			polyline_b.QueryEnvelope2D(env_b);
			if (InteriorEnvExteriorEnv_(env_b, env_a, tolerance))
			{
				envAInflated.SetCoords(env_a);
				envAInflated.Inflate(1000.0 * tolerance, 1000.0 * tolerance);
				envBInflated.SetCoords(env_b);
				envBInflated.Inflate(1000.0 * tolerance, 1000.0 * tolerance);
				envInter.SetCoords(envAInflated);
				envInter.Intersect(envBInflated);
				com.epl.geometry.Polygon _polygonA;
				com.epl.geometry.Polyline _polylineB;
				if (polygon_a.GetPointCount() > 10)
				{
					_polygonA = (com.epl.geometry.Polygon)(com.epl.geometry.Clipper.Clip(polygon_a, envInter, tolerance, 0.0));
					if (_polygonA.IsEmpty())
					{
						return false;
					}
				}
				else
				{
					_polygonA = polygon_a;
				}
				if (polyline_b.GetPointCount() > 10)
				{
					_polylineB = (com.epl.geometry.Polyline)(com.epl.geometry.Clipper.Clip(polyline_b, envInter, tolerance, 0.0));
					if (_polylineB.IsEmpty())
					{
						return false;
					}
				}
				else
				{
					_polylineB = polyline_b;
				}
				string scl = "T********";
				bool bRelation = com.epl.geometry.RelationalOperationsMatrix.PolygonRelatePolyline_(_polygonA, _polylineB, tolerance, scl, progressTracker);
				return bRelation;
			}
			string scl_1 = "T*****T**";
			bool bRelation_1 = com.epl.geometry.RelationalOperationsMatrix.PolygonRelatePolyline_(polygon_a, polyline_b, tolerance, scl_1, progressTracker);
			return bRelation_1;
		}

		private static bool PolygonContainsPolylineImpl_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Polyline polyline_b, double tolerance, com.epl.geometry.ProgressTracker progress_tracker)
		{
			bool[] b_result_known = new bool[1];
			b_result_known[0] = false;
			bool res = PolygonContainsMultiPath_(polygon_a, polyline_b, tolerance, b_result_known, progress_tracker);
			if (b_result_known[0])
			{
				return res;
			}
			// We can clip polygon_a to the extent of polyline_b
			com.epl.geometry.Envelope2D envBInflated = new com.epl.geometry.Envelope2D();
			polyline_b.QueryEnvelope2D(envBInflated);
			envBInflated.Inflate(1000.0 * tolerance, 1000.0 * tolerance);
			com.epl.geometry.Polygon _polygonA = null;
			if (polygon_a.GetPointCount() > 10)
			{
				_polygonA = (com.epl.geometry.Polygon)com.epl.geometry.Clipper.Clip(polygon_a, envBInflated, tolerance, 0.0);
				if (_polygonA.IsEmpty())
				{
					return false;
				}
			}
			else
			{
				_polygonA = polygon_a;
			}
			bool bContains = com.epl.geometry.RelationalOperationsMatrix.PolygonContainsPolyline_(_polygonA, polyline_b, tolerance, progress_tracker);
			return bContains;
		}

		private static bool PolygonContainsPointImpl_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Point2D pt_b, double tolerance, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.PolygonUtils.PiPResult result = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon_a, pt_b, tolerance);
			if (result == com.epl.geometry.PolygonUtils.PiPResult.PiPInside)
			{
				return true;
			}
			return false;
		}

		private static bool PolygonTouchesPointImpl_(com.epl.geometry.Polygon polygon_a, com.epl.geometry.Point2D pt_b, double tolerance, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.PolygonUtils.PiPResult result = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(polygon_a, pt_b, tolerance);
			if (result == com.epl.geometry.PolygonUtils.PiPResult.PiPBoundary)
			{
				return true;
			}
			return false;
		}

		internal static bool MultiPointDisjointPointImpl_(com.epl.geometry.MultiPoint multipoint_a, com.epl.geometry.Point2D pt_b, double tolerance, com.epl.geometry.ProgressTracker progressTracker)
		{
			com.epl.geometry.Point2D pt_a = new com.epl.geometry.Point2D();
			double tolerance_sq = tolerance * tolerance;
			for (int i = 0; i < multipoint_a.GetPointCount(); i++)
			{
				multipoint_a.GetXY(i, pt_a);
				if (com.epl.geometry.Point2D.SqrDistance(pt_a, pt_b) <= tolerance_sq)
				{
					return false;
				}
			}
			return true;
		}

		internal sealed class OverlapEvent
		{
			internal int m_ivertex_a;

			internal int m_ipath_a;

			internal double m_scalar_a_0;

			internal double m_scalar_a_1;

			internal int m_ivertex_b;

			internal int m_ipath_b;

			internal double m_scalar_b_0;

			internal double m_scalar_b_1;

			internal static com.epl.geometry.RelationalOperations.OverlapEvent Construct(int ivertex_a, int ipath_a, double scalar_a_0, double scalar_a_1, int ivertex_b, int ipath_b, double scalar_b_0, double scalar_b_1)
			{
				com.epl.geometry.RelationalOperations.OverlapEvent overlapEvent = new com.epl.geometry.RelationalOperations.OverlapEvent();
				overlapEvent.m_ivertex_a = ivertex_a;
				overlapEvent.m_ipath_a = ipath_a;
				overlapEvent.m_scalar_a_0 = scalar_a_0;
				overlapEvent.m_scalar_a_1 = scalar_a_1;
				overlapEvent.m_ivertex_b = ivertex_b;
				overlapEvent.m_ipath_b = ipath_b;
				overlapEvent.m_scalar_b_0 = scalar_b_0;
				overlapEvent.m_scalar_b_1 = scalar_b_1;
				return overlapEvent;
			}
		}

		internal System.Collections.Generic.List<com.epl.geometry.RelationalOperations.OverlapEvent> m_overlap_events;

		private RelationalOperations()
		{
			m_overlap_events = new System.Collections.Generic.List<com.epl.geometry.RelationalOperations.OverlapEvent>();
		}

		private class OverlapComparer : com.epl.geometry.AttributeStreamOfInt32.IntComparator
		{
			internal OverlapComparer(com.epl.geometry.RelationalOperations rel_ops)
			{
				m_rel_ops = rel_ops;
			}

			public override int Compare(int o_1, int o_2)
			{
				return m_rel_ops.CompareOverlapEvents_(o_1, o_2);
			}

			private com.epl.geometry.RelationalOperations m_rel_ops;
		}

		internal virtual int CompareOverlapEvents_(int o_1, int o_2)
		{
			com.epl.geometry.RelationalOperations.OverlapEvent overlapEvent1 = m_overlap_events[o_1];
			com.epl.geometry.RelationalOperations.OverlapEvent overlapEvent2 = m_overlap_events[o_2];
			if (overlapEvent1.m_ipath_a < overlapEvent2.m_ipath_a)
			{
				return -1;
			}
			if (overlapEvent1.m_ipath_a == overlapEvent2.m_ipath_a)
			{
				if (overlapEvent1.m_ivertex_a < overlapEvent2.m_ivertex_a)
				{
					return -1;
				}
				if (overlapEvent1.m_ivertex_a == overlapEvent2.m_ivertex_a)
				{
					if (overlapEvent1.m_scalar_a_0 < overlapEvent2.m_scalar_a_0)
					{
						return -1;
					}
					if (overlapEvent1.m_scalar_a_0 == overlapEvent2.m_scalar_a_0)
					{
						if (overlapEvent1.m_scalar_a_1 < overlapEvent2.m_scalar_a_1)
						{
							return -1;
						}
						if (overlapEvent1.m_scalar_a_1 == overlapEvent2.m_scalar_a_1)
						{
							if (overlapEvent1.m_ivertex_b < overlapEvent2.m_ivertex_b)
							{
								return -1;
							}
						}
					}
				}
			}
			return 1;
		}

		internal sealed class Accelerate_helper
		{
			internal static bool Accelerate_geometry(com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference sr, com.epl.geometry.Geometry.GeometryAccelerationDegree accel_degree)
			{
				if (!Can_accelerate_geometry(geometry))
				{
					return false;
				}
				double tol = com.epl.geometry.InternalUtils.CalculateToleranceFromGeometry(sr, geometry, false);
				bool bAccelerated = false;
				if (com.epl.geometry.GeometryAccelerators.CanUseRasterizedGeometry(geometry))
				{
					bAccelerated |= ((com.epl.geometry.MultiVertexGeometryImpl)geometry._getImpl())._buildRasterizedGeometryAccelerator(tol, accel_degree);
				}
				com.epl.geometry.Geometry.Type type = geometry.GetType();
				if ((type == com.epl.geometry.Geometry.Type.Polygon || type == com.epl.geometry.Geometry.Type.Polyline) && com.epl.geometry.GeometryAccelerators.CanUseQuadTree(geometry) && accel_degree != com.epl.geometry.Geometry.GeometryAccelerationDegree.enumMild)
				{
					bAccelerated |= ((com.epl.geometry.MultiVertexGeometryImpl)geometry._getImpl())._buildQuadTreeAccelerator(accel_degree);
				}
				if ((type == com.epl.geometry.Geometry.Type.Polygon || type == com.epl.geometry.Geometry.Type.Polyline) && com.epl.geometry.GeometryAccelerators.CanUseQuadTreeForPaths(geometry) && accel_degree != com.epl.geometry.Geometry.GeometryAccelerationDegree.enumMild)
				{
					bAccelerated |= ((com.epl.geometry.MultiPathImpl)geometry._getImpl())._buildQuadTreeForPathsAccelerator(accel_degree);
				}
				return bAccelerated;
			}

			internal static bool Can_accelerate_geometry(com.epl.geometry.Geometry geometry)
			{
				return com.epl.geometry.GeometryAccelerators.CanUseRasterizedGeometry(geometry) || com.epl.geometry.GeometryAccelerators.CanUseQuadTree(geometry) || com.epl.geometry.GeometryAccelerators.CanUseQuadTreeForPaths(geometry);
			}
		}
	}
}
