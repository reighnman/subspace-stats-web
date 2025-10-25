using SubspaceStats.Models;

namespace SubspaceStats.Areas.League.Models.League
{
    public class LeagueDetailsViewModel : ILeagueViewModel
    {
        public required LeagueModel League { get; init; }
        public LeagueSection Section => LeagueSection.Details;
        public required OrderedDictionary<long, GameType> GameTypes { get; init; }
        public required List<SeasonListItem> Seasons { get; init; }
    }
}
