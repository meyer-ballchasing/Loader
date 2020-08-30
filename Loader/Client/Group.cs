using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Meyer.BallChasing.Client
{
    public class Group
    {
        public string BallChasingId { get; set; }

        public string Name { get; set; }

        public string PlayerIdentification { get; set; } = "by-id";

        public string TeamIdentification { get; set; } = "by-distinct-players";

        public Group Parent { get; set; }

        public List<Group> Children { get; set; }

        public List<Replay> Replays { get; set; }

        public Group()
        {
        }

        public Group(DirectoryInfo directory, Group parent)
        {
            this.Name = directory.Name;

            this.Children = directory
                .EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                .Select(x => new Group(x, this))
                .ToList();

            this.Replays = directory
                .EnumerateFiles("*.replay", SearchOption.TopDirectoryOnly)
                .Select(x => new Replay
                {
                    Group = this,
                    LocalFile = x,
                    Public = true
                })
                .ToList();

            if (parent != null)
                this.Parent = parent;
        }
    }
}