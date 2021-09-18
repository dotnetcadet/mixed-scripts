<Query Kind="Program">
  <Namespace>System.Reflection.Emit</Namespace>
</Query>

using Assimalign.Reflection.AnonymousTypes;

public string[] Selects = new[]
{
	"FirstName",
	"Details.Ssn",
	"Details.Age"
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
}

void Main()
{
	var builder = AnonymousTypes.GetTypeBuilder("<>f__Anonymous");
	
	var type = builder.CreateType();
	type.Dump();

	//BuildAnonymousTypeFromSelect<Employee>(Selects);

	//	var user = new User() { FirstName = "Chase" };
	//	var builderA = AnonymousTypes.GetTypeBuilder("ClassC");
	//	var builderB = AnonymousTypes.GetTypeBuilder("ClassB");
	//	
	//	
	//	builderA.CreateProperty("FirstName", typeof(string));
	//	builderA.CreateProperty("LastName", typeof(string));
	//	builderB.CreateProperty("Age", typeof(int));
	//	
	//	
	//	
	//	var typeB = builderB.CreateType();
	//	
	//	builderA.CreateProperty("Details", typeB);
	//	
	//	var type = builderA.CreateType();
	//
	//	var constructor = type.GetConstructors().Single();
	//	var parameters = constructor.GetParameters();
	//	var instance = constructor.Invoke(default);
	//	
	//	//var parameterValues = parameters.Select(p => valueDictionary[p.Name]).ToArray();
	//	//var instance = Activator.CreateInstance(type);
	//	var property = type.GetProperty("FirstName");
	//	
	//	var value = property.GetValue(instance);
	//	
	//	property.SetValue(instance, "Chase");
	//	
	//	var proper = user.GetType().GetProperty("FirstName");
	//	instance.Dump();
	//}

}
public IList<MemberBinding> Bindings = new List<MemberBinding>();

public IQueryable<Employee> source = new List<Employee>().AsQueryable();


public void BuildAnonymousTypeFromSelect<T>(string[] statements)
{
	var type = typeof(T);
	var parameter = Expression.Parameter(type, "x");
	var instance = Expression.New(typeof(T));

	for (int i = 0; i < statements.Length; i++)	
	{
		var prop = GetPropertyMemberExpression(statements[i], parameter);
		var property = typeof(T).GetProperty(statements[i].Split('.').FirstOrDefault());
		var propertyExpression = GetPropertyMemberExpression(statements[i], parameter);
		
		if (property.PropertyType.IsValueType)
		{
			var member = Expression.Property(parameter, property);
			Bindings.Add(Expression.Bind(property, member));
		}
		if (property.PropertyType == typeof(string))
		{
			var member = Expression.Property(parameter, property);
			Bindings.Add(Expression.Bind(property, member));
		}
		else
		{
			var parameter2 = Expression.Parameter(property.PropertyType);
			var newInstance = Expression.New(property.PropertyType);
			var property2 = property.PropertyType.GetProperty(statements[i].Split('.').LastOrDefault());
			var bind2 = Expression.Bind(property2, prop);
			var init = Expression.MemberInit(newInstance, new[]
			{
				bind2
			});
			
			// Bind to Parent
			Bindings.Add(Expression.Bind(property, init));
		}
	}

	var initialize = Expression.MemberInit(instance, Bindings);
	var lambda = Expression.Lambda<Func<T, object>>(initialize, parameter);
	var linq = Expression.Call(
		typeof(Queryable),
		"Select",
		new Type[] { source.ElementType, typeof(object) },
		source.Expression,
		lambda);
}

public static MemberExpression GetPropertyMemberExpression(string property, ParameterExpression parameter)
{
	String[] paths = property.Split('.');
	Expression expression = parameter;

	for (int i = 0; i < paths.Length; i++)
	{
		expression = Expression.Property(expression, paths[i]);
	}

	return expression as MemberExpression;
}






namespace Assimalign.Reflection.AnonymousTypes
{
	
	public static class AnonymousTypes
	{
		private static ModuleBuilder assemblyBuilder = null;
		private static AssemblyName assembly = new AssemblyName() { Name = "AssimalignDynamicLinqTypes" };
		private static Dictionary<string, Type> builtTypes = new Dictionary<string, Type>();
		
		static AnonymousTypes()
		{
			assemblyBuilder = AssemblyBuilder
				.DefineDynamicAssembly(assembly, AssemblyBuilderAccess.Run)
				.DefineDynamicModule(assembly.Name);
		}



		public static TypeBuilder GetTypeBuilder(string name = "Anomymous", TypeAttributes attributes = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable)
		{
			return assemblyBuilder.DefineType(name, attributes, typeof(object));
		}

	}

	public static class AnonymousTypesExtensions
	{
		public static void CreateProperty(this TypeBuilder builder, string propertyName, Type propertyType)
		{
			var field = builder.DefineField($"_{propertyName}", propertyType, FieldAttributes.Private);
			var property = builder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
			
			var getMethodBuilder = builder.DefineMethod($"get_{propertyName}",
				MethodAttributes.Public |
				MethodAttributes.HideBySig |
				MethodAttributes.SpecialName,
				propertyType, 
				Type.EmptyTypes);
			
			var setMethodBuilder = builder.DefineMethod($"set_{propertyName}", 
				MethodAttributes.Public |
				MethodAttributes.HideBySig |
				MethodAttributes.SpecialName,
				null,
				new Type[] { propertyType });
			
			var getBuilder = getMethodBuilder.GetILGenerator();
			getBuilder.Emit(OpCodes.Ldarg_0);
			getBuilder.Emit(OpCodes.Ldfld, field);
			getBuilder.Emit(OpCodes.Ret);
			
			
			var setBuilder = setMethodBuilder.GetILGenerator();
			setBuilder.Emit(OpCodes.Ldarg_0);
			setBuilder.Emit(OpCodes.Ldarg_1);
			setBuilder.Emit(OpCodes.Stfld, field);
			setBuilder.Emit(OpCodes.Ret);
			
			property.SetGetMethod(getMethodBuilder);
			property.SetSetMethod(setMethodBuilder);
		}
	}
}



// You can define other methods, fields, classes and namespaces here
