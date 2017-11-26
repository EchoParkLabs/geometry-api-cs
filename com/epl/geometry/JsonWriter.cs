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
	internal abstract class JsonWriter
	{
		internal abstract object GetJson();

		internal abstract void StartObject();

		internal abstract void StartArray();

		internal abstract void EndObject();

		internal abstract void EndArray();

		internal abstract void AddFieldName(string fieldName);

		internal abstract void AddPairObject(string fieldName);

		internal abstract void AddPairArray(string fieldName);

		internal abstract void AddPairString(string fieldName, string v);

		internal abstract void AddPairDouble(string fieldName, double v);

		internal abstract void AddPairDouble(string fieldName, double v, int precision, bool bFixedPoint);

		internal abstract void AddPairInt(string fieldName, int v);

		internal abstract void AddPairBoolean(string fieldName, bool v);

		internal abstract void AddPairNull(string fieldName);

		internal abstract void AddValueObject();

		internal abstract void AddValueArray();

		internal abstract void AddValueString(string v);

		internal abstract void AddValueDouble(double v);

		internal abstract void AddValueDouble(double v, int precision, bool bFixedPoint);

		internal abstract void AddValueInt(int v);

		internal abstract void AddValueBoolean(bool v);

		internal abstract void AddValueNull();

		protected internal abstract class Action
		{
			public const int accept = 0;

			public const int addObject = 1;

			public const int addArray = 2;

			public const int popObject = 4;

			public const int popArray = 8;

			public const int addKey = 16;

			public const int addTerminal = 32;

			public const int addPair = 64;

			public const int addContainer = addObject | addArray;

			public const int addValue = addContainer | addTerminal;
		}

		protected internal static class ActionConstants
		{
		}

		protected internal abstract class State
		{
			public const int accept = 0;

			public const int start = 1;

			public const int objectStart = 2;

			public const int arrayStart = 3;

			public const int pairEnd = 4;

			public const int elementEnd = 5;

			public const int fieldNameEnd = 6;
		}

		protected internal static class StateConstants
		{
		}
	}
}
