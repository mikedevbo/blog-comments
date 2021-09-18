﻿namespace Bc.Contracts.Internals.Endpoint.GitHubPullRequestCreation.Messages

    open System

    type RequestCreateGitHubPullRequest(
                                           commentId: Guid,
                                           fileName: string,
                                           content: string,
                                           addedDate: DateTime
                                       ) =
        member this.CommentId = commentId
        member this.FileName = fileName
        member this.Content = content
        member this.AddedDate = addedDate

    type ResponseCreateGitHubPullRequest(commentId: Guid, pullRequestUri: string) =
        member this.CommentId = commentId
        member this.PullRequestUri = pullRequestUri

    type RequestCreateBranch(addedDate: DateTime) =
            member this.AddedDate = addedDate

    type ResponseCreateBranch(branchName: string) =
        member this.BranchName = branchName

    type RequestUpdateFile(branchName: string, fileName: string, content: string) =
        member this.BranchName = branchName
        member this.FileName = fileName
        member this.Content = content

    type ResponseUpdateFile() = class end

    type RequestCreatePullRequest(branchName: string) =
        member this.BranchName = branchName

    type ResponseCreatePullRequest(pullRequestUri: string) =
        member this.PullRequestUri = pullRequestUri