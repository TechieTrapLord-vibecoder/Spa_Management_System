/* =============================================
   SPA MANAGEMENT SYSTEM - SQL SERVER (T-SQL)
   Updated: December 2025
   
   This schema includes sync columns for cloud synchronization
   on all tables that support offline-first architecture.
   =============================================
*/

-- =============================================
-- 1. Person Table (Base table for Customers and Employees)
-- =============================================
CREATE TABLE Person (
    person_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    first_name VARCHAR(120) NOT NULL,
    last_name VARCHAR(120) NOT NULL,
    email VARCHAR(200),
    phone VARCHAR(50),
    address VARCHAR(MAX),
    dob DATE,
    created_at DATETIME DEFAULT GETDATE(),
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1
);

-- =============================================
-- 2. Role Table
-- =============================================
CREATE TABLE Role (
    role_id SMALLINT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    is_archived BIT DEFAULT 0,
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1
);

-- =============================================
-- 3. Employee Table
-- =============================================
CREATE TABLE Employee (
    employee_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    person_id BIGINT NOT NULL,
    role_id SMALLINT NOT NULL,
    hire_date DATE,
    status VARCHAR(30) DEFAULT 'active',
    note VARCHAR(MAX),
    created_at DATETIME DEFAULT GETDATE(),
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (person_id) REFERENCES Person(person_id),
    FOREIGN KEY (role_id) REFERENCES Role(role_id)
);

-- =============================================
-- 4. UserAccount Table
-- =============================================
CREATE TABLE UserAccount (
    user_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    employee_id BIGINT NULL,
    username VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    is_active BIT DEFAULT 1,
    last_login DATETIME,
    created_at DATETIME DEFAULT GETDATE(),
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (employee_id) REFERENCES Employee(employee_id)
);

-- =============================================
-- 5. Customer Table
-- =============================================
CREATE TABLE Customer (
    customer_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    person_id BIGINT NOT NULL,
    customer_code VARCHAR(50) UNIQUE,
    loyalty_points INT DEFAULT 0,
    is_archived BIT DEFAULT 0,
    created_at DATETIME DEFAULT GETDATE(),
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (person_id) REFERENCES Person(person_id)
);

-- =============================================
-- 6. ServiceCategory Table
-- =============================================
CREATE TABLE ServiceCategory (
    service_category_id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(MAX),
    is_archived BIT DEFAULT 0,
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1
);

-- =============================================
-- 7. Service Table
-- =============================================
CREATE TABLE Service (
    service_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    service_category_id INT NULL,
    code VARCHAR(60) UNIQUE,
    name VARCHAR(150) NOT NULL,
    description VARCHAR(MAX),
    price DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    duration_minutes INT DEFAULT 0,
    active BIT DEFAULT 1,
    commission_type NVARCHAR(20) NOT NULL DEFAULT 'percentage',
    commission_value DECIMAL(12,2) NOT NULL DEFAULT 0,
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (service_category_id) REFERENCES ServiceCategory(service_category_id)
);

-- =============================================
-- 8. EmployeeServiceCommission Table
-- =============================================
CREATE TABLE EmployeeServiceCommission (
    commission_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    employee_id BIGINT NOT NULL,
    service_id BIGINT NOT NULL,
    commission_type VARCHAR(10) NOT NULL,
    commission_value DECIMAL(10,2) NOT NULL,
    effective_from DATE,
    effective_to DATE,
    is_archived BIT DEFAULT 0,
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (employee_id) REFERENCES Employee(employee_id),
    FOREIGN KEY (service_id) REFERENCES Service(service_id),
    CONSTRAINT CHK_CommissionType CHECK (commission_type IN ('percent','fixed'))
);

-- =============================================
-- 9. Product Table
-- =============================================
CREATE TABLE Product (
    product_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    sku VARCHAR(80) UNIQUE,
    name VARCHAR(200) NOT NULL,
    description VARCHAR(MAX),
    unit_price DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    cost_price DECIMAL(12,2) DEFAULT 0.00,
    unit VARCHAR(20),
    active BIT DEFAULT 1,
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1
);

