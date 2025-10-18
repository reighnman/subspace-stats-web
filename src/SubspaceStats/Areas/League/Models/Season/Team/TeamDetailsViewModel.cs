namespace SubspaceStats.Areas.League.Models.Season.Team
{
    public class TeamDetailsViewModel
    {
        public required TeamWithSeasonInfo TeamInfo { get; init; }
        public required List<TeamGameRecord> GameRecords { get; init; }
        public required List<RosterItem> Roster { get; init; }
    }
}
