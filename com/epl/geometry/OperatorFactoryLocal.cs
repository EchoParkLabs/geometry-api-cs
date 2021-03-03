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
	/// <summary>An abstract class that represent the basic OperatorFactory interface.</summary>
	public class OperatorFactoryLocal : com.epl.geometry.OperatorFactory
	{
		private static readonly com.epl.geometry.OperatorFactoryLocal INSTANCE = new com.epl.geometry.OperatorFactoryLocal();

		private static readonly System.Collections.Generic.Dictionary<com.epl.geometry.Operator.Type, com.epl.geometry.Operator> st_supportedOperators = new System.Collections.Generic.Dictionary<com.epl.geometry.Operator.Type, com.epl.geometry.Operator>();

		static OperatorFactoryLocal()
		{
			// Register all implemented operator allocators in the dictionary
			st_supportedOperators[com.epl.geometry.Operator.Type.Project] = new com.epl.geometry.OperatorProjectLocal();
//			st_supportedOperators[com.epl.geometry.Operator.Type.ExportToJson] = new com.epl.geometry.OperatorExportToJsonLocal();
//			st_supportedOperators[com.epl.geometry.Operator.Type.ImportFromJson] = new com.epl.geometry.OperatorImportFromJsonLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.ExportToESRIShape] = new com.epl.geometry.OperatorExportToESRIShapeLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.ImportFromESRIShape] = new com.epl.geometry.OperatorImportFromESRIShapeLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Proximity2D] = new com.epl.geometry.OperatorProximity2DLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.DensifyByLength] = new com.epl.geometry.OperatorDensifyByLengthLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Relate] = new com.epl.geometry.OperatorRelateLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Equals] = new com.epl.geometry.OperatorEqualsLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Disjoint] = new com.epl.geometry.OperatorDisjointLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Intersects] = new com.epl.geometry.OperatorIntersectsLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Within] = new com.epl.geometry.OperatorWithinLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Contains] = new com.epl.geometry.OperatorContainsLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Crosses] = new com.epl.geometry.OperatorCrossesLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Touches] = new com.epl.geometry.OperatorTouchesLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Overlaps] = new com.epl.geometry.OperatorOverlapsLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.SimplifyOGC] = new com.epl.geometry.OperatorSimplifyLocalOGC();
			st_supportedOperators[com.epl.geometry.Operator.Type.Simplify] = new com.epl.geometry.OperatorSimplifyLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Offset] = new com.epl.geometry.OperatorOffsetLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.GeodeticDensifyByLength] = new com.epl.geometry.OperatorGeodeticDensifyLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.ShapePreservingDensify] = new com.epl.geometry.OperatorShapePreservingDensifyLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.GeodesicBuffer] = new com.epl.geometry.OperatorGeodesicBufferLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.GeodeticLength] = new com.epl.geometry.OperatorGeodeticLengthLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.GeodeticArea] = new com.epl.geometry.OperatorGeodeticAreaLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Buffer] = new com.epl.geometry.OperatorBufferLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Distance] = new com.epl.geometry.OperatorDistanceLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Intersection] = new com.epl.geometry.OperatorIntersectionLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Difference] = new com.epl.geometry.OperatorDifferenceLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.SymmetricDifference] = new com.epl.geometry.OperatorSymmetricDifferenceLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Clip] = new com.epl.geometry.OperatorClipLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Cut] = new com.epl.geometry.OperatorCutLocal();
//			st_supportedOperators[com.epl.geometry.Operator.Type.ExportToWkb] = new com.epl.geometry.OperatorExportToWkbLocal();
//			st_supportedOperators[com.epl.geometry.Operator.Type.ImportFromWkb] = new com.epl.geometry.OperatorImportFromWkbLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.ExportToWkt] = new com.epl.geometry.OperatorExportToWktLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.ImportFromWkt] = new com.epl.geometry.OperatorImportFromWktLocal();
//			st_supportedOperators[com.epl.geometry.Operator.Type.ImportFromGeoJson] = new com.epl.geometry.OperatorImportFromGeoJsonLocal();
//			st_supportedOperators[com.epl.geometry.Operator.Type.ExportToGeoJson] = new com.epl.geometry.OperatorExportToGeoJsonLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Union] = new com.epl.geometry.OperatorUnionLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Generalize] = new com.epl.geometry.OperatorGeneralizeLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.ConvexHull] = new com.epl.geometry.OperatorConvexHullLocal();
			st_supportedOperators[com.epl.geometry.Operator.Type.Boundary] = new com.epl.geometry.OperatorBoundaryLocal();
		}

		private OperatorFactoryLocal()
		{
		}

		// LabelPoint, - not ported
		// comment line for post_generate.py // temp trigger
		/// <summary>Returns a reference to the singleton.</summary>
		public static com.epl.geometry.OperatorFactoryLocal GetInstance()
		{
			return INSTANCE;
		}

		public override com.epl.geometry.Operator GetOperator(com.epl.geometry.Operator.Type type)
		{
			if (st_supportedOperators.ContainsKey(type))
			{
				return st_supportedOperators[type];
			}
			else
			{
				throw new System.ArgumentException();
			}
		}

		public override bool IsOperatorSupported(com.epl.geometry.Operator.Type type)
		{
			return st_supportedOperators.ContainsKey(type);
		}

