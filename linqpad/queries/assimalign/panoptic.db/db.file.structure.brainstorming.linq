<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.IO.MemoryMappedFiles</Namespace>
  <Namespace>System.Runtime.Serialization</Namespace>
</Query>



/*





*/



public long GetMegaBytes(long megabytes) =>
   megabytes * 1000000;


public long GetGigaBytes(long gigabytes) =>
	GetMegaBytes(1000) * gigabytes;
	

unsafe async Task Main()
{
	
	var table = new Hashtable();
	
	table.s
	
	var int16Size = sizeof(Int16);
	var int32Size = sizeof(Int32);
	var int64Size = sizeof(Int64);
	var dateSize = sizeof(DateTime);
	
	var fileName = "data.page.test";
	var filePath = string.Format(@"C:\Users\ccrawford\source\data\{0}.adf", fileName);
	var fileCapacity = GetMegaBytes(8);

	if (File.Exists(filePath))
	{
		var fileInfo = new FileInfo(filePath);
		fileCapacity = (fileInfo.Length / GetMegaBytes(8)) + GetMegaBytes(8);
	}

	
	var headerSize = 96;
	var bodySize = 8060;
	var offsetSize = 36;
	var totalPageSize = headerSize + bodySize + offsetSize;
	
	using (var mappedFile = MemoryMappedFile.CreateFromFile(filePath, FileMode.OpenOrCreate, fileName, fileCapacity))
	{
		for (int i = 0; i < 10; i++)
		{
			using (var accessor = mappedFile.CreateViewAccessor(i * totalPageSize, totalPageSize))
			{
				Page page;
				accessor.Read(0, out page);
				
				
				
			}
		}
	}
	
	
	Console.WriteLine(sizeof(Page));
}

/*
	Page Reading Process: 
	
	BOB: Beginning Of Body |
		
		BOR: Beginning Of Row |
		
			BOC | DT: Data Type |
				... data ... |
			EOC |
			
			BOC | DT: Data Type |
				... data ... |
			EOC |
			
			BOC | DT: Data Type |
				... data ... |
			EOC |
		
		EOR: End Of Row |
		
	EOB: End Of Body
	


*/

public enum DataPage: ushort
{
	
}

public enum DataRow: ushort
{
	BOB = 1, // Beginning of Body
	EOB = 2, // End of Body
	BOR = 3, // Beginning Of Row
	EOR = 4, // End Of Row
	BOC = 5, // Beginning of Column
	EOC = 6  // End of Column
}

public enum DataColumn: ushort
{
	EOC
}




public enum DataType: ushort
{
	Int = 1,
	BigInt = 2,
	Boolean = 3,
	Date = 4,
	DateTime = 5,
	Time = 6,
	Varchar = 7,
	Char = 8
}

/*
	Data Structure: 
	
	
	Block (Database)
	


*/

public struct Record
{
	public DataType dataType;
	public ushort position;
}


/*
	A block is a specific allocaiton of a collection of data entities also
	weel known as a database

*/
public struct Block
{
	public ushort CountOfSegments;
}


/*
	A segment is a specifc allocation of a Database Entity. These 
	entities can consist as a:
	
	- Table
	- Document
	- Index
	- Stored Procedure
	- 

*/
public struct Entity
{
	/// <summary>
	/// Referes to the name of the entity
	/// </summary>
	public ushort Name;
	
	/// <summary>
	/// 
	/// </summary>
	public ushort EntityId;
	
	/// <summary>
	/// The total count of extents within this segment
	/// </summary>
	public ushort Extents;
	
	
	
	public struct Schema

}

public struct Extent
{
	public ushort Pages;
	public const ushort AllowedPageCount = 8; // 64 KBs
	
}





[Serializable]
public struct Page : ISerializable,  IFormattable
{
	public const long HeaderPosition = 0;
	public const long BodyPosition = 96;
	public const long OffsetPosition = 8156;
	
	#region Page Header
	public ushort FreeSpace;
	public ushort RowCount;
	#endregion
	
	

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		throw new NotImplementedException();
	}

	public string ToString(string format, IFormatProvider formatProvider)
	{
		throw new NotImplementedException();
	}
}


public static void WriteString(this MemoryMappedViewAccessor accessor, ref Page page, long position, string value)
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

