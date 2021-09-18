<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Hosting</NuGetReference>
  <NuGetReference>RazorEngineCore</NuGetReference>
  <Namespace>Microsoft.Extensions.Hosting.Internal</Namespace>
  <Namespace>Microsoft.Extensions.FileProviders</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Microsoft.Extensions.Logging.EventLog</Namespace>
  <Namespace>Microsoft.Extensions.Options</Namespace>
</Query>

using RazorEngineCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


static async Task Main()
{
	var app = AppHostBuilder.Create()
		.ConfigureServices((context, services)=>
		{
			services.AddSingleton<IAppService, AppService>();
			services.AddSingleton<IRazorEngine, RazorEngine>(serviceProvider =>
			{
				var engine = new RazorEngine();
				
				return engine;
			});
		}).Build();
		
	app.StartAsync().GetAwaiter().GetResult();
}

public class AppService : IAppService
{
	private readonly IRazorEngine _engine;
	
	public AppService(
		IRazorEngine engine)
	{
		_engine = engine;
	}
	
	
	public async Task StartAsync(CancellationToken cancellationToken = default)
	{
		
	}

	public Task StopAsync(CancellationToken cancellationToken = default)
	{
		return Task.Run(() =>
		{
		});
	}
}



public interface IAppService
{
	Task StartAsync(CancellationToken cancellationToken = default);
	Task StopAsync(CancellationToken cancellationToken = default);
}





public interface IConfigureContainerAdapter
{
	void ConfigureContainer(HostBuilderContext hostContext, object containerBuilder);
}
public interface IServiceFactoryAdapter
{
	object CreateBuilder(IServiceCollection services);

	IServiceProvider CreateServiceProvider(object containerBuilder);
}
public class ServiceFactoryAdapter<TContainerBuilder> : IServiceFactoryAdapter
{
	private IServiceProviderFactory<TContainerBuilder> _serviceProviderFactory;

	private readonly Func<HostBuilderContext> _contextResolver;

	private Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> _factoryResolver;

	public ServiceFactoryAdapter(IServiceProviderFactory<TContainerBuilder> serviceProviderFactory)
	{
		_serviceProviderFactory = serviceProviderFactory ?? throw new ArgumentNullException("serviceProviderFactory");
	}

	public ServiceFactoryAdapter(Func<HostBuilderContext> contextResolver, Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factoryResolver)
	{
		_contextResolver = contextResolver ?? throw new ArgumentNullException("contextResolver");
		_factoryResolver = factoryResolver ?? throw new ArgumentNullException("factoryResolver");
	}

	public object CreateBuilder(IServiceCollection services)
	{
		if (_serviceProviderFactory == null)
		{
			_serviceProviderFactory = _factoryResolver(_contextResolver());
			if (_serviceProviderFactory == null)
			{
				throw new InvalidOperationException("The resolver returned a null IServiceProviderFactory");
			}
		}
		return _serviceProviderFactory.CreateBuilder(services);
	}

	public IServiceProvider CreateServiceProvider(object containerBuilder)
	{
		if (_serviceProviderFactory == null)
		{
			throw new InvalidOperationException("CreateBuilder must be called before CreateServiceProvider");
		}
		return _serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder);
	}
}
public class ConfigureContainerAdapter<TContainerBuilder> : IConfigureContainerAdapter
{
	private Action<HostBuilderContext, TContainerBuilder> _action;

	public ConfigureContainerAdapter(Action<HostBuilderContext, TContainerBuilder> action)
	{
		_action = action ?? throw new ArgumentNullException("action");
	}

	public void ConfigureContainer(HostBuilderContext hostContext, object containerBuilder)
	{
		_action(hostContext, (TContainerBuilder)containerBuilder);
	}
}

public class AppHost : IHost
{
	private readonly ILogger<AppHost> _logger;
	private readonly IServiceProvider _services;
	private readonly IHostLifetime _hostlifetime;
	private readonly ApplicationLifetime _lifetime;
	private readonly AppHostOptions _options;

