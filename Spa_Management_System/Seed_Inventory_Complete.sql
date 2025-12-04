-- Comprehensive Inventory Seed Data
-- Run on local database (spa_erp) and cloud database (db32359)

-- =====================================================
-- 1. ADD MORE PRODUCTS (if needed)
-- =====================================================
-- Check and add more spa products
IF NOT EXISTS (SELECT 1 FROM Product WHERE sku = 'OIL-EUCAL-500')
BEGIN
    INSERT INTO Product (sku, name, description, unit_price, cost_price, unit, active, sync_id, sync_status, sync_version)
    VALUES 
    ('OIL-EUCAL-500', 'Eucalyptus Massage Oil 500ml', 'Refreshing eucalyptus oil for massage therapy', 420.00, 250.00, 'bottle', 1, NEWID(), 'pending', 1),
    ('OIL-AROMA-500', 'Aromatherapy Blend Oil 500ml', 'Premium blend of essential oils', 550.00, 320.00, 'bottle', 1, NEWID(), 'pending', 1),
    ('CREAM-HAND-250', 'Hand Cream 250ml', 'Moisturizing hand cream for manicure', 280.00, 140.00, 'jar', 1, NEWID(), 'pending', 1),
    ('CREAM-FOOT-250', 'Foot Cream 250ml', 'Softening foot cream for pedicure', 280.00, 140.00, 'jar', 1, NEWID(), 'pending', 1),
    ('SCRUB-SALT-500', 'Sea Salt Body Scrub 500g', 'Exfoliating sea salt scrub', 380.00, 180.00, 'jar', 1, NEWID(), 'pending', 1),
    ('SCRUB-SUGAR-500', 'Brown Sugar Scrub 500g', 'Gentle sugar exfoliant', 350.00, 160.00, 'jar', 1, NEWID(), 'pending', 1),
    ('MASK-GOLD-100', 'Gold Face Mask 100g', 'Premium gold-infused face mask', 450.00, 220.00, 'jar', 1, NEWID(), 'pending', 1),
    ('MASK-CLAY-200', 'Bentonite Clay Mask 200g', 'Deep cleansing clay mask', 320.00, 150.00, 'jar', 1, NEWID(), 'pending', 1),
    ('SERUM-VIT-C-30', 'Vitamin C Serum 30ml', 'Brightening vitamin C serum', 650.00, 350.00, 'bottle', 1, NEWID(), 'pending', 1),
    ('SERUM-HYALU-30', 'Hyaluronic Acid Serum 30ml', 'Hydrating serum', 580.00, 300.00, 'bottle', 1, NEWID(), 'pending', 1),
    ('STONE-HOT-SET', 'Hot Stone Set (12 pcs)', 'Basalt stones for hot stone massage', 1200.00, 600.00, 'set', 1, NEWID(), 'pending', 1),
    ('CANDLE-AROMA-L', 'Aromatherapy Candle Large', 'Scented spa candle', 280.00, 120.00, 'piece', 1, NEWID(), 'pending', 1),
    ('ROBE-SPA-M', 'Spa Robe Medium', 'Cotton spa robe for clients', 450.00, 220.00, 'piece', 1, NEWID(), 'pending', 1),
    ('SLIPPER-SPA', 'Disposable Spa Slippers', 'Single-use spa slippers', 25.00, 10.00, 'pair', 1, NEWID(), 'pending', 1),
    ('HEADBAND-SPA', 'Spa Headband', 'Terrycloth headband', 45.00, 18.00, 'piece', 1, NEWID(), 'pending', 1),
    ('POLISH-RED-15', 'Nail Polish Red 15ml', 'Classic red nail polish', 180.00, 80.00, 'bottle', 1, NEWID(), 'pending', 1),
    ('POLISH-PINK-15', 'Nail Polish Pink 15ml', 'Soft pink nail polish', 180.00, 80.00, 'bottle', 1, NEWID(), 'pending', 1),
    ('POLISH-NUDE-15', 'Nail Polish Nude 15ml', 'Natural nude nail polish', 180.00, 80.00, 'bottle', 1, NEWID(), 'pending', 1),
    ('REMOVER-100', 'Nail Polish Remover 100ml', 'Acetone-free remover', 120.00, 50.00, 'bottle', 1, NEWID(), 'pending', 1),
    ('COTTON-PADS-100', 'Cotton Pads (100 pcs)', 'Soft cotton pads for facial', 85.00, 35.00, 'pack', 1, NEWID(), 'pending', 1);
