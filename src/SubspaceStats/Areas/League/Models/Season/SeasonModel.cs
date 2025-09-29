namespace SubspaceStats.Areas.League.Models.Season
{
    public class SeasonModel
    {
        public required long SeasonId { get; set; }
        public required string SeasonName { get; set; }
        public required long LeagueId { get; set; }
    }
}
