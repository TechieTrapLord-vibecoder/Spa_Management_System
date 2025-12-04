using Microsoft.EntityFrameworkCore;
using Spa_Management_System.Data;
using Spa_Management_System.Models;

namespace Spa_Management_System.Services;

/// <summary>
/// Automated accounting service that creates journal entries for business transactions.
/// Triggered automatically when payments are processed - no accountant intervention needed.
/// </summary>
public interface IAccountingService
{
    /// <summary>
    /// Creates journal entries when a sale/payment is completed.
    /// Handles: Service Revenue, Cash/AR, Inventory, COGS, Commission Expense, Sales Discounts
    /// </summary>
    Task CreateSaleJournalEntriesAsync(Sale sale, string paymentMethod, long? userId, long? appointmentId = null, decimal discountAmount = 0, int pointsRedeemed = 0);
    
    /// <summary>
    /// Creates journal entries when inventory is purchased.
    /// </summary>
    Task CreatePurchaseJournalEntriesAsync(PurchaseOrder purchaseOrder, long? userId);

    /// <summary>
    /// Creates journal entries when payroll is paid.
    /// Debit: Salary Expense, Credit: Cash
    /// </summary>
    Task<long?> CreatePayrollJournalEntryAsync(Payroll payroll, long? userId);

    /// <summary>
    /// Creates journal entries when an expense is paid.
    /// Debit: Expense Account (based on category), Credit: Cash
    /// </summary>
    Task<long?> CreateExpenseJournalEntryAsync(Expense expense, long? userId);
}

public class AccountingService : IAccountingService
{
    private readonly AppDbContext _context;

    // Standard Chart of Accounts codes - matching existing LedgerAccount table
    private static class AccountCodes
    {
        // Assets (1xxx)
        public const string CASH = "1100";              // Cash on Hand
        public const string ACCOUNTS_RECEIVABLE = "1200"; // Accounts Receivable
        public const string INVENTORY = "1300";          // Inventory
        
        // Liabilities (2xxx)
        public const string ACCOUNTS_PAYABLE = "2100";   // Accounts Payable
        public const string SALARIES_PAYABLE = "2200";   // Salaries Payable
        public const string VAT_PAYABLE = "2300";        // VAT Payable (Output VAT)
        
        // Revenue (4xxx)
        public const string SERVICE_REVENUE = "4100";    // Service Revenue
        public const string PRODUCT_REVENUE = "4200";    // Product Sales
        
        // Expenses (5xxx)
        public const string SALARY_EXPENSE = "5300";     // Salaries Expense
        public const string COMMISSION_EXPENSE = "5600"; // Commission Expense
        public const string RENT_EXPENSE = "5100";       // Rent Expense
        public const string UTILITIES_EXPENSE = "5200";  // Utilities Expense
        public const string SUPPLIES_EXPENSE = "5400";   // Supplies Expense
        public const string COST_OF_GOODS_SOLD = "5700"; // Cost of Goods Sold
        public const string MARKETING_EXPENSE = "5500";  // Marketing Expense
        public const string MAINTENANCE_EXPENSE = "5750"; // Maintenance Expense
        public const string INSURANCE_EXPENSE = "5800"; // Insurance Expense
        public const string TRANSPORT_EXPENSE = "5900"; // Transportation Expense
        public const string EQUIPMENT_EXPENSE = "6000"; // Equipment Expense
        public const string PROFESSIONAL_EXPENSE = "6100"; // Professional Services
        public const string TAXES_EXPENSE = "6200"; // Taxes & Licenses
        public const string MISC_EXPENSE = "6900"; // Miscellaneous Expense
        
        // Contra-Revenue (reduces revenue)
        public const string SALES_DISCOUNT = "4900"; // Sales Discounts & Loyalty Points
    }

