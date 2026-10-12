# ACXIOM_CRM_Project_Assignment

AcxiomCRM: a role-based CRM built with ASP.NET Core MVC, EF Core, and Identity, covering leads, opportunities, follow-ups, audit logs, and REST APIs.

---

## 🚀 Overview

**AcxiomCRM** is a Customer Relationship Management web application designed for enterprise sales and management workflows. Built using modern **ASP.NET Core 8 MVC**, it implements full Role-Based Access Control (RBAC), database auditing, lead-to-opportunity pipeline management, and integrated REST APIs.

---

## 🛠️ Technology Stack

- **Framework:** ASP.NET Core 8 (MVC)
- **Data Access:** Entity Framework Core 8 (Code-First with SQLite)
- **Identity & Security:** ASP.NET Core Identity with cookie authentication & lockout policies
- **Frontend:** Razor Views, Bootstrap 5, jQuery & Unobtrusive Validation
- **Architecture:** Controller-Service-Repository pattern with DTOs and strongly-typed models

---

## ✨ Features & Modules

### 1. 🔐 Authentication & Role-Based Access Control (RBAC)
- 3 Pre-configured User Roles:
  - **Admin:** Full access to all modules, user administration, and system-wide audit logs.
  - **Manager:** Team performance oversight, pipeline reports, customer and opportunity management.
  - **SalesExecutive:** Operational workflows for leads, customer follow-ups, and assigned opportunities.
- Secure lockout policy (5 failed attempts trigger a 10-minute lockout).

### 2. 👥 Customer Management
- Complete CRUD operations for customer records.
- Duplicate email prevention and company mapping.
- Ownership tracking linked to identity users.

### 3. 🎯 Lead Management
- Capture lead details (Source, Status, Contact information).
- Status lifecycle tracking (`New`, `Contacted`, `Qualified`, `Lost`).

### 4. 💼 Opportunity Pipeline
- Track sales opportunities across multiple pipeline stages.
- Probability scoring and expected close date validation.
- Aggregate pipeline reporting by sales stages.

### 5. 📅 Follow-Up Reminders
- Schedule and track customer touchpoints, meetings, and calls.
- Status management to ensure sales tasks are closed on time.

### 6. 📜 Audit Logging
- Dedicated `AuditService` tracking critical data mutations.
- Records action type, entity name, entity ID, user, and UTC timestamps.

### 7. 🌐 REST API
- Secured endpoints under `/api` requiring authentication:
  - `GET /api/customers`, `GET /api/customers/{id}`, `POST /api/customers`, `PUT /api/customers/{id}`, `DELETE /api/customers/{id}`
  - `GET /api/leads`, `POST /api/leads`
  - `GET /api/opportunities`, `POST /api/opportunities`
  - `GET /api/reports/pipeline` (Admin/Manager role protected)

---

## 🏃 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later

### Installation & Run

1. **Clone the repository:**
   ```bash
   git clone https://github.com/ayaan0060/ACXIOM_CRM_Project_Assignment.git
   cd ACXIOM_CRM_Project_Assignment
   ```

2. **Restore dependencies & build:**
   ```bash
   dotnet restore
   dotnet build
   ```

3. **Run the application:**
   ```bash
   dotnet run --project AcxiomCRM
   ```

4. **Access the application:**
   - Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000` (check console output for exact port).

---

## 🔑 Default Credentials

The system automatically initializes roles and seeds a default administrator on the first launch:

| Role | Email / Username | Password |
|---|---|---|
| **Admin** | `admin@acxiom.com` | `Admin@1234` |

---

## 📁 Project Structure

```
├── AcxiomCRM.sln
├── .gitignore
├── README.md
└── AcxiomCRM/
    ├── AcxiomCRM.csproj
    ├── Program.cs
    ├── appsettings.json
    ├── Controllers/        # MVC & API Controllers
    ├── Data/               # AppDbContext & EF Core config
    ├── DTOs/               # Data Transfer Objects
    ├── Models/             # Entity models (Lead, Customer, Opportunity, etc.)
    ├── Services/           # Business logic & AuditService
    ├── Views/              # Razor views & layouts
    └── wwwroot/            # Static assets (Bootstrap, CSS, JS)
```
