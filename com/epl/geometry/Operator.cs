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
	/// <summary>The base class for Geometry Operators.</summary>
	public abstract class Operator
	{
		/// <summary>The operator type enum.</summary>
		public enum Type
		{
			Project,
			ExportToJson,
			ImportFromJson,
			ImportMapGeometryFromJson,
			ExportToESRIShape,
			ImportFromESRIShape,
			Union,
			Difference,
			Proximity2D,
			Relate,
			Equals,
			Disjoint,
			Intersects,
			Within,
			Contains,
			Crosses,
			Touches,
			Overlaps,
			Buffer,
			Distance,
			Intersection,
			Clip,
			Cut,
			DensifyByLength,
			DensifyByAngle,
			LabelPoint,
			GeodesicBuffer,
			GeodeticDensifyByLength,
			ShapePreservingDensify,
			GeodeticLength,
			GeodeticArea,
			Simplify,
			SimplifyOGC,
			Offset,
			Generalize,
			ExportToWkb,
			ImportFromWkb,
			ExportToWkt,
			ImportFromWkt,
			ImportFromGeoJson,
			ExportToGeoJson,
			SymmetricDifference,
			ConvexHull,
			Boundary
		}

		public abstract com.epl.geometry.Operator.Type GetType();

		/// <summary>Processes Geometry to accelerate operations on it.</summary>
		/// <remarks>
		/// Processes Geometry to accelerate operations on it. The Geometry and it's
		/// copies remain accelerated until modified. The acceleration of Geometry
		/// can be a time consuming operation. The accelerated geometry also takes
		/// more memory. Some operators share the same accelerator, some require
		/// a different one. If the accelerator is built for the given parameters,
		/// the method returns immediately.
		/// </remarks>
		/// <param name="geometry">The geometry to be accelerated</param>
		/// <param name="spatialReference">The spatial reference of that geometry</param>
		/// <param name="accelDegree">The acceleration degree for geometry.</param>
		public virtual bool AccelerateGeometry(com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatialReference, com.epl.geometry.Geometry.GeometryAccelerationDegree accelDegree)
		{
			// Override at specific Operator level
			return false;
		}

		/// <summary>Returns true if the geometry can be accelerated.</summary>
		/// <param name="geometry"/>
		/// <returns>
		/// true for geometries that can be accelerated, false for geometries
		/// that cannot
		/// </returns>
		public virtual bool CanAccelerateGeometry(com.epl.geometry.Geometry geometry)
		{
			// Override at specific Operator level
			return false;
		}

		/// <summary>Removes accelerators from given geometry.</summary>
		/// <param name="geometry">The geometry instance to remove accelerators from.</param>
		public static void DeaccelerateGeometry(com.epl.geometry.Geometry geometry)
		{
			com.epl.geometry.Geometry.Type gt = geometry.GetType();
			if (com.epl.geometry.Geometry.IsMultiVertex(gt.Value()))
			{
				com.epl.geometry.GeometryAccelerators accel = ((com.epl.geometry.MultiVertexGeometryImpl)geometry._getImpl())._getAccelerators();
				if (accel != null)
				{
					accel._setRasterizedGeometry(null);
					accel._setQuadTree(null);
				}
			}
		}
	}
}
