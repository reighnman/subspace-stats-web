namespace SubspaceStats.Areas.League.Models
{
    public class NavViewModel
    {
        public required List<LeagueNavItem> LeagueWithSeasons { get; init; }

        public long? LeagueId { get; init; }
        public long? SeasonId { get; init; }
    }
}
