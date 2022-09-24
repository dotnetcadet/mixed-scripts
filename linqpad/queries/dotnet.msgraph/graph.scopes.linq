<Query Kind="Program">
  <NuGetReference>Microsoft.Graph</NuGetReference>
  <NuGetReference>Microsoft.Identity.Client</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Microsoft.Graph</Namespace>
  <Namespace>Microsoft.Identity.Client</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>



public static string ClientId = "";
public static string ClientSecrent = "";
public static string TenantId = "";
public static string Authority = $"https://login.microsoftonline.com/{TenantId}/oauth2/token";
public static string DefaultScope = "https://graph.microsoft.com/.default";


async Task Main()
{
	var appClient = ConfidentialClientApplicationBuilder.Create(ClientId)
		.WithClientSecret(ClientSecrent)
		.WithAuthority(Authority)
		.Build();
		
	var client = new GraphServiceClient(new GraphAuthProvider(appClient), new HttpProvider());
	
	//----------------------------------------------------------------------//

	var results = client.Users["@eastdilsecured.com"]
		.CalendarView
		.Request(new Option[]
		{
			new QueryOption("startdatetime", "2022-01-13T19:00:00-08:00"),
			new QueryOption("enddatetime", "2022-01-14T19:00:00-08:00")
		})
		.Top(200)
		.Select("subject, categories, end")
		.GetAsync()
		.GetAwaiter()
		.GetResult();
		
		
		
	System.IO.File.WriteAllText(@"C:\source\temp\temp.json", Newtonsoft.Json.JsonConvert.SerializeObject(results));

	
}


public class GraphAuthProvider : IAuthenticationProvider
{
	
	private readonly IConfidentialClientApplication AppClient;
	
	public GraphAuthProvider(IConfidentialClientApplication appClient) =>
		AppClient = appClient;
	
	
	public async Task AuthenticateRequestAsync(HttpRequestMessage request)
	{
		var authResults = await AppClient.AcquireTokenForClient(new[] { DefaultScope }).ExecuteAsync();
		var token = authResults.CreateAuthorizationHeader();
		request.Headers.Add("Authorization", token);
	}
}

