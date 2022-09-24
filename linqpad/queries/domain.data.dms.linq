<Query Kind="Program" />


/*
	Entity: 
		An entity represents a single instance of your domain object saved into 
		the database as a record. It has some attributes that we represent as columns in our tables.

	Model: 
		A model typically represents a real world object that is related to the 
		problem or domain space. In programming, we create classes to represent objects. 
		These classes, known as models, have some properties and methods (defining objects behavior).

	ViewModel: 
		The term ViewModel originates from the MVVM (Model View ViewModel) design pattern. There 
		are instances in which the data to be rendered by the view comes from two different 
		objects. In such scenarios, we create a model class which consists of all properties 
		required by the view. It’s not a domain model but a ViewModel because, a specific 
		view uses it. Also, it doesn’t represent a real world object.

	DataModel: 
		In order to solve a problem, objects interact with each other. Some objects share 
		a relationship among them and consequently, form a data model that represents the 
		objects and the relationship between them.

	In an application managing customer orders, for instance, if we have a customer and order 
	object then these objects share a many to many relationship between them. The data model 
	is eventually dependent on the way our objects interact with each other. In a database, 
	we see the data model as a network of tables referring to some other tables.

*/

void Main()
{
	var domain = default(IDomainBuilder)
		.AddDataModel(builder =>
		{
			builder
				.AddDomainValueObject<Employee>(descritpor =>
				{
					descritpor.Ignore(p => p.FirstName);
				})
				.AddDomainValueObject<EmployeeAddress>(descriptor =>
				{
					
				});
			builder.AddDomainModel<OnBoardingEmployee>(descriptor =>
			{
				descriptor
			});
		})
		.Build();
	
	
}

// Value Objects Test
public class Employee
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
}
public class EmployeeAddress 
{
	public string StreetOne { get; set; }
	public string StreetTwo { get; set; }
}

// Entity
public class EmployeeEntity : Employee
{
	public Guid EmployeeId { get; set; }
}

// Models
public class OnBoardingEmployee
{
	public Employee Employee { get; set; }
	public EmployeeAddress MailingAddress { get; set; }
	public EmployeeAddress PrimaryAddress { get; set; }
	
}


#region Gerneral 
/*
	- Infrastructure Layer (Usually Holds Entities)
		-> Database 
		
	- Domain Layer (Usually Holds Modeles & Value Objects)
	
	
	Namespaces: 
	
	Assimalign.DomainData
	Assimalign.DomainData.Application
	Assimalign.DomainData.Application
	Assimalign.DomainData.Infrastructure

*/




// ValueObjects <-- Carry our data
// Entity/Record <-- Query & Store our data (A database record [The entity/record should be database agnostic. Meaning we should be able to use the same record ])
// Model <-- Represents interfaces for our domain operations (A model can hold multiple entities and value objects)

// DMS -> Data Model Structure
public interface IDomain
{
	/// <summary>
	/// The name of the domain.
	/// </summary>
	string Name { get; }
	
	/// <summary>
	/// 
	/// </summary>
	IDomainObject[] Objects { get; }
	
	/// <summary>
	/// 
	/// </summary>
	IDomainServiceProvider ServiceProvider { get; }
	
	
	IDomainModel[] GetModels();
	IDomainEntity[] GetEntities();
	IDomainValueObject[] GetValueObjects();
}

public interface IDomainServiceProvider : IServiceProvider
{
	
}

public interface IDomainBuilder
{
	IDomainBuilder AddDataModel(IEnumerable<IDomainObject> objects);
	IDomainBuilder AddDataModel(Action<IDomainDataModelBuilder> configure);
	IDomainBuilder AddFeature();
	IDomain Build();
}

public interface IDomainDataModelBuilder
{
	IDomainDataModelBuilder AddDomainModel<T>();
	IDomainDataModelBuilder AddDomainModel<T>(Action<IDomainModelDescriptor<T>> descriptor);
	IDomainDataModelBuilder AddDomainModel(string modelName, Action<IDomainModelDescriptor> descriptor);
	IDomainDataModelBuilder AddDomainEntity<T>();
	IDomainDataModelBuilder AddDomainEntity<T>(Action<IDomainEntityDescriptor<T>> descriptor);
	IDomainDataModelBuilder AddDomainEntity(string entityName, Action<IDomainEntityDescriptor> descriptor);
	IDomainDataModelBuilder AddDomainValueObject<T>();
	IDomainDataModelBuilder AddDomainValueObject<T>(Action<IDomainValueObjectDescriptor<T>> descriptor);
}

public enum DomainObjectType
{
	None,
	Model,
	Entity,
	ValueObject
}
public interface IDomainObject
{
	DomainObjectType ObjectType { get; }
}

/// <summary>
/// 
/// </summary>
public interface IDomainModel : IDomainObject
{
	
}
public interface IDomainModelDescriptor
{
	
}
public interface IDomainModelDescriptor<T>
{
	
}
public interface IDomainEntity : IDomainObject
{
	string Name { get; }
}
public interface IDomainEntityDescriptor
{
	IDomainEntityDescriptor Ignore(string memberName);
}
public interface IDomainEntityDescriptor<T> : IDomainEntityDescriptor
{

	IDomainEntityDescriptor<T> Ignore(Expression<Func<T, object>> expression);
	IDomainEntityMemberDescriptor AddMember(string name)
}
public interface IDomainEntityMemberDescriptor
{
	
}
public interface IDomainValueObject  : IDomainObject
{
	
}
public interface IDomainValueObjectDescriptor
{
	
}
public interface IDomainValueObjectDescriptor<T> : IDomainValueObjectDescriptor
{
	IDomainValueObjectDescriptor<T> Ignore(Expression<Func<T, object>> expression);
}

public interface IDomainAttribute
{
	
}



#endregion


#region Azure Adaptor



public sealed class DomainModelAttribute : Attribute 
{
	
}


#endregion