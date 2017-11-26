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
using NUnit.Framework;

namespace com.epl.geometry
{
	public class GeometryUtils
	{
		public static string GetGeometryType(com.epl.geometry.Geometry geomIn)
		{
			// there are five types: esriGeometryPoint
			// esriGeometryMultipoint
			// esriGeometryPolyline
			// esriGeometryPolygon
			// esriGeometryEnvelope
			if (geomIn is com.epl.geometry.Point)
			{
				return "esriGeometryPoint";
			}
			if (geomIn is com.epl.geometry.MultiPoint)
			{
				return "esriGeometryMultipoint";
			}
			if (geomIn is com.epl.geometry.Polyline)
			{
				return "esriGeometryPolyline";
			}
			if (geomIn is com.epl.geometry.Polygon)
			{
				return "esriGeometryPolygon";
			}
			if (geomIn is com.epl.geometry.Envelope)
			{
				return "esriGeometryEnvelope";
			}
			else
			{
				return null;
			}
		}

		internal static com.epl.geometry.Geometry GetGeometryFromJSon(string jsonStr)
		{
			try
			{
				com.epl.geometry.Geometry geom = com.epl.geometry.GeometryEngine.JsonToGeometry(jsonStr).GetGeometry();
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

		internal static string GetJSonStringFromGeometry(com.epl.geometry.Geometry geomIn, com.epl.geometry.SpatialReference sr)
		{
			string jsonStr4Geom = com.epl.geometry.GeometryEngine.GeometryToJson(sr, geomIn);
			string jsonStrNew = "{\"geometryType\":\"" + GetGeometryType(geomIn) + "\",\"geometries\":[" + jsonStr4Geom + "]}";
			return jsonStrNew;
		}

		/// <exception cref="System.IO.FileNotFoundException"/>
		public static com.epl.geometry.Geometry LoadFromTextFileDbg(string textFileName)
		{
			string fullPath = textFileName;
			// string fullCSVPathName = System.IO.Path.Combine( directoryPath ,
			// CsvFileName);
			java.io.File fileInfo = new java.io.File(fullPath);
			java.util.Scanner scanner = new java.util.Scanner(fileInfo);
			com.epl.geometry.Geometry geom = null;
			// grab first line
			string line = scanner.NextLine();
			string geomTypeString = Sharpen.Runtime.Substring(line, 1);
			if (Sharpen.Runtime.EqualsIgnoreCase(geomTypeString, "polygon"))
			{
				geom = new com.epl.geometry.Polygon();
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(geomTypeString, "polyline"))
				{
					geom = new com.epl.geometry.Polyline();
				}
				else
				{
					if (Sharpen.Runtime.EqualsIgnoreCase(geomTypeString, "multipoint"))
					{
						geom = new com.epl.geometry.MultiPoint();
					}
					else
					{
						if (Sharpen.Runtime.EqualsIgnoreCase(geomTypeString, "point"))
						{
							geom = new com.epl.geometry.Point();
						}
					}
				}
			}
			while (line.StartsWith("*"))
			{
				if (scanner.HasNextLine())
				{
					line = scanner.NextLine();
				}
			}
			int j = 0;
			com.epl.geometry.Geometry.Type geomType = geom.GetType();
			while (scanner.HasNextLine())
			{
				string[] parsedLine = line.Split("\\s+");
				double xVal = double.Parse(parsedLine[0]);
				double yVal = double.Parse(parsedLine[1]);
				if (j == 0 && (geomType == com.epl.geometry.Geometry.Type.Polygon || geomType == com.epl.geometry.Geometry.Type.Polyline))
				{
					((com.epl.geometry.MultiPath)geom).StartPath(xVal, yVal);
				}
				else
				{
					if (geomType == com.epl.geometry.Geometry.Type.Polygon || geomType == com.epl.geometry.Geometry.Type.Polyline)
					{
						((com.epl.geometry.MultiPath)geom).LineTo(xVal, yVal);
					}
					else
					{
						if (geomType == com.epl.geometry.Geometry.Type.MultiPoint)
						{
							((com.epl.geometry.MultiPoint)geom).Add(xVal, yVal);
						}
					}
				}
				// else if(geomType == Geometry.Type.Point)
				// Point geom = null;//new Point(xVal, yVal);
				j++;
				line = scanner.NextLine();
			}
			scanner.Close();
			return geom;
		}
	}
}
