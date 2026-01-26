using BibliotekaSzkolnaAI.API.Services.Management.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Loans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotekaSzkolnaAI.API.Controllers.Management
{
    [Route("api/management/loans")]
    [ApiController]
    [Authorize(Roles = "Admin,Librarian")]
    public class LoansManagementController(IBookLoanManagementService managementService) : ControllerBase
    {
        // GET: api/management/loans
        [HttpGet]
        public async Task<ActionResult<List<LoanGetDto>>> GetAllLoans([FromQuery] List<LoanStatus> statuses)
        {
            var result = await managementService.GetAllLoansAsync(statuses);
            return Ok(result);
        }

        // POST: api/management/loans/approve/{loanId}
        [HttpPost("approve/{loanId:int}")]
        public async Task<IActionResult> ApproveLoan(int loanId)
        {
            var success = await managementService.ApproveLoanAsync(loanId);
            if (!success) return BadRequest("Nie można zatwierdzić (zły status).");
            return Ok("Wypożyczenie aktywowane.");
        }

        // POST: api/management/loans/finalize-return/{loanId}
        [HttpPost("finalize-return/{loanId:int}")]
        public async Task<IActionResult> FinalizeReturn(int loanId)
        {
            var success = await managementService.FinalizeReturnAsync(loanId);
            if (!success) return BadRequest("Nie można sfinalizować zwrotu.");
            return Ok("Zwrot przyjęty, egzemplarz dostępny.");
        }
    }
}
