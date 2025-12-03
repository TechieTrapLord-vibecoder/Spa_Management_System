# 🚀 Getting Started - Spa Management System

## ✅ Setup Complete!

Your Spa Management System is now connected to your SQL Server database!

### Connection Details
- **Server:** NIKOLA\SQLEXPRESS
- **Database:** spa_erp
- **Tables Created:** 24 tables ✅

---

## 🧪 Test Your Database Connection

1. **Run the Application** (Press F5)
2. **Navigate to "DB Test"** in the menu
3. You should see:
   - ✅ Database Connected Successfully!
   - A table showing record counts for all tables

---

## 📝 Next Steps: Add Sample Data

To fully test your system, let's add some sample data to your database.

### Run This SQL Script

Open **SQL Server Management Studio** or **Azure Data Studio** and run this script:

```sql
USE spa_erp;
GO

-- 1. Add Roles
INSERT INTO Role (name) VALUES 
('Manager'),
('Therapist'),
('Receptionist'),
('Accountant');

-- 2. Add Service Categories
INSERT INTO ServiceCategory (name, description) VALUES 
('Massage', 'Various massage therapies'),
('Facial', 'Facial treatments and skincare'),
('Body Treatment', 'Full body treatments'),
('Hair & Beauty', 'Hair styling and beauty services');

-- 3. Add Sample Persons
INSERT INTO Person (first_name, last_name, email, phone, address, dob, created_at) VALUES 
('John', 'Doe', 'john.doe@example.com', '555-0101', '123 Main St, City, State', '1985-05-15', GETDATE()),
('Jane', 'Smith', 'jane.smith@example.com', '555-0102', '456 Oak Ave, City, State', '1990-08-22', GETDATE()),
('Michael', 'Johnson', 'michael.j@example.com', '555-0103', '789 Pine Rd, City, State', '1988-03-10', GETDATE()),
('Emily', 'Brown', 'emily.brown@example.com', '555-0104', '321 Elm St, City, State', '1992-11-30', GETDATE()),
('Sarah', 'Wilson', 'sarah.wilson@example.com', '555-0105', '654 Maple Dr, City, State', '1987-07-18', GETDATE());

-- 4. Add Employees (linking to Persons and Roles)
INSERT INTO Employee (person_id, role_id, hire_date, status, note, created_at) VALUES 
(1, 1, '2020-01-15', 'active', 'Manager - experienced in spa operations', GETDATE()),
(2, 2, '2021-03-20', 'active', 'Senior therapist - specialized in Swedish massage', GETDATE()),
(3, 3, '2022-06-10', 'active', 'Front desk receptionist', GETDATE());

-- 5. Add User Accounts (for login)
-- Password: 'password123' - In production, use proper password hashing!
INSERT INTO UserAccount (employee_id, username, password_hash, is_active, created_at) VALUES 
(1, 'johndoe', 'hashed_password_here', 1, GETDATE()),
(2, 'janesmith', 'hashed_password_here', 1, GETDATE()),
(3, 'michaelj', 'hashed_password_here', 1, GETDATE());

-- 6. Add Customers (linking to remaining Persons)
INSERT INTO Customer (person_id, customer_code, loyalty_points, created_at) VALUES 
(4, 'CUST-20250128-001', 100, GETDATE()),
(5, 'CUST-20250128-002', 50, GETDATE());

-- 7. Add Services
INSERT INTO Service (service_category_id, code, name, description, price, duration_minutes, active) VALUES 
(1, 'MSG-001', 'Swedish Massage', 'Relaxing full body massage', 89.99, 60, 1),
(1, 'MSG-002', 'Deep Tissue Massage', 'Therapeutic deep tissue massage', 109.99, 60, 1),
(1, 'MSG-003', 'Hot Stone Massage', 'Massage with heated stones', 129.99, 90, 1),
(2, 'FCL-001', 'Classic Facial', 'Deep cleansing facial treatment', 79.99, 60, 1),
(2, 'FCL-002', 'Anti-Aging Facial', 'Advanced anti-aging treatment', 149.99, 90, 1),
(3, 'BDY-001', 'Body Scrub', 'Exfoliating body treatment', 99.99, 60, 1),
(4, 'HAR-001', 'Hair Styling', 'Professional hair styling', 59.99, 45, 1);

-- 8. Add Products
INSERT INTO Product (sku, name, description, unit_price, cost_price, unit, active) VALUES 
('PRD-001', 'Massage Oil - Lavender', 'Premium lavender massage oil 250ml', 29.99, 15.00, 'bottle', 1),
('PRD-002', 'Face Cream - Anti-Aging', 'Luxury anti-aging face cream 50ml', 89.99, 40.00, 'jar', 1),
('PRD-003', 'Body Scrub - Sea Salt', 'Exfoliating sea salt body scrub 200g', 34.99, 18.00, 'jar', 1),
('PRD-004', 'Aromatherapy Candle', 'Scented relaxation candle', 19.99, 8.00, 'piece', 1),
('PRD-005', 'Hair Serum', 'Nourishing hair serum 100ml', 45.99, 22.00, 'bottle', 1);

-- 9. Add Inventory for Products
INSERT INTO Inventory (product_id, quantity_on_hand, reorder_level, last_counted_at) VALUES 
(1, 25, 5, GETDATE()),
(2, 15, 3, GETDATE()),
(3, 30, 10, GETDATE()),
(4, 50, 15, GETDATE()),
(5, 20, 5, GETDATE());

-- 10. Add Supplier
INSERT INTO Supplier (name, contact_person, phone, email, address) VALUES 
('Spa Supplies Inc.', 'David Chen', '555-9001', 'orders@spasupplies.com', '100 Industrial Park, Supplier City'),
('Beauty Products Co.', 'Lisa Anderson', '555-9002', 'sales@beautyproducts.com', '200 Commerce Blvd, Beauty Town');

-- 11. Add Commission Rules for Employees
INSERT INTO EmployeeServiceCommission (employee_id, service_id, commission_type, commission_value, effective_from) VALUES 
(2, 1, 'percent', 20.00, '2024-01-01'), -- Jane gets 20% on Swedish Massage
(2, 2, 'percent', 25.00, '2024-01-01'), -- Jane gets 25% on Deep Tissue
(2, 3, 'percent', 30.00, '2024-01-01'); -- Jane gets 30% on Hot Stone

-- 12. Add Sample Appointments
INSERT INTO Appointment (customer_id, scheduled_start, scheduled_end, status, created_by_user_id, notes, created_at) VALUES 
(1, DATEADD(day, 1, GETDATE()), DATEADD(hour, 1, DATEADD(day, 1, GETDATE())), 'scheduled', 1, 'First time customer - Swedish massage', GETDATE()),
(2, DATEADD(day, 2, GETDATE()), DATEADD(minute, 90, DATEADD(day, 2, GETDATE())), 'scheduled', 1, 'Regular customer - Hot stone massage', GETDATE());

-- 13. Add Services to Appointments
INSERT INTO AppointmentService (appointment_id, service_id, therapist_employee_id, price, commission_amount) VALUES 
(1, 1, 2, 89.99, 17.99), -- Swedish Massage with Jane (20% commission = $17.99)
(2, 3, 2, 129.99, 38.99); -- Hot Stone Massage with Jane (30% commission = $38.99)

-- 14. Add Ledger Accounts for Basic Accounting
INSERT INTO LedgerAccount (code, name, account_type, normal_balance) VALUES 
('1000', 'Cash', 'asset', 'debit'),
('1100', 'Accounts Receivable', 'asset', 'debit'),
('1200', 'Inventory', 'asset', 'debit'),
('2000', 'Accounts Payable', 'liability', 'credit'),
('3000', 'Owner Equity', 'equity', 'credit'),
('4000', 'Service Revenue', 'revenue', 'credit'),
('4100', 'Product Sales Revenue', 'revenue', 'credit'),
('5000', 'Cost of Goods Sold', 'expense', 'debit'),
('5100', 'Salaries Expense', 'expense', 'debit'),
('5200', 'Rent Expense', 'expense', 'debit');

PRINT 'Sample data inserted successfully!';
GO
```

