using Microsoft.AspNetCore.Mvc;
using PhotoShare.Server.Contracts;
using PhotoShare.Shared;

namespace PhotoShare.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IFileHandler _fileHandler;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IFileHandler fileHandler)
        {
            _logger = logger;
            _fileHandler = fileHandler;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("file")]
        public async Task<ActionResult> UploadFile()
        {
            using var ms = new MemoryStream();
            await HttpContext.Request.Body.CopyToAsync(ms);
            ms.Position = 0;
            await _fileHandler.SaveToFile($"{Environment.CurrentDirectory}\\Files\\{GetFileName(HttpContext.Request.Headers)}", ms);
            return new OkResult();
        }

        private string GetFileName(IHeaderDictionary headers)
        {
            var fileName = headers.FirstOrDefault(x => x.Key == "FileName").Value.ToString();
            return String.IsNullOrEmpty(fileName) ? "Test.pdf" : fileName ;
        }
    }
}