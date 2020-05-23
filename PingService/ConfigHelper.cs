using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace PingService
{
    public static class ConfigHelper
    {
        public static ConfigModel Config = new ConfigModel();
        private static IConfigurationRoot _configuration;

        public static void InitConfiguration(string environmentName)
        {
            try
            {
                var configbuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.json", true, true);

                configbuilder.AddEnvironmentVariables();

                _configuration = configbuilder.Build();

                _configuration.Bind("Config", Config);

                Config.WakingTimeParsed = DateTime.Parse(_configuration.GetSection("Config").GetValue<string>("WakingTime"));

                Console.WriteLine("Configuration build succeded");
            }
            catch ( Exception e )
            {
                Console.WriteLine("Configuration build error");
                Console.WriteLine(e);
                throw;
            }

        }


        public static IConfigurationRoot GetConfig(bool isMigration = false)
        {
            return _configuration;
        }

    }

    public class ConfigModel
    {
        public string WakingUrl { get; set; }
        public DateTime WakingTimeParsed { get; set; }
        public string WakingTime { get; set; }
        public int TriesCount { get; set; }
    }


}
