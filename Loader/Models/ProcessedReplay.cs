namespace Meyer.BallChasing.Models
{
    public class ProcessedReplay
    {
        public string Id { get; set; }

        public long Duration { get; set; }

        public bool Overtime { get; set; }

        public Team Blue { get; set; }
        
        public Team Orange { get; set; }
    }
}