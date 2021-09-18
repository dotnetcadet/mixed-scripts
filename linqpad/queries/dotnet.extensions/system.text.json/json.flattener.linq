<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

void Main()
{
	var tokens = new Dictionary<string, object>();
	var content = new Parent
	{
		FirstName = "Chase",
		LastName = "Crawford",
		Child = new Child
		{
			Birthdate = "1996-11-21",
			SubChild = new SubChild
			{
				Username = "ccrawford@assimalign.com"
			}
		}
	};
	
	tokens.Add("parent",content);
	var obj = JsonSerializer.Deserialize<IDictionary<string,object>>(JsonSerializer.Serialize(tokens));
	
	var results = FlattenJson(obj);
	results.Dump();
	
}


public static partial class JsonSerializer 
{
	public IDictionary<string, string> Flatten(object @object, string token = null)
	{
		
	}
	
	public IDictionary<string, string> Flatten(IDictionary<string, object> objectCollection)
	{
	
	}
}


private IDictionary<string, string> FlattenJson(IDictionary<string, object> objectPairs)
{
	IDictionary<string, string> tokens = new Dictionary<string, string>();
	
	foreach (var pair in objectPairs)
	{
		if (pair.Value is JsonElement)
		{
			var jsonElement = (JsonElement)pair.Value;
			
			if(jsonElement.ValueKind == JsonValueKind.Object)
			{
				foreach (var jsonProperty in jsonElement.EnumerateObject())
				{
					var propertyValue = (JsonElement)jsonProperty.Value;

					switch (propertyValue.ValueKind)
					{
						case JsonValueKind.Number:
							tokens.Add($"$({pair.Key.ToLower()}:{jsonProperty.Name.ToLower()})", propertyValue.GetInt64().ToString());
							break;

						case JsonValueKind.False:
							tokens.Add($"$({pair.Key.ToLower()}:{jsonProperty.Name.ToLower()})", propertyValue.GetBoolean().ToString());
							break;

						case JsonValueKind.True:
							tokens.Add($"$({pair.Key.ToLower()}:{jsonProperty.Name.ToLower()})", propertyValue.GetBoolean().ToString());
							break;

						case JsonValueKind.String:
							tokens.Add($"$({pair.Key.ToLower()}:{jsonProperty.Name.ToLower()})", propertyValue.GetString());
							break;

						case JsonValueKind.Object:
							MergeTokenDictionaries(ref tokens, FlattenChildDictionaryJObjects(propertyValue.EnumerateObject(), $"{pair.Key}:{jsonProperty.Name}"));
							break;
					}
				}
			}
		}
	}

	return tokens;
}


private IDictionary<string, string> FlattenChildDictionaryJObjects(JsonElement.ObjectEnumerator jsonProperties, string parentKey)
{
	IDictionary<string, string> tokens = new Dictionary<string, string>();
	
	foreach(var jsonProperty in jsonProperties)
	{
		var propertyValue = (JsonElement)jsonProperty.Value;

		switch (propertyValue.ValueKind)
		{
			case JsonValueKind.Number:
				tokens.Add($"$({parentKey.ToLower()}:{jsonProperty.Name.ToLower()})", propertyValue.GetInt64().ToString());
				break;

			case JsonValueKind.False:
				tokens.Add($"$({parentKey.ToLower()}:{jsonProperty.Name.ToLower()})", propertyValue.GetBoolean().ToString());
				break;

			case JsonValueKind.True:
				tokens.Add($"$({parentKey.ToLower()}:{jsonProperty.Name.ToLower()})", propertyValue.GetBoolean().ToString());
				break;

			case JsonValueKind.String:
				tokens.Add($"$({parentKey.ToLower()}:{jsonProperty.Name.ToLower()})", propertyValue.GetString());
				break;

			case JsonValueKind.Object:
				MergeTokenDictionaries(ref tokens, FlattenChildDictionaryJObjects(propertyValue.EnumerateObject(), $"{parentKey}:{jsonProperty.Name}"));
				break;
		}
	}
	
	return tokens;
}


private void MergeTokenDictionaries(ref IDictionary<string, string> parent, IDictionary<string, string> child)
{
	foreach (var value in child)
	{
		parent.Add(value.Key, value.Value);
	}
}



public class Parent
{
	public string FirstName { get; set; }

	public string LastName { get; set; }
	
	public Child Child { get; set; }
	
	public bool Active { get; set; } = true;
}

public class Child 
{
	public string Birthdate { get; set; }
	
	public SubChild SubChild { get; set; }
	
	public int Age { get; set; } = 24;
	
}

public class SubChild
{
	public string Username { get; set; }
}


//private IDictionary<string, string> ParseChildTokenValues(JToken jTokens, string previousKey = "")
//{
//	string tokenKey = previousKey;
//	var tokenReplacements = new Dictionary<string, string>();
//
//	foreach (var jToken in jTokens)
//	{
//		if (jToken.Type == JTokenType.Property)
//		{
//			var jProperty = jToken.ToObject<JProperty>();
//			tokenKey = tokenKey == string.Empty ? tokenKey : $"{tokenKey}:{jProperty.Name}";
//
//			if (jProperty.Value.Type == JTokenType.Object)
//			{
//				foreach (var jChildToken in jToken)
//					foreach (var token in ParseChildTokenValues(jChildToken, tokenKey))
//						tokenReplacements.Add(token.Key, token.Value);
//
//				var keys = tokenKey.Split(':');
//				tokenKey = string.Join(':', keys.Take(keys.Length - 1));
//			}
//			else
//			{
//				tokenReplacements.Add($"$({tokenKey})", jProperty.Value.ToString());
//				tokenKey = string.Join(':', tokenKey.Split(':').Take(tokenKey.Split(':').Length - 1));
//			}
//		}
//	}
//	return tokenReplacements;
//}
//
//
//private IDictionary<string, string> ParseTokenValues(IDictionary<string, object> tokenValues)
//{
//	var replacementTokens = new Dictionary<string, string>();
//
//	foreach (var tokenValue in tokenValues)
//	{
//		string tokenKey = "";
//		var tokens = new Dictionary<string, string>();
//		var jobject = JObject.FromObject(tokenValue);
//
//		foreach (var jtoken in jobject)
//		{
//			if (jtoken.Key == "Key")
//				tokenKey = jtoken.Value.ToString();
//
//			else if (jtoken.Key == "Value" && jtoken.Value.Type == JTokenType.Object)
//				foreach (var jchildToken in ParseChildTokenValues(jtoken.Value, tokenKey))
//					tokens.Add(jchildToken.Key, jchildToken.Value);
//
//			else
//			{
//				tokens.Add($"$({tokenKey})", jtoken.Value.ToString());
//				tokenKey = string.Empty;
//			}
//		}
//
//		foreach (var replacement in tokens)
//			replacementTokens.Add(replacement.Key, replacement.Value);
//	}
//
//	return replacementTokens;
//}

