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

                    if(!x.EnumerateFiles(SavedStateFileName).Any())
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

        private const string SavedStateFileName = "shadow.json";

        static async Task Main(string[] args)
        {
            consoleParameters.Map(args, false);

            Group shadow = JsonConvert.DeserializeObject<Group>(await File.ReadAllTextAsync($"{rootDirectory.FullName}/{SavedStateFileName}"));

            await ballChasingClient.PullParsedReplays(shadow);

            await Output(shadow);
        }

        private static async Task Output(Group group)
        {
            foreach (var child in group.Children)
                await Output(child);

            await OutputGameStats(group);
            await OutputMatchStats(group);
        }

        private static async Task OutputGameStats(Group group)
        {
            foreach (var item in group
                .Replays
                .Select(x => new { x.LocalFile, Stats = x.GetSummary() }).Select(x =>
                {
                    var output = new List<string>
                    {
                        "Name    Team    Mvp    Score    Goals    Assists    Saves    Shots    Cycles    Saviors    Inflicted    Taken    Id    Platform"
                    };
                    output.AddRange(x.Stats.Select(x => $"{x.Name}    {x.TeamName}    {x.Mvp}    {x.Score}    {x.Goals}    {x.Assists}    {x.Saves}    {x.Shots}    {x.Cycles}    {x.Saviors}    {x.Inflicted}    {x.Taken}    {x.Id}    {x.Platform}"));

                    return new
                    {
                        x.LocalFile,
                        Stats = output
                    };
                }))
                await File.WriteAllLinesAsync($"{item.LocalFile.Directory.FullName}/{item.LocalFile.Name}.txt", item.Stats);
        }

        private static async Task OutputMatchStats(Group group)
        {
            var output = new List<string>
            {
                "Name    Team    Mvp    Score    Goals    Assists    Saves    Shots    Cycles    Saviors    Inflicted    Taken"
            };

            output.AddRange(group.GetSummary()
                .Select(x => $"{x.Name}    {x.TeamName}    {x.Mvp}    {x.Score}    {x.Goals}    {x.Assists}    {x.Saves}    {x.Shots}    {x.Cycles}    {x.Saviors}    {x.Inflicted}    {x.Taken}")
            );

            if (output.Count > 1)
                await File.WriteAllLinesAsync($"{group.Replays.First().LocalFile.Directory.FullName}/{group.Replays.First().LocalFile.Directory.Name}.txt", output);
        }
    }
}