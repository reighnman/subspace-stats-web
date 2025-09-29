namespace SubspaceStats.Areas.League.Models.League
{
    public class LeagueListViewModel
    {
        public required List<LeagueModel> Leagues { get; init; }
        public required OrderedDictionary<long, string> GameTypes;
    }
}
