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
using System.Linq;

namespace com.epl.geometry
{
	internal sealed class OperatorUnionCursor : com.epl.geometry.GeometryCursor
	{
		private com.epl.geometry.GeometryCursor m_inputGeoms;

		private com.epl.geometry.ProgressTracker m_progress_tracker;

		private com.epl.geometry.SpatialReferenceImpl m_spatial_reference;

		private int m_index = -1;

		private bool m_b_done = false;

		private bool[] m_had_geometry = new bool[4];

		private int[] m_dim_geom_counts = new int[4];

		private bool m_b_union_all_dimensions = false;

		private int m_max_dimension = -1;

		private int m_added_geoms = 0;

		private int m_current_dim = -1;

		internal sealed class Geom_pair
		{
			internal void Init()
			{
				geom = null;
				vertex_count = -1;
				unioned = false;
			}

			internal com.epl.geometry.Geometry geom;

			internal int vertex_count;

			internal bool unioned;
			//true if geometry is a result of union operation
		}

		internal sealed class Bin_type
		{
			internal int bin_vertex_count = 0;

			internal System.Collections.Generic.List<com.epl.geometry.OperatorUnionCursor.Geom_pair> geometries = new System.Collections.Generic.List<com.epl.geometry.OperatorUnionCursor.Geom_pair>();

			//bin array and the total vertex count in the bin
			internal void Add_pair(com.epl.geometry.OperatorUnionCursor.Geom_pair geom)
			{
				bin_vertex_count += geom.vertex_count;
				geometries.Add(geom);
			}

			internal void Pop_pair()
			{
				bin_vertex_count -= geometries[geometries.Count - 1].vertex_count;
				geometries.RemoveAt(geometries.Count - 1);
			}

			internal com.epl.geometry.OperatorUnionCursor.Geom_pair Back_pair()
			{
				return geometries[geometries.Count - 1];
			}

			internal int Geom_count()
			{
				return geometries.Count;
			}
		}

		internal System.Collections.Generic.List<System.Collections.Generic.SortedDictionary<int, com.epl.geometry.OperatorUnionCursor.Bin_type>> m_union_bins = new System.Collections.Generic.List<System.Collections.Generic.SortedDictionary<int, com.epl.geometry.OperatorUnionCursor.Bin_type>>();

		internal OperatorUnionCursor(com.epl.geometry.GeometryCursor inputGeoms1, com.epl.geometry.SpatialReference sr, com.epl.geometry.ProgressTracker progress_tracker)
		{
			//for each dimension there is a list of bins sorted by level
			m_inputGeoms = inputGeoms1;
			m_spatial_reference = (com.epl.geometry.SpatialReferenceImpl)(sr);
			m_progress_tracker = progress_tracker;
		}

		private com.epl.geometry.Geometry Get_result_geometry(int dim)
		{
			System.Diagnostics.Debug.Assert((m_dim_geom_counts[dim] > 0));
			System.Collections.Generic.SortedDictionary<int, com.epl.geometry.OperatorUnionCursor.Bin_type> map = m_union_bins[dim];
			System.Collections.Generic.KeyValuePair<int, com.epl.geometry.OperatorUnionCursor.Bin_type> e = map.First();
			com.epl.geometry.OperatorUnionCursor.Bin_type bin = e.Value;
			com.epl.geometry.Geometry resG;
			resG = bin.Back_pair().geom;
			bool unioned = bin.Back_pair().unioned;
			map.Remove(e.Key);
			if (unioned)
			{
				resG = com.epl.geometry.OperatorSimplify.Local().Execute(resG, m_spatial_reference, false, m_progress_tracker);
				if (dim == 0 && resG.GetType() == com.epl.geometry.Geometry.Type.Point)
				{
					// must
					// return
					// multipoint
					// for
					// points
					com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint(resG.GetDescription());
					if (!resG.IsEmpty())
					{
						mp.Add((com.epl.geometry.Point)resG);
					}
					resG = mp;
				}
			}
			return resG;
		}

		public override com.epl.geometry.Geometry Next()
		{
			if (m_b_done && m_current_dim == m_max_dimension)
			{
				return null;
			}
			while (!Step_())
			{
			}
			if (m_max_dimension == -1)
			{
				return null;
			}
			// empty input cursor
			if (m_b_union_all_dimensions)
			{
				m_current_dim++;
				while (true)
				{
					if (m_current_dim > m_max_dimension || m_current_dim < 0)
					{
						throw com.epl.geometry.GeometryException.GeometryInternalError();
					}
					if (m_had_geometry[m_current_dim])
					{
						break;
					}
				}
				m_index++;
				return Get_result_geometry(m_current_dim);
			}
			else
			{
				m_index = 0;
				System.Diagnostics.Debug.Assert((m_max_dimension >= 0));
				m_current_dim = m_max_dimension;
				return Get_result_geometry(m_max_dimension);
			}
		}

		public override int GetGeometryID()
		{
			return m_index;
		}

