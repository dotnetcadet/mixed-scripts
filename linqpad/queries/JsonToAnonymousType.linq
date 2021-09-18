<Query Kind="Program">
  <Namespace>System.Reflection.Emit</Namespace>
</Query>

using System.Text.Json;
using System.Text.Json.Serialization;

string json = "{ \"FirstName\": \"Chase\", \"LastName\": \"Crawford\", \"Details\": { \"Age\": 14, \"SecureInfo\": { \"SSN\": \"555-44-8889\", \"Ids\": [ 2, 3 ] } } }";

void Main()
{
	
	JsonObjectInfo.Parse(json).Dump();

	//var jsonInfo = JsonSerializerAnonymous.Deserialize(, new JsonSerializerOptions()
	//{
	//	PropertyNameCaseInsensitive = true,
	//});
	//
	//jsonInfo.Dump();
}


namespace System.Text.Json 
{
	
	/// <summary>
	/// Describes a Json Object in the form of a Native C# Types.
	/// This can be used to build anonymous types.
	/// </summary>
	public readonly struct JsonObjectInfo
	{
		private JsonPropertyInfo[] properties { get; }
		
		/// <summary>
		/// 
		/// </summary>
		public JsonObjectInfo(IEnumerable<JsonPropertyInfo> properties) =>
			this.properties = properties.ToArray();
		
		/// <summary>
		/// 
		/// </summary>
		public JsonObjectInfo(JsonPropertyInfo[] properties) =>
			this.properties = properties;

		
		public JsonPropertyInfo[] Properties => properties;
		
		/// <summary>
		/// 
		/// </summary>
		public bool HasProperty(string name, bool ignoreCase = false)
		{
			return properties.Any(property => 
				property.Name.Equals(
					name, 
					ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture));
		}
		
		
		/// <summary>
		/// </summary>
		public static JsonObjectInfo Parse(string json)
		{
			var bytes = Encoding.UTF8.GetBytes(json);
			var span = bytes.AsSpan();
			return Parse(span);
		}
		
		/// <summary>
		/// </summary>
		public static JsonObjectInfo Parse(ReadOnlySpan<byte> json)
		{
			JsonObjectInfo jsonObject = default;
			var reader = new Utf8JsonReader(json);
			//var properties = new List<JsonPropertyInfo>();
			
			var propertyName = string.Empty;
			
			// All valid Json Documents should start with an array or object start token
			while(reader.Read())
			{
				// Let's Skip any comments that were parsed
				if (reader.TokenType == JsonTokenType.Comment)
				{
					continue;
				}
				if (reader.TokenType == JsonTokenType.PropertyName)
				{
					propertyName = reader.GetString();
					continue;
				}
				if (reader.TokenType == JsonTokenType.StartObject || reader.TokenType == JsonTokenType.EndArray)
				{
					if (ParseObjectType(ref reader, out var jobject))
					{
						jsonObject = jobject;
						continue;
					}
					else 
					{
						throw new Exception("Invalid Token Type");
					}
				}
			}
			
			return jsonObject;
		}
		
		
		private static bool ParseObjectType(ref Utf8JsonReader reader, out JsonObjectInfo jsonObject)
		{
			jsonObject = default;
			
			var isValid = true;
			var properties = new List<JsonPropertyInfo>();
			var propertyName = string.Empty;
			
			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.Comment)
				{
					continue;
				}
				if (reader.TokenType == JsonTokenType.PropertyName)
				{
					propertyName = reader.GetString();
					continue;
				}
				if (reader.TokenType == JsonTokenType.StartObject)
				{
					if (ParseObjectType(ref reader, out var jobject))
					{
						properties.Add(new JsonPropertyInfo(propertyName, JsonPropertyType.Object, jobject, null));
						continue;
					}
					else 
					{
						throw new Exception("Invalid Json Structure");
					}
				}
				if (reader.TokenType == JsonTokenType.EndObject)
				{
					jsonObject = new JsonObjectInfo(properties);
					return isValid;
				}
				if (reader.TokenType == JsonTokenType.StartArray)
				{
					var isValueType = false;
					JsonObjectInfo jsonObjectChild = default;
					do
					{
						reader.Read();
						if (reader.TokenType == JsonTokenType.Comment)
						{
							continue;
						}
						if (reader.TokenType == JsonTokenType.StartObject)
						{
							
						}
						
						if (
					}
					while (reader.TokenType != JsonTokenType.EndArray);
					if (reader.CurrentDepth == 0 && properties.Count == 0)
					{
						jsonObject = jsonObjectChild;
					}
				}
				if (reader.TokenType == JsonTokenType.EndArray)
				{
					throw new Exception("Internal Error: Json Incorrect parsing");
				}
				if (reader.TokenType == JsonTokenType.False || reader.TokenType == JsonTokenType.True)
				{
					properties.Add(new JsonPropertyInfo(propertyName, JsonPropertyType.Boolean, null, reader.GetBoolean()));
					continue;
				}
				if (reader.TokenType == JsonTokenType.String)
				{
					if (reader.TryGetDateTime(out var dateTime))
					{
						properties.Add(new JsonPropertyInfo(propertyName, JsonPropertyType.DateTime, null, dateTime));
						continue;
					}
					else
					{
						properties.Add(new JsonPropertyInfo(propertyName, JsonPropertyType.String, null, reader.GetString()));
						continue;
					}
				}
				if (reader.TokenType == JsonTokenType.Number)
				{
					if (reader.TryGetInt16(out var int16))
					{
						properties.Add(new JsonPropertyInfo(propertyName, JsonPropertyType.ArrayInt16, null, int16));
						continue;
					}
				}
				if (reader.TokenType == JsonTokenType.Null || reader.TokenType == JsonTokenType.None)
				{
					propertyName = string.Empty;
					continue;
				}
			}

			return isValid;
		}
		
