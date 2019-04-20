using OmdbApi.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Consts
{
    public class AppSettingsParameters
    {
        public static string Secret = ConfigurationManager.AppSetting["AppSettings:Secret"];
    }
}
