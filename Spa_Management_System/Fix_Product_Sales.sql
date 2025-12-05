-- Fix Product Sales entries to correct account
UPDATE JournalEntryLine 
SET ledger_account_id = 10043 
WHERE line_memo = 'Product sales revenue';

-- Verify
SELECT la.code, la.name, SUM(jel.credit) as total
FROM JournalEntryLine jel
JOIN LedgerAccount la ON jel.ledger_account_id = la.ledger_account_id
WHERE la.code IN ('4000', '4100', '4200')
GROUP BY la.code, la.name
ORDER BY la.code;
