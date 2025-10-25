namespace SubspaceStats.Areas.League.Models.Franchise
{
    // For listing on GET /league/franchise/{id}
    public class TeamAndSeason
    {
        public required long TeamId { get; set; }
        public required string TeamName { get; set; }
        public required long SeasonId { get; set; }
        public required string SeasonName { get; set; }
        public required long LeagueId { get; set; }
        public required string LeagueName { get; set; }
    }
}
