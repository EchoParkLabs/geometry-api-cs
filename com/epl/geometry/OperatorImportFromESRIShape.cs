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
	/// <summary>Import from ESRI shape format.</summary>
	public abstract class OperatorImportFromESRIShape : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.ImportFromESRIShape;
		}

		/// <summary>Performs the ImportFromESRIShape operation on a stream of shape buffers</summary>
		/// <param name="importFlags">
		/// Use the
		/// <see cref="ShapeImportFlags"/>
		/// interface. The default is 0, which means geometry comes from a trusted source and is topologically simple.
		/// If the geometry comes from non-trusted source (that is it can be non-simple), pass ShapeImportNonTrusted.
		/// </param>
		/// <param name="type">
		/// The geometry type that you want to import. Use the
		/// <see cref="Type"/>
		/// enum. It can be Geometry.Type.Unknown if the type of geometry has to be
		/// figured out from the shape buffer.
		/// </param>
		/// <param name="shapeBuffers">The cursor over shape buffers that hold the Geometries in ESRIShape format.</param>
		/// <returns>Returns a GeometryCursor.</returns>
		internal abstract com.epl.geometry.GeometryCursor Execute(int importFlags, com.epl.geometry.Geometry.Type type, com.epl.geometry.ByteBufferCursor shapeBuffers);

		/// <summary>Performs the ImportFromESRIShape operation.</summary>
		/// <param name="importFlags">
		/// Use the
		/// <see cref="ShapeImportFlags"/>
		/// interface. The default is 0, which means geometry comes from a trusted source and is topologically simple.
		/// If the geometry comes from non-trusted source (that is it can be non-simple), pass ShapeImportNonTrusted.
		/// </param>
		/// <param name="type">
		/// The geometry type that you want to import. Use the
		/// <see cref="Type"/>
		/// enum. It can be Geometry.Type.Unknown if the type of geometry has to be
		/// figured out from the shape buffer.
		/// </param>
		/// <param name="shapeBuffer">The buffer holding the Geometry in ESRIShape format.</param>
		/// <returns>Returns the imported Geometry.</returns>
		public abstract com.epl.geometry.Geometry Execute(int importFlags, com.epl.geometry.Geometry.Type type, System.IO.MemoryStream shapeBuffer);

		public static com.epl.geometry.OperatorImportFromESRIShape Local()
		{
			return (com.epl.geometry.OperatorImportFromESRIShape)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromESRIShape);
		}
	}
}
