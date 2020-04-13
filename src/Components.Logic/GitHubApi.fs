module internal GitHubApi

open FSharp.Data
open System.Runtime.CompilerServices
open System
open System.Text

[<assembly: InternalsVisibleTo("Components.Logic.FSharp.Integration.Tests")>]
do()

[<Literal>]
let ApiBaseUrl = @"https://api.github.com"

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
            let headers = ["User-agent", userAgent; "Authorization", "Token " + authorizationToken; "Accept", "application/json"]
        
            let! sha = GetSha.execute userAgent repositoryName masterBranchName
            let post = Provider.Branch(@"refs/heads/" + newBranchName, sha)
            let! _ = post.JsonValue.RequestAsync (url = url, httpMethod = "POST", headers = headers)
            ()
        }

module UpdatFile =
    [<Literal>]
    let JsonGet = """
    {
        "content": "some_value",
        "path": "some_value",
        "sha": "some_value"
    }
    """
    
    [<Literal>]
    let JsonPost = """
    {
        "message": "some_value",
        "content": "some_value",
        "sha": "some_value",
        "branch": "some_value"
    }
    """

    type ProviderGet = JsonProvider<JsonGet>
    type ProviderPost = JsonProvider<JsonPost, RootName="newContent">
    
    let execute userAgent authorizationToken repositoryName branchName fileName content =
        async { 
            let fileGetUrl = sprintf @"%s/repos/%s/%s/contents/%s?ref=%s" ApiBaseUrl userAgent repositoryName fileName branchName
            let! response = fileGetUrl |> ProviderGet.AsyncLoad
            let currentContent = response.Content |> Convert.FromBase64String |> Encoding.UTF8.GetString
            
            let newContent = currentContent + content
            let newContentBase64string = newContent |> Encoding.UTF8.GetBytes |> Convert.ToBase64String
            
            let filePostUrl = sprintf @"%s/repos/%s/%s/contents/%s" ApiBaseUrl userAgent repositoryName fileName
            let headers = ["User-agent", userAgent; "Authorization", "Token " + authorizationToken; "Accept", "application/json"]
            let post = ProviderPost.NewContent("add comment", newContentBase64string, response.Sha, branchName)
            let! _ = post.JsonValue.RequestAsync (url = filePostUrl, httpMethod = "POST", headers = headers)
            ()
        }