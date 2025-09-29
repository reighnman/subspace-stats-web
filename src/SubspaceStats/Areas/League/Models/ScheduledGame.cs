namespace SubspaceStats.Areas.League.Models
{
    public class ScheduledGame
    {
        public required long LeagueId { get; init; }
        public required string LeagueName { get; init; }
        public required long SeasonId { get; init; }
        public required string SeasonName { get; init; }
        public required long SeasonGameId { get; init; }
        public required int? RoundNumber { get; init; }
        public required string? RoundName { get; init; }
        public required DateTime? ScheduledTimestamp { get; init; }
        public required string Teams { get; init; }
        public required GameStatus Status { get; init; }
    }
}
