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
	internal class IndexMultiDCList
	{
		internal com.epl.geometry.StridedIndexTypeCollection m_list_nodes;

		internal com.epl.geometry.StridedIndexTypeCollection m_lists;

		internal int m_list_of_lists;

		internal bool m_b_store_list_index_with_node;

		// stores lists and list elements.
		// Each list element is Index,
		// Prev, next.
		// stores lists. Each list is Head,
		// Tail, PrevList, NextList, NodeCount,
		// ListData.
		internal virtual void FreeNode_(int node)
		{
			m_list_nodes.DeleteElement(node);
		}

		internal virtual int NewNode_()
		{
			int node = m_list_nodes.NewElement();
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

		internal virtual void SetPrev_(int node, int prev)
		{
			m_list_nodes.SetField(node, 1, prev);
		}

		internal virtual void SetNext_(int node, int next)
		{
			m_list_nodes.SetField(node, 2, next);
		}

		internal virtual void SetData_(int node, int data)
		{
			m_list_nodes.SetField(node, 0, data);
		}

		internal virtual void SetList_(int node, int list)
		{
			m_list_nodes.SetField(node, 3, list);
		}

		internal virtual void SetListSize_(int list, int newsize)
		{
			m_lists.SetField(list, 4, newsize);
		}

		internal virtual void SetNextList_(int list, int next)
		{
			m_lists.SetField(list, 3, next);
		}

		internal virtual void SetPrevList_(int list, int prev)
		{
			m_lists.SetField(list, 2, prev);
		}

		internal IndexMultiDCList()
		{
			// Same as Index_multi_dc_list(true).
			m_list_nodes = new com.epl.geometry.StridedIndexTypeCollection(3);
			m_lists = new com.epl.geometry.StridedIndexTypeCollection(6);
			m_b_store_list_index_with_node = false;
			m_list_of_lists = NullNode();
		}

		internal IndexMultiDCList(bool b_store_list_index_with_node)
		{
			// When bStoreListIndexWithNode is true, the each node stores a pointer to
			// the list. Otherwise it does not.
			// The get_list() method cannot be used if bStoreListIndexWithNode is false.
			m_list_nodes = new com.epl.geometry.StridedIndexTypeCollection(3);
			m_lists = new com.epl.geometry.StridedIndexTypeCollection(6);
			m_b_store_list_index_with_node = false;
			m_list_of_lists = NullNode();
		}

		// Creates new list and returns it's handle.
		// listData is user's info associated with the list
		internal virtual int CreateList(int listData)
		{
			int list = NewList_();
			// m_lists.set_field(list, 0, null_node());//head
			// m_lists.set_field(list, 1, null_node());//tail
			// m_lists.set_field(list, 2, null_node());//prev list
			m_lists.SetField(list, 3, m_list_of_lists);
			// next list
			m_lists.SetField(list, 4, 0);
			// node count in the list
			m_lists.SetField(list, 5, listData);
			if (m_list_of_lists != NullNode())
			{
				SetPrevList_(m_list_of_lists, list);
			}
			m_list_of_lists = list;
			return list;
		}

		// Deletes a list and returns the index of the next list.
		internal virtual int DeleteList(int list)
		{
			Clear(list);
			int prevList = m_lists.GetField(list, 2);
			int nextList = m_lists.GetField(list, 3);
			if (prevList != NullNode())
			{
				SetNextList_(prevList, nextList);
			}
			else
			{
				m_list_of_lists = nextList;
			}
			if (nextList != NullNode())
			{
				SetPrevList_(nextList, prevList);
			}
			FreeList_(list);
			return nextList;
		}

		// Reserves memory for the given number of lists.
		internal virtual void ReserveLists(int listCount)
		{
			m_lists.SetCapacity(listCount);
		}

		// returns user's data associated with the list
		internal virtual int GetListData(int list)
		{
			return m_lists.GetField(list, 5);
		}

		// returns the list associated with the node_index. Do not use if list is
		// created with bStoreListIndexWithNode == false.
		internal virtual int GetList(int node_index)
		{
			System.Diagnostics.Debug.Assert((m_b_store_list_index_with_node));
			return m_list_nodes.GetField(node_index, 3);
		}

		// sets the user data to the list
		internal virtual void SetListData(int list, int data)
		{
			m_lists.SetField(list, 5, data);
		}

		// Adds element to a given list. The element is added to the end. Returns
		// the new
		internal virtual int AddElement(int list, int data)
		{
			return InsertElement(list, -1, data);
		}

		// Inserts a new node before the given one .
		internal virtual int InsertElement(int list, int beforeNode, int data)
		{
			int node = NewNode_();
			int prev = -1;
			if (beforeNode != NullNode())
			{
				prev = GetPrev(beforeNode);
				SetPrev_(beforeNode, node);
			}
			SetNext_(node, beforeNode);
			if (prev != NullNode())
			{
				SetNext_(prev, node);
			}
			int head = m_lists.GetField(list, 0);
			if (beforeNode == head)
			{
				m_lists.SetField(list, 0, node);
			}
			if (beforeNode == NullNode())
			{
				int tail = m_lists.GetField(list, 1);
				SetPrev_(node, tail);
				if (tail != -1)
				{
					SetNext_(tail, node);
				}
				m_lists.SetField(list, 1, node);
			}
			SetData(node, data);
			SetListSize_(list, GetListSize(list) + 1);
			if (m_b_store_list_index_with_node)
			{
				SetList_(node, list);
			}
			return node;
		}

		// Deletes a node from a list. Returns the next node after the deleted one.
		internal virtual int DeleteElement(int list, int node)
		{
			int prev = GetPrev(node);
			int next = GetNext(node);
			if (prev != NullNode())
			{
				SetNext_(prev, next);
			}
			else
			{
				m_lists.SetField(list, 0, next);
			}
			// change head
			if (next != NullNode())
			{
				SetPrev_(next, prev);
			}
			else
			{
				m_lists.SetField(list, 1, prev);
			}
			// change tail
			FreeNode_(node);
			SetListSize_(list, GetListSize(list) - 1);
			return next;
		}

		// Reserves memory for the given number of nodes.
		internal virtual void ReserveNodes(int nodeCount)
		{
			m_list_nodes.SetCapacity(nodeCount);
		}

		// Returns the data from the given list node.
		internal virtual int GetData(int node_index)
		{
			return m_list_nodes.GetField(node_index, 0);
		}

		// Sets the data to the given list node.
		internal virtual void SetData(int node_index, int element)
		{
			m_list_nodes.SetField(node_index, 0, element);
		}

		// Returns index of next node for the give node.
		internal virtual int GetNext(int node_index)
		{
			return m_list_nodes.GetField(node_index, 2);
		}

		// Returns index of previous node for the give node.
		internal virtual int GetPrev(int node_index)
		{
			return m_list_nodes.GetField(node_index, 1);
		}

		// Returns the first node in the list
		internal virtual int GetFirst(int list)
		{
			return m_lists.GetField(list, 0);
		}

		// Returns the last node in the list
		internal virtual int GetLast(int list)
		{
			return m_lists.GetField(list, 1);
		}

		// Check if the node is Null (does not exist)
		internal static int NullNode()
		{
			return -1;
		}

		// Clears all nodes and removes all lists.
		internal virtual void Clear()
		{
			for (int list = GetFirstList(); list != -1; )
			{
				list = DeleteList(list);
			}
		}

		// Clears all nodes from the list.
		internal virtual void Clear(int list)
		{
			int last = GetLast(list);
			while (last != NullNode())
			{
				int n = last;
				last = GetPrev(n);
				FreeNode_(n);
			}
			m_lists.SetField(list, 0, -1);
			m_lists.SetField(list, 1, -1);
			SetListSize_(list, 0);
		}

		// Returns True if the given list is empty.
		internal virtual bool IsEmpty(int list)
		{
			return m_lists.GetField(list, 0) == -1;
		}

		// Returns True if the multilist is empty
		internal virtual bool IsEmpty()
		{
			return m_list_nodes.Size() == 0;
		}

		// Returns node count in all lists
		internal virtual int GetNodeCount()
		{
			return m_list_nodes.Size();
		}

		// returns the number of lists
		internal virtual int GetListCount()
		{
			return m_lists.Size();
		}

		// Returns the node count in the given list
		internal virtual int GetListSize(int list)
		{
			return m_lists.GetField(list, 4);
		}

		// returns the first list
		internal virtual int GetFirstList()
		{
			return m_list_of_lists;
		}

		// returns the next list
		internal virtual int GetNextList(int list)
		{
			return m_lists.GetField(list, 3);
		}
	}
}
