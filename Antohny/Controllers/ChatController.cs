using Microsoft.AspNetCore.Mvc;
using Antohny.Services;

namespace Antohny.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly OpenAiService _openAiService;

        public ChatController(OpenAiService openAiService)
        {
            _openAiService = openAiService;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("API activa");
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Pregunta) || string.IsNullOrWhiteSpace(request.Curso) || string.IsNullOrWhiteSpace(request.Grado))
            {
                return BadRequest(new { error = "Todos los campos son obligatorios: curso, grado y pregunta." });
            }

            var prompt = $"[{request.Curso} - {request.Grado}] {request.Pregunta}";
            var respuesta = await _openAiService.ObtenerRespuesta(prompt);

            return Ok(new { Respuesta = respuesta });
        }
    }

    public class ChatRequest
    {
        public string Curso { get; set; }
        public string Grado { get; set; }
        public string Pregunta { get; set; }
    }
}
