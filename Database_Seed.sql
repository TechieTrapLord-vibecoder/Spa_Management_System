/* =============================================
   SPA MANAGEMENT SYSTEM - SEED DATA
   Run this AFTER creating the schema (Database_Schema.sql)
   ============================================= */

-- 1. Insert default roles
INSERT INTO Role (name) VALUES 
('SuperAdmin'),
('Manager'),
('Therapist'),
('Receptionist'),
('InventoryClerk');

-- 2. Insert a default SuperAdmin person
INSERT INTO Person (first_name, last_name, email, phone, address, dob, created_at)
VALUES ('System', 'Administrator', 'admin@kayespa.com', '09171234567', 'Spa Main Office', '1990-01-01', GETDATE());

-- Get the person_id for the admin
DECLARE @AdminPersonId BIGINT = SCOPE_IDENTITY();

-- 3. Create the SuperAdmin employee
INSERT INTO Employee (person_id, role_id, hire_date, status, note, created_at)
VALUES (@AdminPersonId, 1, GETDATE(), 'active', 'Default system administrator', GETDATE());

DECLARE @AdminEmployeeId BIGINT = SCOPE_IDENTITY();

-- 4. Create the admin user account (password: admin123)
-- Password hash for 'admin123' using SHA256: jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=
INSERT INTO UserAccount (employee_id, username, password_hash, is_active, created_at)
VALUES (@AdminEmployeeId, 'admin', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 1, GETDATE());

-- 5. Insert sample service categories
INSERT INTO ServiceCategory (name, description) VALUES
('Massage', 'Various massage therapy treatments'),
('Facial', 'Facial treatments and skincare'),
('Body Treatment', 'Full body treatments and wraps'),
('Nail Care', 'Manicure and pedicure services'),
('Hair Services', 'Haircut, styling, and treatment');

-- 6. Insert sample services
INSERT INTO Service (service_category_id, code, name, description, price, duration_minutes, active) VALUES
(1, 'MSG-SWE-60', 'Swedish Massage (60 min)', 'Classic relaxation massage with long flowing strokes', 800.00, 60, 1),
(1, 'MSG-SWE-90', 'Swedish Massage (90 min)', 'Extended classic relaxation massage', 1100.00, 90, 1),
(1, 'MSG-DEP-60', 'Deep Tissue Massage (60 min)', 'Intensive massage targeting deep muscle tension', 1000.00, 60, 1),
(1, 'MSG-HOT-60', 'Hot Stone Massage (60 min)', 'Heated stone therapy for ultimate relaxation', 1200.00, 60, 1),
(1, 'MSG-ARO-60', 'Aromatherapy Massage (60 min)', 'Essential oil infused massage therapy', 900.00, 60, 1),
(2, 'FAC-BAS-60', 'Basic Facial', 'Deep cleansing facial with extraction', 600.00, 60, 1),
(2, 'FAC-HYD-60', 'Hydrating Facial', 'Intensive moisturizing treatment', 800.00, 60, 1),
(2, 'FAC-ANT-60', 'Anti-Aging Facial', 'Rejuvenating treatment with premium serums', 1200.00, 60, 1),
(3, 'BOD-SCR-45', 'Body Scrub', 'Full body exfoliation treatment', 700.00, 45, 1),
(3, 'BOD-WRP-60', 'Body Wrap', 'Detoxifying body wrap treatment', 1000.00, 60, 1),
(4, 'NAI-MAN-45', 'Classic Manicure', 'Basic nail care and polish', 300.00, 45, 1),
(4, 'NAI-PED-60', 'Classic Pedicure', 'Basic foot care and polish', 400.00, 60, 1),
(4, 'NAI-GEL-60', 'Gel Manicure', 'Long-lasting gel nail polish', 500.00, 60, 1),
(5, 'HAR-CUT-30', 'Haircut', 'Professional haircut and styling', 350.00, 30, 1),
(5, 'HAR-TRT-45', 'Hair Treatment', 'Deep conditioning treatment', 500.00, 45, 1);

-- 7. Insert sample products
INSERT INTO Product (sku, name, description, unit_price, cost_price, unit, active) VALUES
('OIL-LAV-500', 'Lavender Massage Oil (500ml)', 'Premium lavender essential oil blend', 450.00, 200.00, 'bottle', 1),
('OIL-EUC-500', 'Eucalyptus Massage Oil (500ml)', 'Refreshing eucalyptus oil blend', 450.00, 200.00, 'bottle', 1),
('CRM-FAC-100', 'Facial Moisturizer (100ml)', 'Daily hydrating cream', 350.00, 150.00, 'jar', 1),
('SER-VIT-30', 'Vitamin C Serum (30ml)', 'Anti-aging vitamin C serum', 800.00, 350.00, 'bottle', 1),
('MSK-FAC-10', 'Face Mask (10 sheets)', 'Hydrating sheet masks', 250.00, 100.00, 'pack', 1),
('TOW-SPA-1', 'Spa Towel (White)', 'Premium cotton spa towel', 150.00, 75.00, 'piece', 1),
('SLP-DIS-50', 'Disposable Slippers (50 pairs)', 'Single-use spa slippers', 500.00, 250.00, 'box', 1),
('ROB-SPA-1', 'Spa Robe', 'Plush cotton bathrobe', 600.00, 300.00, 'piece', 1);

-- 8. Set initial inventory for products
INSERT INTO Inventory (product_id, quantity_on_hand, reorder_level, last_counted_at)
SELECT product_id, 50, 10, GETDATE() FROM Product;

-- 9. Insert sample suppliers
INSERT INTO Supplier (name, contact_person, phone, email, address) VALUES
('Beauty Supply Co.', 'Maria Santos', '09171112222', 'sales@beautysupply.ph', '123 Commerce St., Makati City'),
('Spa Essentials Inc.', 'Juan Dela Cruz', '09183334444', 'orders@spaessentials.ph', '456 Business Ave., BGC'),
('Natural Oils PH', 'Ana Garcia', '09195556666', 'info@naturaloils.ph', '789 Green St., Quezon City');

-- 10. Insert default ledger accounts (Chart of Accounts)
INSERT INTO LedgerAccount (code, name, account_type, normal_balance) VALUES
-- Assets
('1000', 'Cash on Hand', 'asset', 'debit'),
('1010', 'Cash in Bank', 'asset', 'debit'),
('1100', 'Accounts Receivable', 'asset', 'debit'),
('1200', 'Inventory', 'asset', 'debit'),
('1500', 'Equipment', 'asset', 'debit'),
-- Liabilities
('2000', 'Accounts Payable', 'liability', 'credit'),
('2100', 'Accrued Expenses', 'liability', 'credit'),
('2200', 'Unearned Revenue', 'liability', 'credit'),
-- Equity
('3000', 'Owner''s Capital', 'equity', 'credit'),
('3100', 'Retained Earnings', 'equity', 'credit'),
-- Revenue
('4000', 'Service Revenue', 'revenue', 'credit'),
('4100', 'Product Sales', 'revenue', 'credit'),
('4200', 'Other Income', 'revenue', 'credit'),
-- Expenses
('5000', 'Salaries & Wages', 'expense', 'debit'),
('5100', 'Commission Expense', 'expense', 'debit'),
('5200', 'Rent Expense', 'expense', 'debit'),
('5300', 'Utilities Expense', 'expense', 'debit'),
('5400', 'Supplies Expense', 'expense', 'debit'),
('5500', 'Cost of Goods Sold', 'expense', 'debit');

PRINT 'Seed data inserted successfully!';
PRINT 'Default admin account created:';
PRINT '  Username: admin';
PRINT '  Password: admin123';