-- =============================================
-- 10. Inventory Table
-- =============================================
CREATE TABLE Inventory (
    inventory_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    product_id BIGINT NOT NULL UNIQUE,
    quantity_on_hand DECIMAL(12,2) DEFAULT 0,
    reorder_level DECIMAL(12,2) DEFAULT 0,
    last_counted_at DATETIME,
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (product_id) REFERENCES Product(product_id)
);

-- =============================================
-- 11. StockTransaction Table
-- =============================================
CREATE TABLE StockTransaction (
    stock_tx_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    product_id BIGINT NOT NULL,
    tx_type VARCHAR(10) NOT NULL,
    qty DECIMAL(12,2) NOT NULL,
    unit_cost DECIMAL(12,2),
    reference VARCHAR(120),
    created_by_user_id BIGINT,
    created_at DATETIME DEFAULT GETDATE(),
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (product_id) REFERENCES Product(product_id),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id),
    CONSTRAINT CHK_TxType CHECK (tx_type IN ('purchase','sale','adjust','return'))
);

-- =============================================
-- 12. Supplier Table
-- =============================================
CREATE TABLE Supplier (
    supplier_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    contact_person VARCHAR(200),
    phone VARCHAR(50),
    email VARCHAR(150),
    address VARCHAR(MAX),
    is_archived BIT DEFAULT 0,
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1
);

-- =============================================
-- 13. SupplierProduct Table (Many-to-Many: Supplier <-> Product)
-- =============================================
CREATE TABLE SupplierProduct (
    supplier_product_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    supplier_id BIGINT NOT NULL,
    product_id BIGINT NOT NULL,
    supplier_price DECIMAL(12,2) NOT NULL DEFAULT 0,
    supplier_sku NVARCHAR(80),
    min_order_qty INT,
    lead_time_days INT,
    is_preferred BIT DEFAULT 0,
    is_active BIT DEFAULT 1,
    notes NVARCHAR(MAX),
    created_at DATETIME2 DEFAULT GETDATE(),
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (supplier_id) REFERENCES Supplier(supplier_id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES Product(product_id) ON DELETE CASCADE,
    CONSTRAINT UQ_SupplierProduct UNIQUE (supplier_id, product_id)
);

-- =============================================
-- 14. PurchaseOrder Table
-- =============================================
CREATE TABLE PurchaseOrder (
    po_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    po_number VARCHAR(80) UNIQUE,
    supplier_id BIGINT NOT NULL,
    status VARCHAR(40),
    created_by_user_id BIGINT,
    created_at DATETIME DEFAULT GETDATE(),
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (supplier_id) REFERENCES Supplier(supplier_id),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id)
);

-- =============================================
-- 15. PurchaseOrderItem Table
-- =============================================
CREATE TABLE PurchaseOrderItem (
    po_item_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    po_id BIGINT NOT NULL,
    product_id BIGINT NOT NULL,
    qty_ordered DECIMAL(12,2) NOT NULL,
    unit_cost DECIMAL(12,2) NOT NULL,
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (po_id) REFERENCES PurchaseOrder(po_id),
    FOREIGN KEY (product_id) REFERENCES Product(product_id)
);

-- =============================================
-- 16. Appointment Table
-- =============================================
CREATE TABLE Appointment (
    appointment_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    customer_id BIGINT NOT NULL,
    scheduled_start DATETIME NOT NULL,
    scheduled_end DATETIME,
    status VARCHAR(40) DEFAULT 'scheduled',
    notes VARCHAR(MAX),
    created_by_user_id BIGINT,
    created_at DATETIME DEFAULT GETDATE(),
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (customer_id) REFERENCES Customer(customer_id),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id)
);

