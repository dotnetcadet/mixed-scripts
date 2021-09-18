<Query Kind="Program">
  <Namespace>Xunit</Namespace>
</Query>

#load "xunit"

void Main()
{

	var user1 = new User() 
	{ 
		FirstName = "Chase", 
		MiddleName = "Rya",
		Int32Age = 34
	};
	
	var user2 = new User()
	{
		MiddleName = "Ryan",
		LastName = "Crawford",
		Int16Age = 24,
		UserStatus = Status.PaidUser,
		Addresses1 = new List<UserAddress>()
		{
			new UserAddress()
			{
				AddressId = Guid.NewGuid()
			}
		}
	};
	
	
	var user = Merger.Merge(user1,user2);
	user1.Dump();
	user2.Dump();
	user.Dump();
	
	
//	RunTests();  // Call RunTests() or press Alt+Shift+T to initiate testing.

//	var us1 = new User() { FirstName = "Chase", Age = 23 };
//	var us2 = new User()
//	{
//		LastName = "Crawford",
//		Addresses1 = new UserAddress[]
//		{
//			new UserAddress()
//		},
//		Addresses2 = new UserAddress[]
//		{
//			new UserAddress()
//		},
//		Addresses3 = new UserAddress[]
//		{
//			new UserAddress()
//		},
//		Addresses4 = new Dictionary<int, UserAddress>
//		{
//			{1, new UserAddress()}
//		}
//
//	}; //, JobCount = new int[] { 2, 4, 5}  };
//												   //var us3 = new User() { FirstName = "Chad", IsActive = true, Details = new UserDetail() { PhoneNumber = "4586" } };
//												   //var us4 = new User() { Age = 24, Details = new UserDetail() { PhoneNumber = "4444", Ssn = "555-84-999"}};
//												   //var us5 = new User() { MiddleName = "Ryan", IsActive = false };
//
//	// Adds Changes 
//	var user = Merger.Merge(us1, us2, new()
//	{
//		IsIntegerDefaultSetAllowed = true
//	});
//	user.Dump();
	
	// Updates Changes (First Name change)
//	user = Merger.Merge(user, us3);
//	user.Dump();
//	
//	// Adds additional changes
//	user = Merger.Merge(user, us4);
//	user.Dump();
//
//	// Allows nulls to be set. Only Middle Name should be set
//	user = Merger.Merge(user, us5, new()
//	{
//		IsNullableSetAllowed = true,
//		IsIntegerDefaultSetAllowed = true
//	});
//	user.Dump();
}







public sealed class MergerOptions
{
	
	
	
	
	
	
	internal IDictionary<Type, PropertyInfo[]> uniqueKeys = new Dictionary<Type, PropertyInfo[]>();
	
	/*
		NOTES: 
			
	
	*/
	
	
	/// <summary>
	/// Indicates whether properties should be set to null.
	/// </summary>
	public bool IsNullableSetAllowed { get; set; } = false;
	
	/// <summary>
	/// Indicates whether Int16, Int32, Int64, Decimals, and Double should be set to 0 if not nullable.
	/// </summary>
	public bool IsNumericDefaultSetAllowed { get; set; } = false;
	
	/// <summary>
	/// 
	/// </summary>
	public bool IsBooleanDefaultSetAllowed { get; set; } = false;
	
	/// <summary>
	/// 
	/// </summary>
	public bool IsDateTimeDefaultSetAllowed { get; set; } = false;
	
	
	/// <summary>
	/// Indicates that only nullable boolean values should be set. Default is 'true'.
	/// </summary>
	/// <remarks>
	/// Since there are only two possibilities for a boolean it's impossible to tell if the value was 
	/// explicitly changed to 'false' from 'true' since that is the default value. By setting this property to 
	/// </remarks>
	//public bool OnlySetNullableBooleanValues { get; set; } = true;
	
	
	/// <summary>
	/// 
	/// </summary>
	public MergerOptions AddTypeKey<T>(Expression<Func<T, PropertyInfo[]>> expression)
	{
		var func = expression.Compile();
		
		//func.Invoke(
		
		return this;
	}
}

public static class Merger
{
	
