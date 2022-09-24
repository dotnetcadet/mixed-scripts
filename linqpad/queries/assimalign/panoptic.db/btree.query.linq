<Query Kind="Program">
  <Namespace>System.Text.Json</Namespace>
</Query>

void Main()
{
	var tree = new BTree<long, long>(1000);
//	var keys = (long)10000;
//	var counter = 0;
//	var page = 1;
//
//	for (long i = 0; i < 1000000; i++)
//	{
//		
//		tree.Insert(keys, page);
//		
//		if(counter == 100)
//		{
//			page++;
//			counter = 0;
//		}
//			
//		keys++;
//		counter++;
//	}
//	
//	keys.Dump();
//	
//	
//	//tree.Insert
//	var t = tree.Search(760145);
//	tree.Delete(760145);
//	tree.Insert(760145, 38);
//	var b = tree.Search(760145);
//	
//	tree.Dump();
//	
	
	/*
		JSON Data Page
		
		| id | { "firstName": "Chase", "lasatName"} |
		| id | { "firstName": "Chase", "lasatName"} |
		| id | { "firstName": "Chase", "lasatName"} |
	*/

	
	
	using (MemoryStream memoryStream = new MemoryStream())
	{
		for(int i = 0;i < 5000; i++)
		{
			var startPosition = memoryStream.Position;
			
			var person = new Person()
			{
				Key = i,
				FirstName = $"Chase {i}",
				LastName = $"Crawford {i}",
				Age = i
			};
			JsonSerializer.SerializeAsync(memoryStream, person);
			
			tree.Insert(person.Key, startPosition);
		}

		JsonSerializer.Serialize(tree, new JsonSerializerOptions()
		{
			WriteIndented = true
		}).Dump();
		
		//byte[] data = null;
		//var record = tree.Search(1623);
		//
		//using(BinaryReader reader = new BinaryReader(memoryStream))
		//{
		//	memoryStream.Position = record.Pointer;
		//	data = reader.ReadBytes(75);
		//}
		//
		//
		//
		//var results = Encoding.UTF8.GetString(data);
		//results.Dump();
		//
		
		
	}

	var masterTree = new BTree<long, BTree<Guid, long>>(5)
	
}




public class Person
{
	public int Key { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public long Age { get; set; }
	
}



internal class BTreeEntry<TKey, TPointer> : IEquatable<BTreeEntry<TKey, TPointer>>
{
	public TKey Key { get; set; }
	public TPointer Pointer { get; set; }
	public bool Equals(BTreeEntry<TKey, TPointer> other) => this.Key.Equals(other.Key) && this.Pointer.Equals(other.Pointer);
}
internal class BTreeNode<TKey, TPointer>
{
	private int degree;

	public BTreeNode(int degree)
	{
		this.degree = degree;
		this.Children = new List<BTreeNode<TKey, TPointer>>(degree);
		this.Entries = new List<BTreeEntry<TKey, TPointer>>(degree);
	}

	public List<BTreeNode<TKey, TPointer>> Children { get; set; }
	public List<BTreeEntry<TKey, TPointer>> Entries { get; set; }
	public bool IsLeaf => this.Children.Count == 0;
	public bool HasReachedMaxEntries => this.Entries.Count == (2 * this.degree) - 1;
	public bool HasReachedMinEntries => this.Entries.Count == this.degree - 1;
}

internal class BTree<TKey, TPointer> where TKey : IComparable<TKey>
{
	public BTree(int degree)
	{
		if (degree < 2)
		{
			throw new ArgumentException("BTree degree must be at least 2", "degree");
		}

		this.Root = new BTreeNode<TKey, TPointer>(degree);
		this.Degree = degree;
		this.Height = 1;
	}

	public BTreeNode<TKey, TPointer> Root { get; private set; }
	public int Degree { get; private set; }
	public int Height { get; private set; }

	/// <summary>
	/// Searches a key in the BTree, returning the entry with it and with the pointer.
	/// </summary>
	/// <param name="key">Key being searched.</param>
	/// <returns>Entry for that key, null otherwise.</returns>
	public BTreeEntry<TKey, TPointer> Search(TKey key)
	{
		return this.SearchInternal(this.Root, key);
	}

