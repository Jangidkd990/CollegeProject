# PRISM Project - Requirement Specification

## Project Title
ConverDyn PRISM (Project for Reporting, Intelligence & Strategic Management)

## Objective
Develop a scalable, secure, and auditable **ASP.NET Core MVC 8.0 Web Application** integrated with **Power BI dashboards** to manage and report on ConverDyn's commercial operations including customers, suppliers, contracts, forecasting, transportation, and financial planning.

## Scope
- Replace Excel-based tracking with centralized web-based solution.
- Embed Power BI for interactive data visualizations.
- Enable secure role-based access and full auditability.
- Lay the foundation for future integration with AI/ML models and external data sources.

## Functional Requirements

### 1. Customer Management
- Customer Profiles (basic info, contacts)
- Contract Management (pricing models, quantity terms, special clauses)
- Delivery Forecasting (monthly, annual)
- Revenue and Pricing Dashboards

### 2. Supplier & Procurement Management
- Supplier Profiles
- Procurement Forecasting
- Cost Impact Analysis (pass-through vs margin deals)

### 3. Contract Module
- Master Contract Repository (customers and suppliers)
- Pricing Types: GDP-indexed, Fixed Escalation, Flat Rate
- Commitment Quantities: Base, Min-Max Range
- Contract Document Upload (PDF with metadata)
- Contract Option/Deferral Flags

### 4. Forecasting & Reporting
- Quantity and Price Forecast (Monthly/Yearly)
- Delivery Revenue Reports
- UF6 Borrowing and Repayment Tracking
- UX Market Data Integration (Future Scope)

### 5. Transportation Planning
- Delivery Planning by Locations (3 facilities)
- Forecasted Shipping Rate Tables

### 6. Production & Capacity Planning
- Quarterly Production Forecasts
- Production vs Delivery Alignment

### 7. Borrowing and Deferral Management
- Borrowed UF6 Tracking
- Customer Deferral Management

### 8. Sales and Position Reports
- Summary and Detailed Sales/Position Views
- Region-wise and Contract-wise Breakdown

### 9. User Management and Security
- User Authentication (ASP.NET Core Identity)
- Role-Based Access Control (Admin, Manager, Viewer)
- Audit Logs for all Create/Update/Delete Operations

### 10. Power BI Integration
- Embed Power BI Reports/Dashboards in the Web App
- Secure Embedded Tokens
- Row Level Security (RLS) in Power BI if needed

## Technical Requirements

| Layer        | Technology                                   |
|--------------|----------------------------------------------|
| Frontend     | ASP.NET Core MVC 8.0                        |
| Backend      | MySQL on Azure (Server: prismdbserver.mysql.database.azure.com, Schema: prism) |
| Reporting    | Power BI Embedded (Premium or PPU)           |
| Authentication| ASP.NET Core Identity                       |
| Hosting      | Azure App Service (Recommended)              |
| ETL          | Automated data import from Excel             |
| Security     | Role-based Access Control + Audit Trails     |

## Database Connection Information
- **Server Name:** prismdbserver.mysql.database.azure.com
- **Admin Login:** prismdbadmin
- **Password:** Wrls_8ome52iyoWrLphi
- **Port:** 3306
- **Schema Name:** prism

## Non-Functional Requirements
- High performance and scalability
- Secure data storage and transmission
- Modular, extensible architecture
- Comprehensive logging and monitoring
- User-friendly interface

## Project Milestones

| Milestone | Description |
|-----------|-------------|
| M1 | Requirements Finalization and Architecture Approval |
| M2 | Database Design and Data Migration Scripts |
| M3 | Web App Core Development (Modules 1-3) |
| M4 | Web App Extended Modules (4-8) |
| M5 | Power BI Report Prototypes and Embedding Setup |
| M6 | User Authentication and Role Management |
| M7 | UAT (User Acceptance Testing) |
| M8 | Deployment to Production Environment |
| M9 | Documentation and User Training |

## Deliverables
- ASP.NET Core MVC 8.0 Web Application Source Code
- Power BI Report Definitions (PBIX Files)
- Deployed App on Azure (or On-prem IIS)
- Complete Technical Documentation
- Data Migration Scripts
- User Training Material

## Future Enhancements (Phase II)
- AI/ML Models for Forecast Optimization and Document Summarization
- External Data Feeds Integration (UX Market Data)
- Mobile App Extension

---

**Prepared For:** ConverDyn  
**Prepared By:** Helia Systems  
**Document Version:** 1.2  
**Date:** May 2025
