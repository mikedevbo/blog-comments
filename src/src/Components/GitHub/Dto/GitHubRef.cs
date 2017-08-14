namespace Components.GitHub.Dto
{
    using Newtonsoft.Json;

    public class GitHubRef
    {
        [JsonProperty("ref")]
        public string Ref { get; set; }

        [JsonProperty("sha")]
        public string Sha { get; set; }
    }
}
