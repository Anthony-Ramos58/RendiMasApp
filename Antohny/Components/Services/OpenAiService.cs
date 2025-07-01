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

        public OpenAiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "TU_API_KEY_OPENAI");
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

            var response = await _httpClient.PostAsync("v1/chat/completions", content);
            var responseString = await response.Content.ReadAsStringAsync();

            using var json = JsonDocument.Parse(responseString);
            var result = json.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return result ?? "Sin respuesta";
        }
    }
}
