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
	internal class OperatorImportFromJsonLocal : com.epl.geometry.OperatorImportFromJson
	{
		internal override com.epl.geometry.MapGeometryCursor Execute(com.epl.geometry.Geometry.Type type, com.epl.geometry.JsonReaderCursor jsonParserCursor)
		{
			return new com.epl.geometry.OperatorImportFromJsonCursor(type.Value(), jsonParserCursor);
		}

		public override com.epl.geometry.MapGeometry Execute(com.epl.geometry.Geometry.Type type, com.epl.geometry.JsonReader jsonParser)
		{
			return com.epl.geometry.OperatorImportFromJsonCursor.ImportFromJsonParser(type.Value(), jsonParser);
		}

		public override com.epl.geometry.MapGeometry Execute(com.epl.geometry.Geometry.Type type, string @string)
		{
			return Execute(type, com.epl.geometry.JsonParserReader.CreateFromString(@string));
		}
	}
}
