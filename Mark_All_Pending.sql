-- =====================================================
-- Mark All Local Data as Pending for Cloud Sync
-- This will mark all records to be pushed to cloud
-- =====================================================

USE spa_erp;
GO

-- Update all tables to sync_status = 'pending'
UPDATE [dbo].[LedgerAccount] SET sync_status = 'pending';
UPDATE [dbo].[Employee] SET sync_status = 'pending';
UPDATE [dbo].[Customer] SET sync_status = 'pending';
UPDATE [dbo].[Product] SET sync_status = 'pending';
UPDATE [dbo].[Service] SET sync_status = 'pending';
UPDATE [dbo].[Inventory] SET sync_status = 'pending';
UPDATE [dbo].[SupplierProduct] SET sync_status = 'pending';
UPDATE [dbo].[Appointment] SET sync_status = 'pending';
UPDATE [dbo].[AppointmentService] SET sync_status = 'pending';
UPDATE [dbo].[Sale] SET sync_status = 'pending';
UPDATE [dbo].[SaleItem] SET sync_status = 'pending';
UPDATE [dbo].[EmployeeServiceCommission] SET sync_status = 'pending';
UPDATE [dbo].[EmployeeAttendance] SET sync_status = 'pending';
UPDATE [dbo].[Payroll] SET sync_status = 'pending';
UPDATE [dbo].[Expense] SET sync_status = 'pending';
UPDATE [dbo].[JournalEntry] SET sync_status = 'pending';
UPDATE [dbo].[JournalEntryLine] SET sync_status = 'pending';
UPDATE [dbo].[AuditLog] SET sync_status = 'pending';
GO

-- Show summary of pending records
SELECT 'LedgerAccount' as TableName, COUNT(*) as PendingCount FROM [dbo].[LedgerAccount] WHERE sync_status = 'pending'
UNION ALL SELECT 'Employee', COUNT(*) FROM [dbo].[Employee] WHERE sync_status = 'pending'
UNION ALL SELECT 'Customer', COUNT(*) FROM [dbo].[Customer] WHERE sync_status = 'pending'
UNION ALL SELECT 'Product', COUNT(*) FROM [dbo].[Product] WHERE sync_status = 'pending'
UNION ALL SELECT 'Service', COUNT(*) FROM [dbo].[Service] WHERE sync_status = 'pending'
UNION ALL SELECT 'Inventory', COUNT(*) FROM [dbo].[Inventory] WHERE sync_status = 'pending'
UNION ALL SELECT 'SupplierProduct', COUNT(*) FROM [dbo].[SupplierProduct] WHERE sync_status = 'pending'
UNION ALL SELECT 'Appointment', COUNT(*) FROM [dbo].[Appointment] WHERE sync_status = 'pending'
UNION ALL SELECT 'AppointmentService', COUNT(*) FROM [dbo].[AppointmentService] WHERE sync_status = 'pending'
UNION ALL SELECT 'Sale', COUNT(*) FROM [dbo].[Sale] WHERE sync_status = 'pending'
UNION ALL SELECT 'SaleItem', COUNT(*) FROM [dbo].[SaleItem] WHERE sync_status = 'pending'
UNION ALL SELECT 'EmployeeServiceCommission', COUNT(*) FROM [dbo].[EmployeeServiceCommission] WHERE sync_status = 'pending'
UNION ALL SELECT 'EmployeeAttendance', COUNT(*) FROM [dbo].[EmployeeAttendance] WHERE sync_status = 'pending'
UNION ALL SELECT 'Payroll', COUNT(*) FROM [dbo].[Payroll] WHERE sync_status = 'pending'
UNION ALL SELECT 'Expense', COUNT(*) FROM [dbo].[Expense] WHERE sync_status = 'pending'
UNION ALL SELECT 'JournalEntry', COUNT(*) FROM [dbo].[JournalEntry] WHERE sync_status = 'pending'
UNION ALL SELECT 'JournalEntryLine', COUNT(*) FROM [dbo].[JournalEntryLine] WHERE sync_status = 'pending'
UNION ALL SELECT 'AuditLog', COUNT(*) FROM [dbo].[AuditLog] WHERE sync_status = 'pending'
ORDER BY TableName;
GO

PRINT 'All local data marked as pending for sync!';
PRINT 'Now run the sync from your app to push everything to cloud.';
GO
