using Microsoft.Extensions.Configuration;
using System.IO;

namespace OmdbApi.DAL.Helpers
{
    public static class ConfigurationManager
    {
        public static IConfiguration AppSetting { get; }

        static ConfigurationManager()
        {
            AppSetting = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.DAL.json")
                    .Build();
        }
    }
}
