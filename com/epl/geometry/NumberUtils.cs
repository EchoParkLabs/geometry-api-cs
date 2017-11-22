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
	public class NumberUtils
	{
		public static int Snap(int v, int minv, int maxv)
		{
			return v < minv ? minv : v > maxv ? maxv : v;
		}

		public static long Snap(long v, long minv, long maxv)
		{
			return v < minv ? minv : v > maxv ? maxv : v;
		}

		public static double Snap(double v, double minv, double maxv)
		{
			return v < minv ? minv : v > maxv ? maxv : v;
		}

		internal static int SizeOf(double v)
		{
			return 8;
		}

		internal static int SizeOfDouble()
		{
			return 8;
		}

		internal static int SizeOf(int v)
		{
			return 4;
		}

		internal static int SizeOf(long v)
		{
			return 8;
		}

		internal static int SizeOf(byte v)
		{
			return 1;
		}

		internal static bool IsNaN(double d)
		{
			return double.IsNaN(d);
		}

		internal const double TheNaN = double.NaN;

		internal static double NaN()
		{
			return double.NaN;
		}

		internal static int Hash(int n)
		{
			int hash = 5381;
			hash = ((hash << 5) + hash) + (n & unchecked((int)(0xFF)));
			/* hash * 33 + c */
			hash = ((hash << 5) + hash) + ((n >> 8) & unchecked((int)(0xFF)));
			hash = ((hash << 5) + hash) + ((n >> 16) & unchecked((int)(0xFF)));
			hash = ((hash << 5) + hash) + ((n >> 24) & unchecked((int)(0xFF)));
			hash &= unchecked((int)(0x7FFFFFFF));
			return hash;
		}

		internal static int Hash(double d)
		{
			long bits = System.BitConverter.DoubleToInt64Bits(d);
			int hc = (int)(bits ^ ((long)(((ulong)bits) >> 32)));
			return Hash(hc);
		}

		internal static int Hash(int hashIn, int n)
		{
			int hash = ((hashIn << 5) + hashIn) + (n & unchecked((int)(0xFF)));
			/* hash * 33 + c */
			hash = ((hash << 5) + hash) + ((n >> 8) & unchecked((int)(0xFF)));
			hash = ((hash << 5) + hash) + ((n >> 16) & unchecked((int)(0xFF)));
			hash = ((hash << 5) + hash) + ((n >> 24) & unchecked((int)(0xFF)));
			hash &= unchecked((int)(0x7FFFFFFF));
			return hash;
		}

		internal static int Hash(int hash, double d)
		{
			long bits = System.BitConverter.DoubleToInt64Bits(d);
			int hc = (int)(bits ^ ((long)(((ulong)bits) >> 32)));
			return Hash(hash, hc);
		}

		internal static long DoubleToInt64Bits(double d)
		{
			return System.BitConverter.DoubleToInt64Bits(d);
		}

		internal static double NegativeInf()
		{
			return double.NegativeInfinity;
		}

		internal static double PositiveInf()
		{
			return double.PositiveInfinity;
		}

		internal static int IntMax()
		{
			return int.MaxValue;
		}

		internal static double DoubleEps()
		{
			return 2.2204460492503131e-016;
		}

		internal static double DoubleMax()
		{
			return double.MaxValue;
		}

		internal static int NextRand(int prevRand)
		{
			return (1103515245 * prevRand + 12345) & IntMax();
		}
		// according to Wiki,
		// this is gcc's
	}
}
