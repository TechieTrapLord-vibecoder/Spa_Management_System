using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using IContainer = QuestPDF.Infrastructure.IContainer;
using Colors = QuestPDF.Helpers.Colors;

namespace Spa_Management_System.Services;

/// <summary>
/// Professional PDF export service with Kaye Spa branding
/// </summary>
public class PdfExportService
{
    // Kaye Spa Color Palette
    private static readonly string PrimaryColor = "#454F4A";      // Dark teal/sage
    private static readonly string SecondaryColor = "#DCD8CE";    // Warm cream
    private static readonly string AccentColor = "#AA9478";       // Warm tan
    private static readonly string TextDark = "#333333";
    private static readonly string TextLight = "#666666";
    private static readonly string SuccessColor = "#6B9B7E";
    private static readonly string DangerColor = "#C97064";
    private static readonly string White = "#FFFFFF";

    // Field to store the current user generating the PDF
    private string _generatedBy = "";

    static PdfExportService()
    {
        // Configure QuestPDF license (Community license for free use)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Generate a Balance Sheet PDF
    /// </summary>
    public byte[] GenerateBalanceSheet(
        DateTime asOfDate,
        List<AccountBalance> assets,
        List<AccountBalance> liabilities,
        List<AccountBalance> equity,
        decimal totalAssets,
        decimal totalLiabilities,
        decimal totalEquity,
        string generatedBy = "")
    {
        _generatedBy = generatedBy;
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                ConfigurePageSettings(page);

                page.Header().Element(c => ComposeHeader(c, "Balance Sheet", $"As of {asOfDate:MMMM dd, yyyy}"));

                page.Content().Element(c => ComposeBalanceSheetContent(c, assets, liabilities, equity, totalAssets, totalLiabilities, totalEquity));

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    /// <summary>
    /// Generate an Income Statement PDF
    /// </summary>
    public byte[] GenerateIncomeStatement(
        DateTime startDate,
        DateTime endDate,
        List<AccountBalance> revenues,
        List<AccountBalance> expenses,
        decimal totalRevenue,
        decimal totalExpenses,
        decimal netIncome,
        string generatedBy = "")
    {
        _generatedBy = generatedBy;
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                ConfigurePageSettings(page);

                page.Header().Element(c => ComposeHeader(c, "Income Statement", $"For the period {startDate:MMM dd, yyyy} to {endDate:MMM dd, yyyy}"));

                page.Content().Element(c => ComposeIncomeStatementContent(c, revenues, expenses, totalRevenue, totalExpenses, netIncome));

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    /// <summary>
    /// Generate a Trial Balance PDF
    /// </summary>
    public byte[] GenerateTrialBalance(
        DateTime asOfDate,
        List<TrialBalanceEntry> entries,
        decimal totalDebits,
        decimal totalCredits,
        string generatedBy = "")
    {
        _generatedBy = generatedBy;
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                ConfigurePageSettings(page);

                page.Header().Element(c => ComposeHeader(c, "Trial Balance", $"As of {asOfDate:MMMM dd, yyyy}"));

                page.Content().Element(c => ComposeTrialBalanceContent(c, entries, totalDebits, totalCredits));

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    /// <summary>
    /// Generate a Sales Report PDF
    /// </summary>
    public byte[] GenerateSalesReport(
        DateTime startDate,
        DateTime endDate,
        List<SalesReportEntry> sales,
        decimal totalSales,
        decimal totalTax,
        decimal grandTotal,
        string generatedBy = "")
    {
        _generatedBy = generatedBy;
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                ConfigurePageSettings(page);

                page.Header().Element(c => ComposeHeader(c, "Sales Report", $"From {startDate:MMM dd, yyyy} to {endDate:MMM dd, yyyy}"));

                page.Content().Element(c => ComposeSalesReportContent(c, sales, totalSales, totalTax, grandTotal));

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    /// <summary>
    /// Generate an Inventory Report PDF
    /// </summary>
    public byte[] GenerateInventoryReport(
        List<InventoryReportEntry> items,
        decimal totalValue,
        string generatedBy = "")
    {
        _generatedBy = generatedBy;
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                ConfigurePageSettings(page);

                page.Header().Element(c => ComposeHeader(c, "Inventory Report", $"Generated on {DateTime.Now:MMMM dd, yyyy}"));

                page.Content().Element(c => ComposeInventoryReportContent(c, items, totalValue));

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    /// <summary>
    /// Generate a generic data table PDF
    /// </summary>
    public byte[] GenerateGenericReport(
        string title,
        string subtitle,
        string[] headers,
        List<string[]> rows,
        string? summaryLabel = null,
        string? summaryValue = null,
        string generatedBy = "")
    {
        _generatedBy = generatedBy;
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                ConfigurePageSettings(page);

                page.Header().Element(c => ComposeHeader(c, title, subtitle));

                page.Content().Element(c => ComposeGenericTableContent(c, headers, rows, summaryLabel, summaryValue));

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    #region Page Configuration

    private void ConfigurePageSettings(PageDescriptor page)
    {
        page.Size(PageSizes.A4);
        page.MarginVertical(40);
        page.MarginHorizontal(40);
        page.DefaultTextStyle(x => x.FontSize(10).FontColor(TextDark));
    }

    #endregion

    #region Header Component

    private void ComposeHeader(IContainer container, string title, string subtitle)
    {
        container.Column(column =>
        {
            // Top bar with branding
            column.Item().Background(PrimaryColor).Padding(15).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("KAYE SPA")
                        .FontSize(18)
                        .Bold()
                        .FontColor(White);
                    col.Item().Text("Management System")
                        .FontSize(9)
                        .FontColor(SecondaryColor);
                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text(title)
                        .FontSize(16)
                        .Bold()
                        .FontColor(White);
                    col.Item().Text(subtitle)
                        .FontSize(9)
                        .FontColor(SecondaryColor);
                });
            });

            // Accent line
            column.Item().Height(3).Background(AccentColor);

            // Spacing
            column.Item().Height(20);
        });
    }

    #endregion

    #region Footer Component

    private void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            // Separator line
            column.Item().Height(1).Background(SecondaryColor);

            column.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span("Generated by: ").FontColor(TextLight);
                    text.Span(string.IsNullOrEmpty(_generatedBy) ? "System" : _generatedBy).FontColor(TextDark).Bold();
                    text.Span(" on ").FontColor(TextLight);
                    text.Span($"{DateTime.Now:MMM dd, yyyy HH:mm}").FontColor(TextDark);
                });

                row.RelativeItem().AlignCenter().Text(text =>
                {
                    text.Span("Kaye Spa Management System").FontColor(TextLight).FontSize(8);
                });

                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Page ").FontColor(TextLight);
                    text.CurrentPageNumber().FontColor(TextDark);
                    text.Span(" of ").FontColor(TextLight);
                    text.TotalPages().FontColor(TextDark);
                });
            });
        });
    }

    #endregion

    #region Balance Sheet Content

    private void ComposeBalanceSheetContent(
        IContainer container,
        List<AccountBalance> assets,
        List<AccountBalance> liabilities,
        List<AccountBalance> equity,
        decimal totalAssets,
        decimal totalLiabilities,
        decimal totalEquity)
    {
        container.Column(column =>
        {
            // Assets Section
            column.Item().Element(c => ComposeSectionHeader(c, "ASSETS"));
            column.Item().Element(c => ComposeAccountTable(c, assets));
            column.Item().Element(c => ComposeSectionTotal(c, "Total Assets", totalAssets));

            column.Item().Height(15);

            // Liabilities Section
            column.Item().Element(c => ComposeSectionHeader(c, "LIABILITIES"));
            column.Item().Element(c => ComposeAccountTable(c, liabilities));
            column.Item().Element(c => ComposeSectionTotal(c, "Total Liabilities", totalLiabilities));

            column.Item().Height(15);

            // Equity Section
            column.Item().Element(c => ComposeSectionHeader(c, "EQUITY"));
            column.Item().Element(c => ComposeAccountTable(c, equity));
            column.Item().Element(c => ComposeSectionTotal(c, "Total Equity", totalEquity));

            column.Item().Height(20);

            // Grand Total
            column.Item().Background(PrimaryColor).Padding(10).Row(row =>
            {
                row.RelativeItem().Text("Total Liabilities & Equity")
                    .Bold()
                    .FontColor(White);
                row.RelativeItem().AlignRight().Text($"{totalLiabilities + totalEquity:N2}")
                    .Bold()
                    .FontColor(White);
            });

            // Balance Check
            var isBalanced = Math.Abs(totalAssets - (totalLiabilities + totalEquity)) < 0.01m;
            column.Item().PaddingTop(10).AlignCenter().Text(text =>
            {
                if (isBalanced)
                {
                    text.Span("Balanced").FontColor(SuccessColor).Bold();
                }
                else
                {
                    text.Span($"Out of Balance by {Math.Abs(totalAssets - (totalLiabilities + totalEquity)):N2}")
                        .FontColor(DangerColor).Bold();
                }
            });
        });
    }

    #endregion

    #region Income Statement Content

    private void ComposeIncomeStatementContent(
        IContainer container,
        List<AccountBalance> revenues,
        List<AccountBalance> expenses,
        decimal totalRevenue,
        decimal totalExpenses,
        decimal netIncome)
    {
        container.Column(column =>
        {
            // Revenue Section
            column.Item().Element(c => ComposeSectionHeader(c, "REVENUE"));
            column.Item().Element(c => ComposeAccountTable(c, revenues));
            column.Item().Element(c => ComposeSectionTotal(c, "Total Revenue", totalRevenue));

            column.Item().Height(15);

            // Expenses Section
            column.Item().Element(c => ComposeSectionHeader(c, "EXPENSES"));
            column.Item().Element(c => ComposeAccountTable(c, expenses));
            column.Item().Element(c => ComposeSectionTotal(c, "Total Expenses", totalExpenses));

            column.Item().Height(20);

            // Net Income
            var bgColor = netIncome >= 0 ? SuccessColor : DangerColor;
            column.Item().Background(bgColor).Padding(12).Row(row =>
            {
                row.RelativeItem().Text(netIncome >= 0 ? "Net Income" : "Net Loss")
                    .Bold()
                    .FontSize(12)
                    .FontColor(White);
                row.RelativeItem().AlignRight().Text($"{Math.Abs(netIncome):N2}")
                    .Bold()
                    .FontSize(12)
                    .FontColor(White);
            });
        });
    }

    #endregion

    #region Trial Balance Content

    private void ComposeTrialBalanceContent(
        IContainer container,
        List<TrialBalanceEntry> entries,
        decimal totalDebits,
        decimal totalCredits)
    {
        container.Column(column =>
        {
            // Table Header
            column.Item().Background(PrimaryColor).Padding(8).Row(row =>
            {
                row.ConstantItem(70).Text("Code").Bold().FontColor(White);
                row.RelativeItem().Text("Account Name").Bold().FontColor(White);
                row.ConstantItem(100).AlignRight().Text("Debit").Bold().FontColor(White);
                row.ConstantItem(100).AlignRight().Text("Credit").Bold().FontColor(White);
            });

            // Table Rows
            var alternate = false;
            foreach (var entry in entries)
            {
                var bgColor = alternate ? SecondaryColor : White;
                column.Item().Background(bgColor).BorderBottom(1).BorderColor(SecondaryColor).Padding(6).Row(row =>
                {
                    row.ConstantItem(70).Text(entry.Code).FontColor(TextLight);
                    row.RelativeItem().Text(entry.AccountName);
                    row.ConstantItem(100).AlignRight().Text(entry.Debit > 0 ? $"{entry.Debit:N2}" : "-");
                    row.ConstantItem(100).AlignRight().Text(entry.Credit > 0 ? $"{entry.Credit:N2}" : "-");
                });
                alternate = !alternate;
            }

            // Totals Row
            column.Item().Background(PrimaryColor).Padding(8).Row(row =>
            {
                row.ConstantItem(70).Text("");
                row.RelativeItem().Text("TOTALS").Bold().FontColor(White);
                row.ConstantItem(100).AlignRight().Text($"{totalDebits:N2}").Bold().FontColor(White);
                row.ConstantItem(100).AlignRight().Text($"{totalCredits:N2}").Bold().FontColor(White);
            });

            // Balance Check
            var isBalanced = Math.Abs(totalDebits - totalCredits) < 0.01m;
            column.Item().PaddingTop(10).AlignCenter().Text(text =>
            {
                if (isBalanced)
                {
                    text.Span("Trial Balance is in Balance").FontColor(SuccessColor).Bold();
                }
                else
                {
                    text.Span($"Out of Balance by {Math.Abs(totalDebits - totalCredits):N2}")
                        .FontColor(DangerColor).Bold();
                }
            });
        });
    }

    #endregion

    #region Sales Report Content

    private void ComposeSalesReportContent(
        IContainer container,
        List<SalesReportEntry> sales,
        decimal totalSales,
        decimal totalTax,
        decimal grandTotal)
    {
        container.Column(column =>
        {
            // Table Header
            column.Item().Background(PrimaryColor).Padding(8).Row(row =>
            {
                row.ConstantItem(80).Text("Date").Bold().FontColor(White);
                row.ConstantItem(80).Text("Sale #").Bold().FontColor(White);
                row.RelativeItem().Text("Customer").Bold().FontColor(White);
                row.ConstantItem(80).AlignRight().Text("Subtotal").Bold().FontColor(White);
                row.ConstantItem(60).AlignRight().Text("Tax").Bold().FontColor(White);
                row.ConstantItem(80).AlignRight().Text("Total").Bold().FontColor(White);
            });

            // Table Rows
            var alternate = false;
            foreach (var sale in sales)
            {
                var bgColor = alternate ? SecondaryColor : White;
                column.Item().Background(bgColor).BorderBottom(1).BorderColor(SecondaryColor).Padding(6).Row(row =>
                {
                    row.ConstantItem(80).Text($"{sale.Date:MMM dd}").FontColor(TextLight);
                    row.ConstantItem(80).Text(sale.SaleNumber);
                    row.RelativeItem().Text(sale.CustomerName ?? "Walk-in");
                    row.ConstantItem(80).AlignRight().Text($"{sale.Subtotal:N2}");
                    row.ConstantItem(60).AlignRight().Text($"{sale.Tax:N2}");
                    row.ConstantItem(80).AlignRight().Text($"{sale.Total:N2}").Bold();
                });
                alternate = !alternate;
            }

            column.Item().Height(10);

            // Summary
            column.Item().AlignRight().Width(250).Column(summary =>
            {
                summary.Item().BorderBottom(1).BorderColor(SecondaryColor).Padding(5).Row(row =>
                {
                    row.RelativeItem().Text("Subtotal:");
                    row.ConstantItem(100).AlignRight().Text($"{totalSales:N2}");
                });
                summary.Item().BorderBottom(1).BorderColor(SecondaryColor).Padding(5).Row(row =>
                {
                    row.RelativeItem().Text("Total Tax:");
                    row.ConstantItem(100).AlignRight().Text($"{totalTax:N2}");
                });
                summary.Item().Background(PrimaryColor).Padding(8).Row(row =>
                {
                    row.RelativeItem().Text("Grand Total:").Bold().FontColor(White);
                    row.ConstantItem(100).AlignRight().Text($"{grandTotal:N2}").Bold().FontColor(White);
                });
            });
        });
    }

    #endregion

    #region Inventory Report Content

    private void ComposeInventoryReportContent(
        IContainer container,
        List<InventoryReportEntry> items,
        decimal totalValue)
    {
        container.Column(column =>
        {
            // Table Header
            column.Item().Background(PrimaryColor).Padding(8).Row(row =>
            {
                row.ConstantItem(80).Text("SKU").Bold().FontColor(White);
                row.RelativeItem().Text("Product Name").Bold().FontColor(White);
                row.ConstantItem(60).AlignRight().Text("Qty").Bold().FontColor(White);
                row.ConstantItem(80).AlignRight().Text("Unit Cost").Bold().FontColor(White);
                row.ConstantItem(80).AlignRight().Text("Value").Bold().FontColor(White);
                row.ConstantItem(60).AlignCenter().Text("Status").Bold().FontColor(White);
            });

            // Table Rows
            var alternate = false;
            foreach (var item in items)
            {
                var bgColor = alternate ? SecondaryColor : White;
                var statusColor = item.Status == "Low" ? DangerColor : (item.Status == "OK" ? SuccessColor : TextLight);

                column.Item().Background(bgColor).BorderBottom(1).BorderColor(SecondaryColor).Padding(6).Row(row =>
                {
                    row.ConstantItem(80).Text(item.Sku).FontColor(TextLight);
                    row.RelativeItem().Text(item.ProductName);
                    row.ConstantItem(60).AlignRight().Text($"{item.Quantity}");
                    row.ConstantItem(80).AlignRight().Text($"{item.UnitCost:N2}");
                    row.ConstantItem(80).AlignRight().Text($"{item.TotalValue:N2}").Bold();
                    row.ConstantItem(60).AlignCenter().Text(item.Status).FontColor(statusColor);
                });
                alternate = !alternate;
            }

            column.Item().Height(15);

            // Total Value
            column.Item().Background(PrimaryColor).Padding(10).Row(row =>
            {
                row.RelativeItem().Text("Total Inventory Value").Bold().FontColor(White);
                row.ConstantItem(120).AlignRight().Text($"{totalValue:N2}").Bold().FontColor(White);
            });
        });
    }

    #endregion

    #region Generic Table Content

    private void ComposeGenericTableContent(
        IContainer container,
        string[] headers,
        List<string[]> rows,
        string? summaryLabel,
        string? summaryValue)
    {
        container.Column(column =>
        {
            // Table Header
            column.Item().Background(PrimaryColor).Padding(8).Row(row =>
            {
                foreach (var header in headers)
                {
                    row.RelativeItem().Text(header).Bold().FontColor(White);
                }
            });

            // Table Rows
            var alternate = false;
            foreach (var dataRow in rows)
            {
                var bgColor = alternate ? SecondaryColor : White;
                column.Item().Background(bgColor).BorderBottom(1).BorderColor(SecondaryColor).Padding(6).Row(row =>
                {
                    foreach (var cell in dataRow)
                    {
                        row.RelativeItem().Text(cell);
                    }
                });
                alternate = !alternate;
            }

            // Summary if provided
            if (!string.IsNullOrEmpty(summaryLabel) && !string.IsNullOrEmpty(summaryValue))
            {
                column.Item().Height(15);
                column.Item().Background(PrimaryColor).Padding(10).Row(row =>
                {
                    row.RelativeItem().Text(summaryLabel).Bold().FontColor(White);
                    row.RelativeItem().AlignRight().Text(summaryValue).Bold().FontColor(White);
                });
            }
        });
    }

    #endregion

    #region Helper Components

    private void ComposeSectionHeader(IContainer container, string title)
    {
        container.Background(AccentColor).Padding(8).Text(title)
            .Bold()
            .FontSize(11)
            .FontColor(White);
    }

    private void ComposeAccountTable(IContainer container, List<AccountBalance> accounts)
    {
        container.Column(column =>
        {
            var alternate = false;
            foreach (var account in accounts)
            {
                var bgColor = alternate ? SecondaryColor : White;
                column.Item().Background(bgColor).BorderBottom(1).BorderColor(SecondaryColor).PaddingVertical(5).PaddingHorizontal(10).Row(row =>
                {
                    row.ConstantItem(60).Text(account.Code).FontSize(9).FontColor(TextLight);
                    row.RelativeItem().Text(account.Name);
                    row.ConstantItem(100).AlignRight().Text($"{account.Balance:N2}");
                });
                alternate = !alternate;
            }
        });
    }

    private void ComposeSectionTotal(IContainer container, string label, decimal amount)
    {
        container.Background(PrimaryColor).Padding(8).Row(row =>
        {
            row.RelativeItem().Text(label).Bold().FontColor(White);
            row.ConstantItem(120).AlignRight().Text($"{amount:N2}").Bold().FontColor(White);
        });
    }

    #endregion

    #region Expense Report

    /// <summary>
    /// Generate an Expense Report PDF
    /// </summary>
    public byte[] GenerateExpenseReport(
        DateTime startDate,
        DateTime endDate,
        List<ExpenseReportEntry> expenses,
        decimal totalAmount,
        string generatedBy = "")
    {
        _generatedBy = generatedBy;
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                ConfigurePageSettings(page);

                page.Header().Element(c => ComposeHeader(c, "Expense Report", $"From {startDate:MMM dd, yyyy} to {endDate:MMM dd, yyyy}"));

                page.Content().Element(c => ComposeExpenseReportContent(c, expenses, totalAmount));

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeExpenseReportContent(
        IContainer container,
        List<ExpenseReportEntry> expenses,
        decimal totalAmount)
    {
        container.Column(column =>
        {
            // Table Header
            column.Item().Background(PrimaryColor).Padding(8).Row(row =>
            {
                row.ConstantItem(80).Text("Date").Bold().FontColor(White);
                row.ConstantItem(100).Text("Category").Bold().FontColor(White);
                row.RelativeItem().Text("Description").Bold().FontColor(White);
                row.ConstantItem(80).Text("Vendor").Bold().FontColor(White);
                row.ConstantItem(80).AlignRight().Text("Amount").Bold().FontColor(White);
                row.ConstantItem(60).AlignCenter().Text("Status").Bold().FontColor(White);
            });

            // Table Rows
            var alternate = false;
            foreach (var expense in expenses)
            {
                var bgColor = alternate ? SecondaryColor : White;
                column.Item().Background(bgColor).BorderBottom(1).BorderColor(SecondaryColor).Padding(6).Row(row =>
                {
                    row.ConstantItem(80).Text($"{expense.Date:MMM dd}").FontColor(TextLight);
                    row.ConstantItem(100).Text(expense.Category);
                    row.RelativeItem().Text(expense.Description ?? "-");
                    row.ConstantItem(80).Text(expense.Vendor ?? "-");
                    row.ConstantItem(80).AlignRight().Text($"{expense.Amount:N2}").Bold();
                    row.ConstantItem(60).AlignCenter().Text(expense.Status).FontColor(expense.Status == "Paid" ? SuccessColor : AccentColor);
                });
                alternate = !alternate;
            }

            column.Item().Height(15);

            // Total
            column.Item().Background(PrimaryColor).Padding(10).Row(row =>
            {
                row.RelativeItem().Text("Total Expenses").Bold().FontColor(White);
                row.ConstantItem(120).AlignRight().Text($"{totalAmount:N2}").Bold().FontColor(White);
            });
        });
    }

    #endregion

    #region VAT Report

    /// <summary>
    /// Generate a VAT Report PDF
    /// </summary>
    public byte[] GenerateVatReport(
        int quarter,
        int year,
        decimal outputVat,
        decimal inputVat,
        decimal netVat,
        List<VatDetailEntry> salesDetails,
        List<VatDetailEntry> purchaseDetails,
        string generatedBy = "")
    {
        _generatedBy = generatedBy;
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                ConfigurePageSettings(page);

                page.Header().Element(c => ComposeHeader(c, "VAT Summary Report", $"Q{quarter} {year} - For BIR Quarterly Filing"));

                page.Content().Element(c => ComposeVatReportContent(c, outputVat, inputVat, netVat, salesDetails, purchaseDetails));

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeVatReportContent(
        IContainer container,
        decimal outputVat,
        decimal inputVat,
        decimal netVat,
        List<VatDetailEntry> salesDetails,
        List<VatDetailEntry> purchaseDetails)
    {
        container.Column(column =>
        {
            // Summary Cards
            column.Item().Row(row =>
            {
                row.RelativeItem().Background(SecondaryColor).Padding(15).Column(col =>
                {
                    col.Item().Text("Output VAT (Sales)").FontColor(TextLight);
                    col.Item().Text($"{outputVat:N2}").FontSize(18).Bold().FontColor(PrimaryColor);
                });
                row.ConstantItem(15);
                row.RelativeItem().Background(SecondaryColor).Padding(15).Column(col =>
                {
                    col.Item().Text("Input VAT (Purchases)").FontColor(TextLight);
                    col.Item().Text($"{inputVat:N2}").FontSize(18).Bold().FontColor(PrimaryColor);
                });
                row.ConstantItem(15);
                row.RelativeItem().Background(netVat >= 0 ? SuccessColor : DangerColor).Padding(15).Column(col =>
                {
                    col.Item().Text(netVat >= 0 ? "VAT Payable" : "VAT Refundable").FontColor(White);
                    col.Item().Text($"{Math.Abs(netVat):N2}").FontSize(18).Bold().FontColor(White);
                });
            });

            column.Item().Height(20);

            // Sales VAT Details
            column.Item().Element(c => ComposeSectionHeader(c, "OUTPUT VAT - SALES"));
            column.Item().Background(PrimaryColor).Padding(6).Row(row =>
            {
                row.ConstantItem(80).Text("Date").Bold().FontColor(White).FontSize(9);
                row.RelativeItem().Text("Description").Bold().FontColor(White).FontSize(9);
                row.ConstantItem(100).AlignRight().Text("Amount").Bold().FontColor(White).FontSize(9);
                row.ConstantItem(80).AlignRight().Text("VAT").Bold().FontColor(White).FontSize(9);
            });
            var alternate = false;
            foreach (var item in salesDetails.Take(10))
            {
                var bgColor = alternate ? SecondaryColor : White;
                column.Item().Background(bgColor).BorderBottom(1).BorderColor(SecondaryColor).Padding(5).Row(row =>
                {
                    row.ConstantItem(80).Text($"{item.Date:MMM dd}").FontSize(9).FontColor(TextLight);
                    row.RelativeItem().Text(item.Description).FontSize(9);
                    row.ConstantItem(100).AlignRight().Text($"{item.Amount:N2}").FontSize(9);
                    row.ConstantItem(80).AlignRight().Text($"{item.Vat:N2}").FontSize(9).Bold();
                });
                alternate = !alternate;
            }
            if (salesDetails.Count > 10)
            {
                column.Item().Padding(5).Text($"... and {salesDetails.Count - 10} more items").FontSize(9).FontColor(TextLight);
            }

            column.Item().Height(15);

            // Purchase VAT Details
            column.Item().Element(c => ComposeSectionHeader(c, "INPUT VAT - PURCHASES"));
            column.Item().Background(PrimaryColor).Padding(6).Row(row =>
            {
                row.ConstantItem(80).Text("Date").Bold().FontColor(White).FontSize(9);
                row.RelativeItem().Text("Description").Bold().FontColor(White).FontSize(9);
                row.ConstantItem(100).AlignRight().Text("Amount").Bold().FontColor(White).FontSize(9);
                row.ConstantItem(80).AlignRight().Text("VAT").Bold().FontColor(White).FontSize(9);
            });
            alternate = false;
            foreach (var item in purchaseDetails.Take(10))
            {
                var bgColor = alternate ? SecondaryColor : White;
                column.Item().Background(bgColor).BorderBottom(1).BorderColor(SecondaryColor).Padding(5).Row(row =>
                {
                    row.ConstantItem(80).Text($"{item.Date:MMM dd}").FontSize(9).FontColor(TextLight);
                    row.RelativeItem().Text(item.Description).FontSize(9);
                    row.ConstantItem(100).AlignRight().Text($"{item.Amount:N2}").FontSize(9);
                    row.ConstantItem(80).AlignRight().Text($"{item.Vat:N2}").FontSize(9).Bold();
                });
                alternate = !alternate;
            }
            if (purchaseDetails.Count > 10)
            {
                column.Item().Padding(5).Text($"... and {purchaseDetails.Count - 10} more items").FontSize(9).FontColor(TextLight);
            }

            column.Item().Height(20);

            // Net VAT
            var netBgColor = netVat >= 0 ? SuccessColor : DangerColor;
            column.Item().Background(netBgColor).Padding(12).Row(row =>
            {
                row.RelativeItem().Text(netVat >= 0 ? "NET VAT PAYABLE" : "NET VAT REFUNDABLE").Bold().FontSize(12).FontColor(White);
                row.ConstantItem(120).AlignRight().Text($"{Math.Abs(netVat):N2}").Bold().FontSize(12).FontColor(White);
            });
        });
    }

    #endregion

    #region Cash Flow Report

    /// <summary>
    /// Generate a Cash Flow Report PDF
    /// </summary>
    public byte[] GenerateCashFlowReport(
        DateTime startDate,
        DateTime endDate,
        decimal totalInflows,
        decimal totalOutflows,
        decimal netCashFlow,
        List<CashFlowEntry> dailyEntries,
        string generatedBy = "")
    {
        _generatedBy = generatedBy;
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                ConfigurePageSettings(page);

                page.Header().Element(c => ComposeHeader(c, "Cash Flow Statement", $"From {startDate:MMM dd, yyyy} to {endDate:MMM dd, yyyy}"));

                page.Content().Element(c => ComposeCashFlowContent(c, totalInflows, totalOutflows, netCashFlow, dailyEntries));

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeCashFlowContent(
        IContainer container,
        decimal totalInflows,
        decimal totalOutflows,
        decimal netCashFlow,
        List<CashFlowEntry> dailyEntries)
    {
        container.Column(column =>
        {
            // Summary Cards
            column.Item().Row(row =>
            {
                row.RelativeItem().Background(SuccessColor).Padding(15).Column(col =>
                {
                    col.Item().Text("Total Inflows").FontColor(White);
                    col.Item().Text($"+{totalInflows:N2}").FontSize(18).Bold().FontColor(White);
                });
                row.ConstantItem(15);
                row.RelativeItem().Background(DangerColor).Padding(15).Column(col =>
                {
                    col.Item().Text("Total Outflows").FontColor(White);
                    col.Item().Text($"-{totalOutflows:N2}").FontSize(18).Bold().FontColor(White);
                });
                row.ConstantItem(15);
                row.RelativeItem().Background(PrimaryColor).Padding(15).Column(col =>
                {
                    col.Item().Text("Net Cash Flow").FontColor(SecondaryColor);
                    col.Item().Text($"{(netCashFlow >= 0 ? "+" : "")}{netCashFlow:N2}").FontSize(18).Bold().FontColor(White);
                });
            });

            column.Item().Height(20);

            // Daily Cash Flow Table
            column.Item().Element(c => ComposeSectionHeader(c, "DAILY CASH FLOW BREAKDOWN"));
            column.Item().Background(PrimaryColor).Padding(8).Row(row =>
            {
                row.ConstantItem(100).Text("Date").Bold().FontColor(White);
                row.RelativeItem().AlignRight().Text("Inflows").Bold().FontColor(White);
                row.RelativeItem().AlignRight().Text("Outflows").Bold().FontColor(White);
                row.RelativeItem().AlignRight().Text("Net").Bold().FontColor(White);
                row.RelativeItem().AlignRight().Text("Running Balance").Bold().FontColor(White);
            });

            var alternate = false;
            decimal runningBalance = 0;
            foreach (var entry in dailyEntries)
            {
                runningBalance += entry.Net;
                var bgColor = alternate ? SecondaryColor : White;
                column.Item().Background(bgColor).BorderBottom(1).BorderColor(SecondaryColor).Padding(6).Row(row =>
                {
                    row.ConstantItem(100).Text($"{entry.Date:MMM dd, yyyy}").FontColor(TextLight);
                    row.RelativeItem().AlignRight().Text(entry.Inflows > 0 ? $"+{entry.Inflows:N2}" : "-").FontColor(SuccessColor);
                    row.RelativeItem().AlignRight().Text(entry.Outflows > 0 ? $"-{entry.Outflows:N2}" : "-").FontColor(DangerColor);
                    row.RelativeItem().AlignRight().Text($"{(entry.Net >= 0 ? "+" : "")}{entry.Net:N2}").Bold().FontColor(entry.Net >= 0 ? SuccessColor : DangerColor);
                    row.RelativeItem().AlignRight().Text($"{runningBalance:N2}").FontColor(TextDark);
                });
                alternate = !alternate;
            }

            column.Item().Height(15);

            // Net Cash Flow Total
            var netBgColor = netCashFlow >= 0 ? SuccessColor : DangerColor;
            column.Item().Background(netBgColor).Padding(12).Row(row =>
            {
                row.RelativeItem().Text(netCashFlow >= 0 ? "NET POSITIVE CASH FLOW" : "NET NEGATIVE CASH FLOW").Bold().FontSize(12).FontColor(White);
                row.ConstantItem(150).AlignRight().Text($"{(netCashFlow >= 0 ? "+" : "")}{netCashFlow:N2}").Bold().FontSize(12).FontColor(White);
            });
        });
    }

    #endregion
}

#region Data Transfer Objects

public class AccountBalance
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Balance { get; set; }
}

public class TrialBalanceEntry
{
    public string Code { get; set; } = "";
    public string AccountName { get; set; } = "";
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
}

public class SalesReportEntry
{
    public DateTime Date { get; set; }
    public string SaleNumber { get; set; } = "";
    public string? CustomerName { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
}

public class InventoryReportEntry
{
    public string Sku { get; set; } = "";
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalValue { get; set; }
    public string Status { get; set; } = "OK";
}

public class ExpenseReportEntry
{
    public DateTime Date { get; set; }
    public string Category { get; set; } = "";
    public string? Description { get; set; }
    public string? Vendor { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending";
}

public class VatDetailEntry
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public decimal Vat { get; set; }
}

public class CashFlowEntry
{
    public DateTime Date { get; set; }
    public decimal Inflows { get; set; }
    public decimal Outflows { get; set; }
    public decimal Net => Inflows - Outflows;
}

#endregion
