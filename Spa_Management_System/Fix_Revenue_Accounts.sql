-- Fix Historical Journal Entry Revenue Account Misallocations
-- The AccountingService.cs had incorrect account code mappings:
-- SERVICE_REVENUE was "4100" but should be "4000"
-- PRODUCT_REVENUE was "4200" but should be "4100"

-- Account IDs in database:
-- 10042 = 4000 (Service Revenue) - CORRECT for services
-- 10043 = 4100 (Product Sales) - CORRECT for products
-- 10044 = 4200 (Other Income) - Should NOT have product revenue

BEGIN TRANSACTION;

-- Step 1: Move "Service revenue" entries that were incorrectly posted to 4100 -> 4000
-- These 2 entries (₱2,600) with memo "Service revenue" are actually service sales
UPDATE JournalEntryLine 
SET ledger_account_id = 10042  -- Move to 4000 Service Revenue
WHERE ledger_account_id = 10043  -- Currently in 4100 Product Sales
  AND line_memo = 'Service revenue';

PRINT 'Moved service revenue from 4100 to 4000';

-- Step 2: Move "Product sales revenue" entries from 4200 (Other Income) -> 4100 (Product Sales)
-- These 4 entries (₱20,535) are actual product sales that went to wrong account
UPDATE JournalEntryLine 
SET ledger_account_id = 10043  -- Move to 4100 Product Sales
WHERE ledger_account_id = 10044  -- Currently in 4200 Other Income
  AND line_memo = 'Product sales revenue';

PRINT 'Moved product revenue from 4200 to 4100';

COMMIT;

-- Verify the fix
SELECT 
    la.code,
    la.name,
    COUNT(*) as entry_count,
    SUM(jel.credit) as total_credit
FROM JournalEntryLine jel
JOIN LedgerAccount la ON jel.ledger_account_id = la.ledger_account_id
WHERE la.code IN ('4000', '4100', '4200')
GROUP BY la.code, la.name
ORDER BY la.code;
