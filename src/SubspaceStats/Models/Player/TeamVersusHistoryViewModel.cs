namespace SubspaceStats.Models.Player
{
    public class TeamVersusHistoryViewModel
    {
        public required string PlayerName { get; set; }
        public required PlayerInfo PlayerInfo { get; set; }
        public required List<StatPeriod> PeriodList { get; set; }
        public required StatPeriod SelectedPeriod { get; set; }
        public required List<TeamVersusPeriodStats> PeriodStatsList { get; set; }
        public required List<TeamVersusGameStats> GameStatsList { get; set; }
        public required PagingInfo GameStatsPaging { get; set; }
        public required List<TeamVersusShipStats> ShipStatsList { get; set; }
        public required List<KillStats> KillStatsList { get; set; }

    }
}