public static class GraphScope
{
	public const string UserReadWrite = "User.ReadWrite";
	public const string MailSend = "Mail.Send";
	public const string UserRead = "User.Read";
	public const string UserReadAll = "User.Read.All";
	public const string MailReadShared = "Mail.Read.Shared";
	public const string MailRead = "Mail.Read";
	public const string AccessReviewReadAll = "AccessReview.Read.All";
	public const string AccessReviewReadWriteAll = "AccessReview.ReadWrite.All";
	public const string AccessReviewReadWriteMembership = "AccessReview.ReadWrite.Membership";
	public const string AdministrativeUnitReadAll = "AdministrativeUnit.Read.All";
	public const string AdministrativeUnitReadWriteAll = "AdministrativeUnit.ReadWrite.All";
	public const string AnalyticsRead = "Analytics.Read";
	public const string AppCatalogReadAll = "AppCatalog.Read.All";
	public const string AppCatalogReadWriteAll = "AppCatalog.ReadWrite.All";
	public const string AppCatalogSubmit = "AppCatalog.Submit";
	public const string ApplicationReadAll = "Application.Read.All";
	public const string ApplicationReadWriteAll = "Application.ReadWrite.All";
	public const string AppRoleAssignmentReadWriteAll = "AppRoleAssignment.ReadWrite.All";
	public const string ApplicationReadWriteOwnedBy = "Application.ReadWrite.OwnedBy";
	public const string BitlockerKeyReadBasicAll = "BitlockerKey.ReadBasic.All";
	public const string BitlockerKeyReadAll = "BitlockerKey.Read.All";
	public const string BookingsReadAll = "Bookings.Read.All";
	public const string BookingsAppointmentReadWriteAll = "BookingsAppointment.ReadWrite.All";
	public const string BookingsReadWriteAll = "Bookings.ReadWrite.All";
	public const string BookingsManageAll = "Bookings.Manage.All";
	public const string CalendarsRead = "Calendars.Read";
	public const string CalendarsReadShared = "Calendars.Read.Shared";
	public const string CalendarsReadWrite = "Calendars.ReadWrite";
	public const string CalendarsReadWriteShared = "Calendars.ReadWrite.Shared";
	public const string CalendarsSend = "Calendars.Send";
	public const string ChannelReadBasicAll = "Channel.ReadBasic.All";
	public const string ChannelCreate = "Channel.Create";
	public const string ChannelDeleteAll = "Channel.Delete.All";
	public const string TeamworkMigrateAll = "Teamwork.Migrate.All";
	public const string CallsInitiateAll = "Calls.Initiate.All";
	public const string CallsInitiateGroupCallAll = "Calls.InitiateGroupCall.All";
	public const string CallsJoinGroupCallAll = "Calls.JoinGroupCall.All";
	public const string CallsJoinGroupCallasGuestAll = "Calls.JoinGroupCallasGuest.All";
	public const string CallsAccessMediaAll = "Calls.AccessMedia.All";
	public const string CallRecordsReadAll = "CallRecords.Read.All";
	public const string ChannelMemberReadAll = "ChannelMember.Read.All";
	public const string ChannelMemberReadWriteAll = "ChannelMember.ReadWrite.All";
	public const string ChannelMessageDelete = "ChannelMessage.Delete";
	public const string ChannelMessageEdit = "ChannelMessage.Edit";
	public const string ChannelMessageReadAll = "ChannelMessage.Read.All";
	public const string ChannelMessageSend = "ChannelMessage.Send";
	public const string ChannelMessageUpdatePolicyViolationAll = "ChannelMessage.UpdatePolicyViolation.All";
	public const string ChannelSettingsReadAll = "ChannelSettings.Read.All";
	public const string ChannelSettingsReadWriteAll = "ChannelSettings.ReadWrite.All";
	public const string ChatRead = "Chat.Read";
	public const string ChatReadBasic = "Chat.ReadBasic";
	public const string ChatReadWrite = "Chat.ReadWrite";
	public const string ChatReadAll = "Chat.Read.All";
	public const string ChatReadBasicAll = "Chat.ReadBasic.All";
	public const string ChatUpdatePolicyViolationAll = "Chat.UpdatePolicyViolation.All";
	public const string ChatMessageSend = "ChatMessage.Send";
	public const string CloudPCReadAll = "CloudPC.Read.All";
	public const string CloudPCReadWriteAll = "CloudPC.ReadWrite.All";
	public const string PrinterReadWriteAll = "Printer.ReadWrite.All";
	public const string PrintJobReadAll = "PrintJob.Read.All";
	public const string PrintJobReadBasicAll = "PrintJob.ReadBasic.All";
	public const string PrintJobReadWriteAll = "PrintJob.ReadWrite.All";
	public const string PrintJobReadWriteBasicAll = "PrintJob.ReadWriteBasic.All";
	public const string ContactsRead = "Contacts.Read";
	public const string ContactsReadShared = "Contacts.Read.Shared";
	public const string ContactsReadWrite = "Contacts.ReadWrite";
	public const string ContactsReadWriteShared = "Contacts.ReadWrite.Shared";
	public const string DeviceRead = "Device.Read";
	public const string DeviceCommand = "Device.Command";
	public const string DeviceReadWriteAll = "Device.ReadWrite.All";
	public const string DirectoryReadAll = "Directory.Read.All";
	public const string DirectoryReadWriteAll = "Directory.ReadWrite.All";
	public const string DirectoryAccessAsUserAll = "Directory.AccessAsUser.All";
	public const string DomainReadWriteAll = "Domain.ReadWrite.All";
	public const string EduAdministrationRead = "EduAdministration.Read";
	public const string EduAdministrationReadWrite = "EduAdministration.ReadWrite";
	public const string EduAssignmentsReadBasic = "EduAssignments.ReadBasic";
	public const string EduAssignmentsReadWriteBasic = "EduAssignments.ReadWriteBasic";
	public const string EduAssignmentsRead = "EduAssignments.Read";
	public const string EduAssignmentsReadWrite = "EduAssignments.ReadWrite";
	public const string EduRosterReadBasic = "EduRoster.ReadBasic";
	public const string EduRosterRead = "EduRoster.Read";
	public const string EduRosterReadWrite = "EduRoster.ReadWrite";
	public const string EduAdministrationReadAll = "EduAdministration.Read.All";
	public const string EduAdministrationReadWriteAll = "EduAdministration.ReadWrite.All";
	public const string EduAssignmentsReadBasicAll = "EduAssignments.ReadBasic.All";
	public const string EduAssignmentsReadWriteBasicAll = "EduAssignments.ReadWriteBasic.All";
	public const string EduAssignmentsReadAll = "EduAssignments.Read.All";
	public const string EduAssignmentsReadWriteAll = "EduAssignments.ReadWrite.All";
	public const string EduRosterReadBasicAll = "EduRoster.ReadBasic.All";
	public const string EduRosterReadAll = "EduRoster.Read.All";
	public const string EduRosterReadWriteAll = "EduRoster.ReadWrite.All";
	public const string EntitlementManagementReadWriteAll = "EntitlementManagement.ReadWrite.All";
	public const string EntitlementManagementReadAll = "EntitlementManagement.Read.All";
	public const string FilesRead = "Files.Read";
	public const string FilesReadAll = "Files.Read.All";
	public const string FilesReadWrite = "Files.ReadWrite";
	public const string FilesReadWriteAll = "Files.ReadWrite.All";
	public const string FilesReadWriteAppFolder = "Files.ReadWrite.AppFolder";
	public const string FilesReadSelected = "Files.Read.Selected";
	public const string FilesReadWriteSelected = "Files.ReadWrite.Selected";
	public const string FinancialsReadWriteAll = "Financials.ReadWrite.All";
	public const string GroupReadAll = "Group.Read.All";
	public const string GroupReadWriteAll = "Group.ReadWrite.All";
	public const string GroupMemberReadAll = "GroupMember.Read.All";
	public const string GroupMemberReadWriteAll = "GroupMember.ReadWrite.All";
	public const string GroupSelected = "Group.Selected";
	public const string GroupCreate = "Group.Create";
	public const string UserReadBasicAll = "User.ReadBasic.All";
	public const string IdentityProviderReadAll = "IdentityProvider.Read.All";
	public const string IdentityProviderReadWriteAll = "IdentityProvider.ReadWrite.All";
	public const string IdentityRiskEventReadAll = "IdentityRiskEvent.Read.All";
	public const string IdentityRiskyUserReadAll = "IdentityRiskyUser.Read.All";
	public const string IdentityRiskyUserReadWriteAll = "IdentityRiskyUser.ReadWrite.All";
	public const string IdentityRiskyUserReadWriteALL = "IdentityRiskyUser.ReadWrite.ALL";
	public const string IdentityUserFlowReadAll = "IdentityUserFlow.Read.All";
	public const string IdentityUserFlowReadWriteAll = "IdentityUserFlow.ReadWrite.All";
	public const string IdentityUserFlowReadWriteALL = "IdentityUserFlow.ReadWrite.ALL";
	public const string IdentitytUserFlowReadWriteAll = "IdentitytUserFlow.ReadWrite.All";
	public const string InformationProtectionPolicyRead = "InformationProtectionPolicy.Read";
	public const string InformationProtectionPolicyReadAll = "InformationProtectionPolicy.Read.All";
	public const string DeviceManagementAppsReadAll = "DeviceManagementApps.Read.All";
	public const string DeviceManagementAppsReadWriteAll = "DeviceManagementApps.ReadWrite.All";
	public const string DeviceManagementConfigurationReadAll = "DeviceManagementConfiguration.Read.All";
	public const string DeviceManagementConfigurationReadWriteAll = "DeviceManagementConfiguration.ReadWrite.All";
	public const string DeviceManagementManagedDevicesPrivilegedOperationsAll = "DeviceManagementManagedDevices.PrivilegedOperations.All";
	public const string DeviceManagementManagedDevicesReadAll = "DeviceManagementManagedDevices.Read.All";
	public const string DeviceManagementManagedDevicesReadWriteAll = "DeviceManagementManagedDevices.ReadWrite.All";
	public const string DeviceManagementRBACReadAll = "DeviceManagementRBAC.Read.All";
	public const string DeviceManagementRBACReadWriteAll = "DeviceManagementRBAC.ReadWrite.All";
	public const string DeviceManagementServiceConfigReadAll = "DeviceManagementServiceConfig.Read.All";
	public const string DeviceManagementServiceConfigReadWriteAll = "DeviceManagementServiceConfig.ReadWrite.All";
	public const string DeviceManagementServiceConfigurationReadAll = "DeviceManagementServiceConfiguration.Read.All";
	public const string DeviceManagementServiceConfigurationReadWriteAll = "DeviceManagementServiceConfiguration.ReadWrite.All";
	public const string MailReadBasic = "Mail.ReadBasic";
	public const string MailReadWrite = "Mail.ReadWrite";
	public const string MailReadWriteShared = "Mail.ReadWrite.Shared";
	public const string MailSendShared = "Mail.Send.Shared";
	public const string MailboxSettingsRead = "MailboxSettings.Read";
	public const string MailboxSettingsReadWrite = "MailboxSettings.ReadWrite";
	public const string IMAPAccessAsUserAll = "IMAP.AccessAsUser.All";
	public const string POPAccessAsUserAll = "POP.AccessAsUser.All";
	public const string SMTPSend = "SMTP.Send";
	public const string MailReadBasicAll = "Mail.ReadBasic.All";
	public const string MemberReadHidden = "Member.Read.Hidden";
	public const string NotesRead = "Notes.Read";
	public const string NotesCreate = "Notes.Create";
	public const string NotesReadWrite = "Notes.ReadWrite";
	public const string NotesReadAll = "Notes.Read.All";
	public const string NotesReadWriteAll = "Notes.ReadWrite.All";
	public const string NotesReadWriteCreatedByApp = "Notes.ReadWrite.CreatedByApp";
	public const string NotificationsReadWriteCreatedByApp = "Notifications.ReadWrite.CreatedByApp";
	public const string CreatedByApp = "CreatedByApp";
	public const string OnlineMeetingsRead = "OnlineMeetings.Read";
	public const string OnlineMeetingsReadWrite = "OnlineMeetings.ReadWrite";
	public const string OnlineMeetingsReadAll = "OnlineMeetings.Read.All";
	public const string OnlineMeetingsReadWriteAll = "OnlineMeetings.ReadWrite.All";
	public const string OrganizationReadAll = "Organization.Read.All";
	public const string OrganizationReadWriteAll = "Organization.ReadWrite.All";
	public const string OrgContactReadAll = "OrgContact.Read.All";
	public const string PeopleRead = "People.Read";
	public const string PeopleReadAll = "People.Read.All";
	public const string PrivilegedAccessReadWriteAzureAD = "PrivilegedAccess.ReadWrite.AzureAD";
	public const string PrivilegedAccessReadWriteAzureADGroups = "PrivilegedAccess.ReadWrite.AzureADGroups";
	public const string PrivilegedAccessReadWriteAzureResources = "PrivilegedAccess.ReadWrite.AzureResources";
	public const string PrivilegedAccessReadAzureAD = "PrivilegedAccess.Read.AzureAD";
	public const string PrivilegedAccessReadAzureADGroups = "PrivilegedAccess.Read.AzureADGroups";
	public const string PrivilegedAccessReadAzureADResources = "PrivilegedAccess.Read.AzureADResources";
	public const string PlaceReadAll = "Place.Read.All";
	public const string PlaceReadWriteAll = "Place.ReadWrite.All";
	public const string PolicyReadAll = "Policy.Read.All";
	public const string PolicyReadPermissionGrant = "Policy.Read.PermissionGrant";
	public const string PolicyReadWriteApplicationConfiguration = "Policy.ReadWrite.ApplicationConfiguration";
	public const string PolicyReadWriteAuthenticationFlows = "Policy.ReadWrite.AuthenticationFlows";
	public const string PolicyReadWriteAuthorization = "Policy.ReadWrite.Authorization";
	public const string PolicyReadWriteConditionalAccess = "Policy.ReadWrite.ConditionalAccess";
	public const string PolicyReadWriteFeatureRollout = "Policy.ReadWrite.FeatureRollout";
	public const string PolicyReadWritePermissionGrant = "Policy.ReadWrite.PermissionGrant";
	public const string PolicyReadWriteTrustFramework = "Policy.ReadWrite.TrustFramework";
	public const string PolicyReadWriteAuthenticationMethod = "Policy.ReadWrite.AuthenticationMethod";
	public const string PolicyReadApplicationConfiguration = "Policy.Read.ApplicationConfiguration";
	public const string PresenceRead = "Presence.Read";
	public const string PresenceReadAll = "Presence.Read.All";
	public const string ProgramControlReadAll = "ProgramControl.Read.All";
	public const string ProgramControlReadWriteAll = "ProgramControl.ReadWrite.All";
	public const string ReportsReadAll = "Reports.Read.All";
	public const string RoleManagementReadAll = "RoleManagement.Read.All";
	public const string RoleManagementReadDirectory = "RoleManagement.Read.Directory";
	public const string RoleManagementReadWriteDirectory = "RoleManagement.ReadWrite.Directory";
	public const string ScheduleReadWriteAll = "Schedule.ReadWrite.All";
	public const string ScheduleReadAll = "Schedule.Read.All";
	public const string WorkforceIntegrationReadWriteAll = "WorkforceIntegration.ReadWrite.All";
	public const string WorkforceIntegrationReadAll = "WorkforceIntegration.Read.All";
	public const string ExternalItemReadWriteAll = "ExternalItem.ReadWrite.All";
	public const string ExternalItemReadAll = "ExternalItem.Read.All";
	public const string SecurityEventsReadAll = "SecurityEvents.Read.All";
	public const string SecurityEventsReadWriteAll = "SecurityEvents.ReadWrite.All";
	public const string SecurityActionsReadAll = "SecurityActions.Read.All";
	public const string SecurityActionsReadWriteAll = "SecurityActions.ReadWrite.All";
	public const string ThreatIndicatorsReadWriteOwnedBy = "ThreatIndicators.ReadWrite.OwnedBy";
	public const string ThreatIndicatorsReadAll = "ThreatIndicators.Read.All";
	public const string ShortNotesRead = "ShortNotes.Read";
	public const string ShortNotesReadWrite = "ShortNotes.ReadWrite";
	public const string ShortNotesReadAll = "ShortNotes.Read.All";
	public const string ShortNotesReadWriteAll = "ShortNotes.ReadWrite.All";
	public const string SitesReadAll = "Sites.Read.All";
	public const string SitesReadWriteAll = "Sites.ReadWrite.All";
	public const string SitesManageAll = "Sites.Manage.All";
	public const string SitesFullControlAll = "Sites.FullControl.All";
	public const string TasksRead = "Tasks.Read";
	public const string TasksReadShared = "Tasks.Read.Shared";
	public const string TasksReadWrite = "Tasks.ReadWrite";
	public const string TasksReadWriteShared = "Tasks.ReadWrite.Shared";
	public const string Tasks = "Tasks";
	public const string Group = "Group";
	public const string Shared = "Shared";
	public const string TermStoreReadAll = "TermStore.Read.All";
	public const string TermStoreReadWriteAll = "TermStore.ReadWrite.All";
	public const string TeamReadBasicAll = "Team.ReadBasic.All";
	public const string TeamCreate = "Team.Create";
	public const string TeamSettingsReadAll = "TeamSettings.Read.All";
	public const string TeamSettingsReadWriteAll = "TeamSettings.ReadWrite.All";
	public const string TeamsActivityRead = "TeamsActivity.Read";
	public const string TeamsActivitySend = "TeamsActivity.Send";
	public const string TeamsActivityReadAll = "TeamsActivity.Read.All";
	public const string TeamsAppReadAll = "TeamsApp.Read.All";
	public const string TeamsAppReadWriteAll = "TeamsApp.ReadWrite.All";
	public const string TeamsAppInstallationReadForUser = "TeamsAppInstallation.ReadForUser";
	public const string TeamsAppInstallationReadWriteForUser = "TeamsAppInstallation.ReadWriteForUser";
	public const string TeamsAppInstallationReadWriteSelfForUser = "TeamsAppInstallation.ReadWriteSelfForUser";
	public const string TeamsAppInstallationReadForTeam = "TeamsAppInstallation.ReadForTeam";
	public const string TeamsAppInstallationReadWriteForTeam = "TeamsAppInstallation.ReadWriteForTeam";
	public const string TeamsAppInstallationReadWriteSelfForTeam = "TeamsAppInstallation.ReadWriteSelfForTeam";
	public const string TeamsAppInstallationReadForUserAll = "TeamsAppInstallation.ReadForUser.All";
	public const string TeamsAppInstallationReadWriteForUserAll = "TeamsAppInstallation.ReadWriteForUser.All";
	public const string TeamsAppInstallationReadWriteSelfForUserAll = "TeamsAppInstallation.ReadWriteSelfForUser.All";
	public const string TeamsAppInstallationReadForTeamAll = "TeamsAppInstallation.ReadForTeam.All";
	public const string TeamsAppInstallationReadWriteForTeamAll = "TeamsAppInstallation.ReadWriteForTeam.All";
	public const string TeamsAppInstallationReadWriteSelfForTeamAll = "TeamsAppInstallation.ReadWriteSelfForTeam.All";
	public const string TeamMemberReadAll = "TeamMember.Read.All";
	public const string TeamMemberReadWriteAll = "TeamMember.ReadWrite.All";
	public const string TeamsTabReadAll = "TeamsTab.Read.All";
	public const string TeamsTabReadWriteAll = "TeamsTab.ReadWrite.All";
	public const string TeamsTabCreate = "TeamsTab.Create";
	public const string AgreementReadAll = "Agreement.Read.All";
	public const string AgreementReadWriteAll = "Agreement.ReadWrite.All";
	public const string AgreementAcceptanceRead = "AgreementAcceptance.Read";
	public const string AgreementAcceptanceReadAll = "AgreementAcceptance.Read.All";
	public const string ThreatAssessmentReadWriteAll = "ThreatAssessment.ReadWrite.All";
	public const string ThreatAssessmentReadAll = "ThreatAssessment.Read.All";
	public const string PrinterCreate = "Printer.Create";
	public const string PrinterFullControlAll = "Printer.FullControl.All";
	public const string PrinterReadAll = "Printer.Read.All";
	public const string PrinterShareReadBasicAll = "PrinterShare.ReadBasic.All";
	public const string PrinterShareReadAll = "PrinterShare.Read.All";
	public const string PrinterShareReadWriteAll = "PrinterShare.ReadWrite.All";
	public const string PrintJobCreate = "PrintJob.Create";
	public const string PrintJobRead = "PrintJob.Read";
	public const string PrintJobReadBasic = "PrintJob.ReadBasic";
	public const string PrintJobReadWrite = "PrintJob.ReadWrite";
	public const string PrintJobReadWriteBasic = "PrintJob.ReadWriteBasic";
	public const string PrintConnectorReadAll = "PrintConnector.Read.All";
	public const string PrintConnectorReadWriteAll = "PrintConnector.ReadWrite.All";
	public const string PrintSettingsReadAll = "PrintSettings.Read.All";
	public const string PrintSettingsReadWriteAll = "PrintSettings.ReadWrite.All";
	public const string PrintJobManageAll = "PrintJob.Manage.All";
	public const string PrintTaskDefinitionReadWriteAll = "PrintTaskDefinition.ReadWrite.All";
	public const string UserReadWriteAll = "User.ReadWrite.All";
	public const string UserInviteAll = "User.Invite.All";
	public const string UserExportAll = "User.Export.All";
	public const string UserManageIdentitiesAll = "User.ManageIdentities.All";
	public const string UserReadwriteAll = "User.Readwrite.All";
	public const string UserActivityReadWriteCreatedByApp = "UserActivity.ReadWrite.CreatedByApp";
	public const string UserAuthenticationMethodRead = "UserAuthenticationMethod.Read";
	public const string UserAuthenticationMethodReadAll = "UserAuthenticationMethod.Read.All";
	public const string UserAuthenticationMethodReadWrite = "UserAuthenticationMethod.ReadWrite";
	public const string UserAuthenticationMethodReadWriteAll = "UserAuthenticationMethod.ReadWrite.All";

	public partial class OpenId
	{
		public const string Email = "email";
		public const string OfflineAccess = "offline_access";

		/// <summary>
		/// Allows users to sign in to the app with their work or school accounts and allows the app to see basic user profile information.
		/// </summary>
		public const string Openid = "openid";
		public const string Profile = "profile";
		public const string Ccope = "scope";
	}
}
