using System.Collections.Generic;

namespace Meyer.BallChasing.Client
{
    public partial class ProecessedReplay
    {
        public class Team
        {
            public string Name { get; set; }
            
            public List<Player> Players { get; set; }
        }
    }
}