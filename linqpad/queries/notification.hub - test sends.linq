<Query Kind="Program">
  <NuGetReference>Microsoft.Azure.NotificationHubs</NuGetReference>
  <Namespace>Microsoft.Azure.NotificationHubs</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>System.Text.Json</Namespace>
</Query>

async Task Main()
{
	
	
	

	var tag = "jbasinger@eastdilsecured.tech";
	
	IEnumerable<string> tags = new string[]{
		tag
	};
	var installation = new Installation()
	{
		InstallationId = "ES20_94E0BB79C4B726E54C055A1EC362B5DAF9E29910E6DCDA82240666DF9DB35264",
		PushChannel = "94E0BB79C4B726E54C055A1EC362B5DAF9E29910E6DCDA82240666DF9DB35264",
		Platform = NotificationPlatform.Apns,
		Tags = tags.ToList()
	
	};

	var test = JsonSerializer.Serialize<Installation>(installation);


	//logger.LogInformation(test);
	
	
	

	//var connectionString = "Endpoint=sb://es2-ntfns-app-qa-wu-01.servicebus.windows.net/;SharedAccessKeyName=test;SharedAccessKey=rY+7REHIUUVyPmUIwpreu4B8gZrtoFBFcDPkylFYobM=";
	var connectionString = "Endpoint=sb://es2-ntfns-app-qa-wu-01.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=TIVubeIbKE0qOzF7LXcJ7vp0xuipa5YYj4kmnp4c6FU=";
	var payload = "{\"aps\":{\"alert\":\"Notification Hub test notification\"}}";

	
	var hubClient = new NotificationHubClient(connectionString, "es2-ntf-app-qa-wu-portal");
	
	//await hubClient.CreateOrUpdateInstallationAsync(installation);
	
	var deviceRegistrations = await hubClient.GetRegistrationsByTagAsync(tag, 2000);
	
	deviceRegistrations.Dump();
	
	foreach(var deviceRegistration in deviceRegistrations)
	{
		
		
		// Delete Registrations
		await hubClient.DeleteRegistrationAsync(deviceRegistration.RegistrationId);
	}
}

// You can define other methods, fields, classes and namespaces here
