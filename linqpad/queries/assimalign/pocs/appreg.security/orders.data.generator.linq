<Query Kind="Program">
  <NuGetReference>Microsoft.Azure.Cosmos</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.Azure.Cosmos</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

async Task Main()
{
	var key = "";
	var endpoint = "";
	
	using(CosmosClient cosmosClient = new CosmosClient(endpoint, key))
	{
		//await cosmosClient.CreateDatabaseIfNotExistsAsync("Orders");
		var cosmosContainer = cosmosClient.GetContainer("Orders","SupplierOrders");
		
		
		var date = DateTime.Now.AddYears(-3);
		
		for(int a = 0; a < 5;a++)
		{
			for (int i = 0; i < 12; i++)
			{
				date = date.AddMonths(1);
				var orderEntries = new List<OrderEntryItem>();
				
				var order = new OrderItem()
				{
					OrderDate = date
				};
				
				
				foreach(var product in Products)
				{
					var productInfo = product.Split(';');
					
					var entry = new OrderEntryItem()
					{
						SupplierCode = productInfo[0],
						SupplierName = productInfo[1],
						ProductCode = productInfo[2],
						ProductName = productInfo[3],
						Measurement = productInfo[4],
						Quantity = long.Parse(productInfo[5]),
						Price = double.Parse(productInfo[6]),
						Currency = productInfo[7]
					};
					
					orderEntries.Add(entry);
				}
				
				order.Orders = orderEntries;
				order.OrderYear = $"{date.Year}";
		
				await cosmosContainer.CreateItemAsync<OrderItem>(order);
			}
		}
	}
}


public string[] Products { get; set; } =
{
	"A01;Steel Corp;FG;Steel Beam;UN;100;500.34;US",
	"A01;Steel Corp;FG;Steel Pike;UN;100;125.56;US",
	"A01;Steel Corp;FG;Steel Pipe;UN;100;250.89;US",
	"C03;Hose Corp;RM;Sewer Hose;Meter;1000;50.01;US",
	"C03;Hose Corp;RM;Faucet Hose;Meter;950;26.13;US",
	"E25;Tool Corp;RM;Nails;Unit;25;30.45;US",
	"E25;Tool Corp;RM;Screws;Unit;75;45.85;US",
	"E25;Tool Corp;RM;Bolts;Unit;80;80.76;US",
	"E25;Tool Corp;RM;Nuts;Unit;15;60.93;US",
};


public class OrderItem
{
	[JsonProperty("id")]
	public string Id { get; set; } = Guid.NewGuid().ToString();
	
	[JsonProperty("orderDate")]
	public DateTime OrderDate { get; set; }
	
	[JsonProperty("orderYear")]
	public string OrderYear { get; set; }
	
	[JsonProperty("orders")]
	public IEnumerable<OrderEntryItem> Orders { get; set; }
}

public class OrderEntryItem
{
	[JsonProperty("supplierCode")]
	public string SupplierCode { get; set; }
	
	[JsonProperty("supplierName")]
	public string SupplierName { get; set; }
	
	[JsonProperty("productCode")]
	public string ProductCode { get; set; }
	
	[JsonProperty("productName")]
	public string ProductName { get; set; }
	
	[JsonProperty("measurement")]
	public string Measurement { get; set; }
	
	[JsonProperty("quantity")]
	public long Quantity { get; set; }
	
	[JsonProperty("price")]
	public double Price { get; set; }
	
	[JsonProperty("currency")]
	public string Currency { get; set; }
	
}


// You can define other methods, fields, classes and namespaces here
