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
using Sharpen;

namespace com.epl.geometry.ogc
{
	/// <summary>OGC Simple Feature Access specification v.1.2.1</summary>
	public abstract class OGCGeometry
	{
		public virtual int Dimension()
		{
			return GetEsriGeometry().GetDimension();
		}

		public virtual int CoordinateDimension()
		{
			int d = 2;
			if (GetEsriGeometry().GetDescription().HasAttribute(com.epl.geometry.VertexDescription.Semantics.M))
			{
				d++;
			}
			if (GetEsriGeometry().GetDescription().HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z))
			{
				d++;
			}
			return d;
		}

		public abstract string GeometryType();

		public virtual int SRID()
		{
			if (esriSR == null)
			{
				return 0;
			}
			return esriSR.GetID();
		}

		public virtual com.epl.geometry.ogc.OGCGeometry Envelope()
		{
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope();
			GetEsriGeometry().QueryEnvelope(env);
			com.epl.geometry.Polygon polygon = new com.epl.geometry.Polygon();
			polygon.AddEnvelope(env, false);
			return new com.epl.geometry.ogc.OGCPolygon(polygon, esriSR);
		}

		public virtual string AsText()
		{
			return com.epl.geometry.GeometryEngine.GeometryToWkt(GetEsriGeometry(), 0);
		}

		public virtual System.IO.MemoryStream AsBinary()
		{
			com.epl.geometry.OperatorExportToWkb op = (com.epl.geometry.OperatorExportToWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkb);
			return op.Execute(0, GetEsriGeometry(), null);
		}

		public virtual string AsGeoJson()
		{
			com.epl.geometry.OperatorExportToGeoJson op = (com.epl.geometry.OperatorExportToGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			return op.Execute(esriSR, GetEsriGeometry());
		}

		internal virtual string AsGeoJsonImpl(int export_flags)
		{
			com.epl.geometry.OperatorExportToGeoJson op = (com.epl.geometry.OperatorExportToGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToGeoJson);
			return op.Execute(export_flags, esriSR, GetEsriGeometry());
		}

		/// <returns>Convert to REST JSON.</returns>
		public virtual string AsJson()
		{
			return com.epl.geometry.GeometryEngine.GeometryToJson(esriSR, GetEsriGeometry());
		}

		public virtual bool IsEmpty()
		{
			return GetEsriGeometry().IsEmpty();
		}

		public virtual double MinZ()
		{
			com.epl.geometry.Envelope1D e = GetEsriGeometry().QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
			return e.vmin;
		}

		public virtual double MaxZ()
		{
			com.epl.geometry.Envelope1D e = GetEsriGeometry().QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0);
			return e.vmax;
		}

