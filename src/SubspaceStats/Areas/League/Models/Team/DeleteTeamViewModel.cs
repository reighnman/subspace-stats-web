using SubspaceStats.Areas.League.Models.Franchise;
using SubspaceStats.Areas.League.Models.Season;

namespace SubspaceStats.Areas.League.Models.Team
{
    public class DeleteTeamViewModel
    {
        public required TeamModel Model { get; set; }
        public required SeasonDetails Season { get; set; }
        public required FranchiseModel? Franchise { get; set; }
    }
}
