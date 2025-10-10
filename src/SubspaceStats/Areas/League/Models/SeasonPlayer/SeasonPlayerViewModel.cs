using SubspaceStats.Areas.League.Models.Team;

namespace SubspaceStats.Areas.League.Models.SeasonPlayer
{

    public class SeasonPlayerViewModel
    {
        public required long SeasonId { get; set; }
        
        public required SeasonPlayer Model { get; set; }

        public required OrderedDictionary<long, TeamModel> Teams { get; set; }
    }
}
