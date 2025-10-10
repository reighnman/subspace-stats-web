using SubspaceStats.Areas.League.Models.League;
using SubspaceStats.Areas.League.Models.SeasonGame;
using SubspaceStats.Areas.League.Models.Team;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class GamesViewModel : ISeasonViewModel
    {
        public required SeasonDetails SeasonDetails { get; init; }
        public SeasonPage Page => SeasonPage.Games;
        LeagueNavItem ISeasonViewModel.League => LeagueNav;
        SeasonNavItem ISeasonViewModel.Season => SeasonNav;
        public required LeagueNavItem LeagueNav { get; init; }
        public required SeasonNavItem SeasonNav { get; init; }
        public required LeagueModel League { get; init; }
        public required SeasonModel Season { get; init; }
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<GameModel> Games { get; init; }
        public required OrderedDictionary<long, TeamModel> Teams { get; init; }
        public required OrderedDictionary<int, SeasonRound> Rounds { get; init; }

    }
}
