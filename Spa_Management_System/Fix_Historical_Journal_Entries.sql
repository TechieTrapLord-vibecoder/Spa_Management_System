-- Fix Missing Journal Entries with CORRECT HISTORICAL DATES
-- Each expense/payroll will be recorded on its actual date

BEGIN TRANSACTION;

DECLARE @journal_id INT;
DECLARE @now DATETIME = GETDATE();

-- =====================================================
-- EXPENSES - Each one on its actual expense_date
-- =====================================================

-- Get ledger account IDs for reference:
-- 10032 = Cash on Hand (1000)
-- 10045 = Salaries & Wages (5000)
-- 10047 = Rent Expense (5200)
-- 10048 = Utilities Expense (5300)
-- 10049 = Supplies Expense (5400)
-- 10055 = Maintenance Expense (5750)
-- 10056 = Insurance Expense (5800)
-- 10058 = Equipment Expense (6000)
-- 10059 = Professional Services (6100)
-- 10060 = Taxes & Licenses (6200)
-- 10061 = Miscellaneous Expense (6900)

-- Loop through each expense and create a journal entry
DECLARE @expense_id INT, @expense_date DATE, @category NVARCHAR(100), @amount DECIMAL(18,2), @vendor NVARCHAR(200);
DECLARE @ledger_id INT, @journal_no NVARCHAR(50);

DECLARE expense_cursor CURSOR FOR
SELECT expense_id, expense_date, category, amount, vendor FROM Expense WHERE status = 'Paid' ORDER BY expense_date;

OPEN expense_cursor;
FETCH NEXT FROM expense_cursor INTO @expense_id, @expense_date, @category, @amount, @vendor;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Determine ledger account based on category
    SET @ledger_id = CASE @category
        WHEN 'Rent' THEN 10047
        WHEN 'Utilities' THEN 10048
        WHEN 'Supplies' THEN 10049
        WHEN 'Maintenance' THEN 10055
        WHEN 'Insurance' THEN 10056
        WHEN 'Equipment' THEN 10058
        WHEN 'Professional Services' THEN 10059
        WHEN 'Taxes' THEN 10060
        WHEN 'Miscellaneous' THEN 10061
        ELSE 10061  -- Default to Miscellaneous
    END;

    SET @journal_no = 'JE-EXP-' + CAST(@expense_id AS NVARCHAR(10));

    -- Create journal entry with actual expense date
    INSERT INTO JournalEntry (journal_no, date, description, created_by_user_id, created_at, last_modified_at, sync_id, sync_status, sync_version)
    VALUES (@journal_no, @expense_date, @category + ' Payment - ' + ISNULL(@vendor, 'N/A'), 10, @now, @now, NEWID(), 'synced', 1);

    SET @journal_id = SCOPE_IDENTITY();

    -- Debit expense account, Credit cash
    INSERT INTO JournalEntryLine (journal_id, ledger_account_id, debit, credit, line_memo, sync_id, last_modified_at, sync_status, sync_version)
    VALUES 
    (@journal_id, @ledger_id, @amount, 0, @category + ' Expense', NEWID(), @now, 'synced', 1),
    (@journal_id, 10032, 0, @amount, 'Cash paid for ' + @category, NEWID(), @now, 'synced', 1);

    -- Update expense record with journal_id
    UPDATE Expense SET journal_id = @journal_id WHERE expense_id = @expense_id;

    FETCH NEXT FROM expense_cursor INTO @expense_id, @expense_date, @category, @amount, @vendor;
END

CLOSE expense_cursor;
DEALLOCATE expense_cursor;

PRINT 'Expense journal entries created with historical dates.';

-- =====================================================
-- PAYROLL - Each payroll on its period_end date
-- =====================================================

DECLARE @payroll_id INT, @period_end DATE, @net_pay DECIMAL(18,2), @employee_id INT;

DECLARE payroll_cursor CURSOR FOR
SELECT payroll_id, period_end, net_pay, employee_id FROM Payroll WHERE status = 'paid' ORDER BY period_end;

