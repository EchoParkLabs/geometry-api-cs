

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestTreap
	{
		/// <exception cref="System.Exception"/>
		protected override void setUp()
		{
			base.setUp();
		}

		/// <exception cref="System.Exception"/>
		protected override void tearDown()
		{
			base.tearDown();
		}

		[NUnit.Framework.Test]
		public static void test1()
		{
			com.esri.core.geometry.Point2D[] pts = new com.esri.core.geometry.Point2D[10];
			for (int i = 0; i < 10; i++)
			{
				com.esri.core.geometry.Point2D pt = new com.esri.core.geometry.Point2D();
				pt.x = i;
				pt.y = 0;
				pts[i] = pt;
			}
			com.esri.core.geometry.TreapComparatorForTesting c = new com.esri.core.geometry.TreapComparatorForTesting
				(pts);
			com.esri.core.geometry.Treap treap = new com.esri.core.geometry.Treap();
			treap.setComparator(c);
			int[] nodes = new int[10];
			for (int i_1 = 0; i_1 < 10; i_1++)
			{
				nodes[i_1] = treap.addElement(i_1, -1);
			}
			for (int i_2 = 1; i_2 < 10; i_2++)
			{
				NUnit.Framework.Assert.IsTrue(treap.getPrev(nodes[i_2]) == nodes[i_2 - 1]);
			}
			for (int i_3 = 0; i_3 < 9; i_3++)
			{
				NUnit.Framework.Assert.IsTrue(treap.getNext(nodes[i_3]) == nodes[i_3 + 1]);
			}
			treap.deleteNode(nodes[0], -1);
			treap.deleteNode(nodes[2], -1);
			treap.deleteNode(nodes[4], -1);
			treap.deleteNode(nodes[6], -1);
			treap.deleteNode(nodes[8], -1);
			NUnit.Framework.Assert.IsTrue(treap.getPrev(nodes[3]) == nodes[1]);
			NUnit.Framework.Assert.IsTrue(treap.getPrev(nodes[5]) == nodes[3]);
			NUnit.Framework.Assert.IsTrue(treap.getPrev(nodes[7]) == nodes[5]);
			NUnit.Framework.Assert.IsTrue(treap.getPrev(nodes[9]) == nodes[7]);
			NUnit.Framework.Assert.IsTrue(treap.getNext(nodes[1]) == nodes[3]);
			NUnit.Framework.Assert.IsTrue(treap.getNext(nodes[3]) == nodes[5]);
			NUnit.Framework.Assert.IsTrue(treap.getNext(nodes[5]) == nodes[7]);
			NUnit.Framework.Assert.IsTrue(treap.getNext(nodes[7]) == nodes[9]);
		}
	}

	internal sealed class TreapComparatorForTesting : com.esri.core.geometry.Treap.Comparator
	{
		internal com.esri.core.geometry.Point2D[] m_pts;

		internal TreapComparatorForTesting(com.esri.core.geometry.Point2D[] pts)
		{
			m_pts = pts;
		}

		internal override int compare(com.esri.core.geometry.Treap treap, int elm, int node
			)
		{
			int elm2 = treap.getElement(node);
			com.esri.core.geometry.Point2D pt1 = m_pts[elm];
			com.esri.core.geometry.Point2D pt2 = m_pts[elm2];
			if (pt1.x < pt2.x)
			{
				return -1;
			}
			return 1;
		}
	}
}
