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
using NUnit.Framework;

namespace com.epl.geometry
{
	public class TestUnion : NUnit.Framework.TestFixtureAttribute
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
		public static void TestUnion__()
		{
			com.epl.geometry.Point pt = new com.epl.geometry.Point(10, 20);
			com.epl.geometry.Point pt2 = new com.epl.geometry.Point();
			pt2.SetXY(10, 10);
			com.epl.geometry.Envelope env1 = new com.epl.geometry.Envelope(10, 10, 30, 50);
			com.epl.geometry.Envelope env2 = new com.epl.geometry.Envelope(30, 10, 60, 50);
			com.epl.geometry.Geometry[] geomArray = new com.epl.geometry.Geometry[] { env1, env2 };
			com.epl.geometry.SimpleGeometryCursor inputGeometries = new com.epl.geometry.SimpleGeometryCursor(geomArray);
			com.epl.geometry.OperatorUnion union = (com.epl.geometry.OperatorUnion)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Union);
			com.epl.geometry.SpatialReference sr = com.epl.geometry.SpatialReference.Create(4326);
			com.epl.geometry.GeometryCursor outputCursor = union.Execute(inputGeometries, sr, null);
			com.epl.geometry.Geometry result = outputCursor.Next();
		}
	}
}
