<Query Kind="Program">
  <NuGetReference>Microsoft.Azure.Cosmos</NuGetReference>
  <Namespace>Microsoft.Azure.Cosmos</Namespace>
  <Namespace>Microsoft.Azure.Cosmos.Linq</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>


private string _database = "ErpCore";
private string _container = "Employee";
private string _connection = "";


async Task Main()
{
	var client = new CosmosClient(_connection, new CosmosClientOptions()
	{
		SerializerOptions = new CosmosSerializationOptions()
		{
			IgnoreNullValues = true,
			PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
		}
	});
	
	var container = client.GetContainer(_database, _container);
	
	var queryable = container.GetItemLinqQueryable<Employee>()
		.Select( x => new 
		{
			FirstName = x.FirstName,
			Details = new 
			{
				Age = x.Details.Age,
				Addresses = x.Details.Addresses.Select(b=> new 
				{
					b.Street	
				})
			}	
		});
		
	var def = queryable.ToQueryDefinition();
		
	using (var iterator = queryable.ToFeedIterator())
	{
		var results = await iterator.ReadNextAsync();
		results.Resource.Dump();
	}
}




public class Employee
{
	public string Id
	{
		get => EmployeeId.ToString();
		set => EmployeeId = Guid.Parse(value);
	}

	public string YearStarted { get; set; } = DateTime.Now.Year.ToString();
	public Guid EmployeeId { get; set; } = Guid.NewGuid();
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public DateTime Birthdate { get; set; }
	public EmployeeDetails Details { get; set; }
	public IEnumerable<EmployeeAddress> Addresses { get; set; }
	
}

public class EmployeeDetails
{
	public string Ssn { get; set; }
	public string Age { get; set; }
	public IEnumerable<EmployeeAddress> Addresses { get; set; }
	
}

public class EmployeeAddress
{
	public string Street { get; set; }
	public string Apt { get; set; }
	public string Unit { get; set; }
	public string City { get; set; }
	public string StateOrRegion { get; set; }
	public string County { get; set; }
	public string Country { get; set; }
	public string CountryCode { get; set; }
}












