using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Antohny.Services
{
    public class OpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string apiKey;

        public OpenAiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new InvalidOperationException("OPENAI_API_KEY no configurado.");
        }

        public async Task<string> ObtenerRespuesta(string prompt)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] {
                    new { role = "user", content = prompt }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (!response.IsSuccessStatusCode)
                return $"Error del servidor: {response.StatusCode}";

            var responseStream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(responseStream);
            return doc.RootElement
                      .GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString() ?? "Sin respuesta.";
        }
    }
}
