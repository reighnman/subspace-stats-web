namespace SubspaceStats.Areas.League.Models.Season
{
    public class PlayerModel
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public DateTime? SignupTimestamp { get; set; }
        public long? TeamId { get; set; }
        public DateTime? EnrollTimestamp { get; set; }
        public bool IsCaptain { get; set; }
        public bool IsSuspended { get; set; }
    }
}
