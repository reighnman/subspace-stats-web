namespace SubspaceStats.Areas.League.Models.Team
{
    public class TeamViewModel
    {
        public required TeamWithSeasonInfo TeamInfo { get; init; }
        public required List<TeamGameRecord> GameRecords { get; init; }
    }
}
