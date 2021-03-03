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
	/// <summary>Finds closest vertices of the Geometry.</summary>
	public abstract class OperatorProximity2D : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.Proximity2D;
		}

		/// <summary>Returns the nearest coordinate on the Geometry to the given input point.</summary>
		/// <param name="geom">The input Geometry.</param>
		/// <param name="inputPoint">The query point.</param>
		/// <param name="bTestPolygonInterior">
		/// When true and geom is a polygon, the function will test if the input_point is inside of the polygon. Points that are
		/// inside of the polygon have zero distance to the polygon. When false, the function will not check if the point is inside of the polygon,
		/// but only determine proximity to the boundary.
		/// </param>
		/// <param name="bCalculateLeftRightSide">
		/// The function will calculate left/right side of polylines or polygons when the parameter is True.
		/// \return Returns the result of proximity calculation. See Proximity_2D_result.
		/// </param>
		public abstract com.epl.geometry.Proximity2DResult GetNearestCoordinate(com.epl.geometry.Geometry geom, com.epl.geometry.Point inputPoint, bool bTestPolygonInterior, bool bCalculateLeftRightSide);

		/// <summary>Returns the nearest coordinate on the Geometry to the given input point.</summary>
		/// <param name="geom">The input Geometry.</param>
		/// <param name="inputPoint">The query point.</param>
		/// <param name="bTestPolygonInterior">
		/// When true and geom is a polygon, the function will test if the input_point is inside of the polygon. Points that are
		/// inside of the polygon have zero distance to the polygon. When false, the function will not check if the point is inside of the polygon,
		/// but only determine proximity to the boundary.
		/// \return Returns the result of proximity calculation. See Proximity_2D_result.
		/// </param>
		public abstract com.epl.geometry.Proximity2DResult GetNearestCoordinate(com.epl.geometry.Geometry geom, com.epl.geometry.Point inputPoint, bool bTestPolygonInterior);

		/// <summary>Returns the nearest vertex of the Geometry to the given input point.</summary>
		public abstract com.epl.geometry.Proximity2DResult GetNearestVertex(com.epl.geometry.Geometry geom, com.epl.geometry.Point inputPoint);

		/// <summary>
		/// Returns vertices of the Geometry that are closer to the given point than
		/// the given radius.
		/// </summary>
		/// <param name="geom">The input Geometry.</param>
		/// <param name="inputPoint">The query point.</param>
		/// <param name="searchRadius">The maximum distance to the query point of the vertices.</param>
		/// <param name="maxVertexCountToReturn">
		/// The maximum vertex count to return. The function returns no
		/// more than this number of vertices.
		/// </param>
		/// <returns>
		/// The array of vertices that are in the given search radius to the
		/// point. The array is sorted by distance to the queryPoint with the
		/// closest point first. When there are more than the
		/// maxVertexCountToReturn vertices to return, it returns the closest
		/// vertices. The array will be empty when geom is empty.
		/// </returns>
		public abstract com.epl.geometry.Proximity2DResult[] GetNearestVertices(com.epl.geometry.Geometry geom, com.epl.geometry.Point inputPoint, double searchRadius, int maxVertexCountToReturn);

		public static com.epl.geometry.OperatorProximity2D Local()
		{
			return (com.epl.geometry.OperatorProximity2D)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Proximity2D);
		}

		internal abstract class ProxResultInfo
		{
			public const int rightSide = unchecked((int)(0x1));
		}

		internal static class ProxResultInfoConstants
		{
		}
	}
}
