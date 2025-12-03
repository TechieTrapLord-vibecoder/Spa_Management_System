-- =============================================
-- CREATE 5 THERAPISTS + 30% COMMISSION
-- Run this in SQL Server Management Studio
-- =============================================

-- STEP 1: Add missing columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeServiceCommission') AND name = 'is_archived')
    ALTER TABLE EmployeeServiceCommission ADD is_archived BIT DEFAULT 0;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ServiceCategory') AND name = 'is_archived')
    ALTER TABLE ServiceCategory ADD is_archived BIT DEFAULT 0;

-- =============================================
-- STEP 2: CREATE 5 THERAPISTS
-- =============================================

-- Create Person records for therapists
INSERT INTO Person (first_name, last_name, email, phone, address, dob, created_at) VALUES
('Maria', 'Santos', 'maria.santos@kayespa.com', '09171111111', 'Manila, Philippines', '1995-03-15', GETDATE()),
('Jose', 'Reyes', 'jose.reyes@kayespa.com', '09172222222', 'Quezon City, Philippines', '1992-07-22', GETDATE()),
('Ana', 'Cruz', 'ana.cruz@kayespa.com', '09173333333', 'Makati, Philippines', '1998-01-10', GETDATE()),
('Juan', 'Garcia', 'juan.garcia@kayespa.com', '09174444444', 'Pasig, Philippines', '1994-11-05', GETDATE()),
('Rosa', 'Mendoza', 'rosa.mendoza@kayespa.com', '09175555555', 'Taguig, Philippines', '1996-06-18', GETDATE());

-- Get person IDs
DECLARE @Maria BIGINT = (SELECT person_id FROM Person WHERE email = 'maria.santos@kayespa.com');
DECLARE @Jose BIGINT = (SELECT person_id FROM Person WHERE email = 'jose.reyes@kayespa.com');
DECLARE @Ana BIGINT = (SELECT person_id FROM Person WHERE email = 'ana.cruz@kayespa.com');
DECLARE @Juan BIGINT = (SELECT person_id FROM Person WHERE email = 'juan.garcia@kayespa.com');
DECLARE @Rosa BIGINT = (SELECT person_id FROM Person WHERE email = 'rosa.mendoza@kayespa.com');
DECLARE @TherapistRole SMALLINT = (SELECT role_id FROM Role WHERE name = 'Therapist');

-- Create Employee records
INSERT INTO Employee (person_id, role_id, hire_date, status, note, created_at) VALUES
(@Maria, @TherapistRole, GETDATE(), 'active', 'Senior Therapist - All services', GETDATE()),
(@Jose, @TherapistRole, GETDATE(), 'active', 'Senior Therapist - All services', GETDATE()),
(@Ana, @TherapistRole, GETDATE(), 'active', 'Therapist - All services', GETDATE()),
(@Juan, @TherapistRole, GETDATE(), 'active', 'Therapist - All services', GETDATE()),
(@Rosa, @TherapistRole, GETDATE(), 'active', 'Therapist - All services', GETDATE());

-- Get employee IDs
DECLARE @MariaEmp BIGINT = (SELECT employee_id FROM Employee WHERE person_id = @Maria);
DECLARE @JoseEmp BIGINT = (SELECT employee_id FROM Employee WHERE person_id = @Jose);
DECLARE @AnaEmp BIGINT = (SELECT employee_id FROM Employee WHERE person_id = @Ana);
DECLARE @JuanEmp BIGINT = (SELECT employee_id FROM Employee WHERE person_id = @Juan);
DECLARE @RosaEmp BIGINT = (SELECT employee_id FROM Employee WHERE person_id = @Rosa);

-- Create UserAccount records (password: lloren123)
INSERT INTO UserAccount (employee_id, username, password_hash, is_active, created_at) VALUES
(@MariaEmp, 'maria', 'lloren123', 1, GETDATE()),
(@JoseEmp, 'jose', 'lloren123', 1, GETDATE()),
(@AnaEmp, 'ana', 'lloren123', 1, GETDATE()),
(@JuanEmp, 'juan', 'lloren123', 1, GETDATE()),
(@RosaEmp, 'rosa', 'lloren123', 1, GETDATE());

-- =============================================
-- STEP 3: SET 30% COMMISSION ON ALL SERVICES
-- =============================================

-- Assign ALL services to each therapist with 30% commission
INSERT INTO EmployeeServiceCommission (employee_id, service_id, commission_type, commission_value, effective_from, is_archived)
SELECT e.employee_id, s.service_id, 'percent', 30.00, GETDATE(), 0
FROM Employee e
CROSS JOIN Service s
WHERE e.employee_id IN (@MariaEmp, @JoseEmp, @AnaEmp, @JuanEmp, @RosaEmp)
AND s.active = 1;

-- =============================================
-- VERIFICATION
-- =============================================
SELECT '===== 5 THERAPISTS CREATED =====' AS [Result];
SELECT 
    e.employee_id AS [ID],
    p.first_name + ' ' + p.last_name AS [Name],
    r.name AS [Role],
    u.username AS [Username],
    'lloren123' AS [Password]
FROM Employee e
JOIN Person p ON e.person_id = p.person_id
JOIN Role r ON e.role_id = r.role_id
JOIN UserAccount u ON e.employee_id = u.employee_id
WHERE u.username IN ('maria', 'jose', 'ana', 'juan', 'rosa');

SELECT '===== 30% COMMISSION ON ALL SERVICES =====' AS [Result];
SELECT 
    p.first_name AS [Therapist],
    COUNT(esc.service_id) AS [Total Services],
    CAST(esc.commission_value AS VARCHAR) + '%' AS [Commission]
FROM Employee e
JOIN Person p ON e.person_id = p.person_id
JOIN EmployeeServiceCommission esc ON e.employee_id = esc.employee_id
WHERE esc.is_archived = 0
GROUP BY e.employee_id, p.first_name, esc.commission_value;
