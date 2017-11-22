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
	/// <summary>Intersection of geometries by a given geometry.</summary>
	public abstract class OperatorIntersection : com.epl.geometry.Operator, com.epl.geometry.CombineOperator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.Intersection;
		}

		/// <summary>Performs the Topological Intersection operation on the geometry set.</summary>
		/// <param name="inputGeometries">is the set of Geometry instances to be intersected by the intersector.</param>
		/// <param name="intersector">
		/// is the intersector Geometry.
		/// The operator intersects every geometry in the inputGeometries with the first geometry of the intersector and returns the result.
		/// </param>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, com.epl.geometry.GeometryCursor intersector, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker);

		/// <summary>Performs the Topological intersection operation on the geometry set.</summary>
		/// <param name="input_geometries">is the set of Geometry instances to be intersected by the intersector.</param>
		/// <param name="intersector">is the intersector Geometry. Only single intersector is used, therefore, the intersector.next() is called only once.</param>
		/// <param name="sr">
		/// The spatial reference is used to get tolerance value. Can be null, then the tolerance is not used and the operation is performed with
		/// a small tolerance value just enough to make the operation robust.
		/// </param>
		/// <param name="progress_tracker">Allows to cancel the operation. Can be null.</param>
		/// <param name="dimensionMask">
		/// The dimension of the intersection. The value is either -1, or a bitmask mask of values (1 &lt;&lt; dim).
		/// The value of -1 means the lower dimension in the intersecting pair.
		/// This is a fastest option when intersecting polygons with polygons or polylines.
		/// The bitmask of values (1 &lt;&lt; dim), where dim is the desired dimension value, is used to indicate
		/// what dimensions of geometry one wants to be returned. For example, to return
		/// multipoints and lines only, pass (1 &lt;&lt; 0) | (1 &lt;&lt; 1), which is equivalen to 1 | 2, or 3.
		/// </param>
		/// <returns>
		/// Returns the cursor of the intersection result. The cursors' get_geometry_ID method returns the current ID of the input geometry
		/// being processed. Wh dimensionMask is a bitmask, there will be n result geometries per one input geometry returned, where n is the number
		/// of bits set in the bitmask. For example, if the dimensionMask is 5, there will be two geometries per one input geometry.
		/// The operator intersects every geometry in the input_geometries with the first geometry of the intersector and returns the result.
		/// Note, when the dimensionMask is -1, then for each intersected pair of geometries,
		/// the result has the lower of dimentions of the two geometries. That is, the dimension of the Polyline/Polyline intersection
		/// is always 1 (that is, for polylines it never returns crossing points, but the overlaps only).
		/// If dimensionMask is 7, the operation will return any possible
		/// </returns>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor input_geometries, com.epl.geometry.GeometryCursor intersector, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progress_tracker, int dimensionMask);

		/// <summary>Performs the Topological Intersection operation on the geometry.</summary>
		/// <remarks>
		/// Performs the Topological Intersection operation on the geometry.
		/// The result has the lower of dimentions of the two geometries. That is, the dimension of the
		/// Polyline/Polyline intersection is always 1 (that is, for polylines it never returns crossing
		/// points, but the overlaps only).
		/// The call is equivalent to calling the overloaded method using cursors:
		/// execute(new SimpleGeometryCursor(input_geometry), new SimpleGeometryCursor(intersector), sr, progress_tracker, mask).next();
		/// where mask can be either -1 or min(1 &lt;&lt; input_geometry.getDimension(), 1 &lt;&lt; intersector.getDimension());
		/// </remarks>
		/// <param name="inputGeometry">is the Geometry instance to be intersected by the intersector.</param>
		/// <param name="intersector">is the intersector Geometry.</param>
		/// <param name="sr">The spatial reference to get the tolerance value from. Can be null, then the tolerance is calculated from the input geometries.</param>
		/// <returns>Returns the intersected Geometry.</returns>
		public abstract com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry inputGeometry, com.epl.geometry.Geometry intersector, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorIntersection Local()
		{
			return (com.epl.geometry.OperatorIntersection)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Intersection);
		}
	}
}