    public AccountingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateSaleJournalEntriesAsync(Sale sale, string paymentMethod, long? userId, long? appointmentId = null, decimal discountAmount = 0, int pointsRedeemed = 0)
    {
        // Load sale with all related data
        var saleWithDetails = await _context.Sales
            .Include(s => s.SaleItems).ThenInclude(si => si.Service)
            .Include(s => s.SaleItems).ThenInclude(si => si.Product)
            .Include(s => s.SaleItems).ThenInclude(si => si.TherapistEmployee)
            .FirstOrDefaultAsync(s => s.SaleId == sale.SaleId);

        if (saleWithDetails == null) return;

        // Calculate totals
        decimal serviceTotal = saleWithDetails.SaleItems
            .Where(si => si.ItemType == "service")
            .Sum(si => si.LineTotal);

        decimal productTotal = saleWithDetails.SaleItems
            .Where(si => si.ItemType == "product")
            .Sum(si => si.LineTotal);

        decimal totalCOGS = 0;
        foreach (var item in saleWithDetails.SaleItems.Where(si => si.ItemType == "product" && si.Product != null))
        {
            totalCOGS += (item.Product!.CostPrice * item.Qty);
        }

        // Calculate commission from appointment services
        decimal totalCommission = 0;
        if (appointmentId.HasValue)
        {
            // Use the specific appointment that was just paid
            var paidAppointment = await _context.Appointments
                .Include(a => a.AppointmentServices)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId.Value);

            if (paidAppointment != null)
            {
                totalCommission = paidAppointment.AppointmentServices.Sum(s => s.CommissionAmount);
            }
        }

        // Get ledger accounts
        var accounts = await GetOrCreateAccountsAsync();

        // Generate journal number
        var journalNo = $"JE-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";
        
        // Calculate total discount (includes loyalty points: 1 point = ₱1)
        decimal loyaltyDiscount = pointsRedeemed * 1.00m;
        decimal totalDiscounts = discountAmount + loyaltyDiscount;

        // ===== JOURNAL ENTRY 1: REVENUE RECOGNITION =====
        // When customer pays, we recognize revenue
        if (serviceTotal > 0 || productTotal > 0)
        {
            var revenueEntry = new JournalEntry
            {
                JournalNo = journalNo + "-REV",
                Date = DateTime.Now,
                Description = $"Sale #{sale.SaleNumber} - Revenue Recognition",
                CreatedByUserId = userId,
                CreatedAt = DateTime.Now
            };

            // Debit: Cash or Accounts Receivable (based on payment method)
            // Amount received = Gross Total - Discounts
            var debitAccountId = paymentMethod == "charge_to_room" 
                ? accounts["AR"] 
                : accounts["CASH"];

            revenueEntry.JournalEntryLines.Add(new JournalEntryLine
            {
                LedgerAccountId = debitAccountId,
                Debit = sale.TotalAmount, // Net amount actually received (after discounts)
                Credit = 0,
                LineMemo = paymentMethod == "charge_to_room" 
                    ? "Charge to Hotel Room" 
                    : $"Payment received ({paymentMethod})"
            });
            
            // Debit: Sales Discount (if any discounts or loyalty points used)
            if (totalDiscounts > 0)
            {
                revenueEntry.JournalEntryLines.Add(new JournalEntryLine
                {
                    LedgerAccountId = accounts["SALES_DISCOUNT"],
                    Debit = totalDiscounts,
                    Credit = 0,
                    LineMemo = pointsRedeemed > 0 
                        ? $"Discount applied (₱{discountAmount:N2}) + Loyalty points redeemed ({pointsRedeemed} pts = ₱{loyaltyDiscount:N2})"
                        : $"Discount applied"
                });
            }

            // Credit: Service Revenue
            if (serviceTotal > 0)
            {
                revenueEntry.JournalEntryLines.Add(new JournalEntryLine
                {
                    LedgerAccountId = accounts["SERVICE_REVENUE"],
                    Debit = 0,
                    Credit = serviceTotal,
                    LineMemo = "Service revenue"
                });
            }

            // Credit: Product Revenue
            if (productTotal > 0)
            {
                revenueEntry.JournalEntryLines.Add(new JournalEntryLine
                {
                    LedgerAccountId = accounts["PRODUCT_REVENUE"],
                    Debit = 0,
                    Credit = productTotal,
                    LineMemo = "Product sales revenue"
                });
            }

            // Credit: VAT Payable (Output VAT collected from customer)
            if (saleWithDetails.TaxAmount > 0)
            {
                revenueEntry.JournalEntryLines.Add(new JournalEntryLine
                {
                    LedgerAccountId = accounts["VAT_PAYABLE"],
                    Debit = 0,
                    Credit = saleWithDetails.TaxAmount,
                    LineMemo = $"VAT collected ({saleWithDetails.TaxRate}%)"
                });
            }

            _context.JournalEntries.Add(revenueEntry);
        }

