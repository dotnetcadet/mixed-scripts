<Query Kind="Program">
  <Namespace>Microsoft.Win32.SafeHandles</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Runtime.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
  <Namespace>System.Buffers</Namespace>
</Query>

using Microsoft.Win32.SafeHandles;


async unsafe Task Main()
{
	Time time = new Time();
	
	TimeSpan
	
	time.Hour = 5;
	


}





[Serializable]
[StructLayout(LayoutKind.Auto)]
// [TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public struct Time : ISerializable, IFormattable, IConvertible
{

	private short hour;
	private short minute;
	
	public const long TicksPerMillisecond = 10000L;
	public const long TicksPerSecond = 10000000L;
	public const long TicksPerMinute = 600000000L;
	public const long TicksPerHour = 36000000000L;
	public const long TicksPerDay = 864000000000L;

	public short Hour
	{
		get
		{
			FullSystemTime time = new FullSystemTime(
			return Hour;
		}
		set 
		{
			hour = value;
		}
	}
	
	
	public static Time Now 
	{
		get
		{
			bool isAmbiguousLocalDst;
			long ticks = TimeZoneInfo.GetDateTimeNowUtcOffsetFromUtc(utcNow, out isAmbiguousLocalDst).Ticks;
			long num = utcNow.Ticks + ticks;
		}
	}



	
	
	
 






	//		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
	//	 	private internal GetSystemTime;

	#region IConvertable Methods

	bool IConvertible.ToBoolean(IFormatProvider provider) =>
		throw new InvalidCastException("");


	public TypeCode GetTypeCode()
	{
		throw new NotImplementedException();
	}

	byte IConvertible.ToByte(IFormatProvider provider) =>
		throw new NotImplementedException();

	public char ToChar(IFormatProvider provider)
	{
		throw new NotImplementedException();
	}

	public System.DateTime ToDateTime(IFormatProvider provider)
	{
		throw new NotImplementedException();
	}

	public decimal ToDecimal(IFormatProvider provider)
	{
		throw new NotImplementedException();
	}

	public double ToDouble(IFormatProvider provider)
	{
		throw new NotImplementedException();
	}

	public short ToInt16(IFormatProvider provider)
	{
		throw new NotImplementedException();
	}

	public int ToInt32(IFormatProvider provider)
	{
		throw new NotImplementedException();
	}

	public long ToInt64(IFormatProvider provider)
	{
		throw new NotImplementedException();
	}

	public sbyte ToSByte(IFormatProvider provider)
	{
		throw new NotImplementedException();
	}

	public float ToSingle(IFormatProvider provider)
	{
		throw new NotImplementedException();
	}

	public string ToString(IFormatProvider provider)
	{
		throw new NotImplementedException();
	}

	public object ToType(Type conversionType, IFormatProvider provider)
	{
		throw new NotImplementedException();
	}

	public ushort ToUInt16(IFormatProvider provider)
	{
		throw new NotImplementedException();
	}

	public uint ToUInt32(IFormatProvider provider)
	{
		throw new NotImplementedException();
	}

	public ulong ToUInt64(IFormatProvider provider)
	{
		throw new NotImplementedException();
	}
	#endregion

	#region IFormattable Methods

	public string ToString(string format, IFormatProvider formatProvider)
	{
		throw new NotImplementedException();
	}

	#endregion

	#region ISerializable Methods
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		throw new NotImplementedException();
	}
	#endregion



	public static bool operator >(Time time1, Time time2)
	{
		return true;
	}

	public static bool operator <(Time time1, Time time2)
	{
		return true;
	}
}

