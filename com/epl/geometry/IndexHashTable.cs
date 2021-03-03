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
	internal sealed class IndexHashTable
	{
		public abstract class HashFunction
		{
			// The hash function abstract class that user need to define to use the
			// IndexHashTable.
			public abstract int GetHash(int element);

			public abstract bool Equal(int element1, int element2);

			public abstract int GetHash(object elementDescriptor);

			public abstract bool Equal(object elementDescriptor, int element);
		}

		internal int m_random;

		internal com.epl.geometry.AttributeStreamOfInt32 m_hashBuckets;

		internal int[] m_bit_filter;

		internal com.epl.geometry.IndexMultiList m_lists;

		internal com.epl.geometry.IndexHashTable.HashFunction m_hash;

		public IndexHashTable(int size, com.epl.geometry.IndexHashTable.HashFunction hashFunction)
		{
			//this is aimed to speedup the find 
			//operation and allows to have less buckets.
			// Create hash table. size is the bin count in the table. The hashFunction
			// is the function to use.
			m_hashBuckets = new com.epl.geometry.AttributeStreamOfInt32(size, NullNode());
			m_lists = new com.epl.geometry.IndexMultiList();
			m_hash = hashFunction;
			m_bit_filter = new int[(size * 10 + 31) >> 5];
		}

		//10 times more bits than buckets
		public void ReserveElements(int capacity)
		{
			m_lists.ReserveLists(System.Math.Min(m_hashBuckets.Size(), capacity));
			m_lists.ReserveNodes(capacity);
		}

		// Adds new element to the hash table.
		public int AddElement(int element, int hash)
		{
			int bit_bucket = hash % (m_bit_filter.Length << 5);
			m_bit_filter[(bit_bucket >> 5)] |= (1 << (bit_bucket & unchecked((int)(0x1F))));
			int bucket = hash % m_hashBuckets.Size();
			int list = m_hashBuckets.Get(bucket);
			if (list == -1)
			{
				list = m_lists.CreateList();
				m_hashBuckets.Set(bucket, list);
			}
			int node = m_lists.AddElement(list, element);
			return node;
		}

		public int AddElement(int element)
		{
			int hash = m_hash.GetHash(element);
			int bit_bucket = hash % (m_bit_filter.Length << 5);
			m_bit_filter[(bit_bucket >> 5)] |= (1 << (bit_bucket & unchecked((int)(0x1F))));
			int bucket = hash % m_hashBuckets.Size();
			int list = m_hashBuckets.Get(bucket);
			if (list == -1)
			{
				list = m_lists.CreateList();
				m_hashBuckets.Set(bucket, list);
			}
			int node = m_lists.AddElement(list, element);
			return node;
		}

		public void DeleteElement(int element, int hash)
		{
			int bucket = hash % m_hashBuckets.Size();
			int list = m_hashBuckets.Get(bucket);
			if (list == -1)
			{
				throw new System.ArgumentException();
			}
			int ptr = m_lists.GetFirst(list);
			int prev = -1;
			while (ptr != -1)
			{
				int e = m_lists.GetElement(ptr);
				int nextptr = m_lists.GetNext(ptr);
				if (e == element)
				{
					m_lists.DeleteElement(list, prev, ptr);
					if (m_lists.GetFirst(list) == -1)
					{
						m_lists.DeleteList(list);
						// do not keep empty lists
						m_hashBuckets.Set(bucket, -1);
					}
				}
				else
				{
					prev = ptr;
				}
				ptr = nextptr;
			}
		}

		// Removes element from the hash table.
		public void DeleteElement(int element)
		{
			int hash = m_hash.GetHash(element);
			int bucket = hash % m_hashBuckets.Size();
			int list = m_hashBuckets.Get(bucket);
			if (list == -1)
			{
				throw new System.ArgumentException();
			}
			int ptr = m_lists.GetFirst(list);
			int prev = -1;
			while (ptr != -1)
			{
				int e = m_lists.GetElement(ptr);
				int nextptr = m_lists.GetNext(ptr);
				if (e == element)
				{
					m_lists.DeleteElement(list, prev, ptr);
					if (m_lists.GetFirst(list) == -1)
					{
						m_lists.DeleteList(list);
						// do not keep empty lists
						m_hashBuckets.Set(bucket, -1);
					}
				}
				else
				{
					prev = ptr;
				}
				ptr = nextptr;
			}
		}

		// Returns the first node in the hash table bucket defined by the given
		// hashValue.
		public int GetFirstInBucket(int hashValue)
		{
			int bit_bucket = hashValue % (m_bit_filter.Length << 5);
			if ((m_bit_filter[(bit_bucket >> 5)] & (1 << (bit_bucket & unchecked((int)(0x1F))))) == 0)
			{
				return -1;
			}
			int bucket = hashValue % m_hashBuckets.Size();
			int list = m_hashBuckets.Get(bucket);
			if (list == -1)
			{
				return -1;
			}
			return m_lists.GetFirst(list);
		}

		// Returns next node in a bucket. Can be used together with GetFirstInBucket
		// only.
		public int GetNextInBucket(int elementHandle)
		{
			return m_lists.GetNext(elementHandle);
		}

		// Returns a node of the first element in the hash table, that is equal to
		// the given one.
		public int FindNode(int element)
		{
			int hash = m_hash.GetHash(element);
			int ptr = GetFirstInBucket(hash);
			while (ptr != -1)
			{
				int e = m_lists.GetElement(ptr);
				if (m_hash.Equal(e, element))
				{
					return ptr;
				}
				ptr = m_lists.GetNext(ptr);
			}
			return -1;
		}

		// Returns a node to the first element in the hash table, that is equal to
		// the given element descriptor.
		public int FindNode(object elementDescriptor)
		{
			int hash = m_hash.GetHash(elementDescriptor);
			int ptr = GetFirstInBucket(hash);
			while (ptr != -1)
			{
				int e = m_lists.GetElement(ptr);
				if (m_hash.Equal(elementDescriptor, e))
				{
					return ptr;
				}
				ptr = m_lists.GetNext(ptr);
			}
			return -1;
		}

		// Gets next equal node.
		public int GetNextNode(int elementHandle)
		{
			int element = m_lists.GetElement(elementHandle);
			int ptr = m_lists.GetNext(elementHandle);
			while (ptr != -1)
			{
				int e = m_lists.GetElement(ptr);
				if (m_hash.Equal(e, element))
				{
					return ptr;
				}
				ptr = m_lists.GetNext(ptr);
			}
			return -1;
		}

		// Removes a node.
		public void DeleteNode(int node)
		{
			int element = GetElement(node);
			int hash = m_hash.GetHash(element);
			int bucket = hash % m_hashBuckets.Size();
			int list = m_hashBuckets.Get(bucket);
			if (list == -1)
			{
				throw new System.ArgumentException();
			}
			int ptr = m_lists.GetFirst(list);
			int prev = -1;
			while (ptr != -1)
			{
				if (ptr == node)
				{
					m_lists.DeleteElement(list, prev, ptr);
					if (m_lists.GetFirst(list) == -1)
					{
						m_lists.DeleteList(list);
						// do not keep empty lists
						m_hashBuckets.Set(bucket, -1);
					}
					return;
				}
				prev = ptr;
				ptr = m_lists.GetNext(ptr);
			}
			throw new System.ArgumentException();
		}

		// Returns a value of the element stored in the given node.
		public int GetElement(int elementHandle)
		{
			return m_lists.GetElement(elementHandle);
		}

		// Returns any existing element from the hash table. Throws if the table is
		// empty.
		public int GetAnyElement()
		{
			return m_lists.GetFirstElement(m_lists.GetFirstList());
		}

		// Returns a node for any existing element from the hash table or NullNode
		// if the table is empty.
		public int GetAnyNode()
		{
			return m_lists.GetFirst(m_lists.GetFirstList());
		}

		public static int NullNode()
		{
			return -1;
		}

		// Removes all elements from the hash table.
		public void Clear()
		{
			java.util.Arrays.Fill(m_bit_filter, 0);
			m_hashBuckets = new com.epl.geometry.AttributeStreamOfInt32(m_hashBuckets.Size(), NullNode());
			m_lists.Clear();
		}

		// Returns the number of elements in the hash table
		public int Size()
		{
			return m_lists.GetNodeCount();
		}
	}
}
