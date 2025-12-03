IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [LedgerAccount] (
        [ledger_account_id] bigint NOT NULL IDENTITY,
        [code] nvarchar(50) NOT NULL,
        [name] nvarchar(200) NOT NULL,
        [account_type] nvarchar(20) NOT NULL,
        [normal_balance] nvarchar(10) NOT NULL,
        CONSTRAINT [PK_LedgerAccount] PRIMARY KEY ([ledger_account_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [Person] (
        [person_id] bigint NOT NULL IDENTITY,
        [first_name] nvarchar(120) NOT NULL,
        [last_name] nvarchar(120) NOT NULL,
        [email] nvarchar(200) NULL,
        [phone] nvarchar(50) NULL,
        [address] nvarchar(max) NULL,
        [dob] datetime2 NULL,
        [created_at] datetime2 NOT NULL,
        CONSTRAINT [PK_Person] PRIMARY KEY ([person_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [Product] (
        [product_id] bigint NOT NULL IDENTITY,
        [sku] nvarchar(80) NULL,
        [name] nvarchar(200) NOT NULL,
        [description] nvarchar(max) NULL,
        [unit_price] decimal(12,2) NOT NULL,
        [cost_price] decimal(12,2) NOT NULL,
        [unit] nvarchar(20) NULL,
        [active] bit NOT NULL,
        CONSTRAINT [PK_Product] PRIMARY KEY ([product_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [Role] (
        [role_id] smallint NOT NULL IDENTITY,
        [name] nvarchar(50) NOT NULL,
        [is_archived] bit NOT NULL,
        CONSTRAINT [PK_Role] PRIMARY KEY ([role_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [ServiceCategory] (
        [service_category_id] int NOT NULL IDENTITY,
        [name] nvarchar(100) NOT NULL,
        [description] nvarchar(max) NULL,
        [is_archived] bit NOT NULL,
        CONSTRAINT [PK_ServiceCategory] PRIMARY KEY ([service_category_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [Supplier] (
        [supplier_id] bigint NOT NULL IDENTITY,
        [name] nvarchar(200) NOT NULL,
        [contact_person] nvarchar(200) NULL,
        [phone] nvarchar(50) NULL,
        [email] nvarchar(150) NULL,
        [address] nvarchar(max) NULL,
        [is_archived] bit NOT NULL,
        CONSTRAINT [PK_Supplier] PRIMARY KEY ([supplier_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [Customer] (
        [customer_id] bigint NOT NULL IDENTITY,
        [person_id] bigint NOT NULL,
        [customer_code] nvarchar(50) NULL,
        [loyalty_points] int NOT NULL,
        [created_at] datetime2 NOT NULL,
        [is_archived] bit NOT NULL,
        CONSTRAINT [PK_Customer] PRIMARY KEY ([customer_id]),
        CONSTRAINT [FK_Customer_Person_person_id] FOREIGN KEY ([person_id]) REFERENCES [Person] ([person_id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [Inventory] (
        [inventory_id] bigint NOT NULL IDENTITY,
        [product_id] bigint NOT NULL,
        [quantity_on_hand] decimal(12,2) NOT NULL,
        [reorder_level] decimal(12,2) NOT NULL,
        [last_counted_at] datetime2 NULL,
        CONSTRAINT [PK_Inventory] PRIMARY KEY ([inventory_id]),
        CONSTRAINT [FK_Inventory_Product_product_id] FOREIGN KEY ([product_id]) REFERENCES [Product] ([product_id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [Employee] (
        [employee_id] bigint NOT NULL IDENTITY,
        [person_id] bigint NOT NULL,
        [role_id] smallint NOT NULL,
        [hire_date] datetime2 NULL,
        [status] nvarchar(30) NOT NULL,
        [note] nvarchar(max) NULL,
        [created_at] datetime2 NOT NULL,
        CONSTRAINT [PK_Employee] PRIMARY KEY ([employee_id]),
        CONSTRAINT [FK_Employee_Person_person_id] FOREIGN KEY ([person_id]) REFERENCES [Person] ([person_id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Employee_Role_role_id] FOREIGN KEY ([role_id]) REFERENCES [Role] ([role_id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [Service] (
        [service_id] bigint NOT NULL IDENTITY,
        [service_category_id] int NULL,
        [code] nvarchar(60) NULL,
        [name] nvarchar(150) NOT NULL,
        [description] nvarchar(max) NULL,
        [price] decimal(12,2) NOT NULL,
        [duration_minutes] int NOT NULL,
        [active] bit NOT NULL,
        CONSTRAINT [PK_Service] PRIMARY KEY ([service_id]),
        CONSTRAINT [FK_Service_ServiceCategory_service_category_id] FOREIGN KEY ([service_category_id]) REFERENCES [ServiceCategory] ([service_category_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [UserAccount] (
        [user_id] bigint NOT NULL IDENTITY,
        [employee_id] bigint NULL,
        [username] nvarchar(100) NOT NULL,
        [password_hash] nvarchar(255) NOT NULL,
        [is_active] bit NOT NULL,
        [last_login] datetime2 NULL,
        [created_at] datetime2 NOT NULL,
        CONSTRAINT [PK_UserAccount] PRIMARY KEY ([user_id]),
        CONSTRAINT [FK_UserAccount_Employee_employee_id] FOREIGN KEY ([employee_id]) REFERENCES [Employee] ([employee_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [EmployeeServiceCommission] (
        [commission_id] bigint NOT NULL IDENTITY,
        [employee_id] bigint NOT NULL,
        [service_id] bigint NOT NULL,
        [commission_type] nvarchar(10) NOT NULL,
        [commission_value] decimal(10,2) NOT NULL,
        [effective_from] datetime2 NULL,
        [effective_to] datetime2 NULL,
        [is_archived] bit NOT NULL,
        CONSTRAINT [PK_EmployeeServiceCommission] PRIMARY KEY ([commission_id]),
        CONSTRAINT [FK_EmployeeServiceCommission_Employee_employee_id] FOREIGN KEY ([employee_id]) REFERENCES [Employee] ([employee_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_EmployeeServiceCommission_Service_service_id] FOREIGN KEY ([service_id]) REFERENCES [Service] ([service_id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [Appointment] (
        [appointment_id] bigint NOT NULL IDENTITY,
        [customer_id] bigint NOT NULL,
        [scheduled_start] datetime2 NOT NULL,
        [scheduled_end] datetime2 NULL,
        [status] nvarchar(40) NOT NULL,
        [created_by_user_id] bigint NULL,
        [created_at] datetime2 NOT NULL,
        [notes] nvarchar(max) NULL,
        CONSTRAINT [PK_Appointment] PRIMARY KEY ([appointment_id]),
        CONSTRAINT [FK_Appointment_Customer_customer_id] FOREIGN KEY ([customer_id]) REFERENCES [Customer] ([customer_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Appointment_UserAccount_created_by_user_id] FOREIGN KEY ([created_by_user_id]) REFERENCES [UserAccount] ([user_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [AuditLog] (
        [audit_id] bigint NOT NULL IDENTITY,
        [entity_name] nvarchar(120) NULL,
        [entity_id] nvarchar(80) NULL,
        [action] nvarchar(10) NULL,
        [changed_by_user_id] bigint NULL,
        [change_summary] nvarchar(max) NULL,
        [created_at] datetime2 NOT NULL,
        CONSTRAINT [PK_AuditLog] PRIMARY KEY ([audit_id]),
        CONSTRAINT [FK_AuditLog_UserAccount_changed_by_user_id] FOREIGN KEY ([changed_by_user_id]) REFERENCES [UserAccount] ([user_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [CRM_Note] (
        [note_id] bigint NOT NULL IDENTITY,
        [customer_id] bigint NOT NULL,
        [created_by_user_id] bigint NOT NULL,
        [note] nvarchar(max) NULL,
        [created_at] datetime2 NOT NULL,
        CONSTRAINT [PK_CRM_Note] PRIMARY KEY ([note_id]),
        CONSTRAINT [FK_CRM_Note_Customer_customer_id] FOREIGN KEY ([customer_id]) REFERENCES [Customer] ([customer_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CRM_Note_UserAccount_created_by_user_id] FOREIGN KEY ([created_by_user_id]) REFERENCES [UserAccount] ([user_id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [EmployeeAttendance] (
        [attendance_id] bigint NOT NULL IDENTITY,
        [employee_id] bigint NOT NULL,
        [work_date] datetime2 NOT NULL,
        [days_worked] decimal(4,1) NOT NULL,
        [notes] nvarchar(max) NULL,
        [created_by_user_id] bigint NULL,
        [created_at] datetime2 NOT NULL,
        [updated_at] datetime2 NULL,
        CONSTRAINT [PK_EmployeeAttendance] PRIMARY KEY ([attendance_id]),
        CONSTRAINT [FK_EmployeeAttendance_Employee_employee_id] FOREIGN KEY ([employee_id]) REFERENCES [Employee] ([employee_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_EmployeeAttendance_UserAccount_created_by_user_id] FOREIGN KEY ([created_by_user_id]) REFERENCES [UserAccount] ([user_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [JournalEntry] (
        [journal_id] bigint NOT NULL IDENTITY,
        [journal_no] nvarchar(80) NULL,
        [date] datetime2 NOT NULL,
        [description] nvarchar(max) NULL,
        [created_by_user_id] bigint NULL,
        [created_at] datetime2 NOT NULL,
        CONSTRAINT [PK_JournalEntry] PRIMARY KEY ([journal_id]),
        CONSTRAINT [FK_JournalEntry_UserAccount_created_by_user_id] FOREIGN KEY ([created_by_user_id]) REFERENCES [UserAccount] ([user_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [PurchaseOrder] (
        [po_id] bigint NOT NULL IDENTITY,
        [supplier_id] bigint NOT NULL,
        [created_by_user_id] bigint NULL,
        [po_number] nvarchar(80) NULL,
        [status] nvarchar(40) NULL,
        [created_at] datetime2 NOT NULL,
        CONSTRAINT [PK_PurchaseOrder] PRIMARY KEY ([po_id]),
        CONSTRAINT [FK_PurchaseOrder_Supplier_supplier_id] FOREIGN KEY ([supplier_id]) REFERENCES [Supplier] ([supplier_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PurchaseOrder_UserAccount_created_by_user_id] FOREIGN KEY ([created_by_user_id]) REFERENCES [UserAccount] ([user_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [Sale] (
        [sale_id] bigint NOT NULL IDENTITY,
        [customer_id] bigint NULL,
        [created_by_user_id] bigint NULL,
        [sale_number] nvarchar(80) NULL,
        [total_amount] decimal(12,2) NOT NULL,
        [payment_status] nvarchar(40) NOT NULL,
        [created_at] datetime2 NOT NULL,
        CONSTRAINT [PK_Sale] PRIMARY KEY ([sale_id]),
        CONSTRAINT [FK_Sale_Customer_customer_id] FOREIGN KEY ([customer_id]) REFERENCES [Customer] ([customer_id]),
        CONSTRAINT [FK_Sale_UserAccount_created_by_user_id] FOREIGN KEY ([created_by_user_id]) REFERENCES [UserAccount] ([user_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [StockTransaction] (
        [stock_tx_id] bigint NOT NULL IDENTITY,
        [product_id] bigint NOT NULL,
        [tx_type] nvarchar(10) NOT NULL,
        [qty] decimal(12,2) NOT NULL,
        [unit_cost] decimal(12,2) NULL,
        [reference] nvarchar(120) NULL,
        [created_by_user_id] bigint NULL,
        [created_at] datetime2 NOT NULL,
        CONSTRAINT [PK_StockTransaction] PRIMARY KEY ([stock_tx_id]),
        CONSTRAINT [FK_StockTransaction_Product_product_id] FOREIGN KEY ([product_id]) REFERENCES [Product] ([product_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_StockTransaction_UserAccount_created_by_user_id] FOREIGN KEY ([created_by_user_id]) REFERENCES [UserAccount] ([user_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [AppointmentService] (
        [appt_srv_id] bigint NOT NULL IDENTITY,
        [appointment_id] bigint NOT NULL,
        [service_id] bigint NOT NULL,
        [therapist_employee_id] bigint NULL,
        [price] decimal(12,2) NOT NULL,
        [commission_amount] decimal(12,2) NOT NULL,
        CONSTRAINT [PK_AppointmentService] PRIMARY KEY ([appt_srv_id]),
        CONSTRAINT [FK_AppointmentService_Appointment_appointment_id] FOREIGN KEY ([appointment_id]) REFERENCES [Appointment] ([appointment_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AppointmentService_Employee_therapist_employee_id] FOREIGN KEY ([therapist_employee_id]) REFERENCES [Employee] ([employee_id]),
        CONSTRAINT [FK_AppointmentService_Service_service_id] FOREIGN KEY ([service_id]) REFERENCES [Service] ([service_id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [Expense] (
        [expense_id] bigint NOT NULL IDENTITY,
        [expense_date] datetime2 NOT NULL,
        [category] nvarchar(100) NOT NULL,
        [description] nvarchar(300) NOT NULL,
        [amount] decimal(12,2) NOT NULL,
        [vendor] nvarchar(100) NULL,
        [reference_number] nvarchar(100) NULL,
        [payment_method] nvarchar(50) NOT NULL,
        [status] nvarchar(30) NOT NULL,
        [notes] nvarchar(max) NULL,
        [ledger_account_id] bigint NULL,
        [journal_id] bigint NULL,
        [created_by_user_id] bigint NULL,
        [created_at] datetime2 NOT NULL,
        [updated_at] datetime2 NULL,
        CONSTRAINT [PK_Expense] PRIMARY KEY ([expense_id]),
        CONSTRAINT [FK_Expense_JournalEntry_journal_id] FOREIGN KEY ([journal_id]) REFERENCES [JournalEntry] ([journal_id]),
        CONSTRAINT [FK_Expense_LedgerAccount_ledger_account_id] FOREIGN KEY ([ledger_account_id]) REFERENCES [LedgerAccount] ([ledger_account_id]),
        CONSTRAINT [FK_Expense_UserAccount_created_by_user_id] FOREIGN KEY ([created_by_user_id]) REFERENCES [UserAccount] ([user_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [JournalEntryLine] (
        [journal_line_id] bigint NOT NULL IDENTITY,
        [journal_id] bigint NOT NULL,
        [ledger_account_id] bigint NOT NULL,
        [debit] decimal(14,2) NOT NULL,
        [credit] decimal(14,2) NOT NULL,
        [line_memo] nvarchar(max) NULL,
        CONSTRAINT [PK_JournalEntryLine] PRIMARY KEY ([journal_line_id]),
        CONSTRAINT [FK_JournalEntryLine_JournalEntry_journal_id] FOREIGN KEY ([journal_id]) REFERENCES [JournalEntry] ([journal_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_JournalEntryLine_LedgerAccount_ledger_account_id] FOREIGN KEY ([ledger_account_id]) REFERENCES [LedgerAccount] ([ledger_account_id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [Payroll] (
        [payroll_id] bigint NOT NULL IDENTITY,
        [employee_id] bigint NOT NULL,
        [period_start] datetime2 NOT NULL,
        [period_end] datetime2 NOT NULL,
        [days_worked] int NOT NULL,
        [daily_rate] decimal(10,2) NOT NULL,
        [gross_pay] decimal(12,2) NOT NULL,
        [deductions] decimal(12,2) NOT NULL,
        [net_pay] decimal(12,2) NOT NULL,
        [status] nvarchar(20) NOT NULL,
        [paid_at] datetime2 NULL,
        [journal_id] bigint NULL,
        [notes] nvarchar(max) NULL,
        [created_by_user_id] bigint NULL,
        [created_at] datetime2 NOT NULL,
        [updated_at] datetime2 NULL,
        CONSTRAINT [PK_Payroll] PRIMARY KEY ([payroll_id]),
        CONSTRAINT [FK_Payroll_Employee_employee_id] FOREIGN KEY ([employee_id]) REFERENCES [Employee] ([employee_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Payroll_JournalEntry_journal_id] FOREIGN KEY ([journal_id]) REFERENCES [JournalEntry] ([journal_id]),
        CONSTRAINT [FK_Payroll_UserAccount_created_by_user_id] FOREIGN KEY ([created_by_user_id]) REFERENCES [UserAccount] ([user_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [PurchaseOrderItem] (
        [po_item_id] bigint NOT NULL IDENTITY,
        [po_id] bigint NOT NULL,
        [product_id] bigint NOT NULL,
        [qty_ordered] decimal(12,2) NOT NULL,
        [unit_cost] decimal(12,2) NOT NULL,
        CONSTRAINT [PK_PurchaseOrderItem] PRIMARY KEY ([po_item_id]),
        CONSTRAINT [FK_PurchaseOrderItem_Product_product_id] FOREIGN KEY ([product_id]) REFERENCES [Product] ([product_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PurchaseOrderItem_PurchaseOrder_po_id] FOREIGN KEY ([po_id]) REFERENCES [PurchaseOrder] ([po_id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [Payment] (
        [payment_id] bigint NOT NULL IDENTITY,
        [sale_id] bigint NOT NULL,
        [payment_method] nvarchar(20) NOT NULL,
        [amount] decimal(12,2) NOT NULL,
        [paid_at] datetime2 NOT NULL,
        [recorded_by_user_id] bigint NULL,
        CONSTRAINT [PK_Payment] PRIMARY KEY ([payment_id]),
        CONSTRAINT [FK_Payment_Sale_sale_id] FOREIGN KEY ([sale_id]) REFERENCES [Sale] ([sale_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Payment_UserAccount_recorded_by_user_id] FOREIGN KEY ([recorded_by_user_id]) REFERENCES [UserAccount] ([user_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE TABLE [SaleItem] (
        [sale_item_id] bigint NOT NULL IDENTITY,
        [sale_id] bigint NOT NULL,
        [item_type] nvarchar(10) NOT NULL,
        [product_id] bigint NULL,
        [service_id] bigint NULL,
        [qty] decimal(12,2) NOT NULL,
        [unit_price] decimal(12,2) NOT NULL,
        [line_total] decimal(12,2) NOT NULL,
        [therapist_employee_id] bigint NULL,
        CONSTRAINT [PK_SaleItem] PRIMARY KEY ([sale_item_id]),
        CONSTRAINT [FK_SaleItem_Employee_therapist_employee_id] FOREIGN KEY ([therapist_employee_id]) REFERENCES [Employee] ([employee_id]),
        CONSTRAINT [FK_SaleItem_Product_product_id] FOREIGN KEY ([product_id]) REFERENCES [Product] ([product_id]),
        CONSTRAINT [FK_SaleItem_Sale_sale_id] FOREIGN KEY ([sale_id]) REFERENCES [Sale] ([sale_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_SaleItem_Service_service_id] FOREIGN KEY ([service_id]) REFERENCES [Service] ([service_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Appointment_created_by_user_id] ON [Appointment] ([created_by_user_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Appointment_customer_id] ON [Appointment] ([customer_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AppointmentService_appointment_id] ON [AppointmentService] ([appointment_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AppointmentService_service_id] ON [AppointmentService] ([service_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AppointmentService_therapist_employee_id] ON [AppointmentService] ([therapist_employee_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AuditLog_changed_by_user_id] ON [AuditLog] ([changed_by_user_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CRM_Note_created_by_user_id] ON [CRM_Note] ([created_by_user_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CRM_Note_customer_id] ON [CRM_Note] ([customer_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Customer_customer_code] ON [Customer] ([customer_code]) WHERE [customer_code] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Customer_person_id] ON [Customer] ([person_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Employee_person_id] ON [Employee] ([person_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Employee_role_id] ON [Employee] ([role_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_EmployeeAttendance_created_by_user_id] ON [EmployeeAttendance] ([created_by_user_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_EmployeeAttendance_employee_id_work_date] ON [EmployeeAttendance] ([employee_id], [work_date]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_EmployeeServiceCommission_employee_id] ON [EmployeeServiceCommission] ([employee_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_EmployeeServiceCommission_service_id] ON [EmployeeServiceCommission] ([service_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Expense_created_by_user_id] ON [Expense] ([created_by_user_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Expense_journal_id] ON [Expense] ([journal_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Expense_ledger_account_id] ON [Expense] ([ledger_account_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Inventory_product_id] ON [Inventory] ([product_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_JournalEntry_created_by_user_id] ON [JournalEntry] ([created_by_user_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_JournalEntry_journal_no] ON [JournalEntry] ([journal_no]) WHERE [journal_no] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_JournalEntryLine_journal_id] ON [JournalEntryLine] ([journal_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_JournalEntryLine_ledger_account_id] ON [JournalEntryLine] ([ledger_account_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LedgerAccount_code] ON [LedgerAccount] ([code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Payment_recorded_by_user_id] ON [Payment] ([recorded_by_user_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Payment_sale_id] ON [Payment] ([sale_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Payroll_created_by_user_id] ON [Payroll] ([created_by_user_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Payroll_employee_id] ON [Payroll] ([employee_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Payroll_journal_id] ON [Payroll] ([journal_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Product_sku] ON [Product] ([sku]) WHERE [sku] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PurchaseOrder_created_by_user_id] ON [PurchaseOrder] ([created_by_user_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_PurchaseOrder_po_number] ON [PurchaseOrder] ([po_number]) WHERE [po_number] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PurchaseOrder_supplier_id] ON [PurchaseOrder] ([supplier_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PurchaseOrderItem_po_id] ON [PurchaseOrderItem] ([po_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PurchaseOrderItem_product_id] ON [PurchaseOrderItem] ([product_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Role_name] ON [Role] ([name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Sale_created_by_user_id] ON [Sale] ([created_by_user_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Sale_customer_id] ON [Sale] ([customer_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Sale_sale_number] ON [Sale] ([sale_number]) WHERE [sale_number] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SaleItem_product_id] ON [SaleItem] ([product_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SaleItem_sale_id] ON [SaleItem] ([sale_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SaleItem_service_id] ON [SaleItem] ([service_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SaleItem_therapist_employee_id] ON [SaleItem] ([therapist_employee_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Service_code] ON [Service] ([code]) WHERE [code] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Service_service_category_id] ON [Service] ([service_category_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StockTransaction_created_by_user_id] ON [StockTransaction] ([created_by_user_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StockTransaction_product_id] ON [StockTransaction] ([product_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserAccount_employee_id] ON [UserAccount] ([employee_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserAccount_username] ON [UserAccount] ([username]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251130234435_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251130234435_InitialCreate', N'9.0.1');
END;

COMMIT;
GO

