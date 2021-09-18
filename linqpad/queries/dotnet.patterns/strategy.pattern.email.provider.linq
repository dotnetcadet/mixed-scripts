<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>

void Main()
{
	/// Example: 
	/// IServiceCollection
	///     |
	///     \/
	/// services.AddEmailProviderManager((serviceProvider) =>
	/// {
	///		return EmailProviderManager.Build()
	///			.AddEmailProvider<SomeEmailProvider, SomeEmailMessage>(new SomeProvider(serviceProvider.GetRequiredService<IOptions<EmailOptions>>()));
	/// });

}

public interface IEmailProvider<in TEmail>
		where TEmail : class, new()
{
	void SendEmail(TEmail email);

	Task SendEmailAsync(TEmail email);
}


public interface IEmailProviderManager
{

	IDictionary<Type, object> Providers { get; }

	void AddProvider<TProvider, TEmail>(TProvider provider)
		where TProvider : IEmailProvider<TEmail>
		where TEmail : class, new();

	Task<bool> SendEmailAsync<TProvider, TEmail>(TEmail email)
		where TProvider : IEmailProvider<TEmail>
		where TEmail : class, new();

	bool SendEmail<TProvider, TEmail>(TEmail email)
		where TProvider : IEmailProvider<TEmail>
		where TEmail : class, new();

	static void SendEmail<TEmail>(TEmail email, Func<IEmailProvider<TEmail>> provider)
		where TEmail : class, new() =>
			provider.Invoke().SendEmail(email);

	static async Task SendEmailAsync<TEmail>(TEmail email, Func<IEmailProvider<TEmail>> provider)
		where TEmail : class, new() =>
			await provider.Invoke().SendEmailAsync(email);
}


public class EmailProviderManager : IEmailProviderManager
    {
        public EmailProviderManager() { }

        public IDictionary<Type, object> Providers { get; private set; } = new Dictionary<Type, object>();


        public void AddProvider<TProvider, TEmail>(TProvider provider)
            where TProvider : IEmailProvider<TEmail>
            where TEmail : class, new() => 
                Providers.Add(typeof(TProvider), provider);

	public async Task<bool> SendEmailAsync<TProvider, TEmail>(TEmail email)
		where TProvider : IEmailProvider<TEmail>
		where TEmail : class, new()
	{
		var provider = Providers.FirstOrDefault(x => x.Key == typeof(TProvider)).Value;

		if (provider is IEmailProvider<TEmail>)
		{
			var emailer = provider as IEmailProvider<TEmail>;

			await emailer.SendEmailAsync(email);

			return true;
		}

		return false;
	}

	public bool SendEmail<TProvider, TEmail>(TEmail email)
		where TProvider : IEmailProvider<TEmail>
		where TEmail : class, new()
	{
		var provider = Providers.FirstOrDefault(x => x.Key == typeof(TProvider)).Value;

		if (provider is IEmailProvider<TEmail>)
		{
			var emailer = provider as IEmailProvider<TEmail>;

			emailer.SendEmail(email);

			return true;
		}

		return false;
	}


	public static EmailProviderManager Build() =>
		new EmailProviderManager();
}


public static class EmailProviderServiceCollection
{
	public static IServiceCollection AddEmailProviderManager(this IServiceCollection services) =>
		services.AddScoped<IEmailProviderManager, EmailProviderManager>();


	public static IServiceCollection AddEmailProviderManager(this IServiceCollection services, Action<IEmailProviderManager> configure)
	{
		var providerManager = new EmailProviderManager();
		configure.Invoke(providerManager);
		return services.AddScoped<IEmailProviderManager, EmailProviderManager>(x => providerManager);
	}

	public static IServiceCollection AddEmailProviderManager(this IServiceCollection services, Func<IServiceProvider, EmailProviderManager> configure) =>
		services.AddScoped<IEmailProviderManager, EmailProviderManager>(x => configure.Invoke(services.BuildServiceProvider()));

	public static EmailProviderManager AddEmailProvider<TProvider, TEmail>(this EmailProviderManager manager, TProvider provider)
		where TProvider : IEmailProvider<TEmail>
		where TEmail : class, new()
	{
		manager.AddProvider<TProvider, TEmail>(provider);
		return manager;
	}
}







