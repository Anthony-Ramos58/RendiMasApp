using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Antohny.Services;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System.Threading;

namespace Antohny.Tests
{
    public class ChatGptServiceTests
    {
        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-api-key");
        }

        [Test]
        public async Task ObtenerRespuesta_RespuestaExitosa_RetornaContenido()
        {
            var responseJson = """
            {
                "choices": [
                    {
                        "message": {
                            "content": "Hola, ¿en qué puedo ayudarte?"
                        }
                    }
                ]
            }
            """;

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson)
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var service = new ChatGptService(httpClient);

            var result = await service.ObtenerRespuesta("Hola");

            Assert.AreEqual("Hola, ¿en qué puedo ayudarte?", result);
        }

        [Test]
        public async Task ObtenerRespuesta_RespuestaConError_RetornaCodigoError()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var service = new ChatGptService(httpClient);

            var result = await service.ObtenerRespuesta("error");

            Assert.AreEqual("Error: BadRequest", result);
        }

        [Test]
        public void Constructor_SinApiKey_LanzaExcepcion()
        {
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);
            var httpClient = new HttpClient();

            Assert.Throws<InvalidOperationException>(() => new ChatGptService(httpClient));
        }
    }
}
