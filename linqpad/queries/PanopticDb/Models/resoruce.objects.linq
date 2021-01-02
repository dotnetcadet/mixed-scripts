<Query Kind="Program" />

void Main()
{
	
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

#endregion


#region Resource Units

internal class Resource
{
	#region Database File Header
	
	public Guid Id { get; set; }
	public ResourceType Type { get; set; }
	public string Name { get; set; }
	
	
	
	
	public long DefinitionBlockAddress { get; set; }
	public long SecurityBlockAddress { get; set; }
	public long IndexBlockAddress { get; set; }
	public long DataBlockAddress { get; set; }
	
	#endregion
}

internal class ResourcePage
{
	
	
	
	
	
	
	private long GetPageId()
	{
		return 0;
	}
}

internal class ResourceDocument
{
	
}

internal class ResourceBlob
{
	
}

#endregion


#region Data File Structure
/*
				     	File Header
	|---------------------------------------------------|
B	| ResourceId: (type: Guid)							|
L	| ResourceType: (type: Enum)						|
O	| ResourceName: (type: String)						|
C	|													|
K	|													|
	|													|
1	|---------------------------------------------------|


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
B	|													|
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