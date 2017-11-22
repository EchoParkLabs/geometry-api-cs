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
	internal class OperatorImportFromWktLocal : com.epl.geometry.OperatorImportFromWkt
	{
		public override com.epl.geometry.Geometry Execute(int import_flags, com.epl.geometry.Geometry.Type type, string wkt_string, com.epl.geometry.ProgressTracker progress_tracker)
		{
			com.epl.geometry.WktParser wkt_parser = new com.epl.geometry.WktParser(wkt_string);
			int current_token = wkt_parser.NextToken();
			return ImportFromWkt(import_flags, type, wkt_parser);
		}

		public override com.epl.geometry.OGCStructure ExecuteOGC(int import_flags, string wkt_string, com.epl.geometry.ProgressTracker progress_tracker)
		{
			System.Collections.Generic.List<com.epl.geometry.OGCStructure> stack = new System.Collections.Generic.List<com.epl.geometry.OGCStructure>(0);
			com.epl.geometry.WktParser wkt_parser = new com.epl.geometry.WktParser(wkt_string);
			com.epl.geometry.OGCStructure root = new com.epl.geometry.OGCStructure();
			root.m_structures = new System.Collections.Generic.List<com.epl.geometry.OGCStructure>(0);
			stack.Add(root);
			// add dummy root
			while (wkt_parser.NextToken() != com.epl.geometry.WktParser.WktToken.not_available)
			{
				int current_token = wkt_parser.CurrentToken();
				if (current_token == com.epl.geometry.WktParser.WktToken.right_paren)
				{
					stack.RemoveAt(stack.Count - 1);
					continue;
				}
				int ogc_type = current_token;
				com.epl.geometry.OGCStructure last = stack[stack.Count - 1];
				if (current_token == com.epl.geometry.WktParser.WktToken.geometrycollection)
				{
					current_token = wkt_parser.NextToken();
					if (current_token == com.epl.geometry.WktParser.WktToken.attribute_z || current_token == com.epl.geometry.WktParser.WktToken.attribute_m || current_token == com.epl.geometry.WktParser.WktToken.attribute_zm)
					{
						wkt_parser.NextToken();
					}
					com.epl.geometry.OGCStructure next = new com.epl.geometry.OGCStructure();
					next.m_type = ogc_type;
					next.m_structures = new System.Collections.Generic.List<com.epl.geometry.OGCStructure>(0);
					last.m_structures.Add(next);
					if (current_token != com.epl.geometry.WktParser.WktToken.empty)
					{
						stack.Add(next);
					}
					continue;
				}
				com.epl.geometry.Geometry geometry = ImportFromWkt(import_flags, com.epl.geometry.Geometry.Type.Unknown, wkt_parser);
				com.epl.geometry.OGCStructure leaf = new com.epl.geometry.OGCStructure();
				leaf.m_type = ogc_type;
				leaf.m_geometry = geometry;
				last.m_structures.Add(leaf);
			}
			return root;
		}

		internal static com.epl.geometry.Geometry ImportFromWkt(int import_flags, com.epl.geometry.Geometry.Type type, com.epl.geometry.WktParser wkt_parser)
		{
			int current_token = wkt_parser.CurrentToken();
			switch (current_token)
			{
				case com.epl.geometry.WktParser.WktToken.multipolygon:
				{
					if (type != com.epl.geometry.Geometry.Type.Polygon && type != com.epl.geometry.Geometry.Type.Unknown)
					{
						throw new System.ArgumentException("invalid shapetype");
					}
					return PolygonTaggedText(true, import_flags, wkt_parser);
				}

				case com.epl.geometry.WktParser.WktToken.multilinestring:
				{
					if (type != com.epl.geometry.Geometry.Type.Polyline && type != com.epl.geometry.Geometry.Type.Unknown)
					{
						throw new System.ArgumentException("invalid shapetype");
					}
					return LineStringTaggedText(true, import_flags, wkt_parser);
				}

				case com.epl.geometry.WktParser.WktToken.multipoint:
				{
					if (type != com.epl.geometry.Geometry.Type.MultiPoint && type != com.epl.geometry.Geometry.Type.Unknown)
					{
						throw new System.ArgumentException("invalid shapetype");
					}
					return MultiPointTaggedText(import_flags, wkt_parser);
				}

				case com.epl.geometry.WktParser.WktToken.polygon:
				{
					if (type != com.epl.geometry.Geometry.Type.Polygon && type != com.epl.geometry.Geometry.Type.Unknown)
					{
						throw new System.ArgumentException("invalid shapetype");
					}
					return PolygonTaggedText(false, import_flags, wkt_parser);
				}

				case com.epl.geometry.WktParser.WktToken.linestring:
				{
					if (type != com.epl.geometry.Geometry.Type.Polyline && type != com.epl.geometry.Geometry.Type.Unknown)
					{
						throw new System.ArgumentException("invalid shapetype");
					}
					return LineStringTaggedText(false, import_flags, wkt_parser);
				}

				case com.epl.geometry.WktParser.WktToken.point:
				{
					if (type != com.epl.geometry.Geometry.Type.Point && type != com.epl.geometry.Geometry.Type.Unknown)
					{
						throw new System.ArgumentException("invalid shapetype");
					}
					return PointTaggedText(import_flags, wkt_parser);
				}

				default:
				{
					break;
				}
			}
			// warning fix
			return null;
		}

		internal static com.epl.geometry.Geometry PolygonTaggedText(bool b_multi_polygon, int import_flags, com.epl.geometry.WktParser wkt_parser)
		{
			com.epl.geometry.MultiPath multi_path;
			com.epl.geometry.MultiPathImpl multi_path_impl;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			com.epl.geometry.AttributeStreamOfDbl position;
			com.epl.geometry.AttributeStreamOfInt32 paths;
			com.epl.geometry.AttributeStreamOfInt8 path_flags;
			position = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0);
			paths = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(1, 0);
			path_flags = (com.epl.geometry.AttributeStreamOfInt8)com.epl.geometry.AttributeStreamBase.CreateByteStream(1, unchecked((byte)0));
			multi_path = new com.epl.geometry.Polygon();
			multi_path_impl = (com.epl.geometry.MultiPathImpl)multi_path._getImpl();
			int current_token = wkt_parser.NextToken();
			if (current_token == com.epl.geometry.WktParser.WktToken.attribute_z)
			{
				zs = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0, com.epl.geometry.NumberUtils.TheNaN);
				multi_path_impl.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
				wkt_parser.NextToken();
			}
			else
			{
				if (current_token == com.epl.geometry.WktParser.WktToken.attribute_m)
				{
					ms = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0, com.epl.geometry.NumberUtils.TheNaN);
					multi_path_impl.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
					wkt_parser.NextToken();
				}
				else
				{
					if (current_token == com.epl.geometry.WktParser.WktToken.attribute_zm)
					{
						zs = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0, com.epl.geometry.NumberUtils.TheNaN);
						ms = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0, com.epl.geometry.NumberUtils.TheNaN);
						multi_path_impl.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
						multi_path_impl.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
						wkt_parser.NextToken();
					}
				}
			}
			int point_count;
			if (b_multi_polygon)
			{
				point_count = MultiPolygonText(zs, ms, position, paths, path_flags, wkt_parser);
			}
			else
			{
				point_count = PolygonText(zs, ms, position, paths, path_flags, 0, wkt_parser);
			}
			if (point_count != 0)
			{
				System.Diagnostics.Debug.Assert((2 * point_count == position.Size()));
				multi_path_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, position);
				multi_path_impl.SetPathStreamRef(paths);
				multi_path_impl.SetPathFlagsStreamRef(path_flags);
				if (zs != null)
				{
					multi_path_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z, zs);
				}
				if (ms != null)
				{
					multi_path_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M, ms);
				}
				multi_path_impl.NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
				com.epl.geometry.AttributeStreamOfInt8 path_flags_clone = new com.epl.geometry.AttributeStreamOfInt8(path_flags);
				for (int i = 0; i < path_flags_clone.Size() - 1; i++)
				{
					if (((int)path_flags_clone.Read(i) & (int)com.epl.geometry.PathFlags.enumOGCStartPolygon) != 0)
					{
						// Should
						// be
						// clockwise
						if (!com.epl.geometry.InternalUtils.IsClockwiseRing(multi_path_impl, i))
						{
							multi_path_impl.ReversePath(i);
						}
					}
					else
					{
						// make clockwise
						// Should be counter-clockwise
						if (com.epl.geometry.InternalUtils.IsClockwiseRing(multi_path_impl, i))
						{
							multi_path_impl.ReversePath(i);
						}
					}
				}
				// make
				// counter-clockwise
				multi_path_impl.SetPathFlagsStreamRef(path_flags_clone);
			}
			if ((import_flags & (int)com.epl.geometry.WktImportFlags.wktImportNonTrusted) == 0)
			{
				multi_path_impl.SetIsSimple(com.epl.geometry.MultiVertexGeometryImpl.GeometryXSimple.Weak, 0.0, false);
			}
			multi_path_impl.SetDirtyOGCFlags(false);
			return multi_path;
		}

		internal static com.epl.geometry.Geometry LineStringTaggedText(bool b_multi_linestring, int import_flags, com.epl.geometry.WktParser wkt_parser)
		{
			com.epl.geometry.MultiPath multi_path;
			com.epl.geometry.MultiPathImpl multi_path_impl;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			com.epl.geometry.AttributeStreamOfDbl position;
			com.epl.geometry.AttributeStreamOfInt32 paths;
			com.epl.geometry.AttributeStreamOfInt8 path_flags;
			position = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0);
			paths = (com.epl.geometry.AttributeStreamOfInt32)com.epl.geometry.AttributeStreamBase.CreateIndexStream(1, 0);
			path_flags = (com.epl.geometry.AttributeStreamOfInt8)com.epl.geometry.AttributeStreamBase.CreateByteStream(1, unchecked((byte)0));
			multi_path = new com.epl.geometry.Polyline();
			multi_path_impl = (com.epl.geometry.MultiPathImpl)multi_path._getImpl();
			int current_token = wkt_parser.NextToken();
			if (current_token == com.epl.geometry.WktParser.WktToken.attribute_z)
			{
				zs = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0, com.epl.geometry.NumberUtils.TheNaN);
				multi_path_impl.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
				wkt_parser.NextToken();
			}
			else
			{
				if (current_token == com.epl.geometry.WktParser.WktToken.attribute_m)
				{
					ms = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0, com.epl.geometry.NumberUtils.TheNaN);
					multi_path_impl.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
					wkt_parser.NextToken();
				}
				else
				{
					if (current_token == com.epl.geometry.WktParser.WktToken.attribute_zm)
					{
						zs = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0, com.epl.geometry.NumberUtils.TheNaN);
						ms = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0, com.epl.geometry.NumberUtils.TheNaN);
						multi_path_impl.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
						multi_path_impl.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
						wkt_parser.NextToken();
					}
				}
			}
			int point_count;
			if (b_multi_linestring)
			{
				point_count = MultiLineStringText(zs, ms, position, paths, path_flags, wkt_parser);
			}
			else
			{
				point_count = LineStringText(false, zs, ms, position, paths, path_flags, wkt_parser);
			}
			if (point_count != 0)
			{
				System.Diagnostics.Debug.Assert((2 * point_count == position.Size()));
				multi_path_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, position);
				multi_path_impl.SetPathStreamRef(paths);
				multi_path_impl.SetPathFlagsStreamRef(path_flags);
				if (zs != null)
				{
					multi_path_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z, zs);
				}
				if (ms != null)
				{
					multi_path_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M, ms);
				}
				multi_path_impl.NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
			}
			return multi_path;
		}

		internal static com.epl.geometry.Geometry MultiPointTaggedText(int import_flags, com.epl.geometry.WktParser wkt_parser)
		{
			com.epl.geometry.MultiPoint multi_point;
			com.epl.geometry.MultiPointImpl multi_point_impl;
			com.epl.geometry.AttributeStreamOfDbl zs = null;
			com.epl.geometry.AttributeStreamOfDbl ms = null;
			com.epl.geometry.AttributeStreamOfDbl position;
			position = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0);
			multi_point = new com.epl.geometry.MultiPoint();
			multi_point_impl = (com.epl.geometry.MultiPointImpl)multi_point._getImpl();
			int current_token = wkt_parser.NextToken();
			if (current_token == com.epl.geometry.WktParser.WktToken.attribute_z)
			{
				zs = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0, com.epl.geometry.NumberUtils.TheNaN);
				multi_point_impl.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
				wkt_parser.NextToken();
			}
			else
			{
				if (current_token == com.epl.geometry.WktParser.WktToken.attribute_m)
				{
					ms = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0, com.epl.geometry.NumberUtils.TheNaN);
					multi_point_impl.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
					wkt_parser.NextToken();
				}
				else
				{
					if (current_token == com.epl.geometry.WktParser.WktToken.attribute_zm)
					{
						zs = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0, com.epl.geometry.NumberUtils.TheNaN);
						ms = (com.epl.geometry.AttributeStreamOfDbl)com.epl.geometry.AttributeStreamBase.CreateDoubleStream(0, com.epl.geometry.NumberUtils.TheNaN);
						multi_point_impl.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
						multi_point_impl.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
						wkt_parser.NextToken();
					}
				}
			}
			int point_count = MultiPointText(zs, ms, position, wkt_parser);
			if (point_count != 0)
			{
				System.Diagnostics.Debug.Assert((2 * point_count == position.Size()));
				multi_point_impl.Resize(point_count);
				multi_point_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.POSITION, position);
				if (zs != null)
				{
					multi_point_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.Z, zs);
				}
				if (ms != null)
				{
					multi_point_impl.SetAttributeStreamRef(com.epl.geometry.VertexDescription.Semantics.M, ms);
				}
				multi_point_impl.NotifyModified(com.epl.geometry.MultiVertexGeometryImpl.DirtyFlags.DirtyAll);
			}
			return multi_point;
		}

		internal static com.epl.geometry.Geometry PointTaggedText(int import_flags, com.epl.geometry.WktParser wkt_parser)
		{
			com.epl.geometry.Point point = new com.epl.geometry.Point();
			int current_token = wkt_parser.NextToken();
			if (current_token == com.epl.geometry.WktParser.WktToken.attribute_z)
			{
				point.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
				wkt_parser.NextToken();
			}
			else
			{
				if (current_token == com.epl.geometry.WktParser.WktToken.attribute_m)
				{
					point.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
					wkt_parser.NextToken();
				}
				else
				{
					if (current_token == com.epl.geometry.WktParser.WktToken.attribute_zm)
					{
						point.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
						point.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
						wkt_parser.NextToken();
					}
				}
			}
			// At start of PointText
			current_token = wkt_parser.CurrentToken();
			if (current_token != com.epl.geometry.WktParser.WktToken.empty)
			{
				wkt_parser.NextToken();
				double x = wkt_parser.CurrentNumericLiteral();
				wkt_parser.NextToken();
				double y = wkt_parser.CurrentNumericLiteral();
				wkt_parser.NextToken();
				point.SetXY(x, y);
				if (wkt_parser.HasZs())
				{
					double z = wkt_parser.CurrentNumericLiteral();
					wkt_parser.NextToken();
					point.SetZ(z);
				}
				if (wkt_parser.HasMs())
				{
					double m = wkt_parser.CurrentNumericLiteral();
					wkt_parser.NextToken();
					point.SetM(m);
				}
			}
			return point;
		}

		internal static int MultiPolygonText(com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt32 paths, com.epl.geometry.AttributeStreamOfInt8 path_flags, com.epl.geometry.WktParser
			 wkt_parser)
		{
			// At start of MultiPolygonText
			int current_token = wkt_parser.CurrentToken();
			int total_point_count = 0;
			if (current_token == com.epl.geometry.WktParser.WktToken.empty)
			{
				return total_point_count;
			}
			current_token = wkt_parser.NextToken();
			while (current_token != com.epl.geometry.WktParser.WktToken.right_paren)
			{
				// At start of PolygonText
				total_point_count = PolygonText(zs, ms, position, paths, path_flags, total_point_count, wkt_parser);
				current_token = wkt_parser.NextToken();
			}
			return total_point_count;
		}

		internal static int MultiLineStringText(com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt32 paths, com.epl.geometry.AttributeStreamOfInt8 path_flags, com.epl.geometry.WktParser
			 wkt_parser)
		{
			// At start of MultiLineStringText
			int current_token = wkt_parser.CurrentToken();
			int total_point_count = 0;
			if (current_token == com.epl.geometry.WktParser.WktToken.empty)
			{
				return total_point_count;
			}
			current_token = wkt_parser.NextToken();
			while (current_token != com.epl.geometry.WktParser.WktToken.right_paren)
			{
				// At start of LineStringText
				int point_count = LineStringText(false, zs, ms, position, paths, path_flags, wkt_parser);
				total_point_count += point_count;
				current_token = wkt_parser.NextToken();
			}
			return total_point_count;
		}

		internal static int MultiPointText(com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.WktParser wkt_parser)
		{
			// At start of MultiPointText
			int current_token = wkt_parser.CurrentToken();
			int point_count = 0;
			if (current_token == com.epl.geometry.WktParser.WktToken.empty)
			{
				return point_count;
			}
			current_token = wkt_parser.NextToken();
			while (current_token != com.epl.geometry.WktParser.WktToken.right_paren)
			{
				// At start of PointText
				point_count += PointText(zs, ms, position, wkt_parser);
				if (current_token == com.epl.geometry.WktParser.WktToken.left_paren || current_token == com.epl.geometry.WktParser.WktToken.empty)
				{
					current_token = wkt_parser.NextToken();
				}
				else
				{
					// ogc standard
					current_token = wkt_parser.CurrentToken();
				}
			}
			// not ogc standard.
			// treat as
			// linestring
			return point_count;
		}

		internal static int PolygonText(com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt32 paths, com.epl.geometry.AttributeStreamOfInt8 path_flags, int total_point_count
			, com.epl.geometry.WktParser wkt_parser)
		{
			// At start of PolygonText
			int current_token = wkt_parser.CurrentToken();
			if (current_token == com.epl.geometry.WktParser.WktToken.empty)
			{
				return total_point_count;
			}
			bool b_first_line_string = true;
			current_token = wkt_parser.NextToken();
			while (current_token != com.epl.geometry.WktParser.WktToken.right_paren)
			{
				// At start of LineStringText
				int point_count = LineStringText(true, zs, ms, position, paths, path_flags, wkt_parser);
				if (point_count != 0)
				{
					if (b_first_line_string)
					{
						b_first_line_string = false;
						path_flags.SetBits(path_flags.Size() - 2, unchecked((byte)com.epl.geometry.PathFlags.enumOGCStartPolygon));
					}
					path_flags.SetBits(path_flags.Size() - 2, unchecked((byte)com.epl.geometry.PathFlags.enumClosed));
					total_point_count += point_count;
				}
				current_token = wkt_parser.NextToken();
			}
			return total_point_count;
		}

		internal static int LineStringText(bool b_ring, com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.AttributeStreamOfInt32 paths, com.epl.geometry.AttributeStreamOfInt8 path_flags
			, com.epl.geometry.WktParser wkt_parser)
		{
			// At start of LineStringText
			int current_token = wkt_parser.CurrentToken();
			int point_count = 0;
			if (current_token == com.epl.geometry.WktParser.WktToken.empty)
			{
				return point_count;
			}
			bool b_start_path = true;
			double startx = com.epl.geometry.NumberUtils.TheNaN;
			double starty = com.epl.geometry.NumberUtils.TheNaN;
			double startz = com.epl.geometry.NumberUtils.TheNaN;
			double startm = com.epl.geometry.NumberUtils.TheNaN;
			current_token = wkt_parser.NextToken();
			while (current_token != com.epl.geometry.WktParser.WktToken.right_paren)
			{
				// At start of x
				double x = wkt_parser.CurrentNumericLiteral();
				wkt_parser.NextToken();
				double y = wkt_parser.CurrentNumericLiteral();
				wkt_parser.NextToken();
				double z = com.epl.geometry.NumberUtils.TheNaN;
				double m = com.epl.geometry.NumberUtils.TheNaN;
				if (wkt_parser.HasZs())
				{
					z = wkt_parser.CurrentNumericLiteral();
					wkt_parser.NextToken();
				}
				if (wkt_parser.HasMs())
				{
					m = wkt_parser.CurrentNumericLiteral();
					wkt_parser.NextToken();
				}
				current_token = wkt_parser.CurrentToken();
				bool b_add_point = true;
				if (b_ring && point_count >= 2 && current_token == com.epl.geometry.WktParser.WktToken.right_paren)
				{
					// If the last point in the ring is not equal to the start
					// point, then let's add it.
					if ((startx == x || (com.epl.geometry.NumberUtils.IsNaN(startx) && com.epl.geometry.NumberUtils.IsNaN(x))) && (starty == y || (com.epl.geometry.NumberUtils.IsNaN(starty) && com.epl.geometry.NumberUtils.IsNaN(y))) && (!wkt_parser.HasZs() || startz == z || (com.epl.geometry.NumberUtils
						.IsNaN(startz) && com.epl.geometry.NumberUtils.IsNaN(z))) && (!wkt_parser.HasMs() || startm == m || (com.epl.geometry.NumberUtils.IsNaN(startm) && com.epl.geometry.NumberUtils.IsNaN(m))))
					{
						b_add_point = false;
					}
				}
				if (b_add_point)
				{
					if (b_start_path)
					{
						b_start_path = false;
						startx = x;
						starty = y;
						startz = z;
						startm = m;
					}
					point_count++;
					AddToStreams(zs, ms, position, x, y, z, m);
				}
			}
			if (point_count == 1)
			{
				point_count++;
				AddToStreams(zs, ms, position, startx, starty, startz, startm);
			}
			paths.Add(position.Size() / 2);
			path_flags.Add(unchecked((byte)0));
			return point_count;
		}

		internal static int PointText(com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, com.epl.geometry.WktParser wkt_parser)
		{
			// At start of PointText
			int current_token = wkt_parser.CurrentToken();
			if (current_token == com.epl.geometry.WktParser.WktToken.empty)
			{
				return 0;
			}
			if (current_token == com.epl.geometry.WktParser.WktToken.left_paren)
			{
				wkt_parser.NextToken();
			}
			// ogc standard
			// At start of x
			double x = wkt_parser.CurrentNumericLiteral();
			wkt_parser.NextToken();
			double y = wkt_parser.CurrentNumericLiteral();
			wkt_parser.NextToken();
			double z = com.epl.geometry.NumberUtils.TheNaN;
			double m = com.epl.geometry.NumberUtils.TheNaN;
			if (zs != null)
			{
				z = wkt_parser.CurrentNumericLiteral();
				wkt_parser.NextToken();
			}
			if (ms != null)
			{
				m = wkt_parser.CurrentNumericLiteral();
				wkt_parser.NextToken();
			}
			AddToStreams(zs, ms, position, x, y, z, m);
			return 1;
		}

		internal static void AddToStreams(com.epl.geometry.AttributeStreamOfDbl zs, com.epl.geometry.AttributeStreamOfDbl ms, com.epl.geometry.AttributeStreamOfDbl position, double x, double y, double z, double m)
		{
			position.Add(x);
			position.Add(y);
			if (zs != null)
			{
				zs.Add(z);
			}
			if (ms != null)
			{
				ms.Add(m);
			}
		}
	}
}
