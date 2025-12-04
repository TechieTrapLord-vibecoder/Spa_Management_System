-- =====================================================
-- Sync Columns Fix Script
-- Run this on BOTH local and cloud databases
-- =====================================================

-- 1. Add sync columns to UserAccount
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('UserAccount') AND name = 'sync_id')
BEGIN
    ALTER TABLE UserAccount ADD sync_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    ALTER TABLE UserAccount ADD last_modified_at DATETIME2 NULL;
    ALTER TABLE UserAccount ADD last_synced_at DATETIME2 NULL;
    ALTER TABLE UserAccount ADD sync_status NVARCHAR(20) NOT NULL DEFAULT 'pending';
    ALTER TABLE UserAccount ADD sync_version INT NOT NULL DEFAULT 1;
    PRINT 'Added sync columns to UserAccount';
END
GO

-- 2. Add sync columns to Role
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Role') AND name = 'sync_id')
BEGIN
    ALTER TABLE Role ADD sync_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    ALTER TABLE Role ADD last_modified_at DATETIME2 NULL;
    ALTER TABLE Role ADD last_synced_at DATETIME2 NULL;
    ALTER TABLE Role ADD sync_status NVARCHAR(20) NOT NULL DEFAULT 'pending';
    ALTER TABLE Role ADD sync_version INT NOT NULL DEFAULT 1;
    PRINT 'Added sync columns to Role';
END
GO

-- 3. Add sync columns to ServiceCategory
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('ServiceCategory') AND name = 'sync_id')
BEGIN
    ALTER TABLE ServiceCategory ADD sync_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    ALTER TABLE ServiceCategory ADD last_modified_at DATETIME2 NULL;
    ALTER TABLE ServiceCategory ADD last_synced_at DATETIME2 NULL;
    ALTER TABLE ServiceCategory ADD sync_status NVARCHAR(20) NOT NULL DEFAULT 'pending';
    ALTER TABLE ServiceCategory ADD sync_version INT NOT NULL DEFAULT 1;
    PRINT 'Added sync columns to ServiceCategory';
END
GO

-- 4. Add sync columns to LedgerAccount
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('LedgerAccount') AND name = 'sync_id')
BEGIN
    ALTER TABLE LedgerAccount ADD sync_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    ALTER TABLE LedgerAccount ADD last_modified_at DATETIME2 NULL;
    ALTER TABLE LedgerAccount ADD last_synced_at DATETIME2 NULL;
    ALTER TABLE LedgerAccount ADD sync_status NVARCHAR(20) NOT NULL DEFAULT 'pending';
    ALTER TABLE LedgerAccount ADD sync_version INT NOT NULL DEFAULT 1;
    PRINT 'Added sync columns to LedgerAccount';
END
GO

-- 5. Add sync columns to EmployeeServiceCommission
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeServiceCommission') AND name = 'sync_id')
BEGIN
    ALTER TABLE EmployeeServiceCommission ADD sync_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    ALTER TABLE EmployeeServiceCommission ADD last_modified_at DATETIME2 NULL;
    ALTER TABLE EmployeeServiceCommission ADD last_synced_at DATETIME2 NULL;
    ALTER TABLE EmployeeServiceCommission ADD sync_status NVARCHAR(20) NOT NULL DEFAULT 'pending';
    ALTER TABLE EmployeeServiceCommission ADD sync_version INT NOT NULL DEFAULT 1;
    PRINT 'Added sync columns to EmployeeServiceCommission';
END
GO

-- 6. Add sync columns to CRM_Note
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CRM_Note') AND name = 'sync_id')
BEGIN
    ALTER TABLE CRM_Note ADD sync_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    ALTER TABLE CRM_Note ADD last_modified_at DATETIME2 NULL;
    ALTER TABLE CRM_Note ADD last_synced_at DATETIME2 NULL;
    ALTER TABLE CRM_Note ADD sync_status NVARCHAR(20) NOT NULL DEFAULT 'pending';
    ALTER TABLE CRM_Note ADD sync_version INT NOT NULL DEFAULT 1;
    PRINT 'Added sync columns to CRM_Note';
END
GO

-- 7. Add sync columns to EmployeeAttendance (if missing)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('EmployeeAttendance') AND name = 'sync_id')
BEGIN
    ALTER TABLE EmployeeAttendance ADD sync_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    ALTER TABLE EmployeeAttendance ADD last_modified_at DATETIME2 NULL;
    ALTER TABLE EmployeeAttendance ADD last_synced_at DATETIME2 NULL;
    ALTER TABLE EmployeeAttendance ADD sync_status NVARCHAR(20) NOT NULL DEFAULT 'pending';
    ALTER TABLE EmployeeAttendance ADD sync_version INT NOT NULL DEFAULT 1;
    PRINT 'Added sync columns to EmployeeAttendance';
END
GO

-- 8. Create SupplierProduct table (if not exists)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SupplierProduct')
BEGIN
    CREATE TABLE SupplierProduct (
        supplier_product_id BIGINT IDENTITY(1,1) PRIMARY KEY,
        sync_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        last_modified_at DATETIME2 NULL,
        last_synced_at DATETIME2 NULL,
        sync_status NVARCHAR(20) NOT NULL DEFAULT 'pending',
        sync_version INT NOT NULL DEFAULT 1,
        supplier_id BIGINT NOT NULL,
        product_id BIGINT NOT NULL,
        supplier_price DECIMAL(12,2) NOT NULL DEFAULT 0,
        supplier_sku NVARCHAR(80) NULL,
        min_order_qty INT NULL,
        lead_time_days INT NULL,
        is_preferred BIT NOT NULL DEFAULT 0,
        is_active BIT NOT NULL DEFAULT 1,
        notes NVARCHAR(MAX) NULL,
        created_at DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_SupplierProduct_Supplier FOREIGN KEY (supplier_id) REFERENCES Supplier(supplier_id) ON DELETE CASCADE,
        CONSTRAINT FK_SupplierProduct_Product FOREIGN KEY (product_id) REFERENCES Product(product_id) ON DELETE CASCADE,
        CONSTRAINT UQ_SupplierProduct_Supplier_Product UNIQUE (supplier_id, product_id)
    );
    PRINT 'Created SupplierProduct table';
END
GO

-- Update existing records to have 'synced' status (for already existing data)
UPDATE UserAccount SET sync_status = 'synced' WHERE sync_status = 'pending';
UPDATE Role SET sync_status = 'synced' WHERE sync_status = 'pending';
UPDATE ServiceCategory SET sync_status = 'synced' WHERE sync_status = 'pending';
UPDATE LedgerAccount SET sync_status = 'synced' WHERE sync_status = 'pending';
UPDATE EmployeeServiceCommission SET sync_status = 'synced' WHERE sync_status = 'pending';
GO

PRINT 'Sync columns fix completed!';
