namespace SubspaceStats.Models.Leaderboard
{
    public class TeamVersusLeaderboardViewModel
    {
        public required GameType GameType { get; init; }
        public required List<StatPeriod> Periods { get; init; }
        public required StatPeriod? SelectedPeriod { get; init; }
		public required StatPeriod? PriorPeriod { get; init; }
		public required List<TopRatingRecord>? TopRatingList { get; init; }
		public required List<TopAvgRatingRecord>? TopAvgRatingList { get; init; }
        public required List<TopKillsPerMinuteRecord>? TopKillsPerMinuteList { get; init; }
        public required List<TopRatingRecord>? TopRatingLastMonth { get; init; }
		public required List<TeamVersusLeaderboardStats>? Stats { get; init; }
        public required PagingInfo StatsPaging { get; init; }
    }
}
