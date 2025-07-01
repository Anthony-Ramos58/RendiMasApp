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
        private readonly string apiKey;

        public ChatGptService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
          ?? throw new InvalidOperationException("No se encontró la variable de entorno OPENAI_API_KEY");

        }

        public async Task<string> ObtenerRespuesta(string prompt)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var requestContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", requestContent);

            if (!response.IsSuccessStatusCode)
                return $"Error: {response.StatusCode}";

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(responseStream);
            var content = doc.RootElement
                             .GetProperty("choices")[0]
                             .GetProperty("message")
                             .GetProperty("content")
                             .GetString();

            return content ?? "Sin respuesta.";
        }
    }
}
