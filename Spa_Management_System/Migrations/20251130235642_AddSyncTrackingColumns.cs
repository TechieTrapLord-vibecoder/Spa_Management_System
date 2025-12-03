using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spa_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddSyncTrackingColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_at",
                table: "Service",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_synced_at",
                table: "Service",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sync_id",
                table: "Service",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "sync_status",
                table: "Service",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "sync_version",
                table: "Service",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_at",
                table: "Sale",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_synced_at",
                table: "Sale",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sync_id",
                table: "Sale",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "sync_status",
                table: "Sale",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "sync_version",
                table: "Sale",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_at",
                table: "Product",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_synced_at",
                table: "Product",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sync_id",
                table: "Product",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "sync_status",
                table: "Product",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "sync_version",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_at",
                table: "Person",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_synced_at",
                table: "Person",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sync_id",
                table: "Person",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "sync_status",
                table: "Person",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "sync_version",
                table: "Person",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_at",
                table: "Payroll",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_synced_at",
                table: "Payroll",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sync_id",
                table: "Payroll",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "sync_status",
                table: "Payroll",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "sync_version",
                table: "Payroll",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_at",
                table: "Payment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_synced_at",
                table: "Payment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sync_id",
                table: "Payment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "sync_status",
                table: "Payment",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "sync_version",
                table: "Payment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_at",
                table: "JournalEntry",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_synced_at",
                table: "JournalEntry",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sync_id",
                table: "JournalEntry",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "sync_status",
                table: "JournalEntry",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "sync_version",
                table: "JournalEntry",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_at",
                table: "Inventory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_synced_at",
                table: "Inventory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sync_id",
                table: "Inventory",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "sync_status",
                table: "Inventory",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "sync_version",
                table: "Inventory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_at",
                table: "Expense",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_synced_at",
                table: "Expense",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sync_id",
                table: "Expense",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "sync_status",
                table: "Expense",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "sync_version",
                table: "Expense",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_at",
                table: "Employee",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_synced_at",
                table: "Employee",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sync_id",
                table: "Employee",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "sync_status",
                table: "Employee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "sync_version",
                table: "Employee",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_at",
                table: "Customer",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_synced_at",
                table: "Customer",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sync_id",
                table: "Customer",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "sync_status",
                table: "Customer",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "sync_version",
                table: "Customer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_at",
                table: "Appointment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_synced_at",
                table: "Appointment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sync_id",
                table: "Appointment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "sync_status",
                table: "Appointment",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "sync_version",
                table: "Appointment",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_modified_at",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "last_synced_at",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "sync_id",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "sync_status",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "sync_version",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "last_modified_at",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "last_synced_at",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "sync_id",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "sync_status",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "sync_version",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "last_modified_at",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "last_synced_at",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "sync_id",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "sync_status",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "sync_version",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "last_modified_at",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "last_synced_at",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "sync_id",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "sync_status",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "sync_version",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "last_modified_at",
                table: "Payroll");

            migrationBuilder.DropColumn(
                name: "last_synced_at",
                table: "Payroll");

            migrationBuilder.DropColumn(
                name: "sync_id",
                table: "Payroll");

            migrationBuilder.DropColumn(
                name: "sync_status",
                table: "Payroll");

            migrationBuilder.DropColumn(
                name: "sync_version",
                table: "Payroll");

            migrationBuilder.DropColumn(
                name: "last_modified_at",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "last_synced_at",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "sync_id",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "sync_status",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "sync_version",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "last_modified_at",
                table: "JournalEntry");

            migrationBuilder.DropColumn(
                name: "last_synced_at",
                table: "JournalEntry");

            migrationBuilder.DropColumn(
                name: "sync_id",
                table: "JournalEntry");

            migrationBuilder.DropColumn(
                name: "sync_status",
                table: "JournalEntry");

            migrationBuilder.DropColumn(
                name: "sync_version",
                table: "JournalEntry");

            migrationBuilder.DropColumn(
                name: "last_modified_at",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "last_synced_at",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "sync_id",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "sync_status",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "sync_version",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "last_modified_at",
                table: "Expense");

            migrationBuilder.DropColumn(
                name: "last_synced_at",
                table: "Expense");

            migrationBuilder.DropColumn(
                name: "sync_id",
                table: "Expense");

            migrationBuilder.DropColumn(
                name: "sync_status",
                table: "Expense");

            migrationBuilder.DropColumn(
                name: "sync_version",
                table: "Expense");

            migrationBuilder.DropColumn(
                name: "last_modified_at",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "last_synced_at",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "sync_id",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "sync_status",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "sync_version",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "last_modified_at",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "last_synced_at",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "sync_id",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "sync_status",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "sync_version",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "last_modified_at",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "last_synced_at",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "sync_id",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "sync_status",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "sync_version",
                table: "Appointment");
        }
    }
}
