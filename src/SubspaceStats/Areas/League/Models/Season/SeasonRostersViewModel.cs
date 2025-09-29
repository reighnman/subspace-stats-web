namespace SubspaceStats.Areas.League.Models.Season
{
    public class SeasonRostersViewModel
    {
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<SeasonRoster> Rosters { get; init; }
    }
}
