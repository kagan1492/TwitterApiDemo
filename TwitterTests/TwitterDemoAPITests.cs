using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json; 
using System.Net;
using System.Net.Http; 
using System.Threading.Tasks;
using TwitterModels;
using Xunit;

namespace TwitterTests
{
    public class TwitterDemoAPITests : IClassFixture<WebApplicationFactory<TwitterDemoAPI.Startup>>
    {
        private HttpClient _client { get; }

        public TwitterDemoAPITests(WebApplicationFactory<TwitterDemoAPI.Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                BaseAddress = new System.Uri("https://localhost:57288/twitter/api/")
            }); 
        } 

        [Fact]
        public async Task Stats_Should_Return_Data()
        {
            // Arrange   
            var response = await _client.GetAsync("stats");
            response.EnsureSuccessStatusCode();

            // Act 
            var data = await response.Content.ReadAsStringAsync();
            var stats = JsonConvert.DeserializeObject<TwitterStatModel>(data);

            //Assert  
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(stats != null);
        }

        
    }
}
