namespace SubspaceStats.Areas.League.Models.Season
{
    public class SeasonStandingsViewModel
    {
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<TeamStanding> Standings { get; init; }
    }
}
