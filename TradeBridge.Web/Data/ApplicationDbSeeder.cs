using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TradeBridge.Core.Entities.Contract;
using TradeBridge.Core.Entities.Customer;
using TradeBridge.Core.Entities.Forecasting;
using TradeBridge.Core.Entities.Location;
using TradeBridge.Core.Entities.Supplier;
using TradeBridge.Core.Enums;
using TradeBridge.Data;

namespace TradeBridge.Web.Data;

public class ApplicationDbSeeder
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ApplicationDbSeeder> _logger;

    public ApplicationDbSeeder(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext dbContext,
        ILogger<ApplicationDbSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        // Ensure database is created and migrated
        await _dbContext.Database.MigrateAsync();
        
        // Ensure we have the AuditLog table
        await EnsureAuditLogTableExistsAsync();

        // Seed roles and users
        await SeedRolesAsync();
        await SeedDefaultUserAsync();
        
        // Seed initial data
        await SeedCustomersAsync();
        await SeedSuppliersAsync();
        await SeedLocationsAsync();
        await SeedContractsAsync();
        await SeedForecastsAsync();
        await SeedUF6BorrowingRecordsAsync();
    }

    private async Task EnsureAuditLogTableExistsAsync()
    {
        try
        {
            // Check if the AuditLog table exists
            bool tableExists = false;
            
            try
            {
                // Try accessing the AuditLogs DbSet
                var count = await _dbContext.AuditLogs.CountAsync();
                tableExists = true;
            }
            catch (Exception)
            {
                tableExists = false;
            }
            
            if (!tableExists)
            {
                // Execute SQL to create the table
                var sql = @"
                CREATE TABLE IF NOT EXISTS AuditLogs (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    UserId VARCHAR(100) NOT NULL,
                    EntityName VARCHAR(100) NOT NULL,
                    EntityId INT NOT NULL,
                    Action INT NOT NULL,
                    Details VARCHAR(1000) NULL,
                    Timestamp DATETIME NOT NULL,
                    IpAddress VARCHAR(50) NULL,
                    UserAgent VARCHAR(500) NULL
                )";
                
                await _dbContext.Database.ExecuteSqlRawAsync(sql);
                _logger.LogInformation("AuditLogs table created successfully.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ensuring AuditLog table exists.");
        }
    }

    private async Task SeedRolesAsync()
    {
        string[] roles = { "Admin", "Manager", "Viewer" };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
                _logger.LogInformation("Created role: {Role}", role);
            }
        }
    }

    private async Task SeedDefaultUserAsync()
    {
        // Create admin user
        var adminUser = new IdentityUser
        {
            UserName = "admin@tradebridge.com",
            Email = "admin@tradebridge.com",
            EmailConfirmed = true
        };

        await CreateUserIfNotExists(adminUser, "Admin@123456", "Admin");

        // Create manager user
        var managerUser = new IdentityUser
        {
            UserName = "manager@tradebridge.com",
            Email = "manager@tradebridge.com",
            EmailConfirmed = true
        };

        await CreateUserIfNotExists(managerUser, "Manager@123456", "Manager");

        // Create viewer user
        var viewerUser = new IdentityUser
        {
            UserName = "viewer@tradebridge.com",
            Email = "viewer@tradebridge.com",
            EmailConfirmed = true
        };

        await CreateUserIfNotExists(viewerUser, "Viewer@123456", "Viewer");
    }

    private async Task CreateUserIfNotExists(IdentityUser user, string password, string role)
    {
        if (string.IsNullOrEmpty(user.Email))
        {
            _logger.LogError("Cannot create user with null or empty email");
            return;
        }

        var existingUser = await _userManager.FindByEmailAsync(user.Email);

        if (existingUser == null)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                _logger.LogInformation("Created user: {Email} with role: {Role}", user.Email, role);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Error creating user {Email}: {Errors}", user.Email, errors);
            }
        }
    }

    private async Task SeedLocationsAsync()
    {
        if (!_dbContext.Locations.Any())
        {
            var locations = new List<Location>
            {
                new Location
                {
                    Name = "Metropolis Plant",
                    Code = "MET-01",
                    Address = "123 Industrial Way",
                    City = "Metropolis",
                    State = "IL",
                    Country = "USA",
                    PostalCode = "62960",
                    ShippingRate = 1250.00M,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                },
                new Location
                {
                    Name = "Paducah Facility",
                    Code = "PAD-01",
                    Address = "4501 Shawnee College Rd",
                    City = "Paducah",
                    State = "KY",
                    Country = "USA",
                    PostalCode = "42001",
                    ShippingRate = 1100.00M,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                },
                new Location
                {
                    Name = "Portsmouth Site",
                    Code = "POR-01",
                    Address = "3930 US-23",
                    City = "Portsmouth",
                    State = "OH",
                    Country = "USA",
                    PostalCode = "45662",
                    ShippingRate = 1350.00M,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                }
            };

            await _dbContext.Locations.AddRangeAsync(locations);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} locations", locations.Count);
        }
    }

    private async Task SeedCustomersAsync()
    {
        if (!_dbContext.Customers.Any())
        {
            var customers = new List<Customer>
            {
                new Customer
                {
                    Name = "Nucleon Energy Corporation",
                    CustomerCode = "NEC-001",
                    Address = "1000 Atomic Blvd",
                    City = "Charlotte",
                    State = "NC",
                    Country = "USA",
                    PostalCode = "28255",
                    PhoneNumber = "704-555-1000",
                    Email = "contact@nucleonenergy.com",
                    Website = "www.nucleonenergy.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system",
                    Contacts = new List<CustomerContact>
                    {
                        new CustomerContact
                        {
                            FirstName = "John",
                            LastName = "Isotope",
                            Title = "Procurement Director",
                            PhoneNumber = "704-555-1001",
                            Email = "j.isotope@nucleonenergy.com",
                            IsPrimary = true,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "system"
                        }
                    }
                },
                new Customer
                {
                    Name = "Fusion Power Systems",
                    CustomerCode = "FPS-001",
                    Address = "4200 Nuclear Way",
                    City = "San Diego",
                    State = "CA",
                    Country = "USA",
                    PostalCode = "92121",
                    PhoneNumber = "619-555-2000",
                    Email = "info@fusionpower.com",
                    Website = "www.fusionpower.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system",
                    Contacts = new List<CustomerContact>
                    {
                        new CustomerContact
                        {
                            FirstName = "Lisa",
                            LastName = "Neutron",
                            Title = "CEO",
                            PhoneNumber = "619-555-2001",
                            Email = "l.neutron@fusionpower.com",
                            IsPrimary = true,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "system"
                        }
                    }
                }
            };

            await _dbContext.Customers.AddRangeAsync(customers);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} customers", customers.Count);
        }
    }

    private async Task SeedSuppliersAsync()
    {
        if (!_dbContext.Suppliers.Any())
        {
            var suppliers = new List<Supplier>
            {
                new Supplier
                {
                    Name = "Uranium One, Inc.",
                    SupplierCode = "U1-001",
                    Address = "333 Bay Street, Suite 1200",
                    City = "Toronto",
                    State = "ON",
                    Country = "Canada",
                    PostalCode = "M5H 2R2",
                    PhoneNumber = "416-555-3000",
                    Email = "info@uranium1.com",
                    Website = "www.uranium1.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system",
                    Contacts = new List<SupplierContact>
                    {
                        new SupplierContact
                        {
                            FirstName = "Robert",
                            LastName = "Yellowcake",
                            Title = "Supply Chain Director",
                            PhoneNumber = "416-555-3001",
                            Email = "r.yellowcake@uranium1.com",
                            IsPrimary = true,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "system"
                        }
                    }
                },
                new Supplier
                {
                    Name = "Cameco Corporation",
                    SupplierCode = "CAM-001",
                    Address = "2121 11th Street West",
                    City = "Saskatoon",
                    State = "SK",
                    Country = "Canada",
                    PostalCode = "S7M 1J3",
                    PhoneNumber = "306-555-4000",
                    Email = "contact@cameco.com",
                    Website = "www.cameco.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system",
                    Contacts = new List<SupplierContact>
                    {
                        new SupplierContact
                        {
                            FirstName = "Sarah",
                            LastName = "Enrichment",
                            Title = "VP of Sales",
                            PhoneNumber = "306-555-4001",
                            Email = "s.enrichment@cameco.com",
                            IsPrimary = true,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "system"
                        }
                    }
                }
            };

            await _dbContext.Suppliers.AddRangeAsync(suppliers);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} suppliers", suppliers.Count);
        }
    }

    private async Task SeedContractsAsync()
    {
        if (!_dbContext.Contracts.Any())
        {
            var customers = await _dbContext.Customers.ToListAsync();
            var suppliers = await _dbContext.Suppliers.ToListAsync();
            var locations = await _dbContext.Locations.ToListAsync();

            if (customers.Count == 0 || suppliers.Count == 0 || locations.Count == 0)
            {
                _logger.LogWarning("Cannot seed contracts: customers, suppliers, or locations not seeded yet");
                return;
            }

            var contracts = new List<Contract>
            {
                // Customer Contracts
                new Contract
                {
                    ContractNumber = "CUST-2025-001",
                    ContractType = ContractType.Customer,
                    CustomerId = customers[0].Id,
                    StartDate = new DateTime(2025, 1, 1),
                    EndDate = new DateTime(2027, 12, 31),
                    BaseQuantity = 300000,
                    MinQuantity = 270000,
                    MaxQuantity = 330000,
                    PricingType = PricingType.FixedEscalation,
                    FixedPrice = 55.25M,
                    FixedEscalationRate = 0.03M,
                    ContractTerms = "Standard terms with quarterly deliveries. Price escalation of 3% annually.",
                    HasDeferralOption = true,
                    IsBorrowingAllowed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system",
                    Deliveries = new List<ContractDelivery>
                    {
                        new ContractDelivery
                        {
                            ScheduledDate = new DateTime(2025, 3, 15),
                            Quantity = 25000,
                            ForecastedPrice = 55.25M,
                            LocationId = locations[0].Id,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "system"
                        },
                        new ContractDelivery
                        {
                            ScheduledDate = new DateTime(2025, 6, 15),
                            Quantity = 25000,
                            ForecastedPrice = 55.25M,
                            LocationId = locations[0].Id,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "system"
                        },
                        new ContractDelivery
                        {
                            ScheduledDate = new DateTime(2025, 9, 15),
                            Quantity = 25000,
                            ForecastedPrice = 55.25M,
                            LocationId = locations[0].Id,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "system"
                        }
                    }
                },
                new Contract
                {
                    ContractNumber = "CUST-2025-002",
                    ContractType = ContractType.Customer,
                    CustomerId = customers[1].Id,
                    StartDate = new DateTime(2025, 3, 1),
                    EndDate = new DateTime(2026, 2, 28),
                    BaseQuantity = 150000,
                    MinQuantity = 140000,
                    MaxQuantity = 160000,
                    PricingType = PricingType.FlatRate,
                    FixedPrice = 58.50M,
                    ContractTerms = "Flat rate pricing with bi-monthly deliveries.",
                    HasDeferralOption = false,
                    IsBorrowingAllowed = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system",
                    Deliveries = new List<ContractDelivery>
                    {
                        new ContractDelivery
                        {
                            ScheduledDate = new DateTime(2025, 5, 1),
                            Quantity = 25000,
                            ForecastedPrice = 58.50M,
                            LocationId = locations[1].Id,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "system"
                        },
                        new ContractDelivery
                        {
                            ScheduledDate = new DateTime(2025, 7, 1),
                            Quantity = 25000,
                            ForecastedPrice = 58.50M,
                            LocationId = locations[1].Id,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "system"
                        }
                    }
                },

                // Supplier Contracts
                new Contract
                {
                    ContractNumber = "SUPP-2025-001",
                    ContractType = ContractType.Supplier,
                    SupplierId = suppliers[0].Id,
                    StartDate = new DateTime(2025, 1, 1),
                    EndDate = new DateTime(2026, 12, 31),
                    BaseQuantity = 500000,
                    MinQuantity = 450000,
                    MaxQuantity = 550000,
                    PricingType = PricingType.FlatRate,
                    FixedPrice = 45.75M,
                    ContractTerms = "Long-term supply agreement with monthly deliveries.",
                    HasDeferralOption = true,
                    IsBorrowingAllowed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system",
                    Deliveries = new List<ContractDelivery>
                    {
                        new ContractDelivery
                        {
                            ScheduledDate = new DateTime(2025, 2, 1),
                            Quantity = 40000,
                            ForecastedPrice = 45.75M,
                            LocationId = locations[2].Id,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "system"
                        },
                        new ContractDelivery
                        {
                            ScheduledDate = new DateTime(2025, 3, 1),
                            Quantity = 40000,
                            ForecastedPrice = 45.75M,
                            LocationId = locations[2].Id,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "system"
                        }
                    }
                },
                new Contract
                {
                    ContractNumber = "SUPP-2025-002",
                    ContractType = ContractType.Supplier,
                    SupplierId = suppliers[1].Id,
                    StartDate = new DateTime(2025, 4, 1),
                    EndDate = new DateTime(2026, 3, 31),
                    BaseQuantity = 200000,
                    MinQuantity = 180000,
                    MaxQuantity = 220000,
                    PricingType = PricingType.Market,
                    ContractTerms = "Market-based pricing with quarterly volume adjustments.",
                    HasDeferralOption = false,
                    IsBorrowingAllowed = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                }
            };

            await _dbContext.Contracts.AddRangeAsync(contracts);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} contracts with {DeliveryCount} deliveries", 
                contracts.Count, 
                contracts.Sum(c => c.Deliveries?.Count ?? 0));
        }
    }

    private async Task SeedForecastsAsync()
    {
        if (!_dbContext.Forecasts.Any())
        {
            var contracts = await _dbContext.Contracts.ToListAsync();
            var customers = await _dbContext.Customers.ToListAsync();

            if (contracts.Count == 0 || customers.Count == 0)
            {
                _logger.LogWarning("Cannot seed forecasts: contracts or customers not seeded yet");
                return;
            }

            var forecasts = new List<Forecast>
            {
                new Forecast
                {
                    Title = "Revenue Forecast 2025",
                    ForecastDate = DateTime.Today,
                    PeriodType = ForecastPeriodType.Quarterly,
                    Type = ForecastType.Revenue,
                    Description = "Quarterly revenue forecast for all customer contracts for 2025.",
                    IsActive = true,
                    IsApproved = true,
                    ApprovedBy = "admin@tradebridge.com",
                    ApprovalDate = DateTime.Today.AddDays(-5),
                    CreatedDate = DateTime.Today.AddDays(-30),
                    CreatedBy = "admin@tradebridge.com"
                },
                new Forecast
                {
                    Title = "UF6 Quantity Forecast 2025",
                    ForecastDate = DateTime.Today,
                    PeriodType = ForecastPeriodType.Monthly,
                    Type = ForecastType.Quantity,
                    Description = "Monthly quantity forecast for UF6 deliveries in 2025.",
                    IsActive = true,
                    IsApproved = false,
                    CreatedDate = DateTime.Today.AddDays(-15),
                    CreatedBy = "manager@tradebridge.com"
                },
                new Forecast
                {
                    Title = "Price Forecast 2025-2026",
                    ForecastDate = DateTime.Today,
                    PeriodType = ForecastPeriodType.Quarterly,
                    Type = ForecastType.Price,
                    Description = "Projected UF6 prices for 2025-2026.",
                    IsActive = true,
                    IsApproved = false,
                    CreatedDate = DateTime.Today.AddDays(-10),
                    CreatedBy = "manager@tradebridge.com"
                }
            };

            await _dbContext.Forecasts.AddRangeAsync(forecasts);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} forecasts", forecasts.Count);

            // Add forecast details
            var forecastDetails = new List<ForecastDetail>();

            // Revenue forecast details
            var revenueForecast = forecasts.First(f => f.Type == ForecastType.Revenue);
            for (int quarter = 1; quarter <= 4; quarter++)
            {
                var startDate = new DateTime(2025, (quarter - 1) * 3 + 1, 1);
                var endDate = startDate.AddMonths(3).AddDays(-1);

                foreach (var contract in contracts.Where(c => c.ContractType == ContractType.Customer))
                {
                    var quarterlyRevenue = contract.BaseQuantity / 4 * (contract.FixedPrice ?? 50.0m);
                    
                    forecastDetails.Add(new ForecastDetail
                    {
                        ForecastId = revenueForecast.Id,
                        PeriodStart = startDate,
                        PeriodEnd = endDate,
                        ContractId = contract.Id,
                        CustomerId = contract.CustomerId,
                        ForecastValue = quarterlyRevenue,
                        Notes = $"Q{quarter} 2025 revenue forecast for {contract.ContractNumber}",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "system"
                    });
                }
            }

            // Quantity forecast details
            var quantityForecast = forecasts.First(f => f.Type == ForecastType.Quantity);
            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(2025, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                foreach (var contract in contracts)
                {
                    var monthlyQuantity = contract.BaseQuantity / 12;
                    
                    forecastDetails.Add(new ForecastDetail
                    {
                        ForecastId = quantityForecast.Id,
                        PeriodStart = startDate,
                        PeriodEnd = endDate,
                        ContractId = contract.Id,
                        CustomerId = contract.ContractType == ContractType.Customer ? contract.CustomerId : null,
                        ForecastValue = monthlyQuantity,
                        Notes = $"{startDate:MMM yyyy} quantity forecast for {contract.ContractNumber}",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "system"
                    });
                }
            }

            // Price forecast details
            var priceForecast = forecasts.First(f => f.Type == ForecastType.Price);
            for (int quarter = 1; quarter <= 8; quarter++) // 8 quarters for 2 years
            {
                int year = 2025 + (quarter - 1) / 4;
                int quarterOfYear = ((quarter - 1) % 4) + 1;
                var startDate = new DateTime(year, (quarterOfYear - 1) * 3 + 1, 1);
                var endDate = startDate.AddMonths(3).AddDays(-1);

                var basePrice = 55.0m;
                var inflationFactor = 1.0m + (0.005m * (quarter - 1)); // 0.5% increase per quarter
                
                forecastDetails.Add(new ForecastDetail
                {
                    ForecastId = priceForecast.Id,
                    PeriodStart = startDate,
                    PeriodEnd = endDate,
                    ForecastValue = basePrice * inflationFactor,
                    Notes = $"Q{quarterOfYear} {year} price forecast",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                });
            }

            await _dbContext.ForecastDetails.AddRangeAsync(forecastDetails);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} forecast details", forecastDetails.Count);
        }
    }

    private async Task SeedUF6BorrowingRecordsAsync()
    {
        if (!_dbContext.UF6BorrowingRecords.Any())
        {
            var contracts = await _dbContext.Contracts
                .Where(c => c.IsBorrowingAllowed)
                .ToListAsync();

            if (contracts.Count == 0)
            {
                _logger.LogWarning("Cannot seed UF6 borrowing records: no contracts with borrowing allowed");
                return;
            }

            var borrowingRecords = new List<UF6BorrowingRecord>();

            // Borrow transactions
            var borrowContract = contracts.FirstOrDefault(c => c.ContractType == ContractType.Customer);
            if (borrowContract != null)
            {
                var borrowRecord1 = new UF6BorrowingRecord
                {
                    ContractId = borrowContract.Id,
                    CustomerId = borrowContract.CustomerId,
                    TransactionType = BorrowingTransactionType.Borrow,
                    TransactionDate = DateTime.Today.AddMonths(-3),
                    Quantity = 15000,
                    Unit = "KgU",
                    DueDate = DateTime.Today.AddMonths(3),
                    IsSettled = false,
                    Notes = "Borrowed to meet delivery obligations for upcoming refueling outage",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                };
                borrowingRecords.Add(borrowRecord1);

                var borrowRecord2 = new UF6BorrowingRecord
                {
                    ContractId = borrowContract.Id,
                    CustomerId = borrowContract.CustomerId,
                    TransactionType = BorrowingTransactionType.Borrow,
                    TransactionDate = DateTime.Today.AddMonths(-6),
                    Quantity = 10000,
                    Unit = "KgU",
                    DueDate = DateTime.Today.AddMonths(-2),
                    IsSettled = true,
                    SettlementDate = DateTime.Today.AddMonths(-1),
                    Notes = "Borrowed for temporary supply shortage",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                };
                borrowingRecords.Add(borrowRecord2);

                // Repayment for borrowRecord2
                var repayRecord = new UF6BorrowingRecord
                {
                    ContractId = borrowContract.Id,
                    CustomerId = borrowContract.CustomerId,
                    TransactionType = BorrowingTransactionType.Repay,
                    TransactionDate = DateTime.Today.AddMonths(-1),
                    Quantity = 10000,
                    Unit = "KgU",
                    DueDate = DateTime.Today.AddMonths(-2),
                    IsSettled = true,
                    SettlementDate = DateTime.Today.AddMonths(-1),
                    Notes = "Repayment of borrowed material",
                    RelatedRecordId = null, // Will be updated after records are added
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                };
                borrowingRecords.Add(repayRecord);
            }

            await _dbContext.UF6BorrowingRecords.AddRangeAsync(borrowingRecords);
            await _dbContext.SaveChangesAsync();

            // Update the related record ID for the repayment record
            var updatedBorrowRecords = await _dbContext.UF6BorrowingRecords.ToListAsync();
            var settledBorrowRecord = updatedBorrowRecords.FirstOrDefault(r => 
                r.TransactionType == BorrowingTransactionType.Borrow && 
                r.IsSettled);
            
            var existingRepayRecord = updatedBorrowRecords.FirstOrDefault(r => 
                r.TransactionType == BorrowingTransactionType.Repay);
            
            if (settledBorrowRecord != null && existingRepayRecord != null)
            {
                existingRepayRecord.RelatedRecordId = settledBorrowRecord.Id;
                await _dbContext.SaveChangesAsync();
            }

            _logger.LogInformation("Seeded {Count} UF6 borrowing records", borrowingRecords.Count);
        }
    }
}