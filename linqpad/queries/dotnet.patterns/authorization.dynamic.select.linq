<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

void Main()
{
	// S01: Identity Authorized Schema
	var properties = typeof(Employee).GetProperties();
	
	foreach (var property in properties)
	{
		var attributes = property.CustomAttributes;
		
		if (attributes.Count() > 0)
		{
			if (attributes.Any(x=>x.AttributeType == typeof(AuthorizeAttribute)))
			{
				var authorizeAttributes = property
						.GetCustomAttributes()
						.Where(x => x.GetType() == typeof(AuthorizeAttribute));

				foreach (var authorizeAttribute in authorizeAttributes)
				{
					var authorize = (AuthorizeAttribute)authorizeAttribute;
					Console.WriteLine($"Authorize Attribute for Property: {property.Name} with {authorize.Policy}");
					//authorize.Policy
				}
			}
		}		
	}
	
	// S02: Begin Dynamic Select
	var employees = new List<Employee>();
	
	
	foreach(var employee in employees)
	{
		
	}
	
	
	
	employees.Select(x=> new
	{
		
	})
}


internal enum SecurityType
{
	None = 0,
	Policy = 1,
	Role = 2,
}


internal class FieldSecurity
{
	public SecurityType SecurityType { get; set; }
	
	
}


namespace System.Linq 
{
	public static class AuthorizeQueryExtensions
	{
		
		public static IEnumerable<T> AuthorizeSelect<T>(this IEnumerable<T> values, ClaimsPrincipal claimsPrinciple)
		{
			throw new NotImplementedException();
		}
	}
}






//public AuthorizeAttribute[] GetAuthorizeAttributes<TObject>() 
//{
//	
//}




#region Employee Data Model

public class Employee
{
	[Authorize("Policy One")]
	[Authorize("Policy Two")]
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string MiddleName { get; set; }
	
	[Authorize("Policy One")
	public EmployeeTaxInformation TaxInformation { get; set; }
	public EmplyeeAddress Address { get; set; }
}

public class EmployeeTaxInformation 
{
	public string SocialSecurity { get; set; }	
	public decimal Salary { get; set; }
}


public class EmplyeeAddress
{
	public string StreetOne { get; set; }
	public string StreetTwo { get; set; }
	public string City { get; set; }
	public string State { get; set; }
}

#endregion

#region Users Authorization Schema
System.Security.AccessControl.AuthorizationRule

#endregion

#region Authorization Implementation


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
public class AuthorizeAttribute : Attribute
{
	
	public AuthorizeAttribute(string policy)
	{
		Policy = policy;
	}
	
	
	public string? Policy { get; set; }
	
	public string? Role { get; set; }
}

#endregion