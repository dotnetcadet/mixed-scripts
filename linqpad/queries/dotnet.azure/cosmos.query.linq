<Query Kind="Program">
  <NuGetReference>Microsoft.Azure.Cosmos</NuGetReference>
  <Namespace>Microsoft.Azure.Cosmos</Namespace>
  <Namespace>Microsoft.Azure.Cosmos.Linq</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>



private string _connection = "";


async Task Main()
{
	var tasks = new List<Task<ItemResponse<object>>>();
	var client = new CosmosClient(_connection, new CosmosClientOptions()
	{
		AllowBulkExecution = true,
		SerializerOptions = new CosmosSerializationOptions()
		{
			IgnoreNullValues = true,
			PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
		}
	});

	var container1 = client.GetContainer("", "");
	var container2 = client.GetContainer("", "");
	long pageNumber = 0;
	while (true)
	{
		using (var iterator = container1.GetItemQueryIterator<object>($"select * From c Offset {pageNumber * 500} Limit 500"))
		{
			while (iterator.HasMoreResults)
			{
				var items = await iterator.ReadNextAsync();

				foreach (var item in items)
				{
					try
					{
						tasks.Add(container2.UpsertItemAsync<object>(item));

						if (tasks.Count >= 500)
						{
							var results = await Task.WhenAll(tasks);

							var failures = results
								.Where(x => ((int)x.StatusCode) < 200 || ((int)x.StatusCode) > 299)
								?.Count();

							if (failures is not null)
							{
								failures.Dump();
							}

							tasks.Clear();
						}
					}
					catch (Exception exception)
					{
						continue;
					}
				}
			}
		}
		pageNumber++;
	}
	


}






