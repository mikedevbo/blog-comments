namespace Web.Integration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;
    using Common;
    using NUnit.Framework;
    using Simple.Data;
    using Web.Models;

    [TestFixture]
    [Ignore("only for manual tests")]
    public class CommentControllerTests
    {
        private IConfigurationManager configurationManager = new ConfigurationManager();

        [Test]
        public async Task Post_ForCommentData_NoException()
        {
            // Arrange
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:50537/")
            };

            var comment = new Comment
            {
                UserName = "testUser",
                UserEmail = "testUser@test.com",
                UserWebsite = "testUser.com",
                FileName = @"test.txt",
                Content = @"new comment",
            };

            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(comment);
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
