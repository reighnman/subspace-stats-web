using SubspaceStats.Areas.League.Models.Team;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class PlayersViewModel : ISeasonViewModel
    {
        public SeasonPage Page => SeasonPage.Players;
        public required LeagueNavItem League { get; init; }
        public required SeasonNavItem Season { get; init; }
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<PlayerListItem> Players { get; set; }
        public required List<TeamModel> Teams { get; set; }
    }
}
