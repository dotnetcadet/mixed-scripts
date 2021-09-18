<Query Kind="Program">
  <NuGetReference Prerelease="true">Statiq.App</NuGetReference>
  <Namespace>Statiq.Core</Namespace>
</Query>

using Statiq.App;
using Statiq.Common;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;



static string[] args = new string[] 
{
	"pipelines", "BlobEventNotificationsPipeline"
};
static async Task Main()
{

	var bootstrapper = Bootstrapper
		.Factory
		.Create(args)
		.BuildPipeline(
			"BlobEventNotificationsPipeline",
			builder => builder
				.With
				.WithInputReadFiles("C:\\Users\\ccrawford\\AppData\\Local\\.IdentityService\\**\\*.json")
				.WithInputModules(new BlobModule())
				.WithProcessModules(
					new BlobModule())
				.WithOutputWriteFiles(".md"));
		
	await bootstrapper.RunAsync();
}



public class BlobContentProvider : IContentProvider
{
	public string MediaType => throw new NotImplementedException();

	public IContentProvider CloneWithMediaType(string mediaType)
	{
		throw new NotImplementedException();
	}

	public Task<int> GetCacheCodeAsync()
	{
		throw new NotImplementedException();
	}

	public long GetLength()
	{
		throw new NotImplementedException();
	}

	public Stream GetStream()
	{
		throw new NotImplementedException();
	}

	public TextReader GetTextReader()
	{
		throw new NotImplementedException();
	}
}

public class BlobDocument : IDocument
{
	public object this[string key] => throw new NotImplementedException();

	public Guid Id => throw new NotImplementedException();

	public NormalizedPath Source => throw new NotImplementedException();

	public NormalizedPath Destination => throw new NotImplementedException();

	public IContentProvider ContentProvider => throw new NotImplementedException();

	public IEnumerable<string> Keys => throw new NotImplementedException();

	public IEnumerable<object> Values => throw new NotImplementedException();

	public int Count => throw new NotImplementedException();

	public IDocument Clone(NormalizedPath source, NormalizedPath destination, IEnumerable<KeyValuePair<string, object>> items, IContentProvider contentProvider = null)
	{
		throw new NotImplementedException();
	}

	public bool ContainsKey(string key)
	{
		throw new NotImplementedException();
	}

	public Task<int> GetCacheCodeAsync()
	{
		throw new NotImplementedException();
	}

	public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
	{
		throw new NotImplementedException();
	}

	public IEnumerator<KeyValuePair<string, object>> GetRawEnumerator()
	{
		throw new NotImplementedException();
	}

	public bool TryGetRaw(string key, out object value)
	{
		throw new NotImplementedException();
	}

	public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
	{
		throw new NotImplementedException();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotImplementedException();
	}
}


public class BlobModule : IModule
{
	public async Task<IEnumerable<IDocument>> ExecuteAsync(IExecutionContext context)
	{
		
		return new[] 
		{
			new BlobDocument()
		};

	
	}
}

// You can define other methods, fields, classes and namespaces here
