-- Seed Inventory Data for Cloud Database
SET NOCOUNT ON;
PRINT 'Starting cloud inventory seeding...';

-- Get user ID
DECLARE @UserId INT = 1;

-- =====================================================
-- SUPPLIERS
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM Supplier WHERE name = 'Beauty Supplies Co')
BEGIN
    INSERT INTO Supplier (name, contact_person, email, phone, address, is_active, sync_id, sync_status, sync_version)
    VALUES 
    ('Beauty Supplies Co', 'Sarah Johnson', 'sarah@beautysupplies.com', '555-0101', '123 Beauty Lane, City', 1, NEWID(), 'pending', 1),
    ('Organic Spa Products', 'Michael Green', 'michael@organicspa.com', '555-0102', '456 Nature Way, Town', 1, NEWID(), 'pending', 1),
    ('Professional Nail Supply', 'Emily Brown', 'emily@nailsupply.com', '555-0103', '789 Polish Ave, Village', 1, NEWID(), 'pending', 1),
    ('Essential Oils Direct', 'David Lee', 'david@essentialoils.com', '555-0104', '321 Aroma St, City', 1, NEWID(), 'pending', 1),
    ('Spa Equipment World', 'Lisa Chen', 'lisa@spaequipment.com', '555-0105', '654 Wellness Blvd, Town', 1, NEWID(), 'pending', 1),
    ('Natural Body Care', 'James Wilson', 'james@naturalbodycare.com', '555-0106', '987 Herbal Road, Village', 1, NEWID(), 'pending', 1);
    PRINT 'Suppliers created';
END

