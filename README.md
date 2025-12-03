# Spa Management System

A comprehensive .NET MAUI Blazor Hybrid application for managing spa operations including appointments, customers, services, products, sales, and accounting.

## Technology Stack

- **.NET 9.0** - Latest .NET framework
- **.NET MAUI** - Cross-platform UI framework
- **Blazor Hybrid** - Component-based UI with Razor syntax
- **Entity Framework Core 9.0** - ORM for database access
- **SQL Server** - Database
- **Radzen Blazor** - UI component library

## Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

```
Spa_Management_System/
├── Models/                          # Entity models (24 tables)
│   ├── Person.cs
│   ├── Customer.cs
│   ├── Employee.cs
│   ├── Service.cs
│   ├── Product.cs
│   ├── Appointment.cs
│   ├── Sale.cs
│   └── ...
│
├── Data/                            # Data access layer
│   ├── AppDbContext.cs             # EF Core DbContext
│   └── Repositories/
│       ├── IRepository.cs          # Generic repository interface
│       ├── Repository.cs           # Generic repository implementation
│       ├── CustomerRepository.cs   # Customer-specific repository
│       └── AppointmentRepository.cs
│
├── Services/                        # Business logic layer
│   ├── CustomerService.cs
│   └── AppointmentService.cs
│
├── Components/
│   ├── Pages/                       # Blazor pages
│   │   └── Home.razor
│   └── Shared/                      # Reusable components
│
└── Database_Schema.txt              # SQL Server schema definition

```

## Database Schema

The system uses 24 tables organized into logical modules:

### Core Entities
- **Person** - Base table for customers and employees
- **Customer** - Customer-specific data
- **Employee** - Employee records
- **UserAccount** - User authentication
- **Role** - Employee roles

### Service Management
- **ServiceCategory** - Service classifications
- **Service** - Spa services offered
- **EmployeeServiceCommission** - Commission configuration

### Inventory Management
- **Product** - Products for sale
- **Inventory** - Stock levels
- **StockTransaction** - Inventory movements
- **Supplier** - Vendor information
- **PurchaseOrder** & **PurchaseOrderItem** - Purchase orders

### Appointment System
- **Appointment** - Customer bookings
- **AppointmentService** - Services in appointments

### Sales & Payments
- **Sale** & **SaleItem** - Sales transactions
- **Payment** - Payment records

### Accounting
- **LedgerAccount** - Chart of accounts
- **JournalEntry** & **JournalEntryLine** - Double-entry bookkeeping

### CRM & Audit
- **CRM_Note** - Customer notes
- **AuditLog** - System audit trail

## Getting Started

### Prerequisites

1. Visual Studio 2022 (17.8 or later)
2. .NET 9.0 SDK
3. SQL Server (LocalDB, Express, or Full)
4. MAUI workload installed

### Database Setup

1. **Update Connection String** in `MauiProgram.cs`:
   ```csharp
   var connectionString = "Server=YOUR_SERVER;Database=SpaManagementDB;Trusted_Connection=True;TrustServerCertificate=True;";
   ```

2. **Create Database** - Run the SQL script in `Database_Schema.txt` to create all tables:
   ```sql
   -- Execute in SQL Server Management Studio or Azure Data Studio
   ```

3. **Or Use EF Core Migrations** (Alternative):
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

### Running the Application

1. Set the target framework (Windows, Android, iOS, or Mac Catalyst)
2. Press F5 to build and run

## Project Features

### Current Implementation

✅ Complete data models for all 24 tables
✅ Entity Framework Core DbContext
✅ Generic repository pattern
✅ Specific repositories for Customers and Appointments
✅ Customer service with CRUD operations
✅ Appointment service with business logic
✅ Dependency injection configured

### Next Steps to Implement

- [ ] Create Blazor UI pages
- [ ] Implement authentication & authorization
- [ ] Add Product/Inventory management service
- [ ] Add Sales/POS functionality
- [ ] Create reporting modules
- [ ] Add accounting features
- [ ] Implement CRM features

## Key Design Patterns

1. **Repository Pattern** - Abstraction over data access
2. **Dependency Injection** - Loose coupling between components
3. **Clean Architecture** - Separation of concerns
4. **MVVM** - Model-View-ViewModel for UI

## Usage Examples

### Injecting Services in Blazor Pages

```csharp
@page "/customers"
@inject ICustomerService CustomerService

@code {
    private List<Customer> customers = new();

    protected override async Task OnInitializedAsync()
    {
        customers = (await CustomerService.GetAllCustomersAsync()).ToList();
    }
}
```

### Creating a New Customer

```csharp
var customer = await CustomerService.CreateCustomerAsync(
    firstName: "John",
    lastName: "Doe",
    email: "john@example.com",
    phone: "555-1234",
    address: "123 Main St"
);
```

### Creating an Appointment

```csharp
var appointment = await AppointmentService.CreateAppointmentAsync(
    customerId: 1,
    scheduledStart: DateTime.Now.AddDays(1),
    scheduledEnd: DateTime.Now.AddDays(1).AddHours(2),
    createdByUserId: currentUserId
);

// Add service to appointment
await AppointmentService.AddServiceToAppointmentAsync(
    appointmentId: appointment.AppointmentId,
    serviceId: 1,
    therapistEmployeeId: 2
);
```

## Contributing

1. Create a feature branch
2. Make your changes
3. Submit a pull request

## License

[Specify your license here]

## Contact

[Your contact information]
