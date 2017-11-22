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
	internal class ObjectCacheTable<K, T>
	{
		private System.Collections.Generic.IDictionary<K, T> m_hashTable = System.Collections.Concurrent.ConcurrentDictionary(new System.Collections.Generic.Dictionary<K, T>());

		private object[] m_lru;

		private bool[] m_places;

		private int m_index;

		public ObjectCacheTable(int maxSize)
		{
			m_lru = new object[maxSize];
			m_places = new bool[maxSize];
			m_index = 0;
			for (int i = 0; i < maxSize; i++)
			{
				m_places[i] = false;
			}
		}

		internal virtual bool Contains(K key)
		{
			return m_hashTable.ContainsKey(key);
		}

		internal virtual T Get(K key)
		{
			return m_hashTable[key];
		}

		internal virtual void Add(K key, T value)
		{
			if (m_places[m_index])
			{
				// remove existing element from the cache
				m_places[m_index] = false;
				m_hashTable.Remove(m_lru[m_index]);
			}
			m_hashTable[key] = value;
			m_lru[m_index] = key;
			m_places[m_index] = true;
			m_index = (m_index + 1) % m_lru.Length;
		}
	}
}
