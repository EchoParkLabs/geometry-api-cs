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
	internal sealed class MathUtils
	{
		/// <summary>The implementation of the Kahan summation algorithm.</summary>
		/// <remarks>
		/// The implementation of the Kahan summation algorithm. Use to get better
		/// precision when adding a lot of values.
		/// </remarks>
		internal sealed class KahanSummator
		{
			private double sum;

			private double compensation;

			private double startValue;

			/// <summary>initialize to the given start value.</summary>
			/// <remarks>
			/// initialize to the given start value. \param startValue_ The value to
			/// be added to the accumulated sum.
			/// </remarks>
			internal KahanSummator(double startValue_)
			{
				// the accumulated sum
				// the Base (the class returns sum +
				// startValue)
				startValue = startValue_;
				Reset();
			}

			/// <summary>Resets the accumulated sum to zero.</summary>
			/// <remarks>
			/// Resets the accumulated sum to zero. The getResult() returns
			/// startValue_ after this call.
			/// </remarks>
			internal void Reset()
			{
				sum = 0;
				compensation = 0;
			}

			/// <summary>add a value.</summary>
			internal void Add(double v)
			{
				double y = v - compensation;
				double t = sum + y;
				double h = t - sum;
				compensation = h - y;
				sum = t;
			}

			/// <summary>Subtracts a value.</summary>
			internal void Sub(double v)
			{
				Add(-v);
			}

			/// <summary>add another summator.</summary>
			internal void Add(com.epl.geometry.MathUtils.KahanSummator v)
			{
				/* const */
				double y = (v.GetResult() + v.compensation) - compensation;
				double t = sum + y;
				double h = t - sum;
				compensation = h - y;
				sum = t;
			}

			/// <summary>Subtracts another summator.</summary>
			internal void Sub(com.epl.geometry.MathUtils.KahanSummator v)
			{
				/* const */
				double y = -(v.GetResult() - v.compensation) - compensation;
				double t = sum + y;
				double h = t - sum;
				compensation = h - y;
				sum = t;
			}

			/// <summary>Returns current value of the sum.</summary>
			internal double GetResult()
			{
				/* const */
				return startValue + sum;
			}

			internal com.epl.geometry.MathUtils.KahanSummator PlusEquals(double v)
			{
				Add(v);
				return this;
			}

			internal com.epl.geometry.MathUtils.KahanSummator MinusEquals(double v)
			{
				Add(-v);
				return this;
			}

			internal com.epl.geometry.MathUtils.KahanSummator PlusEquals(com.epl.geometry.MathUtils.KahanSummator v)
			{
				/* const */
				Add(v);
				return this;
			}

			internal com.epl.geometry.MathUtils.KahanSummator MinusEquals(com.epl.geometry.MathUtils.KahanSummator v)
			{
				/* const */
				Sub(v);
				return this;
			}
		}

		/// <summary>Returns one value with the sign of another (like copysign).</summary>
		internal static double CopySign(double x, double y)
		{
			return y >= 0.0 ? System.Math.Abs(x) : -System.Math.Abs(x);
		}

		/// <summary>Calculates sign of the given value.</summary>
		/// <remarks>Calculates sign of the given value. Returns 0 if the value is equal to 0.</remarks>
		internal static int Sign(double value)
		{
			return value < 0 ? -1 : (value > 0) ? 1 : 0;
		}

		/// <summary>Rounds towards zero.</summary>
		internal static double Truncate(double v)
		{
			if (v >= 0)
			{
				return System.Math.Floor(v);
			}
			else
			{
				return -System.Math.Floor(-v);
			}
		}

		/// <summary>C fmod function.</summary>
		internal static double FMod(double x, double y)
		{
			return x - Truncate(x / y) * y;
		}

		/// <summary>Rounds double to the closest integer value.</summary>
		internal static double Round(double v)
		{
			return System.Math.Floor(v + 0.5);
		}

		internal static double Sqr(double v)
		{
			return v * v;
		}

		/// <summary>Computes interpolation between two values, using the interpolation factor t.</summary>
		/// <remarks>
		/// Computes interpolation between two values, using the interpolation factor t.
		/// The interpolation formula is (end - start) * t + start.
		/// However, the computation ensures that t = 0 produces exactly start, and t = 1, produces exactly end.
		/// It also guarantees that for 0 &lt;= t &lt;= 1, the interpolated value v is between start and end.
		/// </remarks>
		internal static double Lerp(double start_, double end_, double t)
		{
			// When end == start, we want result to be equal to start, for all t
			// values. At the same time, when end != start, we want the result to be
			// equal to start for t==0 and end for t == 1.0
			// The regular formula end_ * t + (1.0 - t) * start_, when end_ ==
			// start_, and t at 1/3, produces value different from start
			double v;
			if (t <= 0.5)
			{
				v = start_ + (end_ - start_) * t;
			}
			else
			{
				v = end_ - (end_ - start_) * (1.0 - t);
			}
			System.Diagnostics.Debug.Assert((t < 0 || t > 1.0 || (v >= start_ && v <= end_) || (v <= start_ && v >= end_) || com.epl.geometry.NumberUtils.IsNaN(start_) || com.epl.geometry.NumberUtils.IsNaN(end_)));
			return v;
		}

		/// <summary>Computes interpolation between two values, using the interpolation factor t.</summary>
		/// <remarks>
		/// Computes interpolation between two values, using the interpolation factor t.
		/// The interpolation formula is (end - start) * t + start.
		/// However, the computation ensures that t = 0 produces exactly start, and t = 1, produces exactly end.
		/// It also guarantees that for 0 &lt;= t &lt;= 1, the interpolated value v is between start and end.
		/// </remarks>
		internal static void Lerp(com.epl.geometry.Point2D start_, com.epl.geometry.Point2D end_, double t, com.epl.geometry.Point2D result)
		{
			System.Diagnostics.Debug.Assert((start_ != result));
			// When end == start, we want result to be equal to start, for all t
			// values. At the same time, when end != start, we want the result to be
			// equal to start for t==0 and end for t == 1.0
			// The regular formula end_ * t + (1.0 - t) * start_, when end_ ==
			// start_, and t at 1/3, produces value different from start
			double rx;
			double ry;
			if (t <= 0.5)
			{
				rx = start_.x + (end_.x - start_.x) * t;
				ry = start_.y + (end_.y - start_.y) * t;
			}
			else
			{
				rx = end_.x - (end_.x - start_.x) * (1.0 - t);
				ry = end_.y - (end_.y - start_.y) * (1.0 - t);
			}
			System.Diagnostics.Debug.Assert((t < 0 || t > 1.0 || (rx >= start_.x && rx <= end_.x) || (rx <= start_.x && rx >= end_.x)));
			System.Diagnostics.Debug.Assert((t < 0 || t > 1.0 || (ry >= start_.y && ry <= end_.y) || (ry <= start_.y && ry >= end_.y)));
			result.x = rx;
			result.y = ry;
		}

		internal static void Lerp(double start_x, double start_y, double end_x, double end_y, double t, com.epl.geometry.Point2D result)
		{
			// When end == start, we want result to be equal to start, for all t
			// values. At the same time, when end != start, we want the result to be
			// equal to start for t==0 and end for t == 1.0
			// The regular formula end_ * t + (1.0 - t) * start_, when end_ ==
			// start_, and t at 1/3, produces value different from start
			if (t <= 0.5)
			{
				result.x = start_x + (end_x - start_x) * t;
				result.y = start_y + (end_y - start_y) * t;
			}
			else
			{
				result.x = end_x - (end_x - start_x) * (1.0 - t);
				result.y = end_y - (end_y - start_y) * (1.0 - t);
			}
			System.Diagnostics.Debug.Assert((t < 0 || t > 1.0 || (result.x >= start_x && result.x <= end_x) || (result.x <= start_x && result.x >= end_x)));
			System.Diagnostics.Debug.Assert((t < 0 || t > 1.0 || (result.y >= start_y && result.y <= end_y) || (result.y <= start_y && result.y >= end_y)));
		}
	}
}
