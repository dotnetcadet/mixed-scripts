<Query Kind="Program">
  <NuGetReference Prerelease="true">Eastdil.AspNetCore.Mvc.Razor</NuGetReference>
  <Namespace>Eastdil.AspNetCore.Mvc.Razor</Namespace>
  <Namespace>System.Text.Json</Namespace>
</Query>





public string MyName = "Hi, My Name is @Model.Name";


void Main()
{
	// Create the Razor Engine to Compile and Run Razor Syntax
	var engine = RazorEngine.Create();
	
	// Build the Razor Syntax into Byte Code
	var template = engine.Build(MyName);
	
	// Write Token Replacement Output
	var outputName = template.Run(new
	{
		Name = "Chase Crawford"
	}).Dump();
}
