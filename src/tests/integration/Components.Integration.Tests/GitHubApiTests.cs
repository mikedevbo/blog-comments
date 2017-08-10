using Components.GitHub;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Integration.Tests
{
    [TestFixture]
    public class GitHubApiTests
    {
        ///private readonly IConfigurationManager configurationManager = 

        [Test]
        public void T()
        {
            // Arrange
            var api = this.GetGitHubApi();

            // Act
            ///api.GetRepository()

            // Assert
        }

        private GitHubApi GetGitHubApi()
        {
            return new GitHubApi();
        }
    }
}
