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
	/// <summary>Projection of geometries to different coordinate systems.</summary>
	internal abstract class OperatorProject : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.Project;
		}

		/// <summary>Performs the Project operation on a geometry cursor</summary>
		/// <returns>Returns a GeometryCursor.</returns>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeoms, com.epl.geometry.ProjectionTransformation projection, com.epl.geometry.ProgressTracker progressTracker);

		/// <summary>Performs the Project operation on a single geometry instance</summary>
		/// <returns>Returns the Geometry after projection</returns>
		public abstract com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry geometry, com.epl.geometry.ProjectionTransformation projection, com.epl.geometry.ProgressTracker progressTracker);

		/// <summary>Transforms an array of points.</summary>
		/// <remarks>Transforms an array of points. Returns the number of points transformed.</remarks>
		public abstract int Transform(com.epl.geometry.ProjectionTransformation transform, com.epl.geometry.Point[] coordsSrc, int length, com.epl.geometry.Point[] coordsDst);

		/// <summary>Transforms an array of 2D points and returns it.</summary>
		/// <remarks>
		/// Transforms an array of 2D points and returns it. The points are stored in
		/// an interleaved array (x0, y0, x1, y1, x2, y2, ...).
		/// </remarks>
		/// <param name="transform">ProjectionTransformation</param>
		/// <param name="coordsSrc">source coordinates to project.</param>
		/// <param name="pointCount">
		/// the point count in the coordSrc. THere has to be at least
		/// pointCount * 2 elements in the coordsSrc array.
		/// </param>
		/// <returns>projected coordinates in the interleaved form.</returns>
		public abstract double[] Transform(com.epl.geometry.ProjectionTransformation transform, double[] coordsSrc, int pointCount);

		/// <summary>Folds a geometry into the 360 degree range of the associated spatial reference.</summary>
		/// <remarks>
		/// Folds a geometry into the 360 degree range of the associated spatial reference. If the spatial reference be a 'pannable' PCS or GCS. For other spatial types, the function throws an invalid
		/// argument exception. A pannable PCS it a Rectangular PCS where the x coordinate range is equivalent to a 360 degree range on the defining geographic Coordinate System(GCS). If the spatial
		/// reference is a GCS then it is always pannable(default 360 range for spatial reference in GCS coordinates is -180 to 180)
		/// If the geometry is an Envelope fold_into_360_range returns a polygon, unless the Envelope is empty, in which case the empty envelope is returned. The result geometry will be completely inside of
		/// the coordinate system extent. The folding happens where geometry intersects the min or max meridian of the spatial reference and when geometry is completely outside of the min-max meridian range.
		/// Folding does not preserve geodetic area or length. Folding does not preserve perimeter of a polygon.
		/// </remarks>
		/// <param name="geom">The geometry to be folded.</param>
		/// <param name="pannableSR">The pannable Spatial Reference.</param>
		/// <returns>Folded geometry.</returns>
		public abstract com.epl.geometry.Geometry FoldInto360Range(com.epl.geometry.Geometry geom, com.epl.geometry.SpatialReference pannableSR);

		/// <summary>Same as fold_into_360_range.</summary>
		/// <remarks>
		/// Same as fold_into_360_range. The difference is that this function preserves geodetic area of polygons and geodetic length of polylines. It does not preserve regular area and length or perimeter
		/// of polygons. Also, this function might change tangent of the lines at the points of folding.
		/// If the geometry is an Envelope fold_into_360_range returns a polygon, unless the Envelope is empty, in which case the empty envelope is returned. The result geometry will be completely inside of
		/// the coordinate system extent. The folding happens where geometry intersects the min or max meridian of the spatial reference and when geometry is completely outside of the min-max meridian range.
		/// </remarks>
		/// <param name="geom">The geometry to be folded.</param>
		/// <param name="pannableSR">The pannable Spatial Reference.</param>
		/// <param name="curveType">The type of geodetic curve to use to produce vertices at the points of folding. \return Folded geometry.</param>
		public abstract com.epl.geometry.Geometry FoldInto360RangeGeodetic(com.epl.geometry.Geometry geom, com.epl.geometry.SpatialReference pannableSR, int curveType);

		public static com.epl.geometry.OperatorProject Local()
		{
			return (com.epl.geometry.OperatorProject)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Project);
		}
	}
}
