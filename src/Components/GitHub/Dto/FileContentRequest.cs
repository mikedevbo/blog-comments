namespace Components.GitHub.Dto
{
    using Newtonsoft.Json;

    public class FileContentRequest
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("sha")]
        public string Sha { get; set; }

        [JsonProperty("branch")]
        public string Branch { get; set; }
    }
}
