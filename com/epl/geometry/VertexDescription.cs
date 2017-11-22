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
	public class VertexDescription
	{
		private double[] m_defaultPointAttributes;

		private int[] m_pointAttributeOffsets;

		internal int m_attributeCount;

		internal int m_total_component_count;

		internal int[] m_semantics;

		internal int[] m_semanticsToIndexMap;

		internal int m_hash;

		internal static double[] _defaultValues = new double[] { 0, 0, com.epl.geometry.NumberUtils.NaN(), 0, 0, 0, 0, 0, 0 };

		internal static int[] _interpolation = new int[] { com.epl.geometry.VertexDescription.Interpolation.LINEAR, com.epl.geometry.VertexDescription.Interpolation.LINEAR, com.epl.geometry.VertexDescription.Interpolation.LINEAR, com.epl.geometry.VertexDescription.Interpolation.NONE, 
			com.epl.geometry.VertexDescription.Interpolation.ANGULAR, com.epl.geometry.VertexDescription.Interpolation.LINEAR, com.epl.geometry.VertexDescription.Interpolation.LINEAR, com.epl.geometry.VertexDescription.Interpolation.LINEAR, com.epl.geometry.VertexDescription.Interpolation
			.NONE };

		internal static int[] _persistence = new int[] { com.epl.geometry.VertexDescription.Persistence.enumDouble, com.epl.geometry.VertexDescription.Persistence.enumDouble, com.epl.geometry.VertexDescription.Persistence.enumDouble, com.epl.geometry.VertexDescription.Persistence.enumInt32
			, com.epl.geometry.VertexDescription.Persistence.enumFloat, com.epl.geometry.VertexDescription.Persistence.enumFloat, com.epl.geometry.VertexDescription.Persistence.enumFloat, com.epl.geometry.VertexDescription.Persistence.enumFloat, com.epl.geometry.VertexDescription.Persistence
			.enumInt32 };

		internal static int[] _persistencesize = new int[] { 4, 8, 4, 8, 1, 2 };

		internal static int[] _components = new int[] { 2, 1, 1, 1, 3, 1, 2, 3, 2 };

		internal VertexDescription()
		{
			m_attributeCount = 0;
			m_total_component_count = 0;
		}

		internal VertexDescription(int hashValue, com.epl.geometry.VertexDescription other)
		{
			m_attributeCount = other.m_attributeCount;
			m_total_component_count = other.m_total_component_count;
			m_semantics = (int [])other.m_semantics.Clone();
			m_semanticsToIndexMap = (int [])other.m_semanticsToIndexMap.Clone();
			m_hash = other.m_hash;
			// Prepare default values for the Point geometry.
			m_pointAttributeOffsets = new int[GetAttributeCount()];
			int offset = 0;
			for (int i = 0; i < GetAttributeCount(); i++)
			{
				m_pointAttributeOffsets[i] = offset;
				offset += GetComponentCount(m_semantics[i]);
			}
			m_total_component_count = offset;
			m_defaultPointAttributes = new double[offset];
			for (int i_1 = 0; i_1 < GetAttributeCount(); i_1++)
			{
				int components = GetComponentCount(GetSemantics(i_1));
				double dv = GetDefaultValue(GetSemantics(i_1));
				for (int icomp = 0; icomp < components; icomp++)
				{
					m_defaultPointAttributes[m_pointAttributeOffsets[i_1] + icomp] = dv;
				}
			}
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

			public const int MAXSEMANTICS = 10;
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
		/// GetAttributeCount() - 1.
		/// </param>
		public int GetSemantics(int attributeIndex)
		{
			if (attributeIndex < 0 || attributeIndex > m_attributeCount)
			{
				throw new System.ArgumentException();
			}
			return m_semantics[attributeIndex];
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

		/// <summary>Returns True for integer persistence type.</summary>
		internal static bool IsIntegerPersistence(int persistence)
		{
			return persistence < com.epl.geometry.VertexDescription.Persistence.enumInt32;
		}

		/// <summary>Returns True for integer semantics type.</summary>
		internal static bool IsIntegerSemantics(int semantics)
		{
			return IsIntegerPersistence(GetPersistence(semantics));
		}

		/// <summary>Returns True if the attribute with the given name and given set exists.</summary>
		/// <param name="semantics">The semantics of the attribute.</param>
		public virtual bool HasAttribute(int semantics)
		{
			return m_semanticsToIndexMap[semantics] >= 0;
		}

		/// <summary>Returns True, if the vertex has Z attribute.</summary>
		public virtual bool HasZ()
		{
			return HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
		}

		/// <summary>Returns True, if the vertex has M attribute.</summary>
		public virtual bool HasM()
		{
			return HasAttribute(com.epl.geometry.VertexDescription.Semantics.M);
		}

		/// <summary>Returns True, if the vertex has ID attribute.</summary>
		public virtual bool HasID()
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

		internal virtual int GetPointAttributeOffset_(int attribute_index)
		{
			return m_pointAttributeOffsets[attribute_index];
		}

		/// <summary>Returns the total component count.</summary>
		public virtual int GetTotalComponentCount()
		{
			return m_total_component_count;
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

		internal static int GetPersistenceFromInt(int size)
		{
			if (size == 4)
			{
				return com.epl.geometry.VertexDescription.Persistence.enumInt32;
			}
			else
			{
				if (size == 8)
				{
					return com.epl.geometry.VertexDescription.Persistence.enumInt64;
				}
				else
				{
					throw new System.ArgumentException();
				}
			}
		}

		public override bool Equals(object _other)
		{
			return (object)this == _other;
		}

		internal virtual int CalculateHashImpl()
		{
			int v = com.epl.geometry.NumberUtils.Hash(m_semantics[0]);
			for (int i = 1; i < m_attributeCount; i++)
			{
				v = com.epl.geometry.NumberUtils.Hash(v, m_semantics[i]);
			}
			return v;
		}

		// if attribute size is 1, it returns 0
		/// <summary>
		/// Returns a packed array of double representation of all ordinates of
		/// attributes of a point, i.e.: X, Y, Z, ID, TEXTURE2D.u, TEXTURE2D.v
		/// </summary>
		internal virtual double[] _getDefaultPointAttributes()
		{
			return m_defaultPointAttributes;
		}

		internal virtual double _getDefaultPointAttributeValue(int attributeIndex, int ordinate)
		{
			return m_defaultPointAttributes[_getPointAttributeOffset(attributeIndex) + ordinate];
		}

		/// <summary>Returns an offset to the first ordinate of the given attribute.</summary>
		/// <remarks>
		/// Returns an offset to the first ordinate of the given attribute. This
		/// method is used for the cases when one wants to have a packed array of
		/// ordinates of all attributes, i.e.: X, Y, Z, ID, TEXTURE2D.u, TEXTURE2D.v
		/// </remarks>
		internal virtual int _getPointAttributeOffset(int attributeIndex)
		{
			return m_pointAttributeOffsets[attributeIndex];
		}

		internal virtual int _getPointAttributeOffsetFromSemantics(int semantics)
		{
			return m_pointAttributeOffsets[GetAttributeIndex(semantics)];
		}

		internal virtual int _getTotalComponents()
		{
			return m_defaultPointAttributes.Length;
		}

		public override int GetHashCode()
		{
			return m_hash;
		}

		internal virtual int _getSemanticsImpl(int attributeIndex)
		{
			return m_semantics[attributeIndex];
		}
		// TODO: clone, equald, hashcode - whats really needed?
	}
}