-- =============================================
-- 17. AppointmentService Table
-- =============================================
CREATE TABLE AppointmentService (
    appt_srv_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    appointment_id BIGINT NOT NULL,
    service_id BIGINT NOT NULL,
    therapist_employee_id BIGINT NULL,
    price DECIMAL(12,2) NOT NULL,
    commission_amount DECIMAL(12,2) DEFAULT 0,
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (appointment_id) REFERENCES Appointment(appointment_id),
    FOREIGN KEY (service_id) REFERENCES Service(service_id),
    FOREIGN KEY (therapist_employee_id) REFERENCES Employee(employee_id)
);

-- =============================================
-- 18. Sale Table
-- =============================================
CREATE TABLE Sale (
    sale_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    customer_id BIGINT NULL,
    created_by_user_id BIGINT,
    sale_number VARCHAR(80) UNIQUE,
    subtotal DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    tax_rate DECIMAL(5,2) DEFAULT 0.00,
    tax_amount DECIMAL(12,2) DEFAULT 0.00,
    total_amount DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    payment_status VARCHAR(40) DEFAULT 'unpaid',
    created_at DATETIME DEFAULT GETDATE(),
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (customer_id) REFERENCES Customer(customer_id),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id)
);

-- =============================================
-- 19. SaleItem Table
-- =============================================
CREATE TABLE SaleItem (
    sale_item_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    sale_id BIGINT NOT NULL,
    item_type VARCHAR(10) NOT NULL,
    product_id BIGINT NULL,
    service_id BIGINT NULL,
    qty DECIMAL(12,2) DEFAULT 1,
    unit_price DECIMAL(12,2) NOT NULL,
    line_total DECIMAL(12,2) NOT NULL,
    therapist_employee_id BIGINT NULL,
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (sale_id) REFERENCES Sale(sale_id),
    FOREIGN KEY (product_id) REFERENCES Product(product_id),
    FOREIGN KEY (service_id) REFERENCES Service(service_id),
    FOREIGN KEY (therapist_employee_id) REFERENCES Employee(employee_id),
    CONSTRAINT CHK_ItemType CHECK (item_type IN ('product','service'))
);

-- =============================================
-- 20. Payment Table
-- =============================================
CREATE TABLE Payment (
    payment_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    sale_id BIGINT NOT NULL,
    payment_method VARCHAR(20) NOT NULL,
    amount DECIMAL(12,2) NOT NULL,
    paid_at DATETIME DEFAULT GETDATE(),
    recorded_by_user_id BIGINT,
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (sale_id) REFERENCES Sale(sale_id),
    FOREIGN KEY (recorded_by_user_id) REFERENCES UserAccount(user_id),
    CONSTRAINT CHK_PaymentMethod CHECK (payment_method IN ('cash','card','gcash','voucher'))
);

-- =============================================
-- 21. LedgerAccount Table
-- =============================================
CREATE TABLE LedgerAccount (
    ledger_account_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    code VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(200) NOT NULL,
    account_type VARCHAR(20) NOT NULL,
    normal_balance VARCHAR(10) NOT NULL,
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    CONSTRAINT CHK_AccountType CHECK (account_type IN ('asset','liability','equity','revenue','expense')),
    CONSTRAINT CHK_NormalBalance CHECK (normal_balance IN ('debit','credit'))
);

-- =============================================
-- 22. JournalEntry Table
-- =============================================
CREATE TABLE JournalEntry (
    journal_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    journal_no VARCHAR(80) UNIQUE,
    date DATE NOT NULL,
    description VARCHAR(MAX),
    created_by_user_id BIGINT,
    created_at DATETIME DEFAULT GETDATE(),
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id)
);

