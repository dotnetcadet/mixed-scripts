<Query Kind="Program" />

void Main()
{
	
}


public abstract class PatcherRules
{
	public readonly IList<string> Keys = new List<string>();
	public readonly IDictionary<string, Func<dynamic,dynamic,bool>> Rules = new IDictionary<string, System.Func<dynamic, dynamic, bool>>();
	
	/// <summary>
	/// Will only set null properties on Target Object.
	/// </summary>
	public bool SetOnlyNulls { get; set; } = true;


	public static void Create<TTarget, TSource>()
	{
		
	}

	public void AddComparisonKey<TTarget, TSource>(Expression<Func<TTarget, TSource, bool>> condition)
	{
		
		
		Rules.Add("",condition.Compile());
		
	}
	


}


public interface IPatcher
{
	
}




public class Patcher : IPatcher
{
	private readonly PatcherRules Rules;
	
	internal bool IsKeysValid<TTarget, TSource>(TTarget targt, TSource source)
	{
		bool isValid = true;
		
		typeof(TTarget).GetProperties(
		
		
		
		return isValid;
	}
	
	
	
}
