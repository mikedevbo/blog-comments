namespace Bc.Logic.Endpoint.CommentAnswer

open System
open System.Configuration
open Bc.Contracts.Internals.Endpoint.CommentAnswer.Logic

module private ConfigurationProvider =
    let checkCommentAnswerTimeoutInSeconds =
        Convert.ToInt32(ConfigurationManager.AppSettings.["CheckCommentAnswerTimeoutInSeconds"])

type CommentAnswerPolicyLogic() =
    interface ICommentAnswerPolicyLogic with
        member this.CheckCommentAnswerTimeoutInSeconds =
            ConfigurationProvider.checkCommentAnswerTimeoutInSeconds

type CommentAnswerPolicyLogicFake() =
    interface ICommentAnswerPolicyLogic with
        member this.CheckCommentAnswerTimeoutInSeconds = 5