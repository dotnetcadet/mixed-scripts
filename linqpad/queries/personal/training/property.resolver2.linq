<Query Kind="Program">
  <Namespace>System.Text.Json</Namespace>
</Query>

static void Main()
{
	var template = "";
	JsonUtility.BuildTokenCollection(json).ReplaceTokens(ref template);
	
	
	
}


static string json = @"{
    ""name"": ""test"",
    ""father"": {
            ""name"": ""test2"", 
         ""age"": 13,
         ""dog"": {
                ""color"": ""brown""
         }
        },
	""array"": [
		{
			""test"": ""value""
		}
	]
    }";




public static class JsonUtility
{
	private static string _tokenPattern = "$({0})";
	
	public static IDictionary<string, JsonElement> BuildTokenCollection(string json)
	{
		IEnumerable<(string Path, JsonProperty Property)> GetLeaves(string path, JsonProperty property)
			=> property.Value.ValueKind != JsonValueKind.Object
				? new[] { (Path: path == null ? property.Name : path + "." + property.Name, property) }
				: property.Value.EnumerateObject().SelectMany(child => GetLeaves(path == null ? property.Name : path + "." + property.Name, child));

		using (JsonDocument document = JsonDocument.Parse(json)) // Optional JsonDocumentOptions options
			return document.RootElement.EnumerateObject()
				.SelectMany(p => GetLeaves(null, p))
				.ToDictionary(k => k.Path, v => v.Property.Value.Clone()); //Clone so that we can use the values outside of using
	}


	public static void  ReplaceTokens(this IDictionary<string, JsonElement> elements, ref string template)
	{
		foreach(var element in elements)
		{
			var token = string.Format(_tokenPattern, element.Key);
			var kind = element.Value.ValueKind;
			
			if (kind == JsonValueKind.False || kind == JsonValueKind.True)
			{
				template.Replace(token, element.Value.GetBoolean().ToString());
			}
			else if (kind == JsonValueKind.Number)
			{
				template.Replace(token, element.Value.GetInt64().ToString());
			}
			
		}
	}
}





