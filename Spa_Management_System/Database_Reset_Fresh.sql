-- =====================================================
-- DATABASE RESET & FRESH SEED SCRIPT
-- Spa Management System
-- Run this to completely reset and start fresh
-- =====================================================

-- Disable foreign key constraints temporarily
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

-- =====================================================
-- STEP 1: DELETE ALL DATA (in correct order)
-- =====================================================
PRINT 'Deleting all data...';

-- Payment & Sales related
DELETE FROM Payment;
DELETE FROM SaleItem;
DELETE FROM Sale;

-- Appointments
DELETE FROM AppointmentService;
DELETE FROM Appointment;

-- Inventory & Purchasing
DELETE FROM StockTransaction;
DELETE FROM PurchaseOrderItem;
DELETE FROM PurchaseOrder;
DELETE FROM Inventory;

-- Accounting
DELETE FROM JournalEntryLine;
DELETE FROM JournalEntry;
DELETE FROM Expense;
DELETE FROM Payroll;

-- Employee related
DELETE FROM EmployeeServiceCommission;
DELETE FROM AuditLog;
DELETE FROM CRM_Note;

-- Core entities
DELETE FROM UserAccount;
DELETE FROM Employee;
DELETE FROM Customer;
DELETE FROM Person;

-- Products & Services
DELETE FROM Product;
DELETE FROM Service;
DELETE FROM ServiceCategory;

-- Reference data
DELETE FROM Supplier;
DELETE FROM LedgerAccount;
DELETE FROM Role;

PRINT 'All data deleted.';

-- =====================================================
-- STEP 2: RESET IDENTITY SEEDS
-- =====================================================
PRINT 'Resetting identity seeds...';

DBCC CHECKIDENT ('Payment', RESEED, 0);
DBCC CHECKIDENT ('SaleItem', RESEED, 0);
DBCC CHECKIDENT ('Sale', RESEED, 0);
DBCC CHECKIDENT ('AppointmentService', RESEED, 0);
DBCC CHECKIDENT ('Appointment', RESEED, 0);
DBCC CHECKIDENT ('StockTransaction', RESEED, 0);
DBCC CHECKIDENT ('PurchaseOrderItem', RESEED, 0);
DBCC CHECKIDENT ('PurchaseOrder', RESEED, 0);
DBCC CHECKIDENT ('Inventory', RESEED, 0);
DBCC CHECKIDENT ('JournalEntryLine', RESEED, 0);
DBCC CHECKIDENT ('JournalEntry', RESEED, 0);
DBCC CHECKIDENT ('Expense', RESEED, 0);
DBCC CHECKIDENT ('Payroll', RESEED, 0);
DBCC CHECKIDENT ('EmployeeServiceCommission', RESEED, 0);
DBCC CHECKIDENT ('AuditLog', RESEED, 0);
DBCC CHECKIDENT ('CRM_Note', RESEED, 0);
DBCC CHECKIDENT ('UserAccount', RESEED, 0);
DBCC CHECKIDENT ('Employee', RESEED, 0);
DBCC CHECKIDENT ('Customer', RESEED, 0);
DBCC CHECKIDENT ('Person', RESEED, 0);
DBCC CHECKIDENT ('Product', RESEED, 0);
DBCC CHECKIDENT ('Service', RESEED, 0);
DBCC CHECKIDENT ('ServiceCategory', RESEED, 0);
DBCC CHECKIDENT ('Supplier', RESEED, 0);
DBCC CHECKIDENT ('LedgerAccount', RESEED, 0);
DBCC CHECKIDENT ('Role', RESEED, 0);

PRINT 'Identity seeds reset.';

-- Re-enable foreign key constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';

-- =====================================================
-- STEP 3: SEED ESSENTIAL DATA
-- =====================================================
PRINT 'Seeding essential data...';

-- 3.1 Roles (only name column, no description in this schema)
INSERT INTO Role (name) VALUES 
('Super Admin'),
('Manager'),
('Therapist'),
('Receptionist');

PRINT 'Roles created.';

