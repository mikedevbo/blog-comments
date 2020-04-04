namespace Components.Logic

open Messages
open Messages.Messages
open System.Threading.Tasks
open System

module private Logic =
    let printfn printValue (returnValue: 'a) =
        async {
            printfn "%s" printValue
            return returnValue
        } |> Async.StartAsTask

type CommentPolicyLogic() =
    interface ICommentPolicyLogic with
        member this.CreateRepositoryBranch(message: RequestCreateBranch): Task<CreateBranchResponse> = 
            Logic.printfn "CreateRepositoryBranch" (new CreateBranchResponse())

        member this.UpdateFile(message: RequestAddComment): Task<AddCommentResponse> = 
            Logic.printfn "UpdateFile" (new AddCommentResponse())

        member this.CreatePullRequest(message: RequestCreatePullRequest): Task<CreatePullRequestResponse> = 
            Logic.printfn "CreatePullRequest" (new CreatePullRequestResponse())

        member this.CheckCommentAnswer(message: RequestCheckCommentAnswer): Task<CheckCommentAnswerResponse> = 
            Logic.printfn "CheckCommentAnswer" (new CheckCommentAnswerResponse())
