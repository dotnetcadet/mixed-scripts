<Query Kind="Statements">
  <NuGetReference>AutoMapper</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
</Query>

using AutoMapper;
using Newtonsoft.Json;



#region Mapping Profile

public class CompanyItemMapProfile : Profile
{
	public CompanyItemMapProfile()
	{
		base.AddMemberConfiguration().AddName<CaseInsensitiveName>();
		base.AllowNullCollections = true;
		base.AllowNullDestinationValues = true;

		CreateMap<CompanyItem, CompanyEntry>()
			// Flattening Maps
			.ForMember(destination => destination.CompanyId, options => options.MapFrom(source => source.Id))
			.ForMember(destination => destination.Status, options => options.MapFrom(source => source.Status))
			.ForMember(destination => destination.CompanyId, options => options.MapFrom(source => source.CompanyInformation.CompanyGUID))
			.ForMember(destination => destination.CompanyName, options => options.MapFrom(source => source.CompanyInformation.CompanyName))
			.ForMember(destination => destination.DbaName, options => options.MapFrom(source => source.CompanyInformation.DBAName))
			.ForMember(destination => destination.CompanyLogo, options => options.MapFrom(source => source.CompanyInformation.CompanyLogo))
			// One To One Object Maps (Lower Level Maps Below)
			.ForMember(destination => destination.DnbEntry, options => options.MapFrom(source => source.DnbRecord))

			// Many To Many Maps
			.ForMember(destination => destination.Addresses, options => options.MapFrom(source => source.CompanyAddresses))
			.ForMember(destination => destination.Offices, options => options.MapFrom(source => source.CompanyOffices))
			.ReverseMap();

		CreateMap<CompanyItem, CompanyDnbEntry>()
		   .ForMember(destination => destination.DnbDunsNumber, options => options.MapFrom(source => source.CompanyInformation.DnBDunsNumber))
		   .ForMember(destination => destination.DnbCheck, options => options.MapFrom(source => source.CompanyInformation.DnBCheck))
		   .ForMember(destination => destination.DnbPrimaryName, options => options.MapFrom(source => source.CompanyInformation.DnBPrimaryName))
		   .ReverseMap();

		CreateMap<CompanyItemDnbInfo, CompanyDnbEntry>()
			.ForMember(destination => destination.DnbMatchDataProfile, options => options.MapFrom(source => source.DnBMatchDataProfile))
			.ForMember(destination => destination.DnbMatchGrade, options => options.MapFrom(source => source.DnBMatchGrade))
			.ForMember(destination => destination.DnbMatchScore, options => options.MapFrom(source => source.DnBMatchScore))
			.ForMember(destination => destination.DnbExportIndicator, options => options.MapFrom(source => source.DnBExportIndicator))
			.ForMember(destination => destination.DnbAccountantName, options => options.MapFrom(source => source.DnBAccountantName))
			.ForMember(destination => destination.DnbAgentIndicator, options => options.MapFrom(source => source.DnBAgentIndicator))
			.ForMember(destination => destination.DnbCompanyPrincipalJobTitle, options => options.MapFrom(source => source.DnBCompanyPrincipalJobTitle))
			.ForMember(destination => destination.DnbEmployeeQuantitySingleLocation, options => options.MapFrom(source => source.DnBEmployeeQuantitySingleLocation))
			.ForMember(destination => destination.DnbYearlyRevenueCurrency, options => options.MapFrom(source => source.DnBYearlyRevenueCurrency))
			.ForMember(destination => destination.DnbFamilyTreeHierarchyLevel, options => options.MapFrom(source => source.DnBFamilyTreeHierarchyLevel))
			.ForMember(destination => destination.DnbStandardIndustryNames, options => options.MapFrom(source => source.DnBStandardIndustryNames))
			.ForMember(destination => destination.DnbFaxNumber, options => options.MapFrom(source => source.DnBFaxNumber))
			.ForMember(destination => destination.DnbDunsNumber, options => options.MapFrom(source => source.DnBDunsNumber))
			.ForMember(destination => destination.DnbTradeStyleName, options => options.MapFrom(source => source.DnBTradeStyleName))
			.ForMember(destination => destination.DnbStandaloneIndicator, options => options.MapFrom(source => source.DnBStandaloneIndicator))
			.ForMember(destination => destination.DnbControlYear, options => options.MapFrom(source => source.DnBControlYear))
			.ForMember(destination => destination.DnbConfIdenceCode, options => options.MapFrom(source => source.DnBConfidenceCode))
			.ForMember(destination => destination.DnbHooversIndustryCode, options => options.MapFrom(source => source.DnBStandardIndustryCode))
			.ForMember(destination => destination.DnbEmployeeQuantityAllLocations, options => options.MapFrom(source => source.DnBEmployeeQuantityAllLocations))
			.ForMember(destination => destination.DnbStandardIndustryCode, options => options.MapFrom(source => source.DnBStandardIndustryCode))
			.ForMember(destination => destination.DnbLegalStructure, options => options.MapFrom(source => source.DnBLegalStructure))
			.ForMember(destination => destination.DnbYearlyRevenue, options => options.MapFrom(source => source.DnBYearlyRevenue))
			.ForMember(destination => destination.DnbTelephone, options => options.MapFrom(source => source.DnBTelephone))
			.ForMember(destination => destination.DnbCompanyPrincipalFirstName, options => options.MapFrom(source => source.DnBCompanyPrincipalFirstName))
			.ForMember(destination => destination.DnbCompanyPrincipalLastName, options => options.MapFrom(source => source.DnBCompanyPrincipalLastName))
			.ReverseMap();


		// Company Address
		CreateMap<CompanyItemAddress, CompanyAddressEntry>()
			.ForMember(destination => destination.AddressId, options => options.MapFrom(source => source.AddressGuid))
			.ForMember(destination => destination.StateOrRegion, options => options.MapFrom(source => source.State))
			.ForMember(destination => destination.CountryCode, options => options.MapFrom(source => source.Countrycode))
			.ReverseMap();

		CreateMap<CompanyItemAddress, CompanyEntry>()
			.ForMember(destination => destination.CompanyId, options => options.MapFrom(source => source.CompanyGuid))
			.ReverseMap();

		// Company Office
		CreateMap<CompanyItemOffice, CompanyOfficeEntry>()
			.ForMember(destination => destination.OfficeName, options => options.MapFrom(source => source.CompanyOfficeName))
			.ForMember(destination => destination.OfficeAddresses, options => options.MapFrom(source => source.addresses))
			.ReverseMap();
	}
}

