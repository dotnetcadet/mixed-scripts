<Query Kind="Program" />


#nullable enable
void Main()
{
	
	FilePath file = "";
	
}

/*
	Note: Comparing file paths as strings can be dangerous as different OS's have case sensitive file system such as linux. 
		  The following approach can be done with a class, or struct as well
*/
public record FilePath
{
	public string Path { get; set; }
	
	/// <summary>
	/// 
	/// </summary>
	public FilePath(string path)
	{
		if (string.IsNullOrWhiteSpace(path)) 
		{
			throw new ArgumentException("path cannot be null or empty");
		}
		else if(System.IO.Path.GetInvalidPathChars().Intersect(path).Any())
		{
			throw new ArgumentException("path contains illegal characters");
		}
		else
		{
			Path = System.IO.Path.GetFullPath(path.Trim());	
		}
	}
	
	
	public override string ToString() => Path;
	
	public virtual bool Equals(FilePath? other) => (Path).Equals(other?.Path, StringComparison.InvariantCultureIgnoreCase);
			
	public static implicit operator FilePath(string name) => new FilePath(name);
	
	public FileInfo GetInfo() => new FileInfo(Path);
	
	public FilePath Combine(params string[] paths) => System.IO.Path.Combine(paths.Prepend (Path).ToArray());

}


