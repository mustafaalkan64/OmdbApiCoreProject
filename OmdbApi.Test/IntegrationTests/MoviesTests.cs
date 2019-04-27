using GST.Fake.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using OmdbApi.Api;
using OmdbApi.DAL.Consts;
using OmdbApi.Business.Helpers;
using OmdbApi.DAL.EFDbContext;
using OmdbApi.DAL.Helpers;
using OmdbApi.DAL.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OmdbApi.Test.IntegrationTests
{
    public class MoviesTests : IClassFixture<TestFixture<Startup>>
    {
        private HttpClient Client;
        private User user;
        private string secretKey;

        public MoviesTests(TestFixture<Startup> fixture)
        {
            Client = fixture.Client;
            user = new User
            {
                Id = 2,
                Username = "mustafa.alkan",
                Email = "mustafaalkan64@gmail.com",
                FirstName = "mustafa",
                LastName = "Alkan"
            };
            secretKey = AppSettingsParameters.Secret;
        }

        [Fact]
        public async Task SearchMovieTestWithAuthorize()
        {
            var titleParam = "Blade";
            // Arrange
            var request = $"/api/Movie/SearchMovie?title={titleParam}";
            // Act
            var token = JWTManager.CreateToken(user, secretKey);
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await Client.GetAsync(request);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var movieResponse = JsonConvert.DeserializeObject<MovieResponse>(jsonResponse);
            response.EnsureSuccessStatusCode();

            // Asserts
            Assert.NotNull(movieResponse);
            Assert.Equal(movieResponse.Response, true);
            Assert.Equal(movieResponse.Error, null);
            Assert.Equal(movieResponse.Title, titleParam);

        }

        [Fact]
        public async Task SearchMovieTestWithNullTitle()
        {
            var titleParam = "";
            // Arrange
            var request = $"/api/Movie/SearchMovie?title={titleParam}";
            // Act
            var token = JWTManager.CreateToken(user, secretKey);
            Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await Client.GetAsync(request);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var movieResponse = JsonConvert.DeserializeObject<MovieResponse>(jsonResponse);

            // Asserts
            Assert.NotNull(movieResponse);
            Assert.Equal(movieResponse.Response, false);
            Assert.Equal(response.StatusCode, HttpStatusCode.BadRequest);
            Assert.NotEqual(movieResponse.Error, null);
        }

        [Fact]
        public async Task SearchMovieTest()
        {
            var titleParam = "blade";
            // Arrange
            var request = $"/api/Movie/SearchMovie?title={titleParam}";

            var response = await Client.GetAsync(request);
            // Assert
            Assert.Equal(response.StatusCode, HttpStatusCode.Unauthorized);

        }
    }
}
