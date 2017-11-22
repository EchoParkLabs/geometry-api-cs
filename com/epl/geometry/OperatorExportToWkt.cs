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
	public abstract class OperatorExportToWkt : com.epl.geometry.Operator
	{
		public override com.epl.geometry.Operator.Type GetType()
		{
			return com.epl.geometry.Operator.Type.ExportToWkt;
		}

		public abstract string Execute(int exportFlags, com.epl.geometry.Geometry geometry, com.epl.geometry.ProgressTracker progress_tracker);

		public static com.epl.geometry.OperatorExportToWkt Local()
		{
			return (com.epl.geometry.OperatorExportToWkt)com.epl.geometry.OperatorFactoryLocal.GetInstance().GetOperator(com.epl.geometry.Operator.Type.ExportToWkt);
		}
	}
}