	/// <summary>
	/// Will two objects of the same type
	/// </summary>
	public static T Merge<T>(T target, T source, MergerOptions options = null)
		where T : class, new()
	{
		options ??= new MergerOptions();
		
		// Create a Clone instance of the target to recevie merges
		var instance = target.Clone();
		
		// Get properties to iterate through. Let's memoize the request for the type so we don't have to b
		var getProperties = Cacher<T, PropertyInfo[]>.Memoize(instance =>
			instance.GetType()
			.GetProperties()
			.Where(prop => prop.CanRead && prop.CanWrite)
			.ToArray());

		// Iterate through properties and evaluate target to source
		foreach (var property in getProperties.Invoke(target))
		{
			var targetValue = property.GetValue(target);
			var sourceValue = property.GetValue(source);

			// Check if property on both target and source are null as well as does not wrapped in nullable class, if so, exit
			if (sourceValue == null && targetValue == null && property.PropertyType != typeof(Nullable<>))
				continue;

			// Check if Nullable target set allowed
			if (sourceValue == null && !options.IsNullableSetAllowed)
				continue;

			// Check if property type if Array Type
			if (property.PropertyType.IsArrayType(out var arrayType))
			{
				// var keys = options.uniqueKeys[type];
				// Check if element type of array is System Value Type
				if (arrayType.IsSystemValueType())
				{
					foreach(var item in (object[])sourceValue)
					{
						
						if (((object[])sourceValue).Contains(item))
						{
							
						}
					}
				}
				// Check if element type of array is System Value Type
				else if (arrayType.IsObjectType())
				{
					
				}
				continue;
			}
			
			// Check if property type if Enumerable Type (IEnumerable<>, IEnumerable, ICollection, IList, IDictionary, etc.,)
			if (property.PropertyType.IsTypedEnumerable(out var enumType))
			{
				targetValue ??= Activator.CreateInstance(typeof(List<>).MakeGenericType(enumType));
				if (enumType.IsSystemValueType())
				{
					
				}
				else 
				{
					//var enumerable = Merge(
				}
				//typeof(List<>).MakeGenericType(property.PropertyType)
			}
			
			// 
			if (property.PropertyType.IsDictionaryType())
			{
				Console.WriteLine($"Found Dictionary Type for: {property.Name}");
			}

			// Check if property type is Object Type (Class or Record )
			if (property.PropertyType.IsObjectType())
			{
				// Check if target value is null. Need to create an instance if not to be able to get the type for child loop
				targetValue ??= Activator.CreateInstance(property.PropertyType);

				// If source is null and the code has made it this far 
				// this means that setting properties to null is allowed
				if (sourceValue == null)
				{
					property.SetValue(instance, null);
				}
				else
				{
					// Capture Child Merge Changes then set them to the new instance of the target
					var changes = Merge(targetValue, sourceValue, options);
					property.SetValue(instance, changes);
				}
				continue;
			}

			// Check if Non-Nullable Source Value is different than target value
			if (targetValue != sourceValue && sourceValue != null)
			{
				// Lets check if property type if numeric 
				if (property.PropertyType.IsSignedNumericType() || property.PropertyType.IsUnsignedNumericType())
				{
					SetNumericValueType(ref instance, sourceValue, property, options);
					continue;
				}
				if (property.PropertyType.IsBooleanType())
				{
				 	SetBooleanValueType(ref instance, sourceValue, property, options);
					continue;
				}
				if (property.PropertyType.IsDateTimeType())
				{
					SetDateTimeValueType(ref instance, sourceValue, property, options);
					continue;
				}
				if (property.PropertyType.IsTimeSpanType())
				{
					SetTimeSpanValueType(ref instance, sourceValue, property, options);
					continue;
				}
				if (property.PropertyType.IsGuidType())
				{
					SetGuidValueType(ref instance, sourceValue, property, options);
					continue;
				}
				if (sourceValue is string stringValue)
				{
					property.SetValue(instance, stringValue);
					continue;
				}
				// Just incase we missed any value types lets try to set the value to the target
				// if IsSystemValueType evaluates true
				if (property.PropertyType.IsSystemValueType())
				{
					property.SetValue(instance, sourceValue);
					continue;
				}
			}
			// If we have made this farin the code then setting property to null value is allowed
			else
			{
				property.SetValue(instance, sourceValue);
			}
		}
		return (T)instance;
	}
	
	
	/// <summary>
	/// 
	/// </summary>
	public static IEnumerable<T> Merge<T>(IEnumerable<T> target, IEnumerable<T> source, MergerOptions options = null)
		where T : class, new()
	{
		options ??= new MergerOptions();
		
		
		
		
		return target;
	}
	
	
	
