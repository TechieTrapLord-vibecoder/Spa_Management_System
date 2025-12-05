-- ============================================
-- HISTORICAL DATA SEED SCRIPT
-- Generates realistic data from Jan 1, 2025 to Dec 4, 2025
-- ============================================

SET NOCOUNT ON;
PRINT 'Starting Historical Data Seed...';
PRINT '';

-- ============================================
-- STEP 1: Clear existing transactional data
-- (Keep: Person, Employee, UserAccount, Role, Service, ServiceCategory, Product, LedgerAccount)
-- ============================================
PRINT 'Clearing existing transactional data...';

-- Disable constraints temporarily
ALTER TABLE Payment NOCHECK CONSTRAINT ALL;
ALTER TABLE SaleItem NOCHECK CONSTRAINT ALL;
ALTER TABLE Sale NOCHECK CONSTRAINT ALL;
ALTER TABLE AppointmentService NOCHECK CONSTRAINT ALL;
ALTER TABLE Appointment NOCHECK CONSTRAINT ALL;
ALTER TABLE JournalEntryLine NOCHECK CONSTRAINT ALL;
ALTER TABLE JournalEntry NOCHECK CONSTRAINT ALL;
ALTER TABLE Expense NOCHECK CONSTRAINT ALL;
ALTER TABLE Inventory NOCHECK CONSTRAINT ALL;
ALTER TABLE Customer NOCHECK CONSTRAINT ALL;

DELETE FROM Payment;
DELETE FROM SaleItem;
DELETE FROM Sale;
DELETE FROM AppointmentService;
DELETE FROM Appointment;
DELETE FROM JournalEntryLine;
DELETE FROM JournalEntry;
DELETE FROM Expense;
DELETE FROM Inventory;
DELETE FROM Customer;

-- Re-enable constraints
ALTER TABLE Customer CHECK CONSTRAINT ALL;
ALTER TABLE Inventory CHECK CONSTRAINT ALL;
ALTER TABLE Expense CHECK CONSTRAINT ALL;
ALTER TABLE JournalEntry CHECK CONSTRAINT ALL;
ALTER TABLE JournalEntryLine CHECK CONSTRAINT ALL;
ALTER TABLE Appointment CHECK CONSTRAINT ALL;
ALTER TABLE AppointmentService CHECK CONSTRAINT ALL;
ALTER TABLE Sale CHECK CONSTRAINT ALL;
ALTER TABLE SaleItem CHECK CONSTRAINT ALL;
ALTER TABLE Payment CHECK CONSTRAINT ALL;

-- Reset identity columns
DBCC CHECKIDENT ('Customer', RESEED, 0);
DBCC CHECKIDENT ('Appointment', RESEED, 0);
DBCC CHECKIDENT ('AppointmentService', RESEED, 0);
DBCC CHECKIDENT ('Sale', RESEED, 0);
DBCC CHECKIDENT ('SaleItem', RESEED, 0);
DBCC CHECKIDENT ('Payment', RESEED, 0);
DBCC CHECKIDENT ('Expense', RESEED, 0);
DBCC CHECKIDENT ('JournalEntry', RESEED, 0);
DBCC CHECKIDENT ('JournalEntryLine', RESEED, 0);
DBCC CHECKIDENT ('Inventory', RESEED, 0);

PRINT 'Transactional data cleared.';
PRINT '';

-- ============================================
-- STEP 2: Create Persons for Customers
-- ============================================
PRINT 'Creating customer persons...';

-- Get max person_id
DECLARE @StartPersonId INT;
SELECT @StartPersonId = ISNULL(MAX(person_id), 0) + 1 FROM Person;

-- Insert 80 customer persons
SET IDENTITY_INSERT Person ON;

