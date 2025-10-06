namespace SubspaceStats.Areas.League.Models.Season
{
    public class RostersViewModel : ISeasonViewModel
    {
        public SeasonPage Page => SeasonPage.Rosters;
        public required LeagueNavItem League { get; set; }
        public required SeasonNavItem Season { get; init; }
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<SeasonRoster> Rosters { get; init; }
    }
}
