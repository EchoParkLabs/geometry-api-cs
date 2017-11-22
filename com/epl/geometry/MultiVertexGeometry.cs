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
	/// <summary>This class is a base for geometries with many vertices.</summary>
	/// <remarks>
	/// This class is a base for geometries with many vertices.
	/// The vertex attributes are stored in separate arrays of corresponding type.
	/// There are as many arrays as there are attributes in the vertex.
	/// </remarks>
	[System.Serializable]
	public abstract class MultiVertexGeometry : com.epl.geometry.Geometry
	{
		protected internal override void _assignVertexDescriptionImpl(com.epl.geometry.VertexDescription newDescription)
		{
			throw new com.epl.geometry.GeometryException("invalid call");
		}

		/// <summary>Returns the total vertex count in this Geometry.</summary>
		public abstract int GetPointCount();

		/// <summary>Returns given vertex of the Geometry.</summary>
		public abstract com.epl.geometry.Point GetPoint(int index);

		// Java only
		/// <summary>Returns given vertex of the Geometry by value.</summary>
		public virtual void GetPoint(int index, com.epl.geometry.Point ptOut)
		{
			GetPointByVal(index, ptOut);
		}

		/// <summary>Sets the vertex at given index of the Geometry.</summary>
		/// <param name="index">The index of the vertex being changed.</param>
		/// <param name="pointSrc">
		/// The Point instance to set given vertex attributes from. The
		/// pointSrc can not be empty. <br />
		/// The method throws if the pointSrc is not of the Point type. <br />
		/// The attributes, that are present in the pointSrc and missing
		/// in this Geometry, will be added to the Geometry. <br />
		/// The vertex attributes missing in the pointSrc but present in
		/// the Geometry will be set to the default values (see
		/// VertexDescription::GetDefaultValue).
		/// </param>
		public abstract void SetPoint(int index, com.epl.geometry.Point pointSrc);

		// Java only
		/// <summary>Returns XY coordinates of the given vertex of the Geometry.</summary>
		public abstract com.epl.geometry.Point2D GetXY(int index);

		public abstract void GetXY(int index, com.epl.geometry.Point2D pt);

		/// <summary>Sets XY coordinates of the given vertex of the Geometry.</summary>
		/// <remarks>
		/// Sets XY coordinates of the given vertex of the Geometry. All other
		/// attributes are unchanged.
		/// </remarks>
		public abstract void SetXY(int index, com.epl.geometry.Point2D pt);

		/// <summary>Returns XYZ coordinates of the given vertex of the Geometry.</summary>
		/// <remarks>
		/// Returns XYZ coordinates of the given vertex of the Geometry. If the
		/// Geometry has no Z's, the default value for Z is returned (0).
		/// </remarks>
		internal abstract com.epl.geometry.Point3D GetXYZ(int index);

		/// <summary>Sets XYZ coordinates of the given vertex of the Geometry.</summary>
		/// <remarks>
		/// Sets XYZ coordinates of the given vertex of the Geometry. If Z attribute
		/// is not present in this Geometry, it is added. All other attributes are
		/// unchanged.
		/// </remarks>
		internal abstract void SetXYZ(int index, com.epl.geometry.Point3D pt);

		/// <summary>Returns XY coordinates as an array.</summary>
		public virtual com.epl.geometry.Point2D[] GetCoordinates2D()
		{
			com.epl.geometry.Point2D[] arr = new com.epl.geometry.Point2D[GetPointCount()];
			QueryCoordinates(arr);
			return arr;
		}

		/// <summary>Returns XYZ coordinates as an array.</summary>
		internal virtual com.epl.geometry.Point3D[] GetCoordinates3D()
		{
			com.epl.geometry.Point3D[] arr = new com.epl.geometry.Point3D[GetPointCount()];
			QueryCoordinates(arr);
			return arr;
		}

		public abstract void QueryCoordinates(com.epl.geometry.Point[] dst);

		/// <summary>Queries XY coordinates as an array.</summary>
		/// <remarks>
		/// Queries XY coordinates as an array. The array must be larg enough (See
		/// GetPointCount()).
		/// </remarks>
		public abstract void QueryCoordinates(com.epl.geometry.Point2D[] dst);

		/// <summary>Queries XYZ coordinates as an array.</summary>
		/// <remarks>
		/// Queries XYZ coordinates as an array. The array must be larg enough (See
		/// GetPointCount()).
		/// </remarks>
		internal abstract void QueryCoordinates(com.epl.geometry.Point3D[] dst);

		/// <summary>Returns value of the given vertex attribute as double.</summary>
		/// <param name="semantics">The atribute semantics.</param>
		/// <param name="index">is the vertex index in the Geometry.</param>
		/// <param name="ordinate">
		/// is the ordinate of a vertex attribute (for example, y has
		/// ordinate of 1, because it is second ordinate of POSITION)
		/// If attribute is not present, the default value is returned.
		/// See VertexDescription::GetDefaultValue() method.
		/// </param>
		internal abstract double GetAttributeAsDbl(int semantics, int index, int ordinate);

		/// <summary>Returns value of the given vertex attribute as int.</summary>
		/// <param name="semantics">The atribute semantics.</param>
		/// <param name="index">is the vertex index in the Geometry.</param>
		/// <param name="ordinate">
		/// is the ordinate of a vertex attribute (for example, y has
		/// ordinate of 1, because it is second ordinate of POSITION)
		/// If attribute is not present, the default value is returned.
		/// See VertexDescription::GetDefaultValue() method. Avoid using
		/// this method on non-integer atributes.
		/// </param>
		internal abstract int GetAttributeAsInt(int semantics, int index, int ordinate);

		/// <summary>Sets the value of given attribute at given posisiotnsis.</summary>
		/// <param name="semantics">The atribute semantics.</param>
		/// <param name="index">is the vertex index in the Geometry.</param>
		/// <param name="ordinate">
		/// is the ordinate of a vertex attribute (for example, y has
		/// ordinate of 1, because it is seond ordinate of POSITION)
		/// </param>
		/// <param name="value">
		/// is the value to set. as well as the number of components of
		/// the attribute.
		/// If the attribute is not present in this Geometry, it is added.
		/// </param>
		internal abstract void SetAttribute(int semantics, int index, int ordinate, double value);

		/// <summary>Same as above, but works with ints.</summary>
		/// <remarks>
		/// Same as above, but works with ints. Avoid using this method on
		/// non-integer atributes because some double attributes may have NaN default
		/// values (e.g. Ms)
		/// </remarks>
		internal abstract void SetAttribute(int semantics, int index, int ordinate, int value);

		/// <summary>Returns given vertex of the Geometry.</summary>
		/// <remarks>
		/// Returns given vertex of the Geometry. The outPoint will have same
		/// VertexDescription as this Geometry.
		/// </remarks>
		internal abstract void GetPointByVal(int index, com.epl.geometry.Point outPoint);

		/// <summary>Sets the vertex at given index of the Geometry.</summary>
		/// <param name="index">The index of the vertex being changed.</param>
		/// <param name="pointSrc">
		/// The Point instance to set given vertex attributes from. The
		/// pointSrc can not be empty. <br />
		/// The method throws if the pointSrc is not of the Point type. <br />
		/// The attributes, that are present in the pointSrc and missing
		/// in this Geometry, will be added to the Geometry. <br />
		/// The vertex attributes missing in the pointSrc but present in
		/// the Geometry will be set to the default values (see
		/// VertexDescription::GetDefaultValue).
		/// </param>
		internal abstract void SetPointByVal(int index, com.epl.geometry.Point pointSrc);
	}
}
