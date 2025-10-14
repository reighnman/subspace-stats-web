using SubspaceStats.Areas.League.Models.Season;
using SubspaceStats.Areas.League.Models.Season.Team;

namespace SubspaceStats.Areas.League.Models.Season.Player
{

    public class SeasonPlayerViewModel
    {
        public required SeasonDetails Season { get; set; }
        
        public required SeasonPlayer Model { get; set; }

        public required OrderedDictionary<long, TeamModel> Teams { get; set; }
    }
}
