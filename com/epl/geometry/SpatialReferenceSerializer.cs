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
	[System.Serializable]
	internal sealed class SpatialReferenceSerializer
	{
		private const long serialVersionUID = 10000L;

		internal string wkt = null;

		internal int wkid = 0;

		/// <exception cref="java.io.ObjectStreamException"/>
		internal object ReadResolve()
		{
			com.epl.geometry.SpatialReference sr = null;
			try
			{
				if (wkid > 0)
				{
					sr = com.epl.geometry.SpatialReference.Create(wkid);
				}
				else
				{
					sr = com.epl.geometry.SpatialReference.Create(wkt);
				}
			}
			catch (System.Exception)
			{
				throw new System.IO.InvalidDataException("Cannot read spatial reference from stream");
			}
			return sr;
		}

		/// <exception cref="java.io.ObjectStreamException"/>
		public void SetSpatialReferenceByValue(com.epl.geometry.SpatialReference sr)
		{
			try
			{
				if (sr.GetID() > 0)
				{
					wkid = sr.GetID();
				}
				else
				{
					wkt = sr.GetText();
				}
			}
			catch (System.Exception)
			{
				throw new System.IO.InvalidDataException("Cannot serialize this geometry");
			}
		}
	}
}
