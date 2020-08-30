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

        private const string SavedStateFileName = "ballchasing.json";

        static async Task Main(string[] args)
        {
            consoleParameters.Map(args, false);

            Group ballChasingGroup = JsonConvert.DeserializeObject<Group>(await File.ReadAllTextAsync($"{rootDirectory.FullName}/{SavedStateFileName}"));

            await ballChasingClient.PullParsedReplays(ballChasingGroup);

            await AAA(ballChasingGroup);
        }

        private static async Task AAA(Group group)
        {
            foreach (var child in group.Children)
                await AAA(child);

            var stats = group
                .Replays
                .Select(x => new
                {
                    ReplayId = x.BallChasingId,
                    OrangeStats = x.ProcessedReplay.Orange.Players.Select(y => new
                    {
                        y.Name,
                        Team = 0,
                        TeamName = x.ProcessedReplay.Orange.Name,
                        y.Stats.Core.Mvp,
                        y.Stats.Core.Score,
                        y.Stats.Core.Goals,
                        y.Stats.Core.Assists,
                        y.Stats.Core.Saves,
                        y.Stats.Core.Shots,
                        y.Id.Platform,
                        y.Id.Id
                    }),
                    BlueStats = x.ProcessedReplay.Blue.Players.Select(y => new
                    {
                        y.Name,
                        Team = 0,
                        TeamName = x.ProcessedReplay.Orange.Name,
                        y.Stats.Core.Mvp,
                        y.Stats.Core.Score,
                        y.Stats.Core.Goals,
                        y.Stats.Core.Assists,
                        y.Stats.Core.Saves,
                        y.Stats.Core.Shots,
                        y.Id.Platform,
                        y.Id.Id
                    })
                })
                .Select(x => new
                {
                    x.ReplayId,
                    Stats = x.BlueStats
                        .Union(x.OrangeStats)
                        .OrderByDescending(x => x.Team)
                        .ThenByDescending(x => x.Mvp)
                        .ThenByDescending(x => x.Score)
                        .Select(x => $"{x.Name}    {x.Team}    {x.Score}    {x.Goals}    {x.Assists}    {x.Saves}    {x.Shots}    {x.Mvp}    {x.Id}    {x.Platform}")
                })
                .Select(x =>
                {
                    var output = new List<string>
                    {
                            "Name    Team    score    goals    assists    saves    shots    mvp    Id    Platform"
                    };
                    output.AddRange(x.Stats);

                    return new
                    {
                        x.ReplayId,
                        Stats = output
                    };
                })
                .ToArray();

            foreach (var item in stats)
                await File.WriteAllLinesAsync($"{rootDirectory.FullName}/{item.ReplayId}.txt", item.Stats);
        }
    }
}
