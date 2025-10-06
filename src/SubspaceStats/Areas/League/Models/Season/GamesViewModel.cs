using SubspaceStats.Areas.League.Models.Team;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class GamesViewModel : ISeasonViewModel
    {
        public SeasonPage Page => SeasonPage.Games;
        public required LeagueNavItem League { get; init; }
        public required SeasonNavItem Season { get; init; }
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<GameListItem> Games { get; set; }
        public required List<TeamModel> Teams { get; set; }
    }
}
