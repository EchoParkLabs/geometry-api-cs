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
	internal class IndexMultiList
	{
		internal com.epl.geometry.StridedIndexTypeCollection m_listNodes;

		internal com.epl.geometry.StridedIndexTypeCollection m_lists;

		internal int m_list_of_lists;

		internal bool m_b_allow_navigation_between_lists;

		// stores lists and list elements.
		// Each list element is Index, next.
		// stores lists. Each list is Head,
		// Tail, [PrevList, NextList].
		// when False, get_first_list,
		// get_next_list return -1.
		internal virtual void FreeNode_(int node)
		{
			m_listNodes.DeleteElement(node);
		}

		internal virtual int NewNode_()
		{
			int node = m_listNodes.NewElement();
			return node;
		}

		internal virtual void FreeList_(int list)
		{
			m_lists.DeleteElement(list);
		}

		internal virtual int NewList_()
		{
			int list = m_lists.NewElement();
			return list;
		}

		internal IndexMultiList()
		{
			// Same as Index_multi_list(true);
			m_listNodes = new com.epl.geometry.StridedIndexTypeCollection(2);
			m_lists = new com.epl.geometry.StridedIndexTypeCollection(4);
			m_list_of_lists = NullNode();
			m_b_allow_navigation_between_lists = true;
		}

		internal IndexMultiList(bool b_allow_navigation_between_lists)
		{
			// When b_allow_navigation_between_lists is False, the get_first_list and
			// get_next_list do not work.
			// There will be two Index_type elements per list and two Index_type
			// elements per list element
			// When b_allow_navigation_between_lists is True, the get_first_list and
			// get_next_list will work.
			// There will be four Index_type elements per list and two Index_type
			// elements per list element
			m_listNodes = new com.epl.geometry.StridedIndexTypeCollection(2);
			m_lists = new com.epl.geometry.StridedIndexTypeCollection(b_allow_navigation_between_lists ? 4 : 2);
			m_list_of_lists = NullNode();
			m_b_allow_navigation_between_lists = b_allow_navigation_between_lists;
		}

		// Creates new list and returns it's handle.
		internal virtual int CreateList()
		{
			int node = NewList_();
			if (m_b_allow_navigation_between_lists)
			{
				m_lists.SetField(node, 3, m_list_of_lists);
				if (m_list_of_lists != NullNode())
				{
					m_lists.SetField(m_list_of_lists, 2, node);
				}
				m_list_of_lists = node;
			}
			return node;
		}

		// Deletes a list.
		internal virtual void DeleteList(int list)
		{
			int ptr = GetFirst(list);
			while (ptr != NullNode())
			{
				int p = ptr;
				ptr = GetNext(ptr);
				FreeNode_(p);
			}
			if (m_b_allow_navigation_between_lists)
			{
				int prevList = m_lists.GetField(list, 2);
				int nextList = m_lists.GetField(list, 3);
				if (prevList != NullNode())
				{
					m_lists.SetField(prevList, 3, nextList);
				}
				else
				{
					m_list_of_lists = nextList;
				}
				if (nextList != NullNode())
				{
					m_lists.SetField(nextList, 2, prevList);
				}
			}
			FreeList_(list);
		}

		// Reserves memory for the given number of lists.
		internal virtual void ReserveLists(int listCount)
		{
			m_lists.SetCapacity(listCount);
		}

		// Adds element to a given list. The element is added to the end. Returns
		// the new
		internal virtual int AddElement(int list, int element)
		{
			int head = m_lists.GetField(list, 0);
			int tail = m_lists.GetField(list, 1);
			int node = NewNode_();
			if (tail != NullNode())
			{
				System.Diagnostics.Debug.Assert((head != NullNode()));
				m_listNodes.SetField(tail, 1, node);
				m_lists.SetField(list, 1, node);
			}
			else
			{
				// empty list
				System.Diagnostics.Debug.Assert((head == NullNode()));
				m_lists.SetField(list, 0, node);
				m_lists.SetField(list, 1, node);
			}
			m_listNodes.SetField(node, 0, element);
			return node;
		}

		// Reserves memory for the given number of nodes.
		internal virtual void ReserveNodes(int nodeCount)
		{
			m_listNodes.SetCapacity(nodeCount);
		}

		// Deletes a node from a list, given the previous node (previous node is
		// required, because the list is singly connected).
		internal virtual void DeleteElement(int list, int prevNode, int node)
		{
			if (prevNode != NullNode())
			{
				System.Diagnostics.Debug.Assert((m_listNodes.GetField(prevNode, 1) == node));
				m_listNodes.SetField(prevNode, 1, m_listNodes.GetField(node, 1));
				if (m_lists.GetField(list, 1) == node)
				{
					// deleting a tail
					m_lists.SetField(list, 1, prevNode);
				}
			}
			else
			{
				System.Diagnostics.Debug.Assert((m_lists.GetField(list, 0) == node));
				m_lists.SetField(list, 0, m_listNodes.GetField(node, 1));
				if (m_lists.GetField(list, 1) == node)
				{
					// removing last element
					System.Diagnostics.Debug.Assert((m_listNodes.GetField(node, 1) == NullNode()));
					m_lists.SetField(list, 1, NullNode());
				}
			}
			FreeNode_(node);
		}

		// Concatenates list1 and list2. The nodes of list2 are added to the end of
		// list1. The list2 index becomes invalid.
		// Returns list1.
		internal virtual int ConcatenateLists(int list1, int list2)
		{
			int tailNode1 = m_lists.GetField(list1, 1);
			int headNode2 = m_lists.GetField(list2, 0);
			if (headNode2 != NullNode())
			{
				// do not concatenate empty lists
				if (tailNode1 != NullNode())
				{
					// connect head of list2 to the tail of list1.
					m_listNodes.SetField(tailNode1, 1, headNode2);
					// set the tail of the list1 to be the tail of list2.
					m_lists.SetField(list1, 1, m_lists.GetField(list2, 1));
				}
				else
				{
					// list1 is empty, while list2 is not.
					m_lists.SetField(list1, 0, headNode2);
					m_lists.SetField(list1, 1, m_lists.GetField(list2, 1));
				}
			}
			if (m_b_allow_navigation_between_lists)
			{
				int prevList = m_lists.GetField(list2, 2);
				int nextList = m_lists.GetField(list2, 3);
				if (prevList != NullNode())
				{
					m_lists.SetField(prevList, 3, nextList);
				}
				else
				{
					m_list_of_lists = nextList;
				}
				if (nextList != NullNode())
				{
					m_lists.SetField(nextList, 2, prevList);
				}
			}
			FreeList_(list2);
			return list1;
		}

		// Returns the data from the given list node.
		internal virtual int GetElement(int node_index)
		{
			return m_listNodes.GetField(node_index, 0);
		}

		// Sets the data to the given list node.
		internal virtual void SetElement(int node_index, int element)
		{
			m_listNodes.SetField(node_index, 0, element);
		}

		// Returns index of next node for the give node.
		internal virtual int GetNext(int node_index)
		{
			return m_listNodes.GetField(node_index, 1);
		}

		// Returns the first node in the least
		internal virtual int GetFirst(int list)
		{
			return m_lists.GetField(list, 0);
		}

		// Returns the element from the first node in the least. Equivalent to
		// get_element(get_first(list));
		internal virtual int GetFirstElement(int list)
		{
			int f = GetFirst(list);
			return GetElement(f);
		}

		// Check if the node is Null (does not exist)
		internal static int NullNode()
		{
			return -1;
		}

		// Clears all nodes and removes all lists. Frees the memory.
		internal virtual void Clear()
		{
			m_listNodes.DeleteAll(true);
			m_lists.DeleteAll(true);
			m_list_of_lists = NullNode();
		}

		// Returns True if the given list is empty.
		internal virtual bool IsEmpty(int list)
		{
			return m_lists.GetField(list, 0) == NullNode();
		}

		internal virtual bool IsEmpty()
		{
			return m_listNodes.Size() == 0;
		}

		internal virtual int GetNodeCount()
		{
			return m_listNodes.Size();
		}

		internal virtual int GetListCount()
		{
			return m_lists.Size();
		}

		internal virtual int GetFirstList()
		{
			System.Diagnostics.Debug.Assert((m_b_allow_navigation_between_lists));
			return m_list_of_lists;
		}

		internal virtual int GetNextList(int list)
		{
			System.Diagnostics.Debug.Assert((m_b_allow_navigation_between_lists));
			return m_lists.GetField(list, 3);
		}
	}
}
