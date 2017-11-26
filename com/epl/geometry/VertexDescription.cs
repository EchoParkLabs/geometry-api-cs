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
	/// <summary>Describes the vertex format of a Geometry.</summary>
	/// <remarks>
	/// Describes the vertex format of a Geometry.
	/// Geometry objects store vertices. The vertex is a multi attribute entity. It
	/// has mandatory X, Y coordinates. In addition it may have Z, M, ID, and other
	/// user specified attributes. Geometries point to VertexDescription instances.
	/// If the two Geometries have same set of attributes, they point to the same
	/// VertexDescription instance. <br />
	/// To create a new VertexDescription use the VertexDescriptionDesigner class. <br />
	/// The VertexDescription allows to add new attribute types easily (see ID2). <br />
	/// The attributes are stored sorted by Semantics value. <br />
	/// Note: You could also think of the VertexDescription as a schema of a database
	/// table. You may look the vertices of a Geometry as if they are stored in a
	/// database table, and the VertexDescription defines the fields of the table.
	/// </remarks>
	public sealed class VertexDescription
	{
		/// <summary>
		/// Describes the attribute and, in case of predefined attributes, provides a
		/// hint of the attribute use.
		/// </summary>
		public abstract class Semantics
		{
			public const int POSITION = 0;

			public const int Z = 1;

			public const int M = 2;

			public const int ID = 3;

			public const int NORMAL = 4;

			public const int TEXTURE1D = 5;

			public const int TEXTURE2D = 6;

			public const int TEXTURE3D = 7;

			public const int ID2 = 8;

			public const int MAXSEMANTICS = 8;
			// xy coordinates of a point (2D
			// vector of double, linear
			// interpolation)
			// z coordinates of a point (double,
			// linear interpolation)
			// m attribute (double, linear
			// interpolation)
			// id (int, no interpolation)
			// xyz coordinates of normal vector
			// (float, angular interpolation)
			// u coordinates of texture
			// (float, linear interpolation)
			// uv coordinates of texture
			// (float, linear interpolation)
			// uvw coordinates of texture
			// (float, linear interpolation)
			// two component ID
			// the max semantics value
		}

		public static class SemanticsConstants
		{
		}

		/// <summary>Specifies how the attribute is interpolated along the segments.</summary>
		/// <remarks>
		/// Specifies how the attribute is interpolated along the segments. are
		/// represented as int64
		/// </remarks>
		internal abstract class Interpolation
		{
			public const int NONE = 0;

			public const int LINEAR = 1;

			public const int ANGULAR = 2;
		}

		internal static class InterpolationConstants
		{
		}

		/// <summary>Specifies the type of the attribute.</summary>
		internal abstract class Persistence
		{
			public const int enumFloat = 0;

			public const int enumDouble = 1;

			public const int enumInt32 = 2;

			public const int enumInt64 = 3;

			public const int enumInt8 = 4;

			public const int enumInt16 = 5;
			// 8 bit integer. Can be signed or
			// unsigned depending on
			// platform.
		}

		internal static class PersistenceConstants
		{
		}

		/// <summary>Returns the attribute count of this description.</summary>
		/// <remarks>
		/// Returns the attribute count of this description. The value is always
		/// greater or equal to 1. The first attribute is always a POSITION.
		/// </remarks>
		public int GetAttributeCount()
		{
			return m_attributeCount;
		}

		/// <summary>Returns the semantics of the given attribute.</summary>
		/// <param name="attributeIndex">
		/// The index of the attribute in the description. Max value is
		/// getAttributeCount() - 1.
		/// </param>
		public int GetSemantics(int attributeIndex)
		{
			return m_indexToSemantics[attributeIndex];
		}

		/// <summary>Returns the index the given attribute in the vertex description.</summary>
		/// <param name="semantics"/>
		/// <returns>Returns the attribute index or -1 of the attribute does not exist</returns>
		public int GetAttributeIndex(int semantics)
		{
			return m_semanticsToIndexMap[semantics];
		}

		/// <summary>Returns the interpolation type for the attribute.</summary>
		/// <param name="semantics">The semantics of the attribute.</param>
		internal static int GetInterpolation(int semantics)
		{
			return _interpolation[semantics];
		}

		/// <summary>Returns the persistence type for the attribute.</summary>
		/// <param name="semantics">The semantics of the attribute.</param>
		internal static int GetPersistence(int semantics)
		{
			return _persistence[semantics];
		}

		/// <summary>Returns the size of the persistence type in bytes.</summary>
		/// <param name="persistence">The persistence type to query.</param>
		internal static int GetPersistenceSize(int persistence)
		{
			return _persistencesize[persistence];
		}

		/// <summary>Returns the size of the semantics in bytes.</summary>
		internal static int GetPersistenceSizeSemantics(int semantics)
		{
			return GetPersistenceSize(GetPersistence(semantics)) * GetComponentCount(semantics);
		}

		/// <summary>Returns the number of the components of the given semantics.</summary>
		/// <remarks>
		/// Returns the number of the components of the given semantics. For example,
		/// it returns 2 for the POSITION.
		/// </remarks>
		/// <param name="semantics">The semantics of the attribute.</param>
		public static int GetComponentCount(int semantics)
		{
			return _components[semantics];
		}

		/// <summary>Returns True if the attribute with the given name and given set exists.</summary>
		/// <param name="semantics">The semantics of the attribute.</param>
		public bool HasAttribute(int semantics)
		{
			return (m_semanticsBitArray & (1 << semantics)) != 0;
		}

		/// <summary>
		/// Returns True if this vertex description includes all attributes from the
		/// src.
		/// </summary>
		/// <param name="src">The Vertex_description to compare with.</param>
		/// <returns>
		/// The function returns false, only when this description does not
		/// have some of the attribute that src has.
		/// </returns>
		public bool HasAttributesFrom(com.epl.geometry.VertexDescription src)
		{
			return (m_semanticsBitArray & src.m_semanticsBitArray) == src.m_semanticsBitArray;
		}

		/// <summary>Returns True, if the vertex has Z attribute.</summary>
		public bool HasZ()
		{
			return HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
		}

		/// <summary>Returns True, if the vertex has M attribute.</summary>
		public bool HasM()
		{
			return HasAttribute(com.epl.geometry.VertexDescription.Semantics.M);
		}

		/// <summary>Returns True, if the vertex has ID attribute.</summary>
		public bool HasID()
		{
			return HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
		}

		/// <summary>
		/// Returns default value for each ordinate of the vertex attribute with
		/// given semantics.
		/// </summary>
		public static double GetDefaultValue(int semantics)
		{
			return _defaultValues[semantics];
		}

		internal int GetPointAttributeOffset_(int attributeIndex)
		{
			return m_pointAttributeOffsets[attributeIndex];
		}

		/// <summary>Returns the total component count.</summary>
		public int GetTotalComponentCount()
		{
			return m_totalComponentCount;
		}

		/// <summary>Checks if the given value is the default one.</summary>
		/// <remarks>
		/// Checks if the given value is the default one. The simple equality test
		/// with GetDefaultValue does not work due to the use of NaNs as default
		/// value for some parameters.
		/// </remarks>
		public static bool IsDefaultValue(int semantics, double v)
		{
			return com.epl.geometry.NumberUtils.DoubleToInt64Bits(_defaultValues[semantics]) == com.epl.geometry.NumberUtils.DoubleToInt64Bits(v);
		}

		internal static bool IsIntegerPersistence(int persistence)
		{
			return persistence >= com.epl.geometry.VertexDescription.Persistence.enumInt32;
		}

		internal static bool IsIntegerSemantics(int semantics)
		{
			return IsIntegerPersistence(GetPersistence(semantics));
		}

		public override bool Equals(object _other)
		{
			return (object)this == _other;
		}

		/// <summary>
		/// Returns a packed array of double representation of all ordinates of
		/// attributes of a point, i.e.: X, Y, Z, ID, TEXTURE2D.u, TEXTURE2D.v
		/// </summary>
		internal double[] _getDefaultPointAttributes()
		{
			return m_defaultPointAttributes;
		}

		internal double _getDefaultPointAttributeValue(int attributeIndex, int ordinate)
		{
			return m_defaultPointAttributes[_getPointAttributeOffset(attributeIndex) + ordinate];
		}

		/// <summary>Returns an offset to the first ordinate of the given attribute.</summary>
		/// <remarks>
		/// Returns an offset to the first ordinate of the given attribute. This
		/// method is used for the cases when one wants to have a packed array of
		/// ordinates of all attributes, i.e.: X, Y, Z, ID, TEXTURE2D.u, TEXTURE2D.v
		/// </remarks>
		internal int _getPointAttributeOffset(int attributeIndex)
		{
			return m_pointAttributeOffsets[attributeIndex];
		}

		internal int _getPointAttributeOffsetFromSemantics(int semantics)
		{
			return m_pointAttributeOffsets[GetAttributeIndex(semantics)];
		}

		public override int GetHashCode()
		{
			return m_hash;
		}

		internal int _getSemanticsImpl(int attributeIndex)
		{
			return m_indexToSemantics[attributeIndex];
		}

		internal VertexDescription(int bitMask)
		{
			m_semanticsBitArray = bitMask;
			m_attributeCount = 0;
			m_totalComponentCount = 0;
			m_semanticsToIndexMap = new int[com.epl.geometry.VertexDescription.Semantics.MAXSEMANTICS + 1];
			java.util.Arrays.Fill(m_semanticsToIndexMap, -1);
			for (int i = 0, flag = 1, n = com.epl.geometry.VertexDescription.Semantics.MAXSEMANTICS + 1; i < n; i++)
			{
				if ((bitMask & flag) != 0)
				{
					m_semanticsToIndexMap[i] = m_attributeCount;
					m_attributeCount++;
					int comps = GetComponentCount(i);
					m_totalComponentCount += comps;
				}
				flag <<= 1;
			}
			m_indexToSemantics = new int[m_attributeCount];
			for (int i_1 = 0, n = com.epl.geometry.VertexDescription.Semantics.MAXSEMANTICS + 1; i_1 < n; i_1++)
			{
				int attrib = m_semanticsToIndexMap[i_1];
				if (attrib >= 0)
				{
					m_indexToSemantics[attrib] = i_1;
				}
			}
			m_defaultPointAttributes = new double[m_totalComponentCount];
			m_pointAttributeOffsets = new int[m_attributeCount];
			int offset = 0;
			for (int i_2 = 0, n = m_attributeCount; i_2 < n; i_2++)
			{
				int semantics = GetSemantics(i_2);
				int comps = GetComponentCount(semantics);
				double v = GetDefaultValue(semantics);
				m_pointAttributeOffsets[i_2] = offset;
				for (int icomp = 0; icomp < comps; icomp++)
				{
					m_defaultPointAttributes[offset] = v;
					offset++;
				}
			}
			m_hash = com.epl.geometry.NumberUtils.Hash(m_semanticsBitArray);
		}

		private int m_attributeCount;

		internal int m_semanticsBitArray;

		private int m_totalComponentCount;

		private int m_hash;

		private int[] m_semanticsToIndexMap;

		private int[] m_indexToSemantics;

		private int[] m_pointAttributeOffsets;

		private double[] m_defaultPointAttributes;

		internal static readonly double[] _defaultValues = new double[] { 0, 0, com.epl.geometry.NumberUtils.NaN(), 0, 0, 0, 0, 0, 0 };

		internal static readonly int[] _interpolation = new int[] { com.epl.geometry.VertexDescription.Interpolation.LINEAR, com.epl.geometry.VertexDescription.Interpolation.LINEAR, com.epl.geometry.VertexDescription.Interpolation.LINEAR, com.epl.geometry.VertexDescription.Interpolation
			.NONE, com.epl.geometry.VertexDescription.Interpolation.ANGULAR, com.epl.geometry.VertexDescription.Interpolation.LINEAR, com.epl.geometry.VertexDescription.Interpolation.LINEAR, com.epl.geometry.VertexDescription.Interpolation.LINEAR, com.epl.geometry.VertexDescription.Interpolation
			.NONE };

		internal static readonly int[] _persistence = new int[] { com.epl.geometry.VertexDescription.Persistence.enumDouble, com.epl.geometry.VertexDescription.Persistence.enumDouble, com.epl.geometry.VertexDescription.Persistence.enumDouble, com.epl.geometry.VertexDescription.Persistence
			.enumInt32, com.epl.geometry.VertexDescription.Persistence.enumFloat, com.epl.geometry.VertexDescription.Persistence.enumFloat, com.epl.geometry.VertexDescription.Persistence.enumFloat, com.epl.geometry.VertexDescription.Persistence.enumFloat, com.epl.geometry.VertexDescription.Persistence
			.enumInt32 };

		internal static readonly int[] _persistencesize = new int[] { 4, 8, 4, 8, 1, 2 };

		internal static readonly int[] _components = new int[] { 2, 1, 1, 1, 3, 1, 2, 3, 2 };
		//the main component
	}
}
