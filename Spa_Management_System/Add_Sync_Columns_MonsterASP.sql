-- Add sync columns to all tables that need them on MonsterASP
-- Run this script on your MonsterASP database (db32359)

-- Person table
ALTER TABLE Person ADD sync_id UNIQUEIDENTIFIER NULL;
ALTER TABLE Person ADD last_modified_at DATETIME2 NULL;
ALTER TABLE Person ADD last_synced_at DATETIME2 NULL;
ALTER TABLE Person ADD sync_status NVARCHAR(20) NULL DEFAULT 'pending';
ALTER TABLE Person ADD sync_version INT NULL DEFAULT 1;
GO

-- Employee table
ALTER TABLE Employee ADD sync_id UNIQUEIDENTIFIER NULL;
ALTER TABLE Employee ADD last_modified_at DATETIME2 NULL;
ALTER TABLE Employee ADD last_synced_at DATETIME2 NULL;
ALTER TABLE Employee ADD sync_status NVARCHAR(20) NULL DEFAULT 'pending';
ALTER TABLE Employee ADD sync_version INT NULL DEFAULT 1;
GO

-- Customer table
ALTER TABLE Customer ADD sync_id UNIQUEIDENTIFIER NULL;
ALTER TABLE Customer ADD last_modified_at DATETIME2 NULL;
ALTER TABLE Customer ADD last_synced_at DATETIME2 NULL;
ALTER TABLE Customer ADD sync_status NVARCHAR(20) NULL DEFAULT 'pending';
ALTER TABLE Customer ADD sync_version INT NULL DEFAULT 1;
GO

-- Service table
ALTER TABLE Service ADD sync_id UNIQUEIDENTIFIER NULL;
ALTER TABLE Service ADD last_modified_at DATETIME2 NULL;
ALTER TABLE Service ADD last_synced_at DATETIME2 NULL;
ALTER TABLE Service ADD sync_status NVARCHAR(20) NULL DEFAULT 'pending';
ALTER TABLE Service ADD sync_version INT NULL DEFAULT 1;
GO

-- Product table
ALTER TABLE Product ADD sync_id UNIQUEIDENTIFIER NULL;
ALTER TABLE Product ADD last_modified_at DATETIME2 NULL;
ALTER TABLE Product ADD last_synced_at DATETIME2 NULL;
ALTER TABLE Product ADD sync_status NVARCHAR(20) NULL DEFAULT 'pending';
ALTER TABLE Product ADD sync_version INT NULL DEFAULT 1;
GO

-- Inventory table
ALTER TABLE Inventory ADD sync_id UNIQUEIDENTIFIER NULL;
ALTER TABLE Inventory ADD last_modified_at DATETIME2 NULL;
ALTER TABLE Inventory ADD last_synced_at DATETIME2 NULL;
ALTER TABLE Inventory ADD sync_status NVARCHAR(20) NULL DEFAULT 'pending';
ALTER TABLE Inventory ADD sync_version INT NULL DEFAULT 1;
GO

-- Appointment table
ALTER TABLE Appointment ADD sync_id UNIQUEIDENTIFIER NULL;
ALTER TABLE Appointment ADD last_modified_at DATETIME2 NULL;
ALTER TABLE Appointment ADD last_synced_at DATETIME2 NULL;
ALTER TABLE Appointment ADD sync_status NVARCHAR(20) NULL DEFAULT 'pending';
ALTER TABLE Appointment ADD sync_version INT NULL DEFAULT 1;
GO

-- Sale table
ALTER TABLE Sale ADD sync_id UNIQUEIDENTIFIER NULL;
ALTER TABLE Sale ADD last_modified_at DATETIME2 NULL;
ALTER TABLE Sale ADD last_synced_at DATETIME2 NULL;
ALTER TABLE Sale ADD sync_status NVARCHAR(20) NULL DEFAULT 'pending';
ALTER TABLE Sale ADD sync_version INT NULL DEFAULT 1;
GO

-- Payment table
ALTER TABLE Payment ADD sync_id UNIQUEIDENTIFIER NULL;
ALTER TABLE Payment ADD last_modified_at DATETIME2 NULL;
ALTER TABLE Payment ADD last_synced_at DATETIME2 NULL;
ALTER TABLE Payment ADD sync_status NVARCHAR(20) NULL DEFAULT 'pending';
ALTER TABLE Payment ADD sync_version INT NULL DEFAULT 1;
GO

-- Expense table
ALTER TABLE Expense ADD sync_id UNIQUEIDENTIFIER NULL;
ALTER TABLE Expense ADD last_modified_at DATETIME2 NULL;
ALTER TABLE Expense ADD last_synced_at DATETIME2 NULL;
ALTER TABLE Expense ADD sync_status NVARCHAR(20) NULL DEFAULT 'pending';
ALTER TABLE Expense ADD sync_version INT NULL DEFAULT 1;
GO

-- JournalEntry table
ALTER TABLE JournalEntry ADD sync_id UNIQUEIDENTIFIER NULL;
ALTER TABLE JournalEntry ADD last_modified_at DATETIME2 NULL;
ALTER TABLE JournalEntry ADD last_synced_at DATETIME2 NULL;
ALTER TABLE JournalEntry ADD sync_status NVARCHAR(20) NULL DEFAULT 'pending';
ALTER TABLE JournalEntry ADD sync_version INT NULL DEFAULT 1;
GO

-- Payroll table
ALTER TABLE Payroll ADD sync_id UNIQUEIDENTIFIER NULL;
ALTER TABLE Payroll ADD last_modified_at DATETIME2 NULL;
ALTER TABLE Payroll ADD last_synced_at DATETIME2 NULL;
ALTER TABLE Payroll ADD sync_status NVARCHAR(20) NULL DEFAULT 'pending';
ALTER TABLE Payroll ADD sync_version INT NULL DEFAULT 1;
GO

PRINT 'All sync columns added successfully!';
