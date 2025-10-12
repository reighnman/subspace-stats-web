using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Options
{
    public class GeneralOptions
    {
        public const string GeneralSectionKey = "General";

        [Required]
        public required string ApplicationName { get; set; }

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
