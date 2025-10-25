using SubspaceStats.Areas.League.Models.League;
using SubspaceStats.Areas.League.Models.League.Roles;

namespace SubspaceStats.Areas.League.Models.Season.Roles
{
    public class SeasonRolesViewModel : ISeasonViewModel
    {
        public required SeasonDetails SeasonDetails { get; init; }
        public required LeagueModel League { get; init; }
        public SeasonPage Page => SeasonPage.Roles;
        public required LeagueNavItem LeagueNav { get; init; }
        public required SeasonNavItem SeasonNav { get; init; }
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }
        public required List<SeasonUserRole> Roles { get; init; }
        public AddUserRoleViewModel? AddUserRole { get; init; }
        public string? AddUserRoleMessage { get; init; }
    }
}
