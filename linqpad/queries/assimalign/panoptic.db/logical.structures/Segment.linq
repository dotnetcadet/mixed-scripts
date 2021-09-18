<Query Kind="Program">
  <Namespace>System.Runtime.Serialization</Namespace>
</Query>

void Main()
{
	
}


public enum SegmentType : ushort
{
	Index = 1,
	Table = 2,
}


[Serializable]
public class Segment : ISerializable, IDisposable
{
	public long ExtendsCount { get ; set; }
	public SegmentType SegmantType  { get ; set; }
	
	
	
	
	
	public void Dispose()
	{
		throw new NotImplementedException();
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		throw new NotImplementedException();
	}
}
