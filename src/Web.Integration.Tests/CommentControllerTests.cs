namespace Web.Integration.Tests
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Web.Models;

    [TestFixture]
    [Ignore("only for manual tests")]
    public class CommentControllerTests
    {
        [Test]
        public async Task Post_ForCommentData_NoException_Async()
        {
            // Arrange
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:62974/"),
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var comment = new Comment
            {
                UserName = "testUser",
                UserEmail = string.Empty,
                UserWebsite = "testUser.com",
                FileName = @"_posts/2018-05-27-test.md",
                Content = @"new comment",
            };

            var json = JsonConvert.SerializeObject(comment);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            HttpResponseMessage response = await client.PostAsync("comment", stringContent)
                .ConfigureAwait(false);

            // Assert
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Assert.True(false);
                return;
            }

            // Assert
            Assert.True(true);
        }
    }
}
