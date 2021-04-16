using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Xunit;

namespace AITIssueTracker.Test.v0
{
    public class UserTest : BaseTest
    {
        public UserTest() : base("http://localhost", 5000)
        {
            
        }

        [Fact]
        public async Task SaveNewUserAsync()
        {
            // Arrange
            string url = "api/user";
            Version = "0.0";
            UserForm newUser = new UserForm
            {
                Username = "Fabian",
            };

            // Act
            HttpResponseMessage response = await ApiClient.PostAsJsonAsync(url, newUser);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetAllUsersAsync()
        {
            // Arrange
            string url = "api/user";
            Version = "0.0";

            // Act
            HttpResponseMessage response = await ApiClient.GetAsync(url);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.IsType<List<UserView>>(await response.Content.ReadAsAsync<List<UserView>>());
        }
    }
}