        // ===== JOURNAL ENTRY 2: COST OF GOODS SOLD (if products were sold) =====
        if (totalCOGS > 0)
        {
            var cogsEntry = new JournalEntry
            {
                JournalNo = journalNo + "-COGS",
                Date = DateTime.Now,
                Description = $"Sale #{sale.SaleNumber} - Cost of Goods Sold",
                CreatedByUserId = userId,
                CreatedAt = DateTime.Now
            };

            // Debit: Cost of Goods Sold (expense increases)
            cogsEntry.JournalEntryLines.Add(new JournalEntryLine
            {
                LedgerAccountId = accounts["COGS"],
                Debit = totalCOGS,
                Credit = 0,
                LineMemo = "Cost of products sold"
            });

            // Credit: Inventory (asset decreases)
            cogsEntry.JournalEntryLines.Add(new JournalEntryLine
            {
                LedgerAccountId = accounts["INVENTORY"],
                Debit = 0,
                Credit = totalCOGS,
                LineMemo = "Inventory reduction"
            });

            _context.JournalEntries.Add(cogsEntry);
        }

        // ===== JOURNAL ENTRY 3: COMMISSION EXPENSE (if therapists earned commission) =====
        if (totalCommission > 0)
        {
            var commissionEntry = new JournalEntry
            {
                JournalNo = journalNo + "-COMM",
                Date = DateTime.Now,
                Description = $"Sale #{sale.SaleNumber} - Therapist Commissions",
                CreatedByUserId = userId,
                CreatedAt = DateTime.Now
            };

            // Debit: Commission Expense (expense increases)
            commissionEntry.JournalEntryLines.Add(new JournalEntryLine
            {
                LedgerAccountId = accounts["COMMISSION"],
                Debit = totalCommission,
                Credit = 0,
                LineMemo = "Therapist commission expense"
            });

            // Credit: Accounts Payable (liability increases - we owe therapists)
            commissionEntry.JournalEntryLines.Add(new JournalEntryLine
            {
                LedgerAccountId = accounts["AP"],
                Debit = 0,
                Credit = totalCommission,
                LineMemo = "Commission payable to therapists"
            });

            _context.JournalEntries.Add(commissionEntry);
        }