OPEN payroll_cursor;
FETCH NEXT FROM payroll_cursor INTO @payroll_id, @period_end, @net_pay, @employee_id;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @journal_no = 'JE-PAY-' + CAST(@payroll_id AS NVARCHAR(10));

    -- Create journal entry with payroll period end date
    INSERT INTO JournalEntry (journal_no, date, description, created_by_user_id, created_at, last_modified_at, sync_id, sync_status, sync_version)
    VALUES (@journal_no, @period_end, 'Payroll Payment - Employee ' + CAST(@employee_id AS NVARCHAR(10)), 10, @now, @now, NEWID(), 'synced', 1);

    SET @journal_id = SCOPE_IDENTITY();

    -- Debit salaries expense, Credit cash
    INSERT INTO JournalEntryLine (journal_id, ledger_account_id, debit, credit, line_memo, sync_id, last_modified_at, sync_status, sync_version)
    VALUES 
    (@journal_id, 10045, @net_pay, 0, 'Salaries & Wages', NEWID(), @now, 'synced', 1),
    (@journal_id, 10032, 0, @net_pay, 'Cash paid for payroll', NEWID(), @now, 'synced', 1);

    -- Update payroll record with journal_id
    UPDATE Payroll SET journal_id = @journal_id WHERE payroll_id = @payroll_id;

    FETCH NEXT FROM payroll_cursor INTO @payroll_id, @period_end, @net_pay, @employee_id;
END

CLOSE payroll_cursor;
DEALLOCATE payroll_cursor;

PRINT 'Payroll journal entries created with historical dates.';

-- =====================================================
-- COMMISSION PAYMENTS - Spread across months based on sales
-- We'll create monthly commission payment entries
-- =====================================================

-- Get commission totals by month from existing journal entries
DECLARE @month_start DATE, @month_end DATE, @commission_total DECIMAL(18,2);

-- January through November - pay commissions at end of each month
DECLARE @m INT = 1;
WHILE @m <= 11
BEGIN
    SET @month_start = DATEFROMPARTS(2025, @m, 1);
    SET @month_end = EOMONTH(@month_start);

    -- Get total commissions accrued in this month
    SELECT @commission_total = ISNULL(SUM(jel.credit), 0)
    FROM JournalEntryLine jel
    JOIN JournalEntry je ON jel.journal_id = je.journal_id
    WHERE jel.ledger_account_id = 10038  -- Accrued Expenses
    AND je.date >= @month_start AND je.date <= @month_end;

    IF @commission_total > 0
    BEGIN
        SET @journal_no = 'JE-COMM-' + CAST(@m AS NVARCHAR(2));

        -- Create journal entry on last day of month
        INSERT INTO JournalEntry (journal_no, date, description, created_by_user_id, created_at, last_modified_at, sync_id, sync_status, sync_version)
        VALUES (@journal_no, @month_end, 'Therapist Commission Payments - ' + DATENAME(MONTH, @month_start) + ' 2025', 10, @now, @now, NEWID(), 'synced', 1);

        SET @journal_id = SCOPE_IDENTITY();

        -- Debit Accrued Expenses (clear liability), Credit Cash (pay out)
        INSERT INTO JournalEntryLine (journal_id, ledger_account_id, debit, credit, line_memo, sync_id, last_modified_at, sync_status, sync_version)
        VALUES 
        (@journal_id, 10038, @commission_total, 0, 'Pay accrued commissions', NEWID(), @now, 'synced', 1),
        (@journal_id, 10032, 0, @commission_total, 'Cash paid to therapists', NEWID(), @now, 'synced', 1);
    END

    SET @m = @m + 1;
END

-- December commissions (up to today)
SET @month_start = '2025-12-01';
SET @month_end = '2025-12-06';

SELECT @commission_total = ISNULL(SUM(jel.credit), 0)
FROM JournalEntryLine jel
JOIN JournalEntry je ON jel.journal_id = je.journal_id
WHERE jel.ledger_account_id = 10038  -- Accrued Expenses
AND je.date >= @month_start AND je.date <= @month_end
AND je.journal_no NOT LIKE 'JE-COMM-%';

IF @commission_total > 0
BEGIN
    SET @journal_no = 'JE-COMM-12';

    INSERT INTO JournalEntry (journal_no, date, description, created_by_user_id, created_at, last_modified_at, sync_id, sync_status, sync_version)
    VALUES (@journal_no, @month_end, 'Therapist Commission Payments - December 2025', 10, @now, @now, NEWID(), 'synced', 1);

    SET @journal_id = SCOPE_IDENTITY();

    INSERT INTO JournalEntryLine (journal_id, ledger_account_id, debit, credit, line_memo, sync_id, last_modified_at, sync_status, sync_version)
    VALUES 
    (@journal_id, 10038, @commission_total, 0, 'Pay accrued commissions', NEWID(), @now, 'synced', 1),
    (@journal_id, 10032, 0, @commission_total, 'Cash paid to therapists', NEWID(), @now, 'synced', 1);
END

PRINT 'Commission payment entries created with historical dates.';

COMMIT TRANSACTION;

PRINT '';
PRINT 'All journal entries created with proper historical dates!';
