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
	internal sealed class CrackAndCluster
	{
		private com.epl.geometry.EditShape m_shape = null;

		private com.epl.geometry.ProgressTracker m_progressTracker = null;

		private double m_tolerance;

		private bool m_filter_degenerate_segments = true;

		private CrackAndCluster(com.epl.geometry.ProgressTracker progressTracker)
		{
			//Implementation of the cracking and clustering algorithm.
			//Cracks and clusters all segments and vertices in the EditShape.
			m_progressTracker = progressTracker;
		}

		internal static bool Non_empty_points_need_to_cluster(double tolerance, com.epl.geometry.Point pt1, com.epl.geometry.Point pt2)
		{
			double tolerance_for_clustering = com.epl.geometry.InternalUtils.Adjust_tolerance_for_TE_clustering(tolerance);
			return com.epl.geometry.Clusterer.IsClusterCandidate_(pt1.GetX(), pt1.GetY(), pt2.GetX(), pt2.GetY(), com.epl.geometry.MathUtils.Sqr(tolerance_for_clustering));
		}

		internal static com.epl.geometry.Point Cluster_non_empty_points(com.epl.geometry.Point pt1, com.epl.geometry.Point pt2, double w1, int rank1, double w2, int rank2)
		{
			if (rank1 > rank2)
			{
				return pt1;
			}
			else
			{
				if (rank2 < rank1)
				{
					return pt2;
				}
			}
			int[] rank = null;
			double[] w = null;
			com.epl.geometry.Point pt = new com.epl.geometry.Point();
			com.epl.geometry.Clusterer.MergeVertices(pt1, pt2, w1, rank1, w2, rank2, pt, w, rank);
			return pt;
		}

		public static bool Execute(com.epl.geometry.EditShape shape, double tolerance, com.epl.geometry.ProgressTracker progressTracker, bool filter_degenerate_segments)
		{
			com.epl.geometry.CrackAndCluster cracker = new com.epl.geometry.CrackAndCluster(progressTracker);
			cracker.m_shape = shape;
			cracker.m_tolerance = tolerance;
			cracker.m_filter_degenerate_segments = filter_degenerate_segments;
			return cracker._do();
		}

		private bool _cluster(double toleranceCluster)
		{
			bool res = com.epl.geometry.Clusterer.ExecuteNonReciprocal(m_shape, toleranceCluster);
			return res;
		}

		private bool _crack(double tolerance_for_cracking)
		{
			bool res = com.epl.geometry.Cracker.Execute(m_shape, tolerance_for_cracking, m_progressTracker);
			return res;
		}

		private bool _do()
		{
			double tol = m_tolerance;
			// Use same tolerances as ArcObjects (2 * sqrt(2) * tolerance for
			// clustering)
			// sqrt(2) * tolerance for cracking.
			// Also, inflate the tolerances slightly to insure the simplified result
			// would not change after small rounding issues.
			double c_factor = 1e-5;
			double c_factor_for_needs_cracking = 1e-6;
			double tolerance_for_clustering = com.epl.geometry.InternalUtils.Adjust_tolerance_for_TE_clustering(tol);
			double tolerance_for_needs_cracking = com.epl.geometry.InternalUtils.Adjust_tolerance_for_TE_cracking(tol);
			double tolerance_for_cracking = tolerance_for_needs_cracking * (1.0 + c_factor);
			tolerance_for_needs_cracking *= (1.0 + c_factor_for_needs_cracking);
			// Require tolerance_for_clustering > tolerance_for_cracking >
			// tolerance_for_needs_cracking
			System.Diagnostics.Debug.Assert((tolerance_for_clustering > tolerance_for_cracking));
			System.Diagnostics.Debug.Assert((tolerance_for_cracking > tolerance_for_needs_cracking));
			// double toleranceCluster = m_tolerance * Math.sqrt(2.0) * 1.00001;
			bool bChanged = false;
			int max_iter = m_shape.GetTotalPointCount() + 10 > 30 ? 1000 : (m_shape.GetTotalPointCount() + 10) * (m_shape.GetTotalPointCount() + 10);
			int iter = 0;
			bool has_point_features = m_shape.HasPointFeatures();
			for (; ; iter++)
			{
				if (iter > max_iter)
				{
					throw new com.epl.geometry.GeometryException("Internal Error: max number of iterations exceeded");
				}
				// too
				// many
				// iterations
				bool bClustered = _cluster(tolerance_for_clustering);
				// find
				// close
				// vertices and
				// clamp them
				// together.
				bChanged |= bClustered;
				if (m_filter_degenerate_segments)
				{
					bool bFiltered = (m_shape.FilterClosePoints(tolerance_for_clustering, true, false) != 0);
					// remove all
					// degenerate
					// segments.
					bChanged |= bFiltered;
				}
				bool b_cracked = false;
				if (iter == 0 || has_point_features || com.epl.geometry.Cracker.NeedsCracking(true, m_shape, tolerance_for_needs_cracking, null, m_progressTracker))
				{
					// Cracks only if shape contains segments.
					b_cracked = _crack(tolerance_for_cracking);
					// crack all
					// segments at
					// intersection
					// points and touch
					// points. If
					// Cracked, then the
					// iteration will be
					// repeated.
					bChanged |= b_cracked;
				}
				if (!b_cracked)
				{
					break;
				}
				// was not cracked, so we can bail out.
				// Loop while cracking happens.
				com.epl.geometry.ProgressTracker.CheckAndThrow(m_progressTracker);
			}
			return bChanged;
		}
	}
}