-- =====================================================
-- PRODUCTS
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM Product WHERE name = 'Lavender Massage Oil 500ml')
BEGIN
    INSERT INTO Product (name, sku, description, category, cost_price, selling_price, is_active, sync_id, sync_status, sync_version)
    VALUES 
    -- Massage Oils
    ('Lavender Massage Oil 500ml', 'OIL-LAV-500', 'Premium lavender-scented massage oil', 'Massage', 12.50, 25.00, 1, NEWID(), 'pending', 1),
    ('Eucalyptus Massage Oil 500ml', 'OIL-EUC-500', 'Refreshing eucalyptus massage oil', 'Massage', 11.00, 22.00, 1, NEWID(), 'pending', 1),
    ('Coconut Massage Oil 500ml', 'OIL-COC-500', 'Natural coconut massage oil', 'Massage', 10.00, 20.00, 1, NEWID(), 'pending', 1),
    ('Aromatherapy Blend Oil 500ml', 'OIL-ARO-500', 'Custom aromatherapy blend', 'Massage', 15.00, 30.00, 1, NEWID(), 'pending', 1),
    
    -- Face Products
    ('Vitamin C Serum 30ml', 'FACE-VIT-30', 'Anti-aging vitamin C serum', 'Face', 25.00, 55.00, 1, NEWID(), 'pending', 1),
    ('Hyaluronic Acid Serum 30ml', 'FACE-HYA-30', 'Hydrating serum', 'Face', 22.00, 48.00, 1, NEWID(), 'pending', 1),
    ('Charcoal Face Mask 100g', 'FACE-CHA-100', 'Deep cleansing charcoal mask', 'Face', 8.00, 18.00, 1, NEWID(), 'pending', 1),
    ('Gold Face Mask 100g', 'FACE-GLD-100', 'Luxury gold face mask', 'Face', 35.00, 75.00, 1, NEWID(), 'pending', 1),
    ('Bentonite Clay Mask 200g', 'FACE-BEN-200', 'Natural clay mask', 'Face', 6.00, 15.00, 1, NEWID(), 'pending', 1),
    
    -- Body Products
    ('Sea Salt Body Scrub 500g', 'BODY-SEA-500', 'Exfoliating sea salt scrub', 'Body', 12.00, 28.00, 1, NEWID(), 'pending', 1),
    ('Coffee Body Scrub 250g', 'BODY-COF-250', 'Energizing coffee scrub', 'Body', 10.00, 24.00, 1, NEWID(), 'pending', 1),
    ('Brown Sugar Scrub 500g', 'BODY-SUG-500', 'Gentle sugar body scrub', 'Body', 9.00, 22.00, 1, NEWID(), 'pending', 1),
    ('Aloe Vera Lotion 300ml', 'BODY-ALO-300', 'Soothing aloe vera body lotion', 'Body', 7.00, 18.00, 1, NEWID(), 'pending', 1),
    ('Hand Cream 250ml', 'BODY-HND-250', 'Moisturizing hand cream', 'Body', 5.00, 14.00, 1, NEWID(), 'pending', 1),
    ('Foot Cream 250ml', 'BODY-FOT-250', 'Nourishing foot cream', 'Body', 5.50, 15.00, 1, NEWID(), 'pending', 1),
    
    -- Nail Products
    ('Nail Polish Red 15ml', 'NAIL-RED-15', 'Classic red nail polish', 'Nail', 3.50, 10.00, 1, NEWID(), 'pending', 1),
    ('Nail Polish Pink 15ml', 'NAIL-PNK-15', 'Soft pink nail polish', 'Nail', 3.50, 10.00, 1, NEWID(), 'pending', 1),
    ('Nail Polish Nude 15ml', 'NAIL-NDE-15', 'Natural nude nail polish', 'Nail', 3.50, 10.00, 1, NEWID(), 'pending', 1),
    ('Nail Polish Remover 100ml', 'NAIL-REM-100', 'Acetone-free nail polish remover', 'Nail', 2.00, 6.00, 1, NEWID(), 'pending', 1),
    ('Cotton Pads (100 pcs)', 'NAIL-COT-100', 'Soft cotton pads for nail care', 'Nail', 1.50, 5.00, 1, NEWID(), 'pending', 1),
    
    -- Equipment & Supplies
    ('Hot Stone Set (12 pcs)', 'EQUIP-HST-12', 'Basalt hot stone massage set', 'Equipment', 45.00, 95.00, 1, NEWID(), 'pending', 1),
    ('White Cotton Towel Large', 'EQUIP-TWL-L', 'Large spa towel', 'Supplies', 8.00, 0.00, 1, NEWID(), 'pending', 1),
    ('White Cotton Towel Small', 'EQUIP-TWL-S', 'Small face towel', 'Supplies', 4.00, 0.00, 1, NEWID(), 'pending', 1),
    ('Spa Robe Medium', 'EQUIP-ROB-M', 'Cotton spa robe', 'Supplies', 25.00, 0.00, 1, NEWID(), 'pending', 1),
    ('Spa Headband', 'EQUIP-HDB-1', 'Elastic spa headband', 'Supplies', 1.50, 0.00, 1, NEWID(), 'pending', 1),
    ('Disposable Spa Slippers', 'EQUIP-SLP-1', 'Disposable slippers (pair)', 'Supplies', 0.75, 0.00, 1, NEWID(), 'pending', 1),
    ('Aromatherapy Candle Large', 'EQUIP-CND-L', 'Scented spa candle', 'Supplies', 12.00, 28.00, 1, NEWID(), 'pending', 1);
    PRINT 'Products created';
END

-- =====================================================
-- INVENTORY RECORDS
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM Inventory)
BEGIN
    INSERT INTO Inventory (product_id, quantity_on_hand, reorder_level, last_counted_at, sync_id, sync_status, sync_version)
    SELECT product_id, 0, 
        CASE category
            WHEN 'Massage' THEN FLOOR(RAND(CHECKSUM(NEWID())) * 8) + 5
            WHEN 'Face' THEN FLOOR(RAND(CHECKSUM(NEWID())) * 10) + 5
            WHEN 'Body' THEN FLOOR(RAND(CHECKSUM(NEWID())) * 10) + 10
            WHEN 'Nail' THEN FLOOR(RAND(CHECKSUM(NEWID())) * 10) + 10
            WHEN 'Equipment' THEN FLOOR(RAND(CHECKSUM(NEWID())) * 5) + 3
            ELSE FLOOR(RAND(CHECKSUM(NEWID())) * 10) + 5
        END,
        DATEADD(DAY, -FLOOR(RAND(CHECKSUM(NEWID())) * 14), GETDATE()),
        NEWID(), 'pending', 1
    FROM Product;
    PRINT 'Inventory records created';
END

