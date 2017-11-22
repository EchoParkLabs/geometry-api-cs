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
	internal sealed class JsonParserReader : com.epl.geometry.JsonReader
	{
		private org.codehaus.jackson.JsonParser m_jsonParser;

		internal JsonParserReader(org.codehaus.jackson.JsonParser jsonParser)
		{
			m_jsonParser = jsonParser;
		}

		/// <exception cref="System.Exception"/>
		internal override org.codehaus.jackson.JsonToken NextToken()
		{
			org.codehaus.jackson.JsonToken token = m_jsonParser.NextToken();
			return token;
		}

		/// <exception cref="System.Exception"/>
		internal override org.codehaus.jackson.JsonToken CurrentToken()
		{
			return m_jsonParser.GetCurrentToken();
		}

		/// <exception cref="System.Exception"/>
		internal override void SkipChildren()
		{
			m_jsonParser.SkipChildren();
		}

		/// <exception cref="System.Exception"/>
		internal override string CurrentString()
		{
			return m_jsonParser.GetText();
		}

		/// <exception cref="System.Exception"/>
		internal override double CurrentDoubleValue()
		{
			return m_jsonParser.GetValueAsDouble();
		}

		/// <exception cref="System.Exception"/>
		internal override int CurrentIntValue()
		{
			return m_jsonParser.GetValueAsInt();
		}
	}
}
