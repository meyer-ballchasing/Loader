using Meyer.BallChasing.Client;
using Meyer.BallChasing.Models;
using Meyer.Common.Console;
using Meyer.Common.HttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Meyer.BallChasing.PullStats
{
    class Program
    {
        private static DirectoryInfo rootDirectory;
        private static ParsedReplayClient ballChasingClient;

        private static ConsoleParameters consoleParameters = new ConsoleParameters
        {
            NamedConsoleParameters =
            {
                new DirectoryConsoleParameter(new[] { "d" }, x =>
                {
                    if(!x.Exists)
                        throw new DirectoryNotFoundException(x.FullName);

                    if(!x.EnumerateFiles(Constants.SavedStateFileName).Any())
                        throw new Exception(x.FullName);

                    rootDirectory = x;
                }, "The path to the root directory containing the replay files to push", true),
                new ActionConsoleParameter(new[] { "key" }, x =>
                {
                    ballChasingClient = new ParsedReplayClient
                    (
                        new RestClient(UploadGroupClient.BaseEndpoint, new HttpClientOptions
                        {
                            RetryPolicy = new BackOffRetryPolicy()
                        }), x
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

            Group shadow = JsonConvert.DeserializeObject<Group>(await File.ReadAllTextAsync($"{rootDirectory.FullName}/{Constants.SavedStateFileName}"));

            await ballChasingClient.PullParsedReplays(shadow);

            await Output(shadow);
        }

        private static async Task Output(Group group)
        {
            foreach (var child in group.Children)
                await Output(child);

            await OutputGameSummary(group);
            await OutputMatchSummary(group);
        }

        private static async Task OutputGameSummary(Group group)
        {
            foreach (var item in group
                .Replays
                .Select(x => new { x.LocalFile, Stats = ReplayPlayerSummary.GetSummary(x) }).Select(x =>
                {
                    var output = new List<string>
                    {
                        "Name    Team    Mvp    Score    Goals    Assists    Saves    Shots    Cycles    Saviors    Inflicted    Taken    Id    Platform"
                    };
                    output.AddRange(x.Stats.Select(x => $"{x.Name}{Constants.Delimiter}{x.TeamName}{Constants.Delimiter}{x.Mvp}{Constants.Delimiter}{x.Score}{Constants.Delimiter}{x.Goals}{Constants.Delimiter}{x.Assists}{Constants.Delimiter}{x.Saves}{Constants.Delimiter}{x.Shots}{Constants.Delimiter}{x.Cycles}{Constants.Delimiter}{x.Saviors}{Constants.Delimiter}{x.Inflicted}{Constants.Delimiter}{x.Taken}{Constants.Delimiter}{x.Id}{Constants.Delimiter}{x.Platform}"));

                    return new
                    {
                        x.LocalFile,
                        Stats = output
                    };
                }))
                await File.WriteAllLinesAsync($"{item.LocalFile.Directory.FullName}/{item.LocalFile.Name}.txt", item.Stats);
        }

        private static async Task OutputMatchSummary(Group group)
        {
            var output = new List<string>
            {
                "Name    Team    Mvp    Score    Goals    Assists    Saves    Shots    Cycles    Saviors    Inflicted    Taken"
            };

            output.AddRange(GroupPlayerSummary.GetSummary(group)
                .Select(x => $"{x.Name}{Constants.Delimiter}{x.TeamName}{Constants.Delimiter}{x.Mvp}{Constants.Delimiter}{x.Score}{Constants.Delimiter}{x.Goals}{Constants.Delimiter}{x.Assists}{Constants.Delimiter}{x.Saves}{Constants.Delimiter}{x.Shots}{Constants.Delimiter}{x.Cycles}{Constants.Delimiter}{x.Saviors}{Constants.Delimiter}{x.Inflicted}{Constants.Delimiter}{x.Taken}")
            );

            if (output.Count > 1)
                await File.WriteAllLinesAsync($"{group.Replays.First().LocalFile.Directory.FullName}/{group.Replays.First().LocalFile.Directory.Name}.txt", output);
        }
    }
}