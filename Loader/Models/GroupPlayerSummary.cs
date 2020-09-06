using System.Collections.Generic;
using System.Linq;

namespace Meyer.BallChasing.Models
{
    public class GroupPlayerSummary
    {
        public string Name { get; private set; }
        
        public string TeamName { get; private set; }
        
        public int Score { get; private set; }
        
        public int Mvp { get; private set; }
        
        public int Goals { get; private set; }
        
        public int Assists { get; private set; }
        
        public int Saves { get; private set; }
        
        public int Shots { get; private set; }
        
        public int Inflicted { get; private set; }
        
        public int Taken { get; private set; }
        
        public string Id { get; private set; }
        
        public string Platform { get; private set; }
        
        public int Cycles { get; private set; }
        
        public int Saviors { get; private set; }

        public Group Group { get; private set; }

        public static IEnumerable<GroupPlayerSummary> GetSummary(Group group)
        {
            return group
                .Replays
                .SelectMany(x => ReplayPlayerSummary.GetSummary(x))
                .GroupBy(x => x)
                .Select(x => new GroupPlayerSummary
                {
                    Name = x.Key.Name,
                    TeamName = x.Key.TeamName,
                    Score = x.Sum(y => y.Score),
                    Goals = x.Sum(y => y.Goals),
                    Assists = x.Sum(y => y.Assists),
                    Saves = x.Sum(y => y.Saves),
                    Shots = x.Sum(y => y.Shots),
                    Inflicted = x.Sum(y => y.Inflicted),
                    Taken = x.Sum(y => y.Taken),
                    Mvp = x.Sum(y => y.Mvp ? 1 : 0),
                    Cycles = x.Sum(y => y.Cycles),
                    Saviors = x.Sum(y => y.Saviors),
                    Group = x.First().Replay.Group
                })
                .OrderByDescending(x => x.TeamName)
                .ThenByDescending(x => x.Mvp)
                .ThenByDescending(x => x.Score)
                .AsEnumerable();
        }

        public static IEnumerable<GroupPlayerSummary> GetChildrenSummary(Group group)
        {
            return group
                .Children
                .SelectMany(x => x.Children).SelectMany(x => x.Replays)
                .SelectMany(x => ReplayPlayerSummary.GetSummary(x))
                .GroupBy(x => x)
                .Select(x => new GroupPlayerSummary
                {
                    Name = x.Key.Name,
                    TeamName = x.Key.TeamName,
                    Score = x.Sum(y => y.Score),
                    Goals = x.Sum(y => y.Goals),
                    Assists = x.Sum(y => y.Assists),
                    Saves = x.Sum(y => y.Saves),
                    Shots = x.Sum(y => y.Shots),
                    Inflicted = x.Sum(y => y.Inflicted),
                    Taken = x.Sum(y => y.Taken),
                    Mvp = x.Sum(y => y.Mvp ? 1 : 0),
                    Cycles = x.Sum(y => y.Cycles),
                    Saviors = x.Sum(y => y.Saviors),
                    Group = x.First().Replay.Group
                })
                .OrderByDescending(x => x.TeamName)
                .ThenByDescending(x => x.Mvp)
                .ThenByDescending(x => x.Score)
                .AsEnumerable();
        }


        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ReplayPlayerSummary compare = obj as ReplayPlayerSummary;

            return compare != null
                && (compare.Id == this.Id || compare.Name == this.Name);
        }
    }
}