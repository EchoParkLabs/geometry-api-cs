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
	public abstract class OperatorBoundary : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.Boundary;
		}

		/// <summary>Calculates the boundary geometry.</summary>
		/// <param name="geoms">The input geometry cursor.</param>
		/// <param name="progress_tracker">The progress tracker, that allows to cancel the lengthy operation.</param>
		/// <returns>Returns a cursor over boundaries for each geometry.</returns>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor geoms, com.epl.geometry.ProgressTracker progress_tracker);

		/// <summary>Calculates the boundary.</summary>
		/// <param name="geom">The input geometry.</param>
		/// <param name="progress_tracker">The progress tracker, that allows to cancel the lengthy operation.</param>
		/// <returns>
		/// Returns the boundary.
		/// For Point - returns an empty point.
		/// For Multi_point - returns an empty point.
		/// For Envelope - returns a polyline, that bounds the envelope.
		/// For Polyline - returns a multipoint, using OGC specification (includes path endpoints, using mod 2 rule).
		/// For Polygon - returns a polyline that bounds the polygon (adds all rings of the polygon to a polyline).
		/// </returns>
		public abstract com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry geom, com.epl.geometry.ProgressTracker progress_tracker);

		public static com.epl.geometry.OperatorBoundary Local()
		{
			return (com.epl.geometry.OperatorBoundary)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Boundary);
		}
	}
}
