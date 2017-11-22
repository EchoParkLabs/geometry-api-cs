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
	internal class OperatorInternalRelationUtils
	{
		internal abstract class Relation
		{
			public const int Unknown = 0;

			public const int Contains = 1;

			public const int Within = 2;

			public const int Equals = 3;

			public const int Disjoint = 4;

			public const int Touches = 8;

			public const int Crosses = 16;

			public const int Overlaps = 32;

			public const int NoThisRelation = 64;

			public const int Intersects = unchecked((int)(0x40000000));

			public const int IntersectsOrDisjoint = Intersects | Disjoint;
			// == Within | Contains tests both within
			// and contains
			// returned when the relation is
			// not satisified
			// this means not_disjoint.
			// Used for early bailout
		}

		internal static class RelationConstants
		{
		}

		public static int QuickTest2D(com.epl.geometry.Geometry geomA, com.epl.geometry.Geometry geomB, double tolerance, int testType)
		{
			if (geomB.IsEmpty() || geomA.IsEmpty())
			{
				return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint;
			}
			int geomAtype = geomA.GetType().Value();
			int geomBtype = geomB.GetType().Value();
			// We do not support segments directly for now. Convert to Polyline
			com.epl.geometry.Polyline autoPolyA;
			if (com.epl.geometry.Geometry.IsSegment(geomAtype))
			{
				autoPolyA = new com.epl.geometry.Polyline(geomA.GetDescription());
				geomA = (com.epl.geometry.Geometry)autoPolyA;
				autoPolyA.AddSegment((com.epl.geometry.Segment)geomA, true);
			}
			com.epl.geometry.Polyline autoPolyB;
			if (com.epl.geometry.Geometry.IsSegment(geomBtype))
			{
				autoPolyB = new com.epl.geometry.Polyline(geomB.GetDescription());
				geomB = (com.epl.geometry.Geometry)autoPolyB;
				autoPolyB.AddSegment((com.epl.geometry.Segment)geomB, true);
			}
			switch (geomAtype)
			{
				case com.epl.geometry.Geometry.GeometryType.Point:
				{
					switch (geomBtype)
					{
						case com.epl.geometry.Geometry.GeometryType.Point:
						{
							// Now process GeometryxGeometry case by case
							return QuickTest2DPointPoint((com.epl.geometry.Point)geomA, (com.epl.geometry.Point)geomB, tolerance);
						}

						case com.epl.geometry.Geometry.GeometryType.Envelope:
						{
							return ReverseResult(QuickTest2DEnvelopePoint((com.epl.geometry.Envelope)geomB, (com.epl.geometry.Point)geomA, tolerance));
						}

						case com.epl.geometry.Geometry.GeometryType.MultiPoint:
						{
							return ReverseResult(QuickTest2DMultiPointPoint((com.epl.geometry.MultiPoint)geomB, (com.epl.geometry.Point)geomA, tolerance));
						}

						case com.epl.geometry.Geometry.GeometryType.Polyline:
						{
							return ReverseResult(QuickTest2DPolylinePoint((com.epl.geometry.Polyline)geomB, (com.epl.geometry.Point)geomA, tolerance, testType));
						}

						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							return ReverseResult(QuickTest2DPolygonPoint((com.epl.geometry.Polygon)geomB, (com.epl.geometry.Point)geomA, tolerance));
						}
					}
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}

				case com.epl.geometry.Geometry.GeometryType.Envelope:
				{
					switch (geomBtype)
					{
						case com.epl.geometry.Geometry.GeometryType.Point:
						{
							// GEOMTHROW(internal_error);//what
							// else?
							return QuickTest2DEnvelopePoint((com.epl.geometry.Envelope)geomA, (com.epl.geometry.Point)geomB, tolerance);
						}

						case com.epl.geometry.Geometry.GeometryType.Envelope:
						{
							return QuickTest2DEnvelopeEnvelope((com.epl.geometry.Envelope)geomA, (com.epl.geometry.Envelope)geomB, tolerance);
						}

						case com.epl.geometry.Geometry.GeometryType.MultiPoint:
						{
							return ReverseResult(QuickTest2DMultiPointEnvelope((com.epl.geometry.MultiPoint)geomB, (com.epl.geometry.Envelope)geomA, tolerance, testType));
						}

						case com.epl.geometry.Geometry.GeometryType.Polyline:
						{
							return ReverseResult(QuickTest2DPolylineEnvelope((com.epl.geometry.Polyline)geomB, (com.epl.geometry.Envelope)geomA, tolerance));
						}

						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							return ReverseResult(QuickTest2DPolygonEnvelope((com.epl.geometry.Polygon)geomB, (com.epl.geometry.Envelope)geomA, tolerance));
						}
					}
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}

				case com.epl.geometry.Geometry.GeometryType.MultiPoint:
				{
					switch (geomBtype)
					{
						case com.epl.geometry.Geometry.GeometryType.Point:
						{
							// GEOMTHROW(internal_error);//what
							// else?
							return QuickTest2DMultiPointPoint((com.epl.geometry.MultiPoint)geomA, (com.epl.geometry.Point)geomB, tolerance);
						}

						case com.epl.geometry.Geometry.GeometryType.Envelope:
						{
							return QuickTest2DMultiPointEnvelope((com.epl.geometry.MultiPoint)geomA, (com.epl.geometry.Envelope)geomB, tolerance, testType);
						}

						case com.epl.geometry.Geometry.GeometryType.MultiPoint:
						{
							return QuickTest2DMultiPointMultiPoint((com.epl.geometry.MultiPoint)geomA, (com.epl.geometry.MultiPoint)geomB, tolerance, testType);
						}

						case com.epl.geometry.Geometry.GeometryType.Polyline:
						{
							return ReverseResult(QuickTest2DPolylineMultiPoint((com.epl.geometry.Polyline)geomB, (com.epl.geometry.MultiPoint)geomA, tolerance));
						}

						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							return ReverseResult(QuickTest2DPolygonMultiPoint((com.epl.geometry.Polygon)geomB, (com.epl.geometry.MultiPoint)geomA, tolerance));
						}
					}
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}

				case com.epl.geometry.Geometry.GeometryType.Polyline:
				{
					switch (geomBtype)
					{
						case com.epl.geometry.Geometry.GeometryType.Point:
						{
							// GEOMTHROW(internal_error);//what
							// else?
							return QuickTest2DPolylinePoint((com.epl.geometry.Polyline)geomA, (com.epl.geometry.Point)geomB, tolerance, testType);
						}

						case com.epl.geometry.Geometry.GeometryType.Envelope:
						{
							return QuickTest2DPolylineEnvelope((com.epl.geometry.Polyline)geomA, (com.epl.geometry.Envelope)geomB, tolerance);
						}

						case com.epl.geometry.Geometry.GeometryType.MultiPoint:
						{
							return QuickTest2DPolylineMultiPoint((com.epl.geometry.Polyline)geomA, (com.epl.geometry.MultiPoint)geomB, tolerance);
						}

						case com.epl.geometry.Geometry.GeometryType.Polyline:
						{
							return QuickTest2DPolylinePolyline((com.epl.geometry.Polyline)geomA, (com.epl.geometry.Polyline)geomB, tolerance);
						}

						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							return ReverseResult(QuickTest2DPolygonPolyline((com.epl.geometry.Polygon)geomB, (com.epl.geometry.Polyline)geomA, tolerance));
						}
					}
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}

				case com.epl.geometry.Geometry.GeometryType.Polygon:
				{
					switch (geomBtype)
					{
						case com.epl.geometry.Geometry.GeometryType.Point:
						{
							// GEOMTHROW(internal_error);//what
							// else?
							return QuickTest2DPolygonPoint((com.epl.geometry.Polygon)geomA, (com.epl.geometry.Point)geomB, tolerance);
						}

						case com.epl.geometry.Geometry.GeometryType.Envelope:
						{
							return QuickTest2DPolygonEnvelope((com.epl.geometry.Polygon)geomA, (com.epl.geometry.Envelope)geomB, tolerance);
						}

						case com.epl.geometry.Geometry.GeometryType.MultiPoint:
						{
							return QuickTest2DPolygonMultiPoint((com.epl.geometry.Polygon)geomA, (com.epl.geometry.MultiPoint)geomB, tolerance);
						}

						case com.epl.geometry.Geometry.GeometryType.Polyline:
						{
							return QuickTest2DPolygonPolyline((com.epl.geometry.Polygon)geomA, (com.epl.geometry.Polyline)geomB, tolerance);
						}

						case com.epl.geometry.Geometry.GeometryType.Polygon:
						{
							return QuickTest2DPolygonPolygon((com.epl.geometry.Polygon)geomA, (com.epl.geometry.Polygon)geomB, tolerance);
						}
					}
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}

				default:
				{
					// GEOMTHROW(internal_error);//what
					// else?
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
			}
		}

		// GEOMTHROW(internal_error);//what
		// else?
		// return 0;
		private static int QuickTest2DPointPoint(com.epl.geometry.Point geomA, com.epl.geometry.Point geomB, double tolerance)
		{
			com.epl.geometry.Point2D ptA = geomA.GetXY();
			com.epl.geometry.Point2D ptB = geomB.GetXY();
			return QuickTest2DPointPoint(ptA, ptB, tolerance);
		}

		private static int QuickTest2DPointPoint(com.epl.geometry.Point2D ptA, com.epl.geometry.Point2D ptB, double tolerance)
		{
			ptA.Sub(ptB);
			double len = ptA.SqrLength();
			// Should we test against 2*tol or tol?
			if (len <= tolerance * tolerance)
			{
				// Two points are equal if they are not
				// Disjoint. We consider a point to
				// be a disk of radius tolerance.
				// Any intersection of two disks
				// produces same disk.
				return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Within | (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains;
			}
			// ==Equals
			return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint;
		}

		private static int QuickTest2DEnvelopePoint(com.epl.geometry.Envelope geomA, com.epl.geometry.Point geomB, double tolerance)
		{
			com.epl.geometry.Envelope2D geomAEnv = new com.epl.geometry.Envelope2D();
			geomA.QueryEnvelope2D(geomAEnv);
			com.epl.geometry.Point2D ptB;
			ptB = geomB.GetXY();
			return QuickTest2DEnvelopePoint(geomAEnv, ptB, tolerance);
		}

		private static int QuickTest2DEnvelopePoint(com.epl.geometry.Envelope2D geomAEnv, com.epl.geometry.Point2D ptB, double tolerance)
		{
			com.epl.geometry.Envelope2D envAMinus = geomAEnv;
			envAMinus.Inflate(-tolerance, -tolerance);
			if (envAMinus.Contains(ptB))
			{
				return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains;
			}
			// clementini's contains
			com.epl.geometry.Envelope2D envAPlus = geomAEnv;
			envAPlus.Inflate(tolerance, tolerance);
			if (envAPlus.Contains(ptB))
			{
				return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Touches;
			}
			// clementini's touches
			return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint;
		}

		// clementini's disjoint
		private static int QuickTest2DEnvelopePoint(com.epl.geometry.Envelope2D envAPlus, com.epl.geometry.Envelope2D envAMinus, com.epl.geometry.Point2D ptB, double tolerance)
		{
			if (envAMinus.Contains(ptB))
			{
				return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains;
			}
			// clementini's contains
			if (envAPlus.Contains(ptB))
			{
				return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Touches;
			}
			// clementini's touches
			return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint;
		}

		// clementini's disjoint
		private static int QuickTest2DEnvelopeEnvelope(com.epl.geometry.Envelope geomA, com.epl.geometry.Envelope geomB, double tolerance)
		{
			com.epl.geometry.Envelope2D geomAEnv = new com.epl.geometry.Envelope2D();
			geomA.QueryEnvelope2D(geomAEnv);
			com.epl.geometry.Envelope2D geomBEnv = new com.epl.geometry.Envelope2D();
			geomB.QueryEnvelope2D(geomBEnv);
			return QuickTest2DEnvelopeEnvelope(geomAEnv, geomBEnv, tolerance);
		}

		private static int QuickTest2DEnvelopeEnvelope(com.epl.geometry.Envelope2D geomAEnv, com.epl.geometry.Envelope2D geomBEnv, double tolerance)
		{
			// firstly check for contains and within to give a chance degenerate
			// envelopes to work.
			// otherwise, if there are two degenerate envelopes that are equal,
			// Touch relation may occur.
			int res = 0;
			if (geomAEnv.Contains(geomBEnv))
			{
				res |= (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains;
			}
			if (geomBEnv.Contains(geomAEnv))
			{
				res |= (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Within;
			}
			if (res != 0)
			{
				return res;
			}
			com.epl.geometry.Envelope2D envAMinus = geomAEnv;
			envAMinus.Inflate(-tolerance, -tolerance);
			// Envelope A interior
			com.epl.geometry.Envelope2D envBMinus = geomBEnv;
			envBMinus.Inflate(-tolerance, -tolerance);
			// Envelope B interior
			if (envAMinus.IsIntersecting(envBMinus))
			{
				com.epl.geometry.Envelope2D envAPlus = geomAEnv;
				envAPlus.Inflate(tolerance, tolerance);
				// Envelope A interior plus
				// boundary
				res = envAPlus.Contains(geomBEnv) ? (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains : 0;
				com.epl.geometry.Envelope2D envBPlus = geomBEnv;
				envBPlus.Inflate(tolerance, tolerance);
				// Envelope A interior plus
				// boundary
				res |= envBPlus.Contains(geomAEnv) ? (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Within : 0;
				if (res != 0)
				{
					return res;
				}
				return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Overlaps;
			}
			else
			{
				// Clementini's Overlap
				com.epl.geometry.Envelope2D envAPlus = geomAEnv;
				envAPlus.Inflate(tolerance, tolerance);
				// Envelope A interior plus
				// boundary
				com.epl.geometry.Envelope2D envBPlus = geomBEnv;
				envBPlus.Inflate(tolerance, tolerance);
				// Envelope A interior plus
				// boundary
				if (envAPlus.IsIntersecting(envBPlus))
				{
					return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Touches;
				}
				else
				{
					// Clementini Touch
					return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint;
				}
			}
		}

		// Clementini Disjoint
		private static int QuickTest2DMultiPointPoint(com.epl.geometry.MultiPoint geomA, com.epl.geometry.Point geomB, double tolerance)
		{
			com.epl.geometry.Point2D ptB;
			ptB = geomB.GetXY();
			return QuickTest2DMultiPointPoint(geomA, ptB, tolerance);
		}

		private static int QuickTest2DMultiPointPoint(com.epl.geometry.MultiPoint geomA, com.epl.geometry.Point2D ptB, double tolerance)
		{
			// TODO: Add Geometry accelerator. (RasterizedGeometry + kd-tree or
			// alike)
			for (int i = 0, n = geomA.GetPointCount(); i < n; i++)
			{
				com.epl.geometry.Point2D ptA;
				ptA = geomA.GetXY(i);
				int res = QuickTest2DPointPoint(ptA, ptB, tolerance);
				if (res != (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint)
				{
					if ((res & (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Within) != 0 && n != 1)
					{
						// _ASSERT(res & (int)Relation.Contains);
						return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains;
					}
					return res;
				}
			}
			return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint;
		}

		private static int QuickTest2DMultiPointEnvelope(com.epl.geometry.MultiPoint geomA, com.epl.geometry.Envelope geomB, double tolerance, int testType)
		{
			com.epl.geometry.Envelope2D geomBEnv = new com.epl.geometry.Envelope2D();
			geomB.QueryEnvelope2D(geomBEnv);
			return QuickTest2DMultiPointEnvelope(geomA, geomBEnv, tolerance, testType);
		}

		private static int QuickTest2DMultiPointEnvelope(com.epl.geometry.MultiPoint geomA, com.epl.geometry.Envelope2D geomBEnv, double tolerance, int testType)
		{
			// Add early bailout for disjoint test.
			com.epl.geometry.Envelope2D envBMinus = geomBEnv;
			envBMinus.Inflate(-tolerance, -tolerance);
			com.epl.geometry.Envelope2D envBPlus = geomBEnv;
			envBPlus.Inflate(tolerance, tolerance);
			int dres = 0;
			for (int i = 0, n = geomA.GetPointCount(); i < n; i++)
			{
				com.epl.geometry.Point2D ptA;
				ptA = geomA.GetXY(i);
				int res = ReverseResult(QuickTest2DEnvelopePoint(envBPlus, envBMinus, ptA, tolerance));
				if (res != (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint)
				{
					dres |= res;
					if (testType == (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint)
					{
						return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Intersects;
					}
				}
			}
			if (dres == 0)
			{
				return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint;
			}
			if (dres == (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Within)
			{
				return dres;
			}
			return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Overlaps;
		}

		private static int QuickTest2DMultiPointMultiPoint(com.epl.geometry.MultiPoint geomA, com.epl.geometry.MultiPoint geomB, double tolerance, int testType)
		{
			int counter = 0;
			for (int ib = 0, nb = geomB.GetPointCount(); ib < nb; ib++)
			{
				com.epl.geometry.Point2D ptB;
				ptB = geomB.GetXY(ib);
				int res = QuickTest2DMultiPointPoint(geomA, ptB, tolerance);
				if (res != (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint)
				{
					counter++;
					if (testType == (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint)
					{
						return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Intersects;
					}
				}
			}
			if (counter > 0)
			{
				if (counter == geomB.GetPointCount())
				{
					// every point from B is within
					// A. Means the A contains B
					if (testType == (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Equals)
					{
						// This is slow.
						// Refactor.
						int res = QuickTest2DMultiPointMultiPoint(geomB, geomA, tolerance, (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains);
						return res == (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains ? (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Equals : (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Unknown;
					}
					return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains;
				}
				else
				{
					return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Overlaps;
				}
			}
			return 0;
		}

		private static int QuickTest2DPolylinePoint(com.epl.geometry.Polyline geomA, com.epl.geometry.Point geomB, double tolerance, int testType)
		{
			com.epl.geometry.Point2D ptB;
			ptB = geomB.GetXY();
			return QuickTest2DPolylinePoint(geomA, ptB, tolerance, testType);
		}

		private static int QuickTest2DMVPointRasterOnly(com.epl.geometry.MultiVertexGeometry geomA, com.epl.geometry.Point2D ptB, double tolerance)
		{
			// Use rasterized Geometry:
			com.epl.geometry.RasterizedGeometry2D rgeomA = null;
			com.epl.geometry.MultiVertexGeometryImpl mpImpl = (com.epl.geometry.MultiVertexGeometryImpl)geomA._getImpl();
			com.epl.geometry.GeometryAccelerators gaccel = mpImpl._getAccelerators();
			if (gaccel != null)
			{
				rgeomA = gaccel.GetRasterizedGeometry();
			}
			if (rgeomA != null)
			{
				com.epl.geometry.RasterizedGeometry2D.HitType hitres = rgeomA.QueryPointInGeometry(ptB.x, ptB.y);
				if (hitres == com.epl.geometry.RasterizedGeometry2D.HitType.Outside)
				{
					return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint;
				}
				if (hitres == com.epl.geometry.RasterizedGeometry2D.HitType.Inside)
				{
					return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains;
				}
			}
			else
			{
				return -1;
			}
			return 0;
		}

		private static int QuickTest2DPolylinePoint(com.epl.geometry.Polyline geomA, com.epl.geometry.Point2D ptB, double tolerance, int testType)
		{
			int mask = com.epl.geometry.OperatorInternalRelationUtils.Relation.Touches | com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains | com.epl.geometry.OperatorInternalRelationUtils.Relation.Within | com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint | com.epl.geometry.OperatorInternalRelationUtils.Relation
				.Intersects;
			if ((testType & mask) == 0)
			{
				return com.epl.geometry.OperatorInternalRelationUtils.Relation.NoThisRelation;
			}
			int res = QuickTest2DMVPointRasterOnly(geomA, ptB, tolerance);
			if (res > 0)
			{
				return res;
			}
			// Go through the segments:
			double toleranceSqr = tolerance * tolerance;
			com.epl.geometry.MultiPathImpl mpImpl = (com.epl.geometry.MultiPathImpl)geomA._getImpl();
			com.epl.geometry.SegmentIteratorImpl iter = mpImpl.QuerySegmentIterator();
			while (iter.NextPath())
			{
				int pathIndex = iter.GetPathIndex();
				if (!geomA.IsClosedPath(pathIndex))
				{
					int pathSize = geomA.GetPathSize(pathIndex);
					int pathStart = geomA.GetPathStart(pathIndex);
					if (pathSize == 0)
					{
						continue;
					}
					if (com.epl.geometry.Point2D.SqrDistance(geomA.GetXY(pathStart), ptB) <= toleranceSqr || (pathSize > 1 && com.epl.geometry.Point2D.SqrDistance(geomA.GetXY(pathStart + pathSize - 1), ptB) <= toleranceSqr))
					{
						return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Touches;
					}
				}
				if (testType != com.epl.geometry.OperatorInternalRelationUtils.Relation.Touches)
				{
					while (iter.HasNextSegment())
					{
						com.epl.geometry.Segment segment = iter.NextSegment();
						double t = segment.GetClosestCoordinate(ptB, false);
						com.epl.geometry.Point2D pt = segment.GetCoord2D(t);
						if (com.epl.geometry.Point2D.SqrDistance(pt, ptB) <= toleranceSqr)
						{
							if ((testType & com.epl.geometry.OperatorInternalRelationUtils.Relation.IntersectsOrDisjoint) != 0)
							{
								return com.epl.geometry.OperatorInternalRelationUtils.Relation.Intersects;
							}
							return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains;
						}
					}
				}
			}
			return (testType & com.epl.geometry.OperatorInternalRelationUtils.Relation.IntersectsOrDisjoint) != 0 ? com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint : com.epl.geometry.OperatorInternalRelationUtils.Relation.NoThisRelation;
		}

		private static int QuickTest2DPolylineEnvelope(com.epl.geometry.Polyline geomA, com.epl.geometry.Envelope geomB, double tolerance)
		{
			com.epl.geometry.Envelope2D geomBEnv = new com.epl.geometry.Envelope2D();
			geomB.QueryEnvelope2D(geomBEnv);
			return QuickTest2DPolylineEnvelope(geomA, geomBEnv, tolerance);
		}

		private static int QuickTest2DPolylineEnvelope(com.epl.geometry.Polyline geomA, com.epl.geometry.Envelope2D geomBEnv, double tolerance)
		{
			int res = QuickTest2DMVEnvelopeRasterOnly(geomA, geomBEnv, tolerance);
			if (res > 0)
			{
				return res;
			}
			// TODO: implement me
			return 0;
		}

		private static int QuickTest2DMVEnvelopeRasterOnly(com.epl.geometry.MultiVertexGeometry geomA, com.epl.geometry.Envelope2D geomBEnv, double tolerance)
		{
			// Use rasterized Geometry only:
			com.epl.geometry.RasterizedGeometry2D rgeomA;
			com.epl.geometry.MultiVertexGeometryImpl mpImpl = (com.epl.geometry.MultiVertexGeometryImpl)geomA._getImpl();
			com.epl.geometry.GeometryAccelerators gaccel = mpImpl._getAccelerators();
			if (gaccel != null)
			{
				rgeomA = gaccel.GetRasterizedGeometry();
			}
			else
			{
				return -1;
			}
			if (rgeomA != null)
			{
				com.epl.geometry.RasterizedGeometry2D.HitType hitres = rgeomA.QueryEnvelopeInGeometry(geomBEnv);
				if (hitres == com.epl.geometry.RasterizedGeometry2D.HitType.Outside)
				{
					return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint;
				}
				if (hitres == com.epl.geometry.RasterizedGeometry2D.HitType.Inside)
				{
					return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains;
				}
			}
			else
			{
				return -1;
			}
			return 0;
		}

		private static int QuickTest2DPolylineMultiPoint(com.epl.geometry.Polyline geomA, com.epl.geometry.MultiPoint geomB, double tolerance)
		{
			com.epl.geometry.Envelope2D geomBEnv = new com.epl.geometry.Envelope2D();
			geomB.QueryEnvelope2D(geomBEnv);
			int res = QuickTest2DMVEnvelopeRasterOnly(geomA, geomBEnv, tolerance);
			if (res > 0)
			{
				return res;
			}
			// TODO: implement me
			return 0;
		}

		private static int QuickTest2DMVMVRasterOnly(com.epl.geometry.MultiVertexGeometry geomA, com.epl.geometry.MultiVertexGeometry geomB, double tolerance)
		{
			com.epl.geometry.Envelope2D geomBEnv = new com.epl.geometry.Envelope2D();
			geomB.QueryEnvelope2D(geomBEnv);
			int res = QuickTest2DMVEnvelopeRasterOnly(geomA, geomBEnv, tolerance);
			if (res > 0)
			{
				return res;
			}
			if (res == -1)
			{
				com.epl.geometry.Envelope2D geomAEnv = new com.epl.geometry.Envelope2D();
				geomA.QueryEnvelope2D(geomAEnv);
				res = QuickTest2DMVEnvelopeRasterOnly(geomB, geomAEnv, tolerance);
				if (res > 0)
				{
					return ReverseResult(res);
				}
			}
			// TODO: implement me
			return 0;
		}

		private static int QuickTest2DPolylinePolyline(com.epl.geometry.Polyline geomA, com.epl.geometry.Polyline geomB, double tolerance)
		{
			int res = QuickTest2DMVMVRasterOnly(geomA, geomB, tolerance);
			if (res > 0)
			{
				return res;
			}
			// TODO: implement me
			return 0;
		}

		private static int QuickTest2DPolygonPoint(com.epl.geometry.Polygon geomA, com.epl.geometry.Point geomB, double tolerance)
		{
			com.epl.geometry.Point2D ptB;
			ptB = geomB.GetXY();
			return QuickTest2DPolygonPoint(geomA, ptB, tolerance);
		}

		private static int QuickTest2DPolygonPoint(com.epl.geometry.Polygon geomA, com.epl.geometry.Point2D ptB, double tolerance)
		{
			com.epl.geometry.PolygonUtils.PiPResult pipres = com.epl.geometry.PolygonUtils.IsPointInPolygon2D(geomA, ptB, tolerance);
			// this method uses the accelerator if available
			if (pipres == com.epl.geometry.PolygonUtils.PiPResult.PiPOutside)
			{
				return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint;
			}
			// clementini's disjoint
			if (pipres == com.epl.geometry.PolygonUtils.PiPResult.PiPInside)
			{
				return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains;
			}
			// clementini's contains
			if (pipres == com.epl.geometry.PolygonUtils.PiPResult.PiPBoundary)
			{
				return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Touches;
			}
			// clementini's touches
			throw com.epl.geometry.GeometryException.GeometryInternalError();
		}

		// GEOMTHROW(internal_error);
		// //what else
		// return 0;
		private static int QuickTest2DPolygonEnvelope(com.epl.geometry.Polygon geomA, com.epl.geometry.Envelope geomB, double tolerance)
		{
			com.epl.geometry.Envelope2D geomBEnv = new com.epl.geometry.Envelope2D();
			geomB.QueryEnvelope2D(geomBEnv);
			return QuickTest2DPolygonEnvelope(geomA, geomBEnv, tolerance);
		}

		private static int QuickTest2DPolygonEnvelope(com.epl.geometry.Polygon geomA, com.epl.geometry.Envelope2D geomBEnv, double tolerance)
		{
			int res = QuickTest2DMVEnvelopeRasterOnly(geomA, geomBEnv, tolerance);
			if (res > 0)
			{
				return res;
			}
			// TODO: implement me
			return 0;
		}

		private static int QuickTest2DPolygonMultiPoint(com.epl.geometry.Polygon geomA, com.epl.geometry.MultiPoint geomB, double tolerance)
		{
			int res = QuickTest2DMVMVRasterOnly(geomA, geomB, tolerance);
			if (res > 0)
			{
				return res;
			}
			// TODO: implement me
			return 0;
		}

		private static int QuickTest2DPolygonPolyline(com.epl.geometry.Polygon geomA, com.epl.geometry.Polyline geomB, double tolerance)
		{
			int res = QuickTest2DMVMVRasterOnly(geomA, geomB, tolerance);
			if (res > 0)
			{
				return res;
			}
			// TODO: implement me
			return 0;
		}

		private static int QuickTest2DPolygonPolygon(com.epl.geometry.Polygon geomA, com.epl.geometry.Polygon geomB, double tolerance)
		{
			int res = QuickTest2DMVMVRasterOnly(geomA, geomB, tolerance);
			if (res > 0)
			{
				return res;
			}
			// TODO: implement me
			return 0;
		}

		public static int QuickTest2D_Accelerated_DisjointOrContains(com.epl.geometry.Geometry geomA, com.epl.geometry.Geometry geomB, double tolerance)
		{
			int gtA = geomA.GetType().Value();
			int gtB = geomB.GetType().Value();
			com.epl.geometry.GeometryAccelerators accel;
			bool endWhileStatement = false;
			do
			{
				if (com.epl.geometry.Geometry.IsMultiVertex(gtA))
				{
					com.epl.geometry.MultiVertexGeometryImpl impl = (com.epl.geometry.MultiVertexGeometryImpl)geomA._getImpl();
					accel = impl._getAccelerators();
					if (accel != null)
					{
						com.epl.geometry.RasterizedGeometry2D rgeom = accel.GetRasterizedGeometry();
						if (rgeom != null)
						{
							if (gtB == com.epl.geometry.Geometry.GeometryType.Point)
							{
								com.epl.geometry.Point2D ptB = ((com.epl.geometry.Point)geomB).GetXY();
								com.epl.geometry.RasterizedGeometry2D.HitType hit = rgeom.QueryPointInGeometry(ptB.x, ptB.y);
								if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Inside)
								{
									return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains;
								}
								else
								{
									if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Outside)
									{
										return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint;
									}
								}
								break;
							}
							com.epl.geometry.Envelope2D envB = new com.epl.geometry.Envelope2D();
							geomB.QueryEnvelope2D(envB);
							com.epl.geometry.RasterizedGeometry2D.HitType hit_1 = rgeom.QueryEnvelopeInGeometry(envB);
							if (hit_1 == com.epl.geometry.RasterizedGeometry2D.HitType.Inside)
							{
								return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains;
							}
							else
							{
								if (hit_1 == com.epl.geometry.RasterizedGeometry2D.HitType.Outside)
								{
									return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint;
								}
							}
							break;
						}
					}
				}
			}
			while (endWhileStatement);
			accel = null;
			do
			{
				if (com.epl.geometry.Geometry.IsMultiVertex(gtB))
				{
					com.epl.geometry.MultiVertexGeometryImpl impl = (com.epl.geometry.MultiVertexGeometryImpl)geomB._getImpl();
					accel = impl._getAccelerators();
					if (accel != null)
					{
						com.epl.geometry.RasterizedGeometry2D rgeom = accel.GetRasterizedGeometry();
						if (rgeom != null)
						{
							if (gtA == com.epl.geometry.Geometry.GeometryType.Point)
							{
								com.epl.geometry.Point2D ptA = ((com.epl.geometry.Point)geomA).GetXY();
								com.epl.geometry.RasterizedGeometry2D.HitType hit = rgeom.QueryPointInGeometry(ptA.x, ptA.y);
								if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Inside)
								{
									return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Within;
								}
								else
								{
									if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Outside)
									{
										return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint;
									}
								}
								break;
							}
							com.epl.geometry.Envelope2D envA = new com.epl.geometry.Envelope2D();
							geomA.QueryEnvelope2D(envA);
							com.epl.geometry.RasterizedGeometry2D.HitType hit_1 = rgeom.QueryEnvelopeInGeometry(envA);
							if (hit_1 == com.epl.geometry.RasterizedGeometry2D.HitType.Inside)
							{
								return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Within;
							}
							else
							{
								if (hit_1 == com.epl.geometry.RasterizedGeometry2D.HitType.Outside)
								{
									return (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Disjoint;
								}
							}
							break;
						}
					}
				}
			}
			while (endWhileStatement);
			return 0;
		}

		private static int ReverseResult(int resIn)
		{
			int res = resIn;
			if ((res & (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains) != 0)
			{
				res &= ~(int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains;
				res |= (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Within;
			}
			if ((res & (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Within) != 0)
			{
				res &= ~(int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Within;
				res |= (int)com.epl.geometry.OperatorInternalRelationUtils.Relation.Contains;
			}
			return res;
		}
	}
}
