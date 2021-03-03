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
	internal sealed class Treap
	{
		internal abstract class Comparator
		{
			internal Comparator()
			{
				m_b_notify_on_actions = false;
			}

			internal Comparator(bool bNotifyOnActions)
			{
				m_b_notify_on_actions = bNotifyOnActions;
			}

			// Compares the element elm to the element contained in the given node
			internal abstract int Compare(com.epl.geometry.Treap treap, int elm, int node);

			// These virtual methods are called only when Comparator(true) ctro has
			// been used.
			internal virtual void OnDelete(int elm)
			{
			}

			internal virtual void OnSet(int elm)
			{
			}

			internal virtual void OnEndSearch(int elm)
			{
			}

			internal virtual void OnAddUniqueElementFailed(int elm)
			{
			}

			private bool m_b_notify_on_actions;

			// void operator=(const Comparator&); // do not allow operator =
			internal virtual void OnDeleteImpl_(com.epl.geometry.Treap treap, int node)
			{
				if (m_b_notify_on_actions)
				{
					OnDelete(treap.GetElement(node));
				}
			}

			internal virtual void OnSetImpl_(com.epl.geometry.Treap treap, int node)
			{
				if (m_b_notify_on_actions)
				{
					OnSet(treap.GetElement(node));
				}
			}

			internal virtual void OnAddUniqueElementFailedImpl_(int elm)
			{
				if (m_b_notify_on_actions)
				{
					OnAddUniqueElementFailed(elm);
				}
			}

			internal virtual void OnEndSearchImpl_(int elm)
			{
				if (m_b_notify_on_actions)
				{
					OnEndSearch(elm);
				}
			}
		}

		internal abstract class MonikerComparator
		{
			// Compares the moniker, contained in the MonikerComparator with the
			// element contained in the given node.
			internal abstract int Compare(com.epl.geometry.Treap treap, int node);
		}

		public Treap()
		{
			m_random = 124234251;
			m_b_balancing = true;
			m_touchFlag = 0;
			m_defaultTreap = NullNode();
			m_treapData = new com.epl.geometry.StridedIndexTypeCollection(7);
			m_comparator = null;
		}

		// Sets the comparator
		public void SetComparator(com.epl.geometry.Treap.Comparator comparator)
		{
			m_comparator = comparator;
		}

		// Returns the comparator
		public com.epl.geometry.Treap.Comparator GetComparator()
		{
			return m_comparator;
		}

		// Stops auto-balancing
		public void DisableBalancing()
		{
			m_b_balancing = false;
		}

		// Reserves memory for nodes givne number of nodes
		public void SetCapacity(int capacity)
		{
			m_treapData.SetCapacity(capacity);
		}

		// Create a new treap and returns the treap handle.
		public int CreateTreap(int treap_data)
		{
			int treap = m_treapData.NewElement();
			SetSize_(0, treap);
			SetTreapData_(treap_data, treap);
			return treap;
		}

		// Deletes the treap at the given treap handle.
		public void DeleteTreap(int treap)
		{
			m_treapData.DeleteElement(treap);
		}

		// Adds new element to the treap. Allows duplicates to be added.
		public int AddElement(int element, int treap)
		{
			int treap_;
			if (treap == -1)
			{
				if (m_defaultTreap == NullNode())
				{
					m_defaultTreap = CreateTreap(-1);
				}
				treap_ = m_defaultTreap;
			}
			else
			{
				treap_ = treap;
			}
			return AddElement_(element, 0, treap_);
		}

		// Adds new element to the treap if it is not equal to other elements.
		// If the return value is -1, then get_duplicate_element reutrns the node of
		// the already existing element equal to element.
		public int AddUniqueElement(int element, int treap)
		{
			int treap_;
			if (treap == -1)
			{
				if (m_defaultTreap == NullNode())
				{
					m_defaultTreap = CreateTreap(-1);
				}
				treap_ = m_defaultTreap;
			}
			else
			{
				treap_ = treap;
			}
			return AddElement_(element, 1, treap_);
		}

		// Adds a new element to the treap that is known to be bigger or equal of
		// all elements already in the treap.
		// Use this method when adding elements from a sorted list for maximum
		// performance (it does not call the treap comparator).
		public int AddBiggestElement(int element, int treap)
		{
			int treap_;
			if (treap == -1)
			{
				if (m_defaultTreap == NullNode())
				{
					m_defaultTreap = CreateTreap(-1);
				}
				treap_ = m_defaultTreap;
			}
			else
			{
				treap_ = treap;
			}
			if (GetRoot_(treap_) == NullNode())
			{
				int newNode = NewNode_(element);
				SetRoot_(newNode, treap_);
				AddToList_(-1, newNode, treap_);
				return newNode;
			}
			int cur = GetLast_(treap_);
			int newNode_1 = NewNode_(element);
			SetRight_(cur, newNode_1);
			SetParent_(newNode_1, cur);
			System.Diagnostics.Debug.Assert((m_b_balancing));
			// don't use this method for unbalanced tree, or
			// the performance will be bad.
			BubbleUp_(newNode_1);
			if (GetParent(newNode_1) == NullNode())
			{
				SetRoot_(newNode_1, treap_);
			}
			AddToList_(-1, newNode_1, treap_);
			return newNode_1;
		}

		// template <class Iterator> void build_from_sorted(const Iterator& begin,
		// const Iterator& end);
		// Adds new element to the treap at the known position, thus avoiding a call
		// to the comparator.
		// If bCallCompare is True, the comparator will be called at most twice,
		// once to compare with prevElement and once to compare with nextElement.
		// When bUnique is true, if the return value is -1, then
		// get_duplicate_element reutrns the node of the already existing element.
		public int AddElementAtPosition(int prevNode, int nextNode, int element, bool bUnique, bool bCallCompare, int treap)
		{
			int treap_ = treap;
			if (treap_ == -1)
			{
				if (m_defaultTreap == NullNode())
				{
					m_defaultTreap = CreateTreap(-1);
				}
				treap_ = m_defaultTreap;
			}
			// dbg_check_(m_root);
			if (GetRoot_(treap_) == NullNode())
			{
				System.Diagnostics.Debug.Assert((nextNode == NullNode() && prevNode == NullNode()));
				int root = NewNode_(element);
				SetRoot_(root, treap_);
				AddToList_(-1, root, treap_);
				return root;
			}
			int cmpNext;
			int cmpPrev;
			if (bCallCompare)
			{
				cmpNext = nextNode != NullNode() ? m_comparator.Compare(this, element, nextNode) : -1;
				System.Diagnostics.Debug.Assert((cmpNext <= 0));
				cmpPrev = prevNode != NullNode() ? m_comparator.Compare(this, element, prevNode) : 1;
			}
			else
			{
				// cmpPrev can be negative in plane sweep when intersection is
				// detected.
				cmpNext = -1;
				cmpPrev = 1;
			}
			if (bUnique && (cmpNext == 0 || cmpPrev == 0))
			{
				m_comparator.OnAddUniqueElementFailedImpl_(element);
				int cur = cmpNext == 0 ? nextNode : prevNode;
				SetDuplicateElement_(cur, treap_);
				return -1;
			}
			// return negative value.
			int cur_1;
			int cmp;
			bool bNext;
			if (nextNode != NullNode() && prevNode != NullNode())
			{
				// randomize the the cost to insert a node.
				bNext = m_random > com.epl.geometry.NumberUtils.NextRand(m_random) >> 1;
			}
			else
			{
				bNext = nextNode != NullNode();
			}
			if (bNext)
			{
				cmp = cmpNext;
				cur_1 = nextNode;
			}
			else
			{
				cmp = cmpPrev;
				cur_1 = prevNode;
			}
			int newNode = -1;
			int before = -1;
			bool b_first = true;
			for (; ; )
			{
				if (cmp < 0)
				{
					int left = GetLeft(cur_1);
					if (left != NullNode())
					{
						cur_1 = left;
					}
					else
					{
						before = cur_1;
						newNode = NewNode_(element);
						SetLeft_(cur_1, newNode);
						SetParent_(newNode, cur_1);
						break;
					}
				}
				else
				{
					int right = GetRight(cur_1);
					if (right != NullNode())
					{
						cur_1 = right;
					}
					else
					{
						before = GetNext(cur_1);
						newNode = NewNode_(element);
						SetRight_(cur_1, newNode);
						SetParent_(newNode, cur_1);
						break;
					}
				}
				if (b_first)
				{
					cmp *= -1;
					b_first = false;
				}
			}
			BubbleUp_(newNode);
			if (GetParent(newNode) == NullNode())
			{
				SetRoot_(newNode, treap_);
			}
			AddToList_(before, newNode, treap_);
			// dbg_check_(m_root);
			return newNode;
		}

		// Get duplicate element
		public int GetDuplicateElement(int treap)
		{
			if (treap == -1)
			{
				return GetDuplicateElement_(m_defaultTreap);
			}
			return GetDuplicateElement_(treap);
		}

		// Removes a node from the treap. Throws if doesn't exist.
		public void DeleteNode(int treap_node_index, int treap)
		{
			Touch_();
			// assert(isValidNode(treap_node_index));
			if (m_comparator != null)
			{
				m_comparator.OnDeleteImpl_(this, treap_node_index);
			}
			int treap_;
			if (treap == -1)
			{
				treap_ = m_defaultTreap;
			}
			else
			{
				treap_ = treap;
			}
			if (!m_b_balancing)
			{
				UnbalancedDelete_(treap_node_index, treap_);
			}
			else
			{
				DeleteNode_(treap_node_index, treap_);
			}
		}

		// Finds an element in the treap and returns its node or -1.
		public int Search(int data, int treap)
		{
			int cur = GetRoot(treap);
			while (cur != NullNode())
			{
				int res = m_comparator.Compare(this, data, cur);
				if (res == 0)
				{
					return cur;
				}
				else
				{
					if (res < 0)
					{
						cur = GetLeft(cur);
					}
					else
					{
						cur = GetRight(cur);
					}
				}
			}
			m_comparator.OnEndSearchImpl_(data);
			return NullNode();
		}

		// Find a first node in the treap which is less or equal the moniker.
		// Returns closest smaller (Comparator::compare returns -1) or any equal.
		public int SearchLowerBound(com.epl.geometry.Treap.MonikerComparator moniker, int treap)
		{
			int cur = GetRoot(treap);
			int bound = -1;
			while (cur != NullNode())
			{
				int res = moniker.Compare(this, cur);
				if (res == 0)
				{
					return cur;
				}
				else
				{
					if (res < 0)
					{
						cur = GetLeft(cur);
					}
					else
					{
						bound = cur;
						cur = GetRight(cur);
					}
				}
			}
			return bound;
		}

		// Find a first node in the treap which is greater or equal the moniker.
		// Returns closest greater (Comparator::compare returns 1) or any equal.
		public int SearchUpperBound(com.epl.geometry.Treap.MonikerComparator moniker, int treap)
		{
			int cur = GetRoot(treap);
			int bound = -1;
			while (cur != NullNode())
			{
				int res = moniker.Compare(this, cur);
				if (res == 0)
				{
					return cur;
				}
				else
				{
					if (res < 0)
					{
						bound = cur;
						cur = GetLeft(cur);
					}
					else
					{
						cur = GetRight(cur);
					}
				}
			}
			return bound;
		}

		// Returns treap node data (element) from the given node index.
		public int GetElement(int treap_node_index)
		{
			return m_treapData.GetField(treap_node_index, 3);
		}

		// no error checking
		// here
		// Returns treap node for the left node for the given treap node index
		public int GetLeft(int treap_node_index)
		{
			return m_treapData.GetField(treap_node_index, 0);
		}

		// no error checking
		// here
		// Returns treap index for the right node for the given treap node index
		public int GetRight(int treap_node_index)
		{
			return m_treapData.GetField(treap_node_index, 1);
		}

		// no error checking
		// here
		// Returns treap index for the parent node for the given treap node index
		public int GetParent(int treap_node_index)
		{
			return m_treapData.GetField(treap_node_index, 2);
		}

		// no error checking
		// here
		// Returns next treap index. Allows to navigate Treap in the sorted order
		public int GetNext(int treap_node_index)
		{
			return m_treapData.GetField(treap_node_index, 6);
		}

		// Returns prev treap index. Allows to navigate Treap in the sorted order
		// backwards
		public int GetPrev(int treap_node_index)
		{
			return m_treapData.GetField(treap_node_index, 5);
		}

		// Returns the first element in the treap (least one). Used together with
		// get_next to write a loop
		public int GetFirst(int treap)
		{
			if (treap == -1)
			{
				return GetFirst_(m_defaultTreap);
			}
			return GetFirst_(treap);
		}

		// Returns the last element in the treap (greatest one). Used together with
		// get_prev to write a loop
		public int GetLast(int treap)
		{
			if (treap == -1)
			{
				return GetLast_(m_defaultTreap);
			}
			return GetLast_(treap);
		}

		// Gets the treap data associated with the treap.
		public int GetTreapData(int treap)
		{
			if (treap == -1)
			{
				return GetTreapData_(m_defaultTreap);
			}
			return GetTreapData_(treap);
		}

		// Change the element value. Note: do not call this method if setting the
		// element will change the sorted order.
		public void SetElement(int treap_node_index, int newElement)
		{
			if (m_comparator != null)
			{
				m_comparator.OnSetImpl_(this, treap_node_index);
			}
			SetElement_(treap_node_index, newElement);
		}

		// Returns the root of the treap.
		public int GetRoot(int treap)
		{
			if (treap == -1)
			{
				return GetRoot_(m_defaultTreap);
			}
			return GetRoot_(treap);
		}

		// Check if the node is Null (does not exist).
		public static int NullNode()
		{
			return -1;
		}

		// Clears all nodes
		public void Clear()
		{
			m_treapData.DeleteAll(false);
			m_defaultTreap = NullNode();
		}

		// Total number of nodes
		public int Size(int treap)
		{
			if (treap == -1)
			{
				return GetSize_(m_defaultTreap);
			}
			return GetSize_(treap);
		}

		// Returns the maximum depth of this Treap at given moment
		public int GetMaxDepth(int treap)
		{
			return GetMaxDepthHelper_(GetRoot(treap));
		}

		public int GetStateFlag()
		{
			m_touchFlag &= unchecked((int)(0x7FFFFFFF));
			return m_touchFlag;
		}

		private int m_defaultTreap;

		private int m_random;

		private com.epl.geometry.Treap.Comparator m_comparator;

		private com.epl.geometry.StridedIndexTypeCollection m_treapData;

		private int m_touchFlag;

		private bool m_b_balancing;

		// comparator used to arrange the
		// nodes
		// m_left (0), m_right (1),
		// m_parent (2), m_element
		// (3), m_priority (4),
		// m_prev (5), m_next (6)
		// (optional: m_root (0),
		// m_first (1), m_last (2),
		// m_duplicate_element (3),
		// m_treap_size (4),
		// m_treapData (5))
		private void Touch_()
		{
			if (m_touchFlag >= 0)
			{
				m_touchFlag += unchecked((int)(0x80000001));
			}
		}

		private int GetPriority_(int treap_node_index)
		{
			return m_treapData.GetField(treap_node_index, 4);
		}

		// no error checking
		// here
		private void BubbleDown_(int treap_node_index)
		{
			int left = GetLeft(treap_node_index);
			int right = GetRight(treap_node_index);
			int priority = GetPriority_(treap_node_index);
			while (left != NullNode() || right != NullNode())
			{
				int lcprior = left != NullNode() ? GetPriority_(left) : com.epl.geometry.NumberUtils.IntMax();
				int rcprior = right != NullNode() ? GetPriority_(right) : com.epl.geometry.NumberUtils.IntMax();
				int minprior = System.Math.Min(lcprior, rcprior);
				if (priority <= minprior)
				{
					return;
				}
				if (lcprior <= rcprior)
				{
					RotateRight_(left);
				}
				else
				{
					RotateLeft_(treap_node_index);
				}
				left = GetLeft(treap_node_index);
				right = GetRight(treap_node_index);
			}
		}

		private void BubbleUp_(int node)
		{
			if (!m_b_balancing)
			{
				return;
			}
			int priority = GetPriority_(node);
			int parent = GetParent(node);
			while (parent != NullNode() && GetPriority_(parent) > priority)
			{
				if (GetLeft(parent) == node)
				{
					RotateRight_(node);
				}
				else
				{
					RotateLeft_(parent);
				}
				parent = GetParent(node);
			}
		}

		private void RotateLeft_(int treap_node_index)
		{
			int px = treap_node_index;
			int py = GetRight(treap_node_index);
			int ptemp;
			SetParent_(py, GetParent(px));
			SetParent_(px, py);
			ptemp = GetLeft(py);
			SetRight_(px, ptemp);
			if (ptemp != NullNode())
			{
				SetParent_(ptemp, px);
			}
			SetLeft_(py, px);
			ptemp = GetParent(py);
			if (ptemp != NullNode())
			{
				if (GetLeft(ptemp) == px)
				{
					SetLeft_(ptemp, py);
				}
				else
				{
					System.Diagnostics.Debug.Assert((GetRight(ptemp) == px));
					SetRight_(ptemp, py);
				}
			}
		}

		private void RotateRight_(int treap_node_index)
		{
			int py = GetParent(treap_node_index);
			int px = treap_node_index;
			int ptemp;
			SetParent_(px, GetParent(py));
			SetParent_(py, px);
			ptemp = GetRight(px);
			SetLeft_(py, ptemp);
			if (ptemp != NullNode())
			{
				SetParent_(ptemp, py);
			}
			SetRight_(px, py);
			ptemp = GetParent(px);
			if (ptemp != NullNode())
			{
				if (GetLeft(ptemp) == py)
				{
					SetLeft_(ptemp, px);
				}
				else
				{
					System.Diagnostics.Debug.Assert((GetRight(ptemp) == py));
					SetRight_(ptemp, px);
				}
			}
		}

		private void SetParent_(int treap_node_index, int new_parent)
		{
			m_treapData.SetField(treap_node_index, 2, new_parent);
		}

		// no error
		// checking here
		private void SetLeft_(int treap_node_index, int new_left)
		{
			m_treapData.SetField(treap_node_index, 0, new_left);
		}

		// no error
		// checking here
		private void SetRight_(int treap_node_index, int new_right)
		{
			m_treapData.SetField(treap_node_index, 1, new_right);
		}

		// no error
		// checking here
		private void SetPriority_(int treap_node_index, int new_priority)
		{
			m_treapData.SetField(treap_node_index, 4, new_priority);
		}

		// no error
		// checking
		// here
		private void SetPrev_(int treap_node_index, int prev)
		{
			System.Diagnostics.Debug.Assert((prev != treap_node_index));
			m_treapData.SetField(treap_node_index, 5, prev);
		}

		// no error checking
		// here
		private void SetNext_(int treap_node_index, int next)
		{
			System.Diagnostics.Debug.Assert((next != treap_node_index));
			m_treapData.SetField(treap_node_index, 6, next);
		}

		// no error checking
		// here
		private void SetRoot_(int root, int treap)
		{
			m_treapData.SetField(treap, 0, root);
		}

		private void SetFirst_(int first, int treap)
		{
			m_treapData.SetField(treap, 1, first);
		}

		private void SetLast_(int last, int treap)
		{
			m_treapData.SetField(treap, 2, last);
		}

		private void SetDuplicateElement_(int duplicate_element, int treap)
		{
			m_treapData.SetField(treap, 3, duplicate_element);
		}

		private void SetSize_(int size, int treap)
		{
			m_treapData.SetField(treap, 4, size);
		}

		private void SetTreapData_(int treap_data, int treap)
		{
			m_treapData.SetField(treap, 5, treap_data);
		}

		private int GetRoot_(int treap)
		{
			if (treap == -1)
			{
				return NullNode();
			}
			return m_treapData.GetField(treap, 0);
		}

		private int GetFirst_(int treap)
		{
			if (treap == -1)
			{
				return NullNode();
			}
			return m_treapData.GetField(treap, 1);
		}

		private int GetLast_(int treap)
		{
			if (treap == -1)
			{
				return NullNode();
			}
			return m_treapData.GetField(treap, 2);
		}

		private int GetDuplicateElement_(int treap)
		{
			if (treap == -1)
			{
				return NullNode();
			}
			return m_treapData.GetField(treap, 3);
		}

		private int GetSize_(int treap)
		{
			if (treap == -1)
			{
				return 0;
			}
			return m_treapData.GetField(treap, 4);
		}

		private int GetTreapData_(int treap)
		{
			return m_treapData.GetField(treap, 5);
		}

		private int NewNode_(int element)
		{
			Touch_();
			int newNode = m_treapData.NewElement();
			SetPriority_(newNode, GeneratePriority_());
			SetElement_(newNode, element);
			return newNode;
		}

		private void FreeNode_(int treap_node_index, int treap)
		{
			if (treap_node_index == NullNode())
			{
				return;
			}
			m_treapData.DeleteElement(treap_node_index);
		}

		private int GeneratePriority_()
		{
			m_random = com.epl.geometry.NumberUtils.NextRand(m_random);
			return m_random & (com.epl.geometry.NumberUtils.IntMax() >> 1);
		}

		private int GetMaxDepthHelper_(int node)
		{
			if (node == NullNode())
			{
				return 0;
			}
			return 1 + System.Math.Max(GetMaxDepthHelper_(GetLeft(node)), GetMaxDepthHelper_(GetRight(node)));
		}

		private int AddElement_(int element, int kind, int treap)
		{
			// dbg_check_(m_root);
			if (GetRoot_(treap) == NullNode())
			{
				int newNode = NewNode_(element);
				SetRoot_(newNode, treap);
				AddToList_(-1, newNode, treap);
				return newNode;
			}
			int cur = GetRoot_(treap);
			int newNode_1 = -1;
			int before = -1;
			for (; ; )
			{
				int cmp = kind == -1 ? 1 : m_comparator.Compare(this, element, cur);
				if (cmp < 0)
				{
					int left = GetLeft(cur);
					if (left != NullNode())
					{
						cur = left;
					}
					else
					{
						before = cur;
						newNode_1 = NewNode_(element);
						SetLeft_(cur, newNode_1);
						SetParent_(newNode_1, cur);
						break;
					}
				}
				else
				{
					if (kind == 1 && cmp == 0)
					{
						m_comparator.OnAddUniqueElementFailedImpl_(element);
						SetDuplicateElement_(cur, treap);
						return -1;
					}
					// return negative value.
					int right = GetRight(cur);
					if (right != NullNode())
					{
						cur = right;
					}
					else
					{
						before = GetNext(cur);
						newNode_1 = NewNode_(element);
						SetRight_(cur, newNode_1);
						SetParent_(newNode_1, cur);
						break;
					}
				}
			}
			BubbleUp_(newNode_1);
			if (GetParent(newNode_1) == NullNode())
			{
				SetRoot_(newNode_1, treap);
			}
			AddToList_(before, newNode_1, treap);
			// dbg_check_(m_root);
			return newNode_1;
		}

		private void AddToList_(int before, int node, int treap)
		{
			System.Diagnostics.Debug.Assert((before != node));
			int prev;
			if (before != -1)
			{
				prev = GetPrev(before);
				SetPrev_(before, node);
			}
			else
			{
				prev = GetLast_(treap);
			}
			SetPrev_(node, prev);
			if (prev != -1)
			{
				SetNext_(prev, node);
			}
			SetNext_(node, before);
			if (before == GetFirst_(treap))
			{
				SetFirst_(node, treap);
			}
			if (before == -1)
			{
				SetLast_(node, treap);
			}
			SetSize_(GetSize_(treap) + 1, treap);
		}

		private void RemoveFromList_(int node, int treap)
		{
			int prev = GetPrev(node);
			int next = GetNext(node);
			if (prev != -1)
			{
				SetNext_(prev, next);
			}
			else
			{
				SetFirst_(next, treap);
			}
			if (next != -1)
			{
				SetPrev_(next, prev);
			}
			else
			{
				SetLast_(prev, treap);
			}
			SetSize_(GetSize_(treap) - 1, treap);
		}

		private void UnbalancedDelete_(int treap_node_index, int treap)
		{
			System.Diagnostics.Debug.Assert((!m_b_balancing));
			// dbg_check_(m_root);
			RemoveFromList_(treap_node_index, treap);
			int left = GetLeft(treap_node_index);
			int right = GetRight(treap_node_index);
			int parent = GetParent(treap_node_index);
			int x = treap_node_index;
			if (left != -1 && right != -1)
			{
				m_random = com.epl.geometry.NumberUtils.NextRand(m_random);
				int R;
				if (m_random > (com.epl.geometry.NumberUtils.IntMax() >> 1))
				{
					R = GetNext(treap_node_index);
				}
				else
				{
					R = GetPrev(treap_node_index);
				}
				System.Diagnostics.Debug.Assert((R != -1));
				// cannot be NULL becaus the node has left and
				// right
				bool bFixMe = GetParent(R) == treap_node_index;
				// swap left, right, and parent
				m_treapData.SwapField(treap_node_index, R, 0);
				m_treapData.SwapField(treap_node_index, R, 1);
				m_treapData.SwapField(treap_node_index, R, 2);
				if (parent != -1)
				{
					// Connect ex-parent of int to R.
					if (GetLeft(parent) == treap_node_index)
					{
						SetLeft_(parent, R);
					}
					else
					{
						System.Diagnostics.Debug.Assert((GetRight(parent) == treap_node_index));
						SetRight_(parent, R);
					}
				}
				else
				{
					// int was the root. Make R the Root.
					SetRoot_(R, treap);
				}
				if (bFixMe)
				{
					// R was a child of int
					if (left == R)
					{
						SetLeft_(R, treap_node_index);
						SetParent_(right, R);
					}
					else
					{
						if (right == R)
						{
							SetRight_(R, treap_node_index);
							SetParent_(left, R);
						}
					}
					SetParent_(treap_node_index, R);
					parent = R;
				}
				else
				{
					SetParent_(left, R);
					SetParent_(right, R);
					parent = GetParent(treap_node_index);
					x = R;
				}
				System.Diagnostics.Debug.Assert((parent != -1));
				left = GetLeft(treap_node_index);
				right = GetRight(treap_node_index);
				if (left != -1)
				{
					SetParent_(left, treap_node_index);
				}
				if (right != -1)
				{
					SetParent_(right, treap_node_index);
				}
				System.Diagnostics.Debug.Assert((left == -1 || right == -1));
			}
			// At most one child is not NULL.
			int child = left != -1 ? left : right;
			if (parent == -1)
			{
				SetRoot_(child, treap);
			}
			else
			{
				if (GetLeft(parent) == x)
				{
					SetLeft_(parent, child);
				}
				else
				{
					System.Diagnostics.Debug.Assert((GetRight(parent) == x));
					SetRight_(parent, child);
				}
			}
			if (child != -1)
			{
				SetParent_(child, parent);
			}
			FreeNode_(treap_node_index, treap);
		}

		// dbg_check_(m_root);
		private void DeleteNode_(int treap_node_index, int treap)
		{
			System.Diagnostics.Debug.Assert((m_b_balancing));
			SetPriority_(treap_node_index, com.epl.geometry.NumberUtils.IntMax());
			// set the node
			// priority high
			int prl = NullNode();
			int prr = NullNode();
			int root = GetRoot_(treap);
			bool isroot = (root == treap_node_index);
			if (isroot)
			{
				// remember children of the root node, if the root node is to be
				// deleted
				prl = GetLeft(root);
				prr = GetRight(root);
				if (prl == NullNode() && prr == NullNode())
				{
					RemoveFromList_(root, treap);
					FreeNode_(root, treap);
					SetRoot_(NullNode(), treap);
					return;
				}
			}
			BubbleDown_(treap_node_index);
			// let the node to slide to the leaves of
			// tree
			int p = GetParent(treap_node_index);
			if (p != NullNode())
			{
				if (GetLeft(p) == treap_node_index)
				{
					SetLeft_(p, NullNode());
				}
				else
				{
					SetRight_(p, NullNode());
				}
			}
			RemoveFromList_(treap_node_index, treap);
			FreeNode_(treap_node_index, treap);
			if (isroot)
			{
				// if the root node is deleted, assign new root
				SetRoot_((prl == NullNode() || GetParent(prl) != NullNode()) ? prr : prl, treap);
			}
			System.Diagnostics.Debug.Assert((GetParent(GetRoot(treap)) == NullNode()));
		}

		private void SetElement_(int treap_node_index, int newElement)
		{
			Touch_();
			m_treapData.SetField(treap_node_index, 3, newElement);
		}
		// no error
		// checking here
	}
}
