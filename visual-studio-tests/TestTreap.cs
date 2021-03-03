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
using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestTreap : NUnit.Framework.TestFixtureAttribute
	{
		/// <exception cref="System.Exception"/>
		[SetUp]
        protected void SetUp()
		{
			
		}

		/// <exception cref="System.Exception"/>
		protected void TearDown()
		{
			
		}

		[NUnit.Framework.Test]
		public static void Test1()
		{
			com.epl.geometry.Point2D[] pts = new com.epl.geometry.Point2D[10];
			for (int i = 0; i < 10; i++)
			{
				com.epl.geometry.Point2D pt = new com.epl.geometry.Point2D();
				pt.x = i;
				pt.y = 0;
				pts[i] = pt;
			}
			com.epl.geometry.TreapComparatorForTesting c = new com.epl.geometry.TreapComparatorForTesting(pts);
			com.epl.geometry.Treap treap = new com.epl.geometry.Treap();
			treap.SetComparator(c);
			int[] nodes = new int[10];
			for (int i_1 = 0; i_1 < 10; i_1++)
			{
				nodes[i_1] = treap.AddElement(i_1, -1);
			}
			for (int i_2 = 1; i_2 < 10; i_2++)
			{
				NUnit.Framework.Assert.IsTrue(treap.GetPrev(nodes[i_2]) == nodes[i_2 - 1]);
			}
			for (int i_3 = 0; i_3 < 9; i_3++)
			{
				NUnit.Framework.Assert.IsTrue(treap.GetNext(nodes[i_3]) == nodes[i_3 + 1]);
			}
			treap.DeleteNode(nodes[0], -1);
			treap.DeleteNode(nodes[2], -1);
			treap.DeleteNode(nodes[4], -1);
			treap.DeleteNode(nodes[6], -1);
			treap.DeleteNode(nodes[8], -1);
			NUnit.Framework.Assert.IsTrue(treap.GetPrev(nodes[3]) == nodes[1]);
			NUnit.Framework.Assert.IsTrue(treap.GetPrev(nodes[5]) == nodes[3]);
			NUnit.Framework.Assert.IsTrue(treap.GetPrev(nodes[7]) == nodes[5]);
			NUnit.Framework.Assert.IsTrue(treap.GetPrev(nodes[9]) == nodes[7]);
			NUnit.Framework.Assert.IsTrue(treap.GetNext(nodes[1]) == nodes[3]);
			NUnit.Framework.Assert.IsTrue(treap.GetNext(nodes[3]) == nodes[5]);
			NUnit.Framework.Assert.IsTrue(treap.GetNext(nodes[5]) == nodes[7]);
			NUnit.Framework.Assert.IsTrue(treap.GetNext(nodes[7]) == nodes[9]);
		}
	}

	internal sealed class TreapComparatorForTesting : com.epl.geometry.Treap.Comparator
	{
		internal com.epl.geometry.Point2D[] m_pts;

		internal TreapComparatorForTesting(com.epl.geometry.Point2D[] pts)
		{
			m_pts = pts;
		}

		internal override int Compare(com.epl.geometry.Treap treap, int elm, int node)
		{
			int elm2 = treap.GetElement(node);
			com.epl.geometry.Point2D pt1 = m_pts[elm];
			com.epl.geometry.Point2D pt2 = m_pts[elm2];
			if (pt1.x < pt2.x)
			{
				return -1;
			}
			return 1;
		}
	}
}
