namespace SubspaceStats.Areas.League.Models.Season
{
    public class RostersViewModel : ISeasonViewModel
    {
        public required SeasonDetails SeasonDetails { get; init; }
        public SeasonPage Page => SeasonPage.Rosters;
        public required LeagueNavItem League { get; init; }
        public required SeasonNavItem Season { get; init; }
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<SeasonRoster> Rosters { get; init; }
    }
}
