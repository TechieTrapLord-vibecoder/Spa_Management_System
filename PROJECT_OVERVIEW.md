# Kaye Spa Management System

## Complete Project Overview

---

## 1. Project Summary

**Kaye Spa Management System** is a comprehensive Enterprise Resource Planning (ERP) solution designed for spa and wellness businesses. Built with **.NET MAUI Blazor Hybrid**, it runs as a desktop application with cloud synchronization capabilities.

### Key Technologies

| Technology                | Purpose                              |
| ------------------------- | ------------------------------------ |
| .NET 9 MAUI Blazor Hybrid | Cross-platform desktop app framework |
| SQL Server (Local)        | Primary database                     |
| SQL Server (Cloud)        | Backup & sync database               |
| Entity Framework Core     | ORM for database operations          |
| Chart.js                  | Data visualization                   |
| Bootstrap Icons           | UI icons                             |

---

## 2. System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                        │
│  (Blazor Components - .razor files in /Components/Pages/)   │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                     SERVICE LAYER                            │
│  (Business Logic - /Services/)                               │
│  • AuthStateService    • AccountingService                   │
│  • ToastService        • PdfExportService                    │
│  • SyncService         • CommissionService                   │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      DATA LAYER                              │
│  (Entity Framework Core - /Data/ & /Models/)                 │
│  • AppDbContext        • Repositories                        │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      DATABASE                                │
│  Local: SQL Server Express (NIKOLA\SQLEXPRESS)              │
│  Cloud: MonsterASP SQL Server (db32359)                      │
└─────────────────────────────────────────────────────────────┘
```

---

## 3. Database Schema (28 Tables)

### Core Business Tables

| Table                | Description               | Key Fields                                                                          |
| -------------------- | ------------------------- | ----------------------------------------------------------------------------------- |
| `Person`             | Base table for all people | person_id, first_name, last_name, email, phone                                      |
| `Customer`           | Spa customers             | customer_id, person_id, loyalty_points                                              |
| `Employee`           | Staff members             | employee_id, person_id, role_id, hire_date, status                                  |
| `Appointment`        | Booking records           | appointment_id, customer_id, scheduled_start, status                                |
| `AppointmentService` | Services in appointment   | appointment_service_id, appointment_id, service_id, therapist_id, price, commission |
| `Service`            | Spa services offered      | service_id, name, price, duration, commission_rate                                  |
| `ServiceCategory`    | Service categories        | category_id, name                                                                   |
| `Product`            | Retail products           | product_id, name, sku, cost_price, selling_price                                    |

### Sales & Payments

| Table      | Description        | Key Fields                                                         |
| ---------- | ------------------ | ------------------------------------------------------------------ |
| `Sale`     | Sales transactions | sale_id, sale_number, customer_id, total_amount, payment_status    |
| `SaleItem` | Items in a sale    | sale_item_id, sale_id, service_id/product_id, quantity, line_total |
| `Payment`  | Payment records    | payment_id, sale_id, payment_method, amount, paid_at               |

### Inventory Management

| Table               | Description              | Key Fields                                                |
| ------------------- | ------------------------ | --------------------------------------------------------- |
| `Inventory`         | Stock levels             | inventory_id, product_id, quantity_on_hand, reorder_level |
| `StockTransaction`  | Stock movements          | tx_id, product_id, tx_type (sale/purchase/adjust), qty    |
| `PurchaseOrder`     | Orders to suppliers      | po_id, supplier_id, status, total_amount                  |
| `PurchaseOrderItem` | PO line items            | po_item_id, po_id, product_id, quantity, unit_cost        |
| `Supplier`          | Product vendors          | supplier_id, name, contact_info                           |
| `SupplierProduct`   | Supplier-product mapping | supplier_id, product_id, cost_price                       |

### Accounting (Double-Entry)

| Table              | Description        | Key Fields                                                  |
| ------------------ | ------------------ | ----------------------------------------------------------- |
| `LedgerAccount`    | Chart of accounts  | ledger_account_id, code, name, account_type, normal_balance |
| `JournalEntry`     | Accounting entries | journal_id, journal_no, date, description                   |
| `JournalEntryLine` | Debit/Credit lines | line_id, journal_id, ledger_account_id, debit, credit       |
| `Expense`          | Business expenses  | expense_id, category, amount, expense_date                  |

### HR & Payroll

| Table                       | Description       | Key Fields                                                                               |
| --------------------------- | ----------------- | ---------------------------------------------------------------------------------------- |
| `Role`                      | User roles        | role_id, name (SuperAdmin, Manager, Therapist, Receptionist, Accountant, InventoryClerk) |
| `UserAccount`               | Login credentials | user_id, employee_id, username, password_hash                                            |
| `EmployeeServiceCommission` | Commission rates  | employee_id, service_id, commission_rate                                                 |
| `Payroll`                   | Salary processing | payroll_id, employee_id, period, gross_pay, deductions                                   |

### Other Tables

| Table      | Description              |
| ---------- | ------------------------ |
| `AuditLog` | System activity tracking |

---

## 4. User Roles & Permissions

| Role               | Access Level       | Key Pages                                                   |
| ------------------ | ------------------ | ----------------------------------------------------------- |
| **SuperAdmin**     | Full system access | Admin Dashboard, Settings, Roles, all modules               |
| **Manager**        | Management access  | Manager Dashboard, Reports, Employees, Audit Logs           |
| **Accountant**     | Financial access   | Journal Entries, Financial Statements, Expenses, Audit Logs |
| **Therapist**      | Service access     | Appointments, Checkout, own schedule                        |
| **Receptionist**   | Front desk access  | Appointments, Customers, Checkout                           |
| **InventoryClerk** | Inventory access   | Inventory, Stock Adjustments, Purchase Orders               |

### Default User Accounts

| Username     | Role           |
| ------------ | -------------- |
| superadmin   | SuperAdmin     |
| manager      | Manager        |
| accountant   | Accountant     |
| therapist    | Therapist      |
| receptionist | Receptionist   |
| Inventory    | InventoryClerk |

---

## 5. Application Pages (Modules)

### 5.1 Dashboard & Analytics

| Page              | File                     | Description                                                             |
| ----------------- | ------------------------ | ----------------------------------------------------------------------- |
| Admin Dashboard   | `AdminDashboard.razor`   | KPIs, charts, alerts for SuperAdmin                                     |
| Manager Dashboard | `ManagerDashboard.razor` | KPIs, charts, alerts for Manager role                                   |
| Reports           | `Reports.razor`          | Business analytics (revenue, appointments, customers, staff, inventory) |

### 5.2 Appointment Management

| Page         | File                 | Description                                                      |
| ------------ | -------------------- | ---------------------------------------------------------------- |
| Appointments | `Appointments.razor` | Calendar view, booking, rescheduling, status management          |
| -            | -                    | Statuses: scheduled → confirmed → in-progress → completed → paid |

### 5.3 Point of Sale

| Page          | File                 | Description                       |
| ------------- | -------------------- | --------------------------------- |
| Checkout      | `Checkout.razor`     | POS terminal for processing sales |
| Sales History | `SalesHistory.razor` | View past transactions, receipts  |

### 5.4 Customer Management

| Page            | File                   | Description                                          |
| --------------- | ---------------------- | ---------------------------------------------------- |
| Customers       | `Customers.razor`      | Customer list, add/edit                              |
| Customer Detail | `CustomerDetail.razor` | Individual customer profile, history, loyalty points |

### 5.5 Service & Product Management

| Page               | File                      | Description                                    |
| ------------------ | ------------------------- | ---------------------------------------------- |
| Services           | `Services.razor`          | Manage spa services, pricing, commission rates |
| Service Categories | `ServiceCategories.razor` | Categorize services                            |
| Products           | `Products.razor`          | Retail product management                      |

### 5.6 Inventory Management

| Page              | File                     | Description                   |
| ----------------- | ------------------------ | ----------------------------- |
| Inventory         | `Inventory.razor`        | Stock levels, reorder alerts  |
| Stock Adjustments | `StockAdjustments.razor` | Manual stock corrections      |
| Purchase Orders   | `PurchaseOrders.razor`   | Order products from suppliers |
| Suppliers         | `Suppliers.razor`        | Vendor management             |

### 5.7 Human Resources

| Page               | File                      | Description        |
| ------------------ | ------------------------- | ------------------ |
| Employees          | `Employees.razor`         | Staff management   |
| Payroll            | `Payroll.razor`           | Salary processing  |
| Attendance         | `Attendance.razor`        | Time tracking      |
| Commission History | `CommissionHistory.razor` | Therapist earnings |

### 5.8 Accounting & Finance

| Page                 | File                        | Description                                    |
| -------------------- | --------------------------- | ---------------------------------------------- |
| Financial Statements | `FinancialStatements.razor` | Balance Sheet, Income Statement, Trial Balance |
| Journal Entries      | `JournalEntries.razor`      | Double-entry bookkeeping                       |
| Chart of Accounts    | `ChartOfAccounts.razor`     | Account management                             |
| Expenses             | `Expenses.razor`            | Business expense tracking                      |
| Cash Flow            | `CashFlow.razor`            | Cash flow statement                            |
| Capital Investments  | `CapitalInvestments.razor`  | Owner's equity, equipment purchases            |
| Financial Reports    | `FinancialReports.razor`    | Profit & loss analysis                         |

### 5.9 System Administration

| Page          | File                 | Description             |
| ------------- | -------------------- | ----------------------- |
| Settings      | `Settings.razor`     | System configuration    |
| User Accounts | `UserAccounts.razor` | User management         |
| Audit Log     | `AuditLog.razor`     | System activity history |
| Sync          | `Sync.razor`         | Cloud synchronization   |

---

## 6. Key Features

### 6.1 Appointment Workflow

```
Customer Books → Scheduled → Staff Confirms → Confirmed →
Service Starts → In-Progress → Service Done → Completed →
Payment Made → Paid
```

### 6.2 Sales & Checkout Flow

```
1. Select Appointment OR Walk-in
2. Add Services/Products to cart
3. Apply discounts (optional)
4. Apply loyalty points (optional)
5. Select payment method (Cash/Card/GCash)
6. Complete sale → Journal entries auto-created
7. Inventory auto-deducted for products
```

### 6.3 Inventory Automation

- **Sales**: Auto-deduct from `Inventory.quantity_on_hand`
- **Purchase Orders**: Auto-add when received
- **Low Stock Alerts**: When `quantity_on_hand ≤ reorder_level`
- **Stock Transactions**: All movements logged in `StockTransaction`

### 6.4 Commission System

- Each **Service** has a `commission_rate` (percentage)
- Therapists earn commission on services performed
- Calculated as: `service_price × commission_rate`
- Stored in `AppointmentService.commission_amount`

### 6.5 Double-Entry Accounting

Every sale creates balanced journal entries:

```
Example: ₱1,000 Cash Sale with 12% VAT
  Debit:  Cash (Asset)           ₱1,000
  Credit: Service Revenue        ₱  892.86
  Credit: VAT Payable (Liability)₱  107.14
