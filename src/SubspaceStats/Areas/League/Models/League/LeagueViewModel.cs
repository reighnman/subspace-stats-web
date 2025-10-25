using SubspaceStats.Models;

namespace SubspaceStats.Areas.League.Models.League
{
    public class LeagueViewModel : ILeagueViewModel
    {
        public LeagueModel? League { get; set; }
        public LeagueSection Section => LeagueSection.Edit;
        public required OrderedDictionary<long, GameType> GameTypes { get; init; }
    }
}
