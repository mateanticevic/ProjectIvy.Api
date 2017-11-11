using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.File;
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
    }
}
