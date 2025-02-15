using ExpenseTrackerAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        #region DashboardRepository 
        private readonly DashboardRepository _dashboardRepository;

        public DashboardController(DashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }
        #endregion

        #region GetDashboardData

        [HttpGet("{userID}")]
        public IActionResult GetDashboardData(int userID)
        {
            try
            {
                var dashboardData = _dashboardRepository.GetDashboardData(userID);
                if (dashboardData == null)
                {
                    return NotFound(new { message = "Dashboard data not found for the given user." });
                }
                return Ok(JsonConvert.SerializeObject(dashboardData)); // Directly return the dashboard data object
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the dashboard data.", error = ex.Message });
            }
        }

        #endregion
    }
}
