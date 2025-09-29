using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Options
{
    public class LeagueOptions
    {
        public const string LeagueSectionKey = "League";

        /// <summary>
        /// IDs of the leagues to show on the league home page.
        /// </summary>
        [Required]
        public required long[] LeagueIds { get; set; }


        private string? _timeZoneId;
        [Required]
        public required string TimeZoneId
        {
            get => _timeZoneId ?? TimeZoneInfo.Utc.Id;
            set
            {
                if (value is null)
                {
                    _timeZoneId = TimeZoneInfo.Utc.Id;
                    TimeZone = TimeZoneInfo.Utc;
                }
                else
                {
                    _timeZoneId = value;
                    TimeZone = TimeZoneInfo.FindSystemTimeZoneById(_timeZoneId);
                }
            }
        }

        public TimeZoneInfo TimeZone { get; private set; } = TimeZoneInfo.Utc;


    }
}
