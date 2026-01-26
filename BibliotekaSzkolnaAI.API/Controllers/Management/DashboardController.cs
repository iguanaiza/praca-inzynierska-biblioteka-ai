using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.API.Services.Management.Interfaces;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotekaSzkolnaAI.API.Controllers.Management
{
    [Route("api/management/dashboard")]
    [ApiController]
    [Authorize(Roles = "Admin,Librarian")]
    public class DashboardController(IDashboardService dashboardService) : ControllerBase
    {
        // GET /api/management/dashboard
        [HttpGet]
        public async Task<ActionResult<DashboardViewModel>> GetDashboard()
        {
            return Ok(await dashboardService.GetDashboardDataAsync());
        }
    }
}
