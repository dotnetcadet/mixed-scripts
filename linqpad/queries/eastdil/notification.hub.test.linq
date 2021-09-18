<Query Kind="Program">
  <NuGetReference>Microsoft.Azure.NotificationHubs</NuGetReference>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;


const string path = "";
const string connection = "";


async Task Main()
{
	var user = "hharris@eastdilsecured.com";
	var client = new NotificationHubClient(connection, path);
	var tags = new []
	{
		user
	};
	
	 var registrations = await client.GetAllRegistrationsAsync(100);
	
	 foreach(var registration in registrations.Where(x => x.Tags.Contains(user)))
	 {
	    await client.DeleteRegistrationAsync(registration);
		registration.Dump();	
	 }
	
	
	var notification = "{\"aps\":{\"alert\":\"Notification Hub test notification\"}}";
	var response = await client.SendAppleNativeNotificationAsync(notification, tags);
	
	response.Dump();
}



