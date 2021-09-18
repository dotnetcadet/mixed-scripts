<Query Kind="Program">
  <NuGetReference>Microsoft.Identity.Client</NuGetReference>
  <NuGetReference>RestSharp</NuGetReference>
  <Namespace>RestSharp.Authenticators</Namespace>
</Query>

using RestSharp
using RestSharp.Authenticators.OAuth;
using Microsoft.Identity.Client;


void Main()
{
	RestClient client = new RestClient() 
	{
		PreAuthenticate = true,
		Authenticator = new MyAuthenticator()
	};
	
}

public class MyAuthenticator : IAuthenticator
{
	public void Authenticate(IRestClient client, IRestRequest request)
	{
		var authClient = ConfidentialClientApplicationBuilder.Create("{Client ID}")
			.WithClientSecret("Client Secret")
			.WithAuthority("https://login.microsoft.com/{Tenant ID}")
			.Build();

		var token = authClient.AcquireTokenForClient(new[] { "./default" });

		client.AddDefaultHeader("Authorization", $"Bearer {token}");
		request.AddHeader("Authorization", $"Bearer {token}");
	}
}


