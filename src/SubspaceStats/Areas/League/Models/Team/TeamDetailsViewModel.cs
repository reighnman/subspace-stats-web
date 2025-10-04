namespace SubspaceStats.Areas.League.Models.Team
{
    public class TeamDetailsViewModel
    {
        public required TeamWithSeasonInfo TeamInfo { get; init; }
        public required List<TeamGameRecord> GameRecords { get; init; }
    }
}
