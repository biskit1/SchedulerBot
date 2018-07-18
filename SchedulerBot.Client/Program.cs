﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;

namespace SchedulerBot.Client
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        static DiscordClient Client { get; set; }

        static void Main(string[] args = null)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (String.IsNullOrWhiteSpace(environment))
            {
                throw new ArgumentNullException("Environment not found in ASPNETCORE_ENVIRONMENT");
            }

            Console.WriteLine($"Environment: {environment}");

            var builder = new ConfigurationBuilder();
            if (environment == "Development")
            {
                builder.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), string.Format("..{0}..{0}..{0}", Path.DirectorySeparatorChar)));
            }
            else
            {
                builder.SetBasePath(Directory.GetCurrentDirectory());
            }
            builder.AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json");

            Configuration = builder.Build();
            
            var serviceProvider = ConfigureServices(new ServiceCollection());

            // Bot

            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = Configuration.GetSection("Bot").GetValue<string>("Token"),
                TokenType = TokenType.Bot
            });

            Client.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower().StartsWith("ping"))
                {
                    await e.Message.RespondAsync("Pong!");
                }
            };

            Console.WriteLine("Connecting...");
            await Client.ConnectAsync();
            Console.WriteLine("Bot connected");
            await Task.Delay(-1);
        }

        static IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(options =>
            {
                options.AddConfiguration(Configuration.GetSection("Logging"));
                options.AddConsole();
            });

            return services.BuildServiceProvider();
        }
    }
}