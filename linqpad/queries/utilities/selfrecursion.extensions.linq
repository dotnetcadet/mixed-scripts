<Query Kind="Program" />

void Main()
{
	
}



public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
{
	foreach (var value in values)
	{
		action(value);
	}
}

/// <summary>
/// 
/// </summary>
/// <param name="values"></param>
/// <param name="action"></param>
/// <returns></returns>
public static IEnumerable<string> ForEach(this IEnumerable<string> values, Func<string, string> action)
{
	var items = new List<string>();
	foreach (var value in values)
	{
		items.Add(action(value));
	}
	return items;
}
