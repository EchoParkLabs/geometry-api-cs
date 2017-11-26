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
	public class EnvSrlzr
	{
		private const long serialVersionUID = 1L;

		internal double[] attribs;

		internal int descriptionBitMask;

		//This is a writeReplace class for Envelope
		/// <exception cref="java.io.ObjectStreamException"/>
		public virtual object ReadResolve()
		{
			com.epl.geometry.Envelope env = null;
			if (descriptionBitMask == -1)
			{
				return null;
			}
			try
			{
				com.epl.geometry.VertexDescription vd = com.epl.geometry.VertexDescriptionDesignerImpl.GetVertexDescription(descriptionBitMask);
				env = new com.epl.geometry.Envelope(vd);
				if (attribs != null)
				{
					env.SetCoords(attribs[0], attribs[1], attribs[2], attribs[3]);
					int index = 4;
					for (int i = 1, n = vd.GetAttributeCount(); i < n; i++)
					{
						int semantics = vd.GetSemantics(i);
						int comps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
						for (int ord = 0; ord < comps; ord++)
						{
							env.SetInterval(semantics, ord, attribs[index++], attribs[index++]);
						}
					}
				}
			}
			catch (System.Exception)
			{
				throw new System.IO.InvalidDataException("Cannot read geometry from stream");
			}
			return env;
		}

		/// <exception cref="java.io.ObjectStreamException"/>
		public virtual void SetGeometryByValue(com.epl.geometry.Envelope env)
		{
			try
			{
				attribs = null;
				if (env == null)
				{
					descriptionBitMask = -1;
				}
				com.epl.geometry.VertexDescription vd = env.GetDescription();
				descriptionBitMask = vd.m_semanticsBitArray;
				if (env.IsEmpty())
				{
					return;
				}
				attribs = new double[vd.GetTotalComponentCount() * 2];
				attribs[0] = env.GetXMin();
				attribs[1] = env.GetYMin();
				attribs[2] = env.GetXMax();
				attribs[3] = env.GetYMax();
				int index = 4;
				for (int i = 1, n = vd.GetAttributeCount(); i < n; i++)
				{
					int semantics = vd.GetSemantics(i);
					int comps = com.epl.geometry.VertexDescription.GetComponentCount(semantics);
					for (int ord = 0; ord < comps; ord++)
					{
						com.epl.geometry.Envelope1D e = env.QueryInterval(semantics, ord);
						attribs[index++] = e.vmin;
						attribs[index++] = e.vmax;
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
