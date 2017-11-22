

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestAttributes
	{
		[NUnit.Framework.Test]
		public virtual void testPoint()
		{
			com.esri.core.geometry.Point pt = new com.esri.core.geometry.Point();
			pt.setXY(100, 200);
			NUnit.Framework.Assert.IsFalse(pt.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			pt.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsTrue(pt.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(pt.getM()));
			pt.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 13);
			NUnit.Framework.Assert.IsTrue(pt.getM() == 13);
			pt.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
			NUnit.Framework.Assert.IsTrue(pt.getZ() == 0);
			NUnit.Framework.Assert.IsTrue(pt.getM() == 13);
			pt.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 11);
			NUnit.Framework.Assert.IsTrue(pt.getZ() == 11);
			NUnit.Framework.Assert.IsTrue(pt.getM() == 13);
			pt.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID);
			NUnit.Framework.Assert.IsTrue(pt.getID() == 0);
			NUnit.Framework.Assert.IsTrue(pt.getZ() == 11);
			NUnit.Framework.Assert.IsTrue(pt.getM() == 13);
			pt.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 0, 1);
			NUnit.Framework.Assert.IsTrue(pt.getID() == 1);
			NUnit.Framework.Assert.IsTrue(pt.getZ() == 11);
			NUnit.Framework.Assert.IsTrue(pt.getM() == 13);
			pt.dropAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsTrue(pt.getID() == 1);
			NUnit.Framework.Assert.IsTrue(pt.getZ() == 11);
			NUnit.Framework.Assert.IsFalse(pt.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			com.esri.core.geometry.Point pt1 = new com.esri.core.geometry.Point();
			NUnit.Framework.Assert.IsFalse(pt1.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			NUnit.Framework.Assert.IsFalse(pt1.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsFalse(pt1.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.ID));
			pt1.mergeVertexDescription(pt.getDescription());
			NUnit.Framework.Assert.IsFalse(pt1.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			NUnit.Framework.Assert.IsTrue(pt1.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(pt1.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.ID));
		}

		[NUnit.Framework.Test]
		public virtual void testEnvelope()
		{
			com.esri.core.geometry.Envelope env = new com.esri.core.geometry.Envelope();
			env.setCoords(100, 200, 250, 300);
			NUnit.Framework.Assert.IsFalse(env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			env.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsTrue(env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0).isEmpty());
			env.setInterval(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 1, 2);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0).vmin == 1);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0).vmax == 2);
			NUnit.Framework.Assert.IsFalse(env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			env.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
			NUnit.Framework.Assert.IsTrue(env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0).vmin == 0);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0).vmax == 0);
			env.setInterval(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 3, 4);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0).vmin == 3);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0).vmax == 4);
			NUnit.Framework.Assert.IsFalse(env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.ID));
			env.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID);
			NUnit.Framework.Assert.IsTrue(env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.ID));
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0).vmin == 0);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0).vmax == 0);
			env.setInterval(com.esri.core.geometry.VertexDescription.Semantics.ID, 0, 5, 6);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0).vmin == 5);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0).vmax == 6);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0).vmin == 3);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0).vmax == 4);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0).vmin == 1);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0).vmax == 2);
			env.dropAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsFalse(env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0).vmin == 5);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0).vmax == 6);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0).vmin == 3);
			NUnit.Framework.Assert.IsTrue(env.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0).vmax == 4);
			com.esri.core.geometry.Envelope env1 = new com.esri.core.geometry.Envelope();
			env.copyTo(env1);
			NUnit.Framework.Assert.IsFalse(env1.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			NUnit.Framework.Assert.IsTrue(env1.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0).vmin == 5);
			NUnit.Framework.Assert.IsTrue(env1.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0).vmax == 6);
			NUnit.Framework.Assert.IsTrue(env1.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0).vmin == 3);
			NUnit.Framework.Assert.IsTrue(env1.queryInterval(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0).vmax == 4);
		}

		[NUnit.Framework.Test]
		public virtual void testLine()
		{
			com.esri.core.geometry.Line env = new com.esri.core.geometry.Line();
			env.setStartXY(100, 200);
			env.setEndXY(250, 300);
			NUnit.Framework.Assert.IsFalse(env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			env.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsTrue(env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			env.setStartAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 1);
			env.setEndAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 2);
			NUnit.Framework.Assert.IsTrue(env.getStartAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0) == 1);
			NUnit.Framework.Assert.IsTrue(env.getEndAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0) == 2);
			NUnit.Framework.Assert.IsFalse(env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			env.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
			NUnit.Framework.Assert.IsTrue(env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			env.setStartAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 3);
			env.setEndAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 4);
			NUnit.Framework.Assert.IsTrue(env.getStartAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0) == 3);
			NUnit.Framework.Assert.IsTrue(env.getEndAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0) == 4);
			NUnit.Framework.Assert.IsFalse(env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.ID));
			env.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID);
			NUnit.Framework.Assert.IsTrue(env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.ID));
			env.setStartAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 0, 5
				);
			env.setEndAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 0, 6);
			NUnit.Framework.Assert.IsTrue(env.getStartAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0) == 5);
			NUnit.Framework.Assert.IsTrue(env.getEndAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0) == 6);
			NUnit.Framework.Assert.IsTrue(env.getStartAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0) == 1);
			NUnit.Framework.Assert.IsTrue(env.getEndAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0) == 2);
			NUnit.Framework.Assert.IsTrue(env.getStartAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0) == 3);
			NUnit.Framework.Assert.IsTrue(env.getEndAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0) == 4);
			env.dropAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsFalse(env.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			NUnit.Framework.Assert.IsTrue(env.getStartAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0) == 5);
			NUnit.Framework.Assert.IsTrue(env.getEndAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0) == 6);
			NUnit.Framework.Assert.IsTrue(env.getStartAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0) == 3);
			NUnit.Framework.Assert.IsTrue(env.getEndAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0) == 4);
			com.esri.core.geometry.Line env1 = new com.esri.core.geometry.Line();
			env.copyTo(env1);
			NUnit.Framework.Assert.IsFalse(env1.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			NUnit.Framework.Assert.IsTrue(env.getStartAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0) == 5);
			NUnit.Framework.Assert.IsTrue(env.getEndAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0) == 6);
			NUnit.Framework.Assert.IsTrue(env.getStartAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0) == 3);
			NUnit.Framework.Assert.IsTrue(env.getEndAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0) == 4);
		}

		[NUnit.Framework.Test]
		public virtual void testMultiPoint()
		{
			com.esri.core.geometry.MultiPoint mp = new com.esri.core.geometry.MultiPoint();
			mp.add(new com.esri.core.geometry.Point(100, 200));
			mp.add(new com.esri.core.geometry.Point(101, 201));
			mp.add(new com.esri.core.geometry.Point(102, 202));
			NUnit.Framework.Assert.IsFalse(mp.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			mp.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsTrue(mp.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0)));
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 0, 1);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, 2);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 2, 0, 3);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0) == 1);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0) == 2);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0) == 3);
			NUnit.Framework.Assert.IsFalse(mp.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			mp.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
			NUnit.Framework.Assert.IsTrue(mp.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 2, 0) == 0);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 11);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 1, 0, 21);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 2, 0, 31);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0) == 1);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0) == 2);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0) == 3);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsFalse(mp.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.ID));
			mp.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID);
			NUnit.Framework.Assert.IsTrue(mp.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.ID));
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 2, 0) == 0);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 0, 0, -11);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 1, 0, -21);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 2, 0, -31);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0) == 1);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0) == 2);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0) == 3);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0, 0) == -11);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 1, 0) == -21);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 2, 0) == -31);
			mp.dropAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsFalse(mp.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0, 0) == -11);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 1, 0) == -21);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 2, 0) == -31);
			com.esri.core.geometry.MultiPoint mp1 = new com.esri.core.geometry.MultiPoint();
			mp.copyTo(mp1);
			NUnit.Framework.Assert.IsFalse(mp1.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0, 0) == -11);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 1, 0) == -21);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 2, 0) == -31);
			mp1.dropAllAttributes();
			mp1.mergeVertexDescription(mp.getDescription());
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 2, 0) == 0);
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0)));
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 2, 0) == 0);
		}

		[NUnit.Framework.Test]
		public virtual void testPolygon()
		{
			com.esri.core.geometry.Polygon mp = new com.esri.core.geometry.Polygon();
			mp.startPath(new com.esri.core.geometry.Point(100, 200));
			mp.lineTo(new com.esri.core.geometry.Point(101, 201));
			mp.lineTo(new com.esri.core.geometry.Point(102, 202));
			NUnit.Framework.Assert.IsFalse(mp.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			mp.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsTrue(mp.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0)));
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 0, 0, 1);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 1, 0, 2);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.M, 2, 0, 3);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0) == 1);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0) == 2);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0) == 3);
			NUnit.Framework.Assert.IsFalse(mp.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			mp.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z);
			NUnit.Framework.Assert.IsTrue(mp.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.Z));
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 2, 0) == 0);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 0, 0, 11);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 1, 0, 21);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.Z, 2, 0, 31);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0) == 1);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0) == 2);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0) == 3);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsFalse(mp.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.ID));
			mp.addAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID);
			NUnit.Framework.Assert.IsTrue(mp.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.ID));
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 2, 0) == 0);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 0, 0, -11);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 1, 0, -21);
			mp.setAttribute(com.esri.core.geometry.VertexDescription.Semantics.ID, 2, 0, -31);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0) == 1);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0) == 2);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0) == 3);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0, 0) == -11);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 1, 0) == -21);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 2, 0) == -31);
			mp.dropAttribute(com.esri.core.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsFalse(mp.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0, 0) == -11);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 1, 0) == -21);
			NUnit.Framework.Assert.IsTrue(mp.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 2, 0) == -31);
			com.esri.core.geometry.Polygon mp1 = new com.esri.core.geometry.Polygon();
			mp.copyTo(mp1);
			NUnit.Framework.Assert.IsFalse(mp1.hasAttribute(com.esri.core.geometry.VertexDescription.Semantics
				.M));
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0, 0) == -11);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 1, 0) == -21);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 2, 0) == -31);
			mp1.dropAllAttributes();
			mp1.mergeVertexDescription(mp.getDescription());
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.Z, 2, 0) == 0);
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 0, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 1, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.M, 2, 0)));
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.getAttributeAsDbl(com.esri.core.geometry.VertexDescription.Semantics
				.ID, 2, 0) == 0);
		}
	}
}
