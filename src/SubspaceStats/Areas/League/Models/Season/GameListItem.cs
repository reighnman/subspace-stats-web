namespace SubspaceStats.Areas.League.Models.Season
{
    public class GameListItem
    {
        public long SeasonGameId { get; set; }
        public int? RoundNumber { get; set; }
        public DateTime? ScheduledTimestamp { get; set; }
        public long? GameId { get; set; }
        public long GameStatusId { get; set; }
        public required long[] TeamIds { get; set; }
    }
}
