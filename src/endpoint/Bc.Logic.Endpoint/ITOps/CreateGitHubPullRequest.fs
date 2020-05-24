module Bc.Logic.Endpoint.ITOps.CreateGitHubPullRequest

open System.Configuration
open System.Threading.Tasks
open Bc.Contracts.Internals.Endpoint.ITOps.CreateGitHubPullRequest

type PolicyLogic(configurationProvider: IConfigurationProvider) =
        member this.ConfigurationProvider = configurationProvider
        interface IPolicyLogic with
            member this.CreateBranch(creationDate) =
                async {
                    return "test"
                } |> Async.StartAsTask

            member this.UpdateFile(branchName, fileName, content) =
                async {
                    ()
                } |> Async.StartAsTask :> Task

            member this.CreatePullRequest(branchName) =
                async {
                    return "uri"
                } |> Async.StartAsTask
                
type ConfigurationProvider() =
    interface IConfigurationProvider with
        member this.UserAgent =
            ConfigurationManager.AppSettings.["UserAgent"];

        member this.AuthorizationToken =
            ConfigurationManager.AppSettings.["AuthorizationToken"];

        member this.RepositoryName =
            ConfigurationManager.AppSettings.["RepositoryName"];

        member this.MasterBranchName =
            ConfigurationManager.AppSettings.["MasterBranchName"];

type PolicyLogicFake() =
    interface IPolicyLogic with
        member this.CreateBranch(_) =
            async {
                return "branch_1234"
            } |> Async.StartAsTask

        member this.UpdateFile(_, _, _) =
            async {
                ()
            } |> Async.StartAsTask :> Task

        member this.CreatePullRequest(_) =
            async {
                return "uri_1234"
            } |> Async.StartAsTask