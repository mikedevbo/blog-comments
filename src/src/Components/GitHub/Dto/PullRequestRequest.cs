namespace Components.GitHub.Dto
{
    using Newtonsoft.Json;

    public class PullRequestRequest
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("head")]
        public string Head { get; set; }

        [JsonProperty("base")]
        public string Base { get; set; }
    }
}
