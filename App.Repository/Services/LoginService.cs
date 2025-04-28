using App.Shared.Models;
using App.Utils;
using App.Utils.Serializable;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace App.Repository.Services
{
    public interface ILoginService
    {
        Task<Answer<TokenModel>> LoginAsync(viUserLogin model);
    }

    public sealed class LoginService : ILoginService
    {
        private readonly ILogger<LoginService> logger;
        private readonly IMemoryCache cache;
        private readonly string baseUrl;

        public LoginService(ILogger<LoginService> logger, IConfiguration conf, IMemoryCache cache)
        {
            this.logger = logger;
            this.baseUrl = conf["Server:Url"];
            this.cache = cache;
        }

        public async Task<Answer<TokenModel>> LoginAsync(viUserLogin model)
        {
            try
            {
                if (cache.TryGetValue(model, out Answer<TokenModel> token))
                    return token;

                using var client = new HttpClient();
                string url = $"{baseUrl}/api/User/Login";
                using var req = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(model.ToJson(), System.Text.Encoding.UTF8, "application/json")
                };

                using var res = await client.SendAsync(req);
                var json = await res.Content.ReadAsStringAsync();
                token = json.FromJson<Answer<TokenModel>>();
                if (token.Code == 1)
                    cache.Set(model, token, TimeSpan.FromHours(2));

                return token;
            }
            catch (Exception ex)
            {
                logger.LogError($"LoginService.LoginAsync error: {ex.GetAllMessages()}, stack: {ex.GetStackTrace(5)}");
                return new Answer<TokenModel>(0, "Тизимда хато", "");
            }
        }
    }
}
