namespace Components.GitHub
{
    using System.Net.Http;
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
    }
}
