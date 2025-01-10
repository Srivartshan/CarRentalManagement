// ApiRepository.cs
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IRepository;

namespace CarRentalManagement.Repositories
{
    public class ApiRepository<T> : IApiRepository<T> where T : class
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl;

        public ApiRepository(IHttpClientFactory httpClientFactory, string apiUrl)
        {
            _httpClientFactory = httpClientFactory;
            _apiUrl = apiUrl;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var client = _httpClientFactory.CreateClient();
            return await client.GetFromJsonAsync<IEnumerable<T>>(_apiUrl);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            return await client.GetFromJsonAsync<T>($"{_apiUrl}/{id}");
        }

        public async Task AddAsync(T entity)
        {
            var client = _httpClientFactory.CreateClient();
            await client.PostAsJsonAsync(_apiUrl, entity);
        }

        public async Task UpdateAsync(T entity)
        {
            var client = _httpClientFactory.CreateClient();
            var entityId = typeof(T).GetProperty("Id")?.GetValue(entity)?.ToString();
            if (entityId != null)
            {
                await client.PutAsJsonAsync($"{_apiUrl}/{entityId}", entity);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            await client.DeleteAsync($"{_apiUrl}/{id}");
        }
    }
}
