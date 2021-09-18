<Query Kind="Program">
  <NuGetReference>System.IO.Compression</NuGetReference>
  <NuGetReference>System.IO.FileSystem.Primitives</NuGetReference>
  <NuGetReference>System.IO.MemoryMappedFiles</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.IO.MemoryMappedFiles</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Text.Json</Namespace>
</Query>





internal const uint headerPosition = 0;


// Writing String to Mapped File: https://stackoverflow.com/questions/10806518/write-string-data-to-memorymappedfile

async unsafe Task Main()
{
	var fileName = "commentator1";
	var filePath = string.Format(@"C:\Users\ccrawford\source\data\{0}.adf", fileName);
	var fileCapacity = GetMegaBytes(8);
	
	if (File.Exists(filePath))
	{
		var fileInfo = new FileInfo(filePath);
		fileCapacity = (fileInfo.Length / GetMegaBytes(8)) + GetMegaBytes(8);
	}
	
	
	using(var mappedFile = MemoryMappedFile.CreateFromFile(filePath, FileMode.OpenOrCreate, fileName, fileCapacity))
	{
		
		var size = sizeof(FileBlock);
		
		//using(var accessor = mappedFile.CreateViewAccessor(0, 
		
		
//		var header = new Header();
//		
//		using(var accessor = mappedFile.CreateViewAccessor(0, headerSize))
//		{
//			header.SetFileType(FileType.RDBMS);
//			header.SetFileSize(545684);
//			header.SetDataBlockAddress(686547);
//			header.SetIndexBlockAddress(96868);
//			
//			
//			accessor.Write<Header>(0, ref header);
//			
//			Header test;
//			
//		 	accessor.Read<Header>(0, out test);
//
//			var data = JsonSerializer.Serialize(test, new JsonSerializerOptions()
//			{
//			WriteIndented = true,
//				IncludeFields = true
//			});
//			
//			Console.Write(data);
//			
			
		
			
			
//		}


		//using(var bodyAccessor = mappedFile.CreateViewAccessor(GetGigaBytes(256), 
	}
}


public enum FileType: ushort
{
	RDBMS = 1,
	DocumentDb = 2
}


public struct FileHeader
{
	public ulong FileSize;
	public FileType FileType;
	public ulong IndexBlockAddress;
	public ulong DataBlockAddress;
	
	
	public void SetFileType(FileType type) =>
		FileType = type;
	
	public void SetFileSize(ulong size) => 
		FileSize = size;
		
	public void SetIndexBlockAddress(ulong address) =>
		IndexBlockAddress = address;
		
	public void SetDataBlockAddress(ulong address) =>
		DataBlockAddress = address;
	
}


public struct FileBlock
{
	public ulong Pages;
	
	
	public FilePage[] GetPages()
	{
		
	}
}


public struct FilePage
{
	public ushort Header;
	public ushort Body;
	public ushort Offset;
}


public class Column
{
	
}

public struct FilePageRow
{
	public FilePageColumn[] Columns;
}

public enum DataType: ushort
{
	Varcahr, 
	Char,
	Boolean
}

public struct FilePageColumn
{
	public DataType DataType;
	
}



namespace System.IO.MemoryMappedFiles
{
	public static class MemoryMappedFileExtensions
	{
		
		
	
		
		
		public static void WriteString(this MemoryMappedViewAccessor accessor, long position, string value)
		{
			var buffer = Encoding.UTF8.GetBytes(value);
			
			// let write a the length of the buffer first before writing the actual string
			accessor.Write(position, (uint)buffer.Length);
			
			// let's now write the array of char from the string
			accessor.WriteArray(position + 2, buffer, 0, buffer.Length);			
		}
		
		
		public static string ReadString(this MemoryMappedViewAccessor accessor, long position)
		{
			// First lets read the length of an int16 fro mthe starting position
			// This should be the same length of the buffer when the string was written to the file
			var length = accessor.ReadInt16(position);
			
			// Lets now create a buffer of the length
			var buffer = new byte[length];
			
			// Now lets read bytes into the buffer 
			accessor.ReadArray(position + 2, buffer, 0, buffer.Length);
			
			// Now lets convert the byte array into a string
			// hoepfully there is no data conversion issues
			return Encoding.UTF8.GetString(buffer);
		}
	}
	
}







public long GetMegaBytes(long megabytes) => 
	megabytes * 1000000;
	
	
public long GetGigaBytes(long gigabytes) => 
	GetMegaBytes(1000) * gigabytes;



#region btree.index

internal class BTreeEntry<TKey, TPointer> : IEquatable<BTreeEntry<TKey, TPointer>>
{
	public TKey Key { get; set; }

	public TPointer Pointer { get; set; }

	public bool Equals(BTreeEntry<TKey, TPointer> other)
	{
		return this.Key.Equals(other.Key) && this.Pointer.Equals(other.Pointer);
	}
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

	public bool IsLeaf
	{
		get
		{
			return this.Children.Count == 0;
		}
	}

	public bool HasReachedMaxEntries
	{
		get
		{
			return this.Entries.Count == (2 * this.degree) - 1;
		}
	}

	public bool HasReachedMinEntries
	{
		get
		{
			return this.Entries.Count == this.degree - 1;
		}
	}
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
#endregion
