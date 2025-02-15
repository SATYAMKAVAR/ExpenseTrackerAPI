using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.Models;
using ExpenseTrackerAPI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Google.Apis.Auth;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly IValidator<UserModel> _userValidator;
        private readonly IValidator<LoginModel> _loginValidator;
        private readonly JwtService _jwtService;

        public UserController(UserRepository userRepository, IValidator<UserModel> userValidator, IValidator<LoginModel> loginValidator, JwtService jwtService)
        {
            _userRepository = userRepository;
            _userValidator = userValidator;
            _loginValidator = loginValidator;
            _jwtService = jwtService; 
        }
       
        #region GetUser
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetUser(int id)
        {
            try
            {
                var user = _userRepository.GetUser(id);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                return Ok(JsonConvert.SerializeObject(user));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching user.", error = ex.Message });
            }
        }
        #endregion

        #region CreateAccountUser

        [HttpPost("CreateAccount")]
        public IActionResult CreateAccount([FromBody] UserModel user)
        {
            try
            {
                if (user == null)
                    return BadRequest(new { message = "Invalid user data." });

                // Validate the user model
                var validationResult = _userValidator.Validate(user);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                // Call the repository to create the user account
                string result = _userRepository.CreateAccount(user);

                if (result == "Success")
                    return Ok(new { message = "Account created successfully!" });

                if (result == "EmailExists")
                    return Conflict(new { message = "User with this email already exists." });

                return StatusCode(500, new { message = "An error occurred while creating the account." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the account.", error = ex.Message });
            }
        }

        #endregion

        #region LoginUser

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            try
            {
                if (loginModel == null)
                    return BadRequest(new { message = "Invalid login data." });

                // Validate the login model
                var validationResult = _loginValidator.Validate(loginModel);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var user = _userRepository.Login(loginModel.Email, loginModel.Password);

                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                if (user.IsActive == null || !user.IsActive.Value)
                {
                    return Unauthorized(new { message = "Your account is not active. Please contact admin for assistance." });
                }

                if (user != null && user.UserID > 0)
                {
                    user.AccessToken = _jwtService.GenerateJWTToken(user);
                    return Ok(new { success = true, user = user });
                }

                return Unauthorized(new { success = false, message = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while logging in.", error = ex.Message });
            }
        }

        #endregion
 
        #region LoginGoogle

        [HttpPost("LoginGoogle")]
        public async Task<IActionResult> LoginGoogle([FromBody] loginGoogle loginModel)
        {
            try
            {
                if (loginModel == null)
                    return BadRequest(new { message = "Invalid login data." });

                // 🔥 Step 1: Get user info from Google API using `access_token`
                using var client = new HttpClient();
                var googleUserResponse = await client.GetAsync($"https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token={loginModel.accessToken}");

                if (!googleUserResponse.IsSuccessStatusCode)
                {
                    return Unauthorized(new { message = "Invalid Google access token." });
                }

                var googleUserJson = await googleUserResponse.Content.ReadAsStringAsync();
                var googleUser = JsonConvert.DeserializeObject<GoogleUserInfo>(googleUserJson);

                // 🔥 Step 2: Validate email from Google API response
                if (googleUser.Email != loginModel.Email)
                {
                    return Unauthorized(new { message = "Invalid Google login." });
                }

                var user = _userRepository.LoginGoogle(loginModel.Email);

                if (user == null)
                {
                    return Unauthorized(new { message = "User does not exist" });
                }

                if (user.IsActive == null || !user.IsActive.Value)
                {
                    return Unauthorized(new { message = "Your account is not active. Please contact admin for assistance." });
                }

                if (user != null && user.UserID > 0)
                {
                    user.AccessToken = _jwtService.GenerateJWTToken(user);
                    return Ok(new { success = true, user = user });
                }

                return Unauthorized(new { success = false, message = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while logging in.", error = ex.Message });
            }
        }

        #endregion

        #region DeleteUser
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> SoftDeleteUser(int id)
        {
            try
            {
                await _userRepository.SoftDeleteUser(id);
                return Ok(new { message = "User soft deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while soft deleting the user.", error = ex.Message });
            }
        }
        #endregion

        #region ForgotPassword
        [HttpPut("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassword forgotPassword)
        {
            try
            {
                int rowsAffected = await _userRepository.ForgotPassword(forgotPassword);

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Password updated successfully." });
                }
                else
                {
                    return NotFound(new { message = "User not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the password.", error = ex.Message });
            }
        }
        #endregion

        #region UpdateUser
        [HttpPut("Update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserModel user)
        {
            try
            {
                if (user == null)
                    return BadRequest(new { message = "Invalid user data." });

                // Validate the user model
                var validationResult = _userValidator.Validate(user);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                    return BadRequest(new { message = "Validation failed", errors });
                }

                int rowsAffected = await _userRepository.UpdateUser(user);
                if (rowsAffected > 0)
                {
                    return Ok(new { message = "User updated successfully." });
                }
                return StatusCode(500, new { message = "An error occurred while creating the account." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the User.", error = ex.Message });
            }
        }
        #endregion

        #region CheckUserNotExists
        [HttpPut("CheckUserNotExists/")]
        public async Task<IActionResult> CheckUserNotExists([FromBody] CheckUserNotExists checkUserNotExists)
        {
            try
            {
                if (checkUserNotExists == null)
                    return BadRequest(new { message = "Invalid user data." });

                // Validate the user model
                //var validationResult = _userValidator.Validate(user);
                //if (!validationResult.IsValid)
                //{
                //    var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                //    return BadRequest(new { message = "Validation failed", errors });
                //}

                bool UserExists = await _userRepository.CheckUserNotExists(checkUserNotExists);
                if (UserExists == false)
                {
                    return Ok(new { message = "User does not exist", IsExists = 0 });
                }else   if (UserExists == true)
                {
                    return Ok(new { message = "User already exists"  , IsExists = 1 });
                }
                return StatusCode(500, new { message = "An error occurred while Checking the User." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while Checking the User.", error = ex.Message });
            }
        }
        #endregion
    }
}
