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
