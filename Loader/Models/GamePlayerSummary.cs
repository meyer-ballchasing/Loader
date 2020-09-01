using System;

namespace Meyer.BallChasing.Models
{
    public class GamePlayerSummary
    {
        public string Name { get; internal set; }
        public string TeamName { get; internal set; }
        public int Score { get; internal set; }
        public bool Mvp { get; internal set; }
        public int Goals { get; internal set; }
        public int Assists { get; internal set; }
        public int Saves { get; internal set; }
        public int Shots { get; internal set; }
        public int Inflicted { get; internal set; }
        public int Taken { get; internal set; }
        public string Id { get; internal set; }
        public string Platform { get; internal set; }
        public int Saviors => this.Saves / 3;
        public int Cycles
        {
            get
            {
                var min = Math.Min(Math.Min(this.Goals, this.Shots), Math.Min(this.Saves, this.Assists));

                return min > 0 && this.Goals >= min && this.Assists >= min && this.Saves >= min && this.Shots >= min 
                    ? min 
                    : 0;
            }
        }

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