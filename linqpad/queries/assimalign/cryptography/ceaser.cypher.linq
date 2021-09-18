<Query Kind="Program">
  <Namespace>System.Runtime.InteropServices</Namespace>
</Query>

void Main()
{
	var value = "Chase Crawford Ryan";
	var cipher = new CeaserCipher();
	cipher.Key = new int[]{ 45, 85, 65, 78, 90};
	
	var cipherText = cipher.ShiftString(value).Dump();
	
	cipher.Key = new int[]{ -45, -85, -65, -78, -90};
	
	var decipherText = cipher.ShiftString(cipherText).Dump();
	
	DateTime date
	
}

[DllImport("Chases.Machine.Apis.dll")]
public static extern void AssemblyFunction(); 


public interface ICipher
{
	
}



public class CeaserCipher : ICipher
{
	private int[] key;
	private int shift = 13;
	private const string characters = "abcdefghijklmnopqrstuvwxyz";
	
	
	public int[] Key 
	{
		private get => key;
		set
		{
			key = value;	
		}
	}


	public int Shift
	{
		private get => shift;
		set 
		{
			shift = value;
		}
	}

	//perform the shift per char
	public char ShiftChar(char c)
	{
		var index = characters.IndexOf(c);

		if (index == -1)
			return c;
		index = ((index + Shift) + characters.Length) % characters.Length;
		return characters[index];
	}

	//perform the shift on a string
	public string ShiftString(string s)
	{
		var shifts = s;
		foreach(var key in Key)
		{
			this.Shift = key;
			shifts = new string(shifts.Select(ShiftChar).ToArray());
		}
		
		return shifts;
	}
}
