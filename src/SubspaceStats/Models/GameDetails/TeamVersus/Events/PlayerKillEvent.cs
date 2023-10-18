using System.Text.Json.Serialization;

namespace SubspaceStats.Models.GameDetails.TeamVersus.Events
{
    public class PlayerKillEvent : GameEvent
    {
        [JsonPropertyName("killed_player")]
        public required string KilledPlayerName { get; init; }

        [JsonPropertyName("killer_player")]
        public required string KillerPlayerName { get; init; }

        [JsonPropertyName("is_knockout")]
        public required bool IsKnockout { get; init; }

        [JsonPropertyName("is_team_kill")]
        public required bool IsTeamKill { get; init; }

        [JsonPropertyName("x_coord")]
        public required short XCoord { get; init; }

        [JsonPropertyName("y_coord")]
        public required short YCoord { get; init; }

        [JsonPropertyName("killed_ship")]
        public required ShipType KilledShip { get; init; }

        [JsonPropertyName("killer_ship")]
        public required ShipType KillerShip { get; init; }

        [JsonPropertyName("score")]
        public required int[] Score { get; init; }

        [JsonPropertyName("remaining_slots")]
        public required int[] RemainingSlots { get; init; }
    }
}
