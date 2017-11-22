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
	/// <summary>Implementation for the vertex clustering.</summary>
	/// <remarks>
	/// Implementation for the vertex clustering.
	/// Used by the TopoGraph and Simplify.
	/// </remarks>
	internal sealed class Clusterer
	{
		// Clusters vertices of the shape. Returns True, if some vertices were moved
		// (clustered).
		// Uses reciprocal clustering (cluster vertices that are mutual nearest
		// neighbours)
		/*
		* static boolean executeReciprocal(EditShape shape, double tolerance) {
		* Clusterer clusterer = new Clusterer(); clusterer.m_shape = shape;
		* clusterer.m_tolerance = tolerance; clusterer.m_sqr_tolerance = tolerance
		* * tolerance; clusterer.m_cell_size = 2 * tolerance;
		* clusterer.m_inv_cell_size = 1.0 / clusterer.m_cell_size; return
		* clusterer.clusterReciprocal_(); }
		*/
		// Clusters vertices of the shape. Returns True, if some vertices were moved
		// (clustered).
		// Uses non-reciprocal clustering (cluster any vertices that are closer than
		// the tolerance in the first-found-first-clustered order)
		internal static bool ExecuteNonReciprocal(com.epl.geometry.EditShape shape, double tolerance)
		{
			com.epl.geometry.Clusterer clusterer = new com.epl.geometry.Clusterer();
			clusterer.m_shape = shape;
			clusterer.m_tolerance = tolerance;
			clusterer.m_sqr_tolerance = tolerance * tolerance;
			clusterer.m_cell_size = 2 * tolerance;
			clusterer.m_inv_cell_size = 1.0 / clusterer.m_cell_size;
			return clusterer.ClusterNonReciprocal_();
		}

		// Use b_conservative == True for simplify, and False for IsSimple. This
		// makes sure Simplified shape is more robust to transformations.
		internal static bool IsClusterCandidate_(double x_1, double y1, double x2, double y2, double sqr_tolerance)
		{
			double dx = x_1 - x2;
			double dy = y1 - y2;
			return dx * dx + dy * dy <= sqr_tolerance;
		}

		internal com.epl.geometry.Point2D m_origin = new com.epl.geometry.Point2D();

		internal double m_tolerance;

		internal double m_sqr_tolerance;

		internal double m_cell_size;

		internal double m_inv_cell_size;

		internal int[] m_bucket_array = new int[4];

		internal int[] m_bucket_hash = new int[4];

		internal int m_dbg_candidate_check_count = 0;

		internal int m_hash_values = -1;

		internal int m_new_clusters = -1;

		// temporary 4 element array
		// temporary 4 element array
		internal static int HashFunction_(int xi, int yi)
		{
			int h = com.epl.geometry.NumberUtils.Hash(xi);
			return com.epl.geometry.NumberUtils.Hash(h, yi);
		}

		internal sealed class ClusterHashFunction : com.epl.geometry.IndexHashTable.HashFunction
		{
			internal com.epl.geometry.EditShape m_shape;

			internal double m_sqr_tolerance;

			internal double m_inv_cell_size;

			internal com.epl.geometry.Point2D m_origin = new com.epl.geometry.Point2D();

			internal com.epl.geometry.Point2D m_pt = new com.epl.geometry.Point2D();

			internal com.epl.geometry.Point2D m_pt_2 = new com.epl.geometry.Point2D();

			internal int m_hash_values;

			public ClusterHashFunction(Clusterer _enclosing, com.epl.geometry.EditShape shape, com.epl.geometry.Point2D origin, double sqr_tolerance, double inv_cell_size, int hash_values)
			{
				this._enclosing = _enclosing;
				this.m_shape = shape;
				this.m_sqr_tolerance = sqr_tolerance;
				this.m_inv_cell_size = inv_cell_size;
				this.m_origin = origin;
				this.m_hash_values = hash_values;
				this.m_pt.SetNaN();
				this.m_pt_2.SetNaN();
			}

			internal int Calculate_hash(int element)
			{
				return this.Calculate_hash_from_vertex(element);
			}

			internal int Dbg_calculate_hash_from_xy(double x, double y)
			{
				double dx = x - this.m_origin.x;
				int xi = (int)(dx * this.m_inv_cell_size + 0.5);
				double dy = y - this.m_origin.y;
				int yi = (int)(dy * this.m_inv_cell_size + 0.5);
				return com.epl.geometry.Clusterer.HashFunction_(xi, yi);
			}

			internal int Calculate_hash_from_vertex(int vertex)
			{
				this.m_shape.GetXY(vertex, this.m_pt);
				double dx = this.m_pt.x - this.m_origin.x;
				int xi = (int)(dx * this.m_inv_cell_size + 0.5);
				double dy = this.m_pt.y - this.m_origin.y;
				int yi = (int)(dy * this.m_inv_cell_size + 0.5);
				return com.epl.geometry.Clusterer.HashFunction_(xi, yi);
			}

			public override int GetHash(int element)
			{
				return this.m_shape.GetUserIndex(element, this.m_hash_values);
			}

			public override bool Equal(int element_1, int element_2)
			{
				int xyindex_1 = element_1;
				int xyindex_2 = element_2;
				this.m_shape.GetXY(xyindex_1, this.m_pt);
				this.m_shape.GetXY(xyindex_2, this.m_pt_2);
				return com.epl.geometry.Clusterer.IsClusterCandidate_(this.m_pt.x, this.m_pt.y, this.m_pt_2.x, this.m_pt_2.y, this.m_sqr_tolerance);
			}

			public override int GetHash(object element_descriptor)
			{
				// UNUSED
				return 0;
			}

			public override bool Equal(object element_descriptor, int element)
			{
				// UNUSED
				return false;
			}

			private readonly Clusterer _enclosing;
		}

		internal com.epl.geometry.EditShape m_shape;

		internal com.epl.geometry.IndexMultiList m_clusters;

		internal com.epl.geometry.Clusterer.ClusterHashFunction m_hash_function;

		internal com.epl.geometry.IndexHashTable m_hash_table;

		internal class ClusterCandidate
		{
			public int vertex;

			internal double distance;
		}

		internal void GetNearestNeighbourCandidate_(int xyindex, com.epl.geometry.Point2D pointOfInterest, int bucket_ptr, com.epl.geometry.Clusterer.ClusterCandidate candidate)
		{
			candidate.vertex = -1;
			candidate.distance = com.epl.geometry.NumberUtils.DoubleMax();
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			for (int node = bucket_ptr; node != -1; node = m_hash_table.GetNextInBucket(node))
			{
				int xyind = m_hash_table.GetElement(node);
				if (xyindex == xyind)
				{
					continue;
				}
				m_shape.GetXY(xyind, pt);
				if (IsClusterCandidate_(pointOfInterest.x, pointOfInterest.y, pt.x, pt.y, m_sqr_tolerance))
				{
					pt.Sub(pointOfInterest);
					double l = pt.Length();
					if (l < candidate.distance)
					{
						candidate.distance = l;
						candidate.vertex = xyind;
					}
				}
			}
		}

		internal void FindClusterCandidate_(int xyindex, com.epl.geometry.Clusterer.ClusterCandidate candidate)
		{
			com.epl.geometry.Point2D pointOfInterest = new com.epl.geometry.Point2D();
			m_shape.GetXY(xyindex, pointOfInterest);
			double x_0 = pointOfInterest.x - m_origin.x;
			double x = x_0 * m_inv_cell_size;
			double y0 = pointOfInterest.y - m_origin.y;
			double y = y0 * m_inv_cell_size;
			int xi = (int)x;
			int yi = (int)y;
			// find the nearest neighbour in the 4 neigbouring cells.
			candidate.vertex = -1;
			candidate.distance = com.epl.geometry.NumberUtils.DoubleMax();
			com.epl.geometry.Clusterer.ClusterCandidate c = new com.epl.geometry.Clusterer.ClusterCandidate();
			for (int dx = 0; dx <= 1; dx += 1)
			{
				for (int dy = 0; dy <= 1; dy += 1)
				{
					int bucket_ptr = m_hash_table.GetFirstInBucket(HashFunction_(xi + dx, yi + dy));
					if (bucket_ptr != com.epl.geometry.IndexHashTable.NullNode())
					{
						GetNearestNeighbourCandidate_(xyindex, pointOfInterest, bucket_ptr, c);
						if (c.vertex != com.epl.geometry.IndexHashTable.NullNode() && c.distance < candidate.distance)
						{
							candidate = c;
						}
					}
				}
			}
		}

		internal void CollectClusterCandidates_(int xyindex, com.epl.geometry.AttributeStreamOfInt32 candidates)
		{
			com.epl.geometry.Point2D pointOfInterest = new com.epl.geometry.Point2D();
			m_shape.GetXY(xyindex, pointOfInterest);
			double x_0 = pointOfInterest.x - m_origin.x;
			double x = x_0 * m_inv_cell_size;
			double y0 = pointOfInterest.y - m_origin.y;
			double y = y0 * m_inv_cell_size;
			int xi = (int)x;
			int yi = (int)y;
			int bucket_count = 0;
			// find all nearest neighbours in the 4 neigbouring cells.
			// Note, because we check four neighbours, there should be 4 times more
			// bins in the hash table to reduce collision probability in this loop.
			for (int dx = 0; dx <= 1; dx += 1)
			{
				for (int dy = 0; dy <= 1; dy += 1)
				{
					int hash = HashFunction_(xi + dx, yi + dy);
					int bucket_ptr = m_hash_table.GetFirstInBucket(hash);
					if (bucket_ptr != -1)
					{
						// Check if we already have this bucket.
						// There could be a hash collision for neighbouring buckets.
						m_bucket_array[bucket_count] = bucket_ptr;
						m_bucket_hash[bucket_count] = hash;
						bucket_count++;
					}
				}
			}
			// Clear duplicate buckets
			// There could be a hash collision for neighboring buckets.
			for (int j = bucket_count - 1; j >= 1; j--)
			{
				int bucket_ptr = m_bucket_array[j];
				for (int i = j - 1; i >= 0; i--)
				{
					if (bucket_ptr == m_bucket_array[i])
					{
						// hash values for two
						// neighbouring cells have
						// collided.
						m_bucket_hash[i] = -1;
						// forget collided hash
						bucket_count--;
						if (j != bucket_count)
						{
							m_bucket_hash[j] = m_bucket_hash[bucket_count];
							m_bucket_array[j] = m_bucket_array[bucket_count];
						}
						break;
					}
				}
			}
			// duplicate
			for (int i_1 = 0; i_1 < bucket_count; i_1++)
			{
				CollectNearestNeighbourCandidates_(xyindex, m_bucket_hash[i_1], pointOfInterest, m_bucket_array[i_1], candidates);
			}
		}

		internal void CollectNearestNeighbourCandidates_(int xyindex, int hash, com.epl.geometry.Point2D pointOfInterest, int bucket_ptr, com.epl.geometry.AttributeStreamOfInt32 candidates)
		{
			com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
			for (int node = bucket_ptr; node != -1; node = m_hash_table.GetNextInBucket(node))
			{
				int xyind = m_hash_table.GetElement(node);
				if (xyindex == xyind || hash != -1 && m_shape.GetUserIndex(xyind, m_hash_values) != hash)
				{
					continue;
				}
				// processing same vertex, or the bucket hash modulo
				// bin count collides.
				m_shape.GetXY(xyind, pt);
				m_dbg_candidate_check_count++;
				if (IsClusterCandidate_(pointOfInterest.x, pointOfInterest.y, pt.x, pt.y, m_sqr_tolerance))
				{
					candidates.Add(node);
				}
			}
		}

		// note that we add the cluster node
		// instead of the cluster.
		internal bool MergeClusters_(int vertex1, int vertex2, bool update_hash)
		{
			int cluster_1 = m_shape.GetUserIndex(vertex1, m_new_clusters);
			int cluster_2 = m_shape.GetUserIndex(vertex2, m_new_clusters);
			System.Diagnostics.Debug.Assert((cluster_1 != com.epl.geometry.StridedIndexTypeCollection.ImpossibleIndex2()));
			System.Diagnostics.Debug.Assert((cluster_2 != com.epl.geometry.StridedIndexTypeCollection.ImpossibleIndex2()));
			if (cluster_1 == -1)
			{
				cluster_1 = m_clusters.CreateList();
				m_clusters.AddElement(cluster_1, vertex1);
				m_shape.SetUserIndex(vertex1, m_new_clusters, cluster_1);
			}
			if (cluster_2 == -1)
			{
				m_clusters.AddElement(cluster_1, vertex2);
			}
			else
			{
				m_clusters.ConcatenateLists(cluster_1, cluster_2);
			}
			// ensure only single vertex refers to the cluster.
			m_shape.SetUserIndex(vertex2, m_new_clusters, com.epl.geometry.StridedIndexTypeCollection.ImpossibleIndex2());
			// merge cordinates
			bool res = MergeVertices_(vertex1, vertex2);
			if (update_hash)
			{
				int hash = m_hash_function.Calculate_hash_from_vertex(vertex1);
				m_shape.SetUserIndex(vertex1, m_hash_values, hash);
			}
			return res;
		}

		// recalculate coordinates of the vertices by averaging them using weights.
		// return true if the coordinates has changed.
		internal static bool MergeVertices(com.epl.geometry.Point pt_1, com.epl.geometry.Point pt_2, double w_1, int rank_1, double w_2, int rank_2, com.epl.geometry.Point pt_res, double[] w_res, int[] rank_res)
		{
			System.Diagnostics.Debug.Assert((!pt_1.IsEmpty() && !pt_2.IsEmpty()));
			bool res = pt_1.Equals(pt_2);
			if (rank_1 > rank_2)
			{
				pt_res = pt_1;
				if (w_res != null)
				{
					rank_res[0] = rank_1;
					w_res[0] = w_1;
				}
				return res;
			}
			else
			{
				if (rank_2 > rank_1)
				{
					pt_res = pt_2;
					if (w_res != null)
					{
						rank_res[0] = rank_1;
						w_res[0] = w_1;
					}
					return res;
				}
			}
			pt_res = pt_1;
			com.epl.geometry.Point2D pt2d = new com.epl.geometry.Point2D();
			MergeVertices2D(pt_1.GetXY(), pt_2.GetXY(), w_1, rank_1, w_2, rank_2, pt2d, w_res, rank_res);
			pt_res.SetXY(pt2d);
			return res;
		}

		internal static bool MergeVertices2D(com.epl.geometry.Point2D pt_1, com.epl.geometry.Point2D pt_2, double w_1, int rank_1, double w_2, int rank_2, com.epl.geometry.Point2D pt_res, double[] w_res, int[] rank_res)
		{
			double w = w_1 + w_2;
			bool r = false;
			double x = pt_1.x;
			if (pt_1.x != pt_2.x)
			{
				if (rank_1 == rank_2)
				{
					x = (pt_1.x * w_1 + pt_2.x * w_2) / w;
				}
				r = true;
			}
			double y = pt_1.y;
			if (pt_1.y != pt_2.y)
			{
				if (rank_1 == rank_2)
				{
					y = (pt_1.y * w_1 + pt_2.y * w_2) / w;
				}
				r = true;
			}
			if (rank_1 != rank_2)
			{
				if (rank_1 > rank_2)
				{
					if (w_res != null)
					{
						rank_res[0] = rank_1;
						w_res[0] = w_1;
					}
					pt_res = pt_1;
				}
				else
				{
					if (w_res != null)
					{
						rank_res[0] = rank_2;
						w_res[0] = w_2;
					}
					pt_res = pt_2;
				}
			}
			else
			{
				pt_res.SetCoords(x, y);
				if (w_res != null)
				{
					w_res[0] = w;
					rank_res[0] = rank_1;
				}
			}
			return r;
		}

		internal bool MergeVertices_(int vert_1, int vert_2)
		{
			com.epl.geometry.Point2D pt_1 = new com.epl.geometry.Point2D();
			m_shape.GetXY(vert_1, pt_1);
			com.epl.geometry.Point2D pt_2 = new com.epl.geometry.Point2D();
			m_shape.GetXY(vert_2, pt_2);
			double w_1 = m_shape.GetWeight(vert_1);
			double w_2 = m_shape.GetWeight(vert_2);
			double w = w_1 + w_2;
			int r = 0;
			double x = pt_1.x;
			if (pt_1.x != pt_2.x)
			{
				x = (pt_1.x * w_1 + pt_2.x * w_2) / w;
				r++;
			}
			double y = pt_1.y;
			if (pt_1.y != pt_2.y)
			{
				y = (pt_1.y * w_1 + pt_2.y * w_2) / w;
				r++;
			}
			if (r > 0)
			{
				m_shape.SetXY(vert_1, x, y);
			}
			m_shape.SetWeight(vert_1, w);
			return r != 0;
		}

		internal bool ClusterNonReciprocal_()
		{
			int point_count = m_shape.GetTotalPointCount();
			com.epl.geometry.Envelope2D env = m_shape.GetEnvelope2D();
			m_origin = env.GetLowerLeft();
			double dim = System.Math.Max(env.GetHeight(), env.GetWidth());
			double mincell = dim / (com.epl.geometry.NumberUtils.IntMax() - 1);
			if (m_cell_size < mincell)
			{
				m_cell_size = mincell;
				m_inv_cell_size = 1.0 / m_cell_size;
			}
			// This holds clusters.
			m_clusters = new com.epl.geometry.IndexMultiList();
			m_clusters.ReserveLists(m_shape.GetTotalPointCount() / 3 + 1);
			m_clusters.ReserveNodes(m_shape.GetTotalPointCount() / 3 + 1);
			m_hash_values = m_shape.CreateUserIndex();
			m_new_clusters = m_shape.CreateUserIndex();
			// Make the hash table. It serves a purpose of fine grain grid.
			// Make it 25% larger than the 4 times point count to reduce the chance
			// of collision.
			// The 4 times comes from the fact that we check four neighbouring cells
			// in the grid for each point.
			m_hash_function = new com.epl.geometry.Clusterer.ClusterHashFunction(this, m_shape, m_origin, m_sqr_tolerance, m_inv_cell_size, m_hash_values);
			m_hash_table = new com.epl.geometry.IndexHashTable(4 * point_count / 3, m_hash_function);
			m_hash_table.ReserveElements(m_shape.GetTotalPointCount());
			bool b_clustered = false;
			// Go through all vertices stored in the m_shape and put the handles of
			// the vertices into the clusters and the hash table.
			for (int geometry = m_shape.GetFirstGeometry(); geometry != -1; geometry = m_shape.GetNextGeometry(geometry))
			{
				for (int path = m_shape.GetFirstPath(geometry); path != -1; path = m_shape.GetNextPath(path))
				{
					int vertex = m_shape.GetFirstVertex(path);
					for (int index = 0, nindex = m_shape.GetPathSize(path); index < nindex; index++)
					{
						System.Diagnostics.Debug.Assert((vertex != -1));
						int hash = m_hash_function.Calculate_hash_from_vertex(vertex);
						m_shape.SetUserIndex(vertex, m_hash_values, hash);
						m_hash_table.AddElement(vertex, hash);
						// add cluster to the
						// hash table
						System.Diagnostics.Debug.Assert((m_shape.GetUserIndex(vertex, m_new_clusters) == -1));
						vertex = m_shape.GetNextVertex(vertex);
					}
				}
			}
			{
				// m_hash_table->dbg_print_bucket_histogram_();
				// scope for candidates array
				com.epl.geometry.AttributeStreamOfInt32 candidates = new com.epl.geometry.AttributeStreamOfInt32(0);
				candidates.Reserve(10);
				for (int geometry_1 = m_shape.GetFirstGeometry(); geometry_1 != -1; geometry_1 = m_shape.GetNextGeometry(geometry_1))
				{
					for (int path = m_shape.GetFirstPath(geometry_1); path != -1; path = m_shape.GetNextPath(path))
					{
						int vertex = m_shape.GetFirstVertex(path);
						for (int index = 0, nindex = m_shape.GetPathSize(path); index < nindex; index++)
						{
							if (m_shape.GetUserIndex(vertex, m_new_clusters) == com.epl.geometry.StridedIndexTypeCollection.ImpossibleIndex2())
							{
								vertex = m_shape.GetNextVertex(vertex);
								continue;
							}
							// this vertex was merged with another
							// cluster. It also was removed from the
							// hash table.
							int hash = m_shape.GetUserIndex(vertex, m_hash_values);
							m_hash_table.DeleteElement(vertex, hash);
							while (true)
							{
								CollectClusterCandidates_(vertex, candidates);
								if (candidates.Size() == 0)
								{
									// no candidate for
									// clustering has
									// been found for
									// the cluster_1.
									break;
								}
								bool clustered = false;
								for (int candidate_index = 0, ncandidates = candidates.Size(); candidate_index < ncandidates; candidate_index++)
								{
									int cluster_node = candidates.Get(candidate_index);
									int other_vertex = m_hash_table.GetElement(cluster_node);
									m_hash_table.DeleteNode(cluster_node);
									clustered |= MergeClusters_(vertex, other_vertex, candidate_index + 1 == ncandidates);
								}
								b_clustered |= clustered;
								candidates.Clear(false);
								// repeat search for the cluster candidates for
								// cluster_1
								if (!clustered)
								{
									break;
								}
							}
							// positions did not change
							// m_shape->set_user_index(vertex, m_new_clusters,
							// Strided_index_type_collection::impossible_index_2());
							vertex = m_shape.GetNextVertex(vertex);
						}
					}
				}
			}
			if (b_clustered)
			{
				ApplyClusterPositions_();
			}
			m_hash_table = null;
			m_hash_function = null;
			m_shape.RemoveUserIndex(m_hash_values);
			m_shape.RemoveUserIndex(m_new_clusters);
			// output_debug_printf("total: %d\n",m_shape->get_total_point_count());
			// output_debug_printf("clustered: %d\n",m_dbg_candidate_check_count);
			return b_clustered;
		}

		internal void ApplyClusterPositions_()
		{
			com.epl.geometry.Point2D cluster_pt = new com.epl.geometry.Point2D();
			// move vertices to the clustered positions.
			for (int list = m_clusters.GetFirstList(); list != -1; list = m_clusters.GetNextList(list))
			{
				int node = m_clusters.GetFirst(list);
				System.Diagnostics.Debug.Assert((node != -1));
				int vertex = m_clusters.GetElement(node);
				m_shape.GetXY(vertex, cluster_pt);
				for (node = m_clusters.GetNext(node); node != -1; node = m_clusters.GetNext(node))
				{
					int vertex_1 = m_clusters.GetElement(node);
					m_shape.SetXY(vertex_1, cluster_pt);
				}
			}
		}

		internal Clusterer()
		{
		}
	}
}
