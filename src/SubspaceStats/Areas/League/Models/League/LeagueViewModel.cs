namespace SubspaceStats.Areas.League.Models.League
{
    public class LeagueViewModel
    {
        public LeagueModel? League { get; set; }
        public required OrderedDictionary<long, string> GameTypes { get; init; }
    }
}
