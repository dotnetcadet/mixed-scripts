<Query Kind="Program">
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
</Query>

void Main()
{
	var setOne = new List<User>()
	{
		new User {UserId = 0, FirstName = "Chase", LastName = "J"},
		new User {UserId = 0, FirstName = "Chase", LastName = "Crawford"},
		new User {UserId = 3, FirstName = "Chase", LastName = "ji"},
		new User {UserId = 1, FirstName = "Charles", LastName = "Bowers"},
		new User {UserId = 2, FirstName = "John"}
	};


	var setTwo = new List<User>()
	{
		new User {UserId = 0, FirstName = "Chase", LastName = "Crawford"},
		new User {UserId = 0, FirstName = "Chase", LastName = "Crawford"},
		new User {UserId = 1, FirstName = "Charles", LastName = "Bowers"},
		new User {UserId = 2, LastName = "Basinger"}
	};

	
	
	
	var combiner = Combiner<User>.Where((x,y) => x.FirstName == y.FirstName &&  x.UserId == y.UserId);
	combiner.Merge(setOne, setTwo).Dump();
	
}


public class User 
{
	public int UserId { get; set; }
	
	public string FirstName { get; set; }
	
	public string LastName { get; set; }
}


public class Combiner<TData> : IEqualityComparer<TData>
{
	private string PropertyKey;
	private Func<TData,TData,bool> Compare;
	
	protected Combiner() { }
	
	private Combiner(Func<TData,TData,bool> compare, string propertyKey)
	{
		Compare = compare;
		PropertyKey = propertyKey;
	}
		
	public static Combiner<TData> Where(Expression<Func<TData,TData,bool>> comparison)
	{
		var propertyKey = string.Empty;
		
		if (comparison is LambdaExpression)
		{
			var expression = comparison.Body as BinaryExpression;
			
			if (expression.Left.NodeType == ExpressionType.MemberAccess)
			{
				var propertyInfo = ((MemberExpression)expression.Left).Member as PropertyInfo;
				propertyKey = propertyInfo.Name; 
			}
		}
		
		return new Combiner<TData>(comparison.Compile(), propertyKey);
	}
		
	public IEnumerable<TData> Merge(IEnumerable<TData> target, IEnumerable<TData> source)
	{		
		return target.Union(source, this);	
	}

	public void Merge(ref IEnumerable<TData> target, IEnumerable<TData> source)
	{
		target.Union(source, this);
	}

	public static TData Merge(TData target, TData source)
	{
		var type = typeof(TData);

		foreach (var property in type.GetProperties())
		{
			if (property.CanRead && property.CanWrite)
			{
				var value = property.GetValue(source, null);

				if (value != null)
					property.SetValue(target, value);
			}
		}
		
		return target;
	}


	public bool Equals(TData? left, TData? right)
	{
		if(left == null || right == null)
			return false;
		
		return Compare.Invoke(left, right);
	}
		
	public int GetHashCode(TData obj)
	{
		var code = 0;
		
		if(obj != null)
		{
			var property = typeof(TData).GetProperty(PropertyKey);
			
			if(property?.PropertyType == null)
				return code;

			if (property?.PropertyType == typeof(int))
			{
				return (int)property.GetValue(obj);
			}

			code = HashCode.Combine<string>((string)property.GetValue(obj));
		}


		return code;
	}
}