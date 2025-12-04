-- Add sync columns to ServiceCategory, Role, LedgerAccount, UserAccount
-- Run this on your local database

-- ServiceCategory
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('ServiceCategory') AND name = 'sync_id')
BEGIN
    ALTER TABLE ServiceCategory ADD sync_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    ALTER TABLE ServiceCategory ADD last_modified_at DATETIME2 NOT NULL DEFAULT GETDATE();
    ALTER TABLE ServiceCategory ADD last_synced_at DATETIME2 NULL;
    ALTER TABLE ServiceCategory ADD sync_status NVARCHAR(20) NOT NULL DEFAULT 'pending';
    ALTER TABLE ServiceCategory ADD sync_version INT NOT NULL DEFAULT 1;
    PRINT 'Added sync columns to ServiceCategory';
END
ELSE
    PRINT 'ServiceCategory already has sync columns';

-- Role
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Role') AND name = 'sync_id')
BEGIN
    ALTER TABLE [Role] ADD sync_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    ALTER TABLE [Role] ADD last_modified_at DATETIME2 NOT NULL DEFAULT GETDATE();
    ALTER TABLE [Role] ADD last_synced_at DATETIME2 NULL;
    ALTER TABLE [Role] ADD sync_status NVARCHAR(20) NOT NULL DEFAULT 'pending';
    ALTER TABLE [Role] ADD sync_version INT NOT NULL DEFAULT 1;
    PRINT 'Added sync columns to Role';
END
ELSE
    PRINT 'Role already has sync columns';

-- LedgerAccount
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('LedgerAccount') AND name = 'sync_id')
BEGIN
    ALTER TABLE LedgerAccount ADD sync_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    ALTER TABLE LedgerAccount ADD last_modified_at DATETIME2 NOT NULL DEFAULT GETDATE();
    ALTER TABLE LedgerAccount ADD last_synced_at DATETIME2 NULL;
    ALTER TABLE LedgerAccount ADD sync_status NVARCHAR(20) NOT NULL DEFAULT 'pending';
    ALTER TABLE LedgerAccount ADD sync_version INT NOT NULL DEFAULT 1;
    PRINT 'Added sync columns to LedgerAccount';
END
ELSE
    PRINT 'LedgerAccount already has sync columns';

-- UserAccount
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('UserAccount') AND name = 'sync_id')
BEGIN
    ALTER TABLE UserAccount ADD sync_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
    ALTER TABLE UserAccount ADD last_modified_at DATETIME2 NOT NULL DEFAULT GETDATE();
    ALTER TABLE UserAccount ADD last_synced_at DATETIME2 NULL;
    ALTER TABLE UserAccount ADD sync_status NVARCHAR(20) NOT NULL DEFAULT 'pending';
    ALTER TABLE UserAccount ADD sync_version INT NOT NULL DEFAULT 1;
    PRINT 'Added sync columns to UserAccount';
END
ELSE
    PRINT 'UserAccount already has sync columns';

-- Mark all existing records as synced (they're already in cloud from previous syncs)
UPDATE ServiceCategory SET sync_status = 'synced' WHERE sync_status = 'pending';
UPDATE [Role] SET sync_status = 'synced' WHERE sync_status = 'pending';
UPDATE LedgerAccount SET sync_status = 'synced' WHERE sync_status = 'pending';
UPDATE UserAccount SET sync_status = 'synced' WHERE sync_status = 'pending';

PRINT 'Done! All tables now have sync tracking columns.';
