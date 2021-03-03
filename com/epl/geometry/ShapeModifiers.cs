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
	internal abstract class ShapeModifiers
	{
		public const int ShapeHasZs = unchecked((int)(0x80000000));

		public const int ShapeHasMs = unchecked((int)(0x40000000));

		public const int ShapeHasCurves = unchecked((int)(0x20000000));

		public const int ShapeHasIDs = unchecked((int)(0x10000000));

		public const int ShapeHasNormals = unchecked((int)(0x08000000));

		public const int ShapeHasTextures = unchecked((int)(0x04000000));

		public const int ShapeHasPartIDs = unchecked((int)(0x02000000));

		public const int ShapeHasMaterials = unchecked((int)(0x01000000));

		public const int ShapeIsCompressed = unchecked((int)(0x00800000));

		public const int ShapeModifierMask = unchecked((int)(0xFF000000));

		public const int ShapeMultiPatchModifierMask = unchecked((int)(0x0F00000));

		public const int ShapeBasicTypeMask = unchecked((int)(0x000000FF));

		public const int ShapeBasicModifierMask = unchecked((int)(0xC0000000));

		public const int ShapeNonBasicModifierMask = unchecked((int)(0x3F000000));

		public const int ShapeExtendedModifierMask = unchecked((int)(0xDD000000));
	}

	internal static class ShapeModifiersConstants
	{
	}
}
