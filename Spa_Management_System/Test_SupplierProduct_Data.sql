-- =====================================================
-- TEST DATA: Supplier-Product Relationships
-- Run this on your LOCAL database to test
-- =====================================================

-- First, let's add some suppliers
INSERT INTO Supplier (name, contact_person, phone, email, address, sync_status)
VALUES 
('BeautyMax Supplies', 'Maria Santos', '0917-123-4567', 'maria@beautymax.ph', 'Makati City, Metro Manila', 'pending'),
('Spa Essentials PH', 'Juan Cruz', '0918-987-6543', 'juan@spaessentials.ph', 'Quezon City, Metro Manila', 'pending'),
('Natural Wellness Co.', 'Ana Reyes', '0919-555-1234', 'ana@naturalwellness.com', 'Cebu City', 'pending');

-- Get the supplier IDs (will be 1, 2, 3 if starting fresh)
DECLARE @BeautyMax BIGINT = (SELECT supplier_id FROM Supplier WHERE name = 'BeautyMax Supplies');
DECLARE @SpaEssentials BIGINT = (SELECT supplier_id FROM Supplier WHERE name = 'Spa Essentials PH');
DECLARE @NaturalWellness BIGINT = (SELECT supplier_id FROM Supplier WHERE name = 'Natural Wellness Co.');

-- Add some products
INSERT INTO Product (sku, name, description, unit_price, cost_price, unit, active, sync_status)
VALUES 
('OIL-LAVENDER-500', 'Lavender Massage Oil 500ml', 'Premium lavender essential oil for relaxation massage', 450.00, 280.00, 'bottle', 1, 'pending'),
('OIL-COCONUT-500', 'Coconut Massage Oil 500ml', 'Organic virgin coconut oil', 380.00, 220.00, 'bottle', 1, 'pending'),
('TOWEL-WHITE-L', 'White Cotton Towel Large', 'Soft 100% cotton towel', 250.00, 150.00, 'pcs', 1, 'pending'),
('TOWEL-WHITE-S', 'White Cotton Towel Small', 'Soft 100% cotton hand towel', 120.00, 70.00, 'pcs', 1, 'pending'),
('SCRUB-COFFEE-250', 'Coffee Body Scrub 250g', 'Exfoliating coffee scrub with coconut', 350.00, 180.00, 'jar', 1, 'pending'),
('MASK-CHARCOAL-100', 'Charcoal Face Mask 100g', 'Deep cleansing activated charcoal mask', 280.00, 140.00, 'tube', 1, 'pending'),
('LOTION-ALOE-300', 'Aloe Vera Lotion 300ml', 'Moisturizing aloe vera body lotion', 320.00, 160.00, 'bottle', 1, 'pending');

-- Get the product IDs
DECLARE @Lavender BIGINT = (SELECT product_id FROM Product WHERE sku = 'OIL-LAVENDER-500');
DECLARE @Coconut BIGINT = (SELECT product_id FROM Product WHERE sku = 'OIL-COCONUT-500');
DECLARE @TowelL BIGINT = (SELECT product_id FROM Product WHERE sku = 'TOWEL-WHITE-L');
DECLARE @TowelS BIGINT = (SELECT product_id FROM Product WHERE sku = 'TOWEL-WHITE-S');
DECLARE @Coffee BIGINT = (SELECT product_id FROM Product WHERE sku = 'SCRUB-COFFEE-250');
DECLARE @Charcoal BIGINT = (SELECT product_id FROM Product WHERE sku = 'MASK-CHARCOAL-100');
DECLARE @Aloe BIGINT = (SELECT product_id FROM Product WHERE sku = 'LOTION-ALOE-300');

-- Now link products to suppliers with DIFFERENT PRICES per supplier
-- BeautyMax Supplies - sells oils and lotions
INSERT INTO SupplierProduct (supplier_id, product_id, supplier_price, supplier_sku, is_preferred, is_active)
VALUES 
(@BeautyMax, @Lavender, 280.00, 'BM-LAV-500', 1, 1),      -- Preferred supplier for Lavender
(@BeautyMax, @Coconut, 220.00, 'BM-COC-500', 0, 1),
(@BeautyMax, @Aloe, 160.00, 'BM-ALOE-300', 1, 1);         -- Preferred supplier for Aloe

-- Spa Essentials PH - sells oils, towels, and scrubs (some overlap with BeautyMax!)
INSERT INTO SupplierProduct (supplier_id, product_id, supplier_price, supplier_sku, is_preferred, is_active)
VALUES 
(@SpaEssentials, @Lavender, 295.00, 'SE-LAV500', 0, 1),   -- More expensive than BeautyMax!
(@SpaEssentials, @Coconut, 210.00, 'SE-COC500', 1, 1),    -- Cheaper - preferred for Coconut
(@SpaEssentials, @TowelL, 150.00, 'SE-TWL-L', 1, 1),      -- Preferred for Large Towel
(@SpaEssentials, @TowelS, 70.00, 'SE-TWL-S', 1, 1),       -- Preferred for Small Towel
(@SpaEssentials, @Coffee, 175.00, 'SE-SCR-COF', 0, 1);

-- Natural Wellness Co. - sells organic/natural products
INSERT INTO SupplierProduct (supplier_id, product_id, supplier_price, supplier_sku, is_preferred, is_active)
VALUES 
(@NaturalWellness, @Coffee, 165.00, 'NW-COFFEE-250', 1, 1),  -- Cheapest - preferred for Coffee Scrub
(@NaturalWellness, @Charcoal, 140.00, 'NW-CHAR-100', 1, 1),  -- Only supplier for Charcoal
(@NaturalWellness, @Aloe, 175.00, 'NW-ALOE-300', 0, 1);      -- More expensive than BeautyMax

-- Verify the data
SELECT 
    s.name AS Supplier,
    p.name AS Product,
    sp.supplier_price AS SupplierPrice,
    p.cost_price AS DefaultCost,
    sp.supplier_sku AS SupplierSKU,
    CASE WHEN sp.is_preferred = 1 THEN 'YES' ELSE 'no' END AS Preferred
FROM SupplierProduct sp
JOIN Supplier s ON sp.supplier_id = s.supplier_id
JOIN Product p ON sp.product_id = p.product_id
ORDER BY p.name, sp.supplier_price;

PRINT '';
PRINT '=== TEST SCENARIO ===';
PRINT 'When you create a Purchase Order:';
PRINT '1. Select "BeautyMax Supplies" -> You should see: Lavender Oil (₱280), Coconut Oil (₱220), Aloe Lotion (₱160)';
PRINT '2. Select "Spa Essentials PH" -> You should see: Lavender Oil (₱295), Coconut Oil (₱210), Towels, Coffee Scrub';
PRINT '3. Select "Natural Wellness Co." -> You should see: Coffee Scrub (₱165), Charcoal Mask, Aloe Lotion (₱175)';
PRINT '';
PRINT 'Notice: Lavender Oil has DIFFERENT prices from different suppliers!';
PRINT '  - BeautyMax: ₱280 (preferred)';
PRINT '  - Spa Essentials: ₱295';
