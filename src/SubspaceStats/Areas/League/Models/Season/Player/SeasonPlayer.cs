using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models.Season.Player
{
    public class SeasonPlayer
    {
        [Display(Name = "Player ID")]
        public required long PlayerId { get; set; }

        [Display(Name = "Player Name")]
        public required string PlayerName { get; set; }

        [Display(Name = "Team")]
        public required long? TeamId { get; set; }

        [Display(Name = "Is Captain")]
        public required bool IsCaptain { get; set; }

        [Display(Name = "Is Suspended")]
        public required bool IsSuspended { get; set; }
    }
}
