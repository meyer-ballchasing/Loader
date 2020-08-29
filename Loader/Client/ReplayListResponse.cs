namespace Meyer.BallChasing.Client
{
    internal class ReplayListResponse
    {
        public int Count { get; set; }

        public ObjectWithId[] List { get; set; }

        public string Next { get; set; }

        internal class ObjectWithId
        {
            public string Id { get; set; }
        }
    }
}
