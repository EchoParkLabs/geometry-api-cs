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
	/// <summary>Flags used by the OperatorImportFromEsriShape.</summary>
	public abstract class ShapeImportFlags
	{
		public const int ShapeImportDefaults = 0;

		public const int ShapeImportNoSwap = 1;

		public const int ShapeImportNonTrusted = 2;

		public const int ShapeImportAttach = 4;
		//!<Default import flags
		//!<Pass this flag to the import to indicate the shape can contain non-simple geometry.
	}

	public static class ShapeImportFlagsConstants
	{
	}
}
