namespace Components.Logic

open Messages
open Messages.Messages
open System.Threading.Tasks

type CommentPolicyLogic(convigurationProvider: IConfigurationProvider) =
    member this.configurationProvider = convigurationProvider
    interface ICommentPolicyLogic with
        member this.CreateRepositoryBranch(message: RequestCreateBranch): Task<CreateBranchResponse> = 
            //GitHubApi.createRepositoryBranch
            raise (System.NotImplementedException())
        
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
            Logic.printfn "CreateRepositoryBranch" (new CreateBranchResponse())

        member this.UpdateFile(message: RequestAddComment): Task<AddCommentResponse> = 
            Logic.printfn "UpdateFile" (new AddCommentResponse())

        member this.CreatePullRequest(message: RequestCreatePullRequest): Task<CreatePullRequestResponse> = 
            Logic.printfn "CreatePullRequest" (new CreatePullRequestResponse())

        member this.CheckCommentAnswer(message: RequestCheckCommentAnswer): Task<CheckCommentAnswerResponse> =
            let response = new CheckCommentAnswerResponse()
            response.Status <- CommentAnswerStatus.Approved
            response.ETag <- "1234"
            Logic.printfn "CheckCommentAnswer" response