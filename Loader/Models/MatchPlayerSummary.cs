namespace Meyer.BallChasing.Models
{
    public class MatchPlayerSummary
    {
        public string Name { get; internal set; }
        public string TeamName { get; internal set; }
        public int Score { get; internal set; }
        public int Mvp { get; internal set; }
        public int Goals { get; internal set; }
        public int Assists { get; internal set; }
        public int Saves { get; internal set; }
        public int Shots { get; internal set; }
        public int Inflicted { get; internal set; }
        public int Taken { get; internal set; }
        public string Id { get; internal set; }
        public string Platform { get; internal set; }
        public int Cycles { get; internal set; }
        public int Saviors { get; internal set; }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            GamePlayerSummary compare = obj as GamePlayerSummary;

            return compare != null
                && (compare.Id == this.Id || compare.Name == this.Name);
        }
    }
}