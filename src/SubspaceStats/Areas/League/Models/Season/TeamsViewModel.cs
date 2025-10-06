using SubspaceStats.Areas.League.Models.Franchise;
using SubspaceStats.Areas.League.Models.Team;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class TeamsViewModel : ISeasonViewModel
    {
        public SeasonPage Page => SeasonPage.Teams;
        public required LeagueNavItem League { get; init; }
        public required SeasonNavItem Season { get; init; }
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<TeamModel> Teams { get; set; }
        public required OrderedDictionary<long, FranchiseModel> Franchises { get; set; }
    }
}
