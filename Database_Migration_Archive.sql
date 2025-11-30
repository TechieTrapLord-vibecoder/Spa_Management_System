/* =============================================
   SPA MANAGEMENT SYSTEM - ARCHIVE MIGRATION
   Add IsArchived columns to support soft delete
   
   IMPORTANT: Run this script on the spa_erp database
   =============================================
*/

USE spa_erp;
GO

-- Add is_archived column to Supplier table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Supplier')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Supplier') AND name = 'is_archived')
    BEGIN
        ALTER TABLE Supplier ADD is_archived BIT NOT NULL DEFAULT 0;
        PRINT 'Added is_archived column to Supplier table';
    END
    ELSE
        PRINT 'is_archived column already exists in Supplier table';
END
ELSE
    PRINT 'Supplier table does not exist - skipping';
GO

-- Add is_archived column to Role table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Role')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Role') AND name = 'is_archived')
    BEGIN
        ALTER TABLE Role ADD is_archived BIT NOT NULL DEFAULT 0;
        PRINT 'Added is_archived column to Role table';
    END
    ELSE
        PRINT 'is_archived column already exists in Role table';
END
ELSE
    PRINT 'Role table does not exist - skipping';
GO

-- Add is_archived column to ServiceCategory table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ServiceCategory')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'ServiceCategory') AND name = 'is_archived')
    BEGIN
        ALTER TABLE ServiceCategory ADD is_archived BIT NOT NULL DEFAULT 0;
        PRINT 'Added is_archived column to ServiceCategory table';
    END
    ELSE
        PRINT 'is_archived column already exists in ServiceCategory table';
END
ELSE
    PRINT 'ServiceCategory table does not exist - skipping';
GO

-- Add is_archived column to EmployeeServiceCommission table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeServiceCommission')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'EmployeeServiceCommission') AND name = 'is_archived')
    BEGIN
        ALTER TABLE EmployeeServiceCommission ADD is_archived BIT NOT NULL DEFAULT 0;
        PRINT 'Added is_archived column to EmployeeServiceCommission table';
    END
    ELSE
        PRINT 'is_archived column already exists in EmployeeServiceCommission table';
END
ELSE
    PRINT 'EmployeeServiceCommission table does not exist - skipping';
GO

-- Add is_archived column to Customer table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Customer')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Customer') AND name = 'is_archived')
    BEGIN
        ALTER TABLE Customer ADD is_archived BIT NOT NULL DEFAULT 0;
        PRINT 'Added is_archived column to Customer table';
    END
    ELSE
        PRINT 'is_archived column already exists in Customer table';
END
ELSE
    PRINT 'Customer table does not exist - skipping';
GO

-- Note: Product and Service tables already have 'active' column which serves the same purpose
-- Note: Employee table has 'status' column that can be set to 'archived'
-- Note: UserAccount table has 'is_active' column which serves the same purpose

PRINT 'Archive migration completed!';
GO
