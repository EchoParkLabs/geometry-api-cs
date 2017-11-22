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
	/// <summary>A callback to provide progress and cancel tracking mechanism for lengthy operation.</summary>
	public abstract class ProgressTracker
	{
		/// <summary>Periodically called by a lengthy operation to check if the caller requested to cancel.</summary>
		/// <param name="step">The current step of the operation.</param>
		/// <param name="totalExpectedSteps">is the number of steps the operation is expects to complete its task.</param>
		/// <returns>true, if the operation can continue. Returns False, when the operation has to terminate due to a user cancelation.</returns>
		public abstract bool Progress(int step, int totalExpectedSteps);

		/// <summary>Checks the tracker and throws UserCancelException if tracker is not null and progress returns false</summary>
		/// <param name="tracker">can be null, then the method does nothing.</param>
		public static void CheckAndThrow(com.epl.geometry.ProgressTracker tracker)
		{
			if (tracker != null && !tracker.Progress(-1, -1))
			{
				throw new com.epl.geometry.UserCancelException();
			}
		}
	}
}
