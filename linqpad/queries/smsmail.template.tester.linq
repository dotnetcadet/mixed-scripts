<Query Kind="Program">
  <Namespace>System.Net.Mail</Namespace>
  <Namespace>System.Net.Mime</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	//var atach = new Attachment(,
	
	using(HttpClient client = new HttpClient())
	{
		var mailRequest = new MailRequest()
		{
			Body = "This is a test Email",
			Subject = "This is a Test Email",
			From = "ccrawford@assimalign.com"
		};
		
		mailRequest.ToSingle.Add("ccrawford@eastdilsecured.tech");
		
		mailRequest.Attachments.Add(MailAttachment.FromFilePath(@"C:\Users\ccrawford\Documents\MOCK_DATA.xlsx", MailAttachMediaTypes.XLSX));
		
		var content = JsonSerializer.Serialize(mailRequest);
		content.Dump();
		
		var httpRequestMessage = new HttpRequestMessage()
		{
			Method = HttpMethod.Post,
			RequestUri = new Uri("http://localhost:7073/api/Command/SmsMail/SendEmail"),
			Content = new StringContent(content, Encoding.UTF8,"application/json")
		};
		
		var httpResponseMessage = await client.SendAsync(httpRequestMessage);
		
		if(httpResponseMessage.IsSuccessStatusCode)
		{
			Console.WriteLine("Sent Successfully");
		}
		else
		{
			Console.WriteLine("Didn't send");
		}
	}
	
}


public sealed class MailAttachMediaTypes
{
	public const string Csv = "text/csv";
	public const string XlS = "application/msexcel";
	public const string XLSX =  "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
	public const string XLT = "application/msexcel";
	public const string XLTX = "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
	public const string XLA = "application/msexcel";
	public const string XLW = "application/msexcel";
	public const string XLSM = "application/vnd.ms-excel.sheet.macroEnabled.12";
	public const string XLSB = "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
	public const string XLTM = "application/vnd.ms-excel.template.macroEnabled.12";
	public const string XLAM = "application/vnd.ms-excel.addin.macroEnabled.12";
	public const string PPT = "application/mspowerpoint";
	public const string PPTX = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
	public const string POT = "application/mspowerpoint";
	public const string POTX = "application/vnd.openxmlformats-officedocument.presentationml.template";
	public const string PPS = "application/mspowerpoint";
	public const string PPSX = "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
	public const string PPA = "application/mspowerpoint";
	public const string PPAM = "application/vnd.ms-powerpoint.addin.macroEnabled.12";
	public const string PPTM = "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
	public const string PPSM = "application/vnd.ms-powerpoint.slideshow.macroEnabled.12";
	public const string POTM = "application/vnd.ms-powerpoint.template.macroEnabled.12";
	public const string MDB = "application/msaccess";
	public const string ACCDA = "application/msaccess";
	public const string ACCDB = "application/msaccess";
	public const string ACCDE = "application/msaccess";
	public const string ACCDR = "application/msaccess";
	public const string ACCDT = "application/msaccess";
	public const string ADE = "application/msaccess";
	public const string ADP = "application/msaccess";
	public const string ADN = "application/msaccess";
	public const string MDE = "application/msaccess";
	public const string MDF = "application/msaccess";
	public const string MDN = "application/msaccess";
	public const string MDT = "application/msaccess";
	public const string MDW = "application/msaccess";
}



public class MailAttachment
{
	// Should not be sending any attachment over 4GBs
	private const long attachmentMaxSize = 4000000000;
	private readonly Encoding encoding = Encoding.UTF8;
	
	public string Name { get; set; }
	
	public string MediaType { get; set; }
	
	public byte[] Content { get; set; }
	
	public bool IsBase64Encoded { get; set; } = false;
	
	
	
	public static MailAttachment FromFilePath(string path, string mediaType)
	{
		if (!IsMediaTypeSupported(mediaType))
			throw new Exception($"Media Type {mediaType} is not supported.");

		var attachment = new MailAttachment()
		{
			MediaType = mediaType,
			Name = Path.GetFileName(path),
			IsBase64Encoded = true
		};

		using(FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
		{
			if(stream.Length > attachmentMaxSize)
				throw new Exception("File");
			
			int counter = 0;
			attachment.Content = new byte[stream.Length];
			stream.Position = 0;
			
			while(stream.Position < stream.Length)
			{
				var offset = stream.Length - stream.Position;
				
				if(offset > 1000000000)
					stream.Read(attachment.Content, counter * 1000000000, 1000000000);
				else 
					stream.Read(attachment.Content, counter * 1000000000, (int)offset);
				counter++;
			}		
		}
		
		return attachment;
	}
	
	public static MailAttachment FromStream(Stream stream, string attachmentName, string mediaType, bool isBase64Encoded = false)
	{
		if (stream.Length > attachmentMaxSize)
			throw new Exception($"Stream Length is over max size limit of 4GB ({attachmentMaxSize})");

		if (!IsMediaTypeSupported(mediaType))
			throw new Exception($"Media Type {mediaType} is not supported.");

		var attachment = new MailAttachment()
		{
		
			Name = attachmentName,
			IsBase64Encoded = isBase64Encoded
		};

		int counter = 0;
		attachment.Content = new byte[stream.Length];
		stream.Position = 0;

		while (stream.Position < stream.Length)
		{
			var offset = stream.Length - stream.Position;

			if (offset > 1000000000)
				stream.Read(attachment.Content, counter * 1000000000, 1000000000);
			else
				stream.Read(attachment.Content, counter * 1000000000, (int)offset);
			counter++;
		}

		return attachment;
	}

	internal static bool IsMediaTypeSupported(string mediaType)
	{
		var mediaTypes = new MailAttachMediaTypes();
		foreach(var field in typeof(MailAttachMediaTypes).GetFields())
		{
			var value = (string)field.GetValue(mediaTypes);
			
			if(value.ToLower() == mediaType)
				return true;
		}
		
		return false;
	}
	
	internal ContentType GetContentType()
	{
		return new ContentType()
		{
			MediaType = MediaType,
			Name = Name,
			CharSet = "UTF-8"
		};
	}
	
	internal Stream GetBase64Stream()
	{
		if(IsBase64Encoded)
			return new MemoryStream(Content);
			
		var base64 = encoding.GetBytes(Convert.ToBase64String(Content));

		return new MemoryStream(base64);
	}
}

public class MailRequest
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
	/// Carbon Copied Field.
	/// </summary>
	public virtual List<string> Cc { get; set; } = new List<string>();

	/// <summary>
	/// Specifies the sender of the email.(Required)
	/// </summary>
	public virtual string From { get; set; } = string.Empty;

	/// <summary>
	/// Specifies the matter of the email. (Required)
	/// </summary>
	public virtual string Subject { get; set; } = string.Empty;

	/// <summary>
	/// Specifies the content of the email. (Required)
	/// </summary>
	public virtual string Body { get; set; } = string.Empty;

	/// <summary>
	/// Specifies whether the email content is Html.
	/// </summary>
	public virtual bool IsBodyHtml { get; set; } = false;


	/// <summary>
	/// 
	/// </summary>
	public List<MailAttachment> Attachments { get; set; } = new List<MailAttachment>();

}






