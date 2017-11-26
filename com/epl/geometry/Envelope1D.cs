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
	/// <summary>A 1-dimensional interval.</summary>
	[System.Serializable]
	public sealed class Envelope1D
	{
		private const long serialVersionUID = 1L;

		public double vmin;

		public double vmax;

		public Envelope1D()
		{
		}

		public Envelope1D(double _vmin, double _vmax)
		{
			SetCoords(_vmin, _vmax);
		}

		public Envelope1D(com.epl.geometry.Envelope1D other)
		{
			SetCoords(other);
		}

		public void SetCoords(double _vmin, double _vmax)
		{
			vmin = _vmin;
			vmax = _vmax;
			Normalize();
		}

		public void SetCoords(com.epl.geometry.Envelope1D other)
		{
			SetCoords(other.vmin, other.vmax);
		}

		public void Normalize()
		{
			if (com.epl.geometry.NumberUtils.IsNaN(vmin))
			{
				return;
			}
			if (vmin > vmax)
			{
				double v = vmin;
				vmin = vmax;
				vmax = v;
			}
			if (com.epl.geometry.NumberUtils.IsNaN(vmax))
			{
				// vmax can be NAN
				SetEmpty();
			}
		}

		public void SetEmpty()
		{
			vmin = com.epl.geometry.NumberUtils.NaN();
			vmax = com.epl.geometry.NumberUtils.NaN();
		}

		public bool IsEmpty()
		{
			return com.epl.geometry.NumberUtils.IsNaN(vmin) || com.epl.geometry.NumberUtils.IsNaN(vmax);
		}

		public void SetInfinite()
		{
			vmin = com.epl.geometry.NumberUtils.NegativeInf();
			vmax = com.epl.geometry.NumberUtils.PositiveInf();
		}

		public void Merge(double v)
		{
			if (IsEmpty())
			{
				vmin = v;
				vmax = v;
				return;
			}
			// no need to check for NaN, because all comparisons with NaN are false.
			MergeNE(v);
		}

		public void Merge(com.epl.geometry.Envelope1D other)
		{
			if (other.IsEmpty())
			{
				return;
			}
			if (IsEmpty())
			{
				vmin = other.vmin;
				vmax = other.vmax;
				return;
			}
			if (vmin > other.vmin)
			{
				vmin = other.vmin;
			}
			if (vmax < other.vmax)
			{
				vmax = other.vmax;
			}
			if (vmin > vmax)
			{
				SetEmpty();
			}
		}

		public void MergeNE(double v)
		{
			// Note, if v is NaN, vmin and vmax are unchanged
			if (v < vmin)
			{
				vmin = v;
			}
			else
			{
				if (v > vmax)
				{
					vmax = v;
				}
			}
		}

		public bool Contains(double v)
		{
			// If vmin is NaN, return false. No need to check for isEmpty.
			return v >= vmin && v <= vmax;
		}

		/// <summary>
		/// Returns True if the envelope contains the other envelope (boundary
		/// inclusive).
		/// </summary>
		/// <remarks>
		/// Returns True if the envelope contains the other envelope (boundary
		/// inclusive). Note: Will return false if either envelope is empty.
		/// </remarks>
		public bool Contains(com.epl.geometry.Envelope1D other)
		{
			/* const */
			/* const */
			return other.vmin >= vmin && other.vmax <= vmax;
		}

		public void Intersect(com.epl.geometry.Envelope1D other)
		{
			if (IsEmpty() || other.IsEmpty())
			{
				SetEmpty();
				return;
			}
			if (vmin < other.vmin)
			{
				vmin = other.vmin;
			}
			if (vmax > other.vmax)
			{
				vmax = other.vmax;
			}
			if (vmin > vmax)
			{
				SetEmpty();
			}
		}

		public void Inflate(double delta)
		{
			if (IsEmpty())
			{
				return;
			}
			vmin -= delta;
			vmax += delta;
			if (vmax < vmin)
			{
				SetEmpty();
			}
		}

		internal double _calculateToleranceFromEnvelope()
		{
			if (IsEmpty())
			{
				return com.epl.geometry.NumberUtils.DoubleEps() * 100.0;
			}
			// GEOMTERYX_EPSFACTOR
			// 100.0;
			double r = System.Math.Abs(vmin) + System.Math.Abs(vmax) + 1;
			return r * com.epl.geometry.NumberUtils.DoubleEps() * 100.0;
		}

		// GEOMTERYX_EPSFACTOR
		// 100.0;
		internal void NormalizeNoNaN_()
		{
			if (vmin > vmax)
			{
				double v = vmin;
				vmin = vmax;
				vmax = v;
			}
		}

		internal void SetCoordsNoNaN_(double vmin_, double vmax_)
		{
			vmin = vmin_;
			vmax = vmax_;
			NormalizeNoNaN_();
		}

		public double SnapClip(double v)
		{
			/* const */
			return com.epl.geometry.NumberUtils.Snap(v, vmin, vmax);
		}

		public double GetWidth()
		{
			/* const */
			return vmax - vmin;
		}

		public double GetCenter()
		{
			/* const */
			return 0.5 * (vmin + vmax);
		}

		public override bool Equals(object _other)
		{
			if (_other == this)
			{
				return true;
			}
			if (!(_other is com.epl.geometry.Envelope1D))
			{
				return false;
			}
			com.epl.geometry.Envelope1D other = (com.epl.geometry.Envelope1D)_other;
			if (IsEmpty() && other.IsEmpty())
			{
				return true;
			}
			if (vmin != other.vmin || vmax != other.vmax)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return com.epl.geometry.NumberUtils.Hash(com.epl.geometry.NumberUtils.Hash(vmin), vmax);
		}
	}
}
