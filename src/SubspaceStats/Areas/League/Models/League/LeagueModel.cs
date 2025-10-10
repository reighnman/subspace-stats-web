using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.League
{
    public class LeagueModel : IValidatableObject
    {
        [Display(Name = "League ID")]
        public long Id { get; set; }

        [Display(Name = "League Name")]
        [Required]
        [StringLength(128, MinimumLength = 3)]
        public required string Name { get; set; }

        [Display(Name = "Game Type")]
        [Required]
        public long GameTypeId { get; set; }

        [Display(Name = "Minimum Teams Per Game")]
        [Range(2, MaxFreq)]
        public short MinTeamsPerGame { get; set; }

        [Display(Name = "Maximum Teams Per Game")]
        [Range(2, MaxFreq)]
        public short MaxTeamsPerGame { get; set; }

        [Display(Name = "Starting Freq")]
        [Range(0, MaxFreq-1)]
        public short FreqStart { get; set; }

        [Display(Name = "Freq Increment")]
        [Range(1, MaxFreq)]
        public short FreqIncrement { get; set; }

        private const short MaxFreq = 9999;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MaxTeamsPerGame < MinTeamsPerGame)
            {
                yield return new ValidationResult("Must be greater than or equal to the minimum.", [nameof(MaxTeamsPerGame)]);
            }

            if ((FreqStart + (MaxTeamsPerGame - 1) * FreqIncrement) > MaxFreq)
            {
                yield return new ValidationResult("The combined team and freq settings specified must not allow for a freq > 9999.");
            }
        }
    }
}
