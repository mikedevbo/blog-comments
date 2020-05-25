using Newtonsoft.Json;

namespace Components.GitHub.Dto
{
    public class ErrorResponseDto
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("errors")]
        public Error[] Errors { get; set; }

        public class Error
        {
            [JsonProperty("resource")]
            public string Resource { get; set; }

            [JsonProperty("field")]
            public string Field { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }
        }
    }
}
