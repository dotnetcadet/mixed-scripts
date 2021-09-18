<Query Kind="Program">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Reflection.Metadata</Namespace>
</Query>

void Main()
{
	var employee = new Employee();

	
	var where = new CosmosWhere<Employee>()
	{
		And = new[]
		{
			new CosmosWhere<Employee>()
			{
				Or = new[]
				{
					new CosmosWhere<Employee>()
					{
						Property = "Details.FirstName",
						Operator = Operator.Equal,
						StartsWith = new CosmosStartsWith()
						{
							Value = "chase"
						},
						Value = true
					},
					new CosmosWhere<Employee>()
					{
						Property = "Details.FirstName",
						Operator = Operator.Equal,
						EndsWith = new CosmosEndsWith()
						{
							Value = "chase"
						},
						Value = true
					}
				}
			},
			new CosmosWhere<Employee>()
			{
				Property = "LastName",
				Operator = Operator.Equal,
				Value = "Crawford"
			},
		}
	};
	
	var expression = where.GetLambdaExpression();
}

#region query.format
public string query = @"
{

    ""select"": [
        """",
        """"
    ],
    ""where"": {
        ""$property"": """",
        """"$substring"": {
            ""indexStart"": 2,
            ""count"": 45
        },
        ""$operator"": """",
        ""$value"": """"



		""and"": [
            {
                ""or"": [
                    {
                        ""leftOf"": {

                        },
                        ""property"": """",
                        ""operator"": """",
                        ""value"": """"
                    },
                    {
	""property"": """",
                        ""operator"": """",
                        ""value"": """"

					}
                ]
            },
            {
	""property"": """",
                ""operator"": """",
                ""value"": """"

			}
        ]
    }
}


";

#endregion


#region test classes
public class Employee
{
	public string LastName { get; set; }
	public EmployeeDetails Details { get; set; }
}

public class EmployeeDetails
{
	public string FirstName { get; set; }
	public IEnumerable<EmployeeAddress> Addresses { get; set; }
}

public class EmployeeAddress
{
	public string Street { get; set; }
}
#endregion


public enum Operator : int
{
	None = 0,
	In = 1,
	Equal = 2,
	NotEqual = 2,
	GreaterThan = 3,
	GreaterThanEqualTo = 4,
	LessThan = 5,
	LessThanEqualTo = 6,
}

public sealed class CosmosQuery<T>
{
	public IEnumerable<CosmosSelect> Select { get; set; }
	public CosmosWhere<T> Where { get; set; }
}


#region cosmos.where.clause
#region cosmos.string.functions
public class CosmosSubString
{
	public int Start { get; set; } = 0;
	public int Length { get; set; }
}
public class CosmosEndsWith
{
	public string Value { get; set; }
}
public class CosmosStartsWith
{
	public string Value { get; set; }
}
public class CosmosStringEquals
{
	public int MyProperty { get; set; }
}
#endregion

/// <summary>
/// Base level interface for Where Clause
/// </summary>
public interface ICosmosWhere
{
	string Property { get; set; }
	Operator Operator { get; set; }
	object Value { get; set; }
}

public interface ICosmosWhere<T> : ICosmosWhere
{
	IEnumerable<ICosmosWhere<T>> And { get; set; }
	IEnumerable<ICosmosWhere<T>> Or { get; set; }
	CosmosSubString SubString { get; set; }
	CosmosStartsWith StartsWith { get; set; }
	CosmosEndsWith EndsWith { get; set; }
}


/// <summary>
/// - Only Single Level Functions are supported. No embedded function support
/// </summary>
public sealed class CosmosWhere<T> : ICosmosWhere<T>
{
	internal bool HasOperator => this.Operator == Operator.None;
	
	public CosmosWhere() { }

	[JsonPropertyName("$and")]
	public IEnumerable<ICosmosWhere<T>> And { get; set; }

	[JsonPropertyName("$or")]
	public IEnumerable<ICosmosWhere<T>> Or { get; set; }

	[JsonPropertyName("$property")]
	public string Property { get; set; }
	
	[JsonPropertyName("$operator")]
	[JsonConverter(typeof(CosmosWhere<>.OperatorConverter))]
	public Operator Operator {get; set; } = Operator.None;
	
	[JsonPropertyName("$value")]
	[JsonConverter(typeof(CosmosWhere<>.ObjectConverter))]
	public object Value { get; set; }

	[JsonPropertyName("$startwith")]
	public CosmosStartsWith StartsWith { get; set; }
	
	[JsonPropertyName("$endswith")]
	public CosmosEndsWith EndsWith { get; set; }	
	
	[JsonPropertyName("$substring")]
	public CosmosSubString SubString { get; set; }
	


	internal Expression GetLambdaExpression()
	{
		var type = typeof(T);
		return Expression.Lambda<Func<T, bool>>(CompileLambdaExpression(this), Expression.Parameter(type, type.Name));
		Expression CompileLambdaExpression(ICosmosWhere<T> instance)
		{
			Expression or = null;
			Expression and = null;
			
			if (null != instance.And && instance.And.Any())
			{
				foreach (var where in instance.And)
				{
					if (null != where.And && where.And.Any())
					{
						if (and == null)
							and = CompileLambdaExpression(where);
						else
							and = Expression.And(and, CompileLambdaExpression(where));
					}

					else if (null != where.Or && where.Or.Any())
					{
						if (and == null)
							and = CompileLambdaExpression(where);
						else
							and = Expression.Or(and, CompileLambdaExpression(where));
					}

					else
					{
						if (and == null)
							and = QueryHelper.GetWhereExpression(where);
						else
							and = Expression.And(and, QueryHelper.GetWhereExpression(where));
					}
				}

				return and;
			}
			else if (null != instance.Or && instance.Or.Any())
			{
				foreach (var where in instance.Or)
				{
					if (null != where.Or && where.Or.Any())
					{
						if (null == or)
							or = CompileLambdaExpression(where);
						else
							or = Expression.Or(or, CompileLambdaExpression(where));
					}

					else if (null != where.And && where.And.Any())
					{
						if (null == or)
							or = CompileLambdaExpression(where);
						else
							or = Expression.And(or, CompileLambdaExpression(where));
					}
					else
					{
						if (null == or)
							or = QueryHelper.GetWhereExpression(where);
						else
							or = Expression.Or(or, QueryHelper.GetWhereExpression(where));
					}
				}

				return or;
			}
			else
			{
				return QueryHelper.GetWhereExpression(this);
			}
		}
	}

	
	#region Internal Serialization

	internal partial class OperatorConverter : JsonConverter<Operator>
	{
		public override Operator Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.
		}

		public override void Write(Utf8JsonWriter writer, Operator value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}
	internal partial class ObjectConverter : JsonConverter<object>
	{
		public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}

		public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}

	#endregion

	#region internal expression functions

	internal Expression GetSubStringExpression()
	{
		MemberExpression member = null;
		var arg1 = Expression.Constant(this.SubString.Start);
		var arg2 = Expression.Constant(this.SubString.Length);
		var method = typeof(string).GetMethod("SubString", new Type[] { typeof(string) });
		return Expression.Call(member, method, arg1, arg2);
	}
	
	#endregion
}

internal static class QueryHelper
{
	public static Expression GetWhereExpression<T>(ICosmosWhere<T> where)
	{
		Expression expression = null;

		if (null != where.StartsWith)
		{
			if (null == where.Value)
				where.Value = true;

			var argument = QueryHelper.GetArgumentExpressions(where.StartsWith.Value);
			var method = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
			var member = QueryHelper.GetPropertyMemberExpression<T>(where);
			expression = Expression.Call(member, method, argument);
		}

		else if (null != where.EndsWith)
		{
			if (null == where.Value)
				where.Value = true;

			var argument = QueryHelper.GetArgumentExpressions(where.EndsWith.Value);
			var method = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
			var member = QueryHelper.GetPropertyMemberExpression<T>(where);
			expression = Expression.Call(member, method, argument);
		}

		else if (null != where.SubString)
		{
			var arguments = QueryHelper.GetArgumentExpressions(where.SubString.Start, where.SubString.Length);
			var method = typeof(string).GetMethod("SubString", new Type[] { typeof(string) });
			var member = QueryHelper.GetPropertyMemberExpression<T>(where);
			expression = Expression.Call(member, method, arguments);
		}
		else
		{
			expression = QueryHelper.GetPropertyMemberExpression<T>(where);
		}

		return GetOperatorExpression<T>(where, expression);
	}

	private static MemberExpression GetPropertyMemberExpression<T>(ICosmosWhere where)
	{
		var type = typeof(T);
		var parameter = Expression.Parameter(type, type.Name);
		var paths = where.Property.Split('.');
		Expression expression = parameter;

		for (int i = 0; i < paths.Length; i++)
			expression = Expression.Property(expression, paths[i]);

		return expression as MemberExpression;
	}

	private static ConstantExpression GetExpressionArgument(object? value)
	{
		return Expression.Constant(value);
	}

	private static IEnumerable<ConstantExpression> GetArgumentExpressions(params object[]? values)
	{
		for (int i = 0; i < values.Length; i++)
		{
			yield return Expression.Constant(values[i]);
		}
	}

	private static Expression GetOperatorExpression<T>(ICosmosWhere where, Expression expression)
	{
		var constant = Expression.Constant(where.Value);

		if (where.Operator == Operator.Equal)
			return Expression.Equal(expression, constant);

		if (where.Operator == Operator.NotEqual)
			return Expression.NotEqual(expression, constant);

		if (where.Operator == Operator.GreaterThan)
			return Expression.GreaterThan(expression, constant);

		if (where.Operator == Operator.GreaterThanEqualTo)
			return Expression.GreaterThanOrEqual(expression, constant);

		if (where.Operator == Operator.LessThan)
			return Expression.LessThan(expression, constant);

		if (where.Operator == Operator.LessThanEqualTo)
			return Expression.LessThanOrEqual(expression, constant);

		return null;
	}

}

#endregion

public class CosmosSelect
{
	public string Property { get; set; }
	//public CosmosQuery Array { get; set; }
	
	
}

//public class CosmosWhere
//{
//	public string Property { get; set; }
//	public Operator Operator { get; set; }
//	public object Value { get; set; }
//
//	public IEnumerable<CosmosWhere> And { get; set; }
//	public IEnumerable<CosmosWhere> Or { get; set; }
//	
//	
//	internal Expression Compile<T>()
//	{
//		var parameter = QueryHelper.GetParameter<T>();
//		return Expression.Lambda<Func<T, bool>>(CompileWhereClause(this), parameter);	
//		Expression CompileWhereClause(CosmosWhere clause)
//		{
//			Expression or = null;
//			Expression and = null;
//			if (null != clause.And && clause.And.Any())
//			{
//				foreach(var where in clause.And)
//				{
//					if (null != where.And && where.And.Any())
//					{
//						if (and == null)
//							and = CompileWhereClause(where);
//						else
//							and = Expression.And(and, CompileWhereClause(where));
//					}
//
//					else if (null != where.Or && where.Or.Any())
//					{
//						if (and == null)
//							and = CompileWhereClause(where);
//						else
//							and = Expression.Or(and, CompileWhereClause(where));
//					}
//					
//					else
//					{
//						var constant = QueryHelper.GetConstant(where);
//						var field = QueryHelper.GetProperty(where, parameter);
//
//						if (and == null)
//							and = QueryHelper.GetOperator<T>(where, field, constant);
//						else
//							and = Expression.And(and, QueryHelper.GetOperator<T>(where, field, constant));
//					}					
//				}
//				
//				return and;
//			}
//			
//			else if (null != clause.Or && clause.Or.Any())
//			{
//				var parameter = QueryHelper.GetParameter<T>();
//				
//				foreach (var where in clause.Or)
//				{
//					var constant = QueryHelper.GetConstant(where);
//					var field = QueryHelper.GetProperty(where, parameter);
//
//					if (or == null)
//						or = QueryHelper.GetOperator<T>(where, field, constant);
//					else
//						or = Expression.Or(or, QueryHelper.GetOperator<T>(where, field, constant));
//
//					if (null != where.Or && where.Or.Any())
//					{
//						or = Expression.Or(or, CompileWhereClause(where));
//					}
//
//					if (null != where.And && where.And.Any())
//					{
//						or = Expression.And(or, CompileWhereClause(where));
//					}
//				}
//				
//				return or;
//
//			} 
//			else
//			{
//				var parameter = QueryHelper.GetParameter<T>();
//				var constant = QueryHelper.GetConstant(clause);
//				var field = QueryHelper.GetProperty(clause, parameter);
//				var evaluation = QueryHelper.GetOperator<T>(clause, field, constant);
//				return evaluation;
//			}
//		}
//	}
//}
//
//public static class QueryHelper
//{
//	public static ParameterExpression GetParameter<T>(string name = "model") =>
//		Expression.Parameter(typeof(T), name);
//		
//	public static ConstantExpression GetConstant(CosmosWhere clause) =>
//		Expression.Constant(clause.Value);
//
//	public static MemberExpression GetProperty(CosmosWhere clause, ParameterExpression expression)
//	{
//		var paths = clause.Property.Split('.');
//		Expression parameter = expression;
//
//		for(int i = 0; i < paths.Length; i++)
//		{
//			parameter = Expression.Property(parameter, paths[i]);
//		}
//		
//		return parameter as MemberExpression;
//	}
//		
//	public static Expression GetOperator<T>(CosmosWhere clause, MemberExpression member, ConstantExpression constant)
//	{
//		if (clause.Operator == Operator.Equal)
//			return Expression.Equal(member, constant);
//		
//		if (clause.Operator == Operator.NotEqual)
//			return Expression.NotEqual(member, constant);
//			
//		if (clause.Operator == Operator.GreaterThan)
//			return Expression.GreaterThan(member, constant);
//			
//		if (clause.Operator == Operator.GreaterThanEqualTo)
//			return Expression.GreaterThanOrEqual(member, constant);
//			
//		if (clause.Operator == Operator.LessThan)
//			return Expression.LessThan(member, constant);
//			
//		if (clause.Operator == Operator.LessThanEqualTo)
//			return Expression.LessThanOrEqual(member, constant);
//			
//		if (clause.Operator == Operator.StartsWith)
//		{
//			var method = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
//			return Expression.Call(member, method, constant);
//		}
//		if (clause.Operator == Operator.EndsWith)
//		{
//			var method = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
//			return Expression.Call(member, method, constant);
//		}
//
//		
//		
//		return null;
//	}
//}

internal static class ThrowHelper
{
	public static void EmbeddedFunctionsException() =>
		throw new Exception("Embedded Functions are not currently supported in where clause. Example: Where STARTSWITH(SUBSCTRING(0,5), 'Some String').");
}



//public MethodCallExpression GetSelect<T>(Select selet)
//{
//	
//}



//public MethodCallExpression GetWhere<T>(Where where, IQueryable<T> query)
//{
//	//var predicate = BuildWherePredicate<T>(where);
//	return Expression.Call(
//		typeof(Queryable),
//		"where",
//		new Type[] 
//		{
//			query.ElementType
//		},
//		query.Expression,
//		BuildWherePredicate<T>(where));
//
//
//
//	var param = Expression.Parameter(typeof(T), "model");
//	var value = Expression.Constant("Chase");
//	var field = Expression.PropertyOrField(param, "FirstName");
//	var operater = Expression.Equal(field, value);
//	var lambda = Expression.Lambda<Func<T, bool>>(operater,
//		new ParameterExpression[]
//		{
//			param
//		});
//}

//
//public Expression GetOperator<T>(Operator condition, MemberExpression field, ConstantExpression value)
//{
//	var property = typeof(T).GetType().GetProperty(field.Member.Name) ?? throw new Exception("");
//	
//	if (condition == Operator.Any)
//	{
//		if (property is IEnumerable enumerable)
//		{
//			var overload = enumerable.GetType().GetMethod("Any");
//			var anyExpression = Expression.Call(overload,
//			
//			
//								 
//		}
//		
//	}
//	switch(condition)
//	{
//		case Operator.Equals:
//			return Expression.Equal(field, value);
//		case Operator.Contains:
//		
//			if (property is IEnumerable enumerable)
//			{
//				return Expression.Call(value, enumerable.GetType().GetMethod("Contains"), field);
//			}
//			else
//			{
//				throw new Exception("");	
//			}
//		default: 
//			throw new Exception("");
//	}
//}
//
//public Expression<Func<T, bool>> BuildWherePredicate<T>(Where where)
//{
//
//	var param = Expression.Parameter(typeof(T), "model");
//	
//	Expression ParseWhere(Where where)
//	{
//		var value = Expression.Constant(where.Value);
//		var field = Expression.PropertyOrField(param, where.Property);
//		var operater = GetOperator<T>(where.Operator, field, value);
//		
//		
//		if (where.And.Any())
//		{
//			Expression child
//			foreach(var and in where.And)
//			{
//				if (
//				child = ParseWhere(and);
//			}
//		}
//	}
//	var value = Expression.Constant("Chase");
//	var field = Expression.PropertyOrField(param, "FirstName");
//	var operater = Expression.Equal(field, value);
//	
//	
//	var lambda = Expression.Lambda<Func<T, bool>>(operater,
//		new ParameterExpression[]
//		{
//			param
//		});
//
//
//	Expression exp = null;
//	
//	
//}
//
//public Expression ParseWhereClause(IEnumerable<Where> children, Conjunction conjunction, Para
//
//
//
//static ParameterExpression CreateParameter<TType>(int index = 0)
//			=> CreateParameter(typeof(TType), index);
//
//static ParameterExpression CreateParameter(Type type, int index = 0)
//{
//	// TODO use x1...xN we will need to update tests to use this... but its safer than assuming depth
//	string[] parameters = new[] { "x", "y", "z", "a", "b" };
//	return Expression.Parameter(type, parameters.Skip(index).First());
//}
//
//public static Expression<Func<T, bool>> CreatePredicate(Filter filter)
//{
//	ParameterExpression pe = CreateParameter<T>(); //
//
//	Expression predicateBody = GenerateExpression(filter, pe);
//
//	return Expression.Lambda<Func<T, bool>>(predicateBody, pe);
//}
//
//public static Expression GenerateExpression(Filter filter, ParameterExpression pe = null)
//{
//	if (pe == null) pe = CreateParameter<T>(); // Expression.Parameter(typeof(T), "x");
//
//	if (filter.IsSet)
//
//		return Parse(filter.Filters, filter.Operator, pe);
//
//	else
//
//		return Parse(filter, pe);
//
//}
//
//static Expression CreateArrayExpression(IEnumerable right, Type type)
//{
//	List<Expression> trees = new List<Expression>();
//
//	foreach (var o in right) trees.Add(Expression.Constant(o.To(type)));
//
//	// Create an expression tree that represents creating and
//	// initializing a one-dimensional array of type string.
//	NewArrayExpression newArrayExpression = Expression.NewArrayInit(type, trees);
//
//	return newArrayExpression;
//}
//
//
//static Expression Parse(IEnumerable<Filter> children, LogicalOperators olo, ParameterExpression pe, int index = 1)
//{
//	var list = new List<Expression>();
//	foreach (Filter element in children)
//	{
//		if (element.Condition == ConditionTokens.None)
//		{
//			list.Add(Parse(element.Filters, element.Operator, pe, index));
//		}
//		else
//		{
//			list.Add(Parse(element, pe, index));
//		}
//	}
//
//	if (olo == LogicalOperators.AND)
//		return AndClause(list.ToArray());
//
//	else if (olo == LogicalOperators.OR)
//		return OrClause(list.ToArray());
//
//	else if (olo == LogicalOperators.NOT)
//		return Expression.Not(list.First());
//
//	return list.FirstOrDefault();
//	//throw new NotImplementedException("");
//}
//
//static Expression Parse(Filter r, ParameterExpression pe, int index = 1)
//{
//	Value left = default(Expression);
//	Value right = default(Expression);
//
//	left = Value.CreateExpression(r.Property, pe);
//
//	// Handle nulls early
//	if (r.Condition == ConditionTokens.IsNotNull || r.Condition == ConditionTokens.IsNull)
//	{
//		right = Expression.Constant(null);
//		return r.Condition == ConditionTokens.IsNull ? Expression.Equal(left, right) : Expression.NotEqual(left, right);
//	}
//	else if (r.Value == null)
//	{
//		if (left.Type.IsNullableType())
//			right = Expression.Constant(null, left.Type);
//	}
//	else if (r.Value.GetType().IsArray)
//	{
//		right = CreateArrayExpression(r.Value as Array, r.Value.GetType().GetGenericType());
//	}
//	else if (r.Value.GetType().IsEnumerableType() && r.Value.GetType() != typeof(String))
//	{
//		// try to convert the array to the same type of the value
//		if (r.Value.GetType().GetGenericType() != left.Type)
//			right = CreateArrayExpression(r.Value as IEnumerable, left.Type);
//		else
//			right = CreateArrayExpression(r.Value as IEnumerable, r.Value.GetType().GetGenericType());
//	}
//	else
//	{
//		right = Expression.Constant(r.Value.To(left.GetGenericType()), left.GetGenericType());
//	}
//
//	if (r.Condition == ConditionTokens.StartsWith || r.Condition == ConditionTokens.EndsWith)
//	{
//		var method = typeof(string).GetMethod(r.Condition.ToString(), new[] { typeof(string), typeof(StringComparison) });
//
//		return Expression.Call(left, method, new[] { right.Expression, Expression.Constant(StringComparison.OrdinalIgnoreCase) });
//	}
//	else if (r.Condition == ConditionTokens.Contains || r.Condition == ConditionTokens.IN)
//	{
//		if (r.Condition == ConditionTokens.IN)
//		{
//			var temp = right;
//			right = left;
//			left = temp;
//		}
//		if (left.Type == typeof(String))
//		{
//			var method = typeof(string).GetMethod(r.Condition.ToString(), new[] { typeof(string), typeof(StringComparison) });
//
//			return Expression.Call(left, method, new[] { right.Expression, Expression.Constant(StringComparison.OrdinalIgnoreCase) });
//		}
//		else
//		{
//
//			var met = Helper.GetEnumerableExtensionMethod("Contains", new[] { left.GetGenericType() },
//				new[] { left.Type, left.GetGenericType() });
//
//			if (met == null)
//			{
//				throw new Exception($"Unable to use {r.Condition} with {r.Value}");
//			}
//
//			if (right.Type.IsNullableType() && !left.GetGenericType().IsNullableType())
//			{
//				right = Expression.Convert(right, right.Type.GetNonNullableType());
//			}
//			else if (left.Type.GetGenericType() != left.Type)
//			{
//				right = Expression.Convert(right, left.Type.GetGenericType());
//			}
//
//			return Expression.Call(met, left, right);
//		}
//	}
//
//	if (left.Expression is MethodCallExpression mce && mce.Method.Name == "Count")
//	{
//		if (mce.Arguments.Any() && r.Filters != null && r.Filters.Any())
//		{
//			Value count = mce.Arguments.First();
//
//			var ype = CreateParameter(count.Type.GetGenericType(), index);
//
//			var rex = Parse(r.Filters, r.Operator, ype, index + 1);
//
//			var ft = typeof(System.Func<,>).MakeGenericType(new[] { count.Type.GetGenericType(), typeof(bool) });
//
//			var predicate = Expression.Lambda(ft, rex, ype);
//
//			var met = Helper.GetEnumerableExtensionMethod("Count", new[] { count.GetGenericType() },
//				new[] { count.Type, typeof(Func<object, bool>) });
//
//			left = Expression.Call(met, count, predicate);
//		}
//	}
//
//	if (r.Condition == ConditionTokens.Any || r.Condition == ConditionTokens.All)
//	{
//		var ype = CreateParameter(left.GetGenericType(), index); // Expression.Parameter(left.GetGenericType(), $"y{index}");
//
//		if (r.Filters != null && r.Filters.Any())
//		{
//			right = Parse(r.Filters, r.Operator, ype, index + 1);
//		};
//
//		if (right.IsEmpty)
//		{
//			var met = Helper.GetEnumerableExtensionMethod(r.Condition.ToString(), new[] { left.GetGenericType() },
//					new[] { left.Type });
//
//			return Expression.Call(met, left);
//		}
//		else
//		{
//			var met = Helper.GetEnumerableExtensionMethod(r.Condition.ToString(), new[] { left.GetGenericType() },
//					new[] { left.Type, typeof(Func<object, bool>) });
//
//			// Create a typeof System.Func<T, bool>
//			var ft = typeof(System.Func<,>).MakeGenericType(new[] { left.GetGenericType(), typeof(bool) });
//
//			var predicate = Expression.Lambda(ft, right, ype);
//
//			return Expression.Call(met, left, predicate);
//		}
//	}
//
//	if (right.Type.IsString() && new[] { ConditionTokens.GT, ConditionTokens.GTE, ConditionTokens.LT, ConditionTokens.LTE }.Contains(r.Condition))
//	{
//		var method = typeof(string).GetMethod("CompareTo", new[] { typeof(string) });
//		left = Expression.Call(left, method, right);
//
//		right = Expression.Constant(0);
//	}
//
//	switch (r.Condition)
//	{
//		case ConditionTokens.EQ:
//			return Expression.Equal(left, right);
//
//		case ConditionTokens.NE:
//			return Expression.NotEqual(left, right);
//
//		case ConditionTokens.GT:
//			return Expression.GreaterThan(left, right);
//
//		case ConditionTokens.GTE:
//			return Expression.GreaterThanOrEqual(left, right);
//
//		case ConditionTokens.LT:
//			return Expression.LessThan(left, right);
//
//		case ConditionTokens.LTE:
//			return Expression.LessThanOrEqual(left, right);
//
//		case ConditionTokens.IsTrue:
//
//			right = Expression.Constant(true, left.Type);
//			return Expression.Equal(left, right);
//
//		case ConditionTokens.IsFalse:
//
//			right = Expression.Constant(false, left.Type);
//			return Expression.Equal(left, right);
//
//		default:
//			throw new NotImplementedException($"Token: `{r.Condition}` is not supported or implemented.");
//	}
//}
//
//static Expression OrClause(params Expression[] e) => Merge(Expression.OrElse, e);
//
//static Expression AndClause(params Expression[] e) => Merge(Expression.AndAlso, e);
//
//static Expression Merge(Func<Expression, Expression, Expression> merge, params Expression[] e)
//{
//	Expression p = e.ElementAt(0);
//
//	for (int i = 1; i < e.Count(); i++)
//	{
//		var c = e.ElementAt(i);
//
//		p = merge(p, c);
//	}
//
//	return p;
//}
//