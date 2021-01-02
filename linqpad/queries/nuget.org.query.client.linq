<Query Kind="Program">
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
</Query>

async Task Main()
{

	
	var nugetClient = new NugetClientService(Nuget.Query);
	
	nugetClient.Query("Microsoft.Extensions.DependencyInjection");
	
	var results = await nugetClient.GetResultsAsync();
	
	using(var fileStream = new FileStream(@"C:\Users\ccrawford\Documents\Simple.csv",FileMode.OpenOrCreate,FileAccess.Write,FileShare.Write))
	{
		using(var streamWriter = new StreamWriter(fileStream))
		{
			streamWriter.WriteLine("Id,Type,Registration,Package,Title,Description,PackageVersion,Downloads,Id");
			
			foreach (var response in results.Data)
			{
				foreach(var version in response.Versions)
				{
					streamWriter.Write(response.Id.ToString() + ",");
					streamWriter.Write(response.Type + ",");
					streamWriter.Write(response.Registration.ToString() + ",");
					streamWriter.Write(response.Package + ",");
					streamWriter.Write(response.Title + ",");
					streamWriter.Write(response.Description.Replace(Environment.NewLine, " ") + ",");
					
					streamWriter.Write(version.PackageVersion + ",");
					streamWriter.Write(version.Downloads + ",");
					streamWriter.Write(version.Id.ToString());
					streamWriter.Write(streamWriter.NewLine);
				}
			}
		}
	}	
}

public class Nuget
{
	public const string Query = "https://azuresearch-usnc.nuget.org/query";
}


public class NugetClientService
{
	private Uri NugetUri;
	
	public NugetClientService(string nugetUri)
	{
		var uriBuilder = new UriBuilder(nugetUri)
		{
			Query = $"prerelease={IncludePrerelease.ToString().ToLower()}"
		};
		
		NugetUri = uriBuilder.Uri;
	}
	
	public bool HasNextPage { get; set; } = false;
	
	public bool IncludePrerelease { get; set; } = false;
	
	public void Skip(int skip)
	{
		if(NugetUri.Query.Split('=').Contains("skip"))
			throw new Exception("");

		else if (NugetUri.Query != string.Empty && Uri.TryCreate($"{NugetUri.AbsoluteUri}&skip={skip.ToString()}", UriKind.RelativeOrAbsolute, out var currentUri))
			NugetUri = currentUri;
			
		else if(Uri.TryCreate($"{NugetUri.AbsoluteUri}&skip={skip.ToString()}", UriKind.RelativeOrAbsolute, out var newUri))
			NugetUri = newUri;		
	}

	public void Take(int take)
	{
		if (NugetUri.Query.Split('=').Contains("take"))
			throw new Exception("");

		else if (NugetUri.Query != string.Empty && Uri.TryCreate($"{NugetUri.AbsoluteUri}{NugetUri.Query}&take={take.ToString()}", UriKind.RelativeOrAbsolute, out var currentUri))
			NugetUri = currentUri;

		else if (Uri.TryCreate($"{NugetUri.AbsoluteUri}&take={take.ToString()}", UriKind.RelativeOrAbsolute, out var newUri))
			NugetUri = newUri;
	}

	public void Query(string query)
	{
		if (NugetUri.Query.Split('=').Contains("q"))
			throw new Exception("");

		else if (NugetUri.Query != string.Empty && Uri.TryCreate($"{NugetUri.AbsoluteUri}&q={query}", UriKind.RelativeOrAbsolute, out var currentUri))
			NugetUri = currentUri;

		else if (Uri.TryCreate($"{NugetUri.AbsolutePath}&q={query}", UriKind.RelativeOrAbsolute, out var newUri))
			NugetUri = newUri;
	}

	public async Task<NugetResponse> GetResultsAsync(bool ThrowException = false)
	{
		try
		{			
			using(HttpClient Client = new HttpClient())
			{
				var httpRequestMessage = new HttpRequestMessage()
				{
					Method = HttpMethod.Get,
					RequestUri = NugetUri
				};
				
				httpRequestMessage.Headers.Accept.Clear();
				httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				
				var httpResponseMessage = await Client.SendAsync(httpRequestMessage);
				
				if(httpResponseMessage.IsSuccessStatusCode)
				{
					var response = await httpResponseMessage.Content.ReadAsStringAsync();
					
					return JsonSerializer.Deserialize<NugetResponse>(response);
				}
				
				return new NugetResponse();;
			}
		}
		catch(Exception exception)
		{
			if(ThrowException)
				throw exception;
				
			return new NugetResponse();
		}
	}
}


#region Models

public class NugetResponse
{
	[JsonPropertyName("@context")]
	public Context Context { get; set; }
	
	[JsonPropertyName("data")]
	public IEnumerable<Data> Data { get; set; }
}

public class Data
{
	[JsonPropertyName("@id")]
	public Uri Id { get; set; }

	[JsonPropertyName("@type")]
	public string Type { get; set; }

	[JsonPropertyName("registration")]
	public Uri Registration { get; set; }

	[JsonPropertyName("id")]
	public string Package { get; set; }

	[JsonPropertyName("version")]
	public string Version { get; set; }

	[JsonPropertyName("description")]
	public string Description { get; set; }

	[JsonPropertyName("summary")]
	public string Summary { get; set; }

	[JsonPropertyName("title")]
	public string Title { get; set; }

	[JsonPropertyName("iconUrl")]
	public Uri IconUrl { get; set; }

	[JsonPropertyName("licenseUrl")]
	public Uri LicenseUrl { get; set; }

	[JsonPropertyName("projectUrl")]
	public Uri ProjectUrl { get; set; }

	[JsonPropertyName("tags")]
	public List<string> Tags { get; set; }

	[JsonPropertyName("authors")]
	public List<string> Authors { get; set; }

	[JsonPropertyName("totalDownloads")]
	public long TotalDownloads { get; set; }

	[JsonPropertyName("verified")]
	public bool Verified { get; set; }

	[JsonPropertyName("packageTypes")]
	public List<PackageType> PackageTypes { get; set; }

	[JsonPropertyName("versions")]
	public List<Version> Versions { get; set; }
}

public class PackageType
{
	[JsonPropertyName("name")]
	public string Name { get; set; }
}

public class Version
{
	[JsonPropertyName("version")]
	public string PackageVersion { get; set; }

	[JsonPropertyName("downloads")]
	public long Downloads { get; set; }

	[JsonPropertyName("@id")]
	public Uri Id { get; set; }
}

public class Context
{
	[JsonPropertyName("@vocab")]
	public string Vocab { get; set; }

	[JsonPropertyName("@base")]
	public string Base { get; set; }
}

#endregion
