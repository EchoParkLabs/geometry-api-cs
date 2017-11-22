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
	internal class OperatorDensifyByLengthCursor : com.epl.geometry.GeometryCursor
	{
		internal com.epl.geometry.GeometryCursor m_inputGeoms;

		internal double m_maxLength;

		internal int m_index;

		public OperatorDensifyByLengthCursor(com.epl.geometry.GeometryCursor inputGeoms1, double maxLength, com.epl.geometry.ProgressTracker progressTracker)
		{
			// SpatialReferenceImpl m_spatialReference;
			m_index = -1;
			m_inputGeoms = inputGeoms1;
			m_maxLength = maxLength;
		}

		public override int GetGeometryID()
		{
			return m_index;
		}

		public override com.epl.geometry.Geometry Next()
		{
			com.epl.geometry.Geometry geom;
			if ((geom = m_inputGeoms.Next()) != null)
			{
				m_index = m_inputGeoms.GetGeometryID();
				return DensifyByLength(geom);
			}
			return null;
		}

		private com.epl.geometry.Geometry DensifyByLength(com.epl.geometry.Geometry geom)
		{
			if (geom.IsEmpty() || geom.GetDimension() < 1)
			{
				return geom;
			}
			int geometryType = geom.GetType().Value();
			// TODO implement IsMultiPath and remove Polygon and Polyline call to
			// match Native
			// if (Geometry.IsMultiPath(geometryType))
			if (geometryType == com.epl.geometry.Geometry.GeometryType.Polygon)
			{
				return DensifyMultiPath((com.epl.geometry.MultiPath)geom);
			}
			else
			{
				if (com.epl.geometry.Geometry.GeometryType.Polyline == geometryType)
				{
					return DensifyMultiPath((com.epl.geometry.MultiPath)geom);
				}
				else
				{
					if (com.epl.geometry.Geometry.IsSegment(geometryType))
					{
						return DensifySegment((com.epl.geometry.Segment)geom);
					}
					else
					{
						if (geometryType == com.epl.geometry.Geometry.GeometryType.Envelope)
						{
							return DensifyEnvelope((com.epl.geometry.Envelope)geom);
						}
						else
						{
							// TODO fix geometry exception to match native implementation
							throw com.epl.geometry.GeometryException.GeometryInternalError();
						}
					}
				}
			}
		}

		// GEOMTHROW(internal_error);
		// unreachable in java
		// return null;
		private com.epl.geometry.Geometry DensifySegment(com.epl.geometry.Segment geom)
		{
			double length = geom.CalculateLength2D();
			if (length <= m_maxLength)
			{
				return (com.epl.geometry.Geometry)geom;
			}
			com.epl.geometry.Polyline polyline = new com.epl.geometry.Polyline(geom.GetDescription());
			polyline.AddSegment(geom, true);
			return DensifyMultiPath((com.epl.geometry.MultiPath)polyline);
		}

		private com.epl.geometry.Geometry DensifyEnvelope(com.epl.geometry.Envelope geom)
		{
			com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon(geom.GetDescription());
			polygon.AddEnvelope(geom, false);
			com.epl.geometry.Envelope2D env2D = new com.epl.geometry.Envelope2D();
			geom.QueryEnvelope2D(env2D);
			double w = env2D.GetWidth();
			double h = env2D.GetHeight();
			if (w <= m_maxLength && h <= m_maxLength)
			{
				return (com.epl.geometry.Geometry)polygon;
			}
			return DensifyMultiPath((com.epl.geometry.MultiPath)polygon);
		}

		private com.epl.geometry.Geometry DensifyMultiPath(com.epl.geometry.MultiPath geom)
		{
			com.epl.geometry.MultiPath densifiedPoly = (com.epl.geometry.MultiPath)geom.CreateInstance();
			com.epl.geometry.SegmentIterator iter = geom.QuerySegmentIterator();
			while (iter.NextPath())
			{
				bool bStartNewPath = true;
				while (iter.HasNextSegment())
				{
					com.epl.geometry.Segment seg = iter.NextSegment();
					if (seg.GetType().Value() != com.epl.geometry.Geometry.GeometryType.Line)
					{
						throw new com.epl.geometry.GeometryException("not implemented");
					}
					bool bIsClosing = iter.IsClosingSegment();
					double len = seg.CalculateLength2D();
					if (len > m_maxLength)
					{
						// need to split
						double dcount = System.Math.Ceiling(len / m_maxLength);
						com.epl.geometry.Point point = new com.epl.geometry.Point(geom.GetDescription());
						// LOCALREFCLASS1(Point,
						// VertexDescription,
						// point,
						// geom.getDescription());
						if (bStartNewPath)
						{
							bStartNewPath = false;
							seg.QueryStart(point);
							densifiedPoly.StartPath(point);
						}
						double dt = 1.0 / dcount;
						double t = dt;
						for (int i = 0, n = (int)dcount - 1; i < n; i++)
						{
							seg.QueryCoord(t, point);
							densifiedPoly.LineTo(point);
							t += dt;
						}
						if (!bIsClosing)
						{
							seg.QueryEnd(point);
							densifiedPoly.LineTo(point);
						}
						else
						{
							densifiedPoly.ClosePathWithLine();
						}
						bStartNewPath = false;
					}
					else
					{
						if (!bIsClosing)
						{
							densifiedPoly.AddSegment(seg, bStartNewPath);
						}
						else
						{
							densifiedPoly.ClosePathWithLine();
						}
						bStartNewPath = false;
					}
				}
			}
			return (com.epl.geometry.Geometry)densifiedPoly;
		}
	}
}
