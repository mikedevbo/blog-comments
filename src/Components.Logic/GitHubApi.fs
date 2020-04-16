module internal GitHubApi

open FSharp.Data
open System.Runtime.CompilerServices
open System
open System.Text

[<assembly: InternalsVisibleTo("Components.Logic.FSharp.Integration.Tests")>]
do()

[<Literal>]
let ApiBaseUrl = @"https://api.github.com"

let getBaseHeaders userAgent authorizationToken =
     ["User-agent", userAgent; "Authorization", "Token " + authorizationToken; "Accept", "application/json"]

module GetSha =
    [<Literal>]
    let Json = """
    {
	    "ref": "some_value",
	    "node_id": "some_value",
	    "url": "some_value",
	    "object":
        {
		    "sha": "some_value",
		    "type": "some_value",
		    "url": "some_value"
        }
    }
    """

    type Provider = JsonProvider<Json>

    let execute userAgent repositoryName branchName =
        async { 
            let url = sprintf @"%s/repos/%s/%s/git/refs/heads/%s" ApiBaseUrl userAgent repositoryName branchName
            let! response = url |> Provider.AsyncLoad
            return response.Object.Sha
        }

module GetFile =
    [<Literal>]
    let Json = """
    {
        "content": "some_value",
        "path": "some_value",
        "sha": "some_value"
    }
    """

    type Provider = JsonProvider<Json>

    let execute userAgent repositoryName fileName branchName =
        async { 
            let url = sprintf @"%s/repos/%s/%s/contents/%s?ref=%s" ApiBaseUrl userAgent repositoryName fileName branchName
            let! response = url |> Provider.AsyncLoad
            return response
        }

module CreateRepositoryBranch =
    [<Literal>]
    let Json = """
    {
	    "ref": "some_value",
	    "sha": "some_value"
    }
    """

    type Provider = JsonProvider<Json, RootName="branch">

    let execute userAgent authorizationToken repositoryName masterBranchName newBranchName =
        async {
            let url = sprintf @"%s/repos/%s/%s/git/refs" ApiBaseUrl userAgent repositoryName
            let headers = getBaseHeaders userAgent authorizationToken
        
            let! sha = GetSha.execute userAgent repositoryName masterBranchName
            let post = Provider.Branch(@"refs/heads/" + newBranchName, sha)
            let! _ = post.JsonValue.RequestAsync (url = url, httpMethod = "POST", headers = headers)
            ()
        }

module UpdatFile = 
    [<Literal>]
    let Json = """
    {
        "message": "some_value",
        "content": "some_value",
        "sha": "some_value",
        "branch": "some_value"
    }
    """

    type Provider = JsonProvider<Json, RootName="newContent">
    
    let execute userAgent authorizationToken repositoryName fileName branchName content =
        async {
            let! file = GetFile.execute userAgent repositoryName fileName branchName
            let currentContent = file.Content |> Convert.FromBase64String |> Encoding.UTF8.GetString
            
            let newContent = currentContent + content
            let newContentBase64string = newContent |> Encoding.UTF8.GetBytes |> Convert.ToBase64String
            
            let url = sprintf @"%s/repos/%s/%s/contents/%s" ApiBaseUrl userAgent repositoryName fileName
            let headers = getBaseHeaders userAgent authorizationToken

            let post = Provider.NewContent("add comment", newContentBase64string, file.Sha, branchName)
            let! _ = post.JsonValue.RequestAsync (url = url, httpMethod = "PUT", headers = headers)
            ()
        }

module CreatePullRequest =
    [<Literal>]
    let Json = """
    {
        "title": "some_value",
        "body": "some_value",
        "head": "some_value",
        "base": "some_value"
    }
    """

    type Provider = JsonProvider<Json, RootName="pull">

    let execute userAgent authorizationToken repositoryName headBranchName baseBranchName =
        async {
            let url = sprintf @"%s/repos/%s/%s/pulls" ApiBaseUrl userAgent repositoryName
            let headers = getBaseHeaders userAgent authorizationToken

            let post = Provider.Pull(headBranchName, "comment to merge", headBranchName, baseBranchName)
            let! _ = post.JsonValue.RequestAsync (url = url, httpMethod = "POST", headers = headers)
            ()
        }

module IsPullRequesetOpen =
    [<Literal>]
    let Json = """
    {
        "title": "some_value",
        "body": "some_value",
        "head": "some_value",
        "base": "some_value"
    }
    """

    type Provider = JsonProvider<Json, RootName="pull">
    type result = {isOpen: bool; etag: string}

    let execute userAgent authorizationToken pullRequestUrl etag =
        async {
            let! response = Http.AsyncRequest(pullRequestUrl, silentHttpErrors = true)
            match response.StatusCode with
            | 304 ->
                let etag = response.Headers.TryFind "ETag"
                match etag with
                | Some x -> return {isOpen = true; etag = x}
                | None -> return raise (new ArgumentException("There is no ETag Header"))
            | 200 ->
                return {isOpen = true; etag = "aaa"}
            
            
            
            
            //let url = sprintf @"%s/repos/%s/%s/pulls" ApiBaseUrl userAgent repositoryName
            //let headers = getBaseHeaders userAgent authorizationToken

            //let post = Provider.Pull(headBranchName, "comment to merge", headBranchName, baseBranchName)
            //let! _ = post.JsonValue.RequestAsync (url = url, httpMethod = "POST", headers = headers)
            //()
        }