-- Customers with varied join dates
INSERT INTO Person (person_id, first_name, last_name, email, phone, address, dob, created_at, sync_status)
VALUES
(@StartPersonId + 0, 'Ana', 'Santos', 'ana.santos@email.com', '0917-123-0001', '123 Rizal St, Manila', '1985-03-15', '2025-01-02 09:30:00', 'pending'),
(@StartPersonId + 1, 'Bella', 'Reyes', 'bella.reyes@email.com', '0917-123-0002', '456 Mabini St, Makati', '1990-07-22', '2025-01-03 10:00:00', 'pending'),
(@StartPersonId + 2, 'Carmen', 'Cruz', 'carmen.cruz@email.com', '0917-123-0003', '789 Luna St, Quezon City', '1988-11-08', '2025-01-05 14:30:00', 'pending'),
(@StartPersonId + 3, 'Diana', 'Garcia', 'diana.garcia@email.com', '0917-123-0004', '321 Bonifacio Ave, Pasig', '1995-02-14', '2025-01-07 11:15:00', 'pending'),
(@StartPersonId + 4, 'Elena', 'Mendoza', 'elena.mendoza@email.com', '0917-123-0005', '654 Aguinaldo St, Taguig', '1982-09-30', '2025-01-10 16:00:00', 'pending'),
(@StartPersonId + 5, 'Fiona', 'Villanueva', 'fiona.villa@email.com', '0917-123-0006', '987 Del Pilar St, Paranaque', '1992-05-18', '2025-01-12 09:45:00', 'pending'),
(@StartPersonId + 6, 'Grace', 'Fernandez', 'grace.fern@email.com', '0917-123-0007', '147 Quezon Ave, Manila', '1987-12-25', '2025-01-15 13:20:00', 'pending'),
(@StartPersonId + 7, 'Hannah', 'Ramos', 'hannah.ramos@email.com', '0917-123-0008', '258 Roxas Blvd, Pasay', '1993-04-07', '2025-01-18 10:30:00', 'pending'),
(@StartPersonId + 8, 'Isabel', 'Torres', 'isabel.torres@email.com', '0917-123-0009', '369 Taft Ave, Manila', '1989-08-19', '2025-01-20 15:45:00', 'pending'),
(@StartPersonId + 9, 'Julia', 'Lim', 'julia.lim@email.com', '0917-123-0010', '741 EDSA, Mandaluyong', '1991-01-03', '2025-01-22 11:00:00', 'pending'),
(@StartPersonId + 10, 'Karen', 'Tan', 'karen.tan@email.com', '0917-123-0011', '852 Shaw Blvd, Pasig', '1986-06-28', '2025-01-25 14:15:00', 'pending'),
(@StartPersonId + 11, 'Laura', 'Gomez', 'laura.gomez@email.com', '0917-123-0012', '963 Ortigas Ave, Pasig', '1994-10-12', '2025-01-28 09:30:00', 'pending'),
(@StartPersonId + 12, 'Maria', 'Lopez', 'maria.lopez@email.com', '0917-123-0013', '159 Ayala Ave, Makati', '1983-03-24', '2025-02-01 10:45:00', 'pending'),
(@StartPersonId + 13, 'Nina', 'Aquino', 'nina.aquino@email.com', '0917-123-0014', '357 Jupiter St, Makati', '1996-07-16', '2025-02-03 13:00:00', 'pending'),
(@StartPersonId + 14, 'Olivia', 'Bautista', 'olivia.bau@email.com', '0917-123-0015', '468 Pasong Tamo, Makati', '1988-11-29', '2025-02-06 15:30:00', 'pending'),
(@StartPersonId + 15, 'Patricia', 'Castillo', 'patricia.cast@email.com', '0917-123-0016', '579 Kalayaan Ave, Makati', '1990-04-05', '2025-02-08 11:15:00', 'pending'),
(@StartPersonId + 16, 'Queenie', 'Dela Cruz', 'queenie.dc@email.com', '0917-123-0017', '681 Pedro Gil, Manila', '1985-08-21', '2025-02-10 09:00:00', 'pending'),
(@StartPersonId + 17, 'Rosa', 'Estrada', 'rosa.estrada@email.com', '0917-123-0018', '792 Espana Blvd, Manila', '1992-12-14', '2025-02-12 14:45:00', 'pending'),
(@StartPersonId + 18, 'Sofia', 'Francisco', 'sofia.fran@email.com', '0917-123-0019', '813 Welcome Rotonda, QC', '1987-02-08', '2025-02-15 10:30:00', 'pending'),
(@StartPersonId + 19, 'Teresa', 'Gonzales', 'teresa.gonz@email.com', '0917-123-0020', '924 Visayas Ave, QC', '1993-06-19', '2025-02-18 13:15:00', 'pending'),
(@StartPersonId + 20, 'Ursula', 'Hernandez', 'ursula.hern@email.com', '0917-123-0021', '135 Commonwealth, QC', '1989-10-02', '2025-02-20 16:00:00', 'pending'),
(@StartPersonId + 21, 'Vera', 'Ignacio', 'vera.ignacio@email.com', '0917-123-0022', '246 Fairview, QC', '1991-01-25', '2025-02-22 11:30:00', 'pending'),
(@StartPersonId + 22, 'Wendy', 'Jurado', 'wendy.jurado@email.com', '0917-123-0023', '357 Novaliches, QC', '1986-05-11', '2025-02-25 09:45:00', 'pending'),
(@StartPersonId + 23, 'Ximena', 'Kapunan', 'ximena.kap@email.com', '0917-123-0024', '468 Cubao, QC', '1994-09-28', '2025-02-28 14:00:00', 'pending'),
(@StartPersonId + 24, 'Yolanda', 'Laurel', 'yolanda.lau@email.com', '0917-123-0025', '579 Araneta Center, QC', '1988-03-17', '2025-03-01 10:15:00', 'pending'),
(@StartPersonId + 25, 'Zenaida', 'Manalo', 'zenaida.man@email.com', '0917-123-0026', '681 Gateway, QC', '1990-07-04', '2025-03-03 13:30:00', 'pending'),
(@StartPersonId + 26, 'Andrea', 'Navarro', 'andrea.nav@email.com', '0917-123-0027', '792 Trinoma, QC', '1985-11-22', '2025-03-05 15:45:00', 'pending'),
(@StartPersonId + 27, 'Bianca', 'Ocampo', 'bianca.oca@email.com', '0917-123-0028', '813 SM North, QC', '1992-02-15', '2025-03-08 11:00:00', 'pending'),
(@StartPersonId + 28, 'Carla', 'Padilla', 'carla.padilla@email.com', '0917-123-0029', '924 Eastwood, QC', '1987-06-30', '2025-03-10 09:30:00', 'pending'),
(@StartPersonId + 29, 'Delia', 'Quizon', 'delia.quizon@email.com', '0917-123-0030', '135 Libis, QC', '1993-10-18', '2025-03-12 14:15:00', 'pending'),
(@StartPersonId + 30, 'Emma', 'Rivera', 'emma.rivera@email.com', '0917-123-0031', '246 C5, Taguig', '1989-01-09', '2025-03-15 10:45:00', 'pending'),
(@StartPersonId + 31, 'Faith', 'Salvador', 'faith.salva@email.com', '0917-123-0032', '357 BGC, Taguig', '1991-05-26', '2025-03-18 13:00:00', 'pending'),
(@StartPersonId + 32, 'Gemma', 'Tolentino', 'gemma.tol@email.com', '0917-123-0033', '468 Uptown, Taguig', '1986-09-13', '2025-03-20 15:30:00', 'pending'),
(@StartPersonId + 33, 'Helen', 'Uy', 'helen.uy@email.com', '0917-123-0034', '579 Market Market, Taguig', '1994-12-07', '2025-03-22 11:15:00', 'pending'),
(@StartPersonId + 34, 'Irene', 'Velasco', 'irene.vela@email.com', '0917-123-0035', '681 High Street, Taguig', '1988-04-24', '2025-03-25 09:00:00', 'pending'),
(@StartPersonId + 35, 'Jackie', 'Wong', 'jackie.wong@email.com', '0917-123-0036', '792 Venice, Taguig', '1990-08-11', '2025-03-28 14:45:00', 'pending'),
(@StartPersonId + 36, 'Kate', 'Yap', 'kate.yap@email.com', '0917-123-0037', '813 Serendra, Taguig', '1985-12-28', '2025-03-30 10:30:00', 'pending'),
(@StartPersonId + 37, 'Liza', 'Zamora', 'liza.zamora@email.com', '0917-123-0038', '924 One Bonifacio, Taguig', '1992-03-15', '2025-04-01 13:15:00', 'pending'),
(@StartPersonId + 38, 'Mila', 'Abella', 'mila.abella@email.com', '0917-123-0039', '135 Festival Mall, Alabang', '1987-07-02', '2025-04-03 16:00:00', 'pending'),
(@StartPersonId + 39, 'Nora', 'Buenaventura', 'nora.buena@email.com', '0917-123-0040', '246 Alabang Hills, Muntinlupa', '1993-11-19', '2025-04-05 11:30:00', 'pending'),
(@StartPersonId + 40, 'Ofelia', 'Corpuz', 'ofelia.corp@email.com', '0917-123-0041', '357 Filinvest, Alabang', '1989-02-06', '2025-04-08 09:45:00', 'pending'),
(@StartPersonId + 41, 'Pilar', 'Dizon', 'pilar.dizon@email.com', '0917-123-0042', '468 Madrigal, Alabang', '1991-06-23', '2025-04-10 14:00:00', 'pending'),
(@StartPersonId + 42, 'Queen', 'Enriquez', 'queen.enri@email.com', '0917-123-0043', '579 Portofino, Alabang', '1986-10-10', '2025-04-12 10:15:00', 'pending'),
(@StartPersonId + 43, 'Rachel', 'Fajardo', 'rachel.faj@email.com', '0917-123-0044', '681 Ayala Alabang, Muntinlupa', '1994-01-27', '2025-04-15 13:30:00', 'pending'),
(@StartPersonId + 44, 'Sarah', 'Gutierrez', 'sarah.guti@email.com', '0917-123-0045', '792 Molito, Alabang', '1988-05-14', '2025-04-18 15:45:00', 'pending'),
(@StartPersonId + 45, 'Tanya', 'Hilario', 'tanya.hila@email.com', '0917-123-0046', '813 Evia, Las Pinas', '1990-09-01', '2025-04-20 11:00:00', 'pending'),
(@StartPersonId + 46, 'Una', 'Ilagan', 'una.ilagan@email.com', '0917-123-0047', '924 SM Southmall, Las Pinas', '1985-12-18', '2025-04-22 09:30:00', 'pending'),
(@StartPersonId + 47, 'Vicky', 'Javier', 'vicky.javier@email.com', '0917-123-0048', '135 Pamplona, Las Pinas', '1992-04-05', '2025-04-25 14:15:00', 'pending'),
(@StartPersonId + 48, 'Wilma', 'Katigbak', 'wilma.kati@email.com', '0917-123-0049', '246 BF Homes, Paranaque', '1987-08-22', '2025-04-28 10:45:00', 'pending'),
(@StartPersonId + 49, 'Xenia', 'Lacson', 'xenia.lacs@email.com', '0917-123-0050', '357 Sucat, Paranaque', '1993-12-09', '2025-05-01 13:00:00', 'pending'),
(@StartPersonId + 50, 'Yvette', 'Magat', 'yvette.magat@email.com', '0917-123-0051', '468 SM BF, Paranaque', '1989-03-26', '2025-05-03 15:30:00', 'pending'),
(@StartPersonId + 51, 'Zelda', 'Natividad', 'zelda.nati@email.com', '0917-123-0052', '579 Ayala Malls, Paranaque', '1991-07-13', '2025-05-05 11:15:00', 'pending'),
(@StartPersonId + 52, 'Alicia', 'Ortega', 'alicia.orte@email.com', '0917-123-0053', '681 Aseana, Paranaque', '1986-11-30', '2025-05-08 09:00:00', 'pending'),
(@StartPersonId + 53, 'Betty', 'Pascual', 'betty.pasc@email.com', '0917-123-0054', '792 City of Dreams, Paranaque', '1994-02-17', '2025-05-10 14:45:00', 'pending'),
(@StartPersonId + 54, 'Cynthia', 'Quirino', 'cynthia.qui@email.com', '0917-123-0055', '813 NAIA, Pasay', '1988-06-04', '2025-05-12 10:30:00', 'pending'),
(@StartPersonId + 55, 'Dorothy', 'Rosario', 'dorothy.ros@email.com', '0917-123-0056', '924 MOA, Pasay', '1990-10-21', '2025-05-15 13:15:00', 'pending'),
(@StartPersonId + 56, 'Evelyn', 'Salazar', 'evelyn.sala@email.com', '0917-123-0057', '135 SMX, Pasay', '1985-01-08', '2025-05-18 16:00:00', 'pending'),
(@StartPersonId + 57, 'Frances', 'Tinio', 'frances.tin@email.com', '0917-123-0058', '246 PICC, Pasay', '1992-05-25', '2025-05-20 11:30:00', 'pending'),
(@StartPersonId + 58, 'Gloria', 'Umali', 'gloria.umal@email.com', '0917-123-0059', '357 CCP, Pasay', '1987-09-12', '2025-05-22 09:45:00', 'pending'),
(@StartPersonId + 59, 'Hazel', 'Valdez', 'hazel.valdez@email.com', '0917-123-0060', '468 Cultural Center, Pasay', '1993-12-29', '2025-05-25 14:00:00', 'pending'),
(@StartPersonId + 60, 'Ingrid', 'Wenceslao', 'ingrid.wen@email.com', '0917-123-0061', '579 Roxas Blvd, Pasay', '1989-04-16', '2025-05-28 10:15:00', 'pending'),
(@StartPersonId + 61, 'Janet', 'Ybarra', 'janet.ybar@email.com', '0917-123-0062', '681 Heritage Hotel, Pasay', '1991-08-03', '2025-06-01 13:30:00', 'pending'),
(@StartPersonId + 62, 'Kelly', 'Zarate', 'kelly.zarate@email.com', '0917-123-0063', '792 Conrad, Pasay', '1986-11-20', '2025-06-03 15:45:00', 'pending'),
(@StartPersonId + 63, 'Lorena', 'Alcantara', 'lorena.alca@email.com', '0917-123-0064', '813 Okada, Paranaque', '1994-03-07', '2025-06-05 11:00:00', 'pending'),
(@StartPersonId + 64, 'Monica', 'Bernardo', 'monica.bern@email.com', '0917-123-0065', '924 Solaire, Paranaque', '1988-07-24', '2025-06-08 09:30:00', 'pending'),
(@StartPersonId + 65, 'Norma', 'Cayetano', 'norma.caye@email.com', '0917-123-0066', '135 Newport, Pasay', '1990-11-11', '2025-06-10 14:15:00', 'pending'),
(@StartPersonId + 66, 'Ophelia', 'Dimagiba', 'ophelia.dim@email.com', '0917-123-0067', '246 Resorts World, Pasay', '1985-02-28', '2025-06-12 10:45:00', 'pending'),
(@StartPersonId + 67, 'Paula', 'Ejercito', 'paula.ejer@email.com', '0917-123-0068', '357 Marriott, Pasay', '1992-06-15', '2025-06-15 13:00:00', 'pending'),
(@StartPersonId + 68, 'Rita', 'Faustino', 'rita.faust@email.com', '0917-123-0069', '468 Hilton, Taguig', '1987-10-02', '2025-06-18 15:30:00', 'pending'),
(@StartPersonId + 69, 'Sandra', 'Galang', 'sandra.gala@email.com', '0917-123-0070', '579 Grand Hyatt, Taguig', '1993-01-19', '2025-06-20 11:15:00', 'pending'),
(@StartPersonId + 70, 'Tina', 'Hizon', 'tina.hizon@email.com', '0917-123-0071', '681 Shangri-La, Makati', '1989-05-06', '2025-06-22 09:00:00', 'pending'),
(@StartPersonId + 71, 'Urania', 'Imperial', 'urania.imp@email.com', '0917-123-0072', '792 Peninsula, Makati', '1991-09-23', '2025-06-25 14:45:00', 'pending'),
(@StartPersonId + 72, 'Vivian', 'Jacinto', 'vivian.jac@email.com', '0917-123-0073', '813 Raffles, Makati', '1986-12-10', '2025-06-28 10:30:00', 'pending'),
(@StartPersonId + 73, 'Wanda', 'Kabigting', 'wanda.kabi@email.com', '0917-123-0074', '924 Fairmont, Makati', '1994-04-27', '2025-07-01 13:15:00', 'pending'),
(@StartPersonId + 74, 'Xyza', 'Lagman', 'xyza.lagman@email.com', '0917-123-0075', '135 Intercontinental, Makati', '1988-08-14', '2025-07-03 16:00:00', 'pending'),
(@StartPersonId + 75, 'Ysobel', 'Macapagal', 'ysobel.mac@email.com', '0917-123-0076', '246 Dusit Thani, Makati', '1990-12-01', '2025-07-05 11:30:00', 'pending'),
(@StartPersonId + 76, 'Zara', 'Narvaez', 'zara.narv@email.com', '0917-123-0077', '357 New World, Makati', '1985-03-20', '2025-07-08 09:45:00', 'pending'),
(@StartPersonId + 77, 'April', 'Olegario', 'april.oleg@email.com', '0917-123-0078', '468 Ascott, Makati', '1992-07-07', '2025-07-10 14:00:00', 'pending'),
(@StartPersonId + 78, 'Beverly', 'Panganiban', 'beverly.pan@email.com', '0917-123-0079', '579 Discovery, Pasig', '1987-11-24', '2025-07-12 10:15:00', 'pending'),
(@StartPersonId + 79, 'Cristina', 'Quintos', 'cristina.qui@email.com', '0917-123-0080', '681 Edades, Makati', '1993-02-11', '2025-07-15 13:30:00', 'pending');

