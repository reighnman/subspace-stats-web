namespace SubspaceStats.Areas.League.Models.League
{
    public class LeagueDetailsViewModel
    {
        public required LeagueModel League { get; init; }
        public required OrderedDictionary<long, string> GameTypes { get; init; }
        public required List<SeasonListItem> Seasons { get; init; }
    }
}
