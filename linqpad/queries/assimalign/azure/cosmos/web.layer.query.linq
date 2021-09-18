<Query Kind="Program">
  <NuGetReference>Assimalign.Objects.ComplexMapper</NuGetReference>
  <NuGetReference>Assimalign.Objects.FluentValidation.DependencyInjection</NuGetReference>
  <Namespace>Assimalign.Objects.ComplexMapper</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Assimalign.Objects.FluentValidation</Namespace>
  <Namespace>Assimalign.Objects.FluentValidation.Results</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;

void Main()
{
	var configurations = new MapperConfiguration(configure=>
	{
		configure.AddProfile<EmployeeProfile>();
	});
	
	var mapper = new Mapper(configurations);
	var mapperType = mapper.ConfigurationProvider.FindTypeMapFor(typeof(EmployeeDb), typeof(Employee));

	
	foreach(var map in mapperType.MemberMaps)
	{
		Expression<Func<Employee>> exp;
	
		map.DestinationType.Dump();
		map.DestinationName.Dump();
	}
	
	
	var employee = new List<EmployeeDb>();
	employee.Select(x=>x.Employee.FirstName)


	
	
}

public string GetSourcePath<TSource, TDestination>(string destinationPath)
{
	return string.Empty;
}

public string GetDestinationPath<TSource, TDestination>(string sourcePath)
{
	return string.Empty;
}



public class EmployeeProfile : Profile
{
	public EmployeeProfile()
	{
		CreateMap<EmployeeDb, Employee>()
			.ForMember(destination => destination.FirstName, options => options.MapFrom(source => source.Employee.FirstName));
			//.ReverseMap();
	}
	
}


public class Employee
{
	public string FirstName { get; set; }	
}

public class EmployeeDb
{
	public EmployeeDetails Employee { get; set; }
	
	public partial class EmployeeDetails
	{
		public string FirstName { get; set; }
		
		public string LastName { get; set; }
	}
}


#region query builder


public static class AssimalignCosmosServices
{
	public static IServiceCollection AddComosDb(this IServiceCollection services)
	{
		
		
		return services;
	}

}

public enum Conjunction : short 
{
	None = 0,
	And = 1, 
	Or = 2
}

public enum Operator : short
{
	Equals = 1,
	NotEquals = 2,
	Contains,
	IsLike,
	IsNotLike
}


internal class OpertaorConverter : JsonConverter<Operator>
{
	public override Operator Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
	{
		
	}

	public override void Write(Utf8JsonWriter writer, Operator value, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}
	
	
	private Operator GetStringOperator(string value)
	{
		switch(value.ToLower())
		{
			case "equals":
				return Operator.Equals;
			case "notequals":
				return Operator.NotEquals;
			
			default: 
				throw new Exception("");
		}
	}
}

public sealed class Where
{
	public string Property { get; set; }

	[JsonConverter(typeof(OpertaorConverter))]
	public Operator? Operator { get; set; }

	public dynamic Value { get; set; }
	
	public Conjunction Conjunction { get; set; } = Conjunction.None;
	
	public IEnumerable<Where> Children { get; set; }
	
	
	
	public bool IsValid<T>()
	{
		var validator = new WhereValidator<T>();
		var validation = Validate<T>(this, ref validator);
		
		return validation.IsValid;
	}
	
	
	private ValidationResult Validate<T>(Where where, ref WhereValidator<T> validator)
	{
		var validation = new ValidationResult();
		
		if (where.Children.Any())
		{
			foreach(var child in where.Children)
			{
				if (child.Children.Any())
				{
					var childValidation = Validate<T>(child, ref validator);
					
				}
			}
		} 
		else 
		{
			results = validator.Validate(where);
			results.
		}
		
		return results;
	}
	
	
	
	internal partial class WhereValidator<T> : AbstractValidator<Where>
	{
		private Type type;
		
		public WhereValidator()
		{
			When(p => p.Children.Any(), () =>
			{
			 	RuleFor(p=>p.Property)
					.NotEmpty();
					
				RuleFor(p=>p.Conjunction)
					.NotEqual(Conjunction.None);
					
				RuleFor(p=>p.Operator)
					.NotEmpty();
			});
			
		}
	}

}



public class Query<T>
{
	public string Path { get; set; }
	
	public Operator Operator { get; set; }
	
	public dynamic Value { get; set; }
	
	
}


public class CosmosQuery<T>
{
	
	public IEnumerable<string> Select { get; set; }
	
	public 
	
	
	
	public Expression<Func<T>> CompileSelect<T>() where T : class
	{
		
	}
	
	
}






#endregion
