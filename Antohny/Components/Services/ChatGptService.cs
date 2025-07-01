using System.Net.Http;
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
        }

        public async Task<string> ObtenerRespuesta(string prompt)
        {
            var requestBody = new
            {
                prompt = prompt
            };

            var requestContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("https://antohnyramos-leog.onrender.com/chat", requestContent);

            if (!response.IsSuccessStatusCode)
                return $"Error del servidor: no se pudo procesar la pregunta.";

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(responseStream);
            var content = doc.RootElement.GetProperty("respuesta").GetString();

            return content ?? "Sin respuesta.";
        } 
    }
}
