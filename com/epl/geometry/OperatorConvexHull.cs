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
	/// <summary>Creates the convex hull of the input geometry.</summary>
	public abstract class OperatorConvexHull : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.ConvexHull;
		}

		/// <summary>Calculates the convex hull.</summary>
		/// <param name="geoms">The input geometry cursor.</param>
		/// <param name="progress_tracker">The progress tracker. Allows cancellation of a lengthy operation.</param>
		/// <param name="b_merge">
		/// Put true if you want the convex hull of all the geometries in the cursor combined.
		/// Put false if you want the convex hull of each geometry in the cursor individually.
		/// </param>
		/// <returns>Returns a cursor over result convex hulls.</returns>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor geoms, bool b_merge, com.epl.geometry.ProgressTracker progress_tracker);

		/// <summary>Calculates the convex hull geometry.</summary>
		/// <param name="geom">The input geometry.</param>
		/// <param name="progress_tracker">The progress tracker. Allows cancellation of a lengthy operation.</param>
		/// <returns>
		/// Returns the convex hull.
		/// Point - Returns the same point.
		/// Envelope - returns the same envelope.
		/// MultiPoint - If the point count is one, returns the same multipoint. If the point count is two, returns a polyline of the points. Otherwise, computes and returns the convex hull polygon.
		/// Segment - Returns a polyline consisting of the segment.
		/// Polyline - If consists of only one segment, returns the same polyline. Otherwise, computes and returns the convex hull polygon.
		/// Polygon - If more than one path or if the path isn't already convex, computes and returns the convex hull polygon. Otherwise, returns the same polygon.
		/// </returns>
		public abstract com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry geom, com.epl.geometry.ProgressTracker progress_tracker);

		/// <summary>Checks whether a Geometry is convex.</summary>
		/// <param name="geom">The input geometry to test for convex.</param>
		/// <param name="progress_tracker">The progress tracker.</param>
		/// <returns>Returns true if the geometry is convex.</returns>
		public abstract bool IsConvex(com.epl.geometry.Geometry geom, com.epl.geometry.ProgressTracker progress_tracker);

		public static com.epl.geometry.OperatorConvexHull Local()
		{
			return (com.epl.geometry.OperatorConvexHull)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ConvexHull);
		}
	}
}
