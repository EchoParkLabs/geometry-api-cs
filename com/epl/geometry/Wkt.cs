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
	internal sealed class Wkt
	{
		public static double Find_tolerance_from_wkt(string buffer)
		{
			double tolerance = -1.0;
			if (buffer != null && buffer.Length > 0)
			{
				int n1;
				int n2;
				n1 = buffer.IndexOf("PROJCS");
				if (n1 >= 0)
				{
					double factor = 0.0;
					n1 = buffer.LastIndexOf("UNIT");
					if (n1 >= 0)
					{
						n1 = buffer.IndexOf(',', n1 + 4);
						if (n1 > 0)
						{
							n1++;
							n2 = buffer.IndexOf(']', n1 + 1);
							if (n2 > 0)
							{
								try
								{
									factor = double.Parse(buffer.Substring(n1, n2 - n1));
								}
								catch (System.FormatException)
								{
									factor = 0.0;
								}
							}
						}
					}
					if (factor > 0.0)
					{
						tolerance = (0.001 / factor);
					}
				}
				else
				{
					n1 = buffer.IndexOf("GEOGCS");
					if (n1 >= 0)
					{
						double axis = 0.0;
						double factor = 0.0;
						n1 = buffer.IndexOf("SPHEROID", n1 + 6);
						if (n1 > 0)
						{
							n1 = buffer.IndexOf(',', n1 + 8);
							if (n1 > 0)
							{
								n1++;
								n2 = buffer.IndexOf(',', n1 + 1);
								if (n2 > 0)
								{
									try
									{
										axis = double.Parse(buffer.Substring(n1, n2 - n1));
									}
									catch (System.FormatException)
									{
										axis = 0.0;
									}
								}
								if (axis > 0.0)
								{
									n1 = buffer.IndexOf("UNIT", n2 + 1);
									if (n1 >= 0)
									{
										n1 = buffer.IndexOf(',', n1 + 4);
										if (n1 > 0)
										{
											n1++;
											n2 = buffer.IndexOf(']', n1 + 1);
											if (n2 > 0)
											{
												try
												{
													factor = double.Parse(buffer.Substring(n1, n2 - n1));
												}
												catch (System.FormatException)
												{
													factor = 0.0;
												}
											}
										}
									}
								}
							}
						}
						if (axis > 0.0 && factor > 0.0)
						{
							tolerance = (0.001 / (axis * factor));
						}
					}
				}
			}
			return tolerance;
		}
	}
}
