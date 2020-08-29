using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Meyer.BallChasing.Client
{
    public class Group
    {
        public string BallChasingId { get; set; }

        public string Name { get; }

        public string PlayerIdentification { get; } = "by-id";

        public string TeamIdentification { get; } = "by-distinct-players";

        public Group Parent { get; }

        public IEnumerable<Group> Children { get; }

        public IEnumerable<Replay> Replays { get; }

        public Group()
        {
            this.Children = new Group[0];
            this.Replays = new Replay[0];
        }

        public Group(DirectoryInfo directory, Group parent)
        {
            this.Name = directory.Name;

            this.Children = directory
                .EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                .Select(x => new Group(x, this))
                .ToArray();

            this.Replays = directory
                .EnumerateFiles("*.replay", SearchOption.TopDirectoryOnly)
                .Select(x => new Replay
                {
                    Group = this,
                    LocalFile = x,
                    Public = true
                })
                .ToArray();

            if (parent != null)
                this.Parent = parent;
        }
    }
}