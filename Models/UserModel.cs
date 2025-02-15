namespace ExpenseTrackerAPI.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAdmin { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? AccessToken { get; set; }

    }
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class ForgotPassword
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
    public class CheckUserNotExists 
    {
        public string Email { get; set; }
    }
    public class loginGoogle 
    {
        public string Email { get; set; }
        public string accessToken { get; set; }
    }
    public class GoogleUserInfo
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Verified_Email { get; set; }
        public string Picture { get; set; }
        public string Name { get; set; }
    }
}
