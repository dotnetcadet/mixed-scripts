<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Text.Json</Namespace>
</Query>



public const string json = "[{\"widget\": {\"property\": \"test\" }}, {\"widget\": {\"property\": \"test\" }}]";
public const string Json = "{\"widget\": {\"debug\": \"on\",\"window\": {\"title\": \"Sample Konfabulator Widget\",\"name\": \"main_window\",\"width\": 500,\"height\": 500},\"image\": {\"src\": \"Images/Sun.png\",\"name\": \"sun1\",\"hOffset\": 250,\"vOffset\": 250,\"alignment\": \"center\"},\"text\": {\"data\": \"Click Here\",\"size\": 36,\"style\": \"bold\",\"name\": \"text1\",\"hOffset\": 250,\"vOffset\": 100,\"alignment\": \"center\",\"onMouseUp\": \"sun1.opacity = (sun1.opacity / 100) * 90;\"}}}";
public JObject JData = JObject.Parse(Json);


void Main()
{

	var data = JsonObject.Parse(json);
	
	var token = data["widget"];
	
	Console.Write(token);
	
	
}

public void Parse(string path)
{
	using(JsonDocument document = JsonDocument.Parse(Json))
	{
		var root = document.RootElement;
		foreach (var element in path.Split(':'))
		{
			root.GetProperty(element);
		}
	}
}

public object GetJsonObject(string path)
{
	JToken jtoken = null;
	foreach(var token in path.Split(':'))
	{
		jtoken = JData[token];
	}
	if (jtoken.Type == JTokenType.Array)
	{
		
	}
	
	return null;
}

public sealed class JsonObject : IDisposable, IFormattable
{
	private readonly JsonDocument _document;
	
	
	protected JsonObject() { }
	
	internal JsonObject(JsonDocument document)
	{
		_document = document;
	}
	
	/// <summary>
	/// </summary>
	public JsonObject this[string path]
	{
		get
		{
			if (_document.RootElement.ValueKind == JsonValueKind.Array)
			{
				var elements = new List<JsonElement>();
				foreach (var jobject in _document.RootElement.EnumerateArray())
				{
					if (!jobject.ValueKind.Equals(JsonValueKind.Object))
						continue;
						
					foreach (var jelement in jobject.EnumerateObject())
					{
						if (jelement.Name.Equals(path, StringComparison.InvariantCultureIgnoreCase))
						{
							elements.Add(jelement.Value);
						}
					}
				}
				if (elements.Count > 0)
				{
					return this.New(elements);
				}
				return null;
			}
			else if (_document.RootElement.ValueKind == JsonValueKind.Object)
			{
				foreach (var jelement in _document.RootElement.EnumerateObject())
				{
					if (jelement.Name.Equals(path, StringComparison.InvariantCultureIgnoreCase))
					{
						return this.New(jelement.Value);
					}
				}
				return null;
			}
			else
			{
				return null;
			}
		}
	}
	
	
	public T CastAs<T>()
	{
		var type = typeof(T);
		if (type.IsValueType)
		{
			
		}
		if (type.IsGenericType)
		{
			
		}
		
		return default;
	}
	
	//public object GetObject

	public void Dispose()
	{
		_document.Dispose();
	}

	public static JsonObject Parse(string json)
	{
		var document = JsonDocument.Parse(json, new JsonDocumentOptions()
		{
			CommentHandling = JsonCommentHandling.Skip,
			AllowTrailingCommas = true,
		});
		return new JsonObject(document);
	}

	private JsonObject New(JsonElement element)
	{
		using (MemoryStream stream = new MemoryStream())
		{
			using (Utf8JsonWriter writer = new Utf8JsonWriter(stream))
			{
				element.WriteTo(writer);
			}
			if (stream.Length > 0)
			{
				stream.Position = 0;
				return new JsonObject(JsonDocument.Parse(stream));
			}
			else
			{
				return null;
			}
		}
	}
	private JsonObject New(IEnumerable<JsonElement> elements)
	{
		using (MemoryStream stream = new MemoryStream())
		{
			using (Utf8JsonWriter writer = new Utf8JsonWriter(stream))
			{
				writer.WriteStartArray();
				foreach (var element in elements)
				{
					element.WriteTo(writer);
				}
				writer.WriteEndArray();
			}
			if (stream.Length > 0)
			{
				stream.Position = 0;
				return new JsonObject(JsonDocument.Parse(stream));
			}
			else
			{
				return null;
			}
		}
	}

	public override string ToString()
	{
		using (MemoryStream stream = new MemoryStream())
		{
			using (Utf8JsonWriter writer = new Utf8JsonWriter(stream))
			{
				_document.WriteTo(writer);
			}
			if (stream.Length > 0)
			{
				stream.Position = 0;

				using (StreamReader reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
			else
			{
				return null;
			}
		}
	}
	public string ToString(string format, IFormatProvider formatProvider) =>
		this.ToString();
}

