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
	/// <summary>
	/// The modes for converting between military grid reference system (MGRS)
	/// notation and coordinates.
	/// </summary>
	public abstract class MgrsConversionMode
	{
		/// <summary>Uses the spheroid to determine the military grid string.</summary>
		public const int mgrsAutomatic = 0;

		/// <summary>
		/// Treats all spheroids as new, like WGS 1984, when creating or reading a
		/// military grid string.
		/// </summary>
		/// <remarks>
		/// Treats all spheroids as new, like WGS 1984, when creating or reading a
		/// military grid string. The 180 longitude falls into zone 60.
		/// </remarks>
		public const int mgrsNewStyle = unchecked((int)(0x100));

		/// <summary>
		/// Treats all spheroids as old, like Bessel 1841, when creating or reading a
		/// military grid string.
		/// </summary>
		/// <remarks>
		/// Treats all spheroids as old, like Bessel 1841, when creating or reading a
		/// military grid string. The 180 longitude falls into zone 60.
		/// </remarks>
		public const int mgrsOldStyle = unchecked((int)(0x200));

		/// <summary>
		/// Treats all spheroids as new, like WGS 1984, when creating or reading a
		/// military grid string.
		/// </summary>
		/// <remarks>
		/// Treats all spheroids as new, like WGS 1984, when creating or reading a
		/// military grid string. The 180 longitude falls into zone 01.
		/// </remarks>
		public const int mgrsNewWith180InZone01 = unchecked((int)(0x1000)) + unchecked((int)(0x100));

		/// <summary>
		/// Treats all spheroids as old, like Bessel 1841, when creating or reading a
		/// military grid string.
		/// </summary>
		/// <remarks>
		/// Treats all spheroids as old, like Bessel 1841, when creating or reading a
		/// military grid string. The 180 longitude falls into zone 01.
		/// </remarks>
		public const int mgrsOldWith180InZone01 = unchecked((int)(0x1000)) + unchecked((int)(0x200));
		// PE_MGRS_STYLE_AUTO
		// PE_MGRS_STYLE_NEW
		// PE_MGRS_STYLE_OLD
		// PE_MGRS_180_ZONE_1_PLUS
		// |
		// PE_MGRS_STYLE_NEW
		// PE_MGRS_180_ZONE_1_PLUS
		// |
		// PE_MGRS_STYLE_OLD
	}

	public static class MgrsConversionModeConstants
	{
	}
}
