using SubspaceStats.Areas.League.Models.Franchise;
using SubspaceStats.Areas.League.Models.Team;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class TeamsViewModel : ISeasonViewModel
    {
        public required SeasonDetails SeasonDetails { get; init; }
        public SeasonPage Page => SeasonPage.Teams;
        public required LeagueNavItem League { get; init; }
        public required SeasonNavItem Season { get; init; }
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required OrderedDictionary<long, TeamModel> Teams { get; init; }
        public required OrderedDictionary<long, FranchiseModel> Franchises { get; init; }
    }
}
