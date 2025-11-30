-- =====================================================
-- Database Phase 3: Payroll & Expenses
-- Run this script after the base database is created
-- =====================================================

USE spa_erp;
GO

-- =====================================================
-- Drop EmployeeAttendance table if it exists (not needed)
-- =====================================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeAttendance')
BEGIN
    DROP TABLE EmployeeAttendance;
    PRINT 'Dropped EmployeeAttendance table (not needed)';
END
GO

-- =====================================================
-- 1. Simple Payroll Table (Days × Rate = Pay)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Payroll')
BEGIN
    CREATE TABLE Payroll (
        payroll_id BIGINT IDENTITY(1,1) PRIMARY KEY,
        employee_id BIGINT NOT NULL,
        period_start DATE NOT NULL,
        period_end DATE NOT NULL,
        days_worked INT NOT NULL DEFAULT 0,
        daily_rate DECIMAL(10,2) NOT NULL DEFAULT 500,
        gross_pay DECIMAL(12,2) NOT NULL DEFAULT 0,     -- days_worked × daily_rate
        deductions DECIMAL(12,2) NOT NULL DEFAULT 0,    -- SSS, PhilHealth, etc.
        net_pay DECIMAL(12,2) NOT NULL DEFAULT 0,       -- gross_pay - deductions
        status VARCHAR(20) NOT NULL DEFAULT 'draft',    -- draft, paid
        paid_at DATETIME NULL,
        journal_id BIGINT NULL,                         -- Link to journal entry when paid
        notes NVARCHAR(500) NULL,
        created_by_user_id BIGINT NULL,
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME NULL,
        
        CONSTRAINT FK_Payroll_Employee FOREIGN KEY (employee_id) REFERENCES Employee(employee_id),
        CONSTRAINT FK_Payroll_CreatedBy FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id),
        CONSTRAINT FK_Payroll_Journal FOREIGN KEY (journal_id) REFERENCES JournalEntry(journal_id)
    );

    CREATE INDEX IX_Payroll_Employee ON Payroll(employee_id);
    CREATE INDEX IX_Payroll_Period ON Payroll(period_start, period_end);
    CREATE INDEX IX_Payroll_Status ON Payroll(status);
    
    PRINT 'Created Payroll table';
END
GO

-- Add journal_id column if table already exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Payroll')
   AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Payroll') AND name = 'journal_id')
BEGIN
    ALTER TABLE Payroll ADD journal_id BIGINT NULL;
    ALTER TABLE Payroll ADD CONSTRAINT FK_Payroll_Journal FOREIGN KEY (journal_id) REFERENCES JournalEntry(journal_id);
    PRINT 'Added journal_id column to Payroll table';
END
GO

-- =====================================================
-- 3. Expense Table (for expense tracking)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Expense')
BEGIN
    CREATE TABLE Expense (
        expense_id BIGINT IDENTITY(1,1) PRIMARY KEY,
        expense_date DATE NOT NULL,
        category VARCHAR(50) NOT NULL,              -- Utilities, Supplies, Rent, etc.
        description NVARCHAR(500) NOT NULL,
        amount DECIMAL(12,2) NOT NULL,
        vendor NVARCHAR(200) NULL,
        reference_number VARCHAR(100) NULL,         -- Receipt/Invoice number
        payment_method VARCHAR(30) NULL,            -- Cash, Card, Bank Transfer
        status VARCHAR(20) NOT NULL DEFAULT 'pending', -- pending, paid, cancelled
        ledger_account_id BIGINT NULL,
        journal_id BIGINT NULL,
        notes NVARCHAR(500) NULL,
        created_by_user_id BIGINT NULL,
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME NULL,
        
        CONSTRAINT FK_Expense_LedgerAccount FOREIGN KEY (ledger_account_id) REFERENCES LedgerAccount(ledger_account_id),
        CONSTRAINT FK_Expense_Journal FOREIGN KEY (journal_id) REFERENCES JournalEntry(journal_id),
        CONSTRAINT FK_Expense_CreatedBy FOREIGN KEY (created_by_user_id) REFERENCES UserAccount(user_id)
    );

    CREATE INDEX IX_Expense_Date ON Expense(expense_date);
    CREATE INDEX IX_Expense_Category ON Expense(category);
    CREATE INDEX IX_Expense_Status ON Expense(status);
    
    PRINT 'Created Expense table';
END
GO

PRINT '';
PRINT '=== Phase 3 Database Setup Complete ===';
PRINT 'Tables created: Payroll, Expense';
GO
