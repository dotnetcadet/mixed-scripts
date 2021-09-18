<Query Kind="Program" />

using System.Collections;

public string[] Selects = new[]
{
	"FirstName",
	"Details.Ssn",
	"Details.Age",
	"Details.Addresses.Street",
	"Details.Addresses.Apt"
};

public class Employee
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public EmployeeDetails Details { get; set; }
}
public class EmployeeDetails
{
	public string Ssn { get; set; }
	public int Age { get; set; }
	public IEnumerable<EmployeeAddress> Addresses { get; set; }
}
public class EmployeeAddress
{
	public string Street { get; set; }
	public int Apt { get; set; }
}

void Main()
{
	
	var propertyCollection = Selects.StringArrayToDictionary();
	
	var test = BuildSelect(typeof(Employee), propertyCollection);
	
	
	
	var propertySort = Selects.AsQueryable()
		.OrderBy(x=>x.Count())
		.ThenBy(x=> x);
		
	var properties = propertySort.ToDictionary(x=> x.Split('.') );
}


public Expression BuildSelect(Type type, IDictionary<string, string[]> collection, ParameterExpression parameter = null)
{
	parameter ??= Expression.Parameter(type);
	var bindings = new List<MemberBinding>();
	var instance = Expression.New(type);
	
	foreach (var propertyCollection in collection)
	{
		var member = Expression.Property(parameter, propertyCollection.Key);
		if (propertyCollection.Value.All(x => x == null))
		{
			bindings.Add(Expression.Bind(member.Member, member));
		}
		else if (propertyCollection.Value.Any(x => x != null))
		{
			if (member.Type.IsIEnumerable())
			{
				var enumerableType = member.Type.GetIEnumerableArgument();
				var enumerableParameter = Expression.Parameter(enumerableType, enumerableType.Name);
				var enumerable = BuildSelect(enumerableType, propertyCollection.Value.StringArrayToDictionary(), enumerableParameter);

				var lambda = Expression.Lambda(enumerable, enumerableParameter);
				var linq = Expression.Call(
					typeof(Enumerable),
					"Select",
					new Type[] { enumerableType, enumerable.Type },
					member,
					lambda);
				bindings.Add(Expression.Bind(member.Member, linq));
			}
			else
			{
				var child = BuildSelect(member.Type, propertyCollection.Value.StringArrayToDictionary());
				bindings.Add(Expression.Bind(member.Member, child));
			}
		}
	}
	
	return Expression.MemberInit(instance, bindings);
}


namespace System.Collections
{
	public static class EnumerableExtensions
	{
		public static IDictionary<string, string[]> StringArrayToDictionary(this string[] strings)
		{
			var collection = new Dictionary<string, string[]>();
			var distinct = strings.Select(x => x.Split('.').First()).Distinct();

			foreach (var value in distinct)
			{
				var results = strings
					.Where(x => x.Split('.').First() == value)
					.Select(x =>
					{
						var field = string.Join('.', x.Split('.').Skip(1));
						if (field == string.Empty)
							return null;
						return field;

					}).ToArray();

				collection.Add(value, results);
			}

			return collection;
		}

		public static bool IsIEnumerable(this Type type) =>
			type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);


		public static Type GetIEnumerableArgument(this Type type) =>
			type.GetGenericArguments()[0];
	}
}


//public Expression GetMembers(Type declaringType, string[] properties)
//{
//	
//	for (int i = 0; i < properties.Length; i++)
//	{
//		
//	}
//}
