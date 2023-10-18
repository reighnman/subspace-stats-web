using System.Text.Json.Serialization;

namespace SubspaceStats.Models.GameDetails.TeamVersus.Events
{
    public class AssignSlotEvent : GameEvent
    {
        [JsonPropertyName("freq")]
        public required short Freq { get; init; }

        [JsonPropertyName("slot_idx")]
        public required short SlotIdx { get; init; }

        [JsonPropertyName("player")]
        public required string PlayerName { get; init; }
    }
}
