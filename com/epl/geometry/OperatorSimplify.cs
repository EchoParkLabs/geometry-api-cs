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
	/// <summary>Simplifies the geometry or determines if the geometry is simple.</summary>
	/// <remarks>
	/// Simplifies the geometry or determines if the geometry is simple. The goal of the OperatorSimplify is to produce a geometry that is
	/// valid for the Geodatabase to store without additional processing.
	/// The Geoprocessing tool CheckGeometries should accept geometries
	/// produced by this operator's execute method. For Polylines the effect of execute is the same as
	/// IPolyline6.NonPlanarSimplify, while for the Polygons and Multipoints it is same as ITopologicalOperator.Simplify.
	/// For the Point class this operator does nothing, and the point is always simple.
	/// The isSimpleAsFeature should return true after the execute method.
	/// See also OperatorSimplifyOGC.
	/// </remarks>
	public abstract class OperatorSimplify : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.Simplify;
		}

		/// <summary>Tests if the Geometry is simple.</summary>
		/// <param name="geom">The Geometry to be tested.</param>
		/// <param name="spatialRef">
		/// Spatial reference from which the tolerance is obtained. Can be null, then a
		/// very small tolerance value is derived from the geometry bounds.
		/// </param>
		/// <param name="bForceTest">When True, the Geometry will be tested regardless of the internal IsKnownSimple flag.</param>
		/// <param name="result">if not null, will contain the results of the check.</param>
		/// <param name="progressTracker">Allows cancellation of a long operation. Can be null.</param>
		public abstract bool IsSimpleAsFeature(com.epl.geometry.Geometry geom, com.epl.geometry.SpatialReference spatialRef, bool bForceTest, com.epl.geometry.NonSimpleResult result, com.epl.geometry.ProgressTracker progressTracker);

		/// <summary>Tests if the Geometry is simple (second call will use a cached IsKnownSimple flag and immediately return).</summary>
		/// <param name="geom">The Geometry to be tested.</param>
		/// <param name="spatialRef">
		/// Spatial reference from which the tolerance is obtained. Can be null, then a
		/// very small tolerance value is derived from the geometry bounds.
		/// </param>
		/// <param name="progressTracker">Allows cancellation of a long operation. Can be null.</param>
		public virtual bool IsSimpleAsFeature(com.epl.geometry.Geometry geom, com.epl.geometry.SpatialReference spatialRef, com.epl.geometry.ProgressTracker progressTracker)
		{
			return IsSimpleAsFeature(geom, spatialRef, false, null, progressTracker);
		}

		/// <summary>Performs the Simplify operation on the geometry cursor.</summary>
		/// <param name="geoms">Geometries to simplify.</param>
		/// <param name="sr">
		/// Spatial reference from which the tolerance is obtained. When null, the tolerance
		/// will be derived individually for each geometry from its bounds.
		/// </param>
		/// <param name="bForceSimplify">When True, the Geometry will be simplified regardless of the internal IsKnownSimple flag.</param>
		/// <param name="progressTracker">Allows cancellation of a long operation. Can be null.</param>
		/// <returns>
		/// Returns a GeometryCursor of simplified geometries.
		/// The isSimpleAsFeature returns true after this method.
		/// </returns>
		public abstract com.epl.geometry.GeometryCursor Execute(com.epl.geometry.GeometryCursor geoms, com.epl.geometry.SpatialReference sr, bool bForceSimplify, com.epl.geometry.ProgressTracker progressTracker);

		/// <summary>Performs the Simplify operation on the geometry.</summary>
		/// <param name="geom">Geometry to simplify.</param>
		/// <param name="sr">
		/// Spatial reference from which the tolerance is obtained. When null, the tolerance
		/// will be derived individually for each geometry from its bounds.
		/// </param>
		/// <param name="bForceSimplify">When True, the Geometry will be simplified regardless of the internal IsKnownSimple flag.</param>
		/// <param name="progressTracker">Allows cancellation of a long operation. Can be null.</param>
		/// <returns>
		/// Returns a simple geometry.
		/// The isSimpleAsFeature returns true after this method.
		/// </returns>
		public abstract com.epl.geometry.Geometry Execute(com.epl.geometry.Geometry geom, com.epl.geometry.SpatialReference sr, bool bForceSimplify, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorSimplify Local()
		{
			return (com.epl.geometry.OperatorSimplify)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Simplify);
		}
	}
}
