using NpgsqlTypes;

namespace SubspaceStats.Areas.League.Models.Season
{
    public class SeasonDetails
    {
        public long LeagueId { get; set; }
        public required string LeagueName { get; set; }
        public long SeasonId { get; set; }
        public required string SeasonName { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public long? StatPeriodId { get; set; }
        public NpgsqlRange<DateTime>? StatPeriodRange { get; set; }
        public long? StatGameTypeId { get; set; }
        public long? LeagueGameTypeId { get; set; }
    }
}
