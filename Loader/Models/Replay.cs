﻿using Newtonsoft.Json;
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