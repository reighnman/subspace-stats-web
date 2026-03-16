namespace SubspaceStats.Areas.League.Models.League.PlayerRoles
{
    public class LeaguePlayerRolesViewModel : ILeagueViewModel
    {
        public required LeagueModel League { get; init; }
        public LeagueSection Section => LeagueSection.PlayerRoles;
        public required LeagueSeasonChooserViewModel LeagueSeasonChooser { get; init; }

        public required List<LeaguePlayerRoleRequest> Requests { get; init; }
        public required List<LeaguePlayerRole> Roles { get; init; }

        public required AddPlayerRoleViewModel AddPlayerRole { get; init; }
    }
}
