namespace ExpenseTrackerAPI.Models
{
    public class Dashboard
    {
        public DashboardTotals? Totals { get; set; }
        public List<RecentTransaction>? RecentTransactions { get; set; }
        public List<ExpensesByCategory>? ExpensesByCategories { get; set; }
        public List<IncomeVsExpense>? IncomeVsExpenseByDate { get; set; }
        public List<IncomeVsExpenseByMonth>? IncomeVsExpenseByMonth { get; set; }
    }

    public class DashboardTotals
    {
        public decimal? TotalIncome { get; set; }
        public decimal? TotalExpense { get; set; }
        public decimal? Balance { get; set; }
    }

    public class RecentTransaction
    {
        public int? TransactionID { get; set; }
        public DateTime? Date { get; set; }
        public string? Category { get; set; }
        public decimal? Amount { get; set; }
        public string? Type { get; set; }
    }

    public class ExpensesByCategory
    {
        public string? CategoryName { get; set; }
        public decimal? Amount { get; set; }
    }

    public class IncomeVsExpense
    {
        public DateOnly? Date { get; set; }
        public decimal? IncomeAmount { get; set; }
        public decimal? ExpenseAmount { get; set; }
    }

    public class IncomeVsExpenseByMonth
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public decimal? IncomeAmount { get; set; }
        public decimal? ExpenseAmount { get; set; }
    }
}