-- 3.2 Person for Admin
INSERT INTO Person (first_name, last_name, email, phone, address, sync_id, sync_status, sync_version) 
VALUES ('System', 'Administrator', 'admin@kayespa.com', '09171234567', 'Main Branch', NEWID(), 'synced', 1);

PRINT 'Admin person created.';

-- 3.3 Employee for Admin (role_id = 1 = Super Admin)
-- Note: Employee uses 'status' not 'employment_status'
INSERT INTO Employee (person_id, role_id, hire_date, status, sync_id, sync_status, sync_version)
VALUES (1, 1, GETDATE(), 'active', NEWID(), 'synced', 1);

PRINT 'Admin employee created.';

-- 3.4 UserAccount with HASHED password
-- Password: admin123 -> SHA256: 240be518fabd2724ddb6f04eeb9d5b076de1ea718d3d1d4a7abb3e0f4b5f8f4e
INSERT INTO UserAccount (employee_id, username, password_hash, is_active)
VALUES (1, 'admin', '240be518fabd2724ddb6f04eeb9d5b076de1ea718d3d1d4a7abb3e0f4b5f8f4e', 1);

PRINT 'Admin user account created.';

-- 3.5 Basic Service Categories
INSERT INTO ServiceCategory (name, description) VALUES
('Massage', 'Therapeutic and relaxation massage services'),
('Facial', 'Facial treatments and skincare'),
('Body Treatment', 'Body scrubs, wraps, and treatments'),
('Nail Care', 'Manicure, pedicure, and nail art'),
('Hair Care', 'Hair treatments and styling');

PRINT 'Service categories created.';

-- 3.6 Basic Ledger Accounts (Chart of Accounts)
-- Using: code, name, account_type, normal_balance
INSERT INTO LedgerAccount (code, name, account_type, normal_balance) VALUES
('1000', 'Assets', 'Asset', 'Debit'),
('1100', 'Cash on Hand', 'Asset', 'Debit'),
('1200', 'Accounts Receivable', 'Asset', 'Debit'),
('1300', 'Inventory', 'Asset', 'Debit'),
('2000', 'Liabilities', 'Liability', 'Credit'),
('2100', 'Accounts Payable', 'Liability', 'Credit'),
('3000', 'Equity', 'Equity', 'Credit'),
('3100', 'Owner Capital', 'Equity', 'Credit'),
('3200', 'Retained Earnings', 'Equity', 'Credit'),
('4000', 'Revenue', 'Revenue', 'Credit'),
('4100', 'Service Revenue', 'Revenue', 'Credit'),
('4200', 'Product Sales', 'Revenue', 'Credit'),
('5000', 'Expenses', 'Expense', 'Debit'),
('5100', 'Rent Expense', 'Expense', 'Debit'),
('5200', 'Utilities Expense', 'Expense', 'Debit'),
('5300', 'Salaries Expense', 'Expense', 'Debit'),
('5400', 'Supplies Expense', 'Expense', 'Debit'),
('5500', 'Marketing Expense', 'Expense', 'Debit'),
('5600', 'Commission Expense', 'Expense', 'Debit'),
('5700', 'Cost of Goods Sold', 'Expense', 'Debit');

PRINT 'Ledger accounts created.';

-- =====================================================
-- DONE!
-- =====================================================
PRINT '';
PRINT '=====================================================';
PRINT 'DATABASE RESET COMPLETE!';
PRINT '=====================================================';
PRINT 'Login credentials:';
PRINT '  Username: admin';
PRINT '  Password: admin123';
PRINT '=====================================================';

SELECT 'Database reset successful!' AS Result, 
       (SELECT COUNT(*) FROM Role) AS Roles,
       (SELECT COUNT(*) FROM Person) AS Persons,
       (SELECT COUNT(*) FROM Employee) AS Employees,
       (SELECT COUNT(*) FROM UserAccount) AS Users,
       (SELECT COUNT(*) FROM ServiceCategory) AS ServiceCategories,
       (SELECT COUNT(*) FROM LedgerAccount) AS LedgerAccounts;
