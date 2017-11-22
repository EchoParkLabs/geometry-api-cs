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
	internal sealed class JSONUtils
	{
		/// <exception cref="System.Exception"/>
		internal static bool IsObjectStart(com.epl.geometry.JsonReader parser)
		{
			return parser.CurrentToken() == null ? parser.NextToken() == org.codehaus.jackson.JsonToken.START_OBJECT : parser.CurrentToken() == org.codehaus.jackson.JsonToken.START_OBJECT;
		}

		/// <exception cref="org.codehaus.jackson.JsonParseException"/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		internal static double ReadDouble(com.epl.geometry.JsonReader parser)
		{
			if (parser.CurrentToken() == org.codehaus.jackson.JsonToken.VALUE_NUMBER_FLOAT)
			{
				return parser.CurrentDoubleValue();
			}
			else
			{
				if (parser.CurrentToken() == org.codehaus.jackson.JsonToken.VALUE_NUMBER_INT)
				{
					return parser.CurrentIntValue();
				}
				else
				{
					if (parser.CurrentToken() == org.codehaus.jackson.JsonToken.VALUE_NULL)
					{
						return com.epl.geometry.NumberUtils.NaN();
					}
					else
					{
						if (parser.CurrentToken() == org.codehaus.jackson.JsonToken.VALUE_STRING)
						{
							if (parser.CurrentString().Equals("NaN"))
							{
								return com.epl.geometry.NumberUtils.NaN();
							}
						}
					}
				}
			}
			throw new com.epl.geometry.GeometryException("invalid parameter");
		}
	}
}
