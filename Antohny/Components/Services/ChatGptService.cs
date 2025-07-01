using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Antohny.Services
{
    public class ChatGptService
    {
        private readonly HttpClient _httpClient;

        public ChatGptService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://rendimasapp.onrender.com/"); // ✅ URL correcta del backend en Render
        }

        public async Task<string> ObtenerRespuesta(string curso, string grado, string pregunta)
        {
            var request = new
            {
                Curso = curso,
                Grado = grado,
                Pregunta = pregunta
            };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await _httpClient.PostAsync("api/chat", content);
                if (!response.IsSuccessStatusCode)
                    return $"Error del servidor: no se pudo procesar la pregunta.";

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var respuesta = doc.RootElement.GetProperty("Respuesta").GetString();

                return respuesta ?? "Sin respuesta.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
