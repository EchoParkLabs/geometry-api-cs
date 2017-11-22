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
	/// <summary>A runtime exception raised when a geometry related exception occurs.</summary>
	[System.Serializable]
	public class GeometryException : System.Exception
	{
		private const long serialVersionUID = 1L;

		/// <summary>Constructs a Geometry Exception with the given error string/message.</summary>
		/// <param name="str">- The error string.</param>
		public GeometryException(string str)
			: base(str)
		{
		}

		internal static com.epl.geometry.GeometryException GeometryInternalError()
		{
			return new com.epl.geometry.GeometryException("internal error");
		}
	}
}
