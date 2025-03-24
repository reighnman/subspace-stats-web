using System.Diagnostics;
using System.Text.Json.Serialization;

namespace SubspaceStats.Models.GameDetails.TeamVersus
{
    [DebuggerDisplay("{Freq} {IsPremade}")]
    public class TeamStats
    {
        [JsonPropertyName("freq")]
        public required short Freq { get; init; }

        [JsonPropertyName("is_winner")]
        public required bool IsWinner { get; init; }

        [JsonPropertyName("score")]
        public required int Score { get; init; }

        [JsonPropertyName("members")]
        public required List<MemberStats> Members { get; init; }
    }
}
