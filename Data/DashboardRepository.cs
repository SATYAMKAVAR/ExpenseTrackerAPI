using System.Data.SqlClient;
using System.Data;
using ExpenseTrackerAPI.Models;

namespace ExpenseTrackerAPI.Data
{
    public class DashboardRepository
    {
        private readonly IConfiguration _configuration;

        public DashboardRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Dashboard GetDashboardData(int userID)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            var dashboard = new Dashboard
            {
                Totals = new DashboardTotals(),
                RecentTransactions = new List<RecentTransaction>(),
                ExpensesByCategories = new List<ExpensesByCategory>(),
                IncomeVsExpenseByDate = new List<IncomeVsExpense>(),
                IncomeVsExpenseByMonth = new List<IncomeVsExpenseByMonth>() // Initialize new list
            };

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var objCmd = new SqlCommand("PR_GetDashboardData", conn))
                {
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.AddWithValue("@UserID", userID);

                    using (var objSDR = objCmd.ExecuteReader())
                    {
                        // Step 1: Read Totals
                        if (objSDR.HasRows)
                        {
                            while (objSDR.Read())
                            {
                                string metric = objSDR["Metric"].ToString();
                                decimal? value = objSDR["Value"] as decimal?;

                                switch (metric)
                                {
                                    case "TotalIncome":
                                        dashboard.Totals.TotalIncome = value;
                                        break;
                                    case "TotalExpense":
                                        dashboard.Totals.TotalExpense = value;
                                        break;
                                    case "Balance":
                                        dashboard.Totals.Balance = value;
                                        break;
                                }
                            }
                        }

                        // Ensure default values for new users
                        dashboard.Totals.TotalIncome ??= 0;
                        dashboard.Totals.TotalExpense ??= 0;
                        dashboard.Totals.Balance ??= 0;

                        // Step 2: Read Recent Transactions
                        if (objSDR.NextResult() && objSDR.HasRows)
                        {
                            while (objSDR.Read())
                            {
                                dashboard.RecentTransactions.Add(new RecentTransaction
                                {
                                    TransactionID = objSDR["TransactionID"] as int?,
                                    Date = objSDR["Date"] as DateTime?,
                                    Category = objSDR["Category"] as string,
                                    Amount = objSDR["Amount"] as decimal?,
                                    Type = objSDR["Type"] as string
                                });
                            }
                        }

                        // Step 3: Read Expenses By Category
                        if (objSDR.NextResult() && objSDR.HasRows) // Ensure HasRows before reading
                        {
                            while (objSDR.Read())
                            {
                                dashboard.ExpensesByCategories.Add(new ExpensesByCategory
                                {
                                    CategoryName = objSDR["CategoryName"] as string,
                                    Amount = objSDR["Amount"] as decimal?
                                });
                            }
                        }

                        // Step 4: Read Income vs Expense By Date
                        if (objSDR.NextResult() && objSDR.HasRows) // Ensure HasRows before reading
                        {
                            while (objSDR.Read())
                            {
                                dashboard.IncomeVsExpenseByDate.Add(new IncomeVsExpense
                                {
                                    Date = objSDR["Date"] == DBNull.Value ? null : DateOnly.FromDateTime((DateTime)objSDR["Date"]),
                                    IncomeAmount = objSDR["IncomeAmount"] as decimal?,
                                    ExpenseAmount = objSDR["ExpenseAmount"] as decimal?
                                });
                            }
                        }

                        // Step 5: Read Income vs Expense By Month
                        if (objSDR.NextResult() && objSDR.HasRows) // Ensure HasRows before reading
                        {
                            while (objSDR.Read())
                            {
                                dashboard.IncomeVsExpenseByMonth.Add(new IncomeVsExpenseByMonth
                                {
                                    Year = objSDR["Year"] as int?,
                                    Month = objSDR["Month"] as int?,
                                    IncomeAmount = objSDR["IncomeAmount"] as decimal?,
                                    ExpenseAmount = objSDR["ExpenseAmount"] as decimal?
                                });
                            }
                        }
                    }
                }
            }

            return dashboard;
        }
    }
}