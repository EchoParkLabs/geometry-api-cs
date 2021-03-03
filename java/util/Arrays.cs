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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace java.util {
    public class Arrays {


        public static void Fill<T>(T[] array, int start, int end, T value) {
            if (array == null) {
                throw new ArgumentNullException("array");
            }
			if (start == end)
				return;
            if (start < 0 || start > end) {
                throw new ArgumentOutOfRangeException("fromIndex");
            }
            if (end > array.Length) {
                throw new ArgumentOutOfRangeException("toIndex");
            }
            for (int i = start; i < end; i++) {
                array[i] = value;
            }
        }

        public static void Fill<T>(T[] array, T value) {
            for (int i = 0; i < array.Length; i++) {
                array[i] = value;
            }
        }

        public static void Sort<T>(T[] array, int start, int end) {
            Array.Sort(array, start, end - start);
        }


    }
}
