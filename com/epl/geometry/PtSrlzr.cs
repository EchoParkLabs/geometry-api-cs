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
	[System.Serializable]
	public class PtSrlzr
	{
		private const long serialVersionUID = 1L;

		internal double[] attribs;

		internal int descriptionBitMask;

		//This is a writeReplace class for Point
		/// <exception cref="java.io.ObjectStreamException"/>
		public virtual object ReadResolve()
		{
			com.epl.geometry.Point point = null;
			try
			{
				com.epl.geometry.VertexDescription vd = com.epl.geometry.VertexDescriptionDesignerImpl.GetVertexDescription(descriptionBitMask);
				point = new com.epl.geometry.Point(vd);
				if (attribs != null)
				{
					point.SetXY(attribs[0], attribs[1]);
					int index = 2;
					for (int i = 1, n = vd.GetAttributeCount(); i < n; i++)
					{
						int semantics = vd.GetSemantics(i);
						int comps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
						for (int ord = 0; ord < comps; ord++)
						{
							point.SetAttribute(semantics, ord, attribs[index++]);
						}
					}
				}
			}
			catch (System.Exception)
			{
				throw new System.IO.InvalidDataException("Cannot read geometry from stream");
			}
			return point;
		}

		/// <exception cref="java.io.ObjectStreamException"/>
		public virtual void SetGeometryByValue(com.epl.geometry.Point point)
		{
			try
			{
				attribs = null;
				if (point == null)
				{
					descriptionBitMask = 1;
				}
				com.epl.geometry.VertexDescription vd = point.GetDescription();
				descriptionBitMask = vd.m_semanticsBitArray;
				if (point.IsEmpty())
				{
					return;
				}
				attribs = new double[vd.GetTotalComponentCount()];
				attribs[0] = point.GetX();
				attribs[1] = point.GetY();
				int index = 2;
				for (int i = 1, n = vd.GetAttributeCount(); i < n; i++)
				{
					int semantics = vd.GetSemantics(i);
					int comps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
					for (int ord = 0; ord < comps; ord++)
					{
						attribs[index++] = point.GetAttributeAsDbl(semantics, ord);
					}
				}
			}
			catch (System.Exception)
			{
				throw new System.IO.InvalidDataException("Cannot serialize this geometry");
			}
		}
	}
}
