﻿using Meyer.BallChasing.Client;
using Meyer.BallChasing.Models;
using Meyer.Common.Console;
using Meyer.Common.HttpClient;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Meyer.BallChasing.Push
{
    class Program
    {
        private static DirectoryInfo rootDirectory;
        private static UploadGroupClient ballChasingClient;

        private readonly static ConsoleParameters consoleParameters = new ConsoleParameters
        {
            NamedConsoleParameters =
            {
                new DirectoryConsoleParameter(new[] { "d" }, x =>
                {
                    if(!x.Exists)
                        throw new DirectoryNotFoundException(x.FullName);

                    if(!x.EnumerateFiles("*.replay", SearchOption.AllDirectories).Any())
                        throw new Exception(x.FullName);

                    rootDirectory = x;
                }, "The path to the root directory containing the replay files to push", true),
                new ActionConsoleParameter(new[] { "key" }, x =>
                {
                    ballChasingClient = new UploadGroupClient
                    (
                        new RestClient(UploadGroupClient.BaseEndpoint, new HttpClientOptions
                        {
                            RetryPolicy = new BackOffRetryPolicy()
                        }),
                        new FileUploadClient(UploadGroupClient.BaseEndpoint, new HttpClientOptions()), x
                    );
                }, "The access key for ballchasing.com/api", true)
            }
        };

        static async Task Main(string[] args)
        {
            try
            {
                consoleParameters.Map(args, false);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                Console.WriteLine(consoleParameters.ToString());

                Environment.Exit(-1);
            }

            Group localGroup = new Group(rootDirectory, null);

            if (rootDirectory.EnumerateFiles(Constants.SavedStateFileName).Any())
                localGroup.Merge(JsonConvert.DeserializeObject<Group>(await File.ReadAllTextAsync($"{rootDirectory.FullName}/{Constants.SavedStateFileName}")));

            await ballChasingClient.PushGroupRecursive(localGroup);

            await File.WriteAllTextAsync($"{rootDirectory.FullName}/{Constants.SavedStateFileName}", JsonConvert.SerializeObject(localGroup, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            }));
        }
    }
}