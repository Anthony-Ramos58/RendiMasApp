using Firebase.Auth;
using System;
using System.Threading.Tasks;

namespace Antohny.Services
{
    public class FirebaseLoginService
    {
        private readonly FirebaseAuthProvider _authProvider;

        public FirebaseLoginService()
        {
            var apiKey = Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("La variable de entorno FIREBASE_API_KEY no está configurada.");
            }
            var config = new FirebaseConfig(apiKey);
            _authProvider = new FirebaseAuthProvider(config);
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            try
            {
                var auth = await _authProvider.SignInWithEmailAndPasswordAsync(email, password);
                return auth.FirebaseToken;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> RegisterAsync(string email, string password, string nombre)
        {
            try
            {
                var auth = await _authProvider.CreateUserWithEmailAndPasswordAsync(email, password, nombre, true);
                Console.WriteLine("Usuario registrado con éxito:");
                Console.WriteLine($"Email: {email}");
                Console.WriteLine($"Token: {auth.FirebaseToken}");
                return auth.FirebaseToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }
    }
}
