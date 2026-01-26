
using BibliotekaSzkolnaAI.API.Services.Catalog.Interfaces;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Loans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BibliotekaSzkolnaAI.API.Controllers.Public
{
    [Route("api/loans")]
    [ApiController]
    [Authorize]
    public class LoansPublicController(IBookLoanPublicService loansService) : ControllerBase
    {
        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        // GET: api/loans/my-loans
        [HttpGet("my-loans")]
        public async Task<ActionResult<List<LoanGetDto>>> GetMyLoans()
        {
            var result = await loansService.GetMyLoansAsync(GetUserId());
            return Ok(result);
        }

        // POST: api/loans/prolong/{loanId}
        [HttpPost("prolong/{loanId:int}")]
        public async Task<IActionResult> Prolong(int loanId)
        {
            var success = await loansService.ProlongLoanAsync(GetUserId(), loanId);
            return success ? Ok() : BadRequest("Nie można przedłużyć.");
        }

        // POST: api/loans/return-request/{loanId}
        [HttpPost("return-request/{loanId:int}")]
        public async Task<IActionResult> RequestReturn(int loanId)
        {
            var success = await loansService.RequestReturnAsync(GetUserId(), loanId);
            return success ? Ok() : BadRequest("Nie można zgłosić zwrotu.");
        }
    }
}
