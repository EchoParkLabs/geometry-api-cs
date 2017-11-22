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
	public abstract class OperatorOffset : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.Offset;
		}

		/// <summary>Join types for the offset operation.</summary>
		public enum JoinType
		{
			Round,
			Bevel,
			Miter,
			Square
		}

		/// <summary>Creates offset version of the input geometries.</summary>
		/// <remarks>
		/// Creates offset version of the input geometries.
		/// The offset operation creates a geometry that is a constant distance from
		/// an input polyline or polygon. It is similar to buffering, but produces a
		/// one sided result. If offsetDistance &gt; 0, then the offset geometry is
		/// constructed to the right of the oriented input geometry, otherwise it is
		/// constructed to the left. For a simple polygon, the orientation of outer
		/// rings is clockwise and for inner rings it is counter clockwise. So the
		/// "right side" of a simple polygon is always its inside. The bevelRatio is
		/// multiplied by the offset distance and the result determines how far a
		/// mitered offset intersection can be from the input curve before it is
		/// beveled.
		/// </remarks>
		/// <param name="inputGeometries">
		/// The geometries to calculate offset for. Point and MultiPoint
		/// are not supported.
		/// </param>
		/// <param name="sr">The SpatialReference of the Geometries.</param>
		/// <param name="distance">The offset distance for the Geometries.</param>
		/// <param name="joins">The join type of the offset geometry.</param>
		/// <param name="bevelRatio">
		/// The ratio used to produce a bevel join instead of a miter join
		/// (used only when joins is Miter)
		/// </param>
		/// <param name="flattenError">
		/// The maximum distance of the resulting segments compared to the
		/// true circular arc (used only when joins is Round). If
		/// flattenError is 0, tolerance value is used. Also, the
		/// algorithm never produces more than around 180 vertices for
		/// each round join.
		/// </param>
		/// <returns>Returns the result of the offset operation.</returns>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor inputGeometries, com.epl.geometry.SpatialReference sr, double distance, com.epl.geometry.OperatorOffset.JoinType joins, double bevelRatio, double flattenError, com.epl.geometry.ProgressTracker
			 progressTracker);

		/// <summary>Creates offset version of the input geometry.</summary>
		/// <remarks>
		/// Creates offset version of the input geometry.
		/// The offset operation creates a geometry that is a constant distance from
		/// an input polyline or polygon. It is similar to buffering, but produces a
		/// one sided result. If offsetDistance &gt; 0, then the offset geometry is
		/// constructed to the right of the oriented input geometry, otherwise it is
		/// constructed to the left. For a simple polygon, the orientation of outer
		/// rings is clockwise and for inner rings it is counter clockwise. So the
		/// "right side" of a simple polygon is always its inside. The bevelRatio is
		/// multiplied by the offset distance and the result determines how far a
		/// mitered offset intersection can be from the input curve before it is
		/// beveled.
		/// </remarks>
		/// <param name="inputGeometry">
		/// The geometry to calculate offset for. Point and MultiPoint are
		/// not supported.
		/// </param>
		/// <param name="sr">The SpatialReference of the Geometries.</param>
		/// <param name="distance">The offset distance for the Geometries.</param>
		/// <param name="joins">The join type of the offset geometry.</param>
		/// <param name="bevelRatio">
		/// The ratio used to produce a bevel join instead of a miter join
		/// (used only when joins is Miter)
		/// </param>
		/// <param name="flattenError">
		/// The maximum distance of the resulting segments compared to the
		/// true circular arc (used only when joins is Round). If
		/// flattenError is 0, tolerance value is used. Also, the
		/// algorithm never produces more than around 180 vetices for each
		/// round join.
		/// </param>
		/// <returns>Returns the result of the offset operation.</returns>
		public abstract com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry inputGeometry, com.epl.geometry.SpatialReference sr, double distance, com.epl.geometry.OperatorOffset.JoinType joins, double bevelRatio, double flattenError, com.epl.geometry.ProgressTracker progressTracker
			);

		public static com.epl.geometry.OperatorOffset Local()
		{
			return (com.epl.geometry.OperatorOffset)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Offset);
		}
	}
}
