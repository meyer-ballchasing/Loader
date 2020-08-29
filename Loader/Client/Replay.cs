using System.IO;

namespace Meyer.BallChasing.Client
{
    public class Replay
    {
        public string Id { get; set; }

        public Group Group { get; set; }

        public ProecessedReplay ProcessedReplay { get; set; }

        public FileInfo LocalFile { get; set; }

        public bool Public { get; set; } = true;
    }
}