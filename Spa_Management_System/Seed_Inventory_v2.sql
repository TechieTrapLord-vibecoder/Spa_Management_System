-- Comprehensive Inventory Seed Data (Corrected Column Names)
-- Run on local database (spa_erp) and cloud database (db32359)

-- =====================================================
-- 1. CREATE INVENTORY RECORDS FOR ALL PRODUCTS
-- =====================================================
-- Insert inventory for products that don't have inventory yet
INSERT INTO Inventory (product_id, quantity_on_hand, reorder_level, sync_id, sync_status, sync_version)
SELECT p.product_id, 
       -- Random starting quantity between 10-50
       CAST(10 + (ABS(CHECKSUM(NEWID())) % 41) AS INT),
       -- Reorder level: 5-15
       CAST(5 + (ABS(CHECKSUM(NEWID())) % 11) AS INT),
       NEWID(), 'pending', 1
FROM Product p
WHERE NOT EXISTS (SELECT 1 FROM Inventory i WHERE i.product_id = p.product_id)
  AND p.active = 1;
GO

-- =====================================================
-- 2. CREATE SOME STOCK TRANSACTIONS (HISTORY)
-- =====================================================
-- Add some receiving transactions for initial stock
INSERT INTO StockTransaction (product_id, tx_type, qty, unit_cost, reference, created_at, sync_id, sync_status, sync_version)
SELECT i.product_id, 
       'received',
       i.quantity_on_hand,
       p.cost_price,
       'Initial inventory setup',
       DATEADD(DAY, -30 + (ABS(CHECKSUM(NEWID())) % 30), GETDATE()),
       NEWID(), 'pending', 1
FROM Inventory i
JOIN Product p ON i.product_id = p.product_id
WHERE NOT EXISTS (SELECT 1 FROM StockTransaction st WHERE st.product_id = i.product_id);
GO

-- Add some usage/sales transactions
INSERT INTO StockTransaction (product_id, tx_type, qty, unit_cost, reference, created_at, sync_id, sync_status, sync_version)
SELECT TOP 15 i.product_id, 
       'used',
       CAST(2 + (ABS(CHECKSUM(NEWID())) % 5) AS INT),
       p.cost_price,
       'Used in service',
       DATEADD(DAY, -7 + (ABS(CHECKSUM(NEWID())) % 7), GETDATE()),
       NEWID(), 'pending', 1
FROM Inventory i
JOIN Product p ON i.product_id = p.product_id
ORDER BY NEWID();
GO

-- =====================================================
-- 3. CREATE SAMPLE PURCHASE ORDERS
-- =====================================================
DECLARE @supId1 BIGINT = (SELECT TOP 1 supplier_id FROM Supplier WHERE is_archived = 0 ORDER BY supplier_id);
DECLARE @supId2 BIGINT = (SELECT TOP 1 supplier_id FROM Supplier WHERE is_archived = 0 AND supplier_id > @supId1 ORDER BY supplier_id);

-- PO 1: Completed/Received
IF NOT EXISTS (SELECT 1 FROM PurchaseOrder WHERE po_number = 'PO-2024-0001')
BEGIN
    INSERT INTO PurchaseOrder (po_number, supplier_id, status, created_at, sync_id, sync_status, sync_version)
    VALUES ('PO-2024-0001', @supId1, 'received', DATEADD(DAY, -14, GETDATE()), NEWID(), 'pending', 1);
    
    DECLARE @po1 BIGINT = SCOPE_IDENTITY();
    
    INSERT INTO PurchaseOrderItem (po_id, product_id, qty_ordered, unit_cost, sync_id, sync_status, sync_version)
    SELECT @po1, product_id, 
           CAST(10 + (ABS(CHECKSUM(NEWID())) % 20) AS INT),
           cost_price,
           NEWID(), 'pending', 1
    FROM Product WHERE active = 1 AND product_id <= 5;
END

-- PO 2: Pending
IF NOT EXISTS (SELECT 1 FROM PurchaseOrder WHERE po_number = 'PO-2024-0002')
BEGIN
    INSERT INTO PurchaseOrder (po_number, supplier_id, status, created_at, sync_id, sync_status, sync_version)
    VALUES ('PO-2024-0002', @supId2, 'pending', DATEADD(DAY, -3, GETDATE()), NEWID(), 'pending', 1);
    
    DECLARE @po2 BIGINT = SCOPE_IDENTITY();
    
    INSERT INTO PurchaseOrderItem (po_id, product_id, qty_ordered, unit_cost, sync_id, sync_status, sync_version)
    SELECT @po2, product_id, 
           CAST(15 + (ABS(CHECKSUM(NEWID())) % 15) AS INT),
           cost_price,
           NEWID(), 'pending', 1
    FROM Product WHERE active = 1 AND product_id > 5 AND product_id <= 12;
END

-- PO 3: Draft
IF NOT EXISTS (SELECT 1 FROM PurchaseOrder WHERE po_number = 'PO-2024-0003')
BEGIN
    INSERT INTO PurchaseOrder (po_number, supplier_id, status, created_at, sync_id, sync_status, sync_version)
    VALUES ('PO-2024-0003', @supId1, 'draft', GETDATE(), NEWID(), 'pending', 1);
    
    DECLARE @po3 BIGINT = SCOPE_IDENTITY();
    
    INSERT INTO PurchaseOrderItem (po_id, product_id, qty_ordered, unit_cost, sync_id, sync_status, sync_version)
    SELECT @po3, product_id, 
           CAST(5 + (ABS(CHECKSUM(NEWID())) % 10) AS INT),
           cost_price,
           NEWID(), 'pending', 1
    FROM Product WHERE active = 1 AND product_id > 12 AND product_id <= 18;
END
GO

-- =====================================================
-- Update inventory quantities to reflect stock transactions
-- =====================================================
UPDATE i
SET i.quantity_on_hand = (
    SELECT COALESCE(SUM(CASE WHEN st.tx_type = 'received' THEN st.qty ELSE -st.qty END), 0)
    FROM StockTransaction st
    WHERE st.product_id = i.product_id
)
FROM Inventory i;
GO

-- Make sure no negative inventory
UPDATE Inventory SET quantity_on_hand = 5 WHERE quantity_on_hand < 0;
GO

-- =====================================================
-- VERIFICATION
-- =====================================================
SELECT 'Products' as [Table], COUNT(*) as [Count] FROM Product
UNION ALL
SELECT 'Inventory', COUNT(*) FROM Inventory
UNION ALL
SELECT 'Suppliers', COUNT(*) FROM Supplier
UNION ALL
SELECT 'SupplierProduct', COUNT(*) FROM SupplierProduct
UNION ALL
SELECT 'StockTransactions', COUNT(*) FROM StockTransaction
UNION ALL
SELECT 'PurchaseOrders', COUNT(*) FROM PurchaseOrder
UNION ALL
SELECT 'PurchaseOrderItems', COUNT(*) FROM PurchaseOrderItem;
GO

-- Show inventory summary
SELECT TOP 15 p.sku, p.name, i.quantity_on_hand as qty, i.reorder_level as reorder_at,
       CASE WHEN i.quantity_on_hand <= i.reorder_level THEN 'LOW STOCK!' ELSE 'OK' END as status
FROM Inventory i
JOIN Product p ON i.product_id = p.product_id
ORDER BY i.quantity_on_hand;
GO
