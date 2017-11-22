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
/*
* To change this template, choose Tools | Templates
* and open the template in the editor.
*/


namespace com.epl.geometry
{
	internal abstract class ClassicSort
	{
		public abstract void UserSort(int begin, int end, com.epl.geometry.AttributeStreamOfInt32 indices);

		public abstract double GetValue(int index);
	}
}
