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
	/// <summary>A class that represents the spatial reference for the geometry.</summary>
	/// <remarks>
	/// A class that represents the spatial reference for the geometry.
	/// This class provide tolerance value for the topological and relational operations.
	/// </remarks>
	[System.Serializable]
	public abstract class SpatialReference
	{
		private const long serialVersionUID = 2L;

		// Note: We use writeReplace with SpatialReferenceSerializer. This field is
		// irrelevant. Needs to be removed after final.
		/// <summary>
		/// Creates an instance of the spatial reference based on the provided well
		/// known ID for the horizontal coordinate system.
		/// </summary>
		/// <param name="wkid">The well-known ID.</param>
		/// <returns>SpatialReference The spatial reference.</returns>
		/// <exception cref="System.ArgumentException">if wkid is not supported or does not exist.</exception>
		public static com.epl.geometry.SpatialReference Create(int wkid)
		{
			com.epl.geometry.SpatialReferenceImpl spatRef = com.epl.geometry.SpatialReferenceImpl.CreateImpl(wkid);
			return spatRef;
		}

		/// <summary>
		/// Creates an instance of the spatial reference based on the provided well
		/// known text representation for the horizontal coordinate system.
		/// </summary>
		/// <param name="wktext">
		/// The well-known text string representation of spatial
		/// reference.
		/// </param>
		/// <returns>SpatialReference The spatial reference.</returns>
		public static com.epl.geometry.SpatialReference Create(string wktext)
		{
			return com.epl.geometry.SpatialReferenceImpl.CreateImpl(wktext);
		}

		/// <returns>boolean Is spatial reference local?</returns>
		internal virtual bool IsLocal()
		{
			return false;
		}

		/// <summary>Returns spatial reference from the JsonParser.</summary>
		/// <param name="parser">The JSON parser.</param>
		/// <returns>
		/// The spatial reference or null if there is no spatial reference
		/// information, or the parser does not point to an object start.
		/// </returns>
		/// <exception cref="System.Exception">if parsing has failed</exception>
//		public static com.epl.geometry.SpatialReference FromJson(com.fasterxml.jackson.core.JsonParser parser)
//		{
////			return FromJson(new com.epl.geometry.JsonParserReader(parser));
////		}
//
//		/// <exception cref="System.Exception"/>
//		public static com.epl.geometry.SpatialReference FromJson(string @string)
//		{
//			return FromJson(com.epl.geometry.JsonParserReader.CreateFromString(@string));
//		}
//
//		/// <exception cref="System.Exception"/>
//		public static com.epl.geometry.SpatialReference FromJson(com.epl.geometry.JsonReader parser)
//		{
//			// Note this class is processed specially: it is expected that the
//			// iterator points to the first element of the SR object.
//			bool bFoundWkid = false;
//			bool bFoundLatestWkid = false;
//			bool bFoundVcsWkid = false;
//			bool bFoundLatestVcsWkid = false;
//			bool bFoundWkt = false;
//			int wkid = -1;
//			int latestWkid = -1;
//			int vcs_wkid = -1;
//			int latestVcsWkid = -1;
//			string wkt = null;
//			while (parser.NextToken() != com.epl.geometry.JsonReader.Token.END_OBJECT)
//			{
//				string name = parser.CurrentString();
//				parser.NextToken();
//				if (!bFoundWkid && name.Equals("wkid"))
//				{
//					bFoundWkid = true;
//					if (parser.CurrentToken() == com.epl.geometry.JsonReader.Token.VALUE_NUMBER_INT)
//					{
//						wkid = parser.CurrentIntValue();
//					}
//				}
//				else
//				{
//					if (!bFoundLatestWkid && name.Equals("latestWkid"))
//					{
//						bFoundLatestWkid = true;
//						if (parser.CurrentToken() == com.epl.geometry.JsonReader.Token.VALUE_NUMBER_INT)
//						{
//							latestWkid = parser.CurrentIntValue();
//						}
//					}
//					else
//					{
//						if (!bFoundWkt && name.Equals("wkt"))
//						{
//							bFoundWkt = true;
//							if (parser.CurrentToken() == com.epl.geometry.JsonReader.Token.VALUE_STRING)
//							{
//								wkt = parser.CurrentString();
//							}
//						}
//						else
//						{
//							if (!bFoundVcsWkid && name.Equals("vcsWkid"))
//							{
//								bFoundVcsWkid = true;
//								if (parser.CurrentToken() == com.epl.geometry.JsonReader.Token.VALUE_NUMBER_INT)
//								{
//									vcs_wkid = parser.CurrentIntValue();
//								}
//							}
//							else
//							{
//								if (!bFoundLatestVcsWkid && name.Equals("latestVcsWkid"))
//								{
//									bFoundLatestVcsWkid = true;
//									if (parser.CurrentToken() == com.epl.geometry.JsonReader.Token.VALUE_NUMBER_INT)
//									{
//										latestVcsWkid = parser.CurrentIntValue();
//									}
//								}
//							}
//						}
//					}
//				}
//			}
//			if (latestVcsWkid <= 0 && vcs_wkid > 0)
//			{
//				latestVcsWkid = vcs_wkid;
//			}
//			// iter.step_out_after(); do not step out for the spatial reference,
//			// because this method is used standalone
//			com.epl.geometry.SpatialReference spatial_reference = null;
//			if (wkt != null && wkt.Length != 0)
//			{
//				try
//				{
//					spatial_reference = com.epl.geometry.SpatialReference.Create(wkt);
//				}
//				catch (System.Exception)
//				{
//				}
//			}
//			if (spatial_reference == null && latestWkid > 0)
//			{
//				try
//				{
//					spatial_reference = com.epl.geometry.SpatialReference.Create(latestWkid);
//				}
//				catch (System.Exception)
//				{
//				}
//			}
//			if (spatial_reference == null && wkid > 0)
//			{
//				try
//				{
//					spatial_reference = com.epl.geometry.SpatialReference.Create(wkid);
//				}
//				catch (System.Exception)
//				{
//				}
//			}
//			return spatial_reference;
//		}

		/// <summary>
		/// Returns the well known ID for the horizontal coordinate system of the
		/// spatial reference.
		/// </summary>
		/// <returns>wkid The well known ID.</returns>
		public abstract int GetID();

		public abstract string GetText();

		/// <summary>
		/// Returns the oldest value of the well known ID for the horizontal
		/// coordinate system of the spatial reference.
		/// </summary>
		/// <remarks>
		/// Returns the oldest value of the well known ID for the horizontal
		/// coordinate system of the spatial reference. This ID is used for JSON
		/// serialization. Not public.
		/// </remarks>
		internal abstract int GetOldID();

		/// <summary>
		/// Returns the latest value of the well known ID for the horizontal
		/// coordinate system of the spatial reference.
		/// </summary>
		/// <remarks>
		/// Returns the latest value of the well known ID for the horizontal
		/// coordinate system of the spatial reference. This ID is used for JSON
		/// serialization. Not public.
		/// </remarks>
		internal abstract int GetLatestID();

		/// <summary>Returns the XY tolerance of the spatial reference.</summary>
		/// <remarks>
		/// Returns the XY tolerance of the spatial reference.
		/// The tolerance value defines the precision of topological operations, and
		/// "thickness" of boundaries of geometries for relational operations.
		/// When two points have xy coordinates closer than the tolerance value, they
		/// are considered equal. As well as when a point is within tolerance from
		/// the line, the point is assumed to be on the line.
		/// During topological operations the tolerance is increased by a factor of
		/// about 1.41 and any two points within that distance are snapped
		/// together.
		/// </remarks>
		/// <returns>The XY tolerance of the spatial reference.</returns>
		public virtual double GetTolerance()
		{
			return GetTolerance(com.epl.geometry.VertexDescription.Semantics.POSITION);
		}

		/// <summary>Get the XY tolerance of the spatial reference</summary>
		/// <returns>The XY tolerance of the spatial reference as double.</returns>
		internal abstract double GetTolerance(int semantics);

		/// <exception cref="java.io.ObjectStreamException"/>
		internal virtual object WriteReplace()
		{
			com.epl.geometry.SpatialReferenceSerializer srSerializer = new com.epl.geometry.SpatialReferenceSerializer();
			srSerializer.SetSpatialReferenceByValue(this);
			return srSerializer;
		}

		/// <summary>Returns string representation of the class for debugging purposes.</summary>
		/// <remarks>
		/// Returns string representation of the class for debugging purposes. The
		/// format and content of the returned string is not part of the contract of
		/// the method and is subject to change in any future release or patch
		/// without further notice.
		/// </remarks>
		public override string ToString()
		{
			return "[ tol: " + GetTolerance() + "; wkid: " + GetID() + "; wkt: " + GetText() + "]";
		}
	}
}
