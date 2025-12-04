-- Seed Services with Commission Rates
-- Run on local database (spa_erp)

-- Clear existing services if any
-- DELETE FROM [Service];

-- Insert Services with Commission Data
-- Category 1: Massage
INSERT INTO [Service] (service_category_id, code, name, description, price, duration_minutes, active, commission_type, commission_value, sync_id, sync_status, sync_version)
VALUES 
(1, 'SWE-60', 'Swedish Massage (60 min)', 'Classic relaxation massage using long flowing strokes', 800.00, 60, 1, 'percentage', 30, NEWID(), 'pending', 1),
(1, 'SWE-90', 'Swedish Massage (90 min)', 'Extended relaxation massage for full body wellness', 1200.00, 90, 1, 'percentage', 30, NEWID(), 'pending', 1),
(1, 'DTM-60', 'Deep Tissue Massage (60 min)', 'Intensive massage targeting deep muscle layers', 1000.00, 60, 1, 'percentage', 35, NEWID(), 'pending', 1),
(1, 'DTM-90', 'Deep Tissue Massage (90 min)', 'Extended deep tissue work for chronic tension', 1500.00, 90, 1, 'percentage', 35, NEWID(), 'pending', 1),
(1, 'HOT-60', 'Hot Stone Massage (60 min)', 'Heated basalt stones for deep relaxation', 1200.00, 60, 1, 'percentage', 30, NEWID(), 'pending', 1),
(1, 'ART-60', 'Aromatherapy Massage (60 min)', 'Essential oil massage for mind-body balance', 900.00, 60, 1, 'percentage', 30, NEWID(), 'pending', 1),
(1, 'PRE-60', 'Prenatal Massage (60 min)', 'Gentle massage designed for expecting mothers', 900.00, 60, 1, 'percentage', 30, NEWID(), 'pending', 1),
(1, 'REF-30', 'Reflexology (30 min)', 'Foot massage targeting pressure points', 500.00, 30, 1, 'percentage', 25, NEWID(), 'pending', 1),
(1, 'HNS-30', 'Head, Neck & Shoulders (30 min)', 'Focused massage for upper body tension', 500.00, 30, 1, 'percentage', 25, NEWID(), 'pending', 1);

-- Category 2: Facial
INSERT INTO [Service] (service_category_id, code, name, description, price, duration_minutes, active, commission_type, commission_value, sync_id, sync_status, sync_version)
VALUES 
(2, 'FAC-BS', 'Basic Facial', 'Deep cleansing facial with extraction', 600.00, 45, 1, 'percentage', 25, NEWID(), 'pending', 1),
(2, 'FAC-HY', 'Hydrating Facial', 'Intensive moisture treatment for dry skin', 800.00, 60, 1, 'percentage', 25, NEWID(), 'pending', 1),
(2, 'FAC-AA', 'Anti-Aging Facial', 'Premium treatment with anti-aging serums', 1500.00, 75, 1, 'percentage', 30, NEWID(), 'pending', 1),
(2, 'FAC-AC', 'Acne Treatment Facial', 'Targeted treatment for problem skin', 900.00, 60, 1, 'percentage', 25, NEWID(), 'pending', 1),
(2, 'FAC-GL', 'Glow Facial', 'Brightening facial for radiant skin', 1000.00, 60, 1, 'percentage', 25, NEWID(), 'pending', 1),
(2, 'FAC-DX', 'Diamond Peel Facial', 'Microdermabrasion for skin renewal', 1200.00, 60, 1, 'percentage', 30, NEWID(), 'pending', 1);

-- Category 3: Body Treatment
INSERT INTO [Service] (service_category_id, code, name, description, price, duration_minutes, active, commission_type, commission_value, sync_id, sync_status, sync_version)
VALUES 
(3, 'BOD-SC', 'Body Scrub', 'Full body exfoliation treatment', 800.00, 45, 1, 'percentage', 25, NEWID(), 'pending', 1),
(3, 'BOD-WR', 'Body Wrap', 'Detoxifying seaweed or mud wrap', 1200.00, 60, 1, 'percentage', 25, NEWID(), 'pending', 1),
(3, 'BOD-SL', 'Slimming Treatment', 'Contouring treatment for problem areas', 1500.00, 90, 1, 'percentage', 30, NEWID(), 'pending', 1),
(3, 'BOD-BT', 'Back Treatment', 'Deep cleansing and treatment for back', 700.00, 45, 1, 'percentage', 25, NEWID(), 'pending', 1);

-- Category 4: Nail Care
INSERT INTO [Service] (service_category_id, code, name, description, price, duration_minutes, active, commission_type, commission_value, sync_id, sync_status, sync_version)
VALUES 
(4, 'NAI-MC', 'Classic Manicure', 'Basic nail care and polish', 250.00, 30, 1, 'fixed', 50.00, NEWID(), 'pending', 1),
(4, 'NAI-PC', 'Classic Pedicure', 'Basic foot care and polish', 350.00, 45, 1, 'fixed', 70.00, NEWID(), 'pending', 1),
(4, 'NAI-MG', 'Gel Manicure', 'Long-lasting gel polish manicure', 450.00, 45, 1, 'fixed', 90.00, NEWID(), 'pending', 1),
(4, 'NAI-PG', 'Gel Pedicure', 'Long-lasting gel polish pedicure', 550.00, 60, 1, 'fixed', 100.00, NEWID(), 'pending', 1),
(4, 'NAI-MS', 'Spa Manicure', 'Premium manicure with massage and mask', 400.00, 45, 1, 'fixed', 80.00, NEWID(), 'pending', 1),
(4, 'NAI-PS', 'Spa Pedicure', 'Premium pedicure with massage and mask', 500.00, 60, 1, 'fixed', 100.00, NEWID(), 'pending', 1),
(4, 'NAI-NA', 'Nail Art (per nail)', 'Custom nail art design', 50.00, 10, 1, 'fixed', 20.00, NEWID(), 'pending', 1);

-- Category 5: Hair Care
INSERT INTO [Service] (service_category_id, code, name, description, price, duration_minutes, active, commission_type, commission_value, sync_id, sync_status, sync_version)
VALUES 
(5, 'HAI-TR', 'Hair Treatment', 'Deep conditioning and repair treatment', 600.00, 45, 1, 'percentage', 25, NEWID(), 'pending', 1),
(5, 'HAI-SC', 'Scalp Treatment', 'Intensive scalp massage and treatment', 500.00, 30, 1, 'percentage', 25, NEWID(), 'pending', 1),
(5, 'HAI-KE', 'Keratin Treatment', 'Smoothing keratin treatment', 2500.00, 120, 1, 'percentage', 20, NEWID(), 'pending', 1),
(5, 'HAI-HS', 'Hair Spa', 'Complete hair and scalp wellness treatment', 800.00, 60, 1, 'percentage', 25, NEWID(), 'pending', 1);

-- Verify
SELECT service_id, code, name, price, commission_type, commission_value,
       CASE 
           WHEN commission_type = 'percentage' THEN price * commission_value / 100
           ELSE commission_value
       END AS therapist_earns
FROM [Service]
ORDER BY service_category_id, name;
GO
