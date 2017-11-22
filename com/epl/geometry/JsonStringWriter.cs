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
	internal sealed class JsonStringWriter : com.epl.geometry.JsonWriter
	{
		internal override object GetJson()
		{
			Next_(com.epl.geometry.JsonWriter.Action.accept);
			return m_jsonString.ToString();
		}

		internal override void StartObject()
		{
			Next_(com.epl.geometry.JsonWriter.Action.addContainer);
			m_jsonString.Append('{');
			m_functionStack.Add(com.epl.geometry.JsonWriter.State.objectStart);
		}

		internal override void StartArray()
		{
			Next_(com.epl.geometry.JsonWriter.Action.addContainer);
			m_jsonString.Append('[');
			m_functionStack.Add(com.epl.geometry.JsonWriter.State.arrayStart);
		}

		internal override void EndObject()
		{
			Next_(com.epl.geometry.JsonWriter.Action.popObject);
			m_jsonString.Append('}');
		}

		internal override void EndArray()
		{
			Next_(com.epl.geometry.JsonWriter.Action.popArray);
			m_jsonString.Append(']');
		}

		internal override void AddPairObject(string fieldName)
		{
			Next_(com.epl.geometry.JsonWriter.Action.addPair);
			AppendQuote_(fieldName);
			m_jsonString.Append(":");
			AddValueObject_();
		}

		internal override void AddPairArray(string fieldName)
		{
			Next_(com.epl.geometry.JsonWriter.Action.addPair);
			AppendQuote_(fieldName);
			m_jsonString.Append(":");
			AddValueArray_();
		}

		internal override void AddPairString(string fieldName, string v)
		{
			Next_(com.epl.geometry.JsonWriter.Action.addPair);
			AppendQuote_(fieldName);
			m_jsonString.Append(":");
			AddValueString_(v);
		}

		internal override void AddPairDouble(string fieldName, double v)
		{
			Next_(com.epl.geometry.JsonWriter.Action.addPair);
			AppendQuote_(fieldName);
			m_jsonString.Append(":");
			AddValueDouble_(v);
		}

		internal override void AddPairDoubleF(string fieldName, double v, int decimals)
		{
			Next_(com.epl.geometry.JsonWriter.Action.addPair);
			AppendQuote_(fieldName);
			m_jsonString.Append(":");
			AddValueDoubleF_(v, decimals);
		}

		internal override void AddPairInt(string fieldName, int v)
		{
			Next_(com.epl.geometry.JsonWriter.Action.addPair);
			AppendQuote_(fieldName);
			m_jsonString.Append(":");
			AddValueInt_(v);
		}

		internal override void AddPairBoolean(string fieldName, bool v)
		{
			Next_(com.epl.geometry.JsonWriter.Action.addPair);
			AppendQuote_(fieldName);
			m_jsonString.Append(":");
			AddValueBoolean_(v);
		}

		internal override void AddPairNull(string fieldName)
		{
			Next_(com.epl.geometry.JsonWriter.Action.addPair);
			AppendQuote_(fieldName);
			m_jsonString.Append(":");
			AddValueNull_();
		}

		internal override void AddValueObject()
		{
			Next_(com.epl.geometry.JsonWriter.Action.addContainer);
			AddValueObject_();
		}

		internal override void AddValueArray()
		{
			Next_(com.epl.geometry.JsonWriter.Action.addContainer);
			AddValueArray_();
		}

		internal override void AddValueString(string v)
		{
			Next_(com.epl.geometry.JsonWriter.Action.addTerminal);
			AddValueString_(v);
		}

		internal override void AddValueDouble(double v)
		{
			Next_(com.epl.geometry.JsonWriter.Action.addTerminal);
			AddValueDouble_(v);
		}

		internal override void AddValueDoubleF(double v, int decimals)
		{
			Next_(com.epl.geometry.JsonWriter.Action.addTerminal);
			AddValueDoubleF_(v, decimals);
		}

		internal override void AddValueInt(int v)
		{
			Next_(com.epl.geometry.JsonWriter.Action.addTerminal);
			AddValueInt_(v);
		}

		internal override void AddValueBoolean(bool v)
		{
			Next_(com.epl.geometry.JsonWriter.Action.addTerminal);
			AddValueBoolean_(v);
		}

		internal override void AddValueNull()
		{
			Next_(com.epl.geometry.JsonWriter.Action.addTerminal);
			AddValueNull_();
		}

		internal JsonStringWriter()
		{
			m_jsonString = new System.Text.StringBuilder();
			m_functionStack = new com.epl.geometry.AttributeStreamOfInt32(0);
			m_functionStack.Add(com.epl.geometry.JsonWriter.State.accept);
			m_functionStack.Add(com.epl.geometry.JsonWriter.State.start);
		}

		private System.Text.StringBuilder m_jsonString;

		private com.epl.geometry.AttributeStreamOfInt32 m_functionStack;

		private void AddValueObject_()
		{
			m_jsonString.Append('{');
			m_functionStack.Add(com.epl.geometry.JsonWriter.State.objectStart);
		}

		private void AddValueArray_()
		{
			m_jsonString.Append('[');
			m_functionStack.Add(com.epl.geometry.JsonWriter.State.arrayStart);
		}

		private void AddValueString_(string v)
		{
			AppendQuote_(v);
		}

		private void AddValueDouble_(double v)
		{
			if (com.epl.geometry.NumberUtils.IsNaN(v))
			{
				AddValueNull_();
				return;
			}
			com.epl.geometry.StringUtils.AppendDouble(v, 17, m_jsonString);
		}

		private void AddValueDoubleF_(double v, int decimals)
		{
			if (com.epl.geometry.NumberUtils.IsNaN(v))
			{
				AddValueNull_();
				return;
			}
			com.epl.geometry.StringUtils.AppendDoubleF(v, decimals, m_jsonString);
		}

		private void AddValueInt_(int v)
		{
			m_jsonString.Append(v);
		}

		private void AddValueBoolean_(bool v)
		{
			if (v)
			{
				m_jsonString.Append("true");
			}
			else
			{
				m_jsonString.Append("false");
			}
		}

		private void AddValueNull_()
		{
			m_jsonString.Append("null");
		}

		private void Next_(int action)
		{
			switch (m_functionStack.GetLast())
			{
				case com.epl.geometry.JsonWriter.State.accept:
				{
					Accept_(action);
					break;
				}

				case com.epl.geometry.JsonWriter.State.start:
				{
					Start_(action);
					break;
				}

				case com.epl.geometry.JsonWriter.State.objectStart:
				{
					ObjectStart_(action);
					break;
				}

				case com.epl.geometry.JsonWriter.State.arrayStart:
				{
					ArrayStart_(action);
					break;
				}

				case com.epl.geometry.JsonWriter.State.pairEnd:
				{
					PairEnd_(action);
					break;
				}

				case com.epl.geometry.JsonWriter.State.elementEnd:
				{
					ElementEnd_(action);
					break;
				}

				default:
				{
					throw new com.epl.geometry.GeometryException("internal error");
				}
			}
		}

		private void Accept_(int action)
		{
			if (action != com.epl.geometry.JsonWriter.Action.accept)
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
		}

		private void Start_(int action)
		{
			if (action == com.epl.geometry.JsonWriter.Action.addContainer)
			{
				m_functionStack.RemoveLast();
			}
			else
			{
				throw new com.epl.geometry.GeometryException("invalid call");
			}
		}

		private void ObjectStart_(int action)
		{
			m_functionStack.RemoveLast();
			if (action == com.epl.geometry.JsonWriter.Action.addPair)
			{
				m_functionStack.Add(com.epl.geometry.JsonWriter.State.pairEnd);
			}
			else
			{
				if (action != com.epl.geometry.JsonWriter.Action.popObject)
				{
					throw new com.epl.geometry.GeometryException("invalid call");
				}
			}
		}

		private void PairEnd_(int action)
		{
			if (action == com.epl.geometry.JsonWriter.Action.addPair)
			{
				m_jsonString.Append(',');
			}
			else
			{
				if (action == com.epl.geometry.JsonWriter.Action.popObject)
				{
					m_functionStack.RemoveLast();
				}
				else
				{
					throw new com.epl.geometry.GeometryException("invalid call");
				}
			}
		}

		private void ArrayStart_(int action)
		{
			m_functionStack.RemoveLast();
			if ((action & com.epl.geometry.JsonWriter.Action.addValue) != 0)
			{
				m_functionStack.Add(com.epl.geometry.JsonWriter.State.elementEnd);
			}
			else
			{
				if (action != com.epl.geometry.JsonWriter.Action.popArray)
				{
					throw new com.epl.geometry.GeometryException("invalid call");
				}
			}
		}

		private void ElementEnd_(int action)
		{
			if ((action & com.epl.geometry.JsonWriter.Action.addValue) != 0)
			{
				m_jsonString.Append(',');
			}
			else
			{
				if (action == com.epl.geometry.JsonWriter.Action.popArray)
				{
					m_functionStack.RemoveLast();
				}
				else
				{
					throw new com.epl.geometry.GeometryException("invalid call");
				}
			}
		}

		private void AppendQuote_(string @string)
		{
			int count = 0;
			int start = 0;
			int end = @string.Length;
			m_jsonString.Append('"');
			for (int i = 0; i < end; i++)
			{
				switch (@string[i])
				{
					case '"':
					{
						if (count > 0)
						{
							m_jsonString.Append(@string, start, start + count);
							count = 0;
						}
						m_jsonString.Append("\\\"");
						start = i + 1;
						break;
					}

					case '\\':
					{
						if (count > 0)
						{
							m_jsonString.Append(@string, start, start + count);
							count = 0;
						}
						m_jsonString.Append("\\\\");
						start = i + 1;
						break;
					}

					case '/':
					{
						if (i > 0 && @string[i - 1] == '<')
						{
							if (count > 0)
							{
								m_jsonString.Append(@string, start, start + count);
								count = 0;
							}
							m_jsonString.Append("\\/");
							start = i + 1;
						}
						else
						{
							count++;
						}
						break;
					}

					case '\b':
					{
						if (count > 0)
						{
							m_jsonString.Append(@string, start, start + count);
							count = 0;
						}
						m_jsonString.Append("\\b");
						start = i + 1;
						break;
					}

					case '\f':
					{
						if (count > 0)
						{
							m_jsonString.Append(@string, start, start + count);
							count = 0;
						}
						m_jsonString.Append("\\f");
						start = i + 1;
						break;
					}

					case '\n':
					{
						if (count > 0)
						{
							m_jsonString.Append(@string, start, start + count);
							count = 0;
						}
						m_jsonString.Append("\\n");
						start = i + 1;
						break;
					}

					case '\r':
					{
						if (count > 0)
						{
							m_jsonString.Append(@string, start, start + count);
							count = 0;
						}
						m_jsonString.Append("\\r");
						start = i + 1;
						break;
					}

					case '\t':
					{
						if (count > 0)
						{
							m_jsonString.Append(@string, start, start + count);
							count = 0;
						}
						m_jsonString.Append("\\t");
						start = i + 1;
						break;
					}

					default:
					{
						count++;
						break;
					}
				}
			}
			if (count > 0)
			{
				m_jsonString.Append(@string, start, start + count);
			}
			m_jsonString.Append('"');
		}
	}
}