-- =====================================================
-- SUPPLIER-PRODUCT RELATIONSHIPS
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM SupplierProduct)
BEGIN
    INSERT INTO SupplierProduct (supplier_id, product_id, supplier_price, is_preferred, is_active, sync_id, sync_status, sync_version)
    SELECT s.supplier_id, p.product_id, 
        p.cost_price * (0.85 + (RAND(CHECKSUM(NEWID())) * 0.2)),
        CASE WHEN ROW_NUMBER() OVER (PARTITION BY p.product_id ORDER BY NEWID()) = 1 THEN 1 ELSE 0 END,
        1, NEWID(), 'pending', 1
    FROM Supplier s CROSS JOIN Product p;
    PRINT 'Supplier-Product relationships created';
END

-- =====================================================
-- PURCHASE ORDERS
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM PurchaseOrder)
BEGIN
    INSERT INTO PurchaseOrder (supplier_id, created_by_user_id, po_number, status, created_at, sync_id, sync_status, sync_version)
    SELECT supplier_id, @UserId, 'PO-2024-' + RIGHT('000' + CAST(ROW_NUMBER() OVER (ORDER BY supplier_id) AS VARCHAR), 3),
        CASE WHEN supplier_id % 3 = 0 THEN 'received' WHEN supplier_id % 3 = 1 THEN 'pending' ELSE 'ordered' END,
        DATEADD(DAY, -supplier_id * 5, GETDATE()),
        NEWID(), 'pending', 1
    FROM Supplier WHERE supplier_id <= 3;
    PRINT 'Purchase orders created';
END

-- =====================================================
-- PURCHASE ORDER ITEMS
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM PurchaseOrderItem)
BEGIN
    INSERT INTO PurchaseOrderItem (po_id, product_id, qty_ordered, unit_cost, sync_id, sync_status, sync_version)
    SELECT po.po_id, p.product_id, 
        FLOOR(RAND(CHECKSUM(NEWID())) * 20) + 10,
        p.cost_price,
        NEWID(), 'pending', 1
    FROM PurchaseOrder po
    CROSS APPLY (SELECT TOP 6 * FROM Product ORDER BY NEWID()) p;
    PRINT 'Purchase order items created';
END

-- =====================================================
-- STOCK TRANSACTIONS
-- =====================================================
IF NOT EXISTS (SELECT 1 FROM StockTransaction)
BEGIN
    -- Purchase transactions (receiving stock)
    INSERT INTO StockTransaction (product_id, tx_type, qty, unit_cost, reference, created_at, sync_id, sync_status, sync_version)
    SELECT i.product_id, 'purchase', 25, p.cost_price, 'Initial stock - PO-INIT', DATEADD(DAY, -30, GETDATE()), NEWID(), 'pending', 1
    FROM Inventory i JOIN Product p ON i.product_id = p.product_id;

    -- Some sale transactions
    INSERT INTO StockTransaction (product_id, tx_type, qty, unit_cost, reference, created_at, sync_id, sync_status, sync_version)
    SELECT TOP 10 i.product_id, 'sale', 3, p.cost_price, 'Service usage', DATEADD(DAY, -7, GETDATE()), NEWID(), 'pending', 1
    FROM Inventory i JOIN Product p ON i.product_id = p.product_id ORDER BY NEWID();
    
    PRINT 'Stock transactions created';
END

-- =====================================================
-- UPDATE INVENTORY QUANTITIES
-- =====================================================
UPDATE i SET i.quantity_on_hand = (
    SELECT COALESCE(SUM(CASE WHEN st.tx_type IN ('purchase','return') THEN st.qty ELSE -st.qty END), 0)
    FROM StockTransaction st WHERE st.product_id = i.product_id
) FROM Inventory i;
PRINT 'Inventory quantities updated';

-- Summary
SELECT 'Products' AS [Table], COUNT(*) AS [Count] FROM Product UNION ALL
SELECT 'Inventory', COUNT(*) FROM Inventory UNION ALL
SELECT 'Suppliers', COUNT(*) FROM Supplier UNION ALL
SELECT 'SupplierProduct', COUNT(*) FROM SupplierProduct UNION ALL
SELECT 'StockTransactions', COUNT(*) FROM StockTransaction UNION ALL
SELECT 'PurchaseOrders', COUNT(*) FROM PurchaseOrder UNION ALL
SELECT 'PurchaseOrderItems', COUNT(*) FROM PurchaseOrderItem;

PRINT 'Cloud inventory seeding complete!';
