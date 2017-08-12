namespace Web.Integration.Tests
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;
    using NUnit.Framework;
    using Web.Models;

    [TestFixture]
    ////[Ignore("do not auto run")]
    public class CommentControllerTests
    {
        [Test]
        public async Task Post_ForCommentData_FullProcessShouldBeSuccess()
        {
            // Arrange
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:49866/");
            var comment = new Comment { Id = Guid.NewGuid() };

            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(comment);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            ////return await client.PostAsync(requestUrl, stringContent);

            // Act
            HttpResponseMessage response = await client.PostAsync("Comment/RequestForComment", stringContent);

            // Assert
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Assert.True(false);
            }
        }
    }
}
