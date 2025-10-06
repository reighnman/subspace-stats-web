using System.Text.Json.Serialization;

namespace SubspaceStats.Areas.League.Models
{
    public class GameRecord
    {
        [JsonPropertyName("season_game_id")]
        public required long SeasonGameId { get; set; }

        [JsonPropertyName("round_number")]
        public required int? RoundNumber { get; set; }

        [JsonPropertyName("round_name")]
        public required string RoundName { get; set; }

        [JsonPropertyName("game_timestamp")]
        public required DateTime? GameTimestamp { get; set; }

        [JsonPropertyName("teams")]
        public required List<TeamRecord> Teams { get; set; }

        [JsonPropertyName("game_id")]
        public required long? GameId { get; set; }
    }

    public class TeamRecord
    {
        [JsonPropertyName("team_id")]
        public required long TeamId { get; set; }

        [JsonPropertyName("team_name")]
        public required string TeamName { get; set; }

        [JsonPropertyName("freq")]
        public required short Freq { get; set; }

        [JsonPropertyName("is_winner")]
        public required bool IsWinner { get; set; }

        [JsonPropertyName("score")]
        public required int Score { get; set; }
    }

    [JsonSerializable(typeof(List<GameRecord>), GenerationMode = JsonSourceGenerationMode.Metadata)]
    internal partial class GameRecordSourceGenerationContext : JsonSerializerContext
    {
    }
}
