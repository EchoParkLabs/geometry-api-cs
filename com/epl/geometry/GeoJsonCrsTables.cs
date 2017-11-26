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
	internal class GeoJsonCrsTables
	{
		internal static int GetWkidFromCrsShortForm(string crs_identifier)
		{
			int last_colon = crs_identifier.LastIndexOf((int)':');
			// skip version
			if (last_colon == -1)
			{
				return -1;
			}
			int code_start = last_colon + 1;
			int wkid = GetWkidFromCrsCode_(crs_identifier, code_start);
			return wkid;
		}

		internal static int GetWkidFromCrsName(string crs_identifier)
		{
			int wkid = -1;
			int last_colon = crs_identifier.LastIndexOf((int)':');
			// skip
			// authority,
			// version, and
			// other things.
			// Just try to
			// get a wkid.
			// This works
			// for
			// short/long
			// form.
			if (last_colon == -1)
			{
				return -1;
			}
			int code_start = last_colon + 1;
			wkid = GetWkidFromCrsCode_(crs_identifier, code_start);
			if (wkid != -1)
			{
				return wkid;
			}
			wkid = GetWkidFromCrsOgcUrn(crs_identifier);
			// could be an OGC
			// "preferred" urn
			return wkid;
		}

		internal static int GetWkidFromCrsOgcUrn(string crs_identifier)
		{
			int wkid = -1;
			if (crs_identifier.RegionMatches(0, "urn:ogc:def:crs:OGC", 0, 19))
			{
				wkid = GetWkidFromCrsOgcUrn_(crs_identifier);
			}
			return wkid;
		}

		private static int GetWkidFromCrsCode_(string crs_identifier, int code_start)
		{
			System.Diagnostics.Debug.Assert((code_start > 0));
			int wkid = -1;
			int code_count = crs_identifier.Length - code_start;
			try
			{
				wkid = System.Convert.ToInt32(Sharpen.Runtime.Substring(crs_identifier, code_start, code_start + code_count));
			}
			catch (System.Exception)
			{
			}
			return (int)wkid;
		}

		private static int GetWkidFromCrsOgcUrn_(string crs_identifier)
		{
			System.Diagnostics.Debug.Assert((crs_identifier.RegionMatches(0, "urn:ogc:def:crs:OGC", 0, 19)));
			int last_colon = crs_identifier.LastIndexOf((int)':');
			// skip version
			if (last_colon == -1)
			{
				return -1;
			}
			int ogc_code_start = last_colon + 1;
			int ogc_code_count = crs_identifier.Length - ogc_code_start;
			if (crs_identifier.RegionMatches(ogc_code_start, "CRS84", 0, ogc_code_count))
			{
				return 4326;
			}
			if (crs_identifier.RegionMatches(ogc_code_start, "CRS83", 0, ogc_code_count))
			{
				return 4269;
			}
			if (crs_identifier.RegionMatches(ogc_code_start, "CRS27", 0, ogc_code_count))
			{
				return 4267;
			}
			return -1;
		}

		internal static int GetWkidFromCrsHref(string crs_identifier)
		{
			int sr_org_code_start = -1;
			if (crs_identifier.RegionMatches(0, "http://spatialreference.org/ref/epsg/", 0, 37))
			{
				sr_org_code_start = 37;
			}
			else
			{
				if (crs_identifier.RegionMatches(0, "www.spatialreference.org/ref/epsg/", 0, 34))
				{
					sr_org_code_start = 34;
				}
				else
				{
					if (crs_identifier.RegionMatches(0, "http://www.spatialreference.org/ref/epsg/", 0, 41))
					{
						sr_org_code_start = 41;
					}
				}
			}
			if (sr_org_code_start != -1)
			{
				int sr_org_code_end = crs_identifier.IndexOf('/', sr_org_code_start);
				if (sr_org_code_end == -1)
				{
					return -1;
				}
				int count = sr_org_code_end - sr_org_code_start;
				int wkid = -1;
				try
				{
					wkid = System.Convert.ToInt32(Sharpen.Runtime.Substring(crs_identifier, sr_org_code_start, sr_org_code_start + count));
				}
				catch (System.Exception)
				{
				}
				return wkid;
			}
			int open_gis_epsg_slash_end = -1;
			if (crs_identifier.RegionMatches(0, "http://opengis.net/def/crs/EPSG/", 0, 32))
			{
				open_gis_epsg_slash_end = 32;
			}
			else
			{
				if (crs_identifier.RegionMatches(0, "www.opengis.net/def/crs/EPSG/", 0, 29))
				{
					open_gis_epsg_slash_end = 29;
				}
				else
				{
					if (crs_identifier.RegionMatches(0, "http://www.opengis.net/def/crs/EPSG/", 0, 36))
					{
						open_gis_epsg_slash_end = 36;
					}
				}
			}
			if (open_gis_epsg_slash_end != -1)
			{
				int last_slash = crs_identifier.LastIndexOf('/');
				// skip over the
				// "0/"
				if (last_slash == -1)
				{
					return -1;
				}
				int open_gis_code_start = last_slash + 1;
				int count = crs_identifier.Length - open_gis_code_start;
				int wkid = -1;
				try
				{
					wkid = System.Convert.ToInt32(Sharpen.Runtime.Substring(crs_identifier, open_gis_code_start, open_gis_code_start + count));
				}
				catch (System.Exception)
				{
				}
				return wkid;
			}
			if (crs_identifier.CompareToIgnoreCase("http://spatialreference.org/ref/sr-org/6928/ogcwkt/") == 0)
			{
				return 3857;
			}
			return -1;
		}

		internal static string GetWktFromCrsName(string crs_identifier)
		{
			int last_colon = crs_identifier.LastIndexOf((int)':');
			// skip
			// authority
			int wkt_start = last_colon + 1;
			int wkt_count = crs_identifier.Length - wkt_start;
			string wkt = Sharpen.Runtime.Substring(crs_identifier, wkt_start, wkt_start + wkt_count);
			return wkt;
		}
	}
}
