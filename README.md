# PRISM (Project for Reporting, Intelligence & Strategic Management)

## Overview
PRISM is a comprehensive ASP.NET Core MVC 8.0 web application integrated with Power BI dashboards, developed for ConverDyn to manage and report on commercial operations including customers, suppliers, contracts, forecasting, transportation, and financial planning.

## Project Architecture
The solution follows a clean architecture pattern with the following projects:

1. **PRISM.Web** - ASP.NET Core MVC 8.0 web application
   - User interface layer with Razor views, controllers, and view models
   - Power BI dashboard integration
   - User authentication and authorization

2. **PRISM.Core** - Domain models and business logic
   - Domain entities
   - Interfaces for repositories and services
   - Business rules and validation

3. **PRISM.Infrastructure** - Cross-cutting concerns
   - External service integrations (Power BI)
   - Logging, caching, and other utilities
   - Security and user management implementations

4. **PRISM.Data** - Data access layer
   - Entity Framework Core with MySQL provider
   - Database migrations
   - Repository implementations

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 (for running and debugging)
- MySQL Server (or Azure MySQL)

### Database Configuration
- Server: prismdbserver.mysql.database.azure.com
- Admin Login: prismdbadmin
- Password: Wrls_8ome52iyoWrLphi
- Port: 3306
- Schema Name: prism

### Development Setup
1. Clone the repository
2. Open the solution in Visual Studio
3. Run database migrations: `dotnet ef database update`
4. Start the application in Visual Studio with debugging (F5)

## Key Features
- Customer and Supplier Management
- Contract Management with various pricing models
- Forecasting and Reporting
- Transportation Planning
- Production and Capacity Planning
- Borrowing and Deferral Management
- Sales and Position Reports
- User Management with Role-Based Access Control
- Power BI integration for interactive visualizations

## Technology Stack
- ASP.NET Core MVC 8.0
- Entity Framework Core with MySQL
- ASP.NET Core Identity for authentication
- Power BI Embedded
- Azure App Service (for hosting)

## Project Structure
- Controllers: Business logic for handling HTTP requests
- Models: Domain models and view models
- Views: Razor views for UI
- Services: Business logic and external service integration
- Data: Database context and migrations

## Contact
For any questions or issues, please contact the development team at [email@example.com]. 