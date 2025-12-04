-- Add Commission Columns to Service Table
-- Run this on BOTH local (spa_erp) and cloud (db32359) databases

-- Check if columns already exist and add them if not
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Service') AND name = 'commission_type')
BEGIN
    ALTER TABLE [Service] ADD commission_type NVARCHAR(20) NOT NULL DEFAULT 'percentage';
    PRINT 'Added commission_type column to Service table';
END
ELSE
BEGIN
    PRINT 'commission_type column already exists';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Service') AND name = 'commission_value')
BEGIN
    ALTER TABLE [Service] ADD commission_value DECIMAL(12,2) NOT NULL DEFAULT 0;
    PRINT 'Added commission_value column to Service table';
END
ELSE
BEGIN
    PRINT 'commission_value column already exists';
END
GO

-- Set default commission for existing services (30% percentage-based)
UPDATE [Service] 
SET commission_type = 'percentage', 
    commission_value = 30 
WHERE commission_value = 0;
GO

-- Verify the changes
SELECT service_id, name, price, commission_type, commission_value,
       CASE 
           WHEN commission_type = 'percentage' THEN CONCAT(commission_value, '%')
           ELSE CONCAT('₱', FORMAT(commission_value, 'N2'))
       END AS commission_display,
       CASE 
           WHEN commission_type = 'percentage' THEN price * commission_value / 100
           ELSE commission_value
       END AS therapist_earns
FROM [Service];
GO