#endregion

#region Database Models

public class CompanyItem
{
	// Contract Mapped
	[JsonProperty("id")]
	public Guid Id { get; set; }

	// Contract Mapped
	public string Status { get; set; }

	[JsonProperty("addresses")]
	public IEnumerable<CompanyItemAddress> CompanyAddresses { get; set; }

	[JsonProperty("communications")]
	public CompanyItemCommunication[] CompanyCommunications { get; set; }

	[JsonProperty("companyInfo")]
	public CompanyItemInfo CompanyInformation { get; set; }


	[JsonProperty("offices")]
	public CompanyItemOffice[] CompanyOffices { get; set; }

	public string Region { get; set; }

	[JsonProperty("dnb")]
	public CompanyItemDnbInfo DnbRecord { get; set; }
}

public class CompanyItemAddress
{
	public string GlobalRollup { get; set; }

	// Contract Mapped
	[JsonProperty("AddressLine1")]
	public string StreetOne { get; set; }

	// Contract Mapped
	[JsonProperty("AddressLine2")]
	public string StreetTwo { get; set; }

	// Contract Mapped
	[JsonProperty("AddressLine3")]
	public string StreetThree { get; set; }

	// Contract Mapped
	[JsonProperty("AddressLine4")]
	public string StreetFour { get; set; }

	// Contract Mapped
	[JsonProperty("CompanyGUID")]
	public Guid CompanyGuid { get; set; }

	// Contract Mapped
	public string WorldRegion { get; set; }

	[JsonProperty("AddressGUID")]
	public Guid AddressGuid { get; set; }

	// Contract Mapped
	public string State { get; set; }


	public string WorldSubregion { get; set; }

	// Contract Mapped
	public string PostalCode { get; set; }

	// Contract Mapped
	public string City { get; set; }

	public string AddressType { get; set; }

	public DateTime? DateAdded { get; set; }

	// Contract Mapped
	public string Countrycode { get; set; }

	public DateTime? LastUpdated { get; set; }

	// Contract Mapped
	public string CrossStreet { get; set; }

	// Contract Mapped
	public string Salutation { get; set; }

	// Contract Mapped
	public string Timezone { get; set; }

