using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Models
{
    public enum GameStatus
    {
        [Display(Name = "Pending")]
        Pending = 1,

        [Display(Name = "In Progress")]
        InProgress = 2,

        [Display(Name = "Complete")]
        Complete = 3,
    }
}