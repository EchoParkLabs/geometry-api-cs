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
using System.Linq;

namespace com.epl.geometry
{
	[System.Serializable]
	internal class SpatialReferenceImpl : com.epl.geometry.SpatialReference
	{
		internal const bool no_projection_engine = true;

		public const int c_SULIMIT32 = 2147483645;

		public const long c_SULIMIT64 = 9007199254740990L;

		internal enum Precision
		{
			Integer32,
			Integer64,
			FloatingPoint
		}

		internal int m_userWkid;

		internal int m_userLatestWkid;

		internal int m_userOldestWkid;

		internal string m_userWkt;

//		private static readonly java.util.concurrent.locks.ReentrantLock m_lock = new java.util.concurrent.locks.ReentrantLock();
//
		internal SpatialReferenceImpl()
		{
			// this wkid is provided by user to the create method.
			// a string, the well-known text.
			// public SgCoordRef m_sgCoordRef;
			// TODO If one was going to create member object for locking it would be
			// here.
			m_userWkid = 0;
			m_userLatestWkid = -1;
			m_userOldestWkid = -1;
			m_userWkt = null;
		}

		public override int GetID()
		{
			return m_userWkid;
		}

		internal virtual double GetFalseX()
		{
			return 0;
		}

		internal virtual double GetFalseY()
		{
			return 0;
		}

		internal virtual double GetFalseZ()
		{
			return 0;
		}

		internal virtual double GetFalseM()
		{
			return 0;
		}

		internal virtual double GetGridUnitsXY()
		{
			return 1 / (1.0e-9 * 0.0174532925199433);
		}

		/* getOneDegreeGCSUnit() */
		internal virtual double GetGridUnitsZ()
		{
			return 1 / 0.001;
		}

		internal virtual double GetGridUnitsM()
		{
			return 1 / 0.001;
		}

		internal virtual com.epl.geometry.SpatialReferenceImpl.Precision GetPrecision()
		{
			return com.epl.geometry.SpatialReferenceImpl.Precision.Integer64;
		}

		internal override double GetTolerance(int semantics)
		{
			double tolerance = 0.001;
			if (m_userWkid != 0)
			{
				tolerance = com.epl.geometry.Wkid.Find_tolerance_from_wkid(m_userWkid);
			}
			else
			{
				if (m_userWkt != null)
				{
					tolerance = com.epl.geometry.Wkt.Find_tolerance_from_wkt(m_userWkt);
				}
			}
			return tolerance;
		}

		public virtual void QueryValidCoordinateRange(com.epl.geometry.Envelope2D env2D)
		{
			double delta = 0;
			switch (GetPrecision())
			{
				case com.epl.geometry.SpatialReferenceImpl.Precision.Integer32:
				{
					delta = c_SULIMIT32 / GetGridUnitsXY();
					break;
				}

				case com.epl.geometry.SpatialReferenceImpl.Precision.Integer64:
				{
					delta = c_SULIMIT64 / GetGridUnitsXY();
					break;
				}

				default:
				{
					// TODO
					throw com.epl.geometry.GeometryException.GeometryInternalError();
				}
			}
			env2D.SetCoords(GetFalseX(), GetFalseY(), GetFalseX() + delta, GetFalseY() + delta);
		}

		public virtual bool RequiresReSimplify(com.epl.geometry.SpatialReference dst)
		{
			return dst != this;
		}

		public override string GetText()
		{
			return m_userWkt;
		}

		/// <summary>
		/// Returns the oldest value of the well known ID for the horizontal
		/// coordinate system of the spatial reference.
		/// </summary>
		/// <remarks>
		/// Returns the oldest value of the well known ID for the horizontal
		/// coordinate system of the spatial reference. This ID is used for JSON
		/// serialization. Not public.
		/// </remarks>
		internal override int GetOldID()
		{
			int ID_ = GetID();
			if (m_userOldestWkid != -1)
			{
				return m_userOldestWkid;
			}
			m_userOldestWkid = com.epl.geometry.Wkid.Wkid_to_old(ID_);
			if (m_userOldestWkid != -1)
			{
				return m_userOldestWkid;
			}
			return ID_;
		}

		/// <summary>
		/// Returns the latest value of the well known ID for the horizontal
		/// coordinate system of the spatial reference.
		/// </summary>
		/// <remarks>
		/// Returns the latest value of the well known ID for the horizontal
		/// coordinate system of the spatial reference. This ID is used for JSON
		/// serialization. Not public.
		/// </remarks>
		internal override int GetLatestID()
		{
			int ID_ = GetID();
			if (m_userLatestWkid != -1)
			{
				return m_userLatestWkid;
			}
			m_userLatestWkid = com.epl.geometry.Wkid.Wkid_to_new(ID_);
			if (m_userLatestWkid != -1)
			{
				return m_userLatestWkid;
			}
			return ID_;
		}

