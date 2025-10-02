namespace SubspaceStats.Areas.League.Models.Season
{
    public class SeasonRoundListViewModel
    {
        public required long SeasonId { get; set; }
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<SeasonRound> Rounds { get; init; }
    }
}
