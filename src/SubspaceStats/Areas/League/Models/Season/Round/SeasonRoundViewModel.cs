using SubspaceStats.Areas.League.Models.League;

namespace SubspaceStats.Areas.League.Models.Season.Round
{
    public class SeasonRoundViewModel
    {
        public required SeasonRound Round { get; init; }

        public required SeasonModel Season { get; init; }

        public required LeagueModel League { get; init; }
    }
}
