/* =============================================
   SPA MANAGEMENT SYSTEM - SQL SERVER (T-SQL)
   =============================================
*/

-- 1. Person Table (Base table for Customers and Employees)
CREATE TABLE Person (
    person_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    first_name VARCHAR(120) NOT NULL,
    last_name VARCHAR(120) NOT NULL,
    email VARCHAR(200),
    phone VARCHAR(50),
    address VARCHAR(MAX),
    dob DATE,
    created_at DATETIME DEFAULT GETDATE()
);

-- 2. Role Table
CREATE TABLE Role (
    role_id SMALLINT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

-- 3. Employee Table
CREATE TABLE Employee (
    employee_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    person_id BIGINT NOT NULL,
    role_id SMALLINT NOT NULL,
    hire_date DATE,
    status VARCHAR(30) DEFAULT 'active',
    note VARCHAR(MAX),
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (person_id) REFERENCES Person(person_id),
    FOREIGN KEY (role_id) REFERENCES Role(role_id)
);

-- 4. UserAccount Table
CREATE TABLE UserAccount (
    user_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    employee_id BIGINT NULL,
    username VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    is_active BIT DEFAULT 1,
    last_login DATETIME,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (employee_id) REFERENCES Employee(employee_id)
);

-- 5. Customer Table
CREATE TABLE Customer (
    customer_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    person_id BIGINT NOT NULL,
    customer_code VARCHAR(50) UNIQUE,
    loyalty_points INT DEFAULT 0,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (person_id) REFERENCES Person(person_id)
);

-- 6. ServiceCategory Table
CREATE TABLE ServiceCategory (
    service_category_id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(MAX)
);

-- 7. Service Table
CREATE TABLE Service (
    service_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    service_category_id INT NULL,
    code VARCHAR(60) UNIQUE,
    name VARCHAR(150) NOT NULL,
    description VARCHAR(MAX),
    price DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    duration_minutes INT DEFAULT 0,
    active BIT DEFAULT 1,
    FOREIGN KEY (service_category_id) REFERENCES ServiceCategory(service_category_id)
);

-- 8. EmployeeServiceCommission Table
CREATE TABLE EmployeeServiceCommission (
    commission_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    employee_id BIGINT NOT NULL,
    service_id BIGINT NOT NULL,
    commission_type VARCHAR(10) NOT NULL,
    commission_value DECIMAL(10,2) NOT NULL,
    effective_from DATE,
    effective_to DATE,
    FOREIGN KEY (employee_id) REFERENCES Employee(employee_id),
    FOREIGN KEY (service_id) REFERENCES Service(service_id),
    CONSTRAINT CHK_CommissionType CHECK (commission_type IN ('percent','fixed'))
);

-- 9. Product Table
CREATE TABLE Product (
    product_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    sku VARCHAR(80) UNIQUE,
    name VARCHAR(200) NOT NULL,
    description VARCHAR(MAX),
    unit_price DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    cost_price DECIMAL(12,2) DEFAULT 0.00,
    unit VARCHAR(20),
    active BIT DEFAULT 1
);

-- 10. Inventory Table
CREATE TABLE Inventory (
    inventory_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    product_id BIGINT NOT NULL UNIQUE,
    quantity_on_hand DECIMAL(12,2) DEFAULT 0,
    reorder_level DECIMAL(12,2) DEFAULT 0,
    last_counted_at DATETIME,
    FOREIGN KEY (product_id) REFERENCES Product(product_id)
);

-- 11. StockTransaction Table
CREATE TABLE StockTransaction (
    stock_tx_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    product_id BIGINT NOT NULL,
    tx_type VARCHAR(10) NOT NULL,
    qty DECIMAL(12,2) NOT NULL,
    unit_cost DECIMAL(12,2),
    reference VARCHAR(120),
    created_by_user_id BIGINT,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (product_id) REFERENCES Product(product_id),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id),
    CONSTRAINT CHK_TxType CHECK (tx_type IN ('purchase','sale','adjust','return'))
);

-- 12. Supplier Table
CREATE TABLE Supplier (
    supplier_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    contact_person VARCHAR(200),
    phone VARCHAR(50),
    email VARCHAR(150),
    address VARCHAR(MAX)
);

-- 13. PurchaseOrder Table
CREATE TABLE PurchaseOrder (
    po_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    supplier_id BIGINT NOT NULL,
    created_by_user_id BIGINT,
    po_number VARCHAR(80) UNIQUE,
    status VARCHAR(40),
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (supplier_id) REFERENCES Supplier(supplier_id),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id)
);

-- 14. PurchaseOrderItem Table
CREATE TABLE PurchaseOrderItem (
    po_item_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    po_id BIGINT NOT NULL,
    product_id BIGINT NOT NULL,
    qty_ordered DECIMAL(12,2) NOT NULL,
    unit_cost DECIMAL(12,2) NOT NULL,
    FOREIGN KEY (po_id) REFERENCES PurchaseOrder(po_id),
    FOREIGN KEY (product_id) REFERENCES Product(product_id)
);

