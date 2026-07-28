using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace SibersTest.API.Controllers
{
    /// <summary>
    /// Controller responsible for document/file upload operations.
    /// Separated from ProjectController to follow single responsibility principle
    /// and keep API interaction concerns isolated.
    /// </summary>
    [ApiController]
    [Route("api/projects/{id:int}/documents")]
    public class DocumentsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public DocumentsController(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// Uploads documents for a specific project.
        /// </summary>
        /// <param name="id">The unique identifier of the project.</param>
        /// <param name="files">The files to upload.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>A 200 OK response with the list of uploaded file names.</returns>
        /// <response code="200">Returns the list of uploaded file names.</response>
        /// <response code="400">If no files are provided.</response>
        [HttpPost]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload(int id, [FromForm] List<IFormFile> files, CancellationToken ct)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files provided.");

            var uploadsDir = Path.Combine(
                _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                "uploads",
                $"project_{id}");
            Directory.CreateDirectory(uploadsDir);

            var uploadedFiles = new List<string>();

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream, ct);
                }

                uploadedFiles.Add(fileName);
            }

            return Ok(uploadedFiles);
        }
    }
}