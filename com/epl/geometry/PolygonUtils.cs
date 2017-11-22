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
	internal sealed class PolygonUtils
	{
		internal enum PiPResult
		{
			PiPOutside,
			PiPInside,
			PiPBoundary
		}

		// enum_class PiPResult { PiPOutside = 0, PiPInside = 1, PiPBoundary = 2};
		/// <summary>Tests if Point is inside the Polygon.</summary>
		/// <remarks>
		/// Tests if Point is inside the Polygon. Returns PiPOutside if not in
		/// polygon, PiPInside if in the polygon, PiPBoundary is if on the border. It
		/// tests border only if the tolerance is &gt; 0, otherwise PiPBoundary cannot
		/// be returned. Note: If the tolerance is not 0, the test is more expensive
		/// because it calculates closest distance from a point to each segment.
		/// O(n) complexity, where n is the number of polygon segments.
		/// </remarks>
		public static com.epl.geometry.PolygonUtils.PiPResult IsPointInPolygon2D(com.epl.geometry.Polygon polygon, com.epl.geometry.Point inputPoint, double tolerance)
		{
			int res = com.epl.geometry.PointInPolygonHelper.IsPointInPolygon(polygon, inputPoint, tolerance);
			if (res == 0)
			{
				return com.epl.geometry.PolygonUtils.PiPResult.PiPOutside;
			}
			if (res == 1)
			{
				return com.epl.geometry.PolygonUtils.PiPResult.PiPInside;
			}
			return com.epl.geometry.PolygonUtils.PiPResult.PiPBoundary;
		}

		public static com.epl.geometry.PolygonUtils.PiPResult IsPointInPolygon2D(com.epl.geometry.Polygon polygon, com.epl.geometry.Point2D inputPoint, double tolerance)
		{
			int res = com.epl.geometry.PointInPolygonHelper.IsPointInPolygon(polygon, inputPoint, tolerance);
			if (res == 0)
			{
				return com.epl.geometry.PolygonUtils.PiPResult.PiPOutside;
			}
			if (res == 1)
			{
				return com.epl.geometry.PolygonUtils.PiPResult.PiPInside;
			}
			return com.epl.geometry.PolygonUtils.PiPResult.PiPBoundary;
		}

		internal static com.epl.geometry.PolygonUtils.PiPResult IsPointInPolygon2D(com.epl.geometry.Polygon polygon, double inputPointXVal, double inputPointYVal, double tolerance)
		{
			int res = com.epl.geometry.PointInPolygonHelper.IsPointInPolygon(polygon, inputPointXVal, inputPointYVal, tolerance);
			if (res == 0)
			{
				return com.epl.geometry.PolygonUtils.PiPResult.PiPOutside;
			}
			if (res == 1)
			{
				return com.epl.geometry.PolygonUtils.PiPResult.PiPInside;
			}
			return com.epl.geometry.PolygonUtils.PiPResult.PiPBoundary;
		}

		/// <summary>Tests if Point is inside the Polygon's ring.</summary>
		/// <remarks>
		/// Tests if Point is inside the Polygon's ring. Returns PiPOutside if not in
		/// ring, PiPInside if in the ring, PiPBoundary is if on the border. It tests
		/// border only if the tolerance is &gt; 0, otherwise PiPBoundary cannot be
		/// returned. Note: If the tolerance is not 0, the test is more expensive
		/// because it calculates closest distance from a point to each segment.
		/// O(n) complexity, where n is the number of ring segments.
		/// </remarks>
		public static com.epl.geometry.PolygonUtils.PiPResult IsPointInRing2D(com.epl.geometry.Polygon polygon, int iRing, com.epl.geometry.Point2D inputPoint, double tolerance)
		{
			com.epl.geometry.MultiPathImpl polygonImpl = (com.epl.geometry.MultiPathImpl)polygon._getImpl();
			int res = com.epl.geometry.PointInPolygonHelper.IsPointInRing(polygonImpl, iRing, inputPoint, tolerance, null);
			if (res == 0)
			{
				return com.epl.geometry.PolygonUtils.PiPResult.PiPOutside;
			}
			if (res == 1)
			{
				return com.epl.geometry.PolygonUtils.PiPResult.PiPInside;
			}
			// return PiPResult.PiPBoundary;
			return com.epl.geometry.PolygonUtils.PiPResult.PiPInside;
		}

		// we do not return PiPBoundary. Overwise,
		// we would have to do more complex
		// calculations to differentiat between
		// internal and external boundaries.
		/// <summary>Tests if Point is inside of the any outer ring of a Polygon.</summary>
		/// <remarks>
		/// Tests if Point is inside of the any outer ring of a Polygon. Returns
		/// PiPOutside if not in any outer ring, PiPInside if in the any outer ring,
		/// or on the boundary. PiPBoundary is never returned. Note: If the tolerance
		/// is not 0, the test is more expensive because it calculates closest
		/// distance from a point to each segment.
		/// O(n) complexity, where n is the number of polygon segments.
		/// </remarks>
		public static com.epl.geometry.PolygonUtils.PiPResult IsPointInAnyOuterRing(com.epl.geometry.Polygon polygon, com.epl.geometry.Point2D inputPoint, double tolerance)
		{
			int res = com.epl.geometry.PointInPolygonHelper.IsPointInAnyOuterRing(polygon, inputPoint, tolerance);
			if (res == 0)
			{
				return com.epl.geometry.PolygonUtils.PiPResult.PiPOutside;
			}
			if (res == 1)
			{
				return com.epl.geometry.PolygonUtils.PiPResult.PiPInside;
			}
			// return PiPResult.PiPBoundary;
			return com.epl.geometry.PolygonUtils.PiPResult.PiPInside;
		}

		// we do not return PiPBoundary. Overwise,
		// we would have to do more complex
		// calculations to differentiat between
		// internal and external boundaries.
		// #ifndef DOTNET
		// /**
		// *Tests point is inside the Polygon for an array of points.
		// *Returns PiPOutside if not in polygon, PiPInside if in the polygon,
		// PiPBoundary is if on the border.
		// *It tests border only if the tolerance is > 0, otherwise PiPBoundary
		// cannot be returned.
		// *Note: If the tolerance is not 0, the test is more expensive.
		// *
		// *O(n*m) complexity, where n is the number of polygon segments, m is the
		// number of input points.
		// */
		// static void TestPointsInPolygon2D(Polygon polygon, const Point2D*
		// inputPoints, int count, double tolerance, PiPResult testResults)
		// {
		// LOCALREFCLASS2(Array<Point2D>, Point2D*, int, inputPointsArr,
		// const_cast<Point2D*>(inputPoints), count);
		// LOCALREFCLASS2(Array<PolygonUtils::PiPResult>, PolygonUtils::PiPResult*,
		// int, testResultsArr, testResults, count);
		// TestPointsInPolygon2D(polygon, inputPointsArr, count, tolerance,
		// testResultsArr);
		// }
		// #endif
		/// <summary>Tests point is inside the Polygon for an array of points.</summary>
		/// <remarks>
		/// Tests point is inside the Polygon for an array of points. Returns
		/// PiPOutside if not in polygon, PiPInside if in the polygon, PiPBoundary is
		/// if on the border. It tests border only if the tolerance is &gt; 0, otherwise
		/// PiPBoundary cannot be returned. Note: If the tolerance is not 0, the test
		/// is more expensive.
		/// O(n*m) complexity, where n is the number of polygon segments, m is the
		/// number of input points.
		/// </remarks>
		public static void TestPointsInPolygon2D(com.epl.geometry.Polygon polygon, com.epl.geometry.Point2D[] inputPoints, int count, double tolerance, com.epl.geometry.PolygonUtils.PiPResult[] testResults)
		{
			if (inputPoints.Length < count || testResults.Length < count)
			{
				throw new System.ArgumentException();
			}
			// GEOMTHROW(invalid_argument);
			for (int i = 0; i < count; i++)
			{
				testResults[i] = IsPointInPolygon2D(polygon, inputPoints[i], tolerance);
			}
		}

		internal static void TestPointsInPolygon2D(com.epl.geometry.Polygon polygon, double[] xyStreamBuffer, int pointCount, double tolerance, com.epl.geometry.PolygonUtils.PiPResult[] testResults)
		{
			if (xyStreamBuffer.Length / 2 < pointCount || testResults.Length < pointCount)
			{
				throw new System.ArgumentException();
			}
			// GEOMTHROW(invalid_argument);
			for (int i = 0; i < pointCount; i++)
			{
				testResults[i] = IsPointInPolygon2D(polygon, xyStreamBuffer[i * 2], xyStreamBuffer[i * 2 + 1], tolerance);
			}
		}

		// public static void testPointsInPolygon2D(Polygon polygon, Geometry geom,
		// int count, double tolerance, PiPResult[] testResults)
		// {
		// if(geom.getType() == Type.Point)
		// {
		//
		// }
		// else if(Geometry.isMultiVertex(geom.getType()))
		// {
		//
		// }
		//
		//
		// if (inputPoints.length < count || testResults.length < count)
		// throw new IllegalArgumentException();//GEOMTHROW(invalid_argument);
		//
		// for (int i = 0; i < count; i++)
		// testResults[i] = isPointInPolygon2D(polygon, inputPoints[i], tolerance);
		// }
		/// <summary>
		/// Tests point is inside an Area Geometry (Envelope, Polygon) for an array
		/// of points.
		/// </summary>
		/// <remarks>
		/// Tests point is inside an Area Geometry (Envelope, Polygon) for an array
		/// of points. Returns PiPOutside if not in area, PiPInside if in the area,
		/// PiPBoundary is if on the border. It tests border only if the tolerance is
		/// &gt; 0, otherwise PiPBoundary cannot be returned. Note: If the tolerance is
		/// not 0, the test is more expensive.
		/// O(n*m) complexity, where n is the number of polygon segments, m is the
		/// number of input points.
		/// </remarks>
		public static void TestPointsInArea2D(com.epl.geometry.Geometry polygon, com.epl.geometry.Point2D[] inputPoints, int count, double tolerance, com.epl.geometry.PolygonUtils.PiPResult[] testResults)
		{
			if (polygon.GetType() == com.epl.geometry.Geometry.Type.Polygon)
			{
				TestPointsInPolygon2D((com.epl.geometry.Polygon)polygon, inputPoints, count, tolerance, testResults);
			}
			else
			{
				if (polygon.GetType() == com.epl.geometry.Geometry.Type.Envelope)
				{
					com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
					((com.epl.geometry.Envelope)polygon).QueryEnvelope2D(env2D);
					_testPointsInEnvelope2D(env2D, inputPoints, count, tolerance, testResults);
				}
				else
				{
					throw new com.epl.geometry.GeometryException("invalid_call");
				}
			}
		}

		// GEOMTHROW(invalid_call);
		public static void TestPointsInArea2D(com.epl.geometry.Geometry polygon, double[] xyStreamBuffer, int count, double tolerance, com.epl.geometry.PolygonUtils.PiPResult[] testResults)
		{
			if (polygon.GetType() == com.epl.geometry.Geometry.Type.Polygon)
			{
				TestPointsInPolygon2D((com.epl.geometry.Polygon)polygon, xyStreamBuffer, count, tolerance, testResults);
			}
			else
			{
				if (polygon.GetType() == com.epl.geometry.Geometry.Type.Envelope)
				{
					com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
					((com.epl.geometry.Envelope)polygon).QueryEnvelope2D(env2D);
					_testPointsInEnvelope2D(env2D, xyStreamBuffer, count, tolerance, testResults);
				}
				else
				{
					throw new com.epl.geometry.GeometryException("invalid_call");
				}
			}
		}

		// GEOMTHROW(invalid_call);
		private static void _testPointsInEnvelope2D(com.epl.geometry.Envelope2D env2D, com.epl.geometry.Point2D[] inputPoints, int count, double tolerance, com.epl.geometry.PolygonUtils.PiPResult[] testResults)
		{
			if (inputPoints.Length < count || testResults.Length < count)
			{
				throw new System.ArgumentException();
			}
			if (env2D.IsEmpty())
			{
				for (int i = 0; i < count; i++)
				{
					testResults[i] = com.epl.geometry.PolygonUtils.PiPResult.PiPOutside;
				}
				return;
			}
			com.epl.geometry.Envelope2D envIn = env2D;
			// note for java port - assignement by value
			envIn.Inflate(-tolerance * 0.5, -tolerance * 0.5);
			com.epl.geometry.Envelope2D envOut = env2D;
			// note for java port - assignement by value
			envOut.Inflate(tolerance * 0.5, tolerance * 0.5);
			for (int i_1 = 0; i_1 < count; i_1++)
			{
				if (envIn.Contains(inputPoints[i_1]))
				{
					testResults[i_1] = com.epl.geometry.PolygonUtils.PiPResult.PiPInside;
				}
				else
				{
					if (!envOut.Contains(inputPoints[i_1]))
					{
						testResults[i_1] = com.epl.geometry.PolygonUtils.PiPResult.PiPOutside;
					}
					else
					{
						testResults[i_1] = com.epl.geometry.PolygonUtils.PiPResult.PiPBoundary;
					}
				}
			}
		}

		private static void _testPointsInEnvelope2D(com.epl.geometry.Envelope2D env2D, double[] xyStreamBuffer, int pointCount, double tolerance, com.epl.geometry.PolygonUtils.PiPResult[] testResults)
		{
			if (xyStreamBuffer.Length / 2 < pointCount || testResults.Length < pointCount)
			{
				throw new System.ArgumentException();
			}
			if (env2D.IsEmpty())
			{
				for (int i = 0; i < pointCount; i++)
				{
					testResults[i] = com.epl.geometry.PolygonUtils.PiPResult.PiPOutside;
				}
				return;
			}
			com.epl.geometry.Envelope2D envIn = env2D;
			// note for java port - assignement by value
			envIn.Inflate(-tolerance * 0.5, -tolerance * 0.5);
			com.epl.geometry.Envelope2D envOut = env2D;
			// note for java port - assignement by value
			envOut.Inflate(tolerance * 0.5, tolerance * 0.5);
			for (int i_1 = 0; i_1 < pointCount; i_1++)
			{
				if (envIn.Contains(xyStreamBuffer[i_1 * 2], xyStreamBuffer[i_1 * 2 + 1]))
				{
					testResults[i_1] = com.epl.geometry.PolygonUtils.PiPResult.PiPInside;
				}
				else
				{
					if (!envIn.Contains(xyStreamBuffer[i_1 * 2], xyStreamBuffer[i_1 * 2 + 1]))
					{
						testResults[i_1] = com.epl.geometry.PolygonUtils.PiPResult.PiPOutside;
					}
					else
					{
						testResults[i_1] = com.epl.geometry.PolygonUtils.PiPResult.PiPBoundary;
					}
				}
			}
		}

		internal static void TestPointsOnSegment_(com.epl.geometry.Segment seg, com.epl.geometry.Point2D[] input_points, int count, double tolerance, com.epl.geometry.PolygonUtils.PiPResult[] test_results)
		{
			for (int i = 0; i < count; i++)
			{
				if (seg.IsIntersecting(input_points[i], tolerance))
				{
					test_results[i] = com.epl.geometry.PolygonUtils.PiPResult.PiPBoundary;
				}
				else
				{
					test_results[i] = com.epl.geometry.PolygonUtils.PiPResult.PiPOutside;
				}
			}
		}

		internal static void TestPointsOnPolyline2D_(com.epl.geometry.Polyline poly, com.epl.geometry.Point2D[] input_points, int count, double tolerance, com.epl.geometry.PolygonUtils.PiPResult[] test_results)
		{
			com.epl.geometry.MultiPathImpl mp_impl = (com.epl.geometry.MultiPathImpl)poly._getImpl();
			com.epl.geometry.GeometryAccelerators accel = mp_impl._getAccelerators();
			com.epl.geometry.RasterizedGeometry2D rgeom = null;
			if (accel != null)
			{
				rgeom = accel.GetRasterizedGeometry();
			}
			int pointsLeft = count;
			for (int i = 0; i < count; i++)
			{
				test_results[i] = com.epl.geometry.PolygonUtils.PiPResult.PiPInside;
				// set to impossible value
				if (rgeom != null)
				{
					com.epl.geometry.Point2D input_point = input_points[i];
					com.epl.geometry.RasterizedGeometry2D.HitType hit = rgeom.QueryPointInGeometry(input_point.x, input_point.y);
					if (hit == com.epl.geometry.RasterizedGeometry2D.HitType.Outside)
					{
						test_results[i] = com.epl.geometry.PolygonUtils.PiPResult.PiPOutside;
						pointsLeft--;
					}
				}
			}
			if (pointsLeft != 0)
			{
				com.epl.geometry.SegmentIteratorImpl iter = mp_impl.QuerySegmentIterator();
				while (iter.NextPath() && pointsLeft != 0)
				{
					while (iter.HasNextSegment() && pointsLeft != 0)
					{
						com.epl.geometry.Segment segment = iter.NextSegment();
						for (int i_1 = 0; i_1 < count && pointsLeft != 0; i_1++)
						{
							if (test_results[i_1] == com.epl.geometry.PolygonUtils.PiPResult.PiPInside)
							{
								if (segment.IsIntersecting(input_points[i_1], tolerance))
								{
									test_results[i_1] = com.epl.geometry.PolygonUtils.PiPResult.PiPBoundary;
									pointsLeft--;
								}
							}
						}
					}
				}
			}
			for (int i_2 = 0; i_2 < count; i_2++)
			{
				if (test_results[i_2] == com.epl.geometry.PolygonUtils.PiPResult.PiPInside)
				{
					test_results[i_2] = com.epl.geometry.PolygonUtils.PiPResult.PiPOutside;
				}
			}
		}

		internal static void TestPointsOnLine2D(com.epl.geometry.Geometry line, com.epl.geometry.Point2D[] input_points, int count, double tolerance, com.epl.geometry.PolygonUtils.PiPResult[] test_results)
		{
			com.epl.geometry.Geometry.Type gt = line.GetType();
			if (gt == com.epl.geometry.Geometry.Type.Polyline)
			{
				TestPointsOnPolyline2D_((com.epl.geometry.Polyline)line, input_points, count, tolerance, test_results);
			}
			else
			{
				if (com.epl.geometry.Geometry.IsSegment(gt.Value()))
				{
					TestPointsOnSegment_((com.epl.geometry.Segment)line, input_points, count, tolerance, test_results);
				}
				else
				{
					throw new com.epl.geometry.GeometryException("Invalid call.");
				}
			}
		}
	}
}
