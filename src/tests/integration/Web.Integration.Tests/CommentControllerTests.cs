namespace Web.Integration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;
    using NUnit.Framework;
    using Simple.Data;
    using Web.Models;

    [TestFixture]
    ////[Ignore("do not auto run")]
    public class CommentControllerTests
    {
        private IConfigurationManager configurationManager = new ConfigurationManager();

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

            var db = Database.OpenConnection(this.configurationManager.NsbTransportConnectionString);

            db.SagaTestResults.DeleteAll();

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
                return;
            }

            await Task.Delay(15000).ConfigureAwait(false);

            List<ResultRow> result = db
                    .SagaTestResults
                    .All()
                    .ToList<ResultRow>();

            Assert.NotNull(result.FirstOrDefault(row => row.Result == 1));
            Assert.NotNull(result.FirstOrDefault(row => row.Result == 2));
            Assert.NotNull(result.FirstOrDefault(row => row.Result == 3));
            Assert.NotNull(result.FirstOrDefault(row => row.Result == 4));
            Assert.NotNull(result.FirstOrDefault(row => row.Result == 5));
        }

        public class ResultRow
        {
            public int Id { get; set; }

            public int Result { get; set; }
        }
    }
}
