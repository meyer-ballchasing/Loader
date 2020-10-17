using System;
using System.Collections.Generic;
using System.Linq;

namespace Meyer.BallChasing.Models
{
    public class ReplayPlayerSummary : EqualityComparer<GroupPlayerSummary>
    {
        public string Name { get; private set; }
        
        public string TeamName { get; private set; }
        
        public int Score { get; private set; }
        
        public bool Mvp { get; private set; }
        
        public int Goals { get; private set; }
        
        public int Assists { get; private set; }
        
        public int Saves { get; private set; }
        
        public int Shots { get; private set; }
        
        public int Inflicted { get; private set; }
        
        public int Taken { get; private set; }
        
        public string Id { get; private set; }
        
        public string Platform { get; private set; }
        
        public int Saviors => this.Saves / 3;

        public long Duration { get; private set; }

        public bool Overtime { get; private set; }

        public bool IsWin { get; private set; }

        public int Cycles
        {
            get
            {
                var min = Math.Min(Math.Min(this.Goals, this.Shots), Math.Min(this.Saves, this.Assists));

                return min > 0 && this.Goals >= min && this.Assists >= min && this.Saves >= min && this.Shots >= min 
                    ? min 
                    : 0;
            }
        }

        public Replay Replay { get; private set; }

        public static IEnumerable<ReplayPlayerSummary> GetSummary(Replay replay)
        {
            return replay.ProcessedReplay.Orange.Players.Select(x => new
            {
                x.Name,
                TeamName = replay.ProcessedReplay.Orange.Name ?? "Orange",
                x.Stats.Core.Mvp,
                x.Stats.Core.Score,
                x.Stats.Core.Goals,
                x.Stats.Core.Assists,
                x.Stats.Core.Saves,
                x.Stats.Core.Shots,
                x.Stats.Demo.Inflicted,
                x.Stats.Demo.Taken,
                x.Id.Platform,
                x.Id.Id,
                replay.ProcessedReplay.Overtime,
                replay.ProcessedReplay.Duration,
                IsWin = replay.ProcessedReplay.Orange.Players.Sum(y => y.Stats.Core.Goals) > replay.ProcessedReplay.Blue.Players.Sum(y => y.Stats.Core.Goals)
            })
            .Union(replay.ProcessedReplay.Blue.Players.Select(x => new
            {
                x.Name,
                TeamName = replay.ProcessedReplay.Blue.Name ?? "Blue",
                x.Stats.Core.Mvp,
                x.Stats.Core.Score,
                x.Stats.Core.Goals,
                x.Stats.Core.Assists,
                x.Stats.Core.Saves,
                x.Stats.Core.Shots,
                x.Stats.Demo.Inflicted,
                x.Stats.Demo.Taken,
                x.Id.Platform,
                x.Id.Id,
                replay.ProcessedReplay.Overtime,
                replay.ProcessedReplay.Duration,
                IsWin = replay.ProcessedReplay.Blue.Players.Sum(y => y.Stats.Core.Goals) > replay.ProcessedReplay.Orange.Players.Sum(y => y.Stats.Core.Goals)
            }))
            .OrderByDescending(x => x.TeamName)
            .ThenByDescending(x => x.Mvp)
            .ThenByDescending(x => x.Score)
            .Select(x => new ReplayPlayerSummary
            {
                Name = x.Name,
                TeamName = x.TeamName,
                Score = x.Score,
                Mvp = x.Mvp,
                Goals = x.Goals,
                Assists = x.Assists,
                Saves = x.Saves,
                Shots = x.Shots,
                Inflicted = x.Inflicted,
                Taken = x.Taken,
                Id = x.Id,
                Platform = x.Platform,
                Overtime = x.Overtime,
                Duration = x.Duration,
                IsWin = x.IsWin,
                Replay = replay
            });
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ReplayPlayerSummary compare = obj as ReplayPlayerSummary;

            return compare != null
                && (compare.Id == this.Id || compare.Name == this.Name);
        }

        public override bool Equals(GroupPlayerSummary x, GroupPlayerSummary y)
        {
            return x.Equals(y);
        }

        public override int GetHashCode(GroupPlayerSummary obj)
        {
            return obj.GetHashCode();
        }
    }
}