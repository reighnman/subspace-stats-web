namespace SubspaceStats.Models.Player
{
    public class TeamVersusShipStats
    {
        public ShipType ShipType { get; set; }
        public int GameUseCount { get; set; }
        public TimeSpan UseDuration { get; set; }
        public long Kills { get; set; }
        public long Deaths { get; set; }
        public long Knockouts { get; set; }
        public long SoloKills { get; set; }
    }
}
