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
                $"Name{Constants.Delimiter}Team{Constants.Delimiter}Win{Constants.Delimiter}Mvp{Constants.Delimiter}Score{Constants.Delimiter}Goals{Constants.Delimiter}Assists{Constants.Delimiter}Saves{Constants.Delimiter}Shots{Constants.Delimiter}Cycles{Constants.Delimiter}Saviors{Constants.Delimiter}Inflicted{Constants.Delimiter}Taken{Constants.Delimiter}Duration{Constants.Delimiter}Ovetime{Constants.Delimiter}Id{Constants.Delimiter}Platform"
            };

            output.AddRange(group
                .Replays
                .SelectMany(x => ReplayPlayerSummary.GetSummary(x))
                .Select(x => $"{x.Name}{Constants.Delimiter}{x.TeamName}{Constants.Delimiter}{x.IsWin}{Constants.Delimiter}{x.Mvp}{Constants.Delimiter}{x.Score}{Constants.Delimiter}{x.Goals}{Constants.Delimiter}{x.Assists}{Constants.Delimiter}{x.Saves}{Constants.Delimiter}{x.Shots}{Constants.Delimiter}{x.Cycles}{Constants.Delimiter}{x.Saviors}{Constants.Delimiter}{x.Inflicted}{Constants.Delimiter}{x.Taken}{Constants.Delimiter}{x.Duration}{Constants.Delimiter}{x.Overtime}{Constants.Delimiter}{x.Id}{Constants.Delimiter}{x.Platform}")
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
                $"Name{Constants.Delimiter}Team{Constants.Delimiter}GamesPlayed{Constants.Delimiter}GamesWon{Constants.Delimiter}Mvp{Constants.Delimiter}Score{Constants.Delimiter}Goals{Constants.Delimiter}Assists{Constants.Delimiter}Saves{Constants.Delimiter}Shots{Constants.Delimiter}Cycles{Constants.Delimiter}Saviors{Constants.Delimiter}Inflicted{Constants.Delimiter}Taken{Constants.Delimiter}Duration{Constants.Delimiter}Ovetimes"
            };

            output.AddRange(GroupPlayerSummary.GetSummary(group)
                .Select(x => $"{x.Name}{Constants.Delimiter}{x.TeamName}{Constants.Delimiter}{x.GamesPlayed}{Constants.Delimiter}{x.GamesWon}{Constants.Delimiter}{x.Mvp}{Constants.Delimiter}{x.Score}{Constants.Delimiter}{x.Goals}{Constants.Delimiter}{x.Assists}{Constants.Delimiter}{x.Saves}{Constants.Delimiter}{x.Shots}{Constants.Delimiter}{x.Cycles}{Constants.Delimiter}{x.Saviors}{Constants.Delimiter}{x.Inflicted}{Constants.Delimiter}{x.Taken}{Constants.Delimiter}{x.Duration}{Constants.Delimiter}{x.Overtimes}")
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
                    $"Name{Constants.Delimiter}GamesPlayed{Constants.Delimiter}GamesWon{Constants.Delimiter}Mvp{Constants.Delimiter}Score{Constants.Delimiter}Goals{Constants.Delimiter}Assists{Constants.Delimiter}Saves{Constants.Delimiter}Shots{Constants.Delimiter}Cycles{Constants.Delimiter}Saviors{Constants.Delimiter}Inflicted{Constants.Delimiter}Taken{Constants.Delimiter}Duration{Constants.Delimiter}Ovetimes"
                };

                output.AddRange(GroupPlayerSummary.GetChildrenSummary(childDepth4)
                    .Select(x => $"{x.Name}{Constants.Delimiter}{x.GamesPlayed}{Constants.Delimiter}{x.GamesWon}{Constants.Delimiter}{x.Mvp}{Constants.Delimiter}{x.Score}{Constants.Delimiter}{x.Goals}{Constants.Delimiter}{x.Assists}{Constants.Delimiter}{x.Saves}{Constants.Delimiter}{x.Shots}{Constants.Delimiter}{x.Cycles}{Constants.Delimiter}{x.Saviors}{Constants.Delimiter}{x.Inflicted}{Constants.Delimiter}{x.Taken}{Constants.Delimiter}{x.Duration}{Constants.Delimiter}{x.Overtimes}")
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