		private bool Step_()
		{
			if (m_b_done)
			{
				return true;
			}
			com.epl.geometry.Geometry geom = null;
			if (m_inputGeoms != null)
			{
				geom = m_inputGeoms.Next();
				if (geom == null)
				{
					m_b_done = true;
					m_inputGeoms = null;
				}
			}
			com.epl.geometry.ProgressTracker.CheckAndThrow(m_progress_tracker);
			if (geom != null)
			{
				int dim = geom.GetDimension();
				m_had_geometry[dim] = true;
				if (dim >= m_max_dimension && !m_b_union_all_dimensions)
				{
					Add_geom(dim, false, geom);
					if (dim > m_max_dimension && !m_b_union_all_dimensions)
					{
						//this geometry has higher dimension than the previously processed one
						//Therefore we delete all lower dimensions (unless m_b_union_all_dimensions is true).
						Remove_all_bins_with_lower_dimension(dim);
					}
				}
			}
			//this geometry is skipped
			//geom is null. do nothing
			if (m_added_geoms > 0)
			{
				for (int dim = 0; dim <= m_max_dimension; dim++)
				{
					while (m_dim_geom_counts[dim] > 1)
					{
						System.Collections.Generic.List<com.epl.geometry.Geometry> batch_to_union = Collect_geometries_to_union(dim);
						bool serial_execution = true;
						if (serial_execution)
						{
							if (batch_to_union.Count != 0)
							{
								com.epl.geometry.Geometry geomRes = com.epl.geometry.TopologicalOperations.DissolveDirty(batch_to_union, m_spatial_reference, m_progress_tracker);
								Add_geom(dim, true, geomRes);
							}
							else
							{
								break;
							}
						}
					}
				}
			}
			return m_b_done;
		}

		internal System.Collections.Generic.List<com.epl.geometry.Geometry> Collect_geometries_to_union(int dim)
		{
			System.Collections.Generic.List<com.epl.geometry.Geometry> batch_to_union = new System.Collections.Generic.List<com.epl.geometry.Geometry>();
			System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, com.epl.geometry.OperatorUnionCursor.Bin_type>> entriesToRemove = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int, com.epl.geometry.OperatorUnionCursor.Bin_type>>();
			System.Collections.Generic.SortedDictionary<int, com.epl.geometry.OperatorUnionCursor.Bin_type> set = m_union_bins[dim];
			foreach (System.Collections.Generic.KeyValuePair<int, com.epl.geometry.OperatorUnionCursor.Bin_type> e in set)
			{
				//int level = e.getKey();
				com.epl.geometry.OperatorUnionCursor.Bin_type bin = e.Value;
				int binVertexThreshold = 10000;
				if (m_b_done || (bin.bin_vertex_count > binVertexThreshold && bin.Geom_count() > 1))
				{
					m_dim_geom_counts[dim] -= bin.Geom_count();
					m_added_geoms -= bin.Geom_count();
					while (bin.geometries.Count > 0)
					{
						// empty geometries will be unioned too.
						batch_to_union.Add(bin.Back_pair().geom);
						bin.Pop_pair();
					}
					entriesToRemove.Add(e);
				}
			}
			entriesToRemove.ForEach(e => set.Remove(e.Key));
			return batch_to_union;
		}

		private void Remove_all_bins_with_lower_dimension(int dim)
		{
			// this geometry has higher dimension than the previously processed one
			for (int i = 0; i < dim; i++)
			{
				m_union_bins[i].Clear();
				m_added_geoms -= m_dim_geom_counts[i];
				m_dim_geom_counts[i] = 0;
			}
		}

		private void Add_geom(int dimension, bool unioned, com.epl.geometry.Geometry geom)
		{
			com.epl.geometry.OperatorUnionCursor.Geom_pair pair = new com.epl.geometry.OperatorUnionCursor.Geom_pair();
			pair.Init();
			pair.geom = geom;
			int sz = Get_vertex_count_(geom);
			pair.vertex_count = sz;
			int level = Get_level_(sz);
			if (dimension + 1 > (int)m_union_bins.Count)
			{
				for (int i = 0, n = System.Math.Max(2, dimension + 1); i < n; i++)
				{
					m_union_bins.Add(new System.Collections.Generic.SortedDictionary<int, com.epl.geometry.OperatorUnionCursor.Bin_type>());
				}
			}
			//com.epl.geometry.OperatorUnionCursor.Bin_type bin = m_union_bins[dimension][level];
			//return null if level is abscent
			com.epl.geometry.OperatorUnionCursor.Bin_type bin;
         if (!m_union_bins[dimension].TryGetValue(level, out bin))
			{
				bin = new com.epl.geometry.OperatorUnionCursor.Bin_type();
				m_union_bins[dimension][level] = bin;
			}
			pair.unioned = unioned;
			bin.Add_pair(pair);
			// Update global cursor state
			m_dim_geom_counts[dimension]++;
			m_added_geoms++;
			m_max_dimension = System.Math.Max(m_max_dimension, dimension);
		}

		private static int Get_level_(int sz)
		{
			// calculates logarithm of sz to base
			// 4.
			return sz > 0 ? (int)(System.Math.Log((double)sz) / System.Math.Log(4.0) + 0.5) : (int)0;
		}

		private static int Get_vertex_count_(com.epl.geometry.Geometry geom)
		{
			int gt = geom.GetType().Value();
			if (com.epl.geometry.Geometry.IsMultiVertex(gt))
			{
				return ((com.epl.geometry.MultiVertexGeometry)geom).GetPointCount();
			}
			else
			{
				if (gt == com.epl.geometry.Geometry.GeometryType.Point)
				{
					return 1;
				}
				else
				{
					if (gt == com.epl.geometry.Geometry.GeometryType.Envelope)
					{
						return 4;
					}
					else
					{
						if (com.epl.geometry.Geometry.IsSegment(gt))
						{
							return 2;
						}
						else
						{
							throw com.epl.geometry.GeometryException.GeometryInternalError();
						}
					}
				}
			}
		}

		public override bool Tock()
		{
			return Step_();
		}
	}
}
