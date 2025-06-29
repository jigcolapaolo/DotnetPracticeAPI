using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Collections.Concurrent;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        private static readonly ConcurrentDictionary<string, FileInfo> _fileRegistry = new();

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file");

            var allowedTypes = new[] { "image/png", "image/jpeg", "application/pdf" };
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest("Invalid file format");

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("File exceedes maximum size (5 MB)");

            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);

            var randomFileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_uploadPath, randomFileName);

            _fileRegistry.TryAdd(randomFileName, new FileInfo
            {
                OriginalName = file.FileName,
                StoredName = randomFileName,
                Path = filePath
            });

            using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            return Ok(new { message = "File uploaded successfully!", fileId = randomFileName, originalName = file.FileName });
        }

        [HttpPost("download")]
        public IActionResult Download([FromQuery] string fileId)
        {
            if (!_fileRegistry.TryGetValue(fileId, out var fileInfo))
                return NotFound("File not found");

            var filePath = Path.Combine(_uploadPath, fileInfo.StoredName);
            if (!System.IO.File.Exists(filePath))
                return NotFound("File does not exist");

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileInfo.OriginalName, out var mime))
            {
                mime = "application/octet-stream";
            }

            return PhysicalFile(filePath, mime, fileInfo.OriginalName);
        }

        [HttpGet("getFileNames")]
        public IActionResult GetFileNames()
        {
            var fileNames = _fileRegistry
                .Select(kvp => new
                {
                    kvp.Value.OriginalName,
                    kvp.Value.StoredName
                })
                .ToList();



            return fileNames.Count == 0 ? Ok(new { message = "No files found" }) : Ok(fileNames);
        }
    }

    public class FileInfo
    {
        public string OriginalName { get; set; } = default!;
        public string StoredName { get; set; } = default!;
        public string Path { get; set; } = default!;
    }
}
