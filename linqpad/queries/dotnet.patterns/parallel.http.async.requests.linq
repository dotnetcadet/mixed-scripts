<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>


public HttpClient Client = new HttpClient();
public IList<GithubUser> Users = new List<GithubUser>();

async Task Main()
{
	Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
	Client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
	
	var url = "https://api.github.com/users/{0}";
	var usernames = new string[] 
	{
		"chasec2018",
		"ccrawford-assimalign",
		"mgambill"
	};
	
	var tasks = new List<Task>();
	foreach(var username in usernames)
	{
		tasks.Add(Task.Run(async () =>
		{
			var uri = new Uri(string.Format(url,username));
			var httpRequest = new HttpRequestMessage()
			{
				Method = HttpMethod.Get,
				RequestUri = uri
			};

			var response = await Client.SendAsync(httpRequest);
			
			if(response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				Users.Add(JsonSerializer.Deserialize<GithubUser>(content, new JsonSerializerOptions()
				{
					PropertyNameCaseInsensitive = true
				}));
			}
		}));
	}
	
	await Task.WhenAll(tasks);
	Users.Dump();
}


public class GithubUser
{
	[JsonPropertyName("id")]
	public long Id { get; set; }
	
	[JsonPropertyName("login")]
	public string Login { get; set; }
	
	[JsonPropertyName("avatar_url")]
	public string Aavatar { get; set; }
	
	[JsonPropertyName("followers")]
	public int Followers { get; set; }
}