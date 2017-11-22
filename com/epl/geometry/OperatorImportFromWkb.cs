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
	/// <summary>Import from WKB format.</summary>
	public abstract class OperatorImportFromWkb : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.ImportFromWkb;
		}

		/// <summary>Performs the ImportFromWKB operation.</summary>
		/// <param name="importFlags">
		/// Use the
		/// <see cref="WkbImportFlags"/>
		/// interface.
		/// </param>
		/// <param name="type">
		/// Use the
		/// <see cref="Type"/>
		/// enum.
		/// </param>
		/// <param name="wkbBuffer">The buffer holding the Geometry in wkb format.</param>
		/// <returns>Returns the imported Geometry.</returns>
		public abstract com.epl.geometry.Geometry Execute(int importFlags, com.epl.geometry.Geometry.Type type, System.IO.MemoryStream wkbBuffer, com.epl.geometry.ProgressTracker progress_tracker);

		/// <summary>Performs the ImportFromWkb operation.</summary>
		/// <param name="importFlags">
		/// Use the
		/// <see cref="WkbImportFlags"/>
		/// interface.
		/// </param>
		/// <param name="wkbBuffer">The buffer holding the Geometry in wkb format.</param>
		/// <returns>Returns the imported OGCStructure.</returns>
		public abstract com.epl.geometry.OGCStructure ExecuteOGC(int importFlags, System.IO.MemoryStream wkbBuffer, com.epl.geometry.ProgressTracker progress_tracker);

		public static com.epl.geometry.OperatorImportFromWkb Local()
		{
			return (com.epl.geometry.OperatorImportFromWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ImportFromWkb);
		}
	}
}
