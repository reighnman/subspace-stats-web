using SubspaceStats.Areas.League.Models.Season.Team;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class PlayersViewModel : ISeasonViewModel
    {
        public required SeasonDetails SeasonDetails { get; init; }
        public SeasonPage Page => SeasonPage.Players;
        public required LeagueNavItem LeagueNav { get; init; }
        public required SeasonNavItem SeasonNav { get; init; }
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<PlayerListItem> Players { get; init; }
        public required OrderedDictionary<long, TeamModel> Teams { get; init; }
    }
}
