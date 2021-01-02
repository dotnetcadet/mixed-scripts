<Query Kind="Program" />



void Main()
{
	var samples = new Sample(){
		Name = "A",
		Samples = new[] 
		{
			new Sample(){
				Name = "A.A",
				Samples = new[]
				{
					new Sample()
					{
						Name = "A.A.A",
						Samples = new[]
						{
							new Sample()
							{
								Name = "A.A.A.A",
								Samples = new[]
								{
									new Sample()
									{
										Name = "A.A.A.A.A"
									}
								}
							}
						}
					}
				}
			},
			new Sample(){
				Name = "A.B",
				Samples = new[]
				{
					new Sample()
					{
						Name = "A.B.C",
					}
				}
			}
		}
	};
	
	//Console.WriteLine(samples.Samples.Count());
	
	
	RecursionDelegates.SelfLoop(samples, action =>
	{
		var currentName = action.Name;
		
		action.Name = "H" + action.Name.Substring(1,action.Name.Length - 1);


		Console.WriteLine($"Old Name: {currentName}");
		Console.WriteLine($"New Name: {action.Name}");
		Console.WriteLine();
	});
}


public static class RecursionDelegates
{
	/// <summary>Allows you to apply logic to Self embedded IEnumerables</summary>
	public static void SelfLoop<TObject>(TObject customType, Action<TObject> action) where TObject : class, new()
	{
		var properties = typeof(TObject).GetProperties();
		
		foreach(var property in properties)
		{		
			if (property.PropertyType == typeof(IEnumerable<TObject>))
			{
				var currentValue = (IEnumerable<TObject>)property.GetValue(customType);
				
				if(null != currentValue)
				{
					foreach (var childObject in currentValue)
					{
						SelfLoop<TObject>(childObject, action);
					}
				}
			}
			else
			{
				action(customType);
			}
		}
	}
}


public class Sample
{
	public string Name { get; set; }
	
	public IEnumerable<Sample> Samples { get; set; }
}

