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
	public class TestAttributes : NUnit.Framework.TestFixtureAttribute
	{
		[NUnit.Framework.Test]
		public virtual void TestPoint()
		{
			com.epl.geometry.Point pt = new com.epl.geometry.Point();
			pt.SetXY(100, 200);
			NUnit.Framework.Assert.IsFalse(pt.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			pt.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsTrue(pt.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(pt.GetM()));
			pt.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 13);
			NUnit.Framework.Assert.IsTrue(pt.GetM() == 13);
			pt.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			NUnit.Framework.Assert.IsTrue(pt.GetZ() == 0);
			NUnit.Framework.Assert.IsTrue(pt.GetM() == 13);
			pt.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 0, 11);
			NUnit.Framework.Assert.IsTrue(pt.GetZ() == 11);
			NUnit.Framework.Assert.IsTrue(pt.GetM() == 13);
			pt.AddAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
			NUnit.Framework.Assert.IsTrue(pt.GetID() == 0);
			NUnit.Framework.Assert.IsTrue(pt.GetZ() == 11);
			NUnit.Framework.Assert.IsTrue(pt.GetM() == 13);
			pt.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 0, 1);
			NUnit.Framework.Assert.IsTrue(pt.GetID() == 1);
			NUnit.Framework.Assert.IsTrue(pt.GetZ() == 11);
			NUnit.Framework.Assert.IsTrue(pt.GetM() == 13);
			pt.DropAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsTrue(pt.GetID() == 1);
			NUnit.Framework.Assert.IsTrue(pt.GetZ() == 11);
			NUnit.Framework.Assert.IsFalse(pt.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			com.epl.geometry.Point pt1 = new com.epl.geometry.Point();
			NUnit.Framework.Assert.IsFalse(pt1.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			NUnit.Framework.Assert.IsFalse(pt1.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			NUnit.Framework.Assert.IsFalse(pt1.HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID));
			pt1.MergeVertexDescription(pt.GetDescription());
			NUnit.Framework.Assert.IsFalse(pt1.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			NUnit.Framework.Assert.IsTrue(pt1.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			NUnit.Framework.Assert.IsTrue(pt1.HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID));
		}

		[NUnit.Framework.Test]
		public virtual void TestEnvelope()
		{
			com.epl.geometry.Envelope env = new com.epl.geometry.Envelope();
			env.SetCoords(100, 200, 250, 300);
			NUnit.Framework.Assert.IsFalse(env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			env.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsTrue(env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0).IsEmpty());
			env.SetInterval(com.epl.geometry.VertexDescription.Semantics.M, 0, 1, 2);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0).vmin == 1);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0).vmax == 2);
			NUnit.Framework.Assert.IsFalse(env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			env.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			NUnit.Framework.Assert.IsTrue(env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0).vmin == 0);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0).vmax == 0);
			env.SetInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0, 3, 4);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0).vmin == 3);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0).vmax == 4);
			NUnit.Framework.Assert.IsFalse(env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID));
			env.AddAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
			NUnit.Framework.Assert.IsTrue(env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID));
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0).vmin == 0);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0).vmax == 0);
			env.SetInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0, 5, 6);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0).vmin == 5);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0).vmax == 6);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0).vmin == 3);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0).vmax == 4);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0).vmin == 1);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.M, 0).vmax == 2);
			env.DropAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsFalse(env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0).vmin == 5);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0).vmax == 6);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0).vmin == 3);
			NUnit.Framework.Assert.IsTrue(env.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0).vmax == 4);
			com.epl.geometry.Envelope env1 = new com.epl.geometry.Envelope();
			env.CopyTo(env1);
			NUnit.Framework.Assert.IsFalse(env1.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			NUnit.Framework.Assert.IsTrue(env1.QueryInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0).vmin == 5);
			NUnit.Framework.Assert.IsTrue(env1.QueryInterval(com.epl.geometry.VertexDescription.Semantics.ID, 0).vmax == 6);
			NUnit.Framework.Assert.IsTrue(env1.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0).vmin == 3);
			NUnit.Framework.Assert.IsTrue(env1.QueryInterval(com.epl.geometry.VertexDescription.Semantics.Z, 0).vmax == 4);
		}

		[NUnit.Framework.Test]
		public virtual void TestLine()
		{
			com.epl.geometry.Line env = new com.epl.geometry.Line();
			env.SetStartXY(100, 200);
			env.SetEndXY(250, 300);
			NUnit.Framework.Assert.IsFalse(env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			env.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsTrue(env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			env.SetStartAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 1);
			env.SetEndAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 2);
			NUnit.Framework.Assert.IsTrue(env.GetStartAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0) == 1);
			NUnit.Framework.Assert.IsTrue(env.GetEndAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0) == 2);
			NUnit.Framework.Assert.IsFalse(env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			env.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			NUnit.Framework.Assert.IsTrue(env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			env.SetStartAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 0, 3);
			env.SetEndAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 0, 4);
			NUnit.Framework.Assert.IsTrue(env.GetStartAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0) == 3);
			NUnit.Framework.Assert.IsTrue(env.GetEndAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0) == 4);
			NUnit.Framework.Assert.IsFalse(env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID));
			env.AddAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
			NUnit.Framework.Assert.IsTrue(env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID));
			env.SetStartAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 0, 5);
			env.SetEndAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 0, 6);
			NUnit.Framework.Assert.IsTrue(env.GetStartAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0) == 5);
			NUnit.Framework.Assert.IsTrue(env.GetEndAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0) == 6);
			NUnit.Framework.Assert.IsTrue(env.GetStartAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0) == 1);
			NUnit.Framework.Assert.IsTrue(env.GetEndAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0) == 2);
			NUnit.Framework.Assert.IsTrue(env.GetStartAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0) == 3);
			NUnit.Framework.Assert.IsTrue(env.GetEndAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0) == 4);
			env.DropAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsFalse(env.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			NUnit.Framework.Assert.IsTrue(env.GetStartAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0) == 5);
			NUnit.Framework.Assert.IsTrue(env.GetEndAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0) == 6);
			NUnit.Framework.Assert.IsTrue(env.GetStartAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0) == 3);
			NUnit.Framework.Assert.IsTrue(env.GetEndAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0) == 4);
			com.epl.geometry.Line env1 = new com.epl.geometry.Line();
			env.CopyTo(env1);
			NUnit.Framework.Assert.IsFalse(env1.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			NUnit.Framework.Assert.IsTrue(env.GetStartAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0) == 5);
			NUnit.Framework.Assert.IsTrue(env.GetEndAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0) == 6);
			NUnit.Framework.Assert.IsTrue(env.GetStartAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0) == 3);
			NUnit.Framework.Assert.IsTrue(env.GetEndAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0) == 4);
		}

		[NUnit.Framework.Test]
		public virtual void TestMultiPoint()
		{
			com.epl.geometry.MultiPoint mp = new com.epl.geometry.MultiPoint();
			mp.Add(new com.epl.geometry.Point(100, 200));
			mp.Add(new com.epl.geometry.Point(101, 201));
			mp.Add(new com.epl.geometry.Point(102, 202));
			NUnit.Framework.Assert.IsFalse(mp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			mp.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsTrue(mp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0)));
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 0, 1);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, 2);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 2, 0, 3);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0) == 1);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == 2);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0) == 3);
			NUnit.Framework.Assert.IsFalse(mp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			mp.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			NUnit.Framework.Assert.IsTrue(mp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0) == 0);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0, 11);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0, 21);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0, 31);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0) == 1);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == 2);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0) == 3);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsFalse(mp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID));
			mp.AddAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
			NUnit.Framework.Assert.IsTrue(mp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID));
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 2, 0) == 0);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0, -11);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 1, 0, -21);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 2, 0, -31);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0) == 1);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == 2);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0) == 3);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0) == -11);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 1, 0) == -21);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 2, 0) == -31);
			mp.DropAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsFalse(mp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0) == -11);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 1, 0) == -21);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 2, 0) == -31);
			com.epl.geometry.MultiPoint mp1 = new com.epl.geometry.MultiPoint();
			mp.CopyTo(mp1);
			NUnit.Framework.Assert.IsFalse(mp1.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0) == -11);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 1, 0) == -21);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 2, 0) == -31);
			mp1.DropAllAttributes();
			mp1.MergeVertexDescription(mp.GetDescription());
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0) == 0);
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0)));
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 2, 0) == 0);
		}

		[NUnit.Framework.Test]
		public virtual void TestPolygon()
		{
			com.epl.geometry.Polygon mp = new com.epl.geometry.Polygon();
			mp.StartPath(new com.epl.geometry.Point(100, 200));
			mp.LineTo(new com.epl.geometry.Point(101, 201));
			mp.LineTo(new com.epl.geometry.Point(102, 202));
			NUnit.Framework.Assert.IsFalse(mp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			mp.AddAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsTrue(mp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0)));
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 0, 0, 1);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 1, 0, 2);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.M, 2, 0, 3);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0) == 1);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == 2);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0) == 3);
			NUnit.Framework.Assert.IsFalse(mp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			mp.AddAttribute(com.epl.geometry.VertexDescription.Semantics.Z);
			NUnit.Framework.Assert.IsTrue(mp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.Z));
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0) == 0);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0, 11);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0, 21);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0, 31);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0) == 1);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == 2);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0) == 3);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsFalse(mp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID));
			mp.AddAttribute(com.epl.geometry.VertexDescription.Semantics.ID);
			NUnit.Framework.Assert.IsTrue(mp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.ID));
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 2, 0) == 0);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0, -11);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 1, 0, -21);
			mp.SetAttribute(com.epl.geometry.VertexDescription.Semantics.ID, 2, 0, -31);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0) == 1);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0) == 2);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0) == 3);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0) == -11);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 1, 0) == -21);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 2, 0) == -31);
			mp.DropAttribute(com.epl.geometry.VertexDescription.Semantics.M);
			NUnit.Framework.Assert.IsFalse(mp.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0) == -11);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 1, 0) == -21);
			NUnit.Framework.Assert.IsTrue(mp.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 2, 0) == -31);
			com.epl.geometry.Polygon mp1 = new com.epl.geometry.Polygon();
			mp.CopyTo(mp1);
			NUnit.Framework.Assert.IsFalse(mp1.HasAttribute(com.epl.geometry.VertexDescription.Semantics.M));
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0) == 11);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0) == 21);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0) == 31);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0) == -11);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 1, 0) == -21);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 2, 0) == -31);
			mp1.DropAllAttributes();
			mp1.MergeVertexDescription(mp.GetDescription());
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.Z, 2, 0) == 0);
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 0, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 1, 0)));
			NUnit.Framework.Assert.IsTrue(double.IsNaN(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.M, 2, 0)));
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 0, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 1, 0) == 0);
			NUnit.Framework.Assert.IsTrue(mp1.GetAttributeAsDbl(com.epl.geometry.VertexDescription.Semantics.ID, 2, 0) == 0);
		}
	}
}
