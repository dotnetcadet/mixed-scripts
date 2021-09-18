<Query Kind="Program" />

void Main()
{
	QueryBuilder.Where( "",Operator.EqualTo, "" )
	
}

public enum Operator
{
	EqualTo
	Contains,
	GreaterThan,
	GreaterThanOrEqualTo,
	LessThan,
	LessThanOrEqualTo
}

public enum Conjunction 
{
	and, 
	or, 
	None
}


public class Where 
{
	public string Property { get; set; }
	
	public Operator Operator { get; set; }
	
	public string Value { get; set; }
	
	public Conjunction Conjunction { get; set; } = Conjunction.None;
	
	public IEnumerable<Where> Child { get; set; }
}


public class QueryBuilder
{
	
	public static Where Where(string property, Operator operators, string value, Conjunction conjunction = Conjunction.None)
	{
		return new Where
		{
			
		};
	}
}
