using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Meyer.BallChasing.Models
{
    public class Replay
    {
        public string BallChasingId { get; set; }

        public Group Group { get; set; }

        [JsonIgnore]
        public ProcessedReplay ProcessedReplay { get; set; }

        [JsonIgnore]
        public FileInfo LocalFile { get; set; }

        public string LocalFilePath
        {
            get => this.LocalFile?.FullName;
            set => this.LocalFile = new FileInfo(value);
        }

        public bool Public { get; set; } = true;

        public IEnumerable<GamePlayerSummary> GetSummary()
        {
            return this.ProcessedReplay.Orange.Players.Select(x => new
            {
                x.Name,
                TeamName = this.ProcessedReplay.Orange.Name ?? "Orange",
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
            .Union(this.ProcessedReplay.Blue.Players.Select(x => new
            {
                x.Name,
                TeamName = this.ProcessedReplay.Blue.Name ?? "Blue",
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
            .Select(x => new GamePlayerSummary
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
            return this.LocalFilePath.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Replay compare = obj as Replay;

            return compare != null
                && ((compare.BallChasingId != null && this.BallChasingId != null && compare.BallChasingId == this.BallChasingId)
                    || compare.LocalFilePath == this.LocalFilePath);
        }
    }
}