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
using System;

namespace com.epl.geometry
{
	public sealed class Wkid
	{
		internal static System.Collections.Generic.List<double> ReadTolerances(string resource)
		{
			try
			{
				System.Collections.Generic.List<double> tolerances = new System.Collections.Generic.List<double>();
				//System.IO.TextReader reader = new System.IO.StreamReader(resourceName);
				String[] lines = resource.Split('\n');
				foreach (String line in lines) {
					if (line.Trim().Length == 0) {
						continue;
					}
					int sep = line.IndexOf('\t', 0);
					string id_s = line.Substring(0, sep - 0);
					int tol_index = System.Convert.ToInt32(id_s);
					if (tol_index != tolerances.Count)
					{
						throw new System.ArgumentException("Wkid.readTolerances");
					}
					string tol_val = line.Substring(sep + 1, line.Length - sep - 1);
					tolerances.Add(double.Parse(tol_val));
				}
				return tolerances;
			} catch (System.Exception e) {
				throw new GeometryException(e.Message);
			}
			return null;
		}

		internal static System.Collections.Generic.Dictionary<int, int> ReadToleranceMap(string resource)
		{
			try
			{
				System.Collections.Generic.Dictionary<int, int> hashMap = new System.Collections.Generic.Dictionary<int, int>(600);
				//System.IO.TextReader reader = new System.IO.StreamReader(resourceName);
				String[] lines = resource.Split('\n');
				foreach (String line in lines) {
					if (line.Trim().Length == 0) {
						continue;
					}
					int sep = line.IndexOf('\t', 0);
					string id_s = line.Substring(0, sep - 0);
					int wkid = System.Convert.ToInt32(id_s);
					string id_t = line.Substring(sep + 1, line.Length - sep - 1);
					int tol = System.Convert.ToInt32(id_t);
					hashMap[wkid] = tol;
				}
				return hashMap;
			} catch (System.Exception e) {
				throw new GeometryException(e.Message);
			}
			return null;
		}

		internal static System.Collections.Generic.List<double> m_gcs_tolerances = ReadTolerances(geometry_api_cs.GeometryResources.gcs_tolerances);//"gcs_tolerances.txt");

		internal static System.Collections.Generic.List<double> m_pcs_tolerances = ReadTolerances(geometry_api_cs.GeometryResources.pcs_tolerances);//"pcs_tolerances.txt");

		internal static System.Collections.Generic.Dictionary<int, int> m_gcsToTol = ReadToleranceMap(geometry_api_cs.GeometryResources.gcs_id_to_tolerance);//"gcs_id_to_tolerance.txt");

		internal static System.Collections.Generic.Dictionary<int, int> m_pcsToTol = ReadToleranceMap(geometry_api_cs.GeometryResources.pcs_id_to_tolerance);//"pcs_id_to_tolerance.txt");

		internal static System.Collections.Generic.Dictionary<int, int> m_wkid_to_new;

		internal static System.Collections.Generic.Dictionary<int, int> m_wkid_to_old;

		static Wkid()
		{
			try
			{
				m_wkid_to_new = new System.Collections.Generic.Dictionary<int, int>(100);
				m_wkid_to_old = new System.Collections.Generic.Dictionary<int, int>(100);
				{
					//System.IO.TextReader reader = new System.IO.StreamReader("new_to_old_wkid.txt");
					String resource = geometry_api_cs.GeometryResources.new_to_old_wkid;
//					java.io.BufferedReader reader = new java.io.BufferedReader(new System.IO.StreamReader(input));
					String[] lines = resource.Split('\n');
					foreach (String line in lines) {
						if (line.Trim().Length == 0)
						{
							continue;
						}
						int sep = line.IndexOf('\t', 0);
						string id_s = line.Substring(0, sep - 0);
						int wkid_new = System.Convert.ToInt32(id_s);
						string id_t = line.Substring(sep + 1, line.Length - sep - 1);
						int wkid_old = System.Convert.ToInt32(id_t);
						m_wkid_to_new[wkid_old] = wkid_new;
						m_wkid_to_old[wkid_new] = wkid_old;
					}
				}
				{
					//System.IO.TextReader reader = new System.IO.StreamReader("intermediate_to_old_wkid.txt");
					String resource = geometry_api_cs.GeometryResources.intermediate_to_old_wkid;
//					java.io.BufferedReader reader = new java.io.BufferedReader(new System.IO.StreamReader(input));
					String[] lines = resource.Split('\n');
					foreach (String line in lines) {
						if (line.Trim().Length == 0) {
							continue;
						}
						int sep = line.IndexOf('\t', 0);
						string id_s = line.Substring(0, sep - 0);
						int wkid = System.Convert.ToInt32(id_s);
						string id_t = line.Substring(sep + 1, line.Length - sep - 1);
						int wkid_old = System.Convert.ToInt32(id_t);
						m_wkid_to_old[wkid] = wkid_old;
						m_wkid_to_new[wkid] = m_wkid_to_new[wkid_old];
					}
				}
			}
			catch (System.Exception e)
			{
				throw new GeometryException(e.Message);
			}
		}

		public static double Find_tolerance_from_wkid(int wkid)
		{
			double tol = Find_tolerance_from_wkid_helper(wkid);
			if (tol == 1e38)
			{
				int old = Wkid_to_old(wkid);
				if (old != wkid)
				{
					tol = Find_tolerance_from_wkid_helper(old);
				}
				if (tol == 1e38)
				{
					return 1e-10;
				}
			}
			return tol;
		}

		private static double Find_tolerance_from_wkid_helper(int wkid)
		{
			if (m_gcsToTol.ContainsKey(wkid))
			{
				return m_gcs_tolerances[m_gcsToTol[wkid]];
			}
			if (m_pcsToTol.ContainsKey(wkid))
			{
				return m_pcs_tolerances[m_pcsToTol[wkid]];
			}
			return 1e38;
		}

		public static int Wkid_to_new(int wkid)
		{
			if (m_wkid_to_new.ContainsKey(wkid))
			{
				return m_wkid_to_new[wkid];
			}
			return wkid;
		}

		public static int Wkid_to_old(int wkid)
		{
			if (m_wkid_to_old.ContainsKey(wkid))
			{
				return m_wkid_to_old[wkid];
			}
			return wkid;
		}
	}
}
