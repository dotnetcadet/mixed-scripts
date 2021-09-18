<Query Kind="Program">
  <NuGetReference>Azure.Messaging.ServiceBus</NuGetReference>
  <NuGetReference Prerelease="true">Eastdil.AspNetCore.Mvc.Razor</NuGetReference>
  <Namespace>Eastdil.AspNetCore.Mvc.Razor</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Xml.Serialization</Namespace>
  <Namespace>Azure.Messaging.ServiceBus</Namespace>
  <UseNoncollectibleLoadContext>true</UseNoncollectibleLoadContext>
</Query>







public string[] emails = new[] 
{
	"ccrawford@eastdilsecured.com"
	//"cjlee@eastdilsecured.com",
	//"cmuchler@eastdilsecured.com",
	//"droberts@eastdilsecured.com",
	//"mgambill@eastdilsecured.com",
	//"mlankes@eastdilsecured.com",
	//"smyron@eastdilsecured.com",
	//"jbasinger@eastdilsecured.com",
	//"abaker@eastdilsecured.com",
	//"amiller@eastdilsecured.com"
};


void Main()
{
	var content = GetXmlRazorContent().Dump();
	
	//Console.WriteLine("-------------------------------------------------------------------------------------");
	//var contentReplacement = ReplaceRazorSyntax(content, new
	//{
	//	Emails = emails,
	//	Subject = "LNL Test Email",
	//	From = "donotreply@eastdilsecured.tech",
	//	Body = "Test Email"
	//}).Dump();
	//
	//
	//Console.WriteLine("-------------------------------------------------------------------------------------");
	//var request = GetRequest(contentReplacement).Dump();
	//
	//
	//SendEmailAsync(request);
}


public string GetXmlRazorContent()
{
	var path = @"C:\Users\ccrawford\source\repos\personal\mixed-scripts\linqpad\queries\demos\notifications.engine\data\razor.markuparray.xml";
	using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
	{
		using (StreamReader reader = new StreamReader(stream))
		{
			return reader.ReadToEnd();
		}
	}
}

public string ReplaceRazorSyntax(string content, object data)
{
	// Create the Razor Engine to Compile and Run Razor Syntax
	var engine = RazorEngine.Create();

	// Build the Razor Syntax into Byte Code
	var template = engine.Build(content);

	// Write Token Replacement Output
	var output = template.Run(data);
	
	return output;
}

public MailRequest GetRequest(string xmldata)
{
	var serializer = new XmlSerializer(typeof(MailRequest));
	using (MemoryStream ms = new MemoryStream())
	{
		using (StreamWriter writer = new StreamWriter(ms, leaveOpen: true))
		{
			writer.Write(xmldata);
		}

		ms.Position = 0;

		return (MailRequest)serializer.Deserialize(ms);
	}
}

public async void SendEmailAsync(MailRequest request)
{
	var client = new ServiceBusClient("");
	var sender = client.CreateSender("sbq-notifications-email");
	
	await sender.SendMessageAsync(new ServiceBusMessage()
	{
		ContentType = "application/json",
		Body = BinaryData.FromBytes(JsonSerializer.SerializeToUtf8Bytes(request))
	});
}


[XmlRoot("email")]
public class MailRequest //: Entity
{
	/// <summary>
	/// 'ToSingle' will send the same email to each recipient separately. 
	/// </summary>
	[XmlArray("to-single")]
	[XmlArrayItem("email")]
	[JsonPropertyName("toSingle")]
	public virtual List<string> ToSingle { get; set; } = new List<string>();

	/// <summary>
	/// 'ToMultiple' will send one email to all recipients at one time. 
	/// </summary>
	[XmlArray("to-multiple")]
	[XmlArrayItem("email")]
	[JsonPropertyName("toMultiple")]
	public virtual List<string> ToMultiple { get; set; } = new List<string>();

	/// <summary>
	/// Carbon copies any recipients to all emails. (Optional)
	/// </summary>
	[XmlArray("bcc")]
	[XmlArrayItem("email")]
	[JsonPropertyName("bcc")]
	public virtual List<string> Bcc { get; set; } = new List<string>();

	/// <summary>
	/// Carbon Copied Field.
	/// </summary>
	[XmlArray("cc")]
	[XmlArrayItem("email")]
	[JsonPropertyName("cc")]
	public virtual List<string> Cc { get; set; } = new List<string>();

	/// <summary>
	/// Specifies the sender of the email.(Optional)
	/// </summary>
	/// <remarks>If no email is applied it default to noreplay email on server side.</remarks>
	[XmlElement("from")]
	[JsonPropertyName("from")]
	public virtual string From { get; set; } = string.Empty;

	/// <summary>
	/// Specifies the matter of the email. (Required)
	/// </summary>
	[XmlElement("subject")]
	[JsonPropertyName("subject")]
	public virtual string Subject { get; set; } = string.Empty;

	/// <summary>
	/// Specifies the content of the email. (Required)
	/// </summary>
	[XmlElement("body")]
	[JsonPropertyName("body")]
	public virtual string Body { get; set; } = string.Empty;

}
