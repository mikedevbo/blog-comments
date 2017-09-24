namespace Components.GitHub.Dto
{
    public class RefResponse
    {
        public string Ref { get; set; }

        public string Url { get; set; }

        public RefObjectResponse Object { get; set; }
    }
}
