<Query Kind="Program">
  <Namespace>System.Runtime.ConstrainedExecution</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


internal static Resource DbResource { get; set; }


internal async Task Main()
{
	using(FileStream fileStream = new FileStream(@"C:\Users\ccrawford\source\data\panopticdb.adf", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
	{
		//var test = Resource.CreateResource();

		var resource = new Resource(fileStream, Encoding.UTF8);
		//{
		//	Id = Guid.NewGuid(),
		//	Name = "Chases.db",
		//	Type = ResourceType.RDBFile
		//};
		//
		//resource.IndexBlockAddress = 1039201;
		//resource.SecurityBlockAddress = 2094032;
		//resource.DataBlockAddress = 304953;
		
		Console.WriteLine(resource.Id);
		Console.WriteLine(resource.Name.Trim());
		Console.WriteLine(resource.Type);
		Console.WriteLine(resource.IndexBlockAddress);
		Console.WriteLine(resource.SecurityBlockAddress);
		Console.WriteLine(resource.DataBlockAddress);
	}
}

#region Resource Unit Header Identifiers
internal enum ResourceType : short
{
	RDBFile = 1,                // Relational Database File
	NRKeyValuePairFile = 2,     // Non-Relational Key Value Paired Database File
	NRDocFile = 3,              // Non-Relational Document Database File
	NRColumnFamilyFile = 4,     // Non-Relational Column Database File
	NRGraphFile = 5,            // Non-Relational Graph File
	NRFSGridFile = 6,           // Non-Relational File-System Grid Database File

	// We set system files to 100 to leave room for future database files within pipeline
	LogFile = 101,
	BackupFile = 102,
}

internal enum PageType: short
{
	DataPage = 1,
	IndexPage = 2,
	TextImage = 4
}

internal enum IndexType : short
{
	/// <summary>
	/// Clustered index sorts and stores the rows data of a table / view based on the order of clustered index key. Clustered 
	/// index key is implemented in B-tree index structure.
	/// </summary>
	Clustered = 1,
	
	/// <summary>
	/// A non clustered index is created using clustered index. Each index row in the non clustered index has non 
	/// clustered key value and a row locator. Locator positions to the data row in the clustered index that has key value.
	/// </summary>
	NonClustered = 2,
	
	/// <summary>
	/// Unique index ensures the availability of only non-duplicate values and therefore, every row is unique.
	/// </summary>
	Unique = 3,
	
	/// <summary>
	/// It supports is efficient in searching words in string data. This type of indexes is used in certain database managers.
	/// </summary>
	FullText = 4,
	
	/// <summary>
	/// It facilitates the ability for performing operations in efficient manner on spatial objects. To 
	/// perform this, the column should be of geometry type.
	/// </summary>
	Spatial = 5,
	
	/// <summary>
	/// A non clustered index. Completely optimized for query data from a well defined subset of data. A filter 
	/// is utilized to predicate a portion of rows in the table to be indexed.
	/// </summary>
	Filtered = 6
	
}

internal enum PartitionType : short
{
	LogicalPartition = 1,
	PhysicalPartition = 2
}

internal enum KeyTypes : short
{
	PartitionKey = 1,
	CompositeKey = 2,
	ClusteredKey = 3,
	ForeignKey = 4

	/*
	CREATE TABLE books (
   		isbn text,
   		title text,
   		author text,
  		publisher text,
   		category text,
		
				 ______________________________				
		        |                              |
    PRIMARY KEY ( (isbn, author),    publisher )
   				  |_____________|   |_________|
					     |			   |
	);			   Partition Key  Clustered Key
	*/

}

internal enum ConstraintType
{
	Check = 1
}

[Obsolete("Not ready for implementation.")]
internal enum CollationType : short
{

}
#endregion


#region Resource Units

internal class ResourceOptions { }

internal interface IResourceManager { }

internal static class Integrals
{
	public const long FileHeaderSize = 100000;
	
	
	internal partial class Blocks
	{
		public const int Header = 500;
	}
	
	
	internal partial class Extends
	{
		public static long Size() => (Page.Size() * PageLimit) + Header;
		public const int Header = 500; 
		public const int PageLimit = 8;
	}

	
	internal partial class Page
	{
		public static int Size() => Header + Body + Offset;
		public const int Header = 96;
		public const int Body = 8060;
		public const int Offset = 36;
	}
}

internal class ServerConfigurations
{
	public string DataPath { get; set; }
	public string LogPath { get; set; }
	public string BackupPath { get; set; }
}

internal interface IResource
{
	Stream Stream { get; }
	Encoding Encoding { get; }

	// Block Referes to all the Segment, Extends, and page in the
	Guid Id { get; }
	String Name { get; set; }
	ResourceType Type { get; set; }
	
	Int64 IndexBlockAddress { get; set; }
	Int64 SecurityBlockAddress { get; set; }
	Int64 DataBlockAddress { get; set; }
	
	// IResource GetResource(ServerConfigurations configurations);
	// IResource GetNewResource(ServerConfigurations configurations);
	// IResource GetCustomResource(ServerConfigurations configurations);
}

internal class Resource : IResource, IDisposable
{	
	private readonly BinaryReader Reader;
	private readonly BinaryWriter Writer;
	
	protected Resource() { }
	
	public Resource(Stream stream, Encoding encoding)
	{		
		Stream = stream;		
		Encoding = encoding;
		Reader = new BinaryReader(stream, encoding, true);
		Writer = new BinaryWriter(stream, encoding, true);
	}
	
	public Stream Stream { get; private set; }
	public Encoding Encoding { get; private set; }

	
	#region Db.File.Header
	public Guid Id
	{
		get
		{
			if (Stream.Length == 0)
				return new Guid();
				
			Stream.Position = 0;
			return new Guid(Reader.ReadBytes(16));
		}
		set
		{
			Stream.Position = 0;
			Writer.Write(value.ToByteArray());
		}
	}

	public String Name
	{
		get
		{
			if (Stream.Length >= 255 + 16)
			{
				Stream.Position = 16;
				var bytes = Reader.ReadBytes(255);
				var str = new String(bytes);
				return str;//Reader.ReadChars(255));
			}
			
			return string.Empty;
		}
		set
		{
			Stream.Position = 16;
			
			var bytes = Encoding.GetBytes(value, 0, value.Length);

			Writer.Write(bytes);
			Writer.Write(new byte[255 - bytes.Length]);
		}
	}

	public ResourceType Type
	{
		get
		{
			Stream.Position = 271;
			return (ResourceType)Reader.ReadInt16();
		}
		set
		{
			Stream.Position = 271;
			Writer.Write((short)value);
		}
	}
	
	public long IndexBlockAddress
	{
		get
		{
			Stream.Position = 273;
			return Reader.ReadInt64();
		}
		set 
		{
			Stream.Position = 273;
			Writer.Write(value);
		}
	}

	public long SecurityBlockAddress
	{
		get
		{
			Stream.Position = 281;
			return Reader.ReadInt64();
		}
		set
		{
			if(value <= IndexBlockAddress )
				throw new Exception("Address must be larger than Index Blok Address.");
				
			Stream.Position = 281;
			Writer.Write(value);
		}
	}
	
	public long DataBlockAddress 
	{
		get
		{
			Stream.Position = 289;
			return Reader.ReadInt64();
		}
		set
		{
			Stream.Position = 289;
			Writer.Write(value);
		}
	}


	#endregion
	
	
	public static IResource CreateResource()
	{
		return new Resource(new FileStream(@"", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None), Encoding.UTF8);
	}
	
	public IResource GetResource(ServerConfigurations configurations)
	{
		throw new NotImplementedException();
	}
	
	public static IResource GetNewResource(ServerConfigurations configurations)
	{
		throw new NotImplementedException();
	}
	
	public IResource GetCustomResource(ServerConfigurations configurations)
	{
		throw new NotImplementedException();
	}



	public partial class Page
	{
		public long Id { get; set; }
		public long ChildId { get; set; }
		public long ParentId { get; set; }
		public PageType PageType { get; set; }
	}

	public void Dispose()
	{
		Writer.Flush();
		Writer.Dispose();
		Reader.Dispose();
		Stream.Dispose();
	}
}

internal class Page
{
	public long Id { get; set; }
	
	
}



internal class ResourceDocument
{
	
}

internal class ResourceBlob
{
	
}
#endregion

#region BTree

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

#region Data File Structure
/*
				     					  File Header
	|-----------------------------------------------------------------------------------|
B	| ResourceId: (type: Guid)					DefinitionBlockAddress: (type: long)	|
L	| ResourceType: (type: Enum)				IndexBlockAddress: (type: long)			|
O	| ResourceName: (type: String)				DocBlockAddress: (type: long)			|
C	|											FsGridBlockAddress: (type: long)		|
K	|											DataBlockAddress: (type: long)			|
	|											GraphBlockAddress: (type: long)			|
	|																					|
	|																					|
1	|-----------------------------------------------------------------------------------|
	

					  Definition Block
	|---------------------------------------------------|
B	|													|
L	|													|
O	|													|
C	|													|
K	|													|
	|													|
2	|													|
	|													|
	|---------------------------------------------------|


				        Index Block 
	|---------------------------------------------------|
B	| 													|
L	|													|
O	|													|
C	|													|
K	|													|
	|													|
3	|													|
	|													|
	|---------------------------------------------------|


		     Document Block (Json/Xml/PlainText)
	|---------------------------------------------------|
B	|													|
L	|													|
O	|													|
C	|													|
K	|													|
	|													|
4	|													|
	|													|
	|---------------------------------------------------|


				  Definition Block
	|---------------------------------------------------|
B	|													|
L	|													|
O	|													|
C	|													|
K	|													|
	|													|
5	|													|
	|													|
	|---------------------------------------------------|



*/
#endregion