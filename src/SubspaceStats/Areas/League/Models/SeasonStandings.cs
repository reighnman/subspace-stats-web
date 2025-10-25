using System.Text.Json.Serialization;

namespace SubspaceStats.Areas.League.Models
{
    public class SeasonStandings
    {
        [JsonPropertyName("league_id")]
        public required long LeagueId { get; init; }
        [JsonPropertyName("league_name")]
        public required string LeagueName { get; init; }
        [JsonPropertyName("season_id")]
        public required long SeasonId { get; init; }
        [JsonPropertyName("season_name")]
        public required string SeasonName { get; init; }
        [JsonPropertyName("standings")]
        public required List<TeamStanding> Standings { get; init; }
    }

    public class TeamStanding
    {
        [JsonPropertyName("team_id")]
        public required long TeamId { get; init; }
        [JsonPropertyName("team_name")]
        public required string TeamName { get; init; }
        [JsonPropertyName("wins")]
        public required int Wins { get; init; }
        [JsonPropertyName("losses")]
        public required int Losses { get; init; }
        [JsonPropertyName("draws")]
        public required int Draws { get; init; }
    }

    [JsonSerializable(typeof(List<SeasonStandings>), GenerationMode = JsonSourceGenerationMode.Metadata)]
    internal partial class SeasonStandingsSourceGenerationContext : JsonSerializerContext
    {
    }
}