```

### 6.6 Cloud Synchronization

- Local database: Real-time operations
- Cloud database: Backup & multi-device sync
- Sync status tracked per record: `pending` → `synced`

---

## 7. File Structure

```
Spa_Management_System/
├── App.xaml                    # Application entry
├── MauiProgram.cs              # Dependency injection setup
├── appsettings.json            # Configuration
│
├── Components/
│   ├── Layout/
│   │   └── MainLayout.razor    # Main app layout with sidebar
│   ├── Pages/
│   │   ├── Login.razor         # Authentication
│   │   ├── AdminDashboard.razor
│   │   ├── Appointments.razor
│   │   ├── Checkout.razor
│   │   ├── Customers.razor
│   │   ├── FinancialStatements.razor
│   │   └── ... (20+ pages)
│   └── _Imports.razor          # Global using statements
│
├── Data/
│   ├── AppDbContext.cs         # EF Core database context
│   └── Repositories/           # Data access layer
│
├── Models/                     # Entity classes
│   ├── Appointment.cs
│   ├── Customer.cs
│   ├── Sale.cs
│   ├── JournalEntry.cs
│   └── ... (28 models)
│
├── Services/                   # Business logic
│   ├── AuthStateService.cs     # Authentication
│   ├── AccountingService.cs    # Journal entries
│   ├── SyncService.cs          # Cloud sync
│   └── ...
│
├── wwwroot/                    # Static assets
│   ├── css/
│   └── js/
│
└── Migrations/                 # EF Core migrations
```

---

## 8. Design System

### Color Palette

| Color             | Hex       | Usage              |
| ----------------- | --------- | ------------------ |
| Primary (Olive)   | `#454F4A` | Headers, buttons   |
| Secondary (Beige) | `#DCD8CE` | Cards, backgrounds |
| Accent (Green)    | `#5a7a6b` | Highlights         |
| Success           | `#28a745` | Positive actions   |
| Danger            | `#dc3545` | Errors, delete     |
| Warning           | `#ffc107` | Alerts             |

