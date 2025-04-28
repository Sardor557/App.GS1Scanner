using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;


namespace App.Utils
{
    public interface ICRestClient
    {
        void ClearToken();
        void SetTimeout(int seconds);
        Task<TResult> GetApiWithBasicAuthorizationAsync<TResult>(string url, string token = "");
        Task<TResult> GetApiAsync<TResult>(string url, string token = "");
        Task<(bool IsSuccess, string Message, byte[] data)> GetApiByteArrayAsync(string url, string token = "");
        Task<ModelRestResponse> GetApiStringResultAsync(string url, string token = "");
        Task<ModelRestResponse> PostApiStringResultAsync<TParam>(string url, TParam param, string token = "");
        Task<ModelRestResponse> PutApiStringResultAsync<TParam>(string url, TParam param, string token = "");
        Task<TResult> PostApiAsync<TResult>(string url, Dictionary<string, string> values, string token = "");
        Task<TResult> PostApiAsync<TParam, TResult>(string url, TParam param, string token = "");
        Task<TResult> PutApiAsync<TParam, TResult>(string url, TParam param, string token = "");

        Task<TResult> SendFileAsync<TResult>(string url, string FilePath, string token = "", string FormFileName = "fileForm");
        Task<TResult> SendFileAsync<TResult>(string url, MemoryStream stream, string FileName, string token = "", string FormFileName = "fileForm");
        Task<(bool IsSuccess, string response)> SendFileAsync(string url, byte[] data, Dictionary<string, string> parameters, string token = "");
        Task<(bool IsSuccess, string response)> SendFileAsync(string url, byte[] data, ModelUniversalUploadFile parameters, string token = "");
        Task<bool> GetDownloadFileAsync(string url, string SaveToFile);

        Task<(bool IsSuccess, string DataStr)> UniApiStringResultAsync<TParam>(HttpMethodEnum httpMethod, string url, TParam param, Func<string> GetToken);
        Task<(bool IsSuccess, string DataStr)> UniApiStringResultAsync<TParam>(HttpMethodEnum httpMethod, string url, TParam param, string token = "");
        Task<(bool IsSuccess, string DataStr)> UniApiStringResultAsync(HttpMethodEnum httpMethod, string url, Dictionary<string, string> param, Func<string> GetToken);

        Task<(bool IsSuccess, TResult Result)> UniApiStringResultAsync<TParam, TResult>(HttpMethodEnum httpMethod, string url, TParam param, string token = "");
        Task<(bool IsSuccess, TResult Result, TError Error)> UniApiStringResultAsync<TParam, TResult, TError>(HttpMethodEnum httpMethod, string url, TParam param, string token = "");
    }

    public class CRestClient : ICRestClient, IDisposable
    {
        private const string contentType = "application/json";
        private readonly HttpClient client;
        private bool disposedValue;

        public CRestClient()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };

