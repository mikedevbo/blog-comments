namespace Components.GitHub
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public static class HttpClientExtensionMethods
    {
        public static async Task<TModel> ReadtAsJsonAsync<TModel>(this HttpContent content)
        {
            var jsonResult = await content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TModel>(jsonResult);
            return result;
        }

        public static async Task<HttpResponseMessage> PostAsJsonAsync<TModel>(this HttpClient client, string requestUrl, TModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(requestUrl, stringContent);
            return result;
        }

        public static async Task<HttpResponseMessage> PutAsJsonAsync<TModel>(this HttpClient client, string requestUrl, TModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await client.PutAsync(requestUrl, stringContent);
            return result;
        }
    }
}
