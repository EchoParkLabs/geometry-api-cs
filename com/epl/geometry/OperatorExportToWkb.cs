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
	/// <summary>Export to WKB format.</summary>
	public abstract class OperatorExportToWkb : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.ExportToWkb;
		}

		/// <summary>Performs the ExportToWKB operation.</summary>
		/// <param name="exportFlags">
		/// Use the
		/// <see cref="WkbExportFlags"/>
		/// interface.
		/// </param>
		/// <param name="geometry">The Geometry being exported.</param>
		/// <returns>Returns a ByteBuffer object containing the Geometry in WKB format</returns>
		public abstract System.IO.MemoryStream Execute(int exportFlags, com.epl.geometry.Geometry geometry, com.epl.geometry.ProgressTracker progressTracker);

		/// <summary>Performs the ExportToWKB operation.</summary>
		/// <param name="exportFlags">
		/// Use the
		/// <see cref="WkbExportFlags"/>
		/// interface.
		/// </param>
		/// <param name="geometry">The Geometry being exported.</param>
		/// <param name="wkbBuffer">The ByteBuffer to contain the exported Geometry in WKB format.</param>
		/// <returns>If the input buffer is null, then the size needed for the buffer is returned. Otherwise the number of bytes written to the buffer is returned.</returns>
		public abstract int Execute(int exportFlags, com.epl.geometry.Geometry geometry, System.IO.MemoryStream wkbBuffer, com.epl.geometry.ProgressTracker progressTracker);

		public static com.epl.geometry.OperatorExportToWkb Local()
		{
			return (com.epl.geometry.OperatorExportToWkb)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkb);
		}
	}
}
