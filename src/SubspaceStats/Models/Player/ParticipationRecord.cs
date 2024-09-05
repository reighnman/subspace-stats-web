namespace SubspaceStats.Models.Player
{
    public class ParticipationRecord
    {
        public required StatPeriod LastStatPeriod { get; init; }
        public required int? Rating { get; init; }
    }
}
