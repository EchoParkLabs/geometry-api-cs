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
	internal class StringUtils
	{
		internal static void AppendDouble(double value, int precision, System.Text.StringBuilder stringBuilder)
		{
			if (precision < 0)
			{
				precision = 0;
			}
			else
			{
				if (precision > 17)
				{
					precision = 17;
				}
			}
			string format = "{0:G" + precision + "}";
			string str_dbl = string.Format(System.Globalization.CultureInfo.InvariantCulture, format, value);
			bool b_found_dot = false;
			bool b_found_exponent = false;
			for (int i = 0; i < str_dbl.Length; i++)
			{
				char c = str_dbl[i];
				if (c == '.')
				{
					b_found_dot = true;
				}
				else
				{
					if (c == 'e' || c == 'E')
					{
						b_found_exponent = true;
						break;
					}
				}
			}
			if (b_found_dot && !b_found_exponent)
			{
				System.Text.StringBuilder buffer = RemoveTrailingZeros_(str_dbl);
				stringBuilder.Append(buffer);
			}
			else
			{
				stringBuilder.Append(str_dbl);
			}
		}

		internal static void AppendDoubleF(double value, int decimals, System.Text.StringBuilder stringBuilder)
		{
			if (decimals < 0)
			{
				decimals = 0;
			}
			else
			{
				if (decimals > 17)
				{
					decimals = 17;
				}
			}
			string format = "{0:F" + decimals + "}";
			string str_dbl = string.Format(System.Globalization.CultureInfo.InvariantCulture, format, value);
			bool b_found_dot = false;
			for (int i = 0; i < str_dbl.Length; i++)
			{
				char c = str_dbl[i];
				if (c == '.')
				{
					b_found_dot = true;
					break;
				}
			}
			if (b_found_dot)
			{
				System.Text.StringBuilder buffer = RemoveTrailingZeros_(str_dbl);
				stringBuilder.Append(buffer);
			}
			else
			{
				stringBuilder.Append(str_dbl);
			}
		}

		private static System.Text.StringBuilder RemoveTrailingZeros_(string str_dbl)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder(str_dbl);
			int non_zero = buffer.Length - 1;
			while (buffer[non_zero] == '0')
			{
				non_zero--;
			}
			buffer.Remove(non_zero + 1, buffer.Length - non_zero - 1);
			if (buffer[non_zero] == '.')
			{
				buffer.Remove(non_zero, buffer.Length - non_zero);
			}
			return buffer;
		}
	}
}
