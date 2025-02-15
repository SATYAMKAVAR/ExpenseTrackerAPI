using ExpenseTrackerAPI.Models;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace ExpenseTrackerAPI.Data
{
    public class TransactionRepository
    {
        #region Configuration
        private readonly IConfiguration _connectionString;
        public TransactionRepository(IConfiguration configuration)
        {
            _connectionString = configuration;
        }
        #endregion

        #region GetAllTransactions
        public List<TransactionModel> GetAllTransactions(int UserID)
        {
            string connectionString = _connectionString.GetConnectionString("ConnectionString");
            var transactionList = new List<TransactionModel>();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var objCmd = new SqlCommand("PR_Transaction_SelectAll", conn)) // Assuming you have a stored procedure PR_Transaction_SelectAll
                {
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.AddWithValue("@UserID", UserID);

                    using (var objSDR = objCmd.ExecuteReader())
                    {
                        while (objSDR.Read())
                        {
                            var transaction = new TransactionModel
                            {
                                TransactionID = Convert.ToInt32(objSDR["TransactionID"]),
                                UserID = Convert.ToInt32(objSDR["UserID"]),
                                CategoryID = Convert.ToInt32(objSDR["CategoryID"]),
                                Note = objSDR["Note"] != DBNull.Value ? objSDR["Note"].ToString() : (String?)null,
                                CategoryName = objSDR["CategoryName"].ToString(),
                                Type = objSDR["Type"].ToString(),
                                Icon = objSDR["Icon"].ToString(),
                                Date = Convert.ToDateTime(objSDR["Date"]),
                                Amount = Convert.ToDecimal(objSDR["Amount"]),
                                CreatedAt = objSDR["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(objSDR["CreatedAt"]) : (DateTime?)null,
                                ModifiedAt = objSDR["ModifiedAt"] != DBNull.Value ? Convert.ToDateTime(objSDR["ModifiedAt"]) : (DateTime?)null
                            };

                            transactionList.Add(transaction);
                        }
                    }
                }
            }

            return transactionList;
        }
        #endregion

        #region GetTransaction
        public TransactionModel GetTransaction(int transactionID)
        {
            string connectionString = _connectionString.GetConnectionString("ConnectionString");
            TransactionModel transaction = null;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var objCmd = new SqlCommand("PR_Transaction_SelectByID", conn)) // Assuming PR_Transaction_SelectByID is the stored procedure
                {
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.AddWithValue("@TransactionID", transactionID);

                    using (var objSDR = objCmd.ExecuteReader())
                    {
                        if (objSDR.Read())
                        {
                            transaction = new TransactionModel
                            {
                                TransactionID = Convert.ToInt32(objSDR["TransactionID"]),
                                UserID = Convert.ToInt32(objSDR["UserID"]),
                                CategoryID = Convert.ToInt32(objSDR["CategoryID"]),
                                CategoryName = objSDR["CategoryName"].ToString(),
                                Icon = objSDR["Icon"].ToString(),
                                Date = Convert.ToDateTime(objSDR["Date"]),
                                Amount = Convert.ToDecimal(objSDR["Amount"]),
                                Note = objSDR["Note"] != DBNull.Value ? objSDR["Note"].ToString() : (String?)null,
                                CreatedAt = objSDR["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(objSDR["CreatedAt"]) : (DateTime?)null,
                                ModifiedAt = objSDR["ModifiedAt"] != DBNull.Value ? Convert.ToDateTime(objSDR["ModifiedAt"]) : (DateTime?)null
                            };
                        }
                    }
                }
            }

            return transaction;
        }
        #endregion

        #region Insert
        public bool Insert(TransactionModel transaction)
        {
            string connectionString = _connectionString.GetConnectionString("ConnectionString");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Transaction_Insert", conn) // Assuming PR_Transaction_Insert is the stored procedure
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@UserID", transaction.UserID);
                cmd.Parameters.AddWithValue("@CategoryID", transaction.CategoryID);
                cmd.Parameters.AddWithValue("@Note", transaction.Note ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Date", transaction.Date);
                cmd.Parameters.AddWithValue("@Amount", transaction.Amount);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        #endregion

        #region Update
        public bool Update(TransactionModel transaction)
        {
            string connectionString = _connectionString.GetConnectionString("ConnectionString");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Transaction_Update", conn) // Assuming PR_Transaction_Update is the stored procedure
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@TransactionID", transaction.TransactionID);
                cmd.Parameters.AddWithValue("@UserID", transaction.UserID);
                cmd.Parameters.AddWithValue("@CategoryID", transaction.CategoryID);
                cmd.Parameters.AddWithValue("@Note", transaction.Note ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Date", transaction.Date);
                cmd.Parameters.AddWithValue("@Amount", transaction.Amount);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        #endregion

        #region Delete
        public bool Delete(int transactionID)
        {
            string connectionString = _connectionString.GetConnectionString("ConnectionString");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Transaction_Delete", conn) // Assuming PR_Transaction_Delete is the stored procedure
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@TransactionID", transactionID);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        #endregion

        #region GetCategories
        public IEnumerable<CategoryDropDownModel> GetCategories(int userId)
        {
            var categories = new List<CategoryDropDownModel>();
            string connectionString = _connectionString.GetConnectionString("ConnectionString");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Category_DropDown", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add userId parameter to the command
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    categories.Add(new CategoryDropDownModel
                    {
                        CategoryID = Convert.ToInt32(reader["CategoryID"]),
                        CategoryName = reader["CategoryName"].ToString(),
                        Icon = reader["Icon"].ToString() // Read the icon from the database
                    });
                }
            }

            return categories;
        }
        #endregion

        #region TransactionFilter
        public List<TransactionModel> GetFilteredTransactions(TransactionFilterModel filter)
        {
            string connectionString = _connectionString.GetConnectionString("ConnectionString");
            List<TransactionModel> transactions = new List<TransactionModel>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PR_Transaction_FilterTransactions", conn)) // Stored Procedure Name
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Required parameter
                    cmd.Parameters.AddWithValue("@UserID", filter.UserID);

                    // Optional parameters: Use DBNull.Value if null
                    cmd.Parameters.AddWithValue("@CategoryID", (object?)filter.CategoryID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MinAmount", (object?)filter.MinAmount ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaxAmount", (object?)filter.MaxAmount ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Type", (object?)filter.Type ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StartDate", (object?)filter.StartDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EndDate", (object?)filter.EndDate ?? DBNull.Value);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(new TransactionModel
                            {
                                TransactionID = Convert.ToInt32(reader["TransactionID"]),
                                Note = reader["Note"].ToString(),
                                UserID = Convert.ToInt32(reader["UserID"]),
                                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                CategoryName = reader["CategoryName"].ToString(),
                                Type = reader["Type"].ToString(),
                                Icon = reader["Icon"].ToString(),
                                Date = Convert.ToDateTime(reader["Date"]),
                                Amount = Convert.ToDecimal(reader["Amount"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                ModifiedAt = reader["ModifiedAt"] != DBNull.Value ? Convert.ToDateTime(reader["ModifiedAt"]) : (DateTime?)null
                            });
                        }
                    }
                }
            }
            return transactions;
        }

        #endregion
    }
}
