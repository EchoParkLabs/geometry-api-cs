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
	internal sealed class WktParser
	{
		internal abstract class WktToken
		{
			public const int not_available = 0;

			public const int empty = 50;

			public const int left_paren = 51;

			public const int right_paren = 52;

			public const int x_literal = unchecked((int)(0x80000000));

			public const int y_literal = unchecked((int)(0x40000000));

			public const int z_literal = unchecked((int)(0x20000000));

			public const int m_literal = unchecked((int)(0x10000000));

			public const int point = 1;

			public const int linestring = 2;

			public const int polygon = 3;

			public const int multipoint = 4;

			public const int multilinestring = 5;

			public const int multipolygon = 6;

			public const int geometrycollection = 7;

			public const int attribute_z = 1000;

			public const int attribute_m = 2000;

			public const int attribute_zm = 3000;
		}

		internal static class WktTokenConstants
		{
		}

		internal WktParser()
		{
		}

		internal WktParser(string @string)
		{
			ResetParser(@string);
		}

		internal void ResetParser(string @string)
		{
			if (m_function_stack == null)
			{
				m_function_stack = new com.epl.geometry.AttributeStreamOfInt32(0);
			}
			Reset_();
			m_wkt_string = @string;
		}

		internal int NextToken()
		{
			switch (m_function_stack.GetLast())
			{
				case com.epl.geometry.WktParser.State.xLiteral:
				{
					XLiteral_();
					break;
				}

				case com.epl.geometry.WktParser.State.yLiteral:
				{
					YLiteral_();
					break;
				}

				case com.epl.geometry.WktParser.State.zLiteral:
				{
					ZLiteral_();
					break;
				}

				case com.epl.geometry.WktParser.State.mLiteral:
				{
					MLiteral_();
					break;
				}

				case com.epl.geometry.WktParser.State.pointStart:
				{
					PointStart_();
					break;
				}

				case com.epl.geometry.WktParser.State.pointStartAlt:
				{
					PointStartAlt_();
					break;
				}

				case com.epl.geometry.WktParser.State.pointEnd:
				{
					PointEnd_();
					break;
				}

				case com.epl.geometry.WktParser.State.lineStringStart:
				{
					LineStringStart_();
					break;
				}

				case com.epl.geometry.WktParser.State.lineStringEnd:
				{
					LineStringEnd_();
					break;
				}

				case com.epl.geometry.WktParser.State.multiPointStart:
				{
					MultiPointStart_();
					break;
				}

				case com.epl.geometry.WktParser.State.multiPointEnd:
				{
					MultiPointEnd_();
					break;
				}

				case com.epl.geometry.WktParser.State.polygonStart:
				{
					PolygonStart_();
					break;
				}

				case com.epl.geometry.WktParser.State.polygonEnd:
				{
					PolygonEnd_();
					break;
				}

				case com.epl.geometry.WktParser.State.multiLineStringStart:
				{
					MultiLineStringStart_();
					break;
				}

				case com.epl.geometry.WktParser.State.multiLineStringEnd:
				{
					MultiLineStringEnd_();
					break;
				}

				case com.epl.geometry.WktParser.State.multiPolygonStart:
				{
					MultiPolygonStart_();
					break;
				}

				case com.epl.geometry.WktParser.State.multiPolygonEnd:
				{
					MultiPolygonEnd_();
					break;
				}

				case com.epl.geometry.WktParser.State.geometryCollectionStart:
				{
					GeometryCollectionStart_();
					break;
				}

				case com.epl.geometry.WktParser.State.geometryCollectionEnd:
				{
					GeometryCollectionEnd_();
					break;
				}

				case com.epl.geometry.WktParser.State.accept:
				{
					Accept_();
					break;
				}

				case com.epl.geometry.WktParser.State.geometry:
				{
					Geometry_();
					break;
				}

				case com.epl.geometry.WktParser.State.attributes:
				{
					Attributes_();
					break;
				}
			}
			return m_current_token_type;
		}

		internal double CurrentNumericLiteral()
		{
			if (((int)m_current_token_type & (int)com.epl.geometry.WktParser.Number.signed_numeric_literal) == 0)
			{
				throw new com.epl.geometry.GeometryException("runtime error");
			}
			if (m_b_nan)
			{
				return com.epl.geometry.NumberUtils.TheNaN;
			}
			double value = double.Parse(m_wkt_string.Substring(m_start_token, m_end_token - m_start_token));
			return value;
		}

		internal int CurrentToken()
		{
			return m_current_token_type;
		}

		internal bool HasZs()
		{
			return m_b_has_zs;
		}

		internal bool HasMs()
		{
			return m_b_has_ms;
		}

		private string m_wkt_string;

		private int m_start_token;

		private int m_end_token;

		private int m_current_token_type;

		private bool m_b_has_zs;

		private bool m_b_has_ms;

		private bool m_b_check_consistent_attributes;

		private bool m_b_nan;

		private com.epl.geometry.AttributeStreamOfInt32 m_function_stack;

		private abstract class State
		{
			public const int xLiteral = 0;

			public const int yLiteral = 1;

			public const int zLiteral = 2;

			public const int mLiteral = 3;

			public const int pointStart = 4;

			public const int pointStartAlt = 5;

			public const int pointEnd = 6;

			public const int lineStringStart = 7;

			public const int lineStringEnd = 8;

			public const int multiPointStart = 9;

			public const int multiPointEnd = 10;

			public const int polygonStart = 11;

			public const int polygonEnd = 12;

			public const int multiLineStringStart = 13;

			public const int multiLineStringEnd = 14;

			public const int multiPolygonStart = 15;

			public const int multiPolygonEnd = 16;

			public const int geometryCollectionStart = 17;

			public const int geometryCollectionEnd = 18;

			public const int accept = 19;

			public const int geometry = 20;

			public const int attributes = 21;
		}

		private static class StateConstants
		{
		}

		private abstract class Number
		{
			public const int signed_numeric_literal = com.epl.geometry.WktParser.WktToken.x_literal | com.epl.geometry.WktParser.WktToken.y_literal | com.epl.geometry.WktParser.WktToken.z_literal | com.epl.geometry.WktParser.WktToken.m_literal;
		}

		private static class NumberConstants
		{
		}

		private void Reset_()
		{
			m_function_stack.Add(com.epl.geometry.WktParser.State.accept);
			m_function_stack.Add(com.epl.geometry.WktParser.State.geometry);
			m_start_token = -1;
			m_end_token = 0;
			m_current_token_type = com.epl.geometry.WktParser.WktToken.not_available;
			m_b_has_zs = false;
			m_b_has_ms = false;
			m_b_check_consistent_attributes = false;
			m_b_nan = false;
		}

		private void Accept_()
		{
			m_start_token = m_end_token;
			m_current_token_type = com.epl.geometry.WktParser.WktToken.not_available;
		}

		private void Geometry_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			m_function_stack.RemoveLast();
			if (m_start_token + 5 <= m_wkt_string.Length && (m_wkt_string.IndexOf("point", m_start_token, "point".Length, System.StringComparison.OrdinalIgnoreCase) > -1))
			{
				m_end_token = m_start_token + 5;
				m_current_token_type = com.epl.geometry.WktParser.WktToken.point;
				m_function_stack.Add(com.epl.geometry.WktParser.State.pointStart);
			}
			else
			{
				if (m_start_token + 10 <= m_wkt_string.Length && (m_wkt_string.IndexOf("linestring", m_start_token, "linestring".Length, System.StringComparison.OrdinalIgnoreCase) > -1))
				{
					m_end_token = m_start_token + 10;
					m_current_token_type = com.epl.geometry.WktParser.WktToken.linestring;
					m_function_stack.Add(com.epl.geometry.WktParser.State.lineStringStart);
				}
				else
				{
					if (m_start_token + 10 <= m_wkt_string.Length && (m_wkt_string.IndexOf("multipoint", m_start_token, "multipoint".Length, System.StringComparison.OrdinalIgnoreCase) > -1))
					{
						m_end_token = m_start_token + 10;
						m_current_token_type = com.epl.geometry.WktParser.WktToken.multipoint;
						m_function_stack.Add(com.epl.geometry.WktParser.State.multiPointStart);
					}
					else
					{
						if (m_start_token + 7 <= m_wkt_string.Length && (m_wkt_string.IndexOf("polygon", m_start_token, "polygon".Length, System.StringComparison.OrdinalIgnoreCase) > -1))
						{
							m_end_token = m_start_token + 7;
							m_current_token_type = com.epl.geometry.WktParser.WktToken.polygon;
							m_function_stack.Add(com.epl.geometry.WktParser.State.polygonStart);
						}
						else
						{
							if (m_start_token + 15 <= m_wkt_string.Length && (m_wkt_string.IndexOf("multilinestring", m_start_token, "multilinestring".Length, System.StringComparison.OrdinalIgnoreCase) > -1))
							{
								m_end_token = m_start_token + 15;
								m_current_token_type = com.epl.geometry.WktParser.WktToken.multilinestring;
								m_function_stack.Add(com.epl.geometry.WktParser.State.multiLineStringStart);
							}
							else
							{
								if (m_start_token + 12 <= m_wkt_string.Length && (m_wkt_string.IndexOf("multipolygon", m_start_token, "multipolygon".Length, System.StringComparison.OrdinalIgnoreCase) > -1))
								{
									m_end_token = m_start_token + 12;
									m_current_token_type = com.epl.geometry.WktParser.WktToken.multipolygon;
									m_function_stack.Add(com.epl.geometry.WktParser.State.multiPolygonStart);
								}
								else
								{
									if (m_start_token + 18 <= m_wkt_string.Length && (m_wkt_string.IndexOf("geometrycollection", m_start_token, "geometrycollection".Length, System.StringComparison.OrdinalIgnoreCase) > -1))
									{
										m_end_token = m_start_token + 18;
										m_current_token_type = com.epl.geometry.WktParser.WktToken.geometrycollection;
										m_function_stack.Add(com.epl.geometry.WktParser.State.geometryCollectionStart);
									}
									else
									{
										//String snippet = (m_wkt_string.length() > 200 ? m_wkt_string
										//		.substring(0, 200) + "..." : m_wkt_string);
										//throw new IllegalArgumentException(
										//		"Could not parse Well-Known Text: " + snippet);
										throw new System.ArgumentException("Could not parse Well-Known Text around position: " + m_end_token);
									}
								}
							}
						}
					}
				}
			}
			m_function_stack.Add(com.epl.geometry.WktParser.State.attributes);
		}

		private void Attributes_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			m_function_stack.RemoveLast();
			// Z and M is not allowed to have a space between them
			bool b_has_zs = false;
			bool b_has_ms = false;
			if (m_wkt_string[m_end_token] == 'z' || m_wkt_string[m_end_token] == 'Z')
			{
				b_has_zs = true;
				if (++m_end_token >= m_wkt_string.Length)
				{
					throw new System.ArgumentException();
				}
			}
			if (m_wkt_string[m_end_token] == 'm' || m_wkt_string[m_end_token] == 'M')
			{
				b_has_ms = true;
				if (++m_end_token >= m_wkt_string.Length)
				{
					throw new System.ArgumentException();
				}
			}
			if (m_b_check_consistent_attributes)
			{
				if (b_has_zs != m_b_has_zs || b_has_ms != m_b_has_ms)
				{
					throw new System.ArgumentException();
				}
			}
			m_b_has_zs = b_has_zs;
			m_b_has_ms = b_has_ms;
			if (m_b_has_zs || m_b_has_ms)
			{
				if (m_b_has_zs && !m_b_has_ms)
				{
					m_current_token_type = com.epl.geometry.WktParser.WktToken.attribute_z;
				}
				else
				{
					if (m_b_has_ms && !m_b_has_zs)
					{
						m_current_token_type = com.epl.geometry.WktParser.WktToken.attribute_m;
					}
					else
					{
						m_current_token_type = com.epl.geometry.WktParser.WktToken.attribute_zm;
					}
				}
			}
			else
			{
				NextToken();
			}
		}

		private void GeometryCollectionStart_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			m_b_check_consistent_attributes = true;
			if (Empty_())
			{
				m_function_stack.RemoveLast();
			}
			else
			{
				if (LeftParen_())
				{
					m_function_stack.RemoveLast();
					m_function_stack.Add(com.epl.geometry.WktParser.State.geometryCollectionEnd);
					m_function_stack.Add(com.epl.geometry.WktParser.State.geometry);
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}

		private void GeometryCollectionEnd_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			if (Comma_())
			{
				m_function_stack.Add(com.epl.geometry.WktParser.State.geometry);
				Geometry_();
			}
			else
			{
				if (RightParen_())
				{
					m_function_stack.RemoveLast();
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}

		private void MultiPolygonStart_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			if (Empty_())
			{
				m_function_stack.RemoveLast();
			}
			else
			{
				if (LeftParen_())
				{
					m_function_stack.RemoveLast();
					m_function_stack.Add(com.epl.geometry.WktParser.State.multiPolygonEnd);
					m_function_stack.Add(com.epl.geometry.WktParser.State.polygonStart);
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}

		private void MultiPolygonEnd_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			if (Comma_())
			{
				m_function_stack.Add(com.epl.geometry.WktParser.State.polygonStart);
				PolygonStart_();
			}
			else
			{
				if (RightParen_())
				{
					m_function_stack.RemoveLast();
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}

		private void MultiLineStringStart_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			if (Empty_())
			{
				m_function_stack.RemoveLast();
			}
			else
			{
				if (LeftParen_())
				{
					m_function_stack.RemoveLast();
					m_function_stack.Add(com.epl.geometry.WktParser.State.multiLineStringEnd);
					m_function_stack.Add(com.epl.geometry.WktParser.State.lineStringStart);
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}

		private void MultiLineStringEnd_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			if (Comma_())
			{
				m_function_stack.Add(com.epl.geometry.WktParser.State.lineStringStart);
				LineStringStart_();
			}
			else
			{
				if (RightParen_())
				{
					m_function_stack.RemoveLast();
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}

		private void MultiPointStart_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			if (Empty_())
			{
				m_function_stack.RemoveLast();
			}
			else
			{
				if (LeftParen_())
				{
					m_function_stack.RemoveLast();
					m_function_stack.Add(com.epl.geometry.WktParser.State.multiPointEnd);
					m_function_stack.Add(com.epl.geometry.WktParser.State.pointStartAlt);
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}

		private void MultiPointEnd_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			if (Comma_())
			{
				m_function_stack.Add(com.epl.geometry.WktParser.State.pointStart);
				PointStart_();
			}
			else
			{
				if (RightParen_())
				{
					m_function_stack.RemoveLast();
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}

		private void PolygonStart_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			if (Empty_())
			{
				m_function_stack.RemoveLast();
			}
			else
			{
				if (LeftParen_())
				{
					m_function_stack.RemoveLast();
					m_function_stack.Add(com.epl.geometry.WktParser.State.polygonEnd);
					m_function_stack.Add(com.epl.geometry.WktParser.State.lineStringStart);
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}

		private void PolygonEnd_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			if (Comma_())
			{
				m_function_stack.Add(com.epl.geometry.WktParser.State.lineStringStart);
				LineStringStart_();
			}
			else
			{
				if (RightParen_())
				{
					m_function_stack.RemoveLast();
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}

		private void LineStringStart_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			if (Empty_())
			{
				m_function_stack.RemoveLast();
			}
			else
			{
				if (LeftParen_())
				{
					m_function_stack.RemoveLast();
					m_function_stack.Add(com.epl.geometry.WktParser.State.lineStringEnd);
					m_function_stack.Add(com.epl.geometry.WktParser.State.xLiteral);
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}

		private void LineStringEnd_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			if (Comma_())
			{
				m_function_stack.Add(com.epl.geometry.WktParser.State.xLiteral);
				XLiteral_();
			}
			else
			{
				if (RightParen_())
				{
					m_function_stack.RemoveLast();
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}

		private void PointStart_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			if (Empty_())
			{
				m_function_stack.RemoveLast();
			}
			else
			{
				if (LeftParen_())
				{
					m_function_stack.RemoveLast();
					m_function_stack.Add(com.epl.geometry.WktParser.State.pointEnd);
					m_function_stack.Add(com.epl.geometry.WktParser.State.xLiteral);
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}

		private void PointStartAlt_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			if (Empty_())
			{
				// ogc standard
				m_function_stack.RemoveLast();
			}
			else
			{
				if (LeftParen_())
				{
					// ogc standard
					m_function_stack.RemoveLast();
					m_function_stack.Add(com.epl.geometry.WktParser.State.pointEnd);
					m_function_stack.Add(com.epl.geometry.WktParser.State.xLiteral);
				}
				else
				{
					// not ogc standard. treat as linestring
					m_function_stack.RemoveLast();
					m_function_stack.RemoveLast();
					m_function_stack.Add(com.epl.geometry.WktParser.State.lineStringEnd);
					m_function_stack.Add(com.epl.geometry.WktParser.State.xLiteral);
					NextToken();
				}
			}
		}

		private void PointEnd_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			if (RightParen_())
			{
				m_function_stack.RemoveLast();
			}
			else
			{
				throw new System.ArgumentException();
			}
		}

		private void XLiteral_()
		{
			SignedNumericLiteral_();
			m_current_token_type = com.epl.geometry.WktParser.WktToken.x_literal;
			m_function_stack.RemoveLast();
			m_function_stack.Add(com.epl.geometry.WktParser.State.yLiteral);
		}

		private void YLiteral_()
		{
			SignedNumericLiteral_();
			m_current_token_type = com.epl.geometry.WktParser.WktToken.y_literal;
			m_function_stack.RemoveLast();
			if (m_b_has_zs)
			{
				m_function_stack.Add(com.epl.geometry.WktParser.State.zLiteral);
			}
			else
			{
				if (m_b_has_ms)
				{
					m_function_stack.Add(com.epl.geometry.WktParser.State.mLiteral);
				}
			}
		}

		private void ZLiteral_()
		{
			SignedNumericLiteral_();
			m_current_token_type = com.epl.geometry.WktParser.WktToken.z_literal;
			m_function_stack.RemoveLast();
			if (m_b_has_ms)
			{
				m_function_stack.Add(com.epl.geometry.WktParser.State.mLiteral);
			}
		}

		private void MLiteral_()
		{
			SignedNumericLiteral_();
			m_current_token_type = com.epl.geometry.WktParser.WktToken.m_literal;
			m_function_stack.RemoveLast();
		}

		private bool Nan_()
		{
			if (m_start_token + 3 <= m_wkt_string.Length && (m_wkt_string.IndexOf("nan", m_start_token, "nan".Length, System.StringComparison.OrdinalIgnoreCase) > -1))
			{
				m_end_token += 3;
				m_b_nan = true;
				return true;
			}
			m_b_nan = false;
			return false;
		}

		private void Sign_()
		{
			// Optional - or + sign
			if (m_wkt_string[m_end_token] == '-' || m_wkt_string[m_end_token] == '+')
			{
				if (++m_end_token >= m_wkt_string.Length)
				{
					throw new System.ArgumentException();
				}
			}
		}

		private void SignedNumericLiteral_()
		{
			SkipWhiteSpace_();
			m_start_token = m_end_token;
			if (Nan_())
			{
				return;
			}
			Sign_();
			// Optional
			UnsignedNumericLiteral_();
		}

		private void UnsignedNumericLiteral_()
		{
			ExactNumericLiteral_();
			Exp_();
		}

		// Optional
		private void ExactNumericLiteral_()
		{
			if (char.IsDigit(m_wkt_string[m_end_token]))
			{
				Digits_();
				// Optional
				if (m_wkt_string[m_end_token] == '.')
				{
					if (++m_end_token >= m_wkt_string.Length)
					{
						throw new System.ArgumentException();
					}
					// Optional
					if (char.IsDigit(m_wkt_string[m_end_token]))
					{
						Digits_();
					}
				}
			}
			else
			{
				if (m_wkt_string[m_end_token] == '.')
				{
					if (++m_end_token >= m_wkt_string.Length)
					{
						throw new System.ArgumentException();
					}
					if (!char.IsDigit(m_wkt_string[m_end_token]))
					{
						throw new System.ArgumentException();
					}
					Digits_();
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}

		private void Digits_()
		{
			do
			{
				if (++m_end_token >= m_wkt_string.Length)
				{
					throw new System.ArgumentException();
				}
			}
			while (char.IsDigit(m_wkt_string[m_end_token]));
		}

		private void Exp_()
		{
			// This is an optional state
			if (m_wkt_string[m_end_token] == 'e' || m_wkt_string[m_end_token] == 'E')
			{
				if (++m_end_token >= m_wkt_string.Length)
				{
					throw new System.ArgumentException();
				}
				Sign_();
				// optional
				if (!char.IsDigit(m_wkt_string[m_end_token]))
				{
					throw new System.ArgumentException();
				}
				Digits_();
			}
		}

		private void SkipWhiteSpace_()
		{
			if (m_end_token >= m_wkt_string.Length)
			{
				throw new System.ArgumentException();
			}
			while (char.IsWhiteSpace(m_wkt_string[m_end_token]))
			{
				if (++m_end_token >= m_wkt_string.Length)
				{
					throw new System.ArgumentException();
				}
			}
		}

		private bool Empty_()
		{
			if (m_start_token + 5 <= m_wkt_string.Length && (m_wkt_string.IndexOf("empty", m_start_token, "empty".Length, System.StringComparison.OrdinalIgnoreCase) > -1))
			{
				m_end_token += 5;
				m_current_token_type = com.epl.geometry.WktParser.WktToken.empty;
				return true;
			}
			return false;
		}

		private bool Comma_()
		{
			if (m_wkt_string[m_end_token] == ',')
			{
				m_end_token++;
				return true;
			}
			return false;
		}

		private bool LeftParen_()
		{
			if (m_wkt_string[m_end_token] == '(')
			{
				m_end_token++;
				m_current_token_type = com.epl.geometry.WktParser.WktToken.left_paren;
				return true;
			}
			return false;
		}

		private bool RightParen_()
		{
			if (m_wkt_string[m_end_token] == ')')
			{
				m_end_token++;
				m_current_token_type = com.epl.geometry.WktParser.WktToken.right_paren;
				return true;
			}
			return false;
		}
	}
}