            client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(500);
            client.MaxResponseContentBufferSize = 2147483647;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        }

        public void SetTimeout(int seconds)
        {
            client.Timeout = TimeSpan.FromSeconds(seconds);
        }

        public async Task<TResult> GetApiWithBasicAuthorizationAsync<TResult>(string url, string token = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

            if (token != "") request.Headers.Add("Authorization", "Basic " + token);

            string json = "";
            HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<TResult>(result);
            else if (response.StatusCode == HttpStatusCode.NotFound)
                throw new Exception("Сервис топилмади");
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new Exception("Ваколат етарли эмас");
            else
                throw new Exception($"Url: {url}, StatusCode: {response.StatusCode}, Param: {json}, Error_Message:{result}");
        }

        public async Task<(bool IsSuccess, string response)> GetApiWithBasicAuthorizationAsync(string url, string token = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

            if (token != "") request.Headers.Add("Authorization", "Basic " + token);

            HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

            string result = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, result);
        }



        public async Task<TResult> GetApiAsync<TResult>(string url, string token = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

            if (token != "") request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string json = "";
            HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<TResult>(result);
            else if (response.StatusCode == HttpStatusCode.NotFound)
                throw new Exception("Сервис топилмади");
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new Exception("Ваколат етарли эмас");
            else
                throw new Exception($"Url: {url}, StatusCode: {response.StatusCode}, Param: {json}, Error_Message:{result}");
        }

        public async Task<(bool IsSuccess, string Message, byte[] data)> GetApiByteArrayAsync(string url, string token = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

            if (token != "") request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                return (true, "", data);
            }
            else
            {
                var str = await response.Content.ReadAsStringAsync();
                return (false, str, null);
            }
        }


        public async Task<ModelRestResponse> GetApiStringResultAsync(string url, string token = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

            if (token != "") request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

            string result = await response.Content.ReadAsStringAsync();
            return new ModelRestResponse() { IsSuccess = response.IsSuccessStatusCode, DataStr = result };
        }

        public async Task<ModelRestResponse> PostApiStringResultAsync<TParam>(string url, TParam param, string token = "")
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (token != "")
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (param != null)
                    request.Content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, contentType);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                var result = await response.Content.ReadAsStringAsync();
                return new ModelRestResponse() { IsSuccess = response.IsSuccessStatusCode, DataStr = result };
            }
        }

        public async Task<ModelRestResponse> PutApiStringResultAsync<TParam>(string url, TParam param, string token = "")
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (token != "")
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (param != null)
                    request.Content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, contentType);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                var result = await response.Content.ReadAsStringAsync();
                return new ModelRestResponse() { IsSuccess = response.IsSuccessStatusCode, DataStr = result };
            }
        }

        public async Task<TResult> PostApiAsync<TResult>(string url, Dictionary<string, string> param, string token = "")
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (token != "")
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = "";

                if (param != null)
                {
                    json = JsonConvert.SerializeObject(param);
                    request.Content = new FormUrlEncodedContent(param);
                }

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TResult>(result);
                else if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Сервис топилмади");
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("Ваколат етарли эмас");
                else
                    throw new Exception($"Url: {url}, StatusCode: {response.StatusCode}, Param: {json}, Error_Message:{result}");
            }
        }

        public async Task<TResult> PostApiAsync<TParam, TResult>(string url, TParam param, string token = "")
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (token != "")
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = "";

                if (param != null)
                {
                    json = JsonConvert.SerializeObject(param);
                    request.Content = new StringContent(json, Encoding.UTF8, contentType);
                }

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TResult>(result);
                else if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Сервис топилмади");
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("Ваколат етарли эмас");
                else
                    throw new Exception($"Url: {url}, StatusCode: {response.StatusCode}, Param: {json}, Error_Message:{result}");
            }
        }

        public async Task<TResult> PutApiAsync<TParam, TResult>(string url, TParam param, string token = "")
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (token != "")
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = "";

                if (param != null)
                {
                    json = JsonConvert.SerializeObject(param);
                    request.Content = new StringContent(json, Encoding.UTF8, contentType);
                }

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TResult>(result);
                else if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Сервис топилмади");
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("Ваколат етарли эмас");
                else
                    throw new Exception($"Url: {url}, StatusCode: {response.StatusCode}, Param: {json}, Error_Message:{result}");
            }
        }

        public async Task<TResult> SendFileAsync<TResult>(string url, string FilePath, string token = "", string FormFileName = "fileForm")
        {

            client.DefaultRequestHeaders.Add("accept", "application/json");
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                if (token != "")
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var ba = File.ReadAllBytes(FilePath);

                var fi = new FileInfo(FilePath);

                using (var content = new MultipartFormDataContent() { { new ByteArrayContent(ba), FormFileName, fi.Name } })
                {
                    request.Content = content;
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    var responseStr = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        switch (Type.GetTypeCode(typeof(TResult)))
                        {
                            case TypeCode.String:
                                return (TResult)Convert.ChangeType(responseStr, typeof(TResult));

                            default:
                                return JsonConvert.DeserializeObject<TResult>(responseStr);
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                        throw new Exception("Сервис топилмади");
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Exception("Ваколат етарли эмас");
                    else
                        throw new Exception($"Url: {url}, StatusCode: {response.StatusCode}, Param: {ba.Length}, Error_Message:{responseStr}");
                };
            }
        }

        public async Task<TResult> SendFileAsync<TResult>(string url, MemoryStream stream, string FileName, string token = "", string FormFileName = "fileForm")
        {

            client.DefaultRequestHeaders.Add("accept", "application/json");
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                if (token != "")
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new ByteArrayContent(stream.ToArray()), FormFileName, FileName);

                    request.Content = content;
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    var responseStr = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        switch (Type.GetTypeCode(typeof(TResult)))
                        {
                            case TypeCode.String:
                                return (TResult)Convert.ChangeType(responseStr, typeof(TResult));

                            default:
                                return JsonConvert.DeserializeObject<TResult>(responseStr);
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                        throw new Exception("Сервис топилмади");
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Exception("Ваколат етарли эмас");
                    else
                        throw new Exception($"Url: {url}, StatusCode: {response.StatusCode}, Param: {stream.Length}, Error_Message:{responseStr}");
                };
            }
        }


        public async Task<(bool IsSuccess, string response)> SendFileAsync(string url, byte[] data, Dictionary<string, string> parameters, string token = "")
        {
            client.DefaultRequestHeaders.Add("accept", "application/json");
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                if (token != "")
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


                if (!parameters.TryGetValue("FileName", out var FileName))
                    FileName = "file";


                if (!parameters.TryGetValue("FileData", out var FileData))
                    FileData = "FileData";

                using (var content = new MultipartFormDataContent())
                {

                    content.Add(new ByteArrayContent(data), FileData, FileName);
                    foreach (var it in parameters)
                        content.Add(new StringContent(it.Value), it.Key);


                    request.Content = content;
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    var responseStr = await response.Content.ReadAsStringAsync();

                    return (response.IsSuccessStatusCode, responseStr);
                }
            }
        }

        public async Task<(bool IsSuccess, string response)> SendFileAsync(string url, byte[] data, ModelUniversalUploadFile parameters, string token = "")
        {
            client.DefaultRequestHeaders.Add("accept", "application/json");
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                if (token != "")
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new ByteArrayContent(data), "FileData", "file");
                    content.Add(new StringContent(parameters.TableId.ToStr()), "TableId");
                    content.Add(new StringContent(parameters.RequestId.ToStr()), "RequestId");
                    content.Add(new StringContent(parameters.RequestGuid.ToStr()), "RequestGuid");
                    content.Add(new StringContent(parameters.FileType.ToStr()), "FileType");
                    content.Add(new StringContent(parameters.Comment.ToStr()), "Comment");
                    content.Add(new StringContent(parameters.FileName.ToStr()), "FileName");

                    request.Content = content;
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    var responseStr = await response.Content.ReadAsStringAsync();

                    return (response.IsSuccessStatusCode, responseStr);
                };
            }
        }

        public async Task<bool> GetDownloadFileAsync(string url, string SaveToFile)
        {
            using (WebClient dc = new WebClient())
            {
                await dc.DownloadFileTaskAsync(new Uri(url), SaveToFile);
                return true;
            }
        }

        #region Uni Api Client
        public async Task<(bool IsSuccess, string DataStr)> UniApiStringResultAsync<TParam>(HttpMethodEnum httpMethod, string url, TParam param, string token = "")
        {
            using (var request = new HttpRequestMessage(GetHttpMethod(httpMethod), url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (token != "")
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (param != null)
                    request.Content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, contentType);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                var result = await response.Content.ReadAsStringAsync();
                return (response.IsSuccessStatusCode, result);
            }
        }

        public async Task<(bool IsSuccess, TResult Result)> UniApiStringResultAsync<TParam, TResult>(HttpMethodEnum httpMethod, string url, TParam param, string token = "")
        {
            using (var request = new HttpRequestMessage(GetHttpMethod(httpMethod), url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (token != "")
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (param != null)
                    request.Content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, contentType);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                var resultStr = await response.Content.ReadAsStringAsync();


                switch (Type.GetTypeCode(typeof(TResult)))
                {
                    case TypeCode.String:
                        return (response.IsSuccessStatusCode, (TResult)Convert.ChangeType(resultStr, typeof(TResult)));

                    default:
                        return (response.IsSuccessStatusCode, JsonConvert.DeserializeObject<TResult>(resultStr));
                }
            }
        }

        public async Task<(bool IsSuccess, TResult Result, TError Error)> UniApiStringResultAsync<TParam, TResult, TError>(HttpMethodEnum httpMethod, string url, TParam param, string token = "")
        {
            using (var request = new HttpRequestMessage(GetHttpMethod(httpMethod), url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (token != "")
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (param != null)
                    request.Content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, contentType);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                var resultStr = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return (IsSuccess: response.IsSuccessStatusCode, JsonConvert.DeserializeObject<TResult>(resultStr), default(TError));
                else
                    return (response.IsSuccessStatusCode, default(TResult), JsonConvert.DeserializeObject<TError>(resultStr));
            }
        }


        public async Task<(bool IsSuccess, string DataStr)> UniApiStringResultAsync(HttpMethodEnum httpMethod,
            string url, string param, Func<string> fnToken)
        {
            using (var request = new HttpRequestMessage(GetHttpMethod(httpMethod), url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (fnToken != null)
                {
                    var token = fnToken();

                    if (!string.IsNullOrWhiteSpace(token))
                        request.Headers.Add("Authorization", token);
                }


                if (!string.IsNullOrWhiteSpace(param))
                    request.Content = new StringContent(param, Encoding.UTF8, contentType);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                var result = await response.Content.ReadAsStringAsync();
                return (response.IsSuccessStatusCode, result);
            }
        }



        private HttpMethod GetHttpMethod(HttpMethodEnum httpMethod)
        {
            switch (httpMethod)
            {
                case HttpMethodEnum.Get:
                    return HttpMethod.Get;

                case HttpMethodEnum.Post:
                    return HttpMethod.Post;

                case HttpMethodEnum.Put:
                    return HttpMethod.Put;

                case HttpMethodEnum.Delete:
                    return HttpMethod.Delete;


                case HttpMethodEnum.Head:
                    return HttpMethod.Head;

                case HttpMethodEnum.Options:
                    return HttpMethod.Options;

                case HttpMethodEnum.Trace:
                    return HttpMethod.Trace;

                case HttpMethodEnum.Patch:
                    return new HttpMethod("PATCH");

                default:
                    throw new Exception("HttpMethodEnum топилмади");
            }
        }

        #endregion

        public void ClearToken()
        {
            client.DefaultRequestHeaders.Authorization = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    client.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #region Sync
        public TResult GetApi<TResult>(string url, string token = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
            if (token != "") request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = client.SendAsync(request).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<TResult>(result);
            else if (response.StatusCode == HttpStatusCode.NotFound)
                throw new Exception("Сервис топилмади");
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new Exception("Ваколат етарли эмас");
            else
                throw new Exception($"Url: {url}, StatusCode: {response.StatusCode}, Error_Message:{result}");
        }

        public ModelRestResponse GetApiStringResult(string url, string token = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
            if (token != "") request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = client.SendAsync(request).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return new ModelRestResponse() { IsSuccess = response.IsSuccessStatusCode, DataStr = result };
        }

        public ModelRestResponse PostApiStringResult<TParam>(string url, TParam param, string token = "")
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                if (token != "") request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                if (param != null) request.Content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, contentType);

                HttpResponseMessage response = client.SendAsync(request).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                return new ModelRestResponse() { IsSuccess = response.IsSuccessStatusCode, DataStr = result };
            }
        }

        public ModelRestResponse PutApiStringResult<TParam>(string url, TParam param, string token = "")
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (token != "") request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                if (param != null) request.Content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, contentType);

                HttpResponseMessage response = client.SendAsync(request).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                return new ModelRestResponse() { IsSuccess = response.IsSuccessStatusCode, DataStr = result };
            }
        }

        public TResult PostApi<TResult>(string url, Dictionary<string, string> param, string token = "")
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                if (token != "") request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = "";
                if (param != null)
                {
                    json = JsonConvert.SerializeObject(param);
                    request.Content = new FormUrlEncodedContent(param);
                }
                HttpResponseMessage response = client.SendAsync(request).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TResult>(result);
                else if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Сервис топилмади");
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("Ваколат етарли эмас");
                else
                    throw new Exception($"Url: {url}, StatusCode: {response.StatusCode}, Param: {json}, Error_Message:{result}");
            }
        }

        public TResult PostApi<TParam, TResult>(string url, TParam param, string token = "")
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                if (token != "") request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = "";
                if (param != null)
                {
                    json = JsonConvert.SerializeObject(param);
                    request.Content = new StringContent(json, Encoding.UTF8, contentType);
                }

                HttpResponseMessage response = client.SendAsync(request).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TResult>(result);
                else if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Сервис топилмади");
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("Ваколат етарли эмас");
                else
                    throw new Exception($"Url: {url}, StatusCode: {response.StatusCode}, Param: {json}, Error_Message:{result}");
            }
        }

        public TResult PutApi<TParam, TResult>(string url, TParam param, string token = "")
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                if (token != "") request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = "";
                if (param != null)
                {
                    json = JsonConvert.SerializeObject(param);
                    request.Content = new StringContent(json, Encoding.UTF8, contentType);
                }

                HttpResponseMessage response = client.SendAsync(request).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TResult>(result);
                else if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Сервис топилмади");
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("Ваколат етарли эмас");
                else
                    throw new Exception($"Url: {url}, StatusCode: {response.StatusCode}, Param: {json}, Error_Message:{result}");
            }
        }

        public TResult SendFile<TResult>(string url, string FilePath, string token = "", string FormFileName = "fileForm")
        {

            client.DefaultRequestHeaders.Add("accept", "application/json");
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                if (token != "") request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var ba = File.ReadAllBytes(FilePath);
                using (var content = new MultipartFormDataContent() { { new ByteArrayContent(ba), FormFileName, FormFileName } })
                {
                    request.Content = content;
                    HttpResponseMessage response = client.SendAsync(request).Result;
                    var responseStr = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        switch (Type.GetTypeCode(typeof(TResult)))
                        {
                            case TypeCode.String:
                                return (TResult)Convert.ChangeType(responseStr, typeof(TResult));

                            default:
                                return JsonConvert.DeserializeObject<TResult>(responseStr);
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                        throw new Exception("Сервис топилмади");
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Exception("Ваколат етарли эмас");
                    else
                        throw new Exception($"Url: {url}, StatusCode: {response.StatusCode}, Param: {ba.Length}, Error_Message:{responseStr}");
                };
            }
        }

        public TResult SendFile<TResult>(string url, MemoryStream stream, string token = "", string FormFileName = "fileForm")
        {
            client.DefaultRequestHeaders.Add("accept", "application/json");
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                if (token != "") request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using (var content = new MultipartFormDataContent() { { new ByteArrayContent(stream.ToArray()), FormFileName, FormFileName } })
                {
                    request.Content = content;
                    HttpResponseMessage response = client.SendAsync(request).Result;
                    var responseStr = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        switch (Type.GetTypeCode(typeof(TResult)))
                        {
                            case TypeCode.String:
                                return (TResult)Convert.ChangeType(responseStr, typeof(TResult));

                            default:
                                return JsonConvert.DeserializeObject<TResult>(responseStr);
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                        throw new Exception("Сервис топилмади");
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Exception("Ваколат етарли эмас");
                    else
                        throw new Exception($"Url: {url}, StatusCode: {response.StatusCode}, Param: {stream.Length}, Error_Message:{responseStr}");
                };
            }
        }


        public async Task<(bool IsSuccess, string DataStr)> UniApiStringResultAsync(HttpMethodEnum httpMethod, string url, Dictionary<string, string> param, Func<string> GetToken)
        {
            using (var request = new HttpRequestMessage(GetHttpMethod(httpMethod), url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (GetToken != null)
                {
                    var token = GetToken();

                    if (!string.IsNullOrWhiteSpace(token))
                        request.Headers.Add("Authorization", token);
                }

                string json = "";

                if (param != null)
                {
                    json = JsonConvert.SerializeObject(param);
                    request.Content = new FormUrlEncodedContent(param);
                }

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                var result = await response.Content.ReadAsStringAsync();
                return (response.IsSuccessStatusCode, result);
            }
        }

        public async Task<(bool IsSuccess, string DataStr)> UniApiStringResultAsync<TParam>(HttpMethodEnum httpMethod, string url, TParam param, Func<string> GetToken)
        {
            using (var request = new HttpRequestMessage(GetHttpMethod(httpMethod), url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (GetToken != null)
                {
                    var token = GetToken();

                    if (!string.IsNullOrWhiteSpace(token))
                        request.Headers.Add("Authorization", token);
                }

                if (param != null)
                    request.Content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, contentType);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                var result = await response.Content.ReadAsStringAsync();
                return (response.IsSuccessStatusCode, result);
            }

        }

        #endregion
    }


    public enum HttpMethodEnum
    {
        Delete,
        Get,
        Head,
        Options,
        Post,
        Put,
        Patch,
        Trace
    }

    public class ModelRestResponse
    {
        public bool IsSuccess { get; set; }
        public string DataStr { get; set; }
    }

    public class ModelUniversalUploadFile
    {
        public long TableId { get; set; }
        public long? RequestId { get; set; }
        public string RequestGuid { get; set; }
        public long FileType { get; set; }
        public string Comment { get; set; }
        public string FileName { get; set; }
    }

}
