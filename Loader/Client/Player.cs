namespace Meyer.BallChasing.Client
{
    public partial class ProecessedReplay
    {
        public class Player
        {
            public PlayerId Id { get; set; }

            public string Name { get; set; }
            
            public Stats Stats { get; set; }
        }
    }
}