

namespace com.esri.core.geometry
{
	[NUnit.Framework.TestFixture]
	[NUnit.Framework.TestFixture]
	public class TestSerialization
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
		public virtual void testSerializePoint()
		{
			try
			{
				java.io.ByteArrayOutputStream streamOut = new java.io.ByteArrayOutputStream();
				java.io.ObjectOutputStream oo = new java.io.ObjectOutputStream(streamOut);
				com.esri.core.geometry.Point pt = new com.esri.core.geometry.Point(10, 40);
				oo.writeObject(pt);
				java.io.ByteArrayInputStream streamIn = new java.io.ByteArrayInputStream(streamOut
					.toByteArray());
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(streamIn);
				com.esri.core.geometry.Point ptRes = (com.esri.core.geometry.Point)ii.readObject(
					);
				NUnit.Framework.Assert.IsTrue(ptRes.Equals(pt));
			}
			catch (System.Exception)
			{
				fail("Point serialization failure");
			}
			// try
			// {
			// FileOutputStream streamOut = new FileOutputStream(m_thisDirectory +
			// "savedPoint.txt");
			// ObjectOutputStream oo = new ObjectOutputStream(streamOut);
			// Point pt = new Point(10, 40);
			// oo.writeObject(pt);
			// }
			// catch(Exception ex)
			// {
			// fail("Point serialization failure");
			// }
			try
			{
				java.io.InputStream s = Sharpen.Runtime.getClassForType(typeof(com.esri.core.geometry.TestSerialization
					)).getResourceAsStream("savedPoint.txt");
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(s);
				com.esri.core.geometry.Point ptRes = (com.esri.core.geometry.Point)ii.readObject(
					);
				NUnit.Framework.Assert.IsTrue(ptRes.getX() == 10 && ptRes.getY() == 40);
			}
			catch (System.Exception)
			{
				fail("Point serialization failure");
			}
		}

		[NUnit.Framework.Test]
		public virtual void testSerializePolygon()
		{
			try
			{
				java.io.ByteArrayOutputStream streamOut = new java.io.ByteArrayOutputStream();
				java.io.ObjectOutputStream oo = new java.io.ObjectOutputStream(streamOut);
				com.esri.core.geometry.Polygon pt = new com.esri.core.geometry.Polygon();
				pt.startPath(10, 10);
				pt.lineTo(100, 100);
				pt.lineTo(200, 100);
				oo.writeObject(pt);
				java.io.ByteArrayInputStream streamIn = new java.io.ByteArrayInputStream(streamOut
					.toByteArray());
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(streamIn);
				com.esri.core.geometry.Polygon ptRes = (com.esri.core.geometry.Polygon)ii.readObject
					();
				NUnit.Framework.Assert.IsTrue(ptRes.Equals(pt));
			}
			catch (System.Exception)
			{
				fail("Polygon serialization failure");
			}
			try
			{
				java.io.ByteArrayOutputStream streamOut = new java.io.ByteArrayOutputStream();
				java.io.ObjectOutputStream oo = new java.io.ObjectOutputStream(streamOut);
				com.esri.core.geometry.Polygon pt = new com.esri.core.geometry.Polygon();
				pt.startPath(10, 10);
				pt.lineTo(100, 100);
				pt.lineTo(200, 100);
				pt = (com.esri.core.geometry.Polygon)com.esri.core.geometry.GeometryEngine.simplify
					(pt, null);
				oo.writeObject(pt);
				java.io.ByteArrayInputStream streamIn = new java.io.ByteArrayInputStream(streamOut
					.toByteArray());
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(streamIn);
				com.esri.core.geometry.Polygon ptRes = (com.esri.core.geometry.Polygon)ii.readObject
					();
				NUnit.Framework.Assert.IsTrue(ptRes.Equals(pt));
			}
			catch (System.Exception)
			{
				fail("Polygon serialization failure");
			}
			// try
			// {
			// FileOutputStream streamOut = new FileOutputStream(m_thisDirectory +
			// "savedPolygon.txt");
			// ObjectOutputStream oo = new ObjectOutputStream(streamOut);
			// Polygon pt = new Polygon();
			// pt.startPath(10, 10);
			// pt.lineTo(100, 100);
			// pt.lineTo(200, 100);
			// pt = (Polygon)GeometryEngine.simplify(pt, null);
			// oo.writeObject(pt);
			// }
			// catch(Exception ex)
			// {
			// fail("Polygon serialization failure");
			// }
			try
			{
				java.io.InputStream s = Sharpen.Runtime.getClassForType(typeof(com.esri.core.geometry.TestSerialization
					)).getResourceAsStream("savedPolygon.txt");
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(s);
				com.esri.core.geometry.Polygon ptRes = (com.esri.core.geometry.Polygon)ii.readObject
					();
				NUnit.Framework.Assert.IsTrue(ptRes != null);
			}
			catch (System.Exception)
			{
				fail("Polygon serialization failure");
			}
		}

