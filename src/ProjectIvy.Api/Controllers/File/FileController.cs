using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.File;
using ProjectIvy.Model.Binding.File;
using System.Threading.Tasks;

namespace ProjectIvy.Api.Controllers.File
{
    [Route("[controller]")]
    public class FileController : BaseController<FileController>
    {
        private readonly IFileHandler _fileHandler;

        public FileController(ILogger<FileController> logger, IFileHandler fileHandler) : base(logger) => _fileHandler = fileHandler;

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _fileHandler.DeleteFile(id);

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var file = await _fileHandler.GetFile(id);

            return File(file.Data, file.Type.MimeType);
        }

        [HttpPost("")]
        public async Task<IActionResult> Post()
        {
            var bytes = new byte[HttpContext.Request.ContentLength.Value];

            using (var ms = new System.IO.MemoryStream(bytes.Length))
            {
                HttpContext.Request.Body.CopyTo(ms);
                bytes = ms.ToArray();
                string fileName = await _fileHandler.UploadFile(new FileBinding() { Data = bytes, MimeType = HttpContext.Request.ContentType });

                return Ok(fileName);
            }
        }
    }
}
