

namespace com.esri.core.geometry
{
	public class GeometryUtils
	{
		public static string getGeometryType(com.esri.core.geometry.Geometry geomIn)
		{
			// there are five types: esriGeometryPoint
			// esriGeometryMultipoint
			// esriGeometryPolyline
			// esriGeometryPolygon
			// esriGeometryEnvelope
			if (geomIn is com.esri.core.geometry.Point)
			{
				return "esriGeometryPoint";
			}
			if (geomIn is com.esri.core.geometry.MultiPoint)
			{
				return "esriGeometryMultipoint";
			}
			if (geomIn is com.esri.core.geometry.Polyline)
			{
				return "esriGeometryPolyline";
			}
			if (geomIn is com.esri.core.geometry.Polygon)
			{
				return "esriGeometryPolygon";
			}
			if (geomIn is com.esri.core.geometry.Envelope)
			{
				return "esriGeometryEnvelope";
			}
			else
			{
				return null;
			}
		}

		internal static com.esri.core.geometry.Geometry getGeometryFromJSon(string jsonStr
			)
		{
			com.fasterxml.jackson.core.JsonFactory jf = new com.fasterxml.jackson.core.JsonFactory();
			try
			{
				com.fasterxml.jackson.core.JsonParser jp = jf.createJsonParser(jsonStr);
				jp.nextToken();
				com.esri.core.geometry.Geometry geom = com.esri.core.geometry.GeometryEngine.jsonToGeometry
					(jp).getGeometry();
				return geom;
			}
			catch (System.Exception)
			{
				return null;
			}
		}

		public enum SpatialRelationType
		{
			esriGeometryRelationCross,
			esriGeometryRelationDisjoint,
			esriGeometryRelationIn,
			esriGeometryRelationInteriorIntersection,
			esriGeometryRelationIntersection,
			esriGeometryRelationLineCoincidence,
			esriGeometryRelationLineTouch,
			esriGeometryRelationOverlap,
			esriGeometryRelationPointTouch,
			esriGeometryRelationTouch,
			esriGeometryRelationWithin,
			esriGeometryRelationRelation
		}

		internal static string getJSonStringFromGeometry(com.esri.core.geometry.Geometry 
			geomIn, com.esri.core.geometry.SpatialReference sr)
		{
			string jsonStr4Geom = com.esri.core.geometry.GeometryEngine.geometryToJson(sr, geomIn
				);
			string jsonStrNew = "{\"geometryType\":\"" + getGeometryType(geomIn) + "\",\"geometries\":["
				 + jsonStr4Geom + "]}";
			return jsonStrNew;
		}

		/// <exception cref="java.io.FileNotFoundException"/>
		public static com.esri.core.geometry.Geometry loadFromTextFileDbg(string textFileName
			)
		{
			string fullPath = textFileName;
			// string fullCSVPathName = System.IO.Path.Combine( directoryPath ,
			// CsvFileName);
			java.io.File fileInfo = new java.io.File(fullPath);
			java.util.Scanner scanner = new java.util.Scanner(fileInfo);
			com.esri.core.geometry.Geometry geom = null;
			// grab first line
			string line = scanner.nextLine();
			string geomTypeString = Sharpen.Runtime.substring(line, 1);
			if (Sharpen.Runtime.equalsIgnoreCase(geomTypeString, "polygon"))
			{
				geom = new com.esri.core.geometry.Polygon();
			}
			else
			{
				if (Sharpen.Runtime.equalsIgnoreCase(geomTypeString, "polyline"))
				{
					geom = new com.esri.core.geometry.Polyline();
				}
				else
				{
					if (Sharpen.Runtime.equalsIgnoreCase(geomTypeString, "multipoint"))
					{
						geom = new com.esri.core.geometry.MultiPoint();
					}
					else
					{
						if (Sharpen.Runtime.equalsIgnoreCase(geomTypeString, "point"))
						{
							geom = new com.esri.core.geometry.Point();
						}
					}
				}
			}
			while (line.StartsWith("*"))
			{
				if (scanner.hasNextLine())
				{
					line = scanner.nextLine();
				}
			}
			int j = 0;
			com.esri.core.geometry.Geometry.Type geomType = geom.getType();
			while (scanner.hasNextLine())
			{
				string[] parsedLine = line.split("\\s+");
				double xVal = double.parseDouble(parsedLine[0]);
				double yVal = double.parseDouble(parsedLine[1]);
				if (j == 0 && (geomType == com.esri.core.geometry.Geometry.Type.Polygon || geomType
					 == com.esri.core.geometry.Geometry.Type.Polyline))
				{
					((com.esri.core.geometry.MultiPath)geom).startPath(xVal, yVal);
				}
				else
				{
					if (geomType == com.esri.core.geometry.Geometry.Type.Polygon || geomType == com.esri.core.geometry.Geometry.Type
						.Polyline)
					{
						((com.esri.core.geometry.MultiPath)geom).lineTo(xVal, yVal);
					}
					else
					{
						if (geomType == com.esri.core.geometry.Geometry.Type.MultiPoint)
						{
							((com.esri.core.geometry.MultiPoint)geom).add(xVal, yVal);
						}
					}
				}
				// else if(geomType == Geometry.Type.Point)
				// Point geom = null;//new Point(xVal, yVal);
				j++;
				line = scanner.nextLine();
			}
			scanner.close();
			return geom;
		}
	}
}