		private static bool ParseArrayType(ref Utf8JsonReader reader, out JsonPropertyType propertyType)
		{
			var isValid = true;
			propertyType = JsonPropertyType.Unknown;
			
			while(reader.TokenType != JsonTokenType.EndArray)
			{
				reader.Read();
			}
			
			if (propertyType == JsonPropertyType.Unknown)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	public readonly struct JsonPropertyInfo
	{
		private string name { get; }    // The Name of the property
		private JsonPropertyType type { get; } 		// The property Type
		private JsonObjectInfo? objectInfo { get; }
		private object? value { get; }
		//private IDictionary<int, object> valueIndex { get; } // Holds a set of values for a specific property 
		
		internal JsonPropertyInfo(string propertyName, JsonPropertyType propertyType, JsonObjectInfo? objectInfo, object? value)
		{
			this.name = propertyName;
			this.type = propertyType;
			this.objectInfo = objectInfo;
			this.value = value;
		}
		
		/// <summary>
		/// The Name of the Property
		/// </summary>
		public string Name => name;
		
		/// <summary>
		/// 
		/// </summary>
		public JsonPropertyType PropertyType => type;
		
		
		/// <summary>
		/// 
		/// </summary>
		public JsonObjectInfo? ObjectInfo => objectInfo;
		
	}
	
	/// <summary>
	/// </summary>
	public enum JsonPropertyType
	{
		Unknown = 0,
		Int16 = 1,
		Int32 = 2,
		Int64 = 3,
		Decimal = 4, // Only Support Floating Point Decimal since it is a the largest bit available
		Guid = 5,
		String = 6,
		DateTime = 7,
		Boolean = 8,
		Object = 9,
		ArrayGuid = 12,
		ArrayString = 11,
		ArrayInt16 = 12,
		ArrayInt32 = 13,
		ArrayInt64 = 14,
		ArrayDateTime = 15,
		ArrayDecimal = 16,
		ArrayBoolean = 17,
		ArrayObject = 18
	}
	
	
	
	
	
	public static class JsonSerializerAnonymous
	{
		private static TypeBuilder typeBuilder;
		
		static JsonSerializerAnonymous()
		{
			typeBuilder = JsonTypeBuilder.GetTypeBuilder();	
		}
		
		// Flow: Parse Type Definition -> Build Type from Definition -> Deserialize Json From Type
		public static object Deserialize(string json, JsonSerializerOptions options = null)
		{
			
			// Check & set default options if null
			options ??= new JsonSerializerOptions()
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase	
			};
			
			var bytes = Encoding.UTF8.GetBytes(json);
			var span = new Span<byte>(bytes);
			var reader = new Utf8JsonReader(new Span<byte>(bytes), true, new JsonReaderState(new JsonReaderOptions()
			{
				AllowTrailingCommas = options.AllowTrailingCommas,
				CommentHandling = options.ReadCommentHandling,
				MaxDepth = options.MaxDepth
			}));
			var type = BuildType(ref reader, options: options);
			
			type.GetProperties().Dump();
			
			return JsonSerializer.Deserialize(json, type, options);
		}
		
		
		
		
		internal static Type BuildType(ref Utf8JsonReader reader, TypeBuilder builder = null, JsonSerializerOptions options = null, string propertyName = null, bool overrideTypeCreation = false)
		{
			builder ??= JsonTypeBuilder.GetTypeBuilder();
			propertyName ??= string.Empty;
			
			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.Comment)
				{
					if (options.ReadCommentHandling == JsonCommentHandling.Skip || 
						options.ReadCommentHandling == JsonCommentHandling.Allow)
					{
						continue;
					}
					else 
					{
						throw new JsonException("Json Comments are not allowed.");
					}
				}
				if (reader.TokenType == JsonTokenType.PropertyName)
				{
					propertyName = reader.GetString();
					continue;
				}
				if (reader.TokenType == JsonTokenType.StartObject)
				{
					var type = BuildType(ref reader, options: options);
					if (reader.CurrentDepth == 0 || string.IsNullOrEmpty(propertyName))
					{
						return type;
					}
					else 
					{
						builder.CreateProperty(propertyName, type);
						continue;
					}
				}
				if (reader.TokenType == JsonTokenType.EndObject)
				{
					if (overrideTypeCreation)
					{
						continue;
					}
					return builder.CreateType();
				}
				if (reader.TokenType == JsonTokenType.StartArray)
				{
					Type type = null;
					if (string.IsNullOrEmpty(propertyName))
					{
						type = BuildType(ref reader, builder, options, default, true);
					}
					else 
					{
						type = BuildType(ref reader, builder, options, propertyName, true);
					}
					
				}
				if (reader.TokenType == JsonTokenType.EndArray)
				{
					return builder.CreateType();
				}
				if (reader.TokenType == JsonTokenType.String)
				{
					var stringValue = reader.GetString();
					if (DateTime.TryParse(stringValue, out var date))
					{
						builder.CreateProperty(propertyName, typeof(DateTime));
						continue;
					}
					else if (IsGuid(stringValue))
					{
						builder.CreateProperty(propertyName, typeof(Guid));
						continue;
					}
					else if (IsTimeSpan(stringValue))
					{
						builder.CreateProperty(propertyName, typeof(TimeSpan));
						continue;
					}
					else
					{
						if (!builder.HasProperty(propertyName))
						{
							builder.CreateProperty(propertyName, typeof(string));
						}
						
						continue;
					}
				}
				if (reader.TokenType == JsonTokenType.Number)
				{
					if (reader.TryGetInt16(out var int16))
					{
						if (!builder.HasProperty(propertyName))
						{
							builder.CreateProperty(propertyName, typeof(short));
						}
						continue;
					}
				}
				//if (reader.IsValueType(out var valueType))
				//{
				//	builder.CreateProperty(propertyName, valueType);
				//	continue;
				//}
			}
			