-- 15. Appointment Table
CREATE TABLE Appointment (
    appointment_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    customer_id BIGINT NOT NULL,
    scheduled_start DATETIME NOT NULL,
    scheduled_end DATETIME,
    status VARCHAR(40) DEFAULT 'scheduled',
    created_by_user_id BIGINT,
    created_at DATETIME DEFAULT GETDATE(),
    notes VARCHAR(MAX),
    FOREIGN KEY (customer_id) REFERENCES Customer(customer_id),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id)
);

-- 16. AppointmentService Table
CREATE TABLE AppointmentService (
    appt_srv_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    appointment_id BIGINT NOT NULL,
    service_id BIGINT NOT NULL,
    therapist_employee_id BIGINT NULL,
    price DECIMAL(12,2) NOT NULL,
    commission_amount DECIMAL(12,2) DEFAULT 0,
    FOREIGN KEY (appointment_id) REFERENCES Appointment(appointment_id),
    FOREIGN KEY (service_id) REFERENCES Service(service_id),
    FOREIGN KEY (therapist_employee_id) REFERENCES Employee(employee_id)
);

-- 17. Sale Table
CREATE TABLE Sale (
    sale_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    customer_id BIGINT NULL,
    created_by_user_id BIGINT,
    sale_number VARCHAR(80) UNIQUE,
    total_amount DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    payment_status VARCHAR(40) DEFAULT 'unpaid',
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (customer_id) REFERENCES Customer(customer_id),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id)
);

-- 18. SaleItem Table
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
    FOREIGN KEY (sale_id) REFERENCES Sale(sale_id),
    FOREIGN KEY (product_id) REFERENCES Product(product_id),
    FOREIGN KEY (service_id) REFERENCES Service(service_id),
    FOREIGN KEY (therapist_employee_id) REFERENCES Employee(employee_id),
    CONSTRAINT CHK_ItemType CHECK (item_type IN ('product','service'))
);

-- 19. Payment Table
CREATE TABLE Payment (
    payment_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    sale_id BIGINT NOT NULL,
    payment_method VARCHAR(20) NOT NULL,
    amount DECIMAL(12,2) NOT NULL,
    paid_at DATETIME DEFAULT GETDATE(),
    recorded_by_user_id BIGINT,
    FOREIGN KEY (sale_id) REFERENCES Sale(sale_id),
    FOREIGN KEY (recorded_by_user_id) REFERENCES UserAccount(user_id),
    CONSTRAINT CHK_PaymentMethod CHECK (payment_method IN ('cash','card','voucher'))
);

-- 20. LedgerAccount Table
CREATE TABLE LedgerAccount (
    ledger_account_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    code VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(200) NOT NULL,
    account_type VARCHAR(20) NOT NULL,
    normal_balance VARCHAR(10) NOT NULL,
    CONSTRAINT CHK_AccountType CHECK (account_type IN ('asset','liability','equity','revenue','expense')),
    CONSTRAINT CHK_NormalBalance CHECK (normal_balance IN ('debit','credit'))
);

-- 21. JournalEntry Table
CREATE TABLE JournalEntry (
    journal_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    journal_no VARCHAR(80) UNIQUE,
    date DATE NOT NULL,
    description VARCHAR(MAX),
    created_by_user_id BIGINT,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id)
);

-- 22. JournalEntryLine Table
CREATE TABLE JournalEntryLine (
    journal_line_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    journal_id BIGINT NOT NULL,
    ledger_account_id BIGINT NOT NULL,
    debit DECIMAL(14,2) DEFAULT 0,
    credit DECIMAL(14,2) DEFAULT 0,
    line_memo VARCHAR(MAX),
    FOREIGN KEY (journal_id) REFERENCES JournalEntry(journal_id),
    FOREIGN KEY (ledger_account_id) REFERENCES LedgerAccount(ledger_account_id)
);

-- 23. CRM_Note Table
CREATE TABLE CRM_Note (
    note_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    customer_id BIGINT NOT NULL,
    created_by_user_id BIGINT NOT NULL,
    note VARCHAR(MAX),
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (customer_id) REFERENCES Customer(customer_id),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id)
);

-- 24. AuditLog Table
CREATE TABLE AuditLog (
    audit_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    entity_name VARCHAR(120),
    entity_id VARCHAR(80),
    action VARCHAR(10),
    changed_by_user_id BIGINT,
    change_summary VARCHAR(MAX),
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (changed_by_user_id) REFERENCES UserAccount(user_id),
    CONSTRAINT CHK_AuditAction CHECK (action IN ('create','update','delete'))
);
