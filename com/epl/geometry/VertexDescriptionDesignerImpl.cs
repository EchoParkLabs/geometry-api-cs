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
	/// <summary>
	/// This factory class allows to describe and create a VertexDescription
	/// instance.
	/// </summary>
	internal sealed class VertexDescriptionDesignerImpl
	{
		internal static com.epl.geometry.VertexDescription GetVertexDescription(int descriptionBitMask)
		{
			return com.epl.geometry.VertexDescriptionHash.GetInstance().FindOrAdd(descriptionBitMask);
		}

		internal static com.epl.geometry.VertexDescription GetMergedVertexDescription(com.epl.geometry.VertexDescription descr1, com.epl.geometry.VertexDescription descr2)
		{
			int mask = descr1.m_semanticsBitArray | descr2.m_semanticsBitArray;
			if ((mask & descr1.m_semanticsBitArray) == mask)
			{
				return descr1;
			}
			else
			{
				if ((mask & descr2.m_semanticsBitArray) == mask)
				{
					return descr2;
				}
			}
			return GetVertexDescription(mask);
		}

		internal static com.epl.geometry.VertexDescription GetMergedVertexDescription(com.epl.geometry.VertexDescription descr, int semantics)
		{
			int mask = descr.m_semanticsBitArray | (1 << semantics);
			if ((mask & descr.m_semanticsBitArray) == mask)
			{
				return descr;
			}
			return GetVertexDescription(mask);
		}

		internal static com.epl.geometry.VertexDescription RemoveSemanticsFromVertexDescription(com.epl.geometry.VertexDescription descr, int semanticsToRemove)
		{
			int mask = (descr.m_semanticsBitArray | (1 << (int)semanticsToRemove)) - (1 << (int)semanticsToRemove);
			if (mask == descr.m_semanticsBitArray)
			{
				return descr;
			}
			return GetVertexDescription(mask);
		}

		internal static com.epl.geometry.VertexDescription GetDefaultDescriptor2D()
		{
			return com.epl.geometry.VertexDescriptionHash.GetInstance().GetVD2D();
		}

		internal static com.epl.geometry.VertexDescription GetDefaultDescriptor3D()
		{
			return com.epl.geometry.VertexDescriptionHash.GetInstance().GetVD3D();
		}

		internal static int[] MapAttributes(com.epl.geometry.VertexDescription src, com.epl.geometry.VertexDescription dest)
		{
			int[] srcToDst = new int[src.GetAttributeCount()];
			java.util.Arrays.Fill(srcToDst, -1);
			for (int i = 0, nsrc = src.GetAttributeCount(); i < nsrc; i++)
			{
				srcToDst[i] = dest.GetAttributeIndex(src.GetSemantics(i));
			}
			return srcToDst;
		}
	}
}
