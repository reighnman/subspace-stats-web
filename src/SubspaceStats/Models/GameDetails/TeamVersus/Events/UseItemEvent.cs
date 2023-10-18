using System.Text.Json.Serialization;

namespace SubspaceStats.Models.GameDetails.TeamVersus.Events
{
    public class UseItemEvent : GameEvent
    {
        [JsonPropertyName("player")]
        public required string PlayerName { get; init; }

        [JsonPropertyName("ship_item_id")]
        public required ShipItemType Item { get; init; }
    }
}
