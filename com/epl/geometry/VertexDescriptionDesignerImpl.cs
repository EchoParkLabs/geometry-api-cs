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
	/// <summary>
	/// This factory class allows to describe and create a VertexDescription
	/// instance.
	/// </summary>
	internal class VertexDescriptionDesignerImpl : com.epl.geometry.VertexDescription
	{
		/// <summary>
		/// Designer default constructor produces XY vertex description (POSITION
		/// semantics only).
		/// </summary>
		public VertexDescriptionDesignerImpl()
			: base()
		{
			m_semantics = new int[com.epl.geometry.VertexDescription.Semantics.MAXSEMANTICS];
			m_semantics[0] = com.epl.geometry.VertexDescription.Semantics.POSITION;
			m_attributeCount = 1;
			m_semanticsToIndexMap = new int[com.epl.geometry.VertexDescription.Semantics.MAXSEMANTICS];
			for (int i = 0; i < com.epl.geometry.VertexDescription.Semantics.MAXSEMANTICS; i++)
			{
				m_semanticsToIndexMap[i] = -1;
			}
			m_semanticsToIndexMap[m_semantics[0]] = 0;
			m_bModified = true;
		}

		/// <summary>
		/// Creates description designer and initializes it from the given
		/// description.
		/// </summary>
		/// <remarks>
		/// Creates description designer and initializes it from the given
		/// description. Use this to add or remove attributes from the description.
		/// </remarks>
		public VertexDescriptionDesignerImpl(com.epl.geometry.VertexDescription other)
			: base(other.GetHashCode(), other)
		{
			m_bModified = true;
		}

		/// <summary>Adds a new attribute to the VertexDescription.</summary>
		/// <param name="semantics">Attribute semantics.</param>
		public virtual void AddAttribute(int semantics)
		{
			if (HasAttribute(semantics))
			{
				return;
			}
			m_semanticsToIndexMap[semantics] = 0;
			// assign a value >= 0 to mark it
			// as existing
			_initMapping();
		}

		/// <summary>Removes given attribute.</summary>
		/// <param name="semantics">Attribute semantics.</param>
		internal virtual void RemoveAttribute(int semantics)
		{
			if (semantics == com.epl.geometry.VertexDescription.Semantics.POSITION)
			{
				throw new System.ArgumentException("Position attribue cannot be removed");
			}
			// not allowed to
			// remove the xy
			if (!HasAttribute(semantics))
			{
				return;
			}
			m_semanticsToIndexMap[semantics] = -1;
			// assign a value < 0 to mark it
			// as removed
			_initMapping();
		}

		/// <summary>
		/// Removes all attributes from the designer with exception of the POSITION
		/// attribute.
		/// </summary>
		public virtual void Reset()
		{
			m_semantics[0] = com.epl.geometry.VertexDescription.Semantics.POSITION;
			m_attributeCount = 1;
			foreach (int i in m_semanticsToIndexMap)
			{
				m_semanticsToIndexMap[i] = -1;
			}
			m_semanticsToIndexMap[m_semantics[0]] = 0;
			m_bModified = true;
		}

		/// <summary>Returns a VertexDescription corresponding to the vertex design.</summary>
		/// <remarks>
		/// Returns a VertexDescription corresponding to the vertex design. <br />
		/// Note: the same instance of VertexDescription will be returned each time
		/// for the same same set of attributes and attribute properties. <br />
		/// The method searches for the VertexDescription in a global hash table. If
		/// found, it is returned. Else, a new instance of the VertexDescription is
		/// added to the has table and returned.
		/// </remarks>
		public virtual com.epl.geometry.VertexDescription GetDescription()
		{
			com.epl.geometry.VertexDescriptionHash vdhash = com.epl.geometry.VertexDescriptionHash.GetInstance();
			com.epl.geometry.VertexDescriptionDesignerImpl vdd = this;
			return vdhash.Add(vdd);
		}

		/// <summary>Returns a default VertexDescription that has X and Y coordinates only.</summary>
		internal static com.epl.geometry.VertexDescription GetDefaultDescriptor2D()
		{
			com.epl.geometry.VertexDescriptionHash vdhash = com.epl.geometry.VertexDescriptionHash.GetInstance();
			com.epl.geometry.VertexDescription vd = vdhash.GetVD2D();
			return vd;
		}

		/// <summary>Returns a default VertexDescription that has X, Y, and Z coordinates only</summary>
		internal static com.epl.geometry.VertexDescription GetDefaultDescriptor3D()
		{
			com.epl.geometry.VertexDescriptionHash vdhash = com.epl.geometry.VertexDescriptionHash.GetInstance();
			com.epl.geometry.VertexDescription vd = vdhash.GetVD3D();
			return vd;
		}

		internal virtual com.epl.geometry.VertexDescription _createInternal()
		{
			int hash = GetHashCode();
			com.epl.geometry.VertexDescription vd = new com.epl.geometry.VertexDescription(hash, this);
			return vd;
		}

		protected internal bool m_bModified;

		protected internal virtual void _initMapping()
		{
			m_attributeCount = 0;
			for (int i = 0, j = 0; i < com.epl.geometry.VertexDescription.Semantics.MAXSEMANTICS; i++)
			{
				if (m_semanticsToIndexMap[i] >= 0)
				{
					m_semantics[j] = i;
					m_semanticsToIndexMap[i] = j;
					j++;
					m_attributeCount++;
				}
			}
			m_bModified = true;
		}

		public override int GetHashCode()
		{
			if (m_bModified)
			{
				m_hash = CalculateHashImpl();
				m_bModified = false;
			}
			return m_hash;
		}

		public override bool Equals(object _other)
		{
			if (_other == null)
			{
				return false;
			}
			if (_other == this)
			{
				return true;
			}
			if (_other.GetType() != GetType())
			{
				return false;
			}
			com.epl.geometry.VertexDescriptionDesignerImpl other = (com.epl.geometry.VertexDescriptionDesignerImpl)(_other);
			if (other.GetAttributeCount() != GetAttributeCount())
			{
				return false;
			}
			for (int i = 0; i < m_attributeCount; i++)
			{
				if (m_semantics[i] != other.m_semantics[i])
				{
					return false;
				}
			}
			if (m_bModified != other.m_bModified)
			{
				return false;
			}
			return true;
		}

		public virtual bool IsDesignerFor(com.epl.geometry.VertexDescription vd)
		{
			if (vd.GetAttributeCount() != GetAttributeCount())
			{
				return false;
			}
			for (int i = 0; i < m_attributeCount; i++)
			{
				if (m_semantics[i] != vd.m_semantics[i])
				{
					return false;
				}
			}
			return true;
		}

		// returns a mapping from the source attribute indices to the destination
		// attribute indices.
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

		internal static com.epl.geometry.VertexDescription GetMergedVertexDescription(com.epl.geometry.VertexDescription src, int semanticsToAdd)
		{
			com.epl.geometry.VertexDescriptionDesignerImpl vdd = new com.epl.geometry.VertexDescriptionDesignerImpl(src);
			vdd.AddAttribute(semanticsToAdd);
			return vdd.GetDescription();
		}

		internal static com.epl.geometry.VertexDescription GetMergedVertexDescription(com.epl.geometry.VertexDescription d1, com.epl.geometry.VertexDescription d2)
		{
			com.epl.geometry.VertexDescriptionDesignerImpl vdd = null;
			for (int semantics = com.epl.geometry.VertexDescription.Semantics.POSITION; semantics < com.epl.geometry.VertexDescription.Semantics.MAXSEMANTICS; semantics++)
			{
				if (!d1.HasAttribute(semantics) && d2.HasAttribute(semantics))
				{
					if (vdd == null)
					{
						vdd = new com.epl.geometry.VertexDescriptionDesignerImpl(d1);
					}
					vdd.AddAttribute(semantics);
				}
			}
			if (vdd != null)
			{
				return vdd.GetDescription();
			}
			return d1;
		}

		internal static com.epl.geometry.VertexDescription RemoveSemanticsFromVertexDescription(com.epl.geometry.VertexDescription src, int semanticsToRemove)
		{
			com.epl.geometry.VertexDescriptionDesignerImpl vdd = new com.epl.geometry.VertexDescriptionDesignerImpl(src);
			vdd.RemoveAttribute(semanticsToRemove);
			return vdd.GetDescription();
		}
	}
}