        await _context.SaveChangesAsync();
    }

    public async Task CreatePurchaseJournalEntriesAsync(PurchaseOrder purchaseOrder, long? userId)
    {
        // Load PO with items
        var po = await _context.PurchaseOrders
            .Include(p => p.PurchaseOrderItems).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(p => p.PoId == purchaseOrder.PoId);

        if (po == null || po.Status != "received") return;

        var totalCost = po.PurchaseOrderItems.Sum(i => i.QtyOrdered * i.UnitCost);

        var accounts = await GetOrCreateAccountsAsync();

        var journalNo = $"JE-PO-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";

        // ===== JOURNAL ENTRY: INVENTORY PURCHASE =====
        var purchaseEntry = new JournalEntry
        {
            JournalNo = journalNo,
            Date = DateTime.Now,
            Description = $"PO #{po.PoNumber} - Inventory Received",
            CreatedByUserId = userId,
            CreatedAt = DateTime.Now
        };

        // Debit: Inventory (asset increases)
        purchaseEntry.JournalEntryLines.Add(new JournalEntryLine
        {
            LedgerAccountId = accounts["INVENTORY"],
            Debit = totalCost,
            Credit = 0,
            LineMemo = "Inventory received"
        });

        // Credit: Accounts Payable (liability increases - we owe supplier)
        purchaseEntry.JournalEntryLines.Add(new JournalEntryLine
        {
            LedgerAccountId = accounts["AP"],
            Debit = 0,
            Credit = totalCost,
            LineMemo = $"Amount owed to supplier"
        });

        _context.JournalEntries.Add(purchaseEntry);
        await _context.SaveChangesAsync();
    }

    public async Task<long?> CreatePayrollJournalEntryAsync(Payroll payroll, long? userId)
    {
        // Load payroll with employee info
        var pay = await _context.Payrolls
            .Include(p => p.Employee).ThenInclude(e => e!.Person)
            .FirstOrDefaultAsync(p => p.PayrollId == payroll.PayrollId);

        if (pay == null || pay.NetPay <= 0) return null;

        var accounts = await GetOrCreateAccountsAsync();
        var journalNo = $"JE-PAY-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";
        var employeeName = $"{pay.Employee?.Person?.FirstName} {pay.Employee?.Person?.LastName}";

        // ===== JOURNAL ENTRY: PAYROLL PAYMENT =====
        // Debit: Salary Expense (expense increases)
        // Credit: Cash (asset decreases)
        var payrollEntry = new JournalEntry
        {
            JournalNo = journalNo,
            Date = DateTime.Now,
            Description = $"Payroll - {employeeName} ({pay.PeriodStart:MMM dd} - {pay.PeriodEnd:MMM dd})",
            CreatedByUserId = userId,
            CreatedAt = DateTime.Now
        };

        // Debit: Salary Expense (Gross Pay)
        payrollEntry.JournalEntryLines.Add(new JournalEntryLine
        {
            LedgerAccountId = accounts["SALARY_EXPENSE"],
            Debit = pay.GrossPay,
            Credit = 0,
            LineMemo = $"Salary expense for {employeeName}"
        });

        // Credit: Salaries Payable (Deductions - owed to gov agencies)
        if (pay.Deductions > 0)
        {
            payrollEntry.JournalEntryLines.Add(new JournalEntryLine
            {
                LedgerAccountId = accounts["SALARIES_PAYABLE"],
                Debit = 0,
                Credit = pay.Deductions,
                LineMemo = "SSS/PhilHealth/Pag-IBIG deductions"
            });
        }

        // Credit: Cash (Net Pay - actual cash paid out)
        payrollEntry.JournalEntryLines.Add(new JournalEntryLine
        {
            LedgerAccountId = accounts["CASH"],
            Debit = 0,
            Credit = pay.NetPay,
            LineMemo = $"Net pay to {employeeName}"
        });

        _context.JournalEntries.Add(payrollEntry);
        await _context.SaveChangesAsync();

        return payrollEntry.JournalId;
    }

    public async Task<long?> CreateExpenseJournalEntryAsync(Expense expense, long? userId)
    {
        if (expense == null || expense.Amount <= 0 || expense.Status != "paid") return null;

        var accounts = await GetOrCreateAccountsAsync();
        var journalNo = $"JE-EXP-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";

        // Map category to expense account
        var expenseAccountKey = GetExpenseAccountKey(expense.Category);

        // ===== JOURNAL ENTRY: EXPENSE PAYMENT =====
        // Debit: Expense Account (expense increases)
        // Credit: Cash (asset decreases)
        var expenseEntry = new JournalEntry
        {
            JournalNo = journalNo,
            Date = DateTime.Now,
            Description = $"Expense: {expense.Category} - {expense.Description}",
            CreatedByUserId = userId,
            CreatedAt = DateTime.Now
        };

        // Use specific ledger account if provided, otherwise use category-based account
        var expenseAccountId = expense.LedgerAccountId ?? accounts[expenseAccountKey];

        // Debit: Expense Account
        expenseEntry.JournalEntryLines.Add(new JournalEntryLine
        {
            LedgerAccountId = expenseAccountId,
            Debit = expense.Amount,
            Credit = 0,
            LineMemo = $"{expense.Category}: {expense.Description}"
        });

        // Credit: Cash
        expenseEntry.JournalEntryLines.Add(new JournalEntryLine
        {
            LedgerAccountId = accounts["CASH"],
            Debit = 0,
            Credit = expense.Amount,
            LineMemo = $"Paid via {expense.PaymentMethod}" + (!string.IsNullOrEmpty(expense.Vendor) ? $" to {expense.Vendor}" : "")
        });

        _context.JournalEntries.Add(expenseEntry);
        await _context.SaveChangesAsync();

        return expenseEntry.JournalId;
    }

    /// <summary>
    /// Maps expense category to account key
    /// </summary>
    private string GetExpenseAccountKey(string category) => category switch
    {
        "Rent" => "RENT_EXPENSE",
        "Utilities" => "UTILITIES_EXPENSE",
        "Supplies" => "SUPPLIES_EXPENSE",
        "Salaries" => "SALARY_EXPENSE",
        "Marketing" => "MARKETING_EXPENSE",
        "Maintenance" => "MAINTENANCE_EXPENSE",
        "Insurance" => "INSURANCE_EXPENSE",
        "Transportation" => "TRANSPORT_EXPENSE",
        "Equipment" => "EQUIPMENT_EXPENSE",
        "Professional Services" => "PROFESSIONAL_EXPENSE",
        "Taxes & Licenses" => "TAXES_EXPENSE",
        _ => "MISC_EXPENSE"
    };

    /// <summary>
    /// Gets existing ledger accounts or creates them if they don't exist
    /// </summary>
    private async Task<Dictionary<string, long>> GetOrCreateAccountsAsync()
    {
        var requiredAccounts = new List<(string Code, string Name, string Type, string NormalBalance, string Key)>
        {
            // Assets
            (AccountCodes.CASH, "Cash on Hand", "asset", "debit", "CASH"),
            (AccountCodes.ACCOUNTS_RECEIVABLE, "Accounts Receivable", "asset", "debit", "AR"),
            (AccountCodes.INVENTORY, "Inventory", "asset", "debit", "INVENTORY"),
            // Liabilities
            (AccountCodes.ACCOUNTS_PAYABLE, "Accounts Payable", "liability", "credit", "AP"),
            (AccountCodes.SALARIES_PAYABLE, "Salaries Payable", "liability", "credit", "SALARIES_PAYABLE"),
            (AccountCodes.VAT_PAYABLE, "VAT Payable", "liability", "credit", "VAT_PAYABLE"),
            // Revenue
            (AccountCodes.SERVICE_REVENUE, "Service Revenue", "revenue", "credit", "SERVICE_REVENUE"),
            (AccountCodes.PRODUCT_REVENUE, "Product Revenue", "revenue", "credit", "PRODUCT_REVENUE"),
            // Expenses
            (AccountCodes.SALARY_EXPENSE, "Salary Expense", "expense", "debit", "SALARY_EXPENSE"),
            (AccountCodes.COMMISSION_EXPENSE, "Commission Expense", "expense", "debit", "COMMISSION"),
            (AccountCodes.RENT_EXPENSE, "Rent Expense", "expense", "debit", "RENT_EXPENSE"),
            (AccountCodes.UTILITIES_EXPENSE, "Utilities Expense", "expense", "debit", "UTILITIES_EXPENSE"),
            (AccountCodes.SUPPLIES_EXPENSE, "Supplies Expense", "expense", "debit", "SUPPLIES_EXPENSE"),
            (AccountCodes.COST_OF_GOODS_SOLD, "Cost of Goods Sold", "expense", "debit", "COGS"),
            (AccountCodes.MARKETING_EXPENSE, "Marketing Expense", "expense", "debit", "MARKETING_EXPENSE"),
            (AccountCodes.MAINTENANCE_EXPENSE, "Maintenance Expense", "expense", "debit", "MAINTENANCE_EXPENSE"),
            (AccountCodes.INSURANCE_EXPENSE, "Insurance Expense", "expense", "debit", "INSURANCE_EXPENSE"),
            (AccountCodes.TRANSPORT_EXPENSE, "Transportation Expense", "expense", "debit", "TRANSPORT_EXPENSE"),
            (AccountCodes.EQUIPMENT_EXPENSE, "Equipment Expense", "expense", "debit", "EQUIPMENT_EXPENSE"),
            (AccountCodes.PROFESSIONAL_EXPENSE, "Professional Services", "expense", "debit", "PROFESSIONAL_EXPENSE"),
            (AccountCodes.TAXES_EXPENSE, "Taxes & Licenses", "expense", "debit", "TAXES_EXPENSE"),
            (AccountCodes.MISC_EXPENSE, "Miscellaneous Expense", "expense", "debit", "MISC_EXPENSE"),
            // Contra-Revenue (still classified as 'revenue' type in the database)
            (AccountCodes.SALES_DISCOUNT, "Sales Discounts & Loyalty Points", "revenue", "debit", "SALES_DISCOUNT"),
        };

        var result = new Dictionary<string, long>();

        foreach (var (code, name, type, normalBalance, key) in requiredAccounts)
        {
            var account = await _context.LedgerAccounts.FirstOrDefaultAsync(a => a.Code == code);
            
            if (account == null)
            {
                account = new LedgerAccount
                {
                    Code = code,
                    Name = name,
                    AccountType = type,
                    NormalBalance = normalBalance
                };
                _context.LedgerAccounts.Add(account);
                await _context.SaveChangesAsync();
            }

            result[key] = account.LedgerAccountId;
        }

        return result;
    }
}
