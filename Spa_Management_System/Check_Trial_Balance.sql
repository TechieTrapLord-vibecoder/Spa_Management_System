-- Check Trial Balance data
SELECT 
    la.code,
    la.name,
    la.normal_balance,
    ISNULL(SUM(jel.debit), 0) as total_debit,
    ISNULL(SUM(jel.credit), 0) as total_credit,
    CASE 
        WHEN la.normal_balance = 'debit' THEN ISNULL(SUM(jel.debit), 0) - ISNULL(SUM(jel.credit), 0)
        ELSE ISNULL(SUM(jel.credit), 0) - ISNULL(SUM(jel.debit), 0)
    END as balance
FROM LedgerAccount la
LEFT JOIN JournalEntryLine jel ON la.ledger_account_id = jel.ledger_account_id
GROUP BY la.code, la.name, la.normal_balance
ORDER BY la.code;