		[NUnit.Framework.Test]
		public virtual void testSerializePolyline()
		{
			try
			{
				java.io.ByteArrayOutputStream streamOut = new java.io.ByteArrayOutputStream();
				java.io.ObjectOutputStream oo = new java.io.ObjectOutputStream(streamOut);
				com.esri.core.geometry.Polyline pt = new com.esri.core.geometry.Polyline();
				pt.startPath(10, 10);
				pt.lineTo(100, 100);
				pt.lineTo(200, 100);
				oo.writeObject(pt);
				java.io.ByteArrayInputStream streamIn = new java.io.ByteArrayInputStream(streamOut
					.toByteArray());
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(streamIn);
				com.esri.core.geometry.Polyline ptRes = (com.esri.core.geometry.Polyline)ii.readObject
					();
				NUnit.Framework.Assert.IsTrue(ptRes.Equals(pt));
			}
			catch (System.Exception)
			{
				fail("Polyline serialization failure");
			}
			// try
			// {
			// FileOutputStream streamOut = new FileOutputStream(m_thisDirectory +
			// "savedPolyline.txt");
			// ObjectOutputStream oo = new ObjectOutputStream(streamOut);
			// Polyline pt = new Polyline();
			// pt.startPath(10, 10);
			// pt.lineTo(100, 100);
			// pt.lineTo(200, 100);
			// oo.writeObject(pt);
			// }
			// catch(Exception ex)
			// {
			// fail("Polyline serialization failure");
			// }
			try
			{
				java.io.InputStream s = Sharpen.Runtime.getClassForType(typeof(com.esri.core.geometry.TestSerialization
					)).getResourceAsStream("savedPolyline.txt");
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(s);
				com.esri.core.geometry.Polyline ptRes = (com.esri.core.geometry.Polyline)ii.readObject
					();
				NUnit.Framework.Assert.IsTrue(ptRes != null);
			}
			catch (System.Exception)
			{
				fail("Polyline serialization failure");
			}
		}

		[NUnit.Framework.Test]
		public virtual void testSerializeEnvelope()
		{
			try
			{
				java.io.ByteArrayOutputStream streamOut = new java.io.ByteArrayOutputStream();
				java.io.ObjectOutputStream oo = new java.io.ObjectOutputStream(streamOut);
				com.esri.core.geometry.Envelope pt = new com.esri.core.geometry.Envelope(10, 10, 
					400, 300);
				oo.writeObject(pt);
				java.io.ByteArrayInputStream streamIn = new java.io.ByteArrayInputStream(streamOut
					.toByteArray());
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(streamIn);
				com.esri.core.geometry.Envelope ptRes = (com.esri.core.geometry.Envelope)ii.readObject
					();
				NUnit.Framework.Assert.IsTrue(ptRes.Equals(pt));
			}
			catch (System.Exception)
			{
				fail("Envelope serialization failure");
			}
			// try
			// {
			// FileOutputStream streamOut = new FileOutputStream(m_thisDirectory +
			// "savedEnvelope.txt");
			// ObjectOutputStream oo = new ObjectOutputStream(streamOut);
			// Envelope pt = new Envelope(10, 10, 400, 300);
			// oo.writeObject(pt);
			// }
			// catch(Exception ex)
			// {
			// fail("Envelope serialization failure");
			// }
			try
			{
				java.io.InputStream s = Sharpen.Runtime.getClassForType(typeof(com.esri.core.geometry.TestSerialization
					)).getResourceAsStream("savedEnvelope.txt");
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(s);
				com.esri.core.geometry.Envelope ptRes = (com.esri.core.geometry.Envelope)ii.readObject
					();
				NUnit.Framework.Assert.IsTrue(ptRes.getXMax() == 400);
			}
			catch (System.Exception)
			{
				fail("Envelope serialization failure");
			}
		}

		[NUnit.Framework.Test]
		public virtual void testSerializeMultiPoint()
		{
			try
			{
				java.io.ByteArrayOutputStream streamOut = new java.io.ByteArrayOutputStream();
				java.io.ObjectOutputStream oo = new java.io.ObjectOutputStream(streamOut);
				com.esri.core.geometry.MultiPoint pt = new com.esri.core.geometry.MultiPoint();
				pt.add(10, 30);
				pt.add(120, 40);
				oo.writeObject(pt);
				java.io.ByteArrayInputStream streamIn = new java.io.ByteArrayInputStream(streamOut
					.toByteArray());
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(streamIn);
				com.esri.core.geometry.MultiPoint ptRes = (com.esri.core.geometry.MultiPoint)ii.readObject
					();
				NUnit.Framework.Assert.IsTrue(ptRes.Equals(pt));
			}
			catch (System.Exception)
			{
				fail("MultiPoint serialization failure");
			}
			// try
			// {
			// FileOutputStream streamOut = new FileOutputStream(m_thisDirectory +
			// "savedMultiPoint.txt");
			// ObjectOutputStream oo = new ObjectOutputStream(streamOut);
			// MultiPoint pt = new MultiPoint();
			// pt.add(10, 30);
			// pt.add(120, 40);
			// oo.writeObject(pt);
			// }
			// catch(Exception ex)
			// {
			// fail("MultiPoint serialization failure");
			// }
			try
			{
				java.io.InputStream s = Sharpen.Runtime.getClassForType(typeof(com.esri.core.geometry.TestSerialization
					)).getResourceAsStream("savedMultiPoint.txt");
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(s);
				com.esri.core.geometry.MultiPoint ptRes = (com.esri.core.geometry.MultiPoint)ii.readObject
					();
				NUnit.Framework.Assert.IsTrue(ptRes.getPoint(1).getY() == 40);
			}
			catch (System.Exception)
			{
				fail("MultiPoint serialization failure");
			}
		}

