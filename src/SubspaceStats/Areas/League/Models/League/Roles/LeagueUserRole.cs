using SubspaceStats.Areas.League.Authorization;

namespace SubspaceStats.Areas.League.Models.League.Roles
{
    public record class LeagueUserRole(string UserId, LeagueRole Role);
}
