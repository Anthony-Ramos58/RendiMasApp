using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Antohny.Services
{
    public class OpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenAiService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["OPENAI_API_KEY"];
        }

        public async Task<string> ObtenerRespuesta(string prompt)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                return "API Key no configurada.";
            }

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            try
            {
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"Error al llamar a OpenAI: {response.StatusCode} - {errorContent}";
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                var respuesta = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return respuesta ?? "Sin respuesta generada.";
            }
            catch (Exception ex)
            {
                return $"Excepción al llamar a OpenAI: {ex.Message}";
            }
        }
    }
}
