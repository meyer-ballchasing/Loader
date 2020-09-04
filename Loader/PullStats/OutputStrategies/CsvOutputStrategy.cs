using Meyer.BallChasing.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Meyer.BallChasing.PullStats.OutputStrategies
{
    public class CsvOutputStrategy : IOutputStrategy
    {
        public async Task Output(Group group)
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
                await File.WriteAllLinesAsync($"{item.LocalFile.Directory.FullName}/{item.LocalFile.Name}.csv", item.Stats);
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
                await File.WriteAllLinesAsync($"{group.Replays.First().LocalFile.Directory.Parent.FullName}/{group.Replays.First().LocalFile.Directory.Name}.csv", output);
        }
    }
}