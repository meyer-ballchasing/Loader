using Newtonsoft.Json;
using System.IO;

namespace Meyer.BallChasing.Client
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

            return compare != null && compare.LocalFilePath == this.LocalFilePath;
        }
    }
}