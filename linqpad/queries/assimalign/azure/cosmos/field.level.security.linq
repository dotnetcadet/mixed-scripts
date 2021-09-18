<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
</Query>

using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

void Main()
{
	
	var services = new ServiceCollection();

	services.AddConfidentialCosmosServices<Employee>(configure =>
	{
		configure.AddPolicy("Test", builder =>
		{
			builder
				.AddField(x=>x.Details.Ssn)
				.AddField(x => x.FirstName);
			
		});
	});
	
	
	
	
	
}

#region test classes
public class Employee 
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public EmployeeDetails Details { get; set; }
}

public class EmployeeDetails
{
	public string Ssn { get; set; }
}

public class User
{
	public string Username { get; set; }
}

#endregion

namespace Assimalign.Azure.Cosmos.Authorization
{
	public sealed class CosmosAuthorizationOptions<T>
		where T : class
	{
		// The Policy Collection
		private IDictionary<string, ICosmosAuthorizationPolicy> _policies = new Dictionary<string, ICosmosAuthorizationPolicy>();

		public CosmosAuthorizationOptions() { }

		/// <summary>
		/// </summary>
		public string Container { get; set; }

		/// <summary>
		/// </summary>
		public string Database { get; set; }
		
		/// <summary>
		/// </summary>
		public string Connection { get; set; }

		/// <summary>
		/// </summary>
		public IEnumerable<ICosmosAuthorizationPolicy<T>> GetPolicies(ClaimsPrincipal claimsPrincipal)
		{
			var policies = _policies.Where(a =>
			{
				var isMatch = true;
				
				if (!Array.TrueForAll(a.Value.Claims, b => claimsPrincipal.Claims.Any(c => c.Type == b))
					isMatch = false;

				if (!Array.TrueForAll(a.Value.Roles, b => claimsPrincipal.Claims.Any(c => c.Type == "role" && c.Value == b))
					isMatch = false;

				return isMatch;
				
			}).Select( x=> x.Value as ICosmosAuthorizationPolicy<T>);
			
			
			return policies;
		}
		
		
		public ICosmosAuthorizationPolicy<T> GetPolicy(string name)
		{
			if (_policies.TryGetValue(name, out var policy)
			{
				return policy as ICosmosAuthorizationPolicy<T>;
			}
			else 
			{
				throw new Exception("");
			}
		}

		public ICosmosAuthorizationPolicy<T> AddPolicy(string name, Action<CosmosAuthorizationBuilder<T>> builder)	
		{
			var policyBuilder = new CosmosAuthorizationBuilder<T>();
			builder.Invoke(policyBuilder);
			var policy = policyBuilder.Build();
			_policies.Add(name, policy);
			return policy;
		}
	}



	public interface ICosmosAuthorizationPolicy
	{
		/// <summary>
		/// </summary>
		string Policy { get; }

		///<summary>
		/// Ths Roles required for this policy
		/// </summary>
		string[] Roles { get; }
		
		///<summary>
		/// 
		/// </summary>
		string[] Claims { get; }
		
		/// <summary>
		/// </summary>
		void AddField(string name, MemberInfo member);
	}

	public interface ICosmosAuthorizationPolicy<T> : ICosmosAuthorizationPolicy
		where T : class
	{
		IEnumerable<MemberInfo> Members { get; }
	}

	public class CosmosAuthorizationPolicy<T> : ICosmosAuthorizationPolicy<T>
		where T : class
	{
		private IDictionary<string, MemberInfo> _members = new Dictionary<string, MemberInfo>();

		internal CosmosAuthorizationPolicy(string policy, string[] roles)
		{
			this.Policy = policy;
			this.Roles = roles;
		}

		/// <summary>
		/// </summary>
		public string Policy { get; }

		/// <summary>
		/// </summary>
		public string[] Roles { get; }
		
		/// <summary>
		/// </summary>
		public string[] Claims { get; }
		
		/// <summary>
		/// </summary>
		public IEnumerable<MemberInfo> Members => _members.Select(x => x.Value);

		/// <summary>
		/// </summary>
		public void AddField(string name, MemberInfo member) =>
			_members.Add(name, member);
			
	}

	public class CosmosAuthorizationBuilder<T>
		where T : class
	{
		private ICosmosAuthorizationPolicy<T> _policy;
		
		/// <summary>
		/// </summary>
		internal ICosmosAuthorizationPolicy<T> Build() => _policy;

		/// <summary>
		/// </summary>
		public CosmosAuthorizationBuilder<T> AddField<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			if (expression.Body is MemberExpression member)
			{
				_policy.AddField(member.Member.Name, member.Member);
			}
			else
			{
				throw new Exception("");
			}
			
			return this;
		}
	}

	public interface ICosmosAuthorizationHandler<T>
		where T : class
	{

	}
}

namespace Microsoft.Extensions.DependencyInjection
{
	using Assimalign.Azure.Cosmos.Authorization;
	
	public static class CosmosAuthorizationExtensions
	{
		public static IServiceCollection AddConfidentialCosmosServices<T>(this IServiceCollection services, Action<CosmosAuthorizationOptions<T>> options)
			where T : class
		{
			var settings = new CosmosAuthorizationOptions<T>();
			options.Invoke(settings);
			
			
			
			
			return services;
		}
	} 
}














