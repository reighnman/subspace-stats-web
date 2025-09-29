using System.Text.Json.Serialization;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class SeasonRoster
    {
        [JsonPropertyName("team_id")]
        public required long TeamId { get; init; }

        [JsonPropertyName("team_name")]
        public required string TeamName { get; init; }

        [JsonPropertyName("banner_small")]
        public required string? BannerSmall { get; init; }

        [JsonPropertyName("banner_large")]
        public required string? BannerLarge { get; init; }

        [JsonPropertyName("roster")]
        public required List<RosterItem> Roster { get; init; }
    }

    [JsonSerializable(typeof(List<SeasonRoster>), GenerationMode = JsonSourceGenerationMode.Metadata)]
    internal partial class SeasonRosterSourceGenerationContext : JsonSerializerContext
    {
    }
}