//		public static void SaveJSONToTextFileDbg(string file_name, com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatial_ref)
//		{
//			if (file_name == null)
//			{
//				throw new System.ArgumentException();
//			}
//			com.epl.geometry.OperatorFactoryLocal engine = com.epl.geometry.OperatorFactoryLocal.GetInstance();
//			com.epl.geometry.OperatorExportToJson exporterJSON = (com.epl.geometry.OperatorExportToJson)engine.GetOperator(com.epl.geometry.Operator.Type.ExportToJson);
//			string jsonString = exporterJSON.Execute(spatial_ref, geometry);
//			try
//			{
//				java.io.FileOutputStream outfile = new java.io.FileOutputStream(file_name);
//				System.IO.TextWriter p = new System.IO.TextWriter(outfile);
//				p.Write(jsonString);
//				p.Close();
//			}
//			catch (System.Exception)
//			{
//			}
//		}
//
//		public static com.epl.geometry.MapGeometry LoadGeometryFromJSONFileDbg(string file_name)
//		{
//			if (file_name == null)
//			{
//				throw new System.ArgumentException();
//			}
//			string jsonString = null;
//			try
//			{
//				java.io.FileInputStream stream = new java.io.FileInputStream(file_name);
//				System.IO.StreamReader reader = new java.io.BufferedReader(new System.IO.StreamReader(stream));
//				System.Text.StringBuilder builder = new System.Text.StringBuilder();
//				char[] buffer = new char[8192];
//				int read;
//				while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
//				{
//					builder.Append(buffer, 0, read);
//				}
//				stream.Close();
//				jsonString = builder.ToString();
//			}
//			catch (System.Exception)
//			{
//			}
//			com.epl.geometry.MapGeometry mapGeom = com.epl.geometry.OperatorImportFromJson.Local().Execute(com.epl.geometry.Geometry.Type.Unknown, jsonString);
//			return mapGeom;
//		}
//
//		public static com.epl.geometry.MapGeometry LoadGeometryFromJSONStringDbg(string json)
//		{
//			if (json == null)
//			{
//				throw new System.ArgumentException();
//			}
//			com.epl.geometry.MapGeometry mapGeom = null;
//			try
//			{
//				mapGeom = com.epl.geometry.OperatorImportFromJson.Local().Execute(com.epl.geometry.Geometry.Type.Unknown, json);
//			}
//			catch (System.Exception e)
//			{
//				throw new System.ArgumentException(e.ToString());
//			}
//			return mapGeom;
//		}
//
//		public static com.epl.geometry.Geometry LoadGeometryFromEsriShapeDbg(string file_name)
//		{
//			if (file_name == null)
//			{
//				throw new System.ArgumentException();
//			}
//			try
//			{
//				java.io.FileInputStream stream = new java.io.FileInputStream(file_name);
//				java.nio.channels.FileChannel fchan = stream.GetChannel();
//				System.IO.MemoryStream bb = System.IO.MemoryStream.Allocate((int)fchan.Size());
//				fchan.Read(bb);
//				bb.Order(java.nio.ByteOrder.LITTLE_ENDIAN);
//				com.epl.geometry.Geometry g = com.epl.geometry.OperatorImportFromESRIShape.Local().Execute(0, com.epl.geometry.Geometry.Type.Unknown, bb);
//				fchan.Close();
//				stream.Close();
//				return g;
//			}
//			catch (System.Exception)
//			{
//				throw new System.ArgumentException();
//			}
//		}
//
//		public static void SaveGeometryToEsriShapeDbg(string file_name, com.epl.geometry.Geometry geometry)
//		{
//			if (file_name == null)
//			{
//				throw new System.ArgumentException();
//			}
//			try
//			{
//				System.IO.MemoryStream bb = com.epl.geometry.OperatorExportToESRIShape.Local().Execute(0, geometry);
//				java.io.FileOutputStream outfile = new java.io.FileOutputStream(file_name);
//				java.nio.channels.FileChannel fchan = outfile.GetChannel();
//				fchan.Write(bb);
//				fchan.Close();
//				outfile.Close();
//			}
//			catch (System.Exception)
//			{
//				throw new System.ArgumentException();
//			}
//		}
//
//		public static void SaveToWKTFileDbg(string file_name, com.epl.geometry.Geometry geometry, com.epl.geometry.SpatialReference spatial_ref)
//		{
//			if (file_name == null)
//			{
//				throw new System.ArgumentException();
//			}
//			string jsonString = com.epl.geometry.OperatorExportToWkt.Local().Execute(0, geometry, null);
//			try
//			{
//				java.io.FileOutputStream outfile = new java.io.FileOutputStream(file_name);
//				System.IO.TextWriter p = new System.IO.TextWriter(outfile);
//				p.Write(jsonString);
//				p.Close();
//			}
//			catch (System.Exception)
//			{
//			}
//		}
//
//		public static com.epl.geometry.Geometry LoadGeometryFromWKTFileDbg(string file_name)
//		{
//			if (file_name == null)
//			{
//				throw new System.ArgumentException();
//			}
//			string s = null;
//			try
//			{
//				java.io.FileInputStream stream = new java.io.FileInputStream(file_name);
//				System.IO.StreamReader reader = new java.io.BufferedReader(new System.IO.StreamReader(stream));
//				System.Text.StringBuilder builder = new System.Text.StringBuilder();
//				char[] buffer = new char[8192];
//				int read;
//				while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
//				{
//					builder.Append(buffer, 0, read);
//				}
//				stream.Close();
//				s = builder.ToString();
//			}
//			catch (System.Exception)
//			{
//			}
//			return com.epl.geometry.OperatorImportFromWkt.Local().Execute(0, com.epl.geometry.Geometry.Type.Unknown, s, null);
//		}
	}
}
