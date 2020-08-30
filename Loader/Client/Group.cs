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
            this.Children = new List<Group>(0);
            this.Replays = new List<Replay>(0);
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

        public Group FindSubGroup(Group group)
        {
            if (group == null)
                return null;

            if (group.Equals(this))
            {
                group.BallChasingId = this.BallChasingId;
                return this;
            }
            
            foreach (var child in this.Children)
            {
                var found = child.FindSubGroup(group);
                if (found != null)
                {
                    group.BallChasingId = found.BallChasingId;
                    return found;
                }
            }

            return null;
        }

        public bool ContainsReplay(Replay replay)
        {
            if (replay == null)
                return false;

            if (this.Replays.Contains(replay))
            {
                replay = this.Replays.SingleOrDefault(x => x.Equals(replay));
                
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Group compare = obj as Group;

            return compare != null && compare.Name == this.Name;
        }
    }
}