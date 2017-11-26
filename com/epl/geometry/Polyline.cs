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
	/// <summary>A polyline is a collection of one or many paths.</summary>
	[System.Serializable]
	public class Polyline : com.epl.geometry.MultiPath
	{
		private const long serialVersionUID = 2L;

		/// <summary>Creates an empty polyline.</summary>
		public Polyline()
		{
			// TODO:remove as we use
			// writeReplace and
			// GeometrySerializer
			m_impl = new com.epl.geometry.MultiPathImpl(false);
		}

		public Polyline(com.epl.geometry.VertexDescription vd)
		{
			m_impl = new com.epl.geometry.MultiPathImpl(false, vd);
		}

		/// <summary>Creates a polyline with one line segment.</summary>
		public Polyline(com.epl.geometry.Point start, com.epl.geometry.Point end)
		{
			m_impl = new com.epl.geometry.MultiPathImpl(false, start.GetDescription());
			StartPath(start);
			LineTo(end);
		}

		public override com.epl.geometry.Geometry CreateInstance()
		{
			return new com.epl.geometry.Polyline(GetDescription());
		}

		public override int GetDimension()
		{
			return 1;
		}

		public override com.epl.geometry.Geometry.Type GetType()
		{
			return com.epl.geometry.Geometry.Type.Polyline;
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
			return m_impl.Equals(((com.epl.geometry.Polyline)other)._getImpl());
		}

		/// <summary>Returns the hash code for the polyline.</summary>
		public override int GetHashCode()
		{
			return m_impl.GetHashCode();
		}

		public override void AddSegment(com.epl.geometry.Segment segment, bool bStartNewPath)
		{
			m_impl.AddSegment(segment, bStartNewPath);
		}

		public virtual void InterpolateAttributes(int from_path_index, int from_point_index, int to_path_index, int to_point_index)
		{
			m_impl.InterpolateAttributes(from_path_index, from_point_index, to_path_index, to_point_index);
		}

		public virtual void InterpolateAttributes(int semantics, int from_path_index, int from_point_index, int to_path_index, int to_point_index)
		{
			m_impl.InterpolateAttributesForSemantics(semantics, from_path_index, from_point_index, to_path_index, to_point_index);
		}
	}
}