		public static com.epl.geometry.SpatialReferenceImpl CreateImpl(int wkid)
		{
			if (wkid <= 0)
			{
				throw new System.ArgumentException("Invalid or unsupported wkid: " + wkid);
			}
			com.epl.geometry.SpatialReferenceImpl spatRef = new com.epl.geometry.SpatialReferenceImpl();
			spatRef.m_userWkid = wkid;
			return spatRef;
		}

		public static com.epl.geometry.SpatialReferenceImpl CreateImpl(string wkt)
		{
			if (wkt == null || wkt.Length == 0)
			{
				throw new System.ArgumentException("Cannot create SpatialReference from null or empty text.");
			}
			com.epl.geometry.SpatialReferenceImpl spatRef = new com.epl.geometry.SpatialReferenceImpl();
			spatRef.m_userWkt = wkt;
			return spatRef;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				return false;
			}
			com.epl.geometry.SpatialReferenceImpl sr = (com.epl.geometry.SpatialReferenceImpl)obj;
			if (m_userWkid != sr.m_userWkid)
			{
				return false;
			}
			if (m_userWkid == 0)
			{
				if (!m_userWkt.Equals(sr.m_userWkt))
				{
					// m_userWkt cannot be null here!
					return false;
				}
			}
			return true;
		}

		internal static double GeodesicDistanceOnWGS84Impl(com.epl.geometry.Point ptFrom, com.epl.geometry.Point ptTo)
		{
			double a = 6378137.0;
			// radius of spheroid for WGS_1984
			double e2 = 0.0066943799901413165;
			// ellipticity for WGS_1984
			double rpu = System.Math.PI / 180.0;
			com.epl.geometry.PeDouble answer = new com.epl.geometry.PeDouble();
			com.epl.geometry.GeoDist.Geodesic_distance_ngs(a, e2, ptFrom.GetX() * rpu, ptFrom.GetY() * rpu, ptTo.GetX() * rpu, ptTo.GetY() * rpu, answer, null, null);
			return answer.val;
		}

		public virtual string GetAuthority()
		{
			int latestWKID = GetLatestID();
			if (latestWKID <= 0)
			{
				return "";
			}
			return GetAuthority_(latestWKID);
		}

		private string GetAuthority_(int latestWKID)
		{
			string authority;
			if (latestWKID >= 1024 && latestWKID <= 32767)
			{
				int index = m_esri_codes.ToList().BinarySearch(latestWKID);
				if (index >= 0)
				{
					authority = "ESRI";
				}
				else
				{
					authority = "EPSG";
				}
			}
			else
			{
				authority = "ESRI";
			}
			return authority;
		}

		private static readonly int[] m_esri_codes = new int[] { 2181, 2182, 2183, 2184, 2185, 2186, 2187, 4305, 4812, 20002, 20003, 20062, 20063, 24721, 26761, 26762, 26763, 26764, 26765, 26788, 26789, 26790, 30591, 30592, 31491, 31492, 31493, 31494, 31495, 32059, 32060 };

		// ED_1950_Turkey_9
		// ED_1950_Turkey_10
		// ED_1950_Turkey_11
		// ED_1950_Turkey_12
		// ED_1950_Turkey_13
		// ED_1950_Turkey_14
		// ED_1950_Turkey_15
		// GCS_Voirol_Unifie_1960
		// GCS_Voirol_Unifie_1960_Paris
		// Pulkovo_1995_GK_Zone_2
		// Pulkovo_1995_GK_Zone_3
		// Pulkovo_1995_GK_Zone_2N
		// Pulkovo_1995_GK_Zone_3N
		// La_Canoa_UTM_Zone_21N
		// NAD_1927_StatePlane_Hawaii_1_FIPS_5101
		// NAD_1927_StatePlane_Hawaii_2_FIPS_5102
		// NAD_1927_StatePlane_Hawaii_3_FIPS_5103
		// NAD_1927_StatePlane_Hawaii_4_FIPS_5104
		// NAD_1927_StatePlane_Hawaii_5_FIPS_5105
		// NAD_1927_StatePlane_Michigan_North_FIPS_2111
		// NAD_1927_StatePlane_Michigan_Central_FIPS_2112
		// NAD_1927_StatePlane_Michigan_South_FIPS_2113
		// Nord_Algerie
		// Sud_Algerie
		// Germany_Zone_1
		// Germany_Zone_2
		// Germany_Zone_3
		// Germany_Zone_4
		// Germany_Zone_5
		// NAD_1927_StatePlane_Puerto_Rico_FIPS_5201
		// NAD_1927_StatePlane_Virgin_Islands_St_Croix_FIPS_5202
		public override int GetHashCode()
		{
			if (m_userWkid != 0)
			{
				return com.epl.geometry.NumberUtils.Hash(m_userWkid);
			}
			return m_userWkt.GetHashCode();
		}
	}
}