	private IEnumerable<IAppService> _appServices;
	
	public AppHost(
		IServiceProvider services,
		IHostApplicationLifetime lifetime,
		IHostLifetime hostLifetime,
		ILogger<AppHost> logger,
		IOptions<AppHostOptions> options)
	{
		_services = services ?? throw new ArgumentNullException("services");
		_lifetime = (lifetime ?? throw new ArgumentNullException("applicationLifetime")) as ApplicationLifetime;

		if (_lifetime == null)
		{
			throw new ArgumentException("Replacing IHostApplicationLifetime is not supported.", "applicationLifetime");
		}
		_logger = logger ?? throw new ArgumentNullException("logger");
		_hostlifetime = hostLifetime ?? throw new ArgumentNullException("hostLifetime");
		_options = options?.Value ?? throw new ArgumentNullException("options");
	}


	public IServiceProvider Services => _services;

	public async Task StartAsync(CancellationToken cancellationToken = default)
	{
		//_logger.();
		using (CancellationTokenSource combinedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.ApplicationStopping))
		{
			var combinedCancellationToken = combinedCancellationTokenSource.Token;
			await _hostlifetime
				.WaitForStartAsync(combinedCancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
				
			combinedCancellationToken.ThrowIfCancellationRequested();
			_appServices = Services.GetService<IEnumerable<IAppService>>();
			
			foreach (var hostedService in _appServices)
			{
				await hostedService
					.StartAsync(combinedCancellationToken)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		_lifetime.NotifyStarted();
		//_logger.Started();
	}
	public async Task StopAsync(CancellationToken cancellationToken = default)
	{
		//_logger.Stopping();
		using (CancellationTokenSource tokenSource = new CancellationTokenSource(_options.ShutdownTimeout))
		{
			using( CancellationTokenSource linkedSource = CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token, cancellationToken))
			{
				var token = linkedSource.Token;
				var exceptions = new List<Exception>();
				
				_lifetime.StopApplication();
				if (_appServices != null)
				{
					foreach (var service in _appServices.Reverse())
					{
						try
						{
							await service.StopAsync(token).ConfigureAwait(continueOnCapturedContext: false);
						}
						catch (Exception item)
						{
							exceptions.Add(item);
						}
					}
				}
				_lifetime.NotifyStopped();
				
				try
				{
					await _hostlifetime.StopAsync(token).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (Exception exception)
				{
					exceptions.Add(exception);
				}
				if (exceptions.Count > 0)
				{
					var aggregateException = new AggregateException("One or more hosted services failed to stop.", exceptions);
					//_logger.StoppedWithException(ex);
					throw aggregateException;
				}
			}
		}
		//_logger.Stopped();
	}

	public void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();
	public async ValueTask DisposeAsync()
	{
		IServiceProvider services = Services;
		IAsyncDisposable asyncDisposable = services as IAsyncDisposable;
		if (asyncDisposable == null)
		{
			(services as IDisposable)?.Dispose();
		}
		else
		{
			await asyncDisposable.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
public class AppHostOptions
{
	/// <summary>The default timeout for <see cref="M:Microsoft.Extensions.Hosting.IHost.StopAsync(System.Threading.CancellationToken)" />.</summary>
	public TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromSeconds(5.0);

	public AppHostOptions() { }
}
public class AppHostBuilder : IHostBuilder
{
	private List<Action<IConfigurationBuilder>> 						_hostConfigurations = new();
	private List<Action<HostBuilderContext, IConfigurationBuilder>> 	_appConfigurations = new();
	private List<Action<HostBuilderContext, IServiceCollection>> 		_serviceConfigurations = new();
	private List<IConfigureContainerAdapter> 							_containerConfigurations = new();
	private IServiceFactoryAdapter 										_serviceFactory = new ServiceFactoryAdapter<IServiceCollection>(new DefaultServiceProviderFactory());

	private IConfiguration 		_hostConfiguration;
	private HostBuilderContext 	_hostBuilderContext;
	private HostingEnvironment 	_hostEnvironment;
	private IConfiguration 		_appConfiguration;
	private IServiceProvider 	_appServices;
	private bool _hostBuilt;
	
	
	public AppHostBuilder()
	{
		
	}
	
	public IDictionary<object, object> Properties => new Dictionary<object, object>();
	public IHost Build()
	{
		if (_hostBuilt)
		{
			throw new InvalidOperationException("Build can only be called once.");
		}
		_hostBuilt = true;
		BuildHostConfiguration();
		CreateHostingEnvironment();
		CreateHostBuilderContext();
		BuildAppConfiguration();
		CreateServiceProvider();
		return _appServices.GetRequiredService<IHost>();
	}
	public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configure)
	{
		_appConfigurations.Add(configure ?? throw new ArgumentNullException("configureDelegate"));
		return this;
	}
	public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configure)
	{
		_containerConfigurations.Add(new ConfigureContainerAdapter<TContainerBuilder>(configure ?? throw new ArgumentNullException("configureDelegate")));
		return this;
	}
	public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configure)
	{
		_hostConfigurations.Add(configure ?? throw new ArgumentNullException("configureDelegate"));
		return this;
	}
	public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configure)
	{
		_serviceConfigurations.Add(configure ?? throw new ArgumentNullException("configureDelegate"));
		return this;
	}
	public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
	{
		_serviceFactory = new ServiceFactoryAdapter<TContainerBuilder>(factory ?? throw new ArgumentNullException("factory"));
		return this;
	}
	public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
	{
		_serviceFactory = new ServiceFactoryAdapter<TContainerBuilder>(() => _hostBuilderContext, factory ?? throw new ArgumentNullException("factory"));
		return this;
	}


	private void BuildHostConfiguration()
	{
		var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection();
		foreach (var configuration in _hostConfigurations)
		{
			configuration(configurationBuilder);
		}
		_hostConfiguration = configurationBuilder.Build();
	}
	private void CreateHostingEnvironment()
	{
		_hostEnvironment = new HostingEnvironment
		{
			ApplicationName = _hostConfiguration[HostDefaults.ApplicationKey],
			EnvironmentName = (_hostConfiguration[HostDefaults.EnvironmentKey] ?? Environments.Production),
			ContentRootPath = ResolveContentRootPath(_hostConfiguration[HostDefaults.ContentRootKey], AppContext.BaseDirectory)
		};
		if (string.IsNullOrEmpty(_hostEnvironment.ApplicationName))
		{
			_hostEnvironment.ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name;
		}
		_hostEnvironment.ContentRootFileProvider = new PhysicalFileProvider(_hostEnvironment.ContentRootPath);
	}
	private void CreateHostBuilderContext()
	{
		_hostBuilderContext = new HostBuilderContext(Properties)
		{
			HostingEnvironment = _hostEnvironment,
			Configuration = _hostConfiguration
		};
	}
	private void BuildAppConfiguration()
	{
		var configurationBuilder = new ConfigurationBuilder()
			.SetBasePath(_hostEnvironment.ContentRootPath)
			.AddConfiguration(_hostConfiguration, shouldDisposeConfiguration: true);
			
		foreach (var _configuration in _appConfigurations)
		{
			_configuration(_hostBuilderContext, configurationBuilder);
		}
		
		_appConfiguration = configurationBuilder.Build();
		_hostBuilderContext.Configuration = _appConfiguration;
	}
	private void CreateServiceProvider()
	{
		var serviceCollection = new ServiceCollection();
		((IServiceCollection)serviceCollection).AddSingleton((IHostingEnvironment)_hostEnvironment);
		((IServiceCollection)serviceCollection).AddSingleton((IHostEnvironment)_hostEnvironment);
		serviceCollection.AddSingleton(_hostBuilderContext);
		serviceCollection.AddSingleton((IServiceProvider _) => _appConfiguration);
		serviceCollection.AddSingleton((IServiceProvider s) => (IApplicationLifetime)s.GetService<IHostApplicationLifetime>());
		serviceCollection.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>();
		serviceCollection.AddSingleton<IHostLifetime, ConsoleLifetime>();
		serviceCollection.AddSingleton<IHost, AppHost>();
		serviceCollection.AddOptions();
		serviceCollection.AddLogging();
		
		foreach (var configuration in _serviceConfigurations)
		{
			configuration(_hostBuilderContext, serviceCollection);
		}
		object containerBuilder = _serviceFactory.CreateBuilder(serviceCollection);
		foreach (var configuration in _containerConfigurations)
		{
			configuration.ConfigureContainer(_hostBuilderContext, containerBuilder);
		}
		_appServices = _serviceFactory.CreateServiceProvider(containerBuilder);
		if (_appServices == null)
		{
			throw new InvalidOperationException("The IServiceProviderFactory returned a null IServiceProvider.");
		}
		_appServices.GetService<IConfiguration>();
	}
	private string ResolveContentRootPath(string contentRootPath, string basePath)
	{
		if (string.IsNullOrEmpty(contentRootPath))
		{
			return basePath;
		}
		if (Path.IsPathRooted(contentRootPath))
		{
			return contentRootPath;
		}
		return Path.Combine(Path.GetFullPath(basePath), contentRootPath);
	}

	public static IHostBuilder Create(string[] args = null)
	{
		var builder = new AppHostBuilder();
		builder.UseContentRoot(Directory.GetCurrentDirectory());
		builder.ConfigureHostConfiguration(delegate (IConfigurationBuilder config)
		{
			config.AddEnvironmentVariables("DOTNET_");
			if (args != null)
			{
				config.AddCommandLine(args);
			}
		});
		builder.ConfigureAppConfiguration(delegate (HostBuilderContext hostingContext, IConfigurationBuilder config)
		{
			var hostingEnvironment = hostingContext.HostingEnvironment;
			var value = hostingContext.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);
			
			config.AddJsonFile("appsettings.json", optional: true, value).AddJsonFile("appsettings." + hostingEnvironment.EnvironmentName + ".json", optional: true, value);
			
			if (hostingEnvironment.IsDevelopment() && !string.IsNullOrEmpty(hostingEnvironment.ApplicationName))
			{
				Assembly assembly = Assembly.Load(new AssemblyName(hostingEnvironment.ApplicationName));
				if (assembly != null)
				{
					config.AddUserSecrets(assembly, optional: true);
				}
			}
			config.AddEnvironmentVariables();
			if (args != null)
			{
				config.AddCommandLine(args);
			}
		}).ConfigureLogging(delegate (HostBuilderContext hostingContext, ILoggingBuilder logging)
		{
			bool flag2 = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
			if (flag2)
			{
				logging.AddFilter<EventLogLoggerProvider>((LogLevel level) => level >= LogLevel.Warning);
			}
			logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
			logging.AddConsole();
			logging.AddDebug();
			logging.AddEventSourceLogger();
			if (flag2)
			{
				logging.AddEventLog();
			}
			logging.Configure(delegate (LoggerFactoryOptions options)
			{
				options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId | ActivityTrackingOptions.TraceId | ActivityTrackingOptions.ParentId;
			});
		}).UseDefaultServiceProvider(delegate (HostBuilderContext context, ServiceProviderOptions options)
		{
			bool validateOnBuild = (options.ValidateScopes = context.HostingEnvironment.IsDevelopment());
			options.ValidateOnBuild = validateOnBuild;
		});
		return builder;
	}
}
public class AppHostBuilderOptions
{
	public int MyProperty { get; set; }
}

