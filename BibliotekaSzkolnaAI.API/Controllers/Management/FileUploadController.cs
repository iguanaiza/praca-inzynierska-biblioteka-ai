using BibliotekaSzkolnaAI.API.Services.Management.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BibliotekaSzkolnaAI.API.Controllers.Management
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController(IFileService fileService) : ControllerBase
    {
        // POST: api/FileUpload/upload/{folderName}
        [HttpPost("upload/{folderName}")]
        public async Task<IActionResult> Upload(IFormFile file, string folderName)
        {
            try
            {
                var fileUrl = await fileService.SaveFileAsync(file, folderName);

                return Ok(new { Url = fileUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Wystąpił błąd podczas zapisu pliku.");
            }
        }

        // DELETE: api/FileUpload/delete?fileUrl={fileUrl}
        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] string fileUrl)
        {
            fileService.DeleteFile(fileUrl);
            return NoContent();
        }
    }
}
