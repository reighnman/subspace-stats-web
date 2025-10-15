using SubspaceStats.Areas.League.Models.Season.Round;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class RoundsViewModel : ISeasonViewModel
    {
        public required SeasonDetails SeasonDetails { get; init; }
        public SeasonPage Page => SeasonPage.Rounds;
        public required LeagueNavItem LeagueNav { get; init; }
        public required SeasonNavItem SeasonNav { get; init; }
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required OrderedDictionary<int, SeasonRound> Rounds { get; init; }
    }
}