	/// <summary>
	/// Inserts a new key associated with a pointer in the BTree. This
	/// operation splits nodes as required to keep the BTree properties.
	/// </summary>
	/// <param name="newKey">Key to be inserted.</param>
	/// <param name="newPointer">Pointer to be associated with inserted key.</param>
	public void Insert(TKey newKey, TPointer newPointer)
	{
		// there is space in the root node
		if (!this.Root.HasReachedMaxEntries)
		{
			this.InsertNonFull(this.Root, newKey, newPointer);
			return;
		}

		// need to create new node and have it split
		BTreeNode<TKey, TPointer> oldRoot = this.Root;
		this.Root = new BTreeNode<TKey, TPointer>(this.Degree);
		this.Root.Children.Add(oldRoot);
		this.SplitChild(this.Root, 0, oldRoot);
		this.InsertNonFull(this.Root, newKey, newPointer);

		this.Height++;
	}

	/// <summary>
	/// Deletes a key from the BTree. This operations moves keys and nodes
	/// as required to keep the BTree properties.
	/// </summary>
	/// <param name="keyToDelete">Key to be deleted.</param>
	public void Delete(TKey keyToDelete)
	{
		this.DeleteInternal(this.Root, keyToDelete);

		// if root's last entry was moved to a child node, remove it
		if (this.Root.Entries.Count == 0 && !this.Root.IsLeaf)
		{
			this.Root = this.Root.Children.Single();
			this.Height--;
		}
	}

	/// <summary>
	/// Internal method to delete keys from the BTree
	/// </summary>
	/// <param name="node">Node to use to start search for the key.</param>
	/// <param name="keyToDelete">Key to be deleted.</param>
	private void DeleteInternal(BTreeNode<TKey, TPointer> node, TKey keyToDelete)
	{
		int i = node.Entries.TakeWhile(entry => keyToDelete.CompareTo(entry.Key) > 0).Count();

		// found key in node, so delete if from it
		if (i < node.Entries.Count && node.Entries[i].Key.CompareTo(keyToDelete) == 0)
		{
			this.DeleteKeyFromNode(node, keyToDelete, i);
			return;
		}

		// delete key from subtree
		if (!node.IsLeaf)
		{
			this.DeleteKeyFromSubtree(node, keyToDelete, i);
		}
	}