### UI Components

- **Cards**: Rounded corners, subtle shadows
- **Tables**: Striped rows, hover effects
- **Buttons**: Primary (filled), Outline, Icon buttons
- **Forms**: Floating labels, validation states
- **Modals**: Overlay dialogs for CRUD operations

---

## 9. API Endpoints (Cloud Sync)

The `Spa_Sync_API` project provides REST endpoints:

| Endpoint           | Method | Description          |
| ------------------ | ------ | -------------------- |
| `/api/sync/pull`   | GET    | Get cloud data       |
| `/api/sync/push`   | POST   | Upload local changes |
| `/api/sync/status` | GET    | Check sync status    |

---

## 10. Getting Started

### Prerequisites

1. Visual Studio 2022 with .NET MAUI workload
2. SQL Server Express
3. .NET 9 SDK

### Setup Steps

```powershell
# 1. Clone repository
git clone [repository-url]

# 2. Restore packages
dotnet restore

# 3. Update connection string in appsettings.json

# 4. Run database migrations
dotnet ef database update

# 5. Run the application
dotnet run
```

### Default Login

- **Username**: `superadmin`
- **Password**: `admin123`
- **Role**: SuperAdmin

Other test accounts: `manager`, `accountant`, `therapist`, `receptionist`, `Inventory`

