<Query Kind="Program" />

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Assimalign.PanopticDb.Types;

public const long TicksPerMillisecond = 10000L;
public const long TicksPerSecond = 10000000L;
public const long TicksPerMinute = 600000000L;
public const long TicksPerHour = 36000000000L;
public const long TicksPerDay = 864000000000L;

public long CurrentTicks = Environment.TickCount64;

unsafe void Main()
{
	
	//var dataTypeSizes = new Dictionary<string, string>()
	//{
	//	{ "Int16" , $"{sizeof(Int16)}"},
	//	{ "Int32" , $"{sizeof(Int32)}"},
	//	{ "Int64" , $"{sizeof(Int64)}"},
	//	{ "DateTime" , $"{sizeof(DateTime)}"},
	//	{ "Time" , $"{sizeof(TimeSpan)}"}
	//};
	//
	//foreach(var size in dataTypeSizes)
	//{
	//	Console.WriteLine($"Data Type: {size.Key} - {size.Value} Bytes");
	//}
	//Console.WriteLine(DateTime.Now);
	//Console.WriteLine(Date.Today());
	
	var time = new Time();
	
	
	var days = CurrentTicks / TicksPerDay;
	
	Console.WriteLine(days);
	
	//var time = new Time();
	
	
}


namespace Assimalign.PanopticDb.Types
{

	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	[TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public readonly struct Time : ISerializable, IFormattable, IConvertible
	{		
		public const long TicksPerMillisecond = 10000L;
		public const long TicksPerSecond = 10000000L;
		public const long TicksPerMinute = 600000000L;
		public const long TicksPerHour = 36000000000L;
		public const long TicksPerDay = 864000000000L;
		
		public readonly uint Hour;
		
		
		public Time(int hour)
		{
			Hour = (uint)hour;
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

		public byte ToByte(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

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
		
		
		
		public static bool operator >(Time ta, Time tb)
		{
			return true;
		}
		
		public static bool operator <(Time ta, Time tb)
		{
			return true;
		}
	}

	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	[TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public readonly struct Date
	{
		
	}

	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	[TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public readonly struct TimeStamp
	{
		
	}

	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	[TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public readonly struct Varchar
	{
		
	}

	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	[TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public readonly struct Bit
	{
		
	}
}


