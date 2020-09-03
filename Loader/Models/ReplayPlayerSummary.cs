﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Meyer.BallChasing.Models
{
    public class ReplayPlayerSummary
    {
        public string Name { get; internal set; }
        
        public string TeamName { get; internal set; }
        
        public int Score { get; internal set; }
        
        public bool Mvp { get; internal set; }
        
        public int Goals { get; internal set; }
        
        public int Assists { get; internal set; }
        
        public int Saves { get; internal set; }
        
        public int Shots { get; internal set; }
        
        public int Inflicted { get; internal set; }
        
        public int Taken { get; internal set; }
        
        public string Id { get; internal set; }
        
        public string Platform { get; internal set; }
        
        public int Saviors => this.Saves / 3;
        
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
                x.Id.Id
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
                x.Id.Id
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
                Platform = x.Platform
            });
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