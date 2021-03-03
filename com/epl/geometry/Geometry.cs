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
	/// <summary>Common properties and methods shared by all geometric objects.</summary>
	/// <remarks>
	/// Common properties and methods shared by all geometric objects. Geometries are
	/// objects that define a spatial location and and associated geometric shape.
	/// </remarks>
	[System.Serializable]
	public abstract class Geometry
	{
		internal com.epl.geometry.VertexDescription m_description;

		internal volatile int m_touchFlag;

		internal Geometry()
		{
			m_description = null;
			m_touchFlag = 0;
		}

		/// <summary>Geometry types</summary>
		public abstract class GeometryType
		{
			public const int Unknown = 0;

			public const int Point = 1 + unchecked((int)(0x20));

			public const int Line = 2 + unchecked((int)(0x40)) + unchecked((int)(0x100));

			public const int Bezier = 3 + unchecked((int)(0x40)) + unchecked((int)(0x100));

			public const int EllipticArc = 4 + unchecked((int)(0x40)) + unchecked((int)(0x100));

			public const int Envelope = 5 + unchecked((int)(0x40)) + unchecked((int)(0x80));

			public const int MultiPoint = 6 + unchecked((int)(0x20)) + unchecked((int)(0x200));

			public const int Polyline = 7 + unchecked((int)(0x40)) + unchecked((int)(0x200)) + unchecked((int)(0x400));

			public const int Polygon = 8 + unchecked((int)(0x40)) + unchecked((int)(0x80)) + unchecked((int)(0x200)) + unchecked((int)(0x400));
			// points
			// lines, segment
			// lines, segment
			// lines, segment
			// lines, areas
			// points,
			// multivertex
			// lines,
			// multivertex,
			// multipath
		}

		public static class GeometryTypeConstants
		{
		}

		/// <summary>The type of this geometry.</summary>
		[System.Serializable]
		public sealed class Type
		{
			/// <summary>
			/// Used to indicate that the geometry type is not known before executing
			/// a method.
			/// </summary>
			public static readonly com.epl.geometry.Geometry.Type Unknown = new com.epl.geometry.Geometry.Type(com.epl.geometry.Geometry.GeometryType.Unknown);

			/// <summary>The value representing a point as geometry type.</summary>
			public static readonly com.epl.geometry.Geometry.Type Point = new com.epl.geometry.Geometry.Type(com.epl.geometry.Geometry.GeometryType.Point);

			/// <summary>The value representing a line as geometry type.</summary>
			public static readonly com.epl.geometry.Geometry.Type Line = new com.epl.geometry.Geometry.Type(com.epl.geometry.Geometry.GeometryType.Line);

			/// <summary>The value representing an envelope as geometry type.</summary>
			public static readonly com.epl.geometry.Geometry.Type Envelope = new com.epl.geometry.Geometry.Type(com.epl.geometry.Geometry.GeometryType.Envelope);

			/// <summary>The value representing a multipoint as geometry type.</summary>
			public static readonly com.epl.geometry.Geometry.Type MultiPoint = new com.epl.geometry.Geometry.Type(com.epl.geometry.Geometry.GeometryType.MultiPoint);

			/// <summary>The value representing a polyline as geometry type.</summary>
			public static readonly com.epl.geometry.Geometry.Type Polyline = new com.epl.geometry.Geometry.Type(com.epl.geometry.Geometry.GeometryType.Polyline);

			/// <summary>The value representing a polygon as geometry type.</summary>
			public static readonly com.epl.geometry.Geometry.Type Polygon = new com.epl.geometry.Geometry.Type(com.epl.geometry.Geometry.GeometryType.Polygon);

			private int enumValue;

			/// <summary>Returns the integer representation of the enumeration value.</summary>
			public int Value()
			{
				return enumValue;
			}

			internal Type(int val)
			{
				enumValue = val;
			}

//			public static com.epl.geometry.Geometry.Type IntToType(int geometryType)
//			{
//				com.epl.geometry.Geometry.Type[] v = com.epl.geometry.Geometry.Type.Values();
//				for (int i = 0; i < v.Length; i++)
//				{
//					if (v[i].Value() == geometryType)
//					{
//						return v[i];
//					}
//				}
//				throw new System.ArgumentException();
//			}
		}

		/// <summary>Returns the geometry type.</summary>
		/// <returns>Returns the geometry type.</returns>
		public abstract com.epl.geometry.Geometry.Type GetType();

		/// <summary>
		/// Returns the topological dimension of the geometry object based on the
		/// geometry's type.
		/// </summary>
		/// <remarks>
		/// Returns the topological dimension of the geometry object based on the
		/// geometry's type.
		/// <p>
		/// Returns 0 for point and multipoint.
		/// <p>
		/// Returns 1 for lines and polylines.
		/// <p>
		/// Returns 2 for polygons and envelopes
		/// <p>
		/// Returns 3 for objects with volume
		/// </remarks>
		/// <returns>Returns the integer value of the dimension of geometry.</returns>
		public abstract int GetDimension();

		/// <summary>Returns the VertexDescription of this geomtry.</summary>
		public virtual com.epl.geometry.VertexDescription GetDescription()
		{
			return m_description;
		}

		/// <summary>Assigns the new VertexDescription by adding or dropping attributes.</summary>
		/// <remarks>
		/// Assigns the new VertexDescription by adding or dropping attributes. The
		/// Geometry will have the src description as a result.
		/// </remarks>
		public virtual void AssignVertexDescription(com.epl.geometry.VertexDescription src)
		{
			_touch();
			if (src == m_description)
			{
				return;
			}
			_assignVertexDescriptionImpl(src);
		}

		protected internal abstract void _assignVertexDescriptionImpl(com.epl.geometry.VertexDescription src);

		/// <summary>
		/// Merges the new VertexDescription by adding missing attributes from the
		/// src.
		/// </summary>
		/// <remarks>
		/// Merges the new VertexDescription by adding missing attributes from the
		/// src. The Geometry will have a union of the current and the src
		/// descriptions.
		/// </remarks>
		public virtual void MergeVertexDescription(com.epl.geometry.VertexDescription src)
		{
			_touch();
			if (src == m_description)
			{
				return;
			}
			// check if we need to do anything (if the src has same attributes)
			com.epl.geometry.VertexDescription newdescription = com.epl.geometry.VertexDescriptionDesignerImpl.GetMergedVertexDescription(m_description, src);
			if (newdescription == m_description)
			{
				return;
			}
			_assignVertexDescriptionImpl(newdescription);
		}

		/// <summary>A shortcut for getDescription().hasAttribute()</summary>
		public virtual bool HasAttribute(int semantics)
		{
			return GetDescription().HasAttribute(semantics);
		}

		/// <summary>Adds a new attribute to the Geometry.</summary>
		/// <param name="semantics"/>
		public virtual void AddAttribute(int semantics)
		{
			_touch();
			if (m_description.HasAttribute(semantics))
			{
				return;
			}
			com.epl.geometry.VertexDescription newvd = com.epl.geometry.VertexDescriptionDesignerImpl.GetMergedVertexDescription(m_description, semantics);
			_assignVertexDescriptionImpl(newvd);
		}

		/// <summary>Drops an attribute from the Geometry.</summary>
		/// <remarks>
		/// Drops an attribute from the Geometry. Dropping the attribute is
		/// equivalent to setting the attribute to the default value for each vertex,
		/// However, it is faster and the result Geometry has smaller memory
		/// footprint and smaller size when persisted.
		/// </remarks>
		public virtual void DropAttribute(int semantics)
		{
			_touch();
			if (!m_description.HasAttribute(semantics))
			{
				return;
			}
			com.epl.geometry.VertexDescription newvd = com.epl.geometry.VertexDescriptionDesignerImpl.RemoveSemanticsFromVertexDescription(m_description, semantics);
			_assignVertexDescriptionImpl(newvd);
		}

		/// <summary>Drops all attributes from the Geometry with exception of POSITON.</summary>
		public virtual void DropAllAttributes()
		{
			AssignVertexDescription(com.epl.geometry.VertexDescriptionDesignerImpl.GetDefaultDescriptor2D());
		}

		/// <summary>Returns the min and max attribute values at the ordinate of the Geometry</summary>
		public abstract com.epl.geometry.Envelope1D QueryInterval(int semantics, int ordinate);

		/// <summary>Returns the axis aligned bounding box of the geometry.</summary>
		/// <param name="env">The envelope to return the result in.</param>
		public abstract void QueryEnvelope(com.epl.geometry.Envelope env);

		// {
		// Envelope2D e2d = new Envelope2D();
		// queryEnvelope2D(e2d);
		// env.setEnvelope2D(e2d);
		// }
		/// <summary>Returns tight bbox of the Geometry in X, Y plane.</summary>
		public abstract void QueryEnvelope2D(com.epl.geometry.Envelope2D env);

		/// <summary>Returns tight bbox of the Geometry in 3D.</summary>
		internal abstract void QueryEnvelope3D(com.epl.geometry.Envelope3D env);

		/// <summary>Returns the conservative bbox of the Geometry in X, Y plane.</summary>
		/// <remarks>
		/// Returns the conservative bbox of the Geometry in X, Y plane. This is a
		/// faster method than QueryEnvelope2D. However, the bbox could be larger
		/// than the tight box.
		/// </remarks>
		public virtual void QueryLooseEnvelope2D(com.epl.geometry.Envelope2D env)
		{
			QueryEnvelope2D(env);
		}

		/// <summary>Returns tight conservative box of the Geometry in 3D.</summary>
		/// <remarks>
		/// Returns tight conservative box of the Geometry in 3D. This is a faster
		/// method than the QueryEnvelope3D. However, the box could be larger than
		/// the tight box.
		/// </remarks>
		internal virtual void QueryLooseEnvelope3D(com.epl.geometry.Envelope3D env)
		{
			QueryEnvelope3D(env);
		}

		/// <summary>
		/// IsEmpty returns TRUE when the Geometry object does not contain geometric
		/// information beyond its original initialization state.
		/// </summary>
		/// <returns>boolean Returns TRUE if this geometry is empty.</returns>
		public abstract bool IsEmpty();

		/// <summary>
		/// Returns the geometry to its original initialization state by releasing
		/// all data referenced by the geometry.
		/// </summary>
		public abstract void SetEmpty();

		/// <summary>Applies 2D affine transformation in XY plane.</summary>
		/// <param name="transform">The affine transformation to be applied to this geometry.</param>
		public abstract void ApplyTransformation(com.epl.geometry.Transformation2D transform);

		/// <summary>Applies 3D affine transformation.</summary>
		/// <remarks>Applies 3D affine transformation. Adds Z attribute if it is missing.</remarks>
		/// <param name="transform">The affine transformation to be applied to this geometry.</param>
		internal abstract void ApplyTransformation(com.epl.geometry.Transformation3D transform);

		/// <summary>Creates an instance of an empty geometry of the same type.</summary>
		public abstract com.epl.geometry.Geometry CreateInstance();

		/// <summary>Copies this geometry to another geometry of the same type.</summary>
		/// <remarks>
		/// Copies this geometry to another geometry of the same type. The result
		/// geometry is an exact copy.
		/// </remarks>
		/// <exception>
		/// GeometryException
		/// invalid_argument if the geometry is of different type.
		/// </exception>
		public abstract void CopyTo(com.epl.geometry.Geometry dst);

		/// <summary>Calculates the area of the geometry.</summary>
		/// <remarks>
		/// Calculates the area of the geometry. If the spatial reference is a
		/// Geographic Coordinate System (WGS84) then the 2D area calculation is
		/// defined in angular units.
		/// </remarks>
		/// <returns>A double value representing the 2D area of the geometry.</returns>
		public virtual double CalculateArea2D()
		{
			return 0;
		}

		/// <summary>Calculates the length of the geometry.</summary>
		/// <remarks>
		/// Calculates the length of the geometry. If the spatial reference is a
		/// Geographic Coordinate System (a system where coordinates are defined
		/// using angular units such as longitude and latitude) then the 2D distance
		/// calculation is returned in angular units. In cases where length must be
		/// calculated on a Geographic Coordinate System consider the using the
		/// geodeticLength method on the
		/// <see cref="GeometryEngine"/>
		/// </remarks>
		/// <returns>A double value representing the 2D length of the geometry.</returns>
		public virtual double CalculateLength2D()
		{
			return 0;
		}

		protected internal virtual object _getImpl()
		{
			throw new System.Exception("invalid call");
		}

		/// <summary>Adds the Z attribute to this Geometry</summary>
		internal virtual void AddZ()
		{
			AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
		}

		/// <summary>Returns true if this Geometry has the Z attribute</summary>
		/// <returns>true if this Geometry has the Z attribute</returns>
		public virtual bool HasZ()
		{
			return HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
		}

		/// <summary>Adds the M attribute to this Geometry</summary>
		public virtual void AddM()
		{
			AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
		}

		/// <summary>Returns true if this Geometry has an M attribute</summary>
		/// <returns>true if this Geometry has an M attribute</returns>
		public virtual bool HasM()
		{
			return HasAttribute(com.epl.geometry.VertexDescription.Semantics.M);
		}

		/// <summary>Adds the ID attribute to this Geometry</summary>
		public virtual void AddID()
		{
			AddAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
		}

		/// <summary>Returns true if this Geometry has an ID attribute</summary>
		/// <returns>true if this Geometry has an ID attribute</returns>
		public virtual bool HasID()
		{
			return HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
		}

		/// <summary>Returns this geometry's dimension.</summary>
		/// <remarks>
		/// Returns this geometry's dimension.
		/// <p>
		/// Returns 0 for point and multipoint.
		/// <p>
		/// Returns 1 for lines and polylines.
		/// <p>
		/// Returns 2 for polygons and envelopes
		/// <p>
		/// Returns 3 for objects with volume
		/// </remarks>
		/// <param name="type">
		/// The integer value from geometry enumeration. You can use the
		/// method
		/// <see cref="Type.Value()"/>
		/// to get at the integer value.
		/// </param>
		/// <returns>The integer dimension of this geometry.</returns>
		public static int GetDimensionFromType(int type)
		{
			return (((type & (unchecked((int)(0x40)) | unchecked((int)(0x80)))) >> 6) + 1) >> 1;
		}

		/// <summary>
		/// Indicates if the integer value of the enumeration is a point type
		/// (dimension 0).
		/// </summary>
		/// <param name="type">
		/// The integer value from geometry enumeration. You can use the
		/// method
		/// <see cref="Type.Value()"/>
		/// to get at the integer value.
		/// </param>
		/// <returns>TRUE if the geometry is a point.</returns>
		public static bool IsPoint(int type)
		{
			return (type & unchecked((int)(0x20))) != 0;
		}

		/// <summary>
		/// Indicates if the integer value of the enumeration is linear (dimension
		/// 1).
		/// </summary>
		/// <param name="type">
		/// The integer value from geometry enumeration. You can use the
		/// method
		/// <see cref="Type.Value()"/>
		/// to get at the integer value.
		/// </param>
		/// <returns>TRUE if the geometry is a line.</returns>
		public static bool IsLinear(int type)
		{
			return (type & unchecked((int)(0x40))) != 0;
		}

		/// <summary>
		/// Indicates if the integer value of the enumeration is an area (dimension
		/// 2).
		/// </summary>
		/// <param name="type">
		/// The integer value from geometry enumeration. You can use the
		/// method
		/// <see cref="Type.Value()"/>
		/// to get at the integer value.
		/// </param>
		/// <returns>TRUE if the geometry is a polygon.</returns>
		public static bool IsArea(int type)
		{
			return (type & unchecked((int)(0x80))) != 0;
		}

		/// <summary>Indicates if the integer value of the enumeration is a segment.</summary>
		/// <param name="type">
		/// The integer value from geometry enumeration. You can use the
		/// method
		/// <see cref="Type.Value()"/>
		/// to get at the integer value.
		/// </param>
		/// <returns>TRUE if the geometry is a segment.</returns>
		public static bool IsSegment(int type)
		{
			return (type & unchecked((int)(0x100))) != 0;
		}

		/// <summary>
		/// Indicates if the integer value of the enumeration is a multivertex (ie,
		/// multipoint, line, or area).
		/// </summary>
		/// <param name="type">
		/// The integer value from geometry enumeration. You can use the
		/// method
		/// <see cref="Type.Value()"/>
		/// to get at the integer value.
		/// </param>
		/// <returns>TRUE if the geometry has multiple vertices.</returns>
		public static bool IsMultiVertex(int type)
		{
			return (type & unchecked((int)(0x200))) != 0;
		}

		/// <summary>
		/// Indicates if the integer value of the enumeration is a multipath (ie,
		/// line or area).
		/// </summary>
		/// <param name="type">
		/// The integer value from geometry enumeration. You can use the
		/// method
		/// <see cref="Type.Value()"/>
		/// to get at the integer value.
		/// </param>
		/// <returns>TRUE if the geometry is a multipath.</returns>
		public static bool IsMultiPath(int type)
		{
			return (type & unchecked((int)(0x400))) != 0;
		}

		/// <summary>Creates a copy of the geometry.</summary>
		/// <returns>Returns a copy of this geometry.</returns>
		public virtual com.epl.geometry.Geometry Copy()
		{
			com.epl.geometry.Geometry geom = CreateInstance();
			this.CopyTo(geom);
			return geom;
		}

		/// <summary>Returns boundary of this geometry.</summary>
		/// <remarks>
		/// Returns boundary of this geometry.
		/// Polygon and Envelope boundary is a Polyline. For Polyline and Line, the
		/// boundary is a Multi_point consisting of path endpoints. For Multi_point
		/// and Point NULL is returned.
		/// </remarks>
		public abstract com.epl.geometry.Geometry GetBoundary();

		/// <summary>Replaces NaNs in the attribute with the given value.</summary>
		/// <remarks>
		/// Replaces NaNs in the attribute with the given value.
		/// If the geometry is not empty, it adds the attribute if geometry does not have it yet, and replaces the values.
		/// If the geometry is empty, it adds the attribute and does not set any values.
		/// </remarks>
		public abstract void ReplaceNaNs(int semantics, double value);

		internal static com.epl.geometry.Geometry _clone(com.epl.geometry.Geometry src)
		{
			com.epl.geometry.Geometry geom = src.CreateInstance();
			src.CopyTo(geom);
			return geom;
		}

		/// <summary>The stateFlag value changes with changes applied to this geometry.</summary>
		/// <remarks>
		/// The stateFlag value changes with changes applied to this geometry. This
		/// allows the user to keep track of the geometry's state.
		/// </remarks>
		/// <returns>The state of the geometry.</returns>
		public virtual int GetStateFlag()
		{
			m_touchFlag &= unchecked((int)(0x7FFFFFFF));
			return m_touchFlag;
		}

		// Called whenever geometry changes
		internal virtual void _touch()
		{
			lock (this)
			{
				if (m_touchFlag >= 0)
				{
					m_touchFlag += unchecked((int)(0x80000001));
				}
			}
		}

		/// <summary>Describes the degree of acceleration of the geometry.</summary>
		/// <remarks>
		/// Describes the degree of acceleration of the geometry.
		/// Acceleration usually builds a raster and a quadtree.
		/// </remarks>
		public enum GeometryAccelerationDegree
		{
			enumMild,
			enumMedium,
			enumHot
		}

//		/// <exception cref="java.io.ObjectStreamException"/>
//		internal virtual object WriteReplace()
//		{
//			com.epl.geometry.Geometry.Type gt = GetType();
//			if (gt == com.epl.geometry.Geometry.Type.Point)
//			{
//				com.epl.geometry.PtSrlzr pt = new com.epl.geometry.PtSrlzr();
//				pt.SetGeometryByValue((com.epl.geometry.Point)this);
//				return pt;
//			}
//			else
//			{
//				if (gt == com.epl.geometry.Geometry.Type.Envelope)
//				{
//					com.epl.geometry.EnvSrlzr e = new com.epl.geometry.EnvSrlzr();
//					e.SetGeometryByValue((com.epl.geometry.Envelope)this);
//					return e;
//				}
//				else
//				{
//					if (gt == com.epl.geometry.Geometry.Type.Line)
//					{
//						com.epl.geometry.LnSrlzr ln = new com.epl.geometry.LnSrlzr();
//						ln.SetGeometryByValue((com.epl.geometry.Line)this);
//						return ln;
//					}
//				}
//			}
//			com.epl.geometry.GenericGeometrySerializer geomSerializer = new com.epl.geometry.GenericGeometrySerializer();
//			geomSerializer.SetGeometryByValue(this);
//			return geomSerializer;
//		}
//
//		/// <summary>The output of this method can be only used for debugging.</summary>
//		/// <remarks>The output of this method can be only used for debugging. It is subject to change without notice.</remarks>
//		public override string ToString()
//		{
//			string snippet = com.epl.geometry.OperatorExportToJson.Local().Execute(null, this);
//			if (snippet.Length > 200)
//			{
//				return snippet.Substring(0, 197 - 0) + "... (" + snippet.Length + " characters)";
//			}
//			else
//			{
//				return snippet;
//			}
//		}

		/// <summary>
		/// Returns count of geometry vertices:
		/// 1 for Point, 4 for Envelope, get_point_count for MultiVertexGeometry types,
		/// 2 for segment types
		/// Returns 0 if geometry is empty.
		/// </summary>
		public static int Vertex_count(com.epl.geometry.Geometry geom)
		{
			com.epl.geometry.Geometry.Type gt = geom.GetType();
			if (com.epl.geometry.Geometry.IsMultiVertex(gt.Value()))
			{
				return ((com.epl.geometry.MultiVertexGeometry)geom).GetPointCount();
			}
			if (geom.IsEmpty())
			{
				return 0;
			}
			if (gt == com.epl.geometry.Geometry.Type.Envelope)
			{
				return 4;
			}
			if (gt == com.epl.geometry.Geometry.Type.Point)
			{
				return 1;
			}
			if (com.epl.geometry.Geometry.IsSegment(gt.Value()))
			{
				return 2;
			}
			throw new com.epl.geometry.GeometryException("missing type");
		}
	}
}
