<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
</Query>

void Main()
{


	var mailRequest = JsonConvert.DeserializeObject<MailRequestTemplate>(json);
	
	
	foreach(var token in mailRequest.Tokens)
	{
		string Key = "";
		var jobject = JObject.FromObject(token);
		
		foreach(var tokenReplacement in GetTokenValues(jobject))
		{
			Console.WriteLine($"{tokenReplacement.Key} : {tokenReplacement.Value}");
		}
		
		
		
	    // var tokens = GetTokenValues(jobject);
		//jobject.Dump();
		//
		//foreach(var jtoken in jobject)
		//{
		//	if(jtoken.Value.Type == JTokenType.Object)
		//	{
		//		foreach(var jchild in jtoken.Value)
		//		{
		//			if(jchild.HasValues)
		//			{
		//				foreach(var jchila in jchild)
		//				{
		//					if(jchila.Type == JTokenType.Object)
		//					{
		//						foreach(var v in jchila)
		//						{
		//							v.ToString().Dump();
		//						}
		//					}
		//					
		//				}
		//			}
		//		}
		//	}
		//}
		
		//ValidateTokenValues(jobject);	
	}
}



public IDictionary<string, string> GetChildTokenValues(JToken jtokens, string previousKey = "")
{
	string key = previousKey;
	var tokens = new Dictionary<string, string>();
	
	foreach(var jtoken in jtokens)
	{
		if(jtoken.Type == JTokenType.Property)
		{
			var jproperty = jtoken.ToObject<JProperty>();
			key = key == string.Empty ? key : $"{key}:{jproperty.Name.Substring(1, jproperty.Name.Length - 1)}";
			
			if (jproperty.Value.Type == JTokenType.Object)
			{
				foreach (var jchildProperty in jtoken)
				{					
					foreach(var token in GetChildTokenValues(jchildProperty, key))
					{
						tokens.Add(token.Key, token.Value);	
					}
				}
				var keys = key.Split(':');
				key = string.Join(':', keys.Take(keys.Length - 1));
			}
			else
			{
				tokens.Add($"$({key})", jproperty.Value.ToString());
				var keys = key.Split(':');
				key = string.Join(':', keys.Take(keys.Length -1));
	
			}
		}
	}
	
	return tokens;
}


private IDictionary<string, string> GetTokenValues(IDictionary<string, object> tokenValues)
{
	var tokens = new Dictionary<string, string>();
	
	foreach(var tokenValue in tokenValues)
	{
		string key = "";
		var tokens = new Dictionary<string, string>();
		var jobject = JObject.FromObject(tokenValue);

		foreach (var jtoken in jobject)
		{
			if (jtoken.Key == "Key")
				key = jtoken.Value.ToString().Substring(1, jtoken.Value.ToString().Length - 1);

			else if (jtoken.Key == "Value" && jtoken.Value.Type == JTokenType.Object)
			{
				foreach (var jchildToken in GetChildTokenValues(jtoken.Value, key))
				{
					tokens.Add(jchildToken.Key, jchildToken.Value);
				}
			}
			else
			{
				tokens.Add($"$({key})", jtoken.Value.ToString());
				key = string.Empty;
			}
		}
	}

	return tokens;
}


private void GetSubTokenValues(JToken jtokens)
{
	string Key = "";
	var childTokens = new Dictionary<string,string>();
	
	foreach (var jtoken in jtokens)
	{
		if (jtoken.Type == JTokenType.Property)
		{
			var jproperty = jtoken.ToObject<JProperty>();
			var key = jproperty.Name;

			if (jproperty.Value.Type == JTokenType.Object)
			{
				foreach (var jchildProperty in jtoken)
				{
					ValidateSubTokenValues(jchildProperty);
				}
			}
			else
			{
				jtoken.Path.Dump();
			}
		}
	}
}



private void ValidateSubTokenValues(JToken jtokens)
{
	foreach(var jtoken in jtokens)
	{
		if(jtoken.Type == JTokenType.Property)
		{
			var jproperty = jtoken.ToObject<JProperty>();
			
			if(jproperty.Value.Type == JTokenType.Object)
			{
				foreach(var jchildProperty in jtoken)
				{
					ValidateSubTokenValues(jchildProperty);
				}
			}
			else
			{
				if(!jproperty.Name.StartsWith('@'))
					Console.WriteLine($"There was a failure with: {jproperty.Name}");
				//jproperty.Name.Dump();
				//jproperty.Value.Dump();
			}
		}
	}
}


 private void ValidateTokenValues(JObject jobject)
 {
	foreach(var jtoken in jobject)
	{
		if(jtoken.Key == "Key" && !jtoken.Value.ToString().StartsWith('@'))
		{
			Console.WriteLine($"There was a failure with: {jtoken.Value.ToString()}");
		}
		else if(jtoken.Key == "Value" && jtoken.Value.Type == JTokenType.Object)
		{
			ValidateSubTokenValues(jtoken.Value);
		}
	}
 }


string json = @"
	{
    ToSingle: [

		'ccrawford@eastdilsecured.tech',
        'ccrawford@assimalign.com',
        'chasecrawford2018@gmail.com'
    ],
    ToMultiple: [

		'ccrawford@eastdilsecured.tech',
        'ccrawford@assimalign.com',
        'chasecrawford2018@gmail.com'
    ],
    Bcc: [


	],
    'From': 'noreply@eastdilsecured.tech',
    'Subject': 'This is a Test Email',
    'Container': 'temp',
    'File': 'templates/sample-template.htm',
    Tokens: {
		'@title': 'Test Title',
		'@event': 'Party',
        '@my': {
			'@name': 'Chase Crawford',
            '@age': 23,
            '@birthdate': '1996-11-21',
			'@info': {
				'@key1': 'value 1',
				'@key2': 'value 2',
				'@key3': {
					'@subKey1': 'Sub value 1',
					'@subKey2': 'Sub value 2'
				}
			}
		}
	}
}
";



public class MailRequestTemplate
{
	/// <summary>
	/// 'ToSingle' will send the same email to each recipient seperately. 
	/// </summary>
	public virtual List<string> ToSingle { get; set; } = new List<string>();

	/// <summary>
	/// 'ToMultiple' will send one email to all recipients at one time. 
	/// </summary>
	public virtual List<string> ToMultiple { get; set; } = new List<string>();

	/// <summary>
	/// Carbon copies any recipients to all emails. (Optional)
	/// </summary>
	public virtual List<string> Bcc { get; set; } = new List<string>();

	/// <summary>
	/// Specifies the sender of the email.(Required)
	/// </summary>
	public virtual string From { get; set; } = string.Empty;

	/// <summary>
	/// 
	/// </summary>
	public virtual bool IsTemplateHtml { get; set; } = true;

	/// <summary>
	/// Specifies the matter of the email. (Required)
	/// </summary>
	public virtual string Subject { get; set; } = string.Empty;

	/// <summary>
	/// 
	/// </summary>
	public virtual string Container { get; set; }

	/// <summary>
	/// 
	/// </summary>
	public virtual string File { get; set; }

	/// <summary>
	/// Employee:Value1
	/// </summary>
	public virtual IDictionary<string, object> Tokens { get; set; }
}