-- =============================================
-- 23. JournalEntryLine Table
-- =============================================
CREATE TABLE JournalEntryLine (
    journal_line_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    journal_id BIGINT NOT NULL,
    ledger_account_id BIGINT NOT NULL,
    debit DECIMAL(14,2) DEFAULT 0,
    credit DECIMAL(14,2) DEFAULT 0,
    line_memo VARCHAR(MAX),
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (journal_id) REFERENCES JournalEntry(journal_id),
    FOREIGN KEY (ledger_account_id) REFERENCES LedgerAccount(ledger_account_id)
);

-- =============================================
-- 24. CRM_Note Table
-- =============================================
CREATE TABLE CRM_Note (
    note_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    customer_id BIGINT NOT NULL,
    created_by_user_id BIGINT NOT NULL,
    note VARCHAR(MAX),
    created_at DATETIME DEFAULT GETDATE(),
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (customer_id) REFERENCES Customer(customer_id),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id)
);

-- =============================================
-- 25. AuditLog Table
-- =============================================
CREATE TABLE AuditLog (
    audit_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    entity_name VARCHAR(120),
    entity_id VARCHAR(80),
    action VARCHAR(10),
    changed_by_user_id BIGINT,
    change_summary VARCHAR(MAX),
    created_at DATETIME DEFAULT GETDATE(),
    -- Sync columns (AuditLog typically syncs one-way but included for consistency)
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (changed_by_user_id) REFERENCES UserAccount(user_id),
    CONSTRAINT CHK_AuditAction CHECK (action IN ('create','update','delete'))
);

-- =============================================
-- 26. Expense Table
-- =============================================
CREATE TABLE Expense (
    expense_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    expense_date DATE NOT NULL,
    category VARCHAR(50) NOT NULL,
    description NVARCHAR(500) NOT NULL,
    amount DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    vendor NVARCHAR(200),
    reference_number VARCHAR(100),
    payment_method VARCHAR(30) DEFAULT 'Cash',
    status VARCHAR(20) DEFAULT 'paid',
    notes NVARCHAR(500),
    ledger_account_id BIGINT NULL,
    journal_id BIGINT NULL,
    created_by_user_id BIGINT,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME,
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (ledger_account_id) REFERENCES LedgerAccount(ledger_account_id),
    FOREIGN KEY (journal_id) REFERENCES JournalEntry(journal_id),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id),
    CONSTRAINT CHK_ExpenseStatus CHECK (status IN ('paid','pending','cancelled'))
);

-- =============================================
-- 27. Payroll Table
-- =============================================
CREATE TABLE Payroll (
    payroll_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    employee_id BIGINT NOT NULL,
    period_start DATE NOT NULL,
    period_end DATE NOT NULL,
    days_worked INT DEFAULT 0,
    daily_rate DECIMAL(10,2) DEFAULT 500.00,
    gross_pay DECIMAL(12,2) DEFAULT 0.00,
    deductions DECIMAL(12,2) DEFAULT 0.00,
    net_pay DECIMAL(12,2) DEFAULT 0.00,
    status VARCHAR(20) DEFAULT 'draft',
    paid_at DATETIME,
    journal_id BIGINT NULL,
    notes NVARCHAR(500),
    created_by_user_id BIGINT,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME,
    -- Sync columns
    sync_id UNIQUEIDENTIFIER DEFAULT NEWID(),
    last_modified_at DATETIME2,
    last_synced_at DATETIME2,
    sync_status NVARCHAR(20) DEFAULT 'pending',
    sync_version INT DEFAULT 1,
    FOREIGN KEY (employee_id) REFERENCES Employee(employee_id),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id),
    FOREIGN KEY (journal_id) REFERENCES JournalEntry(journal_id),
    CONSTRAINT CHK_PayrollStatus CHECK (status IN ('draft','paid'))
);

