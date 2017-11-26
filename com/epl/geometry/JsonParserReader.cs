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
	/// <summary>A throw in JsonReader built around the Jackson JsonParser.</summary>
	public class JsonParserReader : com.epl.geometry.JsonReader
	{
		private com.fasterxml.jackson.core.JsonParser m_jsonParser;

		public JsonParserReader(com.fasterxml.jackson.core.JsonParser jsonParser)
		{
			m_jsonParser = jsonParser;
		}

		/// <summary>Creates a JsonReader for the string.</summary>
		/// <remarks>
		/// Creates a JsonReader for the string.
		/// The nextToken is called by this method.
		/// </remarks>
		public static com.epl.geometry.JsonReader CreateFromString(string str)
		{
			try
			{
				com.fasterxml.jackson.core.JsonFactory factory = new com.fasterxml.jackson.core.JsonFactory();
				com.fasterxml.jackson.core.JsonParser jsonParser = factory.CreateParser(str);
				jsonParser.NextToken();
				return new com.epl.geometry.JsonParserReader(jsonParser);
			}
			catch (System.Exception ex)
			{
				throw new com.epl.geometry.JsonGeometryException(ex.Message);
			}
		}

		/// <summary>Creates a JsonReader for the string.</summary>
		/// <remarks>
		/// Creates a JsonReader for the string.
		/// The nextToken is not called by this method.
		/// </remarks>
		public static com.epl.geometry.JsonReader CreateFromStringNNT(string str)
		{
			try
			{
				com.fasterxml.jackson.core.JsonFactory factory = new com.fasterxml.jackson.core.JsonFactory();
				com.fasterxml.jackson.core.JsonParser jsonParser = factory.CreateParser(str);
				return new com.epl.geometry.JsonParserReader(jsonParser);
			}
			catch (System.Exception ex)
			{
				throw new com.epl.geometry.JsonGeometryException(ex.Message);
			}
		}

		private static com.epl.geometry.JsonReader.Token MapToken(com.fasterxml.jackson.core.JsonToken token)
		{
			if (token == com.fasterxml.jackson.core.JsonToken.END_ARRAY)
			{
				return com.epl.geometry.JsonReader.Token.END_ARRAY;
			}
			if (token == com.fasterxml.jackson.core.JsonToken.END_OBJECT)
			{
				return com.epl.geometry.JsonReader.Token.END_OBJECT;
			}
			if (token == com.fasterxml.jackson.core.JsonToken.FIELD_NAME)
			{
				return com.epl.geometry.JsonReader.Token.FIELD_NAME;
			}
			if (token == com.fasterxml.jackson.core.JsonToken.START_ARRAY)
			{
				return com.epl.geometry.JsonReader.Token.START_ARRAY;
			}
			if (token == com.fasterxml.jackson.core.JsonToken.START_OBJECT)
			{
				return com.epl.geometry.JsonReader.Token.START_OBJECT;
			}
			if (token == com.fasterxml.jackson.core.JsonToken.VALUE_FALSE)
			{
				return com.epl.geometry.JsonReader.Token.VALUE_FALSE;
			}
			if (token == com.fasterxml.jackson.core.JsonToken.VALUE_NULL)
			{
				return com.epl.geometry.JsonReader.Token.VALUE_NULL;
			}
			if (token == com.fasterxml.jackson.core.JsonToken.VALUE_NUMBER_FLOAT)
			{
				return com.epl.geometry.JsonReader.Token.VALUE_NUMBER_FLOAT;
			}
			if (token == com.fasterxml.jackson.core.JsonToken.VALUE_NUMBER_INT)
			{
				return com.epl.geometry.JsonReader.Token.VALUE_NUMBER_INT;
			}
			if (token == com.fasterxml.jackson.core.JsonToken.VALUE_STRING)
			{
				return com.epl.geometry.JsonReader.Token.VALUE_STRING;
			}
			if (token == com.fasterxml.jackson.core.JsonToken.VALUE_TRUE)
			{
				return com.epl.geometry.JsonReader.Token.VALUE_TRUE;
			}
			if (token == null)
			{
				return null;
			}
			throw new com.epl.geometry.JsonGeometryException("unexpected token");
		}

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public override com.epl.geometry.JsonReader.Token NextToken()
		{
			try
			{
				com.fasterxml.jackson.core.JsonToken token = m_jsonParser.NextToken();
				return MapToken(token);
			}
			catch (System.Exception ex)
			{
				throw new com.epl.geometry.JsonGeometryException(ex);
			}
		}

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public override com.epl.geometry.JsonReader.Token CurrentToken()
		{
			try
			{
				return MapToken(m_jsonParser.GetCurrentToken());
			}
			catch (System.Exception ex)
			{
				throw new com.epl.geometry.JsonGeometryException(ex);
			}
		}

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public override void SkipChildren()
		{
			try
			{
				m_jsonParser.SkipChildren();
			}
			catch (System.Exception ex)
			{
				throw new com.epl.geometry.JsonGeometryException(ex);
			}
		}

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public override string CurrentString()
		{
			try
			{
				return m_jsonParser.GetText();
			}
			catch (System.Exception ex)
			{
				throw new com.epl.geometry.JsonGeometryException(ex);
			}
		}

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public override double CurrentDoubleValue()
		{
			try
			{
				return m_jsonParser.GetValueAsDouble();
			}
			catch (System.Exception ex)
			{
				throw new com.epl.geometry.JsonGeometryException(ex);
			}
		}

		/// <exception cref="com.epl.geometry.JsonGeometryException"/>
		public override int CurrentIntValue()
		{
			try
			{
				return m_jsonParser.GetValueAsInt();
			}
			catch (System.Exception ex)
			{
				throw new com.epl.geometry.JsonGeometryException(ex);
			}
		}

		public override bool CurrentBooleanValue()
		{
			com.epl.geometry.JsonReader.Token t = CurrentToken();
			if (t == com.epl.geometry.JsonReader.Token.VALUE_TRUE)
			{
				return true;
			}
			else
			{
				if (t == com.epl.geometry.JsonReader.Token.VALUE_FALSE)
				{
					return false;
				}
			}
			throw new com.epl.geometry.JsonGeometryException("Not a boolean");
		}
	}
}
