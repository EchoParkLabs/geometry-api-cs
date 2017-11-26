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
	public class LnSrlzr
	{
		private const long serialVersionUID = 1L;

		internal double[] attribs;

		internal int descriptionBitMask;

		//This is a writeReplace class for Lin
		/// <exception cref="java.io.ObjectStreamException"/>
		public virtual object ReadResolve()
		{
			com.epl.geometry.Line ln = null;
			if (descriptionBitMask == -1)
			{
				return null;
			}
			try
			{
				com.epl.geometry.VertexDescription vd = com.epl.geometry.VertexDescriptionDesignerImpl.GetVertexDescription(descriptionBitMask);
				ln = new com.epl.geometry.Line(vd);
				if (attribs != null)
				{
					ln.SetStartXY(attribs[0], attribs[1]);
					ln.SetEndXY(attribs[2], attribs[3]);
					int index = 4;
					for (int i = 1, n = vd.GetAttributeCount(); i < n; i++)
					{
						int semantics = vd.GetSemantics(i);
						int comps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
						for (int ord = 0; ord < comps; ord++)
						{
							ln.SetStartAttribute(semantics, ord, attribs[index++]);
							ln.SetEndAttribute(semantics, ord, attribs[index++]);
						}
					}
				}
			}
			catch (System.Exception)
			{
				throw new System.IO.InvalidDataException("Cannot read geometry from stream");
			}
			return ln;
		}

		/// <exception cref="java.io.ObjectStreamException"/>
		public virtual void SetGeometryByValue(com.epl.geometry.Line ln)
		{
			try
			{
				attribs = null;
				if (ln == null)
				{
					descriptionBitMask = -1;
				}
				com.epl.geometry.VertexDescription vd = ln.GetDescription();
				descriptionBitMask = vd.m_semanticsBitArray;
				attribs = new double[vd.GetTotalComponentCount() * 2];
				attribs[0] = ln.GetStartX();
				attribs[1] = ln.GetStartY();
				attribs[2] = ln.GetEndX();
				attribs[3] = ln.GetEndY();
				int index = 4;
				for (int i = 1, n = vd.GetAttributeCount(); i < n; i++)
				{
					int semantics = vd.GetSemantics(i);
					int comps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
					for (int ord = 0; ord < comps; ord++)
					{
						attribs[index++] = ln.GetStartAttributeAsDbl(semantics, ord);
						attribs[index++] = ln.GetEndAttributeAsDbl(semantics, ord);
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