	// Contract Mapped
	public double? Latitude { get; set; }

	// Contract Mapped
	public double? Longitude { get; set; }

	public int? AddedbyEmployeeID { get; set; }

	public int? UpdatedbyEmployeeID { get; set; }
}

public class CompanyItemCommunication
{
	public Guid CompanyGUID { get; set; }

	public string ElectronicCommunicationValue { get; set; }

	public string ElectronicCommunicationType { get; set; }

	public DateTime? LastUpdated { get; set; }

	public DateTime? DateAdded { get; set; }

	public int? AddedbyEmployeeID { get; set; }

	public int? UpdatedbyEmployeeID { get; set; }
}

public class CompanyItemInfo
{
	public string Status { get; set; }

	[AutoMapper.IgnoreMap]
	public string[] Roles { get; set; }

	// Contract Mapped
	public string CompanyName { get; set; }

	public string CompanyNameUppercase { get; set; }

	public Guid CompanyGUID { get; set; }

	// Contract Mappoed
	public long? CompanyID { get; set; }

	// Contract Mapped
	public string MDMCode { get; set; }

	// Contract Mapped
	public string DnBPrimaryName { get; set; }


	public string Region { get; set; }

	// Contract Mapped
	public string DnBDunsNumber { get; set; }

	public DateTime? LastUpdated { get; set; }

	public DateTime? DateAdded { get; set; }

	// Contract Mapped
	public bool? DnBCheck { get; set; }

	// Contract Mapped
	public int? NumActiveDeals { get; set; }

	// Contract Mapped
	public string CompanyLogo { get; set; }

	// Contract Mapped
	public int? NumDealsAsClient { get; set; }

	// Contract Mapped
	public int? NumContacts { get; set; }

	// Contract Mapped
	public int? NumDealsAsLender { get; set; }

	// Contract Mapped
	public int? NumDealsAsInvestor { get; set; }

	// Contract Mapped
	public int? NumActiveContacts { get; set; }

	// Contract Mapped
	public string ParentDunsNumber { get; set; }

	// Contract Mapped
	public string DBAName { get; set; }

	// Contract Mapped
	public Guid ParentCompanyGUID { get; set; }

	// Contract Mapped
	public string StatusNote { get; set; }

	// Contract Mapped
	public string TickerSymbol { get; set; }

	// Contract Mapped
	public string MDMMasterID { get; set; }

	public int? AddedbyEmployeeID { get; set; }

	public int? UpdatedbyEmployeeID { get; set; }

	public string GovernmentAffiliationType { get; set; }

	// Contract Mapped
	public string Ranking { get; set; }
}

public class CompanyItemOffice
{
	public Guid CompanyOfficeGUID { get; set; }

	public Guid CompanyGUID { get; set; }

	public string CompanyOfficeName { get; set; }

	public DateTime? DateAdded { get; set; }

	public DateTime? LastUpdated { get; set; }

	public int? AddedbyEmployeeID { get; set; }

	public int? UpdatedbyEmployeeID { get; set; }

	public CompanyItemAddress[] addresses { get; set; }

	[AutoMapper.IgnoreMap]
	public CompanyItemCommunication[] communications { get; set; }
}

public class CompanyItemDnbInfo
{
	// Contract Mapped
	public string DnBEmployeeQuantityAllLocations { get; set; }

	// Contract Mapped
	public string DnBLegalStructure { get; set; }

	// Contract Mapped
	public string DnBStandardIndustryCode { get; set; }

	// Contract Mapped
	public string DnBHooversIndustryCode { get; set; }

	// Contract Mapped
	public string DnBConfidenceCode { get; set; }

	// Contract Mapped
	public string DnBTelephone { get; set; }

	// Contract Mapped
	public string DnBControlYear { get; set; }

	// Contract Mapped
	public string DnBYearlyRevenueCurrency { get; set; }

	// Contract Mapped
	public string DnBYearlyRevenue { get; set; }

	// Contract Mapped
	public string DnBStandaloneIndicator { get; set; }

	// Contract Mapped
	public string DnBTradeStyleName { get; set; }

	// Contract Mapped
	public string DnBDunsNumber { get; set; }

	// Contract Mapped
	public string DnBStandardIndustryNames { get; set; }

	// Contract Mapped
	public DateTime? DateAdded { get; set; }

	// Contract Mapped
	public string DnBAccountantName { get; set; }

