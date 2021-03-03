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
	internal class GeometryAccelerators
	{
		private com.epl.geometry.RasterizedGeometry2D m_rasterizedGeometry;

		private com.epl.geometry.QuadTreeImpl m_quad_tree;

		private com.epl.geometry.QuadTreeImpl m_quad_tree_for_paths;

		public virtual com.epl.geometry.RasterizedGeometry2D GetRasterizedGeometry()
		{
			return m_rasterizedGeometry;
		}

		public virtual com.epl.geometry.QuadTreeImpl GetQuadTree()
		{
			return m_quad_tree;
		}

		public virtual com.epl.geometry.QuadTreeImpl GetQuadTreeForPaths()
		{
			return m_quad_tree_for_paths;
		}

		internal virtual void _setRasterizedGeometry(com.epl.geometry.RasterizedGeometry2D rg)
		{
			m_rasterizedGeometry = rg;
		}

		internal virtual void _setQuadTree(com.epl.geometry.QuadTreeImpl quad_tree)
		{
			m_quad_tree = quad_tree;
		}

		internal virtual void _setQuadTreeForPaths(com.epl.geometry.QuadTreeImpl quad_tree)
		{
			m_quad_tree_for_paths = quad_tree;
		}

		internal static bool CanUseRasterizedGeometry(com.epl.geometry.Geometry geom)
		{
			if (geom.IsEmpty() || !(geom.GetType() == com.epl.geometry.Geometry.Type.Polyline || geom.GetType() == com.epl.geometry.Geometry.Type.Polygon))
			{
				return false;
			}
			return true;
		}

		internal static bool CanUseQuadTree(com.epl.geometry.Geometry geom)
		{
			if (geom.IsEmpty() || !(geom.GetType() == com.epl.geometry.Geometry.Type.Polyline || geom.GetType() == com.epl.geometry.Geometry.Type.Polygon))
			{
				return false;
			}
			if (((com.epl.geometry.MultiVertexGeometry)geom).GetPointCount() < 20)
			{
				return false;
			}
			return true;
		}

		internal static bool CanUseQuadTreeForPaths(com.epl.geometry.Geometry geom)
		{
			if (geom.IsEmpty() || !(geom.GetType() == com.epl.geometry.Geometry.Type.Polyline || geom.GetType() == com.epl.geometry.Geometry.Type.Polygon))
			{
				return false;
			}
			if (((com.epl.geometry.MultiVertexGeometry)geom).GetPointCount() < 20)
			{
				return false;
			}
			return true;
		}
	}
}
