﻿using GST.Fake.Authentication.JwtBearer;
using Newtonsoft.Json;
using OmdbApi.Api;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OmdbApi.Test
{
    public class MoviesTests : IClassFixture<TestFixture<Startup>>
    {
        private HttpClient Client;

        public MoviesTests(TestFixture<Startup> fixture)
        {
            Client = fixture.Client;
            fixture.Client.SetFakeBearerToken("admin", new[] { "ROLE_ADMIN", "ROLE_GENTLEMAN" });
        }

        [Fact]
        public async Task TestGetStockItemsAsync()
        {
            // Arrange
            var request = "/api/Movie/SearchMovie?title=blade";
            // Act
            var response = await Client.GetAsync(request);
            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestPostStockItemAsync()
        {
            // Arrange
            var request = new
            {
                Url = "/api/v1/Warehouse/StockItem",
                Body = new
                {
                    StockItemName = string.Format("USB anime flash drive - Vegeta {0}", Guid.NewGuid()),
                    SupplierID = 12,
                    UnitPackageID = 7,
                    OuterPackageID = 7,
                    LeadTimeDays = 14,
                    QuantityPerOuter = 1,
                    IsChillerStock = false,
                    TaxRate = 15.000m,
                    UnitPrice = 32.00m,
                    RecommendedRetailPrice = 47.84m,
                    TypicalWeightPerUnit = 0.050m,
                    CustomFields = "{ \"CountryOfManufacture\": \"Japan\", \"Tags\": [\"32GB\",\"USB Powered\"] }",
                    Tags = "[\"32GB\",\"USB Powered\"]",
                    SearchDetails = "USB anime flash drive - Vegeta",
                    LastEditedBy = 1,
                    ValidFrom = DateTime.Now,
                    ValidTo = DateTime.Now.AddYears(5)
                }
            };

            // Act
            var response = await Client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestPutStockItemAsync()
        {
            // Arrange
            var request = new
            {
                Url = "/api/v1/Warehouse/StockItem/1",
                Body = new
                {
                    StockItemName = string.Format("USB anime flash drive - Vegeta {0}", Guid.NewGuid()),
                    SupplierID = 12,
                    Color = 3,
                    UnitPrice = 39.00m
                }
            };

            // Act
            var response = await Client.PutAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            // Assert
            response.EnsureSuccessStatusCode();
        }

        //[Fact]
        //public async Task TestDeleteStockItemAsync()
        //{
        //    // Arrange

        //    var postRequest = new
        //    {
        //        Url = "/api/v1/Warehouse/StockItem",
        //        Body = new
        //        {
        //            StockItemName = string.Format("Product to delete {0}", Guid.NewGuid()),
        //            SupplierID = 12,
        //            UnitPackageID = 7,
        //            OuterPackageID = 7,
        //            LeadTimeDays = 14,
        //            QuantityPerOuter = 1,
        //            IsChillerStock = false,
        //            TaxRate = 10.000m,
        //            UnitPrice = 10.00m,
        //            RecommendedRetailPrice = 47.84m,
        //            TypicalWeightPerUnit = 0.050m,
        //            CustomFields = "{ \"CountryOfManufacture\": \"USA\", \"Tags\": [\"Sample\"] }",
        //            Tags = "[\"Sample\"]",
        //            SearchDetails = "Product to delete",
        //            LastEditedBy = 1,
        //            ValidFrom = DateTime.Now,
        //            ValidTo = DateTime.Now.AddYears(5)
        //        }
        //    };

        //    // Act
        //    var postResponse = await Client.PostAsync(postRequest.Url, ContentHelper.GetStringContent(postRequest.Body));
        //    var jsonFromPostResponse = await postResponse.Content.ReadAsStringAsync();

        //    var singleResponse = JsonConvert.DeserializeObject<SingleResponse<StockItem>>(jsonFromPostResponse);

        //    var deleteResponse = await Client.DeleteAsync(string.Format("/api/v1/Warehouse/StockItem/{0}", singleResponse.Model.StockItemID));

        //    // Assert
        //    postResponse.EnsureSuccessStatusCode();

        //    Assert.False(singleResponse.DidError);

        //    deleteResponse.EnsureSuccessStatusCode();
        //}
    }
}
