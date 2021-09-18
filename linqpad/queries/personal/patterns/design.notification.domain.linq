<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.DependencyInjection</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
</Query>

void Main()
{
	var services = new ServiceCollection();
	
	services.AddNotificationManager(
	
}

/*
	Notification Types: 
	- Device Notifications
	- Email Notifications
	- SMS Notifications
	- Voice Mail Notifications
	- Web Notifications
	- Teams Notifications
	
	
	Azure Services: 
	- Communication Services : https://docs.microsoft.com/en-us/azure/communication-services/overview
	- Notification Services

*/


public enum NotificationType
{
	Email,
	Sms,
	Device,
	Teams,
	Web
}




/// <summary>
/// This is a generic interface that should apply to all Notificaiton Types, Email, Device Notifications (iOS, Android, Windows, etc), 
/// Text Message, etc
/// </summary>
public interface INotification<in T>
{
	/// <summary>
	/// All notification types require a recipient
	/// </summary>
	string Recipient { get; set; }
	
	/// <summary>
	/// All notifications are subject to have a sender regardless if it's an automated sender.
	/// </summary>
	string Sender { get; set; }
	
	/// <summary>
	/// All notifications require a message. (NOTE: Messages may be limited based on notification type)
	/// </summary>
	string Body { get; set; }
	
	/// <summary>
	/// 
	/// </summary>
	NotificationType? SendVia { get; set; }
	
}


/// <summary>
/// 
/// </summary>
public interface INotificationProvider<in TNotification> 
	where TNotification : INotification
{
 	void Send(TNotification notification);
	
	Task SendAsync(TNotification notification);
}

public sealed class NotificationOptions
{
	
	public IDictionary< { get; set; }
	
	
	
}


public interface INotificationProviderManager
{
	IDictionary<Type, INotification> Providers { get; }

	void AddProvider<TProvider, TNotification>(TProvider provider)
		where TProvider : INotificationProvider<TNotification>
		where TNotification : INotification
		
		
//	void Send<TProvider, TNotifications>
//	Task<bool>
//		
//
//	Task<bool> SendAsync<TProvider, TEmail>(TEmail email)
//		where TProvider : IEmailProvider<TEmail>
//		where TEmail : class, new();
//
//	bool Send<TProvider, TEmail>(TEmail email)
//		where TProvider : IEmailProvider<TEmail>
//		where TEmail : class, new();
//		
//		
//
//	static void SendEmail<TEmail>(TEmail email, Func<IEmailProvider<TEmail>> provider)
//		where TEmail : class, new() =>
//			provider.Invoke().SendEmail(email);
//
//	static async Task SendEmailAsync<TEmail>(TEmail email, Func<IEmailProvider<TEmail>> provider)
//		where TEmail : class, new() =>
//			await provider.Invoke().SendEmailAsync(email);
}

public class NotificationProviderManager : INotificationProviderManager
{
	
	private IDictionary<Type, INotification> _providers = new Dictionary<Type, UserQuery.INotification>();
	
	
	public NotificationProviderManager(NotificationOptions options)
	{
		
	}
	
	
	
	public IDictionary<Type, INotification> Providers => _providers;
	
	
	
	/// <summary>
	/// </summary>
	public void AddProvider<TProvider, TNotification>(TProvider provider)
		where TProvider : INotificationProvider<TNotification>
		where TNotification : INotification
	{
		
	}
	
	
	public INotificationProvider<TNotification> GetProvider<TNotification>()
	{
		
	}
}



public static class NotificationServiceCollectionExtensions
{
	
	
	public static IServiceCollection AddNotificationManager(this IServiceCollection services, Action<>)
	{
		
		
		return services;
	}
}






#region Azure Notificaiton Hub






























#endregion