		public virtual double MinMeasure()
		{
			com.epl.geometry.Envelope1D e = GetEsriGeometry().QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0);
			return e.vmin;
		}

		public virtual double MaxMeasure()
		{
			com.epl.geometry.Envelope1D e = GetEsriGeometry().QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0);
			return e.vmax;
		}

		/// <summary>
		/// Returns true if this geometric object has no anomalous geometric points,
		/// such as self intersection or self tangency.
		/// </summary>
		/// <remarks>
		/// Returns true if this geometric object has no anomalous geometric points,
		/// such as self intersection or self tangency. See the
		/// "Simple feature access - Part 1" document (OGC 06-103r4) for meaning of
		/// "simple" for each geometry type.
		/// The method has O(n log n) complexity when the input geometry is simple.
		/// For non-simple geometries, it terminates immediately when the first issue
		/// is encountered.
		/// </remarks>
		/// <returns>
		/// True if geometry is simple and false otherwise.
		/// Note: If isSimple is true, then isSimpleRelaxed is true too.
		/// </returns>
		public virtual bool IsSimple()
		{
			return com.epl.geometry.OperatorSimplifyOGC.Local().IsSimpleOGC(GetEsriGeometry(), esriSR, true, null, null);
		}

		/// <summary>Extension method - checks if geometry is simple for Geodatabase.</summary>
		/// <returns>
		/// Returns true if geometry is simple, false otherwise.
		/// Note: If isSimpleRelaxed is true, then isSimple is either true or false. Geodatabase has more relaxed requirements for simple geometries than OGC.
		/// </returns>
		public virtual bool IsSimpleRelaxed()
		{
			com.epl.geometry.OperatorSimplify op = (com.epl.geometry.OperatorSimplify)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Simplify);
			return op.IsSimpleAsFeature(GetEsriGeometry(), esriSR, true, null, null);
		}

		[System.Obsolete]
		public virtual com.epl.geometry.ogc.OGCGeometry MakeSimpleRelaxed(bool forceProcessing)
		{
			return MakeSimpleRelaxed(forceProcessing);
		}

		/// <summary>Makes a simple geometry for Geodatabase.</summary>
		/// <returns>
		/// Returns simplified geometry.
		/// Note: isSimpleRelaxed should return true after this operation.
		/// </returns>
		public virtual com.epl.geometry.ogc.OGCGeometry MakeSimpleRelaxed(bool forceProcessing)
		{
			com.epl.geometry.OperatorSimplify op = (com.epl.geometry.OperatorSimplify)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Simplify);
			return com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(op.Execute(GetEsriGeometry(), esriSR, forceProcessing, null), esriSR);
		}

		/// <summary>Resolves topological issues in this geometry and makes it Simple according to OGC specification.</summary>
		/// <returns>
		/// Returns simplified geometry.
		/// Note: isSimple and isSimpleRelaxed should return true after this operation.
		/// </returns>
		public virtual com.epl.geometry.ogc.OGCGeometry MakeSimple()
		{
			return SimplifyBunch_(GetEsriGeometryCursor());
		}

		public virtual bool Is3D()
		{
			return GetEsriGeometry().GetDescription().HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
		}

		public virtual bool IsMeasured()
		{
			return GetEsriGeometry().GetDescription().HasAttribute(com.epl.geometry.VertexDescription.Semantics.M);
		}

		public abstract com.epl.geometry.ogc.OGCGeometry Boundary();

		/// <summary>OGC equals.</summary>
		/// <remarks>
		/// OGC equals. Performs topological comparison with tolerance.
		/// This is different from equals(Object), that uses exact comparison.
		/// </remarks>
		public virtual bool Equals(com.epl.geometry.ogc.OGCGeometry another)
		{
			if (this == another)
			{
				return true;
			}
			if (another == null)
			{
				return false;
			}
			com.epl.geometry.Geometry geom1 = GetEsriGeometry();
			com.epl.geometry.Geometry geom2 = another.GetEsriGeometry();
			return com.epl.geometry.GeometryEngine.Equals(geom1, geom2, GetEsriSpatialReference());
		}

		[System.Obsolete]
		public virtual bool Equals(com.epl.geometry.ogc.OGCGeometry another)
		{
			return Equals(another);
		}

		public virtual bool Disjoint(com.epl.geometry.ogc.OGCGeometry another)
		{
			com.epl.geometry.Geometry geom1 = GetEsriGeometry();
			com.epl.geometry.Geometry geom2 = another.GetEsriGeometry();
			return com.epl.geometry.GeometryEngine.Disjoint(geom1, geom2, GetEsriSpatialReference());
		}

		public virtual bool Intersects(com.epl.geometry.ogc.OGCGeometry another)
		{
			return !Disjoint(another);
		}

		public virtual bool Touches(com.epl.geometry.ogc.OGCGeometry another)
		{
			com.epl.geometry.Geometry geom1 = GetEsriGeometry();
			com.epl.geometry.Geometry geom2 = another.GetEsriGeometry();
			return com.epl.geometry.GeometryEngine.Touches(geom1, geom2, GetEsriSpatialReference());
		}

		public virtual bool Crosses(com.epl.geometry.ogc.OGCGeometry another)
		{
			com.epl.geometry.Geometry geom1 = GetEsriGeometry();
			com.epl.geometry.Geometry geom2 = another.GetEsriGeometry();
			return com.epl.geometry.GeometryEngine.Crosses(geom1, geom2, GetEsriSpatialReference());
		}

		public virtual bool Within(com.epl.geometry.ogc.OGCGeometry another)
		{
			com.epl.geometry.Geometry geom1 = GetEsriGeometry();
			com.epl.geometry.Geometry geom2 = another.GetEsriGeometry();
			return com.epl.geometry.GeometryEngine.Within(geom1, geom2, GetEsriSpatialReference());
		}

		public virtual bool Contains(com.epl.geometry.ogc.OGCGeometry another)
		{
			com.epl.geometry.Geometry geom1 = GetEsriGeometry();
			com.epl.geometry.Geometry geom2 = another.GetEsriGeometry();
			return com.epl.geometry.GeometryEngine.Contains(geom1, geom2, GetEsriSpatialReference());
		}

		public virtual bool Overlaps(com.epl.geometry.ogc.OGCGeometry another)
		{
			com.epl.geometry.Geometry geom1 = GetEsriGeometry();
			com.epl.geometry.Geometry geom2 = another.GetEsriGeometry();
			return com.epl.geometry.GeometryEngine.Overlaps(geom1, geom2, GetEsriSpatialReference());
		}

		public virtual bool Relate(com.epl.geometry.ogc.OGCGeometry another, string matrix)
		{
			com.epl.geometry.Geometry geom1 = GetEsriGeometry();
			com.epl.geometry.Geometry geom2 = another.GetEsriGeometry();
			return com.epl.geometry.GeometryEngine.Relate(geom1, geom2, GetEsriSpatialReference(), matrix);
		}

		public abstract com.epl.geometry.ogc.OGCGeometry LocateAlong(double mValue);

		public abstract com.epl.geometry.ogc.OGCGeometry LocateBetween(double mStart, double mEnd);

		// analysis
		public virtual double Distance(com.epl.geometry.ogc.OGCGeometry another)
		{
			com.epl.geometry.Geometry geom1 = GetEsriGeometry();
			com.epl.geometry.Geometry geom2 = another.GetEsriGeometry();
			return com.epl.geometry.GeometryEngine.Distance(geom1, geom2, GetEsriSpatialReference());
		}

		// This method firstly groups geometries by dimension (points, lines,
		// areas),
		// then simplifies each group such that each group is reduced to a single
		// geometry.
		// As a result there are at most three geometries, each geometry is Simple.
		// Afterwards
		// it produces a single OGCGeometry.
		private com.epl.geometry.ogc.OGCGeometry SimplifyBunch_(com.epl.geometry.GeometryCursor gc)
		{
			// Combines geometries into multipoint, polyline, and polygon types,
			// simplifying them and unioning them,
			// then produces OGCGeometry from the result.
			// Can produce OGCConcreteGoemetryCollection
			com.epl.geometry.MultiPoint dstMultiPoint = null;
			System.Collections.Generic.List<com.epl.geometry.Geometry> dstPolylines = new System.Collections.Generic.List<com.epl.geometry.Geometry>();
			System.Collections.Generic.List<com.epl.geometry.Geometry> dstPolygons = new System.Collections.Generic.List<com.epl.geometry.Geometry>();
			for (com.epl.geometry.Geometry g = gc.Next(); g != null; g = gc.Next())
			{
				switch (g.GetType())
				{
					case com.epl.geometry.Geometry.Type.Point:
					{
						if (dstMultiPoint == null)
						{
							dstMultiPoint = new com.epl.geometry.MultiPoint();
						}
						dstMultiPoint.Add((com.epl.geometry.Point)g);
						break;
					}

					case com.epl.geometry.Geometry.Type.MultiPoint:
					{
						if (dstMultiPoint == null)
						{
							dstMultiPoint = new com.epl.geometry.MultiPoint();
						}
						dstMultiPoint.Add((com.epl.geometry.MultiPoint)g, 0, -1);
						break;
					}

					case com.epl.geometry.Geometry.Type.Polyline:
					{
						dstPolylines.Add((com.epl.geometry.Polyline)g.Copy());
						break;
					}

					case com.epl.geometry.Geometry.Type.Polygon:
					{
						dstPolygons.Add((com.epl.geometry.Polygon)g.Copy());
						break;
					}

					default:
					{
						throw new System.NotSupportedException();
					}
				}
			}
			System.Collections.Generic.List<com.epl.geometry.Geometry> result = new System.Collections.Generic.List<com.epl.geometry.Geometry>(3);
			if (dstMultiPoint != null)
			{
				com.epl.geometry.Geometry resMP = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(dstMultiPoint, esriSR, true, null);
				result.Add(resMP);
			}
			if (dstPolylines.Count > 0)
			{
				if (dstPolylines.Count == 1)
				{
					com.epl.geometry.Geometry resMP = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(dstPolylines[0], esriSR, true, null);
					result.Add(resMP);
				}
				else
				{
					com.epl.geometry.GeometryCursor res = com.epl.geometry.OperatorUnion.Local().Execute(new com.epl.geometry.SimpleGeometryCursor(dstPolylines), esriSR, null);
					com.epl.geometry.Geometry resPolyline = res.Next();
					com.epl.geometry.Geometry resMP = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(resPolyline, esriSR, true, null);
					result.Add(resMP);
				}
			}
			if (dstPolygons.Count > 0)
			{
				if (dstPolygons.Count == 1)
				{
					com.epl.geometry.Geometry resMP = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(dstPolygons[0], esriSR, true, null);
					result.Add(resMP);
				}
				else
				{
					com.epl.geometry.GeometryCursor res = com.epl.geometry.OperatorUnion.Local().Execute(new com.epl.geometry.SimpleGeometryCursor(dstPolygons), esriSR, null);
					com.epl.geometry.Geometry resPolygon = res.Next();
					com.epl.geometry.Geometry resMP = com.epl.geometry.OperatorSimplifyOGC.Local().Execute(resPolygon, esriSR, true, null);
					result.Add(resMP);
				}
			}
			return com.epl.geometry.ogc.OGCGeometry.CreateFromEsriCursor(new com.epl.geometry.SimpleGeometryCursor(result), esriSR);
		}

		public virtual com.epl.geometry.ogc.OGCGeometry Buffer(double distance)
		{
			com.epl.geometry.OperatorBuffer op = (com.epl.geometry.OperatorBuffer)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Buffer);
			if (distance == 0)
			{
				// when distance is 0, return self (maybe we should
				// create a copy instead).
				return this;
			}
			double[] d = new double[] { distance };
			com.epl.geometry.GeometryCursor cursor = op.Execute(GetEsriGeometryCursor(), GetEsriSpatialReference(), d, true, null);
			return com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(cursor.Next(), esriSR);
		}

		public virtual com.epl.geometry.ogc.OGCGeometry ConvexHull()
		{
			com.epl.geometry.OperatorConvexHull op = (com.epl.geometry.OperatorConvexHull)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ConvexHull);
			com.epl.geometry.GeometryCursor cursor = op.Execute(GetEsriGeometryCursor(), true, null);
			return com.epl.geometry.ogc.OGCGeometry.CreateFromEsriCursor(cursor, esriSR);
		}

		public virtual com.epl.geometry.ogc.OGCGeometry Intersection(com.epl.geometry.ogc.OGCGeometry another)
		{
			com.epl.geometry.OperatorIntersection op = (com.epl.geometry.OperatorIntersection)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Intersection);
			com.epl.geometry.GeometryCursor cursor = op.Execute(GetEsriGeometryCursor(), another.GetEsriGeometryCursor(), GetEsriSpatialReference(), null, 7);
			return com.epl.geometry.ogc.OGCGeometry.CreateFromEsriCursor(cursor, esriSR, true);
		}

		public virtual com.epl.geometry.ogc.OGCGeometry Union(com.epl.geometry.ogc.OGCGeometry another)
		{
			com.epl.geometry.OperatorUnion op = (com.epl.geometry.OperatorUnion)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Union);
			com.epl.geometry.GeometryCursorAppend ap = new com.epl.geometry.GeometryCursorAppend(GetEsriGeometryCursor(), another.GetEsriGeometryCursor());
			com.epl.geometry.GeometryCursor cursor = op.Execute(ap, GetEsriSpatialReference(), null);
			return com.epl.geometry.ogc.OGCGeometry.CreateFromEsriCursor(cursor, esriSR);
		}

		public virtual com.epl.geometry.ogc.OGCGeometry Difference(com.epl.geometry.ogc.OGCGeometry another)
		{
			com.epl.geometry.Geometry geom1 = GetEsriGeometry();
			com.epl.geometry.Geometry geom2 = another.GetEsriGeometry();
			return CreateFromEsriGeometry(com.epl.geometry.GeometryEngine.Difference(geom1, geom2, GetEsriSpatialReference()), esriSR);
		}

		public virtual com.epl.geometry.ogc.OGCGeometry SymDifference(com.epl.geometry.ogc.OGCGeometry another)
		{
			com.epl.geometry.Geometry geom1 = GetEsriGeometry();
			com.epl.geometry.Geometry geom2 = another.GetEsriGeometry();
			return CreateFromEsriGeometry(com.epl.geometry.GeometryEngine.SymmetricDifference(geom1, geom2, GetEsriSpatialReference()), esriSR);
		}

		public abstract com.epl.geometry.Geometry GetEsriGeometry();

		public virtual com.epl.geometry.GeometryCursor GetEsriGeometryCursor()
		{
			return new com.epl.geometry.SimpleGeometryCursor(GetEsriGeometry());
		}

		public virtual com.epl.geometry.SpatialReference GetEsriSpatialReference()
		{
			return esriSR;
		}

		/// <summary>Create an OGCGeometry instance from the GeometryCursor.</summary>
		/// <param name="gc"/>
		/// <param name="sr"/>
		/// <returns>Geometry instance created from the geometry cursor.</returns>
		public static com.epl.geometry.ogc.OGCGeometry CreateFromEsriCursor(com.epl.geometry.GeometryCursor gc, com.epl.geometry.SpatialReference sr)
		{
			return CreateFromEsriCursor(gc, sr, false);
		}

		public static com.epl.geometry.ogc.OGCGeometry CreateFromEsriCursor(com.epl.geometry.GeometryCursor gc, com.epl.geometry.SpatialReference sr, bool skipEmpty)
		{
			System.Collections.Generic.List<com.epl.geometry.ogc.OGCGeometry> geoms = new System.Collections.Generic.List<com.epl.geometry.ogc.OGCGeometry>(10);
			com.epl.geometry.Geometry emptyGeom = null;
			for (com.epl.geometry.Geometry g = gc.Next(); g != null; g = gc.Next())
			{
				emptyGeom = g;
				if (!skipEmpty || !g.IsEmpty())
				{
					geoms.Add(CreateFromEsriGeometry(g, sr));
				}
			}
			if (geoms.Count == 1)
			{
				return geoms[0];
			}
			else
			{
				if (geoms.Count == 0)
				{
					return CreateFromEsriGeometry(emptyGeom, sr);
				}
				else
				{
					return new com.epl.geometry.ogc.OGCConcreteGeometryCollection(geoms, sr);
				}
			}
		}

		public static com.epl.geometry.ogc.OGCGeometry FromText(string text)
		{
			com.epl.geometry.OperatorImportFromWkt op = (com.epl.geometry.OperatorImportFromWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkt);
			com.epl.geometry.OGCStructure ogcStructure = op.ExecuteOGC(0, text, null);
			return com.epl.geometry.ogc.OGCGeometry.CreateFromOGCStructure(ogcStructure, com.epl.geometry.SpatialReference.Create(4326));
		}

		public static com.epl.geometry.ogc.OGCGeometry FromBinary(System.IO.MemoryStream binary)
		{
			com.epl.geometry.OperatorImportFromWkb op = (com.epl.geometry.OperatorImportFromWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkb);
			com.epl.geometry.OGCStructure ogcStructure = op.ExecuteOGC(0, binary, null);
			return com.epl.geometry.ogc.OGCGeometry.CreateFromOGCStructure(ogcStructure, com.epl.geometry.SpatialReference.Create(4326));
		}

		public static com.epl.geometry.ogc.OGCGeometry FromEsriShape(System.IO.MemoryStream buffer)
		{
			com.epl.geometry.OperatorImportFromESRIShape op = (com.epl.geometry.OperatorImportFromESRIShape)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromESRIShape);
			com.epl.geometry.Geometry g = op.Execute(0, com.epl.geometry.Geometry.Type.Unknown, buffer);
			return com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(g, com.epl.geometry.SpatialReference.Create(4326));
		}

		public static com.epl.geometry.ogc.OGCGeometry FromJson(string @string)
		{
			com.epl.geometry.MapGeometry mapGeom = com.epl.geometry.GeometryEngine.JsonToGeometry(com.epl.geometry.JsonParserReader.CreateFromString(@string));
			return com.epl.geometry.ogc.OGCGeometry.CreateFromEsriGeometry(mapGeom.GetGeometry(), mapGeom.GetSpatialReference());
		}

		public static com.epl.geometry.ogc.OGCGeometry FromGeoJson(string @string)
		{
			com.epl.geometry.OperatorImportFromGeoJson op = (com.epl.geometry.OperatorImportFromGeoJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromGeoJson);
			com.epl.geometry.MapOGCStructure mapOGCStructure = op.ExecuteOGC(0, @string, null);
			return com.epl.geometry.ogc.OGCGeometry.CreateFromOGCStructure(mapOGCStructure.m_ogcStructure, mapOGCStructure.m_spatialReference);
		}

		public static com.epl.geometry.ogc.OGCGeometry CreateFromEsriGeometry(com.epl.geometry.Geometry geom, com.epl.geometry.SpatialReference sr)
		{
			return CreateFromEsriGeometry(geom, sr, false);
		}

		public static com.epl.geometry.ogc.OGCGeometry CreateFromEsriGeometry(com.epl.geometry.Geometry geom, com.epl.geometry.SpatialReference sr, bool multiType)
		{
			if (geom == null)
			{
				return null;
			}
			com.epl.geometry.Geometry.Type t = geom.GetType();
			if (t == com.epl.geometry.Geometry.Type.Polygon)
			{
				if (!multiType && ((com.epl.geometry.Polygon)geom).GetExteriorRingCount() == 1)
				{
					return new com.epl.geometry.ogc.OGCPolygon((com.epl.geometry.Polygon)geom, sr);
				}
				else
				{
					return new com.epl.geometry.ogc.OGCMultiPolygon((com.epl.geometry.Polygon)geom, sr);
				}
			}
			if (t == com.epl.geometry.Geometry.Type.Polyline)
			{
				if (!multiType && ((com.epl.geometry.Polyline)geom).GetPathCount() == 1)
				{
					return new com.epl.geometry.ogc.OGCLineString((com.epl.geometry.Polyline)geom, 0, sr);
				}
				else
				{
					return new com.epl.geometry.ogc.OGCMultiLineString((com.epl.geometry.Polyline)geom, sr);
				}
			}
			if (t == com.epl.geometry.Geometry.Type.MultiPoint)
			{
				if (!multiType && ((com.epl.geometry.MultiPoint)geom).GetPointCount() <= 1)
				{
					if (geom.IsEmpty())
					{
						return new com.epl.geometry.ogc.OGCPoint(new com.epl.geometry.Point(), sr);
					}
					else
					{
						return new com.epl.geometry.ogc.OGCPoint(((com.epl.geometry.MultiPoint)geom).GetPoint(0), sr);
					}
				}
				else
				{
					return new com.epl.geometry.ogc.OGCMultiPoint((com.epl.geometry.MultiPoint)geom, sr);
				}
			}
			if (t == com.epl.geometry.Geometry.Type.Point)
			{
				if (!multiType)
				{
					return new com.epl.geometry.ogc.OGCPoint((com.epl.geometry.Point)geom, sr);
				}
				else
				{
					return new com.epl.geometry.ogc.OGCMultiPoint((com.epl.geometry.Point)geom, sr);
				}
			}
			if (t == com.epl.geometry.Geometry.Type.Envelope)
			{
				com.epl.geometry.Polygon p = new com.epl.geometry.Polygon();
				p.AddEnvelope((com.epl.geometry.Envelope)geom, false);
				return CreateFromEsriGeometry(p, sr, multiType);
			}
			throw new System.NotSupportedException();
		}

		public static com.epl.geometry.ogc.OGCGeometry CreateFromOGCStructure(com.epl.geometry.OGCStructure ogcStructure, com.epl.geometry.SpatialReference sr)
		{
			System.Collections.Generic.List<com.epl.geometry.ogc.OGCConcreteGeometryCollection> collectionStack = new System.Collections.Generic.List<com.epl.geometry.ogc.OGCConcreteGeometryCollection>(0);
			System.Collections.Generic.List<com.epl.geometry.OGCStructure> structureStack = new System.Collections.Generic.List<com.epl.geometry.OGCStructure>(0);
			System.Collections.Generic.List<int> indices = new System.Collections.Generic.List<int>(0);
			com.epl.geometry.ogc.OGCGeometry[] geometries = new com.epl.geometry.ogc.OGCGeometry[1];
			com.epl.geometry.ogc.OGCConcreteGeometryCollection root = new com.epl.geometry.ogc.OGCConcreteGeometryCollection(java.util.Arrays.AsList(geometries), sr);
			structureStack.Add(ogcStructure);
			collectionStack.Add(root);
			indices.Add(0);
			while (!structureStack.IsEmpty())
			{
				com.epl.geometry.OGCStructure lastStructure = structureStack[structureStack.Count - 1];
				if (indices[indices.Count - 1] == lastStructure.m_structures.Count)
				{
					structureStack.Remove(structureStack.Count - 1);
					collectionStack.Remove(collectionStack.Count - 1);
					indices.Remove(indices.Count - 1);
					continue;
				}
				com.epl.geometry.ogc.OGCConcreteGeometryCollection lastCollection = collectionStack[collectionStack.Count - 1];
				com.epl.geometry.ogc.OGCGeometry g;
				int i = indices[indices.Count - 1];
				int type = lastStructure.m_structures[i].m_type;
				switch (type)
				{
					case 1:
					{
						g = new com.epl.geometry.ogc.OGCPoint((com.epl.geometry.Point)lastStructure.m_structures[i].m_geometry, sr);
						lastCollection.geometries[i] = g;
						indices[indices.Count - 1] = i + 1;
						break;
					}

					case 2:
					{
						g = new com.epl.geometry.ogc.OGCLineString((com.epl.geometry.Polyline)lastStructure.m_structures[i].m_geometry, 0, sr);
						lastCollection.geometries[i] = g;
						indices[indices.Count - 1] = i + 1;
						break;
					}

					case 3:
					{
						g = new com.epl.geometry.ogc.OGCPolygon((com.epl.geometry.Polygon)lastStructure.m_structures[i].m_geometry, 0, sr);
						lastCollection.geometries[i] = g;
						indices[indices.Count - 1] = i + 1;
						break;
					}

					case 4:
					{
						g = new com.epl.geometry.ogc.OGCMultiPoint((com.epl.geometry.MultiPoint)lastStructure.m_structures[i].m_geometry, sr);
						lastCollection.geometries[i] = g;
						indices[indices.Count - 1] = i + 1;
						break;
					}

					case 5:
					{
						g = new com.epl.geometry.ogc.OGCMultiLineString((com.epl.geometry.Polyline)lastStructure.m_structures[i].m_geometry, sr);
						lastCollection.geometries[i] = g;
						indices[indices.Count - 1] = i + 1;
						break;
					}

					case 6:
					{
						g = new com.epl.geometry.ogc.OGCMultiPolygon((com.epl.geometry.Polygon)lastStructure.m_structures[i].m_geometry, sr);
						lastCollection.geometries[i] = g;
						indices[indices.Count - 1] = i + 1;
						break;
					}

					case 7:
					{
						geometries = new com.epl.geometry.ogc.OGCGeometry[lastStructure.m_structures[i].m_structures.Count];
						g = new com.epl.geometry.ogc.OGCConcreteGeometryCollection(java.util.Arrays.AsList(geometries), sr);
						lastCollection.geometries[i] = g;
						indices[indices.Count - 1] = i + 1;
						structureStack.Add(lastStructure.m_structures[i]);
						collectionStack.Add((com.epl.geometry.ogc.OGCConcreteGeometryCollection)g);
						indices.Add(0);
						break;
					}

					default:
					{
						throw new System.NotSupportedException();
					}
				}
			}
			return root.geometries[0];
		}

		protected internal virtual bool IsConcreteGeometryCollection()
		{
			return false;
		}

		/// <summary>SpatialReference of the Geometry.</summary>
		public com.epl.geometry.SpatialReference esriSR;

		public virtual void SetSpatialReference(com.epl.geometry.SpatialReference esriSR_)
		{
			esriSR = esriSR_;
		}

		/// <summary>
		/// Converts this Geometry to the OGCMulti* if it is not OGCMulti* or
		/// OGCGeometryCollection already.
		/// </summary>
		/// <returns>OGCMulti* or OGCGeometryCollection instance.</returns>
		public abstract com.epl.geometry.ogc.OGCGeometry ConvertToMulti();

		public override string ToString()
		{
			string snippet = AsText();
			if (snippet.Length > 200)
			{
				snippet = snippet.Substring(0, 197 - 0) + "...";
			}
			return string.Format("%s: %s", this.GetType().FullName, snippet);
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (other.GetType() != GetType())
			{
				return false;
			}
			com.epl.geometry.ogc.OGCGeometry another = (com.epl.geometry.ogc.OGCGeometry)other;
			com.epl.geometry.Geometry geom1 = GetEsriGeometry();
			com.epl.geometry.Geometry geom2 = another.GetEsriGeometry();
			if (geom1 == null)
			{
				if (geom2 != null)
				{
					return false;
				}
			}
			else
			{
				if (!geom1.Equals(geom2))
				{
					return false;
				}
			}
			if (esriSR == another.esriSR)
			{
				return true;
			}
			if (esriSR != null && another.esriSR != null)
			{
				return esriSR.Equals(another.esriSR);
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hash = 1;
			com.epl.geometry.Geometry geom1 = GetEsriGeometry();
			if (geom1 != null)
			{
				hash = geom1.GetHashCode();
			}
			if (esriSR != null)
			{
				hash = com.epl.geometry.NumberUtils.HashCombine(hash, esriSR.GetHashCode());
			}
			return hash;
		}
	}
}
