using ExpenseTrackerAPI.Models;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace ExpenseTrackerAPI.Data
{
    public class UserRepository
    {
        #region Configuration
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region CreateAccount
        public string CreateAccount(UserModel user)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("PR_User_CreateAccount", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@ProfileImageUrl", user.ProfileImageUrl?? (object)DBNull.Value);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return "Success"; // Successfully created account
                    }
                    catch (SqlException ex)
                    {
                        // Check for specific SQL error related to the duplicate email
                        if (ex.Message.Contains("User with this email already exists"))
                        {
                            return "EmailExists"; // Duplicate email error
                        }
                        // For other errors, return a generic error message
                        return "Error";
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        #endregion

        #region LoginUser
        public UserModel Login(string email, string password)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            UserModel user = null;

            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand("PR_User_Login", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new UserModel
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                UserName = reader["UserName"].ToString(),
                                Email = reader["Email"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                IsAdmin = Convert.ToBoolean(reader["IsAdmin"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                ModifiedAt = reader["ModifiedAt"] != DBNull.Value ? Convert.ToDateTime(reader["ModifiedAt"]) : (DateTime?)null,
                                ProfileImageUrl = reader["ProfileImageUrl"] != DBNull.Value ? reader["ProfileImageUrl"].ToString() : (string?)null,
                                Address = reader["Address"].ToString()
                            };
                        }
                    }
                }
            }

            return user;
        }

        #endregion
        
        #region LoginGoogle
        public UserModel LoginGoogle(string email)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            UserModel user = null;

            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand("PR_User_LoginGoogle", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new UserModel
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                UserName = reader["UserName"].ToString(),
                                Email = reader["Email"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                IsAdmin = Convert.ToBoolean(reader["IsAdmin"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                ModifiedAt = reader["ModifiedAt"] != DBNull.Value ? Convert.ToDateTime(reader["ModifiedAt"]) : (DateTime?)null,
                                ProfileImageUrl = reader["ProfileImageUrl"] != DBNull.Value ? reader["ProfileImageUrl"].ToString() : (string?)null,
                                Address = reader["Address"].ToString()
                            };
                        }
                    }
                }
            }

            return user;
        }

        #endregion

        #region DeleteUser
        public async Task SoftDeleteUser(int userId)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("ConnectionString");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("PR_User_SoftDelete", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while soft deleting the user.", ex);
            }
        }
        #endregion

        #region ForgotPassword

        public async Task<int> ForgotPassword(ForgotPassword forgotPassword)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("ConnectionString");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("PR_User_ForgotPassword", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@Email", forgotPassword.Email);
                    cmd.Parameters.AddWithValue("@NewPassword", forgotPassword.NewPassword);

                    conn.Open();
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while updating the password.", ex);
            }
        }

        #endregion

        #region GetUser
        public UserModel GetUser(int UserID)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            UserModel user = null;

            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand("PR_User_SelectByID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new UserModel
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                UserName = reader["UserName"].ToString(),
                                Email = reader["Email"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                IsAdmin = Convert.ToBoolean(reader["IsAdmin"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                ModifiedAt = reader["ModifiedAt"] != DBNull.Value ? Convert.ToDateTime(reader["ModifiedAt"]) : (DateTime?)null,
                                ProfileImageUrl = reader["ProfileImageUrl"] != DBNull.Value ? reader["ProfileImageUrl"].ToString() : (string?)null,
                                Address = reader["Address"].ToString()
                            };
                        }
                    }
                }
            }

            return user;
        }

        #endregion

        #region UpdateUser
        public async Task<int> UpdateUser(UserModel user)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("ConnectionString");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("PR_User_UpdateByID", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@UserID", user.UserID);
                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@ProfileImageUrl", user.ProfileImageUrl ?? (object)DBNull.Value);

                    conn.Open();
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while updating the User.", ex);
            }
        }
        #endregion

        #region CheckUserNotExists
        public async Task<bool> CheckUserNotExists(CheckUserNotExists checkUserNotExists)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("ConnectionString");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("PR_User_CheckUserNotExists", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@Email", checkUserNotExists.Email);
                    conn.Open();
                    var result = await cmd.ExecuteScalarAsync();

                    return result != null && Convert.ToInt32(result) == 1;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while updating the User.", ex);
            }
        }
        #endregion
    }
}
