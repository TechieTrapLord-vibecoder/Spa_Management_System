-- Fix LedgerAccount names and remove unused duplicates
-- The AccountingService.cs uses these account codes - names should match

BEGIN TRANSACTION;

-- Step 1: Fix account names to match what the code expects
-- Account 1000 should be "Cash on Hand" - OK already
-- Account 1100 should be "Accounts Receivable" - OK already (but code uses "CASH = 1100" which is wrong!)
-- Account 1200 is "Inventory" but code expects "Accounts Receivable" 
-- Account 1300 is "Inventory" - correct per code

-- The code has a bug: CASH = "1100" but 1100 is Accounts Receivable
-- Actually looking at the DB: 1000 = Cash on Hand, 1100 = Accounts Receivable
-- The code should use 1000 for CASH, not 1100

-- Fix account 5100: DB has "Commission Expense" but code expects "Rent Expense"
UPDATE LedgerAccount SET name = 'Rent Expense' WHERE code = '5100';
PRINT 'Updated 5100 to Rent Expense';

-- Fix account 5500: DB has "Cost of Goods Sold" but code expects "Marketing Expense"  
UPDATE LedgerAccount SET name = 'Marketing Expense' WHERE code = '5500';
PRINT 'Updated 5500 to Marketing Expense';

-- Step 2: Handle the entries in 5100 (Commission Expense -> Rent Expense)
-- There are 1302 entries in 5100 that are actually commission entries, need to move to 5600
UPDATE JournalEntryLine SET ledger_account_id = 10053 WHERE ledger_account_id = 10046;
PRINT 'Moved 5100 entries to 5600 (Commission Expense)';

-- Step 3: Delete the duplicate/unused accounts
-- 1200 (Inventory duplicate - code expects A/R here, but we have 1100 for that)
-- 5500 is now Marketing Expense, keep it
-- 5100 is now Rent Expense, keep it

-- Check for any remaining entries in 1200 before deleting
DECLARE @Count1200 INT;
SELECT @Count1200 = COUNT(*) FROM JournalEntryLine WHERE ledger_account_id = 10035;
IF @Count1200 > 0
BEGIN
    PRINT 'WARNING: Account 1200 has ' + CAST(@Count1200 AS VARCHAR) + ' entries, moving to 1300';
    UPDATE JournalEntryLine SET ledger_account_id = 10066 WHERE ledger_account_id = 10035;
END

-- Now delete the duplicate 1200 (named Inventory but should be A/R)
DELETE FROM LedgerAccount WHERE ledger_account_id = 10035;
PRINT 'Deleted duplicate account 1200 (was wrongly named Inventory)';

-- Create proper 1200 Accounts Receivable since we deleted it
INSERT INTO LedgerAccount (code, name, account_type, normal_balance, sync_id, sync_status, sync_version)
VALUES ('1200', 'Accounts Receivable', 'asset', 'debit', NEWID(), 'synced', 1);
PRINT 'Created proper 1200 Accounts Receivable';

COMMIT;

-- Verify final state
PRINT '';
PRINT '=== FINAL ACCOUNT LIST ===';
SELECT la.ledger_account_id, la.code, la.name, 
       (SELECT COUNT(*) FROM JournalEntryLine jel WHERE jel.ledger_account_id = la.ledger_account_id) as entries
FROM LedgerAccount la 
ORDER BY la.code;