	// Contract Mapped
	public string DnBAgentIndicator { get; set; }

	// Contract Mapped
	public string DnBCompanyPrincipalFirstName { get; set; }

	// Contract Mapped
	public string DnBCompanyPrincipalJobTitle { get; set; }

	// Contract Mapped
	public string DnBCompanyPrincipalLastName { get; set; }

	// Contract Mapped
	public string DnBEmployeeQuantitySingleLocation { get; set; }

	// Contract Mapped
	public string DnBExportIndicator { get; set; }

	// Contract Mapped
	public string DnBFamilyTreeHierarchyLevel { get; set; }

	// Contract Mapped
	public string DnBFaxNumber { get; set; }

	// Contract Mapped
	public string DnBMatchDataProfile { get; set; }

	// Contract Mapped
	public string DnBMatchGrade { get; set; }

	// Contract Mapped
	public string DnBMatchScore { get; set; }

	public DateTime? LastUpdated { get; set; }

	public int? AddedbyEmployeeID { get; set; }

	public int? UpdatedbyEmployeeID { get; set; }
}

#endregion

#region Contract Models

public class CompanyEntry
{
	// Cosmos Mapped
	public virtual string Status { get; set; }

	// Contract Mapped
	public virtual int StatusId { get; set; }

	// Contract Mapped
	public virtual int GovernmentAffiliationId { get; set; }

	// Command Mapped [Same Property Name]
	// Cosmos Mapped (companyInfo)
	public virtual string CompanyName { get; set; }

	// Command Mapped 
	// Cosmos Mapped (company, address)
	public virtual Guid CompanyId { get; set; } = Guid.NewGuid();

	// Command Mapped
	// Cosmos Mapped
	public virtual long? CompanyEspId { get; set; }

	// Cosmos Mapped
	public virtual Guid ParentCompanyId { get; set; }

	// Command Mapped
	// Cosmos Mapped
	public virtual string MdmCode { get; set; }

	// Cosmos Mapped
	public virtual string CompanyLogo { get; set; }

	// Cosmos Mapped
	public virtual string ParentDunsNumber { get; set; }

	// Cosmos Mapped
	public virtual string DbaName { get; set; }

	// Cosmos Mapped
	public virtual int? NumberOfContacts { get; set; }

	// Cosmos Mapped
	public virtual int? NumberOfActiveContacts { get; set; }

	// Cosmos Mapped
	public virtual int? NumberOfDealsAsLender { get; set; }

	// Cosmos Mapped
	public virtual int? NumberOfDealsAsInvestor { get; set; }

	// Cosmos Mapped
	public virtual int? NumberOfActiveDeals { get; set; }

	// Cosmos Mapped
	public virtual int? NumberOfDealsAsClient { get; set; }

	// Cosmos Mapped
	public virtual string StatusNote { get; set; }

	// Cosmos Mapped
	public virtual string TickerSymbol { get; set; }

	// Cosmos Mapped
	public virtual string MdmMasterId { get; set; }

	// Cosmos Mapped
	public virtual string Ranking { get; set; }

	// Command Mapped
	public virtual CompanyDnbEntry DnbEntry { get; set; }

	public virtual IEnumerable<CompanyRoleEntry> Roles { get; set; }

	public virtual IEnumerable<CompanyAddressEntry> Addresses { get; set; }


	public virtual IEnumerable<CompanyOfficeEntry> Offices { get; set; }

	public virtual DateTime? LastUpdated { get; set; } = DateTime.UtcNow;

	public virtual DateTime? DateAdded { get; set; } = DateTime.UtcNow;

}

public class CompanyOfficeEntry
{
	public virtual Guid OfficeId { get; set; } = Guid.NewGuid();

	public virtual string OfficeName { get; set; }

	public virtual IEnumerable<CompanyAddressEntry> OfficeAddresses { get; set; }

}

public class CompanyRoleEntry
{
	public Guid CompanyId { get; set; } = Guid.NewGuid();

	public int RoleTypeId { get; set; }

}

public class CompanyDnbEntry
{
	// Cosmos Mapped
	public virtual string DnbPrimaryName { get; set; }

	// Cosmos Mapped
	public virtual bool? DnbCheck { get; set; }

	// Cosmos Mapped
	// Contract Mapped
	public virtual string DnbLegalStructure { get; set; }

	// Cosmos Mapped
	// Contract Mapped
	public virtual string DnbStandardIndustryCode { get; set; }

