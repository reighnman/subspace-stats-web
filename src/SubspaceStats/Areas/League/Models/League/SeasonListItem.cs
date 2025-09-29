using System.Text.Json.Serialization;

namespace SubspaceStats.Areas.League.Models.League
{
    public class SeasonListItem
    {
        public required long Id { get; init; }
        public required string Name { get; init; }
        public required DateTime CreatedTimestamp { get; init; }
    }
}