	/// <summary>
	/// Helper method that deletes a key from a subtree.
	/// </summary>
	/// <param name="parentNode">Parent node used to start search for the key.</param>
	/// <param name="keyToDelete">Key to be deleted.</param>
	/// <param name="subtreeIndexInNode">Index of subtree node in the parent node.</param>
	private void DeleteKeyFromSubtree(BTreeNode<TKey, TPointer> parentNode, TKey keyToDelete, int subtreeIndexInNode)
	{
		BTreeNode<TKey, TPointer> childNode = parentNode.Children[subtreeIndexInNode];

		// node has reached min # of entries, and removing any from it will break the btree property,
		// so this block makes sure that the "child" has at least "degree" # of nodes by moving an 
		// entry from a sibling node or merging nodes
		if (childNode.HasReachedMinEntries)
		{
			int leftIndex = subtreeIndexInNode - 1;
			BTreeNode<TKey, TPointer> leftSibling = subtreeIndexInNode > 0 ? parentNode.Children[leftIndex] : null;

			int rightIndex = subtreeIndexInNode + 1;
			BTreeNode<TKey, TPointer> rightSibling = subtreeIndexInNode < parentNode.Children.Count - 1
											? parentNode.Children[rightIndex]
											: null;

			if (leftSibling != null && leftSibling.Entries.Count > this.Degree - 1)
			{
				// left sibling has a node to spare, so this moves one node from left sibling 
				// into parent's node and one node from parent into this current node ("child")
				childNode.Entries.Insert(0, parentNode.Entries[subtreeIndexInNode]);
				parentNode.Entries[subtreeIndexInNode] = leftSibling.Entries.Last();
				leftSibling.Entries.RemoveAt(leftSibling.Entries.Count - 1);

				if (!leftSibling.IsLeaf)
				{
					childNode.Children.Insert(0, leftSibling.Children.Last());
					leftSibling.Children.RemoveAt(leftSibling.Children.Count - 1);
				}
			}
			else if (rightSibling != null && rightSibling.Entries.Count > this.Degree - 1)
			{
				// right sibling has a node to spare, so this moves one node from right sibling 
				// into parent's node and one node from parent into this current node ("child")
				childNode.Entries.Add(parentNode.Entries[subtreeIndexInNode]);
				parentNode.Entries[subtreeIndexInNode] = rightSibling.Entries.First();
				rightSibling.Entries.RemoveAt(0);

				if (!rightSibling.IsLeaf)
				{
					childNode.Children.Add(rightSibling.Children.First());
					rightSibling.Children.RemoveAt(0);
				}
			}
			else
			{
				// this block merges either left or right sibling into the current node "child"
				if (leftSibling != null)
				{
					childNode.Entries.Insert(0, parentNode.Entries[subtreeIndexInNode]);
					var oldEntries = childNode.Entries;
					childNode.Entries = leftSibling.Entries;
					childNode.Entries.AddRange(oldEntries);
					if (!leftSibling.IsLeaf)
					{
						var oldChildren = childNode.Children;
						childNode.Children = leftSibling.Children;
						childNode.Children.AddRange(oldChildren);
					}

					parentNode.Children.RemoveAt(leftIndex);
					parentNode.Entries.RemoveAt(subtreeIndexInNode);
				}
				else
				{
					Debug.Assert(rightSibling != null, "Node should have at least one sibling");
					childNode.Entries.Add(parentNode.Entries[subtreeIndexInNode]);
					childNode.Entries.AddRange(rightSibling.Entries);
					if (!rightSibling.IsLeaf)
					{
						childNode.Children.AddRange(rightSibling.Children);
					}

					parentNode.Children.RemoveAt(rightIndex);
					parentNode.Entries.RemoveAt(subtreeIndexInNode);
				}
			}
		}

		// at this point, we know that "child" has at least "degree" nodes, so we can
		// move on - this guarantees that if any node needs to be removed from it to
		// guarantee BTree's property, we will be fine with that
		this.DeleteInternal(childNode, keyToDelete);
	}

	/// <summary>
	/// Helper method that deletes key from a node that contains it, be this
	/// node a leaf node or an internal node.
	/// </summary>
	/// <param name="node">Node that contains the key.</param>
	/// <param name="keyToDelete">Key to be deleted.</param>
	/// <param name="keyIndexInNode">Index of key within the node.</param>
	private void DeleteKeyFromNode(BTreeNode<TKey, TPointer> node, TKey keyToDelete, int keyIndexInNode)
	{
		// if leaf, just remove it from the list of entries (we're guaranteed to have
		// at least "degree" # of entries, to BTree property is maintained
		if (node.IsLeaf)
		{
			node.Entries.RemoveAt(keyIndexInNode);
			return;
		}

		BTreeNode<TKey, TPointer> predecessorChild = node.Children[keyIndexInNode];
		if (predecessorChild.Entries.Count >= this.Degree)
		{
			BTreeEntry<TKey, TPointer> predecessor = this.DeletePredecessor(predecessorChild);
			node.Entries[keyIndexInNode] = predecessor;
		}
		else
		{
			BTreeNode<TKey, TPointer> successorChild = node.Children[keyIndexInNode + 1];
			if (successorChild.Entries.Count >= this.Degree)
			{
				BTreeEntry<TKey, TPointer> successor = this.DeleteSuccessor(predecessorChild);
				node.Entries[keyIndexInNode] = successor;
			}
			else
			{
				predecessorChild.Entries.Add(node.Entries[keyIndexInNode]);
				predecessorChild.Entries.AddRange(successorChild.Entries);
				predecessorChild.Children.AddRange(successorChild.Children);

				node.Entries.RemoveAt(keyIndexInNode);
				node.Children.RemoveAt(keyIndexInNode + 1);

				this.DeleteInternal(predecessorChild, keyToDelete);
			}
		}
	}

