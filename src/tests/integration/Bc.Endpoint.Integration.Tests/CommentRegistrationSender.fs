module Bc.Endpoint.Integration.Tests.CommentRegistrationSender

open System
open System.Threading.Tasks
open Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands
open NServiceBus
open NUnit.Framework

[<Ignore("Only for manual tests")>]
module Sender =

    let sendMessage<'T> (message:'T) =
        async {
            let! endpoint = EndpointFactory.getSenderEndpoint () |> Async.AwaitTask
            do! endpoint.Send(message) |> Async.AwaitTask
            do! endpoint.Stop() |> Async.AwaitTask
        } |> Async.RunSynchronously

    [<Test>]
    let registerComment_send_noException () =

        // Arrange
        let commentId = Guid.NewGuid()
        let userName = "test_user"
        let userWebsite = "test_user_website"
        let userComment = "new_comment"
        let articleFileName = @"_posts/2018-05-27-test.md"
        let addedDate = DateTime.UtcNow

        let message =
            RegisterComment (
                commentId,
                userName,
                userWebsite,
                userComment,
                articleFileName,
                addedDate
            )

        // Act
        sendMessage<RegisterComment> message

        // Assert
        Assert.Pass()

// namespace Bc.Endpoint.Integration.Tests
// {
//     [TestFixture]
//     [Ignore("Only for manual tests")]
//     public class CommentRegistrationSender
//     {
//         private IEndpointInstance endpointInstance;

//         [SetUp]
//         public async Task SetUp()
//         {
//             this.endpointInstance = await EndpointFactory.GetSenderEndpoint().ConfigureAwait(false);
//         }

//         [TearDown]
//         public Task TearDown()
//         {
//             return this.endpointInstance.Stop();
//         }

//         [Test]
//         public async Task RegisterComment_Send_NoException()
//         {
//             // Arrange
//             var commentId = Guid.NewGuid();
//             const string userName = "test_user";
//             const string userWebsite = "test_user_website";
//             const string userComment = "new_comment";
//             const string articleFileName = @"_posts/2018-05-27-test.md";
//             var addedDate = DateTime.UtcNow;

//             var message = new RegisterComment(
//                 commentId,
//                 userName,
//                 userWebsite,
//                 userComment,
//                 articleFileName,
//                 addedDate);

//             // Act
//             await this.endpointInstance.Send(message).ConfigureAwait(false);

//             // Assert
//             Assert.Pass();
//         }
//     }
// }