using SubspaceStats.Models.GameDetails.TeamVersus.Events;
using System.Text.Json.Serialization;

namespace SubspaceStats.Models.GameDetails
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "event_type_id")]
    [JsonDerivedType(typeof(AssignSlotEvent), 1)]
    [JsonDerivedType(typeof(PlayerKillEvent), 2)]
    [JsonDerivedType(typeof(ShipChangeEvent), 3)]
    [JsonDerivedType(typeof(UseItemEvent), 4)]
    public class GameEvent
    {
        [JsonPropertyName("timestamp")]
        public required DateTime Timestamp { get; init; }

        [JsonPropertyName("damage_stats")]
        public Dictionary<string, int>? DamageStats { get; init; }

        [JsonPropertyName("rating_changes")]
        public Dictionary<string, float>? RatingChanges { get; init; }
    }
}
