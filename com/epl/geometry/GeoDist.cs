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
	internal sealed class GeoDist
	{
		private const double PE_PI = 3.14159265358979323846264;

		private const double PE_PI2 = 1.57079632679489661923132;

		private const double PE_2PI = 6.283185307179586476925287;

		private const double PE_EPS = 3.55271367880050092935562e-15;

		/// <summary>Get the absolute value of a number</summary>
		private static double PE_ABS(double a)
		{
			return (a < 0) ? -a : a;
		}

		/// <summary>Assign the sign of the second number to the first</summary>
		private static double PE_SGN(double a, double b)
		{
			return (b >= 0) ? PE_ABS(a) : -PE_ABS(a);
		}

		/// <summary>Determine if two doubles are equal within a default tolerance</summary>
		private static bool PE_EQ(double a, double b)
		{
			return (a == b) || PE_ABS(a - b) <= PE_EPS * (1 + (PE_ABS(a) + PE_ABS(b)) / 2);
		}

		/// <summary>Determine if a double is within a given tolerance of zero</summary>
		private static bool PE_ZERO(double a)
		{
			return (a == 0.0) || (PE_ABS(a) <= PE_EPS);
		}

		private static double Lam_delta(double lam)
		{
			double d = System.Math.IEEERemainder(lam, PE_2PI);
			return (PE_ABS(d) <= PE_PI) ? d : ((d < 0) ? d + PE_2PI : d - PE_2PI);
		}

		private static void Lam_phi_reduction(com.epl.geometry.PeDouble p_lam, com.epl.geometry.PeDouble p_phi)
		{
			p_lam.val = Lam_delta(p_lam.val);
			p_phi.val = Lam_delta(p_phi.val);
			if (PE_ABS(p_phi.val) > PE_PI2)
			{
				p_lam.val = Lam_delta(p_lam.val + PE_PI);
				p_phi.val = PE_SGN(PE_PI, p_phi.val) - p_phi.val;
			}
		}

		private static double Q90(double a, double e2)
		{
			/*
			* Rapp // Geometric Geodesy (Part I) // p. 39. Adams, O.S. // Latitude
			* Developments ... // pp. 122-127. Terms extended past n4 by David
			* Burrows, ESRI
			*/
			/* Calculate meridional arc distance from equator to pole */
			/*
			* q90 = a * PE_PI2 * (1 + 1/4 n2 + 1/64 n4 + 1/256 n6 + 25/16384 n8 +
			* 49/65536 n10 + ...)/(1.0 + n)
			*/
			double t = System.Math.Sqrt(1.0 - e2);
			double n = (1.0 - t) / (1.0 + t);
			double n2 = n * n;
			return a / (1.0 + n) * (1.0 + n2 * (1.0 / 4.0 + n2 * (1.0 / 64.0 + n2 * (1.0 / 256.0)))) * PE_PI2;
		}

		public static void Geodesic_distance_ngs(double a, double e2, double lam1, double phi1, double lam2, double phi2, com.epl.geometry.PeDouble p_dist, com.epl.geometry.PeDouble p_az12, com.epl.geometry.PeDouble p_az21)
		{
			/* Highly edited version (plus lots of additions) of NGS FORTRAN code */
			/*
			* inverse for long-line and antipodal cases.* latitudes may be 90
			* degrees exactly.* latitude positive north, longitude positive east,
			* radians.* azimuth clockwise from north, radians.* original programmed
			* by thaddeus vincenty, 1975, 1976* removed back side solution option,
			* debugged, revised -- 2011may01 -- dgm* this version of code is
			* interim -- antipodal boundary needs work
			*
			* * output (besides az12, az21, and dist):* These have been removed
			* from this esri version of the ngs code* it, iteration count* sigma,
			* spherical distance on auxiliary sphere* lam_sph, longitude difference
			* on auxiliary sphere* kind, solution flag: kind=1, long-line; kind=2,
			* antipodal
			*
			*
			* All references to Rapp are Part II
			*/
			double tol = 1.0e-14;
			double eps = 1.0e-15;
			double boa = 0.0;
			double dlam = 0.0;
			double eta1 = 0.0;
			double sin_eta1 = 0.0;
			double cos_eta1 = 0.0;
			double eta2 = 0.0;
			double sin_eta2 = 0.0;
			double cos_eta2 = 0.0;
			double prev = 0.0;
			double test = 0.0;
			double sin_lam_sph = 0.0;
			double cos_lam_sph = 0.0;
			double temp = 0.0;
			double sin_sigma = 0.0;
			double cos_sigma = 0.0;
			double sin_azeq = 0.0;
			double cos2_azeq = 0.0;
			double costm = 0.0;
			double costm2 = 0.0;
			double c = 0.0;
			double d = 0.0;
			double tem1 = 0.0;
			double tem2 = 0.0;
			double ep2 = 0.0;
			double bige = 0.0;
			double bigf = 0.0;
			double biga = 0.0;
			double bigb = 0.0;
			double z = 0.0;
			double dsigma = 0.0;
			bool q_continue_looping;
			double f = 0.0;
			double az12 = 0.0;
			double az21 = 0.0;
			double dist = 0.0;
			double sigma = 0.0;
			double lam_sph = 0.0;
			int it = 0;
			int kind = 0;
			com.epl.geometry.PeDouble lam = new com.epl.geometry.PeDouble();
			com.epl.geometry.PeDouble phi = new com.epl.geometry.PeDouble();
			/* Are there any values to calculate? */
			if (p_dist == null && p_az12 == null && p_az21 == null)
			{
				return;
			}
			/* Normalize point 1 and 2 */
			lam.val = lam1;
			phi.val = phi1;
			Lam_phi_reduction(lam, phi);
			lam1 = lam.val;
			phi1 = phi.val;
			lam.val = lam2;
			phi.val = phi2;
			Lam_phi_reduction(lam, phi);
			lam2 = lam.val;
			phi2 = phi.val;
			dlam = Lam_delta(lam2 - lam1);
			/* longitude difference [-Pi, Pi] */
			if (PE_EQ(phi1, phi2) && (PE_ZERO(dlam) || PE_EQ(PE_ABS(phi1), PE_PI2)))
			{
				/* Check that the points are not the same */
				if (p_dist != null)
				{
					p_dist.val = 0.0;
				}
				if (p_az12 != null)
				{
					p_az12.val = 0.0;
				}
				if (p_az21 != null)
				{
					p_az21.val = 0.0;
				}
				return;
			}
			else
			{
				if (PE_EQ(phi1, -phi2))
				{
					/* Check if they are perfectly antipodal */
					if (PE_EQ(PE_ABS(phi1), PE_PI2))
					{
						/* Check if they are at opposite poles */
						if (p_dist != null)
						{
							p_dist.val = 2.0 * Q90(a, e2);
						}
						if (p_az12 != null)
						{
							p_az12.val = phi1 > 0.0 ? Lam_delta(PE_PI - Lam_delta(lam2)) : Lam_delta(lam2);
						}
						if (p_az21 != null)
						{
							p_az21.val = phi1 > 0.0 ? Lam_delta(lam2) : Lam_delta(PE_PI - Lam_delta(lam2));
						}
						return;
					}
					else
					{
						if (PE_EQ(PE_ABS(dlam), PE_PI))
						{
							/* Other antipodal */
							if (p_dist != null)
							{
								p_dist.val = 2.0 * Q90(a, e2);
							}
							if (p_az12 != null)
							{
								p_az12.val = 0.0;
							}
							if (p_az21 != null)
							{
								p_az21.val = 0.0;
							}
							return;
						}
					}
				}
			}
			if (PE_ZERO(e2))
			{
				/* Sphere */
				double cos_phi1;
				double cos_phi2;
				double sin_phi1;
				double sin_phi2;
				cos_phi1 = System.Math.Cos(phi1);
				cos_phi2 = System.Math.Cos(phi2);
				sin_phi1 = System.Math.Sin(phi1);
				sin_phi2 = System.Math.Sin(phi2);
				if (p_dist != null)
				{
					tem1 = System.Math.Sin((phi2 - phi1) / 2.0);
					tem2 = System.Math.Sin(dlam / 2.0);
					sigma = 2.0 * System.Math.Asin(System.Math.Sqrt(tem1 * tem1 + cos_phi1 * cos_phi2 * tem2 * tem2));
					p_dist.val = sigma * a;
				}
				if (p_az12 != null)
				{
					if (PE_EQ(PE_ABS(phi1), PE_PI2))
					{
						/* Origin at N or S Pole */
						p_az12.val = phi1 < 0.0 ? lam2 : Lam_delta(PE_PI - lam2);
					}
					else
					{
						p_az12.val = System.Math.Atan2(cos_phi2 * System.Math.Sin(dlam), cos_phi1 * sin_phi2 - sin_phi1 * cos_phi2 * System.Math.Cos(dlam));
					}
				}
				if (p_az21 != null)
				{
					if (PE_EQ(PE_ABS(phi2), PE_PI2))
					{
						/* Destination at N or S Pole */
						p_az21.val = phi2 < 0.0 ? lam1 : Lam_delta(PE_PI - lam1);
					}
					else
					{
						p_az21.val = System.Math.Atan2(cos_phi1 * System.Math.Sin(dlam), sin_phi2 * cos_phi1 * System.Math.Cos(dlam) - cos_phi2 * sin_phi1);
						p_az21.val = Lam_delta(p_az21.val + PE_PI);
					}
				}
				return;
			}
			f = 1.0 - System.Math.Sqrt(1.0 - e2);
			boa = 1.0 - f;
			eta1 = System.Math.Atan(boa * System.Math.Tan(phi1));
			/* better reduced latitude */
			sin_eta1 = System.Math.Sin(eta1);
			cos_eta1 = System.Math.Cos(eta1);
			eta2 = System.Math.Atan(boa * System.Math.Tan(phi2));
			/* better reduced latitude */
			sin_eta2 = System.Math.Sin(eta2);
			cos_eta2 = System.Math.Cos(eta2);
			prev = dlam;
			test = dlam;
			it = 0;
			kind = 1;
			lam_sph = dlam;
			/* v13 (Rapp ) */
			/* top of the long-line loop (kind = 1) */
			q_continue_looping = true;
			while (q_continue_looping && it < 100)
			{
				it = it + 1;
				if (kind == 1)
				{
					sin_lam_sph = System.Math.Sin(lam_sph);
					/*
					* if ( PE_ABS(PE_PI - PE_ABS(dlam)) < 2.0e-11 ) sin_lam_sph =
					* 0.0 no--troublesome
					*/
					cos_lam_sph = System.Math.Cos(lam_sph);
					tem1 = cos_eta2 * sin_lam_sph;
					temp = cos_eta1 * sin_eta2 - sin_eta1 * cos_eta2 * cos_lam_sph;
					sin_sigma = System.Math.Sqrt(tem1 * tem1 + temp * temp);
					/*
					* v14 (Rapp
					* 1.87)
					*/
					cos_sigma = sin_eta1 * sin_eta2 + cos_eta1 * cos_eta2 * cos_lam_sph;
					/* v15 (Rapp 1.88) */
					sigma = System.Math.Atan2(sin_sigma, cos_sigma);
					/* (Rapp 1.89) */
					if (PE_ABS(sin_sigma) < eps)
					{
						/* avoid division by 0 */
						sin_azeq = cos_eta1 * cos_eta2 * sin_lam_sph / PE_SGN(eps, sin_sigma);
					}
					else
					{
						sin_azeq = cos_eta1 * cos_eta2 * sin_lam_sph / sin_sigma;
					}
					/* v17 (Rapp 1.90) */
					cos2_azeq = 1.0 - sin_azeq * sin_azeq;
					if (PE_ABS(cos2_azeq) < eps)
					{
						/* avoid division by 0 */
						costm = cos_sigma - 2.0 * (sin_eta1 * sin_eta2 / PE_SGN(eps, cos2_azeq));
					}
					else
					{
						costm = cos_sigma - 2.0 * (sin_eta1 * sin_eta2 / cos2_azeq);
					}
					/* v18 (Rapp 1.91) */
					costm2 = costm * costm;
					c = ((-3.0 * cos2_azeq + 4.0) * f + 4.0) * cos2_azeq * f / 16.0;
				}
				/*
				* v10
				* (
				* Rapp
				* 1.83
				* )
				*/
				/* entry point of the antipodal loop (kind = 2) */
				d = (1.0 - c) * f * (sigma + c * sin_sigma * (costm + cos_sigma * c * (2.0 * costm2 - 1.0)));
				/* v11 (Rapp 1.84) */
				if (kind == 1)
				{
					lam_sph = dlam + d * sin_azeq;
					if (PE_ABS(lam_sph - test) < tol)
					{
						q_continue_looping = false;
						continue;
					}
					if (PE_ABS(lam_sph) > PE_PI)
					{
						kind = 2;
						lam_sph = PE_PI;
						if (dlam < 0.0)
						{
							lam_sph = -lam_sph;
						}
						sin_azeq = 0.0;
						cos2_azeq = 1.0;
						test = 2.0;
						prev = test;
						sigma = PE_PI - PE_ABS(System.Math.Atan(sin_eta1 / cos_eta1) + System.Math.Atan(sin_eta2 / cos_eta2));
						sin_sigma = System.Math.Sin(sigma);
						cos_sigma = System.Math.Cos(sigma);
						c = ((-3.0 * cos2_azeq + 4.0) * f + 4.0) * cos2_azeq * f / 16.0;
						/* v10 (Rapp 1.83) */
						if (PE_ABS(sin_azeq - prev) < tol)
						{
							q_continue_looping = false;
							continue;
						}
						if (PE_ABS(cos2_azeq) < eps)
						{
							/* avoid division by 0 */
							costm = cos_sigma - 2.0 * (sin_eta1 * sin_eta2 / PE_SGN(eps, cos2_azeq));
						}
						else
						{
							costm = cos_sigma - 2.0 * (sin_eta1 * sin_eta2 / cos2_azeq);
						}
						/* v18 (Rapp 1.91) */
						costm2 = costm * costm;
						continue;
					}
					if (((lam_sph - test) * (test - prev)) < 0.0 && it > 5)
					{
						/* refined converge */
						lam_sph = (2.0 * lam_sph + 3.0 * test + prev) / 6.0;
					}
					prev = test;
					test = lam_sph;
					continue;
				}
				else
				{
					/* kind == 2 */
					sin_azeq = (lam_sph - dlam) / d;
					if (((sin_azeq - test) * (test - prev)) < 0.0 && it > 5)
					{
						/* refined converge */
						sin_azeq = (2.0 * sin_azeq + 3.0 * test + prev) / 6.0;
					}
					prev = test;
					test = sin_azeq;
					cos2_azeq = 1.0 - sin_azeq * sin_azeq;
					sin_lam_sph = sin_azeq * sin_sigma / (cos_eta1 * cos_eta2);
					cos_lam_sph = -System.Math.Sqrt(PE_ABS(1.0 - sin_lam_sph * sin_lam_sph));
					lam_sph = System.Math.Atan2(sin_lam_sph, cos_lam_sph);
					tem1 = cos_eta2 * sin_lam_sph;
					temp = cos_eta1 * sin_eta2 - sin_eta1 * cos_eta2 * cos_lam_sph;
					sin_sigma = System.Math.Sqrt(tem1 * tem1 + temp * temp);
					cos_sigma = sin_eta1 * sin_eta2 + cos_eta1 * cos_eta2 * cos_lam_sph;
					sigma = System.Math.Atan2(sin_sigma, cos_sigma);
					c = ((-3.0 * cos2_azeq + 4.0) * f + 4.0) * cos2_azeq * f / 16.0;
					/*
					* v10
					* (
					* Rapp
					* 1.83
					* )
					*/
					if (PE_ABS(sin_azeq - prev) < tol)
					{
						q_continue_looping = false;
						continue;
					}
					if (PE_ABS(cos2_azeq) < eps)
					{
						/* avoid division by 0 */
						costm = cos_sigma - 2.0 * (sin_eta1 * sin_eta2 / PE_SGN(eps, cos2_azeq));
					}
					else
					{
						costm = cos_sigma - 2.0 * (sin_eta1 * sin_eta2 / cos2_azeq);
					}
					/* v18 (Rapp 1.91) */
					costm2 = costm * costm;
					continue;
				}
			}
			/* End of while q_continue_looping */
			/* Convergence */
			if (p_dist != null)
			{
				/*
				* Helmert 1880 from Vincenty's
				* "Geodetic inverse solution between antipodal points"
				*/
				ep2 = 1.0 / (boa * boa) - 1.0;
				bige = System.Math.Sqrt(1.0 + ep2 * cos2_azeq);
				/* 15 */
				bigf = (bige - 1.0) / (bige + 1.0);
				/* 16 */
				biga = (1.0 + bigf * bigf / 4.0) / (1.0 - bigf);
				/* 17 */
				bigb = bigf * (1.0 - 0.375 * bigf * bigf);
				/* 18 */
				z = bigb / 6.0 * costm * (-3.0 + 4.0 * sin_sigma * sin_sigma) * (-3.0 + 4.0 * costm2);
				dsigma = bigb * sin_sigma * (costm + bigb / 4.0 * (cos_sigma * (-1.0 + 2.0 * costm2) - z));
				/* 19 */
				dist = (boa * a) * biga * (sigma - dsigma);
				/* 20 */
				p_dist.val = dist;
			}
			if (p_az12 != null || p_az21 != null)
			{
				if (kind == 2)
				{
					/* antipodal */
					az12 = sin_azeq / cos_eta1;
					az21 = System.Math.Sqrt(1.0 - az12 * az12);
					if (temp < 0.0)
					{
						az21 = -az21;
					}
					az12 = System.Math.Atan2(az12, az21);
					tem1 = -sin_azeq;
					tem2 = sin_eta1 * sin_sigma - cos_eta1 * cos_sigma * az21;
					az21 = System.Math.Atan2(tem1, tem2);
				}
				else
				{
					/* long-line */
					tem1 = cos_eta2 * sin_lam_sph;
					tem2 = cos_eta1 * sin_eta2 - sin_eta1 * cos_eta2 * cos_lam_sph;
					az12 = System.Math.Atan2(tem1, tem2);
					tem1 = -cos_eta1 * sin_lam_sph;
					tem2 = sin_eta1 * cos_eta2 - cos_eta1 * sin_eta2 * cos_lam_sph;
					az21 = System.Math.Atan2(tem1, tem2);
				}
				if (p_az12 != null)
				{
					p_az12.val = Lam_delta(az12);
				}
				if (p_az21 != null)
				{
					p_az21.val = Lam_delta(az21);
				}
			}
		}
	}
}