SET IDENTITY_INSERT Person OFF;

PRINT 'Created 80 customer persons.';
PRINT '';

-- ============================================
-- STEP 3: Create Customer records
-- ============================================
PRINT 'Creating customer records...';

INSERT INTO Customer (person_id, customer_code, loyalty_points, created_at, is_archived, sync_status)
SELECT person_id, 
       'CUST-' + RIGHT('0000' + CAST(ROW_NUMBER() OVER (ORDER BY person_id) AS VARCHAR), 4),
       ABS(CHECKSUM(NEWID())) % 500,
       created_at,
       0,
       'pending'
FROM Person
WHERE person_id >= @StartPersonId;

PRINT 'Created customer records.';
PRINT '';

-- ============================================
-- STEP 4: Generate Appointments (Jan 1 - Dec 4, 2025)
-- ~3-5 appointments per day
-- ============================================
PRINT 'Generating appointments...';

DECLARE @CurrentDate DATE = '2025-01-01';
DECLARE @EndDate DATE = '2025-12-04';
DECLARE @TherapistId INT = 15;  -- Maria Kulot
DECLARE @ReceptionistUserId INT = 12;  -- Receptionist user
DECLARE @AccountantUserId INT = 14;  -- Accountant user

-- Create temp table for services with prices
CREATE TABLE #Services (service_id INT, price DECIMAL(18,2), commission DECIMAL(18,2));
INSERT INTO #Services (service_id, price, commission)
SELECT service_id, price, 
    CASE 
        WHEN commission_type = 'Percentage' THEN commission_value * price / 100
        ELSE commission_value
    END
