<Query Kind="Program" />

void Main()
{
	var route = "test/{param1}/{param2}/test/{param3}";
	
	route.FormatRoute("value1", "value2", "value3").Dump();
}

public static class Test
{
	public static string FormatRoute(this string route, params object[] parameters)
	{
		foreach (var parameter in parameters)
		{
			var start = route.IndexOf('{');
			var end = route.IndexOf('}');

			if (start == -1 || end == -1)
			{
				return route;
			}

			var variable = route.Substring(start, end + 1 - start);
			route = route.Replace(variable, parameter.ToString());
		}

		return route;
	}
}

