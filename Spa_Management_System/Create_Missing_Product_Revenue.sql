-- Create Missing Product Sales Revenue Journal Entries
-- Problem: Historical product sales (₱217,185) were not fully journaled (only ₱20,535)
-- Missing: ₱196,650 in product revenue entries

-- First, let's verify the missing amount
DECLARE @TotalProductSales DECIMAL(18,2);
DECLARE @JournaledProductSales DECIMAL(18,2);
DECLARE @MissingAmount DECIMAL(18,2);

SELECT @TotalProductSales = SUM(line_total) FROM SaleItem WHERE item_type = 'product';
SELECT @JournaledProductSales = ISNULL(SUM(credit), 0) FROM JournalEntryLine WHERE ledger_account_id = 10043;
SET @MissingAmount = @TotalProductSales - @JournaledProductSales;

PRINT 'Total Product Sales in SaleItem: ' + CAST(@TotalProductSales AS VARCHAR);
PRINT 'Already Journaled Product Sales: ' + CAST(@JournaledProductSales AS VARCHAR);
PRINT 'Missing Amount to Journal: ' + CAST(@MissingAmount AS VARCHAR);

-- Only proceed if there's a missing amount
IF @MissingAmount > 0
BEGIN
    -- Create Journal Entry Header
    DECLARE @JournalId BIGINT;
    DECLARE @JournalNo NVARCHAR(50) = 'JE-ADJ-' + FORMAT(GETDATE(), 'yyyyMMdd-HHmmss');
    
    INSERT INTO JournalEntry (journal_no, date, description, created_at, sync_id, sync_status, sync_version)
    VALUES (
        @JournalNo,
        GETDATE(),
        'Adjustment: Missing Product Sales Revenue - Historical data correction',
        GETDATE(),
        NEWID(),
        'synced',
        1
    );
    
    SET @JournalId = SCOPE_IDENTITY();
    PRINT 'Created Journal Entry ID: ' + CAST(@JournalId AS VARCHAR);
    
    -- Debit: Retained Earnings (3100) - since these sales already happened and cash was received
    -- This assumes the cash was already received but not properly recorded to revenue
    -- Using Retained Earnings as the balancing account for historical adjustments
    
    -- Get Retained Earnings account ID
    DECLARE @RetainedEarningsId BIGINT;
    SELECT @RetainedEarningsId = ledger_account_id FROM LedgerAccount WHERE code = '3100';
    
    IF @RetainedEarningsId IS NULL
    BEGIN
        -- Create Retained Earnings account if it doesn't exist
        INSERT INTO LedgerAccount (code, name, account_type, normal_balance, sync_id, sync_status, sync_version)
        VALUES ('3100', 'Retained Earnings', 'equity', 'credit', NEWID(), 'synced', 1);
        SET @RetainedEarningsId = SCOPE_IDENTITY();
        PRINT 'Created Retained Earnings account ID: ' + CAST(@RetainedEarningsId AS VARCHAR);
    END
    
    -- Create journal entry lines
    -- Debit: Retained Earnings (reduces equity to balance the revenue increase)
    INSERT INTO JournalEntryLine (journal_id, ledger_account_id, debit, credit, line_memo, sync_id, sync_status, sync_version)
    VALUES (
        @JournalId,
        @RetainedEarningsId,
        @MissingAmount,
        0,
        'Historical adjustment - Product sales revenue correction',
        NEWID(),
        'synced',
        1
    );
    
    -- Credit: Product Sales (4100 = 10043) - recognize the missing revenue
    INSERT INTO JournalEntryLine (journal_id, ledger_account_id, debit, credit, line_memo, sync_id, sync_status, sync_version)
    VALUES (
        @JournalId,
        10043,  -- Product Sales (4100)
        0,
        @MissingAmount,
        'Product sales revenue',
        NEWID(),
        'synced',
        1
    );
    
    PRINT 'Created adjustment journal entry for: ₱' + CAST(@MissingAmount AS VARCHAR);
END
ELSE
BEGIN
    PRINT 'No adjustment needed - amounts are balanced';
END

-- Verify final state
PRINT '';
PRINT '=== FINAL VERIFICATION ===';
SELECT 
    la.code,
    la.name,
    SUM(jel.credit) as total_credit
FROM JournalEntryLine jel
JOIN LedgerAccount la ON jel.ledger_account_id = la.ledger_account_id
WHERE la.code IN ('4000', '4100', '4200')
GROUP BY la.code, la.name
ORDER BY la.code;