-- =============================================
-- INDEXES FOR PERFORMANCE
-- =============================================
CREATE INDEX IX_Appointment_CustomerId ON Appointment(customer_id);
CREATE INDEX IX_Appointment_ScheduledStart ON Appointment(scheduled_start);
CREATE INDEX IX_Appointment_Status ON Appointment(status);
CREATE INDEX IX_Sale_CreatedAt ON Sale(created_at);
CREATE INDEX IX_Sale_CustomerId ON Sale(customer_id);
CREATE INDEX IX_Payment_SaleId ON Payment(sale_id);
CREATE INDEX IX_Expense_Date ON Expense(expense_date);
CREATE INDEX IX_Payroll_EmployeeId ON Payroll(employee_id);
CREATE INDEX IX_Payroll_PeriodStart ON Payroll(period_start);

-- Sync-related indexes for faster sync queries
CREATE INDEX IX_Person_SyncStatus ON Person(sync_status);
CREATE INDEX IX_Customer_SyncStatus ON Customer(sync_status);
CREATE INDEX IX_Employee_SyncStatus ON Employee(sync_status);
CREATE INDEX IX_Appointment_SyncStatus ON Appointment(sync_status);
CREATE INDEX IX_Sale_SyncStatus ON Sale(sync_status);
CREATE INDEX IX_Service_SyncStatus ON Service(sync_status);
CREATE INDEX IX_Product_SyncStatus ON Product(sync_status);

-- =============================================
-- INITIAL DATA: Roles
-- =============================================
INSERT INTO Role (name, is_archived) VALUES 
('superadmin', 0),
('admin', 0),
('manager', 0),
('therapist', 0),
('receptionist', 0),
('accountant', 0);

-- =============================================
-- INITIAL DATA: Ledger Accounts (Chart of Accounts)
-- =============================================
INSERT INTO LedgerAccount (code, name, account_type, normal_balance) VALUES
-- Assets
('1000', 'Cash on Hand', 'asset', 'debit'),
('1010', 'Cash in Bank', 'asset', 'debit'),
('1100', 'Accounts Receivable', 'asset', 'debit'),
('1200', 'Inventory', 'asset', 'debit'),
('1300', 'Prepaid Expenses', 'asset', 'debit'),
('1500', 'Equipment', 'asset', 'debit'),
('1510', 'Accumulated Depreciation', 'asset', 'credit'),
-- Liabilities
('2000', 'Accounts Payable', 'liability', 'credit'),
('2100', 'Accrued Expenses', 'liability', 'credit'),
('2200', 'Unearned Revenue', 'liability', 'credit'),
('2300', 'Loans Payable', 'liability', 'credit'),
-- Equity
('3000', 'Owner Capital', 'equity', 'credit'),
('3100', 'Retained Earnings', 'equity', 'credit'),
('3200', 'Owner Draws', 'equity', 'debit'),
-- Revenue
('4000', 'Service Revenue', 'revenue', 'credit'),
('4100', 'Product Sales', 'revenue', 'credit'),
('4200', 'Other Income', 'revenue', 'credit'),
-- Expenses
('5000', 'Cost of Goods Sold', 'expense', 'debit'),
('5100', 'Salaries & Wages', 'expense', 'debit'),
('5200', 'Rent Expense', 'expense', 'debit'),
('5300', 'Utilities Expense', 'expense', 'debit'),
('5400', 'Supplies Expense', 'expense', 'debit'),
('5500', 'Marketing Expense', 'expense', 'debit'),
('5600', 'Insurance Expense', 'expense', 'debit'),
('5700', 'Depreciation Expense', 'expense', 'debit'),
('5800', 'Miscellaneous Expense', 'expense', 'debit'),
('5900', 'Commission Expense', 'expense', 'debit');

-- =============================================
-- INITIAL DATA: Service Categories
-- =============================================
INSERT INTO ServiceCategory (name, description, is_archived) VALUES
('Massage', 'Various massage therapy services', 0),
('Facial', 'Facial treatments and skincare', 0),
('Body Treatment', 'Body scrubs, wraps, and treatments', 0),
('Nail Care', 'Manicure and pedicure services', 0),
('Hair Care', 'Hair treatments and styling', 0),
('Packages', 'Bundled spa packages', 0);
