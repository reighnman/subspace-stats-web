using System.ComponentModel.DataAnnotations;

namespace SubspaceStats.Areas.League.Authorization
{
    /// <summary>
    /// Roles for a league that can be assigned to website users or players.
    /// </summary>
    public enum LeagueRole
    {
        Manager = 1,

        [Display(Name = "Practice Permit")]
        PracticePermit = 2,

        [Display(Name = "Permit Manager")]
        PermitManager = 3,
    }
}
