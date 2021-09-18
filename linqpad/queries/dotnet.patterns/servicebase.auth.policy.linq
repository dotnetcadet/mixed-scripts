<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>

using System.Security.Claims;

void Main()
{
	var Services = new ServiceCollection();

	Services.AddAuthService(configure =>
	{
		configure.AddPolicy("Deal.Basic.Read", configure => configure.RequireRole(""));
		//configure.AddPolicy("", configure => configure.
	});
}


public interface IAuthService
{
	Task<TResults> InvokeServiceAsync<TResults>();
	Task<TResults> InvokeServiceAsync<TResults, TOptions>(TOptions options); 
}


public interface IAuthServiceManager
{
	Task<TResults> InvokePolicyAsync<TResults>(string policyName, ClaimsPrincipal claimsPrincipal);
	Task<TResults> InvokePolicyAsync<TResults, TOptions>(string policyName, TOptions parameters, ClaimsPrincipal claimsPrincipal);
}



public class AuthServiceManager : IAuthServiceManager
{
	private readonly AuthServiceOptions Options;
	
	public AuthServiceManager(AuthServiceOptions options)
	{
		Options = options;
	}
	
	
	public async Task<TResults> InvokePolicyAsync<TResults>(string policyName, ClaimsPrincipal claimsPrincipal)
	{
		var authorized = true;
		var policy = Options.GetPolicy(policyName);

		foreach(var role in policy.RequiredRoles)
		{
			authorized = claimsPrincipal.Claims.Any(x =>
			{
				return (x.Type == "roles" && x.Value == role);
			});
		}

		var results = await policy.AuthService.InvokeServiceAsync<TResults>();
		
		return results;
	}
	
	public async Task<TResults> InvokePolicyAsync<TResults, TOptions>(string policyName, TOptions options, ClaimsPrincipal claimsPrincipal)
	{
		var policy = Options.GetPolicy(policyName).AuthService;

		var results = await policy.InvokeServiceAsync<TResults, TOptions>(options);

		return results;
	}
}

public class AuthPolicy
{
	public IEnumerable<string> RequiredRoles { get; set; } = new List<string>();

	public IAuthService AuthService { get; set; }

	
	public void RequireRole(params string[] roles)
	{
		var requiredRoles = new List<string>();
		
		if(RequiredRoles.Any())
			requiredRoles.AddRange(RequiredRoles);
		
		requiredRoles.AddRange(roles);
		
		RequiredRoles = requiredRoles;
	}
}


public class AuthServiceOptions
{
	private IDictionary<string, AuthPolicy> Policies { get; set; } = new Dictionary<string, AuthPolicy>();
	
	
	
	public void AddPolicy(string policyName, AuthPolicy policy)
	{
		Policies.Add(policyName, policy);
	}
	
	public void AddPolicy(string policyName, Action<AuthPolicy> configure)
	{
		var authPolicy = new AuthPolicy();
		configure.Invoke(authPolicy);
		
		Policies.Add(policyName, authPolicy);
	}
	
	public AuthPolicy GetPolicy(string policyName) => 
		Policies[policyName];
}


public static class AuthManagerServiceCollection
{
	
	public static IServiceCollection AddAuthService(this IServiceCollection services, Action<AuthServiceOptions> configure)
	{
		var options = new AuthServiceOptions();
		configure.Invoke(options);
		
		return services.AddSingleton<IAuthServiceManager, AuthServiceManager>();
		
	}
}



