using System.Collections.Generic;

namespace Meyer.BallChasing.Client
{
    public class Group
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string PlayerIdentification { get; set; } = "by-id";

        public string TeamIdentification { get; set; } = "by-distinct-players";

        public Group Parent { get; set; }

        public IEnumerable<Group> Children { get; set; }

        public IEnumerable<Replay> Replays { get; set; }
    }
}