---

## 🎯 What You Can Do Now

### 1. Test Database Connection
- Navigate to **DB Test** page
- Verify all tables show record counts

### 2. Build Customer Management Page
```razor
@page "/customers"
@inject ICustomerService CustomerService

<h3>Customer List</h3>
<!-- Add your customer list UI here -->
```

### 3. Build Appointment Calendar
```razor
@page "/appointments"
@inject IAppointmentService AppointmentService

<h3>Appointments</h3>
<!-- Add your appointment calendar here -->
```

### 4. Build POS/Sales Page
- Create sales transactions
- Process payments
- Track inventory

---

## 📚 Available Services

You can inject these services into your Blazor pages:

- `ICustomerService` - Customer CRUD operations
- `IAppointmentService` - Appointment management
- `AppDbContext` - Direct database access (if needed)

### Example Usage:

```csharp
@inject ICustomerService CustomerService

@code {
    private async Task CreateNewCustomer()
    {
        var customer = await CustomerService.CreateCustomerAsync(
            firstName: "New",
            lastName: "Customer",
            email: "new@example.com",
            phone: "555-1234",
            address: "123 Street"
        );
    }
}
```

---

## 🐛 Troubleshooting

### If DB Test fails:

1. **Check SQL Server is running**
   - Open SQL Server Configuration Manager
   - Ensure SQL Server (SQLEXPRESS) is running

2. **Verify connection string**
   - Server: `NIKOLA\SQLEXPRESS`
   - Database: `spa_erp`

3. **Check Windows Authentication**
   - Make sure your Windows user has access to SQL Server

---

## 🎨 UI Framework: Radzen Blazor

You have **Radzen.Blazor** components available:

```razor
<RadzenDataGrid Data="@customers" TItem="Customer">
    <Columns>
        <RadzenDataGridColumn Property="CustomerCode" Title="Code" />
        <RadzenDataGridColumn Property="Person.FirstName" Title="First Name" />
    </Columns>
</RadzenDataGrid>
```

[Radzen Documentation](https://blazor.radzen.com/)

---

## ✨ You're Ready to Build!

Your foundation is solid. Start by:
1. ✅ Testing database connection
2. ✅ Adding sample data (run SQL script above)
3. ✅ Creating your first customer management page
4. ✅ Building the appointment booking system

**Happy Coding! 🚀**
