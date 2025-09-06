using System.Text;
using GroceryMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroceryMvc.Controllers
{
    public class UserCategoryController : Controller
    {
        private readonly HttpClient _client;

        public UserCategoryController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:5134/api/ProductCategoryAPI/");
        }
      
    }
}
