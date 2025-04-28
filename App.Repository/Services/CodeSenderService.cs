using App.Shared.Models;
using App.Utils.Serializable;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using App.Utils;

namespace App.Repository.Services
{
    public interface ICodeSenderService
    {
        Task<AnswerBasic> SendCodeAsync(string token, CodeModel model);
    }

    public sealed class CodeSenderService : ICodeSenderService
    {
        private readonly ILogger<CodeSenderService> logger;
        private readonly string baseUrl;

        public CodeSenderService(ILogger<CodeSenderService> logger, IConfiguration conf)
        {
            this.logger = logger;
            this.baseUrl = conf["Server:Url"];
        }

        public async Task<AnswerBasic> SendCodeAsync(string token, CodeModel model)
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(500);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                using var req = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/MarkingCode/SetStatusByMobile")
                {
                    Content = content
                };

                using var res = await client.SendAsync(req);
                var json = await res.Content.ReadAsStringAsync();

                return json.FromJson<AnswerBasic>();
            }
            catch (Exception ex)
            {
                logger.LogError($"MarkingCodeService.SendCodeAsync error: {ex.GetAllMessages()}, stack: {ex.GetStackTrace(5)}");
                return new AnswerBasic { code = 0, message = ex.Message };
            }
        }
    }
}
