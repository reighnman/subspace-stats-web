using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Options
{
    public class GeneralOptions
    {
        public const string GeneralSectionKey = "General";

        [Required]
        public required string ApplicationName { get; set; }

        // TODO: move timezone settings from LeagueOptions into here
    }
}
