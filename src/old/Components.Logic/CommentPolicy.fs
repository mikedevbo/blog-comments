namespace Components.Logic

open Messages
open Messages.Messages
open System.Threading.Tasks

type CommentPolicyLogic(configurationProvider: IConfigurationProvider) =
    member this.configurationProvider = configurationProvider
    interface ICommentPolicyLogic with
        member this.CreateRepositoryBranch(message: RequestCreateBranch): Task<CreateBranchResponse> = 
             async {
                let branchName = sprintf "c-%s" (message.AddedDate.ToString("yyyy-MM-dd-HH-mm-ss-fff"))
                do! GitHubApi.CreateRepositoryBranch.execute this.configurationProvider.UserAgent
                        this.configurationProvider.AuthorizationToken
                        this.configurationProvider.RepositoryName
                        this.configurationProvider.MasterBranchName
                        branchName
                return CreateBranchResponse(branchName)
             } |> Async.StartAsTask

        member this.UpdateFile(message: RequestAddComment): Task<AddCommentResponse> = 
            raise (System.NotImplementedException())
        
        member this.CreatePullRequest(message: RequestCreatePullRequest): Task<CreatePullRequestResponse> = 
            raise (System.NotImplementedException())
        
        member this.CheckCommentAnswer(message: RequestCheckCommentAnswer): Task<CheckCommentAnswerResponse> = 
            raise (System.NotImplementedException())

module private Logic =
    let printfn printValue (returnValue: 'a) =
        async {
            printfn "%s" printValue
            return returnValue
        } |> Async.StartAsTask

type CommentPolicyLogicFake() =
    interface ICommentPolicyLogic with
        member this.CreateRepositoryBranch(message: RequestCreateBranch): Task<CreateBranchResponse> = 
            Logic.printfn "CreateRepositoryBranch" (new CreateBranchResponse("test_branch"))

        member this.UpdateFile(message: RequestAddComment): Task<AddCommentResponse> = 
            Logic.printfn "UpdateFile" (new AddCommentResponse())

        member this.CreatePullRequest(message: RequestCreatePullRequest): Task<CreatePullRequestResponse> = 
            Logic.printfn "CreatePullRequest" (new CreatePullRequestResponse())

        member this.CheckCommentAnswer(message: RequestCheckCommentAnswer): Task<CheckCommentAnswerResponse> =
            let response = new CheckCommentAnswerResponse()
            response.Status <- CommentAnswerStatus.Approved
            response.ETag <- "1234"
            Logic.printfn "CheckCommentAnswer" response