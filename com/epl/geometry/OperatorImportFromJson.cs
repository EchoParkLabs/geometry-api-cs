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
	/// <summary>Import from JSON format.</summary>
	public abstract class OperatorImportFromJson : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.ImportFromJson;
		}

		/// <summary>Performs the ImportFromJson operation on a number of Json Strings</summary>
		/// <returns>Returns a MapGeometryCursor.</returns>
		internal abstract com.epl.geometry.MapGeometryCursor Execute(com.epl.geometry.Geometry.Type type, com.epl.geometry.JsonParserCursor jsonParserCursor);

		/// <summary>Performs the ImportFromJson operation on a single Json string</summary>
		/// <returns>Returns a MapGeometry.</returns>
		public abstract com.epl.geometry.MapGeometry Execute(com.epl.geometry.Geometry.Type type, org.codehaus.jackson.JsonParser jsonParser);

		/// <summary>Performs the ImportFromJson operation on a single Json string</summary>
		/// <returns>Returns a MapGeometry.</returns>
		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		public abstract com.epl.geometry.MapGeometry Execute(com.epl.geometry.Geometry.Type type, string @string);

		/// <summary>Performs the ImportFromJson operation on a JSONObject</summary>
		/// <returns>Returns a MapGeometry.</returns>
		/// <exception cref="org.json.JSONException"/>
		/// <exception cref="System.IO.IOException"/>
		public abstract com.epl.geometry.MapGeometry Execute(com.epl.geometry.Geometry.Type type, org.json.JSONObject jsonObject);

		public static com.epl.geometry.OperatorImportFromJson Local()
		{
			return (com.epl.geometry.OperatorImportFromJson)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromJson);
		}
	}
}
