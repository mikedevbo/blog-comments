namespace Messages.Messages
{
    public class CreateBranchResponse
    {
        public CreateBranchResponse(string createdBranchName)
        {
            this.CreatedBranchName = createdBranchName;
        }

        public string CreatedBranchName { get; }
    }
}
