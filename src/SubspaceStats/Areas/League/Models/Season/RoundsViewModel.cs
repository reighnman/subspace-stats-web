namespace SubspaceStats.Areas.League.Models.Season
{
    public class RoundsViewModel : ISeasonViewModel
    {
        public SeasonPage Page => SeasonPage.Rounds;
        public required LeagueNavItem League { get; set; }
        public required SeasonNavItem Season { get; init; }
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<SeasonRound> Rounds { get; init; }
    }
}