			return null;
		}
		
		
		internal static bool IsValueType(this Utf8JsonReader reader, out Type type)
		{
			type = null;
			
			if (reader.TokenType == JsonTokenType.String)
			{
				var stringValue = reader.GetString();

				if (IsDateTime(stringValue))
				{
					type = typeof(DateTime);
					return true;
				}
				else if (IsGuid(stringValue))
				{
					type = typeof(Guid);
					return true;
				}
				else if (IsTimeSpan(stringValue))
				{
					type = typeof(TimeSpan);
					return true;
				}
				else
				{
					type = typeof(string);
					return true;
				}
			}
			if (reader.TokenType == JsonTokenType.False || 
				reader.TokenType == JsonTokenType.True)
			{
				type = typeof(bool);
				return true;
			}
			if (reader.TokenType == JsonTokenType.Number)
			{
				type = typeof(int);
				return true;
				
			}
			
			return false;
		}
		
		internal static bool IsDateTime(string dateTime)
		{
			return DateTime.TryParse(dateTime, out var date);
		}
		
		internal static bool IsGuid(string guid)
		{
			return Guid.TryParse(guid, out var id);
		}
		
		internal static bool IsTimeSpan(string timeSpan)
		{
			return TimeSpan.TryParse(timeSpan, out var tim);
		}
	}

	internal static class JsonTypeBuilder
	{
		private static int assemblyTypeIndex = 0;
		private static ModuleBuilder assemblyBuilder;
		private static AssemblyName assembly = new AssemblyName() { Name = "AssimalignDynamicLinqTypes" };

		private static Dictionary<string, Type> assemblyTypes = new Dictionary<string, Type>();
		private static IDictionary<string, Type> assemblyTypeProperties = new Dictionary<string, Type>();

		
		static JsonTypeBuilder()
		{
			assemblyBuilder = AssemblyBuilder
				.DefineDynamicAssembly(assembly, AssemblyBuilderAccess.Run)
				.DefineDynamicModule(assembly.Name);
		}
		


		public static TypeBuilder GetTypeBuilder(string name = "Anomymous", TypeAttributes attributes = TypeAttributes.Public | TypeAttributes.Class )
		{
			assemblyTypeIndex++;
			return assemblyBuilder.DefineType($"{name}<>`{assemblyTypeIndex}", attributes, typeof(object));
		}



		/// <summary>
		///
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="key"></param>
		public static Type CreateType(this TypeBuilder builder, string key = "")
		{
			return assemblyTypes.TryGetValue(key, out var type) ? 
				type : 
				builder.CreateType();
		}
		
		
		public static bool HasProperty(this TypeBuilder builder, string propertyName, Type propertyType = null)
		{
			if (assemblyTypeProperties.TryGetValue($"{builder.Name}-{propertyName}", out var type))
			{
				return true;
			}
			else 
			{
				return false;
			}
		}





		/// <summary>
		/// Defines a public property on a the type being built
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="propertyName"></param>
		/// <param name="propertyType"></param>
		public static void CreateProperty(this TypeBuilder builder, string propertyName, Type propertyType)
		{

			assemblyTypeProperties.Add($"{builder.Name}-{propertyName}", propertyType);
			
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


