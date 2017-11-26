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
	/// <summary>An abstract reader for Json.</summary>
	/// <remarks>
	/// An abstract reader for Json.
	/// See JsonParserReader for a concrete implementation around JsonParser.
	/// </remarks>
	public abstract class JsonReader
	{
		public enum Token
		{
			END_ARRAY,
			END_OBJECT,
			FIELD_NAME,
			START_ARRAY,
			START_OBJECT,
			VALUE_FALSE,
			VALUE_NULL,
			VALUE_NUMBER_FLOAT,
			VALUE_NUMBER_INT,
			VALUE_STRING,
			VALUE_TRUE
		}

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public abstract com.epl.geometry.JsonReader.Token NextToken();

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public abstract com.epl.geometry.JsonReader.Token CurrentToken();

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public abstract void SkipChildren();

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public abstract string CurrentString();

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public abstract double CurrentDoubleValue();

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public abstract int CurrentIntValue();

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public abstract bool CurrentBooleanValue();
	}

	public static class JsonReaderConstants
	{
	}
}