		[NUnit.Framework.Test]
		public virtual void testSerializeLine()
		{
			try
			{
				java.io.ByteArrayOutputStream streamOut = new java.io.ByteArrayOutputStream();
				java.io.ObjectOutputStream oo = new java.io.ObjectOutputStream(streamOut);
				com.esri.core.geometry.Line pt = new com.esri.core.geometry.Line();
				pt.setStart(new com.esri.core.geometry.Point(10, 30));
				pt.setEnd(new com.esri.core.geometry.Point(120, 40));
				oo.writeObject(pt);
				java.io.ByteArrayInputStream streamIn = new java.io.ByteArrayInputStream(streamOut
					.toByteArray());
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(streamIn);
				com.esri.core.geometry.Line ptRes = (com.esri.core.geometry.Line)ii.readObject();
				NUnit.Framework.Assert.IsTrue(ptRes.equals(pt));
			}
			catch (System.Exception ex)
			{
				// fail("Line serialization failure");
				NUnit.Framework.Assert.AreEqual(ex.Message, "Cannot serialize this geometry");
			}
		}

		[NUnit.Framework.Test]
		public virtual void testSerializeSR()
		{
			try
			{
				java.io.ByteArrayOutputStream streamOut = new java.io.ByteArrayOutputStream();
				java.io.ObjectOutputStream oo = new java.io.ObjectOutputStream(streamOut);
				com.esri.core.geometry.SpatialReference sr = com.esri.core.geometry.SpatialReference
					.create(102100);
				oo.writeObject(sr);
				java.io.ByteArrayInputStream streamIn = new java.io.ByteArrayInputStream(streamOut
					.toByteArray());
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(streamIn);
				com.esri.core.geometry.SpatialReference ptRes = (com.esri.core.geometry.SpatialReference
					)ii.readObject();
				NUnit.Framework.Assert.IsTrue(ptRes.Equals(sr));
			}
			catch (System.Exception)
			{
				fail("Spatial Reference serialization failure");
			}
		}

		[NUnit.Framework.Test]
		public virtual void testSerializeEnvelope2D()
		{
			try
			{
				java.io.ByteArrayOutputStream streamOut = new java.io.ByteArrayOutputStream();
				java.io.ObjectOutputStream oo = new java.io.ObjectOutputStream(streamOut);
				com.esri.core.geometry.Envelope2D env = new com.esri.core.geometry.Envelope2D(1.213948734
					, 2.213948734, 11.213948734, 12.213948734);
				oo.writeObject(env);
				java.io.ByteArrayInputStream streamIn = new java.io.ByteArrayInputStream(streamOut
					.toByteArray());
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(streamIn);
				com.esri.core.geometry.Envelope2D envRes = (com.esri.core.geometry.Envelope2D)ii.
					readObject();
				NUnit.Framework.Assert.IsTrue(envRes.Equals(env));
			}
			catch (System.Exception)
			{
				fail("Envelope2D serialization failure");
			}
			//		try
			//		{
			//			 FileOutputStream streamOut = new FileOutputStream(
			//			 "c:/temp/savedEnvelope2D.txt");
			//			 ObjectOutputStream oo = new ObjectOutputStream(streamOut);
			//			 Envelope2D e = new Envelope2D(177.123, 188.234, 999.122, 888.999);
			//			 oo.writeObject(e);
			//		 }
			//		 catch(Exception ex)
			//		 {
			//		   fail("Envelope2D serialization failure");
			//		 }
			try
			{
				java.io.InputStream s = Sharpen.Runtime.getClassForType(typeof(com.esri.core.geometry.TestSerialization
					)).getResourceAsStream("savedEnvelope2D.txt");
				java.io.ObjectInputStream ii = new java.io.ObjectInputStream(s);
				com.esri.core.geometry.Envelope2D e = (com.esri.core.geometry.Envelope2D)ii.readObject
					();
				NUnit.Framework.Assert.IsTrue(e != null);
				NUnit.Framework.Assert.IsTrue(e.Equals(new com.esri.core.geometry.Envelope2D(177.123
					, 188.234, 999.122, 888.999)));
			}
			catch (System.Exception)
			{
				fail("Envelope2D serialization failure");
			}
		}
	}
}
