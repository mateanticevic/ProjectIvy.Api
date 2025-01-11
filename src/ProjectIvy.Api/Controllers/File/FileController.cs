using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.File;
using ProjectIvy.Model.Binding.File;

namespace ProjectIvy.Api.Controllers.File;

public class FileController : BaseController<FileController>
{
    private readonly IFileHandler _fileHandler;

    public FileController(ILogger<FileController> logger, IFileHandler fileHandler) : base(logger) => _fileHandler = fileHandler;

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id) => await Ok(_fileHandler.DeleteFile(id));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var file = await _fileHandler.GetFile(id);

        return File(file.Data, file.Type.MimeType);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromQuery] double? imageResize)
    {
        var bytes = new byte[HttpContext.Request.ContentLength.Value];

        using (var ms = new System.IO.MemoryStream(bytes.Length))
        {
            await HttpContext.Request.Body.CopyToAsync(ms);
            bytes = ms.ToArray();
            string fileName = await _fileHandler.UploadFile(new FileBinding() { Data = bytes, MimeType = HttpContext.Request.ContentType, ImageResize = imageResize });

            return Ok(fileName);
        }
    }
}