END
GO

-- =====================================================
-- 2. CREATE INVENTORY RECORDS FOR ALL PRODUCTS
-- =====================================================
-- Insert inventory for products that don't have inventory yet
INSERT INTO Inventory (product_id, qty_on_hand, reorder_level, reorder_qty, warehouse_location, sync_id, sync_status, sync_version)
SELECT p.product_id, 
       -- Random starting quantity between 10-50
       CAST(10 + (ABS(CHECKSUM(NEWID())) % 41) AS INT),
       -- Reorder level: 5-15
       CAST(5 + (ABS(CHECKSUM(NEWID())) % 11) AS INT),
       -- Reorder quantity: 20-50
       CAST(20 + (ABS(CHECKSUM(NEWID())) % 31) AS INT),
       'Main Storage',
       NEWID(), 'pending', 1
FROM Product p
WHERE NOT EXISTS (SELECT 1 FROM Inventory i WHERE i.product_id = p.product_id)
  AND p.active = 1;
GO

-- =====================================================
-- 3. ADD MORE SUPPLIERS (if needed)
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM Supplier WHERE name = 'Manila Beauty Supply')
BEGIN
    INSERT INTO Supplier (name, contact_person, phone, email, address, is_archived, sync_id, sync_status, sync_version)
    VALUES 
    ('Manila Beauty Supply', 'Rosa Mendoza', '0917-555-1234', 'rosa@manilabeauty.ph', '123 Makati Ave, Makati City', 0, NEWID(), 'pending', 1),
    ('Cebu Spa Distributors', 'Marco Tan', '0918-555-5678', 'marco@cebuspa.ph', '456 Mango Ave, Cebu City', 0, NEWID(), 'pending', 1),
    ('Oriental Wellness Trading', 'Kim Lee', '0919-555-9012', 'kim@orientalwellness.ph', '789 Binondo St, Manila', 0, NEWID(), 'pending', 1);
END
GO

-- =====================================================
-- 4. CREATE SUPPLIER-PRODUCT RELATIONSHIPS
-- =====================================================
-- Link suppliers to products with prices
-- Each product can be supplied by multiple suppliers at different prices

-- Get supplier and product IDs dynamically
DECLARE @sup1 BIGINT = (SELECT TOP 1 supplier_id FROM Supplier WHERE name LIKE '%BeautyMax%');
DECLARE @sup2 BIGINT = (SELECT TOP 1 supplier_id FROM Supplier WHERE name LIKE '%Essentials%');
DECLARE @sup3 BIGINT = (SELECT TOP 1 supplier_id FROM Supplier WHERE name LIKE '%Natural%');
DECLARE @sup4 BIGINT = (SELECT TOP 1 supplier_id FROM Supplier WHERE name LIKE '%Manila%');
DECLARE @sup5 BIGINT = (SELECT TOP 1 supplier_id FROM Supplier WHERE name LIKE '%Cebu%');

-- Insert supplier-product relationships (if not exists)
INSERT INTO SupplierProduct (supplier_id, product_id, supplier_price, supplier_sku, min_order_qty, lead_time_days, is_preferred, is_active, sync_id, sync_status, sync_version, created_at)
SELECT s.supplier_id, p.product_id, 
       p.cost_price * (0.85 + (ABS(CHECKSUM(NEWID())) % 20) / 100.0), -- Random price 85-105% of cost
       CONCAT('SUP', s.supplier_id, '-', p.sku),
       CASE WHEN ABS(CHECKSUM(NEWID())) % 2 = 0 THEN 5 ELSE 10 END, -- Min order 5 or 10
       CASE WHEN ABS(CHECKSUM(NEWID())) % 3 = 0 THEN 3 ELSE 7 END,  -- Lead time 3 or 7 days
       CASE WHEN ROW_NUMBER() OVER (PARTITION BY p.product_id ORDER BY s.supplier_id) = 1 THEN 1 ELSE 0 END, -- First supplier is preferred
       1,
       NEWID(), 'pending', 1, GETDATE()
