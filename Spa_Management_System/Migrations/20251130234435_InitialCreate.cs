using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spa_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LedgerAccount",
                columns: table => new
                {
                    ledger_account_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    account_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    normal_balance = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerAccount", x => x.ledger_account_id);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    person_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dob = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.person_id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    product_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sku = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    unit_price = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    cost_price = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.product_id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    role_id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    is_archived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCategory",
                columns: table => new
                {
                    service_category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_archived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCategory", x => x.service_category_id);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    supplier_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    contact_person = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_archived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.supplier_id);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    customer_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<long>(type: "bigint", nullable: false),
                    customer_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    loyalty_points = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_archived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.customer_id);
                    table.ForeignKey(
                        name: "FK_Customer_Person_person_id",
                        column: x => x.person_id,
                        principalTable: "Person",
                        principalColumn: "person_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    inventory_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    quantity_on_hand = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    reorder_level = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    last_counted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.inventory_id);
                    table.ForeignKey(
                        name: "FK_Inventory_Product_product_id",
                        column: x => x.product_id,
                        principalTable: "Product",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    employee_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    person_id = table.Column<long>(type: "bigint", nullable: false),
                    role_id = table.Column<short>(type: "smallint", nullable: false),
                    hire_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.employee_id);
                    table.ForeignKey(
                        name: "FK_Employee_Person_person_id",
                        column: x => x.person_id,
                        principalTable: "Person",
                        principalColumn: "person_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Role_role_id",
                        column: x => x.role_id,
                        principalTable: "Role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    service_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    service_category_id = table.Column<int>(type: "int", nullable: true),
                    code = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    price = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    duration_minutes = table.Column<int>(type: "int", nullable: false),
                    active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.service_id);
                    table.ForeignKey(
                        name: "FK_Service_ServiceCategory_service_category_id",
                        column: x => x.service_category_id,
                        principalTable: "ServiceCategory",
                        principalColumn: "service_category_id");
                });

            migrationBuilder.CreateTable(
                name: "UserAccount",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_id = table.Column<long>(type: "bigint", nullable: true),
                    username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    last_login = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccount", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_UserAccount_Employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "Employee",
                        principalColumn: "employee_id");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeServiceCommission",
                columns: table => new
                {
                    commission_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_id = table.Column<long>(type: "bigint", nullable: false),
                    service_id = table.Column<long>(type: "bigint", nullable: false),
                    commission_type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    commission_value = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    effective_from = table.Column<DateTime>(type: "datetime2", nullable: true),
                    effective_to = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_archived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeServiceCommission", x => x.commission_id);
                    table.ForeignKey(
                        name: "FK_EmployeeServiceCommission_Employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeServiceCommission_Service_service_id",
                        column: x => x.service_id,
                        principalTable: "Service",
                        principalColumn: "service_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    appointment_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    scheduled_start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    scheduled_end = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.appointment_id);
                    table.ForeignKey(
                        name: "FK_Appointment_Customer_customer_id",
                        column: x => x.customer_id,
                        principalTable: "Customer",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointment_UserAccount_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    audit_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    entity_name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    entity_id = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    action = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    changed_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    change_summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.audit_id);
                    table.ForeignKey(
                        name: "FK_AuditLog_UserAccount_changed_by_user_id",
                        column: x => x.changed_by_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "CRM_Note",
                columns: table => new
                {
                    note_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<long>(type: "bigint", nullable: false),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: false),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRM_Note", x => x.note_id);
                    table.ForeignKey(
                        name: "FK_CRM_Note_Customer_customer_id",
                        column: x => x.customer_id,
                        principalTable: "Customer",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CRM_Note_UserAccount_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAttendance",
                columns: table => new
                {
                    attendance_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_id = table.Column<long>(type: "bigint", nullable: false),
                    work_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    days_worked = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAttendance", x => x.attendance_id);
                    table.ForeignKey(
                        name: "FK_EmployeeAttendance_Employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeAttendance_UserAccount_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "JournalEntry",
                columns: table => new
                {
                    journal_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    journal_no = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntry", x => x.journal_id);
                    table.ForeignKey(
                        name: "FK_JournalEntry_UserAccount_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrder",
                columns: table => new
                {
                    po_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    supplier_id = table.Column<long>(type: "bigint", nullable: false),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    po_number = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrder", x => x.po_id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_Supplier_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "Supplier",
                        principalColumn: "supplier_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_UserAccount_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Sale",
                columns: table => new
                {
                    sale_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<long>(type: "bigint", nullable: true),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    sale_number = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    total_amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    payment_status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sale", x => x.sale_id);
                    table.ForeignKey(
                        name: "FK_Sale_Customer_customer_id",
                        column: x => x.customer_id,
                        principalTable: "Customer",
                        principalColumn: "customer_id");
                    table.ForeignKey(
                        name: "FK_Sale_UserAccount_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "StockTransaction",
                columns: table => new
                {
                    stock_tx_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    tx_type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    qty = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    unit_cost = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    reference = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransaction", x => x.stock_tx_id);
                    table.ForeignKey(
                        name: "FK_StockTransaction_Product_product_id",
                        column: x => x.product_id,
                        principalTable: "Product",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockTransaction_UserAccount_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "AppointmentService",
                columns: table => new
                {
                    appt_srv_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    appointment_id = table.Column<long>(type: "bigint", nullable: false),
                    service_id = table.Column<long>(type: "bigint", nullable: false),
                    therapist_employee_id = table.Column<long>(type: "bigint", nullable: true),
                    price = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    commission_amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentService", x => x.appt_srv_id);
                    table.ForeignKey(
                        name: "FK_AppointmentService_Appointment_appointment_id",
                        column: x => x.appointment_id,
                        principalTable: "Appointment",
                        principalColumn: "appointment_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentService_Employee_therapist_employee_id",
                        column: x => x.therapist_employee_id,
                        principalTable: "Employee",
                        principalColumn: "employee_id");
                    table.ForeignKey(
                        name: "FK_AppointmentService_Service_service_id",
                        column: x => x.service_id,
                        principalTable: "Service",
                        principalColumn: "service_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expense",
                columns: table => new
                {
                    expense_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    expense_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    vendor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    reference_number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    payment_method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ledger_account_id = table.Column<long>(type: "bigint", nullable: true),
                    journal_id = table.Column<long>(type: "bigint", nullable: true),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expense", x => x.expense_id);
                    table.ForeignKey(
                        name: "FK_Expense_JournalEntry_journal_id",
                        column: x => x.journal_id,
                        principalTable: "JournalEntry",
                        principalColumn: "journal_id");
                    table.ForeignKey(
                        name: "FK_Expense_LedgerAccount_ledger_account_id",
                        column: x => x.ledger_account_id,
                        principalTable: "LedgerAccount",
                        principalColumn: "ledger_account_id");
                    table.ForeignKey(
                        name: "FK_Expense_UserAccount_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "JournalEntryLine",
                columns: table => new
                {
                    journal_line_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    journal_id = table.Column<long>(type: "bigint", nullable: false),
                    ledger_account_id = table.Column<long>(type: "bigint", nullable: false),
                    debit = table.Column<decimal>(type: "decimal(14,2)", nullable: false),
                    credit = table.Column<decimal>(type: "decimal(14,2)", nullable: false),
                    line_memo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntryLine", x => x.journal_line_id);
                    table.ForeignKey(
                        name: "FK_JournalEntryLine_JournalEntry_journal_id",
                        column: x => x.journal_id,
                        principalTable: "JournalEntry",
                        principalColumn: "journal_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalEntryLine_LedgerAccount_ledger_account_id",
                        column: x => x.ledger_account_id,
                        principalTable: "LedgerAccount",
                        principalColumn: "ledger_account_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payroll",
                columns: table => new
                {
                    payroll_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_id = table.Column<long>(type: "bigint", nullable: false),
                    period_start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    period_end = table.Column<DateTime>(type: "datetime2", nullable: false),
                    days_worked = table.Column<int>(type: "int", nullable: false),
                    daily_rate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    gross_pay = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    deductions = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    net_pay = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    paid_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    journal_id = table.Column<long>(type: "bigint", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payroll", x => x.payroll_id);
                    table.ForeignKey(
                        name: "FK_Payroll_Employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "Employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payroll_JournalEntry_journal_id",
                        column: x => x.journal_id,
                        principalTable: "JournalEntry",
                        principalColumn: "journal_id");
                    table.ForeignKey(
                        name: "FK_Payroll_UserAccount_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItem",
                columns: table => new
                {
                    po_item_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    po_id = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    qty_ordered = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    unit_cost = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItem", x => x.po_item_id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItem_Product_product_id",
                        column: x => x.product_id,
                        principalTable: "Product",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItem_PurchaseOrder_po_id",
                        column: x => x.po_id,
                        principalTable: "PurchaseOrder",
                        principalColumn: "po_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    payment_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sale_id = table.Column<long>(type: "bigint", nullable: false),
                    payment_method = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    paid_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    recorded_by_user_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK_Payment_Sale_sale_id",
                        column: x => x.sale_id,
                        principalTable: "Sale",
                        principalColumn: "sale_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_UserAccount_recorded_by_user_id",
                        column: x => x.recorded_by_user_id,
                        principalTable: "UserAccount",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "SaleItem",
                columns: table => new
                {
                    sale_item_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sale_id = table.Column<long>(type: "bigint", nullable: false),
                    item_type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    product_id = table.Column<long>(type: "bigint", nullable: true),
                    service_id = table.Column<long>(type: "bigint", nullable: true),
                    qty = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    line_total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    therapist_employee_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleItem", x => x.sale_item_id);
                    table.ForeignKey(
                        name: "FK_SaleItem_Employee_therapist_employee_id",
                        column: x => x.therapist_employee_id,
                        principalTable: "Employee",
                        principalColumn: "employee_id");
                    table.ForeignKey(
                        name: "FK_SaleItem_Product_product_id",
                        column: x => x.product_id,
                        principalTable: "Product",
                        principalColumn: "product_id");
                    table.ForeignKey(
                        name: "FK_SaleItem_Sale_sale_id",
                        column: x => x.sale_id,
                        principalTable: "Sale",
                        principalColumn: "sale_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleItem_Service_service_id",
                        column: x => x.service_id,
                        principalTable: "Service",
                        principalColumn: "service_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_created_by_user_id",
                table: "Appointment",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_customer_id",
                table: "Appointment",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentService_appointment_id",
                table: "AppointmentService",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentService_service_id",
                table: "AppointmentService",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentService_therapist_employee_id",
                table: "AppointmentService",
                column: "therapist_employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_changed_by_user_id",
                table: "AuditLog",
                column: "changed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_CRM_Note_created_by_user_id",
                table: "CRM_Note",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_CRM_Note_customer_id",
                table: "CRM_Note",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_customer_code",
                table: "Customer",
                column: "customer_code",
                unique: true,
                filter: "[customer_code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_person_id",
                table: "Customer",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_person_id",
                table: "Employee",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_role_id",
                table: "Employee",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttendance_created_by_user_id",
                table: "EmployeeAttendance",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttendance_employee_id_work_date",
                table: "EmployeeAttendance",
                columns: new[] { "employee_id", "work_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeServiceCommission_employee_id",
                table: "EmployeeServiceCommission",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeServiceCommission_service_id",
                table: "EmployeeServiceCommission",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_created_by_user_id",
                table: "Expense",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_journal_id",
                table: "Expense",
                column: "journal_id");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_ledger_account_id",
                table: "Expense",
                column: "ledger_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_product_id",
                table: "Inventory",
                column: "product_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_created_by_user_id",
                table: "JournalEntry",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_journal_no",
                table: "JournalEntry",
                column: "journal_no",
                unique: true,
                filter: "[journal_no] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLine_journal_id",
                table: "JournalEntryLine",
                column: "journal_id");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLine_ledger_account_id",
                table: "JournalEntryLine",
                column: "ledger_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerAccount_code",
                table: "LedgerAccount",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_recorded_by_user_id",
                table: "Payment",
                column: "recorded_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_sale_id",
                table: "Payment",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payroll_created_by_user_id",
                table: "Payroll",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payroll_employee_id",
                table: "Payroll",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payroll_journal_id",
                table: "Payroll",
                column: "journal_id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_sku",
                table: "Product",
                column: "sku",
                unique: true,
                filter: "[sku] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_created_by_user_id",
                table: "PurchaseOrder",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_po_number",
                table: "PurchaseOrder",
                column: "po_number",
                unique: true,
                filter: "[po_number] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_supplier_id",
                table: "PurchaseOrder",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItem_po_id",
                table: "PurchaseOrderItem",
                column: "po_id");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItem_product_id",
                table: "PurchaseOrderItem",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_Role_name",
                table: "Role",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sale_created_by_user_id",
                table: "Sale",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Sale_customer_id",
                table: "Sale",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Sale_sale_number",
                table: "Sale",
                column: "sale_number",
                unique: true,
                filter: "[sale_number] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItem_product_id",
                table: "SaleItem",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItem_sale_id",
                table: "SaleItem",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItem_service_id",
                table: "SaleItem",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItem_therapist_employee_id",
                table: "SaleItem",
                column: "therapist_employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_Service_code",
                table: "Service",
                column: "code",
                unique: true,
                filter: "[code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Service_service_category_id",
                table: "Service",
                column: "service_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransaction_created_by_user_id",
                table: "StockTransaction",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransaction_product_id",
                table: "StockTransaction",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_employee_id",
                table: "UserAccount",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_username",
                table: "UserAccount",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentService");

            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "CRM_Note");

            migrationBuilder.DropTable(
                name: "EmployeeAttendance");

            migrationBuilder.DropTable(
                name: "EmployeeServiceCommission");

            migrationBuilder.DropTable(
                name: "Expense");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "JournalEntryLine");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Payroll");

            migrationBuilder.DropTable(
                name: "PurchaseOrderItem");

            migrationBuilder.DropTable(
                name: "SaleItem");

            migrationBuilder.DropTable(
                name: "StockTransaction");

            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "LedgerAccount");

            migrationBuilder.DropTable(
                name: "JournalEntry");

            migrationBuilder.DropTable(
                name: "PurchaseOrder");

            migrationBuilder.DropTable(
                name: "Sale");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "UserAccount");

            migrationBuilder.DropTable(
                name: "ServiceCategory");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