	/// <summary>
	/// Helper method that deletes a predecessor key (i.e. rightmost key) for a given node.
	/// </summary>
	/// <param name="node">Node for which the predecessor will be deleted.</param>
	/// <returns>Predecessor entry that got deleted.</returns>
	private BTreeEntry<TKey, TPointer> DeletePredecessor(BTreeNode<TKey, TPointer> node)
	{
		if (node.IsLeaf)
		{
			var result = node.Entries[node.Entries.Count - 1];
			node.Entries.RemoveAt(node.Entries.Count - 1);
			return result;
		}

		return this.DeletePredecessor(node.Children.Last());
	}

	/// <summary>
	/// Helper method that deletes a successor key (i.e. leftmost key) for a given node.
	/// </summary>
	/// <param name="node">Node for which the successor will be deleted.</param>
	/// <returns>Successor entry that got deleted.</returns>
	private BTreeEntry<TKey, TPointer> DeleteSuccessor(BTreeNode<TKey, TPointer> node)
	{
		if (node.IsLeaf)
		{
			var result = node.Entries[0];
			node.Entries.RemoveAt(0);
			return result;
		}

		return this.DeletePredecessor(node.Children.First());
	}

	/// <summary>
	/// Helper method that search for a key in a given BTree.
	/// </summary>
	/// <param name="node">Node used to start the search.</param>
	/// <param name="key">Key to be searched.</param>
	/// <returns>Entry object with key information if found, null otherwise.</returns>
	private BTreeEntry<TKey, TPointer> SearchInternal(BTreeNode<TKey, TPointer> node, TKey key)
	{
		int i = node.Entries.TakeWhile(entry => key.CompareTo(entry.Key) > 0).Count();

		if (i < node.Entries.Count && node.Entries[i].Key.CompareTo(key) == 0)
		{
			return node.Entries[i];
		}

		return node.IsLeaf ? null : this.SearchInternal(node.Children[i], key);
	}

	/// <summary>
	/// Helper method that splits a full node into two nodes.
	/// </summary>
	/// <param name="parentNode">Parent node that contains node to be split.</param>
	/// <param name="nodeToBeSplitIndex">Index of the node to be split within parent.</param>
	/// <param name="nodeToBeSplit">Node to be split.</param>
	private void SplitChild(BTreeNode<TKey, TPointer> parentNode, int nodeToBeSplitIndex, BTreeNode<TKey, TPointer> nodeToBeSplit)
	{
		var newNode = new BTreeNode<TKey, TPointer>(this.Degree);

		parentNode.Entries.Insert(nodeToBeSplitIndex, nodeToBeSplit.Entries[this.Degree - 1]);
		parentNode.Children.Insert(nodeToBeSplitIndex + 1, newNode);

		newNode.Entries.AddRange(nodeToBeSplit.Entries.GetRange(this.Degree, this.Degree - 1));

		// remove also Entries[this.Degree - 1], which is the one to move up to the parent
		nodeToBeSplit.Entries.RemoveRange(this.Degree - 1, this.Degree);

		if (!nodeToBeSplit.IsLeaf)
		{
			newNode.Children.AddRange(nodeToBeSplit.Children.GetRange(this.Degree, this.Degree));
			nodeToBeSplit.Children.RemoveRange(this.Degree, this.Degree);
		}
	}

	private void InsertNonFull(BTreeNode<TKey, TPointer> node, TKey newKey, TPointer newPointer)
	{
		int positionToInsert = node.Entries.TakeWhile(entry => newKey.CompareTo(entry.Key) >= 0).Count();

		// leaf node
		if (node.IsLeaf)
		{
			node.Entries.Insert(positionToInsert, new BTreeEntry<TKey, TPointer>() { Key = newKey, Pointer = newPointer });
			return;
		}

		// non-leaf
		BTreeNode<TKey, TPointer> child = node.Children[positionToInsert];
		if (child.HasReachedMaxEntries)
		{
			this.SplitChild(node, positionToInsert, child);
			if (newKey.CompareTo(node.Entries[positionToInsert].Key) > 0)
			{
				positionToInsert++;
			}
		}

		this.InsertNonFull(node.Children[positionToInsert], newKey, newPointer);
	}
}