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
	/// <summary>A polygon is a collection of one or many interior or exterior rings.</summary>
	[System.Serializable]
	public sealed class Polygon : com.epl.geometry.MultiPath
	{
		private const long serialVersionUID = 2L;

		/// <summary>Creates a polygon.</summary>
		public Polygon()
		{
			// TODO:remove as we use
			// writeReplace and
			// GeometrySerializer
			m_impl = new com.epl.geometry.MultiPathImpl(true);
		}

		internal Polygon(com.epl.geometry.VertexDescription vd)
		{
			m_impl = new com.epl.geometry.MultiPathImpl(true, vd);
		}

		public override com.epl.geometry.Geometry CreateInstance()
		{
			return new com.epl.geometry.Polygon(GetDescription());
		}

		public override int GetDimension()
		{
			return 2;
		}

		public override com.epl.geometry.Geometry.Type GetType()
		{
			return com.epl.geometry.Geometry.Type.Polygon;
		}

		/// <summary>Calculates the ring area for this ring.</summary>
		/// <param name="ringIndex">The index of this ring.</param>
		/// <returns>The ring area for this ring.</returns>
		public double CalculateRingArea2D(int ringIndex)
		{
			return m_impl.CalculateRingArea2D(ringIndex);
		}

		/// <summary>Returns TRUE if the ring is an exterior ring.</summary>
		/// <remarks>
		/// Returns TRUE if the ring is an exterior ring. Valid only for simple
		/// polygons.
		/// </remarks>
		public bool IsExteriorRing(int partIndex)
		{
			return m_impl.IsExteriorRing(partIndex);
		}

		/// <summary>
		/// Returns TRUE when this geometry has exactly same type, properties, and
		/// coordinates as the other geometry.
		/// </summary>
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
			if (((Geometry)other).GetType() != GetType())
			{
				return false;
			}
			return m_impl.Equals(((com.epl.geometry.Polygon)other)._getImpl());
		}

		/// <summary>Returns a hash code value for this polygon.</summary>
		public override int GetHashCode()
		{
			return m_impl.GetHashCode();
		}

		/// <summary>Sets a new vertex for the polygon.</summary>
		/// <param name="i">The index of the new vertex.</param>
		/// <param name="x">The X coordinate for the new vertex.</param>
		/// <param name="y">The Y coordinate for the new vertex.</param>
		public void SetXY(int i, double x, double y)
		{
			m_impl.SetXY(i, x, y);
		}

		public void InterpolateAttributes(int path_index, int from_point_index, int to_point_index)
		{
			m_impl.InterpolateAttributes(path_index, from_point_index, to_point_index);
		}

		public void InterpolateAttributes(int semantics, int path_index, int from_point_index, int to_point_index)
		{
			m_impl.InterpolateAttributesForSemantics(semantics, path_index, from_point_index, to_point_index);
		}

		public int GetExteriorRingCount()
		{
			return m_impl.GetOGCPolygonCount();
		}

		public abstract class FillRule
		{
			/// <summary>odd-even fill rule.</summary>
			/// <remarks>
			/// odd-even fill rule. This is the default value. A point is in the polygon interior if a ray
			/// from this point to infinity crosses odd number of segments of the polygon.
			/// </remarks>
			public const int enumFillRuleOddEven = 0;

			/// <summary>winding fill rule (aka non-zero winding rule).</summary>
			/// <remarks>
			/// winding fill rule (aka non-zero winding rule). A point is in the polygon interior if a winding number is not zero.
			/// To compute a winding number for a point, draw a ray from this point to infinity. If N is the number of times the ray
			/// crosses segments directed up and the M is the number of times it crosses segments directed down,
			/// then the winding number is equal to N-M.
			/// </remarks>
			public const int enumFillRuleWinding = 1;
		}

		public static class FillRuleConstants
		{
		}

		/// <summary>Fill rule for the polygon that defines the interior of the self intersecting polygon.</summary>
		/// <remarks>
		/// Fill rule for the polygon that defines the interior of the self intersecting polygon. It affects the Simplify operation.
		/// Can be use by drawing code to pass around the fill rule of graphic path.
		/// This property is not persisted in any format yet.
		/// See also Polygon.FillRule.
		/// </remarks>
		public void SetFillRule(int rule)
		{
			m_impl.SetFillRule(rule);
		}

		/// <summary>Fill rule for the polygon that defines the interior of the self intersecting polygon.</summary>
		/// <remarks>
		/// Fill rule for the polygon that defines the interior of the self intersecting polygon. It affects the Simplify operation.
		/// Changing the fill rule on the polygon that has no self intersections has no physical effect.
		/// Can be use by drawing code to pass around the fill rule of graphic path.
		/// This property is not persisted in any format yet.
		/// See also Polygon.FillRule.
		/// </remarks>
		public int GetFillRule()
		{
			return m_impl.GetFillRule();
		}
	}
}
