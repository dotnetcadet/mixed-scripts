<Query Kind="Program">
  <NuGetReference>Microsoft.EntityFrameworkCore</NuGetReference>
  <NuGetReference>Microsoft.EntityFrameworkCore.Design</NuGetReference>
  <NuGetReference>Microsoft.EntityFrameworkCore.SqlServer</NuGetReference>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.ValueGeneration</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.ChangeTracking</Namespace>
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
</Query>



async Task Main()
{
	try
	{
		var context = new SampleDbContext();
		
		foreach (var sample in context.Sample)
		{
			sample.Dump();
		}
		var value = new SampleEntity()
		{
			FirstName = "Chase",
			LastName = "Crawford"

		};
		
		context.Add(value);
		
		await context.SaveChangesAsync();
	}
	catch(Exception exception)
	{
		exception.Dump();	
	}
}



public class SampleEntity
{
	public Guid EmployeeId { get; set; }
	
	public string FirstName { get; set; }
	
	public string LastName { get; set; }
	
	public DateTime UpdateDateTime { get; set; }
	
	public DateTime CreationDateTime { get; set; }
}


public class DateTimeGenertator : ValueGenerator
{
	public override bool GeneratesTemporaryValues => throw new NotImplementedException();

	protected override object NextValue([NotNullAttribute] EntityEntry entry)
	{
		throw new NotImplementedException();
	}
}



public class SampleDbContext : DbContext
{
	
	private const string Connection = "Server=(localdb)\\MSSQLLOcalDB;Database=SampleDb;Trusted_Connection=True;";


	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (!optionsBuilder.IsConfigured)
			optionsBuilder.UseSqlServer(Connection);
			
		
	}
	
	public DbSet<SampleEntity> Sample { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{		
		modelBuilder.Entity<SampleEntity>(entity=>
		{
			entity.ToTable("SampleEntity","dbo");
			entity.HasKey(p => p.EmployeeId).HasName("PK__SampleEn__7AD04F11B507B1A3");
			entity.Property(p => p.EmployeeId).HasDefaultValueSql("(newid())");
			entity.Property(p => p.FirstName).IsRequired().HasMaxLength(255).IsUnicode(false);
			entity.Property(p => p.LastName).IsRequired().HasMaxLength(255).IsUnicode(false);
			entity.Property(p=>p.CreationDateTime).IsRequired();
			entity.Property(p=>p.UpdateDateTime).IsRequired();
		});
	}


	public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
	{
		entity.
		return base.Add(entity);
	}
}