	/// <summary>
	/// 
	/// </summary>
	private static void SetNumericValueType<T>(ref T target, object value, PropertyInfo property, MergerOptions options)
	{
		var isAllowed = options.IsNumericDefaultSetAllowed;
		
		if (value is ushort uint16 && ((uint16 == 0 && isAllowed) || uint16 > 0))
		{
			property.SetValue(target, value);
		}
		else if (value is short int16 && ((int16 == 0 && isAllowed) || int16 > 0 || int16 < 0))
		{
			property.SetValue(target, value);
		}
		else if (value is uint uint32 && ((uint32 == 0 && isAllowed) || uint32 > 0))
		{
			property.SetValue(target, value);
		}
		else if (value is int int32 && ((int32 == 0 && isAllowed) || int32 > 0 || int32 < 0))
		{
			property.SetValue(target, value);
		}
		else if (value is ulong uint64 && ((uint64 == 0 && isAllowed) || uint64 > 0))
		{
			property.SetValue(target, value);
		}
		else if (value is long int64 && ((int64 == 0 && isAllowed) || int64 > 0 || int64 < 0))
		{
			property.SetValue(target, value);
		}
		else if (value is decimal deci && ((deci == 0 && isAllowed) || deci > 0 || deci < 0))
		{
			property.SetValue(target, value);
		}
		else if (value is double dbl && ((dbl == 0 && isAllowed) || dbl > 0 || dbl < 0))
		{
			property.SetValue(target, value);
		}
		else if (value is null && options.IsNullableSetAllowed)
		{
			property.SetValue(target, null);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	private static void SetBooleanValueType<T>(ref T target, object value, PropertyInfo property, MergerOptions options)
	{
		// Check if the boolean value is 'true' or if default value set is allowed
		if (value is bool boolean && (boolean || options.IsBooleanDefaultSetAllowed))
		{
			property.SetValue(target, boolean);
		}
		if (value is null && options.IsNullableSetAllowed)
		{
			property.SetValue(target, null);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	private static void SetDateTimeValueType<T>(ref T target, object value, PropertyInfo property, MergerOptions options)
	{
		if (value is DateTime dateTime && options.IsDateTimeDefaultSetAllowed)
		{
			property.SetValue(target, dateTime);
		}
		if (value is null && options.IsNullableSetAllowed)
		{
			property.SetValue(target, null);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	private static void SetTimeSpanValueType<T>(ref T target, object value, PropertyInfo property, MergerOptions options)
	{
		if (value is TimeSpan timeSpan && options.IsBooleanDefaultSetAllowed)
		{
			property.SetValue(target, timeSpan);
		}
		if (value is null && options.IsNullableSetAllowed)
		{
			property.SetValue(target, null);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	private static void SetGuidValueType<T>(ref T target, object value, PropertyInfo property, MergerOptions options)
	{
		if (value is Guid guid && options.IsBooleanDefaultSetAllowed)
		{
			property.SetValue(target, guid);
		}
		if (value is null && options.IsNullableSetAllowed)
		{
			property.SetValue(target, null);
		}
	}

	#region extensions
	/// <summary>
	/// Check if the type is an array
	/// </summary>
	/// <param name="type"></param>
	/// <param name="implementation"></param>
	/// <returns></returns>
	public static bool IsArrayType(this Type type, out Type implementation)
	{
		var isArray = false;
		if (type.IsArray)
		{
			implementation = type.GetElementType();
			isArray = true;
		}
		else
			implementation = null;
		return isArray;
	}

	/// <summary>
	/// Identifies whether type implements the IEnumerable interface
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public static bool IsTypedEnumerable(this Type type, bool search = false)
	{
		if (type.IsGenericType)
		{
			if (type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				return true;

			var interfaces = type.GetInterfaces();
			foreach (var argument in type.GetGenericArguments())
			{
				var enumerableType = typeof(IEnumerable<>).MakeGenericType(argument);

				if (interfaces.Contains(enumerableType))
					return true;
			}
		}
		else if (search)
		{
			return type.FindInterfaces((filter, criteria) => IsTypedEnumerable(filter), null)[0] != null;
		}

		return false;
	}

	/// <summary>
	/// Identifies whether type implements the IEnumerable interface and returns interface implementation type.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="implementation">The IEnumerable Implementation</param>
	/// <returns></returns>
	public static bool IsTypedEnumerable(this Type type, out Type implementation)
	{
		implementation = null;
		if (type.IsGenericType)
		{
			var arguments = type.GetGenericArguments();
			if (type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				implementation = arguments[0];
				return true;
			}
			var interfaces = type.GetInterfaces();
			foreach (var argument in arguments)
			{
				var enumerableType = typeof(IEnumerable<>).MakeGenericType(argument);

				if (interfaces.Contains(enumerableType))
				{
					implementation = argument;
					return true;
				}
			}
		}
		else
		{
			var other = type.FindInterfaces((filter, criteria) => 
				IsTypedEnumerable(filter), null);

			if (null != other && other.Length > 0)
			{
				implementation = other[0].GetGenericArguments()?.First();
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	public static bool IsObjectType(this Type type)
	{
		if (type.IsClass && 
			!type.IsArray &&
			!type.IsTypedEnumerable() &&
			type != typeof(string))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Checks whether the type is Signed Numeric (meaning it can be negative or positive)
	/// </summary>
	/// <param name="type"></param>
	/// <param name="checkNullable">
	public static bool IsSignedNumericType(this Type type, bool checkNullable = true)
	{
		var numericTypes = new Type[]
		{
			typeof(short),
			typeof(int),
			typeof(long),
			typeof(double),
			typeof(decimal),
			typeof(Single)
		};
		
		foreach(var numericType in numericTypes)
		{
			if (checkNullable && type == typeof(Nullable<>).MakeGenericType(numericType))
			{
				return true;
			}
			else if (type == numericType)
			{
				return true;
			}
		}

		return false;
	}
	
	/// <summary>
	/// Checks whether the type is Unsigned Numeric Type
	/// </summary>
	/// <param name="type"></param>
	public static bool IsUnsignedNumericType(this Type type, bool checkNullable = true)
	{
		var numericTypes = new Type[]
		{
			typeof(ushort),
			typeof(uint),
			typeof(ulong)
		};
		
		foreach(var numericType in numericTypes)
		{
			if (checkNullable && type == typeof(Nullable<>).MakeGenericType(numericType))
			{
				return true;
			}
			else if (type == numericType)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <param name="checkNullable">
	public static bool IsBooleanType(this Type type, bool checkNullable = true)
	{
		if (checkNullable && type == typeof(Nullable<>).MakeGenericType(typeof(bool)))
		{
			return true;
		}
		if (type == typeof(bool))
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <param name="checkNullable">
	public static bool IsGuidType(this Type type, bool checkNullable = true)
	{
		if (checkNullable && type == typeof(Nullable<>).MakeGenericType(typeof(Guid)))
		{
			return true;
		}
		if (type == typeof(Guid))
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <param name="checkNullable">
	public static bool IsDateTimeType(this Type type, bool checkNullable = true)
	{
		if (checkNullable && type == typeof(Nullable<>).MakeGenericType(typeof(DateTime)))
		{
			return true;
		}
		if (type == typeof(DateTime))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	public static bool IsTimeSpanType(this Type type, bool checkNullable = true)
	{
		if (checkNullable && type == typeof(Nullable<>).MakeGenericType(typeof(TimeSpan)))
		{
			return true;
		}
		if (type == typeof(TimeSpan))
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <param name="checkNullable">
	public static bool IsSystemValueType(this Type type, bool checkNullable = true)
	{
		// Will use this array of types to check for nullable value and enum types
		var valueTypes = new Type[]
		{
			typeof(short),
			typeof(int),
			typeof(long),
			typeof(double),
			typeof(decimal),
			typeof(Single),
			typeof(ushort),
			typeof(uint),
			typeof(ulong),
			typeof(char),
			typeof(byte),
			typeof(sbyte),
			typeof(bool),
			typeof(Guid),
			typeof(DateTime),
			typeof(TimeSpan)
		};
		
		// Since Value types are immutable by default one needs to explicitly 
		// set the value type as nullable by adding '?', adding [NullableAttribute], 
		// or wrapping it Nullable<?>
		if (type.IsValueType || type.IsEnum)
		{
			return true;
		}
		
		// Since a string is mutable it is not nessarlity considered a
		// value type. Let's include it as a value type since it is a supported system type.
		if (type == typeof(string))
		{
			return true;
		}
		
		// Let's ensure that the type is not wrapped in the Nullable<> type class
		if (checkNullable)
		{
			foreach(var valueType in valueTypes)
			{
				if (type == typeof(Nullable<>).MakeGenericType(valueType))
				{
					return true;
				}
			}
		}
		
		return false;
	}

	/// <summary>
	/// Checks if Type is Dictionary Type
	/// </summary>
	/// <param name="type"></param>
	/// <param name="checkNullable">
	public static bool IsDictionaryType(this Type type)
	{
		var results = type.FindInterfaces((type, criteria) => type == typeof(IDictionary), null);
		
		if (results.Length > 0)
		{
			return true;
		}
		
		return false;
	}

	/// <summary>
	/// Creates a basic Deep Clone of an object. May need
	/// </summary>
	/// <param name="type"></param>
	/// <param name="checkNullable">
	private static object Clone(this object source)
	{
		// Get the type of source object and create a new instance of that type
		var sourceType = source.GetType();
		var instance = Activator.CreateInstance(sourceType);

		// Get all the properties of source object type that are readable and writable
		var properties = sourceType
			.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			.Where(prop => prop.CanRead && prop.CanWrite);

		// Assign all source property to taget object 's properties
		foreach (var property in properties)
		{
			// Check whether property type is value type, enum or string type
			if (property.PropertyType.IsSystemValueType())
			{
				property.SetValue(instance, property.GetValue(source, null), null);
			}
			// Assumption: property type is object/complex types, so need to recursively call this method until the end of the tree is reached
			else
			{
				var value = property.GetValue(source, null);
				if (value == null)
				{
					property.SetValue(instance, null, null);
				}
				else
				{
					property.SetValue(instance, value.Clone(), null);
				}
			}
		}

		return instance;
	}

	#endregion



	internal static class Cacher<TIn, TOut>
	{
		// Will hold reflection methods for specified types
		private static IDictionary<TIn, TOut> cache = new Dictionary<TIn, TOut>();
		
		/// <summary>
		/// Will use this method to cache requests for specified types
		/// </summary>
		public static Func<TIn, TOut> Memoize(Func<TIn, TOut> method)
		{
			return input => cache.TryGetValue(input, out var results) ?
				results :
				cache[input] = method(input);
		}
	}

	
}



#region private::Tests

#region private::Tests:Classes
public class User
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string MiddleName { get; set; }

	public short Int16Age { get; set; }
	public int Int32Age { get; set; }
	public long Int64Age { get; set; }
	
	public Status? UserStatus { get; set; }
	
	
	public int? AgeNullable { get; set; }
	
	public bool IsActive { get; set; }
	public bool? IsActiveNullable {get; set; }
	
	public UserDetail Details { get; set; }
	
	public IList<UserAddress> Addresses1 { get; set; }
	public IEnumerable<UserAddress> Addresses2 { get; set; }
	public ICollection<UserAddress> Addresses3 { get; set; }
	public IDictionary<int, UserAddress> Addresses4 { get; set; }
}
public class UserDetail
{
	public string Ssn { get; set; }
	public string PhoneNumber { get; set; }
	
	public short Int16Age { get; set; }
	public int Int32Age { get; set; }
	public long Int64Age { get; set; }
	
	public int? AgeNullable { get; set; }
}
public class UserAddress
{
	public Guid AddressId { get; set; }
	public string StreetA { get; set; }
}
public enum Status
{
	PaidUser,
	FreeUser
}
#endregion





[Fact]
void TestParentStringMerge()
{
	var user1 = new User() { FirstName = "Chase" };
	var user2 = new User() { LastName = "Crawford", MiddleName = "Ryan" };
	
	var user = Merger.Merge(user1, user2);
	
	Assert.Equal(user1.FirstName, user.FirstName);
	Assert.Equal(user2.LastName, user.LastName);
	Assert.Equal(user2.MiddleName, user.MiddleName);
}

[Fact]
void TestChildStringMerge()
{
	var user1 = new User() { Details = new UserDetail() { PhoneNumber = "555-555-5555" } };
	var user2 = new User() { Details = new UserDetail() { Ssn = "222-55-8888" } };
	
	var user = Merger.Merge(user1, user2);
	
	Assert.Equal(user1.Details.PhoneNumber, user.Details.PhoneNumber);
	Assert.Equal(user1.Details.Ssn, user.Details.Ssn);
	
}




[Fact]
void TestParentInt16Merge()
{
	var user1 = new User() { Int16Age = 24 };
	var user2 = new User() {  };

	var user = Merger.Merge(user1, user2);

	Assert.Equal(user1.FirstName, user.FirstName);
	Assert.Equal(user2.LastName, user.LastName);
	Assert.Equal(user2.MiddleName, user.MiddleName);
}

[Fact]
void TestChildInt16Merge()
{
	var user1 = new User() { Details = new UserDetail() { PhoneNumber = "555-555-5555" } };
	var user2 = new User() { Details = new UserDetail() { Ssn = "222-55-8888" } };

	var user = Merger.Merge(user1, user2);

	Assert.Equal(user1.Details.PhoneNumber, user.Details.PhoneNumber);
	Assert.Equal(user1.Details.Ssn, user.Details.Ssn);

}














#endregion