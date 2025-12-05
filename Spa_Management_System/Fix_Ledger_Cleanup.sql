-- Comprehensive Ledger Account Cleanup
-- Fix account names, consolidate duplicates, remove empty duplicates

BEGIN TRANSACTION;

-- ========================================
-- STEP 1: Fix expense account names to proper chart of accounts
-- ========================================

-- 5100 has 1302 commission entries - rename to "Commission Expense" (if not already)
-- Actually it IS Commission Expense, so keep it

-- 5200 should be Rent Expense - already correct
-- 5300 should be Utilities Expense - already correct  
-- 5400 should be Supplies Expense - already correct

-- 5500 is "Cost of Goods Sold" with 0 entries - rename to Marketing Expense
UPDATE LedgerAccount SET name = 'Marketing Expense' WHERE code = '5500';
PRINT 'Renamed 5500 to Marketing Expense';

-- 5600 is duplicate "Commission Expense" with 1 entry - need to merge into 5100
-- Move entries from 5600 to 5100
DECLARE @Id5100 BIGINT, @Id5600 BIGINT;
SELECT @Id5100 = ledger_account_id FROM LedgerAccount WHERE code = '5100';
SELECT @Id5600 = ledger_account_id FROM LedgerAccount WHERE code = '5600';

UPDATE JournalEntryLine SET ledger_account_id = @Id5100 WHERE ledger_account_id = @Id5600;
PRINT 'Moved entries from 5600 to 5100 (Commission Expense)';

-- Delete 5600 duplicate
DELETE FROM LedgerAccount WHERE code = '5600';
PRINT 'Deleted duplicate 5600 (Commission Expense)';

-- 5700 is "Cost of Goods Sold" with 5 entries - keep this as COGS
-- 5500 is now Marketing (0 entries) - keep it

-- ========================================
-- STEP 2: Handle Inventory duplicates (1200 and 1300)
-- ========================================
-- 1200 is "Inventory" with 1 entry - should NOT exist (1200 should be A/R in proper COA)
-- 1300 is "Inventory" with 5 entries - this is the correct one per code

DECLARE @Id1200 BIGINT, @Id1300 BIGINT;
SELECT @Id1200 = ledger_account_id FROM LedgerAccount WHERE code = '1200';
SELECT @Id1300 = ledger_account_id FROM LedgerAccount WHERE code = '1300';

-- Move 1200 entries to 1300
UPDATE JournalEntryLine SET ledger_account_id = @Id1300 WHERE ledger_account_id = @Id1200;
PRINT 'Moved entries from 1200 to 1300 (Inventory)';

-- Delete 1200
DELETE FROM LedgerAccount WHERE code = '1200';
PRINT 'Deleted duplicate 1200 (Inventory)';

-- ========================================
-- STEP 3: Update the AccountingService code mapping comments
-- The code now uses:
-- SALARY_EXPENSE = 5000 but DB has "Salaries & Wages"
-- COMMISSION_EXPENSE = 5600 but we deleted 5600, should use 5100
-- ========================================

-- Actually, let me re-think this. The code I updated uses:
-- COMMISSION_EXPENSE = "5600" - but we just deleted 5600!
-- Need to either recreate 5600 or change code to use 5100

-- Since 5100 has all the commission data (1302 entries), let's keep that
-- and update the code to use 5100 instead

PRINT 'NOTE: Update AccountingService.cs COMMISSION_EXPENSE from "5600" to "5100"';

COMMIT;

-- ========================================
-- FINAL VERIFICATION
-- ========================================
PRINT '';
PRINT '=== FINAL ACCOUNT LIST ===';
SELECT la.code, la.name, 
       (SELECT COUNT(*) FROM JournalEntryLine jel WHERE jel.ledger_account_id = la.ledger_account_id) as entries
FROM LedgerAccount la 
ORDER BY la.code;
