using ExpenseTrackerAPI.Models;
using System.Data;
using System.Data.SqlClient;
namespace ExpenseTrackerAPI.Data
{
    public class CategoryRepository
    {
        #region Configuration
        private readonly IConfiguration _connectionString;

        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration;
        }
        #endregion

        #region GetAllCategories
        public List<CategoryModel> GetAllCategories(int userID)
        {
            string connectionString = _connectionString.GetConnectionString("ConnectionString");
            var categoryList = new List<CategoryModel>();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var objCmd = new SqlCommand("PR_Category_SelectAll", conn))
                {
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.AddWithValue("@UserID", userID); 

                    using (var objSDR = objCmd.ExecuteReader())
                    {
                        while (objSDR.Read())
                        {
                            var category = new CategoryModel
                            {
                                CategoryID = Convert.ToInt32(objSDR["CategoryID"]),
                                UserID = Convert.ToInt32(objSDR["UserID"]),
                                CategoryName = objSDR["CategoryName"].ToString(),
                                Icon = objSDR["Icon"].ToString(),
                                Type = objSDR["Type"].ToString(),
                                CreatedAt = Convert.ToDateTime(objSDR["CreatedAt"]),
                                ModifiedAt = objSDR["ModifiedAt"] != DBNull.Value ? Convert.ToDateTime(objSDR["ModifiedAt"]) : (DateTime?)null
                            };
                            categoryList.Add(category);
                        }
                    }
                }
            }

            return categoryList;
        }
        #endregion

        #region Delete
        public bool Delete(int categoryID)
        {
            string connectionString = _connectionString.GetConnectionString("ConnectionString");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Category_DeleteByID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@categoryID", categoryID);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion

        #region Insert
        public bool Insert(CategoryModel category)
        {
            string connectionString = _connectionString.GetConnectionString("ConnectionString");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Category_Insert", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserID", category.UserID);
                cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                cmd.Parameters.AddWithValue("@Icon", category.Icon);
                cmd.Parameters.AddWithValue("@Type", category.Type);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        #endregion

        #region Update
        public bool Update(CategoryModel category)
        {
            string connectionString = _connectionString.GetConnectionString("ConnectionString");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("PR_Category_UpdateByID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@CategoryID", category.CategoryID);
                cmd.Parameters.AddWithValue("@UserID", category.UserID);
                cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                cmd.Parameters.AddWithValue("@Icon", category.Icon);
                cmd.Parameters.AddWithValue("@Type", category.Type);
                
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        #endregion

        #region GetCategory
        public List<CategoryModel> GetCategory(int CategoryID)
        {
            string connectionString = _connectionString.GetConnectionString("ConnectionString");
            var categoryList = new List<CategoryModel>();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var objCmd = new SqlCommand("PR_Category_SelectByID", conn)) // Assuming PR_Category_SelectByID is the stored procedure
                {
                    objCmd.CommandType = CommandType.StoredProcedure;
                    objCmd.Parameters.AddWithValue("@CategoryID", CategoryID); // Adding CategoryID parameter to the stored procedure call

                    using (var objSDR = objCmd.ExecuteReader())
                    {
                        while (objSDR.Read())
                        {
                            var category = new CategoryModel
                            {
                                CategoryID = objSDR["CategoryID"] == DBNull.Value ? (int?)null : Convert.ToInt32(objSDR["CategoryID"]),
                                UserID = Convert.ToInt32(objSDR["UserID"]),
                                CategoryName = objSDR["CategoryName"].ToString(),
                                Icon = objSDR["Icon"].ToString(),
                                Type = objSDR["Type"].ToString(),
                                CreatedAt = Convert.ToDateTime(objSDR["CreatedAt"]),
                                ModifiedAt = objSDR["ModifiedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(objSDR["ModifiedAt"])
                            };

                            categoryList.Add(category); // Adding the category object to the list
                        }
                    }
                }
            }

            return categoryList; // Returning the list of CategoryModel objects
        }
        #endregion

    }
}
