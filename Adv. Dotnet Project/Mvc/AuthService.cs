using Newtonsoft.Json;
using System.Text;

namespace GroceryMvc.Service
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ✅ Login user
        public async Task<string?> AuthenticateUserAsync(string username, string password)
        {
            var requestData = new
            {
                CustomerName = username,
                Password = password
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("http://localhost:5134/api/CustomerAPI/login", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }

        // ✅ Register new customer
        public async Task<string> RegisterUserAsync(object newCustomer)
        {
            var content = new StringContent(JsonConvert.SerializeObject(newCustomer), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("http://localhost:5134/api/CustomerAPI", content);

            return await response.Content.ReadAsStringAsync(); 
        }

    }
}
