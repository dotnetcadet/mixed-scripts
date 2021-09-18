<Query Kind="Program">
  <Namespace>System.Reflection.Emit</Namespace>
  <Namespace>System.Text.Json</Namespace>
</Query>


#load "assimalign\utilities\function.helpers"

void Main()
{
	var path = @"C:\Users\ccrawford\source\data\assimalign\json.type.bilder\json.object.typea.json";
	var json = GetFileAsync(path).GetAwaiter().GetResult();
	var @object = JsonTypeBuilder.Parse(json).Build();
	
	
}

public class JsonTypeBuilder
{
	private bool _created;
	private Type _type;
	private readonly JsonDocument _document;
	private readonly IDictionary<string, TypeBuilder> _builders = new Dictionary<string, TypeBuilder>();
	
	private string _parent = "parent";
	
	public JsonTypeBuilder(JsonDocument document)
	{
		_document = document;
	}
	
	
	private bool OnParent { get; set; } = true;
	
	
	public object Build()
	{
		if (_created)
		{
			return Activator.CreateInstance(_type);
		}
		else
		{
			var builder = AnonymousTypes.GetTypeBuilder();
			
			if (_document.RootElement.ValueKind == JsonValueKind.Array)
			{
				IterateArray(builder, _document.RootElement, OnParent);
			}
			else if (_document.RootElement.ValueKind == JsonValueKind.Object) 
			{
				IterateObject(builder, _document.RootElement, OnParent);
			}
			
			_type = builder.CreateType();
		}
		
		var instance = Activator.CreateInstance(_type);
		
		if (null != instance)
		{
			_created = true;
		}
		
		return instance;
	}
	
	private void IterateArray(TypeBuilder builder, JsonElement elements, bool onParent)
	{
		var cach = new Dictionary<string, JsonElement>();
		foreach(var element in elements.EnumerateArray())
		{
			if (element.ValueKind == JsonValueKind.Object)
			{
				foreach(var property in element.EnumerateObject())
				{
					if (!cach.ContainsKey(property.Name))
					{
						cach.Add(property.Name, property.Value);
					}
				}
				//IterateObject(builder, element, onParent);
			}
			
			if (element.ValueKind == JsonValueKind.String)
			{
				//builder.CreateProperty(elements
			}
		}
	}
	
	private void IterateObject(TypeBuilder builder, JsonElement element, bool onParent)
	{
		foreach(var member in element.EnumerateObject())
		{
			if (member.Value.ValueKind == JsonValueKind.Object)
			{
				if (_builders.TryGetValue(member.Name, out var cachedBuilder))
				{
					//IterateObject(cachedBuilder,
				}
				else
				{
					var builder1 = AnonymousTypes.GetTypeBuilder(member.Name);
					_builders.Add(member.Name, builder1);
					IterateObject(builder1, member.Value, false);
					builder.CreateProperty(member.Name, builder1.CreateType());
				}
			}
			if (member.Value.ValueKind == JsonValueKind.Array)
			{
				var cache = new Dictionary<string, JsonElement>();
				
				foreach (var entry in member.Value.EnumerateArray())
				{
					foreach (var property in entry.EnumerateObject())
					{
						if (!cache.ContainsKey(property.Name))
						{
							cache.Add(property.Name, property.Value);
						}
					}
				}
				
				foreach(var value in cache)
				{
					if (value.Value.ValueKind == JsonValueKind.Number)
					{
						
					}
				}
			}

			if (member.Value.ValueKind == JsonValueKind.String)
			{
				if (_builders.TryGetValue(member.Name, out var cachedBuilder))
				{
					
				}
				else
				{
					builder.CreateProperty(member.Name, typeof(string));
				}
			}
		}
	}
	
	
	private void CreateProperty(TypeBuilder builder, JsonElement element)
	{
		
	}
	
	
	

	public static JsonTypeBuilder Parse(string json)
	{
		var document = JsonDocument.Parse(json);
		return new JsonTypeBuilder(document);
	}
	
}



public static class AnonymousTypes
{
	private static ModuleBuilder assemblyBuilder = null;
	private static AssemblyName assembly = new AssemblyName() { Name = "AssimalignDynamicLinqTypes" };
	private static Dictionary<string, Type> assemblyTypes = new Dictionary<string, Type>();

	static AnonymousTypes()
	{
		assemblyBuilder = AssemblyBuilder
			.DefineDynamicAssembly(assembly, AssemblyBuilderAccess.Run)
			.DefineDynamicModule(assembly.Name);
	}

	public static TypeBuilder GetTypeBuilder(string name = "Anomymous1", TypeAttributes attributes = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable)
	{
		return assemblyBuilder.DefineType(name, attributes, typeof(object));
	}


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
	
	public static void CreateField(this TypeBuilder builder, string propertyName, Type propertyType)
	{
		
	}

	public static void TruncateAssembly(this TypeBuilder builder)
	{
		
	}
}



