namespace TradeBridge.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TradeBridge.Core.Entities.Audit;
using TradeBridge.Core.Entities.Contract;
using TradeBridge.Core.Entities.Customer;
using TradeBridge.Core.Entities.Forecasting;
using TradeBridge.Core.Entities.Location;
using TradeBridge.Core.Entities.Production;
using TradeBridge.Core.Entities.Supplier;
using TradeBridge.Core.Entities.Transportation;
using System.Linq.Expressions;

/// <summary>
/// Main database context for the TradeBridge application
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<CustomerContact> CustomerContacts { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<SupplierContact> SupplierContacts { get; set; } = null!;
    public DbSet<Contract> Contracts { get; set; } = null!;
    public DbSet<ContractDelivery> ContractDeliveries { get; set; } = null!;
    public DbSet<ContractBorrowing> ContractBorrowings { get; set; } = null!;
    public DbSet<Location> Locations { get; set; } = null!;
    public DbSet<ProductionForecast> ProductionForecasts { get; set; } = null!;
    public DbSet<TransportationPlan> TransportationPlans { get; set; } = null!;
    public DbSet<TransportationRoute> TransportationRoutes { get; set; } = null!;
    public DbSet<ShippingRate> ShippingRates { get; set; } = null!;
    public DbSet<Forecast> Forecasts { get; set; } = null!;
    public DbSet<ForecastDetail> ForecastDetails { get; set; } = null!;
    public DbSet<UF6BorrowingRecord> UF6BorrowingRecords { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configuration for all entities
        ApplyCustomerConfigurations(modelBuilder);
        ApplySupplierConfigurations(modelBuilder);
        ApplyContractConfigurations(modelBuilder);
        ApplyLocationConfigurations(modelBuilder);
        ApplyProductionConfigurations(modelBuilder);
        ApplyTransportationConfigurations(modelBuilder);
        ApplyForecastingConfigurations(modelBuilder);
        ApplyAuditLogConfigurations(modelBuilder);

        // Add soft delete filter for all entities that inherit from BaseEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                entityType.AddSoftDeleteQueryFilter();
            }
        }
    }

    private void ApplyCustomerConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CustomerCode).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.CustomerCode).IsUnique();

            entity.HasMany(e => e.Contacts)
                  .WithOne(e => e.Customer)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Contracts)
                  .WithOne(e => e.Customer)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CustomerContact>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
        });
    }

    private void ApplySupplierConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SupplierCode).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.SupplierCode).IsUnique();

            entity.HasMany(e => e.Contacts)
                  .WithOne(e => e.Supplier)
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SupplierContact>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
        });
    }

    private void ApplyContractConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ContractNumber).IsRequired().HasMaxLength(30);
            entity.HasIndex(e => e.ContractNumber).IsUnique();
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.EndDate).IsRequired();
            entity.Property(e => e.BaseQuantity).HasPrecision(18, 2);
            entity.Property(e => e.MinQuantity).HasPrecision(18, 2);
            entity.Property(e => e.MaxQuantity).HasPrecision(18, 2);
            entity.Property(e => e.FixedPrice).HasPrecision(18, 2);
            entity.Property(e => e.FixedEscalationRate).HasPrecision(5, 4);

            entity.HasMany(e => e.Deliveries)
                  .WithOne(e => e.Contract)
                  .HasForeignKey(e => e.ContractId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Borrowings)
                  .WithOne(e => e.Contract)
                  .HasForeignKey(e => e.ContractId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ContractDelivery>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ScheduledDate).IsRequired();
            entity.Property(e => e.Quantity).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.ForecastedPrice).HasPrecision(18, 2);
            entity.Property(e => e.ActualPrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<ContractBorrowing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BorrowDate).IsRequired();
            entity.Property(e => e.BorrowedQuantity).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.ScheduledRepaymentDate).IsRequired();
            entity.Property(e => e.RepaidQuantity).HasPrecision(18, 2);
        });
    }

    private void ApplyLocationConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.ShippingRate).HasPrecision(10, 2);
        });
    }

    private void ApplyProductionConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductionForecast>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ForecastDate).IsRequired();
            entity.Property(e => e.PlannedQuantity).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.ActualQuantity).HasPrecision(18, 2);
            entity.Property(e => e.VariancePercent).HasPrecision(5, 2);
        });
    }

    private void ApplyTransportationConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransportationPlan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PlanNumber).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.PlanNumber).IsUnique();
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.EndDate).IsRequired();
            entity.Property(e => e.Carrier).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TransportMode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalQuantity).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.EstimatedCostPerUnit).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.TotalEstimatedCost).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.ActualCost).HasPrecision(18, 2);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.OriginLocation)
                  .WithMany()
                  .HasForeignKey(e => e.OriginLocationId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DestinationLocation)
                  .WithMany()
                  .HasForeignKey(e => e.DestinationLocationId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TransportationRoute>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransportMode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Carrier).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Distance).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.EstimatedTransitTime).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.EstimatedCost).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.ActualCost).HasPrecision(18, 2);
            entity.Property(e => e.ScheduledDeparture).IsRequired();
            entity.Property(e => e.ScheduledArrival).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.TransportationPlan)
                  .WithMany(p => p.Routes)
                  .HasForeignKey(e => e.TransportationPlanId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.FromLocation)
                  .WithMany()
                  .HasForeignKey(e => e.FromLocationId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ToLocation)
                  .WithMany()
                  .HasForeignKey(e => e.ToLocationId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ShippingRate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransportMode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EffectiveDate).IsRequired();
            entity.Property(e => e.BaseRate).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.FuelSurchargePercent).HasPrecision(5, 2).IsRequired();
            entity.Property(e => e.AdditionalFees).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.MinimumQuantity).HasPrecision(18, 2);
            entity.Property(e => e.Carrier).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ReferenceNumber).HasMaxLength(50);

            entity.HasOne(e => e.OriginLocation)
                  .WithMany()
                  .HasForeignKey(e => e.OriginLocationId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DestinationLocation)
                  .WithMany()
                  .HasForeignKey(e => e.DestinationLocationId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Create a unique index for origin, destination, transport mode, and effective date
            entity.HasIndex(e => new { e.OriginLocationId, e.DestinationLocationId, e.TransportMode, e.EffectiveDate }).IsUnique();
        });
    }

    private void ApplyForecastingConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Forecast>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ForecastDate).IsRequired();
            entity.Property(e => e.PeriodType).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.ApprovedBy).HasMaxLength(100);
            entity.Property(e => e.LastModifiedBy).HasMaxLength(100);

            entity.HasMany<ForecastDetail>()
                  .WithOne(fd => fd.Forecast)
                  .HasForeignKey(fd => fd.ForecastId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ForecastDetail>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PeriodStart).IsRequired();
            entity.Property(e => e.PeriodEnd).IsRequired();
            entity.Property(e => e.ForecastValue).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.ActualValue).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(500);
        });

        modelBuilder.Entity<UF6BorrowingRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransactionDate).IsRequired();
            entity.Property(e => e.Quantity).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.Unit).HasMaxLength(10).IsRequired();
            entity.Property(e => e.DueDate).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.TransactionType).IsRequired();

            entity.HasOne(e => e.RelatedRecord)
                  .WithMany()
                  .HasForeignKey(e => e.RelatedRecordId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ApplyAuditLogConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityId).IsRequired();
            entity.Property(e => e.Action).IsRequired();
            entity.Property(e => e.Details).HasMaxLength(1000);
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
        });
    }
}

// Extension method for applying soft delete filters
public static class ModelBuilderExtensions
{
    public static void AddSoftDeleteQueryFilter(this IMutableEntityType entityType)
    {
        var methodToCall = typeof(ModelBuilderExtensions)
            .GetMethod(nameof(GetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            ?.MakeGenericMethod(entityType.ClrType);

        if (methodToCall != null)
        {
            var filter = methodToCall.Invoke(null, Array.Empty<object>());
            entityType.SetQueryFilter((System.Linq.Expressions.LambdaExpression)filter!);
        }
    }

    private static LambdaExpression GetSoftDeleteFilter<TEntity>() where TEntity : BaseEntity
    {
        Expression<Func<TEntity, bool>> filter = x => !x.IsDeleted;
        return filter;
    }
}