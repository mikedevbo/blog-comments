module GitHubApi

open System.Configuration
open FSharp.Data
open System
open System.Net
open System.Net.Http
open System.Text

// [<assembly: InternalsVisibleTo("Bc.Common.Logic.Integration.Tests")>]
// do()

[<Literal>]
let ApiBaseUrl = @"https://api.github.com"

let getBaseHeaders userAgent authorizationToken (etag: string option) =
     let baseHeaders = ["User-agent", userAgent; "Authorization", "Token " + authorizationToken; "Accept", "application/json"]
     match etag with
     | Some etag -> baseHeaders |> List.append ["If-None-Match", etag]
     | None -> baseHeaders

module GitHubConfigurationProvider =

    let userAgent =
        ConfigurationManager.AppSettings.["UserAgent"]

    let authorizationToken =
        ConfigurationManager.AppSettings.["AuthorizationToken"]

    let repositoryName =
        ConfigurationManager.AppSettings.["RepositoryName"]

    let masterBranchName =
        ConfigurationManager.AppSettings.["MasterBranchName"]

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
            let headers = getBaseHeaders userAgent authorizationToken None

            let! sha = GetSha.execute userAgent repositoryName masterBranchName
            let post = Provider.Branch(@"refs/heads/" + newBranchName, sha)
            let! _ = post.JsonValue.RequestAsync (url = url, httpMethod = "POST", headers = headers)
            ()
        }

module UpdateFile =
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
            let headers = getBaseHeaders userAgent authorizationToken None

            let put = Provider.NewContent("add comment", newContentBase64string, file.Sha, branchName)
            let! _ = put.JsonValue.RequestAsync (url = url, httpMethod = "PUT", headers = headers)
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
            let headers = getBaseHeaders userAgent authorizationToken None

            let post = Provider.Pull(headBranchName, "comment to merge", headBranchName, baseBranchName)
            let! response = post.JsonValue.RequestAsync (url = url, httpMethod = "POST", headers = headers)

            let locationHeader = response.Headers.TryFind "Location"
            let locationValue =
                match locationHeader with
                | Some location -> location
                | None -> raise (ArgumentException("There is no Location Header."))

            return locationValue
        }

module IsPullRequestOpen =
    [<Literal>]
    let Json = """
    {
        "state": "some_value"
    }
    """

    type Provider = JsonProvider<Json>
    type Result = {IsOpen: bool; Etag: string}

    let execute userAgent authorizationToken pullRequestUrl etag =
        async {
            let headers = getBaseHeaders userAgent authorizationToken etag
            let! response = Http.AsyncRequest(pullRequestUrl, headers = headers, silentHttpErrors = true)
            let etagHeader = response.Headers.TryFind "ETag"
            let etagValue =
                match etagHeader with
                | Some etag -> etag
                | None -> raise (ArgumentException("There is no ETag Header."))

            match response.StatusCode with
            | HttpStatusCodes.NotModified ->
                return {IsOpen = true; Etag = etagValue}
            | HttpStatusCodes.OK ->
                match response.Body with
                | Text body ->
                    let state = (Provider.Parse body).State
                    return {IsOpen = (state = "open"); Etag = etagValue}
                | _ -> return raise (ArgumentException("Invalid body format. Expected json as text."))
            | _ ->
                let ex = HttpRequestException(sprintf "Response bad status code: %d" response.StatusCode)
                ex.Data.Add("response", response)
                return raise ex
        }

module IsPullRequestMerged =
    let execute userAgent authorizationToken pullRequestUrl =
        async {
            let headers = getBaseHeaders userAgent authorizationToken None
            let url = sprintf "%s/merge" pullRequestUrl
            let! response = Http.AsyncRequest(url, headers = headers, silentHttpErrors = true)

            match response.StatusCode with
            | HttpStatusCodes.NoContent ->
                return true
            | HttpStatusCodes.NotFound ->
                return false
            | _ ->
                let ex = HttpRequestException(sprintf "Response bad status code: %d" response.StatusCode)
                ex.Data.Add("response", response)
                return raise ex
        }