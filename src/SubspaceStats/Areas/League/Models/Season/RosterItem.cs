using System.Text.Json.Serialization;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class RosterItem
    {
        [JsonPropertyName("player_id")]
        public required long PlayerId { get; init; }

        [JsonPropertyName("player_name")]
        public required string PlayerName { get; init; }

        [JsonPropertyName("is_captain")]
        public required bool IsCaptain { get; init; }

        [JsonPropertyName("is_suspended")]
        public required bool IsSuspended { get; set; }

        [JsonPropertyName("enroll_timestamp")]
        public DateTime? EnrollTimestamp { get; init; }
    }
}
