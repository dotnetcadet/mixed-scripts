<Query Kind="Program">
  <Namespace>System.Collections.Concurrent</Namespace>
</Query>

using System.Reflection;

void Main()
{
	var resolver = new PropertyResolver();
	resolver.RegisterType<User>();
}

#region Test Type
public class User
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public IEnumerable<string> Tags { get; set; }
	public IEnumerable<Interest> Interest { get; set; }
}

public class Interest
{
	public InterestType	InterestType { get; set; }
	public string InterestDescription { get; set; }
}

public enum InterestType
{
	Sports,
	Arts, 
}

#endregion

#region Utilities
namespace System.Reflection
{
	public static class TypeExtensions
	{
		public static bool IsEnumerable(this Type type)
		{
			if (type.IsInterface)
				return type.GetInterface("IEnumerable") != null;
			return false;
		}
	}
}

#endregion

public interface IPropertyResolver
{
	
	
	
}

public class PropertyResolver : IPropertyResolver
{
	private readonly ConcurrentDictionary<Type, IDictionary<string, PropertyInfo>> _cache = new();


	public PropertyResolver()
	{

	}



	public void RegisterType<T>()
	{
		var propertyResults = new Dictionary<string, PropertyInfo>();
		Register(typeof(T).GetProperties());
		void Register(PropertyInfo[] properties, string propertyPath = null)
		{
			foreach (var property in properties)
				if (property.PropertyType.IsEnumerable())
					foreach (var argument in property.PropertyType.GetGenericArguments())
						Register(argument.GetProperties(), property.Name);
				else
				{
					var propertyName = string.Join('.', propertyPath, property.Name);
					propertyResults.Add(propertyName, property);
				}
		}
		
		_cache.TryAdd(typeof(T), propertyResults);
	}
	
	
	
	
	public static IPropertyResolver Build<T>()
	{
		var resolver = new PropertyResolver();
		
		
		
		return resolver;
	}
	
	
	
}


