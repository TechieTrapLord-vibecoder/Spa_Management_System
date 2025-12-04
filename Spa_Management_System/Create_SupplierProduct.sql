-- Create SupplierProduct table for many-to-many supplier-product relationship
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SupplierProduct')
BEGIN
    CREATE TABLE SupplierProduct (
        supplier_product_id BIGINT IDENTITY(1,1) PRIMARY KEY,
        sync_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        last_modified_at DATETIME2 NULL,
        last_synced_at DATETIME2 NULL,
        sync_status NVARCHAR(20) NOT NULL DEFAULT 'pending',
        sync_version INT NOT NULL DEFAULT 1,
        supplier_id BIGINT NOT NULL,
        product_id BIGINT NOT NULL,
        supplier_price DECIMAL(12,2) NOT NULL DEFAULT 0,
        supplier_sku NVARCHAR(80) NULL,
        min_order_qty INT NULL,
        lead_time_days INT NULL,
        is_preferred BIT NOT NULL DEFAULT 0,
        is_active BIT NOT NULL DEFAULT 1,
        notes NVARCHAR(MAX) NULL,
        created_at DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_SupplierProduct_Supplier FOREIGN KEY (supplier_id) REFERENCES Supplier(supplier_id) ON DELETE CASCADE,
        CONSTRAINT FK_SupplierProduct_Product FOREIGN KEY (product_id) REFERENCES Product(product_id) ON DELETE CASCADE,
        CONSTRAINT UQ_SupplierProduct_Supplier_Product UNIQUE (supplier_id, product_id)
    );
    PRINT 'SupplierProduct table created successfully.';
END
ELSE
BEGIN
    PRINT 'SupplierProduct table already exists.';
END
GO