FROM Supplier s
CROSS JOIN Product p
WHERE NOT EXISTS (
    SELECT 1 FROM SupplierProduct sp 
    WHERE sp.supplier_id = s.supplier_id AND sp.product_id = p.product_id
)
AND s.is_archived = 0 AND p.active = 1
-- Limit to ~2-3 suppliers per product randomly
AND (ABS(CHECKSUM(NEWID())) % 3) < 2;
GO

-- =====================================================
-- 5. CREATE SOME STOCK TRANSACTIONS (HISTORY)
-- =====================================================
-- Add some receiving transactions
INSERT INTO StockTransaction (product_id, transaction_type, qty_change, reference_type, notes, created_at, sync_id, sync_status, sync_version)
SELECT i.product_id, 
       'received',
       CAST(20 + (ABS(CHECKSUM(NEWID())) % 30) AS INT),
       'Initial Stock',
       'Initial inventory setup',
       DATEADD(DAY, -30 + (ABS(CHECKSUM(NEWID())) % 30), GETDATE()),
       NEWID(), 'pending', 1
FROM Inventory i
WHERE NOT EXISTS (SELECT 1 FROM StockTransaction st WHERE st.product_id = i.product_id);
GO

-- =====================================================
-- 6. CREATE SAMPLE PURCHASE ORDERS
-- =====================================================
-- Check if we have any POs
IF NOT EXISTS (SELECT 1 FROM PurchaseOrder)
BEGIN
    DECLARE @supId1 BIGINT = (SELECT TOP 1 supplier_id FROM Supplier WHERE is_archived = 0 ORDER BY supplier_id);
    DECLARE @supId2 BIGINT = (SELECT TOP 1 supplier_id FROM Supplier WHERE is_archived = 0 AND supplier_id > @supId1 ORDER BY supplier_id);
    
    -- PO 1: Completed/Received
    INSERT INTO PurchaseOrder (po_number, supplier_id, order_date, expected_delivery, status, notes, created_at, sync_id, sync_status, sync_version)
    VALUES ('PO-2024-0001', @supId1, DATEADD(DAY, -14, GETDATE()), DATEADD(DAY, -7, GETDATE()), 'received', 'Regular restocking order', DATEADD(DAY, -14, GETDATE()), NEWID(), 'pending', 1);
    
    DECLARE @po1 BIGINT = SCOPE_IDENTITY();
    
    INSERT INTO PurchaseOrderItem (purchase_order_id, product_id, qty_ordered, unit_cost, sync_id, sync_status, sync_version)
    SELECT @po1, product_id, 
           CAST(10 + (ABS(CHECKSUM(NEWID())) % 20) AS INT),
           cost_price,
           NEWID(), 'pending', 1
    FROM Product WHERE active = 1 AND product_id <= 5;
    
    -- PO 2: Pending
    INSERT INTO PurchaseOrder (po_number, supplier_id, order_date, expected_delivery, status, notes, created_at, sync_id, sync_status, sync_version)
    VALUES ('PO-2024-0002', @supId2, DATEADD(DAY, -3, GETDATE()), DATEADD(DAY, 4, GETDATE()), 'pending', 'Monthly supplies order', DATEADD(DAY, -3, GETDATE()), NEWID(), 'pending', 1);
    
    DECLARE @po2 BIGINT = SCOPE_IDENTITY();
    
    INSERT INTO PurchaseOrderItem (purchase_order_id, product_id, qty_ordered, unit_cost, sync_id, sync_status, sync_version)
    SELECT @po2, product_id, 
           CAST(15 + (ABS(CHECKSUM(NEWID())) % 15) AS INT),
           cost_price,
           NEWID(), 'pending', 1
    FROM Product WHERE active = 1 AND product_id > 5 AND product_id <= 10;
END
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
SELECT TOP 10 p.sku, p.name, i.qty_on_hand, i.reorder_level, 
       CASE WHEN i.qty_on_hand <= i.reorder_level THEN 'LOW STOCK' ELSE 'OK' END as stock_status
FROM Inventory i
JOIN Product p ON i.product_id = p.product_id
ORDER BY i.qty_on_hand;
GO
