<Query Kind="Program">
  <Output>DataGrids</Output>
  <NuGetReference>RazorEngineCore</NuGetReference>
  <Namespace>Microsoft.AspNetCore.Razor.Language</Namespace>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Emit</Namespace>
  <Namespace>System.Reflection.Metadata</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Dynamic</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
  <Namespace>System.Net.Http</Namespace>
</Query>


namespace Assimalign.AspNetCore.Mvc.Razor
{
	public class Program 
	{
		static void Main()
		{

			var path = @"C:\Users\ccrawford\source\data";
			var data = string.Empty;
			using (FileStream stream = new FileStream(path + "\\test.csjson", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					data = reader.ReadToEnd();
				}
			}
			var options = new RazorEngineCompilationOptions();
			data = WriteDirectives(data, options);


			var builder = new RazorEngineCompilationOptionsBuilder();
			builder.Inherits(typeof(RazorEngineTemplateBase));


			var file = Path.GetRandomFileName();

			var engine = RazorProjectEngine.Create(
				RazorConfiguration.Default,
				RazorProjectFileSystem.Create(@"."), builder =>
				{
					builder.SetNamespace(options.TemplateNamespace);
				});

			var document = RazorSourceDocument.Create(data, file);
			//var item = engine.FileSystem.EnumerateItems(path).First();
			var results = engine.Process(
				document,
				null,
				new List<RazorSourceDocument>(),
				new List<TagHelperDescriptor>());
			var csharp = results.GetCSharpDocument();
			var syntaxTree = CSharpSyntaxTree.ParseText(csharp.GeneratedCode);
			var compilation = CSharpCompilation.Create(
						file,
						new[]
						{
							syntaxTree
						},
						options.ReferencedAssemblies.Select(ass =>
						{

							unsafe
							{
								ass.TryGetRawMetadata(out byte* blob, out int length);
								var moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)blob, length);
								var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
								var metadataReference = assemblyMetadata.GetReference();
								return metadataReference;
							}

						})
							.Concat(options.MetadataReferences)
							.ToList(),
						new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

			MemoryStream memoryStream = new MemoryStream();

			EmitResult emitResult = compilation.Emit(memoryStream);
			
			memoryStream.Position = 0;
			var template = new RazorEngineCompiledTemplate(memoryStream);
			
			var changes = template.Run(new Test 
			{
				Tags = new List<Test.Tag>()
				{
					new Test.Tag()
					{
						Type = "employee",
						Value = "ccrawford@eastdilsecured.com"
					}
				}
			});

			changes.Dump();


			


		}

