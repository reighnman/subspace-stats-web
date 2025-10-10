using SubspaceStats.Models;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class DetailsViewModel : ISeasonViewModel
    {
        public required SeasonDetails SeasonDetails { get; init; }
        public SeasonPage Page => SeasonPage.Details;
        public required LeagueNavItem League { get; init; }
        public required SeasonNavItem Season { get; init; }
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required OrderedDictionary<long, GameType> GameTypes { get; set; }

    }
}
