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
	/// <summary>
	/// The MapGeometry class bundles the geometry with its spatial reference
	/// together.
	/// </summary>
	/// <remarks>
	/// The MapGeometry class bundles the geometry with its spatial reference
	/// together. To work with a geometry object in a map it is necessary to have a
	/// spatial reference defined for this geometry.
	/// </remarks>
	[System.Serializable]
	public class MapGeometry
	{
		private const long serialVersionUID = 1L;

		internal com.epl.geometry.Geometry m_geometry = null;

		internal com.epl.geometry.SpatialReference sr = null;

		/// <summary>
		/// Construct a MapGeometry instance using the specified geometry instance
		/// and its corresponding spatial reference.
		/// </summary>
		/// <param name="g">The geometry to construct the new MapGeometry object.</param>
		/// <param name="_sr">The spatial reference of the geometry.</param>
		public MapGeometry(com.epl.geometry.Geometry g, com.epl.geometry.SpatialReference _sr)
		{
			m_geometry = g;
			sr = _sr;
		}

		/// <summary>
		/// Gets the only geometry without the spatial reference from the
		/// MapGeometry.
		/// </summary>
		public virtual com.epl.geometry.Geometry GetGeometry()
		{
			return m_geometry;
		}

		/// <summary>Sets the geometry for this MapGeometry.</summary>
		/// <param name="geometry">The geometry.</param>
		public virtual void SetGeometry(com.epl.geometry.Geometry geometry)
		{
			this.m_geometry = geometry;
		}

		/// <summary>Sets the spatial reference for this MapGeometry.</summary>
		/// <param name="sr">The spatial reference.</param>
		public virtual void SetSpatialReference(com.epl.geometry.SpatialReference sr)
		{
			this.sr = sr;
		}

		/// <summary>Gets the spatial reference for this MapGeometry.</summary>
		public virtual com.epl.geometry.SpatialReference GetSpatialReference()
		{
			return sr;
		}

		/// <summary>The output of this method can be only used for debugging.</summary>
		/// <remarks>The output of this method can be only used for debugging. It is subject to change without notice.</remarks>
//		public override string ToString()
//		{
//			string snippet = com.epl.geometry.OperatorExportToJson.Local().Execute(GetSpatialReference(), GetGeometry());
//			if (snippet.Length > 200)
//			{
//				return snippet.Substring(0, 197 - 0) + "... (" + snippet.Length + " characters)";
//			}
//			else
//			{
//				return snippet;
//			}
//		}

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
			if (other.GetType() != GetType())
			{
				return false;
			}
			com.epl.geometry.MapGeometry omg = (com.epl.geometry.MapGeometry)other;
			com.epl.geometry.SpatialReference sr = GetSpatialReference();
			com.epl.geometry.Geometry g = GetGeometry();
			com.epl.geometry.SpatialReference osr = omg.GetSpatialReference();
			com.epl.geometry.Geometry og = omg.GetGeometry();
			if (sr != osr)
			{
				if (sr == null || !sr.Equals(osr))
				{
					return false;
				}
			}
			if (g != og)
			{
				if (g == null || !g.Equals(og))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			com.epl.geometry.SpatialReference sr = GetSpatialReference();
			com.epl.geometry.Geometry g = GetGeometry();
			int hc = unchecked((int)(0x2937912));
			if (sr != null)
			{
				hc ^= sr.GetHashCode();
			}
			if (g != null)
			{
				hc ^= g.GetHashCode();
			}
			return hc;
		}
	}
}
