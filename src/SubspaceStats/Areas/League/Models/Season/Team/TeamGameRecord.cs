namespace SubspaceStats.Areas.League.Models.Season.Team
{
    public class TeamGameRecord
    {
        public required long SeasonGameId { get; init; }
        public required int? RoundNumber { get; init; }
        public required string? RoundName { get; init; }
        public required DateTime? GameTimestamp { get; init; }
        public required long? GameId { get; init; }
        public required string Teams { get; init; }
        public required GameResult? Result { get; init; }
        public required string? Scores { get; init; }
    }

    public enum GameResult
    {
        Win,
        Loss,
        Draw,
    }
}
