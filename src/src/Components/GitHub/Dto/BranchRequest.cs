namespace Components.GitHub.Dto
{
    using Newtonsoft.Json;

    public class BranchRequest
    {
        [JsonProperty("ref")]
        public string Ref { get; set; }

        [JsonProperty("sha")]
        public string Sha { get; set; }
    }
}
