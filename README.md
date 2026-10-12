 ACXIOM_CRM_Project_Assignment

AcxiomCRM: a role-based CRM built with ASP.NET Core MVC, EF Core, and Identity, covering leads, opportunities, follow-ups, audit logs, and REST APIs.

 Overview

AcxiomCRM is a Customer Relationship Management web application designed for enterprise sales and management workflows. Built using modern ASP.NET Core 8 MVC it implements full Role-Based Access Control (RBAC), database auditing, lead-to-opportunity pipeline management, and integrated REST APIs.

---

 Technology Stack

- **Framework:** ASP.NET Core 8 (MVC)
- **Data Access:** Entity Framework Core 8 (Code-First with SQLite)
- **Identity & Security:** ASP.NET Core Identity with cookie authentication & lockout policies
- **Frontend:** Razor Views, Bootstrap 5, jQuery & Unobtrusive Validation
- **Architecture:** Controller-Service-Repository pattern with DTOs and strongly-typed models

---



Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later


 Default Credentials

The system automatically initializes roles and seeds a default administrator on the first launch:

Admin  `admin@acxiom.com` | `Admin@1234` |


