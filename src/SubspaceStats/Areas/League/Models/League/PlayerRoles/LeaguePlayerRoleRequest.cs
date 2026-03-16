using SubspaceStats.Areas.League.Authorization;

namespace SubspaceStats.Areas.League.Models.League.PlayerRoles
{
    public record class LeaguePlayerRoleRequest(string PlayerName, LeagueRole Role, DateTime RequestTimestamp);
}
