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
	internal class ECoordinate
	{
		private double m_value;

		private double m_eps;

		internal ECoordinate()
		{
			Set(0.0, 0.0);
		}

		internal ECoordinate(double v)
		{
			Set(v);
		}

		internal ECoordinate(com.epl.geometry.ECoordinate v)
		{
			Set(v);
		}

		internal virtual double EpsCoordinate()
		{
			return com.epl.geometry.NumberUtils.DoubleEps();
		}

		internal virtual void ScaleError(double f)
		{
			m_eps *= f;
		}

		internal virtual void SetError(double e)
		{
			m_eps = e;
		}

		internal virtual void Set(double v, double e)
		{
			m_value = v;
			m_eps = e;
		}

		internal virtual void Set(double v)
		{
			m_value = v;
			m_eps = 0;
		}

		internal virtual void Set(com.epl.geometry.ECoordinate v)
		{
			m_value = v.m_value;
			m_eps = v.m_eps;
		}

		internal virtual double Value()
		{
			return m_value;
		}

		internal virtual double Eps()
		{
			return m_eps;
		}

		internal virtual void ResetError()
		{
			m_eps = 0;
		}

		internal virtual void Add(com.epl.geometry.ECoordinate v)
		{
			// +=
			double r = m_value + v.m_value;
			double e = m_eps + v.m_eps + EpsCoordinate() * System.Math.Abs(r);
			m_value = r;
			m_eps = e;
		}

		internal virtual void Add(double v)
		{
			// +=
			double r = m_value + v;
			double e = m_eps + EpsCoordinate() * System.Math.Abs(r);
			m_value = r;
			m_eps = e;
		}

		internal virtual void Sub(com.epl.geometry.ECoordinate v)
		{
			// -=
			double r = m_value - v.m_value;
			double e = m_eps + v.m_eps + EpsCoordinate() * System.Math.Abs(r);
			m_value = r;
			m_eps = e;
		}

		internal virtual void Sub(double v)
		{
			// -=
			double r = m_value - v;
			double e = m_eps + EpsCoordinate() * System.Math.Abs(r);
			m_value = r;
			m_eps = e;
		}

		internal virtual void Add(com.epl.geometry.ECoordinate v_1, com.epl.geometry.ECoordinate v_2)
		{
			// +
			m_value = v_1.m_value + v_2.m_value;
			m_eps = v_1.m_eps + v_2.m_eps + EpsCoordinate() * System.Math.Abs(m_value);
		}

		internal virtual void Add(double v_1, double v_2)
		{
			// +
			m_value = v_1 + v_2;
			m_eps = EpsCoordinate() * System.Math.Abs(m_value);
		}

		internal virtual void Add(com.epl.geometry.ECoordinate v_1, double v_2)
		{
			// +
			m_value = v_1.m_value + v_2;
			m_eps = v_1.m_eps + EpsCoordinate() * System.Math.Abs(m_value);
		}

		internal virtual void Add(double v_1, com.epl.geometry.ECoordinate v_2)
		{
			// +
			m_value = v_1 + v_2.m_value;
			m_eps = v_2.m_eps + EpsCoordinate() * System.Math.Abs(m_value);
		}

		internal virtual void Sub(com.epl.geometry.ECoordinate v_1, com.epl.geometry.ECoordinate v_2)
		{
			// -
			m_value = v_1.m_value - v_2.m_value;
			m_eps = v_1.m_eps + v_2.m_eps + EpsCoordinate() * System.Math.Abs(m_value);
		}

		internal virtual void Sub(double v_1, double v_2)
		{
			// -
			m_value = v_1 - v_2;
			m_eps = EpsCoordinate() * System.Math.Abs(m_value);
		}

		internal virtual void Sub(com.epl.geometry.ECoordinate v_1, double v_2)
		{
			// -
			m_value = v_1.m_value - v_2;
			m_eps = v_1.m_eps + EpsCoordinate() * System.Math.Abs(m_value);
		}

		internal virtual void Sub(double v_1, com.epl.geometry.ECoordinate v_2)
		{
			// -
			m_value = v_1 - v_2.m_value;
			m_eps = v_2.m_eps + EpsCoordinate() * System.Math.Abs(m_value);
		}

		internal virtual void Mul(com.epl.geometry.ECoordinate v)
		{
			double r = m_value * v.m_value;
			m_eps = m_eps * System.Math.Abs(v.m_value) + v.m_eps * System.Math.Abs(m_value) + m_eps * v.m_eps + EpsCoordinate() * System.Math.Abs(r);
			m_value = r;
		}

		internal virtual void Mul(double v)
		{
			double r = m_value * v;
			m_eps = m_eps * System.Math.Abs(v) + EpsCoordinate() * System.Math.Abs(r);
			m_value = r;
		}

		internal virtual void Mul(com.epl.geometry.ECoordinate v_1, com.epl.geometry.ECoordinate v_2)
		{
			double r = v_1.m_value * v_2.m_value;
			m_eps = v_1.m_eps * System.Math.Abs(v_2.m_value) + v_2.m_eps * System.Math.Abs(v_1.m_value) + v_1.m_eps * v_2.m_eps + EpsCoordinate() * System.Math.Abs(r);
			m_value = r;
		}

		internal virtual void Mul(double v_1, double v_2)
		{
			m_value = v_1 * v_2;
			m_eps = EpsCoordinate() * System.Math.Abs(m_value);
		}

		internal virtual void Mul(com.epl.geometry.ECoordinate v_1, double v_2)
		{
			Set(v_1);
			Mul(v_2);
		}

		internal virtual void Mul(double v_1, com.epl.geometry.ECoordinate v_2)
		{
			Set(v_2);
			Mul(v_1);
		}

		internal virtual void Div(com.epl.geometry.ECoordinate divis)
		{
			double fabsdivis = System.Math.Abs(divis.m_value);
			double r = m_value / divis.m_value;
			double e = (m_eps + System.Math.Abs(r) * divis.m_eps) / fabsdivis;
			if (divis.m_eps > 0.01 * fabsdivis)
			{
				// more accurate error calculation
				// for very inaccurate divisor
				double rr = divis.m_eps / fabsdivis;
				e *= (1.0 + (1.0 + rr) * rr);
			}
			m_value = r;
			m_eps = e + EpsCoordinate() * System.Math.Abs(r);
		}

		internal virtual void Div(double v)
		{
			double fabsdivis = System.Math.Abs(v);
			m_value /= v;
			m_eps = m_eps / fabsdivis + EpsCoordinate() * System.Math.Abs(m_value);
		}

		internal virtual void Div(com.epl.geometry.ECoordinate v_1, com.epl.geometry.ECoordinate v_2)
		{
			Set(v_1);
			Div(v_2);
		}

		internal virtual void Div(double v_1, double v_2)
		{
			m_value = v_1 / v_2;
			m_eps = EpsCoordinate() * System.Math.Abs(m_value);
		}

		internal virtual void Div(com.epl.geometry.ECoordinate v_1, double v_2)
		{
			Set(v_1);
			Div(v_2);
		}

		internal virtual void Div(double v_1, com.epl.geometry.ECoordinate v_2)
		{
			Set(v_1);
			Div(v_2);
		}

		internal virtual void Sqrt()
		{
			double r;
			double dr;
			if (m_value >= 0)
			{
				// assume non-negative input
				r = System.Math.Sqrt(m_value);
				if (m_value > 10.0 * m_eps)
				{
					dr = 0.5 * m_eps / r;
				}
				else
				{
					dr = (m_value > m_eps) ? r - System.Math.Sqrt(m_value - m_eps) : System.Math.Max(r, System.Math.Sqrt(m_value + m_eps) - r);
				}
				dr += EpsCoordinate() * System.Math.Abs(r);
			}
			else
			{
				if (m_value < -m_eps)
				{
					// Assume negative input. Return value
					// undefined
					r = com.epl.geometry.NumberUtils.TheNaN;
					dr = com.epl.geometry.NumberUtils.TheNaN;
				}
				else
				{
					// assume zero input
					r = 0.0;
					dr = System.Math.Sqrt(m_eps);
				}
			}
			m_value = r;
			m_eps = dr;
		}

		internal virtual void Sqr()
		{
			double r = m_value * m_value;
			m_eps = 2 * m_eps * m_value + m_eps * m_eps + EpsCoordinate() * r;
			m_value = r;
		}

		// Assigns sin(angle) to this coordinate.
		internal virtual void Sin(com.epl.geometry.ECoordinate angle)
		{
			double sinv = System.Math.Sin(angle.m_value);
			double cosv = System.Math.Cos(angle.m_value);
			m_value = sinv;
			double absv = System.Math.Abs(sinv);
			m_eps = (System.Math.Abs(cosv) + absv * 0.5 * angle.m_eps) * angle.m_eps + EpsCoordinate() * absv;
		}

		// Assigns cos(angle) to this coordinate.
		internal virtual void Cos(com.epl.geometry.ECoordinate angle)
		{
			double sinv = System.Math.Sin(angle.m_value);
			double cosv = System.Math.Cos(angle.m_value);
			m_value = cosv;
			double absv = System.Math.Abs(cosv);
			m_eps = (System.Math.Abs(sinv) + absv * 0.5 * angle.m_eps) * angle.m_eps + EpsCoordinate() * absv;
		}

		// Calculates natural log of v and assigns to this coordinate
		internal virtual void Log(com.epl.geometry.ECoordinate v)
		{
			double d = v.m_eps / v.m_value;
			m_value = System.Math.Log(v.m_value);
			m_eps = d * (1.0 + 0.5 * d) + EpsCoordinate() * System.Math.Abs(m_value);
		}

		// void SinAndCos(ECoordinate& _sin, ECoordinate& _cos);
		// ECoordinate abs();
		// ECoordinate exp();
		// ECoordinate acos();
		// ECoordinate asin();
		// ECoordinate atan();
		internal virtual bool Eq(com.epl.geometry.ECoordinate v)
		{
			// ==
			return System.Math.Abs(m_value - v.m_value) <= m_eps + v.m_eps;
		}

		internal virtual bool Ne(com.epl.geometry.ECoordinate v)
		{
			// !=
			return !Eq(v);
		}

		internal virtual bool GT(com.epl.geometry.ECoordinate v)
		{
			// >
			return m_value - v.m_value > m_eps + v.m_eps;
		}

		internal virtual bool Lt(com.epl.geometry.ECoordinate v)
		{
			// <
			return v.m_value - m_value > m_eps + v.m_eps;
		}

		internal virtual bool Ge(com.epl.geometry.ECoordinate v)
		{
			// >=
			return !Lt(v);
		}

		internal virtual bool Le(com.epl.geometry.ECoordinate v)
		{
			// <=
			return !GT(v);
		}

		// The following methods take into account the rounding erros as well as
		// user defined tolerance.
		internal virtual bool TolEq(com.epl.geometry.ECoordinate v, double tolerance)
		{
			// ! == with tolerance
			return System.Math.Abs(m_value - v.m_value) <= tolerance || Eq(v);
		}

		internal virtual bool Tol_ne(com.epl.geometry.ECoordinate v, double tolerance)
		{
			// ! !=
			return !TolEq(v, tolerance);
		}

		internal virtual bool TolGT(com.epl.geometry.ECoordinate v, double tolerance)
		{
			// ! >
			return (m_value - v.m_value > tolerance) && GT(v);
		}

		internal virtual bool Tollt(com.epl.geometry.ECoordinate v, double tolerance)
		{
			// ! <
			return (v.m_value - m_value > tolerance) && Lt(v);
		}

		internal virtual bool Tolge(com.epl.geometry.ECoordinate v, double tolerance)
		{
			// ! >=
			return !Tollt(v, tolerance);
		}

		internal virtual bool Tolle(com.epl.geometry.ECoordinate v, double tolerance)
		{
			// ! <=
			return !TolGT(v, tolerance);
		}

		internal virtual bool IsZero()
		{
			return System.Math.Abs(m_value) <= m_eps;
		}

		internal virtual bool IsFuzzyZero()
		{
			return IsZero() && m_eps != 0.0;
		}

		internal virtual bool TolIsZero(double tolerance)
		{
			return System.Math.Abs(m_value) <= System.Math.Max(m_eps, tolerance);
		}

		internal virtual void SetPi()
		{
			Set(System.Math.PI, EpsCoordinate());
		}

		internal virtual void SetE()
		{
			Set(2.71828182845904523536, EpsCoordinate());
		}
	}
}
