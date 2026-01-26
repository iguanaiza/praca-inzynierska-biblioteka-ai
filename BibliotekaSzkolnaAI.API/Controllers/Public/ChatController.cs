using BibliotekaSzkolnaAI.API.Services.Bot;
using BibliotekaSzkolnaAI.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotekaSzkolnaAI.API.Controllers.Public
{
    [ApiController]
    [Route("api/chat")]
    [AllowAnonymous]
    public class ChatController : ControllerBase
    {
        private readonly IFoundryAgentProvider _agentProvider;

        public ChatController(IFoundryAgentProvider agentProvider)
        {
            _agentProvider = agentProvider;
        }

        // POST api/chat
        [HttpPost]
        public async Task<ActionResult<ChatResponseDto>> SendMessage([FromBody] ChatRequestDto request)
        {
            var result = await _agentProvider.GetAnswerAsync(request.Message, request.ThreadId);
            return Ok(result);
        }
    }
}
