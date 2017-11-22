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
	/// <summary>Interface for operators that act on two geometries to produce a new geometry as result.</summary>
	public interface CombineOperator
	{
		/// <summary>Operation on two geometries, returning a third.</summary>
		/// <remarks>
		/// Operation on two geometries, returning a third. Examples include
		/// Intersection, Difference, and so forth.
		/// </remarks>
		/// <param name="geom1">and geom2 are the geometry instances to be operated on.</param>
		/// <param name="sr">
		/// The spatial reference to get the tolerance value from.
		/// When sr is null, the tolerance is calculated from the input geometries.
		/// </param>
		/// <param name="progressTracker">ProgressTracker instance that is used to cancel the lengthy operation. Can be null.</param>
		/// <returns>
		/// Returns the result geoemtry. In some cases the returned value can point to geom1 or geom2
		/// instance. For example, the OperatorIntersection may return geom2 when it is completely
		/// inside of the geom1.
		/// </returns>
		com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry geom1, com.epl.geometry.Geometry geom2, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progressTracker);
	}
}
