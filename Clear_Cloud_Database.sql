-- =====================================================
-- Clear All Cloud Database Tables
-- Server: db32359.public.databaseasp.net
-- Database: db32359
-- =====================================================

USE db32359;
GO

-- Disable all foreign key constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';
GO

-- Delete all data from tables (in correct order to avoid FK issues)
DELETE FROM [dbo].[JournalEntryLine];
DELETE FROM [dbo].[JournalEntry];
DELETE FROM [dbo].[SaleItem];
DELETE FROM [dbo].[Sale];
DELETE FROM [dbo].[AppointmentService];
DELETE FROM [dbo].[Appointment];
DELETE FROM [dbo].[EmployeeServiceCommission];
DELETE FROM [dbo].[EmployeeAttendance];
DELETE FROM [dbo].[Payroll];
DELETE FROM [dbo].[AuditLog];
DELETE FROM [dbo].[CrmNote];
DELETE FROM [dbo].[Expense];
DELETE FROM [dbo].[Inventory];
DELETE FROM [dbo].[SupplierProduct];
DELETE FROM [dbo].[Service];
DELETE FROM [dbo].[Product];
DELETE FROM [dbo].[Customer];
DELETE FROM [dbo].[Employee];
DELETE FROM [dbo].[LedgerAccount];

-- Re-enable all foreign key constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL';
GO

-- Verify all tables are empty
SELECT 
    t.name AS TableName,
    SUM(p.rows) AS RowCount
FROM sys.tables t
INNER JOIN sys.partitions p ON t.object_id = p.object_id
WHERE p.index_id IN (0,1)
    AND t.name NOT IN ('__EFMigrationsHistory', 'sysdiagrams')
GROUP BY t.name
ORDER BY t.name;
GO

PRINT 'Cloud database cleared successfully!';
PRINT 'You can now push all data from local to cloud.';
GO
