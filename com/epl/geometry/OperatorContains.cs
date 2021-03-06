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


namespace com.epl.geometry
{
	/// <summary>Relational operation Contains.</summary>
	public abstract class OperatorContains : com.epl.geometry.OperatorSimpleRelation
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.Contains;
		}

		public static com.epl.geometry.OperatorContains Local()
		{
			return (com.epl.geometry.OperatorContains)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.Contains);
		}
	}
}
