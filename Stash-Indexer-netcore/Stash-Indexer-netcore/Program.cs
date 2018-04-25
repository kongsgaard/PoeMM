﻿using System;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Stash_Indexer_netcore
{
    class Program
    {
        public static IConfiguration Configuration = BuildJsonConfiguration("AppSettings.json");
        public static string DataDir = $"{Configuration["DataDir"]}";

        static void Main(string[] args)
        {
            log4net.GlobalContext.Properties["TransactionLogFileName"] = DataDir + "Logs\\TransactionLog";
            log4net.GlobalContext.Properties["ErrorLogFileName"] = DataDir + "Logs\\ErrorLog";
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            StashApiRequester apiRequester = new StashApiRequester(DataDir);
            apiRequester.InitializeFromCheckpoint();

            ConsoleMonitor monitor = new ConsoleMonitor(apiRequester, 500);

            Task tsk1 = new Task(monitor.StartConsoleMonitor);
            Task tsk = new Task(apiRequester.StartRequesting);

            tsk1.Start();
            tsk.Start();
            
            tsk.Wait();

            
            Console.WriteLine("Done");


            Console.ReadLine();
        }

        public static IConfiguration BuildJsonConfiguration(string jsonFile)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(jsonFile);
            return builder.Build();

        }
    }
}