		private static string WriteDirectives(string content, RazorEngineCompilationOptions options)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"@inherits {options.Inherits}");

			foreach (string entry in options.DefaultUsings)
			{
				stringBuilder.AppendLine($"@using {entry}");
			}

			stringBuilder.Append(content);

			return stringBuilder.ToString();
		}

	}
	
	public class Test
	{
		public IEnumerable<Tag> Tags { get; set; }
		
		public partial class Tag
		{
			public string Value { get; set; }
			public string Type { get; set; }
		}
	}


	public class RazorJsonFileSystem : RazorProjectFileSystem
	{
		private string _base;
		private readonly IList<RazorJsonItem> _items = new List<RazorJsonItem>();

		public RazorJsonFileSystem()
		{

		}

		public override IEnumerable<RazorProjectItem> EnumerateItems(string basePath)
		{
			_base = basePath;
			var directory = new DirectoryInfo(basePath);
			WalkDirectoryTree(directory);
			return _items;
		}

		public override RazorProjectItem GetItem(string path)
		{
			throw new NotImplementedException();
		}

		public override RazorProjectItem GetItem(string path, string fileKind)
		{
			throw new NotImplementedException();
		}


		void WalkDirectoryTree(DirectoryInfo root)
		{
			FileInfo[] files = null;
			DirectoryInfo[] subDirs = null;

			// First, process all the files directly under this folder
			try
			{
				files = root.GetFiles("*.csjson");

				foreach (var file in files)
				{
					var filePath = file.FullName.Substring(_base.Length, file.FullName.Length - _base.Length);
					var item = new RazorJsonItem(_base, filePath);
					_items.Add(item);
				}
			}
			// This is thrown if even one of the files requires permissions greater
			// than the application provides.
			catch (UnauthorizedAccessException e)
			{
				// This code just writes out the message and continues to recurse.
				// You may decide to do something different here. For example, you
				// can try to elevate your privileges and access the file again.
				//log.Add(e.Message);
			}

			catch (DirectoryNotFoundException exception)
			{
				Console.WriteLine(exception.Message);
			}

			if (files != null)
			{
				foreach (System.IO.FileInfo fi in files)
				{
					// In this example, we only access the existing FileInfo object. If we
					// want to open, delete or modify the file, then
					// a try-catch block is required here to handle the case
					// where the file has been deleted since the call to TraverseTree().
					Console.WriteLine(fi.FullName);
				}

				// Now find all the subdirectories under this directory.
				subDirs = root.GetDirectories();

				foreach (System.IO.DirectoryInfo dirInfo in subDirs)
				{
					// Resursive call for each subdirectory.
					WalkDirectoryTree(dirInfo);
				}
			}
		}
	}
	public class RazorJsonItem : RazorProjectItem
	{
		private readonly string _filePath;
		private readonly string _basePath;

		public RazorJsonItem(string basePath, string filePath)
		{
			_basePath = basePath;
			_filePath = filePath;
		}

		public override string BasePath => _basePath;
		public override string FilePath => _filePath;
		public override string PhysicalPath => _basePath + _filePath;
		public override bool Exists => File.Exists(PhysicalPath);

		public override Stream Read()
		{
			return new MemoryStream();
		}



	}

	public class RazorEngineCompilationOptions
	{
		public HashSet<Assembly> ReferencedAssemblies { get; set; }
		public HashSet<MetadataReference> MetadataReferences { get; set; } = new HashSet<MetadataReference>();
		public string TemplateNamespace { get; set; } = "TemplateNamespace";
		public string Inherits { get; set; } = "Assimalign.AspNetCore.Mvc.Razor.RazorEngineTemplateBase";

		public HashSet<string> DefaultUsings { get; set; } = new HashSet<string>()
		{
			"System",
			"System.Collections",
			"System.Collections.Generic",
			"System.Data",
			"System.Diagnostics",
			"System.IO",
			"System.Linq",
			"System.Linq.Expressions",
			"System.Reflection",
			"System.Text",
			"System.Text.RegularExpressions",
			"System.Threading",
			"System.Transactions",
			"System.Xml",
			"System.Xml.Linq",
			"System.Xml.XPath"
		};

		public RazorEngineCompilationOptions()
		{
			bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
			bool isFullFramework = RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);

			if (isWindows && isFullFramework)
			{
				this.ReferencedAssemblies = new HashSet<Assembly>()
				{
					typeof(object).Assembly,
					Assembly.Load(new AssemblyName("Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
					typeof(RazorEngineTemplateBase).Assembly,
					typeof(System.Runtime.GCSettings).Assembly,
					typeof(System.Collections.IList).Assembly,
					typeof(System.Collections.Generic.IEnumerable<>).Assembly,
					typeof(System.Linq.Enumerable).Assembly,
					typeof(System.Linq.Expressions.Expression).Assembly,
					Assembly.Load(new AssemblyName("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51"))
				};
			}

			if (isWindows && !isFullFramework) // i.e. NETCore
			{
				this.ReferencedAssemblies = new HashSet<Assembly>()
				{
					typeof(object).Assembly,
					Assembly.Load(new AssemblyName("Microsoft.CSharp")),
					typeof(RazorEngineTemplateBase).Assembly,
					Assembly.Load(new AssemblyName("System.Runtime")),
					typeof(System.Collections.IList).Assembly,
					typeof(System.Collections.Generic.IEnumerable<>).Assembly,
					Assembly.Load(new AssemblyName("System.Linq")),
					Assembly.Load(new AssemblyName("System.Linq.Expressions")),
					Assembly.Load(new AssemblyName("netstandard"))
				};
			}

			if (!isWindows)
			{
				this.ReferencedAssemblies = new HashSet<Assembly>()
				{
					typeof(object).Assembly,
					Assembly.Load(new AssemblyName("Microsoft.CSharp")),
					typeof(RazorEngineTemplateBase).Assembly,
					Assembly.Load(new AssemblyName("System.Runtime")),
					typeof(System.Collections.IList).Assembly,
					typeof(System.Collections.Generic.IEnumerable<>).Assembly,
					Assembly.Load(new AssemblyName("System.Linq")),
					Assembly.Load(new AssemblyName("System.Linq.Expressions")),
					Assembly.Load(new AssemblyName("netstandard"))
				};
			}
		}
	}
	public class RazorEngineCompilationOptionsBuilder
	{
		public RazorEngineCompilationOptions Options { get; set; }

		public RazorEngineCompilationOptionsBuilder(RazorEngineCompilationOptions options = null)
		{
			this.Options = options ?? new RazorEngineCompilationOptions();
		}

		public void AddAssemblyReferenceByName(string assemblyName)
		{
			Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
			this.AddAssemblyReference(assembly);
		}

		public void AddAssemblyReference(Assembly assembly)
		{
			this.Options.ReferencedAssemblies.Add(assembly);
		}

		public void AddAssemblyReference(Type type)
		{
			this.AddAssemblyReference(type.Assembly);

			foreach (Type argumentType in type.GenericTypeArguments)
			{
				this.AddAssemblyReference(argumentType);
			}
		}

		public void AddMetadataReference(MetadataReference reference)
		{
			this.Options.MetadataReferences.Add(reference);
		}

		public void AddUsing(string namespaceName)
		{
			this.Options.DefaultUsings.Add(namespaceName);
		}

		public void Inherits(Type type)
		{
			this.Options.Inherits = this.RenderTypeName(type);
			this.AddAssemblyReference(type);
		}

		private string RenderTypeName(Type type)
		{
			string result = type.Namespace + "." + type.Name;

			if (result.Contains('`'))
			{
				result = result.Substring(0, result.IndexOf("`"));
			}

			if (type.GenericTypeArguments.Length == 0)
			{
				return result;
			}

			return result + "<" + string.Join(",", type.GenericTypeArguments.Select(this.RenderTypeName)) + ">";
		}
	}
	public abstract class RazorEngineTemplateBase : IRazorEngineTemplate
	{
		private readonly StringBuilder stringBuilder = new StringBuilder();

		private string attributeSuffix = null;

		public dynamic Model { get; set; }

		public void WriteLiteral(string literal = null)
		{
			WriteLiteralAsync(literal).GetAwaiter().GetResult();
		}

		public virtual Task WriteLiteralAsync(string literal = null)
		{
			this.stringBuilder.Append(literal);
			return Task.CompletedTask;
		}

		public void Write(object obj = null)
		{
			WriteAsync(obj).GetAwaiter().GetResult();
		}

		public virtual Task WriteAsync(object obj = null)
		{
			this.stringBuilder.Append(obj);
			return Task.CompletedTask;
		}

		public void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset,
			int attributeValuesCount)
		{
			BeginWriteAttributeAsync(name, prefix, prefixOffset, suffix, suffixOffset, attributeValuesCount).GetAwaiter().GetResult();
		}

		public virtual Task BeginWriteAttributeAsync(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
		{
			this.attributeSuffix = suffix;
			this.stringBuilder.Append(prefix);
			return Task.CompletedTask;
		}

		public void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength,
			bool isLiteral)
		{
			WriteAttributeValueAsync(prefix, prefixOffset, value, valueOffset, valueLength, isLiteral).GetAwaiter().GetResult();
		}

		public virtual Task WriteAttributeValueAsync(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
		{
			this.stringBuilder.Append(prefix);
			this.stringBuilder.Append(value);
			return Task.CompletedTask;
		}

		public void EndWriteAttribute()
		{
			EndWriteAttributeAsync().GetAwaiter().GetResult();
		}

		public virtual Task EndWriteAttributeAsync()
		{
			this.stringBuilder.Append(this.attributeSuffix);
			this.attributeSuffix = null;
			return Task.CompletedTask;
		}

		public void Execute()
		{
			ExecuteAsync().GetAwaiter().GetResult();
		}

		public virtual Task ExecuteAsync()
		{
			return Task.CompletedTask;
		}

		public virtual string Result()
		{
			return ResultAsync().GetAwaiter().GetResult();
		}

		public virtual Task<string> ResultAsync()
		{
			return Task.FromResult<string>(this.stringBuilder.ToString());
		}
	}
	public class RazorEngineCompiledTemplate 
	{
		private readonly MemoryStream assemblyByteCode;
		private readonly Type templateType;

		internal RazorEngineCompiledTemplate(MemoryStream assemblyByteCode)
		{
			this.assemblyByteCode = assemblyByteCode;

			Assembly assembly = Assembly.Load(assemblyByteCode.ToArray());
			this.templateType = assembly.GetType("TemplateNamespace.Template");
		}

		public static RazorEngineCompiledTemplate LoadFromFile(string fileName)
		{
			return LoadFromFileAsync(fileName: fileName).GetAwaiter().GetResult();
		}

		public static async Task<RazorEngineCompiledTemplate> LoadFromFileAsync(string fileName)
		{
			MemoryStream memoryStream = new MemoryStream();

			using (FileStream fileStream = new FileStream(
				path: fileName,
				mode: FileMode.Open,
				access: FileAccess.Read,
				share: FileShare.None,
				bufferSize: 4096,
				useAsync: true))
			{
				await fileStream.CopyToAsync(memoryStream);
			}

			return new RazorEngineCompiledTemplate(memoryStream);
		}

		public static RazorEngineCompiledTemplate LoadFromStream(Stream stream)
		{
			return LoadFromStreamAsync(stream).GetAwaiter().GetResult();
		}

		public static async Task<RazorEngineCompiledTemplate> LoadFromStreamAsync(Stream stream)
		{
			MemoryStream memoryStream = new MemoryStream();
			await stream.CopyToAsync(memoryStream);
			memoryStream.Position = 0;

			return new RazorEngineCompiledTemplate(memoryStream);
		}

		public void SaveToStream(Stream stream)
		{
			this.SaveToStreamAsync(stream).GetAwaiter().GetResult();
		}

		public Task SaveToStreamAsync(Stream stream)
		{
			return this.assemblyByteCode.CopyToAsync(stream);
		}

		public void SaveToFile(string fileName)
		{
			this.SaveToFileAsync(fileName).GetAwaiter().GetResult();
		}

		public Task SaveToFileAsync(string fileName)
		{
			using (FileStream fileStream = new FileStream(
				path: fileName,
				mode: FileMode.OpenOrCreate,
				access: FileAccess.Write,
				share: FileShare.None,
				bufferSize: 4096,
				useAsync: true))
			{
				return assemblyByteCode.CopyToAsync(fileStream);
			}
		}

		public string Run(object model = null)
		{
			return this.RunAsync(model).GetAwaiter().GetResult();
		}

		public async Task<string> RunAsync(object model = null)
		{
			if (model != null && model.IsAnonymous())
			{
				model = new AnonymousTypeWrapper(model);
			}
			
			var temp = Activator.CreateInstance(this.templateType);
			IRazorEngineTemplate instance = (IRazorEngineTemplate)temp;
			instance.Model = model;

			await instance.ExecuteAsync();

			return await instance.ResultAsync();
		}
	}

	public class AnonymousTypeWrapper : DynamicObject
	{
		private readonly object model;

		public AnonymousTypeWrapper(object model)
		{
			this.model = model;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			PropertyInfo propertyInfo = this.model.GetType().GetProperty(binder.Name);

			if (propertyInfo == null)
			{
				result = null;
				return false;
			}

			result = propertyInfo.GetValue(this.model, null);

			if (result == null)
			{
				return true;
			}

			var type = result.GetType();

			if (result.IsAnonymous())
			{
				result = new AnonymousTypeWrapper(result);
			}

			bool isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);

			if (isEnumerable && !(result is string))
			{
				result = ((IEnumerable<object>)result)
						.Select(e =>
						{
							if (e.IsAnonymous())
							{
								return new AnonymousTypeWrapper(e);
							}

							return e;
						})
						.ToList();
			}


			return true;
		}
	}
	public interface IRazorEngineTemplate
	{
		dynamic Model { get; set; }

		void WriteLiteral(string literal = null);

		Task WriteLiteralAsync(string literal = null);

		void Write(object obj = null);

		Task WriteAsync(object obj = null);

		void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount);

		Task BeginWriteAttributeAsync(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount);

		void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral);

		Task WriteAttributeValueAsync(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral);

		void EndWriteAttribute();

		Task EndWriteAttributeAsync();

		void Execute();

		Task ExecuteAsync();

		string Result();

		Task<string> ResultAsync();
	}

	public abstract class RazorEngineTemplateBase<T> : RazorEngineTemplateBase
	{
		public new T Model { get; set; }
	}
	
	public static class ObjectExtenders
	{
		public static ExpandoObject ToExpando(this object obj)
		{
			ExpandoObject expando = new ExpandoObject();
			IDictionary<string, object> dictionary = expando;

			foreach (var property in obj.GetType().GetProperties())
			{
				dictionary.Add(property.Name, property.GetValue(obj));
			}

			return expando;
		}

		public static bool IsAnonymous(this object obj)
		{
			Type type = obj.GetType();

			return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
				   && type.IsGenericType && type.Name.Contains("AnonymousType")
				   && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
				   && type.Attributes.HasFlag(TypeAttributes.NotPublic);
		}
	}



}