FROM [Service];

-- Create temp table for customers
CREATE TABLE #Customers (customer_id INT, person_id INT, created_at DATETIME);
INSERT INTO #Customers SELECT customer_id, person_id, created_at FROM Customer;

-- Generate appointments using a numbers table approach
;WITH Numbers AS (
    SELECT TOP 1500 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.objects a CROSS JOIN sys.objects b
),
Dates AS (
    SELECT DATEADD(DAY, n-1, '2025-01-01') AS appt_date
    FROM Numbers
    WHERE DATEADD(DAY, n-1, '2025-01-01') <= '2025-12-04'
),
ApptSlots AS (
    SELECT 
        d.appt_date,
        s.slot_num,
        CASE s.slot_num 
            WHEN 1 THEN '09:00'
            WHEN 2 THEN '10:30'
            WHEN 3 THEN '13:00'
            WHEN 4 THEN '14:30'
            WHEN 5 THEN '16:00'
        END AS time_slot
    FROM Dates d
    CROSS JOIN (SELECT 1 AS slot_num UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5) s
    WHERE ABS(CHECKSUM(NEWID()) % 100) < 75  -- 75% chance each slot is used
)
INSERT INTO Appointment (customer_id, scheduled_start, scheduled_end, status, created_by_user_id, created_at, notes, sync_status)
SELECT 
    (SELECT TOP 1 customer_id FROM #Customers WHERE created_at <= a.appt_date ORDER BY NEWID()),
    CAST(a.appt_date AS DATETIME) + CAST(a.time_slot AS DATETIME),
    DATEADD(HOUR, 1, CAST(a.appt_date AS DATETIME) + CAST(a.time_slot AS DATETIME)),
    'Completed',
    @ReceptionistUserId,
    DATEADD(DAY, -ABS(CHECKSUM(NEWID()) % 3), CAST(a.appt_date AS DATETIME) + CAST(a.time_slot AS DATETIME)),
    NULL,
    'pending'
FROM ApptSlots a
WHERE (SELECT COUNT(*) FROM #Customers WHERE created_at <= a.appt_date) > 0;

PRINT 'Created appointments.';
PRINT '';

-- ============================================
-- STEP 5: Create AppointmentService records
-- ============================================
PRINT 'Creating appointment services...';

-- Create numbered services for proper randomization
CREATE TABLE #NumberedServices (row_num INT, service_id INT, price DECIMAL(18,2), commission DECIMAL(18,2));
INSERT INTO #NumberedServices
SELECT ROW_NUMBER() OVER (ORDER BY service_id) as row_num, service_id, price, commission
FROM #Services;

DECLARE @ServiceCount INT;
SELECT @ServiceCount = COUNT(*) FROM #NumberedServices;

-- Use modulo to distribute services across appointments
INSERT INTO AppointmentService (appointment_id, service_id, therapist_employee_id, price, commission_amount, sync_status)
SELECT 
    a.appointment_id,
    s.service_id,
    @TherapistId,
    s.price,
    s.commission,
    'pending'
FROM Appointment a
JOIN #NumberedServices s ON s.row_num = ((a.appointment_id - 1) % @ServiceCount) + 1;

PRINT 'Created appointment services.';
PRINT '';

-- ============================================
-- STEP 6: Create Sales from Appointments
-- ============================================
PRINT 'Creating sales records...';

DECLARE @SaleCounter INT = 0;

INSERT INTO Sale (customer_id, created_by_user_id, sale_number, subtotal, tax_rate, tax_amount, total_amount, payment_status, created_at, sync_status)
SELECT 
    a.customer_id,
    @ReceptionistUserId,
    'SALE-' + FORMAT(a.scheduled_start, 'yyyyMMdd') + '-' + RIGHT('0000' + CAST(ROW_NUMBER() OVER (ORDER BY a.appointment_id) AS VARCHAR), 4),
    aps.price,
    12.00,
    aps.price * 0.12,
    aps.price * 1.12,
    'paid',
    a.scheduled_end,
    'pending'
FROM Appointment a
JOIN AppointmentService aps ON a.appointment_id = aps.appointment_id;

PRINT 'Created sales.';
PRINT '';

-- ============================================
-- STEP 7: Create SaleItems
-- ============================================
PRINT 'Creating sale items...';

INSERT INTO SaleItem (sale_id, item_type, product_id, service_id, qty, unit_price, line_total, therapist_employee_id, sync_status)
SELECT 
    s.sale_id,
    'Service',
    NULL,
    aps.service_id,
    1,
    aps.price,
    aps.price,
    @TherapistId,
    'pending'
FROM Sale s
JOIN Appointment a ON s.created_at = a.scheduled_end AND s.customer_id = a.customer_id
JOIN AppointmentService aps ON a.appointment_id = aps.appointment_id;

PRINT 'Created sale items.';
PRINT '';

-- ============================================
-- STEP 8: Create Payments
-- ============================================
PRINT 'Creating payments...';

INSERT INTO Payment (sale_id, payment_method, amount, paid_at, recorded_by_user_id, sync_status)
SELECT 
    sale_id,
    CASE ABS(CHECKSUM(NEWID())) % 3
        WHEN 0 THEN 'cash'
        WHEN 1 THEN 'card'
        ELSE 'voucher'
    END,
    total_amount,
    created_at,
    @ReceptionistUserId,
    'pending'
FROM Sale;

PRINT 'Created payments.';
PRINT '';

-- ============================================
-- STEP 9: Create Monthly Expenses (Rent, Utilities, Supplies)
-- ============================================
PRINT 'Creating monthly expenses...';

-- Monthly Rent (30,000 per month, paid 1st of each month)
DECLARE @Month INT = 1;
WHILE @Month <= 12
BEGIN
    IF DATEFROMPARTS(2025, @Month, 1) <= '2025-12-04'
    BEGIN
        INSERT INTO Expense (expense_date, category, description, amount, vendor, payment_method, status, ledger_account_id, notes, created_by_user_id, created_at, sync_status)
        VALUES (
            DATEFROMPARTS(2025, @Month, 1),
            'Rent',
            'Monthly Office Rent - ' + DATENAME(MONTH, DATEFROMPARTS(2025, @Month, 1)) + ' 2025',
            30000.00,
            'ABC Realty Corp',
            'Bank Transfer',
            'Paid',
            10047,
            'Office space rental',
            @AccountantUserId,
            DATEADD(HOUR, 9, CAST(DATEFROMPARTS(2025, @Month, 1) AS DATETIME)),
            'pending'
        );
        
        -- Utilities (5000-8000)
        INSERT INTO Expense (expense_date, category, description, amount, vendor, payment_method, status, ledger_account_id, notes, created_by_user_id, created_at, sync_status)
        VALUES (
            DATEADD(DAY, 14, DATEFROMPARTS(2025, @Month, 1)),
            'Utilities',
            'Electricity & Water - ' + DATENAME(MONTH, DATEFROMPARTS(2025, @Month, 1)) + ' 2025',
            5000 + ABS(CHECKSUM(NEWID())) % 3000,
            'Meralco / Manila Water',
            'Bank Transfer',
            'Paid',
            10048,
            'Monthly utilities',
            @AccountantUserId,
            DATEADD(HOUR, 10, DATEADD(DAY, 14, CAST(DATEFROMPARTS(2025, @Month, 1) AS DATETIME))),
            'pending'
        );
        
        -- Supplies (2000-4000)
        INSERT INTO Expense (expense_date, category, description, amount, vendor, payment_method, status, ledger_account_id, notes, created_by_user_id, created_at, sync_status)
        VALUES (
            DATEADD(DAY, 7, DATEFROMPARTS(2025, @Month, 1)),
            'Supplies',
            'Spa Supplies & Consumables - ' + DATENAME(MONTH, DATEFROMPARTS(2025, @Month, 1)) + ' 2025',
            2000 + ABS(CHECKSUM(NEWID())) % 2000,
            'Spa Supplies Inc.',
            'Cash',
            'Paid',
            10049,
            'Towels, oils, consumables',
            @AccountantUserId,
            DATEADD(HOUR, 11, DATEADD(DAY, 7, CAST(DATEFROMPARTS(2025, @Month, 1) AS DATETIME))),
            'pending'
        );
    END
    SET @Month = @Month + 1;
END;

-- Add some random miscellaneous expenses
INSERT INTO Expense (expense_date, category, description, amount, vendor, payment_method, status, ledger_account_id, notes, created_by_user_id, created_at, sync_status)
VALUES 
('2025-01-15', 'Maintenance', 'Air Conditioning Repair', 3500.00, 'Cool Tech Services', 'Cash', 'Paid', 10055, 'AC unit repair', @AccountantUserId, '2025-01-15 14:00:00', 'pending'),
('2025-02-20', 'Maintenance', 'Plumbing Repair', 2000.00, 'Fix-It Plumbing', 'Cash', 'Paid', 10055, 'Bathroom fixtures', @AccountantUserId, '2025-02-20 10:30:00', 'pending'),
('2025-03-10', 'Equipment', 'New Massage Table', 15000.00, 'Spa Equipment PH', 'Bank Transfer', 'Paid', 10058, 'Replacement table', @AccountantUserId, '2025-03-10 09:00:00', 'pending'),
('2025-04-25', 'Maintenance', 'Electrical Repair', 4500.00, 'Sparky Electrical', 'Cash', 'Paid', 10055, 'Wiring repair', @AccountantUserId, '2025-04-25 11:00:00', 'pending'),
('2025-05-08', 'Professional Services', 'Accounting Services Q1', 8000.00, 'ABC Accounting', 'Bank Transfer', 'Paid', 10059, 'Quarterly bookkeeping', @AccountantUserId, '2025-05-08 09:30:00', 'pending'),
('2025-06-12', 'Equipment', 'Foot Spa Basin Set', 6500.00, 'Spa Equipment PH', 'Card', 'Paid', 10058, '2 units', @AccountantUserId, '2025-06-12 14:15:00', 'pending'),
('2025-07-18', 'Maintenance', 'HVAC Cleaning', 2500.00, 'Cool Tech Services', 'Cash', 'Paid', 10055, 'Regular maintenance', @AccountantUserId, '2025-07-18 10:00:00', 'pending'),
('2025-08-05', 'Professional Services', 'Accounting Services Q2', 8000.00, 'ABC Accounting', 'Bank Transfer', 'Paid', 10059, 'Quarterly bookkeeping', @AccountantUserId, '2025-08-05 09:30:00', 'pending'),
('2025-09-22', 'Insurance', 'Business Insurance Renewal', 25000.00, 'Insular Life', 'Bank Transfer', 'Paid', 10056, 'Annual premium', @AccountantUserId, '2025-09-22 11:00:00', 'pending'),
('2025-10-15', 'Taxes', 'Quarterly Tax Payment', 12000.00, 'BIR', 'Bank Transfer', 'Paid', 10060, 'Q3 taxes', @AccountantUserId, '2025-10-15 09:00:00', 'pending'),
('2025-11-07', 'Professional Services', 'Accounting Services Q3', 8000.00, 'ABC Accounting', 'Bank Transfer', 'Paid', 10059, 'Quarterly bookkeeping', @AccountantUserId, '2025-11-07 09:30:00', 'pending'),
('2025-11-28', 'Miscellaneous', 'Christmas Decorations', 3000.00, 'Party Plus', 'Cash', 'Paid', 10061, 'Holiday decor', @AccountantUserId, '2025-11-28 15:00:00', 'pending');

PRINT 'Created expenses.';
PRINT '';

-- ============================================
-- STEP 10: Initialize Inventory
-- ============================================
PRINT 'Initializing inventory...';

INSERT INTO Inventory (product_id, quantity_on_hand, reorder_level, last_counted_at, sync_status)
SELECT 
    product_id,
    20 + ABS(CHECKSUM(NEWID())) % 80,  -- 20-100 units
    10,
    GETDATE(),
    'pending'
FROM Product
WHERE active = 1;

PRINT 'Initialized inventory.';
PRINT '';

-- Cleanup temp tables
DROP TABLE #Services;
DROP TABLE #NumberedServices;
DROP TABLE #Customers;

-- ============================================
-- SUMMARY
-- ============================================
DECLARE @CustomerCount INT, @ApptCount INT, @SaleCount INT, @PaymentCount INT, @ExpenseCount INT;
DECLARE @TotalRevenue DECIMAL(18,2), @TotalExpenses DECIMAL(18,2);

SELECT @CustomerCount = COUNT(*) FROM Customer;
SELECT @ApptCount = COUNT(*) FROM Appointment;
SELECT @SaleCount = COUNT(*) FROM Sale;
SELECT @PaymentCount = COUNT(*) FROM Payment;
SELECT @ExpenseCount = COUNT(*) FROM Expense;
SELECT @TotalRevenue = ISNULL(SUM(total_amount), 0) FROM Sale;
SELECT @TotalExpenses = ISNULL(SUM(amount), 0) FROM Expense;

PRINT '';
PRINT '========================================';
PRINT 'SEED COMPLETE - SUMMARY';
PRINT '========================================';
PRINT 'Customers: ' + CAST(@CustomerCount AS VARCHAR);
PRINT 'Appointments: ' + CAST(@ApptCount AS VARCHAR);
PRINT 'Sales: ' + CAST(@SaleCount AS VARCHAR);
PRINT 'Payments: ' + CAST(@PaymentCount AS VARCHAR);
PRINT 'Expenses: ' + CAST(@ExpenseCount AS VARCHAR);
PRINT '';
PRINT 'Total Revenue: ' + CAST(@TotalRevenue AS VARCHAR);
PRINT 'Total Expenses: ' + CAST(@TotalExpenses AS VARCHAR);
PRINT '========================================';
