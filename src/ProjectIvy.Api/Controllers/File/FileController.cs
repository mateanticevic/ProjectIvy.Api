using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.File;
using ProjectIvy.Model.Binding.File;
using System.Threading.Tasks;

namespace ProjectIvy.Api.Controllers.Income
{
    [Route("[controller]")]
    public class FileController : BaseController<FileController>
    {
        private IFileHandler _fileHandler;

        public FileController(ILogger<FileController> logger, IFileHandler fileHandler) : base(logger)
        {
            _fileHandler = fileHandler;
        }
        
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var file = await _fileHandler.GetFile(id);

            return File(file.Data, file.FileType.MimeType);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Post()
        {
            var bytes = new byte[HttpContext.Request.ContentLength.Value];
            await HttpContext.Request.Body.ReadAsync(bytes, 0, (int)HttpContext.Request.ContentLength.Value);

            string fileName = await _fileHandler.UploadFile(new FileBinding() { Data = bytes, MimeType = HttpContext.Request.ContentType });

            return Ok(fileName);
        }
    }
}
