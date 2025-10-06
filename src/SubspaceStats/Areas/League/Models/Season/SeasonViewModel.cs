using SubspaceStats.Areas.League.Models.League;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class SeasonViewModel
    {
        public required SeasonModel Model { get; set; }

        public required LeagueModel League { get; set; }
    }
}
