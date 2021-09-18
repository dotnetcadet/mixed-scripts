<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

public async Task Main()
{
	
	var table = new DataTable();
	
	using (Stream stream = new FileStream(@"C:\Users\ccrawford\Documents\panopticdb.df", FileAccess.ReadWrite, FileMode.OpenOrCreate))
	{
		
	}
}

public void CreateHeader(Stream stream, HeaderType headerType, long headerMaxLength)
{
	stream.WriteByte((byte)headerType);
	
	using(StreamWriter writer = new StreamWriter(stream, leaveOpen: true))
	{
		writer.Write(headerType);
		writer.Write(headerMaxLength);
	}
}


public enum HeaderType: byte
{
	FileHeaer = 1,
	BlockHeader = 2,
	PageHeader = 3
}



public class FSHeader
{
	
}