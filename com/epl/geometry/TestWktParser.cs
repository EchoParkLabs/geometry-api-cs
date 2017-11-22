

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	public class TestWktParser
	{
		public virtual void testGeometryCollection()
		{
			string s = "   geometrycollection    emPty ";
			com.esri.core.geometry.WktParser wktParser = new com.esri.core.geometry.WktParser
				();
			wktParser.resetParser(s);
			int currentToken;
			double value;
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.geometrycollection);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.not_available);
			s = "   geometrycollection zm (  geometrycollection zm ( POINT ZM ( 5.  +1.e+0004 13 17) ), LineString  zm  emPty, MULTIpolyGON zM (((1 1 1 1))) ) ";
			wktParser.resetParser(s);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.geometrycollection);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.attribute_zm);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.geometrycollection);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.attribute_zm);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.point);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.attribute_zm);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 5.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 1.0e4);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 17);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.linestring);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.attribute_zm);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.multipolygon);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.attribute_zm);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.not_available);
		}

		public virtual void testMultiPolygon()
		{
			string s = "   MultIPolYgOn    emPty ";
			com.esri.core.geometry.WktParser wktParser = new com.esri.core.geometry.WktParser
				();
			wktParser.resetParser(s);
			int currentToken;
			double value;
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.multipolygon);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			s = "  MULTIpolyGON zM ( empty , (  empty, ( 5.  +1.e+0004  13 17, -.1e07  .006 13 17 ) , empty  , (4  003. 13 17, 02E-3 .3e2 13 17)  ) , empty, ( ( 5.  +1.e+0004  13 17, -.1e07  .006  13 17) , (4  003. 13 17 , 02E-3 .3e2 13 17)  ) )   ";
			wktParser.resetParser(s);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.multipolygon);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.attribute_zm);
			// Start first polygon
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 5.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 1.0e4);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 17);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == -0.1e7);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 0.006);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 17);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 4.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 3.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 17);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 2.0e-3);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 0.3e2);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 17);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			// End of First polygon
			// Start Second Polygon
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 5.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 1.0e4);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 17);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == -0.1e7);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 0.006);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 17);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 4.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 3.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 17);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 2.0e-3);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 0.3e2);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 17);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.not_available);
		}

		public virtual void testMultiLineString()
		{
			string s = "   MultiLineString    emPty ";
			com.esri.core.geometry.WktParser wktParser = new com.esri.core.geometry.WktParser
				();
			wktParser.resetParser(s);
			int currentToken;
			double value;
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.multilinestring);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			s = "  MultiLineString Z (  empty, ( 5.  +1.e+0004  13, -.1e07  .006 13 ) , empty  , (4  003. 13 , 02E-3 .3e2 13 ) , empty, ( 5.  +1.e+0004  13 , -.1e07  .006  13) , (4  003. 13 , 02E-3 .3e2 13 )  )   ";
			wktParser.resetParser(s);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.multilinestring);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.attribute_z);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 5.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 1.0e4);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == -0.1e7);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 0.006);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 4.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 3.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 2.0e-3);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 0.3e2);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 5.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 1.0e4);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == -0.1e7);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 0.006);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 4.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 3.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 2.0e-3);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 0.3e2);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.not_available);
		}

		public virtual void testMultiPoint()
		{
			string s = "   MultipoInt    emPty ";
			com.esri.core.geometry.WktParser wktParser = new com.esri.core.geometry.WktParser
				();
			wktParser.resetParser(s);
			int currentToken;
			double value;
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.multipoint);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			s = "  Multipoint Z (  empty, ( 5.  +1.e+0004  13 ), (-.1e07  .006 13 ) , empty  , (4  003. 13 ), (02E-3 .3e2 13 )  )   ";
			wktParser.resetParser(s);
			currentToken = wktParser.nextToken();
			// bug here
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.multipoint);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.attribute_z);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 5.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 1.0e4);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == -0.1e7);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 0.006);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 4.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 3.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 2.0e-3);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 0.3e2);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.not_available);
		}

		public virtual void testPolygon()
		{
			string s = "   Polygon    emPty ";
			com.esri.core.geometry.WktParser wktParser = new com.esri.core.geometry.WktParser
				();
			wktParser.resetParser(s);
			int currentToken;
			double value;
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.polygon);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			s = "  polyGON M (  empty, ( 5.  +1.e+0004  13, -.1e07  .006 13 ) , empty  , (4  003. 13 , 02E-3 .3e2 13 ) , empty, ( 5.  +1.e+0004  13 , -.1e07  .006  13) , (4  003. 13 , 02E-3 .3e2 13 )  )   ";
			wktParser.resetParser(s);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.polygon);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.attribute_m);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 5.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 1.0e4);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == -0.1e7);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 0.006);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 4.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 3.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 2.0e-3);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 0.3e2);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 5.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 1.0e4);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == -0.1e7);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 0.006);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 4.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 3.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 2.0e-3);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 0.3e2);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.not_available);
		}

		public virtual void testLineString()
		{
			string s = "   LineString    emPty ";
			com.esri.core.geometry.WktParser wktParser = new com.esri.core.geometry.WktParser
				();
			wktParser.resetParser(s);
			int currentToken;
			double value;
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.linestring);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			s = "  LineString ( 5.  +1.e+0004 , -.1e07  .006 )   ";
			wktParser.resetParser(s);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.linestring);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 5.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 1.0e4);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == -0.1e7);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 0.006);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.not_available);
		}

		public virtual void testPoint()
		{
			string s = "   PoInT    emPty ";
			com.esri.core.geometry.WktParser wktParser = new com.esri.core.geometry.WktParser
				();
			wktParser.resetParser(s);
			int currentToken;
			double value;
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.point);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.empty);
			s = "  POINT ZM ( 5.  +1.e+0004 13 17)   ";
			wktParser.resetParser(s);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.point);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.attribute_zm);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.left_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.x_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 5.0);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.y_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 1.0e4);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.z_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 13);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.m_literal);
			value = wktParser.currentNumericLiteral();
			NUnit.Framework.Assert.IsTrue(value == 17);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.right_paren);
			currentToken = wktParser.nextToken();
			NUnit.Framework.Assert.IsTrue(currentToken == com.esri.core.geometry.WktParser.WktToken
				.not_available);
			s = "   PoInt ";
			wktParser.resetParser(s);
			wktParser.nextToken();
		}
	}
}
