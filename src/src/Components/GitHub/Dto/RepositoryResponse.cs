namespace Components.GitHub.Dto
{
    public class RepositoryResponse
    {
        public string Ref { get; set; }

        public string Url { get; set; }

        public RefObject Object { get; set; }
    }
}
