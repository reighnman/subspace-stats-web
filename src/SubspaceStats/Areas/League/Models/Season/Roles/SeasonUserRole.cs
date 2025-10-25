using SubspaceStats.Areas.League.Authorization;

namespace SubspaceStats.Areas.League.Models.Season.Roles
{
    public record class SeasonUserRole(string UserId, SeasonRole Role);
}
