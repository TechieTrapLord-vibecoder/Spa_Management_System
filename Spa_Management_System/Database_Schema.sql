/* =============================================
   SPA MANAGEMENT SYSTEM - SQL SERVER (T-SQL)
   Updated: November 2025
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
    name VARCHAR(50) NOT NULL UNIQUE,
    is_archived BIT DEFAULT 0
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
    is_archived BIT DEFAULT 0,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (person_id) REFERENCES Person(person_id)
);

-- 6. ServiceCategory Table
CREATE TABLE ServiceCategory (
    service_category_id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(MAX),
    is_archived BIT DEFAULT 0
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
    is_archived BIT DEFAULT 0,
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
    address VARCHAR(MAX),
    is_archived BIT DEFAULT 0
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
    CONSTRAINT CHK_PaymentMethod CHECK (payment_method IN ('cash','card','gcash','voucher'))
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

-- 25. Expense Table (NEW)
CREATE TABLE Expense (
    expense_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    expense_date DATE NOT NULL,
    category VARCHAR(100) NOT NULL,
    description VARCHAR(300) NOT NULL,
    amount DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    vendor VARCHAR(100),
    reference_number VARCHAR(100),
    payment_method VARCHAR(50) DEFAULT 'Cash',
    status VARCHAR(30) DEFAULT 'paid',
    notes VARCHAR(MAX),
    ledger_account_id BIGINT NULL,
    journal_id BIGINT NULL,
    created_by_user_id BIGINT,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME,
    FOREIGN KEY (ledger_account_id) REFERENCES LedgerAccount(ledger_account_id),
    FOREIGN KEY (journal_id) REFERENCES JournalEntry(journal_id),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id),
    CONSTRAINT CHK_ExpenseStatus CHECK (status IN ('paid','pending','cancelled'))
);

-- 26. Payroll Table (NEW)
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
    notes VARCHAR(MAX),
    created_by_user_id BIGINT,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME,
    FOREIGN KEY (employee_id) REFERENCES Employee(employee_id),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id),
    FOREIGN KEY (journal_id) REFERENCES JournalEntry(journal_id),
    CONSTRAINT CHK_PayrollStatus CHECK (status IN ('draft','paid'))
);

-- 27. EmployeeAttendance Table (NEW)
CREATE TABLE EmployeeAttendance (
    attendance_id BIGINT IDENTITY(1,1) PRIMARY KEY,
    employee_id BIGINT NOT NULL,
    work_date DATE NOT NULL,
    days_worked DECIMAL(4,1) DEFAULT 0,
    notes VARCHAR(MAX),
    created_by_user_id BIGINT,
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME,
    FOREIGN KEY (employee_id) REFERENCES Employee(employee_id),
    FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id)
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
CREATE INDEX IX_EmployeeAttendance_EmployeeId ON EmployeeAttendance(employee_id);
CREATE INDEX IX_EmployeeAttendance_WorkDate ON EmployeeAttendance(work_date);

-- =============================================
-- SAMPLE DATA: PRODUCTS
-- =============================================
-- Massage Oils & Lotions
INSERT INTO Product (sku, name, description, unit_price, cost_price, unit, active) VALUES
('OIL-LAV-500', 'Lavender Massage Oil', 'Premium organic lavender essential oil blend for relaxation massage, 500ml', 450.00, 280.00, 'bottle', 1),
('OIL-EUC-500', 'Eucalyptus Massage Oil', 'Refreshing eucalyptus oil blend for therapeutic massage, 500ml', 420.00, 250.00, 'bottle', 1),
('OIL-COC-1L', 'Virgin Coconut Oil', 'Pure virgin coconut oil for body treatments, 1 liter', 380.00, 220.00, 'bottle', 1),
('OIL-ARG-250', 'Argan Oil Premium', 'Moroccan argan oil for hair and skin treatments, 250ml', 650.00, 400.00, 'bottle', 1),
('OIL-JOJ-500', 'Jojoba Carrier Oil', 'Cold-pressed jojoba oil for mixing with essentials, 500ml', 520.00, 320.00, 'bottle', 1),
('LOT-ALO-500', 'Aloe Vera Body Lotion', 'Soothing aloe vera lotion for after-treatment care, 500ml', 350.00, 180.00, 'bottle', 1),
('LOT-SHE-300', 'Shea Butter Cream', 'Rich shea butter moisturizing cream, 300g', 480.00, 280.00, 'jar', 1),
('OIL-HOT-250', 'Hot Stone Oil', 'Special warming oil for hot stone massage, 250ml', 380.00, 200.00, 'bottle', 1);

-- Face & Skin Care Products
INSERT INTO Product (sku, name, description, unit_price, cost_price, unit, active) VALUES
('FACE-CLN-200', 'Facial Cleanser Gentle', 'Gentle foaming cleanser for all skin types, 200ml', 320.00, 160.00, 'bottle', 1),
('FACE-TNR-150', 'Rose Toner', 'Alcohol-free rose water toner, 150ml', 280.00, 140.00, 'bottle', 1),
('FACE-SRM-50', 'Vitamin C Serum', 'Brightening vitamin C serum with hyaluronic acid, 50ml', 750.00, 420.00, 'bottle', 1),
('FACE-MSK-100', 'Hydrating Face Mask', 'Deep hydrating gel mask with aloe, 100g', 420.00, 220.00, 'jar', 1),
('FACE-MSK-CLY', 'Clay Detox Mask', 'Bentonite clay mask for deep pore cleansing, 150g', 380.00, 190.00, 'jar', 1),
('FACE-CRM-50', 'Anti-Aging Night Cream', 'Retinol night cream for mature skin, 50g', 850.00, 480.00, 'jar', 1),
('FACE-EYE-15', 'Eye Contour Cream', 'Firming eye cream with peptides, 15ml', 620.00, 350.00, 'tube', 1),
('FACE-SCR-100', 'Exfoliating Face Scrub', 'Gentle exfoliating scrub with natural beads, 100g', 350.00, 170.00, 'tube', 1);

-- Body Scrubs & Treatments
INSERT INTO Product (sku, name, description, unit_price, cost_price, unit, active) VALUES
('BODY-SCR-SAL', 'Himalayan Salt Scrub', 'Pink Himalayan salt body scrub with essential oils, 500g', 580.00, 320.00, 'jar', 1),
('BODY-SCR-SUG', 'Brown Sugar Scrub', 'Gentle brown sugar body scrub with vanilla, 400g', 450.00, 240.00, 'jar', 1),
('BODY-SCR-COF', 'Coffee Body Scrub', 'Energizing arabica coffee scrub for cellulite, 400g', 520.00, 280.00, 'jar', 1),
('BODY-WRP-MUD', 'Dead Sea Mud Wrap', 'Detoxifying Dead Sea mud for body wraps, 1kg', 780.00, 450.00, 'tub', 1),
('BODY-WRP-SEA', 'Seaweed Body Wrap', 'Mineral-rich seaweed powder for slimming wraps, 500g', 650.00, 380.00, 'pack', 1);

-- Aromatherapy & Essential Oils
INSERT INTO Product (sku, name, description, unit_price, cost_price, unit, active) VALUES
('AROM-LAV-30', 'Lavender Essential Oil', 'Pure lavender essential oil for diffuser, 30ml', 380.00, 200.00, 'bottle', 1),
('AROM-PEP-30', 'Peppermint Essential Oil', 'Cooling peppermint oil for aromatherapy, 30ml', 350.00, 180.00, 'bottle', 1),
('AROM-TEA-30', 'Tea Tree Essential Oil', 'Antibacterial tea tree oil, 30ml', 320.00, 160.00, 'bottle', 1),
('AROM-LEM-30', 'Lemongrass Essential Oil', 'Uplifting lemongrass oil for energy, 30ml', 300.00, 150.00, 'bottle', 1),
('AROM-CND-LAV', 'Aromatherapy Candle Lavender', 'Soy wax candle with lavender scent, 200g', 450.00, 220.00, 'piece', 1),
('AROM-CND-VAN', 'Aromatherapy Candle Vanilla', 'Relaxing vanilla scented soy candle, 200g', 450.00, 220.00, 'piece', 1),
('AROM-DIF-REF', 'Reed Diffuser Refill', 'Essential oil refill for reed diffuser, 200ml', 380.00, 190.00, 'bottle', 1);

-- Hair Care Products
INSERT INTO Product (sku, name, description, unit_price, cost_price, unit, active) VALUES
('HAIR-SHP-300', 'Keratin Shampoo', 'Smoothing keratin shampoo for damaged hair, 300ml', 420.00, 220.00, 'bottle', 1),
('HAIR-CND-300', 'Keratin Conditioner', 'Deep conditioning keratin treatment, 300ml', 450.00, 240.00, 'bottle', 1),
('HAIR-MSK-250', 'Hair Repair Mask', 'Intensive repair mask for dry hair, 250g', 520.00, 280.00, 'jar', 1),
('HAIR-OIL-100', 'Argan Hair Serum', 'Smoothing argan oil hair serum, 100ml', 480.00, 260.00, 'bottle', 1),
('HAIR-TRT-50', 'Scalp Treatment Oil', 'Nourishing scalp treatment with tea tree, 50ml', 380.00, 200.00, 'bottle', 1);

-- Nail Care Products
INSERT INTO Product (sku, name, description, unit_price, cost_price, unit, active) VALUES
('NAIL-RMV-120', 'Nail Polish Remover', 'Acetone-free gentle nail polish remover, 120ml', 180.00, 80.00, 'bottle', 1),
('NAIL-OIL-15', 'Cuticle Oil', 'Nourishing cuticle oil with vitamin E, 15ml', 220.00, 100.00, 'bottle', 1),
('NAIL-CRM-50', 'Hand & Nail Cream', 'Intensive hand cream with keratin, 50g', 280.00, 140.00, 'tube', 1),
('NAIL-FILE-SET', 'Professional Nail File Set', 'Set of 3 different grit nail files', 150.00, 60.00, 'set', 1),
('NAIL-BUF-3PK', 'Nail Buffer 3-Pack', 'Three-way nail buffing blocks', 120.00, 45.00, 'pack', 1);

-- Spa Accessories & Supplies
INSERT INTO Product (sku, name, description, unit_price, cost_price, unit, active) VALUES
('ACC-TOW-LRG', 'Spa Towel Large', 'Premium cotton spa towel, 70x140cm, white', 350.00, 180.00, 'piece', 1),
('ACC-TOW-SML', 'Face Towel', 'Soft cotton face towel, 30x30cm, white', 120.00, 55.00, 'piece', 1),
('ACC-ROBE-M', 'Spa Robe Medium', 'Plush cotton spa robe, medium size', 850.00, 480.00, 'piece', 1),
('ACC-ROBE-L', 'Spa Robe Large', 'Plush cotton spa robe, large size', 850.00, 480.00, 'piece', 1),
('ACC-SLIP-S', 'Disposable Slippers', 'Single-use spa slippers, pack of 50 pairs', 750.00, 400.00, 'pack', 1),
('ACC-HDBN-10', 'Spa Headbands', 'Stretch terry headbands, pack of 10', 280.00, 140.00, 'pack', 1),
('ACC-MASK-50', 'Sheet Masks Assorted', 'Assorted hydrating sheet masks, box of 50', 1200.00, 700.00, 'box', 1),
('ACC-GLOVE-100', 'Disposable Gloves', 'Nitrile gloves, powder-free, box of 100', 450.00, 280.00, 'box', 1);

-- Retail Products for Customers
INSERT INTO Product (sku, name, description, unit_price, cost_price, unit, active) VALUES
('RTL-GFTSET-A', 'Spa Gift Set Deluxe', 'Luxury gift set: lotion, scrub, candle, bath bomb', 1500.00, 850.00, 'set', 1),
('RTL-GFTSET-B', 'Relaxation Kit', 'Gift set: massage oil, essential oil, eye mask', 980.00, 550.00, 'set', 1),
('RTL-BATH-BOM', 'Bath Bomb Set', 'Assorted bath bombs, set of 6', 480.00, 240.00, 'set', 1),
('RTL-EYEMSK', 'Silk Sleep Eye Mask', 'Pure silk eye mask for sleep', 380.00, 180.00, 'piece', 1),
('RTL-JADEROL', 'Jade Face Roller', 'Natural jade stone facial roller', 650.00, 320.00, 'piece', 1),
('RTL-GUASHA', 'Gua Sha Stone', 'Rose quartz gua sha facial tool', 550.00, 280.00, 'piece', 1);

-- =============================================
-- SAMPLE DATA: INVENTORY (Stock Levels)
-- =============================================
-- Create inventory records for all products
INSERT INTO Inventory (product_id, quantity_on_hand, reorder_level, last_counted_at)
SELECT p.product_id, 0, 10, GETDATE()
FROM Product p
WHERE NOT EXISTS (SELECT 1 FROM Inventory i WHERE i.product_id = p.product_id);

-- Set stock levels by category
UPDATE Inventory SET quantity_on_hand = 20, reorder_level = 5 WHERE product_id IN (SELECT product_id FROM Product WHERE sku LIKE 'OIL-%');
UPDATE Inventory SET quantity_on_hand = 15, reorder_level = 5 WHERE product_id IN (SELECT product_id FROM Product WHERE sku LIKE 'LOT-%');
UPDATE Inventory SET quantity_on_hand = 25, reorder_level = 8 WHERE product_id IN (SELECT product_id FROM Product WHERE sku LIKE 'FACE-%');
UPDATE Inventory SET quantity_on_hand = 12, reorder_level = 4 WHERE product_id IN (SELECT product_id FROM Product WHERE sku LIKE 'BODY-%');
UPDATE Inventory SET quantity_on_hand = 30, reorder_level = 10 WHERE product_id IN (SELECT product_id FROM Product WHERE sku LIKE 'AROM-%');
UPDATE Inventory SET quantity_on_hand = 18, reorder_level = 6 WHERE product_id IN (SELECT product_id FROM Product WHERE sku LIKE 'HAIR-%');
UPDATE Inventory SET quantity_on_hand = 40, reorder_level = 15 WHERE product_id IN (SELECT product_id FROM Product WHERE sku LIKE 'NAIL-%');
UPDATE Inventory SET quantity_on_hand = 50, reorder_level = 20 WHERE product_id IN (SELECT product_id FROM Product WHERE sku LIKE 'ACC-%');
UPDATE Inventory SET quantity_on_hand = 10, reorder_level = 3 WHERE product_id IN (SELECT product_id FROM Product WHERE sku LIKE 'RTL-%');
