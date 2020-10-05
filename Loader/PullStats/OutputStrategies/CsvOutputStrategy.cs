using Meyer.BallChasing.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Meyer.BallChasing.PullStats.OutputStrategies
{
    public class CsvOutputStrategy : IOutputStrategy
    {
        private readonly DirectoryInfo rootDirectory;

        public CsvOutputStrategy(DirectoryInfo rootDirectory)
        {
            this.rootDirectory = rootDirectory;
        }

        public async Task OutputGameSummary(Group group)
        {
            foreach (var child in group.Children)
                await OutputGameSummary(child);

            var output = new List<string>
            {
                "Name    Team    Mvp    Score    Goals    Assists    Saves    Shots    Cycles    Saviors    Inflicted    Taken    Id    Platform"
            };

            output.AddRange(group
                .Replays
                .SelectMany(x => ReplayPlayerSummary.GetSummary(x))
                .Select(x => $"{x.Name}{Constants.Delimiter}{x.TeamName}{Constants.Delimiter}{x.Mvp}{Constants.Delimiter}{x.Score}{Constants.Delimiter}{x.Goals}{Constants.Delimiter}{x.Assists}{Constants.Delimiter}{x.Saves}{Constants.Delimiter}{x.Shots}{Constants.Delimiter}{x.Cycles}{Constants.Delimiter}{x.Saviors}{Constants.Delimiter}{x.Inflicted}{Constants.Delimiter}{x.Taken}{Constants.Delimiter}{x.Id}{Constants.Delimiter}{x.Platform}")
           );

            if (output.Count > 1)
                await File.WriteAllLinesAsync($"{group.Replays.First().LocalFile.Directory.FullName}/{group.Replays.First().LocalFile.Directory.Name}.csv", output);
        }

        public async Task OutputGroupSummary(Group group)
        {
            foreach (var child in group.Children)
                await OutputGroupSummary(child);

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

        public async Task OutputSummaryAcrossGroups(Group group)
        {
            foreach (var childDepth4 in group.Children.SelectMany(x => x.Children))
            {
                var output = new List<string>
                {
                    "Name    Team    Mvp    Score    Goals    Assists    Saves    Shots    Cycles    Saviors    Inflicted    Taken"
                };

                output.AddRange(GroupPlayerSummary.GetChildrenSummary(childDepth4)
                    .Select(x => $"{x.Name}{Constants.Delimiter}{x.TeamName}{Constants.Delimiter}{x.Mvp}{Constants.Delimiter}{x.Score}{Constants.Delimiter}{x.Goals}{Constants.Delimiter}{x.Assists}{Constants.Delimiter}{x.Saves}{Constants.Delimiter}{x.Shots}{Constants.Delimiter}{x.Cycles}{Constants.Delimiter}{x.Saviors}{Constants.Delimiter}{x.Inflicted}{Constants.Delimiter}{x.Taken}")
                );

                if (output.Count > 1)
                {
                    Group root = group;
                    while (root.Parent != null)
                        root = root.Parent;

                    await File.WriteAllLinesAsync($"{rootDirectory.FullName}/{childDepth4.Parent.Name}/{childDepth4.Name}.csv", output);
                }
            }
        }
    }
}