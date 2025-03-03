using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options; // Add this using for IOptions
using SplititActorsApi.Services;  // Make sure this is present

namespace SplititActorsApi.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthConfig _authConfig;

        public AuthService(HttpClient httpClient, IOptions<AuthConfig> authConfig)
        {
            _httpClient = httpClient;
            _authConfig = authConfig.Value;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://id.sandbox.splitit.com/connect/token");

            // Prepare the content for the POST request
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", "job.assignment.api"),
                new KeyValuePair<string, string>("client_id", _authConfig.ClientId),
                new KeyValuePair<string, string>("client_secret", _authConfig.ClientSecret)
            });

            try
            {
                // Send the request and await the response
                var response = await _httpClient.SendAsync(request);

                // If the response is not successful, log the error and throw an exception
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {errorResponse}"); // Log the detailed error response
                    throw new Exception($"Token request failed with status code {response.StatusCode}: {errorResponse}");
                }

                // If successful, read the response content (access token)
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent; // This contains the access token
            }
            catch (Exception ex)
            {
                // Log any exception that occurs during the process
                Console.WriteLine($"Exception occurred: {ex.Message}"); // For debugging
                throw; // Re-throw the exception so it can be handled by the caller
            }
        }
    }
}
