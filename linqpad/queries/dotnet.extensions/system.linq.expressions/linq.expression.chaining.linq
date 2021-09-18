<Query Kind="Program" />

using System.Reflection;
using System.Reflection.Emit;


void Main()
{
	//var list = new List<Employee>();
	//
	//list.Select(x=>new {
	//	
	//});
	//
	
	var parameter = Expression.Parameter(typeof(Employee), "x");
	var evaluation = Expression.Constant(true);
	var substringArgs = new ConstantExpression[]
	{
		Expression.Constant(0),
		Expression.Constant(1)
	};
	
	var containsArgs = Expression.Constant("t");
	var property = Expression.Property(parameter, "FirstName");
	
	var substringMethod = typeof(string).GetMethod("Substring", new Type[] { typeof(int), typeof(int) });
	var containsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
	
	
	
	var exp1 = Expression.Call(
		property,
		substringMethod,
		substringArgs);
		
	var exp2 = Expression.Call(
		exp1, 
		containsMethod,
		containsArgs);
		
		
	
}


public class Employee
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public EmployeeDetails Details { get; set; }
}

public class EmployeeDetails
{
	public string Ssn { get; set; }
}
