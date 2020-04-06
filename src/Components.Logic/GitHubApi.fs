module internal GitHubApi

open FSharp.Data
open System.Runtime.CompilerServices

[<assembly: InternalsVisibleTo("Components.Logic.FSharp.Integration.Tests")>]
do()

[<Literal>]
let ApiBaseUrl = @"https://api.github.com"

module GetSha =
    [<Literal>]
    let json = """
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

    type Provider = JsonProvider<json>

    let execute userAgent repositoryName branchName =
        async { 
            let url = sprintf @"%s/repos/%s/%s/git/refs/heads/%s" ApiBaseUrl userAgent repositoryName branchName
            let! response = url |> Provider.AsyncLoad
            return response.Object.Sha
        }

module CreateRepositoryBranch =
    [<Literal>]
    let json = """
    {
	    "ref": "some_value",
	    "sha": "some_value"
    }
    """

    type Provider = JsonProvider<json, RootName="branch">

    let execute userAgent authorizationToken repositoryName masterBranchName newBranchName =
        async {
            let url = sprintf @"%s/repos/%s/%s/git/refs" ApiBaseUrl userAgent repositoryName
            let headers = ["User-agent", userAgent; "Authorization", "Token " + authorizationToken; "Accept", "application/json"]
        
            let! sha = GetSha.execute userAgent repositoryName masterBranchName
            let newBranch = Provider.Branch(@"refs/heads/" + newBranchName, sha)
            let! _ = newBranch.JsonValue.RequestAsync (url = url, httpMethod = "POST", headers = headers)
            ()
        }