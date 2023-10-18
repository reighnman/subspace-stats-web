using System.Text.Json.Serialization;

namespace SubspaceStats.Models.GameDetails.TeamVersus.Events
{
    public class ShipChangeEvent : GameEvent
    {
        [JsonPropertyName("player")]
        public required string PlayerName { get; init; }

        [JsonPropertyName("ship")]
        public required ShipType Ship { get; init; }
    }
}
