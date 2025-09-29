namespace SubspaceStats.Models.Player
{
    public class OverviewViewModel
    {
        public required string PlayerName { get; init; }
        public required PlayerInfo PlayerInfo { get; init; }

        public required List<ParticipationRecord> ParticipationRecordList { get; init; }
        public required OrderedDictionary<long, GameType> GameTypes { get; init; }
    }
}