	// Cosmos Mapped
	// Contract Mapped
	public virtual string DnbHooversIndustryCode { get; set; }

	// Cosmos Mapped
	// Contract Mapped
	public virtual string DnbConfIdenceCode { get; set; }

	// Cosmos Mapped
	// Contract Mapped
	public virtual string DnbFamilyTreeHierarchyLevel { get; set; }

	// Cosmos Mapped
	public virtual string DnbTelephone { get; set; }

	// Cosmos Mapped
	public virtual string DnbTradeStyleName { get; set; }

	// Cosmos Mapped
	public virtual string DnbYearlyRevenueCurrency { get; set; }

	// Cosmos Mapped
	// Contract Mapped
	public virtual string DnbCompanyPrincipalJobTitle { get; set; }

	// Cosmos Mapped
	public virtual string DnbYearlyRevenue { get; set; }

	// Cosmos Mapped
	// Contract Mapped
	public virtual string DnbCompanyPrincipalLastName { get; set; }

	// Cosmos Mapped
	public virtual string DnbStandaloneIndicator { get; set; }

	// Cosmos Mapped
	// Contract Mapped
	public virtual string DnbEmployeeQuantityAllLocations { get; set; }

	// Cosmos Mapped
	// Contract Mapped
	public virtual string DnbEmployeeQuantitySingleLocation { get; set; }

	// Cosmos Mapped
	// Contract Mapped
	public virtual string DnbDunsNumber { get; set; }

	// Cosmos Mapped
	// Contract Mapped
	public virtual string DnbFaxNumber { get; set; }

	// Cosmos Mapped
	// Contract Mapped
	public virtual string DnbStandardIndustryNames { get; set; }

	// Cosmos Mapped
	// Contract Mapped
	public virtual string DnbControlYear { get; set; }

	// Cosmos Mapped
	// Contract Mapped
	public virtual string DnbCompanyPrincipalFirstName { get; set; }

	// Cosmos Mapped
	// Contract Mapped
	public virtual string DnbAccountantName { get; set; }

	// Cosmos Mapped
	public virtual string DnbAgentIndicator { get; set; }

	// Cosmos Mapped
	public virtual string DnbExportIndicator { get; set; }

	// Cosmos Mapped
	public virtual string DnbMatchDataProfile { get; set; }

	// Cosmos Mapped
	public virtual string DnbMatchGrade { get; set; }

	// Cosmos Mapped
	public virtual string DnbMatchScore { get; set; }

	// Contract Mapped
	public string DnbDomesticUltimateDunsNumber { get; set; }

	// Contract Mapped
	public virtual string DnbMailingPostalCode { get; set; }

	// Cosmos Mapped
	//public virtual DateTime? DateAdded { get; set; }
}

public class CompanyAddressEntry
{
	// Cosmos Mapped
	public virtual Guid AddressId { get; set; } = Guid.NewGuid();

	// Command Mapped
	// Cosmos Mapped
	public virtual string StreetOne { get; set; }

	// Command Mapped
	// Cosmos Mapped
	public virtual string StreetTwo { get; set; }

	// Command Mapped
	// Cosmos Model Mapped
	public virtual string StreetThree { get; set; }

	// Command Mapped
	// Cosmos Model Mapped
	public virtual string StreetFour { get; set; }

	// Command Mapped
	// Cosmos Model Mapped
	public virtual string City { get; set; }

	// Command Mapped
	// Cosmos Model Mapped
	public virtual string StateOrRegion { get; set; }


	public virtual string Country { get; set; }

	// Cosmos Model Mapped
	public virtual string WorldRegion { get; set; }

	// Command Mapped
	// Cosmos Model Mapped
	public virtual string CountryCode { get; set; }

	// Command Mapped
	// Cosmos Model Mapped
	public virtual string PostalCode { get; set; }

	// Command Mapped
	// Cosmos Model Mapped
	public virtual string CrossStreet { get; set; }

	// Command Mapped
	// Cosmos Model Mapped
	public virtual string Salutation { get; set; }

	// Command Mapped
	// Cosmos Model Mapped
	public virtual string Timezone { get; set; }

	// Command Mapped
	// Cosmos Model Mapped
	public virtual double? Latitude { get; set; }

	// Command Mapped
	// Cosmos Model Mapped
	public virtual double? Longitude { get; set; }


	public virtual int AddressTypeId { get; set; }


}

#endregion