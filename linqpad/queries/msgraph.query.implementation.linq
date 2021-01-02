<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.Configuration.UserSecrets</NuGetReference>
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Hosting</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Http</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Options</NuGetReference>
  <NuGetReference>Microsoft.Graph</NuGetReference>
  <NuGetReference>Microsoft.Identity.Client</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>

using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration.UserSecrets;
using Newtonsoft.Json;


void Main(string[] args)
{
	CreateHostBuilder(args).Build().Run();	
}

public static IHostBuilder CreateHostBuilder(string[] args) =>
	Host.CreateDefaultBuilder(args)
		.ConfigureServices((context, services)=>
		{
			services.AddScoped<ISerializer, GraphRepositorySerializer>();
			services.AddScoped<IAuthenticationProvider, GraphRepositoryAuthProvider>();
			services.AddHttpClient<IHttpProvider, GraphRepositoryHttpProvider>();
			services.AddScoped<IGraphRepository, GraphRepository>();
		});

#region Authentication Context	
public class AzureMsGraphConfigurations
{
	public string AuthClientId { get; set; }
	
	public string AuthTenantId { get; set; }
	
	public string AuthAuthority { get; set; }
	
	public string AuthClientSecret { get; set; }
	
	public string AuthScopes { get; set; }
	
}

public class GraphRepositoryAuthProvider : IAuthenticationProvider
{
	private readonly AzureMsGraphConfigurations Configurations;

	public GraphRepositoryAuthProvider(IOptions<AzureMsGraphConfigurations> options)
	{
		Configurations = options.Value;
	}

	internal AuthenticationResult Results { get; set; }

	internal DateTimeOffset ExpiresOn { get; set; }

	internal string Token { get; set; }

	public async Task AuthenticateRequestAsync(HttpRequestMessage Request)
	{
		if (DateTimeOffset.UtcNow >= ExpiresOn || !Request.Headers.Contains("Authorization"))
		{
			var ClientApplication = ConfidentialClientApplicationBuilder
			   .Create(Configurations.AuthClientId)
			   .WithAuthority(new Uri(Configurations.AuthAuthority))
			   .WithTenantId(Configurations.AuthTenantId)
			   .WithClientSecret(Configurations.AuthClientSecret)
			   .Build();

			var Scopes = new List<string>()
				{
					Configurations.AuthScopes
				};

			Results = await ClientApplication.AcquireTokenForClient(Scopes).ExecuteAsync();
			Token = Results.CreateAuthorizationHeader();
			Request.Headers.Add("Authorization", Token);
			ExpiresOn = Results.ExpiresOn;
		}
	}
}

public interface IGraphRepository
{
	IGraphServiceClient Client { get; set; }
}

public class GraphRepository : IGraphRepository
{
	public GraphRepository(IHttpProvider httpProvider, IAuthenticationProvider authProvider)
	{
		Client = new GraphServiceClient(authProvider, httpProvider);
	}

	public IGraphServiceClient Client { get; set; }
}

public class GraphRepositorySerializer : ISerializer
{
	private readonly JsonSerializerSettings Settings;

	public GraphRepositorySerializer()
	{
		Settings = new JsonSerializerSettings()
		{
			NullValueHandling = NullValueHandling.Ignore
		};

	}

	public T DeserializeObject<T>(Stream stream)
	{
		if (stream.CanSeek)
		{
			using (StreamReader Reader = new StreamReader(stream))
			{
				return JsonConvert.DeserializeObject<T>(Reader.ReadToEnd(), Settings);
			}
		}
		throw new Exception("Unable to Read Stream");
	}

	public T DeserializeObject<T>(string inputString) =>
		JsonConvert.DeserializeObject<T>(inputString, Settings);



	public string SerializeObject(object serializeableObject) =>
		JsonConvert.SerializeObject(serializeableObject, Settings);
}

public class GraphRepositoryHttpProvider : IHttpProvider
{
	private readonly HttpClient Client;

	public GraphRepositoryHttpProvider(ISerializer serializer, HttpClient client)
	{
		Serializer = serializer;
		Client = client;
	}

	public ISerializer Serializer { get; private set; }

	public TimeSpan OverallTimeout { get; set; }


	public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) =>
		await Client.SendAsync(request);

	public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken) =>
		await Client.SendAsync(request, completionOption, cancellationToken);

	public void Dispose() =>
		GC.SuppressFinalize(this);
}
#endregion