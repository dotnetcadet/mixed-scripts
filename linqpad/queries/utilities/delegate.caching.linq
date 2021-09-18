<Query Kind="Program" />

void Main()
{
	var user = new User() { FirstName = "Chase" };
	Func<User, string> GetFirstName = x =>
	{
		return x.FirstName;
	};

	var func = Memoize(GetFirstName);
	var string1 = func.Invoke(user); // Will Call the Function 
	var string2 = func.Invoke(user); // Will Use cached results instead of function
	
}

public class User
{
	public string FirstName { get; set; }
}


public static Func<TIn, TOut> Memoize<TIn, TOut>(Func<TIn, TOut> method)
{
	var cache = new Dictionary<TIn, TOut>();
	return input => cache.TryGetValue(input, out var results) ?
		results :
		cache[input] = method(input);
}