---

## 11. Current Data Statistics

| Entity              | Record Count |
| ------------------- | ------------ |
| Journal Entry Lines | 6,534        |
| Journal Entries     | 2,615        |
| Sale Items          | 1,574        |
| Sales               | 1,303        |
| Appointments        | 1,301        |
| Customers           | 80           |
| Products            | 28           |
| Services            | 30           |
| Employees           | 10           |

---

## 12. Business Rules Summary

1. **Appointment Status Flow**: scheduled → confirmed → in-progress → completed → paid
2. **Payment Methods**: Cash, Card, GCash
3. **VAT Rate**: 12% (configurable)
4. **Loyalty Points**: 1 point per ₱100 spent
5. **Commission**: Calculated per service, not per appointment
6. **Inventory**: Auto-deduct on sale, auto-add on PO receive
7. **Accounting**: All transactions create balanced journal entries

---

## 13. Team Responsibilities

| Module     | Responsibilities                                  |
| ---------- | ------------------------------------------------- |
| Frontend   | Blazor components, UI/UX, responsive design       |
| Backend    | Services, business logic, API                     |
| Database   | Schema design, migrations, queries                |
| Accounting | Journal entries, financial reports, balance sheet |
| Testing    | Unit tests, integration tests, UAT                |

---

## 14. Future Enhancements

- [ ] Mobile app version
- [ ] Online booking portal for customers
- [ ] SMS/Email notifications
- [ ] Advanced analytics with AI
- [ ] Multi-branch support
- [ ] Membership/subscription packages

---

**Document Version**: 1.0  
**Last Updated**: December 6, 2025  
**Generated By**: ************\_\_\_\_************  
**Project Status**: Production Ready ✅
